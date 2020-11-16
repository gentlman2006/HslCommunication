package HslCommunication.Profinet.Melsec;

import HslCommunication.Core.IMessage.MelsecQnA3EBinaryMessage;
import HslCommunication.Core.Net.NetworkBase.NetworkDeviceBase;
import HslCommunication.Core.Transfer.RegularByteTransform;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.Core.Types.OperateResultExTwo;
import HslCommunication.StringResources;
import HslCommunication.Utilities;

/**
 * 三菱的实际数据交互类
 */
public class MelsecMcNet extends NetworkDeviceBase<MelsecQnA3EBinaryMessage,RegularByteTransform> {


    /**
     * 实例化三菱的Qna兼容3E帧协议的通讯对象
     */
    public MelsecMcNet() {
        WordLength = 1;
    }


    /**
     * 实例化一个三菱的Qna兼容3E帧协议的通讯对象
     *
     * @param ipAddress PLCd的Ip地址
     * @param port      PLC的端口
     */
    public MelsecMcNet(String ipAddress, int port) {
        WordLength = 1;
        super.setIpAddress(ipAddress);
        super.setPort(port);
    }


    private byte NetworkNumber = 0x00;                       // 网络号
    private byte NetworkStationNumber = 0x00;                // 网络站号

    /**
     * 获取网络号
     *
     * @return 网络号
     */
    public byte getNetworkNumber() {
        return NetworkNumber;
    }

    /**
     * 设置网络号
     *
     * @param networkNumber 网络号
     */
    public void setNetworkNumber(byte networkNumber) {
        NetworkNumber = networkNumber;
    }

    /**
     * 获取网络站号
     *
     * @return 网络站号
     */
    public byte getNetworkStationNumber() {
        return NetworkStationNumber;
    }

    /**
     * 设置网络站号
     *
     * @param networkStationNumber 网络站号
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
        // 获取指令
        OperateResultExOne<byte[]> command = BuildReadCommand( address, length, NetworkNumber, NetworkStationNumber );
        if (!command.IsSuccess) return OperateResultExOne.CreateFailedResult( command );

        // 核心交互
        OperateResultExOne<byte[]> read = ReadFromCoreServer( command.Content );
        if (!read.IsSuccess) return OperateResultExOne.CreateFailedResult( read );

        // 错误代码验证
        int errorCode = Utilities.getShort(read.Content, 9);
        if (errorCode != 0) return new OperateResultExOne<>( errorCode, StringResources.Language.MelsecPleaseReferToManulDocument() );

        // 数据解析，需要传入是否使用位的参数
        return ExtractActualData( read.Content, command.Content[13] == 1 );
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
        if (analysis.Content1.getDataType() == 0x00) return new OperateResultExOne<boolean[]>( 0, StringResources.Language.MelsecReadBitInfo() );

        // 核心交互
        OperateResultExOne<byte[]> read = Read( address, length );
        if (!read.IsSuccess) return OperateResultExOne.CreateFailedResult( read );

        // 转化bool数组
        boolean[] result = new boolean[read.Content.length];
        for(int i=0;i<result.length;i++){
            if(read.Content[i] == 0x01) result[i] = true;
        }
        return OperateResultExOne.CreateSuccessResult( result );
    }



    /**
     * 从三菱PLC中批量读取位软元件，返回读取结果
     * @param address 起始地址
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<Boolean> ReadBool(String address) {
        OperateResultExOne<boolean[]> read = ReadBool(address, (short) 1);
        if (!read.IsSuccess) return OperateResultExOne.CreateFailedResult(read);

        return OperateResultExOne.CreateSuccessResult(read.Content[0]);
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

        // 错误码校验
        short ErrorCode = Utilities.getShort(read.Content, 9);
        if (ErrorCode != 0) return new OperateResultExOne<byte[]>( ErrorCode, StringResources.Language.MelsecPleaseReferToManulDocument() );

        // 成功
        return OperateResult.CreateSuccessResult( );
    }



    /**
     * 向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
     * @param address 要写入的数据地址
     * @param value 要写入的实际数据，长度为8的倍数
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
            buffer[i] = values[i] ? (byte) 0x01 : (byte) 0x00;
        }
        return Write(address, buffer);
    }


    /**
     * 获取当前对象的字符串标识形式
     * @return 字符串信息
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
    public static OperateResultExOne<byte[]> BuildReadCommand(String address, short length) {
        return BuildReadCommand(address,length,(byte) 0,(byte)0);
    }

    /**
     * 根据类型地址长度确认需要读取的指令头
     * @param address 起始地址
     * @param length 长度
     * @param networkNumber 网络号
     * @param networkStationNumber 网络站号
     * @return 带有成功标志的指令数据
     */
    public static OperateResultExOne<byte[]> BuildReadCommand(String address, short length, byte networkNumber, byte networkStationNumber) {
        OperateResultExTwo<MelsecMcDataType, Short> analysis = MelsecHelper.McAnalysisAddress(address);
        if (!analysis.IsSuccess) return OperateResultExOne.CreateFailedResult(analysis);

        byte[] _PLCCommand = new byte[21];
        _PLCCommand[0] = 0x50;                         // 副标题
        _PLCCommand[1] = 0x00;
        _PLCCommand[2] = networkNumber;                // 网络号
        _PLCCommand[3] = (byte) (0xFF);                         // PLC编号
        _PLCCommand[4] = (byte) (0xFF);
        _PLCCommand[5] = 0x03;
        _PLCCommand[6] = networkStationNumber;         // 目标模块站号
        _PLCCommand[7] = 0x0C;                         // 请求数据长度
        _PLCCommand[8] = 0x00;
        _PLCCommand[9] = 0x0A;                         // CPU监视定时器
        _PLCCommand[10] = 0x00;
        _PLCCommand[11] = 0x01;                        // 批量读取数据命令
        _PLCCommand[12] = 0x04;
        _PLCCommand[13] = analysis.Content1.getDataType();               // 以点为单位还是字为单位成批读取
        _PLCCommand[14] = 0x00;
        _PLCCommand[15] = (byte) (analysis.Content2 % 256);       // 起始地址的地位
        _PLCCommand[16] = (byte) (analysis.Content2 / 256);
        _PLCCommand[17] = 0x00;
        _PLCCommand[18] = analysis.Content1.getDataCode();               // 指明读取的数据
        _PLCCommand[19] = (byte) (length % 256);        // 软元件长度的地位
        _PLCCommand[20] = (byte) (length / 256);

        return OperateResultExOne.CreateSuccessResult(_PLCCommand);
    }



