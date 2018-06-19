package HslCommunication.Profinet.Siemens;

import HslCommunication.BasicFramework.SoftBasic;
import HslCommunication.Core.IMessage.FetchWriteMessage;
import HslCommunication.Core.Net.NetworkBase.NetworkDeviceBase;
import HslCommunication.Core.Transfer.ReverseBytesTransform;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.Core.Types.OperateResultExThree;
import HslCommunication.Utilities;

/**
 * 使用了Fetch/Write协议来和西门子进行通讯，该种方法需要在PLC侧进行一些配置
 */
public class SiemensFetchWriteNet extends NetworkDeviceBase<FetchWriteMessage, ReverseBytesTransform> {

    /**
     * 实例化一个西门子的Fetch/Write协议的通讯对象
     */
    public SiemensFetchWriteNet() {
        WordLength = 2;
    }


    /**
     * 实例化一个西门子的Fetch/Write协议的通讯对象
     *
     * @param ipAddress PLC的Ip地址
     * @param port      PLC的端口
     */
    public SiemensFetchWriteNet(String ipAddress, int port) {
        WordLength = 2;
        setIpAddress(ipAddress);
        setPort(port);
    }


    /**
     * 计算特殊的地址信息
     *
     * @param address 字符串信息
     * @return 实际值
     */
    private int CalculateAddressStarted(String address) {
        if (address.indexOf('.') < 0) {
            return Integer.parseInt(address);
        } else {
            String[] temp = address.split("\\.");
            return Integer.parseInt(temp[0]);
        }
    }


