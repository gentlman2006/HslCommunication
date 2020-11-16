using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication
{
    /// <summary>
    /// 用于PLC通讯及ModBus自定义数据类型的读写操作
    /// </summary>
    /// <remarks>
    /// 主要应用于设备实现设备类的自定义的数据类型读写，以此达到简化代码的操作，但是有一个前提，该数据处于连续的数据区块
    /// </remarks>
    /// <example>
    /// 此处举例读取三菱的自定义的数据，先实现接口，然后再读写操作
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="IDataTransfer Example" title="DataMy示例" />
    /// 接下来就可以实现数据的读取了
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadCustomerExample" title="ReadCustomer示例" />
    /// </example>
    public interface IDataTransfer
    {
        /// <summary>
        /// 读取的数据长度，对于西门子，等同于字节数，对于三菱和Modbus为字节数的一半
        /// </summary>
        ushort ReadCount { get; }

        /// <summary>
        /// 从字节数组进行解析实际的对象
        /// </summary>
        /// <param name="Content">从远程读取的数据源</param>
        void ParseSource(byte[] Content);

        /// <summary>
        /// 将对象生成字符源，写入PLC中
        /// </summary>
        /// <returns>准备写入到远程的数据</returns>
        byte[] ToSource();
    }
}
