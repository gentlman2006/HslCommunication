using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using HslCommunication.Core;

namespace HslCommunication.Profinet.Melsec
{
//    internal sealed class MelsecNetMultiAsync : PlcNetBase
//    {


//        /// <summary>
//        /// 实例化一个多PLC请求的对象
//        /// </summary>
//        /// <param name="networkNumber">要求所有网络号一致</param>
//        /// <param name="networkStationNumber">要求所有站号一致</param>
//        /// <param name="type">要求所有读取类型一致</param>
//        /// <param name="address">要求所有起始地址一致</param>
//        /// <param name="length">要求所有读取数据长度一致</param>
//        /// <param name="timeout">连接PLC过程中的超时时间</param>
//        /// <param name="readCycle">每次请求访问的间隔时间</param>
//        /// <param name="allAddress">所有等待访问的PLC的IP地址</param>
//        /// <param name="portMain">访问PLC时的主端口</param>
//        /// <param name="portBackup">访问PLC时的备用端口</param>
//        public MelsecNetMultiAsync(
//            byte networkNumber,
//            byte networkStationNumber,
//            MelsecMcDataType type,
//            ushort address,
//            ushort length,
//            int timeout,
//            int readCycle,
//            IPAddress[] allAddress,
//            int portMain,
//            int portBackup )
//        {
//            NetworkNumber = networkNumber;
//            NetworkStationNumber = networkStationNumber;
//            MelsecType = type;
//            Address = address;
//            Length = length;
//            Timeout = timeout;
//            ReadCycle = readCycle;
//            //初始化数据对象
//            EveryMachineLength = GetLengthFromPlcType( type, length );
//            BytesResult = new byte[EveryMachineLength * allAddress.Length];

//            PlcStates = new PlcStateOne[allAddress.Length];
//            for (int i = 0; i < PlcStates.Length; i++)
//            {
//                PlcStates[i] = new PlcStateOne( )
//                {
//                    Index = i,
//                    PlcDataHead = new byte[9],
//                    PlcDataContent = new byte[EveryMachineLength],
//                    LengthDataContent = 0,
//                    LengthDataHead = 0,
//                    IsConnect = false,
//                    PlcIpAddress = allAddress[i],
//                    PortMain = portMain,
//                    PortBackup = portBackup,
//                };
//            }
//            //启动线程
//            Thread thread = new Thread( new ThreadStart( ThreadDealWithTimeout ) )
//            {
//                IsBackground = true
//            };
//            thread.Start( );
//        }


//        /*********************************************************************************************
//         * 
//         *    拷贝的数据方式仍然需要修改
//         * 
//         * 
//         *********************************************************************************************/


//        private MelsecMcDataType MelsecType = MelsecMcDataType.D;
//        private ushort Address = 0;
//        private ushort Length = 0;
//        /// <summary>
//        /// 超时时间
//        /// </summary>
//        private int Timeout = 600;
//        /// <summary>
//        /// 访问周期
//        /// </summary>
//        private int ReadCycle = 1000;
//        /// <summary>
//        /// 系统的连接状态，0未连接，1连接中
//        /// </summary>
//        private int ConnectStatus = 0;
//        /// <summary>
//        /// 用于存储结果的字节数组
//        /// </summary>
//        private byte[] BytesResult = null;
//        /// <summary>
//        /// 每台设备的长度
//        /// </summary>
//        private int EveryMachineLength = 0;
//        /// <summary>
//        /// 接收到所有的PLC数据时候触发
//        /// </summary>
//        public event IEDelegate<byte[]> OnReceivedData = null;

//        private PlcStateOne[] PlcStates = null;
//        /// <summary>
//        /// 维护超时连接的线程方法
//        /// </summary>
//        private void ThreadDealWithTimeout()
//        {
//            LogNet?.WriteInfo( LogHeaderText, "waitting one second" );
//            Thread.Sleep( 1000 );// 刚启动的时候进行休眠一小会
//            LogNet?.WriteInfo( LogHeaderText, "begining recyle for reading plc" );
//            while (true)
//            {
//                DateTime firstTime = DateTime.Now;// 连接时间

//                TimerCallBack( );// 启动访问PLC

//                while ((DateTime.Now - firstTime).TotalMilliseconds < Timeout)
//                {
//                    // 超时时间等待
//                }
//                // 连接超时处理
//                for (int i = 0; i < PlcStates.Length; i++)
//                {
//                    if (!PlcStates[i].IsConnect) PlcStates[i].WorkSocket.Close( );
//                }

//                while ((DateTime.Now - firstTime).TotalMilliseconds < ReadCycle)
//                {
//                    // 等待下次连接
//                }

//            }
//        }
//        private void TimerCallBack()
//        {
//            // 如果已经连接就返回，此处采用原子操作实现
//            if (Interlocked.CompareExchange( ref ConnectStatus, 1, 0 ) == 0)
//            {
//                m_ac = new AsyncCoordinator( );
//                for (int i = 0; i < PlcStates.Length; i++)
//                {
//                    PlcStates[i].IsConnect = false;
//                    PlcStates[i].WorkSocket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
//                    PlcStates[i].WorkSocket.BeginConnect( new IPEndPoint( PlcStates[i].PlcIpAddress, PlcStates[i].GetPort( ) ), new AsyncCallback( PlcConnectCallBack ), PlcStates[i] );
//                    PlcStates[i].LengthDataHead = 0;
//                    PlcStates[i].LengthDataContent = 0;

//                    m_ac.AboutToBegin( 1 );
//                }
//                // 启动检查连接情况
//                m_ac.AllBegun( AllDown );
//            }
//        }

//        private byte[] ConnectWrongHead = new byte[] { 0x00, 0x01 };

//        private void PlcConnectCallBack( IAsyncResult ar )
//        {
//            if (ar.AsyncState is PlcStateOne stateone)
//            {
//                try
//                {
//                    stateone.WorkSocket.EndConnect( ar );
//                    stateone.WorkSocket.BeginReceive(
//                        stateone.PlcDataHead,
//                        stateone.LengthDataHead,
//                        stateone.PlcDataHead.Length - stateone.LengthDataHead,
//                        SocketFlags.None,
//                        new AsyncCallback( PlcHeadReceiveCallBack ),
//                        stateone );
//                    stateone.IsConnect = true;// 指示访问成功
//                }
//                catch (Exception ex)
//                {
//                    LogNet?.WriteException( ToString(), "connect failed", ex );
//                    // 访问失败
//                    stateone.WorkSocket.Close( );
//                    // 初始化数据
//                    Array.Copy( ConnectWrongHead, 0, BytesResult, stateone.Index * EveryMachineLength, 2 );
//                    // 更换端口
//                    stateone.ChangePort( );
//                    // 结束任务
//                    m_ac.JustEnded( );
//                }
//            }
//        }

//        private void PlcHeadReceiveCallBack( IAsyncResult ar )
//        {
//            if (ar.AsyncState is PlcStateOne stateone)
//            {
//                try
//                {
//                    stateone.LengthDataHead += stateone.WorkSocket.EndReceive( ar );
//                    if (stateone.LengthDataHead < stateone.PlcDataHead.Length)
//                    {
//                        // 继续接收头格式
//                        stateone.WorkSocket.BeginReceive(
//                        stateone.PlcDataHead,
//                        stateone.LengthDataHead,
//                        stateone.PlcDataHead.Length - stateone.LengthDataHead,
//                        SocketFlags.None,
//                        new AsyncCallback( PlcHeadReceiveCallBack ),
//                        stateone );
//                    }
//                    else
//                    {
//                        // 计算接下来的接收长度，最少还有2个长度的字节数据
//                        int NeedReceived = BitConverter.ToUInt16( stateone.PlcDataHead, 7 );
//                        stateone.PlcDataContent = new byte[NeedReceived];
//                        // 接收内容
//                        stateone.WorkSocket.BeginReceive(
//                        stateone.PlcDataContent,
//                        0,
//                        stateone.PlcDataContent.Length - stateone.LengthDataContent,
//                        SocketFlags.None,
//                        new AsyncCallback( PlcContentReceiveCallBack ),
//                        stateone );
//                    }
//                }
//                catch (Exception ex)
//                {
//                    LogNet?.WriteException( LogHeaderText, "Head receive", ex );
//                    // 由于未知原因，数据接收失败
//                    stateone.WorkSocket.Close( );
//                    // 初始化数据
//                    Array.Copy( ConnectWrongHead, 0, BytesResult, stateone.Index * EveryMachineLength, 2 );
//                    // 结束任务
//                    m_ac.JustEnded( );
//                }
//            }
//        }

//        private void PlcContentReceiveCallBack( IAsyncResult ar )
//        {
//            if (ar.AsyncState is PlcStateOne stateone)
//            {
//                try
//                {
//                    stateone.LengthDataContent += stateone.WorkSocket.EndReceive( ar );
//                    if (stateone.LengthDataHead < stateone.PlcDataHead.Length)
//                    {
//                        // 继续接内容格式
//                        stateone.WorkSocket.BeginReceive(
//                        stateone.PlcDataContent,
//                        stateone.LengthDataContent,
//                        stateone.PlcDataContent.Length - stateone.LengthDataContent,
//                        SocketFlags.None,
//                        new AsyncCallback( PlcContentReceiveCallBack ),
//                        stateone );
//                    }
//                    else
//                    {
//                        // 所有内容接收完成
//                        int error = BitConverter.ToUInt16( stateone.PlcDataContent, 0 );
//                        if (error > 0)
//                        {
//                            Array.Copy( ConnectWrongHead, 0, BytesResult, stateone.Index * EveryMachineLength, 2 );
//                        }
//                        else
//                        {
//                            Array.Copy( stateone.PlcDataContent, 0, BytesResult, stateone.Index * EveryMachineLength, stateone.PlcDataContent.Length );
//                        }
//                        // 关闭连接
//                        stateone.WorkSocket.Close( );
//                        // 结束任务
//                        m_ac.JustEnded( );
//                    }
//                }
//                catch (Exception ex)
//                {
//                    LogNet?.WriteException( LogHeaderText, "Data receive", ex );
//                    // 由于未知原因，数据接收失败
//                    stateone.WorkSocket.Close( );
//                    // 初始化数据
//                    Array.Copy( ConnectWrongHead, 0, BytesResult, stateone.Index * EveryMachineLength, 2 );
//                    // 结束任务
//                    m_ac.JustEnded( );
//                }
//            }
//        }

//        private void AllDown( CoordinationStatus status )
//        {
//            // 此处没有取消和超时状态，直接完成
//            if (status == CoordinationStatus.AllDone)
//            {
//                Interlocked.Exchange( ref ConnectStatus, 0 );
//                LogNet?.WriteDebug( LogHeaderText, "All bytes read complete." );
//                OnReceivedData?.Invoke( BytesResult.ToArray( ) );
//            }
//        }

//        private AsyncCoordinator m_ac = new AsyncCoordinator( );
//    }
    
}
