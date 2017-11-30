using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using HslCommunication.Core;


/*******************************************************************************
 * 
 *    网络通信中的同步通信类
 *
 *    同步通信类的客户端类需要明确收到返回消息
 * 
 *******************************************************************************/


namespace HslCommunication.Enthernet
{

    /// <summary>
    /// 用户同步访问数据的客户端类
    /// 在客户端设置参数后，调用方法即可成功读取服务器数据，甚至是文件
    /// </summary>
    public sealed class NetSimplifyClient : NetShareBase
    {
        /// <summary>
        /// 实例化一个客户端的对象，用于和服务器通信
        /// </summary>
        /// <param name="end_point">服务器的通信地址</param>
        public NetSimplifyClient(IPEndPoint end_point)
        {
            IP_END_POINT = end_point;
            LogHeaderText = "NetSimplifyClient";
        }

        /// <summary>
        /// 实例化一个客户端的对象，用于和服务器通信
        /// </summary>
        /// <param name="ipAddress">服务器的Ip地址</param>
        /// <param name="port">服务器的端口</param>
        public NetSimplifyClient(string ipAddress,int port)
        {
            IP_END_POINT = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            LogHeaderText = "NetSimplifyClient";
        }

        private IPEndPoint IP_END_POINT { get; set; } = null;

        /// <summary>
        /// 客户端向服务器进行请求，请求字符串数据
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">发送数据</param>
        /// <param name="sendStatus">发送数据时的进度报告</param>
        /// <param name="receiveStatus">接收数据时的进度报告</param>
        /// <returns></returns>
        public OperateResult<string> ReadFromServer(
            NetHandle customer,
            string send = null,
            Action<long, long> sendStatus = null,
            Action<long, long> receiveStatus = null
            )
        {
            var result = new OperateResult<string>();
            var data = string.IsNullOrEmpty(send) ? new byte[0] : Encoding.Unicode.GetBytes(send);
            var temp = ReadFromServerBase(HslCommunicationCode.Hsl_Protocol_User_String, customer, data, sendStatus, receiveStatus);
            result.IsSuccess = temp.IsSuccess;
            result.ErrorCode = temp.ErrorCode;
            result.Message = temp.Message;
            if (temp.IsSuccess)
            {
                result.Content = Encoding.Unicode.GetString(temp.Content);
            }
            temp = null;
            return result;
        }


        /// <summary>
        /// 客户端向服务器进行请求，请求字节数据
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send"></param>
        /// <param name="sendStatus">发送数据的进度报告</param>
        /// <param name="receiveStatus">接收数据的进度报告</param>
        /// <returns></returns>
        public OperateResult<byte[]> ReadFromServer(
            NetHandle customer,
            byte[] send,
            Action<long, long> sendStatus = null,
            Action<long, long> receiveStatus = null
            )
        {
            return ReadFromServerBase(HslCommunicationCode.Hsl_Protocol_User_Bytes, customer, send, sendStatus, receiveStatus);
        }

        /// <summary>
        /// 需要发送的底层数据
        /// </summary>
        /// <param name="headcode">数据的指令头</param>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">需要发送的底层数据</param>
        /// <param name="sendStatus">发送状态的进度报告，用于显示上传进度</param>
        /// <param name="receiveStatus">接收状态的进度报告，用于显示下载进度</param>
        /// <returns></returns>
        private OperateResult<byte[]> ReadFromServerBase(
            int headcode,
            int customer,
            byte[] send,
            Action<long, long> sendStatus = null,
            Action<long, long> receiveStatus = null)
        {
            var result = new OperateResult<byte[]>();

            // 创建并连接套接字
            if (!CreateSocketAndConnect(out Socket socket, IP_END_POINT, result))
            {
                return result;
            }

            // 发送并检查数据是否发送完成
            if (!SendBaseAndCheckReceive(socket, headcode, customer, send, result, sendStatus))
            {
                return result;
            }

            // 接收头数据和内容数据
            if (!ReceiveAndCheckBytes(socket, out byte[] head, out byte[] content, result, receiveStatus))
            {
                return result;
            }

            result.Content = content;
            result.IsSuccess = true;
            return result;
        }
    }


    /// <summary>
    /// 同步数据交换的服务器端类，先配置相关的参数，再启动系统
    /// </summary>
    public sealed class NetSimplifyServer : NetServerBase
    {
        /// <summary>
        /// 实例化一个对象
        /// </summary>
        public NetSimplifyServer()
        {
            LogHeaderText = "NetSimplifyServer";
        }

        #region 事件通知块

