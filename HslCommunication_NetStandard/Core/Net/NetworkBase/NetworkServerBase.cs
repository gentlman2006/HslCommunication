using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HslCommunication.Core.Net
{
    /// <summary>
    /// 服务器程序的基础类
    /// </summary>
    public class NetworkServerBase : NetworkXBase
    {


        //服务器端的类，提供一个启动的方法，提供两个接收数据的事件
        /// <summary>
        /// 服务器引擎是否启动
        /// </summary>
        public bool IsStarted { get; protected set; } = false;

        /// <summary>
        /// 异步传入的连接申请请求
        /// </summary>
        /// <param name="iar"></param>
        protected void AsyncAcceptCallback( IAsyncResult iar )
        {
            //还原传入的原始套接字
            if (iar.AsyncState is Socket server_socket)
            {
                Socket client = null;
                try
                {
                    // 在原始套接字上调用EndAccept方法，返回新的套接字
                    client = server_socket.EndAccept( iar );
                    ThreadPool.QueueUserWorkItem( new WaitCallback( ThreadPoolLogin ), client );
                }
                catch (ObjectDisposedException ex)
                {
                    // 服务器关闭时候触发的异常，不进行记录
                    return;
                }
                catch (Exception ex)
                {
                    // 有可能刚连接上就断开了，那就不管
                    client?.Close( );
                    LogNet?.WriteException( ToString(), StringResources.SocketAcceptCallbackException, ex );
                }

                // 如果失败，尝试启动三次
                int i = 0;
                while (i < 3)
                {
                    try
                    {
                        server_socket.BeginAccept( new AsyncCallback( AsyncAcceptCallback ), server_socket );
                        break;
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep( 1000 );
                        LogNet?.WriteException( ToString( ), StringResources.SocketReAcceptCallbackException, ex );
                        i++;
                    }
                }

                if (i >= 3)
                {
                    LogNet?.WriteError( ToString( ), StringResources.SocketReAcceptCallbackException );
                    // 抛出异常，终止应用程序
                    throw new Exception( StringResources.SocketReAcceptCallbackException );
                }
            }
        }

        /// <summary>
        /// 用于登录的回调方法
        /// </summary>
        /// <param name="obj">socket对象</param>
        protected virtual void ThreadPoolLogin( object obj )
        {
            Socket socket = obj as Socket;
            socket?.Close( );
        }


        /// <summary>
        /// 服务器启动时额外的初始化信息
        /// </summary>
        protected virtual void StartInitialization( )
        {

        }


        /// <summary>
        /// 启动服务器的引擎
        /// </summary>
        /// <param name="port">指定一个端口号</param>
        public virtual void ServerStart( int port )
        {
            if (!IsStarted)
            {
                StartInitialization( );

                CoreSocket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
                CoreSocket.Bind( new IPEndPoint( IPAddress.Any, port ) );
                CoreSocket.Listen( 500 );//单次允许最后请求500个，足够小型系统应用了
                CoreSocket.BeginAccept( new AsyncCallback( AsyncAcceptCallback ), CoreSocket );
                IsStarted = true;

                LogNet?.WriteNewLine( );
                LogNet?.WriteInfo( ToString(), StringResources.NetEngineStart );
            }
        }


        /// <summary>
        /// 服务器关闭的时候需要做的事情
        /// </summary>
        protected virtual void CloseAction( )
        {

        }

        /// <summary>
        /// 关闭服务器的引擎
        /// </summary>
        public virtual void ServerClose( )
        {
            if (IsStarted)
            {
                CloseAction( );
                CoreSocket?.Close( );
                IsStarted = false;
                LogNet?.WriteInfo( ToString(), StringResources.NetEngineClose );
            }
        }


    }
}
