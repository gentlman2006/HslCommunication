package HslCommunication.Profinet.Melsec;


/**
 * 三菱的数据类型
 */
public class MelsecMcDataType {


    /**
     * 如果您清楚类型代号，可以根据值进行扩展
     * @param code 数据类型的代号
     * @param type 0或1，默认为0
     * @param asciiCode ASCII格式的类型信息
     * @param fromBase 指示地址的多少进制的，10或是16
     */
    public MelsecMcDataType( byte code, byte type, String asciiCode, int fromBase )
    {
        DataCode = code;
        AsciiCode = asciiCode;
        FromBase = fromBase;
        if (type < 2) DataType = type;
    }



    private byte DataCode = 0x00;                   // 类型代号
    private byte DataType = 0x00;                   // 数据类型
    private String AsciiCode = "";                  // ascii格式通信的字符
    private int FromBase = 0;                       // 类型


    /**
     * 数据的类型代号
     * @return
     */
    public byte getDataCode() {
        return DataCode;
    }

    /**
     * 字访问还是位访问，0表示字，1表示位
     * @return
     */
    public byte getDataType() {
        return DataType;
    }

    /**
     * 当以ASCII格式通讯时的类型描述
     * @return
     */
    public String getAsciiCode() {
        return AsciiCode;
    }

    /**
     * 指示地址是10进制，还是16进制的
     * @return
     */
    public int getFromBase() {
        return FromBase;
    }


    /**
     * X输入寄存器
     */
    public final static MelsecMcDataType X = new MelsecMcDataType( (byte) (0x9C), (byte) (0x01), "X*", 16 );
    /**
     * Y输出寄存器
     */
    public final static MelsecMcDataType Y = new MelsecMcDataType( (byte) (0x9D), (byte) (0x01), "Y*", 16 );
    /**
     * M中间寄存器
     */
    public final static MelsecMcDataType M = new MelsecMcDataType( (byte) (0x90), (byte) (0x01), "M*", 10 );
    /**
     * D数据寄存器
     */
    public final static MelsecMcDataType D = new MelsecMcDataType( (byte) (0xA8), (byte) (0x00), "D*", 10 );
    /**
     * W链接寄存器
     */
    public final static MelsecMcDataType W = new MelsecMcDataType( (byte) (0xB4), (byte) (0x00), "W*", 16 );
    /**
     * L锁存继电器
     */
    public final static MelsecMcDataType L = new MelsecMcDataType( (byte) (0x92), (byte) (0x01), "L*", 10 );
    /**
     * F报警器
     */
    public final static MelsecMcDataType F = new MelsecMcDataType( (byte) (0x93), (byte) (0x01), "F*", 10 );
    /**
     * V边沿继电器
     */
    public final static MelsecMcDataType V = new MelsecMcDataType( (byte) (0x94), (byte) (0x01), "V*", 10 );
    /**
     * B链接继电器
     */
    public final static MelsecMcDataType B = new MelsecMcDataType( (byte) (0xA0), (byte) (0x01), "B*", 16 );
    /**
     * R文件寄存器
     */
    public final static MelsecMcDataType R = new MelsecMcDataType( (byte) (0xAF), (byte) (0x00), "R*", 10 );
    /**
     * S步进继电器
     */
    public final static MelsecMcDataType S = new MelsecMcDataType( (byte) (0x98), (byte) (0x01), "S*", 10 );
    /**
     * 变址寄存器
     */
    public final static MelsecMcDataType Z = new MelsecMcDataType( (byte) (0xCC), (byte) (0x00), "Z*", 10 );
    /**
     * 定时器的值
     */
    public final static MelsecMcDataType T = new MelsecMcDataType( (byte) (0xC2), (byte) (0x00), "TN", 10 );
    /**
     * 计数器的值
     */
    public final static MelsecMcDataType C = new MelsecMcDataType( (byte) (0xC5), (byte) (0x00), "CN", 10 );
}
