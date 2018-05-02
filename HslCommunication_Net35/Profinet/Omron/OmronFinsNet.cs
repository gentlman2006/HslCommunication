using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Core;
using System.Net.Sockets;
using HslCommunication.BasicFramework;

namespace HslCommunication.Profinet.Omron
{
    /// <summary>
    /// 欧姆龙PLC通讯类，采用Fins-Tcp通信协议实现
    /// </summary>
    public class OmronFinsNet : NetworkDoubleBase<FinsMessage,ReverseWordTransform> , IReadWriteNet
    {

        #region Constructor

        /// <summary>
        /// 实例化三菱的Qna兼容3E帧协议的通讯对象
        /// </summary>
        public OmronFinsNet( )
        {

        }

        /// <summary>
        /// 实例化一个三菱的Qna兼容3E帧协议的通讯对象
        /// </summary>
        /// <param name="ipAddress">PLCd的Ip地址</param>
        /// <param name="port">PLC的端口</param>
        public OmronFinsNet( string ipAddress, int port )
        {
            IpAddress = ipAddress;
            Port = port;
        }

        #endregion

        #region Public Member

        /// <summary>
        /// 信息控制字段，默认0x80
        /// </summary>
        public byte ICF { get; set; } = 0x80;
        /// <summary>
        /// 系统使用的内部信息
        /// </summary>
        public byte RSV { get; private set; } = 0x00;

        /// <summary>
        /// 网络层信息，默认0x02，如果有八层消息，就设置为0x07
        /// </summary>
        public byte GCT { get; set; } = 0x02;

        /// <summary>
        /// PLC的网络号地址，默认0x00
        /// </summary>
        public byte DNA { get; set; } = 0x00;

        
        /// <summary>
        /// PLC的节点地址，默认0x13
        /// </summary>
        public byte DA1 { get; set; } = 0x13;

        /// <summary>
        /// PLC的单元号地址
        /// </summary>
        public byte DA2 { get; set; } = 0x00;

        /// <summary>
        /// 上位机的网络号地址
        /// </summary>
        public byte SNA { get; set; } = 0x00;


        private byte computerSA1 = 0x0B;

        /// <summary>
        /// 上位机的节点地址，默认0x0B
        /// </summary>
        public byte SA1
        {
            get { return computerSA1; }
            set
            {
                computerSA1 = value;
                handSingle[19] = value;
            }
        }

        /// <summary>
        /// 上位机的单元号地址
        /// </summary>
        public byte SA2 { get; set; }

        /// <summary>
        /// 设备的标识号
        /// </summary>
        public byte SID { get; set; } = 0x00;


        #endregion

        #region Address Analysis



