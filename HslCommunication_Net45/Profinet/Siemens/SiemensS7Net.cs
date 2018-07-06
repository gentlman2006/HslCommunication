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
 * 
 * 
 *********************************************************************************/




namespace HslCommunication.Profinet.Siemens
{
    /// <summary>
    /// 一个西门子的客户端类，使用S7协议来进行数据交互
    /// </summary>
    public class SiemensS7Net : NetworkDeviceBase<S7Message, ReverseBytesTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化一个西门子的S7协议的通讯对象
        /// </summary>
        /// <param name="siemens">指定西门子的型号</param>
        public SiemensS7Net( SiemensPLCS siemens )
        {
            Initialization( siemens, string.Empty );
        }

        /// <summary>
        /// 实例化一个西门子的S7协议的通讯对象并指定Ip地址
        /// </summary>
        /// <param name="siemens">指定西门子的型号</param>
        /// <param name="ipAddress">Ip地址</param>
        public SiemensS7Net( SiemensPLCS siemens, string ipAddress )
        {
            Initialization( siemens, ipAddress );
        }

        /// <summary>
        /// 初始化方法
        /// </summary>
        /// <param name="siemens"></param>
        /// <param name="ipAddress"></param>
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



        #endregion

        #region NetworkDoubleBase Override

        /// <summary>
        /// 在客户端连接上服务器后，所做的一些初始化操作
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        protected override OperateResult InitializationOnConnect( Socket socket )
        {
            // 第一层通信的初始化
            OperateResult<byte[], byte[]> read_first = ReadFromCoreServerBase( socket, plcHead1 );
            if (!read_first.IsSuccess)
            {
                return read_first;
            }

            // 第二层通信的初始化
            OperateResult<byte[], byte[]> read_second = ReadFromCoreServerBase( socket, plcHead2 );
            if (!read_second.IsSuccess)
            {
                return read_second;
            }

            // 返回成功的信号
            return OperateResult.CreateSuccessResult( );
        }


        #endregion

        #region Address Analysis

