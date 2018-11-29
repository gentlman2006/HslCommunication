using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.LogNet;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;
using HslCommunication.Core.IMessage;

#if (NET45 || NETSTANDARD2_0)
using System.Threading.Tasks;
#endif

/*************************************************************************************
 * 
 *    说明：
 *    本组件的所有网络类的基类。提供了一些基础的操作实现，部分实现需要集成实现
 *    
 *    重构日期：2018年3月8日 21:22:05
 * 
 *************************************************************************************/





namespace HslCommunication.Core.Net
{
    /// <summary>
    /// 本系统所有网络类的基类，该类为抽象类，无法进行实例化
    /// </summary>
    /// <remarks>
    /// network base class, support basic operation with socket
    /// </remarks>
    public abstract class NetworkBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个NetworkBase对象
        /// </summary>
        /// <remarks>
        /// 令牌的默认值为空，都是0x00
        /// </remarks>
        public NetworkBase( )
        {
            Token = Guid.Empty;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 组件的日志工具，支持日志记录
        /// </summary>
        /// <remarks>
        /// 只要实例化即可以记录日志，实例化的对象需要实现接口 <see cref="ILogNet"/> ，本组件提供了三个日志记录类，你可以实现基于 <see cref="ILogNet"/>  的对象。</remarks>
        /// <example>
        /// 如下的实例化适用于所有的Network及其派生类，以下举两个例子，三菱的设备类及服务器类
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkBase.cs" region="LogNetExample1" title="LogNet示例" />
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkBase.cs" region="LogNetExample2" title="LogNet示例" />
        /// </example>
        public ILogNet LogNet { get; set; }

        /// <summary>
        /// 网络类的身份令牌
        /// </summary>
        /// <remarks>
        /// 适用于Hsl协议相关的网络通信类，不适用于设备交互类。
        /// </remarks>
        /// <example>
        /// 此处以 <see cref="Enthernet.NetSimplifyServer"/> 服务器类及 <see cref="Enthernet.NetSimplifyClient"/> 客户端类的令牌设置举例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkBase.cs" region="TokenClientExample" title="Client示例" />
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkBase.cs" region="TokenServerExample" title="Server示例" />
        /// </example>
        public Guid Token { get; set; }

        #endregion

        #region Potect Member

        /// <summary>
        /// 通讯类的核心套接字
        /// </summary>
        protected Socket CoreSocket = null;


        #endregion

        #region Protect Method


        /// <summary>
        /// 检查网络套接字是否操作超时，需要对套接字进行封装
        /// </summary>
        /// <param name="obj">通常是 <see cref="HslTimeOut"/> 对象 </param>
        protected void ThreadPoolCheckTimeOut( object obj )
        {
            if (obj is HslTimeOut timeout)
            {
                while (!timeout.IsSuccessful)
                {
                    Thread.Sleep( 100 );
                    if ((DateTime.Now - timeout.StartTime).TotalMilliseconds > timeout.DelayTime)
                    {
                        // 连接超时或是验证超时
                        if (!timeout.IsSuccessful)
                        {
                            LogNet?.WriteWarn( ToString( ), "Wait Time Out : " + timeout.DelayTime );
                            timeout.Operator?.Invoke( );
                            timeout.WorkSocket?.Close( );
                        }
                        break;
                    }
                }
            }
        }



        #endregion

        /*****************************************************************************
         * 
         *    说明：
         *    下面的三个模块代码指示了如何接收数据，如何发送数据，如何连接网络
         * 
         ********************************************************************************/

        #region Reveive Content

        /// <summary>
        /// 接收固定长度的字节数组
        /// </summary>
        /// <remarks>
        /// Receive Special Length Bytes
        /// </remarks>
        /// <param name="socket">网络通讯的套接字</param>
        /// <param name="length">准备接收的数据长度</param>
        /// <returns>包含了字节数据的结果类</returns>
        protected OperateResult<byte[]> Receive( Socket socket, int length )
        {
            if (length == 0) return OperateResult.CreateSuccessResult( new byte[0] );

            //try
            //{
            //    return OperateResult.CreateSuccessResult( NetSupport.ReadBytesFromSocket( socket, length ) );
            //}
            //catch(Exception ex)
            //{
            //    return new OperateResult<byte[]>( ex.Message );
            //}

//#if NET35

            var result = new OperateResult<byte[]>( );
            var receiveDone = new ManualResetEvent( false );
            AutoResetEvent autoReset = new AutoResetEvent( false );
            var state = new StateObject( length );
            
            try
            {
                state.WaitDone = receiveDone;
                state.WorkSocket = socket;

                // Begin receiving the data from the remote device.
                socket.BeginReceive( state.Buffer, state.AlreadyDealLength,
                    state.DataLength - state.AlreadyDealLength, SocketFlags.None,
                    new AsyncCallback( ReceiveCallback ), state );
            }
            catch (Exception ex)
            {
                // 发生了错误，直接返回
                LogNet?.WriteException( ToString( ), ex );
                result.Message = ex.Message;
                receiveDone.Close( );
                socket?.Close( );
                return result;
            }



            // 等待接收完成，或是发生异常
            receiveDone.WaitOne( );
            receiveDone.Close( );



            // 接收数据失败
            if (state.IsError)
            {
                socket?.Close( );
                result.Message = state.ErrerMsg;
                return result;
            }


            // 远程关闭了连接
            if (state.IsClose)
            {
                // result.IsSuccess = true;
                result.Message = StringResources.Language.RemoteClosedConnection;
                socket?.Close( );
                return result;
            }


            // 正常接收到数据
            result.Content = state.Buffer;
            result.IsSuccess = true;
            state.Clear( );
            state = null;
            return result;

//#else
            //SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs( );
            //byte[] buffer = new byte[length];
            //eventArgs.SetBuffer( buffer, 0, length );
            //int receiveCount = 0;
            //while (true)
            //{
            //    socket.ReceiveAsync( eventArgs );
            //    receiveCount += eventArgs.BytesTransferred;
            //    if (receiveCount == length) break;
            //}
            //return OperateResult.CreateSuccessResult( buffer );
//#endif
        }


        private void ReceiveCallback( IAsyncResult ar )
        {
            if (ar.AsyncState is StateObject state)
            {
                try
                {
                    Socket client = state.WorkSocket;
                    int bytesRead = client.EndReceive( ar );

                    if (bytesRead > 0)
                    {
                        // 接收到了数据
                        state.AlreadyDealLength += bytesRead;
                        if (state.AlreadyDealLength < state.DataLength)
                        {
                            // 获取接下来的所有的数据
                            client.BeginReceive( state.Buffer, state.AlreadyDealLength,
                                state.DataLength - state.AlreadyDealLength, SocketFlags.None,
                                new AsyncCallback( ReceiveCallback ), state );
                        }
                        else
                        {
                            // 接收到了所有的数据，通知接收数据的线程继续
                            state.WaitDone.Set( );
                        }
                    }
                    else
                    {
                        // 对方关闭了网络通讯
                        state.IsClose = true;
                        state.WaitDone.Set( );
                    }
                }
                catch (Exception ex)
                {
                    state.IsError = true;
                    LogNet?.WriteException( ToString( ), ex );
                    state.ErrerMsg = ex.Message;
                    state.WaitDone.Set( );
                }
            }
        }


        #endregion

        #region Receive Message
        
        /// <summary>
        /// 接收一条完整的数据，使用异步接收完成，包含了指令头信息
        /// </summary>
        /// <param name="socket">已经打开的网络套接字</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="netMsg">消息规则</param>
        /// <returns>数据的接收结果对象</returns>
        protected OperateResult<TNetMessage> ReceiveMessage<TNetMessage>( Socket socket, int timeOut, TNetMessage netMsg ) where TNetMessage : INetMessage
        {
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
                LogNet?.WriteError( ToString( ), StringResources.Language.TokenCheckFailed );
                result.Message = StringResources.Language.TokenCheckFailed;
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
                if (!contentResult.IsSuccess)
                {
                    hslTimeOut.IsSuccessful = true;
                    result.CopyErrorFromOther( contentResult );
                    return result;
                }

                netMsg.ContentBytes = contentResult.Content;
            }

            // 防止没有实例化造成后续的操作失败
            if (netMsg.ContentBytes == null) netMsg.ContentBytes = new byte[0];
            hslTimeOut.IsSuccessful = true;
            result.Content = netMsg;
            result.IsSuccess = true;
            return result;
        }
        

