using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Threading;

namespace HslCommunication.Core
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
        }

        #endregion

        #region Private Member
        
        private TTransform byteTransform;                // 数据变换的接口
        private string ipAddress = "127.0.0.1";          // 连接的IP地址
        private int port = 10000;                        // 端口号
        private int connectTimeOut = 10000;              // 连接超时时间设置
        private int receiveTimeOut = 10000;              // 数据接收的超时时间
        private bool IsPersistentConn = false;           // 是否处于长连接的状态
        private SimpleHybirdLock InteractiveLock;        // 一次正常的交互的互斥锁
        private bool IsSocketError = false;         // 指示长连接的套接字是否处于错误的状态


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
            get
            {
                return connectTimeOut;
            }
            set
            {
                connectTimeOut = value;
            }
        }

        /// <summary>
        /// 获取或设置接收服务器反馈的时间，如果为负数，则不接收反馈
        /// </summary>
        public int ReceiveTimeOut
        {
            get
            {
                return receiveTimeOut;
            }
            set
            {
                receiveTimeOut = value;
            }
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
                if(!IPAddress.TryParse( value, out IPAddress address))
                {
                    throw new Exception( "Ip地址设置异常，格式不正确" );
                }
                ipAddress = value;
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

        #endregion

        #region Open Close

        /// <summary>
        /// 切换短连接模式到长连接模式，后面的每次请求都共享一个通道
        /// </summary>
        /// <returns>返回连接结果，如果失败的话（也即IsSuccess为False），包含失败信息</returns>
        public OperateResult ConnectServer( )
        {
            IsPersistentConn = true;
            OperateResult result = new OperateResult( );

            // 重新连接之前，先将旧的数据进行清空
            CoreSocket?.Close( );

            OperateResult<Socket> rSocket = CreateSocketAndInitialication( );

            if (!rSocket.IsSuccess)
            {
                IsSocketError = true;
                // 创建失败
                rSocket.Content = null;
            }
            else
            {
                // 创建成功
                CoreSocket = rSocket.Content;
                // 初始化成功
                result.IsSuccess = true;
                LogNet?.WriteDebug( ToString( ), StringResources.NetEngineStart );
            }

            return result;
        }




        /// <summary>
        /// 在长连接模式下，断开服务器的连接，并切换到短连接模式
        /// </summary>
        /// <returns>关闭连接，不需要查看IsSuccess属性查看</returns>
        public OperateResult ConnectClose( )
        {
            OperateResult result = new OperateResult( );
            IsPersistentConn = false;

            // 额外操作
            result = ExtraOnDisconnect( CoreSocket );
            // 关闭信息
            CoreSocket?.Close( );
            CoreSocket = null;

            LogNet?.WriteDebug( ToString( ), StringResources.NetEngineClose );
            return result;
        }

        #endregion

        #region Receive Message

        /// <summary>
        /// 接收一条完整的数据，使用异步接收完成，包含了指令头信息
        /// </summary>
        /// <param name="socket">已经打开的网络套接字</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns>数据的接收结果对象</returns>
        protected OperateResult<TNetMessage> ReceiveMessage( Socket socket, int timeOut )
        {
            TNetMessage netMsg = new TNetMessage( );
            OperateResult<TNetMessage> result = new OperateResult<TNetMessage>( );

            // 超时接收的代码验证
            HslTimeOut hslTimeOut = new HslTimeOut( )
            {
                DelayTime = timeOut,
                WorkSocket = socket,
            };
            if (timeOut > 0) ThreadPool.QueueUserWorkItem( new WaitCallback( ThreadPoolCheckTimeOut ), hslTimeOut );

            // 接收指令头
            OperateResult<byte[]> headResult = Receive( socket, netMsg.ProtocolHeadBytesLength );
            if (!headResult.IsSuccess)
            {
                hslTimeOut.IsSuccessful = true;
                result.CopyErrorFromOther( headResult );
                return result;
            }

            netMsg.HeadBytes = headResult.Content;
            if (!netMsg.CheckHeadBytesLegal( Token.ToByteArray( ) ))
            {
                // 令牌校验失败
                hslTimeOut.IsSuccessful = true;
                socket?.Close( );
                LogNet?.WriteError( ToString( ), StringResources.TokenCheckFailed );
                result.Message = StringResources.TokenCheckFailed;
                return result;
            }


            int contentLength = netMsg.GetContentLengthByHeadBytes( );
            if (contentLength == 0)
            {
                netMsg.ContentBytes = new byte[0];
            }
            else
            {
                OperateResult<byte[]> contentResult = Receive( socket, contentLength );
                if (!headResult.IsSuccess)
                {
                    hslTimeOut.IsSuccessful = true;
                    result.CopyErrorFromOther( contentResult );
                    return result;
                }

                netMsg.ContentBytes = contentResult.Content;
            }

            hslTimeOut.IsSuccessful = true;
            result.Content = netMsg;
            result.IsSuccess = true;
            return result;
        }

        #endregion

        #region Initialization And Extra

        /// <summary>
        /// 连接上服务器后需要进行的初始化操作
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <returns></returns>
        protected virtual OperateResult InitilizationOnConnect( Socket socket )
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
            var result = new OperateResult<Socket>( );

            if (IsPersistentConn)
            {
                // 长连接模式
                if (IsSocketError || CoreSocket == null)
                {
                    OperateResult connect = ConnectServer( );
                    if (!connect.IsSuccess)
                    {
                        IsSocketError = true;
                        result.CopyErrorFromOther( connect );
                        return result;
                    }
                    else
                    {
                        IsSocketError = false;
                        return OperateResult.CreateSuccessResult( CoreSocket );
                    }
                }
                else
                {
                    return  OperateResult.CreateSuccessResult( CoreSocket );
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
                OperateResult initi = InitilizationOnConnect( result.Content );
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
        /// 使用底层的数据报文来通讯，传入需要发送的消息，返回一条完整的数据指令
        /// </summary>
        /// <param name="send">发送的完整的报文信息</param>
        /// <returns>接收的完整的报文信息</returns>
        public OperateResult<byte[]> ReadFromCoreServer( byte[] send )
        {
            var result = new OperateResult<byte[]>( );
            
            InteractiveLock.Enter( );

            // 获取有用的网络通道，如果没有，就建立新的连接
            OperateResult<Socket> resultSocket = GetAvailableSocket( );
            if (!resultSocket.IsSuccess)
            {
                IsSocketError = true;
                InteractiveLock.Leave( );
                result.CopyErrorFromOther( resultSocket );
                return result;
            }

            var read = ReadFromCoreServerBase( resultSocket.Content, send );

            if (read.IsSuccess)
            {
                IsSocketError = false;
                result.IsSuccess = read.IsSuccess;
                result.Content = new byte[read.Content1.Length + read.Content2.Length];
                if (read.Content1.Length > 0) read.Content1.CopyTo( result.Content, 0 );
                if (read.Content2.Length > 0) read.Content2.CopyTo( result.Content, read.Content1.Length );
            }
            else
            {
                IsSocketError = true;
                result.CopyErrorFromOther( read );
            }


            InteractiveLock.Leave( );
            if (!IsPersistentConn) resultSocket.Content?.Close( );
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
                OperateResult<TNetMessage> resultReceive = ReceiveMessage( socket, receiveTimeOut );
                if (!resultReceive.IsSuccess)
                {
                    socket?.Close( );
                    result.CopyErrorFromOther( resultReceive );
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
        /// 获取本对象的字符串标识形式
        /// </summary>
        /// <returns></returns>
        public override string ToString( )
        {
            return "NetworkDoubleBase<TNetMessage>";
        }


        #endregion

        #region Result Transform

        /// <summary>
        /// 结果转换操作的基础方法，需要支持类型，及转换的委托
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="result"></param>
        /// <param name="translator"></param>
        /// <returns></returns>
        private OperateResult<TResult> GetResultFromBytes<TResult>( OperateResult<byte[]> result, Func<byte[], TResult> translator )
        {
            var tmp = new OperateResult<TResult>( );
            if (result.IsSuccess)
            {
                tmp.Content = translator( result.Content );
            }
            tmp.IsSuccess = result.IsSuccess;
            tmp.CopyErrorFromOther( result );
            return tmp;
        }


        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<bool> GetBoolResultFromBytes( OperateResult<byte[]> result )
        {
            return GetResultFromBytes( result, byteTransform.TransBool );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<byte> GetByteResultFromBytes( OperateResult<byte[]> result )
        {
            return GetResultFromBytes( result, m => byteTransform.TransByte( m, 0 ) );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<short> GetInt16ResultFromBytes( OperateResult<byte[]> result )
        {
            return GetResultFromBytes( result, m => byteTransform.TransInt16( m, 0 ) );
        }


        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<ushort> GetUInt16ResultFromBytes( OperateResult<byte[]> result )
        {
            return GetResultFromBytes( result, m => byteTransform.TransUInt16( m, 0 ) );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<int> GetInt32ResultFromBytes( OperateResult<byte[]> result )
        {
            return GetResultFromBytes( result, m => byteTransform.TransInt32( m, 0 ) );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<uint> GetUInt32ResultFromBytes( OperateResult<byte[]> result )
        {
            return GetResultFromBytes( result, m => byteTransform.TransUInt32( m, 0 ) );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<long> GetInt64ResultFromBytes( OperateResult<byte[]> result )
        {
            return GetResultFromBytes( result, m => byteTransform.TransInt64( m, 0 ) );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<ulong> GetUInt64ResultFromBytes( OperateResult<byte[]> result )
        {
            return GetResultFromBytes( result, m => byteTransform.TransUInt64( m, 0 ) );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<float> GetSingleResultFromBytes( OperateResult<byte[]> result )
        {
            return GetResultFromBytes( result, m => byteTransform.TransSingle( m, 0 ) );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<double> GetDoubleResultFromBytes( OperateResult<byte[]> result )
        {
            return GetResultFromBytes( result, m => byteTransform.TransDouble( m, 0 ) );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <returns>转化后的类型</returns>
        protected OperateResult<string> GetStringResultFromBytes( OperateResult<byte[]> result )
        {
            return GetResultFromBytes<string>( result, byteTransform.TransString );
        }
        

        #endregion
    }


}
