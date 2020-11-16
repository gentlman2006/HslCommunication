package HslCommunication.Language;

/**
 * 系统的语言基类，默认也即是中文版本
 */
public class DefaultLanguage
{
    /***********************************************************************************
     *
     *    一般的错误信息
     *
     ************************************************************************************/

    public String ConnectedFailed (){ return "连接失败："; }
    public String UnknownError (){ return "未知错误"; }
    public String ErrorCode (){ return "错误代号"; }
    public String TextDescription (){ return "文本描述"; }
    public String ExceptionMessage (){ return "错误信息："; }
    public String ExceptionSourse (){ return "错误源："; }
    public String ExceptionType (){ return "错误类型："; }
    public String ExceptionStackTrace (){ return "错误堆栈："; }
    public String ExceptopnTargetSite (){ return "错误方法："; }
    public String ExceprionCustomer (){ return "用户自定义方法出错："; }
    public String SuccessText (){ return "成功"; }
    public String TwoParametersLengthIsNotSame (){ return "两个参数的个数不一致"; }
    public String NotSupportedDataType (){ return "输入的类型不支持，请重新输入"; }
    public String DataLengthIsNotEnough (){ return "接收的数据长度不足，应该值:{0},实际值:{1}"; }
    public String ReceiveDataTimeout (){ return "接收数据超时："; }
    public String ReceiveDataLengthTooShort (){ return "接收的数据长度太短："; }
    public String MessageTip (){ return "消息提示："; }
    public String Close (){ return "关闭"; }
    public String Time (){ return "时间："; }
    public String SoftWare (){ return "软件："; }
    public String BugSubmit (){ return "Bug提交"; }
    public String MailServerCenter (){ return "邮件发送系统"; }
    public String MailSendTail (){ return "邮件服务系统自动发出，请勿回复！"; }
    public String IpAddresError (){ return "Ip地址输入异常，格式不正确"; }
    public String Send (){ return "发送";}
    public String Receive(){ return "接收";}

    /***********************************************************************************
     *
     *    系统相关的错误信息
     *
     ************************************************************************************/

    public String SystemInstallOperater (){ return "安装新系统：IP为"; }
    public String SystemUpdateOperater (){ return "更新新系统：IP为"; }


    /***********************************************************************************
     *
     *    套接字相关的信息描述
     *
     ************************************************************************************/

    public String SocketIOException (){ return "套接字传送数据异常："; }
    public String SocketSendException (){ return "同步数据发送异常："; }
    public String SocketHeadReceiveException (){ return "指令头接收异常："; }
    public String SocketContentReceiveException (){ return "内容数据接收异常："; }
    public String SocketContentRemoteReceiveException (){ return "对方内容数据接收异常："; }
    public String SocketAcceptCallbackException (){ return "异步接受传入的连接尝试"; }
    public String SocketReAcceptCallbackException (){ return "重新异步接受传入的连接尝试"; }
    public String SocketSendAsyncException (){ return "异步数据发送出错:"; }
    public String SocketEndSendException (){ return "异步数据结束挂起发送出错"; }
    public String SocketReceiveException (){ return "异步数据发送出错:"; }
    public String SocketEndReceiveException (){ return "异步数据结束接收指令头出错"; }
    public String SocketRemoteCloseException (){ return "远程主机强迫关闭了一个现有的连接"; }


    /***********************************************************************************
     *
     *    文件相关的信息
     *
     ************************************************************************************/


    public String FileDownloadSuccess (){ return "文件下载成功"; }
    public String FileDownloadFailed (){ return "文件下载异常"; }
    public String FileUploadFailed (){ return "文件上传异常"; }
    public String FileUploadSuccess (){ return "文件上传成功"; }
    public String FileDeleteFailed (){ return "文件删除异常"; }
    public String FileDeleteSuccess (){ return "文件删除成功"; }
    public String FileReceiveFailed (){ return "确认文件接收异常"; }
    public String FileNotExist (){ return "文件不存在"; }
    public String FileSaveFailed (){ return "文件存储失败"; }
    public String FileLoadFailed (){ return "文件加载失败"; }
    public String FileSendClientFailed (){ return "文件发送的时候发生了异常"; }
    public String FileWriteToNetFailed (){ return "文件写入网络异常"; }
    public String FileReadFromNetFailed (){ return "从网络读取文件异常"; }
    public String FilePathCreateFailed (){ return "文件夹路径创建失败："; }
    public String FileRemoteNotExist (){ return "对方文件不存在，无法接收！"; }

