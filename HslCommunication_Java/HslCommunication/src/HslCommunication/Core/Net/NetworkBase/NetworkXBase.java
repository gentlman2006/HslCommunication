package HslCommunication.Core.Net.NetworkBase;

import HslCommunication.BasicFramework.SoftBasic;
import HslCommunication.Core.Net.HslProtocol;
import HslCommunication.Core.Net.StateOne.AppSession;
import HslCommunication.Core.Types.*;
import HslCommunication.StringResources;
import HslCommunication.Utilities;

import java.io.File;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.util.Date;

/**
 * 客户端服务器的共享基类
 */
public class NetworkXBase extends NetworkBase
{

    /**
     * 默认的无参构造方法
     */
    public NetworkXBase()
    {
    }

    /**
     * 发送数据的方法
     * @param session 通信用的核心对象
     * @param content 完整的字节信息
     */
    void SendBytesAsync(AppSession session, byte[] content )
    {
        if (content == null) return;
        try
        {
            // 进入发送数据的锁，然后开启异步的数据发送
            session.getHybirdLockSend().lock();
            OutputStream outputStream = session.getWorkSocket().getOutputStream();
            outputStream.write(content);
        }
        catch (Exception ex)
        {
            if (!ex.getMessage().contains( StringResources.SocketRemoteCloseException ))
            {
                if(LogNet!=null) LogNet.WriteException( toString( ), StringResources.SocketSendException, ex );
            }
        }
        finally {
            session.getHybirdLockSend().unlock();
        }
    }


    private Thread thread;  // 后台线程

    /**
     * 开始接受数据
     * @param session 会话信息
     */
    protected void BeginReceiveBackground(AppSession session){
        thread = new Thread(){
            @Override
            public void run(){
                while (true){
                    OperateResultExOne<byte[]> readHeadBytes = Receive(session.getWorkSocket(),HslProtocol.HeadByteLength);
                    if(!readHeadBytes.IsSuccess){
                        SocketReceiveException( session );
                        return;
                    }

                    int length = Utilities.getInt(readHeadBytes.Content,28);
                    OperateResultExOne<byte[]> readContent = Receive(session.getWorkSocket(),length);
                    if(!readContent.IsSuccess){
                        SocketReceiveException( session );
                        return;
                    }

                    if (CheckRemoteToken( readHeadBytes.Content ))
                    {
                        byte[] head = readHeadBytes.Content;
                        byte[] content = HslProtocol.CommandAnalysis(head,readContent.Content);
                        int protocol = Utilities.getInt( head, 0 );
                        int customer = Utilities.getInt( head, 4 );

                        DataProcessingCenter(session,protocol,customer,content);
                    }
                    else {
                        if(LogNet!=null) LogNet.WriteWarn( toString( ), StringResources.TokenCheckFailed );
                        AppSessionRemoteClose( session );
                    }
                }
            }
        };
        thread.start();
    }

    /**
     * 数据处理中心，应该继承重写
     * @param session 连接状态
     * @param protocol 协议头
     * @param customer 用户自定义
     * @param content 数据内容
     */
    protected void DataProcessingCenter( AppSession session, int protocol, int customer, byte[] content ) {

    }

    /**
     * 检查当前的头子节信息的令牌是否是正确的
     * @param headBytes 头子节数据
     * @return 令牌是验证成功
     */
    protected boolean CheckRemoteToken( byte[] headBytes )
    {
        return SoftBasic.IsTwoBytesEquel( headBytes,12, Utilities.UUID2Byte(Token),0,16 );
    }


    /**
     * 接收出错的时候进行处理
     * @param session 会话内容
     */
    protected void SocketReceiveException( AppSession session ) {

    }


    /**
     * 当远端的客户端关闭连接时触发
     * @param session 会话内容
     */
    protected void AppSessionRemoteClose( AppSession session ) {

    }



