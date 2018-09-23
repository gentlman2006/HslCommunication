package HslCommunication.Profinet.Melsec;

import HslCommunication.Core.IMessage.MelsecQnA3EAsciiMessage;
import HslCommunication.Core.Net.NetworkBase.NetworkDeviceBase;
import HslCommunication.Core.Transfer.RegularByteTransform;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.Core.Types.OperateResultExTwo;
import HslCommunication.StringResources;
import HslCommunication.Utilities;


/**
 * 三菱PLC通讯类，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为ASCII通讯格式
 */
public class MelsecMcAsciiNet extends NetworkDeviceBase<MelsecQnA3EAsciiMessage, RegularByteTransform> {

    /**
     * 实例化三菱的Qna兼容3E帧协议的通讯对象
     */
    public MelsecMcAsciiNet() {
        WordLength = 1;
    }


    /**
     * 实例化一个三菱的Qna兼容3E帧协议的通讯对象
     * @param ipAddress PLC的Ip地址
     * @param port PLC的端口
     */
    public MelsecMcAsciiNet(String ipAddress, int port) {
        WordLength = 1;
        setIpAddress(ipAddress);
        setPort(port);
    }


    private byte NetworkNumber = 0x00;                       // 网络号
    private byte NetworkStationNumber = 0x00;                // 网络站号

    /**
     * 获取网络号
     *
     * @return
     */
    public byte getNetworkNumber() {
        return NetworkNumber;
    }

    /**
     * 设置网络号
     *
     * @param networkNumber
     */
    public void setNetworkNumber(byte networkNumber) {
        NetworkNumber = networkNumber;
    }

    /**
     * 获取网络站号
     *
     * @return
     */
    public byte getNetworkStationNumber() {
        return NetworkStationNumber;
    }

    /**
     * 设置网络站号
     *
     * @param networkStationNumber
     */
    public void setNetworkStationNumber(byte networkStationNumber) {
        NetworkStationNumber = networkStationNumber;
    }




    /**
     * 从三菱PLC中读取想要的数据，返回读取结果
     * @param address 读取地址，格式为"M100","D100","W1A0"
     * @param length 读取的数据长度，字最大值960，位最大值7168
     * @return 带成功标志的结果数据对象
     */
    @Override
    public OperateResultExOne<byte[]> Read(String address, short length) {
        //获取指令
        OperateResultExOne<byte[]> command = BuildReadCommand( address, length, NetworkNumber, NetworkStationNumber );
        if (!command.IsSuccess) return command;

        // 核心交互
        OperateResultExOne<byte[]> read = ReadFromCoreServer( command.Content );
        if (!read.IsSuccess) return read;

        // 错误代码验证
        short errorCode = Short.parseShort(Utilities.getString(read.Content,18,4,"ASCII"), 16 );
        if (errorCode != 0) return new OperateResultExOne<>( errorCode, StringResources.Language.MelsecPleaseReferToManulDocument() );

        // 数据解析，需要传入是否使用位的参数
        return ExtractActualData( read.Content, command.Content[29] == 0x31 );
    }