    /**
     * 解析数据地址，解析出地址类型，起始地址，DB块的地址
     *
     * @param address 数据地址
     * @return 解析出地址类型，起始地址，DB块的地址
     */
    private OperateResultExThree<Byte, Integer, Integer> AnalysisAddress(String address) {
        OperateResultExThree<Byte, Integer, Integer> result = new OperateResultExThree<Byte, Integer, Integer>();
        try {
            result.Content3 = 0;
            if (address.indexOf(0) == 'I') {
                result.Content1 = 0x03;
                result.Content2 = CalculateAddressStarted(address.substring(1));
            } else if (address.indexOf(0) == 'Q') {
                result.Content1 = 0x04;
                result.Content2 = CalculateAddressStarted(address.substring(1));
            } else if (address.indexOf(0) == 'M') {
                result.Content1 = 0x02;
                result.Content2 = CalculateAddressStarted(address.substring(1));
            } else if (address.indexOf(0) == 'D' || address.substring(0, 2).equals("DB")) {
                result.Content1 = 0x01;
                String[] adds = address.split("\\.");
                if (address.indexOf(1) == 'B') {
                    result.Content3 = Integer.parseInt(adds[0].substring(2));
                } else {
                    result.Content3 = Integer.parseInt(adds[0].substring(1));
                }

                if (result.Content3 > 255) {
                    result.Message = "DB块数据无法大于255";
                    return result;
                }

                result.Content2 = CalculateAddressStarted(address.substring(address.indexOf('.') + 1));
            } else if (address.indexOf(0) == 'T') {
                result.Content1 = 0x07;
                result.Content2 = CalculateAddressStarted(address.substring(1));
            } else if (address.indexOf(0) == 'C') {
                result.Content1 = 0x06;
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
     *
     * @param address 地址
     * @param count   长度
     * @return 带结果标识的指令
     */
    private OperateResultExOne<byte[]> BuildReadCommand(String address, int count) {
        OperateResultExOne<byte[]> result = new OperateResultExOne<byte[]>();

        OperateResultExThree<Byte, Integer, Integer> analysis = AnalysisAddress(address);
        if (!analysis.IsSuccess) {
            result.CopyErrorFromOther(analysis);
            return result;
        }

        byte[] _PLCCommand = new byte[16];
        _PLCCommand[0] = 0x53;
        _PLCCommand[1] = 0x35;
        _PLCCommand[2] = 0x10;
        _PLCCommand[3] = 0x01;
        _PLCCommand[4] = 0x03;
        _PLCCommand[5] = 0x05;
        _PLCCommand[6] = 0x03;
        _PLCCommand[7] = 0x08;

        //指定数据区
        _PLCCommand[8] = analysis.Content1;
        _PLCCommand[9] = analysis.Content3.byteValue();

        //指定数据地址
        _PLCCommand[10] = (byte) (analysis.Content2 / 256);
        _PLCCommand[11] = (byte) (analysis.Content2 % 256);

        if (analysis.Content1 == 0x01 || analysis.Content1 == 0x06 || analysis.Content1 == 0x07) {
            if (count % 2 != 0) {
                result.Message = "读取的数据长度必须为偶数";
                return result;
            } else {
                //指定数据长度
                _PLCCommand[12] = (byte) (count / 2 / 256);
                _PLCCommand[13] = (byte) (count / 2 % 256);
            }
        } else {
            //指定数据长度
            _PLCCommand[12] = (byte) (count / 256);
            _PLCCommand[13] = (byte) (count % 256);
        }

        _PLCCommand[14] = (byte) 0xff;
        _PLCCommand[15] = 0x02;

        result.Content = _PLCCommand;
        result.IsSuccess = true;
        return result;
    }


    /**
     * 生成一个写入字节数据的指令
     *
     * @param address 地址
     * @param data    数据
     * @return 带结果标识的指令
     */
    private OperateResultExOne<byte[]> BuildWriteByteCommand(String address, byte[] data) {
        if (data == null) data = new byte[0];
        OperateResultExOne<byte[]> result = new OperateResultExOne<byte[]>();

        OperateResultExThree<Byte, Integer, Integer> analysis = AnalysisAddress(address);
        if (!analysis.IsSuccess) {
            result.CopyErrorFromOther(analysis);
            return result;
        }


        byte[] _PLCCommand = new byte[16 + data.length];
        _PLCCommand[0] = 0x53;
        _PLCCommand[1] = 0x35;
        _PLCCommand[2] = 0x10;
        _PLCCommand[3] = 0x01;
        _PLCCommand[4] = 0x03;
        _PLCCommand[5] = 0x03;
        _PLCCommand[6] = 0x03;
        _PLCCommand[7] = 0x08;

        //指定数据区
        _PLCCommand[8] = analysis.Content1;
        _PLCCommand[9] = analysis.Content3.byteValue();

        //指定数据地址
        _PLCCommand[10] = (byte) (analysis.Content2 / 256);
        _PLCCommand[11] = (byte) (analysis.Content2 % 256);

        if (analysis.Content1 == 0x01 || analysis.Content1 == 0x06 || analysis.Content1 == 0x07) {
            if (data.length % 2 != 0) {
                result.Message = "写入的数据长度必须为偶数";
                return result;
            } else {
                //指定数据长度
                _PLCCommand[12] = (byte) (data.length / 2 / 256);
                _PLCCommand[13] = (byte) (data.length / 2 % 256);
            }
        } else {
            //指定数据长度
            _PLCCommand[12] = (byte) (data.length / 256);
            _PLCCommand[13] = (byte) (data.length % 256);
        }

        _PLCCommand[14] = (byte) 0xff;
        _PLCCommand[15] = 0x02;

        //放置数据
        System.arraycopy(data, 0, _PLCCommand, 16, data.length);

        result.Content = _PLCCommand;
        result.IsSuccess = true;
        return result;
    }


    /**
     * 从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，T100，C100，以字节为单位
     *
     * @param address 起始地址，格式为I100，M100，Q100，DB20.100，T100，C100
     * @param length  读取的数量，以字节为单位
     * @return 带有成功标志的字节信息
     */
    @Override
    public OperateResultExOne<byte[]> Read(String address, short length) {
        OperateResultExOne<byte[]> result = new OperateResultExOne<byte[]>();
        OperateResultExOne<byte[]> command = BuildReadCommand(address, length);
        if (!command.IsSuccess) {
            result.CopyErrorFromOther(command);
            return result;
        }

        OperateResultExOne<byte[]> read = ReadFromCoreServer(command.Content);
        if (read.IsSuccess) {
            if (read.Content[8] == 0x00) {
                // 分析结果
                byte[] buffer = new byte[read.Content.length - 16];
                System.arraycopy(read.Content, 16, buffer, 0, buffer.length);

                result.Content = buffer;
                result.IsSuccess = true;
            } else {
                result.ErrorCode = read.Content[8];
                result.Message = "发生了异常，具体信息查找Fetch/Write协议文档";
            }
        } else {
            result.ErrorCode = read.ErrorCode;
            result.Message = read.Message;
        }

        return result;
    }


    /**
     * 读取指定地址的byte数据
     *
     * @param address 起始地址，格式为I100，M100，Q100，DB20.100
     * @return 返回写入结果
     */
    public OperateResultExOne<Byte> ReadByte(String address) {
        return GetByteResultFromBytes(Read(address, (short) 1));
    }


    /**
     * 将数据写入到PLC数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位
     *
     * @param address 起始地址，格式为I100，M100，Q100，DB20.100
     * @param value   写入的数据，长度根据data的长度来指示
     * @return 返回写入结果
     */
    @Override
    public OperateResult Write(String address, byte[] value) {
        OperateResult result = new OperateResult();

        OperateResultExOne<byte[]> command = BuildWriteByteCommand(address, value);
        if (!command.IsSuccess) {
            result.CopyErrorFromOther(command);
            return result;
        }


        OperateResultExOne<byte[]> write = ReadFromCoreServer(command.Content);
        if (write.IsSuccess) {
            if (write.Content[8] != 0x00) {
                // 写入异常
                result.Message = "写入数据异常，代号为：" + String.valueOf(write.Content[8]);
            } else {
                result.IsSuccess = true;  // 写入成功
            }
        } else {
            result.ErrorCode = write.ErrorCode;
            result.Message = write.Message;
        }
        return result;
    }


    /**
     * 向PLC中写入指定长度的字符串,超出截断，不够补0，编码格式为ASCII
     *
     * @param address 要写入的数据地址
     * @param value   要写入的实际数据
     * @param length  指定的字符串长度，必须大于0
     * @return 返回写入结果
     */
    public OperateResult Write(String address, String value, int length) {
        byte[] temp = Utilities.getBytes(value, "ASCII");
        temp = SoftBasic.ArrayExpandToLength(temp, length);
        return Write(address, temp);
    }


    /**
     * 向PLC中写入字符串，编码格式为Unicode
     *
     * @param address 要写入的数据地址
     * @param value   要写入的实际数据
     * @return 返回写入结果
     */
    public OperateResult WriteUnicodeString(String address, String value) {
        byte[] temp = Utilities.string2Byte(value);
        return Write(address, temp);
    }


    /**
     * 向PLC中写入指定长度的字符串,超出截断，不够补0，编码格式为Unicode
     *
     * @param address 要写入的数据地址
     * @param value   要写入的实际数据
     * @param length  指定的字符串长度，必须大于0
     * @return 返回写入结果
     */
    public OperateResult WriteUnicodeString(String address, String value, int length) {
        byte[] temp = Utilities.string2Byte(value);
        temp = SoftBasic.ArrayExpandToLength(temp, length * 2);
        return Write(address, temp);
    }


    /**
     * 向PLC中写入bool数组，返回值说明，比如你写入M100,那么data[0]对应M100.0
     *
     * @param address 要写入的数据地址
     * @param values  要写入的实际数据，长度为8的倍数
     * @return 返回写入结果
     */
    public OperateResult Write(String address, boolean[] values) {
        return Write(address, SoftBasic.BoolArrayToByte(values));
    }


    /**
     * 向PLC中写入byte数据，返回值说明
     *
     * @param address 要写入的数据地址
     * @param value   要写入的实际数据
     * @return 返回写入结果
     */
    public OperateResult Write(String address, byte value) {
        return Write(address, new byte[]{value});
    }


    /**
     * 返回表示当前对象的字符串
     *
     * @return 字符串
     */
    @Override
    public String toString() {
        return "SiemensFetchWriteNet";
    }

}