    /***********************************************************************************
     *
     *    服务器的引擎相关数据
     *
     ************************************************************************************/

    public String TokenCheckFailed (){ return "接收验证令牌不一致"; }
    public String TokenCheckTimeout (){ return "接收验证超时:"; }
    public String CommandHeadCodeCheckFailed (){ return "命令头校验失败"; }
    public String CommandLengthCheckFailed (){ return "命令长度检查失败"; }
    public String NetClientAliasFailed (){ return "客户端的别名接收失败："; }
    public String NetEngineStart (){ return "启动引擎"; }
    public String NetEngineClose (){ return "关闭引擎"; }
    public String NetClientOnline (){ return "上线"; }
    public String NetClientOffline (){ return "下线"; }
    public String NetClientBreak (){ return "异常掉线"; }
    public String NetClientFull (){ return "服务器承载上限，收到超出的请求连接。"; }
    public String NetClientLoginFailed (){ return "客户端登录中错误："; }
    public String NetHeartCheckFailed (){ return "心跳验证异常："; }
    public String NetHeartCheckTimeout (){ return "心跳验证超时，强制下线："; }
    public String DataSourseFormatError (){ return "数据源格式不正确"; }
    public String ServerFileCheckFailed (){ return "服务器确认文件失败，请重新上传"; }
    public String ClientOnlineInfo (){ return "客户端 [ {0} ] 上线"; }
    public String ClientOfflineInfo (){ return "客户端 [ {0} ] 下线"; }
    public String ClientDisableLogin (){ return "客户端 [ {0} ] 不被信任，禁止登录"; }

    /***********************************************************************************
     *
     *    Client 相关
     *
     ************************************************************************************/

    public String ReConnectServerSuccess (){ return "重连服务器成功"; }
    public String ReConnectServerAfterTenSeconds (){ return "在10秒后重新连接服务器"; }
    public String KeyIsNotAllowedNull (){ return "关键字不允许为空"; }
    public String KeyIsExistAlready (){ return "当前的关键字已经存在"; }
    public String KeyIsNotExist (){ return "当前订阅的关键字不存在"; }
    public String ConnectingServer (){ return "正在连接服务器..."; }
    public String ConnectFailedAndWait (){ return "连接断开，等待{0}秒后重新连接"; }
    public String AttemptConnectServer (){ return "正在尝试第{0}次连接服务器"; }
    public String ConnectServerSuccess (){ return "连接服务器成功"; }
    public String GetClientIpaddressFailed (){ return "客户端IP地址获取失败"; }
    public String ConnectionIsNotAvailable (){ return "当前的连接不可用"; }
    public String DeviceCurrentIsLoginRepeat (){ return "当前设备的id重复登录"; }
    public String DeviceCurrentIsLoginForbidden (){ return "当前设备的id禁止登录"; }
    public String PasswordCheckFailed (){ return "密码验证失败"; }
    public String DataTransformError (){ return "数据转换失败，源数据："; }
    public String RemoteClosedConnection (){ return "远程关闭了连接"; }

    /***********************************************************************************
     *
     *    日志 相关
     *
     ************************************************************************************/
    public String LogNetDebug (){ return "调试"; }
    public String LogNetInfo (){ return "信息"; }
    public String LogNetWarn (){ return "警告"; }
    public String LogNetError (){ return "错误"; }
    public String LogNetFatal (){ return "致命"; }
    public String LogNetAbandon (){ return "放弃"; }
    public String LogNetAll (){ return "全部"; }

    /***********************************************************************************
     *
     *    Modbus相关
     *
     ************************************************************************************/

    public String ModbusTcpFunctionCodeNotSupport (){ return "不支持的功能码"; }
    public String ModbusTcpFunctionCodeOverBound (){ return "读取的数据越界"; }
    public String ModbusTcpFunctionCodeQuantityOver (){ return "读取长度超过最大值"; }
    public String ModbusTcpFunctionCodeReadWriteException (){ return "读写异常"; }
    public String ModbusTcpReadCoilException (){ return "读取线圈异常"; }
    public String ModbusTcpWriteCoilException (){ return "写入线圈异常"; }
    public String ModbusTcpReadRegisterException (){ return "读取寄存器异常"; }
    public String ModbusTcpWriteRegisterException (){ return "写入寄存器异常"; }
    public String ModbusAddressMustMoreThanOne (){ return "地址值在起始地址为1的情况下，必须大于1"; }
    public String ModbusAsciiFormatCheckFailed (){ return "Modbus的ascii指令检查失败，不是modbus-ascii报文"; }
    public String ModbusCRCCheckFailed (){ return "Modbus的CRC校验检查失败"; }
    public String ModbusLRCCheckFailed (){ return "Modbus的LRC校验检查失败"; }
    public String ModbusMatchFailed (){ return "不是标准的modbus协议"; }


