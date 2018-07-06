using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace HslCommunication.Core.Net
{
    internal class AsyncStateSend
    {


        /// <summary>
        /// 传输数据的对象
        /// </summary>
        internal Socket WorkSocket { get; set; }

        /// <summary>
        /// 发送的数据内容
        /// </summary>
        internal byte[] Content { get; set; }
        /// <summary>
        /// 已经发送长度
        /// </summary>
        internal int AlreadySendLength { get; set; }


        internal SimpleHybirdLock HybirdLockSend { get; set; }
    }
}
