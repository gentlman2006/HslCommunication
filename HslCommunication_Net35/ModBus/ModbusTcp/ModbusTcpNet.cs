using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;

namespace HslCommunication.ModBus
{
    

    /// <summary>
    /// Modbus-Tcp协议的客户端通讯类，方便的和服务器进行数据交互
    /// </summary>
    public class ModbusTcpNet : NetworkDoubleBase<ModbusTcpMessage, ReverseWordTransform>
    {

        #region Constructor

        /// <summary>
        /// 实例化一个MOdbus-Tcp协议的客户端对象
        /// </summary>
        public ModbusTcpNet( )
        {
            softIncrementCount = new SoftIncrementCount( ushort.MaxValue );
        }

        #endregion

        #region Private Member

        private byte station = 0x01;             // 本客户端的站号
        private SoftIncrementCount softIncrementCount;              // 自增消息的对象


        #endregion

        #region Address Analysis

        /// <summary>
        /// 解析数据地址，解析出地址类型，起始地址
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns>解析出地址类型，起始地址，DB块的地址</returns>
        private OperateResult<int> AnalysisAddress( string address )
        {
            var result = new OperateResult<int>( );
            try
            {
                result.Content = Convert.ToInt32( address );
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }

            result.IsSuccess = true;
            return result;
        }


        #endregion

        #region Build Command


        /// <summary>
        /// 读取数据的基础指令，需要指定指令码，地址，长度
        /// </summary>
        /// <param name="code"></param>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private OperateResult<byte[]> BuildReadCommandBase(byte code, string address, ushort count )
        {
            var result = new OperateResult<byte[]>( );

            OperateResult<int> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }

            ushort messageId = (ushort)softIncrementCount.GetCurrentValue( );
            byte[] buffer = new byte[12];
            buffer[0] = (byte)(messageId / 256);
            buffer[1] = (byte)(messageId % 256);
            buffer[2] = 0x00;
            buffer[3] = 0x00;
            buffer[4] = 0x00;
            buffer[5] = 0x06;
            buffer[6] = station;
            buffer[7] = code;
            buffer[8] = (byte)(analysis.Content / 256);
            buffer[9] = (byte)(analysis.Content % 256);
            buffer[10] = (byte)(count / 256);
            buffer[11] = (byte)(count % 256);

            result.Content = buffer;
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 生成一个读取线圈的指令头
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="count">长度</param>
        /// <returns>携带有命令字节</returns>
        private OperateResult<byte[]> BuildReadCoilCommand( string address, ushort count )
        {
            return BuildReadCommandBase( 0x01, address, count );
        }

        /// <summary>
        /// 生成一个读取离散信息的指令头
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="count">长度</param>
        /// <returns>携带有命令字节</returns>
        private OperateResult<byte[]> BuildReadDiscreteCommand( string address, ushort count )
        {
            return BuildReadCommandBase( 0x02, address, count );
        }

        


        /// <summary>
        /// 生成一个读取寄存器的指令头
        /// </summary>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns>携带有命令字节</returns>
        private OperateResult<byte[]> BuildReadRegisterCommand( string address, ushort count )
        {
            return BuildReadCommandBase( 0x03, address, count );
        }



        private OperateResult<byte[]> BuildWriteOneCoilCommand( string address, bool value )
        {
            var result = new OperateResult<byte[]>( );

            OperateResult<int> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }

            ushort messageId = (ushort)softIncrementCount.GetCurrentValue( );
            byte[] buffer = new byte[12];
            buffer[0] = (byte)(messageId / 256);
            buffer[1] = (byte)(messageId % 256);
            buffer[2] = 0x00;
            buffer[3] = 0x00;
            buffer[4] = 0x00;
            buffer[5] = 0x06;
            buffer[6] = station;
            buffer[7] = 0x05;
            buffer[8] = (byte)(analysis.Content / 256);
            buffer[9] = (byte)(analysis.Content % 256);
            buffer[10] = (byte)(value ? 0xFF : 0x00);
            buffer[11] = 0x00;

            result.Content = buffer;
            result.IsSuccess = true;
            return result;
        }





        private OperateResult<byte[]> BuildWriteOneRegisterCommand( string address, byte[] data )
        {
            var result = new OperateResult<byte[]>( );

            OperateResult<int> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }

            ushort messageId = (ushort)softIncrementCount.GetCurrentValue( );
            byte[] buffer = new byte[12];
            buffer[0] = (byte)(messageId / 256);
            buffer[1] = (byte)(messageId % 256);
            buffer[2] = 0x00;
            buffer[3] = 0x00;
            buffer[4] = 0x00;
            buffer[5] = 0x06;
            buffer[6] = station;
            buffer[7] = 0x06;
            buffer[8] = (byte)(analysis.Content / 256);
            buffer[9] = (byte)(analysis.Content % 256);
            buffer[10] = data[1];
            buffer[11] = data[0];