    /***********************************************************************************
     *
     *    Melsec PLC 相关
     *
     ************************************************************************************/
    public String MelsecPleaseReferToManulDocument (){ return "请查看三菱的通讯手册来查看报警的具体信息"; }
    public String MelsecReadBitInfo (){ return "读取位变量数组只能针对位软元件，如果读取字软元件，请调用Read方法"; }
    public String MelsecCurrentTypeNotSupportedWordOperate (){ return "当前的类型不支持字读写"; }
    public String MelsecCurrentTypeNotSupportedBitOperate (){ return "当前的类型不支持位读写"; }
    public String MelsecFxReceiveZore (){ return "接收的数据长度为0"; }
    public String MelsecFxAckNagative (){ return "PLC反馈的数据无效"; }
    public String MelsecFxAckWrong (){ return "PLC反馈信号错误："; }
    public String MelsecFxCrcCheckFailed (){ return "PLC反馈报文的和校验失败！"; }

    /***********************************************************************************
     *
     *    Siemens PLC 相关
     *
     ************************************************************************************/

    public String SiemensDBAddressNotAllowedLargerThan255 (){ return "DB块数据无法大于255"; }
    public String SiemensReadLengthMustBeEvenNumber (){ return "读取的数据长度必须为偶数"; }
    public String SiemensWriteError (){ return "写入数据异常，代号为："; }
    public String SiemensReadLengthCannotLargerThan19 (){ return "读取的数组数量不允许大于19"; }
    public String SiemensDataLengthCheckFailed (){ return "数据块长度校验失败，请检查是否开启put/get以及关闭db块优化"; }
    public String SiemensFWError (){ return "发生了异常，具体信息查找Fetch/Write协议文档"; }

    /***********************************************************************************
     *
     *    Omron PLC 相关
     *
     ************************************************************************************/

    public String OmronAddressMustBeZeroToFiveteen (){ return "输入的位地址只能在0-15之间"; }
    public String OmronReceiveDataError (){ return "数据接收异常"; }
    public String OmronStatus0 (){ return "通讯正常"; }
    public String OmronStatus1 (){ return "消息头不是FINS"; }
    public String OmronStatus2 (){ return "数据长度太长"; }
    public String OmronStatus3 (){ return "该命令不支持"; }
    public String OmronStatus20 (){ return "超过连接上限"; }
    public String OmronStatus21 (){ return "指定的节点已经处于连接中"; }
    public String OmronStatus22 (){ return "尝试去连接一个受保护的网络节点，该节点还未配置到PLC中"; }
    public String OmronStatus23 (){ return "当前客户端的网络节点超过正常范围"; }
    public String OmronStatus24 (){ return "当前客户端的网络节点已经被使用"; }
    public String OmronStatus25 (){ return "所有的网络节点已经被使用"; }



    /***********************************************************************************
     *
     *    AB PLC 相关
     *
     ************************************************************************************/


    public String AllenBradley04 (){ return "它没有正确生成或匹配标记不存在。"; }
    public String AllenBradley05 (){ return "引用的特定项（通常是实例）无法找到。"; }
    public String AllenBradley06 (){ return "请求的数据量不适合响应缓冲区。 发生了部分数据传输。"; }
    public String AllenBradley0A (){ return "尝试处理其中一个属性时发生错误。"; }
    public String AllenBradley13 (){ return "命令中没有提供足够的命令数据/参数来执行所请求的服务。"; }
    public String AllenBradley1C (){ return "与属性计数相比，提供的属性数量不足。"; }
    public String AllenBradley1E (){ return "此服务中的服务请求出错。"; }
    public String AllenBradley26 (){ return "IOI字长与处理的IOI数量不匹配。"; }

