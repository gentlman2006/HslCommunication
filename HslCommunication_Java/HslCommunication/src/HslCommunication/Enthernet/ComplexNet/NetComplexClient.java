package HslCommunication.Enthernet.ComplexNet;

import HslCommunication.Core.Net.HslProtocol;
import HslCommunication.Core.Net.NetHandle;
import HslCommunication.Core.Net.NetworkBase.NetworkXBase;
import HslCommunication.Core.Net.StateOne.AppSession;
import HslCommunication.Core.Types.*;
import HslCommunication.Utilities;

import java.net.Socket;
import java.util.Date;

/**
 * 一个基于异步高性能的客户端网络类，支持主动接收服务器的消息
 */
public class NetComplexClient extends NetworkXBase {


    /**
     * 实例化一个对象
     */
    public NetComplexClient() {
        session = new AppSession();
        ServerTime = new Date();

    }


    private AppSession session;                  // 客户端的核心连接对象
    private int isConnecting = 0;                // 指示客户端是否处于连接服务器中，0代表未连接，1代表连接中
    private boolean IsQuie = false;              // 指示系统是否准备退出
    private Thread thread_heart_check = null;   // 心跳线程
    private String ipAddress = "";               // Ip地址
    private int port = 1000;                     // 端口号
    private boolean IsClientStart = false;         // 客户端是否启动
    private int ConnectFailedCount = 0;            // 失败重连次数
    private String ClientAlias = "";             // 客户端的别名
    private Date ServerTime = new Date();        // 服务器的时间，暂时不支持使用
    private int DelayTime = 0;                   // 获取延迟的时间

    /**
     * 获取IP地址
     *
     * @return Ip地址
     */
    public String getIpAddress() {
        return ipAddress;
    }

    /**
     * 设置Ip地址
     *
     * @param ipAddress Ip地址
     */
    public void setIpAddress(String ipAddress) {
        this.ipAddress = ipAddress;
    }

    /**
     * 获取端口号
     *
     * @return 端口号
     */
    public int getPort() {
        return port;
    }

    /**
     * 设置端口号
     *
     * @param port
     */
    public void setPort(int port) {
        this.port = port;
    }

    /**
     * 获取客户端是否启动
     *
     * @return boolean值
     */
    public boolean getIsClientStart() {
        return IsClientStart;
    }

    /**
     * 设置客户端是否启动
     *
     * @param clientStart 客户端是否启动
     */
    public void setClientStart(boolean clientStart) {
        IsClientStart = clientStart;
    }

    /**
     * 获取失败重连次数
     *
     * @return
     */
    public int getConnectFailedCount() {
        return ConnectFailedCount;
    }

    /**
     * 获取客户端的登录标识名
     *
     * @return 字符串信息
     */
    public String getClientAlias() {
        return ClientAlias;
    }

    /**
     * 设置客户端的登录标识名
     *
     * @param clientAlias
     */
    public void setClientAlias(String clientAlias) {
        ClientAlias = clientAlias;
    }


    /**
     * 获取服务器的时间
     * @return
     */
//    public Date getServerTime() {
//        return ServerTime;
//    }

    /**
     * 获取信息延迟的时间
     *
     * @return
     */
    public int getDelayTime() {
        return DelayTime;
    }


    /**
     * 客户端启动成功的事件，重连成功也将触发此事件，参数没有意义
     */
    public ActionOperate LoginSuccess;

    /**
     * 连接失败时触发的事件，参数为连接失败的次数
     */
    public ActionOperateExOne<Integer> LoginFailed;

    /**
     * 服务器的异常，启动，等等一般消息产生的时候，出发此事件
     */
    public ActionOperateExOne<String> MessageAlerts;

    /**
     * 在客户端断开后并在重连服务器之前触发，用于清理系统资源，参数无意义
     */
    public ActionOperate BeforReConnected;

    /**
     * 当接收到文本数据的时候,触发此事件
     */
    public ActionOperateExThree<NetComplexClient, NetHandle, String> AcceptString;

    /**
     * 当接收到字节数据的时候,触发此事件
     */
    public ActionOperateExThree<NetComplexClient, NetHandle, byte[]> AcceptByte;


