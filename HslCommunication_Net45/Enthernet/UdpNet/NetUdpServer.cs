using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using HslCommunication.Core;
using HslCommunication.Core.Net;

namespace HslCommunication.Enthernet
{
    /// <summary>
    /// Udp网络的服务器端类
    /// </summary>
    public class NetUdpServer : NetworkServerBase
    {

        /// <summary>
        /// 获取或设置一次接收时的数据长度，默认2KB数据长度
        /// </summary>
        public int ReceiveCacheLength { get; set; } = 2048;


        /// <summary>
        /// 根据指定的端口启动Upd侦听
        /// </summary>
        /// <param name="port">端口号信息</param>
        public override void ServerStart( int port )
        {
            if (!IsStarted)
            {
                CoreSocket = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );

                //绑定网络地址
                CoreSocket.Bind( new IPEndPoint( IPAddress.Any, port ) );
                RefreshReceive( );
                LogNet?.WriteInfo( ToString(), StringResources.Language.NetEngineStart );
                IsStarted = true;
            }
        }

        /// <summary>
        /// 关闭引擎的操作
        /// </summary>
        protected override void CloseAction( )
        {
            AcceptString = null;
            AcceptByte = null;
            base.CloseAction( );
        }

        /// <summary>
        /// 重新开始接收数据
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        private void RefreshReceive( )
        {
            AppSession session = new AppSession( );
            session.WorkSocket = CoreSocket;
            session.UdpEndPoint = new IPEndPoint( IPAddress.Any, 0 );
            session.BytesContent = new byte[ReceiveCacheLength];
            // WorkSocket.BeginReceiveFrom(state.BytesHead, 0, 8, SocketFlags.None, ref state.UdpEndPoint, new AsyncCallback(ReceiveAsyncCallback), state);
            CoreSocket.BeginReceiveFrom( session.BytesContent, 0, ReceiveCacheLength, SocketFlags.None, ref session.UdpEndPoint, new AsyncCallback( AsyncCallback ), session );
        }

        private void AsyncCallback( IAsyncResult ar )
        {
            if (ar.AsyncState is AppSession session)
            {
                try
                {
                    int received = session.WorkSocket.EndReceiveFrom( ar, ref session.UdpEndPoint );
                    // 释放连接关联
                    session.WorkSocket = null;
                    // 马上开始重新接收，提供性能保障
                    RefreshReceive( );
                    // 处理数据
                    if (received >= HslProtocol.HeadByteLength)
                    {
                        // 检测令牌
                        if (CheckRemoteToken( session.BytesContent ))
                        {
                            session.IpEndPoint = (IPEndPoint)session.UdpEndPoint;
                            int contentLength = BitConverter.ToInt32( session.BytesContent, HslProtocol.HeadByteLength - 4 );
                            if (contentLength == received - HslProtocol.HeadByteLength)
                            {
                                byte[] head = new byte[HslProtocol.HeadByteLength];
                                byte[] content = new byte[contentLength];

                                Array.Copy( session.BytesContent, 0, head, 0, HslProtocol.HeadByteLength );
                                if (contentLength > 0)
                                {
                                    Array.Copy( session.BytesContent, 32, content, 0, contentLength );
                                }

                                // 解析内容
                                content = HslProtocol.CommandAnalysis( head, content );

                                int protocol = BitConverter.ToInt32( head, 0 );
                                int customer = BitConverter.ToInt32( head, 4 );
                                // 丢给数据中心处理
                                DataProcessingCenter( session, protocol, customer, content );
                            }
                            else
                            {
                                // 否则记录到日志
                                LogNet?.WriteWarn( ToString(), $"Should Rece：{(BitConverter.ToInt32( session.BytesContent, 4 ) + 8)} Actual：{received}" );
                            }
                        }
                        else
                        {
                            LogNet?.WriteWarn( ToString( ), StringResources.Language.TokenCheckFailed );
                        }
                    }
                    else
                    {
                        LogNet?.WriteWarn( ToString( ), $"Receive error, Actual：{received}" );
                    }
                }
                catch (ObjectDisposedException)
                {
                    //主程序退出的时候触发
                }
                catch (Exception ex)
                {
                    LogNet?.WriteException( ToString( ), StringResources.Language.SocketEndReceiveException, ex );
                    //重新接收，此处已经排除掉了对象释放的异常
                    RefreshReceive( );
                }
                finally
                {
                    //state = null;
                }

            }
        }

