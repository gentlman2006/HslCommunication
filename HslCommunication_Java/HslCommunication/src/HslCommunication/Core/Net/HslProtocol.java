package HslCommunication.Core.Net;

import HslCommunication.BasicFramework.*;
import HslCommunication.Core.Security.HslSecurity;
import java.util.UUID;

public class HslProtocol
{

    /// <summary>
    /// 规定所有的网络传输指令头都为32字节
    /// </summary>
    public static final int HeadByteLength = 32;

    /// <summary>
    /// 所有网络通信中的缓冲池数据信息
    /// </summary>
    public static final int ProtocolBufferSize = 1024;


    /// <summary>
    /// 用于心跳程序的暗号信息
    /// </summary>
    public static final int ProtocolCheckSecends = 1;

    /// <summary>
    /// 客户端退出消息
    /// </summary>
    public static final int ProtocolClientQuit = 2;

    /// <summary>
    /// 因为客户端达到上限而拒绝登录
    /// </summary>
    public static final int ProtocolClientRefuseLogin = 3;

    /// <summary>
    /// 允许客户端登录到服务器
    /// </summary>
    public static final int ProtocolClientAllowLogin = 4;




    /// <summary>
    /// 说明发送的只是文本信息
    /// </summary>
    public static final int ProtocolUserString = 1001;

    /// <summary>
    /// 发送的数据就是普通的字节数组
    /// </summary>
    public static final int ProtocolUserBytes = 1002;

    /// <summary>
    /// 发送的数据就是普通的图片数据
    /// </summary>
    public static final int ProtocolUserBitmap = 1003;

    /// <summary>
    /// 发送的数据是一条异常的数据，字符串为异常消息
    /// </summary>
    public static final int ProtocolUserException = 1004;


    /// <summary>
    /// 请求文件下载的暗号
    /// </summary>
    public static final int ProtocolFileDownload = 2001;

    /// <summary>
    /// 请求文件上传的暗号
    /// </summary>
    public static final int ProtocolFileUpload = 2002;
    /// <summary>
    /// 请求删除文件的暗号
    /// </summary>
    public static final int ProtocolFileDelete = 2003;

    /// <summary>
    /// 文件校验成功
    /// </summary>
    public static final int ProtocolFileCheckRight = 2004;
    /// <summary>
    /// 文件校验失败
    /// </summary>
    public static final int ProtocolFileCheckError = 2005;
    /// <summary>
    /// 文件保存失败
    /// </summary>
    public static final int ProtocolFileSaveError = 2006;
    /// <summary>
    /// 请求文件列表的暗号
    /// </summary>
    public static final int ProtocolFileDirectoryFiles = 2007;
    /// <summary>
    /// 请求子文件的列表暗号
    /// </summary>
    public static final int ProtocolFileDirectories = 2008;
    /// <summary>
    /// 进度返回暗号
    /// </summary>
    public static final int ProtocolProgressReport = 2009;




    /// <summary>
    /// 不压缩数据字节
    /// </summary>
    public static final int ProtocolNoZipped = 3001;
    /// <summary>
    /// 压缩数据字节
    /// </summary>
    public static final int ProtocolZipped  = 3002;



    /// <summary>
    /// 生成终极传送指令的方法，所有的数据均通过该方法出来
    /// </summary>
    /// <param name="command">命令头</param>
    /// <param name="customer">自用自定义</param>
    /// <param name="token">令牌</param>
    /// <param name="data">字节数据</param>
    /// <returns></returns>
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
            data = HslSecurity.ByteEncrypt( data );
            if (data.length > 102400)
            {
                // 100K以上的数据，进行数据压缩
                data = SoftZipped.CompressBytes( data );
                _zipped = ProtocolZipped;
            }
            _temp = new byte[HeadByteLength + data.length];
            _sendLength = data.Length;
        }

        BitConverter.GetBytes( command ).CopyTo( _temp, 0 );
        BitConverter.GetBytes( customer ).CopyTo( _temp, 4 );
        BitConverter.GetBytes( _zipped ).CopyTo( _temp, 8 );
        token.ToByteArray( ).CopyTo( _temp, 12 );
        BitConverter.GetBytes( _sendLength ).CopyTo( _temp, 28 );

        if (_sendLength > 0)
        {
            Array.Copy( data, 0, _temp, 32, _sendLength );
        }
        return _temp;
    }


    /// <summary>
    /// 解析接收到数据，先解压缩后进行解密
    /// </summary>
    /// <param name="head"></param>
    /// <param name="content"></param>
    public static byte[] CommandAnalysis( byte[] head, byte[] content )
    {
        if (content != null)
        {
            int _zipped = BitConverter.ToInt32( head, 8 );
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


    /// <summary>
    /// 获取发送字节数据的实际数据，带指令头
    /// </summary>
    /// <param name="customer"></param>
    /// <param name="token"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] CommandBytes( int customer, UUID token, byte[] data )
    {
        return CommandBytes( ProtocolUserBytes, customer, token, data );
    }


    /// <summary>
    /// 获取发送字节数据的实际数据，带指令头
    /// </summary>
    /// <param name="customer"></param>
    /// <param name="token"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] CommandBytes( int customer, UUID token, String data )
    {
        if (data == null) return CommandBytes( ProtocolUserString, customer, token, null );
        else return CommandBytes( ProtocolUserString, customer, token, Encoding.Unicode.GetBytes( data ) );
    }


}
