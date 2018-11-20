using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.AllenBradley
{
    /// <summary>
    /// AB PLC的辅助类，用来辅助生成基本的指令信息
    /// </summary>
    public class AllenBradleyHelper
    {
        #region Static Service Code

        /// <summary>
        /// CIP命令中的读取数据的服务
        /// </summary>
        public const byte CIP_READ_DATA = 0x4C;
        /// <summary>
        /// CIP命令中的写数据的服务
        /// </summary>
        public const int CIP_WRITE_DATA = 0x4D;
        /// <summary>
        /// CIP命令中的读片段的数据服务
        /// </summary>
        public const int CIP_READ_FRAGMENT = 0x52;
        /// <summary>
        /// CIP命令中的写片段的数据服务
        /// </summary>
        public const int CIP_WRITE_FRAGMENT = 0x53;
        /// <summary>
        /// CIP命令中的对数据读取服务
        /// </summary>
        public const int CIP_MULTIREAD_DATA = 0x1000;

        #endregion

        #region DataType Code

        /// <summary>
        /// bool型数据，一个字节长度
        /// </summary>
        public const ushort CIP_Type_Bool = 0xC1;

        /// <summary>
        /// byte型数据，一个字节长度
        /// </summary>
        public const ushort CIP_Type_Byte = 0xC2;

        /// <summary>
        /// 整型，两个字节长度
        /// </summary>
        public const ushort CIP_Type_Word = 0xC3;

        /// <summary>
        /// 长整型，四个字节长度
        /// </summary>
        public const ushort CIP_Type_DWord = 0xC4;

        /// <summary>
        /// 实数数据，四个字节长度
        /// </summary>
        public const ushort CIP_Type_Real = 0xCA;

        /// <summary>
        /// 二进制数据内容
        /// </summary>
        public const ushort CIP_Type_BitArray = 0xD3;

        #endregion


        /// <summary>
        /// 将CommandSpecificData的命令，打包成可发送的数据指令
        /// </summary>
        /// <param name="command">实际的命令暗号</param>
        /// <param name="session">当前会话的id</param>
        /// <param name="commandSpecificData">CommandSpecificData命令</param>
        /// <returns>最终可发送的数据命令</returns>
        public static byte[] PackRequestHeader( ushort command, uint session, byte[] commandSpecificData )
        {
            byte[] buffer = new byte[commandSpecificData.Length + 24];
            Array.Copy( commandSpecificData, 0, buffer, 24, commandSpecificData.Length );
            BitConverter.GetBytes( command ).CopyTo( buffer, 0 );
            BitConverter.GetBytes( session ).CopyTo( buffer, 4 );
            BitConverter.GetBytes( (ushort)commandSpecificData.Length ).CopyTo( buffer, 2 );
            return buffer;
        }

        /// <summary>
        /// 打包生成一个请求读取数据的节点信息，CIP指令信息
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="length">指代数组的长度</param>
        /// <returns>CIP的指令信息</returns>
        public static byte[] PackRequsetRead( string address, int length )
        {
            byte[] buffer = new byte[1024];
            int offect = 0;
            string[] tagNames = address.Split( new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries );
            buffer[offect++] = CIP_READ_DATA;
            offect++;

            for (int i = 0; i < tagNames.Length; i++)
            {
                buffer[offect++] = 0x91;                        // 固定
                buffer[offect++] = (byte)tagNames[i].Length;    // 节点的长度值
                byte[] nameBytes = Encoding.ASCII.GetBytes( tagNames[i] );
                nameBytes.CopyTo( buffer, offect );
                offect += nameBytes.Length;
                if (nameBytes.Length % 2 == 1) offect++;
            }

            buffer[1] = (byte)((offect - 2) / 2);
            buffer[offect++] = BitConverter.GetBytes( length )[0];
            buffer[offect++] = BitConverter.GetBytes( length )[1];

            byte[] data = new byte[offect];
            Array.Copy( buffer, 0, data, 0, offect );
            return data;
        }

        /// <summary>
        /// 根据指定的数据和类型，生成对应的数据
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="typeCode">数据类型</param>
        /// <param name="value">字节值</param>
        /// <param name="length">如果节点为数组，就是数组长度</param>
        /// <returns>CIP的指令信息</returns>
        public static byte[] PackRequestWrite( string address, ushort typeCode, byte[] value, int length = 1 )
        {
            byte[] buffer = new byte[1024];
            int offect = 0;
            string[] tagNames = address.Split( new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries );
            buffer[offect++] = CIP_WRITE_DATA;
            offect++;

            for (int i = 0; i < tagNames.Length; i++)
            {
                buffer[offect++] = 0x91;                                 // 固定
                buffer[offect++] = (byte)tagNames[i].Length;             // 节点的长度值
                byte[] nameBytes = Encoding.ASCII.GetBytes( tagNames[i] );
                nameBytes.CopyTo( buffer, offect );
                offect += nameBytes.Length;
                if (nameBytes.Length % 2 == 1) offect++;
            }

            buffer[1] = (byte)((offect - 2) / 2);

            buffer[offect++] = BitConverter.GetBytes( typeCode )[0];     // 数据类型
            buffer[offect++] = BitConverter.GetBytes( typeCode )[1];

            buffer[offect++] = BitConverter.GetBytes( length )[0];       // 固定
            buffer[offect++] = BitConverter.GetBytes( length )[1];

            value.CopyTo( buffer, offect );                              // 数值
            offect += value.Length;

            byte[] data = new byte[offect];
            Array.Copy( buffer, 0, data, 0, offect );
            return data;
        }
        

        /// <summary>
        /// 生成读取直接节点数据信息的内容
        /// </summary>
        /// <param name="cips">cip指令内容</param>
        /// <returns>最终的指令值</returns>
        public static byte[] PackCommandSpecificData( params byte[][] cips )
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream( );
            ms.WriteByte( 0x00 );
            ms.WriteByte( 0x00 );
            ms.WriteByte( 0x00 );
            ms.WriteByte( 0x00 );
            ms.WriteByte( 0x01 );     // 超时
            ms.WriteByte( 0x00 );
            ms.WriteByte( 0x02 );     // 项数
            ms.WriteByte( 0x00 );
            ms.WriteByte( 0x00 );     // 连接的地址项
            ms.WriteByte( 0x00 );
            ms.WriteByte( 0x00 );     // 长度
            ms.WriteByte( 0x00 );
            ms.WriteByte( 0xB2 );     // 连接的项数
            ms.WriteByte( 0x00 );
            ms.WriteByte( 0x00 );     // 后面数据包的长度，等全部生成后在赋值
            ms.WriteByte( 0x00 );

            ms.WriteByte( 0x52 );     // 服务
            ms.WriteByte( 0x02 );     // 请求路径大小
            ms.WriteByte( 0x20 );     // 请求路径
            ms.WriteByte( 0x06 );
            ms.WriteByte( 0x24 );
            ms.WriteByte( 0x01 );
            ms.WriteByte( 0x0A );     // 超时时间
            ms.WriteByte( 0xF0 );
            ms.WriteByte( 0x00 );     // CIP指令长度
            ms.WriteByte( 0x00 );

            int count = 0;
            if (cips.Length == 1)
            {
                ms.Write( cips[0], 0, cips[0].Length );
                count += cips[0].Length;
            }
            else
            {
                ms.WriteByte( 0x0A );   // 固定
                ms.WriteByte( 0x02 );
                ms.WriteByte( 0x20 );
                ms.WriteByte( 0x02 );
                ms.WriteByte( 0x24 );
                ms.WriteByte( 0x01 );
                count += 8;

                ms.Write( BitConverter.GetBytes( (ushort)cips.Length ), 0, 2 );  // 写入项数
                ushort offect = (ushort)(0x02 + 2 * cips.Length);
                count += 2 * cips.Length;

                for (int i = 0; i < cips.Length; i++)
                {
                    ms.Write( BitConverter.GetBytes( offect ), 0, 2 );
                    offect = (ushort)(offect + cips[i].Length);
                }

                for (int i = 0; i < cips.Length; i++)
                {
                    ms.Write( cips[i], 0, cips[i].Length );
                    count += cips[i].Length;
                }
            }
            ms.WriteByte( 0x01 );     // Path Size
            ms.WriteByte( 0x00 );
            ms.WriteByte( 0x01 );     // port
            ms.WriteByte( 0x00 );

            byte[] data = ms.ToArray( );

            BitConverter.GetBytes( (short)count ).CopyTo( data, 24 );
            data[14] = BitConverter.GetBytes( (short)(data.Length - 16) )[0];
            data[15] = BitConverter.GetBytes( (short)(data.Length - 16) )[1];
            return data;
        }

        /// <summary>
        /// 从PLC反馈的数据解析
        /// </summary>
        /// <param name="response">PLC的反馈数据</param>
        /// <param name="isRead">是否是返回的操作</param>
        /// <returns>带有结果标识的最终数据</returns>
        public static OperateResult<byte[]> ExtractActualData( byte[] response, bool isRead )
        {

            List<byte> data = new List<byte>( );

            int offect = 38;
            ushort count = BitConverter.ToUInt16( response, 38 );    // 剩余总字节长度，在剩余的字节里，有可能是一项数据，也有可能是多项
            if (BitConverter.ToInt32( response, 40 ) == 0x8A)
            {
                // 多项数据
                offect = 44;
                int dataCount = BitConverter.ToUInt16( response, offect );
                for (int i = 0; i < dataCount; i++)
                {
                    int offectStart = BitConverter.ToUInt16( response, offect + 2 + i * 2 ) + offect;
                    int offectEnd = (i == dataCount - 1) ? response.Length : (BitConverter.ToUInt16( response, (offect + 4 + i * 2) ) + offect);
                    ushort err = BitConverter.ToUInt16( response, offectStart + 2 );
                    switch (err)
                    {
                        case 0x04: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.Language.AllenBradley04 };
                        case 0x05: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.Language.AllenBradley05 };
                        case 0x06:
                            {
                                if (response[offect + 2] == 0xD2 || response[offect + 2] == 0xCC)
                                    return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.Language.AllenBradley06 };
                                break;
                            }
                        case 0x0A: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.Language.AllenBradley0A };
                        case 0x13: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.Language.AllenBradley13 };
                        case 0x1C: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.Language.AllenBradley1C };
                        case 0x1E: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.Language.AllenBradley1E };
                        case 0x26: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.Language.AllenBradley26 };
                        case 0x00: break;
                        default: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.Language.UnknownError };
                    }

                    if (isRead)
                    {
                        for (int j = offectStart + 6; j < offectEnd; j++)
                        {
                            data.Add( response[j] );
                        }
                    }
                }
            }
            else
            {
                // 单项数据
                byte err = response[offect + 4];
                switch (err)
                {
                    case 0x04: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.Language.AllenBradley04 };
                    case 0x05: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.Language.AllenBradley05 };
                    case 0x06:
                        {
                            if (response[offect + 2] == 0xD2 || response[offect + 2] == 0xCC)
                                return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.Language.AllenBradley06 };
                            break;
                        }
                    case 0x0A: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.Language.AllenBradley0A };
                    case 0x13: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.Language.AllenBradley13 };
                    case 0x1C: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.Language.AllenBradley1C };
                    case 0x1E: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.Language.AllenBradley1E };
                    case 0x26: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.Language.AllenBradley26 };
                    case 0x00: break;
                    default: return new OperateResult<byte[]>( ) { ErrorCode = err, Message = StringResources.Language.UnknownError };
                }

                if (isRead)
                {
                    for (int i = offect + 8; i < offect + 2 + count; i++)
                    {
                        data.Add( response[i] );
                    }
                }
            }

            return OperateResult.CreateSuccessResult( data.ToArray( ) );
        }

    }
}
