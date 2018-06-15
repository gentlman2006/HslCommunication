package HslCommunication.ModBus;

import HslCommunication.BasicFramework.SoftBasic;
import HslCommunication.BasicFramework.SoftIncrementCount;
import HslCommunication.Core.IMessage.ModbusTcpMessage;
import HslCommunication.Core.Net.NetworkBase.NetworkDeviceBase;
import HslCommunication.Core.Transfer.ReverseWordTransform;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.Core.Types.OperateResultExTwo;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.StringResources;
import HslCommunication.Utilities;

import java.io.ByteArrayOutputStream;


/**
 * Modbus-Tcp协议的客户端通讯类，方便的和服务器进行数据交互
 */
public class ModbusTcpNet extends NetworkDeviceBase<ModbusTcpMessage, ReverseWordTransform> {


    /// <summary>
    /// 实例化一个MOdbus-Tcp协议的客户端对象
    /// </summary>
    public ModbusTcpNet() {
        softIncrementCount = new SoftIncrementCount(65535, 0);
        WordLength = 1;
    }


    /// <summary>
    /// 指定服务器地址，端口号，客户端自己的站号来初始化
    /// </summary>
    /// <param name="ipAddress">服务器的Ip地址</param>
    /// <param name="port">服务器的端口号</param>
    /// <param name="station">客户端自身的站号</param>
    public ModbusTcpNet(String ipAddress, int port, byte station) {
        softIncrementCount = new SoftIncrementCount(65535, 0);
        setIpAddress(ipAddress);
        setPort(port);
        WordLength = 1;
        this.station = station;
    }


    private byte station = 0x01;                                // 本客户端的站号
    private SoftIncrementCount softIncrementCount;              // 自增消息的对象
    private boolean isAddressStartWithZero = true;                 // 线圈值的地址值是否从零开始


    /**
     * 获取起始地址是否从0开始
     *
     * @return bool值
     */
    public boolean getAddressStartWithZero() {
        return isAddressStartWithZero;
    }

    /**
     * 设置起始地址是否从0开始
     *
     * @param addressStartWithZero true代表从0开始，false代表从1开始
     */
    public void setAddressStartWithZero(boolean addressStartWithZero) {
        this.isAddressStartWithZero = addressStartWithZero;
    }

    /**
     * 获取站号
     *
     * @return 站号
     */
    public byte getStation() {
        return station;
    }

    /**
     * 设置站号
     *
     * @param station 站号
     */
    public void setStation(byte station) {
        this.station = station;
    }

    /**
     * 获取多字节数据是否反转，适用于int,float,double,long类型的数据
     *
     * @return bool值
     */
    public boolean isMultiWordReverse() {
        return getByteTransform().IsMultiWordReverse;
    }

    /**
     * 设置多字节数据是否反转，适用于int,float,double,long类型的数据
     *
     * @param multiWordReverse bool值
     */
    public void setMultiWordReverse(boolean multiWordReverse) {
        getByteTransform().IsMultiWordReverse = multiWordReverse;
    }

    /**
     * 字符串数据是否发生反转
     *
     * @return bool值
     */
    public boolean isStringReverse() {
        return getByteTransform().IsStringReverse;
    }

    /**
     * 设置字符串数据是否反转
     *
     * @param stringReverse bool值
     */
    public void setStringReverse(boolean stringReverse) {
        getByteTransform().IsStringReverse = stringReverse;
    }


    /**
     * 解析数据地址，解析出地址类型，起始地址
     *
     * @param address 数据地址
     * @return 解析出地址类型，起始地址，DB块的地址
     */
    private OperateResultExOne<Integer> AnalysisAddress(String address) {
        try {
            int add = Integer.parseInt(address);
            add = CheckAddressStartWithZero(add);
            return OperateResultExOne.CreateSuccessResult(add);
        } catch (Exception ex) {
            OperateResultExOne<Integer> resultExOne = new OperateResultExOne<>();
            resultExOne.Message = ex.getMessage();
            return resultExOne;
        }
    }


