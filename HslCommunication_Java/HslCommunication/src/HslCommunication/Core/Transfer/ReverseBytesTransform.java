package HslCommunication.Core.Transfer;

import HslCommunication.Utilities;

/**
 * 反转的字节变换类
 */
public class ReverseBytesTransform extends ByteTransformBase
{

    /**
     * 从缓存中提取short结果
     * @param buffer 缓存数据
     * @param index 索引位置
     * @return short对象
     */
    @Override
    public short TransInt16( byte[] buffer, int index ) {
        byte[] tmp = new byte[2];
        tmp[0] = buffer[1 + index];
        tmp[1] = buffer[0 + index];
        return Utilities.getShort(tmp, 0);
    }


    /**
     * 从缓存中提取int结果
     * @param buffer 缓存数据
     * @param index 索引位置
     * @return int对象
     */
    @Override
    public int TransInt32( byte[] buffer, int index ) {
        byte[] tmp = new byte[4];
        tmp[0] = buffer[3 + index];
        tmp[1] = buffer[2 + index];
        tmp[2] = buffer[1 + index];
        tmp[3] = buffer[0 + index];
        return Utilities.getInt(ByteTransDataFormat4(tmp),0);
    }




    /**
     * 从缓存中提取long结果
     * @param buffer 缓存数据
     * @param index 索引位置
     * @return long对象
     */
    @Override
    public long TransInt64( byte[] buffer, int index ) {
        byte[] tmp = new byte[8];
        tmp[0] = buffer[7 + index];
        tmp[1] = buffer[6 + index];
        tmp[2] = buffer[5 + index];
        tmp[3] = buffer[4 + index];
        tmp[4] = buffer[3 + index];
        tmp[5] = buffer[2 + index];
        tmp[6] = buffer[1 + index];
        tmp[7] = buffer[0 + index];
        return Utilities.getLong(ByteTransDataFormat8(tmp), 0);
    }



    /**
     * 从缓存中提取float结果
     * @param buffer 缓存对象
     * @param index 索引位置
     * @return float对象
     */
    @Override
    public float TransSingle( byte[] buffer, int index ) {
        byte[] tmp = new byte[4];
        tmp[0] = buffer[3 + index];
        tmp[1] = buffer[2 + index];
        tmp[2] = buffer[1 + index];
        tmp[3] = buffer[0 + index];
        return Utilities.getFloat(ByteTransDataFormat4(tmp), 0);
    }


    /**
     * 从缓存中提取double结果
     * @param buffer 缓存对象
     * @param index 索引位置
     * @return double对象
     */
    @Override
    public double TransDouble( byte[] buffer, int index ) {
        byte[] tmp = new byte[8];
        tmp[0] = buffer[7 + index];
        tmp[1] = buffer[6 + index];
        tmp[2] = buffer[5 + index];
        tmp[3] = buffer[4 + index];
        tmp[4] = buffer[3 + index];
        tmp[5] = buffer[2 + index];
        tmp[6] = buffer[1 + index];
        tmp[7] = buffer[0 + index];
        return Utilities.getDouble(ByteTransDataFormat8(tmp), 0);
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
            Utilities.bytesReverse(tmp);
            System.arraycopy(tmp, 0, buffer, 2 * i, tmp.length);
        }

        return buffer;
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
            byte[] tmp = Utilities.getBytes(values[i]);
            Utilities.bytesReverse(tmp);
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
            byte[] tmp = Utilities.getBytes(values[i]);
            Utilities.bytesReverse(tmp);
            System.arraycopy(ByteTransDataFormat8(tmp), 0, buffer, 8 * i, tmp.length);
        }

        return buffer;
    }



    /**
     * float数组变量转化缓存数据
     * @param values 等待转化的数组
     * @return buffer数据
     */
    @Override
    public byte[] TransByte( float[] values ) {
        if (values == null) return null;

        byte[] buffer = new byte[values.length * 4];
        for (int i = 0; i < values.length; i++) {
            byte[] tmp = Utilities.getBytes(values[i]);
            Utilities.bytesReverse(tmp);
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
            byte[] tmp = Utilities.getBytes(values[i]);
            Utilities.bytesReverse(tmp);
            System.arraycopy(ByteTransDataFormat8(tmp), 0, buffer, 8 * i, tmp.length);
        }

        return buffer;
    }


}
