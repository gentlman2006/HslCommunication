package HslCommunication.Profinet.Melsec;

import HslCommunication.BasicFramework.SoftBasic;
import HslCommunication.Core.IMessage.MelsecQnA3EAsciiMessage;
import HslCommunication.Core.Net.NetworkBase.NetworkDeviceBase;
import HslCommunication.Core.Transfer.RegularByteTransform;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.Core.Types.OperateResultExTwo;
import HslCommunication.Utilities;


/**
 * 三菱PLC通讯类，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为ASCII通讯格式
 */
public class MelsecMcAsciiNet extends NetworkDeviceBase<MelsecQnA3EAsciiMessage, RegularByteTransform> {

    /**
     * 实例化三菱的Qna兼容3E帧协议的通讯对象
     */
    public MelsecMcAsciiNet() {
        WordLength = 1;
    }


    /**
     * 实例化一个三菱的Qna兼容3E帧协议的通讯对象
     * @param ipAddress PLC的Ip地址
     * @param port PLC的端口
     */
    public MelsecMcAsciiNet(String ipAddress, int port) {
        WordLength = 1;
        setIpAddress(ipAddress);
        setPort(port);
    }


    private byte NetworkNumber = 0x00;                       // 网络号
    private byte NetworkStationNumber = 0x00;                // 网络站号

    /**
     * 获取网络号
     *
     * @return
     */
    public byte getNetworkNumber() {
        return NetworkNumber;
    }

    /**
     * 设置网络号
     *
     * @param networkNumber
     */
    public void setNetworkNumber(byte networkNumber) {
        NetworkNumber = networkNumber;
    }

    /**
     * 获取网络站号
     *
     * @return
     */
    public byte getNetworkStationNumber() {
        return NetworkStationNumber;
    }

    /**
     * 设置网络站号
     *
     * @param networkStationNumber
     */
    public void setNetworkStationNumber(byte networkStationNumber) {
        NetworkStationNumber = networkStationNumber;
    }


    /**
     * 解析数据地址
     * @param address 数据地址
     * @return 解析后的地址
     */
    private OperateResultExTwo<MelsecMcDataType, Integer> AnalysisAddress(String address) {
        OperateResultExTwo<MelsecMcDataType, Integer> result = new OperateResultExTwo<MelsecMcDataType, Integer>();
        try {
            switch (address.charAt(0)) {
                case 'M':
                case 'm': {
                    result.Content1 = MelsecMcDataType.M;
                    result.Content2 = Integer.parseInt(address.substring(1), MelsecMcDataType.M.getFromBase());
                    break;
                }
                case 'X':
                case 'x': {
                    result.Content1 = MelsecMcDataType.X;
                    result.Content2 = Integer.parseInt(address.substring(1), MelsecMcDataType.X.getFromBase());
                    break;
                }
                case 'Y':
                case 'y': {
                    result.Content1 = MelsecMcDataType.Y;
                    result.Content2 = Integer.parseInt(address.substring(1), MelsecMcDataType.Y.getFromBase());
                    break;
                }
                case 'D':
                case 'd': {
                    result.Content1 = MelsecMcDataType.D;
                    result.Content2 = Integer.parseInt(address.substring(1), MelsecMcDataType.D.getFromBase());
                    break;
                }
                case 'W':
                case 'w': {
                    result.Content1 = MelsecMcDataType.W;
                    result.Content2 = Integer.parseInt(address.substring(1), MelsecMcDataType.W.getFromBase());
                    break;
                }
                case 'L':
                case 'l': {
                    result.Content1 = MelsecMcDataType.L;
                    result.Content2 = Integer.parseInt(address.substring(1), MelsecMcDataType.L.getFromBase());
                    break;
                }
                case 'F':
                case 'f': {
                    result.Content1 = MelsecMcDataType.F;
                    result.Content2 = Integer.parseInt(address.substring(1), MelsecMcDataType.F.getFromBase());
                    break;
                }
                case 'V':
                case 'v': {
                    result.Content1 = MelsecMcDataType.V;
                    result.Content2 = Integer.parseInt(address.substring(1), MelsecMcDataType.V.getFromBase());
                    break;
                }
                case 'B':
                case 'b': {
                    result.Content1 = MelsecMcDataType.B;
                    result.Content2 = Integer.parseInt(address.substring(1), MelsecMcDataType.B.getFromBase());
                    break;
                }
                case 'R':
                case 'r': {
                    result.Content1 = MelsecMcDataType.R;
                    result.Content2 = Integer.parseInt(address.substring(1), MelsecMcDataType.R.getFromBase());
                    break;
                }
                case 'S':
                case 's': {
                    result.Content1 = MelsecMcDataType.S;
                    result.Content2 = Integer.parseInt(address.substring(1), MelsecMcDataType.S.getFromBase());
                    break;
                }
                case 'Z':
                case 'z': {
                    result.Content1 = MelsecMcDataType.Z;
                    result.Content2 = Integer.parseInt(address.substring(1), MelsecMcDataType.Z.getFromBase());
                    break;
                }

                case 'T':
                case 't': {
                    result.Content1 = MelsecMcDataType.T;
                    result.Content2 = Integer.parseInt(address.substring(1), MelsecMcDataType.T.getFromBase());
                    break;
                }
                case 'C':
                case 'c': {
                    result.Content1 = MelsecMcDataType.C;
                    result.Content2 = Integer.parseInt(address.substring(1), MelsecMcDataType.C.getFromBase());
                    break;
                }
                default:
                    throw new Exception("输入的类型不支持，请重新输入");
            }
        } catch (Exception ex) {
            result.Message = "地址格式填写错误：" + ex.getMessage();
            return result;
        }

        result.IsSuccess = true;
        return result;
    }


