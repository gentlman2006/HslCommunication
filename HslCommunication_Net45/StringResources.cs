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
        internal const string ModbusAddressMustMoreThanOne = "地址值在起始地址为1的情况下，必须大于1";
        internal const string ModbusAsciiFormatCheckFailed = "Modbus的ascii指令检查失败，不是modbus-ascii报文";
        internal const string ModbusCRCCheckFailed = "Modbus的CRC校验检查失败";
        internal const string ModbusLRCCheckFailed = "Modbus的LRC校验检查失败";


        /***********************************************************************************
         * 
         *    AB PLC 相关
         * 
         ************************************************************************************/


        internal const string AllenBradley04 = "它没有正确生成或匹配标记不存在。"; // The IOI could not be deciphered. Either it was not formed correctly or the match tag does not exist.
        internal const string AllenBradley05 = "引用的特定项（通常是实例）无法找到。"; // The particular item referenced (usually instance) could not be found.
        internal const string AllenBradley06 = "请求的数据量不适合响应缓冲区。 发生了部分数据传输。"; // The amount of data requested would not fit into the response buffer. Partial data transfer has occurred.
        internal const string AllenBradley0A = "尝试处理其中一个属性时发生错误。";                     // An error has occurred trying to process one of the attributes.
        internal const string AllenBradley13 = "命令中没有提供足够的命令数据/参数来执行所请求的服务。"; // Not enough command data / parameters were supplied in the command to execute the service requested.
        internal const string AllenBradley1C = "与属性计数相比，提供的属性数量不足。"; // An insufficient number of attributes were provided compared to the attribute count.
        internal const string AllenBradley1E = "此服务中的服务请求出错。"; // A service request in this service went wrong.
        internal const string AllenBradley26 = "IOI字长与处理的IOI数量不匹配。"; // The IOI word length did not match the amount of IOI which was processed.

        internal const string AllenBradleySessionStatus00 = "成功"; // success
        internal const string AllenBradleySessionStatus01 = "发件人发出无效或不受支持的封装命令。"; // The sender issued an invalid or unsupported encapsulation command.
        // Insufficient memory resources in the receiver to handle the command. This is not an application error. Instead, it only results if the encapsulation layer cannot obtain memory resources that it need.
        internal const string AllenBradleySessionStatus02 = "接收器中的内存资源不足以处理命令。 这不是一个应用程序错误。 相反，只有在封装层无法获得所需内存资源的情况下才会导致此问题。";
        internal const string AllenBradleySessionStatus03 = "封装消息的数据部分中的数据形成不良或不正确。"; // Poorly formed or incorrect data in the data portion of the encapsulation message.
        internal const string AllenBradleySessionStatus64 = "向目标发送封装消息时，始发者使用了无效的会话句柄。"; // An originator used an invalid session handle when sending an encapsulation message.
        internal const string AllenBradleySessionStatus65 = "目标收到一个无效长度的信息。"; // The target received a message of invalid length.
        internal const string AllenBradleySessionStatus69 = "不支持的封装协议修订。"; // Unsupported encapsulation protocol revision.

        /***********************************************************************************
         * 
         *    Panasonic PLC 相关
         * 
         ************************************************************************************/
        internal const string PanasonicMewStatus20 = "错误未知";
        internal const string PanasonicMewStatus21 = "NACK错误，远程单元无法被正确识别，或者发生了数据错误。";
        internal const string PanasonicMewStatus22 = "WACK 错误:用于远程单元的接收缓冲区已满。";
        internal const string PanasonicMewStatus23 = "多重端口错误:远程单元编号(01 至 16)设置与本地单元重复。";
        internal const string PanasonicMewStatus24 = "传输格式错误:试图发送不符合传输格式的数据，或者某一帧数据溢出或发生了数据错误。";
        internal const string PanasonicMewStatus25 = "硬件错误:传输系统硬件停止操作。";
        internal const string PanasonicMewStatus26 = "单元号错误:远程单元的编号设置超出 01 至 63 的范围。";
        internal const string PanasonicMewStatus27 = "不支持错误:接收方数据帧溢出. 试图在不同的模块之间发送不同帧长度的数据。";
        internal const string PanasonicMewStatus28 = "无应答错误:远程单元不存在. (超时)。";
        internal const string PanasonicMewStatus29 = "缓冲区关闭错误:试图发送或接收处于关闭状态的缓冲区。";
        internal const string PanasonicMewStatus30 = "超时错误:持续处于传输禁止状态。";
        internal const string PanasonicMewStatus40 = "BCC 错误:在指令数据中发生传输错误。";
        internal const string PanasonicMewStatus41 = "格式错误:所发送的指令信息不符合传输格式。";
        internal const string PanasonicMewStatus42 = "不支持错误:发送了一个未被支持的指令。向未被支持的目标站发送了指令。";
        internal const string PanasonicMewStatus43 = "处理步骤错误:在处于传输请求信息挂起时,发送了其他指令。";
        internal const string PanasonicMewStatus50 = "链接设置错误:设置了实际不存在的链接编号。";
        internal const string PanasonicMewStatus51 = "同时操作错误:当向其他单元发出指令时,本地单元的传输缓冲区已满。";
        internal const string PanasonicMewStatus52 = "传输禁止错误:无法向其他单元传输。";
        internal const string PanasonicMewStatus53 = "忙错误:在接收到指令时,正在处理其他指令。";
        internal const string PanasonicMewStatus60 = "参数错误:在指令中包含有无法使用的代码,或者代码没有附带区域指定参数(X, Y, D), 等以外。";
        internal const string PanasonicMewStatus61 = "数据错误:触点编号,区域编号,数据代码格式(BCD,hex,等)上溢出, 下溢出以及区域指定错误。";
        internal const string PanasonicMewStatus62 = "寄存器错误:过多记录数据在未记录状态下的操作（监控记录、跟踪记录等。)。";
        internal const string PanasonicMewStatus63 = "PLC 模式错误:当一条指令发出时，运行模式不能够对指令进行处理。";
        internal const string PanasonicMewStatus65 = "保护错误:在存储保护状态下执行写操作到程序区域或系统寄存器。";
        internal const string PanasonicMewStatus66 = "地址错误:地址（程序地址、绝对地址等）数据编码形式（BCD、hex 等）、上溢、下溢或指定范围错误。";
        internal const string PanasonicMewStatus67 = "丢失数据错误:要读的数据不存在。（读取没有写入注释寄存区的数据。。";
    }
}
