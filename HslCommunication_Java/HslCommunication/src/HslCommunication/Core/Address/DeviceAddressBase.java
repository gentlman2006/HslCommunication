package HslCommunication.Core.Address;

/**
 * 所有通信类的地址基类
 */
public class DeviceAddressBase {


    /**
     * 获取地址信息
     * @return
     */
    public int getAddress() {
        return Address;
    }

    /**
     * 设置地址信息
     * @param address
     */
    public void setAddress(int address) {
        Address = address;
    }

    /**
     * 起始地址
     */
    private int Address = 0;


    /**
     * 解析字符串的地址
     *
     * @param address
     */
    public void AnalysisAddress(String address) {
        Address = Integer.parseInt(address);
    }


    /**
     * 返回表示当前对象的字符串
     * @return 字符串
     */
    @Override
    public String toString() {
        return String.valueOf(Address);
    }
}
