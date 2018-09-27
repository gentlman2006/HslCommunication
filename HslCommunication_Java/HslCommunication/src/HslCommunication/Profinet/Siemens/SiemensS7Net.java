package HslCommunication.Profinet.Siemens;

import HslCommunication.BasicFramework.SoftBasic;
import HslCommunication.Core.IMessage.S7Message;
import HslCommunication.Core.Net.NetworkBase.NetworkDeviceBase;
import HslCommunication.Core.Transfer.ReverseBytesTransform;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.Core.Types.OperateResultExThree;
import HslCommunication.Core.Types.OperateResultExTwo;
import HslCommunication.StringResources;
import HslCommunication.Utilities;

import java.io.ByteArrayOutputStream;
import java.net.Socket;
import java.util.Arrays;

/**
 * 西门子的数据交互类，采用s7协议实现
 */
public class SiemensS7Net extends NetworkDeviceBase<S7Message, ReverseBytesTransform> {


    /**
     * 实例化一个西门子的S7协议的通讯对象
     * @param siemens 指定西门子的型号
     */
    public SiemensS7Net(SiemensPLCS siemens) {
        Initialization(siemens, "");
    }



    /**
     * 实例化一个西门子的S7协议的通讯对象并指定Ip地址
     * @param siemens 指定西门子的型号
     * @param ipAddress Ip地址
     */
    public SiemensS7Net(SiemensPLCS siemens, String ipAddress) {
        Initialization(siemens, ipAddress);
    }


    /**
     * 初始化方法
     * @param siemens 西门子类型
     * @param ipAddress Ip地址
     */
    private void Initialization(SiemensPLCS siemens, String ipAddress) {
        WordLength = 2;
        setIpAddress(ipAddress);
        setPort(102);
        CurrentPlc = siemens;

        switch (siemens) {
            case S1200:
                plcHead1[21] = 0;
                break;
            case S300:
                plcHead1[21] = 2;
                break;
            case S1500:
                plcHead1[21] = 0;
                break;
            case S200Smart: {
                plcHead1 = plcHead1_200smart;
                plcHead2 = plcHead2_200smart;
                break;
            }
            default:
                plcHead1[18] = 0;
                break;
        }
    }


    /**
     * 在客户端连接上服务器后，所做的一些初始化操作 ->
     * Two handshake actions required after connecting to the server
     * @param socket 网络套接字
     * @return 返回连接结果
     */
    @Override
    protected OperateResult InitializationOnConnect(Socket socket) {
        // 第一层通信的初始化
        OperateResultExTwo<byte[], byte[]> read_first = ReadFromCoreServerBase(socket, plcHead1);
        if (!read_first.IsSuccess) return read_first;

        // 第二层通信的初始化
        OperateResultExTwo<byte[], byte[]> read_second = ReadFromCoreServerBase(socket, plcHead2);
        if (!read_second.IsSuccess) return read_second;

        // 返回成功的信号
        return OperateResult.CreateSuccessResult();
    }



    /**
     * 从PLC读取订货号信息
     * @return
     */
    public OperateResultExOne<String> ReadOrderNumber() {
        OperateResultExOne<String> result = new OperateResultExOne<String>();
        OperateResultExOne<byte[]> read = ReadFromCoreServer(plcOrderNumber);
        if (read.IsSuccess) {
            if (read.Content.length > 100) {
                result.IsSuccess = true;
                result.Content = Utilities.getString(Arrays.copyOfRange(read.Content, 71, 20), "ASCII");
            }
        }

        if (!result.IsSuccess) {
            result.CopyErrorFromOther(read);
        }

        return result;
    }