        #endregion

        #region Send Content

        /// <summary>
        /// 发送消息给套接字，直到完成的时候返回
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="data">字节数据</param>
        /// <returns>发送是否成功的结果</returns>
        protected OperateResult Send( Socket socket, byte[] data )
        {
            if (data == null) return OperateResult.CreateSuccessResult( );

            var result = new OperateResult( );
            var sendDone = new ManualResetEvent( false );
            var state = new StateObject( data.Length );

            try
            {
                state.WaitDone = sendDone;
                state.WorkSocket = socket;
                state.Buffer = data;

                socket.BeginSend( state.Buffer, state.AlreadyDealLength, state.DataLength - state.AlreadyDealLength,
                    SocketFlags.None, new AsyncCallback( SendCallBack ), state );
            }
            catch (Exception ex)
            {
                // 发生了错误，直接返回
                LogNet?.WriteException( ToString( ), ex );
                result.Message = ex.Message;
                socket?.Close( );
                sendDone.Close( );
                return result;
            }

            // 等待发送完成
            sendDone.WaitOne( );
            sendDone.Close( );

            if (state.IsError)
            {
                socket.Close( );
                result.Message = state.ErrerMsg;
                return result;
            }

            state.Clear( );
            state = null;
            result.IsSuccess = true;
            result.Message = StringResources.Language.SuccessText;

            return result;
        }

