using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HslCommunication.Enthernet
{ 
    /// <summary>
    /// 通用设备的基础状态
    /// </summary>
    public class DeviceState
    {
        /// <summary>
        /// 设备的连接地址
        /// </summary>
        public IPEndPoint DeviceEndPoint { get; set; }

        /// <summary>
        /// 设备的连接时间
        /// </summary>
        public DateTime ConnectTime { get; set; }

        /// <summary>
        /// 网络套接字
        /// </summary>
        internal Socket WorkSocket { get; set; }

        /// <summary>
        /// 上次接收到信息的时间
        /// </summary>
        public DateTime ReceiveTime { get; set; }

        /// <summary>
        /// 设备的ip地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 缓冲内存块
        /// </summary>
        internal byte[] Buffer = new byte[1];
    }
}
