package HslCommunication.Core.Net.NetworkBase;

import HslCommunication.BasicFramework.SoftBasic;
import HslCommunication.Core.IMessage.INetMessage;
import HslCommunication.Core.Net.StateOne.AlienSession;
import HslCommunication.Core.Transfer.ByteTransformHelper;
import HslCommunication.Core.Transfer.IByteTransform;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.Core.Types.OperateResultExTwo;
import HslCommunication.StringResources;

import java.lang.reflect.ParameterizedType;
import java.net.Socket;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

public class NetworkDoubleBase<TNetMessage extends INetMessage  ,TTransform extends IByteTransform> extends NetworkBase
{

    /// <summary>
    /// 默认的无参构造函数
    /// </summary>
    public NetworkDoubleBase( )
    {
        queueLock = new ReentrantLock();                               // 实例化数据访问锁
        byteTransform = getInstanceOfTTransform();                           // 实例化数据转换规则
        connectionId = SoftBasic.GetUniqueStringByGuidAndRandom( );
    }



    private TTransform byteTransform;                // 数据变换的接口
    private String ipAddress = "127.0.0.1";          // 连接的IP地址
    private int port = 10000;                        // 端口号
    private int connectTimeOut = 10000;              // 连接超时时间设置
    private int receiveTimeOut = 10000;              // 数据接收的超时时间
    private boolean isPersistentConn = false;           // 是否处于长连接的状态
    private Lock queueLock = null;                      // 数据访问的同步锁
    private boolean IsSocketError = false;              // 指示长连接的套接字是否处于错误的状态
    private boolean isUseSpecifiedSocket = false;       // 指示是否使用指定的网络套接字访问数据
    private String connectionId = "";                  // 当前连接



    private TTransform getInstanceOfTTransform( )
    {
        ParameterizedType superClass = (ParameterizedType) getClass().getGenericSuperclass();
        Class<TTransform> type = (Class<TTransform>) superClass.getActualTypeArguments()[1];
        try
        {
            return type.newInstance();
        }
        catch (Exception e)
        {
            throw new RuntimeException(e);
        }
    }


    private TNetMessage getInstanceOfTNetMessage( )
    {
        ParameterizedType superClass = (ParameterizedType) getClass().getGenericSuperclass();
        Class<TNetMessage> type = (Class<TNetMessage>) superClass.getActualTypeArguments()[0];
        try
        {
            return type.newInstance();
        }
        catch (Exception e)
        {
            // Oops, no default constructor
            throw new RuntimeException(e);
        }
    }




    /**
     * 获取数据变换机制
     * @return
     */
    public TTransform getByteTransform( ){
        return  byteTransform;
    }

    /**
     * 设置数据变换机制
     * @param transform 数据变换
     */
    public void setByteTransform(TTransform transform)
    {
        byteTransform = transform;
    }

    /**
     * 获取连接的超时时间
     */
    public int getConnectTimeOut( ){
        return connectTimeOut;
    }

    /**
     * 设置连接的超时时间
     * @param connectTimeOut 超时时间，单位是秒
     */
    public void setConnectTimeOut(int connectTimeOut) {
        this.connectTimeOut = connectTimeOut;
    }


    /**
     * 获取接收服务器反馈的时间，如果为负数，则不接收反馈
     * @return
     */
    public int getReceiveTimeOut( ){
        return receiveTimeOut;
    }

    /**
     * 设置接收服务器反馈的时间，如果为负数，则不接收反馈
     * @param receiveTimeOut
     */
    public void setReceiveTimeOut(int receiveTimeOut){
        this.receiveTimeOut = receiveTimeOut;
    }


    /**
     * 获取服务器的IP地址
     * @return Ip地址信息
     */
    public String getIpAddress() {
        return ipAddress;
    }


    /**
     * 设置服务器的IP地址
     * @param ipAddress IP地址
     */
    public void setIpAddress(String ipAddress) {
        if(!ipAddress.isEmpty()){
            this.ipAddress = ipAddress;
        }
    }


    /**
     * 获取服务器的端口
     * @return 端口
     */
    public int getPort() {
        return port;
    }


    /**
     * 设置服务器的端口号
     * @param port 端口号
     */
    public void setPort(int port) {
        this.port = port;
    }

