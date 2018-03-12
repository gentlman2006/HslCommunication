using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Net;

namespace HslCommunication.Core
{


    /// <summary>
    /// 支持长连接，短连接两个模式的通用客户端基类
    /// </summary>
    public class NetworkDoubleBase<TNetMessage> : NetworkBase where TNetMessage : INetMessage, new()
    {
        #region Constructor
        
        /// <summary>
        /// 默认的无参构造函数
        /// </summary>
        public NetworkDoubleBase()
        {
            InteractiveLock = new SimpleHybirdLock();
        }

        #endregion

        #region Private Member

        /// <summary>
        /// IP地址
        /// </summary>
        protected string ipAddress = "127.0.0.1";
        /// <summary>
        /// 网络的端口
        /// </summary>
        protected int port = 10000;


        private int connectTimeOut = 10000;              // 连接超时时间设置
        private int receiveTimeOut = 10000;              // 数据接收的超时时间
        private bool IsPersistentConn = false;           // 是否处于长连接的状态
        private SimpleHybirdLock InteractiveLock;        // 一次正常的交互的互斥锁
        private bool IsSocketErrorState = false;         // 指示长连接的套接字是否处于错误的状态

        #endregion

        #region Public Member

        /// <summary>
        /// 获取或设置当前通信是否为长连接，如果是长连接，返回<c>True</c>，否则返回<c>False</c>
        /// </summary>
        public bool PersistentConnection
        {
            get
            {
                return IsPersistentConn;
            }
            set
            {
                if (IsPersistentConn != value)
                {
                    IsPersistentConn = value;
                    if(!IsPersistentConn)
                    {
                        CoreSocket?.Close();
                    }
                }
            }
        }


        #endregion

        #region Receive Message

        /// <summary>
        /// 接收一条完整的数据，使用异步接收完成，包含了指令头信息
        /// </summary>
        /// <param name="socket">已经打开的网络套接字</param>
        /// <returns>数据的接收结果对象</returns>
        protected OperateResult<TNetMessage> ReceiveMessage(Socket socket)
        {
            TNetMessage netMsg = new TNetMessage();
            OperateResult<TNetMessage> result = new OperateResult<TNetMessage>();

            // 接收指令头
            OperateResult<byte[]> headResult = Receive(socket, netMsg.ProtocolHeadBytesLength);
            if (!headResult.IsSuccess)
            {
                result.CopyErrorFromOther(headResult);
                return result;
            }

            netMsg.HeadBytes = headResult.Content;
            if (!netMsg.CheckHeadBytesLegal(Token.ToByteArray()))
            {
                // 令牌校验失败
                socket?.Close();
                LogNet?.WriteError(ToString(), StringResources.TokenCheckFailed);
                result.Message = StringResources.TokenCheckFailed;
                return result;
            }


            int contentLength = netMsg.GetContentLengthByHeadBytes();
            if (contentLength == 0)
            {
                netMsg.ContentBytes = new byte[0];
            }
            else
            {
                OperateResult<byte[]> contentResult = Receive(socket, contentLength);
                if (!headResult.IsSuccess)
                {
                    result.CopyErrorFromOther(contentResult);
                    return result;
                }

                netMsg.ContentBytes = contentResult.Content;
            }

            result.Content = netMsg;
            result.IsSuccess = true;
            return result;
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


        private OperateResult<Socket> GetAvailableSocket()
        {
            if (IsPersistentConn)
            {
                // 长连接模式
                if (IsSocketErrorState)
                {
                    // 上次通讯异常或是没有打开
                    IsSocketErrorState = false;
                    OperateResult<Socket> resultSocket = CreateSocketAndConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port), connectTimeOut);
                    if (resultSocket.IsSuccess)
                    {
                        IsSocketErrorState = true;
                        CoreSocket?.Close( );
                        CoreSocket = resultSocket.Content;
                    }
                    return resultSocket;
                }
                else
                {
                    return OperateResult.CreateSuccessResult( CoreSocket );
                }
            }
            else
            {
                // 短连接模式
                return CreateSocketAndConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port), connectTimeOut);
            }
        }


        /// <summary>
        /// 使用底层的数据报文来通讯，传入需要发送的消息，返回最终的数据结果
        /// </summary>
        /// <param name="send">发送的数据</param>
        /// <returns>结果对象</returns>
        public OperateResult<byte[],byte[]> ReadFromCoreServer(byte[] send)
        {
            var result = new OperateResult<byte[], byte[]>();
            // LogNet?.WriteDebug( ToString( ), "Command: " + BasicFramework.SoftBasic.ByteToHexString( send ) );
            InteractiveLock.Enter( );

            // 获取有用的网络通道，如果没有，就建立新的连接
            OperateResult<Socket> resultSocket = GetAvailableSocket( );
            if (!resultSocket.IsSuccess)
            {
                IsSocketErrorState = false;
                InteractiveLock.Leave( );
                result.CopyErrorFromOther( resultSocket );
                return result;
            }

            // 发送数据信息
            OperateResult resultSend = Send( resultSocket.Content, send );
            if(!resultSend.IsSuccess)
            {
                IsSocketErrorState = false;
                InteractiveLock.Leave( );
                resultSocket.Content?.Close( );
                result.CopyErrorFromOther( resultSend );
                return result;
            }

            // 接收超时时间大于0时才允许接收远程的数据
            if (receiveTimeOut >= 0)
            {
                // 接收数据信息
                OperateResult<TNetMessage> resultReceive = ReceiveMessage(resultSocket.Content);
                if (!resultReceive.IsSuccess)
                {
                    IsSocketErrorState = false;
                    InteractiveLock.Leave();
                    resultSocket.Content?.Close();
                    result.CopyErrorFromOther(resultReceive);
                    return result;
                }

                // 复制结果
                result.Content1 = resultReceive.Content.HeadBytes;
                result.Content2 = resultReceive.Content.ContentBytes;
            }

            if (!IsPersistentConn) resultSocket.Content?.Close( );
            InteractiveLock.Leave( );

            IsSocketErrorState = true;
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

    }


}
