using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HslCommunication.Core.Net
{
    internal class StateObject
    {
        #region Constructor

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        public StateObject()
        {

        }

        /// <summary>
        /// 实例化一个对象，指定接收或是发送的数据长度
        /// </summary>
        /// <param name="length"></param>
        public StateObject( int length )
        {
            DataLength = length;
            Buffer = new byte[length];
        }

        #endregion

        #region Public Member

        /// <summary>
        /// 本次接收或是发送的数据长度
        /// </summary>
        public int DataLength { get; } = 32;

        /// <summary>
        /// 已经处理的字节长度
        /// </summary>
        public int AlreadyDealLength { get; set; }

        /// <summary>
        /// 操作完成的信号
        /// </summary>
        public ManualResetEvent WaitDone { get; set; }


        /// <summary>
        /// 缓存器
        /// </summary>
        public byte[] Buffer { get; set; }

        /// <summary>
        /// 网络套接字
        /// </summary>
        public Socket WorkSocket { get; set; }

        /// <summary>
        /// 是否发生了错误
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// 是否关闭了通道
        /// </summary>
        public bool IsClose { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrerMsg { get; set; }

        #endregion

        #region Public Method

        /// <summary>
        /// 清空旧的数据
        /// </summary>
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
