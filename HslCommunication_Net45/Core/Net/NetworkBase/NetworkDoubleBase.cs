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
    /// <example>
    /// 无，请使用继承类实例化，然后进行数据交互。
    /// </example>
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
        /// 当前客户端的数据变换机制，当你需要从字节数据转换类型数据的时候需要。
        /// </summary>
        /// <example>
        ///    主要是用来转换数据类型的，下面仅仅演示了2个方法，其他的类型转换，类似处理。
        ///    <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="ByteTransform" title="ByteTransform示例" />
        /// </example>
        public TTransform ByteTransform
        {
            get { return byteTransform; }
            set { byteTransform = value; }
        }

        /// <summary>
        /// 获取或设置连接的超时时间
        /// </summary>
        /// <example>
        /// 设置1秒的超时的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="ConnectTimeOutExample" title="ConnectTimeOut示例" />
        /// </example>
        /// <remarks>
        /// 不适用于异形模式的连接。
        /// </remarks>
        public int ConnectTimeOut
        {
            get{return connectTimeOut;}
            set { connectTimeOut = value;}
        }

        /// <summary>
        /// 获取或设置接收服务器反馈的时间，如果为负数，则不接收反馈
        /// </summary>
        /// <example>
        /// 设置1秒的接收超时的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="ReceiveTimeOutExample" title="ReceiveTimeOut示例" />
        /// </example>
        /// <remarks>
        /// 超时的通常原因是服务器端没有配置好，导致访问失败，为了不卡死软件，所以有了这个超时的属性。
        /// </remarks>
        public int ReceiveTimeOut
        {
            get { return receiveTimeOut; }
            set { receiveTimeOut = value; }
        }

        /// <summary>
        /// 获取或是设置服务器的IP地址
        /// </summary>
        /// <remarks>
        /// 最好实在初始化的时候进行指定，当使用短连接的时候，支持动态更改，切换；当使用长连接后，无法动态更改
        /// </remarks>
        /// <example>
        /// 以下举例modbus-tcp的短连接及动态更改ip地址的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="IpAddressExample" title="IpAddress示例" />
        /// </example>
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
                        throw new Exception( StringResources.Language.IpAddresError );
                    }
                    ipAddress = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置服务器的端口号
        /// </summary>
        /// <remarks>
        /// 最好实在初始化的时候进行指定，当使用短连接的时候，支持动态更改，切换；当使用长连接后，无法动态更改
        /// </remarks>
        /// <example>
        /// 动态更改请参照IpAddress属性的更改。
        /// </example>
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
        /// 当前连接的唯一ID号，默认为长度20的guid码加随机数组成，方便列表管理，也可以自己指定
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
        /// <remarks>
        /// 具体的使用方法请参照Demo项目中的异形modbus实现。
        /// </remarks>
        public AlienSession AlienSession { get; set; }


        #endregion

        #region Public Method

        /// <summary>
        /// 在读取数据之前可以调用本方法将客户端设置为长连接模式，相当于跳过了ConnectServer的结果验证，对异形客户端无效
        /// </summary>
        /// <example>
        /// 以下的方式演示了另一种长连接的机制
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="SetPersistentConnectionExample" title="SetPersistentConnection示例" />
        /// </example>
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
        /// <example>
        ///   简单的连接示例，调用该方法后，连接设备，创建一个长连接的对象，后续的读写操作均公用一个连接对象。
        ///   <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="Connect1" title="连接设备" />
        ///   如果想知道是否连接成功，请参照下面的代码。
        ///   <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="Connect2" title="判断连接结果" />
        /// </example> 
        public OperateResult ConnectServer( )
        {
            isPersistentConn = true;
            OperateResult result = new OperateResult( );

            // 重新连接之前，先将旧的数据进行清空
            CoreSocket?.Close( );

            OperateResult<Socket> rSocket = CreateSocketAndInitialication( );

            if (!rSocket.IsSuccess)
            {
                IsSocketError = true;
                rSocket.Content = null;                 
                result.Message = rSocket.Message;
            }
            else
            {
                CoreSocket = rSocket.Content;
                result.IsSuccess = true;
                LogNet?.WriteDebug( ToString( ), StringResources.Language.NetEngineStart );
            }

            return result;
        }


        /// <summary>
        /// 使用指定的套接字创建异形客户端
        /// </summary>
        /// <param name="session">异形客户端对象，查看<seealso cref="NetworkAlienClient"/>类型创建的客户端</param>
        /// <returns>通常都为成功</returns>
        /// <example>
        ///   简单的创建示例。
        ///   <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="AlienConnect1" title="连接设备" />
        ///   如果想知道是否创建成功。通常都是成功。
        ///   <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="AlienConnect2" title="判断连接结果" />
        /// </example> 
        /// <remarks>
        /// 不能和之前的长连接和短连接混用，详细参考 Demo程序 
        /// </remarks>
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
        /// <example>
        /// 直接关闭连接即可，基本上是不需要进行成功的判定
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="ConnectCloseExample" title="关闭连接结果" />
        /// </example>
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
            
            LogNet?.WriteDebug( ToString( ), StringResources.Language.NetEngineClose );
            return result;
        }

        #endregion

        #region Initialization And Extra

        /// <summary>
        /// 连接上服务器后需要进行的初始化操作
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <returns>是否初始化成功，依据具体的协议进行重写</returns>
        /// <example>
        /// 有些协议不需要握手信号，比如三菱的MC协议，Modbus协议，西门子和欧姆龙就存在握手信息，此处的例子是继承本类后重写的西门子的协议示例
        /// <code lang="cs" source="HslCommunication_Net45\Profinet\Siemens\SiemensS7Net.cs" region="NetworkDoubleBase Override" title="西门子重连示例" />
        /// </example>
        protected virtual OperateResult InitializationOnConnect( Socket socket )
        {
            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 在将要和服务器进行断开的情况下额外的操作，需要根据对应协议进行重写
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <example>
        /// 目前暂无相关的示例，组件支持的协议都不用实现这个方法。
        /// </example>
        /// <returns>当断开连接时额外的操作结果</returns>
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
                        return new OperateResult<Socket>( StringResources.Language.ConnectionIsNotAvailable );
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
        /// <returns>带有socket的结果对象</returns>
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
        /// <remarks>
        /// 无锁的基于套接字直接进行叠加协议的操作。
        /// </remarks>
        /// <example>
        /// 假设你有一个自己的socket连接了设备，本组件可以直接基于该socket实现modbus读取，三菱读取，西门子读取等等操作，前提是该服务器支持多协议，虽然这个需求听上去比较变态，但本组件支持这样的操作。
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="ReadFromCoreServerExample1" title="ReadFromCoreServer示例" />
        /// </example>
        /// <returns>接收的完整的报文信息</returns>
        public virtual OperateResult<byte[]> ReadFromCoreServer( Socket socket, byte[] send )
        {
            OperateResult<byte[],byte[]> read = ReadFromCoreServerBase( socket, send );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( read );

            // 拼接结果数据
            byte[] Content = new byte[read.Content1.Length + read.Content2.Length];
            if (read.Content1.Length > 0) read.Content1.CopyTo( Content, 0 );
            if (read.Content2.Length > 0) read.Content2.CopyTo( Content, read.Content1.Length );

            return OperateResult.CreateSuccessResult( Content );
        }


        /// <summary>
        /// 使用底层的数据报文来通讯，传入需要发送的消息，返回一条完整的数据指令
        /// </summary>
        /// <param name="send">发送的完整的报文信息</param>
        /// <returns>接收的完整的报文信息</returns>
        /// <remarks>
        /// 本方法用于实现本组件还未实现的一些报文功能，例如有些modbus服务器会有一些特殊的功能码支持，需要收发特殊的报文，详细请看示例
        /// </remarks>
        /// <example>
        /// 此处举例有个modbus服务器，有个特殊的功能码0x09，后面携带子数据0x01即可，发送字节为 0x00 0x00 0x00 0x00 0x00 0x03 0x01 0x09 0x01
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="ReadFromCoreServerExample2" title="ReadFromCoreServer示例" />
        /// </example>
        public OperateResult<byte[]> ReadFromCoreServer( byte[] send )
        {
            var result = new OperateResult<byte[]>( );

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
                result.Message = StringResources.Language.SuccessText;
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
        /// <remarks>
        /// 当子类重写InitializationOnConnect方法和ExtraOnDisconnect方法时，需要和设备进行数据交互后，必须用本方法来数据交互，因为本方法是无锁的。
        /// </remarks>
        protected OperateResult<byte[], byte[]> ReadFromCoreServerBase(Socket socket, byte[] send )
        {
            LogNet?.WriteDebug( ToString( ), StringResources.Language.Send + " : " + BasicFramework.SoftBasic.ByteToHexString( send, ' ' ) );

            TNetMessage netMsg = new TNetMessage
            {
                SendBytes = send
            };

            // 发送数据信息
            OperateResult sendResult = Send( socket, send );
            if (!sendResult.IsSuccess)
            {
                socket?.Close( );
                return OperateResult.CreateFailedResult<byte[], byte[]>( sendResult );
            }

            // 接收超时时间大于0时才允许接收远程的数据
            if (receiveTimeOut >= 0)
            {
                // 接收数据信息
                OperateResult<TNetMessage> resultReceive = ReceiveMessage(socket, receiveTimeOut, netMsg);
                if (!resultReceive.IsSuccess)
                {
                    socket?.Close( );
                    return new OperateResult<byte[], byte[]>( StringResources.Language.ReceiveDataTimeout + receiveTimeOut );
                }

                LogNet?.WriteDebug( ToString( ), StringResources.Language.Receive + " : " +
                    BasicFramework.SoftBasic.ByteToHexString( BasicFramework.SoftBasic.SpliceTwoByteArray( resultReceive.Content.HeadBytes,
                    resultReceive.Content.ContentBytes ), ' ' ) );

                // Success
                return OperateResult.CreateSuccessResult( resultReceive.Content.HeadBytes, resultReceive.Content.ContentBytes );
            }
            else
            {
                // Not need receive
                return OperateResult.CreateSuccessResult( new byte[0], new byte[0] );
            }
        }


        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString( )
        {
            return $"NetworkDoubleBase<{typeof(TNetMessage)},{typeof(TTransform)}>";
        }


        #endregion
    }


}
