using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.Melsec
{
    /// <summary>
    /// 所有三菱通讯类的通用辅助工具类，包含了一些通用的静态方法，可以使用本类来获取一些原始的报文信息。详细的操作参见例子
    /// </summary>
    public class MelsecHelper
    {
        #region Melsec Fx

        #region Public Method


        /// <summary>
        /// 生成位写入的数据报文信息，该报文可直接用于发送串口给PLC
        /// </summary>
        /// <param name="address">地址信息，每个地址存在一定的范围，需要谨慎传入数据。举例：M10,S10,X5,Y10,C10,T10</param>
        /// <param name="value"><c>True</c>或是<c>False</c></param>
        /// <returns>带报文信息的结果对象</returns>
        public static OperateResult<byte[]> FxBuildWriteBoolPacket( string address, bool value )
        {
            var analysis = MelsecFxAnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            // 二次运算起始地址偏移量，根据类型的不同，地址的计算方式不同
            ushort startAddress = analysis.Content2;
            if (analysis.Content1 == MelsecMcDataType.M)
            {
                if (startAddress >= 8000)
                {
                    startAddress = (ushort)(startAddress - 8000 + 0x0F00);
                }
                else
                {
                    startAddress = (ushort)(startAddress + 0x0800);
                }
            }
            else if (analysis.Content1 == MelsecMcDataType.S)
            {
                startAddress = (ushort)(startAddress + 0x0000);
            }
            else if (analysis.Content1 == MelsecMcDataType.X)
            {
                startAddress = (ushort)(startAddress + 0x0400);
            }
            else if (analysis.Content1 == MelsecMcDataType.Y)
            {
                startAddress = (ushort)(startAddress + 0x0500);
            }
            else if (analysis.Content1 == MelsecMcDataType.C)
            {
                startAddress += (ushort)(startAddress + 0x0E00);
            }
            else if (analysis.Content1 == MelsecMcDataType.T)
            {
                startAddress += (ushort)(startAddress + 0x0600);
            }
            else
            {
                return new OperateResult<byte[]>( ) { Message = "当前类型不支持位写入" };
            }


            byte[] _PLCCommand = new byte[9];
            _PLCCommand[0] = 0x02;                                        // STX
            _PLCCommand[1] = value ? (byte)0x37 : (byte)0x38;             // Read
            _PLCCommand[2] = BuildBytesFromData( startAddress )[2];         // 偏移地址
            _PLCCommand[3] = BuildBytesFromData( startAddress )[3];
            _PLCCommand[4] = BuildBytesFromData( startAddress )[0];
            _PLCCommand[5] = BuildBytesFromData( startAddress )[1];
            _PLCCommand[6] = 0x03;                                          // ETX
            FxCalculateCRC( _PLCCommand ).CopyTo( _PLCCommand, 7 );         // CRC
            
            return OperateResult.CreateSuccessResult( _PLCCommand );
        }

        /// <summary>
        /// 根据类型地址长度确认需要读取的指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">长度</param>
        /// <returns>带有成功标志的指令数据</returns>
        public static OperateResult<byte[]> FxBuildReadWordCommand( string address, ushort length )
        {
            var addressResult = FxCalculateWordStartAddress( address );
            if (!addressResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( addressResult );

            length = (ushort)(length * 2);
            ushort startAddress = addressResult.Content;

            byte[] _PLCCommand = new byte[11];
            _PLCCommand[0] = 0x02;                                       // STX
            _PLCCommand[1] = 0x30;                                       // Read
            _PLCCommand[2] = BuildBytesFromData( startAddress )[0];      // 偏移地址
            _PLCCommand[3] = BuildBytesFromData( startAddress )[1];
            _PLCCommand[4] = BuildBytesFromData( startAddress )[2];
            _PLCCommand[5] = BuildBytesFromData( startAddress )[3];
            _PLCCommand[6] = BuildBytesFromData( (byte)length )[0];      // 读取长度
            _PLCCommand[7] = BuildBytesFromData( (byte)length )[1];
            _PLCCommand[8] = 0x03;                                       // ETX
            FxCalculateCRC( _PLCCommand ).CopyTo( _PLCCommand, 9 );      // CRC

            return OperateResult.CreateSuccessResult( _PLCCommand );     // Return
        }

        /// <summary>
        /// 根据类型地址长度确认需要读取的指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">bool数组长度</param>
        /// <returns>带有成功标志的指令数据</returns>
        public static OperateResult<byte[], int> FxBuildReadBoolCommand( string address, ushort length )
        {
            var addressResult = FxCalculateBoolStartAddress( address );
            if (!addressResult.IsSuccess) return OperateResult.CreateFailedResult<byte[], int>( addressResult );

            // 计算下实际需要读取的数据长度
            ushort length2 = (ushort)((addressResult.Content1 + length - 1) / 8 - addressResult.Content1 / 8 + 1);

            ushort startAddress = addressResult.Content1;
            byte[] _PLCCommand = new byte[11];
            _PLCCommand[0] = 0x02;                                                 // STX
            _PLCCommand[1] = 0x30;                                                 // Read
            _PLCCommand[2] = BuildBytesFromData( startAddress )[0];                // 偏移地址
            _PLCCommand[3] = BuildBytesFromData( startAddress )[1];
            _PLCCommand[4] = BuildBytesFromData( startAddress )[2];
            _PLCCommand[5] = BuildBytesFromData( startAddress )[3];
            _PLCCommand[6] = BuildBytesFromData( (byte)length2 )[0];               // 读取长度
            _PLCCommand[7] = BuildBytesFromData( (byte)length2 )[1];
            _PLCCommand[8] = 0x03;                                                 // ETX
            FxCalculateCRC( _PLCCommand ).CopyTo( _PLCCommand, 9 );                // CRC

            return OperateResult.CreateSuccessResult( _PLCCommand, (int)addressResult.Content2 );
        }

        /// <summary>
        /// 根据类型地址以及需要写入的数据来生成指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static OperateResult<byte[]> BuildWriteWordCommand( string address, byte[] value )
        {
            var addressResult = FxCalculateWordStartAddress( address );
            if (!addressResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( addressResult );

            // 字节数据转换成ASCII格式
            if (value == null) value = MelsecHelper.BuildBytesFromData( value );

            ushort startAddress = addressResult.Content;
            byte[] _PLCCommand = new byte[11 + value.Length];
            _PLCCommand[0] = 0x02;                                                              // STX
            _PLCCommand[1] = 0x31;                                                              // Read
            _PLCCommand[2] = BuildBytesFromData( startAddress )[0];                             // 偏移地址
            _PLCCommand[3] = BuildBytesFromData( startAddress )[1];
            _PLCCommand[4] = BuildBytesFromData( startAddress )[2];
            _PLCCommand[5] = BuildBytesFromData( startAddress )[3];
            _PLCCommand[6] = BuildBytesFromData( (byte)(value.Length / 2) )[0];                 // 读取长度
            _PLCCommand[7] = BuildBytesFromData( (byte)(value.Length / 2) )[1];
            Array.Copy( value, 0, _PLCCommand, 8, value.Length );
            _PLCCommand[_PLCCommand.Length - 3] = 0x03;                                         // ETX
            FxCalculateCRC( _PLCCommand ).CopyTo( _PLCCommand, _PLCCommand.Length - 2 );        // CRC

            return OperateResult.CreateSuccessResult( _PLCCommand );
        }

        #endregion

        #region Private Method 


        /// <summary>
        /// 解析数据地址成不同的三菱地址类型
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns>地址结果对象</returns>
        private static OperateResult<MelsecMcDataType, ushort> MelsecFxAnalysisAddress( string address )
        {
            var result = new OperateResult<MelsecMcDataType, ushort>( );
            try
            {
                switch (address[0])
                {
                    case 'M':
                    case 'm':
                        {
                            result.Content1 = MelsecMcDataType.M;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.M.FromBase );
                            break;
                        }
                    case 'X':
                    case 'x':
                        {
                            result.Content1 = MelsecMcDataType.X;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), 8 );
                            break;
                        }
                    case 'Y':
                    case 'y':
                        {
                            result.Content1 = MelsecMcDataType.Y;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), 8 );
                            break;
                        }
                    case 'D':
                    case 'd':
                        {
                            result.Content1 = MelsecMcDataType.D;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.D.FromBase );
                            break;
                        }
                    case 'S':
                    case 's':
                        {
                            result.Content1 = MelsecMcDataType.S;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.S.FromBase );
                            break;
                        }
                    case 'T':
                    case 't':
                        {
                            result.Content1 = MelsecMcDataType.T;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.T.FromBase );
                            break;
                        }
                    case 'C':
                    case 'c':
                        {
                            result.Content1 = MelsecMcDataType.C;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.C.FromBase );
                            break;
                        }
                    default: throw new Exception( "输入的类型不支持，请重新输入" );
                }
            }
            catch (Exception ex)
            {
                result.Message = "地址格式填写错误：" + ex.Message;
                return result;
            }

            result.IsSuccess = true;
            return result;
        }
        
        /// <summary>
        /// 返回读取的地址及长度信息
        /// </summary>
        /// <param name="address">读取的地址信息</param>
        /// <returns>带起始地址的结果对象</returns>
        private static OperateResult<ushort> FxCalculateWordStartAddress( string address )
        {
            // 初步解析，失败就返回
            var analysis = MelsecFxAnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<ushort>( analysis );


            // 二次解析
            ushort startAddress = analysis.Content2;
            if (analysis.Content1 == MelsecMcDataType.D)
            {
                if (startAddress >= 8000)
                {
                    startAddress = (ushort)((startAddress - 8000) * 2 + 0x0E00);
                }
                else
                {
                    startAddress = (ushort)(startAddress * 2 + 0x1000);
                }
            }
            else if (analysis.Content1 == MelsecMcDataType.C)
            {
                if (startAddress >= 200)
                {
                    startAddress = (ushort)((startAddress - 200) * 4 + 0x0C00);
                }
                else
                {
                    startAddress = (ushort)(startAddress * 2 + 0x0A00);
                }
            }
            else if (analysis.Content1 == MelsecMcDataType.T)
            {
                startAddress = (ushort)(startAddress * 2 + 0x0800);
            }
            else
            {
                return new OperateResult<ushort>( ) { Message = "当前类型不支持字读取" };
            }

            return OperateResult.CreateSuccessResult( startAddress );
        }

        /// <summary>
        /// 返回读取的地址及长度信息，以及当前的偏置信息
        /// </summary><param name="address">读取的地址信息</param>
        /// <returns>带起始地址的结果对象</returns>
        private static OperateResult<ushort, ushort> FxCalculateBoolStartAddress( string address )
        {
            // 初步解析
            var analysis = MelsecFxAnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<ushort, ushort>( analysis );

            // 二次解析
            ushort startAddress = analysis.Content2;
            if (analysis.Content1 == MelsecMcDataType.M)
            {
                if (startAddress >= 8000)
                {
                    startAddress = (ushort)((startAddress - 8000) / 8 + 0x01E0);
                }
                else
                {
                    startAddress = (ushort)(startAddress / 8 + 0x0100);
                }
            }
            else if (analysis.Content1 == MelsecMcDataType.X)
            {
                startAddress = (ushort)(startAddress / 8 + 0x0080);
            }
            else if (analysis.Content1 == MelsecMcDataType.Y)
            {
                startAddress = (ushort)(startAddress / 8 + 0x00A0);
            }
            else if (analysis.Content1 == MelsecMcDataType.S)
            {
                startAddress = (ushort)(startAddress / 8 + 0x0000);
            }
            else if (analysis.Content1 == MelsecMcDataType.C)
            {
                startAddress += (ushort)(startAddress / 8 + 0x01C0);
            }
            else if (analysis.Content1 == MelsecMcDataType.T)
            {
                startAddress += (ushort)(startAddress / 8 + 0x00C0);
            }
            else
            {
                return new OperateResult<ushort, ushort>( ) { Message = "当前类型不支持位读取" };
            }

            return OperateResult.CreateSuccessResult( startAddress, (ushort)(analysis.Content2 % 8) );
        }

        #endregion

        #endregion



        #region Common Logic

        /// <summary>
        /// 从字节构建一个ASCII格式的地址字节
        /// </summary>
        /// <param name="value">字节信息</param>
        /// <returns>ASCII格式的地址</returns>
        internal static byte[] BuildBytesFromData( byte value )
        {
            return Encoding.ASCII.GetBytes( value.ToString( "X2" ) );
        }

        /// <summary>
        /// 从short数据构建一个ASCII格式地址字节
        /// </summary>
        /// <param name="value">short值</param>
        /// <returns>ASCII格式的地址</returns>
        internal static byte[] BuildBytesFromData( short value )
        {
            return Encoding.ASCII.GetBytes( value.ToString( "X4" ) );
        }

        /// <summary>
        /// 从ushort数据构建一个ASCII格式地址字节
        /// </summary>
        /// <param name="value">ushort值</param>
        /// <returns>ASCII格式的地址</returns>
        internal static byte[] BuildBytesFromData( ushort value )
        {
            return Encoding.ASCII.GetBytes( value.ToString( "X4" ) );
        }

        /// <summary>
        /// 从三菱的地址中构建MC协议的6字节的ASCII格式的地址
        /// </summary>
        /// <param name="address">三菱地址</param>
        /// <param name="type">三菱的数据类型</param>
        /// <returns>6字节的ASCII格式的地址</returns>
        internal static byte[] BuildBytesFromAddress( int address, MelsecMcDataType type )
        {
            return Encoding.ASCII.GetBytes( address.ToString( type.FromBase == 10 ? "D6" : "X6" ) );
        }


        /// <summary>
        /// 从字节数组构建一个ASCII格式的地址字节
        /// </summary>
        /// <param name="value">字节信息</param>
        /// <returns>ASCII格式的地址</returns>
        internal static byte[] BuildBytesFromData( byte[] value )
        {
            byte[] buffer = new byte[value.Length * 2];
            for (int i = 0; i < value.Length; i++)
            {
                BuildBytesFromData( value[i] ).CopyTo( buffer, 2 * i );
            }
            return buffer;
        }

        #endregion

        #region CRC Check

        /// <summary>
        /// 计算Fx协议指令的和校验信息
        /// </summary>
        /// <param name="data">字节数据</param>
        /// <returns>校验之后的数据</returns>
        internal static byte[] FxCalculateCRC( byte[] data )
        {
            int sum = 0;
            for (int i = 1; i < data.Length - 2; i++)
            {
                sum += data[i];
            }
            return BuildBytesFromData( (byte)sum );
        }

        /// <summary>
        /// 检查指定的和校验是否是正确的
        /// </summary>
        /// <param name="data">字节数据</param>
        /// <returns>是否成功</returns>
        internal static bool CheckCRC( byte[] data )
        {
            byte[] crc = FxCalculateCRC( data );
            if (crc[0] != data[data.Length - 2]) return false;
            if (crc[1] != data[data.Length - 1]) return false;
            return true;
        }

        #endregion
    }
}
