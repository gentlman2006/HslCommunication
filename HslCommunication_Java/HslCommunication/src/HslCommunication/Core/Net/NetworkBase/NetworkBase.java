package HslCommunication.Core.Net.NetworkBase;

import HslCommunication.Core.IMessage.INetMessage;
import HslCommunication.Core.Types.HslTimeOut;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.LogNet.Core.ILogNet;
import HslCommunication.StringResources;
import HslCommunication.Utilities;

import java.io.*;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.net.SocketAddress;
import java.util.Date;
import java.util.UUID;


/**
 * 本系统所有网络类的基类，该类为抽象类，无法进行实例化
 */
public abstract class NetworkBase {

    /**
     * 实例化一个NetworkBase对象
     */
    public NetworkBase( )
    {
        Token = UUID.fromString("00000000-0000-0000-0000-000000000000");
    }



    /**
     * 通讯类的核心套接字
     */
    protected Socket CoreSocket = null;


    /**
     * 线程检查是否发生了超时的方法
     * @param timeout
     * @param millisecond
     */
    public static void ThreadPoolCheckConnect(HslTimeOut timeout, int millisecond) {
        while (!timeout.IsSuccessful) {
            if ((new Date().getTime() - timeout.StartTime.getTime()) > millisecond) {
                // 连接超时或是验证超时
                if (!timeout.IsSuccessful) {
                    try {
                        if (timeout.WorkSocket != null) {
                            timeout.WorkSocket.close();
                        }
                    } catch (java.io.IOException ex) {
                        // 不处理，放弃
                    }
                }
                break;
            }
        }
    }


    /**
     * 从套接字接收定长度的字节数组
     * @param socket
     * @param length
     * @return
     */
    protected OperateResultExOne<byte[]> Receive(Socket socket, int length )
    {
        OperateResultExOne<byte[]> resultExOne = new OperateResultExOne<>();

        if (length == 0) {
            resultExOne.IsSuccess = true;
            resultExOne.Content = new byte[0];
            return  resultExOne;
        }

        int count_receive = 0;
        byte[] bytes_receive = new byte[length];
        try {
            InputStream input = socket.getInputStream();
            while (count_receive<length)
            {
                count_receive += input.read(bytes_receive, count_receive, length-count_receive);
            }
        }
        catch (IOException ex)
        {
            CloseSocket(socket);
            resultExOne.Message = ex.getMessage();
            return  resultExOne;
        }

        resultExOne.IsSuccess = true;
        resultExOne.Content = bytes_receive;
        return  resultExOne;
    }


    /**
     * 从套接字接收指定长度的字节数据
     * @param socket 网络套接字
     * @param timeOut 超时时间
     * @param netMsg 消息格式
     * @param <TNetMessage> 类型
     * @return 消息类
     */
    protected <TNetMessage extends INetMessage> OperateResultExOne<TNetMessage> ReceiveMessage(Socket socket, int timeOut, TNetMessage netMsg)
    {
        OperateResultExOne<TNetMessage> resultExOne = new OperateResultExOne<>();

        // 超时接收的代码验证
        HslTimeOut hslTimeOut = new HslTimeOut( );
        hslTimeOut.DelayTime = timeOut;
        hslTimeOut.WorkSocket = socket;

        // if (timeOut > 0) ThreadPool.QueueUserWorkItem( new WaitCallback( ThreadPoolCheckTimeOut ), hslTimeOut );

        // 接收指令头
        OperateResultExOne<byte[]> headResult = Receive( socket, netMsg.ProtocolHeadBytesLength() );
        if (!headResult.IsSuccess)
        {
            hslTimeOut.IsSuccessful = true;
            resultExOne.CopyErrorFromOther( headResult );
            return resultExOne;
        }

        netMsg.setHeadBytes( headResult.Content );
        if (!netMsg.CheckHeadBytesLegal(Utilities.UUID2Byte(Token)))
        {
            // 令牌校验失败
            hslTimeOut.IsSuccessful = true;
            CloseSocket(socket);
            if(LogNet != null) LogNet.WriteError( toString( ), StringResources.TokenCheckFailed );
            resultExOne.Message = StringResources.TokenCheckFailed;
            return resultExOne;
        }


        int contentLength = netMsg.GetContentLengthByHeadBytes( );
        if (contentLength == 0)
        {
            netMsg.setHeadBytes( new byte[0] );
        }
        else
        {
            OperateResultExOne<byte[]> contentResult = Receive( socket, contentLength );
            if (!contentResult.IsSuccess)
            {
                hslTimeOut.IsSuccessful = true;
                resultExOne.CopyErrorFromOther( contentResult );
                return resultExOne;
            }

            netMsg.setContentBytes( contentResult.Content);
        }

        // 防止没有实例化造成后续的操作失败
        if (netMsg.getContentBytes() == null){ netMsg.setContentBytes( new byte[0]);}
        hslTimeOut.IsSuccessful = true;
        resultExOne.Content = netMsg;
        resultExOne.IsSuccess = true;
        return resultExOne;
    }


