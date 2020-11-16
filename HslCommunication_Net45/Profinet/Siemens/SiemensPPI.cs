using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Serial;
#if Net45
using System.Threading.Tasks;
#endif

namespace HslCommunication.Profinet.Siemens
{
    /// <summary>
    /// 西门子的PPI协议，适用于s7-200plc，注意，本类库有个致命的风险需要注意，由于本类库的每次通讯分成2次操作，故而不支持多线程同时读写，当发生线程竞争的时候，会导致数据异常，
    /// 想要解决的话，需要您在每次数据交互时添加同步锁。
    /// </summary>
    /// <remarks>
    /// 适用于西门子200的通信，非常感谢 合肥-加劲 的测试，让本类库圆满完成。
    /// 
    /// 注意：M地址范围有限 0-31地址
    /// </remarks>
    public class SiemensPPI : SerialDeviceBase<HslCommunication.Core.ReverseBytesTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化一个西门子的PPI协议对象
        /// </summary>
        public SiemensPPI( )
        {
            WordLength = 2;
        }

        #endregion


        /// <summary>
        /// 西门子PLC的站号信息
        /// </summary>
        public byte Station { get => station;
            set {
                station = value;
                executeConfirm[1] = value;

                int count = 0;
                for (int i = 1; i < 4; i++)
                {
                    count += executeConfirm[i];
                }
                executeConfirm[4] = (byte)count;
            }
        }


        /// <summary>
        /// 从西门子的PLC中读取数据信息，地址为"M100","AI100","I0","Q0","V100","S100"等，详细请参照API文档
        /// </summary>
        /// <param name="address">西门子的地址数据信息</param>
        /// <param name="length">数据长度</param>
        /// <returns>带返回结果的结果对象</returns>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            // 解析指令
            OperateResult<byte[]> command = BuildReadCommand( station, address, length, false );
            if (!command.IsSuccess) return command;

            // 第一次数据交互
            OperateResult<byte[]> read1 = ReadBase( command.Content );
            if (!read1.IsSuccess) return read1;

            // 验证
            if (read1.Content[0] != 0xE5) return new OperateResult<byte[]>( "PLC Receive Check Failed:" + BasicFramework.SoftBasic.ByteToHexString( read1.Content, ' ' ) );

            // 第二次数据交互
            OperateResult<byte[]> read2 = ReadBase( executeConfirm );
            if (!read2.IsSuccess) return read2;

            // 错误码判断
            if (read2.Content.Length < 21) return new OperateResult<byte[]>( read2.ErrorCode, "Failed: " + BasicFramework.SoftBasic.ByteToHexString( read2.Content, ' ' ) );
            if (read2.Content[17] != 0x00 || read2.Content[18] != 0x00) return new OperateResult<byte[]>( read2.Content[19], GetMsgFromStatus( read2.Content[18], read2.Content[19] ) );
            if (read2.Content[21] != 0xFF) return new OperateResult<byte[]>( read2.Content[21], GetMsgFromStatus( read2.Content[21] ) );

