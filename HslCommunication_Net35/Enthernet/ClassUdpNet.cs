using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace HslCommunication.Enthernet
{
    /// <summary>
    /// 一个用于UDP通信的类，服务器端用来接收所有客户端发送数据的功能
    /// </summary>
    public sealed class NetUdpServer : NetServerBase
    {
        /// <summary>
        /// 获取或设置一次接收时的数据长度，默认2KB数据长度
        /// </summary>
        public int ReceiveCacheLength { get; set; } = 2048;


        /// <summary>
        /// 根据指定的端口启动Upd侦听
        /// </summary>
        /// <param name="port"></param>
        public override void ServerStart(int port)
        {
            if (!IsStarted)
            {
                WorkSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                //绑定网络地址
                WorkSocket.Bind(new IPEndPoint(IPAddress.Any, port));

                RefreshReceive();

                LogNet?.WriteInfo(StringResources.NetEngineStart);

                IsStarted = true;
            }
        }
        /// <summary>
        /// 关闭引擎的操作
        /// </summary>
        protected override void CloseAction()
        {
            AcceptString = null;
            AcceptByte = null;
            base.CloseAction();
        }

        /// <summary>
        /// 重新开始接收数据
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// 
        private void RefreshReceive()
        {
            AsyncStateOne state = new AsyncStateOne();
            state.WorkSocket = WorkSocket;
            state.UdpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            state.BytesContent = new byte[ReceiveCacheLength];
            //WorkSocket.BeginReceiveFrom(state.BytesHead, 0, 8, SocketFlags.None, ref state.UdpEndPoint, new AsyncCallback(ReceiveAsyncCallback), state);
            WorkSocket.BeginReceiveFrom(state.BytesContent, 0, ReceiveCacheLength, SocketFlags.None, ref state.UdpEndPoint, new AsyncCallback(AsyncCallback), state);
        }

        private void AsyncCallback(IAsyncResult ar)
        {
            if (ar.AsyncState is AsyncStateOne state)
            {
                try
                {
                    int received = state.WorkSocket.EndReceiveFrom(ar, ref state.UdpEndPoint);
                    //释放连接关联
                    state.WorkSocket = null;
                    //马上开始重新接收，提供性能保障
                    RefreshReceive();
                    //处理数据
                    if (received >= HslCommunicationCode.HeadByteLength)
                    {
                        //检测令牌
                        if (NetSupport.IsTwoBytesEquel(state.BytesContent, 12, KeyToken.ToByteArray(), 0, 16))
                        {
                            state.IpEndPoint = (IPEndPoint)state.UdpEndPoint;
                            int contentLength = BitConverter.ToInt32(state.BytesContent, HslCommunicationCode.HeadByteLength - 4);
                            if (contentLength == received - HslCommunicationCode.HeadByteLength)
                            {
                                byte[] head = new byte[HslCommunicationCode.HeadByteLength];
                                byte[] content = new byte[contentLength];

                                Array.Copy(state.BytesContent, 0, head, 0, HslCommunicationCode.HeadByteLength);
                                if (contentLength > 0)
                                {
                                    Array.Copy(state.BytesContent, 32, content, 0, contentLength);
                                }

                                //解析内容
                                content = NetSupport.CommandAnalysis(head, content);

                                int protocol = BitConverter.ToInt32(head, 0);
                                int customer = BitConverter.ToInt32(head, 4);
                                //丢给数据中心处理
                                DataProcessingCenter(state, protocol, customer, content);
                            }
                            else
                            {
                                //否则记录到日志
                                LogNet?.WriteWarn($"接收到异常数据，应接收长度：{(BitConverter.ToInt32(state.BytesContent, 4) + 8)} 实际接收：{received}");
                            }
                        }
                        else
                        {
                            LogNet?.WriteWarn(StringResources.TokenCheckFailed);
                        }
                    }
                    else
                    {
                        LogNet?.WriteWarn($"接收到异常数据，长度不符合要求，实际接收：{received}");
                    }
                }
                catch (ObjectDisposedException ex)
                {
                    //主程序退出的时候触发
                }
                catch (Exception ex)
                {
                    LogNet?.WriteException(StringResources.SocketEndReceiveException, ex);
                    //重新接收，此处已经排除掉了对象释放的异常
                    RefreshReceive();
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
        //            LogHelper.SaveError(StringResources.异步数据结束挂起发送出错, ex);
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
        //            LogHelper.SaveError(StringResources.异步数据结束挂起发送出错, ex);
        //        }


        //    }
        //}

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
            LogNet?.WriteDebug("Protocol:" + protocol + " customer:" + customer + " ip:" + receive.GetRemoteEndPoint().Address.ToString());
            if (protocol == HslCommunicationCode.Hsl_Protocol_User_Bytes)
            {
                AcceptByte?.Invoke(receive, customer, content);
            }
            else if (protocol == HslCommunicationCode.Hsl_Protocol_User_String)
            {
                //接收到文本数据
                string str = Encoding.Unicode.GetString(content);
                AcceptString?.Invoke(receive, customer, str);
            }
        }

        #endregion

        #region 事件委托块



        /// <summary>
        /// 当接收到文本数据的时候,触发此事件
        /// </summary>
        public event IEDelegate<AsyncStateOne, NetHandle, string> AcceptString;


        /// <summary>
        /// 当接收到字节数据的时候,触发此事件
        /// </summary>
        public event IEDelegate<AsyncStateOne, NetHandle, byte[]> AcceptByte;


        #endregion



    }


    /// <summary>
    /// UDP客户端的类，只负责发送数据到服务器，该数据经过封装
    /// </summary>
    public sealed class NetUdpClient : NetBase
    {
        private IPEndPoint ServerEndPoint = null;
        /// <summary>
        /// 实例化对象，
        /// </summary>
        /// <param name="endpoint"></param>
        public NetUdpClient(IPEndPoint endpoint)
        {
            ServerEndPoint = endpoint;
            WorkSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }
        /// <summary>
        /// 发送字节数据到服务器
        /// </summary>
        /// <param name="customer">用户自定义的标记数据</param>
        /// <param name="data">字节数据</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public void SendMessage(NetHandle customer, byte[] data)
        {
            WorkSocket.SendTo(NetSupport.CommandBytes(customer, KeyToken, data), ServerEndPoint);
        }
        /// <summary>
        /// 发送字符串数据到服务器
        /// </summary>
        /// <param name="customer">用户自定义的标记数据</param>
        /// <param name="data">字符串数据</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public void SendMessage(NetHandle customer, string data)
        {
            WorkSocket.SendTo(NetSupport.CommandBytes(customer, KeyToken, data), ServerEndPoint);
        }
    }
}
