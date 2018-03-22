using HslCommunication.Core;
using HslCommunication.Core.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HslCommunication.Enthernet
{ 
    /// <summary>
    /// 高性能的异步网络服务器类，适合搭建局域网聊天程序，消息推送程序
    /// </summary>
    public class NetComplexServer : NetworkServerBase
    {
        #region 构造方法块

        /// <summary>
        /// 实例化一个网络服务器类对象
        /// </summary>
        public NetComplexServer( )
        {
            AsyncCoordinator = new HslAsyncCoordinator( new Action( CalculateOnlineClients ) );
        }


        #endregion

        #region 基本属性块


        private int m_Connect_Max = 1000;


        /// <summary>
        /// 所支持的同时在线客户端的最大数量，商用限制1000个，最小10个
        /// </summary>
        public int ConnectMax
        {
            get { return m_Connect_Max; }
            set
            {
                if (value >= 10 && value < 1001)
                {
                    m_Connect_Max = value;
                }
            }
        }

        /// <summary>
        /// 客户端在线信息显示的格式化文本，如果自定义，必须#开头，
        /// 示例："#IP:{0} Name:{1}"
        /// </summary>
        public string FormatClientOnline { get; set; } = "#IP:{0} Name:{1}";


        /// <summary>
        /// 客户端在线信息缓存
        /// </summary>
        private string m_AllClients = string.Empty;


        #region 高性能乐观并发模型的上下线控制

        private void CalculateOnlineClients( )
        {
            StringBuilder builder = new StringBuilder( );

            HybirdLockSockets.Enter( );
            for (int i = 0; i < All_sockets_connect.Count; i++)
            {

                builder.Append( string.Format( FormatClientOnline, All_sockets_connect[i].IpAddress
                    , All_sockets_connect[i].LoginAlias ) );
            }
            HybirdLockSockets.Leave( );


            if (builder.Length > 0)
            {
                m_AllClients = builder.Remove( 0, 1 ).ToString( );
            }
            else
            {
                m_AllClients = string.Empty;
            }
            // 触发状态变更
            AllClientsStatusChange?.Invoke( m_AllClients );
        }

        /// <summary>
        /// 一个计算上线下线的高性能缓存对象
        /// </summary>
        private HslAsyncCoordinator AsyncCoordinator { get; set; }




        #endregion





        /// <summary>
        /// 计算所有客户端在线的信息
        /// </summary>

        /// <summary>
        /// 获取或设置服务器是否记录客户端上下线信息
        /// </summary>
        public bool IsSaveLogClientLineChange { get; set; } = true;
        /// <summary>
        /// 所有在线客户端的数量
        /// </summary>
        public int ClientCount => All_sockets_connect.Count;

        /// <summary>
        /// 所有的客户端连接的核心对象
        /// </summary>
        private List<AppSession> All_sockets_connect { get; set; } = new List<AppSession>( );


        /// <summary>
        /// 客户端数组操作的线程混合锁
        /// </summary>
        private SimpleHybirdLock HybirdLockSockets = new SimpleHybirdLock( );

        #endregion

        #region 启动停止块


        /// <summary>
        /// 初始化操作
        /// </summary>
        protected override void StartInitialization( )
        {
            Thread_heart_check = new Thread( new ThreadStart( ThreadHeartCheck ) )
            {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };
            Thread_heart_check.Start( );
            base.StartInitialization( );
        }

        /// <summary>
        /// 关闭网络时的操作
        /// </summary>
        protected override void CloseAction( )
        {
            Thread_heart_check?.Abort( );
            ClientOffline = null;
            ClientOnline = null;
            AcceptString = null;
            AcceptByte = null;

            //关闭所有的网络
            All_sockets_connect.ForEach( m => m.WorkSocket?.Close( ) );
            base.CloseAction( );
        }



        #endregion

        #region 客户端上下线块

        private void TcpStateUpLine( AppSession state )
        {
            HybirdLockSockets.Enter( );
            All_sockets_connect.Add( state );
            HybirdLockSockets.Leave( );

            // 提示上线
            ClientOnline?.Invoke( state );
            // 是否保存上线信息
            if (IsSaveLogClientLineChange)
            {
                LogNet?.WriteInfo( ToString(), "IP:" + state.IpAddress + " Name:" + state?.LoginAlias + " " + StringResources.NetClientOnline );
            }
            // 计算客户端在线情况
            AsyncCoordinator.StartOperaterInfomation( );
        }

        private void TcpStateClose( AppSession state )
        {
            state?.WorkSocket.Close( );
        }

        private void TcpStateDownLine( AppSession state, bool is_regular )
        {
            HybirdLockSockets.Enter( );
            All_sockets_connect.Remove( state );
            HybirdLockSockets.Leave( );
            // 关闭连接
            TcpStateClose( state );
            // 判断是否正常下线
            string str = is_regular ? StringResources.NetClientOffline : StringResources.NetClientBreak;
            ClientOffline?.Invoke( state, str );
            // 是否保存上线信息
            if (IsSaveLogClientLineChange)
            {
                LogNet?.WriteInfo( ToString(), "IP:" + state.IpAddress + " Name:" + state?.LoginAlias + " " + str );
            }
            // 计算客户端在线情况
            AsyncCoordinator.StartOperaterInfomation( );
        }

        #endregion

        #region 事件委托块


        /// <summary>
        /// 客户端的上下限状态变更时触发，仅作为在线客户端识别
        /// </summary>
        public event Action<string> AllClientsStatusChange;

        /// <summary>
        /// 当客户端上线的时候，触发此事件
        /// </summary>
        public event Action<AppSession> ClientOnline;
        /// <summary>
        /// 当客户端下线的时候，触发此事件
        /// </summary>
        public event Action<AppSession, string> ClientOffline;
        /// <summary>
        /// 当接收到文本数据的时候,触发此事件
        /// </summary>
        public event Action<AppSession, NetHandle, string> AcceptString;
        /// <summary>
        /// 当接收到字节数据的时候,触发此事件
        /// </summary>
        public event Action<AppSession, NetHandle, byte[]> AcceptByte;




        #endregion

        #region 请求接入块
        /// <summary>
        /// 登录后的处理方法
        /// </summary>
        /// <param name="obj"></param>
        protected override void ThreadPoolLogin( object obj )
        {
            if (obj is Socket socket)
            {
                // 判断连接数是否超出规定
                if (All_sockets_connect.Count > ConnectMax)
                {
                    socket?.Close( );
                    LogNet?.WriteWarn( ToString(), StringResources.NetClientFull );
                    return;
                }

                // 接收用户别名并验证令牌
                OperateResult result = new OperateResult( );
                OperateResult<int, string> readResult = ReceiveStringContentFromSocket( socket );
                if (!readResult.IsSuccess)
                {
                    socket?.Close( );
                    return;
                }



                // 登录成功
                AppSession session = new AppSession( )
                {
                    WorkSocket = socket,
                    LoginAlias = readResult.Content2,
                };


                try
                {
                    session.IpEndPoint = (IPEndPoint)socket.RemoteEndPoint;
                    session.IpAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString( );
                }
                catch(Exception ex)
                {
                    LogNet?.WriteException( ToString( ), "客户端地址获取失败：", ex );
                }

                if (readResult.Content1 == 1)
                {
                    // 电脑端客户端
                    session.ClientType = "Windows";
                }
                else if (readResult.Content1 == 2)
                {
                    // Android 客户端
                    session.ClientType = "Android";
                }


                try
                {
                    session.WorkSocket.BeginReceive( session.BytesHead, session.AlreadyReceivedHead,
                        session.BytesHead.Length - session.AlreadyReceivedHead, SocketFlags.None,
                        new AsyncCallback( HeadBytesReceiveCallback ), session );
                    TcpStateUpLine( session );
                    Thread.Sleep( 500 );//留下一些时间进行反应
                }
                catch (Exception ex)
                {
                    //登录前已经出错
                    TcpStateClose( session );
                    LogNet?.WriteException( ToString(), StringResources.NetClientLoginFailed, ex );
                }
            }
        }



        #endregion

        #region 异步接收发送块

        /// <summary>
        /// 异常下线
        /// </summary>
        /// <param name="session"></param>
        /// <param name="ex"></param>
        internal override void SocketReceiveException( AppSession session, Exception ex )
        {
            if (ex.Message.Contains( StringResources.SocketRemoteCloseException ))
            {
                //异常掉线
                TcpStateDownLine( session, false );
            }
        }


        /// <summary>
        /// 正常下线
        /// </summary>
        /// <param name="session"></param>
        internal override void AppSessionRemoteClose( AppSession session )
        {
            TcpStateDownLine( session, true );
        }





        /// <summary>
        /// 服务器端用于数据发送文本的方法
        /// </summary>
        /// <param name="stateone">数据发送对象</param>
        /// <param name="customer">用户自定义的数据对象，如不需要，赋值为0</param>
        /// <param name="str">发送的文本</param>
        public void Send( AppSession stateone, NetHandle customer, string str )
        {
            SendBytes( stateone, NetSupport.CommandBytes( customer, Token, str ) );
        }
        /// <summary>
        /// 服务器端用于发送字节的方法
        /// </summary>
        /// <param name="stateone">数据发送对象</param>
        /// <param name="customer">用户自定义的数据对象，如不需要，赋值为0</param>
        /// <param name="bytes">实际发送的数据</param>
        public void Send( AppSession stateone, NetHandle customer, byte[] bytes )
        {
            SendBytes( stateone, NetSupport.CommandBytes( customer, Token, bytes ) );
        }

        private void SendBytes( AppSession stateone, byte[] content )
        {
            SendBytesAsync( stateone, content );
        }


        /// <summary>
        /// 服务端用于发送所有数据到所有的客户端
        /// </summary>
        /// <param name="customer">用户自定义的命令头</param>
        /// <param name="str">需要传送的实际的数据</param>
        public void SendAllClients( NetHandle customer, string str )
        {
            for (int i = 0; i < All_sockets_connect.Count; i++)
            {
                Send( All_sockets_connect[i], customer, str );
            }
        }

        /// <summary>
        /// 服务端用于发送所有数据到所有的客户端
        /// </summary>
        /// <param name="customer">用户自定义的命令头</param>
        /// <param name="data">需要群发客户端的字节数据</param>
        public void SendAllClients( NetHandle customer, byte[] data )
        {
            for (int i = 0; i < All_sockets_connect.Count; i++)
            {
                Send( All_sockets_connect[i], customer, data );
            }
        }

        /// <summary>
        /// 根据客户端设置的别名进行发送消息
        /// </summary>
        /// <param name="Alias">客户端上线的别名</param>
        /// <param name="customer">用户自定义的命令头</param>
        /// <param name="str">需要传送的实际的数据</param>
        public void SendClientByAlias( string Alias, NetHandle customer, string str )
        {
            for (int i = 0; i < All_sockets_connect.Count; i++)
            {
                if (All_sockets_connect[i].LoginAlias == Alias)
                {
                    Send( All_sockets_connect[i], customer, str );
                }
            }
        }


        /// <summary>
        /// 根据客户端设置的别名进行发送消息
        /// </summary>
        /// <param name="Alias">客户端上线的别名</param>
        /// <param name="customer">用户自定义的命令头</param>
        /// <param name="data">需要传送的实际的数据</param>
        public void SendClientByAlias( string Alias, NetHandle customer, byte[] data )
        {
            for (int i = 0; i < All_sockets_connect.Count; i++)
            {
                if (All_sockets_connect[i].LoginAlias == Alias)
                {
                    Send( All_sockets_connect[i], customer, data );
                }
            }
        }


        #endregion

        #region 数据中心处理块

        /// <summary>
        /// 数据处理中心
        /// </summary>
        /// <param name="session"></param>
        /// <param name="protocol"></param>
        /// <param name="customer"></param>
        /// <param name="content"></param>
        internal override void DataProcessingCenter( AppSession session, int protocol, int customer, byte[] content )
        {
            if (protocol == HslProtocol.ProtocolCheckSecends)
            {
                BitConverter.GetBytes( DateTime.Now.Ticks ).CopyTo( content, 8 );
                SendBytes( session, NetSupport.CommandBytes( HslProtocol.ProtocolCheckSecends, customer, Token, content ) );
                session.HeartTime = DateTime.Now;
            }
            else if (protocol == HslProtocol.ProtocolClientQuit)
            {
                TcpStateDownLine( session, true );
            }
            else if (protocol == HslProtocol.ProtocolUserBytes)
            {
                //接收到字节数据
                AcceptByte?.Invoke( session, customer, content );
            }
            else if (protocol == HslProtocol.ProtocolUserString)
            {
                //接收到文本数据
                string str = Encoding.Unicode.GetString( content );
                AcceptString?.Invoke( session, customer, str );
            }
            else
            {
                // 其他一概不处理
            }
        }



        #endregion

        #region 心跳线程块

        private Thread Thread_heart_check { get; set; } = null;

        private void ThreadHeartCheck( )
        {
            while (true)
            {
                Thread.Sleep( 2000 );

                try
                {
                    for (int i = All_sockets_connect.Count - 1; i >= 0; i--)
                    {
                        if (All_sockets_connect[i] == null)
                        {
                            All_sockets_connect.RemoveAt( i );
                            continue;
                        }

                        if ((DateTime.Now - All_sockets_connect[i].HeartTime).TotalSeconds > 1 * 8)//8次没有收到失去联系
                        {
                            LogNet?.WriteWarn( ToString(), "心跳验证超时，强制下线：" + All_sockets_connect[i].IpAddress.ToString( ) );
                            TcpStateDownLine( All_sockets_connect[i], false );
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogNet?.WriteException( ToString(), "心跳线程异常：", ex );
                }


                if (!IsStarted) break;
            }
        }



        #endregion

        #region Object Override

        /// <summary>
        /// 获取本对象的字符串表示形式
        /// </summary>
        /// <returns></returns>
        public override string ToString( )
        {
            return "NetComplexServer";
        }

        #endregion

    }
}