    /**
     * 关闭该客户端引擎
     */
    public void ClientClose() {
        IsQuie = true;
        if (IsClientStart)
            SendBytes(session, HslProtocol.CommandBytes(HslProtocol.ProtocolClientQuit, 0, Token, null));

        IsClientStart = false;          // 关闭客户端
        thread_heart_check = null;

        LoginSuccess = null;            // 清空所有的事件
        LoginFailed = null;
        MessageAlerts = null;
        AcceptByte = null;
        AcceptString = null;
        try {
            session.getWorkSocket().close();
        } catch (Exception ex) {

        }
        if (LogNet != null) LogNet.WriteDebug(toString(), "Client Close.");
    }


    /**
     * 启动客户端引擎，连接服务器系统
     */
    public void ClientStart() {
        // 如果处于连接中就退出
        if (isConnecting != 0) return;
        isConnecting = 1;

        // 启动后台线程连接
        new Thread(){
            @Override
            public void run() {
                ThreadLogin();
            }
        }.start();

        // 启动心跳线程，在第一次Start的时候
        if (thread_heart_check == null) {
            thread_heart_check = new Thread(){
                @Override
                public void run() {
                    ThreadHeartCheck();
                }
            };
            thread_heart_check.start();
        }
    }

    /**
     * 连接服务器之前的消息提示，如果是重连的话，就提示10秒等待信息
     */
    private void AwaitToConnect() {
        if (ConnectFailedCount == 0) {
            // English Version : Connecting Server...
            if (MessageAlerts != null) MessageAlerts.Action("正在连接服务器...");
        } else {
            int count = 10;
            while (count > 0) {
                if (IsQuie) return;
                count--;
                // English Version : Disconnected, wait [count] second to restart
                if (MessageAlerts != null) MessageAlerts.Action("连接断开，等待" + count + "秒后重新连接");
                try {
                    Thread.sleep(1000);
                } catch (Exception ex) {

                }
            }
            if (MessageAlerts != null) MessageAlerts.Action("正在尝试第" + ConnectFailedCount + "次连接服务器...");
        }
    }

    private void ConnectFailed() {
        ConnectFailedCount++;
        isConnecting = 0;
        if (LoginFailed != null) LoginFailed.Action(ConnectFailedCount);
        if (LogNet != null) LogNet.WriteDebug(toString(), "Connected Failed, Times: " + ConnectFailedCount);
    }

    private OperateResultExOne<Socket> ConnectServer() {
        OperateResultExOne<Socket> connectResult = CreateSocketAndConnect(ipAddress, port, 10000);
        if (!connectResult.IsSuccess) {
            return connectResult;
        }

        // 连接成功，发送数据信息
        OperateResult sendResult = SendStringAndCheckReceive(connectResult.Content, 2, ClientAlias);
        if (!sendResult.IsSuccess) {
            return OperateResultExOne.<Socket>CreateFailedResult(sendResult);
        }

        if (MessageAlerts != null) MessageAlerts.Action("连接服务器成功！");
        return connectResult;
    }

    private void LoginSuccessMethod(Socket socket) {
        ConnectFailedCount = 0;
        try {
            session.setIpEndPoint(socket.getInetAddress());
            session.setLoginAlias(ClientAlias);
            session.setWorkSocket(socket);
            session.setHeartTime(new Date());
            IsClientStart = true;
            BeginReceiveBackground(session);
        } catch (Exception ex) {
            if (LogNet != null) LogNet.WriteException(toString(), ex);
        }
    }


    private void ThreadLogin() {
        // 连接的消息等待
        AwaitToConnect();

        OperateResultExOne<Socket> connectResult = ConnectServer();
        if (!connectResult.IsSuccess) {
            ConnectFailed();
            // 连接失败，重新连接服务器
            new Thread(){
                @Override
                public void run() {
                    ReconnectServer(null);
                }
            }.start();
            return;
        }

        // 登录成功
        LoginSuccessMethod(connectResult.Content);

        // 登录成功
        if (LoginSuccess != null) LoginSuccess.Action();
        isConnecting = 0;
        try {
            Thread.sleep(200);
        } catch (Exception ex) {

        }
    }


