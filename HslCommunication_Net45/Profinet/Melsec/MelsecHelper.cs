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


        #endregion

        #region Melsec Mc

        /// <summary>
        /// 解析A1E协议数据地址
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns></returns>
        public static OperateResult<MelsecA1EDataType, ushort> McA1EAnalysisAddress(string address)
        {
            var result = new OperateResult<MelsecA1EDataType, ushort>();
            try
            {
                switch (address[0])
                {
                    case 'X':
                    case 'x':
                        {
                            result.Content1 = MelsecA1EDataType.X;
                            result.Content2 = Convert.ToUInt16(address.Substring(1), MelsecA1EDataType.X.FromBase);
                            break;
                        }
                    case 'Y':
                    case 'y':
                        {
                            result.Content1 = MelsecA1EDataType.Y;
                            result.Content2 = Convert.ToUInt16(address.Substring(1), MelsecA1EDataType.Y.FromBase);
                            break;
                        }
                    case 'M':
                    case 'm':
                        {
                            result.Content1 = MelsecA1EDataType.M;
                            result.Content2 = Convert.ToUInt16(address.Substring(1), MelsecA1EDataType.M.FromBase);
                            break;
                        }
                    case 'S':
                    case 's':
                        {
                            result.Content1 = MelsecA1EDataType.S;
                            result.Content2 = Convert.ToUInt16(address.Substring(1), MelsecA1EDataType.S.FromBase);
                            break;
                        }
                    case 'D':
                    case 'd':
                        {
                            result.Content1 = MelsecA1EDataType.D;
                            result.Content2 = Convert.ToUInt16(address.Substring(1), MelsecA1EDataType.D.FromBase);
                            break;
                        }
                    case 'R':
                    case 'r':
                        {
                            result.Content1 = MelsecA1EDataType.R;
                            result.Content2 = Convert.ToUInt16(address.Substring(1), MelsecA1EDataType.R.FromBase);
                            break;
                        }
                    default: throw new Exception(StringResources.Language.NotSupportedDataType);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }

            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 解析数据地址
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns>解析值</returns>
        public static OperateResult<MelsecMcDataType, int> McAnalysisAddress( string address )
        {
            var result = new OperateResult<MelsecMcDataType, int>( );
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
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.X.FromBase );
                            break;
                        }
                    case 'Y':
                    case 'y':
                        {
                            result.Content1 = MelsecMcDataType.Y;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.Y.FromBase );
                            break;
                        }
                    case 'D':
                    case 'd':
                        {
                            result.Content1 = MelsecMcDataType.D;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.D.FromBase );
                            break;
                        }
                    case 'W':
                    case 'w':
                        {
                            result.Content1 = MelsecMcDataType.W;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.W.FromBase );
                            break;
                        }
                    case 'L':
                    case 'l':
                        {
                            result.Content1 = MelsecMcDataType.L;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.L.FromBase );
                            break;
                        }
                    case 'F':
                    case 'f':
                        {
                            result.Content1 = MelsecMcDataType.F;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.F.FromBase );
                            break;
                        }
                    case 'V':
                    case 'v':
                        {
                            result.Content1 = MelsecMcDataType.V;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.V.FromBase );
                            break;
                        }
                    case 'B':
                    case 'b':
                        {
                            result.Content1 = MelsecMcDataType.B;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.B.FromBase );
                            break;
                        }
                    case 'R':
                    case 'r':
                        {
                            result.Content1 = MelsecMcDataType.R;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.R.FromBase );
                            break;
                        }
                    case 'S':
                    case 's':
                        {
                            result.Content1 = MelsecMcDataType.S;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.S.FromBase );
                            break;
                        }
                    case 'Z':
                    case 'z':
                        {
                            if (address.StartsWith( "ZR" ) || address.StartsWith( "zr" ))
                            {
                                result.Content1 = MelsecMcDataType.ZR;
                                result.Content2 = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.ZR.FromBase );
                                break;
                            }
                            else
                            {
                                result.Content1 = MelsecMcDataType.Z;
                                result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.Z.FromBase );
                                break;
                            }
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
                    default: throw new Exception( StringResources.Language.NotSupportedDataType );
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }

            result.IsSuccess = true;
            result.Message = StringResources.Language.SuccessText;
            return result;
        }

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

        /// <summary>
        /// 将0，1，0，1的字节数组压缩成三菱格式的字节数组来表示开关量的
        /// </summary>
        /// <param name="value">原始的数据字节</param>
        /// <returns>压缩过后的数据字节</returns>
        internal static byte[] TransBoolArrayToByteData( byte[] value )
        {
            int length = value.Length % 2 == 0 ? value.Length / 2 : (value.Length / 2) + 1;
            byte[] buffer = new byte[length];

            for (int i = 0; i < length; i++)
            {
                if (value[i * 2 + 0] != 0x00) buffer[i] += 0x10;
                if ((i * 2 + 1) < value.Length)
                {
                    if (value[i * 2 + 1] != 0x00) buffer[i] += 0x01;
                }
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
