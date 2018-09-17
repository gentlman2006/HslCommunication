using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using HslCommunication.Core.IMessage;

namespace HslCommunication.Core.Net
{

    /// <summary>
    /// 异形客户端的基类，提供了基础的异形操作
    /// </summary>
    public class NetworkAlienClient : NetworkServerBase
    {
        #region Constructor

        /// <summary>
        /// 默认的无参构造方法
        /// </summary>
        public NetworkAlienClient( )
        {
            password = new byte[6];
            alreadyLock = new SimpleHybirdLock( );
            alreadyOnline = new List<AlienSession>( );
            trustOnline = new List<string>( );
            trustLock = new SimpleHybirdLock( );
            ThreadCheckStart( );
        }

        #endregion

        #region NetworkServerBase Override

        /// <summary>
        /// 登录的回调方法
        /// </summary>
        /// <param name="obj">传入的异步对象</param>
        protected override void ThreadPoolLogin( object obj )
        {
            if (obj is Socket socket)
            {
                // 注册包
                // 0x48 0x73 0x6E 0x00 0x17 0x31 0x32 0x33 0x34 0x35 0x36 0x37 0x38 0x39 0x30 0x31 0x00 0x00 0x00 0x00 0x00 0x00 0xC0 0xA8 0x00 0x01 0x17 0x10
                // +------------+ +--+ +--+ +----------------------------------------------------+ +---------------------------+ +-----------------+ +-------+
                // + 固定消息头  +备用 长度           DTU码 12345678901 (唯一标识)                  登录密码(不受信的排除)     Ip:192.168.0.1    端口10000
                // +------------+ +--+ +--+ +----------------------------------------------------+ +---------------------------+ +-----------------+

                // 返回
                // 0x48 0x73 0x6E 0x00 0x01 0x00
                // +------------+ +--+ +--+ +--+
                //   固定消息头  备用 长度 结果代码

                // 结果代码 
                // 0x00: 登录成功 
                // 0x01: DTU重复登录 
                // 0x02: DTU禁止登录
                // 0x03: 密码验证失败 

                OperateResult<AlienMessage> check = ReceiveMessage( socket, 5000, new AlienMessage( ) );
                if (!check.IsSuccess) return;

                if (check.Content.HeadBytes[4] != 0x17 || check.Content.ContentBytes.Length != 0x17)
                {
                    socket?.Close( );
                    LogNet?.WriteWarn( ToString( ), "Length Check Failed" );
                    return;
                }

                // 密码验证
                bool isPasswrodRight = true;
                for (int i = 0; i < password.Length; i++)
                {
                    if (check.Content.ContentBytes[11 + i] != password[i])
                    {
                        isPasswrodRight = false;
                        break;
                    }
                }

                string dtu = Encoding.ASCII.GetString( check.Content.ContentBytes, 0, 11 ).Trim( );

                // 密码失败的情况
                if (!isPasswrodRight)
                {
                    OperateResult send = Send( socket, GetResponse( StatusPasswodWrong ) );
                    if (send.IsSuccess) socket?.Close( );
                    LogNet?.WriteWarn( ToString( ), "Login Password Wrong, Id:" + dtu );
                    return;
                }

                AlienSession session = new AlienSession( )
                {
                    DTU = dtu,
                    Socket = socket,
                };

                // 检测是否禁止登录
                if (!IsClientPermission( session ))
                {
                    OperateResult send = Send( socket, GetResponse( StatusLoginForbidden ) );
                    if (send.IsSuccess) socket?.Close( );
                    LogNet?.WriteWarn( ToString( ), "Login Forbidden, Id:" + session.DTU );
                    return;
                }

                // 检测是否重复登录，不重复的话，也就是意味着登录成功了
                if (IsClientOnline( session ))
                {
                    OperateResult send = Send( socket, GetResponse( StatusLoginRepeat ) );
                    if (send.IsSuccess) socket?.Close( );
                    LogNet?.WriteWarn( ToString( ), "Login Repeat, Id:" + session.DTU );
                    return;
                }
                else
                {
                    OperateResult send = Send( socket, GetResponse( StatusOk ) );
                    if (!send.IsSuccess) return;
                }

                // 触发上线消息
                OnClientConnected?.Invoke( this, session );
            }
        }


        #endregion

        #region Client Event

