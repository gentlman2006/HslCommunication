package HslCommunication.ModBus;

import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.StringResources;
import HslCommunication.Utilities;

/**
 * Modbus协议相关的一些信息
 */
public class ModbusInfo {


    /**
     * 读取线圈
     */
    public static final byte ReadCoil = 0x01;

    /**
     * 读取离散量
     */
    public static final byte ReadDiscrete = 0x02;


    /**
     * 读取寄存器
     */
    public static final byte ReadRegister = 0x03;

    /**
     * 读取输入寄存器
     */
    public static final byte ReadInputRegister = 0x04;


    /**
     * 写单个线圈
     */
    public static final byte WriteOneCoil = 0x05;

    /**
     * 写单个寄存器
     */
    public static final byte WriteOneRegister = 0x06;


    /**
     * 写多个线圈
     */
    public static final byte WriteCoil = 0x0F;

    /**
     * 写多个寄存器
     */
    public static final byte WriteRegister = 0x10;


    /*****************************************************************************************
     *
     *    本服务器和客户端支持的异常返回
     *
     *******************************************************************************************/


    /**
     * 不支持该功能码
     */
    public static final byte FunctionCodeNotSupport = 0x01;

    /**
     * 该地址越界
     */
    public static final byte FunctionCodeOverBound = 0x02;

    /**
     * 读取长度超过最大值
     */
    public static final byte FunctionCodeQuantityOver = 0x03;

    /**
     * 读写异常
     */
    public static final byte FunctionCodeReadWriteException = 0x04;


    /**
     * 将modbus指令打包成Modbus-Tcp指令
     *
     * @param value Modbus指令
     * @param id    消息的序号
     * @return Modbus-Tcp指令
     */
    public static byte[] PackCommandToTcp(byte[] value, int id) {
        byte[] buffer = new byte[value.length + 6];
        buffer[0] = Utilities.getBytes(id)[1];
        buffer[1] = Utilities.getBytes(id)[0];

        buffer[4] = Utilities.getBytes(value.length)[1];
        buffer[5] = Utilities.getBytes(value.length)[0];

        System.arraycopy(value, 0, buffer, 6, value.length);
        return buffer;
    }


    /**
     * 分析Modbus协议的地址信息，该地址适应于tcp及rtu模式
     *
     * @param address         带格式的地址，比如"100"，"x=4;100"，"s=1;100","s=1;x=4;100"
     * @param isStartWithZero 起始地址是否从0开始
     * @return 转换后的地址信息
     */
    public static OperateResultExOne<ModbusAddress> AnalysisReadAddress(String address, boolean isStartWithZero) {
        try {
            ModbusAddress mAddress = new ModbusAddress(address);
            if (!isStartWithZero) {
                if (mAddress.getAddress() < 1) throw new Exception("地址值在起始地址为1的情况下，必须大于1");
                mAddress.setAddress(mAddress.getAddress() - 1);
            }
            return OperateResultExOne.CreateSuccessResult(mAddress);
        } catch (Exception ex) {
            OperateResultExOne<ModbusAddress> resultExOne = new OperateResultExOne<>();
            resultExOne.Message = ex.getMessage();
            return resultExOne;
        }
    }


    /**
     * 通过错误码来获取到对应的文本消息
     *
     * @param code 错误码
     * @return 错误的文本描述
     */
    public static String GetDescriptionByErrorCode(byte code) {
        switch (code) {
            case ModbusInfo.FunctionCodeNotSupport:
                return StringResources.Language.ModbusTcpFunctionCodeNotSupport();
            case ModbusInfo.FunctionCodeOverBound:
                return StringResources.Language.ModbusTcpFunctionCodeOverBound();
            case ModbusInfo.FunctionCodeQuantityOver:
                return StringResources.Language.ModbusTcpFunctionCodeQuantityOver();
            case ModbusInfo.FunctionCodeReadWriteException:
                return StringResources.Language.ModbusTcpFunctionCodeReadWriteException();
            default:
                return StringResources.Language.UnknownError();
        }
    }


}
