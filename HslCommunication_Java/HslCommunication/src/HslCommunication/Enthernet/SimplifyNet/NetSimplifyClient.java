package HslCommunication.Enthernet.SimplifyNet;

import HslCommunication.Core.IMessage.HslMessage;
import HslCommunication.Core.Net.HslProtocol;
import HslCommunication.Core.Net.NetHandle;
import HslCommunication.Core.Net.NetworkBase.NetworkDoubleBase;
import HslCommunication.Core.Transfer.RegularByteTransform;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.Utilities;


/**
 * 同步访问的网络客户端
 */
public class NetSimplifyClient extends NetworkDoubleBase<HslMessage,RegularByteTransform>
{


    /**
     * 实例化一个客户端的对象，用于和服务器通信
     * @param ipAddress Ip地址
     * @param port 端口号
     */
    public NetSimplifyClient(String ipAddress, int port)
    {
        this.setIpAddress(ipAddress);
        this.setPort( port);
    }


    /**
     * 实例化一个客户端对象，需要手动指定Ip地址和端口
     */
    public NetSimplifyClient()
    {

    }


    /**
     * 客户端向服务器进行请求，请求字符串数据
     * @param customer 用户的指令头
     * @param send 发送数据
     * @return 带结果说明的对象
     */
    public OperateResultExOne<String> ReadFromServer(NetHandle customer, String send) {
        OperateResultExOne<String> result = new OperateResultExOne<String>();
        byte[] data = send.isEmpty() ? new byte[0] : Utilities.string2Byte(send);
        OperateResultExOne<byte[]> temp = ReadFromServerBase(HslProtocol.ProtocolUserString, customer.get_CodeValue(), data);
        result.IsSuccess = temp.IsSuccess;
        result.ErrorCode = temp.ErrorCode;
        result.Message = temp.Message;
        if (temp.IsSuccess) {
            result.Content = Utilities.byte2String(temp.Content);
        }
        temp = null;
        return result;
    }


    /**
     * 客户端向服务器进行请求，请求字节数据
     * @param customer 用户的指令头
     * @param send 发送的字节内容
     * @return 带结果说明的对象
     */
    public OperateResultExOne<byte[]> ReadFromServer(NetHandle customer, byte[] send) {
        return ReadFromServerBase(HslProtocol.ProtocolUserBytes, customer.get_CodeValue(), send);
    }


    /**
     * 需要发送的底层数据
     * @param headcode 数据的指令头
     * @param customer 需要发送的底层数据
     * @param send 需要发送的底层数据
     * @return
     */
    private OperateResultExOne<byte[]> ReadFromServerBase(int headcode, int customer, byte[] send) {
        OperateResultExOne<byte[]> read = ReadFromCoreServer(HslProtocol.CommandBytes(headcode, customer, Token, send));
        if (!read.IsSuccess) {
            return read;
        }

        byte[] headBytes = new byte[HslProtocol.HeadByteLength];
        byte[] contentBytes = new byte[read.Content.length - HslProtocol.HeadByteLength];

        System.arraycopy(read.Content,0,headBytes,0,HslProtocol.HeadByteLength);
        if (contentBytes.length > 0) {
            System.arraycopy(read.Content, HslProtocol.HeadByteLength, contentBytes, 0, read.Content.length - HslProtocol.HeadByteLength);
        }

        contentBytes = HslProtocol.CommandAnalysis(headBytes, contentBytes);
        return OperateResultExOne.CreateSuccessResult(contentBytes);
    }



    /**
     * 获取本对象的字符串表示形式
     * @return 字符串信息
     */
    @Override
    public String toString() {
        return "NetSimplifyClient";
    }


}