        /// <summary>
        /// 解析数据地址，Omron手册第188页
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="isBit">是否是位地址</param>
        /// <returns></returns>
        private OperateResult<OmronFinsDataType,byte[]> AnalysisAddress( string address ,bool isBit)
        {
            var result = new OperateResult<OmronFinsDataType, byte[]>( );
            try
            {
                switch (address[0])
                {
                    case 'D':
                    case 'd':
                        {
                            // DM区数据
                            result.Content1 = OmronFinsDataType.DM;
                            break;
                        }
                    case 'C':
                    case 'c':
                        {
                            // CIO区数据
                            result.Content1 = OmronFinsDataType.CIO;
                            break;
                        }
                    case 'W':
                    case 'w':
                        {
                            // WR区
                            result.Content1 = OmronFinsDataType.WR;
                            break;
                        }
                    case 'H':
                    case 'h':
                        {
                            // HR区
                            result.Content1 = OmronFinsDataType.HR;
                            break;
                        }
                    case 'A':
                    case 'a':
                        {
                            // AR区
                            result.Content1 = OmronFinsDataType.AR;
                            break;
                        }
                    default: throw new Exception( "输入的类型不支持，请重新输入" );
                }

                if(isBit)
                {
                    // 位操作
                    string[] splits = address.Substring( 1 ).Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries );
                    ushort addr = ushort.Parse( splits[0] );
                    result.Content2 = new byte[3];
                    result.Content2[0] = BitConverter.GetBytes( addr )[1];
                    result.Content2[1] = BitConverter.GetBytes( addr )[0];

                    if (splits.Length > 1)
                    {
                        result.Content2[2] = byte.Parse( splits[1] );
                        if (result.Content2[2] > 15)
                        {
                            throw new Exception( "输入的位地址只能在0-15之间。" );
                        }
                    }
                }
                else
                {
                    // 字操作
                    ushort addr = ushort.Parse( address.Substring( 1 ) );
                    result.Content2 = new byte[3];
                    result.Content2[0] = BitConverter.GetBytes( addr )[1];
                    result.Content2[1] = BitConverter.GetBytes( addr )[0];
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



        private OperateResult<byte[]> ResponseValidAnalysis(byte[] response,bool isRead)
        {
            // 数据有效性分析
            if (response.Length >= 16)
            {
                // 提取错误码
                byte[] buffer = new byte[4];
                buffer[0] = response[15];
                buffer[1] = response[14];
                buffer[2] = response[13];
                buffer[3] = response[12];
                int err = BitConverter.ToInt32( buffer, 0 );
                if (err > 0)
                {
                    return new OperateResult<byte[]>( )
                    {
                        ErrorCode = err,
                        Message = OmronInfo.GetStatusDescription( err ),
                    };
                }

                if (response.Length >= 30)
                {
                    err = response[28] * 256 + response[29];
                    if (err > 0)
                    {
                        return new OperateResult<byte[]>( )
                        {
                            ErrorCode = err,
                            Message = "结束码错误，为：" + err,
                        };
                    }

                    if (!isRead)
                    {
                        // 写入操作
                        return OperateResult.CreateSuccessResult( new byte[0] );
                    }
                    else
                    {
                        // 读取操作
                        byte[] content = new byte[response.Length - 30];
                        if (content.Length > 0)
                        {
                            Array.Copy( response, 30, content, 0, content.Length );
                        }
                        return OperateResult.CreateSuccessResult( content );
                    }
                }
            }

            return new OperateResult<byte[]>( )
            {
                Message = "数据长度接收错误",
            };
        }
        




        #endregion

        #region Build Command


        /// <summary>
        /// 将普通的指令打包成完整的指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private byte[] PackCommand(byte[] cmd)
        {
            byte[] buffer = new byte[26 + cmd.Length];
            Array.Copy( handSingle, 0, buffer, 0, 4 );
            byte[] tmp = BitConverter.GetBytes( buffer.Length - 8 );
            Array.Reverse( tmp );
            tmp.CopyTo( buffer, 4 );
            buffer[11] = 0x02;

            buffer[16] = ICF;
            buffer[17] = RSV;
            buffer[18] = GCT;
            buffer[19] = DNA;
            buffer[20] = DA1;
            buffer[21] = DA2;
            buffer[22] = SNA;
            buffer[23] = SA1;
            buffer[24] = SA2;
            buffer[25] = SID;
            cmd.CopyTo( buffer, 26 );

            return buffer;
        }



        /// <summary>
        /// 根据类型地址长度确认需要读取的指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">长度</param>
        /// <param name="isBit">是否是位读取</param>
        /// <returns>带有成功标志的指令数据</returns>
        private OperateResult<byte[]> BuildReadCommand( string address, ushort length ,bool isBit)
        {
            var result = new OperateResult<byte[]>( );
            var analysis = AnalysisAddress( address, isBit );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            byte[] _PLCCommand = new byte[8];
            _PLCCommand[0] = 0x01;    // 读取存储区数据
            _PLCCommand[1] = 0x01;
            if(isBit)
            {
                _PLCCommand[2] = analysis.Content1.BitCode;
            }
            else
            {
                _PLCCommand[2] = analysis.Content1.WordCode;
            }
            analysis.Content2.CopyTo( _PLCCommand, 3 );
            _PLCCommand[6] = (byte)(length / 256);                       // 长度
            _PLCCommand[7] = (byte)(length % 256);

            try
            {
                result.Content = PackCommand( _PLCCommand );
                result.IsSuccess = true;
            }
            catch(Exception ex)
            {
                LogNet?.WriteException( ToString( ), ex );
                result.Message = ex.Message;
            }
            return result;
        }



        /// <summary>
        /// 根据类型地址以及需要写入的数据来生成指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value"></param>
        /// <param name="isBit">是否是位操作</param>
        /// <returns></returns>
        private OperateResult<byte[]> BuildWriteCommand( string address, byte[] value, bool isBit )
        {
            var result = new OperateResult<byte[]>( );
            var analysis = AnalysisAddress( address, isBit );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            byte[] _PLCCommand = new byte[8 + value.Length];
            _PLCCommand[0] = 0x01;    // 读取存储区数据
            _PLCCommand[1] = 0x02;
            if (isBit)
            {
                _PLCCommand[2] = analysis.Content1.BitCode;
            }
            else
            {
                _PLCCommand[2] = analysis.Content1.WordCode;
            }
            analysis.Content2.CopyTo( _PLCCommand, 3 );

            if (isBit)
            {
                _PLCCommand[6] = (byte)(value.Length / 256);                       // 长度
                _PLCCommand[7] = (byte)(value.Length % 256);
            }
            else
            {
                _PLCCommand[6] = (byte)(value.Length / 2 / 256);                       // 长度
                _PLCCommand[7] = (byte)(value.Length / 2 % 256);
            }

            value.CopyTo( _PLCCommand, 8 );


            try
            {
                result.Content = PackCommand( _PLCCommand );
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), ex );
                result.Message = ex.Message;
            }
            return result;
        }


