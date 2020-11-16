package HslCommunication.Profinet.Melsec;

import HslCommunication.Core.IMessage.MelsecA1EBinaryMessage;
import HslCommunication.Core.Net.NetworkBase.NetworkDeviceBase;
import HslCommunication.Core.Transfer.RegularByteTransform;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.Core.Types.OperateResultExTwo;
import HslCommunication.StringResources;
import HslCommunication.Utilities;

/**
 * 三菱PLC通讯协议，采用A兼容1E帧协议实现，使用二进制码通讯，请根据实际型号来进行选取
 */
public class MelsecA1ENet extends NetworkDeviceBase<MelsecA1EBinaryMessage,RegularByteTransform> {


    /**
     * 实例化三菱的A兼容1E帧协议的通讯对象
     */
    public MelsecA1ENet() {
        WordLength = 1;
    }


    /**
     * 实例化一个三菱的A兼容1E帧协议的通讯对象
     *
     * @param ipAddress PLCd的Ip地址
     * @param port      PLC的端口
     */
    public MelsecA1ENet(String ipAddress, int port) {
        WordLength = 1;
        super.setIpAddress(ipAddress);
        super.setPort(port);
    }


    private byte PLCNumber = (byte) (0xFF);                       // PLC编号

    /**
     * 获取PLC编号
     *
     * @return PLC编号
     */
    public byte getPLCNumber() {
        return PLCNumber;
    }

    /**
     * 设置PLC编号
     *
     * @param plcNumber PLC编号
     */
    public void setPLCNumber(byte plcNumber) {
        PLCNumber = plcNumber;
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
        OperateResultExOne<byte[]> command = BuildReadCommand( address, length, PLCNumber );
        if (!command.IsSuccess) return OperateResultExOne.CreateFailedResult( command );

        // 核心交互
        OperateResultExOne<byte[]> read = ReadFromCoreServer( command.Content );
        if (!read.IsSuccess) return OperateResultExOne.CreateFailedResult( read );

        // 错误代码验证
        if (read.Content[1] != 0) return new OperateResultExOne<>( read.Content[1], StringResources.Language.MelsecPleaseReferToManulDocument() );

        // 数据解析，需要传入是否使用位的参数
        return ExtractActualData( read.Content, command.Content[0] == 0x00 );
    }




    /**
     * 从三菱PLC中批量读取位软元件，返回读取结果
     * @param address 起始地址
     * @param length 读取的长度
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<boolean[]> ReadBool(String address, short length) {
        // 解析地址
        OperateResultExTwo<MelsecA1EDataType, Short> analysis = MelsecHelper.McA1EAnalysisAddress( address );
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
        OperateResultExOne<byte[]> command = BuildWriteCommand( address, value, PLCNumber );
        if (!command.IsSuccess) return command;

        // 核心交互
        OperateResultExOne<byte[]> read = ReadFromCoreServer( command.Content );
        if (!read.IsSuccess) return read;

        // 错误码校验 (在A兼容1E协议中，结束代码后面紧跟的是异常信息的代码)
        if (read.Content[1] != 0) return new OperateResultExOne<byte[]>( read.Content[1], StringResources.Language.MelsecPleaseReferToManulDocument() );

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
        return "MelsecA1ENet";
    }





    /**
     * 根据类型地址长度确认需要读取的指令头
     * @param address 起始地址
     * @param length 长度
     * @return 带有成功标志的指令数据
     */
    public static OperateResultExOne<byte[]> BuildReadCommand(String address, short length) {
        return BuildReadCommand(address,length,(byte)0xFF);
    }

