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
 *    继承自统一的自定义方法
 * 
 * 
 *********************************************************************************/

namespace HslCommunication.Profinet.Siemens
{
    /// <summary>
    /// 使用了Fetch/Write协议来和西门子进行通讯，该种方法需要在PLC侧进行一些配置
    /// </summary>
    public class SiemensFetchWriteNet : NetworkDeviceBase<FetchWriteMessage, ReverseBytesTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化一个西门子的Fetch/Write协议的通讯对象
        /// </summary>
        public SiemensFetchWriteNet()
        {
            WordLength = 2;
        }

        /// <summary>
        /// 实例化一个西门子的Fetch/Write协议的通讯对象
        /// </summary>
        /// <param name="ipAddress">PLC的Ip地址</param>
        /// <param name="port">PLC的端口</param>
        public SiemensFetchWriteNet(string ipAddress,int port)
        {
            WordLength = 2;
            IpAddress = ipAddress;
            Port = port;
        }

        #endregion

        #region Address Analysis

        /// <summary>
        /// 计算特殊的地址信息
        /// </summary>
        /// <param name="address">字符串信息</param>
        /// <returns>实际值</returns>
        private int CalculateAddressStarted(string address)
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
        /// 解析数据地址，解析出地址类型，起始地址，DB块的地址
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns>解析出地址类型，起始地址，DB块的地址</returns>
        private OperateResult<byte, int, ushort> AnalysisAddress(string address)
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
                        result.Message = "DB块数据无法大于255";
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
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns>携带有命令字节</returns>
        private OperateResult<byte[]> BuildReadCommand(string address, ushort count)
        {
            var result = new OperateResult<byte[]>( );

            OperateResult<byte, int, ushort> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }

            byte[] _PLCCommand = new byte[16];
            _PLCCommand[0] = 0x53;
            _PLCCommand[1] = 0x35;
            _PLCCommand[2] = 0x10;
            _PLCCommand[3] = 0x01;
            _PLCCommand[4] = 0x03;
            _PLCCommand[5] = 0x05;
            _PLCCommand[6] = 0x03;
            _PLCCommand[7] = 0x08;

            //指定数据区
            _PLCCommand[8] = analysis.Content1;
            _PLCCommand[9] = (byte)analysis.Content3;

            //指定数据地址
            _PLCCommand[10] = (byte)(analysis.Content2 / 256);
            _PLCCommand[11] = (byte)(analysis.Content2 % 256);

            if (analysis.Content1 == 0x01 || analysis.Content1 == 0x06 || analysis.Content1 == 0x07)
            {
                if (count % 2 != 0)
                {
                    result.Message = "读取的数据长度必须为偶数";
                    return result;
                }
                else
                {
                    //指定数据长度
                    _PLCCommand[12] = (byte)(count / 2 / 256);
                    _PLCCommand[13] = (byte)(count / 2 % 256);
                }
            }
            else
            {
                //指定数据长度
                _PLCCommand[12] = (byte)(count / 256);
                _PLCCommand[13] = (byte)(count % 256);
            }

