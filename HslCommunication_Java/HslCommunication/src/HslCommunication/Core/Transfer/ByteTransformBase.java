package HslCommunication.Core.Transfer;

import HslCommunication.Utilities;


/**
 * 字节转换类的基类，提供了一些基础的转换方法
 */
public class ByteTransformBase implements IByteTransform {

    /**
     * 实例化一个对象
     */
    public ByteTransformBase(){
        this.dataFormat = DataFormat.DCBA;
    }

    /**
     * 从缓存中提取出bool结果
     *
     * @param buffer 缓存数据
     * @param index  索引位置
     * @return boolean值
     */
    public boolean TransBool(byte[] buffer, int index) {
        return buffer[index] != 0x00;
    }

    /**
     * 缓存中提取byte结果
     *
     * @param buffer 缓存数据
     * @param index  索引位置
     * @return byte对象
     */
    public byte TransByte(byte[] buffer, int index) {
        return buffer[index];
    }

    /**
     * 从缓存中提取byte数组结果
     *
     * @param buffer 缓存数据
     * @param index  索引位置
     * @param length 读取的数组长度
     * @return byte数组
     */
    public byte[] TransByte(byte[] buffer, int index, int length) {
        byte[] tmp = new byte[length];
        System.arraycopy(buffer, index, tmp, 0, length);
        return tmp;
    }

    /**
     * 从缓存中提取short结果
     *
     * @param buffer 缓存数据
     * @param index  索引位置
     * @return short对象
     */
    public short TransInt16(byte[] buffer, int index) {
        return Utilities.getShort(buffer, index);
    }

    /**
     * 从缓存中提取short结果
     *
     * @param buffer 缓存数据
     * @param index  索引位置
     * @param length 读取的数组长度
     * @return short数组对象
     */
    public short[] TransInt16(byte[] buffer, int index, int length) {
        short[] tmp = new short[length];
        for (int i = 0; i < length; i++) {
            tmp[i] = TransInt16(buffer, index + 2 * i);
        }
        return tmp;
    }

    /**
     * 从缓存中提取int结果
     *
     * @param buffer 缓存数据
     * @param index  索引位置
     * @return int对象
     */
    public int TransInt32(byte[] buffer, int index) {
        return Utilities.getInt(ByteTransDataFormat4(buffer, index),0);
    }


    /**
     * 从缓存中提取int数组结果
     *
     * @param buffer 缓存数据
     * @param index  索引位置
     * @param length 读取的数组长度
     * @return int数组对象
     */
    public int[] TransInt32(byte[] buffer, int index, int length) {
        int[] tmp = new int[length];
        for (int i = 0; i < length; i++) {
            tmp[i] = TransInt32(buffer, index + 4 * i);
        }
        return tmp;
    }


    /**
     * 从缓存中提取long结果
     *
     * @param buffer 缓存数据
     * @param index  索引位置
     * @return long对象
     */
    public long TransInt64(byte[] buffer, int index) {
        return Utilities.getLong(ByteTransDataFormat8(buffer, index),0);
    }


    /**
     * 从缓存中提取long数组结果
     *
     * @param buffer 缓存数据
     * @param index  索引位置
     * @param length 读取的数组长度
     * @return long数组对象
     */
    public long[] TransInt64(byte[] buffer, int index, int length) {
        long[] tmp = new long[length];
        for (int i = 0; i < length; i++) {
            tmp[i] = TransInt64(buffer, index + 8 * i);
        }
        return tmp;
    }


    /**
     * 从缓存中提取float结果
     *
     * @param buffer 缓存对象
     * @param index  索引位置
     * @return float对象
     */
    public float TransSingle(byte[] buffer, int index) {
        return Utilities.getFloat(ByteTransDataFormat4(buffer, index),0);
    }


    /**
     * 从缓存中提取float数组结果
     *
     * @param buffer 缓存数据
     * @param index  索引位置
     * @param length 读取的数组长度
     * @return float数组对象
     */
    public float[] TransSingle(byte[] buffer, int index, int length) {
        float[] tmp = new float[length];
        for (int i = 0; i < length; i++) {
            tmp[i] = TransSingle(buffer, index + 4 * i);
        }
        return tmp;
    }


    /**
     * 从缓存中提取double结果
     *
     * @param buffer 缓存对象
     * @param index  索引位置
     * @return double对象
     */
    public double TransDouble(byte[] buffer, int index) {
        return Utilities.getDouble(ByteTransDataFormat8(buffer, index),0);
    }