        /// <summary>
        /// 接收字符串信息的事件
        /// </summary>
        public event IEDelegate<AsyncStateOne, NetHandle, string> ReceiveStringEvent;
        /// <summary>
        /// 接收字节信息的事件
        /// </summary>
        public event IEDelegate<AsyncStateOne, NetHandle, byte[]> ReceivedBytesEvent;

        private void OnReceiveStringEvent(AsyncStateOne state, int customer, string str)
        {
            ReceiveStringEvent?.Invoke(state, customer, str);
        }
        private void OnReceivedBytesEvent(AsyncStateOne state, int customer, byte[] temp)
        {
            ReceivedBytesEvent?.Invoke(state, customer, temp);
        }
        #endregion

        #region 启动停止块

        /// <summary>
        /// 关闭网络的操作
        /// </summary>
        protected override void CloseAction()
        {
            ReceivedBytesEvent = null;
            ReceiveStringEvent = null;
            base.CloseAction();
        }


        /// <summary>
        /// 向指定的通信对象发送字符串数据
        /// </summary>
        /// <param name="state">通信对象</param>
        /// <param name="customer">用户的指令头</param>
        /// <param name="str">实际发送的字符串数据</param>
        public void SendMessage(AsyncStateOne state, int customer, string str)
        {
            OperateResult result = new OperateResult();
            SendStringAndCheckReceive(
                state.WorkSocket,
                customer,
                str,
                result,
                null,
                StringResources.SocketSendException);
            result = null;
            state?.WorkSocket.Close();
            state = null;
        }
        /// <summary>
        /// 向指定的通信对象发送字节数据
        /// </summary>
        /// <param name="state">连接对象</param>
        /// <param name="customer">用户的指令头</param>
        /// <param name="bytes">实际的数据</param>
        public void SendMessage(AsyncStateOne state, int customer, byte[] bytes)
        {
            OperateResult result = new OperateResult();
            SendBytesAndCheckReceive(
                state.WorkSocket,
                customer,
                bytes,
                result,
                null,
                StringResources.SocketSendException);
            result = null;
            state?.WorkSocket.Close();
            state = null;
        }

        /// <summary>
        /// 处理请求接收连接后的方法
        /// </summary>
        /// <param name="obj"></param>
        protected override void ThreadPoolLogin(object obj)
        {
            if (obj is Socket socket)
            {
                SynchronalReceiveCallback(socket);
            }
        }
        /// <summary>
        /// 处理异常的方法
        /// </summary>
        /// <param name="receive"></param>
        /// <param name="ex"></param>
        internal override void SocketReceiveException(AsyncStateOne receive, Exception ex)
        {
            receive.WorkSocket?.Close();
            LogNet?.WriteException(LogHeaderText, StringResources.SocketIOException, ex);
        }

        /// <summary>
        /// 数据处理中心
        /// </summary>
        /// <param name="receive"></param>
        /// <param name="protocol"></param>
        /// <param name="customer"></param>
        /// <param name="content"></param>
        internal override void DataProcessingCenter(AsyncStateOne receive, int protocol, int customer, byte[] content)
        {
            LogNet?.WriteDebug(LogHeaderText, "Protocol:" + protocol + " customer:" + customer + " ip:" + receive.GetRemoteEndPoint().Address.ToString());
            //接收数据完成，进行事件通知，优先进行解密操作
            if (protocol == HslCommunicationCode.Hsl_Protocol_User_Bytes)
            {
                //字节数据
                OnReceivedBytesEvent(receive, customer, content);
            }
            else if (protocol == HslCommunicationCode.Hsl_Protocol_User_String)
            {
                //字符串数据
                OnReceiveStringEvent(receive, customer, Encoding.Unicode.GetString(content));
            }
            else
            {
                //数据异常
                receive?.WorkSocket?.Close();
            }
        }
        
        #endregion

        #region 同步数据接收块

        /// <summary>
        /// 同步数据的接收的块
        /// </summary>
        /// <param name="socket">网络的套接字</param>
        private void SynchronalReceiveCallback(Socket socket)
        {
            if (socket != null)
            {
                OperateResult result = new OperateResult();
                if (!ReceiveAndCheckBytes(socket, out byte[] head, out byte[] content, result, null, "接收数据异常"))
                {
                    result = null;
                    return;
                }
                result = null;
                DataProcessingCenter(new AsyncStateOne() { WorkSocket = socket },
                    BitConverter.ToInt32(head, 0),
                    BitConverter.ToInt32(head, 4),
                    content);
            }
        }


        #endregion
    }
}