    /**
     * 根据类型地址以及需要写入的数据来生成指令头
     * @param address 起始地址
     * @param value 值
     * @return 结果
     */
    public static OperateResultExOne<byte[]> BuildWriteCommand(String address, byte[] value) {
        return BuildWriteCommand(address,value,(byte)0,(byte)0);
    }

    /**
     * 根据类型地址以及需要写入的数据来生成指令头
     * @param address 起始地址
     * @param value 值
     * @param networkNumber 网络号
     * @param networkStationNumber 网络站号
     * @return 结果
     */
    public static OperateResultExOne<byte[]> BuildWriteCommand(String address, byte[] value, byte networkNumber, byte networkStationNumber) {
        OperateResultExTwo<MelsecMcDataType, Short> analysis = MelsecHelper.McAnalysisAddress(address);
        if (!analysis.IsSuccess) return OperateResultExOne.CreateFailedResult(analysis);

        int length = -1;
        if (analysis.Content1.getDataType() == 1)
        {
            // 按照位写入的操作，数据需要重新计算
            length = value.length;
            value = MelsecHelper.TransBoolArrayToByteData( value );
        }

        byte[] _PLCCommand = new byte[21 + value.length];
        _PLCCommand[0] = 0x50;                                          // 副标题
        _PLCCommand[1] = 0x00;
        _PLCCommand[2] = networkNumber;                                 // 网络号
        _PLCCommand[3] = (byte) (0xFF);                                          // PLC编号
        _PLCCommand[4] = (byte) (0xFF);                                          // 目标模块IO编号
        _PLCCommand[5] = 0x03;
        _PLCCommand[6] = networkStationNumber;                          // 目标模块站号

        _PLCCommand[7] = (byte) ((_PLCCommand.length - 9) % 256);        // 请求数据长度
        _PLCCommand[8] = (byte) ((_PLCCommand.length - 9) / 256);
        ;
        _PLCCommand[9] = 0x0A;                                          // CPU监视定时器
        _PLCCommand[10] = 0x00;
        _PLCCommand[11] = 0x01;                                         // 批量读取数据命令
        _PLCCommand[12] = 0x14;
        _PLCCommand[13] = analysis.Content1.getDataType();                   // 以点为单位还是字为单位成批读取
        _PLCCommand[14] = 0x00;
        _PLCCommand[15] = (byte) (analysis.Content2 % 256);
        ;            // 起始地址的地位
        _PLCCommand[16] = (byte) (analysis.Content2 / 256);
        _PLCCommand[17] = 0x00;
        _PLCCommand[18] = analysis.Content1.getDataCode();                   // 指明写入的数据

        // 判断是否进行位操作
        if (analysis.Content1.getDataType() == 1) {
            if (length > 0) {
                _PLCCommand[19] = (byte) (length % 256);                 // 软元件长度的地位
                _PLCCommand[20] = (byte) (length / 256);
            } else {
                _PLCCommand[19] = (byte) (value.length * 2 % 256);        // 软元件长度的地位
                _PLCCommand[20] = (byte) (value.length * 2 / 256);
            }
        } else {
            _PLCCommand[19] = (byte) (value.length / 2 % 256);            // 软元件长度的地位
            _PLCCommand[20] = (byte) (value.length / 2 / 256);
        }

        System.arraycopy(value, 0, _PLCCommand, 21, value.length);

        return OperateResultExOne.CreateSuccessResult(_PLCCommand);
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
            byte[] Content = new byte[(response.length - 11) * 2];
            for (int i = 11; i < response.length; i++)
            {
                if ((response[i] & 0x10) == 0x10)
                {
                    Content[(i - 11) * 2 + 0] = 0x01;
                }

                if ((response[i] & 0x01) == 0x01)
                {
                    Content[(i - 11) * 2 + 1] = 0x01;
                }
            }

            return OperateResultExOne.CreateSuccessResult( Content );
        }
        else
        {
            // 字读取
            byte[] Content = new byte[response.length - 11];
            System.arraycopy(response,11,Content,0,Content.length);

            return OperateResultExOne.CreateSuccessResult( Content );
        }
    }

}
