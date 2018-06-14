import HslCommunication.Core.Net.NetHandle;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.Enthernet.PushNet.NetPushClient;
import HslCommunication.Enthernet.SimplifyNet.NetSimplifyClient;
import HslCommunication.Profinet.Melsec.MelsecMcNet;

public class Main {

    public static void main(String[] args) {

        //NetSimplifyClientTest();


        //MelsecTest();
        //PushNetTest();


        System.out.println("Hello World!等待10s关闭");

        try {
            Thread.sleep(10);

        } catch (Exception ex) {

        }
    }


    public static void NetSimplifyClientTest() {
        NetSimplifyClient client = new NetSimplifyClient("127.0.0.1", 12345);

        OperateResultExOne<String> read = client.ReadFromServer(new NetHandle(2), "测试数据");
        if (read.IsSuccess) {
            System.out.println(read.Content);
        } else {
            System.out.println("读取失败：" + read.Message);
        }
    }


    private static void MelsecTest() {
        MelsecMcNet melsec_net = new MelsecMcNet("192.168.1.192", 6001);


        boolean[] M100 = melsec_net.ReadBool("M100", (short) 1).Content;            // 读取M100是否通，十进制地址
        boolean[] X1A0 = melsec_net.ReadBool("X1A0", (short) 1).Content;            // 读取X1A0是否通，十六进制地址
        boolean[] Y1A0 = melsec_net.ReadBool("Y1A0", (short) 1).Content;            // 读取Y1A0是否通，十六进制地址
        boolean[] B1A0 = melsec_net.ReadBool("B1A0", (short) 1).Content;            // 读取B1A0是否通，十六进制地址
        short short_D1000 = melsec_net.ReadInt16("D1000").Content;                 // 读取D1000的short值  ,W3C0,R3C0 效果是一样的
        int int_D1000 = melsec_net.ReadInt32("D1000").Content;                     // 读取D1000-D1001组成的int数据
        float float_D1000 = melsec_net.ReadFloat("D1000").Content;                 // 读取D1000-D1001组成的float数据
        long long_D1000 = melsec_net.ReadInt64("D1000").Content;                   // 读取D1000-D1003组成的long数据
        double double_D1000 = melsec_net.ReadDouble("D1000").Content;              // 读取D1000-D1003组成的double数据
        String str_D1000 = melsec_net.ReadString("D1000", (short) 10).Content;     // 读取D1000-D1009组成的条码数据


        melsec_net.Write("M100", new boolean[]{true});                          // 写入M100为通
        melsec_net.Write("Y1A0", new boolean[]{true});                        // 写入Y1A0为通
        melsec_net.Write("X1A0", new boolean[]{true});                        // 写入X1A0为通
        melsec_net.Write("B1A0", new boolean[]{true});                        // 写入B1A0为通
        melsec_net.Write("D1000", (short) 1234);                                   // 写入D1000  short值  ,W3C0,R3C0 效果是一样的
        melsec_net.Write("D1000", 1234566);                                // 写入D1000  int值
        melsec_net.Write("D1000", 123.456f);                               // 写入D1000  float值
        melsec_net.Write("D1000", 123.456d);                               // 写入D1000  double值
        melsec_net.Write("D1000", 123456661235123534L);                    // 写入D1000  long值
        melsec_net.Write("D1000", "K123456789");                           // 写入D1000  string值


        OperateResultExOne<boolean[]> read = melsec_net.ReadBool("M100", (short) 10);
        if (read.IsSuccess) {
            boolean m100 = read.Content[0];
            boolean m101 = read.Content[1];
            boolean m102 = read.Content[2];
            boolean m103 = read.Content[3];
            boolean m104 = read.Content[4];
            boolean m105 = read.Content[5];
            boolean m106 = read.Content[6];
            boolean m107 = read.Content[7];
            boolean m108 = read.Content[8];
            boolean m109 = read.Content[9];
        } else {
            System.out.print("读取失败：" + read.Message);
        }


        OperateResultExOne<byte[]> read1 = melsec_net.Read("D100", (short) 5);
        if (read1.IsSuccess) {
            short D100 = melsec_net.getByteTransform().TransByte(read1.Content, 0);
            short D101 = melsec_net.getByteTransform().TransByte(read1.Content, 2);
            short D102 = melsec_net.getByteTransform().TransByte(read1.Content, 4);
            short D103 = melsec_net.getByteTransform().TransByte(read1.Content, 6);
            short D104 = melsec_net.getByteTransform().TransByte(read1.Content, 8);
        } else {
            System.out.print("读取失败：" + read1.Message);
        }


        //解析复杂数据
        OperateResultExOne<byte[]> read3 = melsec_net.Read("D4000", (short) 10);
        if (read3.IsSuccess) {
            double 温度 = melsec_net.getByteTransform().TransInt16(read3.Content, 0) / 10d;//索引很重要
            double 压力 = melsec_net.getByteTransform().TransInt16(read3.Content, 2) / 100d;
            boolean IsRun = melsec_net.getByteTransform().TransInt16(read3.Content, 4) == 1;
            int 产量 = melsec_net.getByteTransform().TransInt32(read3.Content, 6);
            String 规格 = melsec_net.getByteTransform().TransString(read3.Content, 10, 10, "ascii");
        } else {
            System.out.print("读取失败：" + read3.Message);
        }

        // 写入测试，M100-M104 写入测试 此处写入后M100:通 M101:断 M102:断 M103:通 M104:通
        boolean[] values = new boolean[]{true, false, false, true, true};
        OperateResult write = melsec_net.Write("M100", values);
        if (write.IsSuccess) {
            System.out.print("写入成功");
        } else {
            System.out.print("写入失败：" + write.Message);
        }


        OperateResultExOne<Boolean> read2 = melsec_net.ReadBool("M100");
        if (read2.IsSuccess) {
            System.out.println(read2.Content);
        } else {
            System.out.println("读取失败：" + read.Message);
        }
    }

    private static void PushNetTest() {
        NetPushClient client = new NetPushClient("127.0.0.1", 12345, "C");
        OperateResult connect = client.CreatePush((NetPushClient c, String content) -> {
            System.out.println(content);
        });
        if (connect.IsSuccess) {
            System.out.println("连接成功!");
        } else {
            System.out.println("连接失败!"+connect.Message);
        }
    }
}