        /// <summary>
        /// 发送数据异步返回的方法
        /// </summary>
        /// <param name="ar">异步对象</param>
        private void SendCallBack( IAsyncResult ar )
        {
            if (ar.AsyncState is StateObject state)
            {
                try
                {
                    Socket socket = state.WorkSocket;
                    int byteSend = socket.EndSend( ar );
                    state.AlreadyDealLength += byteSend;

                    if (state.AlreadyDealLength < state.DataLength)
                    {
                        // 继续发送数据
                        socket.BeginSend( state.Buffer, state.AlreadyDealLength, state.DataLength - state.AlreadyDealLength,
                            SocketFlags.None, new AsyncCallback( SendCallBack ), state );
                    }
                    else
                    {
                        // 发送完成
                        state.WaitDone.Set( );
                    }
                }
                catch (Exception ex)
                {
                    // 发生了异常
                    state.IsError = true;
                    LogNet?.WriteException( ToString( ), ex );
                    state.ErrerMsg = ex.Message;
                    state.WaitDone.Set( );
                }
            }
        }


        #endregion

        #region Socket Connect

        /// <summary>
        /// 创建一个新的socket对象并连接到远程的地址，默认超时时间为10秒钟
        /// </summary>
        /// <param name="ipAddress">Ip地址</param>
        /// <param name="port">端口号</param>
        /// <returns>返回套接字的封装结果对象</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkBase.cs" region="CreateSocketAndConnectExample" title="创建连接示例" />
        /// </example>
        protected OperateResult<Socket> CreateSocketAndConnect( string ipAddress, int port )
        {
            return CreateSocketAndConnect( new IPEndPoint( IPAddress.Parse( ipAddress ), port ), 10000 );
        }


        /// <summary>
        /// 创建一个新的socket对象并连接到远程的地址
        /// </summary>
        /// <param name="ipAddress">Ip地址</param>
        /// <param name="port">端口号</param>
        /// <param name="timeOut">连接的超时时间</param>
        /// <returns>返回套接字的封装结果对象</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkBase.cs" region="CreateSocketAndConnectExample" title="创建连接示例" />
        /// </example>
        protected OperateResult<Socket> CreateSocketAndConnect( string ipAddress, int port, int timeOut )
        {
            return CreateSocketAndConnect( new IPEndPoint( IPAddress.Parse( ipAddress ), port ), timeOut );
        }


        /// <summary>
        /// 创建一个新的socket对象并连接到远程的地址
        /// </summary>
        /// <param name="endPoint">连接的目标终结点</param>
        /// <param name="timeOut">连接的超时时间</param>
        /// <returns>返回套接字的封装结果对象</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkBase.cs" region="CreateSocketAndConnectExample" title="创建连接示例" />
        /// </example>
        protected OperateResult<Socket> CreateSocketAndConnect( IPEndPoint endPoint, int timeOut )
        {
            var result = new OperateResult<Socket>( );
            var connectDone = new ManualResetEvent( false );
            var state = new StateObject( );
            var socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

            // 超时验证的信息
            HslTimeOut connectTimeout = new HslTimeOut( )
            {
                WorkSocket = socket,
                DelayTime = timeOut
            };
            ThreadPool.QueueUserWorkItem( new WaitCallback( ThreadPoolCheckTimeOut ), connectTimeout );

            try
            {
                state.WaitDone = connectDone;
                state.WorkSocket = socket;
                socket.BeginConnect( endPoint, new AsyncCallback( ConnectCallBack ), state );
            }
            catch (Exception ex)
            {
                // 直接失败
                connectTimeout.IsSuccessful = true;                                  // 退出线程池的超时检查
                LogNet?.WriteException( ToString( ), ex );                           // 记录错误日志
                socket.Close( );                                                     // 关闭网络信息
                connectDone.Close( );                                                // 释放等待资源
                result.Message = StringResources.Language.ConnectedFailed + ex.Message;       // 传递错误消息
                return result;
            }
            
            

            // 等待连接完成
            connectDone.WaitOne( );
            connectDone.Close( );
            connectTimeout.IsSuccessful = true;

            if (state.IsError)
            {
                // 连接失败
                result.Message = StringResources.Language.ConnectedFailed + state.ErrerMsg;
                socket?.Close( );
                return result;
            }


            result.Content = socket;
            result.IsSuccess = true;
            state.Clear( );
            state = null;
            return result;
        }


