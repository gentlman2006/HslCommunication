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
    /// <example>
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="Usage" title="简单的短连接使用" />
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="Usage2" title="简单的长连接使用" />
    /// </example>
    public class MelsecMcAsciiNet : NetworkDeviceBase<MelsecQnA3EAsciiMessage, RegularByteTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化三菱的Qna兼容3E帧协议的通讯对象
        /// </summary>
        public MelsecMcAsciiNet( )
        {
            WordLength = 1;
        }

        /// <summary>
        /// 实例化一个三菱的Qna兼容3E帧协议的通讯对象
        /// </summary>
        /// <param name="ipAddress">PLC的Ip地址</param>
        /// <param name="port">PLC的端口</param>
        public MelsecMcAsciiNet( string ipAddress, int port )
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
        public byte NetworkNumber { get; set; } = 0x00;

        /// <summary>
        /// 网络站号
        /// </summary>
        public byte NetworkStationNumber { get; set; } = 0x00;


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
        ///     <term>ZR文件寄存器</term>
        ///     <term>ZR100,ZR2A0</term>
        ///     <term>16</term>
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
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="ReadExample2" title="Read示例" />
        /// 以下是读取不同类型数据的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="ReadExample1" title="Read示例" />
        /// </example>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            // 获取指令
            var command = BuildReadCommand( address, length, NetworkNumber, NetworkStationNumber );
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( command );

            // 核心交互
            var read = ReadFromCoreServer( command.Content );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( read );

            // 错误代码验证
            ushort errorCode = Convert.ToUInt16( Encoding.ASCII.GetString( read.Content, 18, 4 ), 16 );
            if (errorCode != 0) return new OperateResult<byte[]>( errorCode, StringResources.Language.MelsecPleaseReferToManulDocument );

            // 数据解析，需要传入是否使用位的参数
            return ExtractActualData( read.Content, command.Content[29] == 0x31 );
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
        ///  <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="ReadBool" title="Bool类型示例" />
        /// </example>
        public OperateResult<bool[]> ReadBool( string address, ushort length )
        {
            // 解析地址
            OperateResult<MelsecMcDataType, int> analysis = MelsecHelper.McAnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( analysis );

            // 位读取校验
            if (analysis.Content1.DataType == 0x00) return new OperateResult<bool[]>( StringResources.Language.MelsecReadBitInfo );
            
            // 核心交互
            var read = Read( address, length );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( read );

            // 转化bool数组
            return OperateResult.CreateSuccessResult( read.Content.Select( m => m == 0x01 ).Take( length ).ToArray( ) );
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
            // 解析指令
            OperateResult<byte[]> command = BuildWriteCommand( address, value, NetworkNumber, NetworkStationNumber );
            if (!command.IsSuccess) return command;

            // 核心交互
            OperateResult<byte[]> read = ReadFromCoreServer( command.Content );
            if (!read.IsSuccess) return read;

            // 错误码验证
            ushort errorCode = Convert.ToUInt16( Encoding.ASCII.GetString( read.Content, 18, 4 ), 16 );
            if (errorCode != 0) return new OperateResult<byte[]>( errorCode, StringResources.Language.MelsecPleaseReferToManulDocument );

            // 写入成功
            return OperateResult.CreateSuccessResult( );
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
            return Write( address, new bool[] { value } );
        }

        /// <summary>
        /// 向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据，可以指定任意的长度</param>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="WriteBool" title="Write示例" />
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
        public override string ToString( )
        {
            return $"MelsecMcAsciiNet[{IpAddress}:{Port}]";
        }

        #endregion

        #region Static Method Helper


        /// <summary>
        /// 根据类型地址长度确认需要读取的报文
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">长度</param>
        /// <param name="networkNumber">网络号信息</param>
        /// <param name="networkStationNumber">网络站号信息</param>
        /// <returns>带有成功标志的指令数据</returns>
        public static OperateResult<byte[]> BuildReadCommand( string address, ushort length, byte networkNumber = 0, byte networkStationNumber = 0 )
        {
            OperateResult<MelsecMcDataType, int> analysis = MelsecHelper.McAnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );


            // 默认信息----注意：高低字节交错
            byte[] _PLCCommand = new byte[42];
            _PLCCommand[ 0] = 0x35;                                                               // 副标题
            _PLCCommand[ 1] = 0x30;
            _PLCCommand[ 2] = 0x30;
            _PLCCommand[ 3] = 0x30;
            _PLCCommand[ 4] = MelsecHelper.BuildBytesFromData( networkNumber )[0];                // 网络号
            _PLCCommand[ 5] = MelsecHelper.BuildBytesFromData( networkNumber )[1];
            _PLCCommand[ 6] = 0x46;                                                               // PLC编号
            _PLCCommand[ 7] = 0x46;
            _PLCCommand[ 8] = 0x30;                                                               // 目标模块IO编号
            _PLCCommand[ 9] = 0x33;
            _PLCCommand[10] = 0x46;
            _PLCCommand[11] = 0x46;
            _PLCCommand[12] = MelsecHelper.BuildBytesFromData( networkStationNumber )[0];         // 目标模块站号
            _PLCCommand[13] = MelsecHelper.BuildBytesFromData( networkStationNumber )[1];
            _PLCCommand[14] = 0x30;                                                               // 请求数据长度
            _PLCCommand[15] = 0x30;
            _PLCCommand[16] = 0x31;
            _PLCCommand[17] = 0x38;
            _PLCCommand[18] = 0x30;                                                               // CPU监视定时器
            _PLCCommand[19] = 0x30;
            _PLCCommand[20] = 0x31;
            _PLCCommand[21] = 0x30;
            _PLCCommand[22] = 0x30;                                                               // 批量读取数据命令
            _PLCCommand[23] = 0x34;
            _PLCCommand[24] = 0x30;
            _PLCCommand[25] = 0x31;
            _PLCCommand[26] = 0x30;                                                               // 以点为单位还是字为单位成批读取
            _PLCCommand[27] = 0x30;
            _PLCCommand[28] = 0x30;
            _PLCCommand[29] = analysis.Content1.DataType == 0 ? (byte)0x30 : (byte)0x31;
            _PLCCommand[30] = Encoding.ASCII.GetBytes( analysis.Content1.AsciiCode )[0];          // 软元件类型
            _PLCCommand[31] = Encoding.ASCII.GetBytes( analysis.Content1.AsciiCode )[1];
            _PLCCommand[32] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[0];            // 起始地址的地位
            _PLCCommand[33] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[1];
            _PLCCommand[34] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[2];
            _PLCCommand[35] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[3];
            _PLCCommand[36] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[4];
            _PLCCommand[37] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[5];
            _PLCCommand[38] = MelsecHelper.BuildBytesFromData( length )[0];                                             // 软元件点数
            _PLCCommand[39] = MelsecHelper.BuildBytesFromData( length )[1];
            _PLCCommand[40] = MelsecHelper.BuildBytesFromData( length )[2];
            _PLCCommand[41] = MelsecHelper.BuildBytesFromData( length )[3];

            return OperateResult.CreateSuccessResult( _PLCCommand );
        }

        /// <summary>
        /// 根据类型地址以及需要写入的数据来生成报文
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入数据的实际值</param>
        /// <param name="networkNumber">网络号</param>
        /// <param name="networkStationNumber">网络站号</param>
        /// <returns>带有成功标志的指令数据</returns>
        public static OperateResult<byte[]> BuildWriteCommand( string address, byte[] value, byte networkNumber = 0, byte networkStationNumber = 0 )
        {
            OperateResult<MelsecMcDataType, int> analysis = MelsecHelper.McAnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );


            // 预处理指令
            if (analysis.Content1.DataType == 0x01)
            {
                // 位写入
                value = value.Select( m => m == 0x00 ? (byte)0x30 : (byte)0x31 ).ToArray( );
            }
            else
            {
                // 字写入
                byte[] buffer = new byte[value.Length * 2];
                for (int i = 0; i < value.Length / 2; i++)
                {
                    MelsecHelper.BuildBytesFromData( BitConverter.ToUInt16( value, i * 2 ) ).CopyTo( buffer, 4 * i );
                }
                value = buffer;
            }


            // 默认信息----注意：高低字节交错

            byte[] _PLCCommand = new byte[42 + value.Length];

            _PLCCommand[ 0] = 0x35;                                                                              // 副标题
            _PLCCommand[ 1] = 0x30;
            _PLCCommand[ 2] = 0x30;
            _PLCCommand[ 3] = 0x30;
            _PLCCommand[ 4] = MelsecHelper.BuildBytesFromData( networkNumber )[0];                               // 网络号
            _PLCCommand[ 5] = MelsecHelper.BuildBytesFromData( networkNumber )[1];
            _PLCCommand[ 6] = 0x46;                                                                              // PLC编号
            _PLCCommand[ 7] = 0x46;
            _PLCCommand[ 8] = 0x30;                                                                              // 目标模块IO编号
            _PLCCommand[ 9] = 0x33;
            _PLCCommand[10] = 0x46;
            _PLCCommand[11] = 0x46;
            _PLCCommand[12] = MelsecHelper.BuildBytesFromData( networkStationNumber )[0];                        // 目标模块站号
            _PLCCommand[13] = MelsecHelper.BuildBytesFromData( networkStationNumber )[1];
            _PLCCommand[14] = MelsecHelper.BuildBytesFromData( (ushort)(_PLCCommand.Length - 18) )[0];           // 请求数据长度
            _PLCCommand[15] = MelsecHelper.BuildBytesFromData( (ushort)(_PLCCommand.Length - 18) )[1];
            _PLCCommand[16] = MelsecHelper.BuildBytesFromData( (ushort)(_PLCCommand.Length - 18) )[2];
            _PLCCommand[17] = MelsecHelper.BuildBytesFromData( (ushort)(_PLCCommand.Length - 18) )[3];
            _PLCCommand[18] = 0x30;                                                                              // CPU监视定时器
            _PLCCommand[19] = 0x30;
            _PLCCommand[20] = 0x31;
            _PLCCommand[21] = 0x30;
            _PLCCommand[22] = 0x31;                                                                              // 批量写入的命令
            _PLCCommand[23] = 0x34;
            _PLCCommand[24] = 0x30;
            _PLCCommand[25] = 0x31;
            _PLCCommand[26] = 0x30;                                                                              // 子命令
            _PLCCommand[27] = 0x30;
            _PLCCommand[28] = 0x30;
            _PLCCommand[29] = analysis.Content1.DataType == 0 ? (byte)0x30 : (byte)0x31;
            _PLCCommand[30] = Encoding.ASCII.GetBytes( analysis.Content1.AsciiCode )[0];                         // 软元件类型
            _PLCCommand[31] = Encoding.ASCII.GetBytes( analysis.Content1.AsciiCode )[1];
            _PLCCommand[32] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[0];     // 起始地址的地位
            _PLCCommand[33] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[1];
            _PLCCommand[34] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[2];
            _PLCCommand[35] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[3];
            _PLCCommand[36] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[4];
            _PLCCommand[37] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[5];

            // 判断是否进行位操作
            if (analysis.Content1.DataType == 1)
            {
                _PLCCommand[38] = MelsecHelper.BuildBytesFromData( (ushort)value.Length )[0];                    // 软元件点数
                _PLCCommand[39] = MelsecHelper.BuildBytesFromData( (ushort)value.Length )[1];
                _PLCCommand[40] = MelsecHelper.BuildBytesFromData( (ushort)value.Length )[2];
                _PLCCommand[41] = MelsecHelper.BuildBytesFromData( (ushort)value.Length )[3];
            }
            else
            {
                _PLCCommand[38] = MelsecHelper.BuildBytesFromData( (ushort)(value.Length / 4) )[0];              // 软元件点数
                _PLCCommand[39] = MelsecHelper.BuildBytesFromData( (ushort)(value.Length / 4) )[1];
                _PLCCommand[40] = MelsecHelper.BuildBytesFromData( (ushort)(value.Length / 4) )[2];
                _PLCCommand[41] = MelsecHelper.BuildBytesFromData( (ushort)(value.Length / 4) )[3];
            }
            Array.Copy( value, 0, _PLCCommand, 42, value.Length );

            return OperateResult.CreateSuccessResult( _PLCCommand );
        }

        /// <summary>
        /// 从PLC反馈的数据中提取出实际的数据内容，需要传入反馈数据，是否位读取
        /// </summary>
        /// <param name="response">反馈的数据内容</param>
        /// <param name="isBit">是否位读取</param>
        /// <returns>解析后的结果对象</returns>
        public static OperateResult<byte[]> ExtractActualData( byte[] response, bool isBit )
        {
            if (isBit)
            {
                // 位读取
                byte[] Content = new byte[response.Length - 22];
                for (int i = 22; i < response.Length; i++)
                {
                    Content[i - 22] = response[i] == 0x30 ? (byte)0x00 : (byte)0x01;
                }

                return OperateResult.CreateSuccessResult( Content );
            }
            else
            {
                // 字读取
                byte[] Content = new byte[(response.Length - 22) / 2];
                for (int i = 0; i < Content.Length / 2; i++)
                {
                    ushort tmp = Convert.ToUInt16( Encoding.ASCII.GetString( response, i * 4 + 22, 4 ), 16 );
                    BitConverter.GetBytes( tmp ).CopyTo( Content, i * 2 );
                }

                return OperateResult.CreateSuccessResult( Content );
            }
        }

        #endregion
    }
}
