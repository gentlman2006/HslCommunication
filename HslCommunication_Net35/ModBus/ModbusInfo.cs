using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.ModBus
{

    /// <summary>
    /// Modbus协议相关的一些信息
    /// </summary>
    public class ModbusInfo
    {

        /*****************************************************************************************
         * 
         *    本服务器和客户端支持的常用功能码
         * 
         *******************************************************************************************/



        /// <summary>
        /// 读取线圈
        /// </summary>
        public const byte ReadCoil = 0x01;

        /// <summary>
        /// 读取离散量
        /// </summary>
        public const byte ReadDiscrete = 0x02;

        /// <summary>
        /// 读取寄存器
        /// </summary>
        public const byte ReadRegister = 0x03;

        /// <summary>
        /// 读取输入寄存器
        /// </summary>
        public const byte ReadInputRegister = 0x04;

        /// <summary>
        /// 写单个线圈
        /// </summary>
        public const byte WriteOneCoil = 0x05;

        /// <summary>
        /// 写单个寄存器
        /// </summary>
        public const byte WriteOneRegister = 0x06;

        /// <summary>
        /// 写多个线圈
        /// </summary>
        public const byte WriteCoil = 0x0F;

        /// <summary>
        /// 写多个寄存器
        /// </summary>
        public const byte WriteRegister = 0x10;






        /*****************************************************************************************
         * 
         *    本服务器和客户端支持的异常返回
         * 
         *******************************************************************************************/



        /// <summary>
        /// 不支持该功能码
        /// </summary>
        public const byte FunctionCodeNotSupport = 0x01;
        /// <summary>
        /// 该地址越界
        /// </summary>
        public const byte FunctionCodeOverBound = 0x02;
        /// <summary>
        /// 读取长度超过最大值
        /// </summary>
        public const byte FunctionCodeQuantityOver = 0x03;
        /// <summary>
        /// 读写异常
        /// </summary>
        public const byte FunctionCodeReadWriteException = 0x04;








        #region Static Method
        



        #endregion




    }
}
