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





namespace HslCommunication.Core.Net
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

        }

        #endregion

        #region Log Support

        /// <summary>
        /// 组件的日志工具，支持日志记录
        /// </summary>
        public ILogNet LogNet { get; set; }

        #endregion

        #region Potect Member

        /// <summary>
        /// 通讯类的核心套接字
        /// </summary>
        protected Socket CoreSocket = null;

        /// <summary>
        /// 通讯过程中的头子节的长度
        /// </summary>
        protected int ProtocolHeadBytesLength = 32;

        #endregion


        #region Reveive Contect

        /// <summary>
        /// 从当前的头子节文件中提取出接下来需要接收的数据长度
        /// </summary>
        /// <param name="headBytes">需要传入的头子节数据</param>
        /// <returns>返回接下来的数据内容长度</returns>
        protected virtual long GetContentLengthByHeadBytes( byte[] headBytes )
        {
            return BitConverter.ToInt32( headBytes, 0 );
        }


        /// <summary>
        /// 接收一条完成的数据，使用异步接收完成，包含了指令头信息
        /// </summary>
        /// <param name="socket">已经打开的网络套接字</param>
        /// <param name="report"></param>
        /// <param name="isReportBack"></param>
        /// <returns></returns>
        protected OperateResult<byte[], byte[]> ReceiveMessage( Socket socket, Action<long, long> report, bool isReportBack )
        {
          

        }




        private OperateResult<byte[]> Receive(Socket client, int length)
        {
            var result = new OperateResult<byte[]>();
            var receiveDone = new ManualResetEvent(false);
            var state = new StateObject(length);

            try
            {
                state.WaitDone = receiveDone;
                state.WorkSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.Buffer, state.AlreadyDealLength,
                    state.DataLength - state.AlreadyDealLength, SocketFlags.None,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception ex)
            {
                // 发生了错误，直接返回
                LogNet?.WriteException("Receive", ex);
                result.Message = ex.Message;
                receiveDone.Close();
                return result;
            }

            // 等待接收完成，或是发生异常
            receiveDone.WaitOne();

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
                            // Get the rest of the data.
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
                        // 关闭了数据发送，断开连接
                        if (state.sb.Length > 1)
                        {
                            response = state.sb.ToString();
                        }
                        // Signal that all bytes have been received.
                        receiveDone.Set();
                    }
                }
                catch (Exception e)
                {
                    sta
                    Console.WriteLine(e.ToString());
                }
            }
        }


        #endregion

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
            dataLength = length;
            Buffer = new byte[length];
        }


        #endregion



        #region Private Member

        private int dataLength = 32;
        private ManualResetEvent waitDone = null;

        #endregion


        #region Public Member

        public int DataLength
        {
            get { return dataLength; }
        }



        public int AlreadyDealLength { get; set; }

        public ManualResetEvent WaitDone
        {
            get { return waitDone; }
            set { waitDone = value; }
        }

        public byte[] Buffer { get; set; }


        public Socket WorkSocket { get; set; }

        public bool isError = false;

        public string ErrerMsg { get; set; }

        #endregion

    }

}
