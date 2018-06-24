package HslCommunication.Profinet.Omron;

import HslCommunication.BasicFramework.SoftBasic;
import HslCommunication.Core.IMessage.FinsMessage;
import HslCommunication.Core.Net.NetworkBase.NetworkDeviceBase;
import HslCommunication.Core.Transfer.ReverseWordTransform;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.Core.Types.OperateResultExTwo;
import HslCommunication.Utilities;

import java.net.Socket;

/**
 * 欧姆龙Fins帧协议通讯类
 */
public class OmronFinsNet extends NetworkDeviceBase<FinsMessage,ReverseWordTransform> {
    /**
     * 实例化一个欧姆龙Fins帧协议的通讯对象
     */
    public OmronFinsNet() {
        WordLength = 1;
    }


    /**
     * 实例化一个欧姆龙Fins帧协议的通讯对象
     * @param ipAddress PLCd的Ip地址
     * @param port PLC的端口
     */
    public OmronFinsNet(String ipAddress, int port) {
        WordLength = 1;
        setIpAddress(ipAddress);
        setPort(port);
    }


    /**
     * 信息控制字段，默认0x80
     */
    public byte ICF = (byte) 0x80;

    /**
     * 系统使用的内部信息
     */
    public byte RSV = 0x00;

    /**
     * 网络层信息，默认0x02，如果有八层消息，就设置为0x07
     */
    public byte GCT = 0x02;

    /**
     * PLC的网络号地址，默认0x00
     */
    public byte DNA = 0x00;


    /**
     * PLC的节点地址，默认0x13
     */
    public byte DA1 = 0x13;

    /**
     * PLC的单元号地址
     */
    public byte DA2 = 0x00;

    /**
     * 上位机的网络号地址
     */
    public byte SNA = 0x00;


    private byte computerSA1 = 0x0B;

    /**
     * 上位机的节点地址，默认0x0B
     *
     * @return byte数据
     */
    public byte getSA1() {
        return computerSA1;
    }

    /**
     * 设置上位机的节点地址，默认0x0B
     *
     * @param computerSA1
     */
    public void setSA1(byte computerSA1) {
        this.computerSA1 = computerSA1;
        handSingle[19] = computerSA1;
    }


    /**
     * 上位机的单元号地址
     */
    public byte SA2 = 0x00;

    /**
     * 设备的标识号
     */
    public byte SID = 0x00;


    /**
     * 解析数据地址，Omron手册第188页
     * @param address 数据地址
     * @param isBit 是否是位地址
     * @return 结果类对象
     */
    private OperateResultExTwo<OmronFinsDataType, byte[]> AnalysisAddress(String address, boolean isBit) {
        OperateResultExTwo<OmronFinsDataType, byte[]> result = new OperateResultExTwo<OmronFinsDataType, byte[]>();
        try {
            switch (address.charAt(0)) {
                case 'D':
                case 'd': {
                    // DM区数据
                    result.Content1 = OmronFinsDataType.DM;
                    break;
                }
                case 'C':
                case 'c': {
                    // CIO区数据
                    result.Content1 = OmronFinsDataType.CIO;
                    break;
                }
                case 'W':
                case 'w': {
                    // WR区
                    result.Content1 = OmronFinsDataType.WR;
                    break;
                }
                case 'H':
                case 'h': {
                    // HR区
                    result.Content1 = OmronFinsDataType.HR;
                    break;
                }
                case 'A':
                case 'a': {
                    // AR区
                    result.Content1 = OmronFinsDataType.AR;
                    break;
                }
                default:
                    throw new Exception("输入的类型不支持，请重新输入");
            }

            if (isBit) {
                // 位操作
                String[] splits = address.substring(1).split("\\.");
                int addr = Integer.parseInt(splits[0]);
                result.Content2 = new byte[3];
                result.Content2[0] = Utilities.getBytes(addr)[1];
                result.Content2[1] = Utilities.getBytes(addr)[0];

                if (splits.length > 1) {
                    result.Content2[2] = Byte.parseByte(splits[1]);
                    if (result.Content2[2] > 15) {
                        throw new Exception("输入的位地址只能在0-15之间。");
                    }
                }
            } else {
                // 字操作
                int addr = Integer.parseInt(address.substring(1));
                result.Content2 = new byte[3];
                result.Content2[0] = Utilities.getBytes(addr)[1];
                result.Content2[1] = Utilities.getBytes(addr)[0];
            }
        } catch (Exception ex) {
            result.Message = "地址格式填写错误：" + ex.getMessage();
            return result;
        }

        result.IsSuccess = true;
        return result;
    }