    /**
     * 从缓存中提取double数组结果
     *
     * @param buffer 缓存对象
     * @param index  索引位置
     * @param length 读取的数组长度
     * @return double数组
     */
    public double[] TransDouble(byte[] buffer, int index, int length) {
        double[] tmp = new double[length];
        for (int i = 0; i < length; i++) {
            tmp[i] = TransDouble(buffer, index + 8 * i);
        }
        return tmp;
    }


    /**
     * 从缓存中提取string结果，使用指定的编码
     *
     * @param buffer   缓存对象
     * @param index    索引位置
     * @param length   byte数组长度
     * @param encoding 字符串的编码
     * @return string对象
     */
    public String TransString(byte[] buffer, int index, int length, String encoding) {
        return Utilities.getString(TransByte(buffer, index, length), encoding);
    }


    /**
     * bool变量转化缓存数据
     *
     * @param value 等待转化的数据
     * @return buffer数据
     */
    public byte[] TransByte(boolean value) {
        return TransByte(new boolean[]{value});
    }


    /**
     * bool数组变量转化缓存数据
     *
     * @param values 等待转化的数组
     * @return buffer数据
     */
    public byte[] TransByte(boolean[] values) {
        if (values == null) return null;

        byte[] buffer = new byte[values.length];
        for (int i = 0; i < values.length; i++) {
            if (values[i]) buffer[i] = 0x01;
        }

        return buffer;
    }


    /**
     * byte变量转化缓存数据
     *
     * @param value 等待转化的数据
     * @return buffer数据
     */
    public byte[] TransByte(byte value) {
        return new byte[]{value};
    }

    /**
     * short变量转化缓存数据
     *
     * @param value 等待转化的数据
     * @return buffer数据
     */
    public byte[] TransByte(short value) {
        return TransByte(new short[]{value});
    }

    /**
     * short数组变量转化缓存数据
     *
     * @param values 等待转化的数组
     * @return buffer数据
     */
    public byte[] TransByte(short[] values) {
        if (values == null) return null;
        byte[] buffer = new byte[values.length * 2];
        for (int i = 0; i < values.length; i++) {
            System.arraycopy(Utilities.getBytes(values[i]), 0, buffer, 2 * i, 2);
        }
        return buffer;
    }


    /**
     * int变量转化缓存数据
     *
     * @param value 等待转化的数据
     * @return buffer数据
     */
    public byte[] TransByte(int value) {
        return TransByte(new int[]{value});
    }


    /**
     * int数组变量转化缓存数据
     *
     * @param values 等待转化的数组
     * @return buffer数据
     */
    public byte[] TransByte(int[] values) {
        if (values == null) return null;

        byte[] buffer = new byte[values.length * 4];
        for (int i = 0; i < values.length; i++) {
            System.arraycopy(ByteTransDataFormat4(Utilities.getBytes(values[i])), 0, buffer, 4 * i, 4);
        }

        return buffer;
    }

    /**
     * long变量转化缓存数据
     *
     * @param value 等待转化的数据
     * @return buffer数据
     */
    public byte[] TransByte(long value) {
        return TransByte(new long[]{value});
    }


    /**
     * long数组变量转化缓存数据
     *
     * @param values 等待转化的数组
     * @return buffer数据
     */
    public byte[] TransByte(long[] values) {
        if (values == null) return null;

        byte[] buffer = new byte[values.length * 8];
        for (int i = 0; i < values.length; i++) {
            System.arraycopy(ByteTransDataFormat8(Utilities.getBytes(values[i])), 0, buffer, 8 * i, 8);
        }

        return buffer;
    }


    /**
     * float变量转化缓存数据
     *
     * @param value 等待转化的数据
     * @return buffer数据
     */
    public byte[] TransByte(float value) {
        return TransByte(new float[]{value});
    }


    /**
     * float数组变量转化缓存数据
     *
     * @param values 等待转化的数组
     * @return buffer数据
     */
    public byte[] TransByte(float[] values) {
        if (values == null) return null;

        byte[] buffer = new byte[values.length * 4];
        for (int i = 0; i < values.length; i++) {
            System.arraycopy(ByteTransDataFormat4(Utilities.getBytes(values[i])), 0, buffer, 4 * i, 4);
        }

        return buffer;
    }


    /**
     * double变量转化缓存数据
     *
     * @param value 等待转化的数据
     * @return buffer数据
     */
    public byte[] TransByte(double value) {
        return TransByte(new double[]{value});
    }

