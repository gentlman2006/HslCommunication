using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// 生成一个请求读取数据的节点信息，CIP指令信息
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>CIP的指令信息</returns>
        public static byte[] PackRequsetRead(string address )
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
            buffer[offect++] = 0x01;
            buffer[offect++] = 0x00;

            byte[] data = new byte[offect];
            Array.Copy( buffer, 0, data, 0, offect );
            return data;
        }

        public static byte[] PackCommandSpecificData( uint connectionId, params byte[] cips )
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
            ms.WriteByte( 0xA1 );     // 连接的地址项
            ms.WriteByte( 0x00 );
            ms.WriteByte( 0x04 );     // 长度
            ms.WriteByte( 0x00 );
            ms.Write( BitConverter.GetBytes( connectionId ), 0, 4 );  // 连接标识
            ms.WriteByte( 0xB1 );     // 连接的项数
            ms.WriteByte( 0x00 );
            ms.WriteByte( 0x00 );     // 后面数据包的长度，等全部生成后在赋值
            ms.WriteByte( 0x00 );
            return null;
        }

    }
}
