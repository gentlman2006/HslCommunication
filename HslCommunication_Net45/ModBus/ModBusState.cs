using HslCommunication.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HslCommunication.ModBus
{

    /// <summary>
    /// ModBus的异步状态信息
    /// </summary>
    internal class ModBusState
    {
        #region Constructor

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        public ModBusState( )
        {
            hybirdLock = new SimpleHybirdLock( );
            ConnectTime = DateTime.Now;
            HeadByte = new byte[6];
        }

        #endregion

        /// <summary>
        /// 连接的时间
        /// </summary>
        public DateTime ConnectTime { get; private set; }

        /// <summary>
        /// 远端的地址
        /// </summary>
        public IPEndPoint IpEndPoint { get; internal set; }
        /// <summary>
        /// 远端的Ip地址
        /// </summary>
        public string IpAddress { get; internal set; }




        /// <summary>
        /// 工作套接字
        /// </summary>
        public Socket WorkSocket = null;
        /// <summary>
        /// 消息头的缓存
        /// </summary>
        public byte[] HeadByte = new byte[6];

        /// <summary>
        /// 消息头的接收长度
        /// </summary>
        public int HeadByteReceivedLength = 0;

        /// <summary>
        /// 内容数据缓存
        /// </summary>
        public byte[] Content = null;

        /// <summary>
        /// 内容数据接收长度
        /// </summary>
        public int ContentReceivedLength = 0;

        /// <summary>
        /// 回发信息的同步锁
        /// </summary>
        internal SimpleHybirdLock hybirdLock;

        /// <summary>
        /// 指示客户端是否下线，已经下线则为1
        /// </summary>
        private int isSocketOffline = 0;

        /// <summary>
        /// 判断当前的客户端是否已经下线，下线成功的话，就返回True
        /// </summary>
        /// <returns></returns>
        public bool IsModbusOffline( )
        {
            int tmp = System.Threading.Interlocked.CompareExchange( ref isSocketOffline, 1, 0 );
            return tmp == 0;
        }

        /// <summary>
        /// 清除原先的接收状态
        /// </summary>
        public void Clear( )
        {
            Array.Clear( HeadByte, 0, 6 );
            HeadByteReceivedLength = 0;
            Content = null;
            ContentReceivedLength = 0;
        }
    }
}
