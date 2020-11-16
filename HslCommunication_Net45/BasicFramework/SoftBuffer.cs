using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core;

namespace HslCommunication.BasicFramework
{
    /// <summary>
    /// 一个线程安全的缓存数据块，支持批量动态修改，添加，并获取快照
    /// </summary>
    /// <remarks>
    /// 这个类可以实现什么功能呢，就是你有一个大的数组，作为你的应用程序的中间数据池，允许你往byte[]数组里存放指定长度的子byte[]数组，也允许从里面拿数据，
    /// 这些操作都是线程安全的，当然，本类扩展了一些额外的方法支持，也可以直接赋值或获取基本的数据类型对象。
    /// </remarks>
    public class SoftBuffer
    {
        #region Constructor

        /// <summary>
        /// 使用默认的大小初始化缓存空间
        /// </summary>
        public SoftBuffer( )
        {
            buffer = new byte[capacity];
            hybirdLock = new SimpleHybirdLock( );
            byteTransform = new RegularByteTransform( );
        }

        /// <summary>
        /// 使用指定的容量初始化缓存数据块
        /// </summary>
        /// <param name="capacity">初始化的容量</param>
        public SoftBuffer(int capacity )
        {
            buffer = new byte[capacity];
            this.capacity = capacity;
            hybirdLock = new SimpleHybirdLock( );
            byteTransform = new RegularByteTransform( );
        }

        #endregion

        #region Byte Support

        /// <summary>
        /// 设置指定的位置的数据块，如果超出，则丢弃数据
        /// </summary>
        /// <param name="data">数据块信息</param>
        /// <param name="index">存储的索引</param>
        public void SetBytes( byte[] data, int index )
        {
            if (index < capacity && index >= 0 && data != null)
            {
                hybirdLock.Enter( );

                if ((data.Length + index) > buffer.Length)
                {
                    Array.Copy( data, 0, buffer, index, (buffer.Length - index) );
                }
                else
                {
                    data.CopyTo( buffer, index );
                }

                hybirdLock.Leave( );
            }
        }
        
        /// <summary>
        /// 设置指定的位置的数据块，如果超出，则丢弃数据
        /// </summary>
        /// <param name="data">数据块信息</param>
        /// <param name="index">存储的索引</param>
        /// <param name="length">准备拷贝的数据长度</param>
        public void SetBytes( byte[] data, int index, int length )
        {
            if (index < capacity && index >= 0 && data != null)
            {
                if (length > data.Length) length = data.Length;

                hybirdLock.Enter( );

                if ((length + index) > buffer.Length)
                {
                    Array.Copy( data, 0, buffer, index, (buffer.Length - index) );
                }
                else
                {
                    Array.Copy( data, 0, buffer, index, length );
                }

                hybirdLock.Leave( );
            }
        }

        /// <summary>
        /// 获取内存指定长度的数据信息
        /// </summary>
        /// <param name="index">起始位置</param>
        /// <param name="length">数组长度</param>
        /// <returns>返回实际的数据信息</returns>
        public byte[] GetBytes(int index, int length )
        {
            byte[] result = new byte[length];
            if (length > 0)
            {
                hybirdLock.Enter( );
                if (index >= 0 && (index + length) <= buffer.Length)
                {
                    Array.Copy( buffer, index, result, 0, length );
                }
                hybirdLock.Leave( );
            }
            return result;
        }

        /// <summary>
        /// 获取内存所有的数据信息
        /// </summary>
        /// <returns>实际的数据信息</returns>
        public byte[] GetBytes( )
        {
            return GetBytes( 0, capacity );
        }

        #endregion

        #region BCL Set Support
        
        /// <summary>
        /// 设置short类型的数据到缓存区
        /// </summary>
        /// <param name="values">short数组</param>
        /// <param name="index">索引位置</param>
        public void SetValue( short[] values, int index )
        {
            SetBytes( byteTransform.TransByte( values ), index );
        }

        /// <summary>
        /// 设置short类型的数据到缓存区
        /// </summary>
        /// <param name="value">short数值</param>
        /// <param name="index">索引位置</param>
        public void SetValue( short value, int index )
        {
            SetValue( new short[] { value }, index );
        }

