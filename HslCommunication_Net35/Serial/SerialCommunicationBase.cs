using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace HslCommunication.Serial
{ 
    /// <summary>
    /// 所有串行通信类的基类，提供了一些基础的服务
    /// </summary>
    public class SerialBase
    {
        /// <summary>
        /// 用于通信的基础串口
        /// </summary>
        protected SerialPort SP_ReadData = new SerialPort();

        /// <summary>
        /// 初始化串口信息
        /// </summary>
        public void SerialPortInni(string portName)
        {
            if (SP_ReadData.IsOpen)
            {
                return;
            }
            //串口的端口号
            SP_ReadData.PortName = "COM5";
            //串口的波特率
            SP_ReadData.BaudRate = 9600;
            //串口的数据位
            SP_ReadData.DataBits = 8;
            //停止位
            SP_ReadData.StopBits = StopBits.One;
            //奇偶校验为偶数
            SP_ReadData.Parity = Parity.Even;


            SP_ReadData.DataReceived += SP_ReadData_DataReceived;
        }
        /// <summary>
        /// 根据自定义初始化方法进行初始化串口信息
        /// </summary>
        public void SerialPortInni(Action<SerialPort> initi)
        {
            if (SP_ReadData.IsOpen)
            {
                return;
            }
            //串口的端口号
            SP_ReadData.PortName = "COM5";
            //串口的波特率
            SP_ReadData.BaudRate = 9600;
            //串口的数据位
            SP_ReadData.DataBits = 8;
            //停止位
            SP_ReadData.StopBits = StopBits.One;
            //奇偶校验为偶数
            SP_ReadData.Parity = Parity.Even;

            initi.Invoke(SP_ReadData);


            SP_ReadData.DataReceived += SP_ReadData_DataReceived;
        }

        private void SP_ReadData_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            
        }
    }
}
