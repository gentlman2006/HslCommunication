using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core;

namespace HslCommunication.BasicFramework
{
    /// <summary>
    /// 一个线程安全的缓存数据块，支持动态修改，添加，并获取快照
    /// </summary>
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
            hybirdLock = new SimpleHybirdLock( );
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



        #endregion

        #region Private Member

        private int capacity = 10;                      // 缓存的容量
        private byte[] buffer;                          // 缓存的数据
        private SimpleHybirdLock hybirdLock;            // 高效的混合锁
        private IByteTransform byteTransform;           // 数据转换类

        #endregion
    }
}