    /**
     * 当前连接的唯一ID号，默认为长度20的guid码加随机数组成，也可以自己指定
     * @return
     */
    public String getConnectionId() {
        return connectionId;
    }

    /**
     * 设置当前的连接ID
     * @param connectionId
     */
    public void setConnectionId(String connectionId) {
        this.connectionId = connectionId;
    }



    /**
     * 当前的异形连接对象，如果设置了异性连接的话
     */
    public AlienSession AlienSession = null;



    /**
     * 在读取数据之前可以调用本方法将客户端设置为长连接模式，相当于跳过了ConnectServer的结果验证，对异形客户端无效
     */
    public void SetPersistentConnection( )
    {
        isPersistentConn = true;
    }


    /**
     * 切换短连接模式到长连接模式，后面的每次请求都共享一个通道
     * @return 返回连接结果，如果失败的话（也即IsSuccess为False），包含失败信息
     */
    public OperateResult ConnectServer( )
    {
        isPersistentConn = true;
        OperateResult result = new OperateResult( );

        // 重新连接之前，先将旧的数据进行清空
        CloseSocket(CoreSocket);

        OperateResultExOne<Socket> rSocket = CreateSocketAndInitialication( );

        if (!rSocket.IsSuccess)
        {
            IsSocketError = true;                         // 创建失败
            rSocket.Content = null;
            result.Message = rSocket.Message;
        }
        else
        {
            CoreSocket = rSocket.Content;                 // 创建成功
            result.IsSuccess = true;
            if(LogNet != null) LogNet.WriteDebug( toString( ), StringResources.NetEngineStart );
        }

        return result;
    }



    /**
     * 使用指定的套接字创建异形客户端
     * @param session 会话
     * @return 通常都为成功
     */
    public OperateResult ConnectServer( AlienSession session )
    {
        isPersistentConn = true;
        isUseSpecifiedSocket = true;


        if (session != null)
        {
            if(AlienSession != null ) CloseSocket(AlienSession.getSocket());

            if (connectionId.isEmpty())
            {
                connectionId = session.getDTU();
            }

            if (connectionId == session.getDTU())
            {
                CoreSocket = session.getSocket();
                IsSocketError = false;
                AlienSession = session;
                return InitializationOnConnect( session.getSocket() );
            }
            else
            {
                IsSocketError = true;
                return new OperateResult( );
            }
        }
        else
        {
            IsSocketError = true;
            return new OperateResult( );
        }
    }



    /**
     * 在长连接模式下，断开服务器的连接，并切换到短连接模式
     * @return 关闭连接，不需要查看IsSuccess属性查看
     */
    public OperateResult ConnectClose( )
    {
        OperateResult result = new OperateResult( );
        isPersistentConn = false;

        queueLock.lock();

        // 额外操作
        result = ExtraOnDisconnect( CoreSocket );
        // 关闭信息
        if(CoreSocket != null ) CloseSocket(CoreSocket);
        CoreSocket = null;

        queueLock.unlock();

        if(LogNet != null ) LogNet.WriteDebug( toString( ), StringResources.NetEngineClose );
        return result;
    }


    /**
     * 在连接的时候进行初始化
     * @param socket 网络套接字
     * @return 结果类对象
     */
    protected OperateResult InitializationOnConnect( Socket socket ) {
        return OperateResult.CreateSuccessResult();
    }


    /**
     * 在将要和服务器进行断开的情况下额外的操作
     * @param socket 网络套接字
     * @return 结果类对象
     */
    protected OperateResult ExtraOnDisconnect( Socket socket ) {
        return OperateResult.CreateSuccessResult();
    }


    /***************************************************************************************
     *
     *    主要的数据交互分为4步
     *    1. 连接服务器，或是获取到旧的使用的网络信息
     *    2. 发送数据信息
     *    3. 接收反馈的数据信息
     *    4. 关闭网络连接，如果是短连接的话
     *
     **************************************************************************************/


