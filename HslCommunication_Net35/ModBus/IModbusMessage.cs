using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.ModBus
{
    /// <summary>
    /// Modbus tcp的消息接口
    /// </summary>
    public interface IModbusMessage
    {
        ushort MessageId { get; set; }


    }
}
