using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.LogNet;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;

/*************************************************************************************
 * 
 *    说明：
 *    本组件的所有网络类的基类。提供了一些基础的操作实现，部分实现需要集成实现
 *    
 *    重构日期：2018年3月8日 21:22:05
 * 
 *************************************************************************************/





namespace HslCommunication.Core
{
    /// <summary>
    /// 本系统所有网络类的基类，该类为抽象类，无法进行实例化
    /// </summary>
    public abstract class NetworkBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个NetworkBase对象
        /// </summary>
        public NetworkBase()
        {
            Token = Guid.Empty;
        }

        #endregion

        #region Log Support

        /// <summary>
        /// 组件的日志工具，支持日志记录
        /// </summary>
        public ILogNet LogNet { get; set; }

        /// <summary>
        /// 网络类的身份令牌
        /// </summary>
        public Guid Token { get; set; }

        #endregion

        #region Potect Member

        /// <summary>
        /// 通讯类的核心套接字
        /// </summary>
        protected Socket CoreSocket = null;


        #endregion

        /*****************************************************************************
         * 
         *    说明：
         *    下面的三个模块代码指示了如何接收数据，如何发送数据，如何连接网络
         * 
         ********************************************************************************/

        #region Reveive Content

        /// <summary>
        /// 接收固定长度的字节数组
        /// </summary>
        /// <param name="socket">网络通讯的套接字</param>
        /// <param name="length">准备接收的数据长度</param>
        /// <returns>包含了字节数据的结果类</returns>
        protected OperateResult<byte[]> Receive(Socket socket, int length)
        {
            var result = new OperateResult<byte[]>();
            var receiveDone = new ManualResetEvent(false);
            var state = new StateObject(length);

            try
            {
                state.WaitDone = receiveDone;
                state.WorkSocket = socket;

                // Begin receiving the data from the remote device.
                socket.BeginReceive(state.Buffer, state.AlreadyDealLength,
                    state.DataLength - state.AlreadyDealLength, SocketFlags.None,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception ex)
            {
                // 发生了错误，直接返回
                LogNet?.WriteException(ToString(), ex);
                result.Message = ex.Message;
                receiveDone.Close();
                return result;
            }



            // 等待接收完成，或是发生异常
            receiveDone.WaitOne();
            receiveDone.Close();



            // 接收数据失败
            if (state.IsError)
            {
                result.Message = state.ErrerMsg;
                return result;
            }


            // 远程关闭了连接
            if (state.IsClose)
            {
                result.Message = "远程关闭了连接";
                return result;
            }


            // 正常接收到数据
            result.Content = state.Buffer;
            result.IsSuccess = true;
            state.Clear();
            state = null;
            return result;
        }



        private void ReceiveCallback(IAsyncResult ar)
        {
            if (ar.AsyncState is StateObject state)
            {
                try
                {
                    Socket client = state.WorkSocket;
                    int bytesRead = client.EndReceive(ar);

                    if (bytesRead > 0)
                    {
                        // 接收到了数据
                        state.AlreadyDealLength += bytesRead;
                        if (state.AlreadyDealLength < state.DataLength)
                        {
                            // 获取接下来的所有的数据
                            client.BeginReceive(state.Buffer, state.AlreadyDealLength,
                                state.DataLength - state.AlreadyDealLength, SocketFlags.None,
                                new AsyncCallback(ReceiveCallback), state);
                        }
                        else
                        {
                            // 接收到了所有的数据，通知接收数据的线程继续
                            state.WaitDone.Set();
                        }
                    }
                    else
                    {
                        // 对方关闭了网络通讯
                        state.IsClose = true;
                        state.WaitDone.Set();
                    }
                }
                catch (Exception ex)
                {
                    state.IsError = true;
                    LogNet?.WriteException(ToString(), ex);
                    state.ErrerMsg = ex.Message;
                    state.WaitDone.Set();
                }
            }
        }



        #endregion

        #region Send Content

        /// <summary>
        /// 发送消息给套接字，直到完成的时候返回
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected OperateResult Send(Socket socket, byte[] data)
        {
            if (data == null) return OperateResult.CreateSuccessResult();

            var result = new OperateResult();
            var sendDone = new ManualResetEvent(false);
            var state = new StateObject(data.Length);

            try
            {
                state.WaitDone = sendDone;
                state.WorkSocket = socket;
                state.Buffer = data;

                socket.BeginSend(state.Buffer, state.AlreadyDealLength, state.DataLength - state.AlreadyDealLength,
                    SocketFlags.None, new AsyncCallback(SendCallBack), state);
            }
            catch (Exception ex)
            {
                // 发生了错误，直接返回
                LogNet?.WriteException(ToString(), ex);
                result.Message = ex.Message;
                sendDone.Close();
                return result;
            }

            // 等待发送完成
            sendDone.WaitOne();
            sendDone.Close();

            if (state.IsError)
            {
                result.Message = state.ErrerMsg;
                return result;
            }

            state.Clear();
            state = null;
            result.IsSuccess = true;
            result.Message = "success";

            return result;
        }

        /// <summary>
        /// 发送数据异步返回的方法
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallBack(IAsyncResult ar)
        {
            if (ar.AsyncState is StateObject state)
            {
                try
                {
                    Socket socket = state.WorkSocket;
                    int byteSend = socket.EndSend(ar);
                    state.AlreadyDealLength += byteSend;

                    if (state.AlreadyDealLength < state.DataLength)
                    {
                        // 继续发送数据
                        socket.BeginSend(state.Buffer, state.AlreadyDealLength, state.DataLength - state.AlreadyDealLength,
                            SocketFlags.None, new AsyncCallback(SendCallBack), state);
                    }
                    else
                    {
                        // 发送完成
                        state.WaitDone.Set();
                    }
                }
                catch (Exception ex)
                {
                    // 发生了异常
                    state.IsError = true;
                    LogNet?.WriteException(ToString(), ex);
                    state.ErrerMsg = ex.Message;
                    state.WaitDone.Set();
                }
            }
        }


        #endregion

        #region Socket Connect

        /// <summary>
        /// 创建一个新的socket对象并连接到远程的地址
        /// </summary>
        /// <param name="ipAddress">Ip地址</param>
        /// <param name="port">端口号</param>
        /// <returns>返回套接字的封装结果对象</returns>
        protected OperateResult<Socket> CreateSocketAndConnect(string ipAddress, int port)
        {
            return CreateSocketAndConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port));
        }


        /// <summary>
        /// 创建一个新的socket对象并连接到远程的地址
        /// </summary>
        /// <param name="endPoint">连接的目标终结点</param>
        /// <returns>返回套接字的封装结果对象</returns>
        protected OperateResult<Socket> CreateSocketAndConnect(IPEndPoint endPoint)
        {
            var result = new OperateResult<Socket>();
            var connectDone = new ManualResetEvent(false);
            var state = new StateObject();


            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                state.WaitDone = connectDone;
                state.WorkSocket = socket;
                socket.BeginConnect(endPoint, new AsyncCallback(ConnectCallBack), state);
            }
            catch (Exception ex)
            {
                // 直接失败
                LogNet?.WriteException(ToString(), ex);
                socket.Close();
                connectDone.Close();
                result.Message = ex.Message;
                return result;
            }

            // 等待连接完成
            connectDone.WaitOne();
            connectDone.Close();

            if (state.IsError)
            {
                // 连接失败
                result.Message = state.ErrerMsg;
                socket.Close();
                return result;
            }


            result.Content = socket;
            result.IsSuccess = true;
            state.Clear();
            state = null;
            return result;
        }


        /// <summary>
        /// 当连接的结果返回
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallBack(IAsyncResult ar)
        {
            if (ar.AsyncState is StateObject state)
            {
                try
                {
                    Socket socket = state.WorkSocket;
                    socket.EndConnect(ar);
                }
                catch (Exception ex)
                {
                    // 发生了异常
                    state.IsError = true;
                    LogNet?.WriteException(ToString(), ex);
                    state.ErrerMsg = ex.Message;
                    state.WaitDone.Set();
                }
            }
        }


        #endregion

        /// <summary>
        /// 获取字符串表示形式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "NetworkBase";
        }

    }

    


    internal class StateObject
    {
        #region Constructor

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        public StateObject()
        {

        }

        public StateObject(int length)
        {
            DataLength = length;
            Buffer = new byte[length];
        }
        
        #endregion
        
        #region Public Member

        public int DataLength { get; } = 32;
        

        public int AlreadyDealLength { get; set; }

        public ManualResetEvent WaitDone { get; set; }

        public byte[] Buffer { get; set; }


        public Socket WorkSocket { get; set; }

        public bool IsError { get; set; }

        public bool IsClose { get; set; }

        public string ErrerMsg { get; set; }

        #endregion

        #region Public Method

        public void Clear()
        {
            IsError = false;
            IsClose = false;
            AlreadyDealLength = 0;
            Buffer = null;
        }

        #endregion
    }

}