    /**
     * 解析数据地址，解析出地址类型，起始地址
     *
     * @param address 数据地址
     * @return 解析出地址类型，起始地址
     */
    private OperateResultExTwo<Byte, Integer> AnalysisReadAddress(String address) {
        try {
            if (address.indexOf('X') < 0) {
                // 正常地址，功能码03
                int add = Integer.parseInt(address);
                return OperateResultExTwo.CreateSuccessResult(ModbusInfo.ReadRegister, add);
            } else {
                // 带功能码的地址
                String[] list = address.split("X");
                byte function = Byte.parseByte(list[0]);
                int add = Integer.parseInt(list[1]);
                return OperateResultExTwo.CreateSuccessResult(function, add);
            }
        } catch (Exception ex) {
            OperateResultExTwo<Byte, Integer> resultExTwo = new OperateResultExTwo<>();
            resultExTwo.Message = ex.getMessage();
            return resultExTwo;
        }
    }

    private int CheckAddressStartWithZero(int add) {
        if (isAddressStartWithZero) {
            if (add < 0) {
                throw new RuntimeException("地址值必须大于等于0");
            }
        } else {
            if (add < 1) {
                throw new RuntimeException("地址值必须大于等于1");
            }

            add--;
        }
        return add;
    }


    /**
     * 读取数据的基础指令，需要指定指令码，地址，长度
     *
     * @param code    功能码
     * @param address 起始地址
     * @param count   读取长度
     * @return 指令
     */
    private OperateResultExOne<byte[]> BuildReadCommandBase(byte code, String address, short count) {
        OperateResultExOne<Integer> analysis = AnalysisAddress(address);
        if (!analysis.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(analysis);

        short messageId = (short) softIncrementCount.GetCurrentValue();
        byte[] buffer = new byte[12];
        buffer[0] = (byte) (messageId / 256);
        buffer[1] = (byte) (messageId % 256);
        buffer[2] = 0x00;
        buffer[3] = 0x00;
        buffer[4] = 0x00;
        buffer[5] = 0x06;
        buffer[6] = station;
        buffer[7] = code;
        buffer[8] = (byte) (analysis.Content / 256);
        buffer[9] = (byte) (analysis.Content % 256);
        buffer[10] = (byte) (count / 256);
        buffer[11] = (byte) (count % 256);

        return OperateResultExOne.CreateSuccessResult(buffer);
    }


    /**
     * 生成一个读取线圈的指令头
     *
     * @param address 地址
     * @param count   长度
     * @return 携带有命令字节
     */
    private OperateResultExOne<byte[]> BuildReadCoilCommand(String address, short count) {
        return BuildReadCommandBase(ModbusInfo.ReadCoil, address, count);
    }


    /**
     * 生成一个读取离散信息的指令头
     *
     * @param address 地址
     * @param count   长度
     * @return 携带有命令字节
     */
    private OperateResultExOne<byte[]> BuildReadDiscreteCommand(String address, short count) {
        return BuildReadCommandBase(ModbusInfo.ReadDiscrete, address, count);
    }


    /**
     * 生成一个读取寄存器的指令头
     *
     * @param address 起始地址
     * @param count   长度
     * @return 命令字节
     */
    private OperateResultExOne<byte[]> BuildReadRegisterCommand(String address, short count) {
        return BuildReadCommandBase(ModbusInfo.ReadRegister, address, count);
    }


    private OperateResultExOne<byte[]> BuildWriteOneCoilCommand(String address, boolean value) {
        OperateResultExOne<Integer> analysis = AnalysisAddress(address);
        if (!analysis.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(analysis);

        short messageId = (short) softIncrementCount.GetCurrentValue();
        byte[] buffer = new byte[12];
        buffer[0] = (byte) (messageId / 256);
        buffer[1] = (byte) (messageId % 256);
        buffer[2] = 0x00;
        buffer[3] = 0x00;
        buffer[4] = 0x00;
        buffer[5] = 0x06;
        buffer[6] = station;
        buffer[7] = ModbusInfo.WriteOneCoil;
        buffer[8] = (byte) (analysis.Content / 256);
        buffer[9] = (byte) (analysis.Content % 256);
        buffer[10] = (byte) (value ? 0xFF : 0x00);
        buffer[11] = 0x00;

        return OperateResultExOne.CreateSuccessResult(buffer);
    }


    private OperateResultExOne<byte[]> BuildWriteOneRegisterCommand(String address, byte[] data) {
        OperateResultExOne<Integer> analysis = AnalysisAddress(address);
        if (!analysis.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(analysis);

        short messageId = (short) softIncrementCount.GetCurrentValue();
        byte[] buffer = new byte[12];
        buffer[0] = (byte) (messageId / 256);
        buffer[1] = (byte) (messageId % 256);
        buffer[2] = 0x00;
        buffer[3] = 0x00;
        buffer[4] = 0x00;
        buffer[5] = 0x06;
        buffer[6] = station;
        buffer[7] = ModbusInfo.WriteOneRegister;
        buffer[8] = (byte) (analysis.Content / 256);
        buffer[9] = (byte) (analysis.Content % 256);
        buffer[10] = data[1];
        buffer[11] = data[0];

        return OperateResultExOne.CreateSuccessResult(buffer);
    }


    private OperateResultExOne<byte[]> BuildWriteCoilCommand(String address, boolean[] values) {
        byte[] data = SoftBasic.BoolArrayToByte(values);

        OperateResultExOne<Integer> analysis = AnalysisAddress(address);
        if (!analysis.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(analysis);

        short messageId = (short) softIncrementCount.GetCurrentValue();
        byte[] buffer = new byte[13 + data.length];
        buffer[0] = (byte) (messageId / 256);
        buffer[1] = (byte) (messageId % 256);
        buffer[2] = 0x00;
        buffer[3] = 0x00;
        buffer[4] = (byte) ((buffer.length - 6) / 256);
        buffer[5] = (byte) ((buffer.length - 6) % 256);
        buffer[6] = station;
        buffer[7] = ModbusInfo.WriteCoil;
        buffer[8] = (byte) (analysis.Content / 256);
        buffer[9] = (byte) (analysis.Content % 256);
        buffer[10] = (byte) (values.length / 256);
        buffer[11] = (byte) (values.length % 256);

        buffer[12] = (byte) (data.length);
        System.arraycopy(data, 0, buffer, 13, data.length);

        return OperateResultExOne.CreateSuccessResult(buffer);
    }


    private OperateResultExOne<byte[]> BuildWriteRegisterCommand(String address, byte[] data) {
        OperateResultExOne<Integer> analysis = AnalysisAddress(address);
        if (!analysis.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(analysis);

        short messageId = (short) softIncrementCount.GetCurrentValue();
        byte[] buffer = new byte[13 + data.length];
        buffer[0] = (byte) (messageId / 256);
        buffer[1] = (byte) (messageId % 256);
        buffer[2] = 0x00;
        buffer[3] = 0x00;
        buffer[4] = (byte) ((buffer.length - 6) / 256);
        buffer[5] = (byte) ((buffer.length - 6) % 256);
        buffer[6] = station;
        buffer[7] = ModbusInfo.WriteRegister;
        buffer[8] = (byte) (analysis.Content / 256);
        buffer[9] = (byte) (analysis.Content % 256);
        buffer[10] = (byte) (data.length / 2 / 256);
        buffer[11] = (byte) (data.length / 2 % 256);

        buffer[12] = (byte) (data.length);
        System.arraycopy(data, 0, buffer, 13, data.length);
        return OperateResultExOne.CreateSuccessResult(buffer);
    }


    /**
     * 通过错误码来获取到对应的文本消息
     *
     * @param code 错误码
     * @return 错误的文本描述
     */
    private String GetDescriptionByErrorCode(byte code) {
        switch (code) {
            case ModbusInfo.FunctionCodeNotSupport:
                return StringResources.ModbusTcpFunctionCodeNotSupport;
            case ModbusInfo.FunctionCodeOverBound:
                return StringResources.ModbusTcpFunctionCodeOverBound;
            case ModbusInfo.FunctionCodeQuantityOver:
                return StringResources.ModbusTcpFunctionCodeQuantityOver;
            case ModbusInfo.FunctionCodeReadWriteException:
                return StringResources.ModbusTcpFunctionCodeReadWriteException;
            default:
                return StringResources.UnknownError;
        }
    }


    /**
     * 检查当前的Modbus-Tcp响应是否是正确的
     *
     * @param send 发送的数据信息
     * @return 带是否成功的结果数据
     */
    private OperateResultExOne<byte[]> CheckModbusTcpResponse(byte[] send) {
        OperateResultExOne<byte[]> result = ReadFromCoreServer(send);
        if (result.IsSuccess) {
            if ((send[7] + 0x80) == result.Content[7]) {
                // 发生了错误
                result.IsSuccess = false;
                result.Message = GetDescriptionByErrorCode(result.Content[8]);
                result.ErrorCode = result.Content[8];
            }
        }
        return result;
    }


    /**
     * 读取服务器的数据，需要指定不同的功能码
     *
     * @param code    指令
     * @param address 地址
     * @param length  长度
     * @return 指令字节信息
     */
    private OperateResultExOne<byte[]> ReadModBusBase(byte code, String address, short length) {
        OperateResultExOne<byte[]> command = BuildReadCommandBase(code, address, length);
        if (!command.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(command);

        OperateResultExOne<byte[]> resultBytes = CheckModbusTcpResponse(command.Content);
        if (resultBytes.IsSuccess) {
            // 二次数据处理
            if (resultBytes.Content.length >= 9) {
                byte[] buffer = new byte[resultBytes.Content.length - 9];
                System.arraycopy(resultBytes.Content, 9, buffer, 0, buffer.length);
                resultBytes.Content = buffer;
            }
        }
        return resultBytes;
    }


    /**
     * 读取线圈，需要指定起始地址
     *
     * @param address 起始地址，格式为"1234"
     * @return 带有成功标志的bool对象
     */
    public OperateResultExOne<Boolean> ReadCoil(String address) {
        OperateResultExOne<byte[]> read = ReadModBusBase(ModbusInfo.ReadCoil, address, (short) 1);
        if (!read.IsSuccess) return OperateResultExOne.<Boolean>CreateFailedResult(read);

        return GetBoolResultFromBytes(read);
    }


    /**
     * 批量的读取线圈，需要指定起始地址，读取长度
     *
     * @param address 起始地址，格式为"1234"
     * @param length  读取长度
     * @return 带有成功标志的bool数组对象
     */
    public OperateResultExOne<boolean[]> ReadCoil(String address, short length) {
        OperateResultExOne<byte[]> read = ReadModBusBase(ModbusInfo.ReadCoil, address, length);
        if (!read.IsSuccess) return OperateResultExOne.<boolean[]>CreateFailedResult(read);

        return OperateResultExOne.CreateSuccessResult(SoftBasic.ByteToBoolArray(read.Content, length));
    }


    /**
     * 读取输入线圈，需要指定起始地址
     *
     * @param address 起始地址，格式为"1234"
     * @return 带有成功标志的bool对象
     */
    public OperateResultExOne<Boolean> ReadDiscrete(String address) {
        OperateResultExOne<byte[]> read = ReadModBusBase(ModbusInfo.ReadDiscrete, address, (short) 1);
        if (!read.IsSuccess) return OperateResultExOne.<Boolean>CreateFailedResult(read);

        return GetBoolResultFromBytes(read);
    }


    /**
     * 批量的读取输入点，需要指定起始地址，读取长度
     *
     * @param address 起始地址，格式为"1234"
     * @param length  读取长度
     * @return 带有成功标志的bool数组对象
     */
    public OperateResultExOne<boolean[]> ReadDiscrete(String address, short length) {
        OperateResultExOne<byte[]> read = ReadModBusBase(ModbusInfo.ReadDiscrete, address, length);
        if (!read.IsSuccess) return OperateResultExOne.<boolean[]>CreateFailedResult(read);

        return OperateResultExOne.CreateSuccessResult(SoftBasic.ByteToBoolArray(read.Content, length));
    }


    /**
     * 从Modbus服务器批量读取寄存器的信息，需要指定起始地址，读取长度
     *
     * @param address 起始地址，格式为"1234"，或者是带功能码格式03X1234
     * @param length  读取的数量
     * @return 带有成功标志的字节信息
     */
    @Override
    public OperateResultExOne<byte[]> Read(String address, short length) {
        OperateResultExTwo<Byte, Integer> analysis = AnalysisReadAddress(address);
        if (!analysis.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(analysis);

        ByteArrayOutputStream outputStream = new ByteArrayOutputStream();

        int alreadyFinished = 0;
        while (alreadyFinished < length) {
            short lengthTmp = (short) Math.min((length - alreadyFinished), 120);
            OperateResultExOne<byte[]> read = ReadModBusBase(analysis.Content1, String.valueOf(analysis.Content2 + alreadyFinished), lengthTmp);
            if (!read.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(read);

            try {
                outputStream.write(read.Content);
            } catch (Exception ex) {

            }
            alreadyFinished += lengthTmp;
        }

        byte[] data = outputStream.toByteArray();
        try {
            outputStream.close();
        } catch (Exception ex) {

        }
        return OperateResultExOne.CreateSuccessResult(data);
    }


    /**
     * 写一个寄存器数据
     *
     * @param address 起始地址
     * @param high    高位
     * @param low     地位
     * @return 返回写入结果
     */
    public OperateResult WriteOneRegister(String address, byte high, byte low) {
        OperateResultExOne<byte[]> command = BuildWriteOneRegisterCommand(address, new byte[]{high, low});
        if (!command.IsSuccess) {
            return command;
        }

        return CheckModbusTcpResponse(command.Content);
    }


    /**
     * 写一个寄存器数据
     *
     * @param address 起始地址
     * @param value   写入值
     * @return 返回写入结果
     */
    public OperateResult WriteOneRegister(String address, short value) {
        byte[] buffer = Utilities.getBytes(value);
        return WriteOneRegister(address, buffer[1], buffer[0]);
    }


    /**
     * 将数据写入到Modbus的寄存器上去，需要指定起始地址和数据内容
     *
     * @param address 起始地址，格式为"1234"
     * @param value   写入的数据，长度根据data的长度来指示
     * @return 返回写入结果
     */
    @Override
    public OperateResult Write(String address, byte[] value) {
        OperateResultExOne<byte[]> command = BuildWriteRegisterCommand(address, value);
        if (!command.IsSuccess) {
            return command;
        }

        return CheckModbusTcpResponse(command.Content);
    }


    /**
     * 写一个线圈信息，指定是否通断
     *
     * @param address 起始地址
     * @param value   写入值
     * @return 返回写入结果
     */
    public OperateResult WriteCoil(String address, boolean value) {
        OperateResultExOne<byte[]> command = BuildWriteOneCoilCommand(address, value);
        if (!command.IsSuccess) {
            return command;
        }

        return CheckModbusTcpResponse(command.Content);
    }

    /**
     * 写线圈数组，指定是否通断
     *
     * @param address 起始地址
     * @param values  写入值
     * @return 返回写入结果
     */
    public OperateResult WriteCoil(String address, boolean[] values) {
        OperateResultExOne<byte[]> command = BuildWriteCoilCommand(address, values);
        if (!command.IsSuccess) {
            return command;
        }

        return CheckModbusTcpResponse(command.Content);
    }


    /**
     * 向寄存器中写入指定长度的字符串,超出截断，不够补0，编码格式为ASCII
     *
     * @param address 要写入的数据地址
     * @param value   要写入的实际数据
     * @param length  指定的字符串长度，必须大于0
     * @return 返回写入结果
     */
    public OperateResult Write(String address, String value, int length) {
        byte[] temp = getByteTransform().TransByte(value, "ASCII");
        temp = SoftBasic.ArrayExpandToLength(temp, length);
        return Write(address, temp);
    }


    /**
     * 向寄存器中写入字符串，编码格式为Unicode
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
     * 向寄存器中写入指定长度的字符串,超出截断，不够补0，编码格式为Unicode
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
     * 向寄存器中写入bool数组，返回值说明，比如你写入M100,那么data[0]对应M100.0
     *
     * @param address 要写入的数据地址
     * @param values  要写入的实际数据，长度为8的倍数
     * @return 返回写入结果
     */
    public OperateResult Write(String address, boolean[] values) {
        return Write(address, SoftBasic.BoolArrayToByte(values));
    }


    /// <summary>
    /// 返回表示当前对象的字符串
    /// </summary>
    /// <returns>字符串信息</returns>

    @Override
    public String toString() {
        return "ModbusTcpNet";
    }

}
