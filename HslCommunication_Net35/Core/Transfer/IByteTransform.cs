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








namespace HslCommunication.Core
{
    /// <summary>
    /// 支持转换器的基础接口
    /// </summary>
    public interface IByteTransform
    {
        #region Get Value From Bytes
        
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
        /// <param name="index">索引位置</param>
        /// <returns>byte对象</returns>
        byte TransByte( byte[] buffer, int index );

        /// <summary>
        /// 从缓存中提取byte数组结果
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        byte[] TransByte( byte[] buffer, int index, int length );


        /// <summary>
        /// 从缓存中提取short结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>short对象</returns>
        short TransInt16( byte[] buffer, int index );

        /// <summary>
        /// 从缓存中提取short数组结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>short数组对象</returns>
        short[] TransInt16( byte[] buffer, int index, int length );
        /// <summary>
        /// 从缓存中提取ushort结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>ushort对象</returns>
        ushort TransUInt16( byte[] buffer, int index );

        /// <summary>
        /// 从缓存中提取ushort数组结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>ushort数组对象</returns>
        ushort[] TransUInt16( byte[] buffer, int index, int length );



        /// <summary>
        /// 从缓存中提取int结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>int对象</returns>
        int TransInt32( byte[] buffer, int index );
        /// <summary>
        /// 从缓存中提取int数组结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">读取的数组的长度</param>
        /// <returns>int数组对象</returns>
        int[] TransInt32( byte[] buffer, int index, int length );



        /// <summary>
        /// 从缓存中提取uint结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>uint对象</returns>
        uint TransUInt32( byte[] buffer, int index );

        /// <summary>
        /// 从缓存中提取uint数组结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">读取的数组的长度</param>
        /// <returns>uint数组对象</returns>
        uint[] TransUInt32( byte[] buffer, int index, int length );

        /// <summary>
        /// 从缓存中提取long结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>long对象</returns>
        long TransInt64( byte[] buffer, int index );

        /// <summary>
        /// 从缓存中提取long数组结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">读取的数组的长度</param>
        /// <returns>long数组对象</returns>
        long[] TransInt64( byte[] buffer, int index, int length );


        /// <summary>
        /// 从缓存中提取ulong结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>ulong对象</returns>
        ulong TransUInt64( byte[] buffer, int index );

        /// <summary>
        /// 从缓存中提取ulong数组结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">读取的数组的长度</param>
        /// <returns>ulong数组对象</returns>
        ulong[] TransUInt64( byte[] buffer, int index ,int length);

        /// <summary>
        /// 从缓存中提取float结果
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="index">索引位置</param>
        /// <returns>float对象</returns>
        float TransSingle( byte[] buffer, int index );

        /// <summary>
        /// 从缓存中提取float数组结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">读取的数据长度</param>
        /// <returns></returns>
        float[] TransSingle( byte[] buffer, int index, int length );


        /// <summary>
        /// 从缓存中提取double结果
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="index">索引位置</param>
        /// <returns>double对象</returns>
        double TransDouble( byte[] buffer, int index );

        /// <summary>
        /// 从缓存中提取double数组结果
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        double[] TransDouble( byte[] buffer, int index, int length );

        /// <summary>
        /// 从缓存中提取string结果，编码ASCII
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <returns>string对象</returns>
        string TransString( byte[] buffer);


        #endregion


        #region Get Bytes From Value


        /// <summary>
        /// bool变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( bool value );
        /// <summary>
        /// bool数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( bool[] values );
        /// <summary>
        /// byte变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( byte value );
        /// <summary>
        /// short变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( short value );
        /// <summary>
        /// short数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( short[] values );
        /// <summary>
        /// ushort变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( ushort value );
        /// <summary>
        /// ushort数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( ushort[] values );
        /// <summary>
        /// int变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( int value );
        /// <summary>
        /// int数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( int[] values );
        /// <summary>
        /// uint变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( uint value );
        /// <summary>
        /// uint数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( uint[] values );
        /// <summary>
        /// long变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( long value );
        /// <summary>
        /// long数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( long[] values );
        /// <summary>
        /// ulong变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( ulong value );
        /// <summary>
        /// ulong数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( ulong[] values );
        /// <summary>
        /// float变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( float value );
        /// <summary>
        /// float数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( float[] values );
        /// <summary>
        /// double变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( double value );
        /// <summary>
        /// double数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( double[] values );
        /// <summary>
        /// ASCII编码字符串转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] TransByte( string value );


        #endregion

    }


}
