package HslCommunication.Core.Types;

/**
 * 用于PLC通讯及Modbus自定义数据类型的读写操作
 */
public interface IDataTransfer {

    /**
     * 读取的数据长度，对于西门子，等同于字节数，对于三菱和Modbus为字节数的一半
     * @return
     */
    short getReadCount();


    /**
     * 从字节数组进行解析实际的对象
     * @param Content 实际的内容
     */
    void ParseSource(byte[] Content);

    /**
     * 将对象生成字符源，写入PLC中
     * @return 写入PLC中
     */
    byte[] ToSource();
}
