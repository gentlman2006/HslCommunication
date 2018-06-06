package HslCommunication;
;

import java.io.ByteArrayOutputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.util.UUID;


/**
 * 一个工具类，为了支持和C#程序通信交互的转换工具类
 */
public class Utilities {


    /**
     * 将int类型的数据转化成byte[]数组
     * @param num 实际的int值
     * @return 最后的数组
     */
    public static byte[] int2Bytes(int num) {
        byte[] byteNum = new byte[4];
        for (int ix = 0; ix < 4; ++ix) {
            int offset = 32 - (ix + 1) * 8;
            byteNum[3-ix] = (byte) ((num >> offset) & 0xff);
        }
        return byteNum;
    }


    /**
     * 将byte数组转换成int类型的数据
     * @param byteNum byte数组
     * @return int对象
     */
    public static int bytes2Int(byte[] byteNum) {
        int num = 0;
        for (int ix = 0; ix < 4; ++ix) {
            num <<= 8;
            num |= (byteNum[3-ix] & 0xff);
        }
        return num;
    }


    /**
     * 将int转换成一个字节的数据
     * @param num int数据
     * @return 一个字节的数据
     */
    public static byte int2OneByte(int num) {
        return (byte) (num & 0x000000ff);
    }


    /**
     * 将一个有符号的byte转换成一个int数据对象
     * @param byteNum 有符号的字节对象
     * @return int数据类型
     */
    public static int oneByte2Int(byte byteNum) {
        //针对正数的int
        return byteNum > 0 ? byteNum : 256+ byteNum;
    }


    /**
     * 将一个long对象转换成byte[]数组
     * @param num long对象
     * @return byte[]数组
     */
    public static byte[] long2Bytes(long num) {
        byte[] byteNum = new byte[8];
        for (int ix = 0; ix < 8; ++ix) {
            int offset = 64 - (ix + 1) * 8;
            byteNum[7-ix] = (byte) ((num >> offset) & 0xff);
        }
        return byteNum;
    }


    /**
     * 将一个byte数组转换成long对象
     * @param byteNum byte[]数组
     * @return long对象
     */
    public static long bytes2Long(byte[] byteNum) {
        long num = 0;
        for (int ix = 0; ix < 8; ++ix) {
            num <<= 8;
            num |= (byteNum[7-ix] & 0xff);
        }
        return num;
    }


    /**
     * 将一个byte[]数组转换成uuid对象
     * @param data 字节数组
     * @return uuid对象
     */
    public static UUID Byte2UUID(byte[] data) {
        if (data.length != 16) {
            throw new IllegalArgumentException("Invalid UUID byte[]");
        }
        long msb = 0;
        long lsb = 0;
        for (int i = 0; i < 8; i++)
            msb = (msb << 8) | (data[i] & 0xff);
        for (int i = 8; i < 16; i++)
            lsb = (lsb << 8) | (data[i] & 0xff);

        return new UUID(msb, lsb);
    }


    /**
     * 将一个uuid对象转化成byte[]
     * @param uuid uuid对象
     * @return 字节数组
     */
    public static byte[] UUID2Byte(UUID uuid) {
        ByteArrayOutputStream ba = new ByteArrayOutputStream(16);
        DataOutputStream da = new DataOutputStream(ba);
        try {
            da.writeLong(uuid.getMostSignificantBits());
            da.writeLong(uuid.getLeastSignificantBits());
            ba.close();
            da.close();
        }
        catch (IOException e) {
            e.printStackTrace();
        }

        byte[] buffer = ba.toByteArray();
        // 进行错位
        byte temp=buffer[0];
        buffer[0] = buffer[3];
        buffer[3] =temp;
        temp=buffer[1];
        buffer[1]=buffer[2];
        buffer[2]=temp;

        temp = buffer[4];
        buffer[4]=buffer[5];
        buffer[5] =temp;

        temp = buffer[6];
        buffer[6]=buffer[7];
        buffer[7] =temp;

        return buffer;
    }


    /**
     * 将指定位置的数据转换成short对象
     * @param b
     * @param index
     * @return
     */
    public static short byte2Short(byte[] b, int index) {
        return (short) (((b[index + 0] << 8) | b[index + 1] & 0xff));
    }


    /**
     * 将short对象转换成字节数组
     * @param s
     * @return
     */
    public static byte[] short2Byte(short s) {
        byte[] targets = new byte[2];
        for (int i = 0; i < 2; i++) {
            int offset = (targets.length - 1 - i) * 8;
            targets[1-i] = (byte) ((s >>> offset) & 0xff);
        }
        return targets;
    }


    /**
     * 将字符串数据转换成字节数组
     * @param str
     * @return
     */
    public static byte[] string2Byte(String str) {
        if (str == null) {
            return null;
        }
        byte[] byteArray;
        try {
            byteArray = str.getBytes("unicode");
        } catch (Exception ex) {
            byteArray = str.getBytes();
        }
//
//        for(int i=0;i<byteArray.length;i++)
//        {
//            byte temp=byteArray[i];
//            byteArray[i]=byteArray[i+1];
//            byteArray[i+1] =temp;
//            i++;
//        }
        return byteArray;
    }


    /**
     * 将字节数组转换成字符串对象
     * @param byteArray
     * @return
     */
    public static String byte2String(byte[] byteArray) {
        if (byteArray == null) {
            return null;
        }

        for (int i = 0; i < byteArray.length; i++) {
            byte temp = byteArray[i];
            byteArray[i] = byteArray[i + 1];
            byteArray[i + 1] = temp;
            i++;
        }
        String str;
        try {
            str = new String(byteArray, "unicode");
        } catch (Exception ex) {
            str = new String(byteArray);
        }
        return str;
    }



    private static final char[] HEX_CHAR = {'0', '1', '2', '3', '4', '5',
            '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'};


    /**
     * 将字节数组转换成十六进制的字符串形式
     * @param bytes
     * @return
     */
    public static String bytes2HexString(byte[] bytes) {
        char[] buf = new char[bytes.length * 2];
        int index = 0;
        for(byte b : bytes) { // 利用位运算进行转换，可以看作方法一的变种
            buf[index++] = HEX_CHAR[b >>> 4 & 0xf];
            buf[index++] = HEX_CHAR[b & 0xf];
        }

        return new String(buf);
    }
}
