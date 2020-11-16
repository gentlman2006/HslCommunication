package HslCommunication.Language;

/**
 * English Version Text
 */
public class English extends DefaultLanguage {

    /***********************************************************************************
     *
     *    Normal Info
     *
     ************************************************************************************/

    @Override
    public String ConnectedFailed (){ return "Connected Failed: "; }
    @Override
    public String UnknownError (){ return "Unknown Error"; }
    @Override
    public String ErrorCode (){ return "Error Code: "; }
    @Override
    public String TextDescription (){ return "Description: "; }
    @Override
    public String ExceptionMessage (){ return "Exception Info: "; }
    @Override
    public String ExceptionSourse (){ return "Exception Sourse："; }
    @Override
    public String ExceptionType (){ return "Exception Type："; }
    @Override
    public String ExceptionStackTrace (){ return "Exception Stack: "; }
    @Override
    public String ExceptopnTargetSite (){ return "Exception Method: "; }
    @Override
    public String ExceprionCustomer (){ return "Error in user-defined method: "; }
    @Override
    public String SuccessText (){ return "Success"; }
    @Override
    public String TwoParametersLengthIsNotSame (){ return "Two Parameter Length is not same"; }
    @Override
    public String NotSupportedDataType (){ return "Unsupported DataType, input again"; }
    @Override
    public String DataLengthIsNotEnough (){ return "Receive length is not enough，Should:{0},Actual:{1}"; }
    @Override
    public String ReceiveDataTimeout (){ return "Receive timeout: "; }
    @Override
    public String ReceiveDataLengthTooShort (){ return "Receive length is too short: "; }
    @Override
    public String MessageTip (){ return "Message prompt:"; }
    @Override
    public String Close (){ return "Close"; }
    @Override
    public String Time (){ return "Time:"; }
    @Override
    public String SoftWare (){ return "Software:"; }
    @Override
    public String BugSubmit (){ return "Bug submit"; }
    @Override
    public String MailServerCenter (){ return "Mail Center System"; }
    @Override
    public String MailSendTail (){ return "Mail Service system issued automatically, do not reply"; }
    @Override
    public String IpAddresError (){ return "IP address input exception, format is incorrect"; }
    @Override
    public String Send (){ return "Send";}
    @Override
    public String Receive(){ return "Receive";}

    /***********************************************************************************
     *
     *    System about
     *
     ************************************************************************************/

    @Override
    public String SystemInstallOperater (){ return "Install new software: ip address is"; }
    @Override
    public String SystemUpdateOperater (){ return "Update software: ip address is"; }


    /***********************************************************************************
     *
     *    Socket-related Information description
     *
     ************************************************************************************/

    @Override
    public String SocketIOException (){ return "Socket transport error: "; }
    @Override
    public String SocketSendException (){ return "Synchronous Data Send exception: "; }
    @Override
    public String SocketHeadReceiveException (){ return "Command header receive exception: "; }
    @Override
    public String SocketContentReceiveException (){ return "Content Data Receive exception: "; }
    @Override
    public String SocketContentRemoteReceiveException (){ return "Recipient content Data Receive exception: "; }
    @Override
    public String SocketAcceptCallbackException (){ return "Asynchronously accepts an incoming connection attempt: "; }
    @Override
    public String SocketReAcceptCallbackException (){ return "To re-accept incoming connection attempts asynchronously"; }
    @Override
    public String SocketSendAsyncException (){ return "Asynchronous Data send Error: "; }
    @Override
    public String SocketEndSendException (){ return "Asynchronous data end callback send Error"; }
    @Override
    public String SocketReceiveException (){ return "Asynchronous Data send Error: "; }
    @Override
    public String SocketEndReceiveException (){ return "Asynchronous data end receive instruction header error"; }
    @Override
    public String SocketRemoteCloseException (){ return "An existing connection was forcibly closed by the remote host"; }


    /***********************************************************************************
     *
     *    File related information
     *
     ************************************************************************************/