    private void ReconnectServer(Object obj) {
        // 是否连接服务器中，已经在连接的话，则不再连接
        if (isConnecting == 1) return;
        // 是否退出了系统，退出则不再重连
        if (IsQuie) return;
        // 触发连接失败，重连系统前错误
        if (BeforReConnected != null) BeforReConnected.Action();
        if (session != null) {
            CloseSocket(session.getWorkSocket());
        }
        // 重新启动客户端
        ClientStart();
    }


    /**
     * 通信出错后的处理
     *
     * @param receive 通信方法
     */
    @Override
    protected void SocketReceiveException(AppSession receive) {
        if (LogNet != null) LogNet.WriteDebug(toString(), "Socket Excepiton Occured.");
        // 异常掉线
        ReconnectServer(null);

    }


    /**
     * 服务器端用于数据发送文本的方法
     *
     * @param customer 用户自定义的命令头
     * @param str      发送的文本
     */
    public void Send(NetHandle customer, String str) {
        if (IsClientStart) {
            SendBytes(session, HslProtocol.CommandBytes(customer.get_CodeValue(), Token, str));
        }
    }


    /**
     * 服务器端用于发送字节的方法
     *
     * @param customer 用户自定义的命令头
     * @param bytes    实际发送的数据
     */
    public void Send(NetHandle customer, byte[] bytes) {
        if (IsClientStart) {
            SendBytes(session, HslProtocol.CommandBytes(customer.get_CodeValue(), Token, bytes));
        }
    }

    private void SendBytes(AppSession stateone, byte[] content) {
        super.Send(stateone.getWorkSocket(), content);
    }


    /**
     * 客户端的数据处理中心
     *
     * @param session  连接状态
     * @param protocol 协议头
     * @param customer 用户自定义
     * @param content  数据内容
     */
    @Override
    protected void DataProcessingCenter(AppSession session, int protocol, int customer, byte[] content) {
        if (protocol == HslProtocol.ProtocolCheckSecends) {
            Date dt = new Date(Utilities.getLong(content, 0));
            ServerTime = new Date(Utilities.getLong(content, 8));
            DelayTime = (int) (new Date().getTime() - dt.getTime());
            this.session.setHeartTime(new Date());
            // MessageAlerts?.Invoke("心跳时间：" + DateTime.Now.ToString());
        } else if (protocol == HslProtocol.ProtocolClientQuit) {
            // 申请了退出
        } else if (protocol == HslProtocol.ProtocolUserBytes) {
            // 接收到字节数据
            if (AcceptByte != null) AcceptByte.Action(this, new NetHandle(customer), content);
        } else if (protocol == HslProtocol.ProtocolUserString) {
            // 接收到文本数据
            String str = Utilities.byte2String(content);
            if (AcceptString != null) AcceptString.Action(this, new NetHandle(customer), str);
        }
    }


    /**
     * 心跳线程的方法
     */
    private void ThreadHeartCheck() {
        try {
            Thread.sleep(2000);
        } catch (Exception ex) {

        }
        while (true) {
            try {
                Thread.sleep(1000);
            } catch (Exception ex) {

            }

            if (!IsQuie) {
                byte[] send = new byte[16];
                byte[] buffer = Utilities.getBytes(new Date().getTime());
                System.arraycopy(buffer, 0, send, 0, buffer.length);

                try {
                    SendBytes(session, HslProtocol.CommandBytes(HslProtocol.ProtocolCheckSecends, 0, Token, send));
                    double timeSpan = (new Date().getTime() - session.getHeartTime().getTime()) / 1000;
                    if (timeSpan > 1 * 8)//8次没有收到失去联系
                    {
                        if (isConnecting == 0) {
                            if (LogNet != null)
                                LogNet.WriteDebug(toString(), "Heart Check Failed int " + timeSpan + " Seconds.");
                            new Thread(){
                                @Override
                                public void run() {
                                    ReconnectServer(null);
                                }
                            }.start();
                        }
                        if (!IsQuie) {
                            try {
                                Thread.sleep(1000);
                            } catch (Exception ex) {

                            }
                        }
                    }

                } catch (Exception ex) {
                    System.out.println(ex.getStackTrace());
                }
            } else {
                break;
            }
        }


    }


    /**
     * 返回对象的字符串表示形式
     *
     * @return 字符串
     */
    @Override
    public String toString() {
        return "NetComplexClient";
    }
}