    /**
     * [自校验] 发送字节数据并确认对方接收完成数据，如果结果异常，则结束通讯
     * @param socket 网络套接字
     * @param headcode 头指令
     * @param customer 用户指令
     * @param send 发送的数据
     * @return 是否发送成功
     */
    protected OperateResult SendBaseAndCheckReceive(Socket socket, int headcode, int customer, byte[] send )
    {
        // 数据处理
        send = HslProtocol.CommandBytes( headcode, customer, Token, send );


        OperateResult sendResult = Send( socket, send );
        if(!sendResult.IsSuccess)
        {
            return sendResult;
        }

        // 检查对方接收完成
        OperateResultExOne<Long> checkResult = ReceiveLong( socket );
        if(!checkResult.IsSuccess)
        {
            return checkResult;
        }


        // 检查长度接收
        if (checkResult.Content != send.length)
        {
            CloseSocket(socket);
            return OperateResult.CreateFailedResult(StringResources.CommandLengthCheckFailed);
        }

        return checkResult;
    }


    /**
     * [自校验] 发送字节数据并确认对方接收完成数据，如果结果异常，则结束通讯
     * @param socket 网络套接字
     * @param customer 用户指令
     * @param send 发送的数据
     * @return 是否发送成功
     */
    protected OperateResult SendBytesAndCheckReceive( Socket socket, int customer, byte[] send )
    {
        return SendBaseAndCheckReceive( socket, HslProtocol.ProtocolUserBytes, customer, send );
    }


    /**
     * [自校验] 直接发送字符串数据并确认对方接收完成数据，如果结果异常，则结束通讯
     * @param socket 网络套接字
     * @param customer 用户指令
     * @param send 发送的数据
     * @return 是否发送成功
     */
    protected OperateResult SendStringAndCheckReceive( Socket socket, int customer, String send )
    {
        byte[] data =(send == null || send.isEmpty() ) ? null : Utilities.string2Byte( send );

        return SendBaseAndCheckReceive( socket, HslProtocol.ProtocolUserString, customer, data );
    }



    /// <summary>
    /// [自校验] 将文件数据发送至套接字，如果结果异常，则结束通讯
    /// </summary>
    /// <param name="socket">网络套接字</param>
    /// <param name="filename">完整的文件路径</param>
    /// <param name="filelength">文件的长度</param>
    /// <param name="report">进度报告器</param>
    /// <returns>是否发送成功</returns>
//    protected OperateResult SendFileStreamToSocket( Socket socket, String filename, long filelength, BiConsumer<Long, Long> report )
//    {
//        try
//        {
//            OperateResult result = null;
//            FileInputStream
//            using (FileStream fs = new FileStream( filename, FileMode.Open, FileAccess.Read ))
//            {
//                result = SendStream( socket, fs, filelength, report, true );
//            }
//            return result;
//        }
//        catch (Exception ex)
//        {
//            socket?.Close( );
//            LogNet?.WriteException( ToString( ), ex );
//            return new OperateResult( )
//            {
//                Message = ex.Message
//            };
//        }
//    }


    /// <summary>
    /// [自校验] 将文件数据发送至套接字，具体发送细节将在继承类中实现，如果结果异常，则结束通讯
    /// </summary>
    /// <param name="socket">套接字</param>
    /// <param name="filename">文件名称，文件必须存在</param>
    /// <param name="servername">远程端的文件名称</param>
    /// <param name="filetag">文件的额外标签</param>
    /// <param name="fileupload">文件的上传人</param>
    /// <param name="sendReport">发送进度报告</param>
    /// <returns>是否发送成功</returns>
//    protected OperateResult SendFileAndCheckReceive(
//            Socket socket,
//            String filename,
//            String servername,
//            String filetag,
//            String fileupload,
//            BiConsumer<Long, Long> sendReport
//    )
//    {
//        // 发送文件名，大小，标签
//        File file = new File(filename);
//
//        if (!file.exists())
//        {
//            // 如果文件不存在
//            OperateResult stringResult = SendStringAndCheckReceive( socket, 0, "" );
//            if (!stringResult.IsSuccess)
//            {
//                return stringResult;
//            }
//            else
//            {
//                CloseSocket(socket);
//                OperateResult result = new OperateResult();
//                result.Message = StringResources.FileNotExist;
//                return  result;
//            }
//        }
//
//        // 文件存在的情况
//        Newtonsoft.Json.Linq.JObject json = new Newtonsoft.Json.Linq.JObject
//        {
//            { "FileName", new Newtonsoft.Json.Linq.JValue(servername) },
//            { "FileSize", new Newtonsoft.Json.Linq.JValue(file.length()) },
//            { "FileTag", new Newtonsoft.Json.Linq.JValue(filetag) },
//            { "FileUpload", new Newtonsoft.Json.Linq.JValue(fileupload) }
//        };
//
//        // 先发送文件的信息到对方
//        OperateResult sendResult = SendStringAndCheckReceive( socket, 1, json.ToString( ) );
//        if (!sendResult.IsSuccess)
//        {
//            return sendResult;
//        }
//
//        // 最后发送
//        return SendFileStreamToSocket( socket, filename, file.length(), sendReport );
//    }