        #endregion

        #region Customer Support

        /// <summary>
        /// 读取自定义的数据类型，只要规定了写入和解析规则
        /// </summary>
        /// <typeparam name="T">类型名称</typeparam>
        /// <param name="address">起始地址</param>
        /// <returns></returns>
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
        /// 写入自定义的数据类型到PLC去，只要规定了生成字节的方法即可
        /// </summary>
        /// <typeparam name="T">自定义类型</typeparam>
        /// <param name="address">起始地址</param>
        /// <param name="data">实例对象</param>
        /// <returns></returns>
        public OperateResult WriteCustomer<T>( string address, T data ) where T : IDataTransfer, new()
        {
            return Write( address, data.ToSource( ) );
        }


        #endregion

        #region Double Mode Override

        /// <summary>
        /// 在连接上欧姆龙PLC后，需要进行一步握手协议
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        protected override OperateResult InitilizationOnConnect( Socket socket )
        {
            // handSingle就是握手信号字节
            OperateResult<byte[], byte[]> read = ReadFromCoreServerBase( socket, handSingle );
            if (!read.IsSuccess) return read;
            
            // 检查返回的状态
            byte[] buffer = new byte[4];
            buffer[0] = read.Content2[7];
            buffer[1] = read.Content2[6];
            buffer[2] = read.Content2[5];
            buffer[3] = read.Content2[4];
            int status = BitConverter.ToInt32( buffer, 0 );
            if(status != 0)
            {
                return new OperateResult( )
                {
                    ErrorCode = status,
                    Message = "初始化失败，具体原因请根据错误码查找"
                };
            }

            // 提取PLC的节点地址
            if (read.Content2.Length >= 16)
            {
                DA1 = read.Content2[15];
            }
            return OperateResult.CreateSuccessResult( ) ;
        }

        #endregion

        #region Read Support

