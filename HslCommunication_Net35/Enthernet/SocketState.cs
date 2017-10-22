using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HslCommunication.Enthernet
{

    /************************************************************************
     * 
     *    用于socket通信中的异步传送的状态
     *    
     *    For asynchronous socket communication transmission status
     * 
     ************************************************************************/



    /************************************************************************
     * 
     *    2017年5月18日 14:30:10
     *    
     *    准备代码重构，将数据发送和接收的对象进行分割开来
     * 
     ************************************************************************/

    /// <summary>
    /// 异步状态的基类
    /// </summary>
    public class AsyncStateBase
    {
        /// <summary>
        /// 传输数据的对象
        /// </summary>
        internal Socket WorkSocket { get; set; }
        /// <summary>
        /// 获取远程的网络地址
        /// </summary>
        /// <returns></returns>
        public IPEndPoint GetRemoteEndPoint()
        {
            return (IPEndPoint)WorkSocket?.RemoteEndPoint;
        }
        
    }






    /// <summary>
    /// 发送数据的异步状态
    /// </summary>
    internal class AsyncStateSend : AsyncStateBase
    {
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
    

    /// <summary>
    /// 异步多客户端网络的对象
    /// </summary>
    public class AsyncStateOne : AsyncStateBase
    {
        /// <summary>
        /// 实例化一个对象
        /// </summary>
        public AsyncStateOne()
        {
            ClientUniqueID = Guid.NewGuid().ToString("N");

            HybirdLockSend = new SimpleHybirdLock();
        }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; internal set; }
        /// <summary>
        /// 此连接对象连接的远程客户端
        /// </summary>
        public IPEndPoint IpEndPoint { get; internal set; }
        /// <summary>
        /// 远程对象的别名
        /// </summary>
        public string LoginAlias { get; set; }
        /// <summary>
        /// 心跳验证的时间点
        /// </summary>
        public DateTime HeartTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 客户端的类型
        /// </summary>
        public string ClientType { get; set; }
        /// <summary>
        /// 客户端唯一的标识
        /// </summary>
        public string ClientUniqueID { get; private set; }

        /// <summary>
        /// UDP通信中的远程端
        /// </summary>
        internal EndPoint UdpEndPoint = null;


        /// <summary>
        /// 指令头缓存
        /// </summary>
        internal byte[] BytesHead { get; set; } = new byte[HslCommunicationCode.HeadByteLength];
        /// <summary>
        /// 已经接收的指令头长度
        /// </summary>
        internal int AlreadyReceivedHead { get; set; }
        /// <summary>
        /// 数据内容缓存
        /// </summary>
        internal byte[] BytesContent { get; set; }
        /// <summary>
        /// 已经接收的数据内容长度
        /// </summary>
        internal int AlreadyReceivedContent { get; set; }

        /// <summary>
        /// 清除本次的接收内容
        /// </summary>
        internal void Clear()
        {
            BytesHead = new byte[HslCommunicationCode.HeadByteLength];
            AlreadyReceivedHead = 0;
            BytesContent = null;
            AlreadyReceivedContent = 0;
        }



        internal SimpleHybirdLock HybirdLockSend { get; set; }
    }
    



    ///// <summary>
    ///// 网络会话的基类
    ///// </summary>
    //public abstract class AppSessionBase
    //{
        
    //    /// <summary>
    //    /// 连接对象的唯一的标识
    //    /// </summary>
    //    public string UniqueID { get; protected set; }
        
    //    /// <summary>
    //    /// IP地址
    //    /// </summary>
    //    public string IpAddress { get; protected set; }

    //    /// <summary>
    //    /// 客户端的类型
    //    /// </summary>
    //    public string ClientType { get; internal set; }
    //    /// <summary>
    //    /// 会话的连接时间
    //    /// </summary>
    //    public DateTime ConnectedTime { get; protected set; }
    //    /// <summary>
    //    /// 会话的激活时间
    //    /// </summary>
    //    public DateTime ActiveTime { get; internal set; }



    //    /// <summary>
    //    /// 网络终结点
    //    /// </summary>
    //    protected IPEndPoint iPEndPoint;                
    //    /// <summary>
    //    /// 网络套接字
    //    /// </summary>
    //    internal Socket workSocket;                   
    //    /// <summary>
    //    /// 混合同步锁
    //    /// </summary>
    //    protected SimpleHybirdLock hybirdLock; 




    //    /// <summary>
    //    /// 进入发送数据的锁
    //    /// </summary>
    //    internal void EnterSendLocked()
    //    {
    //        hybirdLock?.Enter();
    //    }
    //    /// <summary>
    //    /// 离开发送数据的锁
    //    /// </summary>
    //    internal void LeaveSendLocked()
    //    {
    //        hybirdLock?.Leave();
    //    }
    //}


    ///// <summary>
    ///// 客户端会话的状态
    ///// </summary>
    //public class AppComplexSession : AppSessionBase
    //{
    //    /// <summary>
    //    /// 实例化一个对象
    //    /// </summary>
    //    public AppComplexSession(Socket socket)
    //    {
    //        hybirdLock = new SimpleHybirdLock();
    //        UniqueID = Guid.NewGuid().ToString("N");
    //        workSocket = socket;
    //        IpEndPoint = workSocket?.RemoteEndPoint as IPEndPoint;
    //        LoginAlias = "";
    //        ConnectedTime = DateTime.Now;
    //        ActiveTime = DateTime.Now;
    //    }

        
    //    /// <summary>
    //    /// 此连接对象连接的远程客户端
    //    /// </summary>
    //    public IPEndPoint IpEndPoint
    //    {
    //        get { return iPEndPoint; }
    //        set
    //        {
    //            iPEndPoint = value;
    //            IpAddress = value?.Address?.ToString();
    //        }
    //    }
        
    //    /// <summary>
    //    /// 远程对象的别名
    //    /// </summary>
    //    public string LoginAlias { get; internal set; }





    //    /// <summary>
    //    /// 指令头缓存
    //    /// </summary>
    //    internal byte[] BytesHead { get; set; } = new byte[HslCommunicationCode.HeadByteLength];
    //    /// <summary>
    //    /// 已经接收的指令头长度
    //    /// </summary>
    //    internal int AlreadyReceivedHead { get; set; }
    //    /// <summary>
    //    /// 数据内容缓存
    //    /// </summary>
    //    internal byte[] BytesContent { get; set; }
    //    /// <summary>
    //    /// 已经接收的数据内容长度
    //    /// </summary>
    //    internal int AlreadyReceivedContent { get; set; }

    //    /// <summary>
    //    /// 清除本次的接收内容
    //    /// </summary>
    //    internal void Clear()
    //    {
    //        BytesHead = new byte[HslCommunicationCode.HeadByteLength];
    //        AlreadyReceivedHead = 0;
    //        BytesContent = null;
    //        AlreadyReceivedContent = 0;
    //    }

    //}

}
