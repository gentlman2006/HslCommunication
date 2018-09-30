using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Language
{
    /// <summary>
    /// English Version Text
    /// </summary>
    public class English : DefaultLanguage
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        /***********************************************************************************
         * 
         *    Normal Info
         * 
         ************************************************************************************/

        public override string ConnectedFailed => "Connected Failed: ";
        public override string UnknownError => "Unknown Error";
        public override string ErrorCode => "Error Code: ";
        public override string TextDescription => "Description: ";
        public override string ExceptionMessage => "Exception Info: ";
        public override string ExceptionSourse => "Exception Sourse：";
        public override string ExceptionType => "Exception Type：";
        public override string ExceptionStackTrace => "Exception Stack: ";
        public override string ExceptopnTargetSite => "Exception Method: ";
        public override string ExceprionCustomer => "Error in user-defined method: ";
        public override string SuccessText => "Success";
        public override string TwoParametersLengthIsNotSame => "Two Parameter Length is not same";
        public override string NotSupportedDataType => "Unsupported DataType, input again";
        public override string DataLengthIsNotEnough => "Receive length is not enough，Should:{0},Actual:{1}";
        public override string ReceiveDataTimeout => "Receive timeout: ";
        public override string ReceiveDataLengthTooShort => "Receive length is too short: ";
        public override string MessageTip => "Message prompt:";
        public override string Close => "Close";
        public override string Time => "Time:";
        public override string SoftWare => "Software:";
        public override string BugSubmit => "Bug submit";
        public override string MailServerCenter => "Mail Center System";
        public override string MailSendTail => "Mail Service system issued automatically, do not reply";
        public override string IpAddresError => "IP address input exception, format is incorrect";
        public override string Send => "Send";
        public override string Receive => "Receive";

        /***********************************************************************************
         * 
         *    System about
         * 
         ************************************************************************************/

        public override string SystemInstallOperater => "Install new software: ip address is";
        public override string SystemUpdateOperater => "Update software: ip address is";


        /***********************************************************************************
         * 
         *    Socket-related Information description
         * 
         ************************************************************************************/

        public override string SocketIOException => "Socket transport error: ";
        public override string SocketSendException => "Synchronous Data Send exception: ";
        public override string SocketHeadReceiveException => "Command header receive exception: ";
        public override string SocketContentReceiveException => "Content Data Receive exception: ";
        public override string SocketContentRemoteReceiveException => "Recipient content Data Receive exception: ";
        public override string SocketAcceptCallbackException => "Asynchronously accepts an incoming connection attempt: ";
        public override string SocketReAcceptCallbackException => "To re-accept incoming connection attempts asynchronously";
        public override string SocketSendAsyncException => "Asynchronous Data send Error: ";
        public override string SocketEndSendException => "Asynchronous data end callback send Error";
        public override string SocketReceiveException => "Asynchronous Data send Error: ";
        public override string SocketEndReceiveException => "Asynchronous data end receive instruction header error";
        public override string SocketRemoteCloseException => "An existing connection was forcibly closed by the remote host";


        /***********************************************************************************
         * 
         *    File related information
         * 
         ************************************************************************************/


        public override string FileDownloadSuccess => "File Download Successful";
        public override string FileDownloadFailed => "File Download exception";
        public override string FileUploadFailed => "File Upload exception";
        public override string FileUploadSuccess => "File Upload Successful";
        public override string FileDeleteFailed => "File Delete exception";
        public override string FileDeleteSuccess => "File deletion succeeded";
        public override string FileReceiveFailed => "Confirm File Receive exception";
        public override string FileNotExist => "File does not exist";
        public override string FileSaveFailed => "File Store failed";
        public override string FileLoadFailed => "File load failed";
        public override string FileSendClientFailed => "An exception occurred when the file was sent";
        public override string FileWriteToNetFailed => "File Write Network exception";
        public override string FileReadFromNetFailed => "Read file exceptions from the network";
        public override string FilePathCreateFailed => "Folder path creation failed: ";
        public override string FileRemoteNotExist => "The other file does not exist, cannot receive!";

        /***********************************************************************************
         * 
         *    Engine-related data for the server
         * 
         ************************************************************************************/

        public override string TokenCheckFailed => "Receive authentication token inconsistency";
        public override string TokenCheckTimeout => "Receive authentication timeout: ";
        public override string CommandHeadCodeCheckFailed => "Command header check failed";
        public override string CommandLengthCheckFailed => "Command length check failed";
        public override string NetClientAliasFailed => "Client's alias receive failed: ";
        public override string NetEngineStart => "Start engine";
        public override string NetEngineClose => "Shutting down the engine";
        public override string NetClientOnline => "Online";
        public override string NetClientOffline => "Offline";
        public override string NetClientBreak => "Abnormal offline";
        public override string NetClientFull => "The server hosts the upper limit and receives an exceeded request connection.";
        public override string NetClientLoginFailed => "Error in Client logon: ";
        public override string NetHeartCheckFailed => "Heartbeat Validation exception: ";
        public override string NetHeartCheckTimeout => "Heartbeat verification timeout, force offline: ";
        public override string DataSourseFormatError => "Data source format is incorrect";
        public override string ServerFileCheckFailed => "Server confirmed file failed, please re-upload";
        public override string ClientOnlineInfo => "Client [ {0} ] Online";
        public override string ClientOfflineInfo => "Client [ {0} ] Offline";
        public override string ClientDisableLogin => "Client [ {0} ] is not trusted, login forbidden";

        /***********************************************************************************
         * 
         *    Client related
         * 
         ************************************************************************************/

        public override string ReConnectServerSuccess => "Re-connect server succeeded";
        public override string ReConnectServerAfterTenSeconds => "Reconnect the server after 10 seconds";
        public override string KeyIsNotAllowedNull => "The keyword is not allowed to be empty";
        public override string KeyIsExistAlready => "The current keyword already exists";
        public override string KeyIsNotExist => "The keyword for the current subscription does not exist";
        public override string ConnectingServer => "Connecting to Server...";
        public override string ConnectFailedAndWait => "Connection disconnected, wait {0} seconds to reconnect";
        public override string AttemptConnectServer => "Attempting to connect server {0} times";
        public override string ConnectServerSuccess => "Connection Server succeeded";
        public override string GetClientIpaddressFailed => "Client IP Address acquisition failed";
        public override string ConnectionIsNotAvailable => "The current connection is not available";
        public override string DeviceCurrentIsLoginRepeat => "ID of the current device duplicate login";
        public override string DeviceCurrentIsLoginForbidden => "The ID of the current device prohibits login";
        public override string PasswordCheckFailed => "Password validation failed";
        public override string DataTransformError => "Data conversion failed, source data: ";
        public override string RemoteClosedConnection => "Remote shutdown of connection";
        
        /***********************************************************************************
         * 
         *    Log related
         * 
         ************************************************************************************/
        public override string LogNetDebug => "Debug";
        public override string LogNetInfo => "Info";
        public override string LogNetWarn => "Warn";
        public override string LogNetError => "Error";
        public override string LogNetFatal => "Fatal";
        public override string LogNetAbandon => "Abandon";
        public override string LogNetAll => "All";


        /***********************************************************************************
         * 
         *    Modbus related
         * 
         ************************************************************************************/

        public override string ModbusTcpFunctionCodeNotSupport => "Unsupported function code";
        public override string ModbusTcpFunctionCodeOverBound => "Data read out of bounds";
        public override string ModbusTcpFunctionCodeQuantityOver => "Read length exceeds maximum value";
        public override string ModbusTcpFunctionCodeReadWriteException => "Read and Write exceptions";
        public override string ModbusTcpReadCoilException => "Read Coil anomalies";
        public override string ModbusTcpWriteCoilException => "Write Coil exception";
        public override string ModbusTcpReadRegisterException => "Read Register exception";
        public override string ModbusTcpWriteRegisterException => "Write Register exception";
        public override string ModbusAddressMustMoreThanOne => "The address value must be greater than 1 in the case where the start address is 1";
        public override string ModbusAsciiFormatCheckFailed => "Modbus ASCII command check failed, not MODBUS-ASCII message";
        public override string ModbusCRCCheckFailed => "The CRC checksum check failed for Modbus";
        public override string ModbusLRCCheckFailed => "The LRC checksum check failed for Modbus";
        public override string ModbusMatchFailed => "Not the standard Modbus protocol";


        /***********************************************************************************
         * 
         *    Melsec PLC related
         * 
         ************************************************************************************/
        public override string MelsecPleaseReferToManulDocument => "Please check Mitsubishi's communication manual for details of the alarm.";
        public override string MelsecReadBitInfo => "The read bit variable array can only be used for bit soft elements, if you read the word soft component, call the Read method";
        public override string MelsecCurrentTypeNotSupportedWordOperate => "The current type does not support word read and write";
        public override string MelsecCurrentTypeNotSupportedBitOperate => "The current type does not support bit read and write";
        public override string MelsecFxReceiveZore => "The received data length is 0";
        public override string MelsecFxAckNagative => "Invalid data from PLC feedback";
        public override string MelsecFxAckWrong => "PLC Feedback Signal Error: ";
        public override string MelsecFxCrcCheckFailed => "PLC Feedback message and check failed!";

        /***********************************************************************************
         * 
         *    Siemens PLC related
         * 
         ************************************************************************************/

        public override string SiemensDBAddressNotAllowedLargerThan255 => "DB block data cannot be greater than 255";
        public override string SiemensReadLengthMustBeEvenNumber => "The length of the data read must be an even number";
        public override string SiemensWriteError => "Writes the data exception, the code name is: ";
        public override string SiemensReadLengthCannotLargerThan19 => "The number of arrays read does not allow greater than 19";
        public override string SiemensDataLengthCheckFailed => "Block length checksum failed, please check if Put/get is turned on and DB block optimization is turned off";
        public override string SiemensFWError => "An exception occurred, the specific information to find the Fetch/write protocol document";

        /***********************************************************************************
         * 
         *    Omron PLC related
         * 
         ************************************************************************************/

        public override string OmronAddressMustBeZeroToFiveteen => "The bit address entered can only be between 0-15";
        public override string OmronReceiveDataError => "Data Receive exception";
        public override string OmronStatus0 => "Communication is normal.";
        public override string OmronStatus1 => "The message header is not fins";
        public override string OmronStatus2 => "Data length too long";
        public override string OmronStatus3 => "This command does not support";
        public override string OmronStatus20 => "Exceeding connection limit";
        public override string OmronStatus21 => "The specified node is already in the connection";
        public override string OmronStatus22 => "Attempt to connect to a protected network node that is not yet configured in the PLC";
        public override string OmronStatus23 => "The current client's network node exceeds the normal range";
        public override string OmronStatus24 => "The current client's network node is already in use";
        public override string OmronStatus25 => "All network nodes are already in use";



        /***********************************************************************************
         * 
         *    AB PLC 相关
         * 
         ************************************************************************************/


        public override string AllenBradley04 => "The IOI could not be deciphered. Either it was not formed correctly or the match tag does not exist."; 
        public override string AllenBradley05 => "The particular item referenced (usually instance) could not be found.";
        public override string AllenBradley06 => "The amount of data requested would not fit into the response buffer. Partial data transfer has occurred.";
        public override string AllenBradley0A => "An error has occurred trying to process one of the attributes.";
        public override string AllenBradley13 => "Not enough command data / parameters were supplied in the command to execute the service requested.";
        public override string AllenBradley1C => "An insufficient number of attributes were provided compared to the attribute count.";
        public override string AllenBradley1E => "A service request in this service went wrong.";
        public override string AllenBradley26 => "The IOI word length did not match the amount of IOI which was processed.";

        public override string AllenBradleySessionStatus00 => "success";
        public override string AllenBradleySessionStatus01 => "The sender issued an invalid or unsupported encapsulation command.";
        public override string AllenBradleySessionStatus02 => "Insufficient memory resources in the receiver to handle the command. This is not an application error. Instead, it only results if the encapsulation layer cannot obtain memory resources that it need.";
        public override string AllenBradleySessionStatus03 => "Poorly formed or incorrect data in the data portion of the encapsulation message.";
        public override string AllenBradleySessionStatus64 => "An originator used an invalid session handle when sending an encapsulation message.";
        public override string AllenBradleySessionStatus65 => "The target received a message of invalid length.";
        public override string AllenBradleySessionStatus69 => "Unsupported encapsulation protocol revision.";

        /***********************************************************************************
         * 
         *    Panasonic PLC 相关
         * 
         ************************************************************************************/
        public override string PanasonicReceiveLengthMustLargerThan9 => "The received data length must be greater than 9";
        public override string PanasonicAddressParameterCannotBeNull => "Address parameter is not allowed to be empty";
        public override string PanasonicMewStatus20 => "Error unknown";
        public override string PanasonicMewStatus21 => "Nack error, the remote unit could not be correctly identified, or a data error occurred.";
        public override string PanasonicMewStatus22 => "WACK Error: The receive buffer for the remote unit is full.";
        public override string PanasonicMewStatus23 => "Multiple port error: The remote unit number (01 to 16) is set to repeat with the local unit.";
        public override string PanasonicMewStatus24 => "Transport format error: An attempt was made to send data that does not conform to the transport format, or a frame data overflow or a data error occurred.";
        public override string PanasonicMewStatus25 => "Hardware error: Transport system hardware stopped operation.";
        public override string PanasonicMewStatus26 => "Unit Number error: The remote unit's numbering setting exceeds the range of 01 to 63.";
        public override string PanasonicMewStatus27 => "Error not supported: Receiver data frame overflow. An attempt was made to send data of different frame lengths between different modules.";
        public override string PanasonicMewStatus28 => "No answer error: The remote unit does not exist. (timeout).";
        public override string PanasonicMewStatus29 => "Buffer Close error: An attempt was made to send or receive a buffer that is in a closed state.";
        public override string PanasonicMewStatus30 => "Timeout error: Persisted in transport forbidden State.";
        public override string PanasonicMewStatus40 => "BCC Error: A transmission error occurred in the instruction data.";
        public override string PanasonicMewStatus41 => "Malformed: The sent instruction information does not conform to the transmission format.";
        public override string PanasonicMewStatus42 => "Error not supported: An unsupported instruction was sent. An instruction was sent to a target station that was not supported.";
        public override string PanasonicMewStatus43 => "Processing Step Error: Additional instructions were sent when the transfer request information was suspended.";
        public override string PanasonicMewStatus50 => "Link Settings Error: A link number that does not actually exist is set.";
        public override string PanasonicMewStatus51 => "Simultaneous operation error: When issuing instructions to other units, the transmit buffer for the local unit is full.";
        public override string PanasonicMewStatus52 => "Transport suppression Error: Unable to transfer to other units.";
        public override string PanasonicMewStatus53 => "Busy error: Other instructions are being processed when the command is received.";
        public override string PanasonicMewStatus60 => "Parameter error: Contains code that cannot be used in the directive, or the code does not have a zone specified parameter (X, Y, D), and so on.";
        public override string PanasonicMewStatus61 => "Data error: Contact number, area number, Data code format (BCD,HEX, etc.) overflow, overflow, and area specified error.";
        public override string PanasonicMewStatus62 => "Register ERROR: Excessive logging of data in an unregistered state of operations (Monitoring records, tracking records, etc.). )。";
        public override string PanasonicMewStatus63 => "PLC mode error: When an instruction is issued, the run mode is not able to process the instruction.";
        public override string PanasonicMewStatus65 => "Protection Error: Performs a write operation to the program area or system register in the storage protection state.";
        public override string PanasonicMewStatus66 => "Address Error: Address (program address, absolute address, etc.) Data encoding form (BCD, hex, etc.), overflow, underflow, or specified range error.";
        public override string PanasonicMewStatus67 => "Missing data error: The data to be read does not exist. (reads data that is not written to the comment register.)";

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

    }
}