        /// <summary>
        /// 从欧姆龙PLC中读取想要的数据，返回读取结果，读取单位为字
        /// </summary>
        /// <param name="address">读取地址，格式为"D100","C100","W100","H100","A100"</param>
        /// <param name="length">读取的数据长度，字最大值960，位最大值7168</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<byte[]> Read( string address, ushort length )
        {
            //获取指令
            var command = BuildReadCommand( address, length, false );
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( command );

            // 核心数据交互
            OperateResult<byte[]> read = ReadFromCoreServer( command.Content );
            if(!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( read );

            // 数据有效性分析
            OperateResult<byte[]> valid = ResponseValidAnalysis( read.Content, true );
            if(!valid.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( valid );

            // 读取到了正确的数据
            return OperateResult.CreateSuccessResult( valid.Content );
        }



        /// <summary>
        /// 从欧姆龙PLC中批量读取位软元件，返回读取结果
        /// </summary>
        /// <param name="address">读取地址，格式为"D100","C100","W100","H100","A100"</param>
        /// <param name="length">读取的长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<bool[]> ReadBool( string address, ushort length )
        {
            //获取指令
            var command = BuildReadCommand( address, length, true );
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( command );

            // 核心数据交互
            OperateResult<byte[]> read = ReadFromCoreServer( command.Content );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( read );

            // 数据有效性分析
            OperateResult<byte[]> valid = ResponseValidAnalysis( read.Content, true );
            if (!valid.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( valid );

            // 返回正确的数据信息
            return OperateResult.CreateSuccessResult( valid.Content.Select( m => m != 0x00 ? true : false ).ToArray( ) );
        }


        /// <summary>
        /// 从欧姆龙PLC中批量读取位软元件，返回读取结果
        /// </summary>
        /// <param name="address">读取地址，格式为"D100.0","C100.15","W100.7","H100.4","A100.9"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<bool> ReadBool( string address )
        {
            OperateResult<bool[]> read = ReadBool( address, 1 );
            if (read.IsSuccess)
            {
                return OperateResult.CreateSuccessResult( read.Content[0] );
            }
            else
            {
                return OperateResult.CreateFailedResult<bool>( read );
            }
        }


        /// <summary>
        /// 读取欧姆龙PLC中字软元件指定地址的short数据
        /// </summary>
        /// <param name="address">读取地址，格式为"D100","C100","W100","H100","A100"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<short> ReadInt16( string address )
        {
            return GetInt16ResultFromBytes( Read( address, 1 ) );
        }


        /// <summary>
        /// 读取欧姆龙PLC中字软元件指定地址的ushort数据
        /// </summary>
        /// <param name="address">读取地址，格式为"D100","C100","W100","H100","A100"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<ushort> ReadUInt16( string address )
        {
            return GetUInt16ResultFromBytes( Read( address, 1 ) );
        }

        /// <summary>
        /// 读取欧姆龙PLC中字软元件指定地址的int数据
        /// </summary>
        /// <param name="address">读取地址，格式为"D100","C100","W100","H100","A100"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<int> ReadInt32( string address )
        {
            return GetInt32ResultFromBytes( Read( address, 2 ) );
        }

        /// <summary>
        /// 读取欧姆龙PLC中字软元件指定地址的uint数据
        /// </summary>
        /// <param name="address">读取地址，格式为"D100","C100","W100","H100","A100"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<uint> ReadUInt32( string address )
        {
            return GetUInt32ResultFromBytes( Read( address, 2 ) );
        }

        /// <summary>
        /// 读取欧姆龙PLC中字软元件指定地址的float数据
        /// </summary>
        /// <param name="address">读取地址，格式为"D100","C100","W100","H100","A100"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<float> ReadFloat( string address )
        {
            return GetSingleResultFromBytes( Read( address, 2 ) );
        }

        /// <summary>
        /// 读取欧姆龙PLC中字软元件指定地址的long数据
        /// </summary>
        /// <param name="address">读取地址，格式为"D100","C100","W100","H100","A100"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<long> ReadInt64( string address )
        {
            return GetInt64ResultFromBytes( Read( address, 4 ) );
        }

        /// <summary>
        /// 读取欧姆龙PLC中字软元件指定地址的ulong数据
        /// </summary>
        /// <param name="address">读取地址，格式为"D100","C100","W100","H100","A100"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<ulong> ReadUInt64( string address )
        {
            return GetUInt64ResultFromBytes( Read( address, 4 ) );
        }

        /// <summary>
        /// 读取欧姆龙PLC中字软元件指定地址的double数据
        /// </summary>
        /// <param name="address">读取地址，格式为"D100","C100","W100","H100","A100"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<double> ReadDouble( string address )
        {
            return GetDoubleResultFromBytes( Read( address, 4 ) );
        }

        /// <summary>
        /// 读取欧姆龙PLC中字软元件地址地址的String数据，编码为ASCII
        /// </summary>
        /// <param name="address">读取地址，格式为"D100","C100","W100","H100","A100"</param>
        /// <param name="length">字符串长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<string> ReadString( string address, ushort length )
        {
            return GetStringResultFromBytes( Read( address, length ) );
        }



        #endregion

        #region Write Base


        /// <summary>
        /// 向PLC写入数据，数据格式为原始的字节类型
        /// </summary>
        /// <param name="address">初始地址</param>
        /// <param name="value">原始的字节数据</param>
        /// <returns>结果</returns>
        public OperateResult Write( string address, byte[] value )
        {
            //获取指令
            var command = BuildWriteCommand( address, value, false );
            if (!command.IsSuccess) return command;

            // 核心数据交互
            OperateResult<byte[]> read = ReadFromCoreServer( command.Content );
            if (!read.IsSuccess) return read;

            // 数据有效性分析
            OperateResult<byte[]> valid = ResponseValidAnalysis( read.Content, false );
            if (!valid.IsSuccess) return valid;

            // 成功
            return OperateResult.CreateSuccessResult( ) ;
        }




        #endregion

        #region Write String


        /// <summary>
        /// 向PLC中字软元件写入字符串，编码格式为ASCII
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回读取结果</returns>
        public OperateResult Write( string address, string value )
        {
            byte[] temp = ByteTransform.TransByte( value, Encoding.ASCII );
            temp = SoftBasic.ArrayExpandToLengthEven( temp );
            return Write( address, temp );
        }

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
        /// 向PLC中位软元件写入bool数组，返回值说明，比如你写入D100,values[0]对应D100.0
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据，长度为8的倍数</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, bool value )
        {
            return Write( address, new bool[] { value } );
        }

        /// <summary>
        /// 向PLC中位软元件写入bool数组，返回值说明，比如你写入D100,values[0]对应D100.0
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据，可以指定任意的长度</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, bool[] values )
        {
            OperateResult result = new OperateResult( );

            //获取指令
            var command = BuildWriteCommand( address, values.Select( m => m ? (byte)0x01 : (byte)0x00 ).ToArray( ), true );
            if (!command.IsSuccess) return command;

            // 核心数据交互
            OperateResult<byte[]> read = ReadFromCoreServer( command.Content );
            if (!read.IsSuccess) return read;

            // 数据有效性分析
            OperateResult<byte[]> valid = ResponseValidAnalysis( read.Content, false );
            if (!valid.IsSuccess) return valid;

            // 写入成功
            return OperateResult.CreateSuccessResult( );
        }


        #endregion

        #region Write Short

        /// <summary>
        /// 向PLC中字软元件写入short数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, short[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入short数据，返回值说明
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
        /// 向PLC中字软元件写入ushort数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, ushort[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }


        /// <summary>
        /// 向PLC中字软元件写入ushort数据，返回值说明
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
        /// 向PLC中字软元件写入int数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, int[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入int数据，返回值说明
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
        /// 向PLC中字软元件写入uint数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, uint[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入uint数据，返回值说明
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
        /// 向PLC中字软元件写入float数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, float[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入float数据，返回值说明
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
        /// 向PLC中字软元件写入long数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, long[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入long数据，返回值说明
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
        /// 向PLC中字软元件写入ulong数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, ulong[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入ulong数据，返回值说明
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
        /// 向PLC中字软元件写入double数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, double[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入double数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, double value )
        {
            return Write( address, new double[] { value } );
        }

        #endregion

        #region Hand Single

        // 握手信号
        // 46494E530000000C0000000000000000000000D6 
        private byte[] handSingle = new byte[]
        {
            0x46,0x49,0x4E,0x53, // FINS
            0x00,0x00,0x00,0x0C, // 后面的命令长度
            0x00,0x00,0x00,0x00, // 命令码
            0x00,0x00,0x00,0x00, // 错误码
            0x00,0x00,0x00,0x01  // 节点号
        };


        #endregion

        #region Object Override

        /// <summary>
        /// 获取当前对象的字符串标识形式
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString( )
        {
            return "OmronFinsNet";
        }

        #endregion

    }
}