    private byte[] BuildBytesFromData(byte value) {
        return Utilities.getBytes(String.format("%02x",value),"ASCII");
    }

    private byte[] BuildBytesFromData(short value) {
        return Utilities.getBytes(String.format("%04x",value),"ASCII");
    }

    private byte[] BuildBytesFromData(int value) {
        return Utilities.getBytes(String.format("%04x",value),"ASCII");
    }

    private byte[] BuildBytesFromAddress(int address, MelsecMcDataType type) {
        return Utilities.getBytes(String.format(type.getFromBase() == 10 ? "%06d" : "%06x",address),"ASCII");
    }



    /**
     * 根据类型地址长度确认需要读取的指令头
     * @param address 起始地址
     * @param length 长度
     * @return 带有成功标志的指令数据
     */
    private OperateResultExTwo<MelsecMcDataType, byte[]> BuildReadCommand(String address, Integer length) {
        OperateResultExTwo<MelsecMcDataType, byte[]> result = new OperateResultExTwo<>();
        OperateResultExTwo<MelsecMcDataType, Integer> analysis = AnalysisAddress(address);
        if (!analysis.IsSuccess) {
            result.CopyErrorFromOther(analysis);
            return result;
        }

        // 默认信息----注意：高低字节交错

        try {
            byte[] _PLCCommand = new byte[42];
            _PLCCommand[0] = 0x35;                                      // 副标题
            _PLCCommand[1] = 0x30;
            _PLCCommand[2] = 0x30;
            _PLCCommand[3] = 0x30;
            _PLCCommand[4] = BuildBytesFromData(NetworkNumber)[0];                // 网络号
            _PLCCommand[5] = BuildBytesFromData(NetworkNumber)[1];
            _PLCCommand[6] = 0x46;                         // PLC编号
            _PLCCommand[7] = 0x46;
            _PLCCommand[8] = 0x30;                         // 目标模块IO编号
            _PLCCommand[9] = 0x33;
            _PLCCommand[10] = 0x46;
            _PLCCommand[11] = 0x46;
            _PLCCommand[12] = BuildBytesFromData(NetworkStationNumber)[0];         // 目标模块站号
            _PLCCommand[13] = BuildBytesFromData(NetworkStationNumber)[1];
            _PLCCommand[14] = 0x30;                         // 请求数据长度
            _PLCCommand[15] = 0x30;
            _PLCCommand[16] = 0x31;
            _PLCCommand[17] = 0x38;
            _PLCCommand[18] = 0x30;                         // CPU监视定时器
            _PLCCommand[19] = 0x30;
            _PLCCommand[20] = 0x31;
            _PLCCommand[21] = 0x30;
            _PLCCommand[22] = 0x30;                        // 批量读取数据命令
            _PLCCommand[23] = 0x34;
            _PLCCommand[24] = 0x30;
            _PLCCommand[25] = 0x31;
            _PLCCommand[26] = 0x30;                         // 以点为单位还是字为单位成批读取
            _PLCCommand[27] = 0x30;
            _PLCCommand[28] = 0x30;
            _PLCCommand[29] = analysis.Content1.getDataType() == 0 ? (byte) 0x30 : (byte) 0x31;
            _PLCCommand[30] = (analysis.Content1.getAsciiCode().getBytes("ASCII"))[0];            // 软元件类型
            _PLCCommand[31] = (analysis.Content1.getAsciiCode().getBytes("ASCII"))[1];
            _PLCCommand[32] = BuildBytesFromAddress(analysis.Content2, analysis.Content1)[0];                   // 起始地址的地位
            _PLCCommand[33] = BuildBytesFromAddress(analysis.Content2, analysis.Content1)[1];
            _PLCCommand[34] = BuildBytesFromAddress(analysis.Content2, analysis.Content1)[2];
            _PLCCommand[35] = BuildBytesFromAddress(analysis.Content2, analysis.Content1)[3];
            _PLCCommand[36] = BuildBytesFromAddress(analysis.Content2, analysis.Content1)[4];
            _PLCCommand[37] = BuildBytesFromAddress(analysis.Content2, analysis.Content1)[5];
            _PLCCommand[38] = BuildBytesFromData(length)[0];                                                      // 软元件点数
            _PLCCommand[39] = BuildBytesFromData(length)[1];
            _PLCCommand[40] = BuildBytesFromData(length)[2];
            _PLCCommand[41] = BuildBytesFromData(length)[3];
            result.Content1 = analysis.Content1;
            result.Content2 = _PLCCommand;
            result.IsSuccess = true;
        }
        catch (Exception ex){
            result.Message = ex.getMessage();
        }
        return result;
    }


