import HslCommunication.Utilities;

public class Main {

    public static void main(String[] args)
    {

        byte[] buffer = new byte[4];
        buffer[1]=0x01;

        System.out.println(Utilities.getInt(buffer,0));

        System.out.println(Utilities.bytes2HexString(Utilities.getBytes(256)));

        System.out.println(Utilities.bytes2HexString(Utilities.getBytes(12.34f)));

        System.out.println("Hello World!");
    }
}
