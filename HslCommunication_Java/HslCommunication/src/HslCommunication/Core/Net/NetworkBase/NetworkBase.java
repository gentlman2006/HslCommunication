package HslCommunication.Core.Net.NetworkBase;

import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.LogNet.Core.ILogNet;
import HslCommunication.Core.Types.HslTimeOut;

import java.io.DataInputStream;
import java.io.IOException;
import java.net.Socket;
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
        Token = UUID.nameUUIDFromBytes(new byte[16]);
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
            DataInputStream input = new DataInputStream(socket.getInputStream());
            while (count_receive<length)
            {
                count_receive += input.read(bytes_receive, count_receive, length-count_receive);
            }
        }
        catch (IOException ex)
        {
            resultExOne.Message = ex.getMessage();
            return  resultExOne;
        }

        resultExOne.IsSuccess = true;
        resultExOne.Content = bytes_receive;
        return  resultExOne;
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
