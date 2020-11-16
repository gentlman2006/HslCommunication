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
    /// 字节倒序的转换类
    /// </summary>
    public class ReverseBytesTransform : ByteTransformBase
    {
        #region Get Value From Bytes
        
        /// <summary>
        /// 从缓存中提取short结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>short对象</returns>
        public override short TransInt16( byte[] buffer, int index )
        {
            byte[] tmp = new byte[2];
            tmp[0] = buffer[1 + index];
            tmp[1] = buffer[0 + index];
            return BitConverter.ToInt16( tmp, 0 );
        }
        
        /// <summary>
        /// 从缓存中提取ushort结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>ushort对象</returns>
        public override ushort TransUInt16( byte[] buffer, int index )
        {
            byte[] tmp = new byte[2];
            tmp[0] = buffer[1 + index];
            tmp[1] = buffer[0 + index];
            return BitConverter.ToUInt16( tmp, 0 );
        }

        /// <summary>
        /// 从缓存中提取int结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>int对象</returns>
        public override int TransInt32( byte[] buffer, int index )
        {
            byte[] tmp = new byte[4];
            tmp[0] = buffer[3 + index];
            tmp[1] = buffer[2 + index];
            tmp[2] = buffer[1 + index];
            tmp[3] = buffer[0 + index];
            return BitConverter.ToInt32( ByteTransDataFormat4( tmp ), 0 );
        }

        /// <summary>
        /// 从缓存中提取uint结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>uint对象</returns>
        public override uint TransUInt32( byte[] buffer, int index )
        {
            byte[] tmp = new byte[4];
            tmp[0] = buffer[3 + index];
            tmp[1] = buffer[2 + index];
            tmp[2] = buffer[1 + index];
            tmp[3] = buffer[0 + index];
            return BitConverter.ToUInt32( ByteTransDataFormat4( tmp ), 0 );
        }


        /// <summary>
        /// 从缓存中提取long结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>long对象</returns>
        public override long TransInt64( byte[] buffer, int index )
        {
            byte[] tmp = new byte[8];
            tmp[0] = buffer[7 + index];
            tmp[1] = buffer[6 + index];
            tmp[2] = buffer[5 + index];
            tmp[3] = buffer[4 + index];
            tmp[4] = buffer[3 + index];
            tmp[5] = buffer[2 + index];
            tmp[6] = buffer[1 + index];
            tmp[7] = buffer[0 + index];
            return BitConverter.ToInt64( ByteTransDataFormat8( tmp ), 0 );
        }

        /// <summary>
        /// 从缓存中提取ulong结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>ulong对象</returns>
        public override ulong TransUInt64( byte[] buffer, int index )
        {
            byte[] tmp = new byte[8];
            tmp[0] = buffer[7 + index];
            tmp[1] = buffer[6 + index];
            tmp[2] = buffer[5 + index];
            tmp[3] = buffer[4 + index];
            tmp[4] = buffer[3 + index];
            tmp[5] = buffer[2 + index];
            tmp[6] = buffer[1 + index];
            tmp[7] = buffer[0 + index];
            return BitConverter.ToUInt64( ByteTransDataFormat8( tmp ), 0 );
        }

        /// <summary>
        /// 从缓存中提取float结果
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="index">索引位置</param>
        /// <returns>float对象</returns>
        public override float TransSingle( byte[] buffer, int index )
        {
            byte[] tmp = new byte[4];
            tmp[0] = buffer[3 + index];
            tmp[1] = buffer[2 + index];
            tmp[2] = buffer[1 + index];
            tmp[3] = buffer[0 + index];
            return BitConverter.ToSingle( ByteTransDataFormat4( tmp ), 0 );
        }


        /// <summary>
        /// 从缓存中提取double结果
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="index">索引位置</param>
        /// <returns>double对象</returns>
        public override double TransDouble( byte[] buffer, int index )
        {
            byte[] tmp = new byte[8];
            tmp[0] = buffer[7 + index];
            tmp[1] = buffer[6 + index];
            tmp[2] = buffer[5 + index];
            tmp[3] = buffer[4 + index];
            tmp[4] = buffer[3 + index];
            tmp[5] = buffer[2 + index];
            tmp[6] = buffer[1 + index];
            tmp[7] = buffer[0 + index];
            return BitConverter.ToDouble( ByteTransDataFormat8( tmp ), 0 );
        }



        #endregion


        #region Get Bytes From Value
        

        /// <summary>
        /// short数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public override byte[] TransByte( short[] values )
        {
            if (values == null) return null;

            byte[] buffer = new byte[values.Length * 2];
            for (int i = 0; i < values.Length; i++)
            {
                byte[] tmp = BitConverter.GetBytes( values[i] );
                Array.Reverse( tmp );
                tmp.CopyTo( buffer, 2 * i );
            }

            return buffer;
        }

        /// <summary>
        /// ushort数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public override byte[] TransByte( ushort[] values )
        {
            if (values == null) return null;

            byte[] buffer = new byte[values.Length * 2];
            for (int i = 0; i < values.Length; i++)
            {
                byte[] tmp = BitConverter.GetBytes( values[i] );
                Array.Reverse( tmp );
                tmp.CopyTo( buffer, 2 * i );
            }

            return buffer;
        }

        /// <summary>
        /// int数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public override byte[] TransByte( int[] values )
        {
            if (values == null) return null;

            byte[] buffer = new byte[values.Length * 4];
            for (int i = 0; i < values.Length; i++)
            {
                byte[] tmp = BitConverter.GetBytes( values[i] );
                Array.Reverse( tmp );
                ByteTransDataFormat4( tmp ).CopyTo( buffer, 4 * i );
            }

            return buffer;
        }

        /// <summary>
        /// uint数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public override byte[] TransByte( uint[] values )
        {
            if (values == null) return null;

            byte[] buffer = new byte[values.Length * 4];
            for (int i = 0; i < values.Length; i++)
            {
                byte[] tmp = BitConverter.GetBytes( values[i] );
                Array.Reverse( tmp );
                ByteTransDataFormat4( tmp ).CopyTo( buffer, 4 * i );
            }

            return buffer;
        }

        /// <summary>
        /// long数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public override byte[] TransByte( long[] values )
        {
            if (values == null) return null;

            byte[] buffer = new byte[values.Length * 8];
            for (int i = 0; i < values.Length; i++)
            {
                byte[] tmp = BitConverter.GetBytes( values[i] );
                Array.Reverse( tmp );
                ByteTransDataFormat8( tmp ).CopyTo( buffer, 8 * i );
            }

            return buffer;
        }

        /// <summary>
        /// ulong数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public override byte[] TransByte( ulong[] values )
        {
            if (values == null) return null;

            byte[] buffer = new byte[values.Length * 8];
            for (int i = 0; i < values.Length; i++)
            {
                byte[] tmp = BitConverter.GetBytes( values[i] );
                Array.Reverse( tmp );
                ByteTransDataFormat8( tmp ).CopyTo( buffer, 8 * i );
            }

            return buffer;
        }

        /// <summary>
        /// float数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public override byte[] TransByte( float[] values )
        {
            if (values == null) return null;

            byte[] buffer = new byte[values.Length * 4];
            for (int i = 0; i < values.Length; i++)
            {
                byte[] tmp = BitConverter.GetBytes( values[i] );
                Array.Reverse( tmp );
                ByteTransDataFormat4( tmp ).CopyTo( buffer, 4 * i );
            }

            return buffer;
        }


        /// <summary>
        /// double数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public override byte[] TransByte( double[] values )
        {
            if (values == null) return null;

            byte[] buffer = new byte[values.Length * 8];
            for (int i = 0; i < values.Length; i++)
            {
                byte[] tmp = BitConverter.GetBytes( values[i] );
                Array.Reverse( tmp );
                ByteTransDataFormat8( tmp ).CopyTo( buffer, 8 * i );
            }

            return buffer;
        }



        #endregion

    }
}