    /**
     * double数组变量转化缓存数据
     *
     * @param values 等待转化的数组
     * @return buffer数据
     */
    public byte[] TransByte(double[] values) {
        if (values == null) return null;

        byte[] buffer = new byte[values.length * 8];
        for (int i = 0; i < values.length; i++) {
            System.arraycopy(ByteTransDataFormat8(Utilities.getBytes(values[i])), 0, buffer, 8 * i, 8);
        }

        return buffer;
    }

    /**
     * 使用指定的编码字符串转化缓存数据
     *
     * @param value    等待转化的数据
     * @param encoding 字符串的编码方式
     * @return buffer数据
     */
    public byte[] TransByte(String value, String encoding) {
        return Utilities.getBytes(value, encoding);
    }


    private DataFormat dataFormat = DataFormat.ABCD;

    /**
     * 设置数据解析的格式，ABCD，BADC，CDAB，DCBA格式
     *
     * @param dataFormat
     */
    public void setDataFormat(DataFormat dataFormat) {
        this.dataFormat = dataFormat;
    }

    /**
     * 获取数据解析的格式，默认ABCD，可选BADC，CDAB，DCBA格式
     *
     * @return
     */
    public DataFormat getDataFormat() {
        return this.dataFormat;
    }


    /**
     * 反转多字节的数据信息
     *
     * @param value 数据字节
     * @param Index 起始索引，默认值为0
     * @return 实际字节信息
     */
    protected byte[] ByteTransDataFormat4(byte[] value, int Index) {
        byte[] buffer = new byte[4];
        switch (dataFormat) {
            case ABCD: {
                buffer[0] = value[Index + 3];
                buffer[1] = value[Index + 2];
                buffer[2] = value[Index + 1];
                buffer[3] = value[Index + 0];
                break;
            }
            case BADC: {
                buffer[0] = value[Index + 2];
                buffer[1] = value[Index + 3];
                buffer[2] = value[Index + 0];
                buffer[3] = value[Index + 1];
                break;
            }
            case CDAB: {
                buffer[0] = value[Index + 1];
                buffer[1] = value[Index + 0];
                buffer[2] = value[Index + 3];
                buffer[3] = value[Index + 2];
                break;
            }
            case DCBA: {
                buffer[0] = value[Index + 0];
                buffer[1] = value[Index + 1];
                buffer[2] = value[Index + 2];
                buffer[3] = value[Index + 3];
                break;
            }
        }
        return buffer;
    }


    /**
     * 反转多字节的数据信息
     *
     * @param value 数据字节
     * @return 实际字节信息
     */
    protected byte[] ByteTransDataFormat4(byte[] value) {
        return ByteTransDataFormat4(value, 0);
    }


    /**
     * 反转多字节的数据信息
     *
     * @param value 数据字节
     * @param Index 起始索引，默认值为0
     * @return 实际字节信息
     */
    protected byte[] ByteTransDataFormat8(byte[] value, int Index) {
        byte[] buffer = new byte[8];
        switch (dataFormat) {
            case ABCD: {
                buffer[0] = value[Index + 7];
                buffer[1] = value[Index + 6];
                buffer[2] = value[Index + 5];
                buffer[3] = value[Index + 4];
                buffer[4] = value[Index + 3];
                buffer[5] = value[Index + 2];
                buffer[6] = value[Index + 1];
                buffer[7] = value[Index + 0];
                break;
            }
            case BADC: {
                buffer[0] = value[Index + 6];
                buffer[1] = value[Index + 7];
                buffer[2] = value[Index + 4];
                buffer[3] = value[Index + 5];
                buffer[4] = value[Index + 2];
                buffer[5] = value[Index + 3];
                buffer[6] = value[Index + 0];
                buffer[7] = value[Index + 1];
                break;
            }

            case CDAB: {
                buffer[0] = value[Index + 1];
                buffer[1] = value[Index + 0];
                buffer[2] = value[Index + 3];
                buffer[3] = value[Index + 2];
                buffer[4] = value[Index + 5];
                buffer[5] = value[Index + 4];
                buffer[6] = value[Index + 7];
                buffer[7] = value[Index + 6];
                break;
            }
            case DCBA: {
                buffer[0] = value[Index + 0];
                buffer[1] = value[Index + 1];
                buffer[2] = value[Index + 2];
                buffer[3] = value[Index + 3];
                buffer[4] = value[Index + 4];
                buffer[5] = value[Index + 5];
                buffer[6] = value[Index + 6];
                buffer[7] = value[Index + 7];
                break;
            }
        }
        return buffer;
    }


    /**
     * 反转多字节的数据信息
     *
     * @param value 数据字节
     * @return 实际字节信息
     */
    protected byte[] ByteTransDataFormat8(byte[] value) {
        return ByteTransDataFormat8(value, 0);
    }
}
