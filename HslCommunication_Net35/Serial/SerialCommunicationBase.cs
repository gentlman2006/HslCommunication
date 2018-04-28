using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

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
        }

        #endregion

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

        private void SP_ReadData_DataReceived( object sender, SerialDataReceivedEventArgs e )
        {
            Thread.Sleep( 20 );
            receiveCount = SP_ReadData.Read( buffer, 0, SP_ReadData.BytesToRead );
            resetEvent.Set( );
        }



        #region Public Method

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
        public byte[] ReadBase(byte[] send)
        {
            SP_ReadData.Write( send, 0, send.Length );
            resetEvent.WaitOne( );
            byte[] result = new byte[receiveCount];

            Array.Copy( buffer, 0, result, 0, result.Length );

            return result;
        }

        #endregion


        #region Private Member
        
        private SerialPort SP_ReadData = null;                    // 串口交互的核心
        private AutoResetEvent resetEvent = null;                 // 消息同步机制
        private byte[] buffer = null;                             // 接收缓冲区
        private int receiveCount = 0;                             // 接收的数据长度

        #endregion
    }
}
