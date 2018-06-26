package HslCommunication.ModBus;

import HslCommunication.BasicFramework.SoftBasic;
import HslCommunication.Core.Address.DeviceAddressBase;
import HslCommunication.Utilities;


/**
 * Modbus协议地址格式，可以携带站号，功能码，地址信息
 */
public class ModbusAddress extends DeviceAddressBase {



    /**
     * 实例化一个默认的对象
     */
    public ModbusAddress() {
        Station = -1;
        Function = ModbusInfo.ReadRegister;
        setAddress(0);
    }


    /**
     * 实例化一个默认的对象，使用默认的地址初始化
     * @param address
     */
    public ModbusAddress(String address) {
        Station = -1;
        Function = ModbusInfo.ReadRegister;
        setAddress(0);
        AnalysisAddress(address);
    }


    /**
     * 获取站号
     * @return
     */
    public int getStation() {
        return Station;
    }

    /**
     * 设置站号
     * @param station
     */
    public void setStation(int station) {
        Station = station;
    }


    /**
     * 获取功能码
     * @return
     */
    public byte getFunction() {
        return Function;
    }

    /**
     * 设置功能码
     * @param function
     */
    public void setFunction(byte function) {
        Function = function;
    }

    private int Station = -1;

    private byte Function = -1;



    /**
     * 解析Modbus的地址码
     * @param address 地址
     */
    @Override
    public void AnalysisAddress(String address) {
        if (address.indexOf(';') < 0) {
            // 正常地址，功能码03
            setAddress(Integer.parseInt(address));
        } else {
            // 带功能码的地址
            String[] list = address.split(";");
            for (int i = 0; i < list.length; i++) {
                if (list[i].charAt(0) == 's' || list[i].charAt(0) == 'S') {
                    // 站号信息
                    this.Station = Integer.parseInt(list[i].substring(2));
                } else if (list[i].charAt(0) == 'x' || list[i].charAt(0) == 'X') {
                    this.Function = (byte) Integer.parseInt(list[i].substring(2));
                } else {
                    setAddress(Integer.parseInt(list[i]));
                }
            }
        }
    }



    /**
     * 创建一个读取线圈的字节对象
     * @param station 读取的站号
     * @param length 读取数据的长度
     * @return 原始的modbus指令
     */
    public byte[] CreateReadCoils(byte station, int length) {
        byte[] buffer = new byte[6];
        buffer[0] = this.Station < 0 ? station : (byte) this.Station;
        buffer[1] = ModbusInfo.ReadCoil;
        buffer[2] = Utilities.getBytes(this.getAddress())[1];
        buffer[3] = Utilities.getBytes(this.getAddress())[0];
        buffer[4] = Utilities.getBytes(length)[1];
        buffer[5] = Utilities.getBytes(length)[0];
        return buffer;
    }

    /**
     * 创建一个读取离散输入的字节对象
     * @param station 读取的站号
     * @param length 读取数据的长度
     * @return 原始的modbus指令
     */
    public byte[] CreateReadDiscrete(byte station, int length) {
        byte[] buffer = new byte[6];
        buffer[0] = this.Station < 0 ? station : (byte) this.Station;
        buffer[1] = ModbusInfo.ReadDiscrete;
        buffer[2] = Utilities.getBytes(this.getAddress())[1];
        buffer[3] = Utilities.getBytes(this.getAddress())[0];
        buffer[4] = Utilities.getBytes(length)[1];
        buffer[5] = Utilities.getBytes(length)[0];
        return buffer;
    }



    /**
     * 创建一个读取寄存器的字节对象
     * @param station 读取的站号
     * @param length 读取数据的长度
     * @return 原始的modbus指令
     */
    public byte[] CreateReadRegister(byte station, int length) {
        byte[] buffer = new byte[6];
        buffer[0] = this.Station < 0 ? station : (byte) this.Station;
        buffer[1] = Function;
        buffer[2] = Utilities.getBytes(this.getAddress())[1];
        buffer[3] = Utilities.getBytes(this.getAddress())[0];
        buffer[4] = Utilities.getBytes(length)[1];
        buffer[5] = Utilities.getBytes(length)[0];
        return buffer;
    }