        /// <summary>
        /// 计算特殊的地址信息
        /// </summary>
        /// <param name="address">字符串信息</param>
        /// <returns>实际值</returns>
        private int CalculateAddressStarted( string address )
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
        /// 解析数据地址，解析出地址类型，起始地址，DB块的地址
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns>解析出地址类型，起始地址，DB块的地址</returns>
        private OperateResult<byte, int, ushort> AnalysisAddress( string address )
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
                else
                {
                    result.Message = "不支持的数据类型";
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
        /// 生成一个读取字数据指令头的通用方法
        /// </summary>
        /// <param name="address">解析后的地址</param>
        /// <param name="length">每个地址的读取长度</param>
        /// <returns>携带有命令字节</returns>
        private OperateResult<byte[]> BuildReadCommand( OperateResult<byte, int, ushort>[] address, ushort[] length )
        {
            if (address == null) throw new NullReferenceException( "address" );
            if (length == null) throw new NullReferenceException( "count" );
            if (address.Length != length.Length) throw new Exception( "两个参数的个数不统一" );
            if (length.Length > 19) throw new Exception( "读取的数组数量不允许大于19" );

            var result = new OperateResult<byte[]>( );
            int readCount = length.Length;

            byte[] _PLCCommand = new byte[19 + readCount * 12];

            // ======================================================================================
            // Header
            // 报文头
            _PLCCommand[0] = 0x03;
            _PLCCommand[1] = 0x00;
            // 长度
            _PLCCommand[2] = (byte)(_PLCCommand.Length / 256);
            _PLCCommand[3] = (byte)(_PLCCommand.Length % 256);
            // 固定
            _PLCCommand[4] = 0x02;
            _PLCCommand[5] = 0xF0;
            _PLCCommand[6] = 0x80;
            // 协议标识
            _PLCCommand[7] = 0x32;
            // 命令：发
            _PLCCommand[8] = 0x01;
            // redundancy identification (reserved): 0x0000;
            _PLCCommand[9] = 0x00;
            _PLCCommand[10] = 0x00;
            // protocol data unit reference; it’s increased by request event;
            _PLCCommand[11] = 0x00;
            _PLCCommand[12] = 0x01;
            // 参数命令数据总长度
            _PLCCommand[13] = (byte)((_PLCCommand.Length - 17) / 256);
            _PLCCommand[14] = (byte)((_PLCCommand.Length - 17) % 256);

            // 读取内部数据时为00，读取CPU型号为Data数据长度
            _PLCCommand[15] = 0x00;
            _PLCCommand[16] = 0x00;


            // ======================================================================================
            // Parameter

            // 读写指令，04读，05写
            _PLCCommand[17] = 0x04;
            // 读取数据块个数
            _PLCCommand[18] = (byte)readCount;


            for (int ii = 0; ii < readCount; ii++)
            {
                //===========================================================================================
                // 指定有效值类型
                _PLCCommand[19 + ii * 12] = 0x12;
                // 接下来本次地址访问长度
                _PLCCommand[20 + ii * 12] = 0x0A;
                // 语法标记，ANY
                _PLCCommand[21 + ii * 12] = 0x10;
                // 按字为单位
                _PLCCommand[22 + ii * 12] = 0x02;
                // 访问数据的个数
                _PLCCommand[23 + ii * 12] = (byte)(length[ii] / 256);
                _PLCCommand[24 + ii * 12] = (byte)(length[ii] % 256);
                // DB块编号，如果访问的是DB块的话
                _PLCCommand[25 + ii * 12] = (byte)(address[ii].Content3 / 256);
                _PLCCommand[26 + ii * 12] = (byte)(address[ii].Content3 % 256);
                // 访问数据类型
                _PLCCommand[27 + ii * 12] = address[ii].Content1;
                // 偏移位置
                _PLCCommand[28 + ii * 12] = (byte)(address[ii].Content2 / 256 / 256 % 256);
                _PLCCommand[29 + ii * 12] = (byte)(address[ii].Content2 / 256 % 256);
                _PLCCommand[30 + ii * 12] = (byte)(address[ii].Content2 % 256);
            }
            result.Content = _PLCCommand;
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 生成一个位读取数据指令头的通用方法
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private OperateResult<byte[]> BuildBitReadCommand( string address )
        {
            var result = new OperateResult<byte[]>( );

            byte[] _PLCCommand = new byte[31];
            // 报文头
            _PLCCommand[0] = 0x03;
            _PLCCommand[1] = 0x00;
            // 长度
            _PLCCommand[2] = (byte)(_PLCCommand.Length / 256);
            _PLCCommand[3] = (byte)(_PLCCommand.Length % 256);
            // 固定
            _PLCCommand[4] = 0x02;
            _PLCCommand[5] = 0xF0;
            _PLCCommand[6] = 0x80;
            _PLCCommand[7] = 0x32;
            // 命令：发
            _PLCCommand[8] = 0x01;
            // 标识序列号
            _PLCCommand[9] = 0x00;
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x00;
            _PLCCommand[12] = 0x01;
            // 命令数据总长度
            _PLCCommand[13] = (byte)((_PLCCommand.Length - 17) / 256);
            _PLCCommand[14] = (byte)((_PLCCommand.Length - 17) % 256);

            _PLCCommand[15] = 0x00;
            _PLCCommand[16] = 0x00;

            // 命令起始符
            _PLCCommand[17] = 0x04;
            // 读取数据块个数
            _PLCCommand[18] = 0x01;


            // 填充数据
            OperateResult<byte, int, ushort> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }

            //===========================================================================================
            // 读取地址的前缀
            _PLCCommand[19] = 0x12;
            _PLCCommand[20] = 0x0A;
            _PLCCommand[21] = 0x10;
            // 读取的数据时位
            _PLCCommand[22] = 0x01;
            // 访问数据的个数
            _PLCCommand[23] = 0x00;
            _PLCCommand[24] = 0x01;
            // DB块编号，如果访问的是DB块的话
            _PLCCommand[25] = (byte)(analysis.Content3 / 256);
            _PLCCommand[26] = (byte)(analysis.Content3 % 256);
            // 访问数据类型
            _PLCCommand[27] = analysis.Content1;
            // 偏移位置
            _PLCCommand[28] = (byte)(analysis.Content2 / 256 / 256 % 256);
            _PLCCommand[29] = (byte)(analysis.Content2 / 256 % 256);
            _PLCCommand[30] = (byte)(analysis.Content2 % 256);

            result.Content = _PLCCommand;
            result.IsSuccess = true;
            return result;
        }


        /// <summary>
        /// 生成一个写入字节数据的指令
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private OperateResult<byte[]> BuildWriteByteCommand( string address, byte[] data )
        {
            if (data == null) data = new byte[0];
            var result = new OperateResult<byte[]>( );

            OperateResult<byte, int, ushort> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }

            byte[] _PLCCommand = new byte[35 + data.Length];
            _PLCCommand[0] = 0x03;
            _PLCCommand[1] = 0x00;
            // 长度
            _PLCCommand[2] = (byte)((35 + data.Length) / 256);
            _PLCCommand[3] = (byte)((35 + data.Length) % 256);
            // 固定
            _PLCCommand[4] = 0x02;
            _PLCCommand[5] = 0xF0;
            _PLCCommand[6] = 0x80;
            _PLCCommand[7] = 0x32;
            // 命令 发
            _PLCCommand[8] = 0x01;
            // 标识序列号
            _PLCCommand[9] = 0x00;
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x00;
            _PLCCommand[12] = 0x01;
            // 固定
            _PLCCommand[13] = 0x00;
            _PLCCommand[14] = 0x0E;
            // 写入长度+4
            _PLCCommand[15] = (byte)((4 + data.Length) / 256);
            _PLCCommand[16] = (byte)((4 + data.Length) % 256);
            // 读写指令
            _PLCCommand[17] = 0x05;
            // 写入数据块个数
            _PLCCommand[18] = 0x01;
            // 固定，返回数据长度
            _PLCCommand[19] = 0x12;
            _PLCCommand[20] = 0x0A;
            _PLCCommand[21] = 0x10;
            // 写入方式，1是按位，2是按字
            _PLCCommand[22] = 0x02;
            // 写入数据的个数
            _PLCCommand[23] = (byte)(data.Length / 256);
            _PLCCommand[24] = (byte)(data.Length % 256);
            // DB块编号，如果访问的是DB块的话
            _PLCCommand[25] = (byte)(analysis.Content3 / 256);
            _PLCCommand[26] = (byte)(analysis.Content3 % 256);
            // 写入数据的类型
            _PLCCommand[27] = analysis.Content1;
            // 偏移位置
            _PLCCommand[28] = (byte)(analysis.Content2 / 256 / 256 % 256); ;
            _PLCCommand[29] = (byte)(analysis.Content2 / 256 % 256);
            _PLCCommand[30] = (byte)(analysis.Content2 % 256);
            // 按字写入
            _PLCCommand[31] = 0x00;
            _PLCCommand[32] = 0x04;
            // 按位计算的长度
            _PLCCommand[33] = (byte)(data.Length * 8 / 256);
            _PLCCommand[34] = (byte)(data.Length * 8 % 256);

            data.CopyTo( _PLCCommand, 35 );

            result.Content = _PLCCommand;
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 生成一个写入位数据的指令
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private OperateResult<byte[]> BuildWriteBitCommand( string address, bool data )
        {
            var result = new OperateResult<byte[]>( );

            OperateResult<byte, int, ushort> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }


            byte[] buffer = new byte[1];
            buffer[0] = data ? (byte)0x01 : (byte)0x00;

            byte[] _PLCCommand = new byte[35 + buffer.Length];
            _PLCCommand[0] = 0x03;
            _PLCCommand[1] = 0x00;
            // 长度
            _PLCCommand[2] = (byte)((35 + buffer.Length) / 256);
            _PLCCommand[3] = (byte)((35 + buffer.Length) % 256);
            // 固定
            _PLCCommand[4] = 0x02;
            _PLCCommand[5] = 0xF0;
            _PLCCommand[6] = 0x80;
            _PLCCommand[7] = 0x32;
            // 命令 发
            _PLCCommand[8] = 0x01;
            // 标识序列号
            _PLCCommand[9] = 0x00;
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x00;
            _PLCCommand[12] = 0x01;
            // 固定
            _PLCCommand[13] = 0x00;
            _PLCCommand[14] = 0x0E;
            // 写入长度+4
            _PLCCommand[15] = (byte)((4 + buffer.Length) / 256);
            _PLCCommand[16] = (byte)((4 + buffer.Length) % 256);
            // 命令起始符
            _PLCCommand[17] = 0x05;
            // 写入数据块个数
            _PLCCommand[18] = 0x01;
            _PLCCommand[19] = 0x12;
            _PLCCommand[20] = 0x0A;
            _PLCCommand[21] = 0x10;
            // 写入方式，1是按位，2是按字
            _PLCCommand[22] = 0x01;
            // 写入数据的个数
            _PLCCommand[23] = (byte)(buffer.Length / 256);
            _PLCCommand[24] = (byte)(buffer.Length % 256);
            // DB块编号，如果访问的是DB块的话
            _PLCCommand[25] = (byte)(analysis.Content3 / 256);
            _PLCCommand[26] = (byte)(analysis.Content3 % 256);
            // 写入数据的类型
            _PLCCommand[27] = analysis.Content1;
            // 偏移位置
            _PLCCommand[28] = (byte)(analysis.Content2 / 256 / 256);
            _PLCCommand[29] = (byte)(analysis.Content2 / 256);
            _PLCCommand[30] = (byte)(analysis.Content2 % 256);
            // 按位写入
            _PLCCommand[31] = 0x00;
            _PLCCommand[32] = 0x03;
            // 按位计算的长度
            _PLCCommand[33] = (byte)(buffer.Length / 256);
            _PLCCommand[34] = (byte)(buffer.Length % 256);

            buffer.CopyTo( _PLCCommand, 35 );

            result.Content = _PLCCommand;
            result.IsSuccess = true;
            return result;
        }