        /// <summary>
        /// 当连接的结果返回
        /// </summary>
        /// <param name="ar">异步对象</param>
        private void ConnectCallBack( IAsyncResult ar )
        {
            if (ar.AsyncState is StateObject state)
            {
                try
                {
                    Socket socket = state.WorkSocket;
                    socket.EndConnect( ar );
                    state.WaitDone.Set( );
                }
                catch (Exception ex)
                {
                    // 发生了异常
                    state.IsError = true;
                    LogNet?.WriteException( ToString( ), ex );
                    state.ErrerMsg = ex.Message;
                    state.WaitDone.Set( );
                }
            }
        }


        #endregion


        /*****************************************************************************
         * 
         *    说明：
         *    下面的两个模块代码指示了如何读写文件
         * 
         ********************************************************************************/

        #region Read Stream


        /// <summary>
        /// 读取流中的数据到缓存区
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="buffer">缓冲区</param>
        /// <returns>带有成功标志的读取数据长度</returns>
        protected OperateResult<int> ReadStream( Stream stream, byte[] buffer)
        {
            ManualResetEvent WaitDone = new ManualResetEvent( false );
            FileStateObject stateObject = new FileStateObject
            {
                WaitDone = WaitDone,
                Stream = stream,
                DataLength = buffer.Length,
                Buffer = buffer
            };

            try
            {
                stream.BeginRead( buffer, 0, stateObject.DataLength, new AsyncCallback( ReadStreamCallBack ), stateObject );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), ex );
                stateObject = null;
                WaitDone.Close( );
                return new OperateResult<int>( );
            }

            WaitDone.WaitOne( );
            WaitDone.Close( );
            if (stateObject.IsError)
            {
                return new OperateResult<int>( )
                {
                    Message = stateObject.ErrerMsg
                };
            }
            else
            {
                return OperateResult.CreateSuccessResult( stateObject.AlreadyDealLength );
            }
        }


        private void ReadStreamCallBack( IAsyncResult ar )
        {
            if (ar.AsyncState is FileStateObject stateObject)
            {
                try
                {
                    stateObject.AlreadyDealLength += stateObject.Stream.EndRead( ar );
                    stateObject.WaitDone.Set( );
                }
                catch (Exception ex)
                {
                    LogNet?.WriteException( ToString( ), ex );
                    stateObject.IsError = true;
                    stateObject.ErrerMsg = ex.Message;
                    stateObject.WaitDone.Set( );
                }
            }
        }


        #endregion

        #region Write Stream

        /// <summary>
        /// 将缓冲区的数据写入到流里面去
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="buffer">缓冲区</param>
        /// <returns>是否写入成功</returns>
        protected OperateResult WriteStream( Stream stream, byte[] buffer )
        {
            ManualResetEvent WaitDone = new ManualResetEvent( false );
            FileStateObject stateObject = new FileStateObject
            {
                WaitDone = WaitDone,
                Stream = stream
            };

            try
            {
                stream.BeginWrite( buffer, 0, buffer.Length, new AsyncCallback( WriteStreamCallBack ), stateObject );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), ex );
                stateObject = null;
                WaitDone.Close( );
                return new OperateResult( );
            }

            WaitDone.WaitOne( );
            WaitDone.Close( );
            if (stateObject.IsError)
            {
                return new OperateResult( )
                {
                    Message = stateObject.ErrerMsg
                };
            }
            else
            {
                return OperateResult.CreateSuccessResult( );
            }


        }

        private void WriteStreamCallBack( IAsyncResult ar )
        {
            if (ar.AsyncState is FileStateObject stateObject)
            {
                try
                {
                    stateObject.Stream.EndWrite( ar );
                }
                catch (Exception ex)
                {
                    LogNet?.WriteException( ToString( ), ex );
                    stateObject.IsError = true;
                    stateObject.ErrerMsg = ex.Message;
                }
                finally
                {
                    stateObject.WaitDone.Set( );
                }
            }
        }

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString( )
        {
            return "NetworkBase";
        }

        #endregion

    }

    
}
