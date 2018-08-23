using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;

namespace HslCommunication.Profinet.Melsec
{

    /// <summary>
    /// 三菱PLC通讯类，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为二进制通讯
    /// </summary>
    /// <remarks>
    /// 目前组件测试通过的PLC型号列表，有些来自于网友的测试
    /// <list type="number">
    /// <item>Q06UDV PLC  感谢hwdq0012</item>
    /// <item>fx5u PLC  感谢山楂</item>
    /// <item>Q02CPU PLC </item>
    /// <item>L02CPU PLC </item>
    /// </list>
    /// </remarks>
    /// <example>
    ///   <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="Usage" title="简单的短连接使用" />
    ///   <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="Usage2" title="简单的长连接使用" />
    /// </example>
    public class MelsecMcNet : NetworkDeviceBase<MelsecQnA3EBinaryMessage, RegularByteTransform>
    {

        #region Constructor

        /// <summary>
        /// 实例化三菱的Qna兼容3E帧协议的通讯对象
        /// </summary>
        public MelsecMcNet()
        {
            WordLength = 1;
        }

        /// <summary>
        /// 实例化一个三菱的Qna兼容3E帧协议的通讯对象
        /// </summary>
        /// <param name="ipAddress">PLCd的Ip地址</param>
        /// <param name="port">PLC的端口</param>
        public MelsecMcNet( string ipAddress, int port )
        {
            WordLength = 1;
            IpAddress = ipAddress;
            Port = port;
        }

        #endregion

        #region Public Member

        /// <summary>
        /// 网络号
        /// </summary>
        /// <remarks>
        /// 依据PLC的配置而配置，如果PLC配置了1，那么此处也填0，如果PLC配置了2，此处就填2，测试不通的话，继续测试0
        /// </remarks>
        public byte NetworkNumber { get; set; } = 0x00;

        /// <summary>
        /// 网络站号
        /// </summary>
        /// <remarks>
        /// 依据PLC的配置而配置，如果PLC配置了1，那么此处也填0，如果PLC配置了2，此处就填2，测试不通的话，继续测试0
        /// </remarks>
        public byte NetworkStationNumber { get; set; } = 0x00;


        #endregion

        #region Address Analysis

        /// <summary>
        /// 解析数据地址
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns>解析值</returns>
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
            result.Message = StringResources.SuccessText;
            return result;
        }

        #endregion

        #region Build Command

        /// <summary>
        /// 根据类型地址长度确认需要读取的指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">长度</param>
        /// <returns>带有成功标志的指令数据</returns>
        private OperateResult<MelsecMcDataType, byte[]> BuildReadCommand( string address, ushort length )
        {
            var analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<MelsecMcDataType, byte[]>( analysis );

            // 默认信息----注意：高低字节交错
            byte[] _PLCCommand = new byte[21];
            _PLCCommand[0]  = 0x50;                         // 副标题
            _PLCCommand[1]  = 0x00;
            _PLCCommand[2]  = NetworkNumber;                // 网络号
            _PLCCommand[3]  = 0xFF;                         // PLC编号
            _PLCCommand[4]  = 0xFF;                         // 目标模块IO编号
            _PLCCommand[5]  = 0x03;
            _PLCCommand[6]  = NetworkStationNumber;         // 目标模块站号
            _PLCCommand[7]  = 0x0C;                         // 请求数据长度
            _PLCCommand[8]  = 0x00;
            _PLCCommand[9]  = 0x0A;                         // CPU监视定时器
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x01;                        // 批量读取数据命令
            _PLCCommand[12] = 0x04;
            _PLCCommand[13] = analysis.Content1.DataType;            // 以点为单位还是字为单位成批读取
            _PLCCommand[14] = 0x00;
            _PLCCommand[15] = (byte)(analysis.Content2 % 256);       // 起始地址的地位
            _PLCCommand[16] = (byte)(analysis.Content2 / 256);
            _PLCCommand[17] = 0x00;
            _PLCCommand[18] = analysis.Content1.DataCode;            // 指明读取的数据
            _PLCCommand[19] = (byte)(length % 256);                  // 软元件长度的地位
            _PLCCommand[20] = (byte)(length / 256);

            return OperateResult.CreateSuccessResult( analysis.Content1, _PLCCommand );
        }

