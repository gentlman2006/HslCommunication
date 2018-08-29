using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace HslCommunication.Profinet.AllenBradley
{
    /// <summary>
    /// AB PLC的数据通讯类
    /// </summary>
    [Obsolete("还未完成了类库，由于报文资料不足，暂时无法实现")]
    public class AllenBradleyNet : NetworkDeviceBase<AllenBradleyMessage, ReverseBytesTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化一个AllenBradley PLC协议的通讯对象
        /// </summary>
        public AllenBradleyNet( )
        {
            WordLength = 1;
        }

        /// <summary>
        /// 实例化一个AllenBradley PLC协议的通讯对象
        /// </summary>
        /// <param name="ipAddress">PLCd的Ip地址</param>
        /// <param name="port">PLC的端口</param>
        public AllenBradleyNet( string ipAddress, int port )
        {
            WordLength = 1;
            IpAddress = ipAddress;
            Port = port;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 当前的会话句柄，该值在和PLC握手通信时由PLC进行决定
        /// </summary>
        public uint SessionHandle { get; private set; }
        /// <summary>
        /// 连接标识，O-T Network Connection ID
        /// </summary>
        public uint NetWorkConnectionID { get; private set; }

        #endregion

        #region Double Mode Override

        /// <summary>
        /// 在连接上AllenBradley PLC后，需要进行一步握手协议
        /// </summary>
        /// <param name="socket">连接的套接字</param>
        /// <returns>初始化成功与否</returns>
        protected override OperateResult InitializationOnConnect( Socket socket )
        {
            // 注册会话信息
            OperateResult<byte[], byte[]> read1 = ReadFromCoreServerBase( socket, RegisterSessionHandle( ) );
            if (!read1.IsSuccess) return read1;

            // 检查返回的状态
            OperateResult check1 = CheckResponse( read1.Content1 );
            if (!check1.IsSuccess) return check1;

            // 提取会话ID
            SessionHandle = ByteTransform.TransUInt32( read1.Content1, 4 );

            // 打开注册
            OperateResult<byte[], byte[]> read2 = ReadFromCoreServerBase( socket, SendRRData( ) );
            if (!read2.IsSuccess) return read2;

            // 检查返回的状态
            OperateResult check2 = CheckResponse( read2.Content1 );
            if (!check2.IsSuccess) return check2;

            // 提取连接标识
            NetWorkConnectionID = ByteTransform.TransUInt32( read1.Content2, 20 );

            // 校验成功
            return OperateResult.CreateSuccessResult( );
        }

        #endregion

        #region Build Command

        /// <summary>
        /// 创建一个读取的报文指令
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">长度</param>
        /// <returns>包含结果对象的报文信息</returns>
        public OperateResult<byte[]> BuildReadCommand( string address, ushort length )
        {
            byte tmp = (byte)((length + 1) / 2 + 1);
            // UCMM 请求
            byte[] buff = new byte[]
            {
                0x00, 0x70,                                         // 打开请求
                0x00, 0x40,                                         // 数据长度                                                          ======= [需要修改]
                0x00, 0x00, 0x00, 0x00,                             // 会话句柄初始值
                0x00, 0x00, 0x00, 0x00,                             // 状态信息
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,     // 发送方上下文描述
                0x00, 0x00, 0x00, 0x00,                             // 选项

                0x00, 0x00, 0x00, 0x00,                             // 接口句柄，默认CIP
                0x00, 0x01,                                         // 超时
                0x00, 0x02,                                         // 项目数量
                0x00, 0xA1,                                         // 基于连接的地址项（用户连接的消息）
                0x00, 0x04,                                         // 接下来的连接标识符的数据长度
                0x00, 0x00, 0x00, 0x00,                             // 和打开应答帧中的O-T网络连接号相同，偏移量36
                0x00, 0xB1,                                         // 封装连接的传输数据包的数据项
                0x00, 0x30,                                         // 后面数据包的长度，48个字节                                      ======= [需要修改]

                0x00, 0x01,                                         // 序号，从1开始
                0x0A,                                               // 服务
                0x02,                                               // 请求路径大小
                0x01, 0x24, 0x02, 0x20,                             // 请求路径，有可能会改变
                0x00, 0x01,                                         // 请求数据点的个数
                0x00, 0x01,                                         // 从服务数第一个字节算起，每个服务的偏移量

                0x4C,                                               // 服务标识
                tmp,                                                // 请求路径大小
                0x91,                                               // 扩展符号
                0x01,                                               // 数据大小，该服务所对应的PLC中的测点名大小
                0x1,0x2,                                            // 数据内容，该服务所对应的PLC中的测点名
                0x00, 0x01,                                         // 服务命令指定数据
            };

            ByteTransform.TransByte( SessionHandle ).CopyTo( buff, 4 );
            return OperateResult.CreateSuccessResult( buff );
        }

        /// <summary>
        /// 创建一个写入的报文指令
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">数据</param>
        /// <returns>包含结果对象的报文信息</returns>
        public OperateResult<byte[]> BuildWriteCommand( string address, byte[] data )
        {
            return null;
        }

        #endregion

        #region Handle Single

        /// <summary>
        /// 00 65 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00
        /// </summary>
        /// <returns></returns>
        private byte[] RegisterSessionHandle( )
        {
            return new byte[]
            {
                0x00, 0x65,                                             // 注册请求
                0x00, 0x04,                                             // 数据长度
                0x00, 0x00, 0x00, 0x00,                                 // 会话句柄初始值
                0x00, 0x00, 0x00, 0x00,                                 // 状态信息
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,         // 发送方上下文描述
                0x00, 0x00, 0x00, 0x00,                                 // 选项

                0x00, 0x01,                                             // 协议版本
                0x00, 0x00,                                             // 选项版本
            };
        }

        /// <summary>
        /// 00 66 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
        /// </summary>
        /// <returns></returns>
        private byte[] UnRegisterSessionHandle( )
        {
            byte[] buff = new byte[]
            {
                0x00, 0x66,                                                  // 注册请求
                0x00, 0x00,                                                  // 数据长度
                0x00, 0x00, 0x00, 0x00,                                      // 会话句柄初始值
                0x00, 0x00, 0x00, 0x00,                                      // 状态信息
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,              // 发送方上下文描述
                0x00, 0x00, 0x00, 0x00,                                      // 选项
            };

            ByteTransform.TransByte( SessionHandle ).CopyTo( buff, 4 );
            return buff;
        }

        private byte[] SendRRData( )
        {
            // UCMM 请求
            byte[] buff = new byte[]
            {
                0x00, 0x6F,                                                 // 打开请求
                0x00, 0x40,                                                 // 数据长度
                0x00, 0x00, 0x00, 0x00,                                     // 会话句柄初始值
                0x00, 0x00, 0x00, 0x00,                                     // 状态信息
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,             // 发送方上下文描述
                0x00, 0x00, 0x00, 0x00,                                     // 选项

                0x00, 0x00, 0x00, 0x00,                                     // 接口句柄，默认CIP
                0x00, 0x01,                                                 // 超时
                0x00, 0x02,                                                 // 项目数量，由于使用了一个地址项目和一个数据项目
                0x00, 0x00,                                                 // 地址类型ID，该字段应为0，表示一个UCMM消息
                0x00, 0x00,                                                 // 长度，由于UCMM消息使用NULL地址项，所以该字段应为0
                0x00, 0xB2,                                                 // 数据类型ID，该字段应该是0x00B2来封装UCMM
                0x00, 0x30,                                                 // 后面数据包的长度，48个字节

                                                                            // MR数据请求包
                0x54,                                                       // 服务
                0x02,                                                       // 请求路径大小
                0x01, 0x24, 0x06, 0x20,                                     // 请求路径，有可能会改变
                0x0A,                                                       // Priority/time_tick
                0x05,                                                       // Time-out_ticks
                0x00, 0x00, 0x00, 0x00,                                     // O-T Network Connection ID
                0x00, 0x00, 0x00, 0x00,                                     // T-O Network Connection ID  由驱动产生？？？
                0x00, 0x00,                                                 // Connection Serial Number
                0x01, 0x01,                                                 // Verder ID
                0x00, 0x00, 0x00, 0x00,                                     // Originator Serial Number 和T-O Network Connection ID相同
                0x01,                                                       // 连接超时倍数
                0x00, 0x00, 0x00,                                           // 保留数据
                0x00, 0x4C, 0X4B, 0X40,                                     // O-T RPI
                0x43, 0xF8,                                                 // O-T 网络连接参数
                0x00, 0x4C, 0X4B, 0X40,                                     // T-O RPI
                0x43, 0xF8,                                                 // T-O 网络连接参数
                0xA3,                                                       // 传输类型
                0x03,                                                       // 传输路径大小
                0x01, 0x24, 0x02, 0x20, 0x00, 0x01,                         // 连接路径
            };

            ByteTransform.TransByte( SessionHandle ).CopyTo( buff, 4 );
            return buff;
        }



        private OperateResult CheckResponse( byte[] response )
        {
            try
            {
                int status = ByteTransform.TransInt32( response, 8 );
                if (status == 0) return OperateResult.CreateSuccessResult( );

                string msg = string.Empty;
                switch (status)
                {
                    case 0x01: msg = "发件人发出无效或不受支持的封装命令。";break;
                    case 0x02: msg = "接收器中的内存资源不足以处理命令。 这不是一个应用程序错误。 相反，只有在封装层无法获得所需内存资源的情况下才会导致此问题。";break;
                    case 0x03: msg = "封装消息的数据部分中的数据形成不良或不正确。";break;
                    case 0x64: msg = "向目标发送封装消息时，始发者使用了无效的会话句柄。";break;
                    case 0x65: msg = "目标收到一个无效长度的信息";break;
                    case 0x69: msg = "不支持的封装协议修订。";break;
                    default: msg = "未知错误";break;
                }

                return new OperateResult( status, msg );
            }
            catch(Exception ex)
            {
                return new OperateResult( ) { Message = ex.Message };
            }
        }

        #endregion
    }
}