    /**
     * 根据类型地址长度确认需要读取的指令头
     * @param address 起始地址
     * @param length 长度
     * @param plcNumber PLC号
     * @return 带有成功标志的指令数据
     */
    public static OperateResultExOne<byte[]> BuildReadCommand(String address, short length, byte plcNumber) {
        OperateResultExTwo<MelsecA1EDataType, Short> analysis = MelsecHelper.McA1EAnalysisAddress(address);
        if (!analysis.IsSuccess) return OperateResultExOne.CreateFailedResult(analysis);

        byte subtitle = analysis.Content1.getDataType() == 0x01 ? (byte)0x00 : (byte)0x01;

        byte[] _PLCCommand = new byte[12];
        _PLCCommand[0] = subtitle;                              // 副标题
        _PLCCommand[1] = plcNumber;                             // PLC号
        _PLCCommand[2] = 0x0A;                                  // CPU监视定时器（L）这里设置为0x00,0x0A，等待CPU返回的时间为10*250ms=2.5秒
        _PLCCommand[3] = 0x00;                                  // CPU监视定时器（H）
        _PLCCommand[4] = (byte)(analysis.Content2 % 256);       // 起始软元件（开始读取的地址）
        _PLCCommand[5] = (byte)(analysis.Content2 / 256);
        _PLCCommand[6] = 0x00;
        _PLCCommand[7] = 0x00;
        _PLCCommand[8] = analysis.Content1.getDataCode()[1];    // 软元件代码（L）
        _PLCCommand[9] = analysis.Content1.getDataCode()[0];    // 软元件代码（H）
        _PLCCommand[10] = (byte)(length % 256);                 // 软元件点数
        _PLCCommand[11] = 0x00;

        return OperateResultExOne.CreateSuccessResult(_PLCCommand);
    }



    /**
     * 根据类型地址以及需要写入的数据来生成指令头
     * @param address 起始地址
     * @param value 值
     * @return 结果
     */
    public static OperateResultExOne<byte[]> BuildWriteCommand(String address, byte[] value) {
        return BuildWriteCommand(address,value,(byte)0xFF);
    }

    /**
     * 根据类型地址以及需要写入的数据来生成指令头
     * @param address 起始地址
     * @param value 值
     * @param plcNumber PLC号
     * @return 结果
     */
    public static OperateResultExOne<byte[]> BuildWriteCommand(String address, byte[] value, byte plcNumber) {
        OperateResultExTwo<MelsecA1EDataType, Short> analysis = MelsecHelper.McA1EAnalysisAddress(address);
        if (!analysis.IsSuccess) return OperateResultExOne.CreateFailedResult(analysis);

        int length = -1;
        if (analysis.Content1.getDataType() == 1)
        {
            // 按照位写入的操作，数据需要重新计算
            length = value.length;
            value = MelsecHelper.TransBoolArrayToByteData( value );
        }

        byte subtitle = analysis.Content1.getDataType() == 0x01 ? (byte)0x02 : (byte)0x03;

        byte[] _PLCCommand = new byte[12 + value.length];
        _PLCCommand[0] = subtitle;                              // 副标题
        _PLCCommand[1] = plcNumber;                             // PLC号
        _PLCCommand[2] = 0x0A;                                  // CPU监视定时器（L）这里设置为0x00,0x0A，等待CPU返回的时间为10*250ms=2.5秒
        _PLCCommand[3] = 0x00;                                  // CPU监视定时器（H）
        _PLCCommand[4] = (byte)(analysis.Content2 % 256);       // 起始软元件（开始读取的地址）
        _PLCCommand[5] = (byte)(analysis.Content2 / 256);
        _PLCCommand[6] = 0x00;
        _PLCCommand[7] = 0x00;
        _PLCCommand[8] = analysis.Content1.getDataCode()[1];    // 软元件代码（L）
        _PLCCommand[9] = analysis.Content1.getDataCode()[0];    // 软元件代码（H）
        _PLCCommand[10] = (byte)(length % 256);                 // 软元件点数
        _PLCCommand[11] = 0x00;

        // 判断是否进行位操作
        if (analysis.Content1.getDataType() == 1) {
            if (length > 0) {
                _PLCCommand[10] = (byte)(length % 256);                       // 软元件点数
            }
            else {
                _PLCCommand[10] = (byte)(value.length * 2 % 256);             // 软元件点数
            }
        } else {
            _PLCCommand[10] = (byte)(value.length / 2 % 256);                 // 软元件点数
        }

        System.arraycopy(value, 0, _PLCCommand, 12, value.length);

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
            byte[] Content = new byte[(response.length - 2) * 2];
            for (int i = 2; i < response.length; i++)
            {
                if ((response[i] & 0x10) == 0x10)
                {
                    Content[(i - 2) * 2 + 0] = 0x01;
                }

                if ((response[i] & 0x01) == 0x01)
                {
                    Content[(i - 2) * 2 + 1] = 0x01;
                }
            }

            return OperateResultExOne.CreateSuccessResult( Content );
        }
        else
        {
            // 字读取
            byte[] Content = new byte[response.length - 2];
            System.arraycopy(response,2,Content,0,Content.length);

            return OperateResultExOne.CreateSuccessResult( Content );
        }
    }

}
