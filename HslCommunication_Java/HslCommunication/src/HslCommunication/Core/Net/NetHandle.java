package HslCommunication.Core.Net;

import HslCommunication.Utilities;


/**
 * 用于通信过程中的暗号对象
 */
public final class NetHandle {


    /**
     * 初始化一个暗号对象
     * @param value int值
     */
    public NetHandle(int value)
    {
        byte[] buffer = Utilities.getBytes(value);

        m_CodeMajor = buffer[3];
        m_CodeMinor = buffer[2];
        m_CodeIdentifier = Utilities.getShort(buffer,0);


        m_CodeValue = value;
    }



    /**
     * 根据三个值来初始化暗号对象
     * @param major 主暗号
     * @param minor 主暗号
     * @param identifier 暗号编号
     */
    public NetHandle(int major, int minor, int identifier)
    {
        m_CodeValue = 0;

        byte[] buffer_major=Utilities.getBytes(major);
        byte[] buffer_minor=Utilities.getBytes(minor);
        byte[] buffer_identifier=Utilities.getBytes(identifier);

        m_CodeMajor = buffer_major[0];
        m_CodeMinor = buffer_minor[0];
        m_CodeIdentifier = Utilities.getShort(buffer_identifier,0);

        byte[] buffer = new byte[4];
        buffer[3] = m_CodeMajor;
        buffer[2] = m_CodeMinor;
        buffer[1] = buffer_identifier[1];
        buffer[0] = buffer_identifier[0];

        m_CodeValue = Utilities.getInt(buffer,0);
    }


    /**
     * 完整的暗号值
     */
    private int m_CodeValue;

    /**
     * 主暗号分类0-255
     */
    private byte m_CodeMajor;

    /**
     * 次要的暗号分类0-255
     */
    private byte m_CodeMinor;

    /**
     * 暗号的编号分类0-65535
     */
    private short m_CodeIdentifier;



    /**
     * 获取完整的暗号值
     * @return
     */
    public int get_CodeValue(){
        return  m_CodeValue;
    }


    /**
     * 获取主暗号分类0-255
     * @return 主暗号
     */
    public byte get_CodeMajor() {
        return m_CodeMajor;
    }


    /**
     * 获取次要的暗号分类0-255
     * @return 次暗号
     */
    public byte get_CodeMinor() {
        return m_CodeMinor;
    }


    /**
     * 获取暗号的编号分类0-65535
     * @return 编号分类
     */
    public short get_CodeIdentifier() {
        return m_CodeIdentifier;
    }


}
