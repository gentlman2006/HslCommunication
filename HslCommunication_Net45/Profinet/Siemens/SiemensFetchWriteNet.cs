using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using System.Net.Sockets;
using HslCommunication.Core.Net;


/********************************************************************************
 * 
 *    说明：西门子通讯类，使用Fetch/Write消息解析规格，和反字节转换规格来实现的
 *    
 *    继承自统一的自定义方法，需要在PLC端进行相关的数据配置
 * 
 * 
 *********************************************************************************/

namespace HslCommunication.Profinet.Siemens
{
    /// <summary>
    /// 使用了Fetch/Write协议来和西门子进行通讯，该种方法需要在PLC侧进行一些配置 ->
    /// Using the Fetch/write protocol to communicate with Siemens, this method requires some configuration on the PLC side
    /// </summary>
    /// <remarks>
    /// 与S7协议相比较而言，本协议不支持对单个的点位的读写操作。如果读取M100.0，需要读取M100的值，然后进行提取位数据
    /// </remarks>
    /// <example>
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensFetchWriteNet.cs" region="Usage" title="简单的短连接使用" />
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensFetchWriteNet.cs" region="Usage2" title="简单的长连接使用" />
    /// </example>
    public class SiemensFetchWriteNet : NetworkDeviceBase<FetchWriteMessage, ReverseBytesTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化一个西门子的Fetch/Write协议的通讯对象 ->
        /// Instantiate a communication object for a Siemens Fetch/write protocol
        /// </summary>
        public SiemensFetchWriteNet()
        {
            WordLength = 2;
        }

        /// <summary>
        /// 实例化一个西门子的Fetch/Write协议的通讯对象 ->
        /// Instantiate a communication object for a Siemens Fetch/write protocol
        /// </summary>
        /// <param name="ipAddress">PLC的Ip地址 -> Specify IP Address</param>
        /// <param name="port">PLC的端口 -> Specify IP Port</param>
        public SiemensFetchWriteNet(string ipAddress,int port)
        {
            WordLength = 2;
            IpAddress = ipAddress;
            Port = port;
        }

        #endregion

        #region Read Support
        
        /// <summary>
        /// 从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，T100，C100，以字节为单位 ->
        /// Read data from PLC, address format I100,Q100,DB20.100,M100,T100,C100, in bytes
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100，T100，C100 ->
        /// Starting address, formatted as I100,M100,Q100,DB20.100,T100,C100
        /// </param>
        /// <param name="length">读取的数量，以字节为单位 -> The number of reads, in bytes</param>
        /// <returns>带有成功标志的字节信息 -> Byte information with a success flag</returns>
        /// <remarks>
        /// 地址支持的列表如下：
        /// <list type="table">
        ///   <listheader>
        ///     <term>地址名称</term>
        ///     <term>示例</term>
        ///     <term>地址进制</term>
        ///   </listheader>
        ///   <item>
        ///     <term>中间寄存器</term>
        ///     <term>M100,M200</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>输入寄存器</term>
        ///     <term>I100,I200</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>输出寄存器</term>
        ///     <term>Q100,Q200</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>DB寄存器</term>
        ///     <term>DB1.100,DB1.200</term>
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
        /// 假设起始地址为M100，M100存储了温度，100.6℃值为1006，M102存储了压力，1.23Mpa值为123，M104，M105，M106，M107存储了产量计数，读取如下：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensFetchWriteNet.cs" region="ReadExample2" title="Read示例" />
        /// 以下是读取不同类型数据的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensFetchWriteNet.cs" region="ReadExample1" title="Read示例" />
        /// </example>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            // 指令解析 -> Instruction parsing
            OperateResult<byte[]> command = BuildReadCommand( address, length );
            if (!command.IsSuccess) return command;

            // 核心交互 -> Core Interactions
            OperateResult<byte[]> read = ReadFromCoreServer( command.Content );
            if (!read.IsSuccess) return read;

            // 错误码验证 -> Error code Verification
            if (read.Content[8] != 0x00) return new OperateResult<byte[]>( read.Content[8], StringResources.Language.SiemensFWError );

