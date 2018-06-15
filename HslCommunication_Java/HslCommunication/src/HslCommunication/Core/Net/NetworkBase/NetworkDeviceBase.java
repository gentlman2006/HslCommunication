package HslCommunication.Core.Net.NetworkBase;

import HslCommunication.BasicFramework.SoftBasic;
import HslCommunication.Core.IMessage.INetMessage;
import HslCommunication.Core.Net.IReadWriteNet;
import HslCommunication.Core.Transfer.IByteTransform;
import HslCommunication.Core.Types.IDataTransfer;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.Core.Types.OperateResultExOne;


/**
 * 设备类的基类，提供了基础的字节读写方法，采用泛型继承实现
 * @param <TNetMessage> 消息类型
 * @param <TTransform> 变换类型
 */
public class NetworkDeviceBase<TNetMessage extends INetMessage,TTransform extends IByteTransform> extends NetworkDoubleBase<TNetMessage,TTransform>  implements IReadWriteNet
{



    /**************************************************************************************************
     *
     *    说明：子类中需要重写基础的读取和写入方法，来支持不同的数据访问规则
     *
     *    此处没有将读写位纳入进来，因为各种设备的支持不尽相同，比较麻烦
     *
     **************************************************************************************************/

    protected short WordLength = 1;


    /**
     * 从设备读取原始数据
     * @param address 地址信息
     * @param length 数据长度
     * @return 带有成功标识的结果对象
     */
    public OperateResultExOne<byte[]> Read(String address, short length) {
        return new OperateResultExOne<byte[]>();
    }


    /**
     * 将原始数据写入设备
     * @param address 起始地址
     * @param value 原始数据
     * @return 带有成功标识的结果对象
     */
    public OperateResult Write(String address, byte[] value) {
        return new OperateResult();
    }





    /**
     * 读取自定义类型的数据，需要规定解析规则
     * @param address 起始地址
     * @param tClass 类
     * @param <T> 类型名称
     * @return 带有成功标识的结果对象
     */
    public <T extends IDataTransfer> OperateResultExOne<T> ReadCustomer(String address ,Class<T> tClass)
    {
        OperateResultExOne<T> result = new OperateResultExOne<T>();
        T Content;
        try {
            Content = tClass.newInstance();
        }
        catch (Exception ex){
            Content = null;
        }
        OperateResultExOne<byte[]> read = Read(address, Content.getReadCount());
        if (read.IsSuccess) {
            Content.ParseSource(read.Content);
            result.Content = Content;
            result.IsSuccess = true;
        } else {
            result.ErrorCode = read.ErrorCode;
            result.Message = read.Message;
        }
        return result;
    }


    /**
     * 写入自定义类型的数据到设备去，需要规定生成字节的方法
     * @param address 起始地址
     * @param data 实例对象
     * @param <T> 自定义类型
     * @return 带有成功标识的结果对象
     */
    public <T extends IDataTransfer> OperateResult WriteCustomer(String address, T data )
    {
        return Write(address, data.ToSource());
    }


