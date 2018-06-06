package HslCommunication.BasicFramework;


import java.util.UUID;


public class SoftBasic {


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


}