        /// <summary>
        /// 根据类型地址以及需要写入的数据来生成指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value"></param>
        /// <param name="length">指定长度</param>
        /// <returns>解析后的指令</returns>
        private OperateResult<MelsecMcDataType, byte[]> BuildWriteCommand( string address, byte[] value, int length = -1 )
        {
            var analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<MelsecMcDataType, byte[]>( analysis );

            // 默认信息----注意：高低字节交错

            byte[] _PLCCommand = new byte[21 + value.Length];

            _PLCCommand[0]  = 0x50;                                          // 副标题
            _PLCCommand[1]  = 0x00;
            _PLCCommand[2]  = NetworkNumber;                                 // 网络号
            _PLCCommand[3]  = 0xFF;                                          // PLC编号
            _PLCCommand[4]  = 0xFF;                                          // 目标模块IO编号
            _PLCCommand[5]  = 0x03;
            _PLCCommand[6]  = NetworkStationNumber;                          // 目标模块站号
            _PLCCommand[7]  = (byte)((_PLCCommand.Length - 9) % 256);        // 请求数据长度
            _PLCCommand[8]  = (byte)((_PLCCommand.Length - 9) / 256); ;
            _PLCCommand[9]  = 0x0A;                                          // CPU监视定时器
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x01;                                         // 批量读取数据命令
            _PLCCommand[12] = 0x14;
            _PLCCommand[13] = analysis.Content1.DataType;                   // 以点为单位还是字为单位成批读取
            _PLCCommand[14] = 0x00;
            _PLCCommand[15] = (byte)(analysis.Content2 % 256); ;            // 起始地址的地位
            _PLCCommand[16] = (byte)(analysis.Content2 / 256);
            _PLCCommand[17] = 0x00;
            _PLCCommand[18] = analysis.Content1.DataCode;                   // 指明写入的数据

            // 判断是否进行位操作
            if (analysis.Content1.DataType == 1)
            {
                if (length > 0)
                {
                    _PLCCommand[19] = (byte)(length % 256);                 // 软元件长度的地位
                    _PLCCommand[20] = (byte)(length / 256);
                }
                else
                {
                    _PLCCommand[19] = (byte)(value.Length * 2 % 256);        // 软元件长度的地位
                    _PLCCommand[20] = (byte)(value.Length * 2 / 256);
                }
            }
            else
            {
                _PLCCommand[19] = (byte)(value.Length / 2 % 256);            // 软元件长度的地位
                _PLCCommand[20] = (byte)(value.Length / 2 / 256);
            }
            Array.Copy( value, 0, _PLCCommand, 21, value.Length );

            return OperateResult.CreateSuccessResult( analysis.Content1, _PLCCommand );
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
        ///     <term>链接寄存器</term>
        ///     <term>W100,W1A0</term>
        ///     <term>16</term>
        ///   </item>
        ///   <item>
        ///     <term>文件寄存器</term>
        ///     <term>R100,R200</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>变址寄存器</term>
        ///     <term>Z100,Z200</term>
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
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="ReadExample2" title="Read示例" />
        /// 以下是读取不同类型数据的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="ReadExample1" title="Read示例" />
        /// </example>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            //获取指令
            var command = BuildReadCommand( address, length );
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( command );

            // 核心交互
            var read = ReadFromCoreServer( command.Content2 );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( read );

            // 错误代码验证
            ushort ErrorCode = BitConverter.ToUInt16( read.Content, 9 );
            if (ErrorCode != 0) return new OperateResult<byte[]>( ) { ErrorCode = ErrorCode, Message = "请翻查三菱通讯手册来查看具体的信息。" };
            
            // 数据解析
            if (command.Content1.DataType == 0x01)
            {
                // 位读取
                byte[] Content = new byte[(read.Content.Length - 11) * 2];
                for (int i = 11; i < read.Content.Length; i++)
                {
                    if ((read.Content[i] & 0x10) == 0x10)
                    {
                        Content[(i - 11) * 2 + 0] = 0x01;
                    }

                    if ((read.Content[i] & 0x01) == 0x01)
                    {
                        Content[(i - 11) * 2 + 1] = 0x01;
                    }
                }

                return OperateResult.CreateSuccessResult( Content );
            }
            else
            {
                // 字读取
                byte[] Content = new byte[read.Content.Length - 11];
                Array.Copy( read.Content, 11, Content, 0, Content.Length );

                return OperateResult.CreateSuccessResult( Content );
            }
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
        ///     <term>地址进制</term>
        ///   </listheader>
        ///   <item>
        ///     <term>内部继电器</term>
        ///     <term>M100,M200</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>输入继电器</term>
        ///     <term>X100,X1A0</term>
        ///     <term>16</term>
        ///   </item>
        ///   <item>
        ///     <term>输出继电器</term>
        ///     <term>Y100,Y1A0</term>
        ///     <term>16</term>
        ///   </item>
        ///    <item>
        ///     <term>锁存继电器</term>
        ///     <term>L100,L200</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>报警器</term>
        ///     <term>F100,F200</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>边沿继电器</term>
        ///     <term>V100,V200</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>链接继电器</term>
        ///     <term>B100,B1A0</term>
        ///     <term>16</term>
        ///   </item>
        ///    <item>
        ///     <term>步进继电器</term>
        ///     <term>S100,S200</term>
        ///     <term>10</term>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <example>
        ///  <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="ReadBool" title="Bool类型示例" />
        /// </example>
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
                    result.Message = "读取位变量数组只能针对位软元件，如果读取字软元件，请调用Read方法";
                    return result;
                }
            }
            var read = Read( address, length );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( read );

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
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="WriteExample2" title="Write示例" />
        /// 以下是写入不同类型数据的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="WriteExample1" title="Write示例" />
        /// </example>
        /// <returns>结果</returns>
        public override OperateResult Write( string address, byte[] value )
        {
            OperateResult<byte[]> result = new OperateResult<byte[]>( );

            //获取指令
            var analysis = AnalysisAddress( address );
            if(!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }

            OperateResult<MelsecMcDataType,byte[]> command;
            // 预处理指令
            if (analysis.Content1.DataType == 0x01)
            {
                int length = value.Length % 2 == 0 ? value.Length / 2 : value.Length / 2 + 1;
                byte[] buffer = new byte[length];

                for (int i = 0; i < length; i++)
                {
                    if (value[i * 2 + 0] != 0x00) buffer[i] += 0x10;
                    if ((i * 2 + 1) < value.Length)
                    {
                        if (value[i * 2 + 1] != 0x00) buffer[i] += 0x01;
                    }
                }

                // 位写入
                command = BuildWriteCommand(address, buffer, value.Length );
            }
            else
            {
                // 字写入
                command = BuildWriteCommand( address, value );
            }

            if(!command.IsSuccess)
            {
                result.CopyErrorFromOther( command );
                return result;
            }

            OperateResult<byte[]> read = ReadFromCoreServer( command.Content2 );
            if (read.IsSuccess)
            {
                result.ErrorCode = BitConverter.ToUInt16( read.Content, 9 );
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
        /// 向PLC中字软元件写入指定长度的字符串,超出截断，不够补0，编码格式为ASCII
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>返回读取结果</returns>
        public OperateResult Write( string address, string value, int length )
        {
            byte[] temp = ByteTransform.TransByte( value, Encoding.ASCII );
            temp = SoftBasic.ArrayExpandToLength( temp, length );
            temp = SoftBasic.ArrayExpandToLengthEven( temp );
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
            temp = SoftBasic.ArrayExpandToLength( temp, length * 2 );
            return Write( address, temp );
        }

        #endregion

        #region Write bool[]


        /// <summary>
        /// 向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据，长度为8的倍数</param>
        /// <example>
        /// 详细请查看<see cref="Write(string, bool[])"/>方法的示例
        /// </example>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, bool value )
        {
            return Write( address, new bool[] { value} );
        }

        /// <summary>
        /// 向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据，可以指定任意的长度</param>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="WriteBool" title="Write示例" />
        /// </example>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, bool[] values )
        {
            return Write( address, values.Select( m => m ? (byte)0x01 : (byte)0x00 ).ToArray( ) );
        }


        #endregion
        
        #region Object Override

        /// <summary>
        /// 获取当前对象的字符串标识形式
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString()
        {
            return "MelsecMcNet";
        }

        #endregion


    }
}