    /**
     * 从三菱PLC中批量读取位软元件，返回读取结果
     * @param address 起始地址
     * @param length 读取的长度
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<boolean[]> ReadBool(String address, short length) {
        // 解析地址
        OperateResultExTwo<MelsecMcDataType, Short> analysis = MelsecHelper.McAnalysisAddress( address );
        if (!analysis.IsSuccess) return OperateResultExOne.CreateFailedResult( analysis );

        // 位读取校验
        if (analysis.Content1.getDataType() == 0x00) return new OperateResultExOne<>( StringResources.Language.MelsecReadBitInfo() );

        // 核心交互
        OperateResultExOne<byte[]> read = Read( address, length );
        if (!read.IsSuccess) return OperateResultExOne.CreateFailedResult( read );

        // 转化bool数组
        boolean[] content = new boolean[read.Content.length];
        for (int i = 0; i < read.Content.length; i++) {
            content[i] = read.Content[i] == 0x01;
        }
        return OperateResultExOne.CreateSuccessResult( content );
    }



    /**
     * 从三菱PLC中批量读取位软元件，返回读取结果
     * @param address 起始地址
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<Boolean> ReadBool(String address) {
        OperateResultExOne<boolean[]> read = ReadBool(address, (short) 1);
        if (!read.IsSuccess) return OperateResultExOne.<Boolean>CreateFailedResult(read);

        return OperateResultExOne.<Boolean>CreateSuccessResult(read.Content[0]);
    }


    /**
     * 向PLC写入数据，数据格式为原始的字节类型
     * @param address 起始地址
     * @param value 原始数据
     * @return 结果
     */
    @Override
    public OperateResult Write(String address, byte[] value) {
        // 解析指令
        OperateResultExOne<byte[]> command = BuildWriteCommand( address, value, NetworkNumber, NetworkStationNumber );
        if (!command.IsSuccess) return command;

        // 核心交互
        OperateResultExOne<byte[]> read = ReadFromCoreServer( command.Content );
        if (!read.IsSuccess) return read;

        // 错误码验证
        short errorCode = Short.parseShort( Utilities.getString( read.Content, 18, 4 ,"ASCII"), 16 );
        if (errorCode != 0) return new OperateResultExOne<byte[]>( errorCode, StringResources.Language.MelsecPleaseReferToManulDocument() );

        // 写入成功
        return OperateResult.CreateSuccessResult( );
    }


    /**
     * 向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
     * @param address 要写入的数据地址
     * @param value 要写入的实际数据，true 或者是 false
     * @return 返回写入结果
     */
    public OperateResult Write(String address, boolean value) {
        return Write(address, new boolean[]{value});
    }


    /**
     * 向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
     * @param address 要写入的数据地址
     * @param values 要写入的实际数据，可以指定任意的长度
     * @return 返回写入结果
     */
    public OperateResult Write(String address, boolean[] values) {
        byte[] buffer = new byte[values.length];
        for (int i = 0; i < values.length; i++) {
            if (values[i]) buffer[i] = 0x01;
        }
        return Write(address, buffer);
    }


    /**
     * 返回表示当前对象的字符串
     * @return 字符串
     */
    @Override
    public String toString() {
        return "MelsecMcNet";
    }



    /**
     * 根据类型地址长度确认需要读取的指令头
     * @param address 起始地址
     * @param length 长度
     * @return 带有成功标志的指令数据
     */
    public static OperateResultExOne< byte[]> BuildReadCommand(String address, short length, byte networkNumber, byte networkStationNumber) {
        OperateResultExTwo<MelsecMcDataType, Short> analysis = MelsecHelper.McAnalysisAddress(address);
        if (!analysis.IsSuccess) return OperateResultExOne.CreateFailedResult(analysis);

        // 默认信息----注意：高低字节交错

        try {
            byte[] _PLCCommand = new byte[42];
            _PLCCommand[0] = 0x35;                                      // 副标题
            _PLCCommand[1] = 0x30;
            _PLCCommand[2] = 0x30;
            _PLCCommand[3] = 0x30;
            _PLCCommand[4] = MelsecHelper.BuildBytesFromData(networkNumber)[0];                // 网络号
            _PLCCommand[5] = MelsecHelper.BuildBytesFromData(networkNumber)[1];
            _PLCCommand[6] = 0x46;                         // PLC编号
            _PLCCommand[7] = 0x46;
            _PLCCommand[8] = 0x30;                         // 目标模块IO编号
            _PLCCommand[9] = 0x33;
            _PLCCommand[10] = 0x46;
            _PLCCommand[11] = 0x46;
            _PLCCommand[12] = MelsecHelper.BuildBytesFromData(networkStationNumber)[0];         // 目标模块站号
            _PLCCommand[13] = MelsecHelper.BuildBytesFromData(networkStationNumber)[1];
            _PLCCommand[14] = 0x30;                         // 请求数据长度
            _PLCCommand[15] = 0x30;
            _PLCCommand[16] = 0x31;
            _PLCCommand[17] = 0x38;
            _PLCCommand[18] = 0x30;                         // CPU监视定时器
            _PLCCommand[19] = 0x30;
            _PLCCommand[20] = 0x31;
            _PLCCommand[21] = 0x30;
            _PLCCommand[22] = 0x30;                        // 批量读取数据命令
            _PLCCommand[23] = 0x34;
            _PLCCommand[24] = 0x30;
            _PLCCommand[25] = 0x31;
            _PLCCommand[26] = 0x30;                         // 以点为单位还是字为单位成批读取
            _PLCCommand[27] = 0x30;
            _PLCCommand[28] = 0x30;
            _PLCCommand[29] = analysis.Content1.getDataType() == 0 ? (byte) 0x30 : (byte) 0x31;
            _PLCCommand[30] = (analysis.Content1.getAsciiCode().getBytes("ASCII"))[0];            // 软元件类型
            _PLCCommand[31] = (analysis.Content1.getAsciiCode().getBytes("ASCII"))[1];
            _PLCCommand[32] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[0];                   // 起始地址的地位
            _PLCCommand[33] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[1];
            _PLCCommand[34] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[2];
            _PLCCommand[35] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[3];
            _PLCCommand[36] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[4];
            _PLCCommand[37] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[5];
            _PLCCommand[38] = MelsecHelper.BuildBytesFromData(length)[0];                                                      // 软元件点数
            _PLCCommand[39] = MelsecHelper.BuildBytesFromData(length)[1];
            _PLCCommand[40] = MelsecHelper.BuildBytesFromData(length)[2];
            _PLCCommand[41] = MelsecHelper.BuildBytesFromData(length)[3];

            return OperateResultExOne.CreateSuccessResult( _PLCCommand );
        }
        catch (Exception ex){
            return new OperateResultExOne<byte[]>(ex.getMessage());
        }
    }