        /// <summary>
        /// 设置ushort类型的数据到缓存区
        /// </summary>
        /// <param name="values">ushort数组</param>
        /// <param name="index">索引位置</param>
        public void SetValue( ushort[] values, int index )
        {
            SetBytes( byteTransform.TransByte( values ), index );
        }

        /// <summary>
        /// 设置ushort类型的数据到缓存区
        /// </summary>
        /// <param name="value">ushort数值</param>
        /// <param name="index">索引位置</param>
        public void SetValue( ushort value, int index )
        {
            SetValue( new ushort[] { value }, index );
        }

        /// <summary>
        /// 设置int类型的数据到缓存区
        /// </summary>
        /// <param name="values">int数组</param>
        /// <param name="index">索引位置</param>
        public void SetValue( int[] values, int index )
        {
            SetBytes( byteTransform.TransByte( values ), index );
        }

        /// <summary>
        /// 设置int类型的数据到缓存区
        /// </summary>
        /// <param name="value">int数值</param>
        /// <param name="index">索引位置</param>
        public void SetValue( int value, int index )
        {
            SetValue( new int[] { value }, index );
        }

        /// <summary>
        /// 设置uint类型的数据到缓存区
        /// </summary>
        /// <param name="values">uint数组</param>
        /// <param name="index">索引位置</param>
        public void SetValue(uint[] values, int index )
        {
            SetBytes( byteTransform.TransByte( values ), index );
        }

        /// <summary>
        /// 设置uint类型的数据到缓存区
        /// </summary>
        /// <param name="value">uint数值</param>
        /// <param name="index">索引位置</param>
        public void SetValue( uint value, int index )
        {
            SetValue( new uint[] { value }, index );
        }

        /// <summary>
        /// 设置float类型的数据到缓存区
        /// </summary>
        /// <param name="values">float数组</param>
        /// <param name="index">索引位置</param>
        public void SetValue( float[] values, int index )
        {
            SetBytes( byteTransform.TransByte( values ), index );
        }

        /// <summary>
        /// 设置float类型的数据到缓存区
        /// </summary>
        /// <param name="value">float数值</param>
        /// <param name="index">索引位置</param>
        public void SetValue( float value, int index )
        {
            SetValue( new float[] { value }, index );
        }

        /// <summary>
        /// 设置long类型的数据到缓存区
        /// </summary>
        /// <param name="values">long数组</param>
        /// <param name="index">索引位置</param>
        public void SetValue( long[] values, int index )
        {
            SetBytes( byteTransform.TransByte( values ), index );
        }

        /// <summary>
        /// 设置long类型的数据到缓存区
        /// </summary>
        /// <param name="value">long数值</param>
        /// <param name="index">索引位置</param>
        public void SetValue( long value, int index )
        {
            SetValue( new long[] { value }, index );
        }

        /// <summary>
        /// 设置ulong类型的数据到缓存区
        /// </summary>
        /// <param name="values">ulong数组</param>
        /// <param name="index">索引位置</param>
        public void SetValue( ulong[] values, int index )
        {
            SetBytes( byteTransform.TransByte( values ), index );
        }

        /// <summary>
        /// 设置ulong类型的数据到缓存区
        /// </summary>
        /// <param name="value">ulong数值</param>
        /// <param name="index">索引位置</param>
        public void SetValue( ulong value, int index )
        {
            SetValue( new ulong[] { value }, index );
        }

        /// <summary>
        /// 设置double类型的数据到缓存区
        /// </summary>
        /// <param name="values">double数组</param>
        /// <param name="index">索引位置</param>
        public void SetValue( double[] values, int index )
        {
            SetBytes( byteTransform.TransByte( values ), index );
        }

        /// <summary>
        /// 设置double类型的数据到缓存区
        /// </summary>
        /// <param name="value">double数值</param>
        /// <param name="index">索引位置</param>
        public void SetValue( double value, int index )
        {
            SetValue( new double[] { value }, index );
        }

        #endregion

        #region BCL Get Support

