package HslCommunication.Profinet.Melsec;

import HslCommunication.BasicFramework.SoftBasic;
import HslCommunication.Core.IMessage.MelsecQnA3EBinaryMessage;
import HslCommunication.Core.Net.NetworkBase.NetworkDeviceBase;
import HslCommunication.Core.Transfer.RegularByteTransform;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.Core.Types.OperateResultExTwo;
import HslCommunication.Utilities;

/**
 * 三菱的实际数据交互类
 */
public class MelsecMcNet extends NetworkDeviceBase<MelsecQnA3EBinaryMessage,RegularByteTransform> {


    /**
     * 实例化三菱的Qna兼容3E帧协议的通讯对象
     */
    public MelsecMcNet() {
        WordLength = 1;
    }


    /**
     * 实例化一个三菱的Qna兼容3E帧协议的通讯对象
     *
     * @param ipAddress PLCd的Ip地址
     * @param port      PLC的端口
     */
    public MelsecMcNet(String ipAddress, int port) {
        WordLength = 1;
        super.setIpAddress(ipAddress);
        super.setPort(port);
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
     * @return
     */
    private OperateResultExTwo<MelsecMcDataType, Short> AnalysisAddress(String address) {
        OperateResultExTwo<MelsecMcDataType, Short> result = new OperateResultExTwo<MelsecMcDataType, Short>();
        try {
            switch (address.charAt(0)) {
                case 'M':
                case 'm': {
                    result.Content1 = MelsecMcDataType.M;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.M.getFromBase());
                    break;
                }
                case 'X':
                case 'x': {
                    result.Content1 = MelsecMcDataType.X;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.X.getFromBase());
                    break;
                }
                case 'Y':
                case 'y': {
                    result.Content1 = MelsecMcDataType.Y;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.Y.getFromBase());
                    break;
                }
                case 'D':
                case 'd': {
                    result.Content1 = MelsecMcDataType.D;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.D.getFromBase());
                    break;
                }
                case 'W':
                case 'w': {
                    result.Content1 = MelsecMcDataType.W;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.W.getFromBase());
                    break;
                }
                case 'L':
                case 'l': {
                    result.Content1 = MelsecMcDataType.L;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.L.getFromBase());
                    break;
                }
                case 'F':
                case 'f': {
                    result.Content1 = MelsecMcDataType.F;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.F.getFromBase());
                    break;
                }
                case 'V':
                case 'v': {
                    result.Content1 = MelsecMcDataType.V;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.V.getFromBase());
                    break;
                }
                case 'B':
                case 'b': {
                    result.Content1 = MelsecMcDataType.B;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.B.getFromBase());
                    break;
                }
                case 'R':
                case 'r': {
                    result.Content1 = MelsecMcDataType.R;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.R.getFromBase());
                    break;
                }
                case 'S':
                case 's': {
                    result.Content1 = MelsecMcDataType.S;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.S.getFromBase());
                    break;
                }
                case 'Z':
                case 'z': {
                    result.Content1 = MelsecMcDataType.Z;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.Z.getFromBase());
                    break;
                }
                case 'T':
                case 't': {
                    result.Content1 = MelsecMcDataType.T;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.T.getFromBase());
                    break;
                }
                case 'C':
                case 'c': {
                    result.Content1 = MelsecMcDataType.C;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.C.getFromBase());
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



    /**
     * 根据类型地址长度确认需要读取的指令头
     * @param address 起始地址
     * @param length 长度
     * @return 带有成功标志的指令数据
     */
    private OperateResultExTwo<MelsecMcDataType, byte[]> BuildReadCommand(String address, short length) {
        OperateResultExTwo<MelsecMcDataType, byte[]> result = new OperateResultExTwo<MelsecMcDataType, byte[]>();
        OperateResultExTwo<MelsecMcDataType, Short> analysis = AnalysisAddress(address);
        if (!analysis.IsSuccess) {
            result.CopyErrorFromOther(analysis);
            return result;
        }

        // 默认信息----注意：高低字节交错

        byte[] _PLCCommand = new byte[21];
        _PLCCommand[0] = 0x50;                         // 副标题
        _PLCCommand[1] = 0x00;
        _PLCCommand[2] = NetworkNumber;                // 网络号
        _PLCCommand[3] = (byte) (0xFF);                         // PLC编号
        _PLCCommand[4] = (byte) (0xFF);
        ;                         // 目标模块IO编号
        _PLCCommand[5] = 0x03;
        _PLCCommand[6] = NetworkStationNumber;         // 目标模块站号
        _PLCCommand[7] = 0x0C;                         // 请求数据长度
        _PLCCommand[8] = 0x00;
        _PLCCommand[9] = 0x0A;                         // CPU监视定时器
        _PLCCommand[10] = 0x00;
        _PLCCommand[11] = 0x01;                        // 批量读取数据命令
        _PLCCommand[12] = 0x04;
        _PLCCommand[13] = analysis.Content1.getDataType();               // 以点为单位还是字为单位成批读取
        _PLCCommand[14] = 0x00;
        _PLCCommand[15] = (byte) (analysis.Content2 % 256);       // 起始地址的地位
        _PLCCommand[16] = (byte) (analysis.Content2 / 256);
        _PLCCommand[17] = 0x00;
        _PLCCommand[18] = analysis.Content1.getDataCode();               // 指明读取的数据
        _PLCCommand[19] = (byte) (length % 256);        // 软元件长度的地位
        _PLCCommand[20] = (byte) (length / 256);

        result.Content1 = analysis.Content1;
        result.Content2 = _PLCCommand;
        result.IsSuccess = true;
        return result;
    }


    /**
     * 根据类型地址以及需要写入的数据来生成指令头
     * @param address 起始地址
     * @param value 值
     * @param length 指定长度
     * @return 结果
     */
    private OperateResultExTwo<MelsecMcDataType, byte[]> BuildWriteCommand(String address, byte[] value, int length) {
        OperateResultExTwo<MelsecMcDataType, byte[]> result = new OperateResultExTwo<MelsecMcDataType, byte[]>();
        OperateResultExTwo<MelsecMcDataType, Short> analysis = AnalysisAddress(address);
        if (!analysis.IsSuccess) {
            result.CopyErrorFromOther(analysis);
            return result;
        }

        // 默认信息----注意：高低字节交错

        byte[] _PLCCommand = new byte[21 + value.length];

        _PLCCommand[0] = 0x50;                                          // 副标题
        _PLCCommand[1] = 0x00;
        _PLCCommand[2] = NetworkNumber;                                 // 网络号
        _PLCCommand[3] = (byte) (0xFF);                                          // PLC编号
        _PLCCommand[4] = (byte) (0xFF);                                          // 目标模块IO编号
        _PLCCommand[5] = 0x03;
        _PLCCommand[6] = NetworkStationNumber;                          // 目标模块站号

        _PLCCommand[7] = (byte) ((_PLCCommand.length - 9) % 256);        // 请求数据长度
        _PLCCommand[8] = (byte) ((_PLCCommand.length - 9) / 256);
        ;
        _PLCCommand[9] = 0x0A;                                          // CPU监视定时器
        _PLCCommand[10] = 0x00;
        _PLCCommand[11] = 0x01;                                         // 批量读取数据命令
        _PLCCommand[12] = 0x14;
        _PLCCommand[13] = analysis.Content1.getDataType();                   // 以点为单位还是字为单位成批读取
        _PLCCommand[14] = 0x00;
        _PLCCommand[15] = (byte) (analysis.Content2 % 256);
        ;            // 起始地址的地位
        _PLCCommand[16] = (byte) (analysis.Content2 / 256);
        _PLCCommand[17] = 0x00;
        _PLCCommand[18] = analysis.Content1.getDataCode();                   // 指明写入的数据

        // 判断是否进行位操作
        if (analysis.Content1.getDataType() == 1) {
            if (length > 0) {
                _PLCCommand[19] = (byte) (length % 256);                 // 软元件长度的地位
                _PLCCommand[20] = (byte) (length / 256);
            } else {
                _PLCCommand[19] = (byte) (value.length * 2 % 256);        // 软元件长度的地位
                _PLCCommand[20] = (byte) (value.length * 2 / 256);
            }
        } else {
            _PLCCommand[19] = (byte) (value.length / 2 % 256);            // 软元件长度的地位
            _PLCCommand[20] = (byte) (value.length / 2 / 256);
        }

        System.arraycopy(value, 0, _PLCCommand, 21, value.length);

        result.Content1 = analysis.Content1;
        result.Content2 = _PLCCommand;
        result.IsSuccess = true;
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
        OperateResultExTwo<MelsecMcDataType, byte[]> command = BuildReadCommand(address, length);
        if (!command.IsSuccess) {
            result.CopyErrorFromOther(command);
            return result;
        }

        OperateResultExOne<byte[]> read = ReadFromCoreServer(command.Content2);
        if (read.IsSuccess) {
            result.ErrorCode = Utilities.getShort(read.Content, 9);
            if (result.ErrorCode == 0) {
                if (command.Content1.getDataType() == 0x01) {
                    result.Content = new byte[(read.Content.length - 11) * 2];
                    for (int i = 11; i < read.Content.length; i++) {
                        if ((read.Content[i] & 0x10) == 0x10) {
                            result.Content[(i - 11) * 2 + 0] = 0x01;
                        }

                        if ((read.Content[i] & 0x01) == 0x01) {
                            result.Content[(i - 11) * 2 + 1] = 0x01;
                        }
                    }
                } else {
                    result.Content = new byte[read.Content.length - 11];
                    System.arraycopy(read.Content, 11, result.Content, 0, result.Content.length);
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
        OperateResultExTwo<MelsecMcDataType, Short> analysis = AnalysisAddress(address);
        if (!analysis.IsSuccess) {
            result.CopyErrorFromOther(analysis);
            return result;
        } else {
            if (analysis.Content1.getDataType() == 0x00) {
                result.Message = "读取位变量数组只能针对位软元件，如果读取字软元件，请调用Read方法";
                return result;
            }
        }
        OperateResultExOne<byte[]> read = Read(address, length);
        if (!read.IsSuccess){
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
        if (!read.IsSuccess){
            OperateResultExOne<Boolean> resultExOne = new OperateResultExOne<>();
            resultExOne.CopyErrorFromOther(read);
            return resultExOne;
        }

        return OperateResultExOne.CreateSuccessResult(read.Content[0]);
    }





    /**
     * 向PLC写入数据，数据格式为原始的字节类型
     * @param address 起始地址
     * @param value 原始数据
     * @return 结果
     */
    @Override
    public OperateResult Write(String address, byte[] value) {
        OperateResultExOne<byte[]> result = new OperateResultExOne<byte[]>();

        // 获取指令
        OperateResultExTwo<MelsecMcDataType, Short> analysis = AnalysisAddress(address);
        if (!analysis.IsSuccess) {
            result.CopyErrorFromOther(analysis);
            return result;
        }

        OperateResultExTwo<MelsecMcDataType, byte[]> command;
        // 预处理指令
        if (analysis.Content1.getDataType() == 0x01) {
            int length = value.length % 2 == 0 ? value.length / 2 : value.length / 2 + 1;
            byte[] buffer = new byte[length];

            for (int i = 0; i < length; i++) {
                if (value[i * 2 + 0] != 0x00) buffer[i] += 0x10;
                if ((i * 2 + 1) < value.length) {
                    if (value[i * 2 + 1] != 0x00) buffer[i] += 0x01;
                }
            }

            // 位写入
            command = BuildWriteCommand(address, buffer, value.length);
        } else {
            // 字写入
            command = BuildWriteCommand(address, value,-1);
        }

        if (!command.IsSuccess) {
            result.CopyErrorFromOther(command);
            return result;
        }

        OperateResultExOne<byte[]> read = ReadFromCoreServer(command.Content2);
        if (read.IsSuccess) {
            result.ErrorCode = Utilities.getShort(read.Content, 9);
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

        byte[] temp = super.getByteTransform().TransByte( value, "US-ASCII" );
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
        byte[] temp = super.getByteTransform().TransByte( value, "unicode" );
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
        byte[] temp = super.getByteTransform().TransByte( value, "unicode" );
        temp = SoftBasic.ArrayExpandToLength(temp, length * 2);
        return Write(address, temp);
    }






    /**
     * 向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
     * @param address 要写入的数据地址
     * @param value 要写入的实际数据，长度为8的倍数
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
            buffer[i] = values[i] ? (byte) 0x01 : (byte) 0x00;
        }
        return Write(address, buffer);
    }


    /**
     * 获取当前对象的字符串标识形式
     * @return 字符串信息
     */
    @Override
    public String toString() {
        return "MelsecMcNet";
    }


}
