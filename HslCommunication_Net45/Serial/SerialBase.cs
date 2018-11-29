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
            hybirdLock = new SimpleHybirdLock( );
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 初始化串口信息，9600波特率，8位数据位，1位停止位，无奇偶校验
        /// </summary>
        /// <param name="portName">端口号信息，例如"COM3"</param>
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


            //SP_ReadData.DataReceived += SP_ReadData_DataReceived;
        }

        /// <summary>
        /// 根据自定义初始化方法进行初始化串口信息
        /// </summary>
        /// <param name="initi">初始化的委托方法</param>
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


            //SP_ReadData.DataReceived += SP_ReadData_DataReceived;
        }




        /// <summary>
        /// 打开一个新的串行端口连接
        /// </summary>
        public void Open( )
        {
            if (!SP_ReadData.IsOpen)
            {
                SP_ReadData.Open( );
                InitializationOnOpen( );
            }
        }

        /// <summary>
        /// 获取一个值，指示串口是否处于打开状态
        /// </summary>
        /// <returns>是或否</returns>
        public bool IsOpen( )
        {
            return SP_ReadData.IsOpen;
        }

        /// <summary>
        /// 关闭端口连接
        /// </summary>
        public void Close( )
        {
            if(SP_ReadData.IsOpen)
            {
                ExtraOnClose( );
                SP_ReadData.Close( );
            }
        }

        /// <summary>
        /// 读取串口的数据
        /// </summary>
        /// <param name="send">发送的原始字节数据</param>
        /// <returns>带接收字节的结果对象</returns>
        public OperateResult<byte[]> ReadBase(byte[] send)
        {
            hybirdLock.Enter( );

            OperateResult sendResult = SPSend( SP_ReadData, send );
            if (!sendResult.IsSuccess)
            {
                hybirdLock.Leave( );
                return OperateResult.CreateFailedResult<byte[]>( sendResult );
            }

            OperateResult<byte[]> receiveResult = SPReceived( SP_ReadData, true );
            hybirdLock.Leave( );

            return receiveResult;
        }

        #endregion

        #region virtual Method

        /// <summary>
        /// 检查当前接收的字节数据是否正确的
        /// </summary>
        /// <param name="rBytes">输入字节</param>
        /// <returns>检查是否正确</returns>
        protected virtual bool CheckReceiveBytes(byte[] rBytes )
        {
            return true;
        }

        #endregion

        #region Initialization And Extra

        /// <summary>
        /// 在打开端口时的初始化方法，按照协议的需求进行必要的重写
        /// </summary>
        /// <returns>是否初始化成功</returns>
        protected virtual OperateResult InitializationOnOpen( )
        {
            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 在将要和服务器进行断开的情况下额外的操作，需要根据对应协议进行重写
        /// </summary>
        /// <returns>当断开连接时额外的操作结果</returns>
        protected virtual OperateResult ExtraOnClose( )
        {
            return OperateResult.CreateSuccessResult( );
        }

        #endregion

        #region Private Method
        
        /// <summary>
        /// 发送数据到串口里去
        /// </summary>
        /// <param name="serialPort">串口对象</param>
        /// <param name="data">字节数据</param>
        /// <returns>是否发送成功</returns>
        protected virtual OperateResult SPSend( SerialPort serialPort, byte[] data )
        {
            if (data != null && data.Length > 0)
            {
                try
                {
                    serialPort.Write( data, 0, data.Length );
                    return OperateResult.CreateSuccessResult( );
                }
                catch(Exception ex)
                {
                    return new OperateResult( ex.Message );
                }
            }
            else
            {
                return OperateResult.CreateSuccessResult( );
            }
        }

        /// <summary>
        /// 从串口接收一串数据信息，可以指定是否一定要接收到数据
        /// </summary>
        /// <param name="serialPort">串口对象</param>
        /// <param name="awaitData">是否必须要等待数据返回</param>
        /// <returns>结果数据对象</returns>
        protected virtual OperateResult<byte[]> SPReceived( SerialPort serialPort, bool awaitData )
        {
            byte[] buffer = new byte[1024];
            System.IO.MemoryStream ms = new System.IO.MemoryStream( );
            DateTime start = DateTime.Now;                                  // 开始时间，用于确认是否超时的信息
            while (true)
            {
                Thread.Sleep( sleepTime );
                try
                {
                    if (serialPort.BytesToRead < 1)
                    {
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeout)
                        {
                            ms.Dispose( );
                            return new OperateResult<byte[]>( $"Time out: {ReceiveTimeout}" );
                        }
                        else if (ms.Length > 0)
                        {
                            break;
                        }
                        else if (awaitData)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                    // 继续接收数据
                    int sp_receive = serialPort.Read( buffer, 0, buffer.Length );
                    ms.Write( buffer, 0, sp_receive );
                }
                catch (Exception ex)
                {
                    ms.Dispose( );
                    return new OperateResult<byte[]>( ex.Message );
                }
            }

            //resetEvent.Set( );
            byte[] result = ms.ToArray( );
            ms.Dispose( );
            return OperateResult.CreateSuccessResult( result );
        }
        
        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串</returns>
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
        /// 接收数据的超时时间，默认5000ms
        /// </summary>
        public int ReceiveTimeout
        {
            get { return receiveTimeout; }
            set { receiveTimeout = value; }
        }

        /// <summary>
        /// 连续串口缓冲数据检测的间隔时间，默认20ms
        /// </summary>
        public int SleepTime
        {
            get { return sleepTime; }
            set { if (value > 0) sleepTime = value; }
        }

        #endregion

        #region Private Member

        
        private SerialPort SP_ReadData = null;                    // 串口交互的核心
        private SimpleHybirdLock hybirdLock;                      // 数据交互的锁
        private ILogNet logNet;                                   // 日志存储
        private int receiveTimeout = 5000;                        // 接收数据的超时时间
        private int sleepTime = 20;                               // 睡眠的时间

        #endregion
    }
}
