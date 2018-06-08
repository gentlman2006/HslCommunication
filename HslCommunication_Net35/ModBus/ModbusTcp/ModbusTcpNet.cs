using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;

namespace HslCommunication.ModBus
{


    /// <summary>
    /// Modbus-Tcp协议的客户端通讯类，方便的和服务器进行数据交互
    /// </summary>
    public class ModbusTcpNet : NetworkDeviceBase<ModbusTcpMessage, ReverseWordTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化一个MOdbus-Tcp协议的客户端对象
        /// </summary>
        public ModbusTcpNet( )
        {
            softIncrementCount = new SoftIncrementCount( ushort.MaxValue );
            WordLength = 1;
        }


        /// <summary>
        /// 指定服务器地址，端口号，客户端自己的站号来初始化
        /// </summary>
        /// <param name="ipAddress">服务器的Ip地址</param>
        /// <param name="port">服务器的端口号</param>
        /// <param name="station">客户端自身的站号</param>
        public ModbusTcpNet( string ipAddress, int port = 502, byte station = 0x01 )
        {
            softIncrementCount = new SoftIncrementCount( ushort.MaxValue );
            IpAddress = ipAddress;
            Port = port;
            WordLength = 1;
            this.station = station;
        }

        #endregion

        #region Private Member

        private byte station = 0x01;                                // 本客户端的站号
        private SoftIncrementCount softIncrementCount;              // 自增消息的对象
        private bool isAddressStartWithZero = true;                 // 线圈值的地址值是否从零开始

        #endregion

        #region Public Member

        /// <summary>
        /// 获取或设置起始的地址是否从0开始，默认为True
        /// </summary>
        public bool AddressStartWithZero
        {
            get { return isAddressStartWithZero; }
            set { isAddressStartWithZero = value; }
        }

        /// <summary>
        /// 获取或者重新修改服务器的站号信息
        /// </summary>
        public byte Station
        {
            get { return station; }
            set { station = value; }
        }

        /// <summary>
        /// 多字节的数据是否高低位反转，常用于Int32,UInt32,float,double,Int64,UInt64类型读写
        /// </summary>
        public bool IsMultiWordReverse
        {
            get { return ByteTransform.IsMultiWordReverse; }
            set { ByteTransform.IsMultiWordReverse = value; }
        }

        /// <summary>
        /// 字符串数据是否按照字来反转
        /// </summary>
        public bool IsStringReverse
        {
            get { return ByteTransform.IsStringReverse; }
            set { ByteTransform.IsStringReverse = value; }
        }

        #endregion

        #region Address Analysis

