using HslCommunication.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HslCommunication.Core.Net;
using System.Net;
using System.Net.Sockets;

namespace HslCommunication.Enthernet
{

    /// <summary>
    /// 一个基于异步高性能的客户端网络类，支持主动接收服务器的消息
    /// </summary>
    public class NetComplexClient : NetworkXBase
    {
        #region Constructor
        /// <summary>
        /// 实例化一个对象
        /// </summary>
        public NetComplexClient( )
        {

        }

        #endregion

        #region 基本属性块
        /// <summary>
        /// 客户端的核心连接块
        /// </summary>
        private AppSession stateone = new AppSession( );
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
        private object lock_connecting = new object( );
        /// <summary>
        /// 客户端登录的标识名称，可以为ID号，也可以为登录名
        /// </summary>
        public string ClientAlias { get; set; } = "";
        /// <summary>
        /// 远程服务器的IP地址和端口
        /// </summary>
        public IPEndPoint EndPointServer { get; set; } = new IPEndPoint( IPAddress.Any, 0 );

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
        public event Action LoginSuccess;
        /// <summary>
        /// 连接失败时触发的事件
        /// </summary>
        public event Action<int> LoginFailed;
        /// <summary>
        /// 服务器的异常，启动，等等一般消息产生的时候，出发此事件
        /// </summary>
        public event Action<string> MessageAlerts;
        /// <summary>
        /// 在客户端断开后并在重连服务器之前触发，用于清理系统资源
        /// </summary>
        public event Action BeforReConnected;
        /// <summary>
        /// 当接收到文本数据的时候,触发此事件
        /// </summary>
        public event Action<AppSession, NetHandle, string> AcceptString;
        /// <summary>
        /// 当接收到字节数据的时候,触发此事件
        /// </summary>
        public event Action<AppSession, NetHandle, byte[]> AcceptByte;


        #endregion

        #region 启动停止重连块
        private bool IsQuie { get; set; } = false;

        /// <summary>
        /// 关闭该客户端引擎
        /// </summary>
        public void ClientClose( )
        {
            IsQuie = true;
            if (Is_Client_Start)
                SendBytes( stateone, NetSupport.CommandBytes( HslProtocol.ProtocolClientQuit, 0, Token, null ) );

            thread_heart_check?.Abort( );
            Is_Client_Start = false;
            Thread.Sleep( 5 );
            LoginSuccess = null;
            LoginFailed = null;
            MessageAlerts = null;
            AcceptByte = null;
            AcceptString = null;
            stateone.WorkSocket?.Close( );
            LogNet?.WriteDebug( ToString(), "Client Close." );
        }
        /// <summary>
        /// 启动客户端引擎，连接服务器系统
        /// </summary>
        public void ClientStart( )
        {
            if (Is_Client_Start) return;
            Thread thread_login = new Thread( new ThreadStart( ThreadLogin ) );
            thread_login.IsBackground = true;
            thread_login.Start( );
            LogNet?.WriteDebug( ToString( ), "Client Start." );

            if (thread_heart_check == null)
            {
                thread_heart_check = new Thread( new ThreadStart( ThreadHeartCheck ) );
                thread_heart_check.IsBackground = true;
                thread_heart_check.Start( );
            }
        }
        private void ThreadLogin( )
        {
            lock (lock_connecting)
            {
                if (Is_Client_Connecting) return;
                Is_Client_Connecting = true;
            }


            if (Connect_Failed_Count == 0)
            {
                MessageAlerts?.Invoke( "正在连接服务器..." );
            }
            else
            {
                int count = 10;
                while (count > 0)
                {
                    if (IsQuie) return;
                    MessageAlerts?.Invoke( "连接断开，等待" + count-- + "秒后重新连接" );
                    Thread.Sleep( 1000 );
                }
                MessageAlerts?.Invoke( "正在尝试第" + Connect_Failed_Count + "次连接服务器..." );
            }


            stateone.HeartTime = DateTime.Now;
            LogNet?.WriteDebug( ToString( ), "Begin Connect Server, Times: " + Connect_Failed_Count );


            OperateResult result = new OperateResult( );

            OperateResult<Socket> connectResult = CreateSocketAndConnect( EndPointServer, 10000 );
            if (!connectResult.IsSuccess)
            {
                Connect_Failed_Count++;
                Is_Client_Connecting = false;
                LoginFailed?.Invoke( Connect_Failed_Count );
                LogNet?.WriteDebug( ToString( ), "Connected Failed, Times: " + Connect_Failed_Count );
                // 连接失败，重新连接服务器
                ReconnectServer( );
                return;
            }



            // 连接成功，发送数据信息
            OperateResult sendResult = SendStringAndCheckReceive( connectResult.Content, 1, ClientAlias );

            if (!sendResult.IsSuccess)
            {
                Connect_Failed_Count++;
                Is_Client_Connecting = false;
                LogNet?.WriteDebug( ToString( ), "Login Server Failed, Times: " + Connect_Failed_Count );
                LoginFailed?.Invoke( Connect_Failed_Count );
                // 连接失败，重新连接服务器
                ReconnectServer( );
                return;
            }

            // 登录成功
            Connect_Failed_Count = 0;
            stateone.IpEndPoint = (IPEndPoint)connectResult.Content.RemoteEndPoint;
            stateone.LoginAlias = ClientAlias;
            stateone.WorkSocket = connectResult.Content;
            stateone.WorkSocket.BeginReceive( stateone.BytesHead, stateone.AlreadyReceivedHead,
                stateone.BytesHead.Length - stateone.AlreadyReceivedHead, SocketFlags.None,
                new AsyncCallback( HeadReceiveCallback ), stateone );

            // 发送一条验证消息
            // SendBytes(stateone, CommunicationCode.CommandBytes(CommunicationCode.Hsl_Protocol_Check_Secends));
            byte[] bytesTemp = new byte[16];
            BitConverter.GetBytes( DateTime.Now.Ticks ).CopyTo( bytesTemp, 0 );
            SendBytes( stateone, NetSupport.CommandBytes( HslProtocol.ProtocolCheckSecends, 0, Token, bytesTemp ) );


            stateone.HeartTime = DateTime.Now;
            Is_Client_Start = true;
            LoginSuccess?.Invoke( );

            LogNet?.WriteDebug( ToString( ), "Login Server Success, Times: " + Connect_Failed_Count );

            Is_Client_Connecting = false;

            Thread.Sleep( 1000 );
        }