            // 读取正确 -> Read Right
            byte[] buffer = new byte[read.Content.Length - 16];
            Array.Copy( read.Content, 16, buffer, 0, buffer.Length );
            return OperateResult.CreateSuccessResult( buffer );
        }

        /// <summary>
        /// 读取指定地址的byte数据 -> Reads the byte data for the specified address
        /// </summary>
        /// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -> Starting address, formatted as M100,I100,Q100,DB1.100</param>
        /// <returns>byte类型的结果对象 -> Result object of type Byte</returns>
        /// <remarks>
        /// <note type="warning">
        /// 不适用于DB块，定时器，计数器的数据读取，会提示相应的错误，读取长度必须为偶数
        /// </note>
        /// </remarks>
        public OperateResult<byte> ReadByte(string address)
        {
            return ByteTransformHelper.GetResultFromArray( Read( address, 1 ) );
        }
        
        #endregion

        #region Write Base
        
        /// <summary>
        /// 将数据写入到PLC数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位 ->
        /// Writes data to the PLC data, in the address format i100,q100,db20.100,m100, in bytes
        /// </summary>
        /// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -> Starting address, formatted as M100,I100,Q100,DB1.100</param>
        /// <param name="value">要写入的实际数据 -> The actual data to write</param>
        /// <returns>是否写入成功的结果对象 -> Whether to write a successful result object</returns>
        /// <example>
        /// 假设起始地址为M100，M100,M101存储了温度，100.6℃值为1006，M102,M103存储了压力，1.23Mpa值为123，M104-M107存储了产量计数，写入如下：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensFetchWriteNet.cs" region="WriteExample2" title="Write示例" />
        /// 以下是写入不同类型数据的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensFetchWriteNet.cs" region="WriteExample1" title="Write示例" />
        /// </example>
        public override OperateResult Write( string address, byte[] value )
        {
            // 指令解析 -> Instruction parsing
            OperateResult<byte[]> command = BuildWriteCommand( address, value );
            if (!command.IsSuccess) return command;

            // 核心交互 -> Core Interactions
            OperateResult<byte[]> write = ReadFromCoreServer( command.Content );
            if (!write.IsSuccess) return write;

            // 错误码验证 -> Error code Verification
            if (write.Content[8] != 0x00) return new OperateResult( write.Content[8], StringResources.Language.SiemensWriteError + write.Content[8] );

            // 写入成功 -> Write Right
            return OperateResult.CreateSuccessResult( );

        }
        
        #endregion

        #region Write bool[]

        /// <summary>
        /// 向PLC中写入bool数组，返回值说明，比如你写入M100,那么data[0]对应M100.0 ->
        /// Write the bool array to the PLC, return the value description, for example, if you write M100, then data[0] corresponds to M100.0
        /// </summary>
        /// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -> Starting address, formatted as M100,I100,Q100,DB1.100</param>
        /// <param name="values">要写入的实际数据，长度为8的倍数 -> The actual data to write, a multiple of 8 in length</param>
        /// <returns>是否写入成功的结果对象 -> Whether to write a successful result object</returns>
        /// <remarks>
        /// <note type="warning">
        /// 批量写入bool数组存在一定的风险，原因是只能批量写入长度为8的倍数的数组，否则会影响其他的位的数据，请谨慎使用。 ->
        /// There is a risk in bulk writing to a bool array, because it is possible to write arrays of multiples of length 8 in bulk, otherwise it will affect the data of other bits, please use sparingly.
        /// </note>
        /// </remarks>
        public OperateResult Write(string address, bool[] values)
        {
            return Write( address, BasicFramework.SoftBasic.BoolArrayToByte( values ) );
        }
        
        #endregion

        #region Write Byte

        /// <summary>
        /// 向PLC中写入byte数据，返回是否写入成功 -> Writes byte data to the PLC and returns whether the write succeeded
        /// </summary>
        /// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -> Starting address, formatted as M100,I100,Q100,DB1.100</param>
        /// <param name="value">要写入的实际数据 -> The actual data to write</param>
        /// <returns>是否写入成功的结果对象 -> Whether to write a successful result object</returns>
        public OperateResult Write(string address, byte value)
        {
            return Write( address, new byte[] { value } );
        }

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串 -> Returns a String representing the current object
        /// </summary>
        /// <returns>字符串数据 -> String data</returns>
        public override string ToString()
        {
            return $"SiemensFetchWriteNet[{IpAddress}:{Port}]";
        }

        #endregion

        #region Static Method Helper
        
        /// <summary>
        /// 计算特殊的地址信息
        /// </summary>
        /// <param name="address">字符串信息</param>
        /// <returns>实际值</returns>
        private static int CalculateAddressStarted( string address )
        {
            if (address.IndexOf( '.' ) < 0)
            {
                return Convert.ToInt32( address );
            }
            else
            {
                string[] temp = address.Split( '.' );
                return Convert.ToInt32( temp[0] );
            }
        }

        /// <summary>
        /// 解析数据地址，解析出地址类型，起始地址，DB块的地址 -> Parse data address, parse out address type, start address, db block address
        /// </summary>
        /// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -> Starting address, formatted as M100,I100,Q100,DB1.100</param>
        /// <returns>解析出地址类型，起始地址，DB块的地址 -> Resolves address type, start address, db block address</returns>
        private static OperateResult<byte, int, ushort> AnalysisAddress( string address )
        {
            var result = new OperateResult<byte, int, ushort>( );
            try
            {
                result.Content3 = 0;
                if (address[0] == 'I')
                {
                    result.Content1 = 0x03;
                    result.Content2 = CalculateAddressStarted( address.Substring( 1 ) );
                }
                else if (address[0] == 'Q')
                {
                    result.Content1 = 0x04;
                    result.Content2 = CalculateAddressStarted( address.Substring( 1 ) );
                }
                else if (address[0] == 'M')
                {
                    result.Content1 = 0x02;
                    result.Content2 = CalculateAddressStarted( address.Substring( 1 ) );
                }
                else if (address[0] == 'D' || address.Substring( 0, 2 ) == "DB")
                {
                    result.Content1 = 0x01;
                    string[] adds = address.Split( '.' );
                    if (address[1] == 'B')
                    {
                        result.Content3 = Convert.ToUInt16( adds[0].Substring( 2 ) );
                    }
                    else
                    {
                        result.Content3 = Convert.ToUInt16( adds[0].Substring( 1 ) );
                    }

                    if (result.Content3 > 255)
                    {
                        result.Message = StringResources.Language.SiemensDBAddressNotAllowedLargerThan255;
                        return result;
                    }

                    result.Content2 = CalculateAddressStarted( address.Substring( address.IndexOf( '.' ) + 1 ) );
                }
                else if (address[0] == 'T')
                {
                    result.Content1 = 0x07;
                    result.Content2 = CalculateAddressStarted( address.Substring( 1 ) );
                }
                else if (address[0] == 'C')
                {
                    result.Content1 = 0x06;
                    result.Content2 = CalculateAddressStarted( address.Substring( 1 ) );
                }
                else
                {
                    result.Message = StringResources.Language.NotSupportedDataType;
                    result.Content1 = 0;
                    result.Content2 = 0;
                    result.Content3 = 0;
                    return result;
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
        
        #endregion

        #region Build Command

        /// <summary>
        /// 生成一个读取字数据指令头的通用方法 -> A general method for generating a command header to read a Word data
        /// </summary>
        /// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -> Starting address, formatted as M100,I100,Q100,DB1.100</param>
        /// <param name="count">读取数据个数 -> Number of Read data</param>
        /// <returns>带结果对象的报文数据 -> Message data with a result object</returns>
        public static OperateResult<byte[]> BuildReadCommand( string address, ushort count )
        {
            OperateResult<byte, int, ushort> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            byte[] _PLCCommand = new byte[16];
            _PLCCommand[0] = 0x53;
            _PLCCommand[1] = 0x35;
            _PLCCommand[2] = 0x10;
            _PLCCommand[3] = 0x01;
            _PLCCommand[4] = 0x03;
            _PLCCommand[5] = 0x05;
            _PLCCommand[6] = 0x03;
            _PLCCommand[7] = 0x08;

            // 指定数据区 -> Specify Data area
            _PLCCommand[8] = analysis.Content1;
            _PLCCommand[9] = (byte)analysis.Content3;

            // 指定数据地址 -> Specify Data address
            _PLCCommand[10] = (byte)(analysis.Content2 / 256);
            _PLCCommand[11] = (byte)(analysis.Content2 % 256);

            // DB块，定时器，计数器读取长度按照字为单位，1代表2个字节，I，Q，M的1代表1个字节 ->
            // DB block, timer, counter read length per word, 1 for 2 bytes, i,q,m 1 for 1 bytes
            if (analysis.Content1 == 0x01 || analysis.Content1 == 0x06 || analysis.Content1 == 0x07)
            {
                if (count % 2 != 0)
                {
                    return new OperateResult<byte[]>( StringResources.Language.SiemensReadLengthMustBeEvenNumber );
                }
                else
                {
                    // 指定数据长度 -> Specify data length
                    _PLCCommand[12] = (byte)(count / 2 / 256);
                    _PLCCommand[13] = (byte)(count / 2 % 256);
                }
            }
            else
            {
                // 指定数据长度 -> Specify data length
                _PLCCommand[12] = (byte)(count / 256);
                _PLCCommand[13] = (byte)(count % 256);
            }

            _PLCCommand[14] = 0xff;
            _PLCCommand[15] = 0x02;

            return OperateResult.CreateSuccessResult( _PLCCommand );
        }
        
        /// <summary>
        /// 生成一个写入字节数据的指令 -> Generate an instruction to write byte data
        /// </summary>
        /// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -> Starting address, formatted as M100,I100,Q100,DB1.100</param>
        /// <param name="data">实际的写入的内容 -> The actual content of the write</param>
        /// <returns>带结果对象的报文数据 -> Message data with a result object</returns>
        public static OperateResult<byte[]> BuildWriteCommand( string address, byte[] data )
        {
            if (data == null) data = new byte[0];

            OperateResult<byte, int, ushort> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );
            
            byte[] _PLCCommand = new byte[16 + data.Length];
            _PLCCommand[0] = 0x53;
            _PLCCommand[1] = 0x35;
            _PLCCommand[2] = 0x10;
            _PLCCommand[3] = 0x01;
            _PLCCommand[4] = 0x03;
            _PLCCommand[5] = 0x03;
            _PLCCommand[6] = 0x03;
            _PLCCommand[7] = 0x08;

            // 指定数据区 -> Specify Data area
            _PLCCommand[8] = analysis.Content1;
            _PLCCommand[9] = (byte)analysis.Content3;

            // 指定数据地址 -> Specify Data address
            _PLCCommand[10] = (byte)(analysis.Content2 / 256);
            _PLCCommand[11] = (byte)(analysis.Content2 % 256);

            if (analysis.Content1 == 0x01 || analysis.Content1 == 0x06 || analysis.Content1 == 0x07)
            {
                if (data.Length % 2 != 0)
                {
                    return new OperateResult<byte[]>( StringResources.Language.SiemensReadLengthMustBeEvenNumber );
                }
                else
                {
                    // 指定数据长度 -> Specify data length
                    _PLCCommand[12] = (byte)(data.Length / 2 / 256);
                    _PLCCommand[13] = (byte)(data.Length / 2 % 256);
                }
            }
            else
            {
                // 指定数据长度 -> Specify data length
                _PLCCommand[12] = (byte)(data.Length / 256);
                _PLCCommand[13] = (byte)(data.Length % 256);
            }
            _PLCCommand[14] = 0xff;
            _PLCCommand[15] = 0x02;

            // 放置数据 -> Placing data
            Array.Copy( data, 0, _PLCCommand, 16, data.Length );

            return OperateResult.CreateSuccessResult( _PLCCommand );
        }
        
        #endregion

    }
}
