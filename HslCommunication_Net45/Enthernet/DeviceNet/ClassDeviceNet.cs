using HslCommunication.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using HslCommunication;
using System.IO;
using HslCommunication.Core.Net;

namespace HslCommunication.Enthernet
{

    /// <summary>
    /// 通用设备的基础网络信息
    /// </summary>
    public class DeviceNet : NetworkServerBase
    {

        #region Constructor

        /// <summary>
        /// 实例化一个通用的设备类
        /// </summary>
        public DeviceNet()
        {
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
        public event Action<DeviceState> ClientOnline;

        /// <summary>
        /// 当客户端下线的时候，触发此事件
        /// </summary>
        public event Action<DeviceState> ClientOffline;


        /// <summary>
        /// 按照ASCII文本的方式进行触发接收的数据
        /// </summary>
        public event Action<DeviceState, string> AcceptString;

        /// <summary>
        /// 按照字节的方式进行触发接收的数据
        /// </summary>
        public event Action<DeviceState, byte[]> AcceptBytes;

        #endregion

        #region Private Member

        private readonly byte endByte = 0x0D;                   // 结束的指令


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
                    LogNet?.WriteException( ToString(), StringResources.Language.NetClientLoginFailed, ex );
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
                            byte[] buffer = new byte[1];
                            stateone.WorkSocket.Receive( buffer, 0, 1, SocketFlags.None );
                            next = buffer[0];
                        }

                        // 接收完成
                        stateone.WorkSocket.BeginReceive( stateone.Buffer, 0, stateone.Buffer.Length, SocketFlags.None,
                            new AsyncCallback( ContentReceiveCallBack ), stateone );


                        byte[] receive =ms.ToArray( );
                        ms.Dispose( );

                        lock_list.Enter( );
                        stateone.ReceiveTime = DateTime.Now;
                        lock_list.Leave( );
                        AcceptBytes?.Invoke( stateone, receive );
                        AcceptString?.Invoke( stateone, Encoding.ASCII.GetString( receive ));
                    }
                    else
                    {
                        RemoveClient( stateone );
                        LogNet?.WriteInfo( ToString( ), StringResources.Language.NetClientOffline );
                    }
                }
                catch(Exception ex)
                {
                    //登录前已经出错
                    RemoveClient( stateone );
                    LogNet?.WriteException( ToString( ), StringResources.Language.NetClientLoginFailed, ex );
                }
            }
        }

    }


}
