package HslCommunication.Core.Net.StateOne;

import HslCommunication.BasicFramework.SoftBasic;
import HslCommunication.Core.Net.HslProtocol;

import java.net.InetAddress;
import java.net.Socket;
import java.util.Date;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

/**
 * 网络会话信息
 */
public class AppSession {

    /**
     * 实例化一个构造方法
     */
    public AppSession() {

        ClientUniqueID = SoftBasic.GetUniqueStringByGuidAndRandom();
        HybirdLockSend = new ReentrantLock();
        HeartTime = new Date();
        BytesHead = new byte[HslProtocol.HeadByteLength];
    }


    /**
     * 获取IP地址信息
     * @return
     */
    public String getIpAddress() {
        return IpAddress;
    }

    /**
     * 设置IP地址信息
     * @param ipAddress
     */
    void setIpAddress(String ipAddress) {
        IpAddress = ipAddress;
    }

    /**
     * 获取此对象连接的远程客户端
     * @return 远程客户端
     */
    public InetAddress getIpEndPoint() {
        return IpEndPoint;
    }

    /**
     * 设置此对象的远程连接客户端
     * @param ipEndPoint 远程客户端
     */
    public void setIpEndPoint(InetAddress ipEndPoint) {
        IpEndPoint = ipEndPoint;
    }

    /**
     * 获取远程对象的别名
     * @return 别名
     */
    public String getLoginAlias() {
        return LoginAlias;
    }

    /**
     * 设置远程对象的别名
     * @param loginAlias 别名
     */
    public void setLoginAlias(String loginAlias) {
        LoginAlias = loginAlias;
    }

    /**
     * 获取当前的心跳时间
     * @return 心跳时间
     */
    public Date getHeartTime() {
        return HeartTime;
    }

    /**
     * 设置当前的心跳时间
     * @param date 心跳时间
     */
    public void setHeartTime(Date date){
        this.HeartTime = date;
    }

    /**
     * 获取客户端的类型
     * @return 客户端类型
     */
    public String getClientType() {
        return ClientType;
    }

    /**
     * 设置客户端的类型
     * @param clientType 客户端的类型
     */
    public void setClientType(String clientType) {
        ClientType = clientType;
    }

    /**
     * 获取客户端的唯一的标识
     * @return
     */
    public String getClientUniqueID() {
        return ClientUniqueID;
    }


    private String IpAddress = null;
    private InetAddress IpEndPoint = null;
    private String LoginAlias = null;
    private Date HeartTime = null;
    private String ClientType = null;
    private String ClientUniqueID = null;
    private byte[] BytesHead = null;
    private byte[] BytesContent = null;
    private String KeyGroup = null;
    private Socket WorkSocket = null;
    private Lock HybirdLockSend = null;

    /**
     * 获取头子节信息
     * @return 字节数组
     */
    public byte[] getBytesHead() {
        return BytesHead;
    }

    /**
     * 设置头子节信息
     * @param bytesHead 头子节数组
     */
    public void setBytesHead(byte[] bytesHead) {
        BytesHead = bytesHead;
    }

    /**
     * 获取内容字节
     * @return 字节数组
     */
    public byte[] getBytesContent() {
        return BytesContent;
    }

    /**
     * 设置内容字节
     * @param bytesContent 字节数组
     */
    public void setBytesContent(byte[] bytesContent) {
        BytesContent = bytesContent;
    }

    /**
     * 获取用于分类的关键字
     * @return 关键字
     */
    public String getKeyGroup() {
        return KeyGroup;
    }

    /**
     * 设置用于分类的关键字
     * @param keyGroup 关键字
     */
    public void setKeyGroup(String keyGroup) {
        KeyGroup = keyGroup;
    }

    /**
     * 获取网络套接字
     * @return socket对象
     */
    public Socket getWorkSocket() {
        return WorkSocket;
    }

    /**
     * 设置网络套接字
     * @param workSocket socket对象
     */
    public void setWorkSocket(Socket workSocket) {
        WorkSocket = workSocket;
    }

    /**
     * 获取同步锁
     * @return
     */
    public Lock getHybirdLockSend() {
        return HybirdLockSend;
    }





    /**
     * 清除本次的接收内容
     */
    public void Clear() {
        BytesHead = new byte[HslProtocol.HeadByteLength];
        BytesContent = null;
    }


    /**
     * 返回表示当前对象的字符串，以IP，端口，客户端名称组成
     * @return 字符串数据
     */
    @Override
    public String toString() {
        if (LoginAlias.isEmpty()) {
            return "[" + IpEndPoint.toString() + "]";
        } else {
            return "[" + IpEndPoint + "] [" + LoginAlias + "]";
        }
    }

}
