using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace HslCommunication.Core.Net
{
    /// <summary>
    /// 异形客户端的异步对象
    /// </summary>
    public class AlienSession
    {
        /// <summary>
        /// 网络套接字
        /// </summary>
        public Socket Socket { get; set; }

        /// <summary>
        /// 唯一的标识
        /// </summary>
        public string DTU { get; set; }


    }
}
