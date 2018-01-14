using HslCommunication.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using HslCommunication;
using System.IO;

namespace HslCommunication.Enthernet
{

    /// <summary>
    /// 通用设备的基础网络信息
    /// </summary>
    public class DeviceNet : NetServerBase
    {

        #region Constructor

        /// <summary>
        /// 实例化一个通用的设备类
        /// </summary>
        public DeviceNet()
        {
            LogHeaderText = "DeviceNet";
            list = new List<DeviceState>( );
            lock_list = new SimpleHybirdLock( );
        }

        #endregion

        #region Connection Management


        private List<DeviceState> list;                // 所有客户端的连接对象
        private SimpleHybirdLock lock_list;            // 列表锁

        private void AddClient(DeviceState device)
        {
            lock_list.Enter( );
            list.Add( device );
            lock_list.Leave( );

            ClientOnline?.Invoke( device );
        }

        private void RemoveClient(DeviceState device)
        {
            lock_list.Enter( );
            list.Remove( device );
            device.WorkSocket?.Close( );
            lock_list.Leave( );

            ClientOffline?.Invoke( device );
        }
        

        #endregion

        #region Event Handle


        /// <summary>
        /// 当客户端上线的时候，触发此事件
        /// </summary>
        public event IEDelegate<DeviceState> ClientOnline;

        /// <summary>
        /// 当客户端下线的时候，触发此事件
        /// </summary>
        public event IEDelegate<DeviceState> ClientOffline;


        /// <summary>
        /// 当接收到文本数据的时候,触发此事件
        /// </summary>
        public event IEDelegate<DeviceState, string> AcceptString;

        #endregion

        #region Private Member

        private byte endByte = 0x0D;                   // 结束的指令


        #endregion

        /// <summary>
        /// 登录后的处理方法
        /// </summary>
        /// <param name="obj"></param>
        protected override void ThreadPoolLogin(object obj)
        {
            if (obj is Socket socket)
            {

                // 登录成功
                DeviceState stateone = new DeviceState( )
                {
                    WorkSocket = socket,
                    DeviceEndPoint = (IPEndPoint)socket.RemoteEndPoint,
                    IpAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString( ),
                    ConnectTime = DateTime.Now,
                };
                
                AddClient( stateone );

                try
                {
                    stateone.WorkSocket.BeginReceive( stateone.Buffer, 0, stateone.Buffer.Length, SocketFlags.None,
                        new AsyncCallback( ContentReceiveCallBack ), stateone );
                }
                catch (Exception ex)
                {
                    //登录前已经出错
                    RemoveClient( stateone );
                    LogNet?.WriteException( LogHeaderText, StringResources.NetClientLoginFailed, ex );
                }
            }
        }
        

        private void ContentReceiveCallBack(IAsyncResult ar)
        {
            if(ar.AsyncState is DeviceState stateone)
            {
                try
                {
                    int count = stateone.WorkSocket.EndReceive( ar );

                    if (count > 0)
                    {
                        MemoryStream ms = new MemoryStream( );
                        byte next = stateone.Buffer[0];
                        
                        while(next != endByte)
                        {
                            ms.WriteByte( next );
                            next = NetSupport.ReadBytesFromSocket( stateone.WorkSocket, 1 )[0];
                        }

                        // 接收完成
                        stateone.WorkSocket.BeginReceive( stateone.Buffer, 0, stateone.Buffer.Length, SocketFlags.None,
                            new AsyncCallback( ContentReceiveCallBack ), stateone );

                        string read = Encoding.ASCII.GetString( ms.ToArray( ) );
                        ms.Dispose( );

                        lock_list.Enter( );
                        stateone.ReceiveTime = DateTime.Now;
                        lock_list.Leave( );
                        AcceptString?.Invoke( stateone, read );
                    }
                    else
                    {
                        stateone.WorkSocket.BeginReceive( stateone.Buffer, 0, stateone.Buffer.Length, SocketFlags.None,
                            new AsyncCallback( ContentReceiveCallBack ), stateone );
                    }
                }
                catch(Exception ex)
                {
                    //登录前已经出错
                    RemoveClient( stateone );
                    LogNet?.WriteException( LogHeaderText, StringResources.NetClientLoginFailed, ex );
                }
            }
        }

    }

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