        /// <summary>
        /// 当有服务器连接上来的时候触发
        /// </summary>
        public event Action<NetworkAlienClient, AlienSession> OnClientConnected = null;

        #endregion

        #region Private Method

        /// <summary>
        /// 获取返回的命令信息
        /// </summary>
        /// <param name="status">状态</param>
        /// <returns>回发的指令信息</returns>
        private byte[] GetResponse(byte status)
        {
            return new byte[]
            {
                0x48,0x73,0x6E,0x00,0x01,status
            };
        }

        /// <summary>
        /// 状态登录成功
        /// </summary>
        private const byte StatusOk = 0x00;
        /// <summary>
        /// 重复登录
        /// </summary>
        private const byte StatusLoginRepeat = 0x01;
        /// <summary>
        /// 禁止登录
        /// </summary>
        private const byte StatusLoginForbidden = 0x02;
        /// <summary>
        /// 密码错误
        /// </summary>
        private const byte StatusPasswodWrong = 0x03;


        /// <summary>
        /// 检测当前的DTU是否在线
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private bool IsClientOnline( AlienSession session )
        {
            bool result = false;
            alreadyLock.Enter( );

            for (int i = 0; i < alreadyOnline.Count; i++)
            {
                if (alreadyOnline[i].DTU == session.DTU)
                {
                    result = true;
                    break;
                }
            }


            if (!result)
            {
                alreadyOnline.Add( session );
            }

            alreadyLock.Leave( );

            return result;
        }

        /// <summary>
        /// 检测当前的dtu是否允许登录
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private bool IsClientPermission( AlienSession session )
        {
            bool result = false;

            trustLock.Enter( );

            if (trustOnline.Count == 0)
            {
                result = true;
            }
            else
            {
                for (int i = 0; i < trustOnline.Count; i++)
                {
                    if (trustOnline[i] == session.DTU)
                    {
                        result = true;
                        break;
                    }
                }
            }

            trustLock.Leave( );

            return result;
        }


        #endregion

        #region Public Method
        

        /// <summary>
        /// 设置密码，长度为6
        /// </summary>
        /// <param name="password"></param>
        public void SetPassword(byte[] password)
        {
            if(password?.Length == 6)
            {
                password.CopyTo( this.password, 0 );
            }
        }

        /// <summary>
        /// 设置可信任的客户端列表
        /// </summary>
        /// <param name="clients"></param>
        public void SetTrustClients(string[] clients)
        {
            trustLock.Enter( );

            trustOnline = new List<string>( clients );

            trustLock.Leave( );
        }


        /// <summary>
        /// 退出异形客户端
        /// </summary>
        /// <param name="session">异形客户端的会话</param>
        public void AlienSessionLoginOut( AlienSession session )
        {
            alreadyLock.Enter( );

            alreadyOnline.Remove( session );

            alreadyLock.Leave( );
        }


        #endregion

        #region Thread Check Client

        private void ThreadCheckStart()
        {
            threadCheck = new Thread(new ThreadStart( ThreadCheckAlienClient ));
            threadCheck.IsBackground = true;
            threadCheck.Priority = ThreadPriority.AboveNormal;
            threadCheck.Start( );
        }

        private void ThreadCheckAlienClient( )
        {
            Thread.Sleep( 1000 );
            while (true)
            {
                Thread.Sleep( 1000 );

                alreadyLock.Enter( );

                for (int i = alreadyOnline.Count - 1; i >= 0; i--)
                {
                    if (!alreadyOnline[i].IsStatusOk)
                    {
                        alreadyOnline.RemoveAt( i );
                    }
                }


                alreadyLock.Leave( );
            }
        }

        #endregion
        
        #region Private Member

        private byte[] password;                    // 密码设置
        private List<AlienSession> alreadyOnline;   // 所有在线信息
        private SimpleHybirdLock alreadyLock;       // 列表的同步锁
        private List<string> trustOnline;           // 禁止登录的客户端信息
        private SimpleHybirdLock trustLock;         // 禁止登录的锁
        private Thread threadCheck;                 // 后台检测在线情况的

        #endregion

        #region Object Override

        /// <summary>
        /// 获取本对象的字符串表示形式
        /// </summary>
        /// <returns></returns>
        public override string ToString( )
        {
            return "NetworkAlienBase";
        }


        #endregion
    }
}
