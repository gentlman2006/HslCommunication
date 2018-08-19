using HslCommunication.Core;
using HslCommunication.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.Melsec
{
    /// <summary>
    /// 三菱的串口通信的对象，适用于读取FX系列的串口数据
    /// </summary>
    public class MelsecSerial : SerialDeviceBase<RegularByteTransform>
    {

        #region Constructor

        /// <summary>
        /// 实例化三菱的串口协议的通讯对象
        /// </summary>
        public MelsecSerial( )
        {
            WordLength = 1;
        }
        
        
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

        #region Build Command

        

        private byte[] BuildBytesFromData( byte value )
        {
            return Encoding.ASCII.GetBytes( value.ToString( "X2" ) );
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
        /// 返回读取的地址及长度信息，第一个参数为地址，第二个参数为长度
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="startAddress"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private OperateResult<ushort,ushort> CalculateWordStartAddress( MelsecMcDataType dataType, ushort startAddress ,ushort length)
        {
            if (dataType == MelsecMcDataType.D)
            {
                length = (ushort)(length * 2);
                if (startAddress >= 8000)
                {
                    startAddress = (ushort)((startAddress - 8000) * 2 + 0x0E00);
                }
                else
                {
                    startAddress = (ushort)(startAddress * 2 + 0x1000);
                }
            }
            else if (dataType == MelsecMcDataType.C)
            {
                if(startAddress >= 200)
                {
                    length = (ushort)(length * 2);
                    startAddress = (ushort)((startAddress - 200) * 4 + 0x0C00);
                }
                else
                {
                    length = (ushort)(length * 2);
                    startAddress = (ushort)(startAddress * 2 + 0x0A00);
                }
            }
            else if (dataType == MelsecMcDataType.T)
            {
                length = (ushort)(length * 2);
                startAddress = (ushort)(startAddress * 2 + 0x0800);
            }
            else
            {
                return new OperateResult<ushort,ushort>( ) { Message = "当前类型不支持字读取" };
            }

            return OperateResult.CreateSuccessResult( startAddress ,(ushort)0);
        }



        /// <summary>
        /// 根据类型地址长度确认需要读取的指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">长度</param>
        /// <returns>带有成功标志的指令数据</returns>
        private OperateResult<byte[]> BuildReadWordCommand( string address, ushort length )
        {
            var result = new OperateResult<byte[]>( );
            var analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }
            
            byte[] _PLCCommand = new byte[11];
            _PLCCommand[0] = 0x02;             // STX
            _PLCCommand[1] = 0x30;             // Read

            length = (ushort)(length * 2);
            ushort startAddress = analysis.Content2;
            if (analysis.Content1 == MelsecMcDataType.D)
            {
                if (startAddress >= 8000)
                {
                    startAddress = (ushort)(startAddress * 2 + 0x0E00);
                }
                else
                {
                    startAddress = (ushort)(startAddress * 2 + 0x1000);
                }
            }
            else if (analysis.Content1 == MelsecMcDataType.C)
            {
                startAddress = (ushort)(startAddress * 2 + 0x0A00);
            }
            else if (analysis.Content1 == MelsecMcDataType.T)
            {
                startAddress = (ushort)(startAddress * 2 + 0x0800);
            }
            else
            {
                return new OperateResult<byte[]>( ) { Message = "当前类型不支持字读取" };
            }

            _PLCCommand[2] = BuildBytesFromData( startAddress )[0];      // 偏移地址
            _PLCCommand[3] = BuildBytesFromData( startAddress )[1];
            _PLCCommand[4] = BuildBytesFromData( startAddress )[2];
            _PLCCommand[5] = BuildBytesFromData( startAddress )[3];

            _PLCCommand[6] = BuildBytesFromData( (byte)length )[0];      // 读取长度
            _PLCCommand[7] = BuildBytesFromData( (byte)length )[1];

            _PLCCommand[8] = 0x03;                                       // ETX
            CalculateCRC( _PLCCommand ).CopyTo( _PLCCommand, 9 );        // CRC
            
            result.Content = _PLCCommand;
            result.IsSuccess = true;
            return result;
        }



        /// <summary>
        /// 根据类型地址长度确认需要读取的指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">bool数组长度</param>
        /// <returns>带有成功标志的指令数据</returns>
        private OperateResult<byte[], int> BuildReadBoolCommand( string address, ushort length )
        {
            var result = new OperateResult<byte[], int>( );
            var analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }

            // 默认信息----注意：高低字节交错

            byte[] _PLCCommand = new byte[11];
            _PLCCommand[0] = 0x02;             // STX
            _PLCCommand[1] = 0x30;             // Read

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
            else if (analysis.Content1 == MelsecMcDataType.S)
            {
                startAddress = (ushort)(startAddress / 8 + 0x0000);
            }
            else if (analysis.Content1 == MelsecMcDataType.X)
            {
                startAddress = (ushort)(startAddress / 8 + 0x0080);
            }
            else if (analysis.Content1 == MelsecMcDataType.Y)
            {
                startAddress = (ushort)(startAddress / 8 + 0x00A0);
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
                return new OperateResult<byte[], int>( ) { Message = "当前类型不支持位读取" };
            }


            _PLCCommand[2] = BuildBytesFromData( startAddress )[0];      // 偏移地址
            _PLCCommand[3] = BuildBytesFromData( startAddress )[1];
            _PLCCommand[4] = BuildBytesFromData( startAddress )[2];
            _PLCCommand[5] = BuildBytesFromData( startAddress )[3];

            _PLCCommand[6] = BuildBytesFromData( (byte)(length / 8 + 1) )[0];      // 读取长度
            _PLCCommand[7] = BuildBytesFromData( (byte)(length / 8 + 1) )[1];

            _PLCCommand[8] = 0x03;                                       // ETX
            CalculateCRC( _PLCCommand ).CopyTo( _PLCCommand, 9 );        // CRC

            result.Content1 = _PLCCommand;
            result.Content2 = startAddress % 8;
            result.IsSuccess = true;
            return result;
        }



        /// <summary>
        /// 计算指令的和校验信息
        /// </summary>
        /// <param name="data">字节数据</param>
        /// <returns>校验之后的数据</returns>
        private byte[] CalculateCRC( byte[] data )
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
        private bool CheckCRC( byte[] data )
        {
            byte[] crc = CalculateCRC( data );
            if (crc[0] != data[data.Length - 2]) return false;
            if (crc[1] != data[data.Length - 1]) return false;
            return true;
        }


        private OperateResult CheckPlcResponse(byte[] ack ,byte corrent)
        {
            if (ack.Length == 0) return new OperateResult( ) { Message = "receive data length : 0" };
            if (ack[0] == 0x15) return new OperateResult( ) { Message = "plc ack nagative" };
            if (ack[0] == corrent) return new OperateResult( ) { Message = "plc ack wrong : " + ack[0] };


            if (!CheckCRC( ack ))
            {
                return new OperateResult( ) { Message = "Check CRC Failed" };
            }
            else
            {
                return OperateResult.CreateSuccessResult( );
            }
        }


        /// <summary>
        /// 根据类型地址以及需要写入的数据来生成指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value"></param>
        /// <returns></returns>
        private OperateResult<byte[]> BuildWriteWordCommand( string address, byte[] value )
        {
            var result = new OperateResult<byte[]>( );
            var analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }

            byte[] _PLCCommand = new byte[11 + value.Length];
            _PLCCommand[0] = 0x02;             // STX
            _PLCCommand[1] = 0x31;             // Read

            ushort startAddress = analysis.Content2;
            if (analysis.Content1 == MelsecMcDataType.D)
            {
                startAddress += (ushort)(startAddress * 2 + 0x1000);
            }
            else if (analysis.Content1 == MelsecMcDataType.C)
            {
                startAddress += (ushort)(startAddress * 2 + 0x0A00);
            }
            else if (analysis.Content1 == MelsecMcDataType.T)
            {
                startAddress += (ushort)(startAddress * 2 + 0x0800);
            }
            else
            {
                return new OperateResult<byte[]>( ) { Message = "当前类型不支持字写入" };
            }
            

            _PLCCommand[2] = BuildBytesFromData( startAddress )[0];      // 偏移地址
            _PLCCommand[3] = BuildBytesFromData( startAddress )[1];
            _PLCCommand[4] = BuildBytesFromData( startAddress )[2];
            _PLCCommand[5] = BuildBytesFromData( startAddress )[3];

            _PLCCommand[6] = BuildBytesFromData( (byte)(value.Length / 2 ) )[0];      // 读取长度
            _PLCCommand[7] = BuildBytesFromData( (byte)(value.Length / 2 ) )[1];


            Array.Copy( value, 0, _PLCCommand, 8, value.Length );


            _PLCCommand[_PLCCommand.Length - 3] = 0x03;                                       // ETX
            CalculateCRC( _PLCCommand ).CopyTo( _PLCCommand, _PLCCommand.Length - 2 );        // CRC
            
            result.Content = _PLCCommand;
            result.IsSuccess = true;

            // Console.WriteLine( value.Length );
            // Console.WriteLine( Encoding.ASCII.GetString(_PLCCommand ));
            return result;
        }


        /// <summary>
        /// 根据类型地址以及需要写入的数据来生成指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value"></param>
        /// <returns></returns>
        private OperateResult<byte[]> BuildWriteBoolCommand( string address, bool value )
        {
            var result = new OperateResult<byte[]>( );
            var analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }

            byte[] _PLCCommand = new byte[9];
            _PLCCommand[0] = 0x02;                            // STX
            _PLCCommand[1] = value ? (byte)0x37 : (byte)0x38;             // Read

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
            else if (analysis.Content1 == MelsecMcDataType.S)
            {
                startAddress = (ushort)(startAddress / 8 + 0x0000);
            }
            else if (analysis.Content1 == MelsecMcDataType.X)
            {
                startAddress = (ushort)(startAddress / 8 + 0x0080);
            }
            else if (analysis.Content1 == MelsecMcDataType.Y)
            {
                startAddress = (ushort)(startAddress / 8 + 0x00A0);
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
                return new OperateResult<byte[]>( ) { Message = "当前类型不支持位读取" };
            }


            _PLCCommand[2] = BuildBytesFromData( startAddress )[0];      // 偏移地址
            _PLCCommand[3] = BuildBytesFromData( startAddress )[1];
            _PLCCommand[4] = BuildBytesFromData( startAddress )[2];
            _PLCCommand[5] = BuildBytesFromData( startAddress )[3];
            _PLCCommand[6] = 0x03;                                       // ETX
            CalculateCRC( _PLCCommand ).CopyTo( _PLCCommand, 7 );        // CRC

            result.Content = _PLCCommand;
            result.IsSuccess = true;
            return result;
        }

        #endregion

        #region Read Support

        /// <summary>
        /// 从三菱PLC中读取想要的数据，返回读取结果
        /// </summary>
        /// <param name="address">读取地址，格式为"M100","D100","W1A0"</param>
        /// <param name="length">读取的数据长度，字最大值960，位最大值7168</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <remarks>
        /// 地址支持的列表如下：
        /// <list type="table">
        ///   <listheader>
        ///     <term>地址名称</term>
        ///     <term>示例</term>
        ///     <term>地址进制</term>
        ///   </listheader>
        ///   <item>
        ///     <term>数据寄存器</term>
        ///     <term>D1000,D2000</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>定时器的值</term>
        ///     <term>T100,T200</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>计数器的值</term>
        ///     <term>C100,C200</term>
        ///     <term>10</term>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <example>
        /// 假设起始地址为D100，D100存储了温度，100.6℃值为1006，D101存储了压力，1.23Mpa值为123，D102，D103存储了产量计数，读取如下：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="ReadExample2" title="Read示例" />
        /// 以下是读取不同类型数据的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="ReadExample1" title="Read示例" />
        /// </example>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            var result = new OperateResult<byte[]>( );
            //获取指令
            var command = BuildReadWordCommand( address, length );
            if (!command.IsSuccess)
            {
                result.CopyErrorFromOther( command );
                return result;
            }

            var read = ReadBase( command.Content );
            if (read.IsSuccess)
            {
                OperateResult ackResult = CheckPlcResponse( read.Content, 0x02 );
                if (!ackResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( ackResult );

                result.Content = new byte[(read.Content.Length - 4) / 2];
                for (int i = 0; i < result.Content.Length / 2; i++)
                {
                    byte[] buffer = new byte[4];
                    buffer[0] = read.Content[i * 4 + 1];
                    buffer[1] = read.Content[i * 4 + 2];
                    buffer[2] = read.Content[i * 4 + 3];
                    buffer[3] = read.Content[i * 4 + 4];

                    ushort tmp = Convert.ToUInt16( Encoding.ASCII.GetString( buffer ), 16 );
                    BitConverter.GetBytes( tmp ).CopyTo( result.Content, i * 2 );
                }

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
        /// 从三菱PLC中批量读取位软元件，返回读取结果
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">读取的长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <remarks>
        /// 地址支持的列表如下：
        /// <list type="table">
        ///   <listheader>
        ///     <term>地址名称</term>
        ///     <term>示例</term>
        ///     <term>地址范围</term>
        ///     <term>地址进制</term>
        ///   </listheader>
        ///   <item>
        ///     <term>内部继电器</term>
        ///     <term>M100,M200</term>
        ///     <term>M0-M1023,M8000-M8255</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>输入继电器</term>
        ///     <term>X100,X1A0</term>
        ///     <term>X0-X177</term>
        ///     <term>8</term>
        ///   </item>
        ///   <item>
        ///     <term>输出继电器</term>
        ///     <term>Y10,Y20</term>
        ///     <term>Y0-Y177</term>
        ///     <term>8</term>
        ///   </item>
        ///   <item>
        ///     <term>步进继电器</term>
        ///     <term>S100,S200</term>
        ///     <term>S0-S999</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>定时器</term>
        ///     <term>T10,T20</term>
        ///     <term>T0-T255</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>计数器</term>
        ///     <term>C10,C20</term>
        ///     <term>C0-C255</term>
        ///     <term>10</term>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <example>
        ///  <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="ReadBool" title="Bool类型示例" />
        /// </example>
        public OperateResult<bool[]> ReadBool( string address, ushort length )
        {
            var result = new OperateResult<bool[]>( );
            //获取指令
            var command = BuildReadBoolCommand( address, length );
            if (!command.IsSuccess)
            {
                result.CopyErrorFromOther( command );
                return result;
            }

            var read = ReadBase( command.Content1 );
            if (read.IsSuccess)
            {
                OperateResult ackResult = CheckPlcResponse( read.Content ,0x02);
                if (!ackResult.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( ackResult );

                // 提取真实的数据
                result.Content = new bool[length];

                byte[] data = new byte[(read.Content.Length - 4) / 2];

                for (int i = 0; i < result.Content.Length; i++)
                {
                    byte[] buffer = new byte[2];
                    buffer[0] = read.Content[i * 2 + 1];
                    buffer[1] = read.Content[i * 2 + 2];

                    data[i] = Convert.ToByte( Encoding.ASCII.GetString( buffer ), 16 );
                }

                // 转化bool数组
                bool[] array = BasicFramework.SoftBasic.ByteToBoolArray( data, data.Length * 8 );
                for (int i = 0; i < length; i++)
                {
                    result.Content[i] = array[i + command.Content2];
                }

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
        /// 从三菱PLC中批量读取位软元件，返回读取结果
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>参照 <see cref="ReadBool(string, ushort)"/> 方法 </example>
        public OperateResult<bool> ReadBool( string address )
        {
            OperateResult<bool[]> read = ReadBool( address, 1 );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool>( read );

            return OperateResult.CreateSuccessResult<bool>( read.Content[0] );
        }


        #endregion

        #region Write Base


        /// <summary>
        /// 向PLC写入数据，数据格式为原始的字节类型
        /// </summary>
        /// <param name="address">初始地址</param>
        /// <param name="value">原始的字节数据</param>
        /// <example>
        /// 假设起始地址为D100，D100存储了温度，100.6℃值为1006，D101存储了压力，1.23Mpa值为123，D102，D103存储了产量计数，写入如下：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="WriteExample2" title="Write示例" />
        /// 以下是读取不同类型数据的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="WriteExample1" title="Write示例" />
        /// </example>
        /// <returns>结果</returns>
        public override OperateResult Write( string address, byte[] value )
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

            // 字写入
            byte[] buffer = new byte[value.Length * 2];
            for (int i = 0; i < value.Length / 2; i++)
            {
                BuildBytesFromData( BitConverter.ToUInt16( value, i * 2 ) ).CopyTo( buffer, 4 * i );
            }

            OperateResult<byte[]> command = BuildWriteWordCommand( address, buffer );
            if (!command.IsSuccess)
            {
                result.CopyErrorFromOther( command );
                return result;
            }

            OperateResult<byte[]> read = ReadBase( command.Content );
            if (read.IsSuccess)
            {
                OperateResult checkResult = CheckPlcResponse( read.Content, 0x06 );
                if (!checkResult.IsSuccess) return checkResult;

                result.IsSuccess = true;
            }
            else
            {
                result.ErrorCode = read.ErrorCode;
                result.Message = read.Message;
            }

            return result;
        }




        #endregion

        #region Write Bool

        /// <summary>
        /// 强制写入位数据的通断，支持的类型为X,Y,M,S,C,T
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="value">是否为通</param>
        /// <returns>是否写入成功的结果对象</returns>
        public OperateResult Write( string address, bool value )
        {
            OperateResult<byte[]> result = new OperateResult<byte[]>( );

            // 获取指令
            var analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }

            // 单个的位写入操作
            OperateResult<byte[]> command = BuildWriteBoolCommand( address, value );
            if (!command.IsSuccess)
            {
                result.CopyErrorFromOther( command );
                return result;
            }

            OperateResult<byte[]> read = ReadBase( command.Content );
            if (read.IsSuccess)
            {
                OperateResult checkResult = CheckPlcResponse( read.Content, 0x06 );
                if (!checkResult.IsSuccess) return checkResult;

                result.IsSuccess = true;
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
        /// 向PLC中字软元件写入指定长度的字符串,超出截断，不够补0，编码格式为ASCII
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>是否写入成功的结果对象</returns>
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
        /// <returns>是否写入成功的结果对象</returns>
        public OperateResult WriteUnicodeString( string address, string value, int length )
        {
            byte[] temp = Encoding.Unicode.GetBytes( value );
            temp = BasicFramework.SoftBasic.ArrayExpandToLength( temp, length * 2 );
            return Write( address, temp );
        }

        #endregion

        #region Object Override

        /// <summary>
        /// 获取当前对象的字符串标识形式
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString( )
        {
            return "MelsecSerial";
        }

        #endregion



    }
}