        #endregion

        #region Read OrderNumber

        /// <summary>
        /// 从PLC读取订货号信息
        /// </summary>
        /// <returns></returns>
        public OperateResult<string> ReadOrderNumber( )
        {
            OperateResult<string> result = new OperateResult<string>( );
            OperateResult<byte[]> read = ReadFromCoreServer( plcOrderNumber );
            if (read.IsSuccess)
            {
                if (read.Content.Length > 100)
                {
                    result.IsSuccess = true;
                    result.Content = Encoding.ASCII.GetString( read.Content, 71, 20 );
                }
            }

            if (!result.IsSuccess)
            {
                result.CopyErrorFromOther( read );
            }

            return result;
        }

        #endregion

        #region Read Support

        
        /// <summary>
        /// 从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，T100，C100以字节为单位
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <param name="length">读取的数量，以字节为单位</param>
        /// <returns></returns>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            OperateResult<byte, int, ushort> addressResult = AnalysisAddress( address );
            if (!addressResult.IsSuccess)
            {
                return new OperateResult<byte[]>( )
                {
                    Message = addressResult.Message
                };
            }

            List<byte[]> bytesContent = new List<byte[]>( );
            ushort alreadyFinished = 0;
            while (alreadyFinished < length)
            {
                ushort readLength = (ushort)Math.Min( length - alreadyFinished, 200 );
                OperateResult<byte[]> read = Read( new OperateResult<byte, int, ushort>[] { addressResult }, new ushort[] { readLength } );
                if (read.IsSuccess)
                {
                    bytesContent.Add( read.Content );
                }
                else
                {
                    return read;
                }

                alreadyFinished += readLength;
                addressResult.Content2 += readLength * 8;
            }

