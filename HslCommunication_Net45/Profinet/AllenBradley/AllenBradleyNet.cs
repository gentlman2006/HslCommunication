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
    // [Obsolete("还未完成了类库，由于报文资料不足，暂时无法实现")]
    public class AllenBradleyNet : NetworkDeviceBase<AllenBradleyMessage, RegularByteTransform>
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

        public ushort GetCipNumber( )
        {
            ushort result = 0;
            SimpleHybird.Enter( );
            result = CIPNumber;
            SimpleHybird.Leave( );
            return result;
        }

        private ushort GetIncreseCipNumber( )
        {
            ushort result = 0;
            SimpleHybird.Enter( );
            result = CIPNumber;
            CIPNumber++;
            SimpleHybird.Leave( );
            return result;
        }

        private ushort CIPNumber = 0;
        private HslCommunication.Core.SimpleHybirdLock SimpleHybird = new SimpleHybirdLock( );



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
            //OperateResult<byte[], byte[]> read2 = ReadFromCoreServerBase( socket, SendRRData( ) );
            //if (!read2.IsSuccess) return read2;

            //// 检查返回的状态
            //OperateResult check2 = CheckResponse( read2.Content1 );
            //if (!check2.IsSuccess) return check2;

            // 提取连接标识
            //NetWorkConnectionID = ByteTransform.TransUInt32( read2.Content2, 20 );

            // 校验成功
            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        protected override OperateResult ExtraOnDisconnect( Socket socket )
        {
            // 注册会话信息
            OperateResult<byte[], byte[]> read1 = ReadFromCoreServerBase( socket, UnRegisterSessionHandle( ) );
            if (!read1.IsSuccess) return read1;

            return OperateResult.CreateSuccessResult( );
        }

        #endregion

        #region Build Command

        /// <summary>
        /// 创建一个读取的报文指令
        /// </summary>
        /// <param name="address">tag名的地址</param>
        /// <returns>包含结果对象的报文信息</returns>
        public OperateResult<byte[]> BuildReadCommand( string address )
        {
            byte[] cip = AllenBradleyHelper.PackRequsetRead( address );
            return OperateResult.CreateSuccessResult( AllenBradleyHelper.PackRequestHeader( 0x6F, SessionHandle, AllenBradleyHelper.PackCommandSpecificData2( NetWorkConnectionID, cip ) ) );
        }

        /// <summary>
        /// 创建一个写入的报文指令
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">数据</param>
        /// <returns>包含结果对象的报文信息</returns>
        public OperateResult<byte[]> BuildWriteCommand( string address, byte[] data )
        {
            return new OperateResult<byte[]>();
        }

        #endregion

        #region Override Read

        /// <summary>
        /// 读取数据信息
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            // 指令生成
            OperateResult<byte[]> command = BuildReadCommand( address );
            if (!command.IsSuccess) return command;

            // 核心交互
            OperateResult<byte[]> read = ReadFromCoreServer( command.Content );
            if(!read.IsSuccess) return read;

            // 检查反馈
            OperateResult check = CheckResponse( read.Content );
            if (!check.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( check );

            // 提取数据
            return ExtractActualData( read.Content );
        }

        
        #endregion

        #region Handle Single

        /// <summary>
        /// 65 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 [发送]
        /// 65 00 04 00 01 00 06 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 [返回]
        /// </summary>
        /// <returns></returns>
        public byte[] RegisterSessionHandle( )
        {
            byte[] commandSpecificData = new byte[] { 0x01, 0x00, 0x00, 0x00, };
            return AllenBradleyHelper.PackRequestHeader( 0x65, 0, commandSpecificData );
        }

        /// <summary>
        /// 66 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
        /// </summary>
        /// <returns></returns>
        private byte[] UnRegisterSessionHandle( )
        {
            return AllenBradleyHelper.PackRequestHeader( 0x66, SessionHandle, new byte[0] );
        }



        ///// <summary>
        ///// 6F 00 3E 00 00 00 00 00
        ///// </summary>
        ///// <returns></returns>
        //public byte[] SendRRData( )
        //{
        //    // UCMM 请求
        //    byte[] buff = new byte[]
        //    {
        //        0x00, 0x00, 0x00, 0x00,                                     // 接口句柄，默认CIP
        //        0x20, 0x00,                                                 // 超时
        //        0x02, 0x00,                                                 // 项目数量，由于使用了一个地址项目和一个数据项目
        //        0x00, 0x00,                                                 // 地址类型ID，该字段应为0，表示一个UCMM消息
        //        0x00, 0x00,                                                 // 长度，由于UCMM消息使用NULL地址项，所以该字段应为0
        //        0xB2, 0x00,                                                 // 数据类型ID，该字段应该是0x00B2来封装UCMM
        //        0x2E, 0x00,                                                 // 后面数据包的长度，46个字节

        //                                                                    // MR数据请求包
        //        0x54,                                                       // 服务
        //        0x02,                                                       // 请求路径大小
        //        0x20, 0x06, 0x24, 0x01,                                     // 请求路径，有可能会改变
        //        0x07,                                                       // Priority/time_tick
        //        0xF9,                                                       // Time-out_ticks
        //        0x00, 0x00, 0x00, 0x00,                                     // O-T Network Connection ID
        //        0x08, 0x00, 0xFE, 0x80,                                     // T-O Network Connection ID  由驱动产生？？？
        //        0x09, 0x00,                                                 // Connection Serial Number
        //        0x4D, 0x00,                                                 // Verder ID
        //        0x8B, 0x50, 0xD4, 0x0F,                                     // Originator Serial Number 和T-O Network Connection ID相同
        //        0x01,                                                       // 连接超时倍数
        //        0x00, 0x00, 0x00,                                           // 保留数据
        //        0x00, 0x12, 0X7A, 0X00,                                     // O-T RPI
        //        0xF4, 0x43,                                                 // O-T 网络连接参数
        //        0x00, 0x12, 0X7A, 0X00,                                     // T-O RPI
        //        0xF4, 0x43,                                                 // T-O 网络连接参数
        //        0xA3,                                                       // 传输类型
        //        0x02,                                                       // 传输路径大小，以字为单位
        //        0x20, 0x02, 0x24, 0x01,                                     // 连接路径，消息路由，实例
        //    };

        //    return AllenBradleyHelper.PackRequestHeader( 0x6F, SessionHandle, buff );
        //}
        

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

        #region Static Member

        /// <summary>
        /// 从PLC反馈的数据解析
        /// </summary>
        /// <param name="response">PLC的反馈数据</param>
        /// <returns>带有结果标识的最终数据</returns>
        public static OperateResult<byte[]> ExtractActualData(byte[] response )
        {

            List<byte> data = new List<byte>( );

            int offect = 38;
            while ( offect < response.Length)
            {
                ushort count = BitConverter.ToUInt16( response, offect );
                byte err = response[offect + 4];
                switch (err)
                {
                    case 0x04: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.AllenBradley04 };
                    case 0x05: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.AllenBradley05 };
                    case 0x06:
                        {
                            if (response[offect + 2] != 0xD2 && response[offect + 2] != 0xCC)
                                return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.AllenBradley06 };
                            break;
                        }
                    case 0x0A: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.AllenBradley0A };
                    case 0x13: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.AllenBradley13 };
                    case 0x1C: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.AllenBradley1C };
                    case 0x1E: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.AllenBradley1E };
                    case 0x26: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.AllenBradley26 };
                    case 0x00: break;
                    default: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.UnknownError };
                }

                for (int i = offect + 8; i < offect + 2+ count; i++)
                {
                    data.Add( response[offect] );
                }

                offect += count + 2;
            }
            return OperateResult.CreateSuccessResult( data.ToArray( ) );
        }

        #endregion
    }
}