    /// <summary>
    /// [自校验] 将流数据发送至套接字，具体发送细节将在继承类中实现，如果结果异常，则结束通讯
    /// </summary>
    /// <param name="socket">套接字</param>
    /// <param name="stream">文件名称，文件必须存在</param>
    /// <param name="servername">远程端的文件名称</param>
    /// <param name="filetag">文件的额外标签</param>
    /// <param name="fileupload">文件的上传人</param>
    /// <param name="sendReport">发送进度报告</param>
    /// <returns></returns>
//    protected OperateResult SendFileAndCheckReceive(
//            Socket socket,
//            Stream stream,
//            String servername,
//            String filetag,
//            String fileupload,
//            BiConsumer<Long, Long> sendReport
//    )
//    {
//        // 文件存在的情况
//        Newtonsoft.Json.Linq.JObject json = new Newtonsoft.Json.Linq.JObject
//        {
//            { "FileName", new Newtonsoft.Json.Linq.JValue(servername) },
//            { "FileSize", new Newtonsoft.Json.Linq.JValue(stream.Length) },
//            { "FileTag", new Newtonsoft.Json.Linq.JValue(filetag) },
//            { "FileUpload", new Newtonsoft.Json.Linq.JValue(fileupload) }
//        };
//
//
//        // 发送文件信息
//        OperateResult fileResult = SendStringAndCheckReceive( socket, 1, json.ToString( ) );
//        if (!fileResult.IsSuccess) return fileResult;
//
//
//        return SendStream( socket, stream, stream.count(), sendReport, true );
//    }





    /**
     * [自校验] 接收一条完整的同步数据，包含头子节和内容字节，基础的数据，如果结果异常，则结束通讯
     * @param socket 套接字
     * @param timeout 超时时间设置，如果为负数，则不检查超时
     * @return 接收的结果数据
     */
    protected OperateResultExTwo<byte[], byte[]> ReceiveAndCheckBytes( Socket socket, int timeout ) {
        // 30秒超时接收验证
        HslTimeOut hslTimeOut = new HslTimeOut();
        hslTimeOut.DelayTime = timeout;
        hslTimeOut.IsSuccessful = false;
        hslTimeOut.StartTime = new Date();
        hslTimeOut.WorkSocket = socket;


        //if (timeout > 0) ThreadPool.QueueUserWorkItem( new WaitCallback( ThreadPoolCheckTimeOut ), hslTimeOut );

        // 接收头指令
        OperateResultExOne<byte[]> headResult = Receive(socket, HslProtocol.HeadByteLength);
        if (!headResult.IsSuccess) {
            hslTimeOut.IsSuccessful = true;
            return OperateResultExTwo.<byte[],byte[]>CreateFailedResult(headResult);
        }
        hslTimeOut.IsSuccessful = true;

        // 检查令牌
        if (!CheckRemoteToken(headResult.Content)) {
            CloseSocket(socket);
            OperateResultExTwo<byte[], byte[]> resultExTwo = new OperateResultExTwo<>();
            resultExTwo.Message = StringResources.TokenCheckFailed;
            return resultExTwo;
        }

        int contentLength = Utilities.getInt(headResult.Content, HslProtocol.HeadByteLength - 4);
        // 接收内容
        OperateResultExOne<byte[]> contentResult = Receive(socket, contentLength);
        if (!contentResult.IsSuccess) {
            OperateResultExTwo<byte[], byte[]> resultExTwo = new OperateResultExTwo<>();
            resultExTwo.CopyErrorFromOther(contentResult);
            return resultExTwo;
        }

        // 返回成功信息
        OperateResult checkResult = SendLong(socket, HslProtocol.HeadByteLength + contentLength);
        if (!checkResult.IsSuccess) {
            OperateResultExTwo<byte[], byte[]> resultExTwo = new OperateResultExTwo<>();
            resultExTwo.CopyErrorFromOther(checkResult);
            return resultExTwo;
        }

        byte[] head = headResult.Content;
        byte[] content = contentResult.Content;
        content = HslProtocol.CommandAnalysis(head, content);
        return OperateResultExTwo.CreateSuccessResult(head, content);
    }