    /**
     * 发送一串数据到网络套接字中
     * @param socket 网络套接字
     * @param data 数据
     * @return 是否发送成功
     */
    protected OperateResult Send(Socket socket,byte[] data){
        OperateResult result = new OperateResult();
        if(data == null) {
            result.IsSuccess = true;
            return result;
        }
        try {
            DataOutputStream output = new DataOutputStream(socket.getOutputStream());
            output.write(data, 0, data.length);
        }
        catch (IOException ex)
        {
            result.Message = ex.getMessage();
            return  result;
        }

        result.IsSuccess = true;
        return  result;
    }


    /**
     * 创建一个套接字并且连接到服务器
     * @param endPoint 目标节点
     * @param timeOut 超时时间
     * @return 连接成功的标志
     */
    protected  OperateResultExOne<Socket> CreateSocketAndConnect(SocketAddress endPoint, int timeOut){
        OperateResultExOne<Socket> operateResultExOne = new OperateResultExOne<>();

        Socket socket = new Socket();
        try {
            socket.connect(endPoint,timeOut);
            operateResultExOne.Content = socket;
            operateResultExOne.IsSuccess = true;
        }
        catch (IOException ex)
        {
            operateResultExOne.Message = ex.getMessage();
            CloseSocket(socket);
        }

        return operateResultExOne;
    }

    /**
     * 创建一个套接字并且连接到服务器
     * @param ipAddress ip地址
     * @param port 端口号
     * @param timeOut 超时时间
     * @return 连接成功的标志
     */
    protected  OperateResultExOne<Socket> CreateSocketAndConnect(String ipAddress,int port, int timeOut) {
        SocketAddress endPoint = new InetSocketAddress(ipAddress,port);
        return CreateSocketAndConnect(endPoint,timeOut);
    }


    /**
     * 读取流中的数据到缓存区
     * @param stream 流数据
     * @param buffer 缓冲数据
     * @return
     */
    protected OperateResultExOne<Integer> ReadStream(InputStream stream, byte[] buffer) {
        OperateResultExOne<Integer> resultExOne = new OperateResultExOne<>();
        int read_count = 0;
        try {
            while (read_count < buffer.length) {
                read_count += stream.read(buffer, read_count, buffer.length - read_count);
            }
            resultExOne.Content = read_count;
            resultExOne.IsSuccess = true;
        } catch (IOException ex) {
            resultExOne.Message = ex.getMessage();
        }

        return resultExOne;
    }


    /**
     * 将字节流数据写入到输出流里面去
     * @param stream 字节流
     * @param buffer 缓存数据
     * @return 写入是否成功
     */
    protected OperateResult WriteStream(OutputStream stream, byte[] buffer ) {
        OperateResult result = new OperateResultExOne<>();
        try {
            stream.write(buffer, 0, buffer.length);
            result.IsSuccess = true;
        } catch (IOException ex) {
            result.Message = ex.getMessage();
        }

        return result;
    }


    /**
     * 安全的关闭一个套接字
     * @param socket 网络套接字
     */
    protected void CloseSocket(Socket socket){
        if(socket != null){
            try {
                socket.close();
            }
            catch (Exception ex){

            }
        }
    }


    /**
     * 组件的日志工具，支持日志记录
     */
    public ILogNet LogNet = null;

    /**
     * 网络类的身份令牌
     */
    public UUID Token = null;



    /**
     * 返回对象的字符串表示形式
     * @return
     */
    @Override
    public String toString(){
        return "NetworkBase";
    }


}
