using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Language
{
    /// <summary>
    /// 系统的语言基类，默认也即是中文版本
    /// </summary>
    public class DefaultLanguage
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        /***********************************************************************************
         * 
         *    一般的错误信息
         * 
         ************************************************************************************/

        public virtual string ConnectedFailed => "连接失败：";
        public virtual string UnknownError => "未知错误";
        public virtual string ErrorCode => "错误代号";
        public virtual string TextDescription => "文本描述";
        public virtual string ExceptionMessage => "错误信息：";
        public virtual string ExceptionSourse => "错误源：";
        public virtual string ExceptionType => "错误类型：";
        public virtual string ExceptionStackTrace => "错误堆栈：";
        public virtual string ExceptopnTargetSite => "错误方法：";
        public virtual string ExceprionCustomer => "用户自定义方法出错：";
        public virtual string SuccessText => "成功";
        public virtual string TwoParametersLengthIsNotSame => "两个参数的个数不一致";
        public virtual string NotSupportedDataType => "输入的类型不支持，请重新输入";
        public virtual string DataLengthIsNotEnough => "接收的数据长度不足，应该值:{0},实际值:{1}";
        public virtual string ReceiveDataTimeout => "接收数据超时：";
        public virtual string ReceiveDataLengthTooShort => "接收的数据长度太短：";
        public virtual string MessageTip => "消息提示：";
        public virtual string Close => "关闭";
        public virtual string Time => "时间：";
        public virtual string SoftWare => "软件：";
        public virtual string BugSubmit => "Bug提交";
        public virtual string MailServerCenter => "邮件发送系统";
        public virtual string MailSendTail => "邮件服务系统自动发出，请勿回复！";
        public virtual string IpAddresError => "Ip地址输入异常，格式不正确";
        public virtual string Send => "发送";
        public virtual string Receive => "接收";

        /***********************************************************************************
         * 
         *    系统相关的错误信息
         * 
         ************************************************************************************/

        public virtual string SystemInstallOperater => "安装新系统：IP为";
        public virtual string SystemUpdateOperater => "更新新系统：IP为";


        /***********************************************************************************
         * 
         *    套接字相关的信息描述
         * 
         ************************************************************************************/

        public virtual string SocketIOException => "套接字传送数据异常：";
        public virtual string SocketSendException => "同步数据发送异常：";
        public virtual string SocketHeadReceiveException => "指令头接收异常：";
        public virtual string SocketContentReceiveException => "内容数据接收异常：";
        public virtual string SocketContentRemoteReceiveException => "对方内容数据接收异常：";
        public virtual string SocketAcceptCallbackException => "异步接受传入的连接尝试";
        public virtual string SocketReAcceptCallbackException => "重新异步接受传入的连接尝试";
        public virtual string SocketSendAsyncException => "异步数据发送出错:";
        public virtual string SocketEndSendException => "异步数据结束挂起发送出错";
        public virtual string SocketReceiveException => "异步数据发送出错:";
        public virtual string SocketEndReceiveException => "异步数据结束接收指令头出错";
        public virtual string SocketRemoteCloseException => "远程主机强迫关闭了一个现有的连接";


        /***********************************************************************************
         * 
         *    文件相关的信息
         * 
         ************************************************************************************/


        public virtual string FileDownloadSuccess => "文件下载成功";
        public virtual string FileDownloadFailed => "文件下载异常";
        public virtual string FileUploadFailed => "文件上传异常";
        public virtual string FileUploadSuccess => "文件上传成功";
        public virtual string FileDeleteFailed => "文件删除异常";
        public virtual string FileDeleteSuccess => "文件删除成功";
        public virtual string FileReceiveFailed => "确认文件接收异常";
        public virtual string FileNotExist => "文件不存在";
        public virtual string FileSaveFailed => "文件存储失败";
        public virtual string FileLoadFailed => "文件加载失败";
        public virtual string FileSendClientFailed => "文件发送的时候发生了异常";
        public virtual string FileWriteToNetFailed => "文件写入网络异常";
        public virtual string FileReadFromNetFailed => "从网络读取文件异常";
        public virtual string FilePathCreateFailed => "文件夹路径创建失败：";
        public virtual string FileRemoteNotExist => "对方文件不存在，无法接收！";

        /***********************************************************************************
         * 
         *    服务器的引擎相关数据
         * 
         ************************************************************************************/

        public virtual string TokenCheckFailed => "接收验证令牌不一致";
        public virtual string TokenCheckTimeout => "接收验证超时:";
        public virtual string CommandHeadCodeCheckFailed => "命令头校验失败";
        public virtual string CommandLengthCheckFailed => "命令长度检查失败";
        public virtual string NetClientAliasFailed => "客户端的别名接收失败：";
        public virtual string NetEngineStart => "启动引擎";
        public virtual string NetEngineClose => "关闭引擎";
        public virtual string NetClientOnline => "上线";
        public virtual string NetClientOffline => "下线";
        public virtual string NetClientBreak => "异常掉线";
        public virtual string NetClientFull => "服务器承载上限，收到超出的请求连接。";
        public virtual string NetClientLoginFailed => "客户端登录中错误：";
        public virtual string NetHeartCheckFailed => "心跳验证异常：";
        public virtual string NetHeartCheckTimeout => "心跳验证超时，强制下线：";
        public virtual string DataSourseFormatError => "数据源格式不正确";
        public virtual string ServerFileCheckFailed => "服务器确认文件失败，请重新上传";
        public virtual string ClientOnlineInfo => "客户端 [ {0} ] 上线";
        public virtual string ClientOfflineInfo => "客户端 [ {0} ] 下线";
        public virtual string ClientDisableLogin => "客户端 [ {0} ] 不被信任，禁止登录";

        /***********************************************************************************
         * 
         *    Client 相关
         * 
         ************************************************************************************/

        public virtual string ReConnectServerSuccess => "重连服务器成功";
        public virtual string ReConnectServerAfterTenSeconds => "在10秒后重新连接服务器";
        public virtual string KeyIsNotAllowedNull => "关键字不允许为空";
        public virtual string KeyIsExistAlready => "当前的关键字已经存在";
        public virtual string KeyIsNotExist => "当前订阅的关键字不存在";
        public virtual string ConnectingServer => "正在连接服务器...";
        public virtual string ConnectFailedAndWait => "连接断开，等待{0}秒后重新连接";
        public virtual string AttemptConnectServer => "正在尝试第{0}次连接服务器";
        public virtual string ConnectServerSuccess => "连接服务器成功";
        public virtual string GetClientIpaddressFailed => "客户端IP地址获取失败";
        public virtual string ConnectionIsNotAvailable => "当前的连接不可用";
        public virtual string DeviceCurrentIsLoginRepeat => "当前设备的id重复登录";
        public virtual string DeviceCurrentIsLoginForbidden => "当前设备的id禁止登录";
        public virtual string PasswordCheckFailed => "密码验证失败";
        public virtual string DataTransformError => "数据转换失败，源数据：";
        public virtual string RemoteClosedConnection => "远程关闭了连接";

        /***********************************************************************************
         * 
         *    日志 相关
         * 
         ************************************************************************************/
        public virtual string LogNetDebug => "调试";
        public virtual string LogNetInfo => "信息";
        public virtual string LogNetWarn => "警告";
        public virtual string LogNetError => "错误";
        public virtual string LogNetFatal => "致命";
        public virtual string LogNetAbandon => "放弃";
        public virtual string LogNetAll => "全部";
        
        /***********************************************************************************
         * 
         *    Modbus相关
         * 
         ************************************************************************************/

        public virtual string ModbusTcpFunctionCodeNotSupport => "不支持的功能码";
        public virtual string ModbusTcpFunctionCodeOverBound => "读取的数据越界";
        public virtual string ModbusTcpFunctionCodeQuantityOver => "读取长度超过最大值";
        public virtual string ModbusTcpFunctionCodeReadWriteException => "读写异常";
        public virtual string ModbusTcpReadCoilException => "读取线圈异常";
        public virtual string ModbusTcpWriteCoilException => "写入线圈异常";
        public virtual string ModbusTcpReadRegisterException => "读取寄存器异常";
        public virtual string ModbusTcpWriteRegisterException => "写入寄存器异常";
        public virtual string ModbusAddressMustMoreThanOne => "地址值在起始地址为1的情况下，必须大于1";
        public virtual string ModbusAsciiFormatCheckFailed => "Modbus的ascii指令检查失败，不是modbus-ascii报文";
        public virtual string ModbusCRCCheckFailed => "Modbus的CRC校验检查失败";
        public virtual string ModbusLRCCheckFailed => "Modbus的LRC校验检查失败";
        public virtual string ModbusMatchFailed => "不是标准的modbus协议";


        /***********************************************************************************
         * 
         *    Melsec PLC 相关
         * 
         ************************************************************************************/
        public virtual string MelsecPleaseReferToManulDocument => "请查看三菱的通讯手册来查看报警的具体信息";
        public virtual string MelsecReadBitInfo => "读取位变量数组只能针对位软元件，如果读取字软元件，请调用Read方法";
        public virtual string MelsecCurrentTypeNotSupportedWordOperate => "当前的类型不支持字读写";
        public virtual string MelsecCurrentTypeNotSupportedBitOperate => "当前的类型不支持位读写";
        public virtual string MelsecFxReceiveZore => "接收的数据长度为0";
        public virtual string MelsecFxAckNagative => "PLC反馈的数据无效";
        public virtual string MelsecFxAckWrong => "PLC反馈信号错误：";
        public virtual string MelsecFxCrcCheckFailed => "PLC反馈报文的和校验失败！";

        /***********************************************************************************
         * 
         *    Siemens PLC 相关
         * 
         ************************************************************************************/

        public virtual string SiemensDBAddressNotAllowedLargerThan255 => "DB块数据无法大于255";
        public virtual string SiemensReadLengthMustBeEvenNumber => "读取的数据长度必须为偶数";
        public virtual string SiemensWriteError => "写入数据异常，代号为：";
        public virtual string SiemensReadLengthCannotLargerThan19 => "读取的数组数量不允许大于19";
        public virtual string SiemensDataLengthCheckFailed => "数据块长度校验失败，请检查是否开启put/get以及关闭db块优化";
        public virtual string SiemensFWError => "发生了异常，具体信息查找Fetch/Write协议文档";

        /***********************************************************************************
         * 
         *    Omron PLC 相关
         * 
         ************************************************************************************/

        public virtual string OmronAddressMustBeZeroToFiveteen => "输入的位地址只能在0-15之间";
        public virtual string OmronReceiveDataError => "数据接收异常";
        public virtual string OmronStatus0 => "通讯正常";
        public virtual string OmronStatus1 => "消息头不是FINS";
        public virtual string OmronStatus2 => "数据长度太长";
        public virtual string OmronStatus3 => "该命令不支持";
        public virtual string OmronStatus20 => "超过连接上限";
        public virtual string OmronStatus21 => "指定的节点已经处于连接中";
        public virtual string OmronStatus22 => "尝试去连接一个受保护的网络节点，该节点还未配置到PLC中";
        public virtual string OmronStatus23 => "当前客户端的网络节点超过正常范围";
        public virtual string OmronStatus24 => "当前客户端的网络节点已经被使用";
        public virtual string OmronStatus25 => "所有的网络节点已经被使用";



        /***********************************************************************************
         * 
         *    AB PLC 相关
         * 
         ************************************************************************************/


        public virtual string AllenBradley04 => "它没有正确生成或匹配标记不存在。";
        public virtual string AllenBradley05 => "引用的特定项（通常是实例）无法找到。";
        public virtual string AllenBradley06 => "请求的数据量不适合响应缓冲区。 发生了部分数据传输。";
        public virtual string AllenBradley0A => "尝试处理其中一个属性时发生错误。";
        public virtual string AllenBradley13 => "命令中没有提供足够的命令数据/参数来执行所请求的服务。";
        public virtual string AllenBradley1C => "与属性计数相比，提供的属性数量不足。";
        public virtual string AllenBradley1E => "此服务中的服务请求出错。";
        public virtual string AllenBradley26 => "IOI字长与处理的IOI数量不匹配。";

        public virtual string AllenBradleySessionStatus00 => "成功";
        public virtual string AllenBradleySessionStatus01 => "发件人发出无效或不受支持的封装命令。";
        public virtual string AllenBradleySessionStatus02 => "接收器中的内存资源不足以处理命令。 这不是一个应用程序错误。 相反，只有在封装层无法获得所需内存资源的情况下才会导致此问题。";
        public virtual string AllenBradleySessionStatus03 => "封装消息的数据部分中的数据形成不良或不正确。";
        public virtual string AllenBradleySessionStatus64 => "向目标发送封装消息时，始发者使用了无效的会话句柄。";
        public virtual string AllenBradleySessionStatus65 => "目标收到一个无效长度的信息。";
        public virtual string AllenBradleySessionStatus69 => "不支持的封装协议修订。";

        /***********************************************************************************
         * 
         *    Panasonic PLC 相关
         * 
         ************************************************************************************/
        public virtual string PanasonicReceiveLengthMustLargerThan9 => "接收数据长度必须大于9";
        public virtual string PanasonicAddressParameterCannotBeNull => "地址参数不允许为空";
        public virtual string PanasonicMewStatus20 => "错误未知";
        public virtual string PanasonicMewStatus21 => "NACK错误，远程单元无法被正确识别，或者发生了数据错误。";
        public virtual string PanasonicMewStatus22 => "WACK 错误:用于远程单元的接收缓冲区已满。";
        public virtual string PanasonicMewStatus23 => "多重端口错误:远程单元编号(01 至 16)设置与本地单元重复。";
        public virtual string PanasonicMewStatus24 => "传输格式错误:试图发送不符合传输格式的数据，或者某一帧数据溢出或发生了数据错误。";
        public virtual string PanasonicMewStatus25 => "硬件错误:传输系统硬件停止操作。";
        public virtual string PanasonicMewStatus26 => "单元号错误:远程单元的编号设置超出 01 至 63 的范围。";
        public virtual string PanasonicMewStatus27 => "不支持错误:接收方数据帧溢出. 试图在不同的模块之间发送不同帧长度的数据。";
        public virtual string PanasonicMewStatus28 => "无应答错误:远程单元不存在. (超时)。";
        public virtual string PanasonicMewStatus29 => "缓冲区关闭错误:试图发送或接收处于关闭状态的缓冲区。";
        public virtual string PanasonicMewStatus30 => "超时错误:持续处于传输禁止状态。";
        public virtual string PanasonicMewStatus40 => "BCC 错误:在指令数据中发生传输错误。";
        public virtual string PanasonicMewStatus41 => "格式错误:所发送的指令信息不符合传输格式。";
        public virtual string PanasonicMewStatus42 => "不支持错误:发送了一个未被支持的指令。向未被支持的目标站发送了指令。";
        public virtual string PanasonicMewStatus43 => "处理步骤错误:在处于传输请求信息挂起时,发送了其他指令。";
        public virtual string PanasonicMewStatus50 => "链接设置错误:设置了实际不存在的链接编号。";
        public virtual string PanasonicMewStatus51 => "同时操作错误:当向其他单元发出指令时,本地单元的传输缓冲区已满。";
        public virtual string PanasonicMewStatus52 => "传输禁止错误:无法向其他单元传输。";
        public virtual string PanasonicMewStatus53 => "忙错误:在接收到指令时,正在处理其他指令。";
        public virtual string PanasonicMewStatus60 => "参数错误:在指令中包含有无法使用的代码,或者代码没有附带区域指定参数(X, Y, D), 等以外。";
        public virtual string PanasonicMewStatus61 => "数据错误:触点编号,区域编号,数据代码格式(BCD,hex,等)上溢出, 下溢出以及区域指定错误。";
        public virtual string PanasonicMewStatus62 => "寄存器错误:过多记录数据在未记录状态下的操作（监控记录、跟踪记录等。)。";
        public virtual string PanasonicMewStatus63 => "PLC 模式错误:当一条指令发出时，运行模式不能够对指令进行处理。";
        public virtual string PanasonicMewStatus65 => "保护错误:在存储保护状态下执行写操作到程序区域或系统寄存器。";
        public virtual string PanasonicMewStatus66 => "地址错误:地址（程序地址、绝对地址等）数据编码形式（BCD、hex 等）、上溢、下溢或指定范围错误。";
        public virtual string PanasonicMewStatus67 => "丢失数据错误:要读的数据不存在。（读取没有写入注释寄存区的数据。。";


#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
    }
}
