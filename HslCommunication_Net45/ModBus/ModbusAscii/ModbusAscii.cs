using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.ModBus
{
    /// <summary>
    /// Modbus-Ascii通讯协议的类库，基于rtu类库完善过来
    /// </summary>
    /// <remarks>
    /// 本客户端支持的标准的modbus-tcp协议，内置的消息号会进行自增，地址格式采用富文本表示形式
    /// <note type="important">
    /// 地址共可以携带3个信息，最完整的表示方式"s=2;x=3;100"，对应的modbus报文是 02 03 00 64 00 01 的前四个字节，站号，功能码，起始地址，下面举例
    /// <list type="definition">
    /// <item>
    /// <term>读取线圈</term>
    /// <description>ReadCoil("100")表示读取线圈100的值，ReadCoil("s=2;100")表示读取站号为2，线圈地址为100的值</description>
    /// </item>
    /// <item>
    /// <term>读取离散输入</term>
    /// <description>ReadDiscrete("100")表示读取离散输入100的值，ReadDiscrete("s=2;100")表示读取站号为2，离散地址为100的值</description>
    /// </item>
    /// <item>
    /// <term>读取寄存器</term>
    /// <description>ReadInt16("100")表示读取寄存器100的值，ReadInt16("s=2;100")表示读取站号为2，寄存器100的值</description>
    /// </item>
    /// <item>
    /// <term>读取输入寄存器</term>
    /// <description>ReadInt16("x=4;100")表示读取输入寄存器100的值，ReadInt16("s=2;x=4;100")表示读取站号为2，输入寄存器100的值</description>
    /// </item>
    /// </list>
    /// 对于写入来说也是一致的
    /// <list type="definition">
    /// <item>
    /// <term>写入线圈</term>
    /// <description>WriteCoil("100",true)表示读取线圈100的值，WriteCoil("s=2;100",true)表示读取站号为2，线圈地址为100的值</description>
    /// </item>
    /// <item>
    /// <term>写入寄存器</term>
    /// <description>Write("100",(short)123)表示写寄存器100的值123，Write("s=2;100",(short)123)表示写入站号为2，寄存器100的值123</description>
    /// </item>
    /// </list>
    /// </note>
    /// </remarks>
    /// <example>
    /// 基本的用法请参照下面的代码示例，初始化部分的代码省略
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Modbus\ModbusAsciiExample.cs" region="Example" title="Modbus示例" />
    /// 复杂的读取数据的代码示例如下：
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Modbus\ModbusAsciiExample.cs" region="ReadExample" title="read示例" />
    /// 写入数据的代码如下：
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Modbus\ModbusAsciiExample.cs" region="WriteExample" title="write示例" />
    /// </example>
    public class ModbusAscii : ModbusRtu
    {
        #region Constructor
        
        /// <summary>
        /// 实例化一个Modbus-ascii协议的客户端对象
        /// </summary>
        public ModbusAscii( )
        {

        }

        /// <summary>
        /// 指定服务器地址，端口号，客户端自己的站号来初始化
        /// </summary>
        /// <param name="station">站号</param>
        public ModbusAscii( byte station = 0x01 ) : base( station )
        {

        }

        #endregion

        #region Modbus Rtu Override
        
        /// <summary>
        /// 检查当前的Modbus-Ascii响应是否是正确的
        /// </summary>
        /// <param name="send">发送的数据信息</param>
        /// <returns>带是否成功的结果数据</returns>
        protected override OperateResult<byte[]> CheckModbusTcpResponse( byte[] send )
        {
            // 转ascii
            byte[] modbus_ascii = ModbusInfo.TransRtuToAsciiPackCommand( send );

            // 核心交互
            OperateResult<byte[]> result = ReadBase( modbus_ascii );
            if (!result.IsSuccess) return result;

            // 还原modbus报文
            OperateResult<byte[]> modbus_core = ModbusInfo.TransAsciiPackCommandToRtu( result.Content );
            if (!modbus_core.IsSuccess) return modbus_core;

            // 发生了错误
            if ((send[1] + 0x80) == modbus_core.Content[1]) return new OperateResult<byte[]>( modbus_core.Content[2], ModbusInfo.GetDescriptionByErrorCode( modbus_core.Content[2] ) );

            // 成功的消息
            return OperateResult.CreateSuccessResult( modbus_core.Content );
        }

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString( )
        {
            return "ModbusAscii";
        }

        #endregion
    }
}
