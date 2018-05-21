using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using HslCommunication.Core;
using HslCommunication.LogNet;

namespace HslCommunication.Serial
{
    /// <summary>
    /// 所有串行通信类的基类，提供了一些基础的服务
    /// </summary>
    public class SerialBase
    {

        #region Constructor

        /// <summary>
        /// 实例化一个无参的构造方法
        /// </summary>
        public SerialBase( )
        {
            SP_ReadData = new SerialPort( );
            resetEvent = new AutoResetEvent( false );
            buffer = new byte[2048];
            hybirdLock = new SimpleHybirdLock( );
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 初始化串口信息，9600波特率，8位数据位，1位停止位，无奇偶校验
        /// </summary>
        public void SerialPortInni( string portName )
        {
            if (SP_ReadData.IsOpen)
            {
                return;
            }
            // 串口的端口号
            SP_ReadData.PortName = portName;
            // 串口的波特率
            SP_ReadData.BaudRate = 9600;
            // 串口的数据位
            SP_ReadData.DataBits = 8;
            // 停止位
            SP_ReadData.StopBits = StopBits.One;
            // 奇偶校验为偶数
            SP_ReadData.Parity = Parity.None;


            SP_ReadData.DataReceived += SP_ReadData_DataReceived;
        }
        /// <summary>
        /// 根据自定义初始化方法进行初始化串口信息
        /// </summary>
        public void SerialPortInni( Action<SerialPort> initi )
        {
            if (SP_ReadData.IsOpen)
            {
                return;
            }
            // 串口的端口号
            SP_ReadData.PortName = "COM5";
            // 串口的波特率
            SP_ReadData.BaudRate = 9600;
            // 串口的数据位
            SP_ReadData.DataBits = 8;
            // 停止位
            SP_ReadData.StopBits = StopBits.One;
            // 奇偶校验为偶数
            SP_ReadData.Parity = Parity.None;

            initi.Invoke( SP_ReadData );


            SP_ReadData.DataReceived += SP_ReadData_DataReceived;
        }





        /// <summary>
        /// 打开一个新的串行端口连接
        /// </summary>
        public void Open( )
        {
            if (!SP_ReadData.IsOpen)
            {
                SP_ReadData.Open( );
            }
        }

        /// <summary>
        /// 关闭端口连接
        /// </summary>
        public void Close( )
        {
            if(SP_ReadData.IsOpen)
            {
                SP_ReadData.Close( );
            }
        }

        /// <summary>
        /// 读取串口的数据
        /// </summary>
        /// <param name="send"></param>
        /// <returns></returns>
        public OperateResult<byte[]> ReadBase(byte[] send)
        {
            OperateResult<byte[]> result = null;

            hybirdLock.Enter( );

            try
            {
                isReceiveTimeout = false;
                isReceiveComplete = false;
                if (send == null) send = new byte[0];

                SP_ReadData.Write( send, 0, send.Length );
                ThreadPool.QueueUserWorkItem( new WaitCallback( CheckReceiveTimeout ), null );
                resetEvent.WaitOne( );
                isReceiveComplete = true;

                if (isReceiveTimeout)
                {
                    result = new OperateResult<byte[]>( ) { Message = "Receive Data Timeout : " + ReceiveTimeout };
                }
                else
                {
                    byte[] tmp = new byte[receiveCount];
                    Array.Copy( buffer, 0, tmp, 0, tmp.Length );

                    result = OperateResult.CreateSuccessResult( tmp );
                }
            }
            catch(Exception ex)
            {
                logNet?.WriteException( ToString( ), ex );
                result = new OperateResult<byte[]>( )
                {
                    Message = ex.Message
                };
            }
            finally
            {
                hybirdLock.Leave( );
            }

            return result;
        }

        #endregion

        #region Private Method


        private void CheckReceiveTimeout( object obj )
        {
            int receiveTimes = 0;
            while (!isReceiveComplete)
            {
                Thread.Sleep( 100 );
                receiveTimes += 100;

                if(receiveTimes >= receiveTimeout)
                {
                    if (!isReceiveComplete)
                    {
                        // 超时退出
                        isReceiveTimeout = true;
                        resetEvent.Set( );
                        break;
                    }
                }
            }
        }

        private void SP_ReadData_DataReceived( object sender, SerialDataReceivedEventArgs e )
        {
            Thread.Sleep( 20 );
            receiveCount = SP_ReadData.Read( buffer, 0, SP_ReadData.BytesToRead );
            resetEvent.Set( );
        }

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "SerialBase";
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 当前的日志情况
        /// </summary>
        public ILogNet LogNet
        {
            get { return logNet; }
            set { logNet = value; }
        }

        /// <summary>
        /// 接收数据的超时时间
        /// </summary>
        public int ReceiveTimeout
        {
            get { return receiveTimeout; }
            set { receiveTimeout = value; }
        }

        #endregion

        #region Private Member

        private SerialPort SP_ReadData = null;                    // 串口交互的核心
        private AutoResetEvent resetEvent = null;                 // 消息同步机制
        private byte[] buffer = null;                             // 接收缓冲区
        private int receiveCount = 0;                             // 接收的数据长度
        private SimpleHybirdLock hybirdLock;                      // 数据交互的锁
        private ILogNet logNet;                                   // 日志存储
        private int receiveTimeout = 5000;                        // 接收数据的超时时间
        private bool isReceiveTimeout = false;                    // 是否接收数据超时
        private bool isReceiveComplete = false;                   // 是否接收数据完成

        #endregion
    }
}