    /**
     * 获取本次操作的可用的网络套接字
     * @return 是否成功，如果成功，使用这个套接字
     */
    private OperateResultExOne<Socket> GetAvailableSocket( )
    {
        if (isPersistentConn)
        {
            // 如果是异形模式
            if (isUseSpecifiedSocket)
            {
                if(IsSocketError)
                {
                    OperateResultExOne<Socket> rSocket = new OperateResultExOne<>();
                    rSocket.Message = "连接不可用";
                    return rSocket;
                }
                else
                {

                    return OperateResultExOne.CreateSuccessResult( CoreSocket );
                }
            }
            else
            {
                // 长连接模式
                if (IsSocketError || CoreSocket == null)
                {
                    OperateResult connect = ConnectServer( );
                    if (!connect.IsSuccess)
                    {
                        IsSocketError = true;
                        OperateResultExOne<Socket> rSocket = new OperateResultExOne<>();
                        rSocket.Message = connect.Message;
                        rSocket.ErrorCode = connect.ErrorCode;
                        return rSocket;
                    }
                    else
                    {
                        IsSocketError = false;
                        return OperateResultExOne.CreateSuccessResult( CoreSocket );
                    }
                }
                else
                {
                    return OperateResultExOne.CreateSuccessResult( CoreSocket );
                }
            }
        }
        else
        {
            // 短连接模式
            return CreateSocketAndInitialication( );
        }
    }



    /**
     * 连接并初始化网络套接字
     * @return 最终的连接对象
     */
    private OperateResultExOne<Socket> CreateSocketAndInitialication( )
    {
        OperateResultExOne<Socket> result = CreateSocketAndConnect(  ipAddress , port , connectTimeOut );
        if (result.IsSuccess)
        {
            // 初始化
            OperateResult initi = InitializationOnConnect( result.Content );
            if (!initi.IsSuccess)
            {
                CloseSocket(result.Content);
                result.IsSuccess = initi.IsSuccess;
                result.CopyErrorFromOther( initi );
            }
        }
        return result;
    }


    /**
     * 在其他指定的套接字上，使用报文来通讯，传入需要发送的消息，返回一条完整的数据指令
     * @param socket 指定的套接字
     * @param send 发送的完整的报文信息
     * @return 接收的完整的报文信息
     */
    public OperateResultExOne<byte[]> ReadFromCoreServer( Socket socket, byte[] send )
    {
        OperateResultExOne<byte[]> result = new OperateResultExOne<byte[]>( );

        OperateResultExTwo<byte[], byte[]> read = ReadFromCoreServerBase( socket, send );
        if (read.IsSuccess)
        {
            result.IsSuccess = read.IsSuccess;
            result.Content = new byte[read.Content1.length + read.Content2.length];
            if (read.Content1.length > 0) System.arraycopy(read.Content1,0,result.Content,0,read.Content1.length);
            if (read.Content2.length > 0) System.arraycopy(read.Content2,0,result.Content,read.Content1.length,read.Content2.length);
        }
        else
        {
            result.CopyErrorFromOther( read );
        }
        return result;
    }



    /**
     * 使用底层的数据报文来通讯，传入需要发送的消息，返回一条完整的数据指令
     * @param send 发送的完整的报文信息
     * @return 接收的完整的报文信息
     */
    public OperateResultExOne<byte[]> ReadFromCoreServer( byte[] send )
    {
        OperateResultExOne<byte[]> result = new OperateResultExOne<byte[]>( );
        // string tmp1 = BasicFramework.SoftBasic.ByteToHexString( send, '-' );

        queueLock.lock( );

        // 获取有用的网络通道，如果没有，就建立新的连接
        OperateResultExOne<Socket> resultSocket = GetAvailableSocket( );
        if (!resultSocket.IsSuccess)
        {
            IsSocketError = true;
            if (AlienSession != null) AlienSession.setIsStatusOk( false);
            queueLock.unlock();
            result.CopyErrorFromOther( resultSocket );
            return result;
        }

        OperateResultExOne<byte[]> read = ReadFromCoreServer( resultSocket.Content, send );

        if (read.IsSuccess)
        {
            IsSocketError = false;
            result.IsSuccess = read.IsSuccess;
            result.Content = read.Content;
            // string tmp2 = BasicFramework.SoftBasic.ByteToHexString( result.Content ) ;
        }
        else
        {
            IsSocketError = true;
            if (AlienSession != null) AlienSession.setIsStatusOk(false);
            result.CopyErrorFromOther( read );
        }

        queueLock.unlock();
        if (!isPersistentConn) CloseSocket(resultSocket.Content );
        return result;
    }


