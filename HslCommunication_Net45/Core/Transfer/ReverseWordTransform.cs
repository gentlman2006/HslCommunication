using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core
{
    /// <summary>
    /// 按照字节错位的数据转换类
    /// </summary>
    public class ReverseWordTransform : ByteTransformBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个默认的对象
        /// </summary>
        public ReverseWordTransform( )
        {
            DataFormat = DataFormat.ABCD;
        }

        #endregion

        #region Private Method

        /// <summary>
        /// 按照字节错位的方法
        /// </summary>
        /// <param name="buffer">实际的字节数据</param>
        /// <param name="index">起始字节位置</param>
        /// <param name="length">数据长度</param>
        /// <returns>处理过的数据信息</returns>
        private byte[] ReverseBytesByWord( byte[] buffer, int index, int length )
        {
            if (buffer == null) return null;

            // copy data
            byte[] tmp = new byte[length];
            for (int i = 0; i < length; i++)
            {
                tmp[i] = buffer[index + i];
            }

            // change
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

        #region Public Properties
        
        /// <summary>
        /// 字符串数据是否按照字来反转
        /// </summary>
        public bool IsStringReverse { get; set; }


        #endregion

        #region Get Value From Bytes


        /// <summary>
        /// 从缓存中提取short结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>short对象</returns>
        public override short TransInt16( byte[] buffer, int index )
        {
            return base.TransInt16( ReverseBytesByWord( buffer, index, 2 ), 0 );
        }



        /// <summary>
        /// 从缓存中提取ushort结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>ushort对象</returns>
        public override ushort TransUInt16( byte[] buffer, int index )
        {
            return base.TransUInt16( ReverseBytesByWord( buffer, index, 2 ), 0 );
        }



        /// <summary>
        /// 从缓存中提取int结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>int对象</returns>
        public override int TransInt32( byte[] buffer, int index )
        {
            return base.TransInt32( ReverseBytesByWord( buffer, index, 4 ), 0 );
        }


        /// <summary>
        /// 从缓存中提取uint结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>uint对象</returns>
        public override uint TransUInt32( byte[] buffer, int index )
        {
            return base.TransUInt32( ReverseBytesByWord( buffer, index, 4 ), 0 );
        }


        /// <summary>
        /// 从缓存中提取long结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>long对象</returns>
        public override long TransInt64( byte[] buffer, int index )
        {
            return base.TransInt64( ReverseBytesByWord( buffer, index, 8 ), 0 );
        }



        /// <summary>
        /// 从缓存中提取ulong结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>ulong对象</returns>
        public override ulong TransUInt64( byte[] buffer, int index )
        {
            return base.TransUInt64( ReverseBytesByWord( buffer, index, 8 ), 0 );
        }




        /// <summary>
        /// 从缓存中提取float结果
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="index">索引位置</param>
        /// <returns>float对象</returns>
        public override float TransSingle( byte[] buffer, int index )
        {
            return base.TransSingle( ReverseBytesByWord( buffer, index, 4 ), 0 );
        }



        /// <summary>
        /// 从缓存中提取double结果
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="index">索引位置</param>
        /// <returns>double对象</returns>
        public override double TransDouble( byte[] buffer, int index )
        {
            return base.TransDouble( ReverseBytesByWord( buffer, index, 8 ), 0 );
        }
        

        

        /// <summary>
        /// 从缓存中提取string结果，使用指定的编码
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">byte数组长度</param>
        /// <param name="encoding">字符串的编码</param>
        /// <returns>string对象</returns>
        public override string TransString( byte[] buffer, int index, int length, Encoding encoding )
        {
            byte[] tmp = TransByte( buffer, index, length );

            if(IsStringReverse)
            {
                return encoding.GetString( ReverseBytesByWord( tmp ) );
            }
            else
            {
                return encoding.GetString( tmp );
            }
        }

        #endregion

        #region Get Bytes From Value
        
        /// <summary>
        /// bool数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public override byte[] TransByte( bool[] values )
        {
            return BasicFramework.SoftBasic.BoolArrayToByte( values );
        }



        /// <summary>
        /// short数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public override byte[] TransByte( short[] values )
        {
            byte[] buffer = base.TransByte( values );
            return ReverseBytesByWord( buffer );
        }


        /// <summary>
        /// ushort数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public override byte[] TransByte( ushort[] values )
        {
            byte[] buffer = base.TransByte( values );
            return ReverseBytesByWord( buffer );
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
                ByteTransDataFormat4( ReverseBytesByWord( BitConverter.GetBytes( values[i] ) ) ).CopyTo( buffer, 4 * i );
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
                ByteTransDataFormat4( ReverseBytesByWord( BitConverter.GetBytes( values[i] ) ) ).CopyTo( buffer, 4 * i );
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
                ByteTransDataFormat8( ReverseBytesByWord( BitConverter.GetBytes( values[i] ) ) ).CopyTo( buffer, 8 * i );
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
                ByteTransDataFormat8( ReverseBytesByWord( BitConverter.GetBytes( values[i] ) ) ).CopyTo( buffer, 8 * i );
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
                ByteTransDataFormat4( ReverseBytesByWord( BitConverter.GetBytes( values[i] ) ) ).CopyTo( buffer, 4 * i );
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
                ByteTransDataFormat8( ReverseBytesByWord( BitConverter.GetBytes( values[i] ) ) ).CopyTo( buffer, 8 * i );
            }

            return buffer;
        }
        

        /// <summary>
        /// 使用指定的编码字符串转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <param name="encoding">字符串的编码方式</param>
        /// <returns>buffer数据</returns>
        public override byte[] TransByte( string value, Encoding encoding )
        {
            if (value == null) return null;
            byte[] buffer = encoding.GetBytes( value );
            buffer = BasicFramework.SoftBasic.ArrayExpandToLengthEven( buffer );
            if (IsStringReverse)
            {
                return ReverseBytesByWord( buffer );
            }
            else
            {
                return buffer;
            }
        }

        #endregion

    }
}
