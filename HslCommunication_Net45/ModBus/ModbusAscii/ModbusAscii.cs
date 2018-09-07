using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.ModBus
{
    /// <summary>
    /// Modbus-Ascii通讯协议的类库，基于rtu类库完善过来
    /// </summary>
    public class ModbusAscii : ModbusRtu
    {
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
            if ((send[1] + 0x80) == modbus_core.Content[1]) return new OperateResult<byte[]>( ) { ErrorCode = modbus_core.Content[2], Message = ModbusInfo.GetDescriptionByErrorCode( modbus_core.Content[2] ) };

            // 成功的消息
            return OperateResult.CreateSuccessResult( modbus_core.Content );
        }
    }
}
