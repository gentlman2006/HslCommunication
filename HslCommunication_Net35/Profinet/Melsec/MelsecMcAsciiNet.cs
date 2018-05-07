using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.Melsec
{
    /// <summary>
    /// 三菱PLC通讯类，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为ASCII通讯格式
    /// </summary>
    public class MelsecMcAsciiNet : NetworkDoubleBase<MelsecQnA3EAsciiMessage, RegularByteTransform>, IReadWriteNet
    {
        #region Constructor

        /// <summary>
        /// 实例化三菱的Qna兼容3E帧协议的通讯对象
        /// </summary>
        public MelsecMcAsciiNet( )
        {

        }

        /// <summary>
        /// 实例化一个三菱的Qna兼容3E帧协议的通讯对象
        /// </summary>
        /// <param name="ipAddress">PLCd的Ip地址</param>
        /// <param name="port">PLC的端口</param>
        public MelsecMcAsciiNet( string ipAddress, int port )
        {
            IpAddress = ipAddress;
            Port = port;
        }

        #endregion

        #region Public Member

        /// <summary>
        /// 网络号
        /// </summary>
        public byte NetworkNumber { get; set; } = 0x00;

        /// <summary>
        /// 网络站号
        /// </summary>
        public byte NetworkStationNumber { get; set; } = 0x00;


        #endregion

        #region Address Analysis

        /// <summary>
        /// 解析数据地址
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns></returns>
        private OperateResult<MelsecMcDataType, ushort> AnalysisAddress( string address )
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
                            result.Content1 = MelsecMcDataType.Z;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.Z.FromBase );
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

        #region Build Command
        



        private byte[] BuildBytesFromData(byte value)
        {
            return Encoding.ASCII.GetBytes(value.ToString("X2")) ;
        }

        private byte[] BuildBytesFromData( short value )
        {
            return Encoding.ASCII.GetBytes( value.ToString( "X4" ) );
        }

        private byte[] BuildBytesFromData( ushort value )
        {
            return Encoding.ASCII.GetBytes( value.ToString( "X4" ) );
        }


        private byte[] BuildBytesFromAddress( int address, MelsecMcDataType type )
        {
            return Encoding.ASCII.GetBytes( address.ToString( type.FromBase == 10 ? "D6" : "X6" ) );
        }

        /// <summary>
        /// 根据类型地址长度确认需要读取的指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">长度</param>
        /// <returns>带有成功标志的指令数据</returns>
        private OperateResult<MelsecMcDataType, byte[]> BuildReadCommand( string address, ushort length )
        {
            var result = new OperateResult<MelsecMcDataType, byte[]>( );
            var analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }

            // 默认信息----注意：高低字节交错

            byte[] _PLCCommand = new byte[42];
            _PLCCommand[0] = 0x35;                                      // 副标题
            _PLCCommand[1] = 0x30;
            _PLCCommand[2] = 0x30;
            _PLCCommand[3] = 0x30;
            _PLCCommand[4] = BuildBytesFromData( NetworkNumber )[0];                // 网络号
            _PLCCommand[5] = BuildBytesFromData( NetworkNumber )[1];
            _PLCCommand[6] = 0x46;                         // PLC编号
            _PLCCommand[7] = 0x46;
            _PLCCommand[8] = 0x30;                         // 目标模块IO编号
            _PLCCommand[9] = 0x33;
            _PLCCommand[10] = 0x46;
            _PLCCommand[11] = 0x46;
            _PLCCommand[12] = BuildBytesFromData( NetworkStationNumber )[0];         // 目标模块站号
            _PLCCommand[13] = BuildBytesFromData( NetworkStationNumber )[1];
            _PLCCommand[14] = 0x30;                         // 请求数据长度
            _PLCCommand[15] = 0x30;
            _PLCCommand[16] = 0x31;
            _PLCCommand[17] = 0x38;
            _PLCCommand[18] = 0x30;                         // CPU监视定时器
            _PLCCommand[19] = 0x30;
            _PLCCommand[20] = 0x31;
            _PLCCommand[21] = 0x30;
            _PLCCommand[22] = 0x30;                        // 批量读取数据命令
            _PLCCommand[23] = 0x34;
            _PLCCommand[24] = 0x30;
            _PLCCommand[25] = 0x31;
            _PLCCommand[26] = 0x30;                         // 以点为单位还是字为单位成批读取
            _PLCCommand[27] = 0x30;
            _PLCCommand[28] = 0x30;
            _PLCCommand[29] = analysis.Content1.DataType == 0 ? (byte)0x30 : (byte)0x31;
            _PLCCommand[30] = Encoding.ASCII.GetBytes( analysis.Content1.AsciiCode )[0];                          // 软元件类型
            _PLCCommand[31] = Encoding.ASCII.GetBytes( analysis.Content1.AsciiCode )[1];
            _PLCCommand[32] = BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[0];                   // 起始地址的地位
            _PLCCommand[33] = BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[1];
            _PLCCommand[34] = BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[2];
            _PLCCommand[35] = BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[3];
            _PLCCommand[36] = BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[4];
            _PLCCommand[37] = BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[5];
            _PLCCommand[38] = BuildBytesFromData(length)[0];                                                      // 软元件点数
            _PLCCommand[39] = BuildBytesFromData( length )[1];
            _PLCCommand[40] = BuildBytesFromData( length )[2];
            _PLCCommand[41] = BuildBytesFromData( length )[3];
            result.Content1 = analysis.Content1;
            result.Content2 = _PLCCommand;
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 根据类型地址以及需要写入的数据来生成指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value"></param>
        /// <returns></returns>
        private OperateResult<MelsecMcDataType, byte[]> BuildWriteCommand( string address, byte[] value )
        {
            var result = new OperateResult<MelsecMcDataType, byte[]>( );
            var analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }

            // 默认信息----注意：高低字节交错

            byte[] _PLCCommand = new byte[42 + value.Length];

            _PLCCommand[0] = 0x35;                                      // 副标题
            _PLCCommand[1] = 0x30;
            _PLCCommand[2] = 0x30;
            _PLCCommand[3] = 0x30;
            _PLCCommand[4] = BuildBytesFromData( NetworkNumber )[0];                // 网络号
            _PLCCommand[5] = BuildBytesFromData( NetworkNumber )[1];
            _PLCCommand[6] = 0x46;                         // PLC编号
            _PLCCommand[7] = 0x46;
            _PLCCommand[8] = 0x30;                         // 目标模块IO编号
            _PLCCommand[9] = 0x33;
            _PLCCommand[10] = 0x46;
            _PLCCommand[11] = 0x46;
            _PLCCommand[12] = BuildBytesFromData( NetworkStationNumber )[0];         // 目标模块站号
            _PLCCommand[13] = BuildBytesFromData( NetworkStationNumber )[1];
            _PLCCommand[14] = BuildBytesFromData( (ushort)(_PLCCommand.Length - 18) )[0]; // 请求数据长度
            _PLCCommand[15] = BuildBytesFromData( (ushort)(_PLCCommand.Length - 18) )[1];
            _PLCCommand[16] = BuildBytesFromData( (ushort)(_PLCCommand.Length - 18) )[2];
            _PLCCommand[17] = BuildBytesFromData( (ushort)(_PLCCommand.Length - 18) )[3];
            _PLCCommand[18] = 0x30; // CPU监视定时器
            _PLCCommand[19] = 0x30;
            _PLCCommand[20] = 0x31;
            _PLCCommand[21] = 0x30;
            _PLCCommand[22] = 0x31; // 批量写入的命令
            _PLCCommand[23] = 0x34;
            _PLCCommand[24] = 0x30;
            _PLCCommand[25] = 0x31;
            _PLCCommand[26] = 0x30; // 子命令
            _PLCCommand[27] = 0x30;
            _PLCCommand[28] = 0x30;
            _PLCCommand[29] = analysis.Content1.DataType == 0 ? (byte)0x30 : (byte)0x31;
            _PLCCommand[30] = Encoding.ASCII.GetBytes( analysis.Content1.AsciiCode )[0];                          // 软元件类型
            _PLCCommand[31] = Encoding.ASCII.GetBytes( analysis.Content1.AsciiCode )[1];
            _PLCCommand[32] = BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[0];                   // 起始地址的地位
            _PLCCommand[33] = BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[1];
            _PLCCommand[34] = BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[2];
            _PLCCommand[35] = BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[3];
            _PLCCommand[36] = BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[4];
            _PLCCommand[37] = BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[5];

            // 判断是否进行位操作
            if (analysis.Content1.DataType == 1)
            {
                _PLCCommand[38] = BuildBytesFromData( (ushort)value.Length )[0];                                                      // 软元件点数
                _PLCCommand[39] = BuildBytesFromData( (ushort)value.Length )[1];
                _PLCCommand[40] = BuildBytesFromData( (ushort)value.Length )[2];
                _PLCCommand[41] = BuildBytesFromData( (ushort)value.Length )[3];
            }
            else
            {
                _PLCCommand[38] = BuildBytesFromData( (ushort)(value.Length / 4) )[0];                                                      // 软元件点数
                _PLCCommand[39] = BuildBytesFromData( (ushort)(value.Length / 4) )[1];
                _PLCCommand[40] = BuildBytesFromData( (ushort)(value.Length / 4) )[2];
                _PLCCommand[41] = BuildBytesFromData( (ushort)(value.Length / 4) )[3];
            }
            Array.Copy( value, 0, _PLCCommand, 42, value.Length );

            result.Content1 = analysis.Content1;
            result.Content2 = _PLCCommand;
            result.IsSuccess = true;

            // Console.WriteLine( value.Length );
            // Console.WriteLine( Encoding.ASCII.GetString(_PLCCommand ));
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
        public OperateResult<T> ReadCustomer<T>( string address ) where T : IDataTransfer, new()
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
        public OperateResult WriteCustomer<T>( string address, T data ) where T : IDataTransfer, new()
        {
            return Write( address, data.ToSource( ) );
        }


        #endregion

        #region Read Support

        /// <summary>
        /// 从三菱PLC中读取想要的数据，返回读取结果
        /// </summary>
        /// <param name="address">读取地址，格式为"M100","D100","W1A0"</param>
        /// <param name="length">读取的数据长度，字最大值960，位最大值7168</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<byte[]> Read( string address, ushort length )
        {
            var result = new OperateResult<byte[]>( );
            //获取指令
            var command = BuildReadCommand( address, length );
            if (!command.IsSuccess)
            {
                result.CopyErrorFromOther( command );
                return result;
            }

            var read = ReadFromCoreServer( command.Content2 );
            if (read.IsSuccess)
            {
                byte[] buffer = new byte[4];
                buffer[0] = read.Content[18];
                buffer[1] = read.Content[19];
                buffer[2] = read.Content[20];
                buffer[3] = read.Content[21];
                result.ErrorCode = Convert.ToUInt16( Encoding.ASCII.GetString( buffer ), 16 );
                if (result.ErrorCode == 0)
                {
                    if (command.Content1.DataType == 0x01)
                    {
                        result.Content = new byte[read.Content.Length - 22];
                        for (int i = 22; i < read.Content.Length; i++)
                        {
                            if (read.Content[i]== 0x30)
                            {
                                result.Content[i - 22] = 0x00;
                            }
                            else
                            {
                                result.Content[i - 22] = 0x01;
                            }
                        }
                    }
                    else
                    {
                        result.Content = new byte[(read.Content.Length - 22) / 2];
                        for (int i = 0; i < result.Content.Length / 2; i++)
                        {
                            buffer = new byte[4];
                            buffer[0] = read.Content[i * 4 + 22];
                            buffer[1] = read.Content[i * 4 + 23];
                            buffer[2] = read.Content[i * 4 + 24];
                            buffer[3] = read.Content[i * 4 + 25];

                            ushort tmp = Convert.ToUInt16( Encoding.ASCII.GetString( buffer ), 16 );
                            BitConverter.GetBytes( tmp ).CopyTo( result.Content, i * 2 );
                        }
                    }
                    result.IsSuccess = true;
                }
                else
                {
                    result.Message = "请翻查三菱通讯手册来查看具体的信息。";
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
        /// 从三菱PLC中批量读取位软元件，返回读取结果
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">读取的长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<bool[]> ReadBool( string address, ushort length )
        {
            var result = new OperateResult<bool[]>( );
            var analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }
            else
            {
                if (analysis.Content1.DataType == 0x00)
                {
                    result.Message = "读取位变量数组只能针对位软元件，如果读取字软元件，请自行转化";
                    return result;
                }
            }
            var read = Read( address, length );
            if (!read.IsSuccess)
            {
                result.CopyErrorFromOther( read );
                return result;
            }

            result.Content = new bool[read.Content.Length];
            for (int i = 0; i < read.Content.Length; i++)
            {
                result.Content[i] = read.Content[i] == 0x01;
            }
            result.IsSuccess = true;
            return result;
        }


        /// <summary>
        /// 从三菱PLC中批量读取位软元件，返回读取结果
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<bool> ReadBool( string address )
        {
            OperateResult<bool[]> read = ReadBool( address, 1 );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool>( read );

            return OperateResult.CreateSuccessResult<bool>( read.Content[0] );
        }


        /// <summary>
        /// 读取三菱PLC中字软元件指定地址的short数据
        /// </summary>
        /// <param name="address">起始地址，格式为"D100"，"W1A0"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<short> ReadInt16( string address )
        {
            return GetInt16ResultFromBytes( Read( address, 1 ) );
        }


        /// <summary>
        /// 读取三菱PLC中字软元件指定地址的ushort数据
        /// </summary>
        /// <param name="address">起始地址，格式为"D100"，"W1A0"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<ushort> ReadUInt16( string address )
        {
            return GetUInt16ResultFromBytes( Read( address, 1 ) );
        }

        /// <summary>
        /// 读取三菱PLC中字软元件指定地址的int数据
        /// </summary>
        /// <param name="address">起始地址，格式为"D100"，"W1A0"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<int> ReadInt32( string address )
        {
            return GetInt32ResultFromBytes( Read( address, 2 ) );
        }

        /// <summary>
        /// 读取三菱PLC中字软元件指定地址的uint数据
        /// </summary>
        /// <param name="address">起始地址，格式为"D100"，"W1A0"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<uint> ReadUInt32( string address )
        {
            return GetUInt32ResultFromBytes( Read( address, 2 ) );
        }

        /// <summary>
        /// 读取三菱PLC中字软元件指定地址的float数据
        /// </summary>
        /// <param name="address">起始地址，格式为"D100"，"W1A0"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<float> ReadFloat( string address )
        {
            return GetSingleResultFromBytes( Read( address, 2 ) );
        }

        /// <summary>
        /// 读取三菱PLC中字软元件指定地址的long数据
        /// </summary>
        /// <param name="address">起始地址，格式为"D100"，"W1A0"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<long> ReadInt64( string address )
        {
            return GetInt64ResultFromBytes( Read( address, 4 ) );
        }

        /// <summary>
        /// 读取三菱PLC中字软元件指定地址的ulong数据
        /// </summary>
        /// <param name="address">起始地址，格式为"D100"，"W1A0"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<ulong> ReadUInt64( string address )
        {
            return GetUInt64ResultFromBytes( Read( address, 4 ) );
        }

        /// <summary>
        /// 读取三菱PLC中字软元件指定地址的double数据
        /// </summary>
        /// <param name="address">起始地址，格式为"D100"，"W1A0"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<double> ReadDouble( string address )
        {
            return GetDoubleResultFromBytes( Read( address, 4 ) );
        }

        /// <summary>
        /// 读取三菱PLC中字软元件地址地址的String数据，编码为ASCII
        /// </summary>
        /// <param name="address">起始地址，格式为"D100"，"W1A0"</param>
        /// <param name="length">字符串长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<string> ReadString( string address, ushort length )
        {
            return GetStringResultFromBytes( Read( address, length ) );
        }



        #endregion

        #region Write Base


        /// <summary>
        /// 向PLC写入数据，数据格式为原始的字节类型
        /// </summary>
        /// <param name="address">初始地址</param>
        /// <param name="value">原始的字节数据</param>
        /// <returns>结果</returns>
        public OperateResult Write( string address, byte[] value )
        {
            // Console.WriteLine( BasicFramework.SoftBasic.ByteToHexString( value ) );

            OperateResult<byte[]> result = new OperateResult<byte[]>( );

            //获取指令
            var analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }

            OperateResult<MelsecMcDataType, byte[]> command;
            // 预处理指令
            if (analysis.Content1.DataType == 0x01)
            {
                byte[] buffer = new byte[value.Length];

                for (int i = 0; i < buffer.Length; i++)
                {
                    if (value[i] == 0x00)
                    {
                        buffer[i] = 0x30;
                    }
                    else
                    {
                        buffer[i] = 0x31;
                    }
                }

                // 位写入
                command = BuildWriteCommand( address, buffer );
            }
            else
            {
                // 字写入
                byte[] buffer = new byte[value.Length * 2];
                for (int i = 0; i < value.Length / 2; i++)
                {
                    BuildBytesFromData( BitConverter.ToUInt16( value, i * 2 ) ).CopyTo( buffer, 4 * i );
                }

                command = BuildWriteCommand( address, buffer );
            }

            if (!command.IsSuccess)
            {
                result.CopyErrorFromOther( command );
                return result;
            }

            OperateResult<byte[]> read = ReadFromCoreServer( command.Content2 );
            if (read.IsSuccess)
            {
                byte[] buffer = new byte[4];
                buffer[0] = read.Content[18];
                buffer[1] = read.Content[19];
                buffer[2] = read.Content[20];
                buffer[3] = read.Content[21];
                result.ErrorCode = Convert.ToInt16( Encoding.ASCII.GetString( buffer ), 16 );
                if (result.ErrorCode == 0)
                {
                    result.IsSuccess = true;
                }
            }
            else
            {
                result.ErrorCode = read.ErrorCode;
                result.Message = read.Message;
            }

            return result;
        }




        #endregion

        #region Write String


        /// <summary>
        /// 向PLC中字软元件写入字符串，编码格式为ASCII
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回读取结果</returns>
        public OperateResult Write( string address, string value )
        {
            byte[] temp = Encoding.ASCII.GetBytes( value );
            return Write( address, temp );
        }

        /// <summary>
        /// 向PLC中字软元件写入指定长度的字符串,超出截断，不够补0，编码格式为ASCII
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>返回读取结果</returns>
        public OperateResult Write( string address, string value, int length )
        {
            byte[] temp = Encoding.ASCII.GetBytes( value );
            temp = BasicFramework.SoftBasic.ArrayExpandToLength( temp, length );
            return Write( address, temp );
        }

        /// <summary>
        /// 向PLC中字软元件写入字符串，编码格式为Unicode
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回读取结果</returns>
        public OperateResult WriteUnicodeString( string address, string value )
        {
            byte[] temp = Encoding.Unicode.GetBytes( value );
            return Write( address, temp );
        }

        /// <summary>
        /// 向PLC中字软元件写入指定长度的字符串,超出截断，不够补0，编码格式为Unicode
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>返回读取结果</returns>
        public OperateResult WriteUnicodeString( string address, string value, int length )
        {
            byte[] temp = Encoding.Unicode.GetBytes( value );
            temp = BasicFramework.SoftBasic.ArrayExpandToLength( temp, length * 2 );
            return Write( address, temp );
        }

        #endregion

        #region Write bool[]

        /// <summary>
        /// 向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据，长度为8的倍数</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, bool value )
        {
            return Write( address, new bool[] { value } );
        }

        /// <summary>
        /// 向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据，可以指定任意的长度</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, bool[] values )
        {
            return Write( address, values.Select( m => m ? (byte)0x01 : (byte)0x00 ).ToArray( ) );
        }

        #endregion

        #region Write Short

        /// <summary>
        /// 向PLC中字软元件写入short数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, short[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入short数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, short value )
        {
            return Write( address, new short[] { value } );
        }

        #endregion

        #region Write UShort


        /// <summary>
        /// 向PLC中字软元件写入ushort数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, ushort[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }


        /// <summary>
        /// 向PLC中字软元件写入ushort数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, ushort value )
        {
            return Write( address, new ushort[] { value } );
        }


        #endregion

        #region Write Int

        /// <summary>
        /// 向PLC中字软元件写入int数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, int[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入int数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, int value )
        {
            return Write( address, new int[] { value } );
        }

        #endregion

        #region Write UInt

        /// <summary>
        /// 向PLC中字软元件写入uint数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, uint[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入uint数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, uint value )
        {
            return Write( address, new uint[] { value } );
        }

        #endregion

        #region Write Float

        /// <summary>
        /// 向PLC中字软元件写入float数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, float[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入float数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, float value )
        {
            return Write( address, new float[] { value } );
        }


        #endregion

        #region Write Long

        /// <summary>
        /// 向PLC中字软元件写入long数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, long[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入long数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, long value )
        {
            return Write( address, new long[] { value } );
        }

        #endregion

        #region Write ULong

        /// <summary>
        /// 向PLC中字软元件写入ulong数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, ulong[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入ulong数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, ulong value )
        {
            return Write( address, new ulong[] { value } );
        }

        #endregion

        #region Write Double

        /// <summary>
        /// 向PLC中字软元件写入double数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, double[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入double数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, double value )
        {
            return Write( address, new double[] { value } );
        }

        #endregion

        #region Object Override

        /// <summary>
        /// 获取当前对象的字符串标识形式
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString( )
        {
            return "MelsecMcNet";
        }

        #endregion



    }
}
