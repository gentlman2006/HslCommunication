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


#if !NETSTANDARD2_0

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
        /// 将一个modbus-rtu的数据报文，转换成modbus-ascii的数据报文
        /// </summary>
        /// <param name="value">modbus-rtu的完整报文，携带相关的校验码</param>
        /// <returns>可以用于直接发送的modbus-ascii的报文</returns>
        public static byte[] TransRtuToAsciiPackCommand( byte[] value )
        {
            // remove crc check
            byte[] modbus = BasicFramework.SoftBasic.BytesArrayRemoveLast( value, 2 );

            // add LRC check
            byte[] modbus_lrc = Serial.SoftLRC.LRC( modbus );

            // Translate to ascii information
            byte[] modbus_ascii = BasicFramework.SoftBasic.BytesToAsciiBytes( modbus_lrc );

            // add head and end informarion
            return BasicFramework.SoftBasic.SpliceTwoByteArray( BasicFramework.SoftBasic.SpliceTwoByteArray( new byte[] { 0x3A }, modbus_ascii ), new byte[] { 0x0D, 0x0A } );
        }

        /// <summary>
        /// 将一个modbus-ascii的数据报文，转换成的modbus核心数据报文
        /// </summary>
        /// <param name="value">modbus-ascii的完整报文，携带相关的校验码</param>
        /// <returns>可以用于直接发送的modbus的报文</returns>
        public static OperateResult<byte[]> TransAsciiPackCommandToRtu( byte[] value )
        {
            try
            {
                // response check
                if (value[0] != 0x3A || value[value.Length - 2] != 0x0D || value[value.Length - 1] != 0x0A)
                    return new OperateResult<byte[]>( ) { Message = StringResources.Language.ModbusAsciiFormatCheckFailed + BasicFramework.SoftBasic.ByteToHexString( value ) };

                // remove head and end
                byte[] modbus_ascii = BasicFramework.SoftBasic.BytesArrayRemoveDouble( value, 1, 2 );

                // get modbus core
                byte[] modbus_core = BasicFramework.SoftBasic.AsciiBytesToBytes( modbus_ascii );

                if (!Serial.SoftLRC.CheckLRC( modbus_core ))
                    return new OperateResult<byte[]>( ) { Message = StringResources.Language.ModbusLRCCheckFailed + BasicFramework.SoftBasic.ByteToHexString( modbus_core ) };

                // remove the last info
                return OperateResult.CreateSuccessResult( BasicFramework.SoftBasic.BytesArrayRemoveLast( modbus_core, 1 ) );
            }
            catch(Exception ex)
            {
                return new OperateResult<byte[]>( ) { Message = ex.Message + BasicFramework.SoftBasic.ByteToHexString( value ) };
            }
        }

#endif

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
                    if (mAddress.Address < 1) throw new Exception( StringResources.Language.ModbusAddressMustMoreThanOne );
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
                case ModbusInfo.FunctionCodeNotSupport: return StringResources.Language.ModbusTcpFunctionCodeNotSupport;
                case ModbusInfo.FunctionCodeOverBound: return StringResources.Language.ModbusTcpFunctionCodeOverBound;
                case ModbusInfo.FunctionCodeQuantityOver: return StringResources.Language.ModbusTcpFunctionCodeQuantityOver;
                case ModbusInfo.FunctionCodeReadWriteException: return StringResources.Language.ModbusTcpFunctionCodeReadWriteException;
                default: return StringResources.Language.UnknownError;
            }
        }


        #endregion




    }
}