    private OperateResultExOne<byte[]> ResponseValidAnalysis(byte[] response, boolean isRead) {
        // 数据有效性分析
        if (response.length >= 16) {
            // 提取错误码
            byte[] buffer = new byte[4];
            buffer[0] = response[15];
            buffer[1] = response[14];
            buffer[2] = response[13];
            buffer[3] = response[12];
            int err = Utilities.getInt(buffer, 0);
            if (err > 0) {
                OperateResultExOne<byte[]> result = new OperateResultExOne<>();
                result.Message = OmronInfo.GetStatusDescription(err);
                result.ErrorCode = err;
                return result;
            }

            if (response.length >= 30) {
                err = response[28] * 256 + response[29];
                if (err > 0) {
                    OperateResultExOne<byte[]> result = new OperateResultExOne<>();
                    result.Message = "结束码错误，为：" + err;
                    result.ErrorCode = err;
                    return result;
                }

                if (!isRead) {
                    // 写入操作
                    return OperateResultExOne.CreateSuccessResult(new byte[0]);
                } else {
                    // 读取操作
                    byte[] content = new byte[response.length - 30];
                    if (content.length > 0) {
                        System.arraycopy(response, 30, content, 0, content.length);
                    }
                    return OperateResultExOne.CreateSuccessResult(content);
                }
            }
        }

        OperateResultExOne<byte[]> result = new OperateResultExOne<>();
        result.Message = "数据长度接收错误";
        return result;
    }


    /**
     * 将普通的指令打包成完整的指令
     * @param cmd 指令
     * @return 字节
     */
    private byte[] PackCommand(byte[] cmd) {
        byte[] buffer = new byte[26 + cmd.length];
        System.arraycopy(handSingle, 0, buffer, 0, 4);

        byte[] tmp = Utilities.getBytes(buffer.length - 8);
        Utilities.bytesReverse(tmp);    // 翻转数组

        System.arraycopy(tmp, 0, buffer, 4, tmp.length);
        buffer[11] = 0x02;

        buffer[16] = ICF;
        buffer[17] = RSV;
        buffer[18] = GCT;
        buffer[19] = DNA;
        buffer[20] = DA1;
        buffer[21] = DA2;
        buffer[22] = SNA;
        buffer[23] = getSA1();
        buffer[24] = SA2;
        buffer[25] = SID;
        System.arraycopy(cmd, 0, buffer, 26, cmd.length);

        return buffer;
    }



