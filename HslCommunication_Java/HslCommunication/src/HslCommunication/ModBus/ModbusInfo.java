package HslCommunication.ModBus;

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
     *  写多个线圈
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

}