    /**
     * [自校验] 从网络中接收一个字符串数据，如果结果异常，则结束通讯
     * @param socket 套接字
     * @return 接收的结果数据
     */
    protected OperateResultExTwo<Integer, String> ReceiveStringContentFromSocket( Socket socket )
    {
        OperateResultExTwo<byte[], byte[]> receive = ReceiveAndCheckBytes( socket, 10000 );
        if (!receive.IsSuccess) {
            OperateResultExTwo<Integer, String> resultExTwo = new OperateResultExTwo<>();
            resultExTwo.CopyErrorFromOther(receive);
            return resultExTwo;
        }

        // 检查是否是字符串信息
        if (Utilities.getInt( receive.Content1, 0 ) != HslProtocol.ProtocolUserString)
        {
            if(LogNet!=null) LogNet.WriteError( toString( ), StringResources.CommandHeadCodeCheckFailed );
            CloseSocket(socket);
            OperateResultExTwo<Integer, String> resultExTwo = new OperateResultExTwo<>();
            resultExTwo.Message = StringResources.CommandHeadCodeCheckFailed;
            return resultExTwo;
        }

        if (receive.Content2 == null) receive.Content2 = new byte[0];
        // 分析数据
        return OperateResultExTwo.CreateSuccessResult( Utilities.getInt( receive.Content1, 4 ), Utilities.byte2String( receive.Content2 ) );
    }




    /**
     * [自校验] 从网络中接收一串字节数据，如果结果异常，则结束通讯
     * @param socket 套接字
     * @return 结果数据对象
     */
    protected OperateResultExTwo<Integer, byte[]> ReceiveBytesContentFromSocket( Socket socket )
    {
        OperateResultExTwo<byte[], byte[]> receive = ReceiveAndCheckBytes( socket, 10000 );
        if (!receive.IsSuccess){
            OperateResultExTwo<Integer, byte[]> resultExTwo = new OperateResultExTwo<>();
            resultExTwo.CopyErrorFromOther(receive);
            return resultExTwo;
        }

        // 检查是否是字节信息
        if (Utilities.getInt( receive.Content1, 0 ) != HslProtocol.ProtocolUserBytes)
        {
            if(LogNet!=null) LogNet.WriteError( toString( ), StringResources.CommandHeadCodeCheckFailed );
            CloseSocket(socket);

            OperateResultExTwo<Integer, byte[]> resultExTwo = new OperateResultExTwo<>();
            resultExTwo.Message = StringResources.CommandHeadCodeCheckFailed;
            return resultExTwo;
        }

        // 分析数据
        return OperateResultExTwo.CreateSuccessResult( Utilities.getInt( receive.Content1, 4 ), receive.Content2 );
    }


