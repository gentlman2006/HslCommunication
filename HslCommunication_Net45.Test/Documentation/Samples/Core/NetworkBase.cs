using HslCommunication.Enthernet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.Profinet.Melsec;
using HslCommunication.Core.Net;
using HslCommunication;
using System.Net.Sockets;

namespace HslCommunication_Net45.Test.Documentation.Samples.Core
{
    public class NetworkBaseExample
    {
        public void TokenClientExample( )
        {
            #region TokenClientExample

            NetSimplifyClient simplifyClient = new NetSimplifyClient( "127.0.0.1", 12345 );

            // 这个toeken需要和服务器的设置的token相匹配才可以
            simplifyClient.Token = new Guid( "787f9607-dd7a-4ba7-9f98-769d24de05df" );


            #endregion
        }

        #region TokenServerExample

        private NetSimplifyServer simplifyServer = null;

        public void TokenServerExample( )
        {
            simplifyServer = new NetSimplifyServer( );
            simplifyServer.Token = new Guid( "787f9607-dd7a-4ba7-9f98-769d24de05df" );

            simplifyServer.ReceiveStringEvent += SimplifyServer_ReceiveStringEvent;
            simplifyServer.ServerStart( 12345 );
        }

        private void SimplifyServer_ReceiveStringEvent( HslCommunication.Core.Net.AppSession session, HslCommunication.NetHandle handle, string data )
        {
            // 示例情况，接收到数据后返回消息
            simplifyServer.SendMessage( session, handle, "Back:" + data );
        }

        #endregion

        public void LogNetExample( )
        {

            #region LogNetExample1

            // 设备连接对象的日志
            MelsecMcNet melsec = new MelsecMcNet( "192.168.0.100", 6000 );

            // 举例实现日志文件为单日志文件
            melsec.LogNet = new HslCommunication.LogNet.LogNetSingle( "D://123.txt" );

            #endregion

            #region LogNetExample2

            // 一般服务器对象的
            NetSimplifyServer simplifyServer = new NetSimplifyServer( );
            simplifyServer.LogNet = new HslCommunication.LogNet.LogNetSingle( "D://log.txt" );
            simplifyServer.ReceiveStringEvent += ( HslCommunication.Core.Net.AppSession session, HslCommunication.NetHandle handle, string data ) =>
            {
                simplifyServer.SendMessage( session, handle, "Back:" + data );
            };
            simplifyServer.ServerStart( 45678 );

            #endregion

        }

        #region CreateSocketAndConnectExample

        public class NetworkMy : NetworkBase
        {
            public void CreateSocketAndConnectExample1( )
            {
                // 连接远程的端口
                OperateResult<Socket> socketResult = CreateSocketAndConnect( "192.168.0.100", 12345 );
                if (socketResult.IsSuccess)
                {
                    // connect success
                }
                else
                {
                    // failed
                }
            }

            public void CreateSocketAndConnectExample2( )
            {
                // 连接远程的端口，允许设置超时时间，比如1秒
                OperateResult<Socket> socketResult = CreateSocketAndConnect( "192.168.0.100", 12345, 1000);
                if (socketResult.IsSuccess)
                {
                    // connect success
                }
                else
                {
                    // failed
                }
            }

            public void CreateSocketAndConnectExample3( )
            {
                // 连接远程的端口，允许设置超时时间，比如1秒
                OperateResult<Socket> socketResult = CreateSocketAndConnect( new System.Net.IPEndPoint( System.Net.IPAddress.Parse( "192.168.0.100" ), 12345 ), 1000 );
                if (socketResult.IsSuccess)
                {
                    // connect success
                }
                else
                {
                    // failed
                }
            }
        }

        #endregion
    }
}