    /**
     * 根据类型地址以及需要写入的数据来生成指令头
     * @param address 起始地址
     * @param value 实际的数据
     * @return 命令数据
     */
    private OperateResultExTwo<MelsecMcDataType, byte[]> BuildWriteCommand(String address, byte[] value) {
        OperateResultExTwo<MelsecMcDataType, byte[]> result = new OperateResultExTwo<MelsecMcDataType, byte[]>();
        OperateResultExTwo<MelsecMcDataType, Integer> analysis = AnalysisAddress(address);
        if (!analysis.IsSuccess) {
            result.CopyErrorFromOther(analysis);
            return result;
        }

        // 默认信息----注意：高低字节交错

        byte[] _PLCCommand = new byte[42 + value.length];

        try {
        _PLCCommand[0] = 0x35;                                      // 副标题
        _PLCCommand[1] = 0x30;
        _PLCCommand[2] = 0x30;
        _PLCCommand[3] = 0x30;
        _PLCCommand[4] = BuildBytesFromData(NetworkNumber)[0];                // 网络号
        _PLCCommand[5] = BuildBytesFromData(NetworkNumber)[1];
        _PLCCommand[6] = 0x46;                         // PLC编号
        _PLCCommand[7] = 0x46;
        _PLCCommand[8] = 0x30;                         // 目标模块IO编号
        _PLCCommand[9] = 0x33;
        _PLCCommand[10] = 0x46;
        _PLCCommand[11] = 0x46;
        _PLCCommand[12] = BuildBytesFromData(NetworkStationNumber)[0];         // 目标模块站号
        _PLCCommand[13] = BuildBytesFromData(NetworkStationNumber)[1];
        _PLCCommand[14] = BuildBytesFromData((_PLCCommand.length - 18))[0]; // 请求数据长度
        _PLCCommand[15] = BuildBytesFromData((_PLCCommand.length - 18))[1];
        _PLCCommand[16] = BuildBytesFromData((_PLCCommand.length - 18))[2];
        _PLCCommand[17] = BuildBytesFromData((_PLCCommand.length - 18))[3];
        _PLCCommand[18] = 0x30; // CPU监视定时器
        _PLCCommand[19] = 0x30;
        _PLCCommand[20] = 0x31;
        _PLCCommand[21] = 0x30;
        _PLCCommand[22] = 0x31; // 批量写入的命令
        _PLCCommand[23] = 0x34;
        _PLCCommand[24] = 0x30;
        _PLCCommand[25] = 0x31;
        _PLCCommand[26] = 0x30; // 子命令
        _PLCCommand[27] = 0x30;
        _PLCCommand[28] = 0x30;
        _PLCCommand[29] = analysis.Content1.getDataType() == 0 ? (byte) 0x30 : (byte) 0x31;
        _PLCCommand[30] = (analysis.Content1.getAsciiCode().getBytes("ASCII"))[0];                          // 软元件类型
        _PLCCommand[31] = (analysis.Content1.getAsciiCode().getBytes("ASCII"))[1];
        _PLCCommand[32] = BuildBytesFromAddress(analysis.Content2, analysis.Content1)[0];                   // 起始地址的地位
        _PLCCommand[33] = BuildBytesFromAddress(analysis.Content2, analysis.Content1)[1];
        _PLCCommand[34] = BuildBytesFromAddress(analysis.Content2, analysis.Content1)[2];
        _PLCCommand[35] = BuildBytesFromAddress(analysis.Content2, analysis.Content1)[3];
        _PLCCommand[36] = BuildBytesFromAddress(analysis.Content2, analysis.Content1)[4];
        _PLCCommand[37] = BuildBytesFromAddress(analysis.Content2, analysis.Content1)[5];

        // 判断是否进行位操作
        if (analysis.Content1.getDataType() == 1) {
            _PLCCommand[38] = BuildBytesFromData( value.length)[0];                                                      // 软元件点数
            _PLCCommand[39] = BuildBytesFromData( value.length)[1];
            _PLCCommand[40] = BuildBytesFromData( value.length)[2];
            _PLCCommand[41] = BuildBytesFromData( value.length)[3];
        } else {
            _PLCCommand[38] = BuildBytesFromData( (value.length / 4))[0];                                                      // 软元件点数
            _PLCCommand[39] = BuildBytesFromData( (value.length / 4))[1];
            _PLCCommand[40] = BuildBytesFromData( (value.length / 4))[2];
            _PLCCommand[41] = BuildBytesFromData( (value.length / 4))[3];
        }
        System.arraycopy(value,0,_PLCCommand,42,value.length);

        result.Content1 = analysis.Content1;
        result.Content2 = _PLCCommand;
        result.IsSuccess = true;

        }
        catch (Exception ex){
            result.Message = ex.getMessage();
        }

        return result;
    }



