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
            buffer = new byte[4096];
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


            SP_ReadData.DataReceived += SP_ReadData_DataReceived;
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
            OperateResult<byte[]> result = null;

            hybirdLock.Enter( );

            try
            {
                isReceiveTimeout = false;                         // 是否接收超时的标志位
                isReceiveComplete = false;                        // 是否接收完成的标志位
                isComError = false;                               // 是否异常的标志
                ComErrorMsg = string.Empty;
                if (send == null) send = new byte[0];

                SP_ReadData.Write( send, 0, send.Length );
                ThreadPool.QueueUserWorkItem( new WaitCallback( CheckReceiveTimeout ), null );
                resetEvent.WaitOne( );
                isReceiveComplete = true;

                if (isComError)
                {
                    result = new OperateResult<byte[]>( ComErrorMsg );
                }
                else if (isReceiveTimeout)
                {
                    result = new OperateResult<byte[]>( StringResources.Language.ReceiveDataTimeout + ReceiveTimeout );
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

            receiveCount = 0;
            return result;
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

        /// <summary>
        /// 串口数据接收的回调方法
        /// </summary>
        /// <param name="sender">数据发送</param>
        /// <param name="e">消息</param>
        protected virtual void SP_ReadData_DataReceived( object sender, SerialDataReceivedEventArgs e )
        {
            while (true)
            {
                Thread.Sleep( 20 );

                try
                {
                    if (SP_ReadData.BytesToRead < 1) break;
                    // 继续接收数据
                    receiveCount += SP_ReadData.Read( buffer, receiveCount, SP_ReadData.BytesToRead );
                }
                catch(Exception ex)
                {
                    isComError = true;
                    ComErrorMsg = ex.Message;
                    break;
                }
            }
            resetEvent.Set( );
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
        /// 接收数据的超时时间
        /// </summary>
        public int ReceiveTimeout
        {
            get { return receiveTimeout; }
            set { receiveTimeout = value; }
        }

        #endregion

        #region Private Member

        /// <summary>
        /// 串口交互的核心
        /// </summary>
        protected SerialPort SP_ReadData = null;                  // 串口交互的核心
        /// <summary>
        /// 消息同步机制
        /// </summary>
        protected AutoResetEvent resetEvent = null;               // 消息同步机制
        /// <summary>
        /// 接收缓冲区
        /// </summary>
        protected readonly byte[] buffer = null;                  // 接收缓冲区
        /// <summary>
        /// 接收的数据长度
        /// </summary>
        protected int receiveCount = 0;                           // 接收的数据长度
        /// <summary>
        /// 当前的COM组件是否发生了异常
        /// </summary>
        protected bool isComError = false;                        // 当前的COM组件是否发生了异常
        /// <summary>
        /// 当前的COM组件异常的消息
        /// </summary>
        protected string ComErrorMsg = string.Empty;              // 当前的COM组件异常的消息
        private SimpleHybirdLock hybirdLock;                      // 数据交互的锁
        private ILogNet logNet;                                   // 日志存储
        private int receiveTimeout = 5000;                        // 接收数据的超时时间
        private bool isReceiveTimeout = false;                    // 是否接收数据超时
        private bool isReceiveComplete = false;                   // 是否接收数据完成

        #endregion
    }
}