    /**
     * 读取设备的short类型的数据
     * @param address 起始地址
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<Short> ReadInt16(String address) {
        return GetInt16ResultFromBytes(Read(address, WordLength));
    }


    /**
     * 读取设备的short类型的数组
     * @param address 起始地址
     * @param length 读取的数组长度
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<short[]> ReadInt16(String address, short length) {
        OperateResultExOne<byte[]> read = Read(address, (short) (length * WordLength));
        if (!read.IsSuccess) {
            OperateResultExOne<short[]> result = new OperateResultExOne<short[]>();
            result.CopyErrorFromOther(read);
            return result;
        }
        return OperateResultExOne.CreateSuccessResult(super.getByteTransform().TransInt16(read.Content, 0, length));
    }



    /**
     * 读取设备的int类型的数据
     * @param address 起始地址
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<Integer> ReadInt32(String address) {
        return GetInt32ResultFromBytes(Read(address, (short) (2 * WordLength)));
    }



    /**
     * 读取设备的int类型的数组
     * @param address 起始地址
     * @param length 数组长度
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<int[]> ReadInt32(String address, short length) {
        OperateResultExOne<byte[]> read = Read(address, (short) (length * WordLength * 2));
        if (!read.IsSuccess) {
            OperateResultExOne<int[]> result = new OperateResultExOne<int[]>();
            result.CopyErrorFromOther(read);
            return result;
        }
        return OperateResultExOne.CreateSuccessResult(super.getByteTransform().TransInt32(read.Content, 0, length));
    }


    /**
     * 读取设备的float类型的数据
     * @param address 起始地址
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<Float> ReadFloat(String address) {
        return GetSingleResultFromBytes(Read(address, (short) (2 * WordLength)));
    }



    /**
     * 读取设备的float类型的数组
     * @param address 起始地址
     * @param length 数组长度
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<float[]> ReadFloat(String address, short length) {
        OperateResultExOne<byte[]> read = Read(address, (short) (length * WordLength * 2));
        if (!read.IsSuccess)  {
            OperateResultExOne<float[]> result = new OperateResultExOne<float[]>();
            result.CopyErrorFromOther(read);
            return result;
        }
        return OperateResultExOne.CreateSuccessResult(super.getByteTransform().TransSingle(read.Content, 0, length));
    }



    /**
     * 读取设备的long类型的数据
     * @param address 起始地址
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<Long> ReadInt64(String address) {
        return GetInt64ResultFromBytes(Read(address, (short) (4 * WordLength)));
    }


    /**
     * 读取设备的long类型的数组
     * @param address 起始地址
     * @param length 数组长度
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<long[]> ReadInt64(String address, short length) {
        OperateResultExOne<byte[]> read = Read(address, (short) (length * WordLength * 4));
        if (!read.IsSuccess)  {
            OperateResultExOne<long[]> result = new OperateResultExOne<long[]>();
            result.CopyErrorFromOther(read);
            return result;
        }
        return OperateResultExOne.CreateSuccessResult(super.getByteTransform().TransInt64(read.Content, 0, length));
    }



    /**
     * 读取设备的double类型的数据
     * @param address 起始地址
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<Double> ReadDouble(String address) {
        return GetDoubleResultFromBytes(Read(address, (short) (4 * WordLength)));
    }



    /**
     * 读取设备的double类型的数组
     * @param address 起始地址
     * @param length 数组长度
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<double[]> ReadDouble(String address, short length) {
        OperateResultExOne<byte[]> read = Read(address, (short) (length * WordLength * 4));
        if (!read.IsSuccess) {
            OperateResultExOne<double[]> result = new OperateResultExOne<double[]>();
            result.CopyErrorFromOther(read);
            return result;
        }
        return OperateResultExOne.CreateSuccessResult(super.getByteTransform().TransDouble(read.Content, 0, length));
    }



    /**
     * 读取设备的字符串数据，编码为ASCII
     * @param address 起始地址
     * @param length 数据长度
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<String> ReadString(String address, short length) {
        return GetStringResultFromBytes(Read(address, length));
    }






    /**
     * 向设备中写入short数组，返回是否写入成功
     * @param address 起始地址
     * @param values 写入值
     * @return 返回写入结果
     */
    public OperateResult Write(String address, short[] values) {
        return Write(address, super.getByteTransform().TransByte(values));
    }



    /**
     * 向设备中写入short数据，返回是否写入成功
     * @param address 起始地址
     * @param value 写入值
     * @return 返回写入结果
     */
    public OperateResult Write(String address, short value) {
        return Write(address, new short[]{value});
    }





    /**
     * 向设备中写入int数组，返回是否写入成功
     * @param address 起始地址
     * @param values 写入值
     * @return 返回写入结果
     */
    /// <returns>返回写入结果</returns>
    public OperateResult Write(String address, int[] values) {
        return Write(address, super.getByteTransform().TransByte(values));
    }

    /**
     * 向设备中写入int数据，返回是否写入成功
     * @param address 起始地址
     * @param value 写入值
     * @return 返回写入结果
     */
    public OperateResult Write(String address, int value) {
        return Write(address, new int[]{value});
    }




    /**
     * 向设备中写入float数组，返回是否写入成功
     * @param address 起始地址
     * @param values 写入值
     * @return 返回写入结果
     */
    public OperateResult Write(String address, float[] values) {
        return Write(address, super.getByteTransform().TransByte(values));
    }


    /**
     * 向设备中写入float数据，返回是否写入成功
     * @param address 起始地址
     * @param value 写入值
     * @return 返回写入结果
     */
    public OperateResult Write(String address, float value) {
        return Write(address, new float[]{value});
    }



    /**
     * 向设备中写入long数组，返回是否写入成功
     * @param address 起始地址
     * @param values 写入值
     * @return 返回写入结果
     */
    public OperateResult Write(String address, long[] values) {
        return Write(address, getByteTransform().TransByte(values));
    }


    /**
     * 向设备中写入long数据，返回是否写入成功
     * @param address 起始地址
     * @param value 写入值
     * @return 返回写入结果
     */
    public OperateResult Write(String address, long value) {
        return Write(address, new long[]{value});
    }





    /**
     * 设备中写入double数组，返回是否写入成功
     * @param address 起始地址
     * @param values 写入值
     * @return 返回写入结果
     */
    public OperateResult Write(String address, double[] values) {
        return Write(address, getByteTransform().TransByte(values));
    }


    /**
     * 向设备中写入double数据，返回是否写入成功
     * @param address 起始地址
     * @param value 写入值
     * @return 返回写入结果
     */
    public OperateResult Write(String address, double value) {
        return Write(address, new double[]{value});
    }



    /**
     * 向设备中写入字符串，编码格式为ASCII
     * @param address 起始地址
     * @param value 写入值
     * @return 返回读取结果
     */
    public OperateResult Write(String address, String value) {
        byte[] temp = getByteTransform().TransByte(value, "US-ASCII");
        if (WordLength == 1) temp = SoftBasic.ArrayExpandToLengthEven(temp);
        return Write(address, temp);
    }


    /**
     * 返回表示当前对象的字符串
     * @return 字符串数据
     */
    @Override
    public String toString() {
        return "NetworkDeviceBase<TNetMessage, TTransform>";
    }

}
