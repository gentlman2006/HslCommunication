package HslCommunication.Core.Net;

import HslCommunication.BasicFramework.*;
import HslCommunication.Core.Security.HslSecurity;
import java.util.UUID;
import HslCommunication.Utilities;

public class HslProtocol
{

    /**
     * 规定所有的网络传输的指令头长度
     */
    public static final int HeadByteLength = 32;

    /**
     * 所有网络通信中的缓冲池的数据信息
     */
    public static final int ProtocolBufferSize = 1024;

    /**
     * 用于心跳程序的暗号信息
     */
    public static final int ProtocolCheckSecends = 1;

    /**
     * 客户端退出的消息
     */
    public static final int ProtocolClientQuit = 2;

    /**
     * 因为客户端达到上限而拒绝登录
     */
    public static final int ProtocolClientRefuseLogin = 3;

    /**
     * 允许客户端登录到服务器
     */
    public static final int ProtocolClientAllowLogin = 4;

    /**
     * 说明发送的信息是文本数据
     */
    public static final int ProtocolUserString = 1001;

    /**
     * 说明发送的信息是字节数组数据
     */
    public static final int ProtocolUserBytes = 1002;

    /**
     * 发送的数据是普通的图片数据
     */
    public static final int ProtocolUserBitmap = 1003;

    /**
     * 发送的数据是一条异常的数据，字符串为异常消息
     */
    public static final int ProtocolUserException = 1004;

    /**
     * 请求文件下载的暗号
     */
    public static final int ProtocolFileDownload = 2001;

    /**
     * 请求文件上传的暗号
     */
    public static final int ProtocolFileUpload = 2002;

    /**
     * 请求删除文件的暗号
     */
    public static final int ProtocolFileDelete = 2003;

    /**
     * 文件校验成功
     */
    public static final int ProtocolFileCheckRight = 2004;

    /**
     * 文件校验失败
     */
    public static final int ProtocolFileCheckError = 2005;

    /**
     * 文件保存失败
     */
    public static final int ProtocolFileSaveError = 2006;

    /**
     * 请求文件的列表的暗号
     */
    public static final int ProtocolFileDirectoryFiles = 2007;

    /**
     * 请求子文件的列表暗号
     */
    public static final int ProtocolFileDirectories = 2008;

    /**
     * 进度返回暗号
     */
    public static final int ProtocolProgressReport = 2009;


    /**
     * 不压缩字节数据
     */
    public static final int ProtocolNoZipped = 3001;

    /**
     * 压缩字节数据
     */
    public static final int ProtocolZipped  = 3002;



    /**
     * 生成终极传送指令的方法，所有的数据均通过该方法出来
     * @param command 命令头
     * @param customer 自用自定义
     * @param token 令牌
     * @param data 字节数据
     * @return 发送的消息数据
     */
    public static byte[] CommandBytes(int command, int customer, UUID token, byte[] data )
    {
        byte[] _temp = null;
        int _zipped = ProtocolNoZipped;
        int _sendLength = 0;
        if (data == null)
        {
            _temp = new byte[HeadByteLength];
        }
        else
        {
            // 加密
            data = HslSecurity.ByteEncrypt(data);
            if (data.length > 10240)
            {
                // 10K以上的数据，进行数据压缩
                data = SoftZipped.CompressBytes(data);
                _zipped = ProtocolZipped;
            }
            _temp = new byte[HeadByteLength + data.length];
            _sendLength = data.length;
        }

        Utilities.getBytes(command);

        System.arraycopy(Utilities.getBytes(command),0,_temp,0,4);
        System.arraycopy(Utilities.getBytes(customer),0,_temp,4,4);
        System.arraycopy(Utilities.getBytes(_zipped),0,_temp,8,4);
        System.arraycopy(Utilities.UUID2Byte(token),0,_temp,12,16);
        System.arraycopy(Utilities.getBytes(_sendLength),0,_temp,28,4);
        if (_sendLength > 0)
        {
            System.arraycopy(data,0,_temp,32,_sendLength);
        }
        return _temp;
    }


    /**
     * 解析接收到数据，先解压缩后进行解密
     * @param head
     * @param content
     * @return
     */
    public static byte[] CommandAnalysis( byte[] head, byte[] content )
    {
        if (content != null)
        {
            byte[] buffer = new byte[4];
            buffer[0] = head[8];
            buffer[1] = head[9];
            buffer[2] = head[10];
            buffer[3] = head[11];

            // 获取是否压缩的情况
            int _zipped = Utilities.getInt(buffer,0);


            // 先进行解压
            if (_zipped == ProtocolZipped)
            {
                content = SoftZipped.Decompress( content );
            }
            // 进行解密
            return HslSecurity.ByteDecrypt( content );
        }
        else
        {
            return null;
        }
    }



    /**
     * 获取发送字节数据的实际数据，带指令头
     * @param customer
     * @param token
     * @param data
     * @return
     */
    public static byte[] CommandBytes( int customer, UUID token, byte[] data )
    {
        return CommandBytes( ProtocolUserBytes, customer, token, data );
    }


    /**
     * 获取发送字节数据的实际数据，带指令头
     * @param customer
     * @param token
     * @param data
     * @return
     */
    public static byte[] CommandBytes( int customer, UUID token, String data )
    {
        if (data == null) return CommandBytes( ProtocolUserString, customer, token, null );
        else return CommandBytes( ProtocolUserString, customer, token,  Utilities.string2Byte(data) );
    }


}
