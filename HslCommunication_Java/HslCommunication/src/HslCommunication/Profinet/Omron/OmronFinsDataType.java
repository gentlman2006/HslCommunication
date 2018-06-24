package HslCommunication.Profinet.Omron;

/**
 * 欧姆龙的Fins协议的数据类型
 */
public class OmronFinsDataType
{
    /**
     * 实例化一个Fins的数据类型
     * @param bitCode 位操作的指令
     * @param wordCode 字操作的指令
     */
    public OmronFinsDataType( byte bitCode, byte wordCode )
    {
        BitCode = bitCode;
        WordCode = wordCode;

    }

    /**
     * 获取进行位操作的指令
     * @return
     */
    public byte getBitCode() {
        return BitCode;
    }

    /**
     * 进行字操作的指令
     * @return
     */
    public byte getWordCode() {
        return WordCode;
    }


    private byte BitCode = 0;
    private byte WordCode = 0;



    /**
     * DM Area
     */
    public static final OmronFinsDataType DM = new OmronFinsDataType( (byte) 0x02, (byte) 0x82 );

    /**
     * CIO Area
     */
    public static final OmronFinsDataType CIO = new OmronFinsDataType(  (byte)0x30,  (byte)0xB0 );

    /**
     * Work Area
     */
    public static final OmronFinsDataType WR = new OmronFinsDataType(  (byte)0x31,  (byte)0xB1 );

    /**
     * Holding Bit Area
     */
    public static final OmronFinsDataType HR = new OmronFinsDataType(  (byte)0x32,  (byte)0xB2 );

    /**
     * Auxiliary Bit Area
     */
    public static final OmronFinsDataType AR = new OmronFinsDataType(  (byte)0x33,  (byte)0xB3 );
}
