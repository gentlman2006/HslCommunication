using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication
{
    /// <summary>
    /// 用于PLC通讯及ModBus自定义数据类型的读写操作
    /// </summary>
    public interface IDataTransfer
    {
        /// <summary>
        /// 读取的数据长度，对于西门子，等同于字节数，对于三菱和Modbus为字节数的一半
        /// </summary>
        ushort ReadCount { get; }

        /// <summary>
        /// 从字节数组进行解析实际的对象
        /// </summary>
        /// <param name="Content"></param>
        void ParseSource(byte[] Content);

        /// <summary>
        /// 将对象生成字符源，写入PLC中
        /// </summary>
        /// <returns></returns>
        byte[] ToSource();
    }
}
