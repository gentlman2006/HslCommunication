using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace HslCommunication.Core.Net
{
    /// <summary>
    /// 
    /// </summary>
    internal class FileStateObject
    {
        public int AlreadyDone { get; set; }
        

        /// <summary>
        /// 操作完成的信号
        /// </summary>
        public ManualResetEvent WaitDone { get; set; }

        /// <summary>
        /// 是否发生了错误
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// 操作的流
        /// </summary>
        public Stream Stream { get; set; }
        

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrerMsg { get; set; }
    }
}