        // private bool Is_reconnect_server = false;
        // private object lock_reconnect_server = new object();


        private void ReconnectServer( )
        {
            // 是否连接服务器中，已经在连接的话，则不再连接
            if (Is_Client_Connecting) return;
            // 是否退出了系统，退出则不再重连
            if (IsQuie) return;

            LogNet?.WriteDebug( ToString( ), "Prepare ReConnect Server." );

            // 触发连接失败，重连系统前错误
            BeforReConnected?.Invoke( );
            stateone.WorkSocket?.Close( );

            Thread thread_login = new Thread( new ThreadStart( ThreadLogin ) )
            {
                IsBackground = true
            };
            thread_login.Start( );
        }

        #endregion

        #region 发送接收块

        /// <summary>
        /// 通信出错后的处理
        /// </summary>
        /// <param name="receive"></param>
        /// <param name="ex"></param>
        internal override void SocketReceiveException( AppSession receive, Exception ex )
        {
            if (ex.Message.Contains( StringResources.SocketRemoteCloseException ))
            {
                // 异常掉线
                ReconnectServer( );
            }
            else
            {
                // MessageAlerts?.Invoke("数据接收出错：" + ex.Message);
            }

            LogNet?.WriteDebug( ToString( ), "Socket Excepiton Occured." );
        }


        /// <summary>
        /// 服务器端用于数据发送文本的方法
        /// </summary>
        /// <param name="customer">用户自定义的命令头</param>
        /// <param name="str">发送的文本</param>
        public void Send( NetHandle customer, string str )
        {
            if (Is_Client_Start)
            {
                SendBytes( stateone, NetSupport.CommandBytes( customer, Token, str ) );
            }
        }
        /// <summary>
        /// 服务器端用于发送字节的方法
        /// </summary>
        /// <param name="customer">用户自定义的命令头</param>
        /// <param name="bytes">实际发送的数据</param>
        public void Send( NetHandle customer, byte[] bytes )
        {
            if (Is_Client_Start)
            {
                SendBytes( stateone, NetSupport.CommandBytes( customer, Token, bytes ) );
            }
        }

        private void SendBytes( AppSession stateone, byte[] content )
        {
            SendBytesAsync( stateone, content );
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
        internal override void DataProcessingCenter( AppSession receive, int protocol, int customer, byte[] content )
        {
            if (protocol == HslProtocol.ProtocolCheckSecends)
            {
                DateTime dt = new DateTime( BitConverter.ToInt64( content, 0 ) );
                ServerTime = new DateTime( BitConverter.ToInt64( content, 8 ) );
                DelayTime = (int)(DateTime.Now - dt).TotalMilliseconds;
                stateone.HeartTime = DateTime.Now;
                // MessageAlerts?.Invoke("心跳时间：" + DateTime.Now.ToString());
            }
            else if (protocol == HslProtocol.ProtocolClientQuit)
            {
                // 申请了退出
            }
            else if (protocol == HslProtocol.ProtocolUserBytes)
            {
                // 接收到字节数据
                AcceptByte?.Invoke( stateone, customer, content );
            }
            else if (protocol == HslProtocol.ProtocolUserString)
            {
                // 接收到文本数据
                string str = Encoding.Unicode.GetString( content );
                AcceptString?.Invoke( stateone, customer, str );
            }
        }

        #endregion

        #region 心跳线程块

        private Thread thread_heart_check { get; set; } = null;

        /// <summary>
        /// 心跳线程的方法
        /// </summary>
        private void ThreadHeartCheck( )
        {
            Thread.Sleep( 2000 );
            while (true)
            {
                Thread.Sleep( 1000 );
                if (!IsQuie)
                {
                    byte[] send = new byte[16];
                    BitConverter.GetBytes( DateTime.Now.Ticks ).CopyTo( send, 0 );
                    SendBytes( stateone, NetSupport.CommandBytes( HslProtocol.ProtocolCheckSecends, 0, Token, send ) );
                    double timeSpan = (DateTime.Now - stateone.HeartTime).TotalSeconds;
                    if (timeSpan > 1 * 8)//8次没有收到失去联系
                    {
                        LogNet?.WriteDebug( ToString( ), $"Heart Check Failed int {timeSpan} Seconds." );
                        ReconnectServer( );
                        Thread.Sleep( 1000 );
                    }
                }
                else
                {
                    break;
                }
            }
        }


        #endregion

        #region Object Override

        /// <summary>
        /// 返回对象的字符串表示形式
        /// </summary>
        /// <returns></returns>
        public override string ToString( )
        {
            return "NetComplexClient";
        }

        #endregion
    }
}