    /**
     * 从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，T100，C100以字节为单位
     * @param address 起始地址，格式为I100，M100，Q100，DB20.100
     * @param length 读取的数量，以字节为单位
     * @return 结果对象
     */
    @Override
    public OperateResultExOne<byte[]> Read(String address, short length) {
        OperateResultExThree<Byte, Integer, Integer> addressResult = AnalysisAddress(address);
        if (!addressResult.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(addressResult);

        ByteArrayOutputStream outputStream = new ByteArrayOutputStream();

        short alreadyFinished = 0;
        while (alreadyFinished < length) {
            short readLength = (short) Math.min(length - alreadyFinished, 200);

            OperateResultExThree<Byte, Integer, Integer>[] list = new OperateResultExThree[1];
            list[0] = addressResult;

            OperateResultExOne<byte[]> read = Read(list, new short[]{readLength});
            if (read.IsSuccess) {
                try {
                    outputStream.write(read.Content);
                } catch (Exception ex) {

                }
            } else {
                return read;
            }

            alreadyFinished += readLength;
            addressResult.Content2 += readLength * 8;
        }

        byte[] buffer = outputStream.toByteArray();

        try {
            outputStream.close();
        } catch (Exception ex) {

        }

        return OperateResultExOne.CreateSuccessResult(buffer);
    }


    /**
     * 从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，以位为单位
     * @param address 起始地址，格式为I100，M100，Q100，DB20.100
     * @return 结果对象
     */
    private OperateResultExOne<byte[]> ReadBitFromPLC(String address) {
        OperateResultExOne<byte[]> command = BuildBitReadCommand(address);
        if (!command.IsSuccess) return command;

        OperateResultExOne<byte[]> read = ReadFromCoreServer(command.Content);
        if (!read.IsSuccess) return read;


        int receiveCount = 1;
        if (read.Content.length >= 21 && read.Content[20] == 1) {
            // 分析结果
            byte[] buffer = new byte[receiveCount];

            if (22 < read.Content.length) {
                if (read.Content[21] == (byte) 0xFF && read.Content[22] == 0x03) {
                    // 有数据
                    buffer[0] = read.Content[25];
                }
            }

            return OperateResultExOne.CreateSuccessResult(buffer);
        }
        else {
            return new OperateResultExOne<byte[]>(read.ErrorCode,StringResources.Language.SiemensDataLengthCheckFailed());
        }
    }


    /**
     * 一次性从PLC获取所有的数据，按照先后顺序返回一个统一的Buffer，需要按照顺序处理，两个数组长度必须一致
     * @param address 起始地址数组
     * @param length 数据长度数组
     * @return 结果数据对象
     */
    public OperateResultExOne<byte[]> Read(String[] address, short[] length) {

        OperateResultExThree<Byte, Integer, Integer>[] list = new OperateResultExThree[address.length];
        for (int i = 0; i < address.length; i++) {
            OperateResultExThree<Byte, Integer, Integer> tmp = AnalysisAddress(address[i]);
            if (!tmp.IsSuccess) return OperateResultExOne.CreateFailedResult(tmp);

            list[i] = tmp;
        }

        return Read(list, length);
    }


    /**
     * 读取真实的数据
     * @param address 起始地址
     * @param length 长度
     * @return 结果类对象
     */
    private OperateResultExOne<byte[]> Read(OperateResultExThree<Byte, Integer, Integer>[] address, short[] length) {

        OperateResultExOne<byte[]> command = BuildReadCommand(address, length);
        if (!command.IsSuccess) return command;

        OperateResultExOne<byte[]> read = ReadFromCoreServer(command.Content);
        if (!read.IsSuccess) return read;

        int receiveCount = 0;
        for (int i = 0; i < length.length; i++) {
            receiveCount += length[i];
        }

        if (read.Content.length >= 21 && read.Content[20] == length.length) {
            // 分析结果
            byte[] buffer = new byte[receiveCount];
            int kk = 0;
            int ll = 0;
            for (int ii = 21; ii < read.Content.length; ii++) {
                if ((ii + 1) < read.Content.length) {
                    if (read.Content[ii] == (byte) 0xFF &&
                            read.Content[ii + 1] == 0x04) {
                        // 有数据
                        System.arraycopy(read.Content, ii + 4, buffer, ll, length[kk]);
                        ii += length[kk] + 3;
                        ll += length[kk];
                        kk++;
                    }
                }
            }

            return OperateResultExOne.CreateSuccessResult(buffer);
        } else {
            return new OperateResultExOne<byte[]>(read.ErrorCode,StringResources.Language.SiemensDataLengthCheckFailed());
        }
    }



    /**
     * 读取指定地址的bool数据
     * @param address 起始地址，格式为I100，M100，Q100，DB20.100
     * @return 结果类对象
     */
    public OperateResultExOne<Boolean> ReadBool(String address) {
        return GetBoolResultFromBytes(ReadBitFromPLC(address));
    }



    /**
     * 读取指定地址的byte数据
     * @param address 起始地址，格式为I100，M100，Q100，DB20.100
     * @return 结果类对象
     */
    public OperateResultExOne<Byte> ReadByte(String address) {
        return GetByteResultFromBytes(Read(address, (short) 1));
    }



    /**
     * 基础的写入数据的操作支持
     * @param entireValue 完整的字节数据
     * @return 写入结果
     */
    private OperateResult WriteBase(byte[] entireValue) {
        OperateResultExOne<byte[]> write = ReadFromCoreServer(entireValue);
        if (!write.IsSuccess) return write;

        if (write.Content[write.Content.length - 1] != (byte) 0xFF) {
            return new OperateResult(write.Content[write.Content.length - 1], StringResources.Language.SiemensWriteError() + write.Content[write.Content.length - 1]);
        } else {
            return OperateResult.CreateSuccessResult();
        }
    }



    /**
     * 将数据写入到PLC数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位
     * @param address 起始地址，格式为I100，M100，Q100，DB20.100
     * @param value 写入的数据，长度根据data的长度来指示
     * @return 写入结果
     */
    @Override
    public OperateResult Write(String address, byte[] value) {
        OperateResultExThree<Byte, Integer, Integer> analysis = AnalysisAddress( address );
        if (!analysis.IsSuccess) return OperateResultExOne.CreateFailedResult( analysis );

        int length = value.length;
        int alreadyFinished = 0;
        while (alreadyFinished < length)
        {
            short writeLength = (short) Math.min( length - alreadyFinished, 200 );
            byte[] buffer = getByteTransform().TransByte( value, alreadyFinished, writeLength );

            OperateResultExOne<byte[]> command = BuildWriteByteCommand( analysis, buffer );
            if (!command.IsSuccess) return command;

            OperateResult write = WriteBase( command.Content );
            if (!write.IsSuccess) return write;

            alreadyFinished += writeLength;
            analysis.Content2 += writeLength * 8;
        }

        return OperateResult.CreateSuccessResult( );
    }




    /**
     * 写入PLC的一个位，例如"M100.6"，"I100.7"，"Q100.0"，"DB20.100.0"，如果只写了"M100"默认为"M100.0
     * @param address 起始地址，格式为I100，M100，Q100，DB20.100
     * @param value 写入的数据，True或是False
     * @return 写入结果
     */
    public OperateResult Write(String address, boolean value) {
        // 生成指令
        OperateResultExOne<byte[]> command = BuildWriteBitCommand(address, value);
        if (!command.IsSuccess) return command;

        return WriteBase(command.Content);
    }


    /**
     * 向PLC中写入bool数组，返回值说明，比如你写入M100,那么data[0]对应M100.0
     * @param address 要写入的数据地址
     * @param values 要写入的实际数据，长度为8的倍数
     * @return 写入结果
     */
    public OperateResult Write(String address, boolean[] values) {
        return Write(address, SoftBasic.BoolArrayToByte(values));
    }




    /**
     * 向PLC中写入byte数据，返回值说明
     * @param address 要写入的数据地址
     * @param value 要写入的实际数据
     * @return 写入结果
     */
    public OperateResult Write(String address, byte value) {
        return Write(address, new byte[]{value});
    }


    private byte[] plcHead1 = new byte[]
            {
                    0x03,0x00,0x00,0x16,0x11,(byte) 0xE0,0x00,0x00,0x00,0x01,0x00,(byte) 0xC0,0x01,
                    0x0A,(byte) 0xC1,0x02,0x01,0x02,(byte) 0xC2,0x02,0x01,0x00
            };
    private byte[] plcHead2 = new byte[]
            {
                    0x03,0x00,0x00,0x19,0x02,(byte) 0xF0,(byte) 0x80,0x32,0x01,0x00,0x00,0x04,0x00,0x00,0x08,0x00,0x00,
                    (byte) 0xF0,0x00,0x00,0x01,0x00,0x01,0x01,(byte) 0xE0
            };
    private byte[] plcOrderNumber = new byte[]
            {
                    0x03,0x00,0x00,0x21,0x02,(byte) 0xF0,(byte) 0x80,0x32,0x07,0x00,0x00,0x00,0x01,0x00,0x08,0x00,0x08,
                    0x00,0x01,0x12,0x04,0x11,0x44,0x01,0x00,(byte) 0xFF,0x09,0x00,0x04,0x00,0x11,0x00,0x00
            };
    private SiemensPLCS CurrentPlc = SiemensPLCS.S1200;
    private byte[] plcHead1_200smart = new byte[]
            {
                    0x03,0x00,0x00,0x16,0x11,(byte) 0xE0,0x00,0x00,0x00,0x01,0x00,(byte) 0xC1,0x02,0x10,0x00,(byte) 0xC2,
                    0x02,0x03,0x00,(byte) 0xC0,0x01,0x0A
            };
    private byte[] plcHead2_200smart = new byte[]
            {
                    0x03,0x00,0x00,0x19,0x02,(byte) 0xF0,(byte) 0x80,0x32,0x01,0x00,0x00,(byte) 0xCC,(byte) 0xC1,
                    0x00,0x08,0x00,0x00,(byte) 0xF0,0x00,0x00,0x01,0x00,0x01,0x03,(byte) 0xC0
            };


    /**
     * 返回表示当前对象的字符串
     * @return 字符串信息
     */
    @Override
    public String toString() {
        return "SiemensS7Net";
    }



    /**
     * 计算特殊的地址信息
     * @param address 字符串信息
     * @return 实际值
     */
    public static int CalculateAddressStarted(String address) {
        if (address.indexOf('.') < 0) {
            return Integer.parseInt(address) * 8;
        } else {
            String[] temp = address.split("\\.");
            return Integer.parseInt(temp[0]) * 8 + Integer.parseInt(temp[1]);
        }
    }


    /**
     * 解析数据地址，解析出地址类型，起始地址，DB块的地址
     * @param address 数据地址
     * @return 解析出地址类型，起始地址，DB块的地址
     */
    public static OperateResultExThree<Byte, Integer, Integer> AnalysisAddress(String address) {
        OperateResultExThree<Byte, Integer, Integer> result = new OperateResultExThree<Byte, Integer, Integer>();
        try {
            result.Content3 = 0;
            if (address.charAt(0) == 'I') {
                result.Content1 = (byte) 0x81;
                result.Content2 = CalculateAddressStarted(address.substring(1));
            } else if (address.charAt(0) == 'Q') {
                result.Content1 = (byte) 0x82;
                result.Content2 = CalculateAddressStarted(address.substring(1));
            } else if (address.charAt(0) == 'M') {
                result.Content1 = (byte) 0x83;
                result.Content2 = CalculateAddressStarted(address.substring(1));
            } else if (address.charAt(0) == 'D' || address.substring(0, 2) == "DB") {
                result.Content1 = (byte) 0x84;
                String[] adds = address.split("\\.");
                if (address.charAt(1) == 'B') {
                    result.Content3 = Integer.parseInt(adds[0].substring(2));
                } else {
                    result.Content3 = Integer.parseInt(adds[0].substring(1));
                }

                result.Content2 = CalculateAddressStarted(address.substring(address.indexOf('.') + 1));
            } else if (address.charAt(0) == 'T') {
                result.Content1 = 0x1D;
                result.Content2 = CalculateAddressStarted(address.substring(1));
            } else if (address.charAt(0) == 'C') {
                result.Content1 = 0x1C;
                result.Content2 = CalculateAddressStarted(address.substring(1));
            }
            else if (address.charAt(0) == 'V')
            {
                result.Content1 = (byte) 0x84;
                result.Content3 = 1;
                result.Content2 = CalculateAddressStarted( address.substring( 1 ) );
            } else {
                return new OperateResultExThree<Byte, Integer, Integer>(StringResources.Language.NotSupportedDataType());
            }
        } catch (Exception ex) {
            result.Message = ex.getMessage();
            return result;
        }

        result.IsSuccess = true;
        return result;
    }

    /**
     * 生成一个读取字数据指令头的通用方法 ->
     * A general method for generating a command header to read a Word data
     * @param address 起始地址，例如M100，I0，Q0，DB2.100 -> Start address, such as M100,I0,Q0,DB2.100
     * @param length 读取数据长度 -> Read Data length
     * @return 包含结果对象的报文 -> Message containing the result object
     */
    public static OperateResultExOne<byte[]> BuildReadCommand( String address, short length )
    {
        OperateResultExThree<Byte, Integer, Integer> analysis = AnalysisAddress( address );
        if (!analysis.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult( analysis );

        OperateResultExThree<Byte, Integer, Integer>[] addressList = new OperateResultExThree[1];
        addressList[0] = analysis;
        short[] lengthList = new short[1];
        lengthList[0] = length;

        return BuildReadCommand( addressList, lengthList );
    }


    /**
     * 生成一个读取字数据指令头的通用方法
     * @param address 解析后的地址
     * @param length 每个地址的读取长度
     * @return 携带有命令字节
     */
    public static OperateResultExOne<byte[]> BuildReadCommand(OperateResultExThree<Byte, Integer, Integer>[] address, short[] length) {
        if (address == null) throw new RuntimeException("address is null");
        if (length == null) throw new RuntimeException("count is null");
        if (address.length != length.length) throw new RuntimeException(StringResources.Language.TwoParametersLengthIsNotSame());
        if (length.length > 19) throw new RuntimeException(StringResources.Language.SiemensReadLengthCannotLargerThan19());

        int readCount = length.length;

        byte[] _PLCCommand = new byte[19 + readCount * 12];

        // ======================================================================================
        // Header
        // 报文头
        _PLCCommand[0] = 0x03;
        _PLCCommand[1] = 0x00;
        // 长度
        _PLCCommand[2] = (byte) (_PLCCommand.length / 256);
        _PLCCommand[3] = (byte) (_PLCCommand.length % 256);
        // 固定
        _PLCCommand[4] = 0x02;
        _PLCCommand[5] = (byte) 0xF0;
        _PLCCommand[6] = (byte) 0x80;
        // 协议标识
        _PLCCommand[7] = 0x32;
        // 命令：发
        _PLCCommand[8] = 0x01;
        // redundancy identification (reserved): 0x0000;
        _PLCCommand[9] = 0x00;
        _PLCCommand[10] = 0x00;
        // protocol data unit reference; it’s increased by request event;
        _PLCCommand[11] = 0x00;
        _PLCCommand[12] = 0x01;
        // 参数命令数据总长度
        _PLCCommand[13] = (byte) ((_PLCCommand.length - 17) / 256);
        _PLCCommand[14] = (byte) ((_PLCCommand.length - 17) % 256);

        // 读取内部数据时为00，读取CPU型号为Data数据长度
        _PLCCommand[15] = 0x00;
        _PLCCommand[16] = 0x00;


        // ======================================================================================
        // Parameter

        // 读写指令，04读，05写
        _PLCCommand[17] = 0x04;
        // 读取数据块个数
        _PLCCommand[18] = (byte) readCount;


        for (int ii = 0; ii < readCount; ii++) {
            //===========================================================================================
            // 指定有效值类型
            _PLCCommand[19 + ii * 12] = 0x12;
            // 接下来本次地址访问长度
            _PLCCommand[20 + ii * 12] = 0x0A;
            // 语法标记，ANY
            _PLCCommand[21 + ii * 12] = 0x10;
            // 按字为单位
            _PLCCommand[22 + ii * 12] = 0x02;
            // 访问数据的个数
            _PLCCommand[23 + ii * 12] = (byte) (length[ii] / 256);
            _PLCCommand[24 + ii * 12] = (byte) (length[ii] % 256);
            // DB块编号，如果访问的是DB块的话
            _PLCCommand[25 + ii * 12] = (byte) (address[ii].Content3 / 256);
            _PLCCommand[26 + ii * 12] = (byte) (address[ii].Content3 % 256);
            // 访问数据类型
            _PLCCommand[27 + ii * 12] = address[ii].Content1;
            // 偏移位置
            _PLCCommand[28 + ii * 12] = (byte) (address[ii].Content2 / 256 / 256 % 256);
            _PLCCommand[29 + ii * 12] = (byte) (address[ii].Content2 / 256 % 256);
            _PLCCommand[30 + ii * 12] = (byte) (address[ii].Content2 % 256);
        }

        return OperateResultExOne.CreateSuccessResult(_PLCCommand);
    }


    /**
     * 生成一个位读取数据指令头的通用方法
     * @param address 起始地址
     * @return 指令
     */
    public static OperateResultExOne<byte[]> BuildBitReadCommand(String address) {
        byte[] _PLCCommand = new byte[31];
        // 报文头
        _PLCCommand[0] = 0x03;
        _PLCCommand[1] = 0x00;
        // 长度
        _PLCCommand[2] = (byte) (_PLCCommand.length / 256);
        _PLCCommand[3] = (byte) (_PLCCommand.length % 256);
        // 固定
        _PLCCommand[4] = 0x02;
        _PLCCommand[5] = (byte) 0xF0;
        _PLCCommand[6] = (byte) 0x80;
        _PLCCommand[7] = 0x32;
        // 命令：发
        _PLCCommand[8] = 0x01;
        // 标识序列号
        _PLCCommand[9] = 0x00;
        _PLCCommand[10] = 0x00;
        _PLCCommand[11] = 0x00;
        _PLCCommand[12] = 0x01;
        // 命令数据总长度
        _PLCCommand[13] = (byte) ((_PLCCommand.length - 17) / 256);
        _PLCCommand[14] = (byte) ((_PLCCommand.length - 17) % 256);

        _PLCCommand[15] = 0x00;
        _PLCCommand[16] = 0x00;

        // 命令起始符
        _PLCCommand[17] = 0x04;
        // 读取数据块个数
        _PLCCommand[18] = 0x01;


        // 填充数据
        OperateResultExThree<Byte, Integer, Integer> analysis = AnalysisAddress(address);
        if (!analysis.IsSuccess) return OperateResultExOne.CreateFailedResult(analysis);

        //===========================================================================================
        // 读取地址的前缀
        _PLCCommand[19] = 0x12;
        _PLCCommand[20] = 0x0A;
        _PLCCommand[21] = 0x10;
        // 读取的数据时位
        _PLCCommand[22] = 0x01;
        // 访问数据的个数
        _PLCCommand[23] = 0x00;
        _PLCCommand[24] = 0x01;
        // DB块编号，如果访问的是DB块的话
        _PLCCommand[25] = (byte) (analysis.Content3 / 256);
        _PLCCommand[26] = (byte) (analysis.Content3 % 256);
        // 访问数据类型
        _PLCCommand[27] = analysis.Content1;
        // 偏移位置
        _PLCCommand[28] = (byte) (analysis.Content2 / 256 / 256 % 256);
        _PLCCommand[29] = (byte) (analysis.Content2 / 256 % 256);
        _PLCCommand[30] = (byte) (analysis.Content2 % 256);

        return OperateResultExOne.CreateSuccessResult(_PLCCommand);
    }


    /**
     * 生成一个写入字节数据的指令 -> Generate an instruction to write byte data
     * @param address 起始地址，示例M100,I100,Q100,DB1.100 -> Start Address, example M100,I100,Q100,DB1.100
     * @param data 原始的字节数据 -> Raw byte data
     * @return 包含结果对象的报文 -> Message containing the result object
     */
    public static OperateResultExOne<byte[]> BuildWriteByteCommand( String address, byte[] data )
    {
        if (data == null) data = new byte[0];

        OperateResultExThree<Byte, Integer, Integer> analysis = AnalysisAddress( address );
        if (!analysis.IsSuccess) return OperateResultExOne.CreateFailedResult( analysis );

        return BuildWriteByteCommand( analysis, data );
    }

    /**
     * 生成一个写入字节数据的指令
     * @param analysis 地址
     * @param data 数据
     * @return 指令
     */
    public static OperateResultExOne<byte[]> BuildWriteByteCommand(OperateResultExThree<Byte, Integer, Integer> analysis, byte[] data) {
        if (data == null) data = new byte[0];

        byte[] _PLCCommand = new byte[35 + data.length];
        _PLCCommand[0] = 0x03;
        _PLCCommand[1] = 0x00;
        // 长度
        _PLCCommand[2] = (byte) ((35 + data.length) / 256);
        _PLCCommand[3] = (byte) ((35 + data.length) % 256);
        // 固定
        _PLCCommand[4] = 0x02;
        _PLCCommand[5] = (byte) 0xF0;
        _PLCCommand[6] = (byte) 0x80;
        _PLCCommand[7] = 0x32;
        // 命令 发
        _PLCCommand[8] = 0x01;
        // 标识序列号
        _PLCCommand[9] = 0x00;
        _PLCCommand[10] = 0x00;
        _PLCCommand[11] = 0x00;
        _PLCCommand[12] = 0x01;
        // 固定
        _PLCCommand[13] = 0x00;
        _PLCCommand[14] = 0x0E;
        // 写入长度+4
        _PLCCommand[15] = (byte) ((4 + data.length) / 256);
        _PLCCommand[16] = (byte) ((4 + data.length) % 256);
        // 读写指令
        _PLCCommand[17] = 0x05;
        // 写入数据块个数
        _PLCCommand[18] = 0x01;
        // 固定，返回数据长度
        _PLCCommand[19] = 0x12;
        _PLCCommand[20] = 0x0A;
        _PLCCommand[21] = 0x10;
        // 写入方式，1是按位，2是按字
        _PLCCommand[22] = 0x02;
        // 写入数据的个数
        _PLCCommand[23] = (byte) (data.length / 256);
        _PLCCommand[24] = (byte) (data.length % 256);
        // DB块编号，如果访问的是DB块的话
        _PLCCommand[25] = (byte) (analysis.Content3 / 256);
        _PLCCommand[26] = (byte) (analysis.Content3 % 256);
        // 写入数据的类型
        _PLCCommand[27] = analysis.Content1;
        // 偏移位置
        _PLCCommand[28] = (byte) (analysis.Content2 / 256 / 256 % 256);
        ;
        _PLCCommand[29] = (byte) (analysis.Content2 / 256 % 256);
        _PLCCommand[30] = (byte) (analysis.Content2 % 256);
        // 按字写入
        _PLCCommand[31] = 0x00;
        _PLCCommand[32] = 0x04;
        // 按位计算的长度
        _PLCCommand[33] = (byte) (data.length * 8 / 256);
        _PLCCommand[34] = (byte) (data.length * 8 % 256);

        System.arraycopy(data, 0, _PLCCommand, 35, data.length);

        return OperateResultExOne.CreateSuccessResult(_PLCCommand);
    }


    /**
     * 生成一个写入位数据的指令
     * @param address 起始地址
     * @param data 数据
     * @return 指令
     */
    public static OperateResultExOne<byte[]> BuildWriteBitCommand(String address, boolean data) {
        OperateResultExThree<Byte, Integer, Integer> analysis = AnalysisAddress(address);
        if (!analysis.IsSuccess) return OperateResultExOne.CreateFailedResult(analysis);


        byte[] buffer = new byte[1];
        buffer[0] = data ? (byte) 0x01 : (byte) 0x00;

        byte[] _PLCCommand = new byte[35 + buffer.length];
        _PLCCommand[0] = 0x03;
        _PLCCommand[1] = 0x00;
        // 长度
        _PLCCommand[2] = (byte) ((35 + buffer.length) / 256);
        _PLCCommand[3] = (byte) ((35 + buffer.length) % 256);
        // 固定
        _PLCCommand[4] = 0x02;
        _PLCCommand[5] = (byte) 0xF0;
        _PLCCommand[6] = (byte) 0x80;
        _PLCCommand[7] = 0x32;
        // 命令 发
        _PLCCommand[8] = 0x01;
        // 标识序列号
        _PLCCommand[9] = 0x00;
        _PLCCommand[10] = 0x00;
        _PLCCommand[11] = 0x00;
        _PLCCommand[12] = 0x01;
        // 固定
        _PLCCommand[13] = 0x00;
        _PLCCommand[14] = 0x0E;
        // 写入长度+4
        _PLCCommand[15] = (byte) ((4 + buffer.length) / 256);
        _PLCCommand[16] = (byte) ((4 + buffer.length) % 256);
        // 命令起始符
        _PLCCommand[17] = 0x05;
        // 写入数据块个数
        _PLCCommand[18] = 0x01;
        _PLCCommand[19] = 0x12;
        _PLCCommand[20] = 0x0A;
        _PLCCommand[21] = 0x10;
        // 写入方式，1是按位，2是按字
        _PLCCommand[22] = 0x01;
        // 写入数据的个数
        _PLCCommand[23] = (byte) (buffer.length / 256);
        _PLCCommand[24] = (byte) (buffer.length % 256);
        // DB块编号，如果访问的是DB块的话
        _PLCCommand[25] = (byte) (analysis.Content3 / 256);
        _PLCCommand[26] = (byte) (analysis.Content3 % 256);
        // 写入数据的类型
        _PLCCommand[27] = analysis.Content1;
        // 偏移位置
        _PLCCommand[28] = (byte) (analysis.Content2 / 256 / 256);
        _PLCCommand[29] = (byte) (analysis.Content2 / 256);
        _PLCCommand[30] = (byte) (analysis.Content2 % 256);
        // 按位写入
        _PLCCommand[31] = 0x00;
        _PLCCommand[32] = 0x03;
        // 按位计算的长度
        _PLCCommand[33] = (byte) (buffer.length / 256);
        _PLCCommand[34] = (byte) (buffer.length % 256);

        System.arraycopy(buffer, 0, _PLCCommand, 35, buffer.length);

        return OperateResultExOne.CreateSuccessResult(_PLCCommand);
    }

}
