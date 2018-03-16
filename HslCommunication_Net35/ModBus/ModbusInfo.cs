using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.ModBus
{
    public class ModbusInfo
    {
        /// <summary>
        /// 读取线圈
        /// </summary>
        public const byte ReadCoil = 1;

        /// <summary>
        /// 读取离散量
        /// </summary>
        public const byte ReadDiscrete = 1;

        /// <summary>
        /// 读取寄存器
        /// </summary>
        public const byte ReadRegister = 1;

        /// <summary>
        /// 写单个线圈
        /// </summary>
        public const byte WriteOneCoil = 1;

        /// <summary>
        /// 写单个寄存器
        /// </summary>
        public const byte WriteOneRegister = 1;

        /// <summary>
        /// 写多个线圈
        /// </summary>
        public const byte WriteCoil = 1;

        /// <summary>
        /// 写多个寄存器
        /// </summary>
        public const byte WriteRegister = 1;

    }
}
