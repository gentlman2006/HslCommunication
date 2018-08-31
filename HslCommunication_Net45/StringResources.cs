using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HslCommunication
{

    /*******************************************************************************
     * 
     *    用于显示和保存的数据信息，未来支持中英文
     *
     *    Used to the return result class in the synchronize communication and communication for industrial Ethernet
     * 
     *******************************************************************************/




    internal class StringResources
    {

        /***********************************************************************************
         * 
         *    一般的错误信息
         * 
         ************************************************************************************/


        internal const string ConnectedFailed = "连接失败";
        internal const string UnknownError = "未知错误";
        internal const string ErrorCode = "错误代号";
        internal const string TextDescription = "文本描述";
        internal const string ExceptionMessage = "错误信息：";
        internal const string ExceptionStackTrace = "错误堆栈：";
        internal const string ExceptopnTargetSite = "错误方法：";
        internal const string ExceprionCustomer = "用户自定义方法出错：";
        internal const string SuccessText = "Success";



        /***********************************************************************************
         * 
         *    系统相关的错误信息
         * 
         ************************************************************************************/

        internal const string SystemInstallOperater = "安装新系统：IP为";
        internal const string SystemUpdateOperater = "更新新系统：IP为";


        /***********************************************************************************
         * 
         *    套接字相关的信息描述
         * 
         ************************************************************************************/

        internal const string SocketIOException = "套接字传送数据异常：";
        internal const string SocketSendException = "同步数据发送异常：";
        internal const string SocketHeadReceiveException = "指令头接收异常：";
        internal const string SocketContentReceiveException = "内容数据接收异常：";
        internal const string SocketContentRemoteReceiveException = "对方内容数据接收异常：";
        internal const string SocketAcceptCallbackException = "异步接受传入的连接尝试";
        internal const string SocketReAcceptCallbackException = "重新异步接受传入的连接尝试";
        internal const string SocketSendAsyncException = "异步数据发送出错:";
        internal const string SocketEndSendException = "异步数据结束挂起发送出错";
        internal const string SocketReceiveException = "异步数据发送出错:";
        internal const string SocketEndReceiveException = "异步数据结束接收指令头出错";
        internal const string SocketRemoteCloseException = "远程主机强迫关闭了一个现有的连接";


        /***********************************************************************************
         * 
         *    文件相关的信息
         * 
         ************************************************************************************/


        internal const string FileDownloadSuccess = "文件下载成功";
        internal const string FileDownloadFailed = "文件下载异常";
        internal const string FileUploadFailed = "文件上传异常";
        internal const string FileUploadSuccess = "文件上传成功";
        internal const string FileDeleteFailed = "文件删除异常";
        internal const string FileDeleteSuccess = "文件删除成功";
        internal const string FileReceiveFailed = "确认文件接收异常";
        internal const string FileNotExist = "文件不存在";
        internal const string FileSaveFailed = "文件存储失败";
        internal const string FileLoadFailed = "文件加载失败";
        internal const string FileSendClientFailed = "文件发送的时候发生了异常";
        internal const string FileWriteToNetFailed = "文件写入网络异常";
        internal const string FileReadFromNetFailed = "从网络读取文件异常";
        internal const string FilePathCreateFailed = "文件夹路径创建失败";

        /***********************************************************************************
         * 
         *    服务器的引擎相关数据
         * 
         ************************************************************************************/

        internal const string TokenCheckFailed = "接收验证令牌不一致";
        internal const string TokenCheckTimeout = "接收验证超时:";
        internal const string CommandHeadCodeCheckFailed = "命令头校验失败";
        internal const string CommandLengthCheckFailed = "命令长度检查失败";
        internal const string NetClientAliasFailed = "客户端的别名接收失败：";
        internal const string NetEngineStart = "启动引擎";
        internal const string NetEngineClose = "关闭引擎";
        internal const string NetClientOnline = "上线";
        internal const string NetClientOffline = "下线";
        internal const string NetClientBreak = "异常掉线";
        internal const string NetClientFull = "服务器承载上限，收到超出的请求连接。";
        internal const string NetClientLoginFailed = "客户端登录中错误：";


        /***********************************************************************************
         * 
         *    Modbus-Tcp相关
         * 
         ************************************************************************************/

        internal const string ModbusTcpFunctionCodeNotSupport = "不支持的功能码";
        internal const string ModbusTcpFunctionCodeOverBound = "读取的数据越界";
        internal const string ModbusTcpFunctionCodeQuantityOver = "读取长度超过最大值";
        internal const string ModbusTcpFunctionCodeReadWriteException = "读写异常";
        internal const string ModbusTcpReadCoilException = "读取线圈异常";
        internal const string ModbusTcpWriteCoilException = "写入线圈异常";
        internal const string ModbusTcpReadRegisterException = "读取寄存器异常";
        internal const string ModbusTcpWriteRegisterException = "写入寄存器异常";



        /***********************************************************************************
         * 
         *    AB PLC 相关
         * 
         ************************************************************************************/

            
        internal const string AllenBradley04 = "它没有正确生成或匹配标记不存在。"; // The IOI could not be deciphered. Either it was not formed correctly or the match tag does not exist
        internal const string AllenBradley05 = "引用的特定项（通常是实例）无法找到。"; // The particular item referenced (usually instance) could not be found.
        internal const string AllenBradley06 = "请求的数据量不适合响应缓冲区。 发生了部分数据传输。"; // The amount of data requested would not fit into the response buffer. Partial data transfer has occurred.
        internal const string AllenBradley0A = "尝试处理其中一个属性时发生错误。";                     // An error has occurred trying to process one of the attributes.
        internal const string AllenBradley13 = "命令中没有提供足够的命令数据/参数来执行所请求的服务。"; // Not enough command data / parameters were supplied in the command to execute the service requested.
        internal const string AllenBradley1C = "与属性计数相比，提供的属性数量不足。"; // An insufficient number of attributes were provided compared to the attribute count.
        internal const string AllenBradley1E = "此服务中的服务请求出错。"; // A service request in this service went wrong.
        internal const string AllenBradley26 = "IOI字长与处理的IOI数量不匹配。"; // The IOI word length did not match the amount of IOI which was processed.
    }
}