    /// <summary>
    /// [自校验] 从套接字中接收文件头信息
    /// </summary>
    /// <param name="socket"></param>
    /// <returns></returns>
//    protected OperateResult<FileBaseInfo> ReceiveFileHeadFromSocket( Socket socket )
//    {
//        // 先接收文件头信息
//        OperateResult<int, string> receiveString = ReceiveStringContentFromSocket( socket );
//        if (!receiveString.IsSuccess) return OperateResult.CreateFailedResult<FileBaseInfo>( receiveString );
//
//        // 判断文件是否存在
//        if (receiveString.Content1 == 0)
//        {
//            socket?.Close( );
//            LogNet?.WriteWarn( ToString( ), "对方文件不存在，无法接收！" );
//            return new OperateResult<FileBaseInfo>( )
//            {
//                Message = StringResources.FileNotExist
//            };
//        }
//
//        OperateResult<FileBaseInfo> result = new OperateResult<FileBaseInfo>( );
//        result.Content = new FileBaseInfo( );
//        try
//        {
//            // 提取信息
//            Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse( receiveString.Content2 );
//            result.Content.Name = SoftBasic.GetValueFromJsonObject( json, "FileName", "" );
//            result.Content.Size = SoftBasic.GetValueFromJsonObject( json, "FileSize", 0L );
//            result.Content.Tag = SoftBasic.GetValueFromJsonObject( json, "FileTag", "" );
//            result.Content.Upload = SoftBasic.GetValueFromJsonObject( json, "FileUpload", "" );
//            result.IsSuccess = true;
//        }
//        catch (Exception ex)
//        {
//            socket?.Close( );
//            result.Message = "提取信息失败，" + ex.Message;
//        }
//
//        return result;
//    }

    /// <summary>
    /// [自校验] 从网络中接收一个文件，如果结果异常，则结束通讯
    /// </summary>
    /// <param name="socket">网络套接字</param>
    /// <param name="savename">接收文件后保存的文件名</param>
    /// <param name="receiveReport">接收进度报告</param>
    /// <returns></returns>
//    protected OperateResult<FileBaseInfo> ReceiveFileFromSocket( Socket socket, string savename, Action<long, long> receiveReport )
//    {
//        // 先接收文件头信息
//        OperateResult<FileBaseInfo> fileResult = ReceiveFileHeadFromSocket( socket );
//        if (!fileResult.IsSuccess) return fileResult;
//
//        try
//        {
//            using (FileStream fs = new FileStream( savename, FileMode.Create, FileAccess.Write ))
//            {
//                WriteStream( socket, fs, fileResult.Content.Size, receiveReport, true );
//            }
//            return fileResult;
//        }
//        catch (Exception ex)
//        {
//            LogNet?.WriteException( ToString( ), ex );
//            socket?.Close( );
//            return new OperateResult<FileBaseInfo>( )
//            {
//                Message = ex.Message
//            };
//        }
//    }


    /// <summary>
    /// [自校验] 从网络中接收一个文件，写入数据流，如果结果异常，则结束通讯，参数顺序文件名，文件大小，文件标识，上传人
    /// </summary>
    /// <param name="socket">网络套接字</param>
    /// <param name="stream">等待写入的数据流</param>
    /// <param name="receiveReport">接收进度报告</param>
    /// <returns></returns>
//    protected OperateResult<FileBaseInfo> ReceiveFileFromSocket( Socket socket, Stream stream, Action<long, long> receiveReport )
//    {
//        // 先接收文件头信息
//        OperateResult<FileBaseInfo> fileResult = ReceiveFileHeadFromSocket( socket );
//        if (!fileResult.IsSuccess) return fileResult;
//
//        try
//        {
//            WriteStream( socket, stream, fileResult.Content.Size, receiveReport, true );
//            return fileResult;
//        }
//        catch (Exception ex)
//        {
//            LogNet?.WriteException( ToString( ), ex );
//            socket?.Close( );
//            return new OperateResult<FileBaseInfo>( )
//            {
//                Message = ex.Message
//            };
//        }
//    }


    /**
     * 删除文件的操作
     * @param filename 文件的名称
     * @return 是否删除成功
     */
    protected boolean DeleteFileByName( String filename )
    {
        try
        {
            File file = new File(filename);

            if (!file.exists()) return true;
            file.delete();
            return true;
        }
        catch (Exception ex)
        {
            if(LogNet!=null) LogNet.WriteException( toString( ), "delete file failed:" + filename, ex );
            return false;
        }
    }


    /**
     * 预处理文件夹的名称，除去文件夹名称最后一个'\'，如果有的话
     * @param folder 文件夹名称
     * @return 结果数据
     */
    protected String PreprocessFolderName( String folder )
    {
        if (folder.endsWith( "\\" ))
        {
            return folder.substring( 0, folder.length() - 1 );
        }
            else
        {
            return folder;
        }
    }