            _PLCCommand[14] = 0xff;
            _PLCCommand[15] = 0x02;

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
        private OperateResult<byte[]> BuildWriteByteCommand(string address, byte[] data)
        {
            if (data == null) data = new byte[0];
            var result = new OperateResult<byte[]>( );

            OperateResult<byte, int, ushort> analysis = AnalysisAddress( address );
            if (!analysis.IsSuccess)
            {
                result.CopyErrorFromOther( analysis );
                return result;
            }

           

            byte[] _PLCCommand = new byte[16 + data.Length];
            _PLCCommand[0] = 0x53;
            _PLCCommand[1] = 0x35;
            _PLCCommand[2] = 0x10;
            _PLCCommand[3] = 0x01;
            _PLCCommand[4] = 0x03;
            _PLCCommand[5] = 0x03;
            _PLCCommand[6] = 0x03;
            _PLCCommand[7] = 0x08;

            //指定数据区
            _PLCCommand[8] = analysis.Content1;
            _PLCCommand[9] = (byte)analysis.Content3;

            //指定数据地址
            _PLCCommand[10] = (byte)(analysis.Content2 / 256);
            _PLCCommand[11] = (byte)(analysis.Content2 % 256);

            if (analysis.Content1 == 0x01 || analysis.Content1 == 0x06 || analysis.Content1 == 0x07)
            {
                if (data.Length %2 != 0)
                {
                    result.Message = "写入的数据长度必须为偶数";
                    return result;
                }
                else
                {
                    //指定数据长度
                    _PLCCommand[12] = (byte)(data.Length / 2 / 256);
                    _PLCCommand[13] = (byte)(data.Length / 2 % 256);
                }
            }
            else
            {
                //指定数据长度
                _PLCCommand[12] = (byte)(data.Length / 256);
                _PLCCommand[13] = (byte)(data.Length % 256);
            }
            
            _PLCCommand[14] = 0xff;
            _PLCCommand[15] = 0x02;

            //放置数据
            Array.Copy( data, 0, _PLCCommand, 16, data.Length );

            result.Content = _PLCCommand;
            result.IsSuccess = true;
            return result;
        }




        #endregion

        #region Read Support


        /// <summary>
        /// 从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，T100，C100，以字节为单位
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100，T100，C100，</param>
        /// <param name="length">读取的数量，以字节为单位</param>
        /// <returns>带有成功标志的字节信息</returns>
        public override OperateResult<byte[]> Read(string address, ushort length)
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
                if (read.Content[8] == 0x00)
                {
                    // 分析结果
                    byte[] buffer = new byte[read.Content.Length - 16];
                    Array.Copy( read.Content, 16, buffer, 0, buffer.Length );

                    result.Content = buffer;
                    result.IsSuccess = true;
                }
                else
                {
                    result.ErrorCode = read.Content[8];
                    result.Message = "发生了异常，具体信息查找Fetch/Write协议文档";
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
        /// 读取指定地址的byte数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<byte> ReadByte(string address)
        {
            return GetByteResultFromBytes( Read( address, 1 ) );
        }
        

        #endregion

        #region Write Base


        /// <summary>
        /// 将数据写入到PLC数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <param name="value">写入的数据，长度根据data的长度来指示</param>
        /// <returns></returns>
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult result = new OperateResult( );

            OperateResult<byte[]> command = BuildWriteByteCommand( address, value );
            if (!command.IsSuccess)
            {
                result.CopyErrorFromOther( command );
                return result;
            }


            OperateResult<byte[]> write = ReadFromCoreServer( command.Content );
            if (write.IsSuccess)
            {
                if (write.Content[8] != 0x00)
                {
                    // 写入异常
                    result.Message = "写入数据异常，代号为：" + write.Content[8].ToString( );
                }
                else
                {
                    result.IsSuccess = true;  // 写入成功
                }
            }
            else
            {
                result.ErrorCode = write.ErrorCode;
                result.Message = write.Message;
            }
            return result;
        }



        #endregion

        #region Write String
        

        /// <summary>
        /// 向PLC中写入指定长度的字符串,超出截断，不够补0，编码格式为ASCII
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>返回读取结果</returns>
        public OperateResult Write(string address, string value, int length)
        {
            byte[] temp = Encoding.ASCII.GetBytes( value );
            temp = BasicFramework.SoftBasic.ArrayExpandToLength( temp, length );
            return Write( address, temp );
        }

        /// <summary>
        /// 向PLC中写入字符串，编码格式为Unicode
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回读取结果</returns>
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
        /// <returns>返回读取结果</returns>
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
        /// <returns>返回写入结果</returns>
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
        /// <returns></returns>
        public OperateResult Write(string address, byte value)
        {
            return Write( address, new byte[] { value } );
        }

        #endregion
        
        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString()
        {
            return "SiemensFetchWriteNet";
        }

        #endregion
        
    }
}
