using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core.Net;
using System.Net;
using System.Net.Sockets;

namespace HslCommunication.Enthernet
{

    /// <summary>
    /// 发布订阅类的客户端，使用指定的关键订阅相关的数据推送信息
    /// </summary>
    /// <remarks>
    /// 详细的使用说明，请参照博客<a href="http://www.cnblogs.com/dathlin/p/8992315.html">http://www.cnblogs.com/dathlin/p/8992315.html</a>
    /// </remarks>
    /// <example>
    /// 此处贴上了Demo项目的服务器配置的示例代码
    /// <code lang="cs" source="TestProject\HslCommunicationDemo\FormPushNet.cs" region="FormPushNet" title="NetPushClient示例" />
    /// </example>
    public class NetPushClient : NetworkXBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个发布订阅类的客户端，需要指定ip地址，端口，及订阅关键字
        /// </summary>
        /// <param name="ipAddress">服务器的IP地址</param>
        /// <param name="port">服务器的端口号</param>
        /// <param name="key">订阅关键字</param>
        public NetPushClient( string ipAddress, int port, string key )
        {
            endPoint = new IPEndPoint( IPAddress.Parse( ipAddress ), port );
            keyWord = key;

            if (string.IsNullOrEmpty( key ))
            {
                throw new Exception( StringResources.Language.KeyIsNotAllowedNull );
            }
        }

        #endregion

        #region NetworkXBase Override

        internal override void DataProcessingCenter( AppSession session, int protocol, int customer, byte[] content )
        {
            if(protocol == HslProtocol.ProtocolUserString)
            {
                action?.Invoke( this, Encoding.Unicode.GetString( content ) );
            }
        }

        internal override void SocketReceiveException( AppSession session, Exception ex )
        {
            // 发生异常的时候需要进行重新连接
            while (true)
            {
                Console.WriteLine( ex );
                Console.WriteLine( StringResources.Language.ReConnectServerAfterTenSeconds );
                System.Threading.Thread.Sleep( this.reconnectTime );

                if(CreatePush( ).IsSuccess)
                {
                    Console.WriteLine( StringResources.Language.ReConnectServerSuccess );
                    break;
                }
            }
        }

        #endregion

        #region Private Method

        private OperateResult CreatePush( )
        {
            CoreSocket?.Close( );

            OperateResult<Socket> connect = CreateSocketAndConnect( endPoint, 5000 );
            if (!connect.IsSuccess) return connect;

            OperateResult send = SendStringAndCheckReceive( connect.Content, 0, keyWord );
            if (!send.IsSuccess) return send;

            OperateResult<int, string> receive = ReceiveStringContentFromSocket( connect.Content );
            if (!receive.IsSuccess) return receive;

            if (receive.Content1 != 0) return new OperateResult( receive.Content2 );

            AppSession appSession = new AppSession( );
            CoreSocket = connect.Content;
            appSession.WorkSocket = connect.Content;
            ReBeginReceiveHead( appSession, false );
            
            return OperateResult.CreateSuccessResult( );
        }

        #endregion

        #region Public Method 

        /// <summary>
        /// 创建数据推送服务
        /// </summary>
        /// <param name="pushCallBack">触发数据推送的委托</param>
        /// <returns>是否创建成功</returns>
        public OperateResult CreatePush( Action<NetPushClient, string> pushCallBack )
        {
            action = pushCallBack;
            return CreatePush( );
        }
        
        /// <summary>
        /// 关闭消息推送的界面
        /// </summary>
        public void ClosePush()
        {
            action = null;
            if (CoreSocket != null && CoreSocket.Connected) CoreSocket?.Send( BitConverter.GetBytes( 100 ) );
            CoreSocket?.Close( );
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 本客户端的关键字
        /// </summary>
        public string KeyWord => keyWord;

        /// <summary>
        /// 获取或设置重连服务器的间隔时间
        /// </summary>
        public int ReConnectTime { set => reconnectTime = value; get => reconnectTime; }

        #endregion

        #region Private Member

        private IPEndPoint endPoint;                           // 服务器的地址及端口信息
        private string keyWord = string.Empty;                 // 缓存的订阅关键字
        private Action<NetPushClient, string> action;          // 服务器推送后的回调方法
        private int reconnectTime = 10000;                     // 重连服务器的时间

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString( )
        {
            return $"NetPushClient[{endPoint.ToString()}]";
        }

        #endregion
    }
}
