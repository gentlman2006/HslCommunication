package HslCommunication.Core.Net;

import HslCommunication.Core.Types.IDataTransfer;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.Core.Types.OperateResultExOne;

/**
 * 所有设备交互类的统一的读写接口
 */
public interface IReadWriteNet {

    /**
     * 批量读取底层的数据信息，需要指定地址和长度，具体的结果取决于实现
     * @param address 地址信息
     * @param length 数据长度
     * @return 带有成功标识的byte[]数组
     */
    OperateResultExOne<byte[]> Read(String address, short length );

    /**
     * 读取16位的有符号整型
     * @param address 起始地址
     * @return 带有成功标识的short数据
     */
    OperateResultExOne<Short> ReadInt16(String address);

    /**
     * 读取16位的有符号整型数组
     * @param address 起始地址
     * @param length 读取的数组长度
     * @return 带有成功标识的short数组
     */
    OperateResultExOne<short []> ReadInt16( String address, short length );

    /**
     * 读取32位的有符号整型
     * @param address 起始地址
     * @return 带有成功标识的int数据
     */
    OperateResultExOne<Integer> ReadInt32(String address);

    /**
     * 读取32位有符号整型的数组
     * @param address 起始地址
     * @param length 数组长度
     * @return 带成功标志的结果数据对象
     */
    OperateResultExOne<int[]> ReadInt32( String address, short length );

    /**
     * 读取64位的有符号整型
     * @param address 起始地址
     * @return 带有成功标识的long数据
     */
    OperateResultExOne<Long> ReadInt64(String address);

    /**
     * 读取64位的有符号整型数组
     * @param address 起始地址
     * @param length 数组长度
     * @return 带成功标志的结果数据对象
     */
    OperateResultExOne<long[]> ReadInt64( String address, short length );

    /**
     * 读取单浮点精度的数据
     * @param address 起始地址
     * @return 带有成功标识的float数据
     */
    OperateResultExOne<Float> ReadFloat(String address);

    /**
     * 读取单浮点精度的数组
     * @param address 起始地址
     * @param length 数组长度
     * @return 带成功标志的结果数据对象
     */
    OperateResultExOne<float[]> ReadFloat( String address, short length );

    /**
     * 读取双浮点精度的数据
     * @param address 起始地址
     * @return 带有成功标识的double数据
     */
    OperateResultExOne<Double> ReadDouble(String address);


    /**
     * 读取双浮点精度的数据的数组
     * @param address 起始地址
     * @param length 数组长度
     * @return 带成功标志的结果数据对象
     */
    OperateResultExOne<double[]> ReadDouble( String address, short length );

    /**
     * 读取字符串数据
     * @param address 起始地址
     * @param length 数据长度
     * @return 带有成功标识的string数据
     */
    OperateResultExOne<String> ReadString(String address, short length);


    /**
     * 读取自定义的数据类型，需要继承自IDataTransfer接口
     * @param address 起始地址
     * @param <T> 自定义的类型
     * @return 带有成功标识的自定义类型数据
     */
    <T extends IDataTransfer> OperateResultExOne<T> ReadCustomer(String address,Class<T> tClass);








    /**
     * 写入short数据
     * @param address 起始地址
     * @param value 写入值
     * @return 带有成功标识的结果类对象
     */
    OperateResult Write(String address, short value);

    /**
     * 写入short数组
     * @param address 起始地址
     * @param values 写入值
     * @return 带有成功标识的结果类对象
     */
    OperateResult Write(String address, short[] values);

    /**
     * 写入int数据
     * @param address 起始地址
     * @param value 写入值
     * @return 带有成功标识的结果类对象
     */
    OperateResult Write(String address, int value);

    /**
     * 写入int[]数组
     * @param address 起始地址
     * @param values 写入值
     * @return 带有成功标识的结果类对象
     */
    OperateResult Write(String address, int[] values);

    /**
     * 写入long数据
     * @param address 起始地址
     * @param value 写入值
     * @return 带有成功标识的结果类对象
     */
    OperateResult Write(String address, long value);

    /**
     * 写入long数组
     * @param address 起始地址
     * @param values 写入值
     * @return 带有成功标识的结果类对象
     */
    OperateResult Write(String address, long[] values);

    /**
     * 写入float数据
     * @param address 起始地址
     * @param value 写入值
     * @return 带有成功标识的结果类对象
     */
    OperateResult Write(String address, float value);

    /**
     * 写入float数组
     * @param address 起始地址
     * @param values 写入值
     * @return 带有成功标识的结果类对象
     */
    OperateResult Write(String address, float[] values);

    /**
     * 写入double数据
     * @param address 起始地址
     * @param value 写入值
     * @return 带有成功标识的结果类对象
     */
    OperateResult Write(String address, double value);

    /**
     * 写入double数组
     * @param address 起始地址
     * @param values 写入值
     * @return 带有成功标识的结果类对象
     */
    OperateResult Write(String address, double[] values);

    /**
     * 写入字符串信息，编码为ASCII
     * @param address 起始地址
     * @param value 写入值
     * @return 带有成功标识的结果类对象
     */
    OperateResult Write(String address, String value);


    /**
     * 写入字符串信息，编码为ASCII
     * @param address 起始地址
     * @param value 写入值
     * @param length 写入的字符串的长度
     * @return 带有成功标识的结果类对象
     */
    OperateResult Write(String address, String value, int length);

    /**
     * 写入自定义类型的数据，该类型必须继承自IDataTransfer接口
     * @param address 起始地址
     * @param value 写入值
     * @param <T> 类型对象
     * @return 带有成功标识的结果类对象
     */
    <T extends IDataTransfer> OperateResult WriteCustomer(String address, T value);

}
