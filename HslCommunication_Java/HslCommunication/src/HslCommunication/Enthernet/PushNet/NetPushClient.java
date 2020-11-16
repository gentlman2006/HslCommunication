package HslCommunication.Enthernet.PushNet;

import HslCommunication.Core.Net.HslProtocol;
import HslCommunication.Core.Net.NetworkBase.NetworkXBase;
import HslCommunication.Core.Net.StateOne.AppSession;
import HslCommunication.Core.Types.ActionOperateExTwo;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.Core.Types.OperateResultExTwo;
import HslCommunication.Utilities;

import java.net.Socket;

public class NetPushClient extends NetworkXBase
{

    /**
     * 实例化一个发布订阅类的客户端，需要指定ip地址，端口，及订阅关键字
     * @param ipAddress 服务器的IP地址
     * @param port 服务器的端口号
     * @param key 订阅关键字
     */
    public NetPushClient( String ipAddress, int port, String key )
    {
        this.ipAddress = ipAddress;
        this.port = port;
        keyWord = key;

        if (key == null || key.isEmpty())
        {
            throw new RuntimeException( "key 不允许为空" );
        }
    }


    @Override
    protected void DataProcessingCenter(AppSession session, int protocol, int customer, byte[] content ) {
        if (protocol == HslProtocol.ProtocolUserString) {
            if (action != null) action.Action(this, Utilities.byte2String(content));
        }
    }


    @Override
    protected void SocketReceiveException( AppSession session) {
        // 发生异常的时候需要进行重新连接
        while (true) {
            System.out.print("10 秒钟后尝试重连服务器");
            try {
                Thread.sleep(10000);
            }catch (Exception ex){

            }

            if (CreatePush().IsSuccess) {
                System.out.print("重连服务器成功");
                break;
            }
        }
    }


    private OperateResult CreatePush( )
    {
        CloseSocket(CoreSocket);

        OperateResultExOne<Socket> connect = CreateSocketAndConnect( ipAddress,port, 5000 );
        if (!connect.IsSuccess) return connect;

        OperateResult send = SendStringAndCheckReceive( connect.Content, 0, keyWord );
        if (!send.IsSuccess) return send;

        OperateResultExTwo<Integer, String> receive = ReceiveStringContentFromSocket( connect.Content );
        if (!receive.IsSuccess) return receive;

        if (receive.Content1 != 0)
        {
            OperateResult result = new OperateResult();
            result.Message = receive.Content2;
            return result;
        }

        AppSession appSession = new AppSession( );
        CoreSocket = connect.Content;
        appSession.setWorkSocket(connect.Content);
        BeginReceiveBackground( appSession );

        return OperateResult.CreateSuccessResult( );
    }



    /**
     * 创建数据推送服务
     * @param pushCallBack 触发数据推送的委托
     * @return 是否成功
     */
    public OperateResult CreatePush( ActionOperateExTwo<NetPushClient,String> pushCallBack )
    {
        action = pushCallBack;
        return CreatePush( );
    }



    /**
     * 关闭消息推送的界面
     */
    public void ClosePush()
    {
        action = null;
        if (CoreSocket != null && CoreSocket.isConnected()){
            Send(CoreSocket,Utilities.getBytes( 100 ) );
        }
        CloseSocket(CoreSocket);
    }



    /**
     * 本客户端的关键字
     * @return
     */
    public String getKeyWord(){
        return keyWord;
    }


    private String ipAddress = "";                                 // IP地址
    private int port = 1000;                                       // 端口号
    private String keyWord = "";                                   // 缓存的订阅关键字
    private ActionOperateExTwo<NetPushClient,String> action;       // 服务器推送后的回调方法


    /**
     * 获取本对象的字符串表示形式
     * @return 字符串
     */
    @Override
    public String toString() {
        return "NetPushClient";
    }
}
