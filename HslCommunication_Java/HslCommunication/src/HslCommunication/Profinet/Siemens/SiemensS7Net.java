package HslCommunication.Profinet.Siemens;

import HslCommunication.BasicFramework.SoftBasic;
import HslCommunication.Core.IMessage.S7Message;
import HslCommunication.Core.Net.NetworkBase.NetworkDeviceBase;
import HslCommunication.Core.Transfer.ReverseBytesTransform;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.Core.Types.OperateResultExThree;
import HslCommunication.Core.Types.OperateResultExTwo;
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
     *在客户端连接上服务器后，所做的一些初始化操作
     * @param socket 网络套接字
     * @return 返回连接结果
     */
    @Override
    protected OperateResult InitializationOnConnect(Socket socket) {
        // 第一层通信的初始化
        OperateResultExTwo<byte[], byte[]> read_first = ReadFromCoreServerBase(socket, plcHead1);
        if (!read_first.IsSuccess) {
            return read_first;
        }

        // 第二层通信的初始化
        OperateResultExTwo<byte[], byte[]> read_second = ReadFromCoreServerBase(socket, plcHead2);
        if (!read_second.IsSuccess) {
            return read_second;
        }

        // 返回成功的信号
        return HslCommunication.Core.Types.OperateResult.CreateSuccessResult();
    }


    /**
     * 计算特殊的地址信息
     * @param address 字符串信息
     * @return 实际值
     */
    private int CalculateAddressStarted(String address) {
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
    private OperateResultExThree<Byte, Integer, Integer> AnalysisAddress(String address) {
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
            } else {
                result.Message = "不支持的数据类型";
                result.Content1 = 0;
                result.Content2 = 0;
                result.Content3 = 0;
                return result;
            }
        } catch (Exception ex) {
            result.Message = ex.getMessage();
            return result;
        }

        result.IsSuccess = true;
        return result;
    }


    /**
     * 生成一个读取字数据指令头的通用方法
     * @param address 解析后的地址
     * @param length 每个地址的读取长度
     * @return 携带有命令字节
     */
    private OperateResultExOne<byte[]> BuildReadCommand(OperateResultExThree<Byte, Integer, Integer>[] address, short[] length) {
        if (address == null) throw new RuntimeException("address为空");
        if (length == null) throw new RuntimeException("count为空");
        if (address.length != length.length) throw new RuntimeException("两个参数的个数不统一");
        if (length.length > 19) throw new RuntimeException("读取的数组数量不允许大于19");

        OperateResultExOne<byte[]> result = new OperateResultExOne<byte[]>();
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
        result.Content = _PLCCommand;
        result.IsSuccess = true;
        return result;
    }


    /**
     * 生成一个位读取数据指令头的通用方法
     * @param address 起始地址
     * @return 指令
     */
    private OperateResultExOne<byte[]> BuildBitReadCommand(String address) {
        OperateResultExOne<byte[]> result = new OperateResultExOne<byte[]>();

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
        if (!analysis.IsSuccess) {
            result.CopyErrorFromOther(analysis);
            return result;
        }

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

        result.Content = _PLCCommand;
        result.IsSuccess = true;
        return result;
    }



    /**
     * 生成一个写入字节数据的指令
     * @param address 地址
     * @param data 数据
     * @return 指令
     */
    private OperateResultExOne<byte[]> BuildWriteByteCommand(String address, byte[] data) {
        if (data == null) data = new byte[0];
        OperateResultExOne<byte[]> result = new OperateResultExOne<byte[]>();

        OperateResultExThree<Byte, Integer, Integer> analysis = AnalysisAddress(address);
        if (!analysis.IsSuccess) {
            result.CopyErrorFromOther(analysis);
            return result;
        }

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

        result.Content = _PLCCommand;
        result.IsSuccess = true;
        return result;
    }


    /**
     * 生成一个写入位数据的指令
     * @param address 起始地址
     * @param data 数据
     * @return 指令
     */
    private OperateResultExOne<byte[]> BuildWriteBitCommand(String address, boolean data) {
        OperateResultExOne<byte[]> result = new OperateResultExOne<byte[]>();

        OperateResultExThree<Byte, Integer, Integer> analysis = AnalysisAddress(address);
        if (!analysis.IsSuccess) {
            result.CopyErrorFromOther(analysis);
            return result;
        }


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

        result.Content = _PLCCommand;
        result.IsSuccess = true;
        return result;
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
        if (!addressResult.IsSuccess) {
            return OperateResultExOne.<byte[]>CreateFailedResult(addressResult);
        }


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
        OperateResultExOne<byte[]> result = new OperateResultExOne<byte[]>();

        OperateResultExOne<byte[]> command = BuildBitReadCommand(address);
        if (!command.IsSuccess) {
            result.CopyErrorFromOther(command);
            return result;
        }

        OperateResultExOne<byte[]> read = ReadFromCoreServer(command.Content);
        if (read.IsSuccess) {
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

                result.Content = buffer;
                result.IsSuccess = true;
            } else {
                result.ErrorCode = read.ErrorCode;
                result.Message = "数据块长度校验失败";
            }
        } else {
            result.ErrorCode = read.ErrorCode;
            result.Message = read.Message;
        }

        return result;
    }


    /**
     * 一次性从PLC获取所有的数据，按照先后顺序返回一个统一的Buffer，需要按照顺序处理，两个数组长度必须一致
     * @param address 起始地址数组
     * @param length 数据长度数组
     * @return 结果数据对象
     */
    public OperateResultExOne<byte[]> Read(String[] address, short[] length) {
        OperateResultExOne<byte[]> result = new OperateResultExOne<byte[]>();

        OperateResultExThree<Byte, Integer, Integer>[] list = new OperateResultExThree[address.length];
        for (int i = 0; i < address.length; i++) {
            OperateResultExThree<Byte, Integer, Integer> tmp = AnalysisAddress(address[i]);
            if (!tmp.IsSuccess) {
                result.CopyErrorFromOther(tmp);
                return result;
            }

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
        OperateResultExOne<byte[]> result = new OperateResultExOne<byte[]>();

        OperateResultExOne<byte[]> command = BuildReadCommand(address, length);
        if (!command.IsSuccess) {
            result.CopyErrorFromOther(command);
            return result;
        }

        OperateResultExOne<byte[]> read = ReadFromCoreServer(command.Content);
        if (read.IsSuccess) {
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

                result.Content = buffer;
                result.IsSuccess = true;
            } else {
                result.ErrorCode = read.ErrorCode;
                result.Message = "数据块长度校验失败";
            }
        } else {
            result.ErrorCode = read.ErrorCode;
            result.Message = read.Message;
        }

        return result;
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

        if (write.Content[write.Content.length - 1] != 0xFF) {
            HslCommunication.Core.Types.OperateResult result = new OperateResult();
            result.ErrorCode = write.Content[write.Content.length - 1];
            result.Message = "写入数据异常";
            return result;
        } else {
            return HslCommunication.Core.Types.OperateResult.CreateSuccessResult();
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
        OperateResultExOne<byte[]> command = BuildWriteByteCommand(address, value);
        if (!command.IsSuccess) return command;

        return WriteBase(command.Content);
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
     * 向PLC中写入指定长度的字符串,超出截断，不够补0，编码格式为ASCII
     * @param address 要写入的数据地址
     * @param value 要写入的实际数据
     * @param length 指定的字符串长度，必须大于0
     * @return 写入结果
     */
    public OperateResult Write(String address, String value, int length) {
        byte[] temp = getByteTransform().TransByte(value, "ASCII");
        temp = SoftBasic.ArrayExpandToLength(temp, length);
        return Write(address, temp);
    }




    /**
     * 向PLC中写入字符串，编码格式为Unicode
     * @param address 要写入的数据地址
     * @param value 要写入的实际数据
     * @return 写入结果
     */
    public OperateResult WriteUnicodeString(String address, String value) {
        byte[] temp = Utilities.string2Byte(value);
        return Write(address, temp);
    }


    /**
     * 向PLC中写入指定长度的字符串,超出截断，不够补0，编码格式为Unicode
     * @param address 要写入的数据地址
     * @param value 要写入的实际数据
     * @param length 指定的字符串长度，必须大于0
     * @return 写入结果
     */
    public OperateResult WriteUnicodeString(String address, String value, int length) {
        byte[] temp = Utilities.string2Byte(value);
        temp = SoftBasic.ArrayExpandToLength(temp, length * 2);
        return Write(address, temp);
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
                    0x03,  // 01 RFC1006 Header
                    0x00,  // 02 通常为 0
                    0x00,  // 03 数据长度，高位
                    0x16,  // 04 数据长度，地位
                    0x11,  // 05 连接类型0x11:tcp  0x12 ISO-on-TCP
                    (byte) 0xE0,  // 06 主动建立连接
                    0x00,  // 07 本地接口ID
                    0x00,  // 08 主动连接时为0
                    0x00,  // 09 该参数未使用
                    0x01,  // 10
                    0x00,  // 11
                    (byte) 0xC0,  // 12
                    0x01,  // 13
                    0x0A,  // 14
                    (byte) 0xC1,  // 15
                    0x02,  // 16
                    0x01,  // 17
                    0x02,  // 18
                    (byte) 0xC2,  // 19 指示cpu
                    0x02,  // 20
                    0x01,  // 21
                    0x00   // 22
            };
    private byte[] plcHead2 = new byte[]
            {
                    0x03,
                    0x00,
                    0x00,
                    0x19,
                    0x02,
                    (byte) 0xF0,
                    (byte) 0x80,
                    0x32,
                    0x01,
                    0x00,
                    0x00,
                    0x04,
                    0x00,
                    0x00,
                    0x08,
                    0x00,
                    0x00,
                    (byte) 0xF0,  // 设置通讯
                    0x00,
                    0x00,
                    0x01,
                    0x00,
                    0x01,
                    0x01,
                    (byte) 0xE0
            };
    private byte[] plcOrderNumber = new byte[]
            {
                    0x03,
                    0x00,
                    0x00,
                    0x21,
                    0x02,
                    (byte) 0xF0,
                    (byte) 0x80,
                    0x32,
                    0x07,
                    0x00,
                    0x00,
                    0x00,
                    0x01,
                    0x00,
                    0x08,
                    0x00,
                    0x08,
                    0x00,
                    0x01,
                    0x12,
                    0x04,
                    0x11,
                    0x44,
                    0x01,
                    0x00,
                    (byte) 0xFF,
                    0x09,
                    0x00,
                    0x04,
                    0x00,
                    0x11,
                    0x00,
                    0x00
            };
    private SiemensPLCS CurrentPlc = SiemensPLCS.S1200;
    private byte[] plcHead1_200smart = new byte[]
            {
                    0x03,  // 01 RFC1006 Header
                    0x00,  // 02 通常为 0
                    0x00,  // 03 数据长度，高位
                    0x16,  // 04 数据长度，地位
                    0x11,  // 05 连接类型0x11:tcp  0x12 ISO-on-TCP
                    (byte) 0xE0,  // 06 主动建立连接
                    0x00,  // 07 本地接口ID
                    0x00,  // 08 主动连接时为0
                    0x00,  // 09 该参数未使用
                    0x01,  // 10
                    0x00,  // 11
                    (byte) 0xC1,  // 12
                    0x02,  // 13
                    0x10,  // 14
                    0x00,  // 15
                    (byte) 0xC2,  // 16
                    0x02,  // 17
                    0x03,  // 18
                    0x00,  // 19 指示cpu
                    (byte) 0xC0,  // 20
                    0x01,  // 21
                    0x0A   // 22
            };
    private byte[] plcHead2_200smart = new byte[]
            {
                    0x03,
                    0x00,
                    0x00,
                    0x19,
                    0x02,
                    (byte) 0xF0,
                    (byte) 0x80,
                    0x32,
                    0x01,
                    0x00,
                    0x00,
                    (byte) 0xCC,
                    (byte) 0xC1,
                    0x00,
                    0x08,
                    0x00,
                    0x00,
                    (byte) 0xF0,  // 设置通讯
                    0x00,
                    0x00,
                    0x01,
                    0x00,
                    0x01,
                    0x03,
                    (byte) 0xC0
            };


    /**
     * 返回表示当前对象的字符串
     *
     * @return 字符串信息
     */
    @Override
    public String toString() {
        return "SiemensS7Net";
    }

}
