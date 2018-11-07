using HslCommunication.Core;
using HslCommunication.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.Panasonic
{
    /// <summary>
    /// 松下PLC的数据交互协议，采用Mewtocol协议通讯
    /// </summary>
    public class PanasonicMewtocol : SerialDeviceBase<RegularByteTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化一个默认的松下PLC通信对象，默认站号为1
        /// </summary>
        /// <param name="station">站号信息，默认为0xEE</param>
        public PanasonicMewtocol( byte station = 238 )
        {
            this.Station = station;
            this.ByteTransform.DataFormat = DataFormat.DCBA;
        }

        #endregion

        #region Public Properties
        
        /// <summary>
        /// 设备的目标站号
        /// </summary>
        public byte Station { get; set; }

        #endregion

        #region Read Write Override

        /// <summary>
        /// 从松下PLC中读取数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">长度</param>
        /// <returns>返回数据信息</returns>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            // 创建指令
            OperateResult<byte[]> command = BuildReadCommand( Station, address, length );
            if (!command.IsSuccess) return command;

            // 数据交互
            OperateResult<byte[]> read = ReadBase( command.Content );
            if (!read.IsSuccess) return read;

            // 提取数据
            return ExtraActualData( read.Content );
        }

        /// <summary>
        /// 将数据写入到松下PLC中
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">真实数据</param>
        /// <returns>是否写入成功</returns>
        public override OperateResult Write( string address, byte[] value )
        {
            // 创建指令
            OperateResult<byte[]> command = BuildWriteCommand( Station, address, value );
            if (!command.IsSuccess) return command;

            // 数据交互
            OperateResult<byte[]> read = ReadBase( command.Content );
            if (!read.IsSuccess) return read;

            // 提取结果
            return ExtraActualData( read.Content );
        }

        #endregion

        #region Read Write Bool

        /// <summary>
        /// 批量读取松下PLC的位地址
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数据长度</param>
        /// <returns>读取结果对象</returns>
        public OperateResult<bool[]> ReadBool(string address, ushort length )
        {
            // 读取数据
            OperateResult<byte[]> read = Read( address, length );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( read );

            // 提取bool
            byte[] buffer = BasicFramework.SoftBasic.BytesReverseByWord( read.Content );
            return OperateResult.CreateSuccessResult( BasicFramework.SoftBasic.ByteToBoolArray( read.Content, length ) );
        }

        /// <summary>
        /// 读取单个的Bool数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>读取结果对象</returns>
        public OperateResult<bool> ReadBool(string address )
        {
            // 读取数据
            OperateResult<bool[]> read = ReadBool( address, 1 );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool>( read );

            return OperateResult.CreateSuccessResult( read.Content[0] );
        }

        /// <summary>
        /// 写入bool数据信息
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">数据值信息</param>
        /// <returns>返回是否成功的结果对象</returns>
        public OperateResult Write(string address, bool[] values )
        {
            // 计算字节数据
            byte[] buffer = BasicFramework.SoftBasic.BoolArrayToByte( values );

            // 创建指令
            OperateResult<byte[]> command = BuildWriteCommand(Station, address, BasicFramework.SoftBasic.BytesReverseByWord( buffer ), (short)values.Length );
            if (!command.IsSuccess) return command;

            // 数据交互
            OperateResult<byte[]> read = ReadBase( command.Content );
            if (!read.IsSuccess) return read;

            // 提取结果
            return ExtraActualData( read.Content );
        }

        /// <summary>
        /// 写入bool数据数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">True还是False</param>
        /// <returns>返回是否成功的结果对象</returns>
        public OperateResult Write( string address, bool value )
        {
            return Write( address, new bool[] { value } );
        }

        #endregion

        #region Bulid Read Command

        private static string CalculateCrc(StringBuilder sb )
        {
            byte tmp = 0;
            tmp = (byte)sb[0];
            for (int i = 1; i < sb.Length; i++)
            {
                tmp ^= (byte)sb[i];
            }
            return BasicFramework.SoftBasic.ByteToHexString( new byte[] { tmp } );
        }

        /// <summary>
        /// 解析数据地址，解析出地址类型，起始地址，DB块的地址
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns>解析出地址类型，起始地址，是否位读取</returns>
        private static OperateResult<string, int> AnalysisAddress( string address )
        {
            var result = new OperateResult<string, int>( );
            try
            {
                result.Content2 = 0;
                if(address.StartsWith("IX") || address.StartsWith( "ix" ))
                {
                    result.Content1 = "IX";
                    result.Content2 = int.Parse( address.Substring( 2 ) );
                }
                else if (address.StartsWith( "IY" ) || address.StartsWith( "iy" ))
                {
                    result.Content1 = "IY";
                    result.Content2 = int.Parse( address.Substring( 2 ) );
                }
                else if (address.StartsWith( "ID" ) || address.StartsWith( "id" ))
                {
                    result.Content1 = "ID";
                    result.Content2 = int.Parse( address.Substring( 2 ) );
                }
                else if (address[0] == 'X' || address[0] == 'x')
                {
                    result.Content1 = "X";
                    result.Content2 = ushort.Parse( address.Substring( 1 ) );
                }
                else if(address[0] == 'Y' || address[0] == 'y')
                {
                    result.Content1 = "Y";
                    result.Content2 = ushort.Parse( address.Substring( 1 ) );
                }
                else if (address[0] == 'R' || address[0] == 'r')
                {
                    result.Content1 = "R";
                    result.Content2 = ushort.Parse( address.Substring( 1 ) );
                }
                else if (address[0] == 'T' || address[0] == 't')
                {
                    result.Content1 = "T";
                    result.Content2 = ushort.Parse( address.Substring( 1 ) );
                }
                else if (address[0] == 'C' || address[0] == 'c')
                {
                    result.Content1 = "C";
                    result.Content2 = ushort.Parse( address.Substring( 1 ) );
                }
                else if (address[0] == 'L' || address[0] == 'l')
                {
                    result.Content1 = "L";
                    result.Content2 = ushort.Parse( address.Substring( 1 ) );
                }
                else if (address[0] == 'D' || address[0] == 'd')
                {
                    result.Content1 = "D";
                    result.Content2 = ushort.Parse( address.Substring( 1 ) );
                }
                else if (address[0] == 'F' || address[0] == 'f')
                {
                    result.Content1 = "F";
                    result.Content2 = ushort.Parse( address.Substring( 1 ) );
                }
                else if (address[0] == 'S' || address[0] == 's')
                {
                    result.Content1 = "S";
                    result.Content2 = ushort.Parse( address.Substring( 1 ) );
                }
                else if(address[0] == 'K' || address[0] == 'k')
                {
                    result.Content1 = "K";
                    result.Content2 = ushort.Parse( address.Substring( 1 ) );
                }
                else
                {
                    throw new Exception( StringResources.Language.NotSupportedDataType );
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
        /// 创建读取离散触点的报文指令
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <returns>包含是否成功的结果对象</returns>
        public static OperateResult<byte[]> BuildReadMultiCoil( string[] address )
        {
            // 参数检查
            if (address == null) return new OperateResult<byte[]>( "address is not allowed null" );
            if (address.Length < 1 || address.Length > 8) return new OperateResult<byte[]>( "length must be 1-8" );

            StringBuilder sb = new StringBuilder( "%EE#RCP" );
            sb.Append( address.Length.ToString( ) );
            for (int i = 0; i < address.Length; i++)
            {
                // 解析地址
                OperateResult<string, int> analysis = AnalysisAddress( address[i] );
                if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );
                
                sb.Append( analysis.Content1 );
                sb.Append( analysis.Content2 );
            }

            sb.Append( CalculateCrc( sb ) );
            sb.Append( '\u000D' );

            return OperateResult.CreateSuccessResult( Encoding.ASCII.GetBytes( sb.ToString( ) ) );
        }

        /// <summary>
        /// 创建写入离散触点的报文指令
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="values">bool值数组</param>
        /// <returns>包含是否成功的结果对象</returns>
        public static OperateResult<byte[]> BuildWriteMultiCoil( string[] address, bool[] values )
        {
            // 参数检查
            if (address == null || values == null) return new OperateResult<byte[]>( "address and values is not allowed null" );
            if (address.Length < 1 || address.Length > 8) return new OperateResult<byte[]>( "address length must be 1-8" );
            if (address.Length != values.Length) return new OperateResult<byte[]>( "address and values length must be same" );

            StringBuilder sb = new StringBuilder( "%EE#WCP" );
            sb.Append( address.Length.ToString( ) );
            for (int i = 0; i < address.Length; i++)
            {
                // 解析地址
                OperateResult<string, int> analysis = AnalysisAddress( address[i] );
                if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );
                
                sb.Append( analysis.Content1 );
                sb.Append( analysis.Content2 );

                sb.Append( values[i] ? '1' : '0' );
            }

            sb.Append( CalculateCrc( sb ) );
            sb.Append( '\u000D' );

            return OperateResult.CreateSuccessResult( Encoding.ASCII.GetBytes( sb.ToString( ) ) );
        }


        /// <summary>
        /// 创建批量读取触点的报文指令
        /// </summary>
        /// <param name="station">站号信息</param>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        /// <returns>包含是否成功的结果对象</returns>
        public static OperateResult<byte[]> BuildReadCommand(byte station, string address, ushort length )
        {
            // 参数检查
            if (address == null) return new OperateResult<byte[]>( StringResources.Language.PanasonicAddressParameterCannotBeNull );

            // 解析地址
            OperateResult<string, int> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            StringBuilder sb = new StringBuilder( "%" );
            sb.Append( station.ToString( "X2" ) );
            sb.Append( "#" );

            if(analysis.Content1 == "X" || analysis.Content1 == "Y" || analysis.Content1 == "R" || analysis.Content1 == "L")
            {
                sb.Append( "RCC" );
                sb.Append( analysis.Content1 );
                sb.Append( analysis.Content2.ToString( "D4" ) );
                sb.Append( (analysis.Content2 + length - 1).ToString( "D4" ) );
            }
            else if(analysis.Content1 == "D" || analysis.Content1 == "L" || analysis.Content1 == "F")
            {
                sb.Append( "RD" );
                sb.Append( analysis.Content1 );
                sb.Append( analysis.Content2.ToString( "D5" ) );
                sb.Append( (analysis.Content2 + length - 1).ToString( "D5" ) );
            }
            else if(analysis.Content1 == "IX" || analysis.Content1 == "IY" || analysis.Content1 == "ID")
            {
                sb.Append( "RD" );
                sb.Append( analysis.Content1 );
                sb.Append( "000000000" );
            }
            else if (analysis.Content1 == "C" || analysis.Content1 == "T" )
            {
                sb.Append( "RS" );
                sb.Append( analysis.Content2.ToString( "D4" ) );
                sb.Append( (analysis.Content2 + length - 1).ToString( "D4" ) );
            }
            else
            {
                return new OperateResult<byte[]>( StringResources.Language.NotSupportedDataType );
            }

            sb.Append( CalculateCrc( sb ) );
            sb.Append( '\u000D' );

            return OperateResult.CreateSuccessResult( Encoding.ASCII.GetBytes( sb.ToString( ) ) );
        }

        /// <summary>
        /// 创建批量读取触点的报文指令
        /// </summary>
        /// <param name="station">设备站号</param>
        /// <param name="address">地址信息</param>
        /// <param name="values">数据值</param>
        /// <param name="length">数据长度</param>
        /// <returns>包含是否成功的结果对象</returns>
        public static OperateResult<byte[]> BuildWriteCommand( byte station, string address, byte[] values, short length = -1 )
        {
            // 参数检查
            if (address == null) return new OperateResult<byte[]>( StringResources.Language.PanasonicAddressParameterCannotBeNull );

            // 解析地址
            OperateResult<string, int> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            // 确保偶数长度
            values = BasicFramework.SoftBasic.ArrayExpandToLengthEven( values );
            if (length == -1) length = (short)(values.Length / 2);

            StringBuilder sb = new StringBuilder( "%" );
            sb.Append( station.ToString( "X2" ) );
            sb.Append( "#" );

            if (analysis.Content1 == "X" || analysis.Content1 == "Y" || analysis.Content1 == "R" || analysis.Content1 == "L")
            {
                sb.Append( "WCC" );
                sb.Append( analysis.Content1 );
                sb.Append( analysis.Content2.ToString( "D4" ) );
                sb.Append( (analysis.Content2 + length - 1).ToString( "D4" ) );
            }
            else if (analysis.Content1 == "D" || analysis.Content1 == "L" || analysis.Content1 == "F")
            {
                sb.Append( "WD" );
                sb.Append( analysis.Content1 );
                sb.Append( analysis.Content2.ToString( "D5" ) );
                sb.Append( (analysis.Content2 + length - 1).ToString( "D5" ) );
            }
            else if (analysis.Content1 == "IX" || analysis.Content1 == "IY" || analysis.Content1 == "ID")
            {
                sb.Append( "WD" );
                sb.Append( analysis.Content1 );
                sb.Append( analysis.Content2.ToString( "D9" ) );
                sb.Append( (analysis.Content2 + length - 1).ToString( "D9" ) );
            }
            else if (analysis.Content1 == "C" || analysis.Content1 == "T")
            {
                sb.Append( "WS" );
                sb.Append( analysis.Content2.ToString( "D4" ) );
                sb.Append( (analysis.Content2 + length - 1).ToString( "D4" ) );
            }
            
            sb.Append( BasicFramework.SoftBasic.ByteToHexString( values ) );

            sb.Append( CalculateCrc( sb ) );
            sb.Append( '\u000D' );

            return OperateResult.CreateSuccessResult( Encoding.ASCII.GetBytes( sb.ToString( ) ) );
        }

        /// <summary>
        /// 检查从PLC反馈的数据，并返回正确的数据内容
        /// </summary>
        /// <param name="response">反馈信号</param>
        /// <returns>是否成功的结果信息</returns>
        public static OperateResult<byte[]> ExtraActualData( byte[] response )
        {
            if (response.Length < 9) return new OperateResult<byte[]>( StringResources.Language.PanasonicReceiveLengthMustLargerThan9 );

            if(response[3] == '$')
            {
                byte[] data = new byte[response.Length - 9];
                if (data.Length > 0)
                {
                    Array.Copy( response, 6, data, 0, data.Length );
                    data = BasicFramework.SoftBasic.HexStringToBytes( Encoding.ASCII.GetString( data ) );
                }
                return OperateResult.CreateSuccessResult( data );
            }
            else if(response[3] == '!')
            {
                int err = int.Parse(Encoding.ASCII.GetString( response, 4, 2 ));
                string msg = string.Empty;
                switch (err)
                {
                    case 20: msg = StringResources.Language.PanasonicMewStatus20; break;
                    case 21: msg = StringResources.Language.PanasonicMewStatus21; break;
                    case 22: msg = StringResources.Language.PanasonicMewStatus22; break;
                    case 23: msg = StringResources.Language.PanasonicMewStatus23; break;
                    case 24: msg = StringResources.Language.PanasonicMewStatus24; break;
                    case 25: msg = StringResources.Language.PanasonicMewStatus25; break;
                    case 26: msg = StringResources.Language.PanasonicMewStatus26; break;
                    case 27: msg = StringResources.Language.PanasonicMewStatus27; break;
                    case 28: msg = StringResources.Language.PanasonicMewStatus28; break;
                    case 29: msg = StringResources.Language.PanasonicMewStatus29; break;
                    case 30: msg = StringResources.Language.PanasonicMewStatus30; break;
                    case 40: msg = StringResources.Language.PanasonicMewStatus40; break;
                    case 41: msg = StringResources.Language.PanasonicMewStatus41; break;
                    case 42: msg = StringResources.Language.PanasonicMewStatus42; break;
                    case 43: msg = StringResources.Language.PanasonicMewStatus43; break;
                    case 50: msg = StringResources.Language.PanasonicMewStatus50; break;
                    case 51: msg = StringResources.Language.PanasonicMewStatus51; break;
                    case 52: msg = StringResources.Language.PanasonicMewStatus52; break;
                    case 53: msg = StringResources.Language.PanasonicMewStatus53; break;
                    case 60: msg = StringResources.Language.PanasonicMewStatus60; break;
                    case 61: msg = StringResources.Language.PanasonicMewStatus61; break;
                    case 62: msg = StringResources.Language.PanasonicMewStatus62; break;
                    case 63: msg = StringResources.Language.PanasonicMewStatus63; break;
                    case 65: msg = StringResources.Language.PanasonicMewStatus65; break;
                    case 66: msg = StringResources.Language.PanasonicMewStatus66; break;
                    case 67: msg = StringResources.Language.PanasonicMewStatus67; break;
                    default: msg = StringResources.Language.UnknownError; break;
                }
                return new OperateResult<byte[]>( err, msg );
            }
            else
            {
                return new OperateResult<byte[]>( StringResources.Language.UnknownError );
            }
        }
        

        #endregion

    }
}