        /// <summary>
        /// 解析数据地址，解析出地址类型，起始地址
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns>解析出地址类型，起始地址，DB块的地址</returns>
        private OperateResult<int> AnalysisAddress( string address )
        {
            try
            {
                int add = Convert.ToInt32( address );
                add = CheckAddressStartWithZero( add );
                return OperateResult.CreateSuccessResult( add );
            }
            catch (Exception ex)
            {
                return new OperateResult<int>( )
                {
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 解析数据地址，解析出地址类型，起始地址
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns>解析出地址类型，起始地址，DB块的地址</returns>
        private OperateResult<byte, int> AnalysisReadAddress( string address )
        {
            try
            {
                if (address.IndexOf( 'X' ) < 0)
                {
                    // 正常地址，功能码03
                    int add = Convert.ToInt32( address );
                    return OperateResult.CreateSuccessResult( ModbusInfo.ReadRegister, add );
                }
                else
                {
                    // 带功能码的地址
                    string[] list = address.Split( 'X' );
                    byte function = byte.Parse( list[0] );
                    int add = Convert.ToInt32( list[1] );
                    return OperateResult.CreateSuccessResult( function, add );
                }
            }
            catch (Exception ex)
            {
                return new OperateResult<byte, int>( )
                {
                    Message = ex.Message
                };
            }
        }

        private int CheckAddressStartWithZero( int add )
        {
            if (isAddressStartWithZero)
            {
                if (add < 0)
                {
                    throw new Exception( "地址值必须大于等于0" );
                }
            }
            else
            {
                if (add < 1)
                {
                    throw new Exception( "地址值必须大于等于1" );
                }

                add--;
            }
            return add;
        }


        #endregion

        #region Build Command


        /// <summary>
        /// 读取数据的基础指令，需要指定指令码，地址，长度
        /// </summary>
        /// <param name="code"></param>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private OperateResult<byte[]> BuildReadCommandBase( byte code, string address, ushort count )
        {
            OperateResult<int> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            ushort messageId = (ushort)softIncrementCount.GetCurrentValue( );
            byte[] buffer = new byte[12];
            buffer[0] = (byte)(messageId / 256);
            buffer[1] = (byte)(messageId % 256);
            buffer[2] = 0x00;
            buffer[3] = 0x00;
            buffer[4] = 0x00;
            buffer[5] = 0x06;
            buffer[6] = station;
            buffer[7] = code;
            buffer[8] = (byte)(analysis.Content / 256);
            buffer[9] = (byte)(analysis.Content % 256);
            buffer[10] = (byte)(count / 256);
            buffer[11] = (byte)(count % 256);
            
            return OperateResult.CreateSuccessResult( buffer );
        }



        /// <summary>
        /// 生成一个读取线圈的指令头
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="count">长度</param>
        /// <returns>携带有命令字节</returns>
        private OperateResult<byte[]> BuildReadCoilCommand( string address, ushort count )
        {
            return BuildReadCommandBase( ModbusInfo.ReadCoil, address, count );
        }

        /// <summary>
        /// 生成一个读取离散信息的指令头
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="count">长度</param>
        /// <returns>携带有命令字节</returns>
        private OperateResult<byte[]> BuildReadDiscreteCommand( string address, ushort count )
        {
            return BuildReadCommandBase( ModbusInfo.ReadDiscrete, address, count );
        }




        /// <summary>
        /// 生成一个读取寄存器的指令头
        /// </summary>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns>携带有命令字节</returns>
        private OperateResult<byte[]> BuildReadRegisterCommand( string address, ushort count )
        {
            return BuildReadCommandBase( ModbusInfo.ReadRegister, address, count );
        }



        private OperateResult<byte[]> BuildWriteOneCoilCommand( string address, bool value )
        {
            OperateResult<int> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            ushort messageId = (ushort)softIncrementCount.GetCurrentValue( );
            byte[] buffer = new byte[12];
            buffer[0] = (byte)(messageId / 256);
            buffer[1] = (byte)(messageId % 256);
            buffer[2] = 0x00;
            buffer[3] = 0x00;
            buffer[4] = 0x00;
            buffer[5] = 0x06;
            buffer[6] = station;
            buffer[7] = ModbusInfo.WriteOneCoil;
            buffer[8] = (byte)(analysis.Content / 256);
            buffer[9] = (byte)(analysis.Content % 256);
            buffer[10] = (byte)(value ? 0xFF : 0x00);
            buffer[11] = 0x00;
            
            return OperateResult.CreateSuccessResult( buffer );
        }





        private OperateResult<byte[]> BuildWriteOneRegisterCommand( string address, byte[] data )
        {
            OperateResult<int> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            ushort messageId = (ushort)softIncrementCount.GetCurrentValue( );
            byte[] buffer = new byte[12];
            buffer[0] = (byte)(messageId / 256);
            buffer[1] = (byte)(messageId % 256);
            buffer[2] = 0x00;
            buffer[3] = 0x00;
            buffer[4] = 0x00;
            buffer[5] = 0x06;
            buffer[6] = station;
            buffer[7] = ModbusInfo.WriteOneRegister;
            buffer[8] = (byte)(analysis.Content / 256);
            buffer[9] = (byte)(analysis.Content % 256);
            buffer[10] = data[1];
            buffer[11] = data[0];

            return OperateResult.CreateSuccessResult( buffer );
        }



        private OperateResult<byte[]> BuildWriteCoilCommand( string address, bool[] values )
        {
            byte[] data = SoftBasic.BoolArrayToByte( values );

            OperateResult<int> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            ushort messageId = (ushort)softIncrementCount.GetCurrentValue( );
            byte[] buffer = new byte[13 + data.Length];
            buffer[0] = (byte)(messageId / 256);
            buffer[1] = (byte)(messageId % 256);
            buffer[2] = 0x00;
            buffer[3] = 0x00;
            buffer[4] = (byte)((buffer.Length - 6) / 256);
            buffer[5] = (byte)((buffer.Length - 6) % 256);
            buffer[6] = station;
            buffer[7] = ModbusInfo.WriteCoil;
            buffer[8] = (byte)(analysis.Content / 256);
            buffer[9] = (byte)(analysis.Content % 256);
            buffer[10] = (byte)(values.Length / 256);
            buffer[11] = (byte)(values.Length % 256);

            buffer[12] = (byte)(data.Length);
            data.CopyTo( buffer, 13 );

            return OperateResult.CreateSuccessResult( buffer );
        }


        private OperateResult<byte[]> BuildWriteRegisterCommand( string address, byte[] data )
        {
            OperateResult<int> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            ushort messageId = (ushort)softIncrementCount.GetCurrentValue( );
            byte[] buffer = new byte[13 + data.Length];
            buffer[0] = (byte)(messageId / 256);
            buffer[1] = (byte)(messageId % 256);
            buffer[2] = 0x00;
            buffer[3] = 0x00;
            buffer[4] = (byte)((buffer.Length - 6) / 256);
            buffer[5] = (byte)((buffer.Length - 6) % 256);
            buffer[6] = station;
            buffer[7] = ModbusInfo.WriteRegister;
            buffer[8] = (byte)(analysis.Content / 256);
            buffer[9] = (byte)(analysis.Content % 256);
            buffer[10] = (byte)(data.Length / 2 / 256);
            buffer[11] = (byte)(data.Length / 2 % 256);

            buffer[12] = (byte)(data.Length);
            data.CopyTo( buffer, 13 );
            return OperateResult.CreateSuccessResult( buffer );
        }



        #endregion

        #region Core Interative

        /// <summary>
        /// 通过错误码来获取到对应的文本消息
        /// </summary>
        /// <param name="code">错误码</param>
        /// <returns>错误的文本描述</returns>
        private string GetDescriptionByErrorCode( byte code )
        {
            switch (code)
            {
                case ModbusInfo.FunctionCodeNotSupport: return StringResources.ModbusTcpFunctionCodeNotSupport;
                case ModbusInfo.FunctionCodeOverBound: return StringResources.ModbusTcpFunctionCodeOverBound;
                case ModbusInfo.FunctionCodeQuantityOver: return StringResources.ModbusTcpFunctionCodeQuantityOver;
                case ModbusInfo.FunctionCodeReadWriteException: return StringResources.ModbusTcpFunctionCodeReadWriteException;
                default: return StringResources.UnknownError;
            }
        }

        /// <summary>
        /// 检查当前的Modbus-Tcp响应是否是正确的
        /// </summary>
        /// <param name="send">发送的数据信息</param>
        /// <returns>带是否成功的结果数据</returns>
        private OperateResult<byte[]> CheckModbusTcpResponse( byte[] send )
        {
            OperateResult<byte[]> result = ReadFromCoreServer( send );
            if (result.IsSuccess)
            {
                if ((send[7] + 0x80) == result.Content[7])
                {
                    // 发生了错误
                    result.IsSuccess = false;
                    result.Message = GetDescriptionByErrorCode( result.Content[8] );
                    result.ErrorCode = result.Content[8];
                }
            }
            return result;
        }

        #endregion

        #region Read Support

        /// <summary>
        /// 读取服务器的数据，需要指定不同的功能码
        /// </summary>
        /// <param name="code">指令</param>
        /// <param name="address">地址</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        private OperateResult<byte[]> ReadModBusBase( byte code, string address, ushort length )
        {
            OperateResult<byte[]> command = BuildReadCommandBase( code, address, length );
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( command );

            OperateResult<byte[]> resultBytes = CheckModbusTcpResponse( command.Content );
            if (resultBytes.IsSuccess)
            {
                // 二次数据处理
                if (resultBytes.Content?.Length >= 9)
                {
                    byte[] buffer = new byte[resultBytes.Content.Length - 9];
                    Array.Copy( resultBytes.Content, 9, buffer, 0, buffer.Length );
                    resultBytes.Content = buffer;
                }
            }
            return resultBytes;
        }


        /// <summary>
        /// 读取线圈，需要指定起始地址
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <returns>带有成功标志的bool对象</returns>
        public OperateResult<bool> ReadCoil( string address )
        {
            var read = ReadModBusBase( ModbusInfo.ReadCoil, address, 1 );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool>( read );

            return GetBoolResultFromBytes( read );
        }

        




        /// <summary>
        /// 批量的读取线圈，需要指定起始地址，读取长度
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <param name="length">读取长度</param>
        /// <returns>带有成功标志的bool数组对象</returns>
        public OperateResult<bool[]> ReadCoil( string address, ushort length )
        {
            var read = ReadModBusBase( ModbusInfo.ReadCoil, address, length );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( read );

            return OperateResult.CreateSuccessResult( SoftBasic.ByteToBoolArray( read.Content, length ) );
        }




        /// <summary>
        /// 读取输入线圈，需要指定起始地址
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <returns>带有成功标志的bool对象</returns>
        public OperateResult<bool> ReadDiscrete( string address )
        {
            var read = ReadModBusBase( ModbusInfo.ReadDiscrete, address, 1 );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool>( read );

            return GetBoolResultFromBytes( read );
        }






        /// <summary>
        /// 批量的读取输入点，需要指定起始地址，读取长度
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <param name="length">读取长度</param>
        /// <returns>带有成功标志的bool数组对象</returns>
        public OperateResult<bool[]> ReadDiscrete( string address, ushort length )
        {
            var read = ReadModBusBase( ModbusInfo.ReadDiscrete, address, length );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( read );

            return OperateResult.CreateSuccessResult( SoftBasic.ByteToBoolArray( read.Content, length ) );
        }
        

        

        /// <summary>
        /// 从Modbus服务器批量读取寄存器的信息，需要指定起始地址，读取长度
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"，或者是带功能码格式03X1234</param>
        /// <param name="length">读取的数量</param>
        /// <returns>带有成功标志的字节信息</returns>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            OperateResult<byte,int> analysis = AnalysisReadAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            List<byte> lists = new List<byte>( );
            ushort alreadyFinished = 0;
            while (alreadyFinished < length)
            {
                ushort lengthTmp = (ushort)Math.Min( (length - alreadyFinished), 120 );
                OperateResult<byte[]> read = ReadModBusBase( analysis.Content1, (analysis.Content2 + alreadyFinished).ToString( ), lengthTmp );
                if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( read );

                lists.AddRange( read.Content );
                alreadyFinished += lengthTmp;
            }

            return OperateResult.CreateSuccessResult( lists.ToArray( ) );
        }

        #endregion
        
        #region Write One Register



        /// <summary>
        /// 写一个寄存器数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="high">高位</param>
        /// <param name="low">地位</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteOneRegister( string address, byte high, byte low )
        {
            OperateResult<byte[]> command = BuildWriteOneRegisterCommand( address, new byte[] { high, low } );
            if (!command.IsSuccess)
            {
                return command;
            }

            return CheckModbusTcpResponse( command.Content );
        }

        /// <summary>
        /// 写一个寄存器数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteOneRegister( string address, short value )
        {
            byte[] buffer = BitConverter.GetBytes( value );
            return WriteOneRegister( address, buffer[1], buffer[0] );
        }

        /// <summary>
        /// 写一个寄存器数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteOneRegister( string address, ushort value )
        {
            byte[] buffer = BitConverter.GetBytes( value );
            return WriteOneRegister( address, buffer[1], buffer[0] );
        }



        #endregion

        #region Write Base


        /// <summary>
        /// 将数据写入到Modbus的寄存器上去，需要指定起始地址和数据内容
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <param name="value">写入的数据，长度根据data的长度来指示</param>
        /// <returns>返回写入结果</returns>
        public override OperateResult Write( string address, byte[] value )
        {
            OperateResult<byte[]> command = BuildWriteRegisterCommand( address, value );
            if (!command.IsSuccess)
            {
                return command;
            }

            return CheckModbusTcpResponse( command.Content );
        }


        #endregion

        #region Write Coil

        /// <summary>
        /// 写一个线圈信息，指定是否通断
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteCoil( string address, bool value )
        {
            OperateResult<byte[]> command = BuildWriteOneCoilCommand( address, value );
            if (!command.IsSuccess)
            {
                return command;
            }

            return CheckModbusTcpResponse( command.Content );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteCoil( string address, bool[] values )
        {
            OperateResult<byte[]> command = BuildWriteCoilCommand( address, values );
            if (!command.IsSuccess)
            {
                return command;
            }

            return CheckModbusTcpResponse( command.Content );
        }


        #endregion

        #region Write String

        
        /// <summary>
        /// 向寄存器中写入指定长度的字符串,超出截断，不够补0，编码格式为ASCII
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, string value, int length )
        {
            byte[] temp = ByteTransform.TransByte( value, Encoding.ASCII );
            temp = SoftBasic.ArrayExpandToLength( temp, length );
            return Write( address, temp );
        }

        /// <summary>
        /// 向寄存器中写入字符串，编码格式为Unicode
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteUnicodeString( string address, string value )
        {
            byte[] temp = ByteTransform.TransByte( value, Encoding.Unicode );
            return Write( address, temp );
        }

        /// <summary>
        /// 向寄存器中写入指定长度的字符串,超出截断，不够补0，编码格式为Unicode
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteUnicodeString( string address, string value, int length )
        {
            byte[] temp = ByteTransform.TransByte( value, Encoding.Unicode );
            temp = SoftBasic.ArrayExpandToLength( temp, length * 2 );
            return Write( address, temp );
        }

        #endregion

        #region Write bool[]

        /// <summary>
        /// 向寄存器中写入bool数组，返回值说明，比如你写入M100,那么data[0]对应M100.0
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据，长度为8的倍数</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, bool[] values )
        {
            return Write( address, BasicFramework.SoftBasic.BoolArrayToByte( values ) );
        }


        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString( )
        {
            return "ModbusTcpNet";
        }

        #endregion
        
    }
}