    public String AllenBradleySessionStatus00 (){ return "成功"; }
    public String AllenBradleySessionStatus01 (){ return "发件人发出无效或不受支持的封装命令。"; }
    public String AllenBradleySessionStatus02 (){ return "接收器中的内存资源不足以处理命令。 这不是一个应用程序错误。 相反，只有在封装层无法获得所需内存资源的情况下才会导致此问题。"; }
    public String AllenBradleySessionStatus03 (){ return "封装消息的数据部分中的数据形成不良或不正确。"; }
    public String AllenBradleySessionStatus64 (){ return "向目标发送封装消息时，始发者使用了无效的会话句柄。"; }
    public String AllenBradleySessionStatus65 (){ return "目标收到一个无效长度的信息。"; }
    public String AllenBradleySessionStatus69 (){ return "不支持的封装协议修订。"; }

    /***********************************************************************************
     *
     *    Panasonic PLC 相关
     *
     ************************************************************************************/
    public String PanasonicReceiveLengthMustLargerThan9 (){ return "接收数据长度必须大于9"; }
    public String PanasonicAddressParameterCannotBeNull (){ return "地址参数不允许为空"; }
    public String PanasonicMewStatus20 (){ return "错误未知"; }
    public String PanasonicMewStatus21 (){ return "NACK错误，远程单元无法被正确识别，或者发生了数据错误。"; }
    public String PanasonicMewStatus22 (){ return "WACK 错误:用于远程单元的接收缓冲区已满。"; }
    public String PanasonicMewStatus23 (){ return "多重端口错误:远程单元编号(01 至 16)设置与本地单元重复。"; }
    public String PanasonicMewStatus24 (){ return "传输格式错误:试图发送不符合传输格式的数据，或者某一帧数据溢出或发生了数据错误。"; }
    public String PanasonicMewStatus25 (){ return "硬件错误:传输系统硬件停止操作。"; }
    public String PanasonicMewStatus26 (){ return "单元号错误:远程单元的编号设置超出 01 至 63 的范围。"; }
    public String PanasonicMewStatus27 (){ return "不支持错误:接收方数据帧溢出. 试图在不同的模块之间发送不同帧长度的数据。"; }
    public String PanasonicMewStatus28 (){ return "无应答错误:远程单元不存在. (超时)。"; }
    public String PanasonicMewStatus29 (){ return "缓冲区关闭错误:试图发送或接收处于关闭状态的缓冲区。"; }
    public String PanasonicMewStatus30 (){ return "超时错误:持续处于传输禁止状态。"; }
    public String PanasonicMewStatus40 (){ return "BCC 错误:在指令数据中发生传输错误。"; }
    public String PanasonicMewStatus41 (){ return "格式错误:所发送的指令信息不符合传输格式。"; }
    public String PanasonicMewStatus42 (){ return "不支持错误:发送了一个未被支持的指令。向未被支持的目标站发送了指令。"; }
    public String PanasonicMewStatus43 (){ return "处理步骤错误:在处于传输请求信息挂起时,发送了其他指令。"; }
    public String PanasonicMewStatus50 (){ return "链接设置错误:设置了实际不存在的链接编号。"; }
    public String PanasonicMewStatus51 (){ return "同时操作错误:当向其他单元发出指令时,本地单元的传输缓冲区已满。"; }
    public String PanasonicMewStatus52 (){ return "传输禁止错误:无法向其他单元传输。"; }
    public String PanasonicMewStatus53 (){ return "忙错误:在接收到指令时,正在处理其他指令。"; }
    public String PanasonicMewStatus60 (){ return "参数错误:在指令中包含有无法使用的代码,或者代码没有附带区域指定参数(X, Y, D), 等以外。"; }
    public String PanasonicMewStatus61 (){ return "数据错误:触点编号,区域编号,数据代码格式(BCD,hex,等)上溢出, 下溢出以及区域指定错误。"; }
    public String PanasonicMewStatus62 (){ return "寄存器错误:过多记录数据在未记录状态下的操作（监控记录、跟踪记录等。)。"; }
    public String PanasonicMewStatus63 (){ return "PLC 模式错误:当一条指令发出时，运行模式不能够对指令进行处理。"; }
    public String PanasonicMewStatus65 (){ return "保护错误:在存储保护状态下执行写操作到程序区域或系统寄存器。"; }
    public String PanasonicMewStatus66 (){ return "地址错误:地址（程序地址、绝对地址等）数据编码形式（BCD、hex 等）、上溢、下溢或指定范围错误。"; }
    public String PanasonicMewStatus67 (){ return "丢失数据错误:要读的数据不存在。（读取没有写入注释寄存区的数据。。"; }
}
