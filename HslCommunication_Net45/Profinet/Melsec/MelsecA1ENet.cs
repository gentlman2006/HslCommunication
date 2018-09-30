using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;

namespace HslCommunication.Profinet.Melsec
{
    /// <summary>
    /// 三菱PLC通讯协议，采用A兼容1E帧协议实现，使用二进制码通讯，请根据实际型号来进行选取
    /// </summary>
    /// <remarks>
    /// 本类适用于的PLC列表
    /// <list type="number">
    /// <item>FX3U(C) PLC   测试人sandy_liao</item>
    /// </list>
    /// <note type="important">本通讯类由CKernal推送，感谢</note>
    /// </remarks>
    public class MelsecA1ENet : NetworkDeviceBase<MelsecA1EBinaryMessage, RegularByteTransform>
    {

        #region Constructor

        /// <summary>
        /// 实例化三菱的A兼容1E帧协议的通讯对象
        /// </summary>
        public MelsecA1ENet()
        {
            WordLength = 1;
        }

        /// <summary>
        /// 实例化一个三菱的A兼容1E帧协议的通讯对象
        /// </summary>
        /// <param name="ipAddress">PLC的Ip地址</param>
        /// <param name="port">PLC的端口</param>
        public MelsecA1ENet(string ipAddress, int port)
        {
            WordLength = 1;
            IpAddress = ipAddress;
            Port = port;
        }

        #endregion

        #region Public Member

        /// <summary>
        /// PLC编号
        /// </summary>
        public byte PLCNumber { get; set; } = 0xFF;


        #endregion

        #region Read Support

        /// <summary>
        /// 从三菱PLC中读取想要的数据，返回读取结果
        /// </summary>
        /// <param name="address">读取地址，格式为"M100","D100","W1A0"</param>
        /// <param name="length">读取的数据长度，字最大值960，位最大值7168</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            // 获取指令
            var command = BuildReadCommand(address, length, PLCNumber);
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(command);

            // 核心交互
            var read = ReadFromCoreServer(command.Content);
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(read);

            // 错误代码验证
            if (read.Content[1] != 0) return new OperateResult<byte[]>(read.Content[1], StringResources.Language.MelsecPleaseReferToManulDocument);

            // 数据解析，需要传入是否使用位的参数
            return ExtractActualData(read.Content, command.Content[0] == 0x00);
        }



        /// <summary>
        /// 从三菱PLC中批量读取位软元件，返回读取结果
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">读取的长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            // 地址解析
            var analysis = MelsecHelper.McA1EAnalysisAddress(address);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<bool[]>(analysis);

            // 字读取验证
            if (analysis.Content1.DataType == 0x00)
                return new OperateResult<bool[]>(StringResources.Language.MelsecReadBitInfo);

            // 核心交互
            var read = Read(address, length);
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool[]>(read);