        /// <summary>
        /// 获取short类型的数组到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <param name="length">数组长度</param>
        /// <returns>short数组</returns>
        public short[] GetInt16( int index, int length )
        {
            byte[] tmp = GetBytes( index, length * 2 );
            return byteTransform.TransInt16( tmp, 0, length );
        }

        /// <summary>
        /// 获取short类型的数据到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns>short数据</returns>
        public short GetInt16( int index )
        {
            return GetInt16( index, 1 )[0];
        }

        /// <summary>
        /// 获取ushort类型的数组到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <param name="length">数组长度</param>
        /// <returns>ushort数组</returns>
        public ushort[] GetUInt16( int index, int length )
        {
            byte[] tmp = GetBytes( index, length * 2 );
            return byteTransform.TransUInt16( tmp, 0, length );
        }

        /// <summary>
        /// 获取ushort类型的数据到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns>ushort数据</returns>
        public ushort GetUInt16( int index )
        {
            return GetUInt16( index, 1 )[0];
        }

        /// <summary>
        /// 获取int类型的数组到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <param name="length">数组长度</param>
        /// <returns>int数组</returns>
        public int[] GetInt32( int index, int length )
        {
            byte[] tmp = GetBytes( index, length * 4 );
            return byteTransform.TransInt32( tmp, 0, length );
        }

        /// <summary>
        /// 获取int类型的数据到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns>int数据</returns>
        public int GetInt32( int index )
        {
            return GetInt32( index, 1 )[0];
        }

        /// <summary>
        /// 获取uint类型的数组到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <param name="length">数组长度</param>
        /// <returns>uint数组</returns>
        public uint[] GetUInt32( int index, int length )
        {
            byte[] tmp = GetBytes( index, length * 4 );
            return byteTransform.TransUInt32( tmp, 0, length );
        }

        /// <summary>
        /// 获取uint类型的数据到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns>uint数据</returns>
        public uint GetUInt32( int index )
        {
            return GetUInt32( index, 1 )[0];
        }

        /// <summary>
        /// 获取float类型的数组到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <param name="length">数组长度</param>
        /// <returns>float数组</returns>
        public float[] GetSingle( int index, int length )
        {
            byte[] tmp = GetBytes( index, length * 4 );
            return byteTransform.TransSingle( tmp, 0, length );
        }

        /// <summary>
        /// 获取float类型的数据到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns>float数据</returns>
        public float GetSingle( int index )
        {
            return GetSingle( index, 1 )[0];
        }

        /// <summary>
        /// 获取long类型的数组到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <param name="length">数组长度</param>
        /// <returns>long数组</returns>
        public long[] GetInt64( int index, int length )
        {
            byte[] tmp = GetBytes( index, length * 8 );
            return byteTransform.TransInt64( tmp, 0, length );
        }

        /// <summary>
        /// 获取long类型的数据到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns>long数据</returns>
        public long GetInt64( int index )
        {
            return GetInt64( index, 1 )[0];
        }

        /// <summary>
        /// 获取ulong类型的数组到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <param name="length">数组长度</param>
        /// <returns>ulong数组</returns>
        public ulong[] GetUInt64( int index, int length )
        {
            byte[] tmp = GetBytes( index, length * 8 );
            return byteTransform.TransUInt64( tmp, 0, length );
        }

        /// <summary>
        /// 获取ulong类型的数据到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns>ulong数据</returns>
        public ulong GetUInt64( int index )
        {
            return GetUInt64( index, 1 )[0];
        }

        /// <summary>
        /// 获取double类型的数组到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <param name="length">数组长度</param>
        /// <returns>ulong数组</returns>
        public double[] GetDouble( int index, int length )
        {
            byte[] tmp = GetBytes( index, length * 8 );
            return byteTransform.TransDouble( tmp, 0, length );
        }

        /// <summary>
        /// 获取double类型的数据到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns>double数据</returns>
        public double GetDouble( int index )
        {
            return GetUInt64( index, 1 )[0];
        }

        #endregion

        #region Private Member

        private int capacity = 10;                      // 缓存的容量
        private byte[] buffer;                          // 缓存的数据
        private SimpleHybirdLock hybirdLock;            // 高效的混合锁
        private IByteTransform byteTransform;           // 数据转换类

        #endregion
    }
}
