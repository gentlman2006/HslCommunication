package HslCommunication.ModBus;

import HslCommunication.BasicFramework.SoftBasic;
import HslCommunication.BasicFramework.SoftIncrementCount;
import HslCommunication.Core.IMessage.ModbusTcpMessage;
import HslCommunication.Core.Net.NetworkBase.NetworkDeviceBase;
import HslCommunication.Core.Transfer.ReverseWordTransform;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.StringResources;
import HslCommunication.Utilities;

import java.io.ByteArrayOutputStream;


/**
 * Modbus-Tcp协议的客户端通讯类，方便的和服务器进行数据交互
 */
public class ModbusTcpNet extends NetworkDeviceBase<ModbusTcpMessage, ReverseWordTransform> {


    /**
     * 实例化一个MOdbus-Tcp协议的客户端对象
     */
    public ModbusTcpNet() {
        softIncrementCount = new SoftIncrementCount(65535, 0);
        WordLength = 1;
    }


    /**
     * 指定服务器地址，端口号，客户端自己的站号来初始化
     *
     * @param ipAddress 服务器的Ip地址
     * @param port      服务器的端口号
     * @param station   客户端自身的站号，可以在读取的时候动态配置
     */
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


    private OperateResultExOne<byte[]> BuildReadCoilCommand(String address, short count) {
        OperateResultExOne<ModbusAddress> analysis = ModbusInfo.AnalysisReadAddress(address, isAddressStartWithZero);
        if (!analysis.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(analysis);

        short messageId = (short) softIncrementCount.GetCurrentValue();
        // 生成最终tcp指令
        byte[] buffer = ModbusInfo.PackCommandToTcp(analysis.Content.CreateReadCoils(station, count), messageId);
        return OperateResultExOne.CreateSuccessResult(buffer);
    }

    private OperateResultExOne<byte[]> BuildReadDiscreteCommand(String address, short length) {
        OperateResultExOne<ModbusAddress> analysis = ModbusInfo.AnalysisReadAddress(address, isAddressStartWithZero);
        if (!analysis.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(analysis);

        short messageId = (short) softIncrementCount.GetCurrentValue();
        // 生成最终tcp指令
        byte[] buffer = ModbusInfo.PackCommandToTcp(analysis.Content.CreateReadDiscrete(station, length), messageId);
        return OperateResultExOne.CreateSuccessResult(buffer);
    }


    private OperateResultExOne<byte[]> BuildReadRegisterCommand(String address, short length) {
        OperateResultExOne<ModbusAddress> analysis = ModbusInfo.AnalysisReadAddress(address, isAddressStartWithZero);
        if (!analysis.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(analysis);

        short messageId = (short) softIncrementCount.GetCurrentValue();
        // 生成最终tcp指令
        byte[] buffer = ModbusInfo.PackCommandToTcp(analysis.Content.CreateReadRegister(station, length), messageId);
        return OperateResultExOne.CreateSuccessResult(buffer);
    }


    private OperateResultExOne<byte[]> BuildReadRegisterCommand(ModbusAddress address, short length) {
        short messageId = (short) softIncrementCount.GetCurrentValue();
        // 生成最终tcp指令
        byte[] buffer = ModbusInfo.PackCommandToTcp(address.CreateReadRegister(station, length), messageId);
        return OperateResultExOne.CreateSuccessResult(buffer);
    }


    private OperateResultExOne<byte[]> BuildWriteOneCoilCommand(String address, boolean value) {
        OperateResultExOne<ModbusAddress> analysis = ModbusInfo.AnalysisReadAddress(address, isAddressStartWithZero);
        if (!analysis.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(analysis);

        short messageId = (short) softIncrementCount.GetCurrentValue();
        // 生成最终tcp指令
        byte[] buffer = ModbusInfo.PackCommandToTcp(analysis.Content.CreateWriteOneCoil(station, value), messageId);
        return OperateResultExOne.CreateSuccessResult(buffer);
    }


    private OperateResultExOne<byte[]> BuildWriteOneRegisterCommand(String address, byte[] data) {
        OperateResultExOne<ModbusAddress> analysis = ModbusInfo.AnalysisReadAddress(address, isAddressStartWithZero);
        if (!analysis.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(analysis);

        short messageId = (short) softIncrementCount.GetCurrentValue();
        // 生成最终tcp指令
        byte[] buffer = ModbusInfo.PackCommandToTcp(analysis.Content.CreateWriteOneRegister(station, data), messageId);
        return OperateResultExOne.CreateSuccessResult(buffer);
    }


    private OperateResultExOne<byte[]> BuildWriteCoilCommand(String address, boolean[] values) {
        OperateResultExOne<ModbusAddress> analysis = ModbusInfo.AnalysisReadAddress(address, isAddressStartWithZero);
        if (!analysis.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(analysis);

        short messageId = (short) softIncrementCount.GetCurrentValue();
        // 生成最终tcp指令
        byte[] buffer = ModbusInfo.PackCommandToTcp(analysis.Content.CreateWriteCoil(station, values), messageId);
        return OperateResultExOne.CreateSuccessResult(buffer);
    }


    private OperateResultExOne<byte[]> BuildWriteRegisterCommand(String address, byte[] values) {
        OperateResultExOne<ModbusAddress> analysis = ModbusInfo.AnalysisReadAddress(address, isAddressStartWithZero);
        if (!analysis.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(analysis);

        short messageId = (short) softIncrementCount.GetCurrentValue();
        // 生成最终tcp指令
        byte[] buffer = ModbusInfo.PackCommandToTcp(analysis.Content.CreateWriteRegister(station, values), messageId);
        return OperateResultExOne.CreateSuccessResult(buffer);
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
                result.Message = ModbusInfo.GetDescriptionByErrorCode(result.Content[8]);
                result.ErrorCode = result.Content[8];
            }
        }
        return result;
    }

    /**
     * 读取服务器的数据，需要指定不同的功能码
     *
     * @param address 地址
     * @param length  长度
     * @return 结果数据
     */
    private OperateResultExOne<byte[]> ReadModBusBase(ModbusAddress address, short length) {
        OperateResultExOne<byte[]> command = BuildReadRegisterCommand(address, length);
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
     * 读取服务器的数据，需要指定不同的功能码
     *
     * @param code    指令
     * @param address 地址
     * @param length  长度
     * @return 指令字节信息
     */
    private OperateResultExOne<byte[]> ReadModBusBase(byte code, String address, short length) {

        OperateResultExOne<byte[]> command = null;
        switch (code) {
            case ModbusInfo.ReadCoil: {
                command = BuildReadCoilCommand(address, length);
                break;
            }
            case ModbusInfo.ReadDiscrete: {
                command = BuildReadDiscreteCommand(address, length);
                break;
            }
            case ModbusInfo.ReadRegister: {
                command = BuildReadRegisterCommand(address, length);
                break;
            }
            default: {
                command = new OperateResultExOne<byte[]>();
                command.Message = StringResources.ModbusTcpFunctionCodeNotSupport;
                break;
            }
        }
        if (!command.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(command);

        OperateResultExOne<byte[]> resultBytes = CheckModbusTcpResponse(command.Content);
        if (resultBytes.IsSuccess) {
            // 二次数据处理
            if (resultBytes.Content.length >= 9) {
                byte[] buffer = new byte[resultBytes.Content.length - 9];
                System.arraycopy(resultBytes, 9, buffer, 0, buffer.length);
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
        OperateResultExOne<ModbusAddress> analysis = ModbusInfo.AnalysisReadAddress(address, isAddressStartWithZero);
        if (!analysis.IsSuccess) return OperateResultExOne.<byte[]>CreateFailedResult(analysis);

        ByteArrayOutputStream outputStream = new ByteArrayOutputStream();

        int alreadyFinished = 0;
        while (alreadyFinished < length) {
            short lengthTmp = (short) Math.min((length - alreadyFinished), 120);
            OperateResultExOne<byte[]> read = ReadModBusBase(analysis.Content.AddressAdd(alreadyFinished), lengthTmp);
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
