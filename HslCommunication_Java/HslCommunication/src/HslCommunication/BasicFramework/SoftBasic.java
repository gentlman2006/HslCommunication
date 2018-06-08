package HslCommunication.BasicFramework;


import java.util.Random;
import java.util.UUID;


public class SoftBasic {


    /**
     * 根据大小获取文本描述信息
     * @param size
     * @return
     */
    public static String GetSizeDescription(long size) {
        if (size < 1000) {
            return size + " B";
        } else if (size < 1000 * 1000) {
            float data = (float) size / 1024;
            return String.format("%.2f", data) + " Kb";
        } else if (size < 1000 * 1000 * 1000) {
            float data = (float) size / 1024 / 1024;
            return String.format("%.2f", data) + " Mb";
        } else {
            float data = (float) size / 1024 / 1024 / 1024;
            return String.format("%.2f", data) + " Gb";
        }
    }


    /**
     * 判断两个字节数组是否是一致的，可以指定中间的某个区域
     * @param b1
     * @param start1
     * @param b2
     * @param start2
     * @param length
     * @return
     */
    public static boolean IsTwoBytesEquel(byte[] b1, int start1, byte[] b2, int start2, int length)
    {
        if (b1 == null || b2 == null) return false;
        for (int i = 0; i < length; i++)
        {
            if (b1[i + start1] != b2[i + start2])
            {
                return false;
            }
        }

        return true;
    }



    /**
     * 获取一串唯一的随机字符串，长度为20，由Guid码和4位数的随机数组成，保证字符串的唯一性
     * @return 随机字符串数据
     */
    public static String GetUniqueStringByGuidAndRandom()
    {
        Random random = new Random();
        return UUID.randomUUID().toString() + (random.nextInt(9000)+1000);
    }

}

