using HslCommunication.Core.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HslCommunication.Enthernet.Redis
{
    /// <summary>
    /// Redis协议的订阅操作，一个对象订阅一个或是多个频道的信息
    /// </summary>
    public class RedisSubscribe : NetworkXBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个发布订阅类的客户端，需要指定ip地址，端口，及订阅关键字
        /// </summary>
        /// <param name="ipAddress">服务器的IP地址</param>
        /// <param name="port">服务器的端口号</param>
        /// <param name="keys">订阅关键字</param>
        public RedisSubscribe( string ipAddress, int port, string[] keys )
        {
            endPoint = new IPEndPoint( IPAddress.Parse( ipAddress ), port );
            keyWords = keys;

            if (keys == null)
            {
                throw new Exception( StringResources.Language.KeyIsNotAllowedNull );
            }
        }

        /// <summary>
        /// 实例化一个发布订阅类的客户端，需要指定ip地址，端口，及订阅关键字
        /// </summary>
        /// <param name="ipAddress">服务器的IP地址</param>
        /// <param name="port">服务器的端口号</param>
        /// <param name="key">订阅关键字</param>
        public RedisSubscribe( string ipAddress, int port, string key )
        {
            endPoint = new IPEndPoint( IPAddress.Parse( ipAddress ), port );
            keyWords = new string[] { key };

            if (string.IsNullOrEmpty( key ))
            {
                throw new Exception( StringResources.Language.KeyIsNotAllowedNull );
            }
        }

        #endregion

        #region Private Method

        private OperateResult CreatePush( )
        {
            CoreSocket?.Close( );

            OperateResult<Socket> connect = CreateSocketAndConnect( endPoint, 5000 );
            if (!connect.IsSuccess) return connect;

            // 密码的验证
            if (!string.IsNullOrEmpty( this.Password ))
            {
                OperateResult check = Send( connect.Content, RedisHelper.PackStringCommand( new string[] { "AUTH", this.Password } ) );
                if (!check.IsSuccess) return check;

                OperateResult<byte[]> checkResult = RedisHelper.ReceiveCommand( connect.Content );
                if (!checkResult.IsSuccess) return checkResult;

                string msg = Encoding.UTF8.GetString( checkResult.Content );
                if (!msg.StartsWith( "+OK" )) return new OperateResult( msg );
            }

            List<string> lists = new List<string>( );
            lists.Add( "SUBSCRIBE" );
            lists.AddRange( keyWords );


            OperateResult send = Send( connect.Content, RedisHelper.PackStringCommand( lists.ToArray( ) ) );
            if (!send.IsSuccess) return send;
            CoreSocket = connect.Content;

            try
            {
                connect.Content.BeginReceive( new byte[0], 0, 0, SocketFlags.None, new AsyncCallback( ReceiveCallBack ), connect.Content );
            }
            catch(Exception ex)
            {
                return new OperateResult( ex.Message );
            }

            return OperateResult.CreateSuccessResult( );
        }


        private void ReceiveCallBack( IAsyncResult ar )
        {
            if(ar.AsyncState is Socket socket)
            {
                try
                {
                    int receive = socket.EndReceive( ar );
                    OperateResult<byte[]> read = RedisHelper.ReceiveCommand( socket );
                    if (!read.IsSuccess)
                    {
                        SocketReceiveException( null );
                        return;
                    }
                    else
                    {
                        socket.BeginReceive( new byte[0], 0, 0, SocketFlags.None, new AsyncCallback( ReceiveCallBack ), socket );
                    }

                    OperateResult<string[]> data = RedisHelper.GetStringsFromCommandLine( read.Content );
                    if (!data.IsSuccess) {
                        LogNet?.WriteWarn( data.Message );
                        return;
                    }

                    if(data.Content[0].ToUpper() == "SUBSCRIBE")
                    {
                        return;
                    }
                    else if(data.Content[0].ToUpper( ) == "MESSAGE")
                    {
                        action?.Invoke( data.Content[1], data.Content[2] );
                    }
                    else
                    {
                        LogNet?.WriteWarn( data.Content[0] );
                    }

                }
                catch (ObjectDisposedException)
                {
                    // 通常是主动退出
                    return;
                }
                catch(Exception ex)
                {
                    SocketReceiveException( ex );
                }
            }
        }

        private void SocketReceiveException( Exception ex )
        {
            // 发生异常的时候需要进行重新连接
            while (true)
            {
                if (ex != null) LogNet?.WriteException( "Offline", ex );

                Console.WriteLine( StringResources.Language.ReConnectServerAfterTenSeconds );
                System.Threading.Thread.Sleep( this.reconnectTime );

                if (CreatePush( ).IsSuccess)
                {
                    Console.WriteLine( StringResources.Language.ReConnectServerSuccess );
                    break;
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 如果Redis服务器设置了密码，此处就需要进行设置。必须在CreatePush方法调用前设置
        /// </summary>
        public string Password { get; set; }

        #endregion

        #region Public Method 

        /// <summary>
        /// 创建数据推送服务
        /// </summary>
        /// <param name="pushCallBack">触发数据推送的委托</param>
        /// <returns>是否创建成功</returns>
        public OperateResult CreatePush( Action<string, string> pushCallBack )
        {
            action = pushCallBack;
            return CreatePush( );
        }

        /// <summary>
        /// 关闭消息推送的界面
        /// </summary>
        public void ClosePush( )
        {
            action = null;
            CoreSocket?.Close( );
        }

        #endregion

        #region Private Member

        private IPEndPoint endPoint;                           // 服务器的地址及端口信息
        private string[] keyWords = null;                      // 缓存的订阅关键字
        private Action<string, string> action;                 // 服务器推送后的回调方法
        private int reconnectTime = 10000;                     // 重连服务器的时间

        #endregion
    }
}