            // 数据提取
            byte[] buffer = new byte[length];
            if (read2.Content[21] == 0xFF && read2.Content[22] == 0x04)
            {
                Array.Copy( read2.Content, 25, buffer, 0, length );
            }
            return OperateResult.CreateSuccessResult( buffer );
        }

        /// <summary>
        /// 从西门子的PLC中读取bool数据信息，地址为"M100.0","AI100.1","I0.3","Q0.6","V100.4","S100"等，详细请参照API文档
        /// </summary>
        /// <param name="address">西门子的地址数据信息</param>
        /// <param name="length">数据长度</param>
        /// <returns>带返回结果的结果对象</returns>
        public OperateResult<bool[]> ReadBool( string address, ushort length )
        {
            // 解析指令
            OperateResult<byte[]> command = BuildReadCommand( station, address, length, true );
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( command );

            // 第一次数据交互
            OperateResult<byte[]> read1 = ReadBase( command.Content );
            if (!read1.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( read1 );

            // 验证
            if (read1.Content[0] != 0xE5) return new OperateResult<bool[]>( "PLC Receive Check Failed:" + BasicFramework.SoftBasic.ByteToHexString( read1.Content, ' ' ) );

            // 第二次数据交互
            OperateResult<byte[]> read2 = ReadBase( executeConfirm );
            if (!read2.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( read2 );

            // 错误码判断
            if (read2.Content.Length < 21) return new OperateResult<bool[]>( read2.ErrorCode, "Failed: " + BasicFramework.SoftBasic.ByteToHexString( read2.Content, ' ' ) );
            if (read2.Content[17] != 0x00 || read2.Content[18] != 0x00) return new OperateResult<bool[]>( read2.Content[19], GetMsgFromStatus( read2.Content[18], read2.Content[19] ) );
            if (read2.Content[21] != 0xFF) return new OperateResult<bool[]>( read2.Content[21], GetMsgFromStatus( read2.Content[21] ) );
            // 数据提取

            byte[] buffer = new byte[read2.Content.Length - 27];
            if (read2.Content[21] == 0xFF && read2.Content[22] == 0x03)
            {
                Array.Copy( read2.Content, 25, buffer, 0, buffer.Length );
            }
            return OperateResult.CreateSuccessResult( BasicFramework.SoftBasic.ByteToBoolArray( buffer, length ) );
        }

        /// <summary>
        /// 从西门子的PLC中读取bool数据信息，地址为"M100.0","AI100.1","I0.3","Q0.6","V100.4","S100"等，详细请参照API文档
        /// </summary>
        /// <param name="address">西门子的地址数据信息</param>
        /// <returns>带返回结果的结果对象</returns>
        public OperateResult<bool> ReadBool( string address )
        {
            OperateResult<bool[]> read = ReadBool( address, 1 );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool>( read );

            return OperateResult.CreateSuccessResult( read.Content[0] );
        }


        /// <summary>
        /// 将字节数据写入到西门子PLC中，地址为"M100.0","AI100.1","I0.3","Q0.6","V100.4","S100"等，详细请参照API文档
        /// </summary>
        /// <param name="address">西门子的地址数据信息</param>
        /// <param name="value">数据长度</param>
        public override OperateResult Write( string address, byte[] value )
        {
            // 解析指令
            OperateResult<byte[]> command = BuildWriteCommand( station, address, value );
            if (!command.IsSuccess) return command;

            // 第一次数据交互
            OperateResult<byte[]> read1 = ReadBase( command.Content );
            if (!read1.IsSuccess) return read1;

            // 验证
            if (read1.Content[0] != 0xE5) return new OperateResult<byte[]>( "PLC Receive Check Failed:" + read1.Content[0] );

            // 第二次数据交互
            OperateResult<byte[]> read2 = ReadBase( executeConfirm );
            if (!read2.IsSuccess) return read2;

            // 错误码判断
            if (read2.Content.Length < 21) return new OperateResult( read2.ErrorCode, "Failed: " + BasicFramework.SoftBasic.ByteToHexString( read2.Content, ' ' ) );
            if (read2.Content[17] != 0x00 || read2.Content[18] != 0x00) return new OperateResult( read2.Content[19], GetMsgFromStatus( read2.Content[18], read2.Content[19] ) );
            if (read2.Content[21] != 0xFF) return new OperateResult( read2.Content[21], GetMsgFromStatus( read2.Content[21] ) );
            // 数据提取
            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 将bool数据写入到西门子PLC中，地址为"M100.0","AI100.1","I0.3","Q0.6","V100.4","S100"等，详细请参照API文档
        /// </summary>
        /// <param name="address">西门子的地址数据信息</param>
        /// <param name="value">数据长度</param>
        public OperateResult Write(string address, bool[] value )
        {
            // 解析指令
            OperateResult<byte[]> command = BuildWriteCommand( station, address, value );
            if (!command.IsSuccess) return command;

            // 第一次数据交互
            OperateResult<byte[]> read1 = ReadBase( command.Content );
            if (!read1.IsSuccess) return read1;

            // 验证
            if (read1.Content[0] != 0xE5) return new OperateResult<byte[]>( "PLC Receive Check Failed:" + read1.Content[0] );
            
            // 第二次数据交互
            OperateResult<byte[]> read2 = ReadBase( executeConfirm );
            if (!read2.IsSuccess) return read2;

            // 错误码判断
            if (read2.Content.Length < 21) return new OperateResult( read2.ErrorCode, "Failed: " + BasicFramework.SoftBasic.ByteToHexString( read2.Content, ' ' ) );
            if (read2.Content[17] != 0x00 || read2.Content[18] != 0x00) return new OperateResult( read2.Content[19], GetMsgFromStatus( read2.Content[18], read2.Content[19] ) );
            if (read2.Content[21] != 0xFF) return new OperateResult( read2.Content[21], GetMsgFromStatus( read2.Content[21] ) );
            // 数据提取
            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 将bool数据写入到西门子PLC中，地址为"M100.0","AI100.1","I0.3","Q0.6","V100.4","S100"等，详细请参照API文档
        /// </summary>
        /// <param name="address">西门子的地址数据信息</param>
        /// <param name="value">数据长度</param>
        public OperateResult Write( string address, bool value )
        {
            return Write( address, new bool[] { value } );
        }

        #region Byte Read Write

        /// <summary>
        /// 从西门子的PLC中读取byte数据信息，地址为"M100.0","AI100.1","I0.3","Q0.6","V100.4","S100"等，详细请参照API文档
        /// </summary>
        /// <param name="address">西门子的地址数据信息</param>
        /// <returns>带返回结果的结果对象</returns>
        public OperateResult<byte> ReadByte( string address )
        {
            OperateResult<byte[]> read = Read( address, 1 );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte>( read );

            return OperateResult.CreateSuccessResult( read.Content[0] );
        }

        /// <summary>
        /// 将byte数据写入到西门子PLC中，地址为"M100.0","AI100.1","I0.3","Q0.6","V100.4","S100"等，详细请参照API文档
        /// </summary>
        /// <param name="address">西门子的地址数据信息</param>
        /// <param name="value">数据长度</param>
        public OperateResult WriteByte(string address, byte value )
        {
            return Write( address, new byte[] { value } );
        }


        #endregion

        #region Start Stop

        /// <summary>
        /// 启动西门子PLC为RUN模式
        /// </summary>
        /// <returns>是否启动成功</returns>
        public OperateResult Start( )
        {
            byte[] cmd = new byte[] { 0x68, 0x21, 0x21, 0x68, station, 0x00, 0x6C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFD, 0x00, 0x00, 0x09, 0x50, 0x5F, 0x50, 0x52, 0x4F, 0x47, 0x52, 0x41, 0x4D, 0xAA, 0x16 };
            // 第一次数据交互
            OperateResult<byte[]> read1 = ReadBase( cmd );
            if (!read1.IsSuccess) return read1;

            // 验证
            if (read1.Content[0] != 0xE5) return new OperateResult<byte[]>( "PLC Receive Check Failed:" + read1.Content[0] );

            // 第二次数据交互
            OperateResult<byte[]> read2 = ReadBase( executeConfirm );
            if (!read2.IsSuccess) return read2;

            // 数据提取
            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 停止西门子PLC，切换为Stop模式
        /// </summary>
        /// <returns>是否停止成功</returns>
        public OperateResult Stop( )
        {
            byte[] cmd = new byte[] { 0x68, 0x1D, 0x1D, 0x68, station, 0x00, 0x6C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x29, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x50, 0x5F, 0x50, 0x52, 0x4F, 0x47, 0x52, 0x41, 0x4D, 0xAA, 0x16 };
            // 第一次数据交互
            OperateResult<byte[]> read1 = ReadBase( cmd );
            if (!read1.IsSuccess) return read1;

            // 验证
            if (read1.Content[0] != 0xE5) return new OperateResult<byte[]>( "PLC Receive Check Failed:" + read1.Content[0] );

            // 第二次数据交互
            OperateResult<byte[]> read2 = ReadBase( executeConfirm );
            if (!read2.IsSuccess) return read2;

            // 数据提取
            return OperateResult.CreateSuccessResult( );
        }

        #endregion

        #region Private Member

        private byte station = 0x02;            // PLC的站号信息
        private byte[] executeConfirm = new byte[] { 0x10, 0x02, 0x00, 0x5C, 0x5E, 0x16 };

        #endregion

        #region Static Helper


        /// <summary>
        /// 解析数据地址，解析出地址类型，起始地址，DB块的地址 ->
        /// Parse data address, parse out address type, start address, db block address
        /// </summary>
        /// <param name="address">起始地址，例如M100，I0，Q0，V100 ->
        /// Start address, such as M100,I0,Q0,V100</param>
        /// <returns>解析数据地址，解析出地址类型，起始地址，DB块的地址 ->
        /// Parse data address, parse out address type, start address, db block address</returns>
        private static OperateResult<byte, int, ushort> AnalysisAddress( string address )
        {
            var result = new OperateResult<byte, int, ushort>( );
            try
            {
                result.Content3 = 0;
                if(address.Substring(0,2) == "AI")
                {
                    result.Content1 = 0x06;
                    result.Content2 = SiemensS7Net.CalculateAddressStarted( address.Substring( 2 ) );
                }
                else if (address.Substring( 0, 2 ) == "AQ")
                {
                    result.Content1 = 0x07;
                    result.Content2 = SiemensS7Net.CalculateAddressStarted( address.Substring( 2 ) );
                }
                else if (address[0] == 'T')
                {
                    result.Content1 = 0x1F;
                    result.Content2 = SiemensS7Net.CalculateAddressStarted( address.Substring( 1 ) );
                }
                else if (address[0] == 'C')
                {
                    result.Content1 = 0x1E;
                    result.Content2 = SiemensS7Net.CalculateAddressStarted( address.Substring( 1 ) );
                }
                else if (address.Substring( 0, 2 ) == "SM")
                {
                    result.Content1 = 0x05;
                    result.Content2 = SiemensS7Net.CalculateAddressStarted( address.Substring( 2 ) );
                }
                else if (address[0] == 'S')
                {
                    result.Content1 = 0x04;
                    result.Content2 = SiemensS7Net.CalculateAddressStarted( address.Substring( 1 ) );
                }
                else if (address[0] == 'I')
                {
                    result.Content1 = 0x81;
                    result.Content2 = SiemensS7Net.CalculateAddressStarted( address.Substring( 1 ) );
                }
                else if (address[0] == 'Q')
                {
                    result.Content1 = 0x82;
                    result.Content2 = SiemensS7Net.CalculateAddressStarted( address.Substring( 1 ) );
                }
                else if (address[0] == 'M')
                {
                    result.Content1 = 0x83;
                    result.Content2 = SiemensS7Net.CalculateAddressStarted( address.Substring( 1 ) );
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

                    result.Content2 = SiemensS7Net.CalculateAddressStarted( address.Substring( address.IndexOf( '.' ) + 1 ) );
                }
                else if (address[0] == 'V')
                {
                    result.Content1 = 0x84;
                    result.Content3 = 1;
                    result.Content2 = SiemensS7Net.CalculateAddressStarted( address.Substring( 1 ) );
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

        /// <summary>
        /// 生成一个读取字数据指令头的通用方法 ->
        /// A general method for generating a command header to read a Word data
        /// </summary>
        /// <param name="station">设备的站号信息 -> Station number information for the device</param>
        /// <param name="address">起始地址，例如M100，I0，Q0，V100 ->
        /// Start address, such as M100,I0,Q0,V100</param>
        /// <param name="length">读取数据长度 -> Read Data length</param>
        /// <param name="isBit">是否为位读取</param>
        /// <returns>包含结果对象的报文 -> Message containing the result object</returns>
        public static OperateResult<byte[]> BuildReadCommand( byte station, string address, ushort length, bool isBit )
        {
            OperateResult<byte, int, ushort> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            byte[] _PLCCommand = new byte[33];
            _PLCCommand[ 0] = 0x68;
            _PLCCommand[ 1] = BitConverter.GetBytes( _PLCCommand.Length - 6 )[0];
            _PLCCommand[ 2] = BitConverter.GetBytes( _PLCCommand.Length - 6 )[0];
            _PLCCommand[ 3] = 0x68;
            _PLCCommand[ 4] = station;
            _PLCCommand[ 5] = 0x00;
            _PLCCommand[ 6] = 0x6C;
            _PLCCommand[ 7] = 0x32;
            _PLCCommand[ 8] = 0x01;
            _PLCCommand[ 9] = 0x00;
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x00;
            _PLCCommand[12] = 0x00;
            _PLCCommand[13] = 0x00;
            _PLCCommand[14] = 0x0E;
            _PLCCommand[15] = 0x00;
            _PLCCommand[16] = 0x00;
            _PLCCommand[17] = 0x04;
            _PLCCommand[18] = 0x01;
            _PLCCommand[19] = 0x12;
            _PLCCommand[20] = 0x0A;
            _PLCCommand[21] = 0x10;

            _PLCCommand[22] = isBit ? (byte)0x01 : (byte)0x02;
            _PLCCommand[23] = 0x00;
            _PLCCommand[24] = BitConverter.GetBytes( length )[0];
            _PLCCommand[25] = BitConverter.GetBytes( length )[1];
            _PLCCommand[26] = (byte)analysis.Content3;
            _PLCCommand[27] = analysis.Content1;
            _PLCCommand[28] = BitConverter.GetBytes( analysis.Content2 )[2];
            _PLCCommand[29] = BitConverter.GetBytes( analysis.Content2 )[1];
            _PLCCommand[30] = BitConverter.GetBytes( analysis.Content2 )[0];

            int count = 0;
            for(int i = 4; i< 31; i++)
            {
                count += _PLCCommand[i];
            }
            _PLCCommand[31] = BitConverter.GetBytes( count )[0];
            _PLCCommand[32] = 0x16;

            return OperateResult.CreateSuccessResult( _PLCCommand );
        }

        

        /// <summary>
        /// 生成一个写入PLC数据信息的报文内容
        /// </summary>
        /// <param name="station">PLC的站号</param>
        /// <param name="address">地址</param>
        /// <param name="values">数据值</param>
        /// <returns>是否写入成功</returns>
        public static OperateResult<byte[]> BuildWriteCommand( byte station, string address, byte[] values )
        {
            OperateResult<byte, int, ushort> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            int length = values.Length;
            // 68 21 21 68 02 00 6C 32 01 00 00 00 00 00 0E 00 00 04 01 12 0A 10
            byte[] _PLCCommand = new byte[37 + values.Length];
            _PLCCommand[ 0] = 0x68;
            _PLCCommand[ 1] = BitConverter.GetBytes( _PLCCommand.Length - 6 )[0];
            _PLCCommand[ 2] = BitConverter.GetBytes( _PLCCommand.Length - 6 )[0];
            _PLCCommand[ 3] = 0x68;
            _PLCCommand[ 4] = station;
            _PLCCommand[ 5] = 0x00;
            _PLCCommand[ 6] = 0x7C;
            _PLCCommand[ 7] = 0x32;
            _PLCCommand[ 8] = 0x01;
            _PLCCommand[ 9] = 0x00;
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x00;
            _PLCCommand[12] = 0x00;
            _PLCCommand[13] = 0x00;
            _PLCCommand[14] = 0x0E;
            _PLCCommand[15] = 0x00;
            _PLCCommand[16] = (byte)(values.Length + 4);
            _PLCCommand[17] = 0x05;
            _PLCCommand[18] = 0x01;
            _PLCCommand[19] = 0x12;
            _PLCCommand[20] = 0x0A;
            _PLCCommand[21] = 0x10;

            _PLCCommand[22] = 0x02;
            _PLCCommand[23] = 0x00;
            _PLCCommand[24] = BitConverter.GetBytes( length )[0];
            _PLCCommand[25] = BitConverter.GetBytes( length )[1];
            _PLCCommand[26] = (byte)analysis.Content3;
            _PLCCommand[27] = analysis.Content1;
            _PLCCommand[28] = BitConverter.GetBytes( analysis.Content2 )[2];
            _PLCCommand[29] = BitConverter.GetBytes( analysis.Content2 )[1];
            _PLCCommand[30] = BitConverter.GetBytes( analysis.Content2 )[0];

            _PLCCommand[31] = 0x00;
            _PLCCommand[32] = 0x04;
            _PLCCommand[33] = BitConverter.GetBytes( length * 8 )[1];
            _PLCCommand[34] = BitConverter.GetBytes( length * 8 )[0];


            values.CopyTo( _PLCCommand, 35 );

            int count = 0;
            for (int i = 4; i < _PLCCommand.Length - 2; i++)
            {
                count += _PLCCommand[i];
            }
            _PLCCommand[_PLCCommand.Length - 2] = BitConverter.GetBytes( count )[0];
            _PLCCommand[_PLCCommand.Length - 1] = 0x16;


            return OperateResult.CreateSuccessResult( _PLCCommand );
        }

        /// <summary>
        /// 根据错误信息，获取到文本信息
        /// </summary>
        /// <param name="code">状态</param>
        /// <returns>消息文本</returns>
        public static string GetMsgFromStatus( byte code )
        {
            switch (code)
            {
                case 0xFF: return "No error";
                case 0x01: return "Hardware fault";
                case 0x03: return "Illegal object access";
                case 0x05: return "Invalid address(incorrent variable address)";
                case 0x06: return "Data type is not supported";
                case 0x0A: return "Object does not exist or length error";
                default: return StringResources.Language.UnknownError;
            }
        }

        /// <summary>
        /// 根据错误信息，获取到文本信息
        /// </summary>
        /// <param name="errorClass">错误类型</param>
        /// <param name="errorCode">错误代码</param>
        /// <returns>错误信息</returns>
        public static string GetMsgFromStatus( byte errorClass, byte errorCode )
        {
            if(errorClass == 0x80 && errorCode == 0x01)
            {
                return "Switch in wrong position for requested operation";
            }
            else if (errorClass == 0x81 && errorCode == 0x04)
            {
                return "Miscellaneous structure error in command.  Command is not supportedby CPU";
            }
            else if (errorClass == 0x84 && errorCode == 0x04)
            {
                return "CPU is busy processing an upload or download CPU cannot process command because of system fault condition";
            }
            else if (errorClass == 0x85 && errorCode == 0x00)
            {
                return "Length fields are not correct or do not agree with the amount of data received";
            }
            else if (errorClass == 0xD2 )
            {
                return "Error in upload or download command";
            }
            else if (errorClass == 0xD6)
            {
                return "Protection error(password)";
            }
            else if (errorClass == 0xDC && errorCode == 0x01)
            {
                return "Error in time-of-day clock data";
            }
            else
            {
                return StringResources.Language.UnknownError;
            }
        }

        /// <summary>
        /// 创建写入PLC的bool类型数据报文指令
        /// </summary>
        /// <param name="station">PLC的站号信息</param>
        /// <param name="address">地址信息</param>
        /// <param name="values">bool[]数据值</param>
        /// <returns>带有成功标识的结果对象</returns>
        public static OperateResult<byte[]> BuildWriteCommand( byte station, string address, bool[] values )
        {
            OperateResult<byte, int, ushort> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            byte[] bytesValue = BasicFramework.SoftBasic.BoolArrayToByte( values );
            // 68 21 21 68 02 00 6C 32 01 00 00 00 00 00 0E 00 00 04 01 12 0A 10
            byte[] _PLCCommand = new byte[37 + bytesValue.Length];
            _PLCCommand[ 0] = 0x68;
            _PLCCommand[ 1] = BitConverter.GetBytes( _PLCCommand.Length - 6 )[0];
            _PLCCommand[ 2] = BitConverter.GetBytes( _PLCCommand.Length - 6 )[0];
            _PLCCommand[ 3] = 0x68;
            _PLCCommand[ 4] = station;
            _PLCCommand[ 5] = 0x00;
            _PLCCommand[ 6] = 0x7C;
            _PLCCommand[ 7] = 0x32;
            _PLCCommand[ 8] = 0x01;
            _PLCCommand[ 9] = 0x00;
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x00;
            _PLCCommand[12] = 0x00;
            _PLCCommand[13] = 0x00;
            _PLCCommand[14] = 0x0E;
            _PLCCommand[15] = 0x00;
            _PLCCommand[16] = 0x05;
            _PLCCommand[17] = 0x05;
            _PLCCommand[18] = 0x01;
            _PLCCommand[19] = 0x12;
            _PLCCommand[20] = 0x0A;
            _PLCCommand[21] = 0x10;

            _PLCCommand[22] = 0x01;
            _PLCCommand[23] = 0x00;
            _PLCCommand[24] = BitConverter.GetBytes( values.Length )[0];
            _PLCCommand[25] = BitConverter.GetBytes( values.Length )[1];
            _PLCCommand[26] = (byte)analysis.Content3;
            _PLCCommand[27] = analysis.Content1;
            _PLCCommand[28] = BitConverter.GetBytes( analysis.Content2 )[2];
            _PLCCommand[29] = BitConverter.GetBytes( analysis.Content2 )[1];
            _PLCCommand[30] = BitConverter.GetBytes( analysis.Content2 )[0];

            _PLCCommand[31] = 0x00;
            _PLCCommand[32] = 0x03;
            _PLCCommand[33] = BitConverter.GetBytes( values.Length )[1];
            _PLCCommand[34] = BitConverter.GetBytes( values.Length )[0];


            bytesValue.CopyTo( _PLCCommand, 35 );

            int count = 0;
            for (int i = 4; i < _PLCCommand.Length - 2; i++)
            {
                count += _PLCCommand[i];
            }
            _PLCCommand[_PLCCommand.Length - 2] = BitConverter.GetBytes( count )[0];
            _PLCCommand[_PLCCommand.Length - 1] = 0x16;


            return OperateResult.CreateSuccessResult( _PLCCommand );
        }

        #endregion
    }
}