    @Override
    public String FileDownloadSuccess (){ return "File Download Successful"; }
    @Override
    public String FileDownloadFailed (){ return "File Download exception"; }
    @Override
    public String FileUploadFailed (){ return "File Upload exception"; }
    @Override
    public String FileUploadSuccess (){ return "File Upload Successful"; }
    @Override
    public String FileDeleteFailed (){ return "File Delete exception"; }
    @Override
    public String FileDeleteSuccess (){ return "File deletion succeeded"; }
    @Override
    public String FileReceiveFailed (){ return "Confirm File Receive exception"; }
    @Override
    public String FileNotExist (){ return "File does not exist"; }
    @Override
    public String FileSaveFailed (){ return "File Store failed"; }
    @Override
    public String FileLoadFailed (){ return "File load failed"; }
    @Override
    public String FileSendClientFailed (){ return "An exception occurred when the file was sent"; }
    @Override
    public String FileWriteToNetFailed (){ return "File Write Network exception"; }
    @Override
    public String FileReadFromNetFailed (){ return "Read file exceptions from the network"; }
    @Override
    public String FilePathCreateFailed (){ return "Folder path creation failed: "; }
    @Override
    public String FileRemoteNotExist (){ return "The other file does not exist, cannot receive!"; }

    /***********************************************************************************
     *
     *    Engine-related data for the server
     *
     ************************************************************************************/

    @Override
    public String TokenCheckFailed (){ return "Receive authentication token inconsistency"; }
    @Override
    public String TokenCheckTimeout (){ return "Receive authentication timeout: "; }
    @Override
    public String CommandHeadCodeCheckFailed (){ return "Command header check failed"; }
    @Override
    public String CommandLengthCheckFailed (){ return "Command length check failed"; }
    @Override
    public String NetClientAliasFailed (){ return "Client's alias receive failed: "; }
    @Override
    public String NetEngineStart (){ return "Start engine"; }
    @Override
    public String NetEngineClose (){ return "Shutting down the engine"; }
    @Override
    public String NetClientOnline (){ return "Online"; }
    @Override
    public String NetClientOffline (){ return "Offline"; }
    @Override
    public String NetClientBreak (){ return "Abnormal offline"; }
    @Override
    public String NetClientFull (){ return "The server hosts the upper limit and receives an exceeded request connection."; }
    @Override
    public String NetClientLoginFailed (){ return "Error in Client logon: "; }
    @Override
    public String NetHeartCheckFailed (){ return "Heartbeat Validation exception: "; }
    @Override
    public String NetHeartCheckTimeout (){ return "Heartbeat verification timeout, force offline: "; }
    @Override
    public String DataSourseFormatError (){ return "Data source format is incorrect"; }
    @Override
    public String ServerFileCheckFailed (){ return "Server confirmed file failed, please re-upload"; }
    @Override
    public String ClientOnlineInfo (){ return "Client [ {0} ] Online"; }
    @Override
    public String ClientOfflineInfo (){ return "Client [ {0} ] Offline"; }
    @Override
    public String ClientDisableLogin (){ return "Client [ {0} ] is not trusted, login forbidden"; }

    /***********************************************************************************
     *
     *    Client related
     *
     ************************************************************************************/

    @Override
    public String ReConnectServerSuccess (){ return "Re-connect server succeeded"; }
    @Override
    public String ReConnectServerAfterTenSeconds (){ return "Reconnect the server after 10 seconds"; }
    @Override
    public String KeyIsNotAllowedNull (){ return "The keyword is not allowed to be empty"; }
    @Override
    public String KeyIsExistAlready (){ return "The current keyword already exists"; }
    @Override
    public String KeyIsNotExist (){ return "The keyword for the current subscription does not exist"; }
    @Override
    public String ConnectingServer (){ return "Connecting to Server..."; }
    @Override
    public String ConnectFailedAndWait (){ return "Connection disconnected, wait {0} seconds to reconnect"; }
    @Override
    public String AttemptConnectServer (){ return "Attempting to connect server {0} times"; }
    @Override
    public String ConnectServerSuccess (){ return "Connection Server succeeded"; }
    @Override
    public String GetClientIpaddressFailed (){ return "Client IP Address acquisition failed"; }
    @Override
    public String ConnectionIsNotAvailable (){ return "The current connection is not available"; }
    @Override
    public String DeviceCurrentIsLoginRepeat (){ return "ID of the current device duplicate login"; }
    @Override
    public String DeviceCurrentIsLoginForbidden (){ return "The ID of the current device prohibits login"; }
    @Override
    public String PasswordCheckFailed (){ return "Password validation failed"; }
    @Override
    public String DataTransformError (){ return "Data conversion failed, source data: "; }
    @Override
    public String RemoteClosedConnection (){ return "Remote shutdown of connection"; }

