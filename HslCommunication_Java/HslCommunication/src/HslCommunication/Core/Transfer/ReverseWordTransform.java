package HslCommunication.Core.Transfer;

import HslCommunication.BasicFramework.SoftBasic;
import HslCommunication.Utilities;

/**
 * 以字节为单位的变换操作
 */
public class ReverseWordTransform extends ByteTransformBase
{

    /**
     * 按照字节错位的方法
     * @param buffer 实际的字节数据
     * @param index 起始字节位置
     * @param length 数据长度
     * @return
     */
    private byte[] ReverseBytesByWord( byte[] buffer, int index, int length )
    {
        byte[] tmp = new byte[length];

        for (int i = 0; i < length; i++)
        {
            tmp[i] = buffer[index + i];
        }

        for (int i = 0; i < length / 2; i++)
        {
            byte b = tmp[i * 2 + 0];
            tmp[i * 2 + 0] = tmp[i * 2 + 1];
            tmp[i * 2 + 1] = b;
        }

        return tmp;
    }

    private byte[] ReverseBytesByWord( byte[] buffer )
    {
        return ReverseBytesByWord( buffer, 0, buffer.length );
    }


    /**
     * 字符串数据是否按照字来反转
     */
    public boolean IsStringReverse =false;




    /**
     * 从缓存中提取short结果
     * @param buffer 缓存数据
     * @param index 索引位置
     * @return short对象
     */
    @Override
    public short TransInt16( byte[] buffer, int index ) {
        return Utilities.getShort(ReverseBytesByWord(buffer, index, 2), 0);
    }




    /**
     * 从缓存中提取int结果
     * @param buffer 缓存数据
     * @param index 索引位置
     * @return int对象
     */
    @Override
    public int TransInt32( byte[] buffer, int index ) {
        return super.TransInt32(ReverseBytesByWord(buffer, index, 4), 0);
    }




    /**
     * 从缓存中提取long结果
     * @param buffer 缓存数据
     * @param index 索引位置
     * @return long对象
     */
    @Override
    public long TransInt64( byte[] buffer, int index ) {
        return super.TransInt64(ReverseBytesByWord(buffer, index, 8), 0);
    }





    /**
     * 从缓存中提取float结果
     * @param buffer 缓存对象
     * @param index 索引位置
     * @return float对象
     */
    @Override
    public float TransSingle( byte[] buffer, int index ) {
        return super.TransSingle(ReverseBytesByWord(buffer, index, 4), 0);
    }



    /**
     * 从缓存中提取double结果
     * @param buffer 缓存对象
     * @param index 索引位置
     * @return double对象
     */
    @Override
    public double TransDouble( byte[] buffer, int index ) {
        return super.TransDouble(ReverseBytesByWord(buffer, index, 8), 0);
    }




    /**
     * 从缓存中提取string结果，使用指定的编码
     * @param buffer 缓存对象
     * @param index 索引位置
     * @param length byte数组长度
     * @param encoding 字符串的编码
     * @return string对象
     */
    @Override
    public String TransString( byte[] buffer, int index, int length, String encoding ) {
        byte[] tmp = TransByte(buffer, index, length);

        if (IsStringReverse) {
            return Utilities.getString(ReverseBytesByWord(tmp), "ASCII");
        } else {
            return Utilities.getString(tmp, "ASCII");
        }
    }



    /**
     * bool数组变量转化缓存数据
     * @param values 等待转化的数组
     * @return buffer数据
     */
    @Override
    public byte[] TransByte( boolean[] values ) {
        return SoftBasic.BoolArrayToByte(values);
    }



    /**
     * short数组变量转化缓存数据
     * @param values 等待转化的数组
     * @return buffer数据
     */
    @Override
    public byte[] TransByte( short[] values ) {
        if (values == null) return null;

        byte[] buffer = new byte[values.length * 2];
        for (int i = 0; i < values.length; i++) {
            byte[] tmp = Utilities.getBytes(values[i]);
            System.arraycopy(tmp, 0, buffer, 2 * i, tmp.length);
        }

        return ReverseBytesByWord(buffer);
    }






    /**
     * int数组变量转化缓存数据
     * @param values 等待转化的数组
     * @return buffer数据
     */
    @Override
    public byte[] TransByte( int[] values ) {
        if (values == null) return null;

        byte[] buffer = new byte[values.length * 4];
        for (int i = 0; i < values.length; i++) {
            byte[] tmp = ReverseBytesByWord(Utilities.getBytes(values[i]));
            System.arraycopy(ByteTransDataFormat4(tmp), 0, buffer, 4 * i, tmp.length);
        }

        return buffer;
    }



    /**
     * long数组变量转化缓存数据
     * @param values 等待转化的数组
     * @return buffer数据
     */
    @Override
    public byte[] TransByte( long[] values ) {
        if (values == null) return null;

        byte[] buffer = new byte[values.length * 8];
        for (int i = 0; i < values.length; i++) {
            byte[] tmp = ReverseBytesByWord(Utilities.getBytes(values[i]));
            System.arraycopy(ByteTransDataFormat8(tmp), 0, buffer, 8 * i, tmp.length);
        }

        return buffer;
    }



    /**
     * float数组变量转化缓存数据
     * @param values 等待转化的数组
     * @return buffer数据
     */
    public byte[] TransByte( float[] values ) {
        if (values == null) return null;

        byte[] buffer = new byte[values.length * 4];
        for (int i = 0; i < values.length; i++) {
            byte[] tmp = ReverseBytesByWord(Utilities.getBytes(values[i]));
            System.arraycopy(ByteTransDataFormat4(tmp), 0, buffer, 4 * i, tmp.length);
        }

        return buffer;
    }


    /**
     * double数组变量转化缓存数据
     * @param values 等待转化的数组
     * @return buffer数据
     */
    @Override
    public byte[] TransByte( double[] values ) {
        if (values == null) return null;

        byte[] buffer = new byte[values.length * 8];
        for (int i = 0; i < values.length; i++) {
            byte[] tmp = ReverseBytesByWord(Utilities.getBytes(values[i]));
            System.arraycopy(ByteTransDataFormat8(tmp), 0, buffer, 8 * i, tmp.length);
        }

        return buffer;
    }



    /**
     * 使用指定的编码字符串转化缓存数据
     * @param value 等待转化的数据
     * @param encoding 字符串的编码方式
     * @return buffer数据
     */
    @Override
    public byte[] TransByte( String value, String encoding ) {
        if (value == null) return null;
        byte[] buffer = Utilities.getBytes(value, encoding);
        buffer = SoftBasic.ArrayExpandToLengthEven(buffer);
        if (IsStringReverse) {
            return ReverseBytesByWord(buffer);
        } else {
            return buffer;
        }
    }

}