    /**
     * 使用底层的数据报文来通讯，传入需要发送的消息，返回最终的数据结果，被拆分成了头子节和内容字节信息
     * @param socket 网络套接字
     * @param send 发送的数据
     * @return 结果对象
     */
    protected OperateResultExTwo<byte[], byte[]> ReadFromCoreServerBase(Socket socket, byte[] send )
    {
        OperateResultExTwo<byte[], byte[]> result = new OperateResultExTwo<byte[], byte[]>( );
        // LogNet?.WriteDebug( ToString( ), "Command: " + BasicFramework.SoftBasic.ByteToHexString( send ) );
        TNetMessage netMsg = getInstanceOfTNetMessage();
        netMsg.setSendBytes(send);

        // 发送数据信息
        OperateResult resultSend = Send( socket, send );
        if (!resultSend.IsSuccess)
        {
            CloseSocket(socket);
            result.CopyErrorFromOther( resultSend );
            return result;
        }

        // 接收超时时间大于0时才允许接收远程的数据
        if (receiveTimeOut >= 0)
        {
            // 接收数据信息
            OperateResultExOne<TNetMessage> resultReceive = ReceiveMessage(socket, receiveTimeOut, netMsg);
            if (!resultReceive.IsSuccess)
            {
                CloseSocket(socket );
                result.CopyErrorFromOther( resultReceive );
                // result.Message = "Receive data timeout: " + receiveTimeOut;
                return result;
            }

            // 复制结果
            result.Content1 = resultReceive.Content.getHeadBytes();
            result.Content2 = resultReceive.Content.getContentBytes();
        }

        result.IsSuccess = true;
        return result;
    }



    /**
     * 返回表示当前对象的字符串
     * @return
     */
    @Override
    public String toString( ) {
        return "NetworkDoubleBase<TNetMessage>";
    }



    /**
     * 将指定的OperateResult类型转化
     * @param result 原始的类型
     * @return 转化后的类型
     */
    protected OperateResultExOne<Boolean> GetBoolResultFromBytes( OperateResultExOne<byte[]> result )
    {
        return ByteTransformHelper.GetBoolResultFromBytes( result, byteTransform);
    }


    /**
     * 将指定的OperateResult类型转化
     * @param result 原始的类型
     * @return 转化后的类型
     */
    protected OperateResultExOne<Byte> GetByteResultFromBytes( OperateResultExOne<byte[]> result )
    {
        return ByteTransformHelper.GetByteResultFromBytes( result, byteTransform );
    }



    /**
     * 将指定的OperateResult类型转化
     * @param result 原始的类型
     * @return 转化后的类型
     */
    protected OperateResultExOne<Short> GetInt16ResultFromBytes( OperateResultExOne<byte[]> result )
    {
        return ByteTransformHelper.GetInt16ResultFromBytes( result, byteTransform );
    }



    /**
     * 将指定的OperateResult类型转化
     * @param result 原始的类型
     * @return 转化后的类型
     */
    protected OperateResultExOne<Integer> GetInt32ResultFromBytes( OperateResultExOne<byte[]> result )
    {
        return ByteTransformHelper.GetInt32ResultFromBytes( result, byteTransform );
    }



    /**
     * 将指定的OperateResult类型转化
     * @param result 原始的类型
     * @return 转化后的类型
     */
    protected OperateResultExOne<Long> GetInt64ResultFromBytes( OperateResultExOne<byte[]> result )
    {
        return ByteTransformHelper.GetInt64ResultFromBytes( result, byteTransform );
    }



    /**
     * 将指定的OperateResult类型转化
     * @param result 原始的类型
     * @return 转化后的类型
     */
    protected OperateResultExOne<Float> GetSingleResultFromBytes( OperateResultExOne<byte[]> result )
    {
        return ByteTransformHelper.GetSingleResultFromBytes( result, byteTransform );
    }


    /**
     * 将指定的OperateResult类型转化
     * @param result 原始的类型
     * @return 转化后的类型
     */
    protected OperateResultExOne<Double> GetDoubleResultFromBytes( OperateResultExOne<byte[]> result )
    {
        return ByteTransformHelper.GetDoubleResultFromBytes( result, byteTransform );
    }


    /**
     * 将指定的OperateResult类型转化
     * @param result 原始的类型
     * @return 转化后的类型
     */
    protected OperateResultExOne<String> GetStringResultFromBytes( OperateResultExOne<byte[]> result )
    {
        return ByteTransformHelper.GetStringResultFromBytes( result, byteTransform );
    }



}