    /***********************************************************************************
     *
     *    Log related
     *
     ************************************************************************************/
    @Override
    public String LogNetDebug (){ return "Debug"; }
    @Override
    public String LogNetInfo (){ return "Info"; }
    @Override
    public String LogNetWarn (){ return "Warn"; }
    @Override
    public String LogNetError (){ return "Error"; }
    @Override
    public String LogNetFatal (){ return "Fatal"; }
    @Override
    public String LogNetAbandon (){ return "Abandon"; }
    @Override
    public String LogNetAll (){ return "All"; }


    /***********************************************************************************
     *
     *    Modbus related
     *
     ************************************************************************************/

    @Override
    public String ModbusTcpFunctionCodeNotSupport (){ return "Unsupported function code"; }
    @Override
    public String ModbusTcpFunctionCodeOverBound (){ return "Data read out of bounds"; }
    @Override
    public String ModbusTcpFunctionCodeQuantityOver (){ return "Read length exceeds maximum value"; }
    @Override
    public String ModbusTcpFunctionCodeReadWriteException (){ return "Read and Write exceptions"; }
    @Override
    public String ModbusTcpReadCoilException (){ return "Read Coil anomalies"; }
    @Override
    public String ModbusTcpWriteCoilException (){ return "Write Coil exception"; }
    @Override
    public String ModbusTcpReadRegisterException (){ return "Read Register exception"; }
    @Override
    public String ModbusTcpWriteRegisterException (){ return "Write Register exception"; }
    @Override
    public String ModbusAddressMustMoreThanOne (){ return "The address value must be greater than 1 in the case where the start address is 1"; }
    @Override
    public String ModbusAsciiFormatCheckFailed (){ return "Modbus ASCII command check failed, not MODBUS-ASCII message"; }
    @Override
    public String ModbusCRCCheckFailed (){ return "The CRC checksum check failed for Modbus"; }
    @Override
    public String ModbusLRCCheckFailed (){ return "The LRC checksum check failed for Modbus"; }
    @Override
    public String ModbusMatchFailed (){ return "Not the standard Modbus protocol"; }


    /***********************************************************************************
     *
     *    Melsec PLC related
     *
     ************************************************************************************/
    @Override
    public String MelsecPleaseReferToManulDocument (){ return "Please check Mitsubishi's communication manual for details of the alarm."; }
    @Override
    public String MelsecReadBitInfo (){ return "The read bit variable array can only be used for bit soft elements, if you read the word soft component, call the Read method"; }
    @Override
    public String MelsecCurrentTypeNotSupportedWordOperate (){ return "The current type does not support word read and write"; }
    @Override
    public String MelsecCurrentTypeNotSupportedBitOperate (){ return "The current type does not support bit read and write"; }
    @Override
    public String MelsecFxReceiveZore (){ return "The received data length is 0"; }
    @Override
    public String MelsecFxAckNagative (){ return "Invalid data from PLC feedback"; }
    @Override
    public String MelsecFxAckWrong (){ return "PLC Feedback Signal Error: "; }
    @Override
    public String MelsecFxCrcCheckFailed (){ return "PLC Feedback message and check failed!"; }

    /***********************************************************************************
     *
     *    Siemens PLC related
     *
     ************************************************************************************/

