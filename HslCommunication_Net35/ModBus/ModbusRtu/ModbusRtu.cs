using HslCommunication.BasicFramework;
using HslCommunication.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core;

namespace HslCommunication.ModBus
{
    /// <summary>
    /// Modbus-Rtu通讯协议的类库，多项式码0xA001
    /// </summary>
    public class ModbusRtu : SerialBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个MOdbus-Tcp协议的客户端对象
        /// </summary>
        public ModbusRtu( )
        {
            ByteTransform = new HslCommunication.Core.ReverseWordTransform( );
        }


        /// <summary>
        /// 指定服务器地址，端口号，客户端自己的站号来初始化
        /// </summary>
        /// <param name="station">客户端自身的站号</param>
        public ModbusRtu( byte station = 0x01 )
        {
            ByteTransform = new HslCommunication.Core.ReverseWordTransform( );
            this.station = station;
        }

        #endregion

        #region Private Member

        private byte station = ModbusInfo.ReadCoil;                  // 本客户端的站号
        private bool isAddressStartWithZero = true;                  // 线圈值的地址值是否从零开始
        private HslCommunication.Core.IByteTransform ByteTransform;  // 数组转换规则

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
            
            byte[] buffer = new byte[6];
            buffer[0] = station;
            buffer[1] = code;
            buffer[2] = (byte)(analysis.Content / 256);
            buffer[3] = (byte)(analysis.Content % 256);
            buffer[4] = (byte)(count / 256);
            buffer[5] = (byte)(count % 256);

            return OperateResult.CreateSuccessResult( SoftCRC16.CRC16( buffer ) );
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
            
            byte[] buffer = new byte[6];
            buffer[0] = station;
            buffer[1] = ModbusInfo.WriteOneCoil;
            buffer[2] = (byte)(analysis.Content / 256);
            buffer[3] = (byte)(analysis.Content % 256);
            buffer[4] = (byte)(value ? 0xFF : 0x00);
            buffer[5] = 0x00;

