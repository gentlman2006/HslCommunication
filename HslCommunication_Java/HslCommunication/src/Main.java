import HslCommunication.Core.Net.NetHandle;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.Enthernet.SimplifyNet.NetSimplifyClient;
import HslCommunication.Profinet.Melsec.MelsecMcNet;

import java.util.UUID;

public class Main {

    public static void main(String[] args) {

        //NetSimplifyClientTest();


        //MelsecTest();

        System.out.println("Hello World!");
    }


    public static void NetSimplifyClientTest(){
        NetSimplifyClient client = new NetSimplifyClient("127.0.0.1", 12345);
        client.Token = UUID.fromString("523a5269-4bc0-4da3-9937-47c2a318c5eb");

        OperateResultExOne<String> read = client.ReadFromServer(new NetHandle(2), "测试数据");
        if (read.IsSuccess) {
            System.out.println(read.Content);
        } else {
            System.out.println("读取失败：" + read.Message);
        }
    }


    private static void MelsecTest(){
        MelsecMcNet melsecMcNet = new MelsecMcNet("192.168.1.192",6001);
        OperateResultExOne<Boolean> read = melsecMcNet.ReadBool("M100");
        if(read.IsSuccess){
            System.out.println(read.Content);
        }
        else {
            System.out.println("读取失败：" + read.Message);
        }
    }
}
