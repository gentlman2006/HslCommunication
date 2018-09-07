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

        protected override OperateResult<byte[]> CheckModbusTcpResponse( byte[] send )
        {
            return base.CheckModbusTcpResponse( send ); 
        }
    }
}