            return OperateResult.CreateSuccessResult( SoftCRC16.CRC16( buffer ) );
        }





        private OperateResult<byte[]> BuildWriteOneRegisterCommand( string address, byte[] data )
        {
            OperateResult<int> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );
            
            byte[] buffer = new byte[6];
            buffer[0] = station;
            buffer[1] = ModbusInfo.WriteOneRegister;
            buffer[2] = (byte)(analysis.Content / 256);
            buffer[3] = (byte)(analysis.Content % 256);
            buffer[4] = data[1];
            buffer[5] = data[0];

            return OperateResult.CreateSuccessResult( SoftCRC16.CRC16( buffer ) );
        }



        private OperateResult<byte[]> BuildWriteCoilCommand( string address, bool[] values )
        {
            byte[] data = SoftBasic.BoolArrayToByte( values );

            OperateResult<int> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );
            
            byte[] buffer = new byte[7 + data.Length];
            buffer[0] = station;
            buffer[1] = ModbusInfo.WriteCoil;
            buffer[2] = (byte)(analysis.Content / 256);
            buffer[3] = (byte)(analysis.Content % 256);
            buffer[4] = (byte)(values.Length / 256);
            buffer[5] = (byte)(values.Length % 256);

            buffer[6] = (byte)(data.Length);
            data.CopyTo( buffer, 7 );

            return OperateResult.CreateSuccessResult( SoftCRC16.CRC16( buffer ) );
        }


        private OperateResult<byte[]> BuildWriteRegisterCommand( string address, byte[] data )
        {
            OperateResult<int> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );
            
            byte[] buffer = new byte[7 + data.Length];
            buffer[0] = station;
            buffer[1] = ModbusInfo.WriteRegister;
            buffer[2] = (byte)(analysis.Content / 256);
            buffer[3] = (byte)(analysis.Content % 256);
            buffer[4] = (byte)(data.Length / 2 / 256);
            buffer[5] = (byte)(data.Length / 2 % 256);

            buffer[6] = (byte)(data.Length);
            data.CopyTo( buffer, 7 );
            return OperateResult.CreateSuccessResult( SoftCRC16.CRC16( buffer) );
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
            OperateResult<byte[]> result = ReadBase( send );
            if (!result.IsSuccess) return result;

            if(result.Content.Length < 5)
            {
                return new OperateResult<byte[]>( )
                {
                    IsSuccess = false,
                    Message = "接收数据长度不能小于5",
                };
            }

            if(!SoftCRC16.CheckCRC16(result.Content))
            {
                return new OperateResult<byte[]>( )
                {
                    IsSuccess = false,
                    Message = "CRC校验失败",
                };
            }

            if ((send[1] + 0x80) == result.Content[1])
            {
                // 发生了错误
                return new OperateResult<byte[]>( )
                {
                    IsSuccess = false,
                    Message = GetDescriptionByErrorCode( result.Content[2] ),
                    ErrorCode = result.Content[2],
                };
            }
            else
            {
                // 移除CRC校验
                byte[] buffer = new byte[result.Content.Length - 2];
                Array.Copy( result.Content, 0, buffer, 0, buffer.Length );
                return OperateResult.CreateSuccessResult( buffer );
            }
        }

        #endregion

        #region Customer Support

        /// <summary>
        /// 读取自定义的数据类型，只针对寄存器而言，需要规定了写入和解析规则
        /// </summary>
        /// <typeparam name="T">类型名称</typeparam>
        /// <param name="address">起始地址</param>
        /// <returns>带是否成功的特定类型的对象</returns>
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
        /// 写入自定义的数据类型到寄存器去，只要规定了生成字节的方法即可
        /// </summary>
        /// <typeparam name="T">自定义类型</typeparam>
        /// <param name="address">起始地址</param>
        /// <param name="data">实例对象</param>
        /// <returns>是否成功</returns>
        public OperateResult WriteCustomer<T>( string address, T data ) where T : IDataTransfer, new()
        {
            return Write( address, data.ToSource( ) );
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
                if (resultBytes.Content?.Length >= 3)
                {
                    byte[] buffer = new byte[resultBytes.Content.Length - 3];
                    Array.Copy( resultBytes.Content, 3, buffer, 0, buffer.Length );
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

            return ByteTransformHelper.GetBoolResultFromBytes( read, ByteTransform );
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

            return ByteTransformHelper.GetBoolResultFromBytes( read, ByteTransform );
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
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <param name="length">读取的数量</param>
        /// <returns>带有成功标志的字节信息</returns>
        public OperateResult<byte[]> Read( string address, ushort length )
        {
            OperateResult<byte, int> analysis = AnalysisReadAddress( address );
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




        /// <summary>
        /// 读取指定地址的short数据
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <returns>带有成功标志的short数据</returns>
        public OperateResult<short> ReadInt16( string address )
        {
            return ByteTransformHelper.GetInt16ResultFromBytes( Read( address, 1 ), ByteTransform );
        }


        /// <summary>
        /// 读取指定地址的ushort数据
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <returns>带有成功标志的ushort数据</returns>
        public OperateResult<ushort> ReadUInt16( string address )
        {
            return ByteTransformHelper.GetUInt16ResultFromBytes( Read( address, 1 ), ByteTransform );
        }

        /// <summary>
        /// 读取指定地址的int数据
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <returns>带有成功标志的int数据</returns>
        public OperateResult<int> ReadInt32( string address )
        {
            return ByteTransformHelper.GetInt32ResultFromBytes( Read( address, 2 ), ByteTransform );
        }

        /// <summary>
        /// 读取指定地址的uint数据
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <returns>带有成功标志的uint数据</returns>
        public OperateResult<uint> ReadUInt32( string address )
        {
            return ByteTransformHelper.GetUInt32ResultFromBytes( Read( address, 2 ), ByteTransform );
        }

        /// <summary>
        /// 读取指定地址的float数据
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <returns>带有成功标志的float数据</returns>
        public OperateResult<float> ReadFloat( string address )
        {
            return ByteTransformHelper.GetSingleResultFromBytes( Read( address, 2 ), ByteTransform );
        }

        /// <summary>
        /// 读取指定地址的long数据
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <returns>带有成功标志的long数据</returns>
        public OperateResult<long> ReadInt64( string address )
        {
            return ByteTransformHelper.GetInt64ResultFromBytes( Read( address, 4 ), ByteTransform );
        }

        /// <summary>
        /// 读取指定地址的ulong数据
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <returns>带有成功标志的ulong数据</returns>
        public OperateResult<ulong> ReadUInt64( string address )
        {
            return ByteTransformHelper.GetUInt64ResultFromBytes( Read( address, 4 ), ByteTransform );
        }

        /// <summary>
        /// 读取指定地址的double数据
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <returns>带有成功标志的double数据</returns>
        public OperateResult<double> ReadDouble( string address )
        {
            return ByteTransformHelper.GetDoubleResultFromBytes( Read( address, 4 ), ByteTransform );
        }

        /// <summary>
        /// 读取地址地址的String数据，字符串编码为ASCII
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <param name="length">字符串长度</param>
        /// <returns>带有成功标志的string数据</returns>
        public OperateResult<string> ReadString( string address, ushort length )
        {
            return ByteTransformHelper.GetStringResultFromBytes( Read( address, length ), ByteTransform );
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
        public OperateResult Write( string address, byte[] value )
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
        /// 向寄存器中写入字符串，编码格式为ASCII
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, string value )
        {
            byte[] temp = ByteTransform.TransByte( value, Encoding.ASCII );
            temp = SoftBasic.ArrayExpandToLengthEven( temp );
            return Write( address, temp );
        }

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

        #region Write Short

        /// <summary>
        /// 向寄存器中写入short数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, short[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向寄存器中写入short数据，返回值说明
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
        /// 向寄存器中写入ushort数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, ushort[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }


        /// <summary>
        /// 向寄存器中写入ushort数据，返回值说明
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
        /// 向寄存器中写入int数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, int[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向寄存器中写入int数据，返回值说明
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
        /// 向寄存器中写入uint数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, uint[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向寄存器中写入uint数据，返回值说明
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
        /// 向寄存器中写入float数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, float[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向寄存器中写入float数据，返回值说明
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
        /// 向寄存器中写入long数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, long[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向寄存器中写入long数据，返回值说明
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
        /// 向寄存器中写入ulong数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, ulong[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向寄存器中写入ulong数据，返回值说明
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
        /// 向寄存器中写入double数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, double[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向寄存器中写入double数据，返回值说明
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
            return "ModbusTcpNet";
        }

        #endregion



    }
}