            // 转化bool数组
            return OperateResult.CreateSuccessResult(read.Content.Select(m => m == 0x01).Take(length).ToArray());
        }



        /// <summary>
        /// 从三菱PLC中批量读取位软元件，返回读取结果
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<bool> ReadBool( string address )
        {
            OperateResult<bool[]> read = ReadBool( address, 1 );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool>( read );

            return OperateResult.CreateSuccessResult<bool>( read.Content[0] );
        }


        #endregion

        #region Write Override


        /// <summary>
        /// 向PLC写入数据，数据格式为原始的字节类型
        /// </summary>
        /// <param name="address">初始地址</param>
        /// <param name="value">原始的字节数据</param>
        /// <returns>返回写入结果</returns>
        public override OperateResult Write(string address, byte[] value)
        {
            // 解析指令
            OperateResult<byte[]> command = BuildWriteCommand(address, value, PLCNumber);
            if (!command.IsSuccess) return command;

            // 核心交互
            OperateResult<byte[]> read = ReadFromCoreServer(command.Content);
            if (!read.IsSuccess) return read;

            // 错误码校验 (在A兼容1E协议中，结束代码后面紧跟的是异常信息的代码)
            if (read.Content[1] != 0) return new OperateResult(read.Content[1], StringResources.Language.MelsecPleaseReferToManulDocument);

            // 成功
            return OperateResult.CreateSuccessResult();
        }




        #endregion

        #region Write bool[]


        /// <summary>
        /// 向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据，长度为8的倍数</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write(string address, bool value)
        {
            return Write(address, new bool[] { value });
        }

        /// <summary>
        /// 向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据，可以指定任意的长度</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write(string address, bool[] values)
        {
            return Write(address, values.Select(m => m ? (byte)0x01 : (byte)0x00).ToArray());
        }


        #endregion
        
        #region Object Override

        /// <summary>
        /// 获取当前对象的字符串标识形式
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString()
        {
            return $"MelsecA1ENet[{IpAddress}:{Port}]";
        }

        #endregion

        #region Static Method Helper

        /// <summary>
        /// 根据类型地址长度确认需要读取的指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">长度</param>
        /// <param name="plcNumber">PLC编号</param>
        /// <returns>带有成功标志的指令数据</returns>
        public static OperateResult<byte[]> BuildReadCommand(string address, ushort length, byte plcNumber)
        {
            var analysis = MelsecHelper.McA1EAnalysisAddress(address);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysis);

            // 默认信息----注意：高低字节交错
            byte subtitle = analysis.Content1.DataType == 0x01 ? (byte)0x00 : (byte)0x01;

            byte[] _PLCCommand = new byte[12];
            _PLCCommand[0] = subtitle;                              // 副标题
            _PLCCommand[1] = plcNumber;                             // PLC号
            _PLCCommand[2] = 0x0A;                                  // CPU监视定时器（L）这里设置为0x00,0x0A，等待CPU返回的时间为10*250ms=2.5秒
            _PLCCommand[3] = 0x00;                                  // CPU监视定时器（H）
            _PLCCommand[4] = (byte)(analysis.Content2 % 256);       // 起始软元件（开始读取的地址）
            _PLCCommand[5] = (byte)(analysis.Content2 / 256);
            _PLCCommand[6] = 0x00;
            _PLCCommand[7] = 0x00;
            _PLCCommand[8] = analysis.Content1.DataCode[1];         // 软元件代码（L）
            _PLCCommand[9] = analysis.Content1.DataCode[0];         // 软元件代码（H）
            _PLCCommand[10] = (byte)(length % 256);                 // 软元件点数
            _PLCCommand[11] = 0x00;

            return OperateResult.CreateSuccessResult(_PLCCommand);
        }

        /// <summary>
        /// 根据类型地址以及需要写入的数据来生成指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">数据值</param>
        /// <param name="plcNumber">PLC编号</param>
        /// <returns>带有成功标志的指令数据</returns>
        public static OperateResult<byte[]> BuildWriteCommand(string address, byte[] value, byte plcNumber)
        {
            var analysis = MelsecHelper.McA1EAnalysisAddress(address);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysis);

            int length = -1;
            if (analysis.Content1.DataType == 1)
            {
                // 按照位写入的操作，数据需要重新计算
                length = value.Length;
                value = MelsecHelper.TransBoolArrayToByteData(value);
            }

            // 默认信息----注意：高低字节交错
            byte subtitle = analysis.Content1.DataType == 0x01 ? (byte)0x02 : (byte)0x03;

            byte[] _PLCCommand = new byte[12 + value.Length];
            _PLCCommand[0] = subtitle;                              // 副标题
            _PLCCommand[1] = plcNumber;                             // PLC号
            _PLCCommand[2] = 0x0A;                                  // CPU监视定时器（L）这里设置为0x00,0x0A，等待CPU返回的时间为10*250ms=2.5秒
            _PLCCommand[3] = 0x00;                                  // CPU监视定时器（H）
            _PLCCommand[4] = (byte)(analysis.Content2 % 256);       // 起始软元件（开始读取的地址）
            _PLCCommand[5] = (byte)(analysis.Content2 / 256);
            _PLCCommand[6] = 0x00;
            _PLCCommand[7] = 0x00;
            _PLCCommand[8] = analysis.Content1.DataCode[1];         // 软元件代码（L）
            _PLCCommand[9] = analysis.Content1.DataCode[0];         // 软元件代码（H）
            _PLCCommand[10] = (byte)(length % 256);                 // 软元件点数
            _PLCCommand[11] = 0x00;

            // 判断是否进行位操作
            if (analysis.Content1.DataType == 0x01)
            {
                if (length > 0)
                {
                    _PLCCommand[10] = (byte)(length % 256);                       // 软元件点数
                }
                else
                {
                    _PLCCommand[10] = (byte)(value.Length * 2 % 256);             // 软元件点数
                }
            }
            else
            {
                _PLCCommand[10] = (byte)(value.Length / 2 % 256);                 // 软元件点数
            }

            Array.Copy(value, 0, _PLCCommand, 12, value.Length);                  // 将具体的要写入的数据附加到写入命令后面

            return OperateResult.CreateSuccessResult(_PLCCommand);
        }

        /// <summary>
        /// 从PLC反馈的数据中提取出实际的数据内容，需要传入反馈数据，是否位读取
        /// </summary>
        /// <param name="response">反馈的数据内容</param>
        /// <param name="isBit">是否位读取</param>
        /// <returns>解析后的结果对象</returns>
        public static OperateResult<byte[]> ExtractActualData(byte[] response, bool isBit)
        {
            if (isBit)
            {
                // 位读取
                byte[] Content = new byte[(response.Length - 2) * 2];
                for (int i = 2; i < response.Length; i++)
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

                return OperateResult.CreateSuccessResult(Content);
            }
            else
            {
                // 字读取
                byte[] Content = new byte[response.Length - 2];
                Array.Copy(response, 2, Content, 0, Content.Length);

                return OperateResult.CreateSuccessResult(Content);
            }
        }

        #endregion
    }
}

