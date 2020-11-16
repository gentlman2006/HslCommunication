using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;



/********************************************************************************
 * 
 *    说明：西门子通讯类，使用S7消息解析规格，和反字节转换规格来实现的
 *    
 *    继承自统一的自定义方法
 *    其中的启动，停止的功能代码参考了开源项目：https://github.com/fbarresi/Sharp7
 * 
 *********************************************************************************/




namespace HslCommunication.Profinet.Siemens
{
    /// <summary>
    /// 一个西门子的客户端类，使用S7协议来进行数据交互 ->
    /// A Siemens client class that uses the S7 protocol for data interaction
    /// </summary>
    /// <example>
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="Usage" title="简单的短连接使用" />
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="Usage2" title="简单的长连接使用" />
    /// </example>
    public class SiemensS7Net : NetworkDeviceBase<S7Message, ReverseBytesTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化一个西门子的S7协议的通讯对象 ->
        /// Instantiate a communication object for a Siemens S7 protocol
        /// </summary>
        /// <param name="siemens">指定西门子的型号</param>
        public SiemensS7Net( SiemensPLCS siemens )
        {
            Initialization( siemens, string.Empty );
        }

        /// <summary>
        /// 实例化一个西门子的S7协议的通讯对象并指定Ip地址 ->
        /// Instantiate a communication object for a Siemens S7 protocol and specify an IP address
        /// </summary>
        /// <param name="siemens">指定西门子的型号</param>
        /// <param name="ipAddress">Ip地址</param>
        public SiemensS7Net( SiemensPLCS siemens, string ipAddress )
        {
            Initialization( siemens, ipAddress );
        }

        /// <summary>
        /// 初始化方法 -> Initialize method
        /// </summary>
        /// <param name="siemens">指定西门子的型号 -> Designation of Siemens</param>
        /// <param name="ipAddress">Ip地址 -> IpAddress</param>
        private void Initialization( SiemensPLCS siemens, string ipAddress )
        {
            WordLength = 2;
            IpAddress = ipAddress;
            Port = 102;
            CurrentPlc = siemens;

            switch (siemens)
            {
                case SiemensPLCS.S1200: plcHead1[21] = 0; break;
                case SiemensPLCS.S300: plcHead1[21] = 2; break;
                case SiemensPLCS.S400: plcHead1[21] = 3; plcHead1[17] = 0x00; break;
                case SiemensPLCS.S1500: plcHead1[21] = 0; break;
                case SiemensPLCS.S200Smart:
                    {
                        plcHead1 = plcHead1_200smart;
                        plcHead2 = plcHead2_200smart;
                        break;
                    }
                default: plcHead1[18] = 0; break;
            }
        }

        /// <summary>
        /// PLC的槽号，针对S7-400的PLC设置的
        /// </summary>
        public byte Slot
        {
            get => plc_slot;
            set
            {
                plc_slot = value;
                plcHead1[21] = (byte)((this.plc_rack * 0x20) + this.plc_slot);
            }
        }

        /// <summary>
        /// PLC的机架号，针对S7-400的PLC设置的
        /// </summary>
        public byte Rack
        {
            get => plc_rack;
            set
            {
                this.plc_rack = value;
                plcHead1[21] = (byte)((this.plc_rack * 0x20) + this.plc_slot);
            }
        }

        #endregion

        #region NetworkDoubleBase Override

        /// <summary>
        /// 连接上服务器后需要进行的二次握手操作 -> Two handshake actions required after connecting to the server
        /// </summary>
        /// <param name="socket">网络套接字 -> Network sockets</param>
        /// <returns>是否初始化成功，依据具体的协议进行重写 ->
        /// Whether the initialization succeeds and is rewritten according to the specific protocol</returns>
        protected override OperateResult InitializationOnConnect( Socket socket )
        {
            // 第一次握手 -> First handshake
            OperateResult<byte[], byte[]> read_first = ReadFromCoreServerBase( socket, plcHead1 );
            if (!read_first.IsSuccess) return read_first;

            // 第二次握手 -> Second handshake
            OperateResult<byte[], byte[]> read_second = ReadFromCoreServerBase( socket, plcHead2 );
            if (!read_second.IsSuccess) return read_second;

            // 返回成功的信号 -> Return a successful signal
            return OperateResult.CreateSuccessResult( );
        }


        #endregion

        #region Read OrderNumber

        /// <summary>
        /// 从PLC读取订货号信息 -> Reading order number information from PLC
        /// </summary>
        /// <returns>CPU的订货号信息 -> Order number information for the CPU</returns>
        public OperateResult<string> ReadOrderNumber( )
        {
            OperateResult<byte[]> read = ReadFromCoreServer( plcOrderNumber );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return OperateResult.CreateSuccessResult( Encoding.ASCII.GetString( read.Content, 71, 20 ) );
        }

        #endregion

        #region Start Stop

