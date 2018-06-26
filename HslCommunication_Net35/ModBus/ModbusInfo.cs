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


        /// <summary>
        /// 将modbus指令打包成Modbus-Tcp指令
        /// </summary>
        /// <param name="value">Modbus指令</param>
        /// <param name="id">消息的序号</param>
        /// <returns>Modbus-Tcp指令</returns>
        public static byte[] PackCommandToTcp( byte[] value, ushort id )
        {
            byte[] buffer = new byte[value.Length + 6];
            buffer[0] = BitConverter.GetBytes( id )[1];
            buffer[1] = BitConverter.GetBytes( id )[0];

            buffer[4] = BitConverter.GetBytes( value.Length )[1];
            buffer[5] = BitConverter.GetBytes( value.Length )[0];

            value.CopyTo( buffer, 6 );
            return buffer;
        }


        /// <summary>
        /// 将modbus指令打包成Modbus-Rtu指令
        /// </summary>
        /// <param name="value">Modbus指令</param>
        /// <returns>Modbus-Rtu指令</returns>
        public static byte[] PackCommandToRtu( byte[] value )
        {
            return Serial.SoftCRC16.CRC16( value );
        }


        /// <summary>
        /// 分析Modbus协议的地址信息，该地址适应于tcp及rtu模式
        /// </summary>
        /// <param name="address">带格式的地址，比如"100"，"x=4;100"，"s=1;100","s=1;x=4;100"</param>
        /// <param name="isStartWithZero">起始地址是否从0开始</param>
        /// <returns>转换后的地址信息</returns>
        public static OperateResult<ModbusAddress> AnalysisReadAddress( string address, bool isStartWithZero )
        {
            try
            {
                ModbusAddress mAddress = new ModbusAddress( address );
                if (!isStartWithZero)
                {
                    if (mAddress.Address < 1) throw new Exception( "地址值在起始地址为1的情况下，必须大于1" );
                    mAddress.Address = (ushort)(mAddress.Address - 1);
                }
                return OperateResult.CreateSuccessResult( mAddress );
            }
            catch (Exception ex)
            {
                return new OperateResult<ModbusAddress>( ) { Message = ex.Message };
            }
        }

        /// <summary>
        /// 通过错误码来获取到对应的文本消息
        /// </summary>
        /// <param name="code">错误码</param>
        /// <returns>错误的文本描述</returns>
        public static string GetDescriptionByErrorCode( byte code )
        {
            switch (code)
            {
                case ModbusInfo.FunctionCodeNotSupport: return StringResources.ModbusTcpFunctionCodeNotSupport;
                case ModbusInfo.FunctionCodeOverBound: return StringResources.ModbusTcpFunctionCodeOverBound;
                case ModbusInfo.FunctionCodeQuantityOver: return StringResources.ModbusTcpFunctionCodeQuantityOver;
                case ModbusInfo.FunctionCodeReadWriteException: return StringResources.ModbusTcpFunctionCodeReadWriteException;
                default: return StringResources.UnknownError;
            }
        }
        

        #endregion




    }
}