    /**
     * 根据类型地址以及需要写入的数据来生成指令头
     * @param address 起始地址
     * @param value 实际的数据
     * @return 命令数据
     */
    public static OperateResultExOne<byte[]> BuildWriteCommand(String address, byte[] value, byte networkNumber, byte networkStationNumber) {
        OperateResultExTwo<MelsecMcDataType, Short> analysis = MelsecHelper.McAnalysisAddress(address);
        if (!analysis.IsSuccess) return OperateResultExOne.CreateFailedResult(analysis);

        // 预处理指令
        if (analysis.Content1.getDataType() == 0x01)
        {
            // 位写入
            byte[] buffer = new byte[value.length];
            for(int i=0;i<buffer.length;i++){
                buffer[i] = value[i] == 0x00? (byte)0x30 : (byte)0x31;
            }
            value = buffer;
        }
        else
        {
            // 字写入
            byte[] buffer = new byte[value.length * 2];
            for (int i = 0; i < value.length / 2; i++)
            {
                byte[] tmp = MelsecHelper.BuildBytesFromData( Utilities.getShort( value, i * 2 ) );
                System.arraycopy(tmp,0,buffer,4*i,4);
            }
            value = buffer;
        }

        byte[] _PLCCommand = new byte[42 + value.length];

        try {
            _PLCCommand[0] = 0x35;                                      // 副标题
            _PLCCommand[1] = 0x30;
            _PLCCommand[2] = 0x30;
            _PLCCommand[3] = 0x30;
            _PLCCommand[4] = MelsecHelper.BuildBytesFromData(networkNumber)[0];                // 网络号
            _PLCCommand[5] = MelsecHelper.BuildBytesFromData(networkNumber)[1];
            _PLCCommand[6] = 0x46;                         // PLC编号
            _PLCCommand[7] = 0x46;
            _PLCCommand[8] = 0x30;                         // 目标模块IO编号
            _PLCCommand[9] = 0x33;
            _PLCCommand[10] = 0x46;
            _PLCCommand[11] = 0x46;
            _PLCCommand[12] = MelsecHelper.BuildBytesFromData(networkStationNumber)[0];         // 目标模块站号
            _PLCCommand[13] = MelsecHelper.BuildBytesFromData(networkStationNumber)[1];
            _PLCCommand[14] = MelsecHelper.BuildBytesFromData((_PLCCommand.length - 18))[0]; // 请求数据长度
            _PLCCommand[15] = MelsecHelper.BuildBytesFromData((_PLCCommand.length - 18))[1];
            _PLCCommand[16] = MelsecHelper.BuildBytesFromData((_PLCCommand.length - 18))[2];
            _PLCCommand[17] = MelsecHelper.BuildBytesFromData((_PLCCommand.length - 18))[3];
            _PLCCommand[18] = 0x30; // CPU监视定时器
            _PLCCommand[19] = 0x30;
            _PLCCommand[20] = 0x31;
            _PLCCommand[21] = 0x30;
            _PLCCommand[22] = 0x31; // 批量写入的命令
            _PLCCommand[23] = 0x34;
            _PLCCommand[24] = 0x30;
            _PLCCommand[25] = 0x31;
            _PLCCommand[26] = 0x30; // 子命令
            _PLCCommand[27] = 0x30;
            _PLCCommand[28] = 0x30;
            _PLCCommand[29] = analysis.Content1.getDataType() == 0 ? (byte) 0x30 : (byte) 0x31;
            _PLCCommand[30] = (analysis.Content1.getAsciiCode().getBytes("ASCII"))[0];                          // 软元件类型
            _PLCCommand[31] = (analysis.Content1.getAsciiCode().getBytes("ASCII"))[1];
            _PLCCommand[32] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[0];                   // 起始地址的地位
            _PLCCommand[33] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[1];
            _PLCCommand[34] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[2];
            _PLCCommand[35] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[3];
            _PLCCommand[36] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[4];
            _PLCCommand[37] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[5];

            // 判断是否进行位操作
            if (analysis.Content1.getDataType() == 1) {
                _PLCCommand[38] = MelsecHelper.BuildBytesFromData( value.length)[0];                                                      // 软元件点数
                _PLCCommand[39] = MelsecHelper.BuildBytesFromData( value.length)[1];
                _PLCCommand[40] = MelsecHelper.BuildBytesFromData( value.length)[2];
                _PLCCommand[41] = MelsecHelper.BuildBytesFromData( value.length)[3];
            } else {
                _PLCCommand[38] = MelsecHelper.BuildBytesFromData( (value.length / 4))[0];                                                      // 软元件点数
                _PLCCommand[39] = MelsecHelper.BuildBytesFromData( (value.length / 4))[1];
                _PLCCommand[40] = MelsecHelper.BuildBytesFromData( (value.length / 4))[2];
                _PLCCommand[41] = MelsecHelper.BuildBytesFromData( (value.length / 4))[3];
            }
            System.arraycopy(value,0,_PLCCommand,42,value.length);

            return OperateResultExOne.CreateSuccessResult(_PLCCommand);

        }
        catch (Exception ex){
            return new OperateResultExOne<>(ex.getMessage());
        }
    }


    /**
     * 从PLC反馈的数据中提取出实际的数据内容，需要传入反馈数据，是否位读取
     * @param response 反馈的数据内容
     * @param isBit 是否位读取
     * @return 解析后的结果对象
     */
    public static OperateResultExOne<byte[]> ExtractActualData( byte[] response, boolean isBit )
    {
        if (isBit)
        {
            // 位读取
            byte[] Content = new byte[response.length - 22];
            for (int i = 22; i < response.length; i++)
            {
                if (response[i] == 0x30)
                {
                    Content[i - 22] = 0x00;
                }
                else
                {
                    Content[i - 22] = 0x01;
                }
            }

            return OperateResultExOne.CreateSuccessResult( Content );
        }
        else
        {
            // 字读取
            byte[] Content = new byte[(response.length - 22) / 2];
            for (int i = 0; i < Content.length / 2; i++)
            {
                int tmp = Integer.parseInt( Utilities.getString( response, i * 4 + 22, 4 ,"ASCII"), 16 );
                byte[] buffer = Utilities.getBytes(tmp);

                Content[i*2+0] = buffer[0];
                Content[i*2+1] = buffer[1];
            }

            return OperateResultExOne.CreateSuccessResult( Content );
        }
    }
}