    @Override
    public String SiemensDBAddressNotAllowedLargerThan255 (){ return "DB block data cannot be greater than 255"; }
    @Override
    public String SiemensReadLengthMustBeEvenNumber (){ return "The length of the data read must be an even number"; }
    @Override
    public String SiemensWriteError (){ return "Writes the data exception, the code name is: "; }
    @Override
    public String SiemensReadLengthCannotLargerThan19 (){ return "The number of arrays read does not allow greater than 19"; }
    @Override
    public String SiemensDataLengthCheckFailed (){ return "Block length checksum failed, please check if Put/get is turned on and DB block optimization is turned off"; }
    @Override
    public String SiemensFWError (){ return "An exception occurred, the specific information to find the Fetch/write protocol document"; }

    /***********************************************************************************
     *
     *    Omron PLC related
     *
     ************************************************************************************/

    @Override
    public String OmronAddressMustBeZeroToFiveteen (){ return "The bit address entered can only be between 0-15"; }
    @Override
    public String OmronReceiveDataError (){ return "Data Receive exception"; }
    @Override
    public String OmronStatus0 (){ return "Communication is normal."; }
    @Override
    public String OmronStatus1 (){ return "The message header is not fins"; }
    @Override
    public String OmronStatus2 (){ return "Data length too long"; }
    @Override
    public String OmronStatus3 (){ return "This command does not support"; }
    @Override
    public String OmronStatus20 (){ return "Exceeding connection limit"; }
    @Override
    public String OmronStatus21 (){ return "The specified node is already in the connection"; }
    @Override
    public String OmronStatus22 (){ return "Attempt to connect to a protected network node that is not yet configured in the PLC"; }
    @Override
    public String OmronStatus23 (){ return "The current client's network node exceeds the normal range"; }
    @Override
    public String OmronStatus24 (){ return "The current client's network node is already in use"; }
    @Override
    public String OmronStatus25 (){ return "All network nodes are already in use"; }



    /***********************************************************************************
     *
     *    AB PLC 相关
     *
     ************************************************************************************/


    @Override
    public String AllenBradley04 (){ return "The IOI could not be deciphered. Either it was not formed correctly or the match tag does not exist."; }
    @Override
    public String AllenBradley05 (){ return "The particular item referenced (usually instance) could not be found."; }
    @Override
    public String AllenBradley06 (){ return "The amount of data requested would not fit into the response buffer. Partial data transfer has occurred."; }
    @Override
    public String AllenBradley0A (){ return "An error has occurred trying to process one of the attributes."; }
    @Override
    public String AllenBradley13 (){ return "Not enough command data / parameters were supplied in the command to execute the service requested."; }
    @Override
    public String AllenBradley1C (){ return "An insufficient number of attributes were provided compared to the attribute count."; }
    @Override
    public String AllenBradley1E (){ return "A service request in this service went wrong."; }
    @Override
    public String AllenBradley26 (){ return "The IOI word length did not match the amount of IOI which was processed."; }

    @Override
    public String AllenBradleySessionStatus00 (){ return "success"; }
    @Override
    public String AllenBradleySessionStatus01 (){ return "The sender issued an invalid or unsupported encapsulation command."; }
    @Override
    public String AllenBradleySessionStatus02 (){ return "Insufficient memory resources in the receiver to handle the command. This is not an application error. Instead, it only results if the encapsulation layer cannot obtain memory resources that it need."; }
    @Override
    public String AllenBradleySessionStatus03 (){ return "Poorly formed or incorrect data in the data portion of the encapsulation message."; }
    @Override
    public String AllenBradleySessionStatus64 (){ return "An originator used an invalid session handle when sending an encapsulation message."; }
    @Override
    public String AllenBradleySessionStatus65 (){ return "The target received a message of invalid length."; }
    @Override
    public String AllenBradleySessionStatus69 (){ return "Unsupported encapsulation protocol revision."; }

