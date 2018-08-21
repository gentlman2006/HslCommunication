using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.Melsec
{
    /// <summary>
    /// 所有三菱通讯类的通用辅助工具类，包含了一些通用的静态方法
    /// </summary>
    public class MelsecHelper
    {
        #region Melsec Fx

        public static OperateResult<byte[]> MelsecFxBuildWriteBoolPacket( string address, bool value )
        {
            var analysis = MelsecFxAnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            byte[] _PLCCommand = new byte[9];
            _PLCCommand[0] = 0x02;                                        // STX
            _PLCCommand[1] = value ? (byte)0x37 : (byte)0x38;             // Read

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


            //_PLCCommand[2] = BuildBytesFromData( startAddress )[0];      // 偏移地址
            //_PLCCommand[3] = BuildBytesFromData( startAddress )[1];
            //_PLCCommand[4] = BuildBytesFromData( startAddress )[2];
            //_PLCCommand[5] = BuildBytesFromData( startAddress )[3];
            //_PLCCommand[6] = 0x03;                                       // ETX
            //CalculateCRC( _PLCCommand ).CopyTo( _PLCCommand, 7 );        // CRC

            return OperateResult.CreateSuccessResult( _PLCCommand );
        }

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

        #endregion

    }
}