        /// <summary>
        /// 对PLC进行热启动
        /// </summary>
        /// <returns>是否启动成功的结果对象</returns>
        public OperateResult HotStart( )
        {
            OperateResult<byte[]> read = ReadFromCoreServer( S7_HOT_START );
            if (!read.IsSuccess) return read;

            if (read.Content.Length < 19) return new OperateResult( "Receive error" );

            if (read.Content[19] != pduStart) return new OperateResult( "Can not start PLC" );
            else if(read.Content[20] != pduAlreadyStarted) return new OperateResult( "Can not start PLC" );

            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 对PLC进行冷启动
        /// </summary>
        /// <returns>是否启动成功的结果对象</returns>
        public OperateResult ColdStart( )
        {
            OperateResult<byte[]> read = ReadFromCoreServer( S7_COLD_START );
            if (!read.IsSuccess) return read;

            if (read.Content.Length < 19) return new OperateResult( "Receive error" );

            if (read.Content[19] != pduStart) return new OperateResult( "Can not start PLC" );
            else if (read.Content[20] != pduAlreadyStarted) return new OperateResult( "Can not start PLC" );

            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 对PLC进行停止
        /// </summary>
        /// <returns>是否启动成功的结果对象</returns>
        public OperateResult Stop( )
        {
            OperateResult<byte[]> read = ReadFromCoreServer( S7_STOP );
            if (!read.IsSuccess) return read;
            
            if(read.Content.Length < 19 ) return new OperateResult( "Receive error" );

            if (read.Content[19] != pduStop) return new OperateResult( "Can not stop PLC" );
            else if (read.Content[20] != pduAlreadyStopped) return new OperateResult( "Can not stop PLC" );

            return OperateResult.CreateSuccessResult( );
        }

        #endregion

        #region Read Support


        /// <summary>
        /// 从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，T100，C100以字节为单位 ->
        /// Read data from PLC, address format I100，Q100，DB20.100，M100，T100，C100 in bytes
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 ->
        /// Starting address, formatted as I100,M100,Q100,DB20.100</param>
        /// <param name="length">读取的数量，以字节为单位 -> The number of reads, in bytes</param>
        /// <returns>是否读取成功的结果对象 -> Whether to read the successful result object</returns>
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
        /// <note type="important">对于200smartPLC的V区，就是DB1.X，例如，V100=DB1.100</note>
        /// </remarks>
        /// <example>
        /// 假设起始地址为M100，M100存储了温度，100.6℃值为1006，M102存储了压力，1.23Mpa值为123，M104，M105，M106，M107存储了产量计数，读取如下：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="ReadExample2" title="Read示例" />
        /// 以下是读取不同类型数据的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="ReadExample1" title="Read示例" />
        /// </example>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            OperateResult<byte, int, ushort> addressResult = AnalysisAddress( address );
            if (!addressResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( addressResult );

            // 如果长度超过200，分批次读取 -> If the length is more than 200, read in batches
            List<byte> bytesContent = new List<byte>( );
            ushort alreadyFinished = 0;
            while (alreadyFinished < length)
            {
                ushort readLength = (ushort)Math.Min( length - alreadyFinished, 200 );
                OperateResult<byte[]> read = Read( new OperateResult<byte, int, ushort>[] { addressResult }, new ushort[] { readLength } );
                if (!read.IsSuccess) return read;

                bytesContent.AddRange( read.Content );
                alreadyFinished += readLength;
                addressResult.Content2 += readLength * 8;
            }

            return OperateResult.CreateSuccessResult( bytesContent.ToArray( ) );
        }

        /// <summary>
        /// 从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，以位为单位 ->
        /// Read the data from the PLC, the address format is I100，Q100，DB20.100，M100, in bits units
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 ->
        /// Starting address, formatted as I100,M100,Q100,DB20.100</param>
        /// <returns>是否读取成功的结果对象 -> Whether to read the successful result object</returns>
        private OperateResult<byte[]> ReadBitFromPLC( string address )
        {
            // 指令生成 -> Build bit read command
            OperateResult<byte[]> command = BuildBitReadCommand( address );
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( command );

            // 核心交互 ->  Core interactive
            OperateResult<byte[]> read = ReadFromCoreServer( command.Content );
            if (!read.IsSuccess) return read;
            
            // 分析结果 -> Analysis read result
            int receiveCount = 1;
            if (read.Content.Length >= 21 && read.Content[20] == 1)
            {
                byte[] buffer = new byte[receiveCount];
                if (22 < read.Content.Length)
                {
                    if (read.Content[21] == 0xFF &&
                        read.Content[22] == 0x03)
                    {
                        buffer[0] = read.Content[25];
                    }
                }

                return OperateResult.CreateSuccessResult( buffer );
            }
            else
            {
                return new OperateResult<byte[]>( read.ErrorCode, StringResources.Language.SiemensDataLengthCheckFailed );
            }
        }


        /// <summary>
        /// 一次性从PLC获取所有的数据，按照先后顺序返回一个统一的Buffer，需要按照顺序处理，两个数组长度必须一致 ->
        /// One-time from the PLC to obtain all the data, in order to return a unified buffer, need to be processed sequentially, two array length must be consistent
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 ->
        /// Starting address, formatted as I100,M100,Q100,DB20.100</param>
        /// <param name="length">数据长度数组 -> Array of data Lengths</param>
        /// <returns>是否读取成功的结果对象 -> Whether to read the successful result object</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <remarks>
        /// <note type="warning">批量读取的长度有限制，最大为19个地址</note>
        /// </remarks>
        /// <example>
        /// 参照<see cref="Read(string, ushort)"/>
        /// </example>
        public OperateResult<byte[]> Read( string[] address, ushort[] length )
        {
            OperateResult<byte, int, ushort>[] addressResult = new OperateResult<byte, int, ushort>[address.Length];
            for (int i = 0; i < address.Length; i++)
            {
                OperateResult<byte, int, ushort> tmp = AnalysisAddress( address[i] );
                if (!tmp.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( addressResult[i] );

                addressResult[i] = tmp;
            }

            return Read( addressResult, length );
        }

        private OperateResult<byte[]> Read( OperateResult<byte, int, ushort>[] address, ushort[] length )
        {
            // 构建指令 -> Build read command
            OperateResult<byte[]> command = BuildReadCommand( address, length );
            if (!command.IsSuccess) return command;

            // 核心交互 -> Core Interactions
            OperateResult<byte[]> read = ReadFromCoreServer( command.Content );
            if (!read.IsSuccess) return read;


            // 分析结果 -> Analysis results
            int receiveCount = 0;
            for (int i = 0; i < length.Length; i++)
            {
                receiveCount += length[i];
            }

            if (read.Content.Length >= 21 && read.Content[20] == length.Length)
            {
                byte[] buffer = new byte[receiveCount];
                int kk = 0;
                int ll = 0;
                for (int ii = 21; ii < read.Content.Length; ii++)
                {
                    if ((ii + 1) < read.Content.Length)
                    {
                        if (read.Content[ii] == 0xFF &&
                            read.Content[ii + 1] == 0x04)
                        {
                            Array.Copy( read.Content, ii + 4, buffer, ll, length[kk] );
                            ii += length[kk] + 3;
                            ll += length[kk];
                            kk++;
                        }
                    }
                }

                return OperateResult.CreateSuccessResult( buffer );
            }
            else
            {
                return new OperateResult<byte[]>( ) { ErrorCode = read.ErrorCode, Message = StringResources.Language.SiemensDataLengthCheckFailed };
            }
        }





        /// <summary>
        /// 读取指定地址的bool数据，地址格式为I100，M100，Q100，DB20.100 -> 
        /// reads bool data for the specified address in the format I100，M100，Q100，DB20.100
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 ->
        /// Starting address, formatted as I100,M100,Q100,DB20.100</param>
        /// <returns>是否读取成功的结果对象 -> Whether to read the successful result object</returns>
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
        ///     <term>M100.0,M200.7</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>输入寄存器</term>
        ///     <term>I100.0,I200.7</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>输出寄存器</term>
        ///     <term>Q100.0,Q200.7</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>DB寄存器</term>
        ///     <term>DB1.100.0,DB1.200.7</term>
        ///     <term>10</term>
        ///   </item>
        /// </list>
        /// <note type="important">
        /// 对于200smartPLC的V区，就是DB1.X，例如，V100=DB1.100
        /// </note>
        /// </remarks>
        /// <example>
        /// 假设读取M100.0的位是否通断
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="ReadBool" title="ReadBool示例" />
        /// </example>
        public OperateResult<bool> ReadBool( string address )
        {
            return ByteTransformHelper.GetResultFromBytes( ReadBitFromPLC( address ), m => m[0] != 0x00 );
        }


        /// <summary>
        /// 读取指定地址的byte数据，地址格式I100，M100，Q100，DB20.100 ->
        /// Reads the byte data of the specified address, the address format I100,Q100,DB20.100,M100
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 ->
        /// Starting address, formatted as I100,M100,Q100,DB20.100</param>
        /// <returns>是否读取成功的结果对象 -> Whether to read the successful result object</returns>
        /// <example>参考<see cref="Read(string, ushort)"/>的注释</example>
        public OperateResult<byte> ReadByte( string address )
        {
            return ByteTransformHelper.GetResultFromArray( Read( address, 1 ) );
        }


        #endregion

        #region Write Base


        /// <summary>
        /// 基础的写入数据的操作支持 -> Operational support for the underlying write data
        /// </summary>
        /// <param name="entireValue">完整的字节数据 -> Full byte data</param>
        /// <returns>是否写入成功的结果对象 -> Whether to write a successful result object</returns>
        private OperateResult WriteBase( byte[] entireValue )
        {
            OperateResult<byte[]> write = ReadFromCoreServer( entireValue );
            if (!write.IsSuccess) return write;

            if (write.Content[write.Content.Length - 1] != 0xFF)
            {
                // 写入异常 -> WriteError
                return new OperateResult( write.Content[write.Content.Length - 1], StringResources.Language.SiemensWriteError + write.Content[write.Content.Length - 1] );
            }
            else
            {
                return OperateResult.CreateSuccessResult( );
            }
        }


        /// <summary>
        /// 将数据写入到PLC数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位 ->
        /// Writes data to the PLC data, in the address format I100,Q100,DB20.100,M100, in bytes
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 ->
        /// Starting address, formatted as I100,M100,Q100,DB20.100</param>
        /// <param name="value">写入的原始数据 -> Raw data written to</param>
        /// <returns>是否写入成功的结果对象 -> Whether to write a successful result object</returns>
        /// <example>
        /// 假设起始地址为M100，M100,M101存储了温度，100.6℃值为1006，M102,M103存储了压力，1.23Mpa值为123，M104-M107存储了产量计数，写入如下：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="WriteExample2" title="Write示例" />
        /// 以下是写入不同类型数据的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="WriteExample1" title="Write示例" />
        /// </example>
        public override OperateResult Write( string address, byte[] value )
        {
            OperateResult<byte, int, ushort> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );
            
            int length = value.Length;
            ushort alreadyFinished = 0;
            while (alreadyFinished < length)
            {
                ushort writeLength = (ushort)Math.Min( length - alreadyFinished, 200 );
                byte[] buffer = ByteTransform.TransByte( value, alreadyFinished, writeLength );

                OperateResult<byte[]> command = BuildWriteByteCommand( analysis, buffer );
                if (!command.IsSuccess) return command;

                OperateResult write = WriteBase( command.Content );
                if (!write.IsSuccess) return write;
                
                alreadyFinished += writeLength;
                analysis.Content2 += writeLength * 8;
            }

            return OperateResult.CreateSuccessResult( );
        }


        /// <summary>
        /// 写入PLC的一个位，例如"M100.6"，"I100.7"，"Q100.0"，"DB20.100.0"，如果只写了"M100"默认为"M100.0" ->
        /// Write a bit of PLC, for example  "M100.6",  "I100.7",  "Q100.0",  "DB20.100.0", if only write  "M100" defaults to  "M100.0"
        /// </summary>
        /// <param name="address">起始地址，格式为"M100.6",  "I100.7",  "Q100.0",  "DB20.100.0" ->
        /// Start address, format  "M100.6",  "I100.7",  "Q100.0",  "DB20.100.0"</param>
        /// <param name="value">写入的数据，True或是False -> Writes the data, either True or False</param>
        /// <returns>是否写入成功的结果对象 -> Whether to write a successful result object</returns>
        /// <example>
        /// 假设写入M100.0的位是否通断
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="WriteBool" title="WriteBool示例" />
        /// </example>
        public OperateResult Write( string address, bool value )
        {
            // 生成指令 -> Build Command
            OperateResult<byte[]> command = BuildWriteBitCommand( address, value );
            if (!command.IsSuccess) return command;

            return WriteBase( command.Content );
        }


        #endregion

        #region Write bool[]

        /// <summary>
        /// 向PLC中写入bool数组，比如你写入M100,那么data[0]对应M100.0 ->
        /// Write the bool array to the PLC, for example, if you write M100, then data[0] corresponds to M100.0
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -> Starting address, formatted as I100,mM100,Q100,DB20.100</param>
        /// <param name="values">要写入的bool数组，长度为8的倍数 -> The bool array to write, a multiple of 8 in length</param>
        /// <returns>是否写入成功的结果对象 -> Whether to write a successful result object</returns>
        /// <remarks>
        /// <note type="warning">
        /// 批量写入bool数组存在一定的风险，原因是只能批量写入长度为8的倍数的数组，否则会影响其他的位的数据，请谨慎使用。
        /// </note>
        /// </remarks>
        public OperateResult Write(string address, bool[] values)
        {
            return Write( address, BasicFramework.SoftBasic.BoolArrayToByte( values ) );
        }


        #endregion

        #region Write Byte

        /// <summary>
        /// 向PLC中写入byte数据，返回值说明 -> Write byte data to the PLC, return value description
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -> Starting address, formatted as I100,mM100,Q100,DB20.100</param>
        /// <param name="value">byte数据 -> Byte data</param>
        /// <returns>是否写入成功的结果对象 -> Whether to write a successful result object</returns>
        public OperateResult Write(string address, byte value)
        {
            return Write( address, new byte[] { value } );
        }

        #endregion

        #region Head Codes

        private byte[] plcHead1 = new byte[22]
        {
            0x03,0x00,0x00,0x16,0x11,0xE0,0x00,0x00,0x00,0x01,0x00,0xC0,0x01,0x0A,0xC1,0x02,
            0x01,0x02,0xC2,0x02,0x01,0x00 
        };
        private byte[] plcHead2 = new byte[25]
        {
            0x03,0x00,0x00,0x19,0x02,0xF0,0x80,0x32,0x01,0x00,0x00,0x04,0x00,0x00,0x08,0x00,
            0x00,0xF0,0x00,0x00,0x01,0x00,0x01,0x01,0xE0
        };
        private byte[] plcOrderNumber = new byte[]
        {
            0x03,0x00,0x00,0x21,0x02,0xF0,0x80,0x32,0x07,0x00,0x00,0x00,0x01,0x00,0x08,0x00,
            0x08,0x00,0x01,0x12,0x04,0x11,0x44,0x01,0x00,0xFF,0x09,0x00,0x04,0x00,0x11,0x00,
            0x00
        };
        private SiemensPLCS CurrentPlc = SiemensPLCS.S1200;
        private byte[] plcHead1_200smart = new byte[22]
        {
            0x03,0x00,0x00,0x16,0x11,0xE0,0x00,0x00,0x00,0x01,0x00,0xC1,0x02,0x10,0x00,0xC2,
            0x02,0x03,0x00,0xC0,0x01,0x0A 
        };
        private byte[] plcHead2_200smart = new byte[25]
        {
            0x03,0x00,0x00,0x19,0x02,0xF0,0x80,0x32,0x01,0x00,0x00,0xCC,0xC1,0x00,0x08,0x00,
            0x00,0xF0,0x00,0x00,0x01,0x00,0x01,0x03,0xC0
        };
        
        byte[] S7_STOP = {
            0x03, 0x00, 0x00, 0x21, 0x02, 0xf0, 0x80, 0x32, 0x01, 0x00, 0x00, 0x0e, 0x00, 0x00, 0x10, 0x00,
            0x00, 0x29, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x50, 0x5f, 0x50, 0x52, 0x4f, 0x47, 0x52, 0x41,
            0x4d
        };
        
        byte[] S7_HOT_START = {
            0x03, 0x00, 0x00, 0x25, 0x02, 0xf0, 0x80, 0x32, 0x01, 0x00, 0x00, 0x0c, 0x00, 0x00, 0x14, 0x00,
            0x00, 0x28, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xfd, 0x00, 0x00, 0x09, 0x50, 0x5f, 0x50, 0x52,
            0x4f, 0x47, 0x52, 0x41, 0x4d
        };
        
        byte[] S7_COLD_START = {
            0x03, 0x00, 0x00, 0x27, 0x02, 0xf0, 0x80, 0x32, 0x01, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x16, 0x00,
            0x00, 0x28, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xfd, 0x00, 0x02, 0x43, 0x20, 0x09, 0x50, 0x5f,
            0x50, 0x52, 0x4f, 0x47, 0x52, 0x41, 0x4d
        };

        #endregion

        #region Private Member

        private byte plc_rack = 0x00;
        private byte plc_slot = 0x00;

        const byte pduStart = 0x28;            // CPU start
        const byte pduStop = 0x29;             // CPU stop
        const byte pduAlreadyStarted = 0x02;   // CPU already in run mode
        const byte pduAlreadyStopped = 0x07;   // CPU already in stop mode

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串 -> Returns a String representing the current object
        /// </summary>
        /// <returns>字符串信息 -> String information</returns>
        public override string ToString()
        {
            return $"SiemensS7Net[{IpAddress}:{Port}]";
        }

        #endregion

        #region Static Method Helper
        
        /// <summary>
        /// 计算特殊的地址信息 -> Calculate Special Address information
        /// </summary>
        /// <param name="address">字符串地址 -> String address</param>
        /// <returns>实际值 -> Actual value</returns>
        public static int CalculateAddressStarted( string address )
        {
            if (address.IndexOf( '.' ) < 0)
            {
                return Convert.ToInt32( address ) * 8;
            }
            else
            {
                string[] temp = address.Split( '.' );
                return Convert.ToInt32( temp[0] ) * 8 + Convert.ToInt32( temp[1] );
            }
        }

        /// <summary>
        /// 解析数据地址，解析出地址类型，起始地址，DB块的地址 ->
        /// Parse data address, parse out address type, start address, db block address
        /// </summary>
        /// <param name="address">起始地址，例如M100，I0，Q0，DB2.100 ->
        /// Start address, such as M100,I0,Q0,DB2.100</param>
        /// <returns>解析数据地址，解析出地址类型，起始地址，DB块的地址 ->
        /// Parse data address, parse out address type, start address, db block address</returns>
        private static OperateResult<byte, int, ushort> AnalysisAddress( string address )
        {
            var result = new OperateResult<byte, int, ushort>( );
            try
            {
                result.Content3 = 0;
                if (address[0] == 'I')
                {
                    result.Content1 = 0x81;
                    result.Content2 = CalculateAddressStarted( address.Substring( 1 ) );
                }
                else if (address[0] == 'Q')
                {
                    result.Content1 = 0x82;
                    result.Content2 = CalculateAddressStarted( address.Substring( 1 ) );
                }
                else if (address[0] == 'M')
                {
                    result.Content1 = 0x83;
                    result.Content2 = CalculateAddressStarted( address.Substring( 1 ) );
                }
                else if (address[0] == 'D' || address.Substring( 0, 2 ) == "DB")
                {
                    result.Content1 = 0x84;
                    string[] adds = address.Split( '.' );
                    if (address[1] == 'B')
                    {
                        result.Content3 = Convert.ToUInt16( adds[0].Substring( 2 ) );
                    }
                    else
                    {
                        result.Content3 = Convert.ToUInt16( adds[0].Substring( 1 ) );
                    }

                    result.Content2 = CalculateAddressStarted( address.Substring( address.IndexOf( '.' ) + 1 ) );
                }
                else if (address[0] == 'T')
                {
                    result.Content1 = 0x1D;
                    result.Content2 = CalculateAddressStarted( address.Substring( 1 ) );
                }
                else if (address[0] == 'C')
                {
                    result.Content1 = 0x1C;
                    result.Content2 = CalculateAddressStarted( address.Substring( 1 ) );
                }
                else if (address[0] == 'V')
                {
                    result.Content1 = 0x84;
                    result.Content3 = 1;
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
        /// 生成一个读取字数据指令头的通用方法 ->
        /// A general method for generating a command header to read a Word data
        /// </summary>
        /// <param name="address">起始地址，例如M100，I0，Q0，DB2.100 ->
        /// Start address, such as M100,I0,Q0,DB2.100</param>
        /// <param name="length">读取数据长度 -> Read Data length</param>
        /// <returns>包含结果对象的报文 -> Message containing the result object</returns>
        public static OperateResult<byte[]> BuildReadCommand( string address, ushort length )
        {
            OperateResult<byte, int, ushort> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            return BuildReadCommand( new OperateResult<byte, int, ushort>[] { analysis }, new ushort[] { length } );
        }

        /// <summary>
        /// 生成一个读取字数据指令头的通用方法 ->
        /// A general method for generating a command header to read a Word data
        /// </summary>
        /// <param name="address">起始地址，例如M100，I0，Q0，DB2.100 ->
        /// Start address, such as M100,I0,Q0,DB2.100</param>
        /// <param name="length">读取数据长度 -> Read Data length</param>
        /// <returns>包含结果对象的报文 -> Message containing the result object</returns>
        public static OperateResult<byte[]> BuildReadCommand( OperateResult<byte, int, ushort>[] address, ushort[] length )
        {
            if (address == null) throw new NullReferenceException( "address" );
            if (length == null) throw new NullReferenceException( "count" );
            if (address.Length != length.Length) throw new Exception( StringResources.Language.TwoParametersLengthIsNotSame );
            if (length.Length > 19) throw new Exception( StringResources.Language.SiemensReadLengthCannotLargerThan19 );

            int readCount = length.Length;
            byte[] _PLCCommand = new byte[19 + readCount * 12];
            // ======================================================================================
            _PLCCommand[0] = 0x03;                                                // 报文头 -> Head
            _PLCCommand[1] = 0x00;
            _PLCCommand[2] = (byte)(_PLCCommand.Length / 256);                    // 长度 -> Length
            _PLCCommand[3] = (byte)(_PLCCommand.Length % 256);
            _PLCCommand[4] = 0x02;                                                // 固定 -> Fixed
            _PLCCommand[5] = 0xF0;
            _PLCCommand[6] = 0x80;
            _PLCCommand[7] = 0x32;                                                // 协议标识 -> Protocol identification
            _PLCCommand[8] = 0x01;                                                // 命令：发 -> Command: Send
            _PLCCommand[9] = 0x00;                                                // redundancy identification (reserved): 0x0000;
            _PLCCommand[10] = 0x00;                                               // protocol data unit reference; it’s increased by request event;
            _PLCCommand[11] = 0x00;
            _PLCCommand[12] = 0x01;                                               // 参数命令数据总长度 -> Parameter command Data total length
            _PLCCommand[13] = (byte)((_PLCCommand.Length - 17) / 256);
            _PLCCommand[14] = (byte)((_PLCCommand.Length - 17) % 256);
            _PLCCommand[15] = 0x00;                                               // 读取内部数据时为00，读取CPU型号为Data数据长度 -> Read internal data is 00, read CPU model is data length
            _PLCCommand[16] = 0x00;
            // =====================================================================================
            _PLCCommand[17] = 0x04;                                               // 读写指令，04读，05写 -> Read-write instruction, 04 read, 05 Write
            _PLCCommand[18] = (byte)readCount;                                    // 读取数据块个数 -> Number of data blocks read

            for (int ii = 0; ii < readCount; ii++)
            {
                //===========================================================================================
                // 指定有效值类型 -> Specify a valid value type
                _PLCCommand[19 + ii * 12] = 0x12;
                // 接下来本次地址访问长度 -> The next time the address access length
                _PLCCommand[20 + ii * 12] = 0x0A;
                // 语法标记，ANY -> Syntax tag, any
                _PLCCommand[21 + ii * 12] = 0x10;
                // 按字为单位 -> by word
                _PLCCommand[22 + ii * 12] = 0x02;
                // 访问数据的个数 -> Number of Access data
                _PLCCommand[23 + ii * 12] = (byte)(length[ii] / 256);
                _PLCCommand[24 + ii * 12] = (byte)(length[ii] % 256);
                // DB块编号，如果访问的是DB块的话 -> DB block number, if you are accessing a DB block
                _PLCCommand[25 + ii * 12] = (byte)(address[ii].Content3 / 256);
                _PLCCommand[26 + ii * 12] = (byte)(address[ii].Content3 % 256);
                // 访问数据类型 -> Accessing data types
                _PLCCommand[27 + ii * 12] = address[ii].Content1;
                // 偏移位置 -> Offset position
                _PLCCommand[28 + ii * 12] = (byte)(address[ii].Content2 / 256 / 256 % 256);
                _PLCCommand[29 + ii * 12] = (byte)(address[ii].Content2 / 256 % 256);
                _PLCCommand[30 + ii * 12] = (byte)(address[ii].Content2 % 256);
            }

            return OperateResult.CreateSuccessResult( _PLCCommand );
        }

        /// <summary>
        /// 生成一个位读取数据指令头的通用方法 ->
        /// A general method for generating a bit-read-Data instruction header
        /// </summary>
        /// <param name="address">起始地址，例如M100.0，I0.1，Q0.1，DB2.100.2 ->
        /// Start address, such as M100.0,I0.1,Q0.1,DB2.100.2
        /// </param>
        /// <returns>包含结果对象的报文 -> Message containing the result object</returns>
        public static OperateResult<byte[]> BuildBitReadCommand( string address )
        {
            OperateResult<byte, int, ushort> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );
            
            byte[] _PLCCommand = new byte[31];
            _PLCCommand[0] = 0x03;
            _PLCCommand[1] = 0x00;
            // 长度 -> Length
            _PLCCommand[2] = (byte)(_PLCCommand.Length / 256);
            _PLCCommand[3] = (byte)(_PLCCommand.Length % 256);
            // 固定 -> Fixed
            _PLCCommand[4] = 0x02;
            _PLCCommand[5] = 0xF0;
            _PLCCommand[6] = 0x80;
            _PLCCommand[7] = 0x32;
            // 命令：发 -> command to send
            _PLCCommand[8] = 0x01;
            // 标识序列号
            _PLCCommand[9] = 0x00;
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x00;
            _PLCCommand[12] = 0x01;
            // 命令数据总长度 -> Identification serial Number
            _PLCCommand[13] = (byte)((_PLCCommand.Length - 17) / 256);
            _PLCCommand[14] = (byte)((_PLCCommand.Length - 17) % 256);

            _PLCCommand[15] = 0x00;
            _PLCCommand[16] = 0x00;

            // 命令起始符 -> Command start character
            _PLCCommand[17] = 0x04;
            // 读取数据块个数 -> Number of data blocks read
            _PLCCommand[18] = 0x01;

            //===========================================================================================
            // 读取地址的前缀 -> Read the prefix of the address
            _PLCCommand[19] = 0x12;
            _PLCCommand[20] = 0x0A;
            _PLCCommand[21] = 0x10;
            // 读取的数据时位 -> Data read-time bit
            _PLCCommand[22] = 0x01;
            // 访问数据的个数 -> Number of Access data
            _PLCCommand[23] = 0x00;
            _PLCCommand[24] = 0x01;
            // DB块编号，如果访问的是DB块的话 -> DB block number, if you are accessing a DB block
            _PLCCommand[25] = (byte)(analysis.Content3 / 256);
            _PLCCommand[26] = (byte)(analysis.Content3 % 256);
            // 访问数据类型 -> Types of reading data
            _PLCCommand[27] = analysis.Content1;
            // 偏移位置 -> Offset position
            _PLCCommand[28] = (byte)(analysis.Content2 / 256 / 256 % 256);
            _PLCCommand[29] = (byte)(analysis.Content2 / 256 % 256);
            _PLCCommand[30] = (byte)(analysis.Content2 % 256);

            return OperateResult.CreateSuccessResult( _PLCCommand );
        }


        /// <summary>
        /// 生成一个写入字节数据的指令 -> Generate an instruction to write byte data
        /// </summary>
        /// <param name="address">起始地址，示例M100,I100,Q100,DB1.100 -> Start Address, example M100,I100,Q100,DB1.100</param>
        /// <param name="data">原始的字节数据 -> Raw byte data</param>
        /// <returns>包含结果对象的报文 -> Message containing the result object</returns>
        public static OperateResult<byte[]> BuildWriteByteCommand( string address, byte[] data )
        {
            if (data == null) data = new byte[0];

            OperateResult<byte, int, ushort> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            return BuildWriteByteCommand( analysis, data );
        }

        /// <summary>
        /// 生成一个写入字节数据的指令 -> Generate an instruction to write byte data
        /// </summary>
        /// <param name="analysis">起始地址，示例M100,I100,Q100,DB1.100 -> Start Address, example M100,I100,Q100,DB1.100</param>
        /// <param name="data">原始的字节数据 -> Raw byte data</param>
        /// <returns>包含结果对象的报文 -> Message containing the result object</returns>
        public static OperateResult<byte[]> BuildWriteByteCommand( OperateResult<byte, int, ushort> analysis, byte[] data )
        {
            byte[] _PLCCommand = new byte[35 + data.Length];
            _PLCCommand[0] = 0x03;
            _PLCCommand[1] = 0x00;
            // 长度 -> Length
            _PLCCommand[2] = (byte)((35 + data.Length) / 256);
            _PLCCommand[3] = (byte)((35 + data.Length) % 256);
            // 固定 -> Fixed
            _PLCCommand[4] = 0x02;
            _PLCCommand[5] = 0xF0;
            _PLCCommand[6] = 0x80;
            _PLCCommand[7] = 0x32;
            // 命令 发 -> command to send
            _PLCCommand[8] = 0x01;
            // 标识序列号 -> Identification serial Number
            _PLCCommand[9] = 0x00;
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x00;
            _PLCCommand[12] = 0x01;
            // 固定 -> Fixed
            _PLCCommand[13] = 0x00;
            _PLCCommand[14] = 0x0E;
            // 写入长度+4 -> Write Length +4
            _PLCCommand[15] = (byte)((4 + data.Length) / 256);
            _PLCCommand[16] = (byte)((4 + data.Length) % 256);
            // 读写指令 -> Read and write instructions
            _PLCCommand[17] = 0x05;
            // 写入数据块个数 -> Number of data blocks written
            _PLCCommand[18] = 0x01;
            // 固定，返回数据长度 -> Fixed, return data length
            _PLCCommand[19] = 0x12;
            _PLCCommand[20] = 0x0A;
            _PLCCommand[21] = 0x10;
            // 写入方式，1是按位，2是按字 -> Write mode, 1 is bitwise, 2 is by word
            _PLCCommand[22] = 0x02;
            // 写入数据的个数 -> Number of Write Data
            _PLCCommand[23] = (byte)(data.Length / 256);
            _PLCCommand[24] = (byte)(data.Length % 256);
            // DB块编号，如果访问的是DB块的话 -> DB block number, if you are accessing a DB block
            _PLCCommand[25] = (byte)(analysis.Content3 / 256);
            _PLCCommand[26] = (byte)(analysis.Content3 % 256);
            // 写入数据的类型 -> Types of writing data
            _PLCCommand[27] = analysis.Content1;
            // 偏移位置 -> Offset position
            _PLCCommand[28] = (byte)(analysis.Content2 / 256 / 256 % 256); ;
            _PLCCommand[29] = (byte)(analysis.Content2 / 256 % 256);
            _PLCCommand[30] = (byte)(analysis.Content2 % 256);
            // 按字写入 -> Write by Word
            _PLCCommand[31] = 0x00;
            _PLCCommand[32] = 0x04;
            // 按位计算的长度 -> The length of the bitwise calculation
            _PLCCommand[33] = (byte)(data.Length * 8 / 256);
            _PLCCommand[34] = (byte)(data.Length * 8 % 256);

            data.CopyTo( _PLCCommand, 35 );

            return OperateResult.CreateSuccessResult( _PLCCommand );
        }

        /// <summary>
        /// 生成一个写入位数据的指令 -> Generate an instruction to write bit data
        /// </summary>
        /// <param name="address">起始地址，示例M100,I100,Q100,DB1.100 -> Start Address, example M100,I100,Q100,DB1.100</param>
        /// <param name="data">是否通断 -> Power on or off</param>
        /// <returns>包含结果对象的报文 -> Message containing the result object</returns>
        public static OperateResult<byte[]> BuildWriteBitCommand( string address, bool data )
        {
            OperateResult<byte, int, ushort> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );


            byte[] buffer = new byte[1];
            buffer[0] = data ? (byte)0x01 : (byte)0x00;

            byte[] _PLCCommand = new byte[35 + buffer.Length];
            _PLCCommand[0] = 0x03;
            _PLCCommand[1] = 0x00;
            // 长度 -> length
            _PLCCommand[2] = (byte)((35 + buffer.Length) / 256);
            _PLCCommand[3] = (byte)((35 + buffer.Length) % 256);
            // 固定 -> fixed
            _PLCCommand[4] = 0x02;
            _PLCCommand[5] = 0xF0;
            _PLCCommand[6] = 0x80;
            _PLCCommand[7] = 0x32;
            // 命令 发 -> command to send
            _PLCCommand[8] = 0x01;
            // 标识序列号 -> Identification serial Number
            _PLCCommand[9] = 0x00;
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x00;
            _PLCCommand[12] = 0x01;
            // 固定 -> fixed
            _PLCCommand[13] = 0x00;
            _PLCCommand[14] = 0x0E;
            // 写入长度+4 -> Write Length +4
            _PLCCommand[15] = (byte)((4 + buffer.Length) / 256);
            _PLCCommand[16] = (byte)((4 + buffer.Length) % 256);
            // 命令起始符 -> Command start character
            _PLCCommand[17] = 0x05;
            // 写入数据块个数 -> Number of data blocks written
            _PLCCommand[18] = 0x01;
            _PLCCommand[19] = 0x12;
            _PLCCommand[20] = 0x0A;
            _PLCCommand[21] = 0x10;
            // 写入方式，1是按位，2是按字 -> Write mode, 1 is bitwise, 2 is by word
            _PLCCommand[22] = 0x01;
            // 写入数据的个数 -> Number of Write Data
            _PLCCommand[23] = (byte)(buffer.Length / 256);
            _PLCCommand[24] = (byte)(buffer.Length % 256);
            // DB块编号，如果访问的是DB块的话 -> DB block number, if you are accessing a DB block
            _PLCCommand[25] = (byte)(analysis.Content3 / 256);
            _PLCCommand[26] = (byte)(analysis.Content3 % 256);
            // 写入数据的类型 -> Types of writing data
            _PLCCommand[27] = analysis.Content1;
            // 偏移位置 -> Offset position
            _PLCCommand[28] = (byte)(analysis.Content2 / 256 / 256);
            _PLCCommand[29] = (byte)(analysis.Content2 / 256);
            _PLCCommand[30] = (byte)(analysis.Content2 % 256);
            // 按位写入 -> Bitwise Write
            _PLCCommand[31] = 0x00;
            _PLCCommand[32] = 0x03;
            // 按位计算的长度 -> The length of the bitwise calculation
            _PLCCommand[33] = (byte)(buffer.Length / 256);
            _PLCCommand[34] = (byte)(buffer.Length % 256);

            buffer.CopyTo( _PLCCommand, 35 );

            return OperateResult.CreateSuccessResult( _PLCCommand );
        }
        

        #endregion

    }
}
