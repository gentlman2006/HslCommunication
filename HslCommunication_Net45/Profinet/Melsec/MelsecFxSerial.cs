using HslCommunication.Core;
using HslCommunication.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.Melsec
{
    /// <summary>
    /// 三菱的串口通信的对象，适用于读取FX系列的串口数据
    /// </summary>
    public class MelsecFxSerial : SerialDeviceBase<RegularByteTransform>
    {

        #region Constructor

        /// <summary>
        /// 实例化三菱的串口协议的通讯对象
        /// </summary>
        public MelsecFxSerial( )
        {
            WordLength = 1;
        }
        
        
        #endregion

        #region Check Response
        

        private OperateResult CheckPlcReadResponse(byte[] ack )
        {
            if (ack.Length == 0) return new OperateResult( ) { Message = "receive data length : 0" };
            if (ack[0] == 0x15) return new OperateResult( ) { Message = "plc ack nagative" };
            if (ack[0] != 0x02) return new OperateResult( ) { Message = "plc ack wrong : " + ack[0] };
            
            if (!MelsecHelper.CheckCRC( ack ))
            {
                return new OperateResult( ) { Message = "Check CRC Failed" };
            }
            else
            {
                return OperateResult.CreateSuccessResult( );
            }
        }

        private OperateResult CheckPlcWriteResponse( byte[] ack )
        {
            if (ack.Length == 0) return new OperateResult( ) { Message = "receive data length : 0" };
            if (ack[0] == 0x15) return new OperateResult( ) { Message = "plc ack nagative" };
            if (ack[0] != 0x06) return new OperateResult( ) { Message = "plc ack wrong : " + ack[0] };

            return OperateResult.CreateSuccessResult( );
        }


        /// <summary>
        /// 从PLC反馈的数据进行提炼操作
        /// </summary>
        /// <param name="response">PLC反馈的真实数据</param>
        /// <returns>数据提炼后的真实数据</returns>
        private OperateResult<byte[]> ExtractActualData( byte[] response )
        {
            try
            {
                byte[] data = new byte[(response.Length - 4) / 2];
                for (int i = 0; i < data.Length; i++)
                {
                    byte[] buffer = new byte[2];
                    buffer[0] = response[i * 2 + 1];
                    buffer[1] = response[i * 2 + 2];

                    data[i] = Convert.ToByte( Encoding.ASCII.GetString( buffer ), 16 );
                }

                return OperateResult.CreateSuccessResult( data );
            }
            catch(Exception ex)
            {
                return new OperateResult<byte[]>( ) { Message = "Extract Msg：" + ex.Message + 
                    Environment.NewLine + "Data: " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( response ) };
            }
        }


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
        ///     <term>地址范围</term>
        ///     <term>地址进制</term>
        ///   </listheader>
        ///   <item>
        ///     <term>数据寄存器</term>
        ///     <term>D100,D200</term>
        ///     <term>D0-D511,D8000-D8255</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>定时器的值</term>
        ///     <term>T10,T20</term>
        ///     <term>T0-T255</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>计数器的值</term>
        ///     <term>C10,C20</term>
        ///     <term>C0-C199,C200-C255</term>
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
            OperateResult<byte[]> command = MelsecHelper.FxBuildReadWordCommand( address, length );
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( command );

            // 核心交互
            OperateResult<byte[]> read = ReadBase( command.Content );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( read );

            // 反馈检查
            OperateResult ackResult = CheckPlcReadResponse( read.Content );
            if (!ackResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( ackResult );

            // 数据提炼
            return ExtractActualData( read.Content );
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
        ///     <term>地址范围</term>
        ///     <term>地址进制</term>
        ///   </listheader>
        ///   <item>
        ///     <term>内部继电器</term>
        ///     <term>M100,M200</term>
        ///     <term>M0-M1023,M8000-M8255</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>输入继电器</term>
        ///     <term>X100,X1A0</term>
        ///     <term>X0-X177</term>
        ///     <term>8</term>
        ///   </item>
        ///   <item>
        ///     <term>输出继电器</term>
        ///     <term>Y10,Y20</term>
        ///     <term>Y0-Y177</term>
        ///     <term>8</term>
        ///   </item>
        ///   <item>
        ///     <term>步进继电器</term>
        ///     <term>S100,S200</term>
        ///     <term>S0-S999</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>定时器</term>
        ///     <term>T10,T20</term>
        ///     <term>T0-T255</term>
        ///     <term>10</term>
        ///   </item>
        ///   <item>
        ///     <term>计数器</term>
        ///     <term>C10,C20</term>
        ///     <term>C0-C255</term>
        ///     <term>10</term>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <example>
        ///  <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="ReadBool" title="Bool类型示例" />
        /// </example>
        public OperateResult<bool[]> ReadBool( string address, ushort length )
        {
            //获取指令
            OperateResult<byte[], int> command = MelsecHelper.FxBuildReadBoolCommand( address, length );
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( command );

            // 核心交互
            OperateResult<byte[]> read = ReadBase( command.Content1 );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( read );

            // 结果严重
            OperateResult ackResult = CheckPlcReadResponse( read.Content );
            if (!ackResult.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( ackResult );

            // 提取真实的数据
            OperateResult<byte[]> extraResult = ExtractActualData( read.Content );
            if(!extraResult.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( extraResult );

            // 转化bool数组
            bool[] data = new bool[length];
            bool[] array = BasicFramework.SoftBasic.ByteToBoolArray( extraResult.Content, extraResult.Content.Length * 8 );
            for (int i = 0; i < length; i++)
            {
                array[i] = array[i + command.Content2];
            }

            return OperateResult.CreateSuccessResult( data );
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
        /// <returns>是否写入成功的结果对象</returns>
        public override OperateResult Write( string address, byte[] value )
        {
            // 获取写入
            OperateResult<byte[]> command = MelsecHelper.BuildWriteWordCommand( address, value );
            if (!command.IsSuccess) return command;

            // 核心交互
            OperateResult<byte[]> read = ReadBase( command.Content );
            if (read.IsSuccess) return read;
            
            // 结果验证
            OperateResult checkResult = CheckPlcWriteResponse( read.Content );
            if (!checkResult.IsSuccess) return checkResult;

            return OperateResult.CreateSuccessResult( );
        }




        #endregion

        #region Write Bool

        /// <summary>
        /// 强制写入位数据的通断，支持的类型为X,Y,M,S,C,T
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="value">是否为通</param>
        /// <returns>是否写入成功的结果对象</returns>
        public OperateResult Write( string address, bool value )
        {
            // 先获取指令
            OperateResult<byte[]> command = MelsecHelper.FxBuildWriteBoolPacket( address, value );
            if (!command.IsSuccess) return command;

            // 和串口进行核心的数据交互
            OperateResult<byte[]> read = ReadBase( command.Content );
            if (!read.IsSuccess) return read;
                
            // 检查结果是否正确
            OperateResult checkResult = CheckPlcWriteResponse( read.Content );
            if (!checkResult.IsSuccess) return checkResult;

            return OperateResult.CreateSuccessResult( );
        }

        #endregion

        #region Write String


        /// <summary>
        /// 向PLC中字软元件写入指定长度的字符串,超出截断，不够补0，编码格式为ASCII
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>是否写入成功的结果对象</returns>
        public OperateResult Write( string address, string value, int length )
        {
            byte[] temp = Encoding.ASCII.GetBytes( value );
            temp = BasicFramework.SoftBasic.ArrayExpandToLength( temp, length );
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
        /// <returns>是否写入成功的结果对象</returns>
        public OperateResult WriteUnicodeString( string address, string value, int length )
        {
            byte[] temp = Encoding.Unicode.GetBytes( value );
            temp = BasicFramework.SoftBasic.ArrayExpandToLength( temp, length * 2 );
            return Write( address, temp );
        }

        #endregion

        #region Object Override

        /// <summary>
        /// 获取当前对象的字符串标识形式
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString( )
        {
            return "MelsecFxSerial";
        }

        #endregion
        
    }
}
