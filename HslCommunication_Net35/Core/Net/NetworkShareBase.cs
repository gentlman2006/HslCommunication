using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace HslCommunication.Core
{



    /// <summary>
    /// 异步多客户端和服务器共有的基类 
    /// </summary>
    public class NetworkShareBase : NetworkBase
    {



        protected void BeginReveive()
        {

        }

        
    }


    /// <summary>
    /// 应用与复杂客户端和复杂服务器的中间通话对象
    /// </summary>
    public class AppSession
    {
        /// <summary>
        /// 实例化一个对象
        /// </summary>
        public AppSession()
        {
            UniqueId = BasicFramework.SoftBasic.GetUniqueStringByGuidAndRandom();
            OnlineTime = DateTime.Now;
        }


        /// <summary>
        /// 核心的通讯用的套接字
        /// </summary>
        internal Socket CoreSocket { get; set; }
        /// <summary>
        /// 接收的同步锁操作
        /// </summary>
        internal SimpleHybirdLock ReveiveLock { get; set; }
        /// <summary>
        /// 发送的同步锁操作
        /// </summary>
        internal SimpleHybirdLock SendLock { get; set; }
        /// <summary>
        /// 头子节的信息
        /// </summary>
        internal byte[] HeadBytes { get; set; }
        /// <summary>
        /// 当前已经接收的数据长度
        /// </summary>
        internal int ReceivedLength { get; set; }

        /// <summary>
        /// 当前会话的唯一ID信息
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// 上线时间
        /// </summary>
        public DateTime OnlineTime { get; set; }
    }

}
