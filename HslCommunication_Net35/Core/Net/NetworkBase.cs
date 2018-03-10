using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.LogNet;
using System.Net.Sockets;
using System.IO;
using System.Threading;

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
        public NetworkBase( )
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
        
        #region Reveive Content
        

        /// <summary>
        /// 接收一条完成的数据，使用异步接收完成，包含了指令头信息
        /// </summary>
        /// <param name="socket">已经打开的网络套接字</param>
        /// <returns></returns>
        protected OperateResult<TNetMessage> ReceiveMessage<TNetMessage>( Socket socket ) where TNetMessage : INetMessage,new()
        {
            TNetMessage netMsg = new TNetMessage();
            OperateResult<TNetMessage> result = new OperateResult<TNetMessage>();

            // 接收指令头
            OperateResult<byte[]> headResult = Receive(socket, netMsg.ProtocolHeadBytesLength);
            if(!headResult.IsSuccess)
            {
                socket.Close();
                result.CopyErrorFromOther(headResult);
                return result;
            }

            netMsg.HeadBytes = headResult.Content;
            
            if(!netMsg.CheckHeadBytesLegal(Token.ToByteArray()))
            {
                // 令牌校验失败
                socket.Close();
                LogNet?.WriteError(ToString(), StringResources.TokenCheckFailed);
                result.Message = StringResources.TokenCheckFailed;
                return result;
            }

            int contentLength = netMsg.GetContentLengthByHeadBytes();
            if (contentLength == 0)
            {
                netMsg.ContentBytes = new byte[0];
            }
            else
            {
                OperateResult<byte[]> contentResult = Receive(socket, contentLength);
                if (!headResult.IsSuccess)
                {
                    socket.Close();
                    result.CopyErrorFromOther(contentResult);
                    return result;
                }
            }
            
            result.Content = netMsg;
            result.IsSuccess = true;
            return result;
        }




        private OperateResult<byte[]> Receive(Socket socket, int length)
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
            if(state.IsError)
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

        private OperateResult Send(Socket socket, byte[] data)
        {
            return new OperateResult();
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
