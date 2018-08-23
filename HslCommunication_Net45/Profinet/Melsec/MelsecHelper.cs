using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.Melsec
{
    /// <summary>
    /// 所有三菱通讯类的通用辅助工具类，包含了一些通用的静态方法，可以使用本类来获取一些原始的报文信息。详细的操作参见例子
    /// </summary>
    public class MelsecHelper
    {
        #region Melsec Fx


        #endregion



        #region Common Logic

        /// <summary>
        /// 从字节构建一个ASCII格式的地址字节
        /// </summary>
        /// <param name="value">字节信息</param>
        /// <returns>ASCII格式的地址</returns>
        internal static byte[] BuildBytesFromData( byte value )
        {
            return Encoding.ASCII.GetBytes( value.ToString( "X2" ) );
        }

        /// <summary>
        /// 从short数据构建一个ASCII格式地址字节
        /// </summary>
        /// <param name="value">short值</param>
        /// <returns>ASCII格式的地址</returns>
        internal static byte[] BuildBytesFromData( short value )
        {
            return Encoding.ASCII.GetBytes( value.ToString( "X4" ) );
        }

        /// <summary>
        /// 从ushort数据构建一个ASCII格式地址字节
        /// </summary>
        /// <param name="value">ushort值</param>
        /// <returns>ASCII格式的地址</returns>
        internal static byte[] BuildBytesFromData( ushort value )
        {
            return Encoding.ASCII.GetBytes( value.ToString( "X4" ) );
        }

        /// <summary>
        /// 从三菱的地址中构建MC协议的6字节的ASCII格式的地址
        /// </summary>
        /// <param name="address">三菱地址</param>
        /// <param name="type">三菱的数据类型</param>
        /// <returns>6字节的ASCII格式的地址</returns>
        internal static byte[] BuildBytesFromAddress( int address, MelsecMcDataType type )
        {
            return Encoding.ASCII.GetBytes( address.ToString( type.FromBase == 10 ? "D6" : "X6" ) );
        }


        /// <summary>
        /// 从字节数组构建一个ASCII格式的地址字节
        /// </summary>
        /// <param name="value">字节信息</param>
        /// <returns>ASCII格式的地址</returns>
        internal static byte[] BuildBytesFromData( byte[] value )
        {
            byte[] buffer = new byte[value.Length * 2];
            for (int i = 0; i < value.Length; i++)
            {
                BuildBytesFromData( value[i] ).CopyTo( buffer, 2 * i );
            }
            return buffer;
        }

        #endregion

        #region CRC Check

        /// <summary>
        /// 计算Fx协议指令的和校验信息
        /// </summary>
        /// <param name="data">字节数据</param>
        /// <returns>校验之后的数据</returns>
        internal static byte[] FxCalculateCRC( byte[] data )
        {
            int sum = 0;
            for (int i = 1; i < data.Length - 2; i++)
            {
                sum += data[i];
            }
            return BuildBytesFromData( (byte)sum );
        }

        /// <summary>
        /// 检查指定的和校验是否是正确的
        /// </summary>
        /// <param name="data">字节数据</param>
        /// <returns>是否成功</returns>
        internal static bool CheckCRC( byte[] data )
        {
            byte[] crc = FxCalculateCRC( data );
            if (crc[0] != data[data.Length - 2]) return false;
            if (crc[1] != data[data.Length - 1]) return false;
            return true;
        }

        #endregion
    }
}
