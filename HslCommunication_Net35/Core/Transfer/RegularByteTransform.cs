using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**********************************************************************************************
 * 
 *    说明：一般的转换类
 *    日期：2018年3月14日 17:05:30
 * 
 **********************************************************************************************/


namespace HslCommunication.Core
{

    /// <summary>
    /// 常规的字节转换类
    /// </summary>
    public class RegularByteTransform : IByteTransform
    {

        #region Get Value From Bytes

        /// <summary>
        /// 从缓存中提取出bool结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>bool对象</returns>
        public bool TransBool( byte[] buffer )
        {
            return buffer[0] != 0x00;
        }

        /// <summary>
        /// 从缓存中提取byte结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>byte对象</returns>
        public byte TransByte( byte[] buffer, int index )
        {
            return buffer[index];
        }

        /// <summary>
        /// 从缓存中提取short结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>short对象</returns>
        public short TransInt16( byte[] buffer, int index )
        {
            return BitConverter.ToInt16( buffer, index );
        }

        /// <summary>
        /// 从缓存中提取ushort结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>ushort对象</returns>
        public ushort TransUInt16( byte[] buffer, int index )
        {
            return BitConverter.ToUInt16( buffer, index );
        }

        /// <summary>
        /// 从缓存中提取int结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>int对象</returns>
        public int TransInt32( byte[] buffer, int index )
        {
            return BitConverter.ToInt32( buffer, index );
        }

        /// <summary>
        /// 从缓存中提取uint结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>uint对象</returns>
        public uint TransUInt32( byte[] buffer, int index )
        {
            return BitConverter.ToUInt32( buffer, index );
        }

        /// <summary>
        /// 从缓存中提取long结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>long对象</returns>
        public long TransInt64( byte[] buffer, int index )
        {
            return BitConverter.ToInt64( buffer, index );
        }

        /// <summary>
        /// 从缓存中提取ulong结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>ulong对象</returns>
        public ulong TransUInt64( byte[] buffer, int index )
        {
            return BitConverter.ToUInt64( buffer, index );
        }

        /// <summary>
        /// 从缓存中提取float结果
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="index">索引位置</param>
        /// <returns>float对象</returns>
        public float TransSingle( byte[] buffer, int index )
        {
            return BitConverter.ToSingle( buffer, index );
        }

        /// <summary>
        /// 从缓存中提取double结果
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="index">索引位置</param>
        /// <returns>double对象</returns>
        public double TransDouble( byte[] buffer, int index )
        {
            return BitConverter.ToDouble( buffer, index );
        }

        /// <summary>
        /// 从缓存中提取string结果，编码ASCII
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <returns>string对象</returns>
        public string TransString( byte[] buffer )
        {
            return Encoding.ASCII.GetString( buffer );
        }


        #endregion


        #region Get Bytes From Value


        /// <summary>
        /// bool变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( bool value )
        {
            return TransByte( new bool[] { value } );
        }

        /// <summary>
        /// bool数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( bool[] values )
        {
            if (values == null) return null;

            byte[] buffer = new byte[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i]) buffer[i] = 0x01;
            }

            return buffer;
        }

        /// <summary>
        /// byte变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( byte value )
        {
            return new byte[] { value };
        }


        /// <summary>
        /// short变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( short value )
        {
            return TransByte( new short[] { value } );
        }

        /// <summary>
        /// short数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( short[] values )
        {
            if (values == null) return null;

            byte[] buffer = new byte[values.Length * 2];
            for (int i = 0; i < values.Length; i++)
            {
                BitConverter.GetBytes( values[i] ).CopyTo( buffer, 2 * i );
            }

            return buffer;
        }

        /// <summary>
        /// ushort变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( ushort value )
        {
            return TransByte( new ushort[] { value } );
        }
        /// <summary>
        /// ushort数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( ushort[] values )
        {
            if (values == null) return null;

            byte[] buffer = new byte[values.Length * 2];
            for (int i = 0; i < values.Length; i++)
            {
                BitConverter.GetBytes( values[i] ).CopyTo( buffer, 2 * i );
            }

            return buffer;
        }


        /// <summary>
        /// int变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( int value )
        {
            return TransByte( new int[] { value } );
        }
        /// <summary>
        /// int数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( int[] values )
        {
            if (values == null) return null;

            byte[] buffer = new byte[values.Length * 4];
            for (int i = 0; i < values.Length; i++)
            {
                BitConverter.GetBytes( values[i] ).CopyTo( buffer, 4 * i );
            }

            return buffer;
        }
        /// <summary>
        /// uint变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( uint value )
        {
            return TransByte( new uint[] { value } );
        }
        /// <summary>
        /// uint数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( uint[] values )
        {
            if (values == null) return null;

            byte[] buffer = new byte[values.Length * 4];
            for (int i = 0; i < values.Length; i++)
            {
                BitConverter.GetBytes( values[i] ).CopyTo( buffer, 4 * i );
            }

            return buffer;
        }

        /// <summary>
        /// long变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( long value )
        {
            return TransByte( new long[] { value } );
        }
        /// <summary>
        /// long数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( long[] values )
        {
            if (values == null) return null;

            byte[] buffer = new byte[values.Length * 8];
            for (int i = 0; i < values.Length; i++)
            {
                BitConverter.GetBytes( values[i] ).CopyTo( buffer, 8 * i );
            }

            return buffer;
        }

        /// <summary>
        /// ulong变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( ulong value )
        {
            return TransByte( new ulong[] { value } );
        }
        /// <summary>
        /// ulong数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( ulong[] values )
        {
            if (values == null) return null;

            byte[] buffer = new byte[values.Length * 8];
            for (int i = 0; i < values.Length; i++)
            {
                BitConverter.GetBytes( values[i] ).CopyTo( buffer, 8 * i );
            }

            return buffer;
        }

        /// <summary>
        /// float变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( float value )
        {
            return TransByte( new float[] { value } );
        }
        /// <summary>
        /// float数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( float[] values )
        {
            if (values == null) return null;

            byte[] buffer = new byte[values.Length * 4];
            for (int i = 0; i < values.Length; i++)
            {
                BitConverter.GetBytes( values[i] ).CopyTo( buffer, 4 * i );
            }

            return buffer;
        }

        /// <summary>
        /// double变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( double value )
        {
            return TransByte( new double[] { value } );
        }
        /// <summary>
        /// double数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( double[] values )
        {
            if (values == null) return null;

            byte[] buffer = new byte[values.Length * 8];
            for (int i = 0; i < values.Length; i++)
            {
                BitConverter.GetBytes( values[i] ).CopyTo( buffer, 8 * i );
            }

            return buffer;
        }

        /// <summary>
        /// ASCII编码字符串转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( string value )
        {
            if (value == null) return null;

            return Encoding.ASCII.GetBytes( value );
        }


        #endregion


    }
}
