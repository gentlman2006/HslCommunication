using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core
{
    /// <summary>
    /// 所有的和设备或是交互类统一读写标准
    /// </summary>
    /// <remarks>
    /// Modbus类，PLC类均实现了本接口，可以基于本接口实现统一所有的不同种类的设备的数据交互
    /// </remarks>
    /// <example>
    /// 此处举例实现modbus，三菱，西门子三种设备的统一的数据交互
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\IReadWriteNet.cs" region="IReadWriteNetExample" title="IReadWriteNet示例" />
    /// </example>
    public interface IReadWriteNet
    {
        #region Read Support

        /// <summary>
        /// 批量读取底层的数据信息，需要指定地址和长度，具体的结果取决于实现
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="length">数据长度</param>
        /// <returns>带有成功标识的byte[]数组</returns>
        OperateResult<byte[]> Read( string address, ushort length );
        /// <summary>
        /// 读取16位的有符号整型
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的short数据</returns>
        OperateResult<short> ReadInt16( string address );
        /// <summary>
        /// 读取16位的有符号整型数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>带有成功标识的short数组</returns>
        OperateResult<short[]> ReadInt16( string address, ushort length );
        /// <summary>
        /// 读取16位的无符号整型
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的ushort数据</returns>
        OperateResult<ushort> ReadUInt16( string address );
        /// <summary>
        /// 读取16位的无符号整型数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>带有成功标识的ushort数组</returns>
        OperateResult<ushort[]> ReadUInt16( string address, ushort length );
        /// <summary>
        /// 读取32位的有符号整型
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的int数据</returns>
        OperateResult<int> ReadInt32( string address );
        /// <summary>
        /// 读取32位有符号整型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        OperateResult<int[]> ReadInt32( string address, ushort length );

        /// <summary>
        /// 读取32位的无符号整型
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的uint数据</returns>
        OperateResult<uint> ReadUInt32( string address );
        /// <summary>
        /// 读取设备的uint类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        OperateResult<uint[]> ReadUInt32( string address, ushort length );
        /// <summary>
        /// 读取64位的有符号整型
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的long数据</returns>
        OperateResult<long> ReadInt64( string address );
        /// <summary>
        /// 读取64位的有符号整型数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        OperateResult<long[]> ReadInt64( string address, ushort length );
        /// <summary>
        /// 读取64位的无符号整型
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的ulong数据</returns>
        OperateResult<ulong> ReadUInt64( string address );
        /// <summary>
        /// 读取64位的无符号整型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        OperateResult<ulong[]> ReadUInt64( string address, ushort length );
        /// <summary>
        /// 读取单浮点精度的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的float数据</returns>
        OperateResult<float> ReadFloat( string address );
        /// <summary>
        /// 读取单浮点精度的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        OperateResult<float[]> ReadFloat( string address, ushort length );
        /// <summary>
        /// 读取双浮点精度的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的double数据</returns>
        OperateResult<double> ReadDouble( string address );
        /// <summary>
        /// 读取双浮点精度的数据的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        OperateResult<double[]> ReadDouble( string address, ushort length );
        /// <summary>
        /// 读取字符串数据，
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数据长度</param>
        /// <returns>带有成功标识的string数据</returns>
        OperateResult<string> ReadString( string address, ushort length );
        /// <summary>
        /// 读取自定义的数据类型，需要继承自IDataTransfer接口
        /// </summary>
        /// <typeparam name="T">自定义的类型</typeparam>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的自定义类型数据</returns>
        OperateResult<T> ReadCustomer<T>( string address ) where T : IDataTransfer, new();


        #endregion

        #region Write Support


        /// <summary>
        /// 写入short数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write( string address, short value );
        /// <summary>
        /// 写入short数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write( string address, short[] values );
        /// <summary>
        /// 写入int数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write( string address, int value );
        /// <summary>
        /// 写入int[]数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write( string address, int[] values );
        /// <summary>
        /// 写入long数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write( string address, long value );
        /// <summary>
        /// 写入long数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write( string address, long[] values );
        /// <summary>
        /// 写入float数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write( string address, float value );
        /// <summary>
        /// 写入float数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write( string address, float[] values );
        /// <summary>
        /// 写入double数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write( string address, double value );
        /// <summary>
        /// 写入double数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write( string address, double[] values );
        /// <summary>
        /// 写入字符串信息，编码为ASCII
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write( string address, string value );
        /// <summary>
        /// 写入指定长度的字符串信息，编码为ASCII
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <param name="length">字符串的长度</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult Write( string address, string value, int length );
        /// <summary>
        /// 写入自定义类型的数据，该类型必须继承自IDataTransfer接口
        /// </summary>
        /// <typeparam name="T">类型对象</typeparam>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        OperateResult WriteCustomer<T>( string address, T value ) where T : IDataTransfer, new();
        
        #endregion

    }
}
