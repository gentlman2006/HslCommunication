using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core.Net
{
    /// <summary>
    /// 所有的和设备或是
    /// </summary>
    public interface IReadWriteNet
    {
        OperateResult<bool> ReadBool( string address );




    }
}
