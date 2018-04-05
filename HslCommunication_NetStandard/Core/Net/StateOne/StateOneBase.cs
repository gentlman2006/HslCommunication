using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HslCommunication.Core.Net
{
    /// <summary>
    /// 异步消息的对象
    /// </summary>
    internal class StateOneBase
    {

        /// <summary>
        /// 本次接收或是发送的数据长度
        /// </summary>
        public int DataLength { get; set; } = 32;

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
        /// 是否发生了错误
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrerMsg { get; set; }

    }
}
