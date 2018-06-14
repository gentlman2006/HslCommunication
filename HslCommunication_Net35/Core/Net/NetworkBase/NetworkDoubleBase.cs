using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Threading;
using HslCommunication.Core.IMessage;

namespace HslCommunication.Core.Net
{


    /// <summary>
    /// 支持长连接，短连接两个模式的通用客户端基类
    /// </summary>
    /// <typeparam name="TNetMessage">指定了消息的解析规则</typeparam>
    /// <typeparam name="TTransform">指定了数据转换的规则</typeparam>
    public class NetworkDoubleBase<TNetMessage, TTransform> : NetworkBase where TNetMessage : INetMessage, new() where TTransform : IByteTransform, new()
    {
        #region Constructor

        /// <summary>
        /// 默认的无参构造函数
        /// </summary>
        public NetworkDoubleBase( )
        {
            InteractiveLock = new SimpleHybirdLock( );                    // 实例化数据访问锁
            byteTransform = new TTransform( );                            // 实例化数据转换规则
            connectionId = BasicFramework.SoftBasic.GetUniqueStringByGuidAndRandom( );
        }

        #endregion

        #region Private Member
        
        private TTransform byteTransform;                // 数据变换的接口
        private string ipAddress = "127.0.0.1";          // 连接的IP地址
        private int port = 10000;                        // 端口号
        private int connectTimeOut = 10000;              // 连接超时时间设置
        private int receiveTimeOut = 10000;              // 数据接收的超时时间
        private bool isPersistentConn = false;           // 是否处于长连接的状态
        private SimpleHybirdLock InteractiveLock;        // 一次正常的交互的互斥锁
        private bool IsSocketError = false;              // 指示长连接的套接字是否处于错误的状态
        private bool isUseSpecifiedSocket = false;       // 指示是否使用指定的网络套接字访问数据
        private string connectionId = string.Empty;      // 当前连接

        #endregion

        #region Public Member

        /// <summary>
        /// 当前客户端的数据变换机制
        /// </summary>
        public TTransform ByteTransform
        {
            get { return byteTransform; }
            set { byteTransform = value; }
        }
        
        /// <summary>
        /// 获取或设置连接的超时时间
        /// </summary>
        public int ConnectTimeOut
        {
            get{return connectTimeOut;}
            set { connectTimeOut = value;}
        }

        /// <summary>
        /// 获取或设置接收服务器反馈的时间，如果为负数，则不接收反馈
        /// </summary>
        public int ReceiveTimeOut
        {
            get{return receiveTimeOut;}
            set{ receiveTimeOut = value;}
        }

        /// <summary>
        /// 服务器的IP地址
        /// </summary>
        public string IpAddress
        {
            get
            {
                return ipAddress;
            }
            set
            {
                if (!string.IsNullOrEmpty( value ))
                {
                    if (!IPAddress.TryParse( value, out IPAddress address ))
                    {
                        throw new Exception( "Ip地址设置异常，格式不正确" );
                    }
                    ipAddress = value;
                }
            }
        }

