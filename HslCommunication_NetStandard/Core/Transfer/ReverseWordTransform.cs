using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core
{
    /// <summary>
    /// 按照字节错位的数据转换类
    /// </summary>
    public class ReverseWordTransform : IByteTransform
    {

        #region Private Method

        /// <summary>
        /// 按照字节错位的方法
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private byte[] ReverseBytesByWord( byte[] buffer, int index, int length )
        {
            byte[] tmp = new byte[length];

            for (int i = 0; i < length; i++)
            {
                tmp[i] = buffer[index + i];
            }

            for (int i = 0; i < length / 2; i++)
            {
                byte b = tmp[i * 2 + 0];
                tmp[i * 2 + 0] = tmp[i * 2 + 1];
                tmp[i * 2 + 1] = b;
            }

            return tmp;
        }

        private byte[] ReverseBytesByWord( byte[] buffer )
        {
            return ReverseBytesByWord( buffer, 0, buffer.Length );
        }


        #endregion

        #region Get Value From Bytes

        /// <summary>
        /// 从缓存中提取出bool结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
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
            return BitConverter.ToInt16( ReverseBytesByWord( buffer, index, 2 ), 0 );
        }

        /// <summary>
        /// 从缓存中提取ushort结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>ushort对象</returns>
        public ushort TransUInt16( byte[] buffer, int index )
        {
            return BitConverter.ToUInt16( ReverseBytesByWord( buffer, index, 2 ), 0 );
        }
        /// <summary>
        /// 从缓存中提取int结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>int对象</returns>
        public int TransInt32( byte[] buffer, int index )
        {
            return BitConverter.ToInt32( ReverseBytesByWord( buffer, index, 4 ), 0 );
        }
        /// <summary>
        /// 从缓存中提取uint结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>uint对象</returns>
        public uint TransUInt32( byte[] buffer, int index )
        {
            return BitConverter.ToUInt32( ReverseBytesByWord( buffer, index, 4 ), 0 );
        }
        /// <summary>
        /// 从缓存中提取long结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>long对象</returns>
        public long TransInt64( byte[] buffer, int index )
        {
            return BitConverter.ToInt64( ReverseBytesByWord( buffer, index, 8 ), 0 );
        }
        /// <summary>
        /// 从缓存中提取ulong结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>ulong对象</returns>
        public ulong TransUInt64( byte[] buffer, int index )
        {
            return BitConverter.ToUInt64( ReverseBytesByWord( buffer, index, 8 ), 0 );
        }
        /// <summary>
        /// 从缓存中提取float结果
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="index">索引位置</param>
        /// <returns>float对象</returns>
        public float TransSingle( byte[] buffer, int index )
        {
            return BitConverter.ToSingle( ReverseBytesByWord( buffer, index, 4 ), 0 );
        }
        /// <summary>
        /// 从缓存中提取double结果
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="index">索引位置</param>
        /// <returns>double对象</returns>
        public double TransDouble( byte[] buffer, int index )
        {
            return BitConverter.ToDouble( ReverseBytesByWord( buffer, index, 8 ), 0 );
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
            return value ? new byte[1] { 0x01 } : new byte[1] { 0x00 };
        }
        /// <summary>
        /// bool数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( bool[] values )
        {
            return BasicFramework.SoftBasic.BoolArrayToByte( values );
        }

        /// <summary>
        /// byte变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( byte value )
        {
            return new byte[1] { value };
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
                byte[] tmp = BitConverter.GetBytes( values[i] );
                tmp.CopyTo( buffer, 2 * i );
            }

            return ReverseBytesByWord( buffer );
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
                byte[] tmp = BitConverter.GetBytes( values[i] );
                tmp.CopyTo( buffer, 2 * i );
            }

            return ReverseBytesByWord( buffer );
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

            return ReverseBytesByWord( buffer );
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

            return ReverseBytesByWord( buffer );
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

            return ReverseBytesByWord( buffer );
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

            return ReverseBytesByWord( buffer );
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

            return ReverseBytesByWord( buffer );
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

            return ReverseBytesByWord( buffer );
        }
        /// <summary>
        /// ASCII编码字符串转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public byte[] TransByte( string value )
        {
            return Encoding.ASCII.GetBytes( value );
        }


        #endregion

    }
}