    /**
     * 创建一个写入单个线圈的指令
     * @param station 站号
     * @param value 值
     * @return 原始的modbus指令
     */
    public byte[] CreateWriteOneCoil(byte station, boolean value) {
        byte[] buffer = new byte[6];
        buffer[0] = this.Station < 0 ? station : (byte) this.Station;
        buffer[1] = ModbusInfo.WriteOneCoil;
        buffer[2] = Utilities.getBytes(this.getAddress())[1];
        buffer[3] = Utilities.getBytes(this.getAddress())[0];
        buffer[4] = (byte) (value ? 0xFF : 0x00);
        buffer[5] = 0x00;
        return buffer;
    }



    /**
     * 创建一个写入单个寄存器的指令
     * @param station 站号
     * @param data 值
     * @return 原始的modbus指令
     */
    public byte[] CreateWriteOneRegister(byte station, byte[] data) {
        byte[] buffer = new byte[6];
        buffer[0] = this.Station < 0 ? station : (byte) this.Station;
        buffer[1] = ModbusInfo.WriteOneRegister;
        buffer[2] = Utilities.getBytes(this.getAddress())[1];
        buffer[3] = Utilities.getBytes(this.getAddress())[0];
        buffer[4] = data[1];
        buffer[5] = data[0];
        return buffer;
    }



    /**
     * 创建一个写入批量线圈的指令
     * @param station 站号
     * @param values 值
     * @return 原始的modbus指令
     */
    public byte[] CreateWriteCoil(byte station, boolean[] values) {
        byte[] data = SoftBasic.BoolArrayToByte(values);
        byte[] buffer = new byte[7 + data.length];
        buffer[0] = this.Station < 0 ? station : (byte) this.Station;
        buffer[1] = ModbusInfo.WriteCoil;
        buffer[2] = Utilities.getBytes(this.getAddress())[1];
        buffer[3] = Utilities.getBytes(this.getAddress())[0];
        buffer[4] = (byte) (values.length / 256);
        buffer[5] = (byte) (values.length % 256);
        buffer[6] = (byte) (data.length);
        System.arraycopy(data, 0, buffer, 7, data.length);
        return buffer;
    }



    /**
     * 创建一个写入批量寄存器的指令
     * @param station 站号
     * @param values 值
     * @return 原始的modbus指令
     */
    public byte[] CreateWriteRegister(byte station, byte[] values) {
        byte[] buffer = new byte[7 + values.length];
        buffer[0] = this.Station < 0 ? station : (byte) this.Station;
        buffer[1] = ModbusInfo.WriteRegister;
        buffer[2] = Utilities.getBytes(this.getAddress())[1];
        buffer[3] = Utilities.getBytes(this.getAddress())[0];
        buffer[4] = (byte) (values.length / 2 / 256);
        buffer[5] = (byte) (values.length / 2 % 256);
        buffer[6] = (byte) (values.length);
        System.arraycopy(values, 0, buffer, 7, values.length);
        return buffer;
    }



    /**
     * 地址新增指定的数
     * @param value 地址值
     * @return 新增后的地址信息
     */
    public ModbusAddress AddressAdd(int value) {
        ModbusAddress address = new ModbusAddress();
        address.setAddress(getAddress() + value);
        address.setFunction(getFunction());
        address.setStation(getStation());
        return address;
    }



    /**
     * 地址新增1
     * @return 新增后的地址信息
     */
    public ModbusAddress AddressAdd() {
        return AddressAdd(1);
    }



    /**
     * 获取本对象的字符串表示形式
     * @return 字符串数据
     */
    @Override
    public String toString() {
        StringBuilder sb = new StringBuilder();
        if (Station >= 0) sb.append("s=" + Station + ";");
        if (Function >= 1) sb.append("x=" + Function + ";");
        sb.append(String.valueOf(getAddress()));

        return sb.toString();
    }

}
