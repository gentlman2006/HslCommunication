using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;




/*********************************************************************************************************
 * 
 *    说明：
 * 
 *    统一的数据转换中心，共有3种转换的机制
 *    1. 对等转换，字节不需要颠倒，比如三菱PLC，Hsl通信协议
 *    2. 颠倒转换，字节需要完全颠倒，比如西门子PLC
 *    3. 以2字节为单位颠倒转换，比如Modbus协议
 * 
 *********************************************************************************************************/








namespace HslCommunication.Core.Transfer
{
    /// <summary>
    /// 支持转换器的基础接口
    /// </summary>
    public interface IByteTransform
    {
        /// <summary>
        /// 从缓存中提取出bool结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <returns>bool对象</returns>
        bool TransBool( byte[] buffer);
        /// <summary>
        /// 从缓存中提取byte结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <returns>byte对象</returns>
        byte TransByte( byte[] buffer );
        /// <summary>
        /// 从缓存中提取short结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <returns>short对象</returns>
        short TransInt16( byte[] buffer );
        /// <summary>
        /// 从缓存中提取ushort结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <returns>ushort对象</returns>
        ushort TransUInt16( byte[] buffer );
        /// <summary>
        /// 从缓存中提取int结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <returns>int对象</returns>
        int TransInt32( byte[] buffer );
        /// <summary>
        /// 从缓存中提取uint结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <returns>uint对象</returns>
        uint TransUInt32( byte[] buffer );
        /// <summary>
        /// 从缓存中提取
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        long TransInt64( byte[] buffer );

        ulong TransUInt64( byte[] buffer );

        float TransSingle( byte[] buffer );

        double TransDouble( byte[] buffer );

        string TransString( byte[] buffer );






        byte[] TransByte( bool value );

        byte[] TransByte( byte value );

        byte[] TransByte( short value );

        byte[] TransByte( ushort value );

        byte[] TransByte( int value );

        byte[] TransByte( uint value );

        byte[] TransByte( long value );

        byte[] TransByte( ulong value );

        byte[] TransByte( float value );

        byte[] TransByte( double value );

        byte[] TransByte( string value );

    }


}