    /**
     * 从三菱PLC中读取想要的数据，返回读取结果
     * @param address 读取地址，格式为"M100","D100","W1A0"
     * @param length 读取的数据长度，字最大值960，位最大值7168
     * @return 带成功标志的结果数据对象
     */
    @Override
    public OperateResultExOne<byte[]> Read(String address, short length) {
        OperateResultExOne<byte[]> result = new OperateResultExOne<byte[]>();
        //获取指令
        OperateResultExTwo<MelsecMcDataType, byte[]> command = BuildReadCommand(address, (int) length);
        if (!command.IsSuccess) {
            result.CopyErrorFromOther(command);
            return result;
        }

        OperateResultExOne<byte[]> read = ReadFromCoreServer(command.Content2);
        if (read.IsSuccess) {
            byte[] buffer = new byte[4];
            buffer[0] = read.Content[18];
            buffer[1] = read.Content[19];
            buffer[2] = read.Content[20];
            buffer[3] = read.Content[21];
            result.ErrorCode = Integer.parseInt(Utilities.getString(buffer,"ASCII"), 16);
            if (result.ErrorCode == 0) {
                if (command.Content1.getDataType() == 0x01) {
                    result.Content = new byte[read.Content.length - 22];
                    for (int i = 22; i < read.Content.length; i++) {
                        if (read.Content[i] == 0x30) {
                            result.Content[i - 22] = 0x00;
                        } else {
                            result.Content[i - 22] = 0x01;
                        }
                    }
                } else {
                    result.Content = new byte[(read.Content.length - 22) / 2];
                    for (int i = 0; i < result.Content.length / 2; i++) {
                        buffer = new byte[4];
                        buffer[0] = read.Content[i * 4 + 22];
                        buffer[1] = read.Content[i * 4 + 23];
                        buffer[2] = read.Content[i * 4 + 24];
                        buffer[3] = read.Content[i * 4 + 25];

                        int tmp = Integer.parseInt(Utilities.getString(buffer,"ASCII"),16);
                        byte[] tmp2 = Utilities.getBytes(tmp);
                        System.arraycopy(tmp2,0,result.Content,i*2,2);
                    }
                }
                result.IsSuccess = true;
            } else {
                result.Message = "请翻查三菱通讯手册来查看具体的信息。";
            }
        } else {
            result.ErrorCode = read.ErrorCode;
            result.Message = read.Message;
        }

        return result;
    }