        /***********************************************************************************************************
         * 
         *    无法使用如下的字节头接收来确认网络传输，总是报错为最小
         * 
         ***********************************************************************************************************/

        //private void ReceiveAsyncCallback(IAsyncResult ar)
        //{
        //    if (ar.AsyncState is AsyncStateOne state)
        //    {
        //        try
        //        {
        //            state.AlreadyReceivedHead += state.WorkSocket.EndReceiveFrom(ar, ref state.UdpEndPoint);
        //            if (state.AlreadyReceivedHead < state.HeadLength)
        //            {
        //                //接续接收头数据
        //                WorkSocket.BeginReceiveFrom(state.BytesHead, state.AlreadyReceivedHead, state.HeadLength - state.AlreadyReceivedHead, SocketFlags.None,
        //                    ref state.UdpEndPoint, new AsyncCallback(ReceiveAsyncCallback), state);
        //            }
        //            else
        //            {
        //                //开始接收内容
        //                int ReceiveLenght = BitConverter.ToInt32(state.BytesHead, 4);
        //                if (ReceiveLenght > 0)
        //                {
        //                    state.BytesContent = new byte[ReceiveLenght];
        //                    WorkSocket.BeginReceiveFrom(state.BytesContent, state.AlreadyReceivedContent, state.BytesContent.Length - state.AlreadyReceivedContent, 
        //                        SocketFlags.None, ref state.UdpEndPoint, new AsyncCallback(ContentReceiveAsyncCallback), state);
        //                }
        //                else
        //                {
        //                    //没有内容了
        //                    ThreadDealWithReveice(state, BitConverter.ToInt32(state.BytesHead, 0), state.BytesContent);
        //                    state = null;
        //                    RefreshReceive();
        //                }
        //            }
        //        }
        //        catch(Exception ex)
        //        {
        //            LogHelper.SaveError(StringResources.Language.异步数据结束挂起发送出错, ex);
        //        }


        //    }
        //}

        //private void ContentReceiveAsyncCallback(IAsyncResult ar)
        //{
        //    if (ar.AsyncState is AsyncStateOne state)
        //    {
        //        try
        //        {
        //            state.AlreadyReceivedContent += state.WorkSocket.EndReceiveFrom(ar, ref state.UdpEndPoint);
        //            if (state.AlreadyReceivedContent < state.BytesContent.Length)
        //            {
        //                //还需要继续接收
        //                WorkSocket.BeginReceiveFrom(state.BytesContent, state.AlreadyReceivedContent, state.BytesContent.Length - state.AlreadyReceivedContent,
        //                        SocketFlags.None, ref state.UdpEndPoint, new AsyncCallback(ContentReceiveAsyncCallback), state);
        //            }
        //            else
        //            {
        //                //接收完成了
        //                ThreadDealWithReveice(state, BitConverter.ToInt32(state.BytesHead, 0), new byte[0]);
        //                state = null;
        //                RefreshReceive();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            LogHelper.SaveError(StringResources.Language.异步数据结束挂起发送出错, ex);
        //        }


        //    }
        //}

        #region Data Process Center

        /// <summary>
        /// 数据处理中心
        /// </summary>
        /// <param name="receive"></param>
        /// <param name="protocol"></param>
        /// <param name="customer"></param>
        /// <param name="content"></param>
        internal override void DataProcessingCenter( AppSession receive, int protocol, int customer, byte[] content )
        {
            if (protocol == HslProtocol.ProtocolUserBytes)
            {
                AcceptByte?.Invoke( receive, customer, content );
            }
            else if (protocol == HslProtocol.ProtocolUserString)
            {
                // 接收到文本数据
                string str = Encoding.Unicode.GetString( content );
                AcceptString?.Invoke( receive, customer, str );
            }
        }

        #endregion

        #region Event Handle
        
        /// <summary>
        /// 当接收到文本数据的时候,触发此事件
        /// </summary>
        public event Action<AppSession, NetHandle, string> AcceptString;


        /// <summary>
        /// 当接收到字节数据的时候,触发此事件
        /// </summary>
        public event Action<AppSession, NetHandle, byte[]> AcceptByte;


        #endregion
        
        #region Object Override

        /// <summary>
        /// 获取本对象的字符串表示形式
        /// </summary>
        /// <returns></returns>
        public override string ToString( )
        {
            return "NetUdpServer";
        }
        #endregion

    }
}