    /**
     * 从网络中接收Long数据
     * @param socket 套接字信息
     * @return 返回的结果
     */
    private OperateResultExOne<Long> ReceiveLong( Socket socket )
    {
        OperateResultExOne<byte[]> read = Receive( socket, 8 );
        if (read.IsSuccess)
        {
            return OperateResultExOne.CreateSuccessResult( Utilities.getLong( read.Content, 0 ) );
        }
        else
        {
            OperateResultExOne<Long> resultExOne = new OperateResultExOne<>();
            resultExOne.Message = read.Message;
            return resultExOne;
        }
    }

    /**
     * 将Long数据发送到套接字
     * @param socket 套接字
     * @param value 值
     * @return 返回的结果
     */
    private OperateResult SendLong( Socket socket, long value )
    {
        return Send( socket, Utilities.getBytes( value ) );
    }





    /**
     * 发送一个流的所有数据到网络套接字
     * @param socket 网络套接字
     * @param stream 输入流
     * @param receive 接收长度
     * @param report 进度报告
     * @param reportByPercent 是否按照百分比报告进度
     * @return 是否成功
     */
    protected OperateResult SendStream(Socket socket, InputStream stream, long receive, ActionOperateExTwo<Long, Long> report, boolean reportByPercent )
    {
        byte[] buffer = new byte[102400]; // 100K的数据缓存池
        long SendTotal = 0;
        long percent = 0;
        while (SendTotal < receive)
        {
            // 先从流中接收数据
            OperateResultExOne<Integer> read = ReadStream( stream, buffer );
            if (!read.IsSuccess)
            {
                OperateResult result = new OperateResult();
                result.Message = read.Message;
                return  result;
            }
            else
            {
                SendTotal += read.Content;
            }

            // 然后再异步写到socket中
            byte[] newBuffer = new byte[read.Content];
            System.arraycopy( buffer, 0, newBuffer, 0, newBuffer.length );
            OperateResult write = SendBytesAndCheckReceive( socket, read.Content, newBuffer );
            if (!write.IsSuccess)
            {
                OperateResult result = new OperateResult();
                result.Message = write.Message;
                return  result;
            }

            // 报告进度
            if (reportByPercent)
            {
                long percentCurrent = SendTotal * 100 / receive;
                if (percent != percentCurrent)
                {
                    percent = percentCurrent;
                    if(report!=null) report.Action( SendTotal, receive );
                }
            }
            else
            {
                // 报告进度
                if(report!=null) report.Action( SendTotal, receive );
            }
        }

        return OperateResult.CreateSuccessResult( );
    }


    /**
     * 从套接字中接收所有的数据然后写入到流当中去
     * @param socket 网络套接字
     * @param stream 输出流
     * @param totalLength 总长度
     * @param report 进度报告
     * @param reportByPercent 进度报告是否按照百分比
     * @return 结果类对象
     */
    protected OperateResult WriteStream(Socket socket, OutputStream stream, long totalLength, ActionOperateExTwo<Long, Long> report, boolean reportByPercent )
    {
        long count_receive = 0;
        long percent = 0;
        while (count_receive < totalLength)
        {
            // 先从流中异步接收数据
            OperateResultExTwo<Integer,byte[]> read = ReceiveBytesContentFromSocket( socket );
            if (!read.IsSuccess)
            {
                OperateResult result = new OperateResult();
                result.Message = read.Message;
                return  result;
            }
            count_receive += read.Content1;

            // 开始写入文件流
            OperateResult write = WriteStream( stream, read.Content2 );
            if (!write.IsSuccess)
            {
                OperateResult result = new OperateResult();
                result.Message = write.Message;
                return  result;
            }

            // 报告进度
            if (reportByPercent)
            {
                long percentCurrent = count_receive * 100 / totalLength;
                if (percent != percentCurrent)
                {
                    percent = percentCurrent;
                    if(report!=null) report.Action( count_receive, totalLength );
                }
            }
            else
            {
                if(report!=null) report.Action( count_receive, totalLength );
            }

        }

        return OperateResult.CreateSuccessResult( );
    }



    /**
     * 返回表示当前对象的字符串
     * @return
     */
    @Override
    public String toString()
    {
        return "NetworkXBase";
    }

}
