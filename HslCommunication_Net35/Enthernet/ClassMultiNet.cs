using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;

namespace HslCommunication.Enthernet
{
    /// <summary>
    /// 基于TCP的服务器引擎端核心类
    /// </summary>
    public sealed class NetComplexServer : NetServerBase
    {

        #region 构造方法块
        /// <summary>
        /// 实例化一个网络服务器类对象
        /// </summary>
        public NetComplexServer()
        {
            AsyncCoordinator = new HslAsyncCoordinator(new Action(CalculateOnlineClients));
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

        private void CalculateOnlineClients()
        {
            StringBuilder builder = new StringBuilder();

            HybirdLockSockets.Enter();
            for (int i = 0; i < All_sockets_connect.Count; i++)
            {
                
                builder.Append(string.Format(FormatClientOnline, All_sockets_connect[i].IpAddress
                    , All_sockets_connect[i].LoginAlias));
            }
            HybirdLockSockets.Leave();


            if (builder.Length > 0)
            {
                m_AllClients = builder.Remove(0, 1).ToString();
            }
            else
            {
                m_AllClients = string.Empty;
            }
            // 触发状态变更
            AllClientsStatusChange?.Invoke(m_AllClients);
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
        private List<AsyncStateOne> All_sockets_connect { get; set; } = new List<AsyncStateOne>();


        /// <summary>
        /// 客户端数组操作的线程混合锁
        /// </summary>
        private SimpleHybirdLock HybirdLockSockets = new SimpleHybirdLock();

        #endregion

        #region 启动停止块
        

        /// <summary>
        /// 初始化操作
        /// </summary>
        protected override void StartInitialization()
        {
            Thread_heart_check = new Thread(new ThreadStart(ThreadHeartCheck))
            {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };
            Thread_heart_check.Start();
            base.StartInitialization();
        }

        /// <summary>
        /// 关闭网络时的操作
        /// </summary>
        protected override void CloseAction()
        {
            Thread_heart_check?.Abort();
            MessageAlerts = null;
            ClientOffline = null;
            ClientOnline = null;
            AcceptString = null;
            AcceptByte = null;

            //关闭所有的网络
            All_sockets_connect.ForEach(m => m.WorkSocket?.Close());
            base.CloseAction();
        }
        


        #endregion

        #region 客户端上下线块

        private void TcpStateUpLine(AsyncStateOne state)
        {
            HybirdLockSockets.Enter();
            All_sockets_connect.Add(state);
            HybirdLockSockets.Leave();

            // 提示上线
            ClientOnline?.Invoke(state);
            // 是否保存上线信息
            if (IsSaveLogClientLineChange)
            {
                LogNet?.WriteInfo("IP:" + state.IpAddress + " Name:" + state?.LoginAlias + " " + StringResources.NetClientOnline);
            }
            // 计算客户端在线情况
            AsyncCoordinator.StartOperaterInfomation();
        }
        private void TcpStateClose(AsyncStateOne state)
        {
            state?.WorkSocket.Close();
        }

        private void TcpStateDownLine(AsyncStateOne state, bool is_regular)
        {
            HybirdLockSockets.Enter();
            All_sockets_connect.Remove(state);
            HybirdLockSockets.Leave();
            // 关闭连接
            TcpStateClose(state);
            // 判断是否正常下线
            string str = is_regular ? StringResources.NetClientOffline : StringResources.NetClientBreak;
            ClientOffline?.Invoke(state, str);
            // 是否保存上线信息
            if (IsSaveLogClientLineChange)
            {
                LogNet?.WriteInfo("IP:" + state.IpAddress + " Name:" + state?.LoginAlias + " " + str);
            }
            // 计算客户端在线情况
            AsyncCoordinator.StartOperaterInfomation();
        }

        #endregion

        #region 事件委托块


        /// <summary>
        /// 服务器的异常，启动，等等一般消息产生的时候，出发此事件
        /// </summary>
        public event IEDelegate<string> MessageAlerts;
        /// <summary>
        /// 客户端的上下限状态变更时触发，仅作为在线客户端识别
        /// </summary>
        public event IEDelegate<string> AllClientsStatusChange;

        /// <summary>
        /// 当客户端上线的时候，触发此事件
        /// </summary>
        public event IEDelegate<AsyncStateOne> ClientOnline;
        /// <summary>
        /// 当客户端下线的时候，触发此事件
        /// </summary>
        public event IEDelegate<AsyncStateOne, string> ClientOffline;
        /// <summary>
        /// 当接收到文本数据的时候,触发此事件
        /// </summary>
        public event IEDelegate<AsyncStateOne, NetHandle, string> AcceptString;
        /// <summary>
        /// 当接收到字节数据的时候,触发此事件
        /// </summary>
        public event IEDelegate<AsyncStateOne, NetHandle, byte[]> AcceptByte;




        #endregion

        #region 请求接入块
        /// <summary>
        /// 登录后的处理方法
        /// </summary>
        /// <param name="obj"></param>
        protected override void ThreadPoolLogin(object obj)
        {
            if(obj is Socket socket)
            {
                // 判断连接数是否超出规定
                if (All_sockets_connect.Count > ConnectMax)
                {
                    socket?.Close();
                    LogNet?.WriteWarn(StringResources.NetClientFull);
                    return;
                }

                // 接收用户别名并验证令牌
                OperateResult result = new OperateResult();
                if(!ReceiveStringFromSocket(
                    socket,
                    out int customer,
                    out string login_alias,
                    result,
                    null
                    ))
                {
                    socket?.Close();
                    return;
                }

                

                // 登录成功
                AsyncStateOne stateone = new AsyncStateOne()
                {
                    WorkSocket = socket,
                    LoginAlias = login_alias,
                    IpEndPoint = (IPEndPoint)socket.RemoteEndPoint,
                    IpAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(),
                };

                
                if(customer == 1)
                {
                    // 电脑端客户端
                    stateone.ClientType = "Windows";
                }
                else if(customer == 2)
                {
                    // Android 客户端
                    stateone.ClientType = "Android";
                }

               
                try
                {
                    stateone.WorkSocket.BeginReceive(stateone.BytesHead, stateone.AlreadyReceivedHead,
                        stateone.BytesHead.Length- stateone.AlreadyReceivedHead, SocketFlags.None,
                        new AsyncCallback(HeadReceiveCallback), stateone);
                    TcpStateUpLine(stateone);
                    Thread.Sleep(500);//留下一些时间进行反应
                }
                catch (Exception ex)
                {
                    //登录前已经出错
                    TcpStateClose(stateone);
                    LogNet?.WriteException(StringResources.NetClientLoginFailed, ex);
                }
            }
        }



        #endregion

        #region 异步接收发送块


        internal override void SocketReceiveException(AsyncStateOne receive, Exception ex)
        {
            if (ex.Message.Contains(StringResources.SocketRemoteCloseException))
            {
                //异常掉线
                TcpStateDownLine(receive, false);
            }
        }









        /// <summary>
        /// 服务器端用于数据发送文本的方法
        /// </summary>
        /// <param name="stateone">数据发送对象</param>
        /// <param name="customer">用户自定义的数据对象，如不需要，赋值为0</param>
        /// <param name="str">发送的文本</param>
        public void Send(AsyncStateOne stateone, NetHandle customer, string str)
        {
            SendBytes(stateone, NetSupport.CommandBytes(customer, KeyToken, str));
        }
        /// <summary>
        /// 服务器端用于发送字节的方法
        /// </summary>
        /// <param name="stateone">数据发送对象</param>
        /// <param name="customer">用户自定义的数据对象，如不需要，赋值为0</param>
        /// <param name="bytes">实际发送的数据</param>
        public void Send(AsyncStateOne stateone, NetHandle customer, byte[] bytes)
        {
            SendBytes(stateone, NetSupport.CommandBytes(customer, KeyToken, bytes));
        }

        private void SendBytes(AsyncStateOne stateone, byte[] content)
        {
            SendBytesAsync(stateone, content);
        }


        /// <summary>
        /// 服务端用于发送所有数据到所有的客户端
        /// </summary>
        /// <param name="customer">用户自定义的命令头</param>
        /// <param name="str">需要传送的实际的数据</param>
        public void SendAllClients(NetHandle customer, string str)
        {
            for (int i = 0; i < All_sockets_connect.Count; i++)
            {
                Send(All_sockets_connect[i], customer, str);
            }
        }

        /// <summary>
        /// 服务端用于发送所有数据到所有的客户端
        /// </summary>
        /// <param name="customer">用户自定义的命令头</param>
        /// <param name="data">需要群发客户端的字节数据</param>
        public void SendAllClients(NetHandle customer, byte[] data)
        {
            for (int i = 0; i < All_sockets_connect.Count; i++)
            {
                Send(All_sockets_connect[i], customer, data);
            }
        }

        /// <summary>
        /// 根据客户端设置的别名进行发送消息
        /// </summary>
        /// <param name="Alias">客户端上线的别名</param>
        /// <param name="customer">用户自定义的命令头</param>
        /// <param name="str">需要传送的实际的数据</param>
        public void SendClientByAlias(string Alias, NetHandle customer, string str)
        {
            for (int i = 0; i < All_sockets_connect.Count; i++)
            {
                if (All_sockets_connect[i].LoginAlias == Alias)
                {
                    Send(All_sockets_connect[i], customer, str);
                }
            }
        }


                /// <summary>
        /// 根据客户端设置的别名进行发送消息
        /// </summary>
        /// <param name="Alias">客户端上线的别名</param>
        /// <param name="customer">用户自定义的命令头</param>
        /// <param name="data">需要传送的实际的数据</param>
        public void SendClientByAlias(string Alias, NetHandle customer, byte[] data)
        {
            for (int i = 0; i < All_sockets_connect.Count; i++)
            {
                if (All_sockets_connect[i].LoginAlias == Alias)
                {
                    Send(All_sockets_connect[i], customer, data);
                }
            }
        }


        #endregion

        #region 数据中心处理块
        /// <summary>
        /// 数据处理中心
        /// </summary>
        /// <param name="receive"></param>
        /// <param name="protocol"></param>
        /// <param name="customer"></param>
        /// <param name="content"></param>
        internal override void DataProcessingCenter(AsyncStateOne receive, int protocol, int customer, byte[] content)
        {
            if (protocol == HslCommunicationCode.Hsl_Protocol_Check_Secends)
            {
                BitConverter.GetBytes(DateTime.Now.Ticks).CopyTo(content, 8);
                SendBytes(receive, NetSupport.CommandBytes(HslCommunicationCode.Hsl_Protocol_Check_Secends, customer, KeyToken, content));
                receive.HeartTime = DateTime.Now;
            }
            else if (protocol == HslCommunicationCode.Hsl_Protocol_Client_Quit)
            {
                TcpStateDownLine(receive, true);
            }
            else if (protocol == HslCommunicationCode.Hsl_Protocol_User_Bytes)
            {
                //接收到字节数据
                AcceptByte?.Invoke(receive, customer, content);
                LogNet?.WriteDebug("Protocol:" + protocol + " customer:" + customer + " name:" + receive.LoginAlias);
            }
            else if (protocol == HslCommunicationCode.Hsl_Protocol_User_String)
            {
                //接收到文本数据
                string str = Encoding.Unicode.GetString(content);
                AcceptString?.Invoke(receive, customer, str);
                LogNet?.WriteDebug("Protocol:" + protocol + " customer:" + customer + " name:" + receive.LoginAlias);
            }
        }
        


        #endregion

        #region 心跳线程块

        private Thread Thread_heart_check { get; set; } = null;

        private void ThreadHeartCheck()
        {
            while (true)
            {
                Thread.Sleep(2000);

                try
                {
                    for (int i = All_sockets_connect.Count - 1; i >= 0; i--)
                    {
                        if (All_sockets_connect[i] == null)
                        {
                            All_sockets_connect.RemoveAt(i);
                            continue;
                        }

                        if ((DateTime.Now - All_sockets_connect[i].HeartTime).TotalSeconds > 1 * 8)//8次没有收到失去联系
                        {
                            LogNet?.WriteWarn("心跳验证超时，强制下线：" + All_sockets_connect[i].IpAddress.ToString());
                            TcpStateDownLine(All_sockets_connect[i], false);
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogNet?.WriteException("心跳线程异常：", ex);
                }


                if (!IsStarted) break;
            }
        }



        #endregion
        

    }

    /// <summary>
    /// 基于TCP通信的客户端核心类
    /// </summary>
    public sealed class NetComplexClient : NetShareBase
    {
        #region 基本属性块
        /// <summary>
        /// 客户端的核心连接块
        /// </summary>
        private AsyncStateOne stateone = new AsyncStateOne();
        /// <summary>
        /// 客户端系统是否启动
        /// </summary>
        public bool Is_Client_Start { get; set; } = false;

        /// <summary>
        /// 重连接失败的次数
        /// </summary>
        public int Connect_Failed_Count { get; set; } = 0;
        /// <summary>
        /// 指示客户端是否处于正在连接服务器中
        /// </summary>
        private bool Is_Client_Connecting = false;
        /// <summary>
        /// 登录服务器的判断锁
        /// </summary>
        private object lock_connecting = new object();
        /// <summary>
        /// 客户端登录的标识名称，可以为ID号，也可以为登录名
        /// </summary>
        public string ClientAlias { get; set; } = "";
        /// <summary>
        /// 远程服务器的IP地址和端口
        /// </summary>
        public IPEndPoint EndPointServer { get; set; } = new IPEndPoint(IPAddress.Any, 0);

        /// <summary>
        /// 服务器的时间，自动实现和服务器同步
        /// </summary>
        public DateTime ServerTime { get; private set; } = DateTime.Now;
        /// <summary>
        /// 系统与服务器的延时时间，单位毫秒
        /// </summary>
        public int DelayTime { get; private set; }
        

        #endregion

        #region 事件委托块



        /// <summary>
        /// 客户端启动成功的事件，重连成功也将触发此事件
        /// </summary>
        public event IEDelegate LoginSuccess;
        /// <summary>
        /// 连接失败时触发的事件
        /// </summary>
        public event IEDelegate<int> LoginFailed;
        /// <summary>
        /// 服务器的异常，启动，等等一般消息产生的时候，出发此事件
        /// </summary>
        public event IEDelegate<string> MessageAlerts;
        /// <summary>
        /// 在客户端断开后并在重连服务器之前触发，用于清理系统资源
        /// </summary>
        public event IEDelegate BeforReConnected;
        /// <summary>
        /// 当接收到文本数据的时候,触发此事件
        /// </summary>
        public event IEDelegate<AsyncStateOne, NetHandle, string> AcceptString;
        /// <summary>
        /// 当接收到字节数据的时候,触发此事件
        /// </summary>
        public event IEDelegate<AsyncStateOne, NetHandle, byte[]> AcceptByte;


        #endregion

        #region 启动停止重连块
        private bool IsQuie { get; set; } = false;

        /// <summary>
        /// 关闭该客户端引擎
        /// </summary>
        public void ClientClose()
        {
            IsQuie = true;
            if (Is_Client_Start)
                SendBytes(stateone, NetSupport.CommandBytes(HslCommunicationCode.Hsl_Protocol_Client_Quit, 0, KeyToken, null));

            thread_heart_check?.Abort();
            Is_Client_Start = false;
            Thread.Sleep(5);
            LoginSuccess = null;
            LoginFailed = null;
            MessageAlerts = null;
            AcceptByte = null;
            AcceptString = null;
            stateone.WorkSocket?.Close();
            LogNet?.WriteDebug("Client Close.");
        }
        /// <summary>
        /// 启动客户端引擎，连接服务器系统
        /// </summary>
        public void ClientStart()
        {
            if (Is_Client_Start) return;
            Thread thread_login = new Thread(new ThreadStart(ThreadLogin));
            thread_login.IsBackground = true;
            thread_login.Start();
            LogNet?.WriteDebug("Client Start.");

            if (thread_heart_check == null)
            {
                thread_heart_check = new Thread(new ThreadStart(ThreadHeartCheck));
                thread_heart_check.IsBackground = true;
                thread_heart_check.Start();
            }
        }
        private void ThreadLogin()
        {
            lock (lock_connecting)
            {
                if (Is_Client_Connecting) return;
                Is_Client_Connecting = true;
            }


            if (Connect_Failed_Count == 0)
            {
                MessageAlerts?.Invoke("正在连接服务器...");
            }
            else
            {
                int count = 10;
                while (count > 0)
                {
                    if (IsQuie) return;
                    MessageAlerts?.Invoke("连接断开，等待" + count-- + "秒后重新连接");
                    Thread.Sleep(1000);
                }
                MessageAlerts?.Invoke("正在尝试第" + Connect_Failed_Count + "次连接服务器...");
            }


            stateone.HeartTime = DateTime.Now;
            LogNet?.WriteDebug("Begin Connect Server, Times: " + Connect_Failed_Count);


            OperateResult result = new OperateResult();
            if(!CreateSocketAndConnect(out Socket socket,EndPointServer,result))
            {
                Connect_Failed_Count++;
                Is_Client_Connecting = false;
                LoginFailed?.Invoke(Connect_Failed_Count);
                LogNet?.WriteDebug("Connected Failed, Times: " + Connect_Failed_Count);
                // 连接失败，重新连接服务器
                ReconnectServer();
                return;
            }

            // 连接成功，发送数据信息
            if(!SendStringAndCheckReceive(
                socket,
                1,
                ClientAlias,
                result
                ))
            {
                Connect_Failed_Count++;
                Is_Client_Connecting = false;
                LogNet?.WriteDebug("Login Server Failed, Times: " + Connect_Failed_Count);
                LoginFailed?.Invoke(Connect_Failed_Count);
                // 连接失败，重新连接服务器
                ReconnectServer();
                return;
            }

            // 登录成功
            Connect_Failed_Count = 0;
            stateone.IpEndPoint = (IPEndPoint)socket.RemoteEndPoint;
            stateone.LoginAlias = ClientAlias;
            stateone.WorkSocket = socket;
            stateone.WorkSocket.BeginReceive(stateone.BytesHead, stateone.AlreadyReceivedHead,
                stateone.BytesHead.Length - stateone.AlreadyReceivedHead, SocketFlags.None,
                new AsyncCallback(HeadReceiveCallback), stateone);

            // 发送一条验证消息
            // SendBytes(stateone, CommunicationCode.CommandBytes(CommunicationCode.Hsl_Protocol_Check_Secends));
            byte[] bytesTemp = new byte[16];
            BitConverter.GetBytes(DateTime.Now.Ticks).CopyTo(bytesTemp, 0);
            SendBytes(stateone, NetSupport.CommandBytes(HslCommunicationCode.Hsl_Protocol_Check_Secends, 0, KeyToken, bytesTemp));


            stateone.HeartTime = DateTime.Now;
            Is_Client_Start = true;
            LoginSuccess?.Invoke();

            LogNet?.WriteDebug("Login Server Success, Times: " + Connect_Failed_Count);

            Is_Client_Connecting = false;

            Thread.Sleep(1000);
        }



        // private bool Is_reconnect_server = false;
        // private object lock_reconnect_server = new object();


        private void ReconnectServer()
        {
            // 是否连接服务器中，已经在连接的话，则不再连接
            if (Is_Client_Connecting) return;
            // 是否退出了系统，退出则不再重连
            if (IsQuie) return;

            LogNet?.WriteDebug("Prepare ReConnect Server.");

            // 触发连接失败，重连系统前错误
            BeforReConnected?.Invoke();
            stateone.WorkSocket?.Close();

            Thread thread_login = new Thread(new ThreadStart(ThreadLogin))
            {
                IsBackground = true
            };
            thread_login.Start();
        }

        #endregion

        #region 发送接收块

        /// <summary>
        /// 通信出错后的处理
        /// </summary>
        /// <param name="receive"></param>
        /// <param name="ex"></param>
        internal override void SocketReceiveException(AsyncStateOne receive, Exception ex)
        {
            if (ex.Message.Contains(StringResources.SocketRemoteCloseException))
            {
                // 异常掉线
                ReconnectServer();
            }
            else
            {
                // MessageAlerts?.Invoke("数据接收出错：" + ex.Message);
            }

            LogNet?.WriteDebug("Socket Excepiton Occured.");
        }

        
        /// <summary>
        /// 服务器端用于数据发送文本的方法
        /// </summary>
        /// <param name="customer">用户自定义的命令头</param>
        /// <param name="str">发送的文本</param>
        public void Send(NetHandle customer,string str)
        {
            if (Is_Client_Start)
            {
                SendBytes(stateone, NetSupport.CommandBytes(customer, KeyToken, str));
            }
        }
        /// <summary>
        /// 服务器端用于发送字节的方法
        /// </summary>
        /// <param name="customer">用户自定义的命令头</param>
        /// <param name="bytes">实际发送的数据</param>
        public void Send(NetHandle customer, byte[] bytes)
        {
            if (Is_Client_Start)
            {
                SendBytes(stateone, NetSupport.CommandBytes(customer, KeyToken, bytes));
            }
        }

        private void SendBytes(AsyncStateOne stateone, byte[] content)
        {
            SendBytesAsync(stateone, content);
        }

        #endregion

        #region 信息处理中心
        /// <summary>
        /// 客户端的数据处理中心
        /// </summary>
        /// <param name="receive"></param>
        /// <param name="protocol"></param>
        /// <param name="customer"></param>
        /// <param name="content"></param>
        internal override void DataProcessingCenter(AsyncStateOne receive, int protocol, int customer, byte[] content)
        {
            if (protocol == HslCommunicationCode.Hsl_Protocol_Check_Secends)
            {
                DateTime dt = new DateTime(BitConverter.ToInt64(content, 0));
                ServerTime = new DateTime(BitConverter.ToInt64(content, 8));
                DelayTime = (int)(DateTime.Now - dt).TotalMilliseconds;
                stateone.HeartTime = DateTime.Now;
                // MessageAlerts?.Invoke("心跳时间：" + DateTime.Now.ToString());
            }
            else if (protocol == HslCommunicationCode.Hsl_Protocol_Client_Quit)
            {
                // 申请了退出
            }
            else if (protocol == HslCommunicationCode.Hsl_Protocol_User_Bytes)
            {
                // 接收到字节数据
                AcceptByte?.Invoke(stateone, customer, content);
            }
            else if (protocol == HslCommunicationCode.Hsl_Protocol_User_String)
            {
                // 接收到文本数据
                string str = Encoding.Unicode.GetString(content);
                AcceptString?.Invoke(stateone, customer, str);
            }
        }
        
        #endregion

        #region 心跳线程块

        private Thread thread_heart_check { get; set; } = null;

        /// <summary>
        /// 心跳线程的方法
        /// </summary>
        private void ThreadHeartCheck()
        {
            Thread.Sleep(2000);
            while (true)
            {
                Thread.Sleep(1000);
                if (!IsQuie)
                {
                    byte[] send = new byte[16];
                    BitConverter.GetBytes(DateTime.Now.Ticks).CopyTo(send, 0);
                    SendBytes(stateone, NetSupport.CommandBytes(HslCommunicationCode.Hsl_Protocol_Check_Secends, 0, KeyToken, send));
                    double timeSpan = (DateTime.Now - stateone.HeartTime).TotalSeconds;
                    if (timeSpan > 1 * 8)//8次没有收到失去联系
                    {
                        LogNet?.WriteDebug($"Heart Check Failed int {timeSpan} Seconds.");
                        ReconnectServer();
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    break;
                }
            }
        }


        #endregion
    }


}