    /***********************************************************************************
     *
     *    Panasonic PLC 相关
     *
     ************************************************************************************/
    @Override
    public String PanasonicReceiveLengthMustLargerThan9 (){ return "The received data length must be greater than 9"; }
    @Override
    public String PanasonicAddressParameterCannotBeNull (){ return "Address parameter is not allowed to be empty"; }
    @Override
    public String PanasonicMewStatus20 (){ return "Error unknown"; }
    @Override
    public String PanasonicMewStatus21 (){ return "Nack error, the remote unit could not be correctly identified, or a data error occurred."; }
    @Override
    public String PanasonicMewStatus22 (){ return "WACK Error: The receive buffer for the remote unit is full."; }
    @Override
    public String PanasonicMewStatus23 (){ return "Multiple port error: The remote unit number (01 to 16) is set to repeat with the local unit."; }
    @Override
    public String PanasonicMewStatus24 (){ return "Transport format error: An attempt was made to send data that does not conform to the transport format, or a frame data overflow or a data error occurred."; }
    @Override
    public String PanasonicMewStatus25 (){ return "Hardware error: Transport system hardware stopped operation."; }
    @Override
    public String PanasonicMewStatus26 (){ return "Unit Number error: The remote unit's numbering setting exceeds the range of 01 to 63."; }
    @Override
    public String PanasonicMewStatus27 (){ return "Error not supported: Receiver data frame overflow. An attempt was made to send data of different frame lengths between different modules."; }
    @Override
    public String PanasonicMewStatus28 (){ return "No answer error: The remote unit does not exist. (timeout)."; }
    @Override
    public String PanasonicMewStatus29 (){ return "Buffer Close error: An attempt was made to send or receive a buffer that is in a closed state."; }
    @Override
    public String PanasonicMewStatus30 (){ return "Timeout error: Persisted in transport forbidden State."; }
    @Override
    public String PanasonicMewStatus40 (){ return "BCC Error: A transmission error occurred in the instruction data."; }
    @Override
    public String PanasonicMewStatus41 (){ return "Malformed: The sent instruction information does not conform to the transmission format."; }
    @Override
    public String PanasonicMewStatus42 (){ return "Error not supported: An unsupported instruction was sent. An instruction was sent to a target station that was not supported."; }
    @Override
    public String PanasonicMewStatus43 (){ return "Processing Step Error: Additional instructions were sent when the transfer request information was suspended."; }
    @Override
    public String PanasonicMewStatus50 (){ return "Link Settings Error: A link number that does not actually exist is set."; }
    @Override
    public String PanasonicMewStatus51 (){ return "Simultaneous operation error: When issuing instructions to other units, the transmit buffer for the local unit is full."; }
    @Override
    public String PanasonicMewStatus52 (){ return "Transport suppression Error: Unable to transfer to other units."; }
    @Override
    public String PanasonicMewStatus53 (){ return "Busy error: Other instructions are being processed when the command is received."; }
    @Override
    public String PanasonicMewStatus60 (){ return "Parameter error: Contains code that cannot be used in the directive, or the code does not have a zone specified parameter (X, Y, D), and so on."; }
    @Override
    public String PanasonicMewStatus61 (){ return "Data error: Contact number, area number, Data code format (BCD,HEX, etc.) overflow, overflow, and area specified error."; }
    @Override
    public String PanasonicMewStatus62 (){ return "Register ERROR: Excessive logging of data in an unregistered state of operations (Monitoring records, tracking records, etc.). )。"; }
    @Override
    public String PanasonicMewStatus63 (){ return "PLC mode error: When an instruction is issued, the run mode is not able to process the instruction."; }
    @Override
    public String PanasonicMewStatus65 (){ return "Protection Error: Performs a write operation to the program area or system register in the storage protection state."; }
    @Override
    public String PanasonicMewStatus66 (){ return "Address Error: Address (program address, absolute address, etc.) Data encoding form (BCD, hex, etc.), overflow, underflow, or specified range error."; }
    @Override
    public String PanasonicMewStatus67 (){ return "Missing data error: The data to be read does not exist. (reads data that is not written to the comment register.)"; }

}