            result.Content = buffer;
            result.IsSuccess = true;
            return result;
        }



        private OperateResult<byte[]> BuildWriteCoilCommand( string address, bool[] values )
        {
            var result = new OperateResult<byte[]>( );
            byte[] data = SoftBasic.BoolArrayToByte( values );

            OperateResult<int> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }
            
            ushort messageId = (ushort)softIncrementCount.GetCurrentValue( );
            byte[] buffer = new byte[13 + data.Length];
            buffer[0] = (byte)(messageId / 256);
            buffer[1] = (byte)(messageId % 256);
            buffer[2] = 0x00;
            buffer[3] = 0x00;
            buffer[4] = (byte)((buffer.Length - 6) / 256);
            buffer[5] = (byte)((buffer.Length - 6) % 256);
            buffer[6] = station;
            buffer[7] = 0x0F;
            buffer[8] = (byte)(analysis.Content / 256);
            buffer[9] = (byte)(analysis.Content % 256);
            buffer[10] = (byte)(values.Length / 256);
            buffer[11] = (byte)(values.Length % 256);

            buffer[12] = (byte)(data.Length);
            data.CopyTo( buffer, 13 );

            result.Content = buffer;
            result.IsSuccess = true;
            return result;
        }


        private OperateResult<byte[]> BuildWriteRegisterCommand( string address, byte[] data )
        {
            var result = new OperateResult<byte[]>( );

            OperateResult<int> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }

            ushort messageId = (ushort)softIncrementCount.GetCurrentValue( );
            byte[] buffer = new byte[13 + data.Length];
            buffer[0] = (byte)(messageId / 256);
            buffer[1] = (byte)(messageId % 256);
            buffer[2] = 0x00;
            buffer[3] = 0x00;
            buffer[4] = (byte)((buffer.Length - 6) / 256);
            buffer[5] = (byte)((buffer.Length - 6) % 256);
            buffer[6] = station;
            buffer[7] = 0x10;
            buffer[8] = (byte)(analysis.Content / 256);
            buffer[9] = (byte)(analysis.Content % 256);
            buffer[10] = (byte)(data.Length / 2 / 256);
            buffer[11] = (byte)(data.Length / 2 % 256);

            buffer[12] = (byte)(data.Length);
            data.CopyTo( buffer, 13 );

            result.Content = buffer;
            result.IsSuccess = true;
            return result;
        }



        #endregion

        #region Core Interative

        /// <summary>
        /// 通过错误码来获取到对应的文本消息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string GetDescriptionByErrorCode( byte code )
        {
            switch (code)
            {
                case 0x01: return "不支持该功能码";
                case 0x02: return "越界";
                case 0x03: return "寄存器数量超出范围";
                case 0x04: return "读写异常";
                default: return "未知异常";
            }
        }

        private OperateResult<byte[]> CheckModbusTcpResponse( byte[] send )
        {
            OperateResult<byte[]> result = ReadFromCoreServer( send );
            if (result.IsSuccess)
            {
                if ((send[7] + 0x80) == result.Content[7])
                {
                    // 发生了错误
                    result.IsSuccess = false;
                    result.Message = GetDescriptionByErrorCode( result.Content[8] );
                    result.ErrorCode = result.Content[8];
                }
            }
            return result;
        }

        #endregion

        #region Customer Support

        /// <summary>
        /// 读取自定义的数据类型，只要规定了写入和解析规则
        /// </summary>
        /// <typeparam name="T">类型名称</typeparam>
        /// <param name="address">起始地址</param>
        /// <returns></returns>
        public OperateResult<T> Read<T>( string address ) where T : IDataTransfer, new()
        {
            OperateResult<T> result = new OperateResult<T>( );
            T Content = new T( );
            OperateResult<byte[]> read = Read( address, Content.ReadCount );
            if (read.IsSuccess)
            {
                Content.ParseSource( read.Content );
                result.Content = Content;
                result.IsSuccess = true;
            }
            else
            {
                result.ErrorCode = read.ErrorCode;
                result.Message = read.Message;
            }
            return result;
        }

        /// <summary>
        /// 写入自定义的数据类型到PLC去，只要规定了生成字节的方法即可
        /// </summary>
        /// <typeparam name="T">自定义类型</typeparam>
        /// <param name="address">起始地址</param>
        /// <param name="data">实例对象</param>
        /// <returns></returns>
        public OperateResult Write<T>( string address, T data ) where T : IDataTransfer, new()
        {
            return Write( address, data.ToSource( ) );
        }


        #endregion

        #region Read Support

        /// <summary>
        /// 读取服务器的数据，需要指定不同的功能码
        /// </summary>
        /// <param name="code">指令</param>
        /// <param name="address">地址</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        private OperateResult<byte[]> ReadModBusBase( byte code, string address, ushort length )
        {
            OperateResult<byte[]> command = BuildReadCommandBase( code, address, length );
            if(!command.IsSuccess)
            {
                return new OperateResult<byte[]>( )
                {
                    ErrorCode = command.ErrorCode,
                    Message = command.Message,
                };
            }

            OperateResult<byte[]> resultBytes = CheckModbusTcpResponse( command.Content );
            if (resultBytes.IsSuccess)
            {
                // 二次数据处理
                if (resultBytes.Content?.Length >= 9)
                {
                    byte[] buffer = new byte[resultBytes.Content.Length - 9];
                    Array.Copy( resultBytes.Content, 9, buffer, 0, buffer.Length );
                    resultBytes.Content = buffer;
                }
            }
            return resultBytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public OperateResult<bool[]> ReadCoil( string address, ushort count )
        {
            OperateResult<bool[]> result = new OperateResult<bool[]>( );
            OperateResult<byte[]> read = ReadModBusBase( 0x01, address, count );
        }

        /// <summary>
        /// 从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，T100，C100，以字节为单位
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100，T100，C100，</param>
        /// <param name="count">读取的数量，以字节为单位</param>
        /// <returns>带有成功标志的字节信息</returns>
        public OperateResult<byte[]> Read( string address, ushort count )
        {
            OperateResult<byte[]> result = new OperateResult<byte[]>( );
            OperateResult<byte[]> command = BuildReadCommand( address, count );
            if (!command.IsSuccess)
            {
                result.CopyErrorFromOther( command );
                return result;
            }

            OperateResult<byte[]> read = ReadFromCoreServer( command.Content );
            if (read.IsSuccess)
            {
                if (read.Content[8] == 0x00)
                {
                    // 分析结果
                    byte[] buffer = new byte[read.Content.Length - 16];
                    Array.Copy( read.Content, 16, buffer, 0, buffer.Length );

                    result.Content = buffer;
                    result.IsSuccess = true;
                }
                else
                {
                    result.ErrorCode = read.Content[8];
                    result.Message = "发生了异常，具体信息查找Fetch/Write协议文档";
                }
            }
            else
            {
                result.ErrorCode = read.ErrorCode;
                result.Message = read.Message;
            }

            return result;
        }

        /// <summary>
        /// 读取指定地址的byte数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<byte> ReadByte( string address )
        {
            return GetByteResultFromBytes( Read( address, 1 ) );
        }


        /// <summary>
        /// 读取指定地址的short数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<short> ReadShort( string address )
        {
            return GetInt16ResultFromBytes( Read( address, 2 ) );
        }


        /// <summary>
        /// 读取指定地址的ushort数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<ushort> ReadUShort( string address )
        {
            return GetUInt16ResultFromBytes( Read( address, 2 ) );
        }

        /// <summary>
        /// 读取指定地址的int数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<int> ReadInt( string address )
        {
            return GetInt32ResultFromBytes( Read( address, 4 ) );
        }

        /// <summary>
        /// 读取指定地址的uint数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<uint> ReadUInt( string address )
        {
            return GetUInt32ResultFromBytes( Read( address, 4 ) );
        }

        /// <summary>
        /// 读取指定地址的float数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<float> ReadFloat( string address )
        {
            return GetSingleResultFromBytes( Read( address, 4 ) );
        }

        /// <summary>
        /// 读取指定地址的long数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<long> ReadLong( string address )
        {
            return GetInt64ResultFromBytes( Read( address, 8 ) );
        }

        /// <summary>
        /// 读取指定地址的ulong数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<ulong> ReadULong( string address )
        {
            return GetUInt64ResultFromBytes( Read( address, 8 ) );
        }

        /// <summary>
        /// 读取指定地址的double数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<double> ReadDouble( string address )
        {
            return GetDoubleResultFromBytes( Read( address, 8 ) );
        }

        /// <summary>
        /// 读取地址地址的String数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <param name="length">字符串长度</param>
        /// <returns></returns>
        public OperateResult<string> ReadString( string address, ushort length )
        {
            return GetStringResultFromBytes( Read( address, length ) );
        }



        #endregion

        #region Write Base


        /// <summary>
        /// 将数据写入到PLC数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <param name="data">写入的数据，长度根据data的长度来指示</param>
        /// <returns></returns>
        public OperateResult Write( string address, byte[] data )
        {
            OperateResult result = new OperateResult( );

            OperateResult<byte[]> command = BuildWriteByteCommand( address, data );
            if (!command.IsSuccess)
            {
                result.CopyErrorFromOther( command );
                return result;
            }


            OperateResult<byte[]> write = ReadFromCoreServer( command.Content );
            if (write.IsSuccess)
            {
                if (write.Content[8] != 0x00)
                {
                    // 写入异常
                    result.Message = "写入数据异常，代号为：" + write.Content[8].ToString( );
                }
                else
                {
                    result.IsSuccess = true;  // 写入成功
                }
            }
            else
            {
                result.ErrorCode = write.ErrorCode;
                result.Message = write.Message;
            }
            return result;
        }



        #endregion

        #region Write String


        /// <summary>
        /// 向PLC中写入字符串，编码格式为ASCII
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回读取结果</returns>
        public OperateResult WriteAsciiString( string address, string data )
        {
            byte[] temp = Encoding.ASCII.GetBytes( data );
            return Write( address, temp );
        }

        /// <summary>
        /// 向PLC中写入指定长度的字符串,超出截断，不够补0，编码格式为ASCII
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>返回读取结果</returns>
        public OperateResult WriteAsciiString( string address, string data, int length )
        {
            byte[] temp = Encoding.ASCII.GetBytes( data );
            temp = BasicFramework.SoftBasic.ArrayExpandToLength( temp, length );
            return Write( address, temp );
        }

        /// <summary>
        /// 向PLC中写入字符串，编码格式为Unicode
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回读取结果</returns>
        public OperateResult WriteUnicodeString( string address, string data )
        {
            byte[] temp = Encoding.Unicode.GetBytes( data );
            return Write( address, temp );
        }

        /// <summary>
        /// 向PLC中写入指定长度的字符串,超出截断，不够补0，编码格式为Unicode
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>返回读取结果</returns>
        public OperateResult WriteUnicodeString( string address, string data, int length )
        {
            byte[] temp = Encoding.Unicode.GetBytes( data );
            temp = BasicFramework.SoftBasic.ArrayExpandToLength( temp, length * 2 );
            return Write( address, temp );
        }

        #endregion

        #region Write bool[]

        /// <summary>
        /// 向PLC中写入bool数组，返回值说明，比如你写入M100,那么data[0]对应M100.0
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据，长度为8的倍数</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, bool[] data )
        {
            return Write( address, BasicFramework.SoftBasic.BoolArrayToByte( data ) );
        }


        #endregion
        
        #region Write Short

        /// <summary>
        /// 向PLC中写入short数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, short[] data )
        {
            return Write( address, ByteTransform.TransByte( data ) );
        }

        /// <summary>
        /// 向PLC中写入short数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, short data )
        {
            return Write( address, new short[] { data } );
        }

        #endregion

        #region Write UShort


        /// <summary>
        /// 向PLC中写入ushort数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, ushort[] data )
        {
            return Write( address, ByteTransform.TransByte( data ) );
        }


        /// <summary>
        /// 向PLC中写入ushort数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, ushort data )
        {
            return Write( address, new ushort[] { data } );
        }


        #endregion

        #region Write Int

        /// <summary>
        /// 向PLC中写入int数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, int[] data )
        {
            return Write( address, ByteTransform.TransByte( data ) );
        }

        /// <summary>
        /// 向PLC中写入int数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, int data )
        {
            return Write( address, new int[] { data } );
        }

        #endregion

        #region Write UInt

        /// <summary>
        /// 向PLC中写入uint数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, uint[] data )
        {
            return Write( address, ByteTransform.TransByte( data ) );
        }

        /// <summary>
        /// 向PLC中写入uint数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, uint data )
        {
            return Write( address, new uint[] { data } );
        }

        #endregion

        #region Write Float

        /// <summary>
        /// 向PLC中写入float数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, float[] data )
        {
            return Write( address, ByteTransform.TransByte( data ) );
        }

        /// <summary>
        /// 向PLC中写入float数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, float data )
        {
            return Write( address, new float[] { data } );
        }


        #endregion

        #region Write Long

        /// <summary>
        /// 向PLC中写入long数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, long[] data )
        {
            return Write( address, ByteTransform.TransByte( data ) );
        }

        /// <summary>
        /// 向PLC中写入long数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, long data )
        {
            return Write( address, new long[] { data } );
        }

        #endregion

        #region Write ULong

        /// <summary>
        /// 向PLC中写入ulong数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, ulong[] data )
        {
            return Write( address, ByteTransform.TransByte( data ) );
        }

        /// <summary>
        /// 向PLC中写入ulong数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, ulong data )
        {
            return Write( address, new ulong[] { data } );
        }

        #endregion

        #region Write Double

        /// <summary>
        /// 向PLC中写入double数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, double[] data )
        {
            return Write( address, ByteTransform.TransByte( data ) );
        }

        /// <summary>
        /// 向PLC中写入double数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, double data )
        {
            return Write( address, new double[] { data } );
        }

        #endregion

        #region Object Override

        /// <summary>
        /// 获取当前对象的字符串标识形式
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString( )
        {
            return "ModbusTcpNet";
        }

        #endregion




    }
}
