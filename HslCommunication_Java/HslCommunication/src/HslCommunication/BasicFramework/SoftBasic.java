package HslCommunication.BasicFramework;


import java.io.OutputStream;
import java.io.UnsupportedEncodingException;
import java.util.List;
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




    /**
     * 字节数据转化成16进制表示的字符串
     * @param InBytes 字节数组
     * @return 返回的字符串
     */
    public static String ByteToHexString(byte[] InBytes)
    {
        return ByteToHexString(InBytes, (char)0);
    }


    /**
     * 字节数据转化成16进制表示的字符串
     * @param InBytes 字节数组
     * @param segment 分割符
     * @return 返回的字符串
     */
    public static String ByteToHexString(byte[] InBytes, char segment)
    {
        StringBuilder sb = new StringBuilder();
        for (byte InByte : InBytes)
        {
            if (segment == 0) sb.append(String.format("{0:X2}", InByte));
            else sb.append(String.format("{0:X2}{1}", InByte, segment));
        }

        if (segment != 0 && sb.length() > 1 && sb.charAt(sb.length() - 1) == segment)
        {
            sb.delete(sb.length() - 1, 1);
        }
        return sb.toString();
    }



    /// <summary>
    /// 字符串数据转化成16进制表示的字符串
    /// </summary>
    /// <param name="InString">输入的字符串数据</param>
    /// <returns>返回的字符串</returns>
    /// <exception cref="NullReferenceException"></exception>
    public static String ByteToHexString(String InString) throws UnsupportedEncodingException
    {
        return ByteToHexString(InString.getBytes("unicode"));
    }


    private static char[] hexCharList = new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

    private static boolean isCharinList(char[] chars,char c ){
        for (int i=0;i<hexCharList.length;i++){
            if(hexCharList[i] == c) return true;
        }
        return false;
    }


    /// <summary>
    /// 将16进制的字符串转化成Byte数据，将检测每2个字符转化，也就是说，中间可以是任意字符
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public static byte[] HexStringToBytes(String hex)
    {
        hex = hex.toUpperCase();

        OutputStream ms = new OutputStream();

        for (int i = 0; i < hex.length(); i++)
        {
            if ((i + 1) < hex.length())
            {
                if (isCharinList(hexCharList,hex.charAt(i)) && isCharinList(hexCharList,hex.charAt(i + 1)))
                {
                    // 这是一个合格的字节数据
                    ms.write((byte)(hexCharList.IndexOf(hex[i]) * 16 + hexCharList.IndexOf(hex[i + 1])));
                    i++;
                }
            }
        }

        byte[] result = ms.t();
        ms.Dispose();
        return result;
    }

}

