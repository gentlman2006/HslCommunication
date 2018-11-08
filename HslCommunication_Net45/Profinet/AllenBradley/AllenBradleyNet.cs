using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;


/*********************************************************************************************
 * 
 *    thanks: 江阴-  ∮溪风-⊙_⌒ 提供了测试的PLC
 *    
 *    感谢一个开源的java项目支持才使得本项目顺利开发：https://github.com/Tulioh/Ethernetip4j
 * 
 ***********************************************************************************************/

namespace HslCommunication.Profinet.AllenBradley
{
    /// <summary>
    /// AB PLC的数据通讯类，支持读写PLC的节点数据，支持单个节点的读写，以及数组节点的读写操作。 ->
    /// AB PLC Data communication class, support read and write PLC node data
    /// </summary>
    public class AllenBradleyNet : NetworkDeviceBase<AllenBradleyMessage, RegularByteTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化一个AllenBradley PLC协议的通讯对象 ->
        /// Instantiate a communication object for a Allenbradley PLC protocol
        /// </summary>
        public AllenBradleyNet( )
        {
            WordLength = 2;
        }

        /// <summary>
        /// 实例化一个AllenBradley PLC协议的通讯对象 ->
        /// Instantiate a communication object for a Allenbradley PLC protocol
        /// </summary>
        /// <param name="ipAddress">PLCd的Ip地址</param>
        /// <param name="port">PLC的端口</param>
        public AllenBradleyNet( string ipAddress, int port = 44818 )
        {
            WordLength = 2;
            IpAddress = ipAddress;
            Port = port;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 当前的会话句柄，该值在和PLC握手通信时由PLC进行决定
        /// </summary>
        public uint SessionHandle { get; private set; }

        #endregion

        #region Double Mode Override

        /// <summary>
        /// 在连接上AllenBradley PLC后，需要进行一步握手协议
        /// </summary>
        /// <param name="socket">连接的套接字</param>
        /// <returns>初始化成功与否</returns>
        protected override OperateResult InitializationOnConnect( Socket socket )
        {
            // 注册会话信息
            OperateResult<byte[], byte[]> read1 = ReadFromCoreServerBase( socket, RegisterSessionHandle( ) );
            if (!read1.IsSuccess) return read1;

            // 检查返回的状态
            OperateResult check1 = CheckResponse( read1.Content1 );
            if (!check1.IsSuccess) return check1;

            // 提取会话ID
            SessionHandle = ByteTransform.TransUInt32( read1.Content1, 4 );
            
            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 在断开AllenBradley PLC前，需要进行一步握手协议
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <returns>断开操作是否成功</returns>
        protected override OperateResult ExtraOnDisconnect( Socket socket )
        {
            // 注册会话信息
            OperateResult<byte[], byte[]> read1 = ReadFromCoreServerBase( socket, UnRegisterSessionHandle( ) );
            if (!read1.IsSuccess) return read1;

            return OperateResult.CreateSuccessResult( );
        }

        #endregion

        #region Build Command

        /// <summary>
        /// 创建一个读取的报文指令
        /// </summary>
        /// <param name="address">tag名的地址</param>
        /// <param name="length">数组信息，如果不是数组，就都为1</param>
        /// <returns>包含结果对象的报文信息</returns>
        public OperateResult<byte[]> BuildReadCommand( string[] address, int[] length )
        {
            if (address == null || length == null) return new OperateResult<byte[]>( "address or length is null" );
            if (address.Length != length.Length) return new OperateResult<byte[]>( "address and length is not same array" );

            List<byte[]> cips = new List<byte[]>( );
            for (int i = 0; i < address.Length; i++)
            {
                cips.Add( AllenBradleyHelper.PackRequsetRead( address[i], length[i] ) );
            }
            byte[] commandSpecificData = AllenBradleyHelper.PackCommandSpecificData( cips.ToArray( ) );
            
            return OperateResult.CreateSuccessResult( AllenBradleyHelper.PackRequestHeader( 0x6F, SessionHandle, commandSpecificData ) );
        }

        /// <summary>
        /// 创建一个读取的报文指令
        /// </summary>
        /// <param name="address">tag名的地址</param>
        /// <returns>包含结果对象的报文信息</returns>
        public OperateResult<byte[]> BuildReadCommand( string[] address )
        {
            if (address == null ) return new OperateResult<byte[]>( "address or length is null" );

            int[] length = new int[address.Length];
            for (int i = 0; i < address.Length; i++)
            {
                length[i] = 1;
            }

            return BuildReadCommand( address, length );
        }

        /// <summary>
        /// 创建一个写入的报文指令
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="typeCode">类型数据</param>
        /// <param name="data">数据</param>
        /// <param name="length">如果是数组，就为数组长度</param>
        /// <returns>包含结果对象的报文信息</returns>
        public OperateResult<byte[]> BuildWriteCommand( string address, ushort typeCode, byte[] data, int length = 1 )
        {
            byte[] cip = AllenBradleyHelper.PackRequestWrite( address, typeCode, data, length );
            byte[] commandSpecificData = AllenBradleyHelper.PackCommandSpecificData(  cip );
            
            return OperateResult.CreateSuccessResult( AllenBradleyHelper.PackRequestHeader( 0x6F, SessionHandle, commandSpecificData ) );
        }

        #endregion

        #region Override Read

        /// <summary>
        /// 读取数据信息，数据长度无效
        /// </summary>
        /// <param name="address">节点的地址格式</param>
        /// <param name="length">无效的参数</param>
        /// <returns>带有结果对象的结果数据</returns>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            return Read( new string[] { address }, new int[] { length } );
        }

        /// <summary>
        /// 批量读取数据信息，数据长度无效
        /// </summary>
        /// <param name="address">节点的地址格式</param>
        /// <returns>带有结果对象的结果数据</returns>
        public OperateResult<byte[]> Read( string[] address )
        {
            if (address == null) return new OperateResult<byte[]>( "address can not be null" );

            int[] length = new int[address.Length];
            for (int i = 0; i < length.Length; i++)
            {
                length[i] = 1;
            }

            return Read( address, length );
        }

        /// <summary>
        /// 批量读取数据信息，数据长度无效
        /// </summary>
        /// <param name="address">节点的地址格式</param>
        /// <param name="length">每个地址的数组长度</param>
        /// <returns>带有结果对象的结果数据</returns>
        public OperateResult<byte[]> Read( string[] address, int[] length )
        {
            // 指令生成
            OperateResult<byte[]> command = BuildReadCommand( address, length );
            if (!command.IsSuccess) return command;

            // 核心交互
            OperateResult<byte[]> read = ReadFromCoreServer( command.Content );
            if (!read.IsSuccess) return read;

            // 检查反馈
            OperateResult check = CheckResponse( read.Content );
            if (!check.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( check );

            // 提取数据
            return AllenBradleyHelper.ExtractActualData( read.Content, true );
        }


        /// <summary>
        /// 读取单个的bool数据信息
        /// </summary>
        /// <param name="address">节点数据信息</param>
        /// <returns>带有结果对象的结果数据</returns>
        public OperateResult<bool> ReadBool( string address )
        {
            OperateResult<byte[]> read = Read( address, 0 );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool>( read );

            return OperateResult.CreateSuccessResult( ByteTransform.TransBool( read.Content, 0 ) );
        }

        /// <summary>
        /// 批量读取的bool数据信息
        /// </summary>
        /// <param name="address">节点数据信息</param>
        /// <returns>带有结果对象的结果数据</returns>
        public OperateResult<bool[]> ReadBoolArray( string address )
        {
            OperateResult<byte[]> read = Read( address, 0 );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( read );

            return OperateResult.CreateSuccessResult( ByteTransform.TransBool( read.Content, 0, read.Content.Length ) );
        }

        /// <summary>
        /// 读取设备的byte类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<byte> ReadByte( string address )
        {
            OperateResult<byte[]> read = Read( address, 0 );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte>( read );

            return OperateResult.CreateSuccessResult( ByteTransform.TransByte( read.Content, 0 ) );
        }

        #endregion

        #region Device Override

        /// <summary>
        /// 读取设备的short类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadInt16Array" title="Int16类型示例" />
        /// </example>
        public override OperateResult<short[]> ReadInt16( string address, ushort length )
        {
            return ByteTransformHelper.GetResultFromBytes( Read( address, length ), m => ByteTransform.TransInt16( m, 0, length ) );
        }

        /// <summary>
        /// 读取设备的ushort类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadUInt16Array" title="UInt16类型示例" />
        /// </example>
        public override OperateResult<ushort[]> ReadUInt16( string address, ushort length )
        {
            return ByteTransformHelper.GetResultFromBytes( Read( address, length ), m => ByteTransform.TransUInt16( m, 0, length ) );
        }

        /// <summary>
        /// 读取设备的int类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadInt32Array" title="Int32类型示例" />
        /// </example>
        public override OperateResult<int[]> ReadInt32( string address, ushort length )
        {
            return ByteTransformHelper.GetResultFromBytes( Read( address, length ), m => ByteTransform.TransInt32( m, 0, length ) );
        }

        /// <summary>
        /// 读取设备的uint类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadUInt32Array" title="UInt32类型示例" />
        /// </example>
        public override OperateResult<uint[]> ReadUInt32( string address, ushort length )
        {
            return ByteTransformHelper.GetResultFromBytes( Read( address, length ), m => ByteTransform.TransUInt32( m, 0, length ) );
        }

        /// <summary>
        /// 读取设备的float类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadFloatArray" title="Float类型示例" />
        /// </example>
        public override OperateResult<float[]> ReadFloat( string address, ushort length )
        {
            return ByteTransformHelper.GetResultFromBytes( Read( address, length ), m => ByteTransform.TransSingle( m, 0, length ) );
        }

        /// <summary>
        /// 读取设备的long类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadInt64Array" title="Int64类型示例" />
        /// </example>
        public override OperateResult<long[]> ReadInt64( string address, ushort length )
        {
            return ByteTransformHelper.GetResultFromBytes( Read( address, length ), m => ByteTransform.TransInt64( m, 0, length ) );
        }

        /// <summary>
        /// 读取设备的ulong类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadUInt64Array" title="UInt64类型示例" />
        /// </example>
        public override OperateResult<ulong[]> ReadUInt64( string address, ushort length )
        {
            return ByteTransformHelper.GetResultFromBytes( Read( address, length ), m => ByteTransform.TransUInt64( m, 0, length ) );
        }

        /// <summary>
        /// 读取设备的double类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadDoubleArray" title="Double类型示例" />
        /// </example>
        public override OperateResult<double[]> ReadDouble( string address, ushort length )
        {
            return ByteTransformHelper.GetResultFromBytes( Read( address, length ), m => ByteTransform.TransDouble( m, 0, length ) );
        }

        #endregion

        #region Write Support

        /// <summary>
        /// 使用指定的类型写入指定的节点数据
        /// </summary>
        /// <param name="address">节点地址数据</param>
        /// <param name="typeCode">类型代码，详细参见<see cref="AllenBradleyHelper"/>上的常用字段</param>
        /// <param name="value">实际的数据值</param>
        /// <param name="length">如果节点是数组，就是数组长度</param>
        /// <returns>是否写入成功</returns>
        public OperateResult WriteTag( string address, ushort typeCode, byte[] value, int length = 1 )
        {
            OperateResult<byte[]> command = BuildWriteCommand( address, typeCode, value, length );
            if (!command.IsSuccess) return command;

            // 核心交互
            OperateResult<byte[]> read = ReadFromCoreServer( command.Content );
            if (!read.IsSuccess) return read;

            // 检查反馈
            OperateResult check = CheckResponse( read.Content );
            if (!check.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( check );

            // 提取写入结果
            return AllenBradleyHelper.ExtractActualData( read.Content, false );
        }

        #endregion

        #region Write Override

        /// <summary>
        /// 向设备中写入short数组，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="values">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteInt16Array" title="Int16类型示例" />
        /// </example>
        public override OperateResult Write( string address, short[] values )
        {
            return WriteTag( address, AllenBradleyHelper.CIP_Type_Word, ByteTransform.TransByte( values ), values.Length );
        }

        /// <summary>
        /// 向设备中写入ushort数组，返回是否写入成功
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteUInt16Array" title="UInt16类型示例" />
        /// </example>
        public override OperateResult Write( string address, ushort[] values )
        {
            return WriteTag( address, AllenBradleyHelper.CIP_Type_Word, ByteTransform.TransByte( values ), values.Length );
        }
        
        /// <summary>
        /// 向设备中写入int数组，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="values">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteInt32Array" title="Int32类型示例" />
        /// </example>
        public override OperateResult Write( string address, int[] values )
        {
            return WriteTag( address, AllenBradleyHelper.CIP_Type_DWord, ByteTransform.TransByte( values ), values.Length );
        }

        /// <summary>
        /// 向设备中写入uint数组，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="values">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteUInt32Array" title="UInt32类型示例" />
        /// </example>
        public override OperateResult Write( string address, uint[] values )
        {
            return WriteTag( address, AllenBradleyHelper.CIP_Type_DWord, ByteTransform.TransByte( values ), values.Length );
        }

        /// <summary>
        /// 向设备中写入float数组，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="values">实际数据</param>
        /// <returns>返回写入结果</returns>
        /// <example>
        /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteFloatArray" title="Float类型示例" />
        /// </example>
        public override OperateResult Write( string address, float[] values )
        {
            return WriteTag( address, AllenBradleyHelper.CIP_Type_Real, ByteTransform.TransByte( values ), values.Length );
        }
        

        /// <summary>
        /// 向设备中写入string数据，返回是否写入成功，该string类型是针对PLC的DINT类型，长度自动扩充到8
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">实际数据</param>
        /// <returns>返回写入结果</returns>
        public override OperateResult Write( string address, string value )
        {
            return WriteTag( address, AllenBradleyHelper.CIP_Type_DWord, BasicFramework.SoftBasic.ArrayExpandToLength( ByteTransform.TransByte( value, Encoding.ASCII ), 8 ) );
        }

        /// <summary>
        /// 向设备中写入bool数据，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, bool value )
        {
            return WriteTag( address, AllenBradleyHelper.CIP_Type_Bool, value ? new byte[] { 0xFF, 0xFF } : new byte[] { 0x00, 0x00 } );
        }



        /// <summary>
        /// 向设备中写入byte数据，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, byte value )
        {
            return WriteTag( address, AllenBradleyHelper.CIP_Type_Byte, new byte[] { value, 0x00 } );
        }

        #endregion

        #region Handle Single

        /// <summary>
        /// 向PLC注册会话ID的报文
        /// </summary>
        /// <returns>报文信息</returns>
        public byte[] RegisterSessionHandle( )
        {
            byte[] commandSpecificData = new byte[] { 0x01, 0x00, 0x00, 0x00, };
            return AllenBradleyHelper.PackRequestHeader( 0x65, 0, commandSpecificData );
        }

        /// <summary>
        /// 获取卸载一个已注册的会话的报文
        /// </summary>
        /// <returns>字节报文信息</returns>
        public byte[] UnRegisterSessionHandle( )
        {
            return AllenBradleyHelper.PackRequestHeader( 0x66, SessionHandle, new byte[0] );
        }

        

        private OperateResult CheckResponse( byte[] response )
        {
            try
            {
                int status = ByteTransform.TransInt32( response, 8 );
                if (status == 0) return OperateResult.CreateSuccessResult( );

                string msg = string.Empty;
                switch (status)
                {
                    case 0x01: msg = StringResources.Language.AllenBradleySessionStatus01;break;
                    case 0x02: msg = StringResources.Language.AllenBradleySessionStatus02;break;
                    case 0x03: msg = StringResources.Language.AllenBradleySessionStatus03;break;
                    case 0x64: msg = StringResources.Language.AllenBradleySessionStatus64;break;
                    case 0x65: msg = StringResources.Language.AllenBradleySessionStatus65;break;
                    case 0x69: msg = StringResources.Language.AllenBradleySessionStatus69;break;
                    default: msg = StringResources.Language.UnknownError;break;
                }

                return new OperateResult( status, msg );
            }
            catch(Exception ex)
            {
                return new OperateResult( ex.Message );
            }
        }

        #endregion
        
    }
}