    /**
     * 从三菱PLC中批量读取位软元件，返回读取结果
     * @param address 起始地址
     * @param length 读取的长度
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<boolean[]> ReadBool(String address, short length) {
        OperateResultExOne<boolean[]> result = new OperateResultExOne<boolean[]>();
        OperateResultExTwo<MelsecMcDataType, Integer> analysis = AnalysisAddress(address);
        if (!analysis.IsSuccess) {
            result.CopyErrorFromOther(analysis);
            return result;
        } else {
            if (analysis.Content1.getDataType() == 0x00) {
                result.Message = "读取位变量数组只能针对位软元件，如果读取字软元件，请自行转化";
                return result;
            }
        }
        OperateResultExOne<byte[]> read = Read(address, length);
        if (!read.IsSuccess) {
            result.CopyErrorFromOther(read);
            return result;
        }

        result.Content = new boolean[read.Content.length];
        for (int i = 0; i < read.Content.length; i++) {
            result.Content[i] = read.Content[i] == 0x01;
        }
        result.IsSuccess = true;
        return result;
    }



    /**
     * 从三菱PLC中批量读取位软元件，返回读取结果
     * @param address 起始地址
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<Boolean> ReadBool(String address) {
        OperateResultExOne<boolean[]> read = ReadBool(address, (short) 1);
        if (!read.IsSuccess) return OperateResultExOne.<Boolean>CreateFailedResult(read);

        return OperateResultExOne.<Boolean>CreateSuccessResult(read.Content[0]);
    }


    /**
     * 向PLC写入数据，数据格式为原始的字节类型
     * @param address 起始地址
     * @param value 原始数据
     * @return 结果
     */
    @Override
    public OperateResult Write(String address, byte[] value) {
        // Console.WriteLine( BasicFramework.SoftBasic.ByteToHexString( value ) );

        OperateResultExOne<byte[]> result = new OperateResultExOne<byte[]>();

        //获取指令
        OperateResultExTwo<MelsecMcDataType, Integer> analysis = AnalysisAddress(address);
        if (!analysis.IsSuccess) {
            result.CopyErrorFromOther(analysis);
            return result;
        }

        OperateResultExTwo<MelsecMcDataType, byte[]> command;
        // 预处理指令
        if (analysis.Content1.getDataType() == 0x01) {
            byte[] buffer = new byte[value.length];

            for (int i = 0; i < buffer.length; i++) {
                if (value[i] == 0x00) {
                    buffer[i] = 0x30;
                } else {
                    buffer[i] = 0x31;
                }
            }

            // 位写入
            command = BuildWriteCommand(address, buffer);
        } else {
            // 字写入
            byte[] buffer = new byte[value.length * 2];
            for (int i = 0; i < value.length / 2; i++) {
                byte[] tmp = BuildBytesFromData(value[i*2+0]+value[i*2+1]*256);
                System.arraycopy(tmp,0,buffer,4*i,tmp.length);
            }

            command = BuildWriteCommand(address, buffer);
        }

        if (!command.IsSuccess) {
            result.CopyErrorFromOther(command);
            return result;
        }

        OperateResultExOne<byte[]> read = ReadFromCoreServer(command.Content2);
        if (read.IsSuccess) {
            byte[] buffer = new byte[4];
            buffer[0] = read.Content[18];
            buffer[1] = read.Content[19];
            buffer[2] = read.Content[20];
            buffer[3] = read.Content[21];
            result.ErrorCode = Integer.parseInt(Utilities.getString(buffer,"ASCII"), 16);
            if (result.ErrorCode == 0) {
                result.IsSuccess = true;
            }
        } else {
            result.ErrorCode = read.ErrorCode;
            result.Message = read.Message;
        }

        return result;
    }



    /**
     * 向PLC中字软元件写入指定长度的字符串,超出截断，不够补0，编码格式为ASCII
     * @param address 要写入的数据地址
     * @param value 要写入的实际数据
     * @param length 指定的字符串长度，必须大于0
     * @return 返回读取结果
     */
    public OperateResult Write(String address, String value, int length) {
        byte[] temp = Utilities.getBytes(value, "ASCII");
        temp = SoftBasic.ArrayExpandToLength(temp, length);
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
     * 向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
     * @param address 要写入的数据地址
     * @param value 要写入的实际数据，true 或者是 false
     * @return 返回写入结果
     */
    public OperateResult Write(String address, boolean value) {
        return Write(address, new boolean[]{value});
    }


    /**
     * 向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
     * @param address 要写入的数据地址
     * @param values 要写入的实际数据，可以指定任意的长度
     * @return 返回写入结果
     */
    public OperateResult Write(String address, boolean[] values) {
        byte[] buffer = new byte[values.length];
        for (int i = 0; i < values.length; i++) {
            if (values[i]) buffer[i] = 0x01;
        }
        return Write(address, buffer);
    }


    /**
     * 返回表示当前对象的字符串
     * @return 字符串
     */
    @Override
    public String toString() {
        return "MelsecMcNet";
    }
}
