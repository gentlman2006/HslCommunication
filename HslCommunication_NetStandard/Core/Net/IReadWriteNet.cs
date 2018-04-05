using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core
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
        OperateResult<string> ReadString(string address, ushort length);
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
        /// <summary>
        /// 写入ushort数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write(string address, short[] values);
        /// <summary>
        /// 写入int数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write(string address, int value);
        /// <summary>
        /// 写入int[]数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write(string address, int[] values);
        /// <summary>
        /// 写入long数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write(string address, long value);
        /// <summary>
        /// 写入long数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write(string address, long[] values);
        /// <summary>
        /// 写入float数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write(string address, float value);
        /// <summary>
        /// 写入float数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write(string address, float[] values);
        /// <summary>
        /// 写入double数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write(string address, double value);
        /// <summary>
        /// 写入double数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write(string address, double[] values);
        /// <summary>
        /// 写入字符串信息，编码为ASCII
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write(string address, string value);
        /// <summary>
        /// 写入自定义类型的数据，该类型必须继承自IDataTransfer接口
        /// </summary>
        /// <typeparam name="T">类型对象</typeparam>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult WriteCustomer<T>(string address, T value) where T : IDataTransfer, new();
    }
}