            byte[] buffer = new byte[bytesContent.Sum( m => m.Length )];
            int current = 0;
            for (int i = 0; i < bytesContent.Count; i++)
            {
                bytesContent[i].CopyTo( buffer, current );
                current += bytesContent[i].Length;
            }

            return OperateResult.CreateSuccessResult( buffer );
        }

        /// <summary>
        /// 从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，以位为单位
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        private OperateResult<byte[]> ReadBitFromPLC( string address )
        {
            OperateResult<byte[]> result = new OperateResult<byte[]>( );

            OperateResult<byte[]> command = BuildBitReadCommand( address );
            if (!command.IsSuccess)
            {
                result.CopyErrorFromOther( command );
                return result;
            }

            OperateResult<byte[]> read = ReadFromCoreServer( command.Content );
            if (read.IsSuccess)
            {
                int receiveCount = 1;

                if (read.Content.Length >= 21 && read.Content[20] == 1)
                {
                    // 分析结果
                    byte[] buffer = new byte[receiveCount];

                    if (22 < read.Content.Length)
                    {
                        if (read.Content[21] == 0xFF &&
                            read.Content[22] == 0x03)
                        {
                            // 有数据
                            buffer[0] = read.Content[25];
                        }
                    }

                    result.Content = buffer;
                    result.IsSuccess = true;
                }
                else
                {
                    result.ErrorCode = read.ErrorCode;
                    result.Message = "数据块长度校验失败";
                }
            }
            else
            {
                result.ErrorCode = read.ErrorCode;
                result.Message = read.Message;
            }

            return result;
        }


        /// <summary>
        /// 一次性从PLC获取所有的数据，按照先后顺序返回一个统一的Buffer，需要按照顺序处理，两个数组长度必须一致
        /// </summary>
        /// <param name="address">起始地址数组</param>
        /// <param name="length">数据长度数组</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public OperateResult<byte[]> Read( string[] address, ushort[] length )
        {
            OperateResult<byte[]> result = new OperateResult<byte[]>( );
            OperateResult<byte, int, ushort>[] addressResult = new OperateResult<byte, int, ushort>[address.Length];
            for (int i = 0; i < address.Length; i++)
            {
                OperateResult<byte, int, ushort> tmp = AnalysisAddress( address[i] );
                if (!tmp.IsSuccess)
                {
                    result.CopyErrorFromOther( tmp );
                    return result;
                }

                addressResult[i] = tmp;
            }

            return Read( addressResult, length );
        }

        private OperateResult<byte[]> Read( OperateResult<byte, int, ushort>[] address, ushort[] length )
        {
            OperateResult<byte[]> result = new OperateResult<byte[]>( );

            OperateResult<byte[]> command = BuildReadCommand( address, length );
            if (!command.IsSuccess)
            {
                result.CopyErrorFromOther( command );
                return result;
            }

            OperateResult<byte[]> read = ReadFromCoreServer( command.Content );
            if (read.IsSuccess)
            {
                int receiveCount = 0;
                for (int i = 0; i < length.Length; i++)
                {
                    receiveCount += length[i];
                }

                if (read.Content.Length >= 21 && read.Content[20] == length.Length)
                {
                    // 分析结果
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
                                // 有数据
                                Array.Copy( read.Content, ii + 4, buffer, ll, length[kk] );
                                ii += length[kk] + 3;
                                ll += length[kk];
                                kk++;
                            }
                        }
                    }

                    result.Content = buffer;
                    result.IsSuccess = true;
                }
                else
                {
                    result.ErrorCode = read.ErrorCode;
                    result.Message = "数据块长度校验失败";
                }
            }
            else
            {
                result.ErrorCode = read.ErrorCode;
                result.Message = read.Message;
            }

            return result;
        }





        /// <summary>
        /// 读取指定地址的bool数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<bool> ReadBool( string address )
        {
            return GetBoolResultFromBytes( ReadBitFromPLC( address ) );
        }


        /// <summary>
        /// 读取指定地址的byte数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<byte> ReadByte( string address )
        {
            return GetByteResultFromBytes( Read( address, 1 ) );
        }
        

        #endregion

        #region Write Base


        /// <summary>
        /// 基础的写入数据的操作支持
        /// </summary>
        /// <param name="entireValue">完整的字节数据</param>
        /// <returns>写入结果</returns>
        private OperateResult WriteBase( byte[] entireValue )
        {
            OperateResult<byte[]> write = ReadFromCoreServer( entireValue );
            if (!write.IsSuccess) return write;

            if (write.Content[write.Content.Length - 1] != 0xFF)
            {
                // 写入异常
                return new OperateResult( )
                {
                    ErrorCode = write.Content[write.Content.Length - 1],
                    Message = "写入数据异常"
                };
            }
            else
            {
                return OperateResult.CreateSuccessResult( );
            }
        }


        /// <summary>
        /// 将数据写入到PLC数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <param name="value">写入的数据，长度根据data的长度来指示</param>
        /// <returns>写入结果</returns>
        public override OperateResult Write( string address, byte[] value )
        {
            OperateResult<byte[]> command = BuildWriteByteCommand( address, value );
            if (!command.IsSuccess) return command;

            return WriteBase( command.Content );
        }


        /// <summary>
        /// 写入PLC的一个位，例如"M100.6"，"I100.7"，"Q100.0"，"DB20.100.0"，如果只写了"M100"默认为"M100.0
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <param name="value">写入的数据，True或是False</param>
        /// <returns>写入结果</returns>
        public OperateResult Write( string address, bool value )
        {
            // 生成指令
            OperateResult<byte[]> command = BuildWriteBitCommand( address, value );
            if (!command.IsSuccess) return command;

            return WriteBase( command.Content );
        }


        #endregion

        #region Write String

        

        /// <summary>
        /// 向PLC中写入指定长度的字符串,超出截断，不够补0，编码格式为ASCII
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>写入结果</returns>
        public OperateResult Write(string address, string value, int length)
        {
            byte[] temp = ByteTransform.TransByte( value, Encoding.ASCII );
            temp = BasicFramework.SoftBasic.ArrayExpandToLength( temp, length );
            return Write( address, temp );
        }

        /// <summary>
        /// 向PLC中写入字符串，编码格式为Unicode
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>写入结果</returns>
        public OperateResult WriteUnicodeString(string address, string value)
        {
            byte[] temp = Encoding.Unicode.GetBytes( value );
            return Write( address, temp );
        }

        /// <summary>
        /// 向PLC中写入指定长度的字符串,超出截断，不够补0，编码格式为Unicode
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>写入结果</returns>
        public OperateResult WriteUnicodeString(string address, string value, int length)
        {
            byte[] temp = Encoding.Unicode.GetBytes( value );
            temp = BasicFramework.SoftBasic.ArrayExpandToLength( temp, length * 2 );
            return Write( address, temp );
        }

        #endregion

        #region Write bool[]

        /// <summary>
        /// 向PLC中写入bool数组，返回值说明，比如你写入M100,那么data[0]对应M100.0
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据，长度为8的倍数</param>
        /// <returns>写入结果</returns>
        public OperateResult Write(string address, bool[] values)
        {
            return Write( address, BasicFramework.SoftBasic.BoolArrayToByte( values ) );
        }


        #endregion

        #region Write Byte

        /// <summary>
        /// 向PLC中写入byte数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>写入结果</returns>
        public OperateResult Write(string address, byte value)
        {
            return Write( address, new byte[] { value } );
        }

        #endregion

        #region Head Codes

        private byte[] plcHead1 = new byte[22]
        {
                0x03,  // 01 RFC1006 Header
                0x00,  // 02 通常为 0
                0x00,  // 03 数据长度，高位
                0x16,  // 04 数据长度，地位
                0x11,  // 05 连接类型0x11:tcp  0x12 ISO-on-TCP
                0xE0,  // 06 主动建立连接
                0x00,  // 07 本地接口ID
                0x00,  // 08 主动连接时为0
                0x00,  // 09 该参数未使用
                0x01,  // 10 
                0x00,  // 11
                0xC0,  // 12
                0x01,  // 13
                0x0A,  // 14
                0xC1,  // 15
                0x02,  // 16
                0x01,  // 17
                0x02,  // 18
                0xC2,  // 19 指示cpu
                0x02,  // 20
                0x01,  // 21
                0x00   // 22
        };
        private byte[] plcHead2 = new byte[25]
        {
                0x03,
                0x00,
                0x00,
                0x19,
                0x02,
                0xF0,
                0x80,
                0x32,
                0x01,
                0x00,
                0x00,
                0x04,
                0x00,
                0x00,
                0x08,
                0x00,
                0x00,
                0xF0,  // 设置通讯
                0x00,
                0x00,
                0x01,
                0x00,
                0x01,
                0x01,
                0xE0
        };
        private byte[] plcOrderNumber = new byte[]
        {
            0x03,
            0x00,
            0x00,
            0x21,
            0x02,
            0xF0,
            0x80,
            0x32,
            0x07,
            0x00,
            0x00,
            0x00,
            0x01,
            0x00,
            0x08,
            0x00,
            0x08,
            0x00,
            0x01,
            0x12,
            0x04,
            0x11,
            0x44,
            0x01,
            0x00,
            0xFF,
            0x09,
            0x00,
            0x04,
            0x00,
            0x11,
            0x00,
            0x00
        };
        private SiemensPLCS CurrentPlc = SiemensPLCS.S1200;
        private byte[] plcHead1_200smart = new byte[22]
        {
            0x03,  // 01 RFC1006 Header             
            0x00,  // 02 通常为 0             
            0x00,  // 03 数据长度，高位            
            0x16,  // 04 数据长度，地位           
            0x11,  // 05 连接类型0x11:tcp  0x12 ISO-on-TCP               
            0xE0,  // 06 主动建立连接              
            0x00,  // 07 本地接口ID               
            0x00,  // 08 主动连接时为0              
            0x00,  // 09 该参数未使用              
            0x01,  // 10            
            0x00,  // 11          
            0xC1,  // 12           
            0x02,  // 13          
            0x10,  // 14             
            0x00,  // 15            
            0xC2,  // 16             
            0x02,  // 17           
            0x03,  // 18            
            0x00,  // 19 指示cpu     
            0xC0,  // 20              
            0x01,  // 21            
            0x0A   // 22       
        };
        private byte[] plcHead2_200smart = new byte[25]
        {
            0x03,
            0x00,
            0x00,
            0x19,
            0x02,
            0xF0,
            0x80,
            0x32,
            0x01,
            0x00,
            0x00,
            0xCC,
            0xC1,
            0x00,
            0x08,
            0x00,
            0x00,
            0xF0,  // 设置通讯      
            0x00,
            0x00,
            0x01,
            0x00,
            0x01,
            0x03,
            0xC0
        };

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString()
        {
            return "SiemensS7Net";
        }

        #endregion

    }
}