    /**
     * 根据类型地址长度确认需要读取的指令头
     * @param address 起始地址
     * @param length 长度
     * @param isBit 是否是位读取
     * @return 带有成功标志的指令数据
     */
    private OperateResultExOne<byte[]> BuildReadCommand(String address, int length, boolean isBit) {
        OperateResultExOne<byte[]> result = new OperateResultExOne<byte[]>();
        OperateResultExTwo<OmronFinsDataType, byte[]> analysis = AnalysisAddress(address, isBit);
        if (!analysis.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(analysis);

        byte[] _PLCCommand = new byte[8];
        _PLCCommand[0] = 0x01;    // 读取存储区数据
        _PLCCommand[1] = 0x01;
        if (isBit) {
            _PLCCommand[2] = analysis.Content1.getBitCode();
        } else {
            _PLCCommand[2] = analysis.Content1.getWordCode();
        }

        System.arraycopy(analysis.Content2, 0, _PLCCommand, 3, analysis.Content2.length);
        _PLCCommand[6] = (byte) (length / 256);                       // 长度
        _PLCCommand[7] = (byte) (length % 256);

        try {
            result.Content = PackCommand(_PLCCommand);
            result.IsSuccess = true;
        } catch (Exception ex) {
            if (LogNet != null) LogNet.WriteException(toString(), ex);
            result.Message = ex.getMessage();
        }
        return result;
    }


    /**
     *
     * @param address 根据类型地址以及需要写入的数据来生成指令头
     * @param value 起始地址
     * @param isBit 是否是位操作
     * @return 结果
     */
    private OperateResultExOne<byte[]> BuildWriteCommand(String address, byte[] value, boolean isBit) {
        OperateResultExOne<byte[]> result = new OperateResultExOne<byte[]>();
        OperateResultExTwo<OmronFinsDataType, byte[]> analysis = AnalysisAddress(address, isBit);
        if (!analysis.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(analysis);

        byte[] _PLCCommand = new byte[8 + value.length];
        _PLCCommand[0] = 0x01;    // 读取存储区数据
        _PLCCommand[1] = 0x02;
        if (isBit) {
            _PLCCommand[2] = analysis.Content1.getBitCode();
        } else {
            _PLCCommand[2] = analysis.Content1.getWordCode();
        }

        System.arraycopy(analysis.Content2, 0, _PLCCommand, 3, analysis.Content2.length);

        if (isBit) {
            _PLCCommand[6] = (byte) (value.length / 256);                       // 长度
            _PLCCommand[7] = (byte) (value.length % 256);
        } else {
            _PLCCommand[6] = (byte) (value.length / 2 / 256);                       // 长度
            _PLCCommand[7] = (byte) (value.length / 2 % 256);
        }

        System.arraycopy(value, 0, _PLCCommand, 8, value.length);


        try {
            result.Content = PackCommand(_PLCCommand);
            result.IsSuccess = true;
        } catch (Exception ex) {
            if (LogNet != null) LogNet.WriteException(toString(), ex);
            result.Message = ex.getMessage();
        }
        return result;
    }


    /**
     * 在连接上欧姆龙PLC后，需要进行一步握手协议
     * @param socket 网络套接字
     * @return 结果对象
     */
    @Override
    protected OperateResult InitializationOnConnect(Socket socket) {
        // handSingle就是握手信号字节
        OperateResultExTwo<byte[], byte[]> read = ReadFromCoreServerBase(socket, handSingle);
        if (!read.IsSuccess) return read;

        // 检查返回的状态
        byte[] buffer = new byte[4];
        buffer[0] = read.Content2[7];
        buffer[1] = read.Content2[6];
        buffer[2] = read.Content2[5];
        buffer[3] = read.Content2[4];
        int status = Utilities.getInt(buffer, 0);
        if (status != 0) {
            return OperateResult.CreateFailedResult(status, "初始化失败，具体原因请根据错误码查找");
        }

        // 提取PLC的节点地址
        if (read.Content2.length >= 16) {
            DA1 = read.Content2[15];
        }
        return OperateResult.CreateSuccessResult();
    }


    /**
     * 从欧姆龙PLC中读取想要的数据，返回读取结果，读取单位为字
     * @param address 读取地址，格式为"D100","C100","W100","H100","A100"
     * @param length 读取的数据长度，字最大值960，位最大值7168
     * @return 带成功标志的结果数据对象
     */
    @Override
    public OperateResultExOne<byte[]> Read(String address, short length) {
        //获取指令
        OperateResultExOne<byte[]> command = BuildReadCommand(address, length, false);
        if (!command.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(command);

        // 核心数据交互
        OperateResultExOne<byte[]> read = ReadFromCoreServer(command.Content);
        if (!read.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(read);

        // 数据有效性分析
        OperateResultExOne<byte[]> valid = ResponseValidAnalysis(read.Content, true);
        if (!valid.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(valid);

        // 读取到了正确的数据
        return OperateResultExOne.CreateSuccessResult(valid.Content);
    }



    /**
     * 从欧姆龙PLC中批量读取位软元件，返回读取结果
     * @param address 读取地址，格式为"D100","C100","W100","H100","A100"
     * @param length 读取的长度
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<boolean[]> ReadBool(String address, short length) {
        //获取指令
        OperateResultExOne<byte[]> command = BuildReadCommand(address, length, true);
        if (!command.IsSuccess) return OperateResultExOne.<boolean[]>CreateFailedResult(command);

        // 核心数据交互
        OperateResultExOne<byte[]> read = ReadFromCoreServer(command.Content);
        if (!read.IsSuccess) return OperateResultExOne.<boolean[]>CreateFailedResult(read);

        // 数据有效性分析
        OperateResultExOne<byte[]> valid = ResponseValidAnalysis(read.Content, true);
        if (!valid.IsSuccess) return OperateResultExOne.<boolean[]>CreateFailedResult(valid);

        // 返回正确的数据信息
        boolean[] buffer = new boolean[valid.Content.length];
        for (int i = 0; i < valid.Content.length; i++) {
            buffer[i] = valid.Content[i] != 0x00;
        }
        return OperateResultExOne.CreateSuccessResult(buffer);
    }



    /**
     * 从欧姆龙PLC中批量读取位软元件，返回读取结果
     * @param address 读取地址，格式为"D100.0","C100.15","W100.7","H100.4","A100.9"
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<Boolean> ReadBool(String address) {
        OperateResultExOne<boolean[]> read = ReadBool(address, (short) 1);
        if (read.IsSuccess) {
            return OperateResultExOne.CreateSuccessResult(read.Content[0]);
        } else {
            return OperateResultExOne.<Boolean>CreateFailedResult(read);
        }
    }

    /**
     * 向PLC写入数据，数据格式为原始的字节类型
     * @param address 起始地址
     * @param value 原始数据
     * @return 结果
     */
    @Override
    public OperateResult Write(String address, byte[] value) {
        //获取指令
        OperateResultExOne<byte[]> command = BuildWriteCommand(address, value, false);
        if (!command.IsSuccess) return command;

        // 核心数据交互
        OperateResultExOne<byte[]> read = ReadFromCoreServer(command.Content);
        if (!read.IsSuccess) return read;

        // 数据有效性分析
        OperateResultExOne<byte[]> valid = ResponseValidAnalysis(read.Content, false);
        if (!valid.IsSuccess) return valid;

        // 成功
        return OperateResult.CreateSuccessResult();
    }

    /**
     * 向PLC中字软元件写入指定长度的字符串,超出截断，不够补0，编码格式为ASCII
     * @param address 要写入的数据地址
     * @param value 要写入的实际数据
     * @param length 指定的字符串长度，必须大于0
     * @return 返回读取结果
     */
    public OperateResult Write(String address, String value, int length) {
        byte[] temp = getByteTransform().TransByte(value, "ASCII");
        temp = SoftBasic.ArrayExpandToLength(temp, length);
        temp = SoftBasic.ArrayExpandToLengthEven(temp);
        return Write(address, temp);
    }


    /**
     * 向PLC中字软元件写入字符串，编码格式为Unicode
     * @param address 要写入的数据地址
     * @param value 要写入的实际数据
     * @return 返回读取结果
     */
    public OperateResult WriteUnicodeString(String address, String value) {
        byte[] temp = Utilities.string2Byte(value);
        return Write(address, temp);
    }

    /**
     * 向PLC中字软元件写入指定长度的字符串,超出截断，不够补0，编码格式为Unicode
     * @param address 要写入的数据地址
     * @param value 要写入的实际数据
     * @param length 指定的字符串长度，必须大于0
     * @return 返回读取结果
     */
    public OperateResult WriteUnicodeString(String address, String value, int length) {
        byte[] temp = Utilities.string2Byte(value);
        temp = SoftBasic.ArrayExpandToLength(temp, length * 2);
        return Write(address, temp);
    }



    /**
     * 向PLC中位软元件写入bool数组，返回值说明，比如你写入D100,values[0]对应D100.0
     * @param address 要写入的数据地址
     * @param value 要写入的实际数据，长度为8的倍数
     * @return 返回写入结果
     */
    public OperateResult Write(String address, boolean value) {
        return Write(address, new boolean[]{value});
    }


    /**
     * 向PLC中位软元件写入bool数组，返回值说明，比如你写入D100,values[0]对应D100.0
     * @param address 要写入的数据地址
     * @param values 要写入的实际数据，可以指定任意的长度
     * @return 返回写入结果
     */
    public OperateResult Write(String address, boolean[] values) {
        OperateResult result = new OperateResult();

        byte[] buffer = new byte[values.length];
        for (int i = 0; i < buffer.length; i++) {
            buffer[i] = values[i] ? (byte) 0x01 : (byte) 0x00;
        }

        //获取指令
        OperateResultExOne<byte[]> command = BuildWriteCommand(address, buffer, true);
        if (!command.IsSuccess) return command;

        // 核心数据交互
        OperateResultExOne<byte[]> read = ReadFromCoreServer(command.Content);
        if (!read.IsSuccess) return read;

        // 数据有效性分析
        OperateResultExOne<byte[]> valid = ResponseValidAnalysis(read.Content, false);
        if (!valid.IsSuccess) return valid;

        // 写入成功
        return OperateResult.CreateSuccessResult();
    }


    // 握手信号
    // 46494E530000000C0000000000000000000000D6
    private final byte[] handSingle = new byte[]
            {
                    0x46, 0x49, 0x4E, 0x53, // FINS
                    0x00, 0x00, 0x00, 0x0C, // 后面的命令长度
                    0x00, 0x00, 0x00, 0x00, // 命令码
                    0x00, 0x00, 0x00, 0x00, // 错误码
                    0x00, 0x00, 0x00, 0x01  // 节点号
            };


    /**
     * 返回表示当前对象的字符串
     *
     * @return 字符串
     */
    @Override
    public String toString() {
        return "OmronFinsNet";
    }

}
