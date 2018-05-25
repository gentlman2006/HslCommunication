using HslCommunication.BasicFramework;
using HslCommunication.Core.IMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core.Net
{
    /// <summary>
    /// 设备类的基类，模式为双模式客户端
    /// </summary>
    public class NetworkDeviceBase<TNetMessage, TTransform> : NetworkDoubleBase<TNetMessage, TTransform> , IReadWriteNet where TNetMessage : INetMessage, new() where TTransform : IByteTransform, new()
    {



        public virtual OperateResult<byte[]> Read( string address, ushort length )
        {
            return new OperateResult<byte[]>( );
        }



        public virtual OperateResult<bool[]> ReadBool( string address, ushort length )
        {
            return new OperateResult<bool[]>( );
        }



        public OperateResult<bool> ReadBool( string address )
        {
            OperateResult<bool[]> read = ReadBool( address, 1 );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool>( read );

            return OperateResult.CreateSuccessResult<bool>( read.Content[0] );
        }


        #region Customer Support

        /// <summary>
        /// 读取自定义的数据类型，只要规定了写入和解析规则
        /// </summary>
        /// <typeparam name="T">类型名称</typeparam>
        /// <param name="address">起始地址</param>
        /// <returns></returns>
        public OperateResult<T> ReadCustomer<T>( string address ) where T : IDataTransfer, new()
        {
            OperateResult<T> result = new OperateResult<T>( );
            T Content = new T( );
            OperateResult<byte[]> read = Read( address, Content.ReadCount );
            if (read.IsSuccess)
            {
                Content.ParseSource( read.Content );
                result.Content = Content;
                result.IsSuccess = true;
            }
            else
            {
                result.ErrorCode = read.ErrorCode;
                result.Message = read.Message;
            }
            return result;
        }

        /// <summary>
        /// 写入自定义的数据类型到PLC去，只要规定了生成字节的方法即可
        /// </summary>
        /// <typeparam name="T">自定义类型</typeparam>
        /// <param name="address">起始地址</param>
        /// <param name="data">实例对象</param>
        /// <returns></returns>
        public OperateResult WriteCustomer<T>( string address, T data ) where T : IDataTransfer, new()
        {
            return Write( address, data.ToSource( ) );
        }


        #endregion

        #region Read Support
        

        public OperateResult<short> ReadInt16( string address )
        {
            return GetInt16ResultFromBytes( Read( address, 1 ) );
        }


        /// <summary>
        /// 读取三菱PLC中字软元件指定地址的ushort数据
        /// </summary>
        /// <param name="address">起始地址，格式为"D100"，"W1A0"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<ushort> ReadUInt16( string address )
        {
            return GetUInt16ResultFromBytes( Read( address, 1 ) );
        }

        /// <summary>
        /// 读取三菱PLC中字软元件指定地址的int数据
        /// </summary>
        /// <param name="address">起始地址，格式为"D100"，"W1A0"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<int> ReadInt32( string address )
        {
            return GetInt32ResultFromBytes( Read( address, 2 ) );
        }

        /// <summary>
        /// 读取三菱PLC中字软元件指定地址的uint数据
        /// </summary>
        /// <param name="address">起始地址，格式为"D100"，"W1A0"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<uint> ReadUInt32( string address )
        {
            return GetUInt32ResultFromBytes( Read( address, 2 ) );
        }

        /// <summary>
        /// 读取三菱PLC中字软元件指定地址的float数据
        /// </summary>
        /// <param name="address">起始地址，格式为"D100"，"W1A0"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<float> ReadFloat( string address )
        {
            return GetSingleResultFromBytes( Read( address, 2 ) );
        }

        /// <summary>
        /// 读取三菱PLC中字软元件指定地址的long数据
        /// </summary>
        /// <param name="address">起始地址，格式为"D100"，"W1A0"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<long> ReadInt64( string address )
        {
            return GetInt64ResultFromBytes( Read( address, 4 ) );
        }

        /// <summary>
        /// 读取三菱PLC中字软元件指定地址的ulong数据
        /// </summary>
        /// <param name="address">起始地址，格式为"D100"，"W1A0"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<ulong> ReadUInt64( string address )
        {
            return GetUInt64ResultFromBytes( Read( address, 4 ) );
        }

        /// <summary>
        /// 读取三菱PLC中字软元件指定地址的double数据
        /// </summary>
        /// <param name="address">起始地址，格式为"D100"，"W1A0"</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<double> ReadDouble( string address )
        {
            return GetDoubleResultFromBytes( Read( address, 4 ) );
        }

        /// <summary>
        /// 读取三菱PLC中字软元件地址地址的String数据，编码为ASCII
        /// </summary>
        /// <param name="address">起始地址，格式为"D100"，"W1A0"</param>
        /// <param name="length">字符串长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public OperateResult<string> ReadString( string address, ushort length )
        {
            return GetStringResultFromBytes( Read( address, length ) );
        }



        #endregion

        #region Write Base
        
        public virtual OperateResult Write( string address, byte[] value )
        {
            return new OperateResult( );
        }




        #endregion

        #region Write bool[]


        /// <summary>
        /// 向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据，长度为8的倍数</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, bool value )
        {
            return Write( address, new bool[] { value } );
        }

        /// <summary>
        /// 向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据，可以指定任意的长度</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, bool[] values )
        {
            return Write( address, values.Select( m => m ? (byte)0x01 : (byte)0x00 ).ToArray( ) );
        }


        #endregion

        #region Write Int16

        /// <summary>
        /// 向PLC中字软元件写入short数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, short[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入short数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, short value )
        {
            return Write( address, new short[] { value } );
        }

        #endregion

        #region Write UInt16


        /// <summary>
        /// 向PLC中字软元件写入ushort数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, ushort[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }


        /// <summary>
        /// 向PLC中字软元件写入ushort数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, ushort value )
        {
            return Write( address, new ushort[] { value } );
        }


        #endregion

        #region Write Int32

        /// <summary>
        /// 向PLC中字软元件写入int数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, int[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入int数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, int value )
        {
            return Write( address, new int[] { value } );
        }

        #endregion

        #region Write UInt32

        /// <summary>
        /// 向PLC中字软元件写入uint数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, uint[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入uint数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, uint value )
        {
            return Write( address, new uint[] { value } );
        }

        #endregion

        #region Write Float

        /// <summary>
        /// 向PLC中字软元件写入float数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, float[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入float数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, float value )
        {
            return Write( address, new float[] { value } );
        }


        #endregion

        #region Write Int64

        /// <summary>
        /// 向PLC中字软元件写入long数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, long[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入long数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, long value )
        {
            return Write( address, new long[] { value } );
        }

        #endregion

        #region Write UInt64

        /// <summary>
        /// 向PLC中字软元件写入ulong数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, ulong[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入ulong数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, ulong value )
        {
            return Write( address, new ulong[] { value } );
        }

        #endregion

        #region Write Double

        /// <summary>
        /// 向PLC中字软元件写入double数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, double[] values )
        {
            return Write( address, ByteTransform.TransByte( values ) );
        }

        /// <summary>
        /// 向PLC中字软元件写入double数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult Write( string address, double value )
        {
            return Write( address, new double[] { value } );
        }

        #endregion

        #region Write String

        /// <summary>
        /// 向PLC中字软元件写入字符串，编码格式为ASCII
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回读取结果</returns>
        public OperateResult Write( string address, string value )
        {
            byte[] temp = Encoding.ASCII.GetBytes( value );
            temp = SoftBasic.ArrayExpandToLengthEven( temp );
            return Write( address, temp );
        }

        /// <summary>
        /// 向PLC中字软元件写入指定长度的字符串,超出截断，不够补0，编码格式为ASCII
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>返回读取结果</returns>
        public OperateResult Write( string address, string value, int length )
        {
            byte[] temp = Encoding.ASCII.GetBytes( value );
            temp = SoftBasic.ArrayExpandToLength( temp, length );
            temp = SoftBasic.ArrayExpandToLengthEven( temp );
            return Write( address, temp );
        }

        /// <summary>
        /// 向PLC中字软元件写入字符串，编码格式为Unicode
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <returns>返回读取结果</returns>
        public OperateResult WriteUnicodeString( string address, string value )
        {
            byte[] temp = Encoding.Unicode.GetBytes( value );
            return Write( address, temp );
        }

        /// <summary>
        /// 向PLC中字软元件写入指定长度的字符串,超出截断，不够补0，编码格式为Unicode
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="value">要写入的实际数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>返回读取结果</returns>
        public OperateResult WriteUnicodeString( string address, string value, int length )
        {
            byte[] temp = Encoding.Unicode.GetBytes( value );
            temp = BasicFramework.SoftBasic.ArrayExpandToLength( temp, length * 2 );
            return Write( address, temp );
        }


        #endregion

    }
}
