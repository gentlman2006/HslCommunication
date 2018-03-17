using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core.Net
{
    /// <summary>
    /// 所有的和设备或是交互类统一读写标准
    /// </summary>
    public interface IReadWriteNet
    {
        /// <summary>
        /// 读取16位的有符号整型
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的short数据</returns>
        OperateResult<short> ReadInt16(string address);
        /// <summary>
        /// 读取16位的无符号整型
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的ushort数据</returns>
        OperateResult<ushort> ReadUInt16(string address);
        /// <summary>
        /// 读取32位的有符号整型
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的int数据</returns>
        OperateResult<int> ReadInt32(string address);
        /// <summary>
        /// 读取32位的无符号整型
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的uint数据</returns>
        OperateResult<uint> ReadUInt32(string address);
        /// <summary>
        /// 读取64位的有符号整型
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的long数据</returns>
        OperateResult<long> ReadInt64(string address);
        /// <summary>
        /// 读取64位的无符号整型
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的ulong数据</returns>
        OperateResult<ulong> ReadUInt64(string address);
        /// <summary>
        /// 读取单浮点精度的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的float数据</returns>
        OperateResult<float> ReadFloat(string address);
        /// <summary>
        /// 读取双浮点精度的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的double数据</returns>
        OperateResult<double> ReadDouble(string address);
        /// <summary>
        /// 读取字符串数据，
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数据长度</param>
        /// <returns>带有成功标识的string数据</returns>
        OperateResult<string> ReadString(string address, int length);
        /// <summary>
        /// 读取自定义的数据类型，需要继承自IDataTransfer接口
        /// </summary>
        /// <typeparam name="T">自定义的类型</typeparam>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的自定义类型数据</returns>
        OperateResult<T> ReadCustomer<T>(string address) where T : IDataTransfer, new();




        /// <summary>
        /// 写入short数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write(string address, short value);
        OperateResult Write(string address, short[] values);
        OperateResult Write(string address, int value);
        OperateResult Write(string address, int[] values);
        OperateResult Write(string address, long value);
        OperateResult Write(string address, float value);
        OperateResult Write(string address, float[] values);
        OperateResult Write(string address, double value);
        OperateResult Write(string address, double[] values);
        OperateResult Write(string address, string value);
        OperateResult WriteCustomer<T>(string address, T value) where T : IDataTransfer, new();
    }
}
