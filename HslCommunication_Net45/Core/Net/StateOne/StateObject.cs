using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HslCommunication.Core.Net
{
    /// <summary>
    /// 网络中的异步对象
    /// </summary>
    internal class StateObject : StateOneBase
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

        /// <summary>
        /// 唯一的一串信息
        /// </summary>
        public string UniqueId { get; set; }

        #endregion

        #region Public Member

   
        /// <summary>
        /// 网络套接字
        /// </summary>
        public Socket WorkSocket { get; set; }
        

        /// <summary>
        /// 是否关闭了通道
        /// </summary>
        public bool IsClose { get; set; }
        

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
