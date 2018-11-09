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
    /// <example>
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecFxSerial.cs" region="Usage" title="简单的使用" />
    /// </example>
    public class MelsecFxSerial : SerialDeviceBase<RegularByteTransform>
    {

        #region Constructor

        /// <summary>
        /// 实例化三菱的串口协议的通讯对象
        /// </summary>
        public MelsecFxSerial( )
        {
            WordLength = 1;
        }


        #endregion

        #region Check Response

        private OperateResult CheckPlcReadResponse( byte[] ack )
        {
            if (ack.Length == 0) return new OperateResult( StringResources.Language.MelsecFxReceiveZore );
            if (ack[0] == 0x15) return new OperateResult( StringResources.Language.MelsecFxAckNagative );
            if (ack[0] != 0x02) return new OperateResult( StringResources.Language.MelsecFxAckWrong + ack[0] );

            if (!MelsecHelper.CheckCRC( ack )) return new OperateResult( StringResources.Language.MelsecFxCrcCheckFailed );

            return OperateResult.CreateSuccessResult( );
        }

        private OperateResult CheckPlcWriteResponse( byte[] ack )
        {
            if (ack.Length == 0) return new OperateResult( StringResources.Language.MelsecFxReceiveZore );
            if (ack[0] == 0x15) return new OperateResult( StringResources.Language.MelsecFxAckNagative );
            if (ack[0] != 0x06) return new OperateResult( StringResources.Language.MelsecFxAckWrong + ack[0] );

            return OperateResult.CreateSuccessResult( );
        }
        
        #endregion

        #region Read Support

        /// <summary>
        /// 从三菱PLC中读取想要的数据，返回读取结果
        /// </summary>
        /// <param name="address">读取地址，格式为"M100","D100","W1A0"</param>
        /// <param name="length">读取的数据长度</param>
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
        ///     <term>数据寄存器</term>
        ///     <term>D100,D200</term>
        ///     <term>D0-D511,D8000-D8255</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>定时器的值</term>
        ///     <term>T10,T20</term>
        ///     <term>T0-T255</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>计数器的值</term>
        ///     <term>C10,C20</term>
        ///     <term>C0-C199,C200-C255</term>
        ///     <term>10</term>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <example>
        /// 假设起始地址为D100，D100存储了温度，100.6℃值为1006，D101存储了压力，1.23Mpa值为123，D102，D103存储了产量计数，读取如下：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecFxSerial.cs" region="ReadExample2" title="Read示例" />
        /// 以下是读取不同类型数据的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecFxSerial.cs" region="ReadExample1" title="Read示例" />
        /// </example>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            // 获取指令
            OperateResult<byte[]> command = BuildReadWordCommand( address, length );
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( command );

            // 核心交互
            OperateResult<byte[]> read = ReadBase( command.Content );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( read );

            // 反馈检查
            OperateResult ackResult = CheckPlcReadResponse( read.Content );
            if (!ackResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( ackResult );

            // 数据提炼
            return ExtractActualData( read.Content );
        }



        /// <summary>
        /// 从三菱PLC中批量读取位软元件，返回读取结果，该读取地址最好从0，16，32...等开始读取，这样可以读取比较长得数据数组
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
        ///  <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecFxSerial.cs" region="ReadBool" title="Bool类型示例" />
        /// </example>
        public OperateResult<bool[]> ReadBool( string address, ushort length )
        {
            //获取指令
            OperateResult<byte[], int> command = BuildReadBoolCommand( address, length );
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( command );

            // 核心交互
            OperateResult<byte[]> read = ReadBase( command.Content1 );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( read );

            // 反馈检查
            OperateResult ackResult = CheckPlcReadResponse( read.Content );
            if (!ackResult.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( ackResult );

            // 提取真实的数据
            return ExtractActualBoolData( read.Content, command.Content2, length );
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

        #region Write Override

        /// <summary>
        /// 向PLC写入数据，数据格式为原始的字节类型
        /// </summary>
        /// <param name="address">初始地址</param>
        /// <param name="value">原始的字节数据</param>
        /// <example>
        /// 假设起始地址为D100，D100存储了温度，100.6℃值为1006，D101存储了压力，1.23Mpa值为123，D102，D103存储了产量计数，写入如下：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecFxSerial.cs" region="WriteExample2" title="Write示例" />
        /// 以下是读取不同类型数据的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecFxSerial.cs" region="WriteExample1" title="Write示例" />
        /// </example>
        /// <returns>是否写入成功的结果对象</returns>
        public override OperateResult Write( string address, byte[] value )
        {
            // 获取写入
            OperateResult<byte[]> command = BuildWriteWordCommand( address, value );
            if (!command.IsSuccess) return command;

            // 核心交互
            OperateResult<byte[]> read = ReadBase( command.Content );
            if (!read.IsSuccess) return read;

            // 结果验证
            OperateResult checkResult = CheckPlcWriteResponse( read.Content );
            if (!checkResult.IsSuccess) return checkResult;

            return OperateResult.CreateSuccessResult( );
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
            // 先获取指令
            OperateResult<byte[]> command = BuildWriteBoolPacket( address, value );
            if (!command.IsSuccess) return command;

            // 和串口进行核心的数据交互
            OperateResult<byte[]> read = ReadBase( command.Content );
            if (!read.IsSuccess) return read;
                
            // 检查结果是否正确
            OperateResult checkResult = CheckPlcWriteResponse( read.Content );
            if (!checkResult.IsSuccess) return checkResult;

            return OperateResult.CreateSuccessResult( );
        }

        #endregion

        #region Object Override

        /// <summary>
        /// 获取当前对象的字符串标识形式
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString( )
        {
            return "MelsecFxSerial";
        }

        #endregion

        #region Static Method Helper

        
        /// <summary>
        /// 生成位写入的数据报文信息，该报文可直接用于发送串口给PLC
        /// </summary>
        /// <param name="address">地址信息，每个地址存在一定的范围，需要谨慎传入数据。举例：M10,S10,X5,Y10,C10,T10</param>
        /// <param name="value"><c>True</c>或是<c>False</c></param>
        /// <returns>带报文信息的结果对象</returns>
        public static OperateResult<byte[]> BuildWriteBoolPacket( string address, bool value )
        {
            var analysis = FxAnalysisAddress( address );
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
                return new OperateResult<byte[]>( StringResources.Language.MelsecCurrentTypeNotSupportedBitOperate );
            }


            byte[] _PLCCommand = new byte[9];
            _PLCCommand[0] = 0x02;                                                       // STX
            _PLCCommand[1] = value ? (byte)0x37 : (byte)0x38;                            // Read
            _PLCCommand[2] = MelsecHelper.BuildBytesFromData( startAddress )[2];         // 偏移地址
            _PLCCommand[3] = MelsecHelper.BuildBytesFromData( startAddress )[3];
            _PLCCommand[4] = MelsecHelper.BuildBytesFromData( startAddress )[0];
            _PLCCommand[5] = MelsecHelper.BuildBytesFromData( startAddress )[1];
            _PLCCommand[6] = 0x03;                                                       // ETX
            MelsecHelper.FxCalculateCRC( _PLCCommand ).CopyTo( _PLCCommand, 7 );         // CRC

            return OperateResult.CreateSuccessResult( _PLCCommand );
        }

        /// <summary>
        /// 根据类型地址长度确认需要读取的指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">长度</param>
        /// <returns>带有成功标志的指令数据</returns>
        public static OperateResult<byte[]> BuildReadWordCommand( string address, ushort length )
        {
            var addressResult = FxCalculateWordStartAddress( address );
            if (!addressResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( addressResult );

            length = (ushort)(length * 2);
            ushort startAddress = addressResult.Content;

            byte[] _PLCCommand = new byte[11];
            _PLCCommand[0] = 0x02;                                                    // STX
            _PLCCommand[1] = 0x30;                                                    // Read
            _PLCCommand[2] = MelsecHelper.BuildBytesFromData( startAddress )[0];      // 偏移地址
            _PLCCommand[3] = MelsecHelper.BuildBytesFromData( startAddress )[1];
            _PLCCommand[4] = MelsecHelper.BuildBytesFromData( startAddress )[2];
            _PLCCommand[5] = MelsecHelper.BuildBytesFromData( startAddress )[3];
            _PLCCommand[6] = MelsecHelper.BuildBytesFromData( (byte)length )[0];      // 读取长度
            _PLCCommand[7] = MelsecHelper.BuildBytesFromData( (byte)length )[1];
            _PLCCommand[8] = 0x03;                                                    // ETX
            MelsecHelper.FxCalculateCRC( _PLCCommand ).CopyTo( _PLCCommand, 9 );      // CRC

            return OperateResult.CreateSuccessResult( _PLCCommand );                  // Return
        }

        /// <summary>
        /// 根据类型地址长度确认需要读取的指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">bool数组长度</param>
        /// <returns>带有成功标志的指令数据</returns>
        public static OperateResult<byte[], int> BuildReadBoolCommand( string address, ushort length )
        {
            var addressResult = FxCalculateBoolStartAddress( address );
            if (!addressResult.IsSuccess) return OperateResult.CreateFailedResult<byte[], int>( addressResult );

            // 计算下实际需要读取的数据长度
            ushort length2 = (ushort)((addressResult.Content2 + length - 1) / 8 - (addressResult.Content2 / 8) + 1);

            ushort startAddress = addressResult.Content1;
            byte[] _PLCCommand = new byte[11];
            _PLCCommand[0] = 0x02;                                                    // STX
            _PLCCommand[1] = 0x30;                                                    // Read
            _PLCCommand[2] = MelsecHelper.BuildBytesFromData( startAddress )[0];      // 偏移地址
            _PLCCommand[3] = MelsecHelper.BuildBytesFromData( startAddress )[1];
            _PLCCommand[4] = MelsecHelper.BuildBytesFromData( startAddress )[2];
            _PLCCommand[5] = MelsecHelper.BuildBytesFromData( startAddress )[3];
            _PLCCommand[6] = MelsecHelper.BuildBytesFromData( (byte)length2 )[0];     // 读取长度
            _PLCCommand[7] = MelsecHelper.BuildBytesFromData( (byte)length2 )[1];
            _PLCCommand[8] = 0x03;                                                    // ETX
            MelsecHelper.FxCalculateCRC( _PLCCommand ).CopyTo( _PLCCommand, 9 );      // CRC

            return OperateResult.CreateSuccessResult( _PLCCommand, (int)addressResult.Content3 );
        }

        /// <summary>
        /// 根据类型地址以及需要写入的数据来生成指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">实际的数据信息</param>
        /// <returns>带有成功标志的指令数据</returns>
        public static OperateResult<byte[]> BuildWriteWordCommand( string address, byte[] value )
        {
            var addressResult = FxCalculateWordStartAddress( address );
            if (!addressResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( addressResult );

            // 字节数据转换成ASCII格式
            if (value != null) value = MelsecHelper.BuildBytesFromData( value );

            ushort startAddress = addressResult.Content;
            byte[] _PLCCommand = new byte[11 + value.Length];
            _PLCCommand[0] = 0x02;                                                                    // STX
            _PLCCommand[1] = 0x31;                                                                    // Read
            _PLCCommand[2] = MelsecHelper.BuildBytesFromData( startAddress )[0];                      // Offect Address
            _PLCCommand[3] = MelsecHelper.BuildBytesFromData( startAddress )[1];
            _PLCCommand[4] = MelsecHelper.BuildBytesFromData( startAddress )[2];
            _PLCCommand[5] = MelsecHelper.BuildBytesFromData( startAddress )[3];
            _PLCCommand[6] = MelsecHelper.BuildBytesFromData( (byte)(value.Length / 2) )[0];          // Read Length
            _PLCCommand[7] = MelsecHelper.BuildBytesFromData( (byte)(value.Length / 2) )[1];
            Array.Copy( value, 0, _PLCCommand, 8, value.Length );
            _PLCCommand[_PLCCommand.Length - 3] = 0x03;                                               // ETX
            MelsecHelper.FxCalculateCRC( _PLCCommand ).CopyTo( _PLCCommand, _PLCCommand.Length - 2 ); // CRC

            return OperateResult.CreateSuccessResult( _PLCCommand );
        }


        /// <summary>
        /// 从PLC反馈的数据进行提炼操作
        /// </summary>
        /// <param name="response">PLC反馈的真实数据</param>
        /// <returns>数据提炼后的真实数据</returns>
        public static OperateResult<byte[]> ExtractActualData( byte[] response )
        {
            try
            {
                byte[] data = new byte[(response.Length - 4) / 2];
                for (int i = 0; i < data.Length; i++)
                {
                    byte[] buffer = new byte[2];
                    buffer[0] = response[i * 2 + 1];
                    buffer[1] = response[i * 2 + 2];

                    data[i] = Convert.ToByte( Encoding.ASCII.GetString( buffer ), 16 );
                }

                return OperateResult.CreateSuccessResult( data );
            }
            catch (Exception ex)
            {
                return new OperateResult<byte[]>( )
                {
                    Message = "Extract Msg：" + ex.Message + Environment.NewLine +
                    "Data: " + BasicFramework.SoftBasic.ByteToHexString( response )
                };
            }
        }


        /// <summary>
        /// 从PLC反馈的数据进行提炼bool数组操作
        /// </summary>
        /// <param name="response">PLC反馈的真实数据</param>
        /// <param name="start">起始提取的点信息</param>
        /// <param name="length">bool数组的长度</param>
        /// <returns>数据提炼后的真实数据</returns>
        public static OperateResult<bool[]> ExtractActualBoolData( byte[] response, int start, int length )
        {
            OperateResult<byte[]> extraResult = ExtractActualData( response );
            if (!extraResult.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( extraResult );

            // 转化bool数组
            try
            {
                bool[] data = new bool[length];
                bool[] array = BasicFramework.SoftBasic.ByteToBoolArray( extraResult.Content, extraResult.Content.Length * 8 );
                for (int i = 0; i < length; i++)
                {
                    data[i] = array[i + start];
                }

                return OperateResult.CreateSuccessResult( data );
            }
            catch (Exception ex)
            {
                return new OperateResult<bool[]>( )
                {
                    Message = "Extract Msg：" + ex.Message + Environment.NewLine +
                    "Data: " + BasicFramework.SoftBasic.ByteToHexString( response )
                };
            }
        }

        /// <summary>
        /// 解析数据地址成不同的三菱地址类型
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns>地址结果对象</returns>
        private static OperateResult<MelsecMcDataType, ushort> FxAnalysisAddress( string address )
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
                    default: throw new Exception( StringResources.Language.NotSupportedDataType );
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
        /// 返回读取的地址及长度信息
        /// </summary>
        /// <param name="address">读取的地址信息</param>
        /// <returns>带起始地址的结果对象</returns>
        private static OperateResult<ushort> FxCalculateWordStartAddress( string address )
        {
            // 初步解析，失败就返回
            var analysis = FxAnalysisAddress( address );
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
                return new OperateResult<ushort>( StringResources.Language.MelsecCurrentTypeNotSupportedWordOperate );
            }

            return OperateResult.CreateSuccessResult( startAddress );
        }

        /// <summary>
        /// 返回读取的地址及长度信息，以及当前的偏置信息
        /// </summary><param name="address">读取的地址信息</param>
        /// <returns>带起始地址的结果对象</returns>
        private static OperateResult<ushort, ushort, ushort> FxCalculateBoolStartAddress( string address )
        {
            // 初步解析
            var analysis = FxAnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<ushort, ushort, ushort>( analysis );

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
                return new OperateResult<ushort, ushort, ushort>( StringResources.Language.MelsecCurrentTypeNotSupportedBitOperate );
            }

            return OperateResult.CreateSuccessResult( startAddress, analysis.Content2, ( ushort)(analysis.Content2 % 8) );
        }
        
        #endregion
    }
}