        /// <summary>
        /// 服务器的端口号
        /// </summary>
        public int Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
            }
        }

        /// <summary>
        /// 当前连接的唯一ID号，默认为长度20的guid码加随机数组成，也可以自己指定
        /// </summary>
        /// <remarks>
        /// Current Connection ID, conclude guid and random data, also, you can spcified
        /// </remarks>
        public string ConnectionId
        {
            get { return connectionId; }
            set { connectionId = value; }
        }


        /// <summary>
        /// 当前的异形连接对象，如果设置了异形连接的话
        /// </summary>
        public AlienSession AlienSession { get; set; }


        #endregion

        #region Public Method

        /// <summary>
        /// 在读取数据之前可以调用本方法将客户端设置为长连接模式，相当于跳过了ConnectServer的结果验证，对异形客户端无效
        /// </summary>
        public void SetPersistentConnection( )
        {
            isPersistentConn = true;
        }

        #endregion

        #region Connect Close

        /// <summary>
        /// 切换短连接模式到长连接模式，后面的每次请求都共享一个通道
        /// </summary>
        /// <returns>返回连接结果，如果失败的话（也即IsSuccess为False），包含失败信息</returns>
        public OperateResult ConnectServer( )
        {
            isPersistentConn = true;
            OperateResult result = new OperateResult( );

            // 重新连接之前，先将旧的数据进行清空
            CoreSocket?.Close( );

            OperateResult<Socket> rSocket = CreateSocketAndInitialication( );

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
                LogNet?.WriteDebug( ToString( ), StringResources.NetEngineStart );
            }

            return result;
        }


        /// <summary>
        /// 使用指定的套接字创建异形客户端
        /// </summary>
        /// <returns>通常都为成功</returns>
        public OperateResult ConnectServer( AlienSession session )
        {
            isPersistentConn = true;
            isUseSpecifiedSocket = true;


            if (session != null)
            {
                AlienSession?.Socket?.Close( );

                if (string.IsNullOrEmpty( ConnectionId ))
                {
                    ConnectionId = session.DTU;
                }

                if (ConnectionId == session.DTU)
                {
                    CoreSocket = session.Socket;
                    IsSocketError = false;
                    AlienSession = session;
                    return InitializationOnConnect( session.Socket );
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


        /// <summary>
        /// 在长连接模式下，断开服务器的连接，并切换到短连接模式
        /// </summary>
        /// <returns>关闭连接，不需要查看IsSuccess属性查看</returns>
        public OperateResult ConnectClose( )
        {
            OperateResult result = new OperateResult( );
            isPersistentConn = false;

            InteractiveLock.Enter( );
            // 额外操作
            result = ExtraOnDisconnect( CoreSocket );
            // 关闭信息
            CoreSocket?.Close( );
            CoreSocket = null;
            InteractiveLock.Leave( );
            
            LogNet?.WriteDebug( ToString( ), StringResources.NetEngineClose );
            return result;
        }

        #endregion

        #region Initialization And Extra

        /// <summary>
        /// 连接上服务器后需要进行的初始化操作
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <returns></returns>
        protected virtual OperateResult InitializationOnConnect( Socket socket )
        {
            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 在将要和服务器进行断开的情况下额外的操作
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <returns></returns>
        protected virtual OperateResult ExtraOnDisconnect( Socket socket )
        {
            return OperateResult.CreateSuccessResult( );
        }

        #endregion
        
        #region Core Communication

        /***************************************************************************************
         * 
         *    主要的数据交互分为4步
         *    1. 连接服务器，或是获取到旧的使用的网络信息
         *    2. 发送数据信息
         *    3. 接收反馈的数据信息
         *    4. 关闭网络连接，如果是短连接的话
         * 
         **************************************************************************************/

        /// <summary>
        /// 获取本次操作的可用的网络套接字
        /// </summary>
        /// <returns>是否成功，如果成功，使用这个套接字</returns>
        private OperateResult<Socket> GetAvailableSocket( )
        {
            if (isPersistentConn)
            {
                // 如果是异形模式
                if (isUseSpecifiedSocket)
                {
                    if(IsSocketError)
                    {
                        return new OperateResult<Socket>( ) { Message = "连接不可用" };
                    }
                    else
                    {
                        return OperateResult.CreateSuccessResult( CoreSocket );
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
                            return OperateResult.CreateFailedResult<Socket>( connect );
                        }
                        else
                        {
                            IsSocketError = false;
                            return OperateResult.CreateSuccessResult( CoreSocket );
                        }
                    }
                    else
                    {
                        return OperateResult.CreateSuccessResult( CoreSocket );
                    }
                }
            }
            else
            {
                // 短连接模式
                return CreateSocketAndInitialication( );
            }
        }


        /// <summary>
        /// 连接并初始化网络套接字
        /// </summary>
        /// <returns></returns>
        private OperateResult<Socket> CreateSocketAndInitialication( )
        {
            OperateResult<Socket> result = CreateSocketAndConnect( new IPEndPoint( IPAddress.Parse( ipAddress ), port ), connectTimeOut );
            if (result.IsSuccess)
            {
                // 初始化
                OperateResult initi = InitializationOnConnect( result.Content );
                if (!initi.IsSuccess)
                {
                    result.Content?.Close( );
                    result.IsSuccess = initi.IsSuccess;
                    result.CopyErrorFromOther( initi );
                }
            }
            return result;
        }


        /// <summary>
        /// 在其他指定的套接字上，使用报文来通讯，传入需要发送的消息，返回一条完整的数据指令
        /// </summary>
        /// <param name="socket">指定的套接字</param>
        /// <param name="send">发送的完整的报文信息</param>
        /// <returns>接收的完整的报文信息</returns>
        public OperateResult<byte[]> ReadFromCoreServer( Socket socket, byte[] send )
        {
            var result = new OperateResult<byte[]>( );

            var read = ReadFromCoreServerBase( socket, send );
            if (read.IsSuccess)
            {
                result.IsSuccess = read.IsSuccess;
                result.Content = new byte[read.Content1.Length + read.Content2.Length];
                if (read.Content1.Length > 0) read.Content1.CopyTo( result.Content, 0 );
                if (read.Content2.Length > 0) read.Content2.CopyTo( result.Content, read.Content1.Length );
            }
            else
            {
                result.CopyErrorFromOther( read );
            }
            return result;
        }


        /// <summary>
        /// 使用底层的数据报文来通讯，传入需要发送的消息，返回一条完整的数据指令
        /// </summary>
        /// <param name="send">发送的完整的报文信息</param>
        /// <returns>接收的完整的报文信息</returns>
        public OperateResult<byte[]> ReadFromCoreServer( byte[] send )
        {
            var result = new OperateResult<byte[]>( );
            // string tmp1 = BasicFramework.SoftBasic.ByteToHexString( send, '-' );

            InteractiveLock.Enter( );

            // 获取有用的网络通道，如果没有，就建立新的连接
            OperateResult<Socket> resultSocket = GetAvailableSocket( );
            if (!resultSocket.IsSuccess)
            {
                IsSocketError = true;
                if (AlienSession != null) AlienSession.IsStatusOk = false;
                InteractiveLock.Leave( );
                result.CopyErrorFromOther( resultSocket );
                return result;
            }

            OperateResult<byte[]> read = ReadFromCoreServer( resultSocket.Content, send );

            if (read.IsSuccess)
            {
                IsSocketError = false;
                result.IsSuccess = read.IsSuccess;
                result.Content = read.Content;
                //string tmp2 = BasicFramework.SoftBasic.ByteToHexString( result.Content, '-' );
                
            }
            else
            {
                IsSocketError = true;
                if (AlienSession != null) AlienSession.IsStatusOk = false;
                result.CopyErrorFromOther( read );
            }

            InteractiveLock.Leave( );
            if (!isPersistentConn) resultSocket.Content?.Close( );
            return result;
        }

        /// <summary>
        /// 使用底层的数据报文来通讯，传入需要发送的消息，返回最终的数据结果，被拆分成了头子节和内容字节信息
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="send">发送的数据</param>
        /// <returns>结果对象</returns>
        protected OperateResult<byte[], byte[]> ReadFromCoreServerBase(Socket socket, byte[] send )
        {
            var result = new OperateResult<byte[], byte[]>( );
            // LogNet?.WriteDebug( ToString( ), "Command: " + BasicFramework.SoftBasic.ByteToHexString( send ) );
            TNetMessage netMsg = new TNetMessage
            {
                SendBytes = send
            };

            // 发送数据信息
            OperateResult resultSend = Send( socket, send );
            if (!resultSend.IsSuccess)
            {
                socket?.Close( );
                result.CopyErrorFromOther( resultSend );
                return result;
            }

            // 接收超时时间大于0时才允许接收远程的数据
            if (receiveTimeOut >= 0)
            {
                // 接收数据信息
                OperateResult<TNetMessage> resultReceive = ReceiveMessage(socket, receiveTimeOut, netMsg);
                if (!resultReceive.IsSuccess)
                {
                    socket?.Close( );
                    // result.CopyErrorFromOther( resultReceive );
                    result.Message = "Receive data timeout: " + receiveTimeOut;
                    return result;
                }

                // 复制结果
                result.Content1 = resultReceive.Content.HeadBytes;
                result.Content2 = resultReceive.Content.ContentBytes;
            }
            
            result.IsSuccess = true;
            return result;
        }


        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString( )
        {
            return "NetworkDoubleBase<TNetMessage>";
        }


        #endregion

        #region Result Transform
        

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<bool> GetBoolResultFromBytes( OperateResult<byte[]> result )
        {
            return ByteTransformHelper.GetBoolResultFromBytes( result, byteTransform);
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<byte> GetByteResultFromBytes( OperateResult<byte[]> result )
        {
            return ByteTransformHelper.GetByteResultFromBytes( result, byteTransform );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<short> GetInt16ResultFromBytes( OperateResult<byte[]> result )
        {
            return ByteTransformHelper.GetInt16ResultFromBytes( result, byteTransform );
        }


        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<ushort> GetUInt16ResultFromBytes( OperateResult<byte[]> result )
        {
            return ByteTransformHelper.GetUInt16ResultFromBytes( result, byteTransform );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<int> GetInt32ResultFromBytes( OperateResult<byte[]> result )
        {
            return ByteTransformHelper.GetInt32ResultFromBytes( result, byteTransform );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<uint> GetUInt32ResultFromBytes( OperateResult<byte[]> result )
        {
            return ByteTransformHelper.GetUInt32ResultFromBytes( result, byteTransform );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<long> GetInt64ResultFromBytes( OperateResult<byte[]> result )
        {
            return ByteTransformHelper.GetInt64ResultFromBytes( result, byteTransform );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<ulong> GetUInt64ResultFromBytes( OperateResult<byte[]> result )
        {
            return ByteTransformHelper.GetUInt64ResultFromBytes( result, byteTransform );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<float> GetSingleResultFromBytes( OperateResult<byte[]> result )
        {
            return ByteTransformHelper.GetSingleResultFromBytes( result, byteTransform );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<double> GetDoubleResultFromBytes( OperateResult<byte[]> result )
        {
            return ByteTransformHelper.GetDoubleResultFromBytes( result, byteTransform );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<string> GetStringResultFromBytes( OperateResult<byte[]> result )
        {
            return ByteTransformHelper.GetStringResultFromBytes( result, byteTransform );
        }
        

        #endregion
    }


}
