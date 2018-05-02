using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core
{
    /// <summary>
    /// 所有数据转换类的静态辅助方法
    /// </summary>
    public static class ByteTransformHelper
    {

        /// <summary>
        /// 结果转换操作的基础方法，需要支持类型，及转换的委托
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="result">源</param>
        /// <param name="translator"></param>
        /// <returns></returns>
        public static OperateResult<TResult> GetResultFromBytes<TResult>( OperateResult<byte[]> result, Func<byte[], TResult> translator )
        {
            var tmp = new OperateResult<TResult>( );
            try
            {
                if (result.IsSuccess)
                {
                    tmp.Content = translator( result.Content );
                    tmp.IsSuccess = result.IsSuccess;
                }
                tmp.CopyErrorFromOther( result );
            }
            catch (Exception ex)
            {
                tmp.Message = "数据转化失败，源数据：" + BasicFramework.SoftBasic.ByteToHexString( result.Content ) + " 消息：" + ex.Message;
            }

            return tmp;
        }



        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<bool> GetBoolResultFromBytes( OperateResult<byte[]> result, IByteTransform byteTransform )
        {
            return GetResultFromBytes( result, m => byteTransform.TransBool( m, 0 ) );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<byte> GetByteResultFromBytes( OperateResult<byte[]> result, IByteTransform byteTransform )
        {
            return GetResultFromBytes( result, m => byteTransform.TransByte( m, 0 ) );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<short> GetInt16ResultFromBytes( OperateResult<byte[]> result, IByteTransform byteTransform )
        {
            return GetResultFromBytes( result, m => byteTransform.TransInt16( m, 0 ) );
        }


        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<ushort> GetUInt16ResultFromBytes( OperateResult<byte[]> result, IByteTransform byteTransform )
        {
            return GetResultFromBytes( result, m => byteTransform.TransUInt16( m, 0 ) );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<int> GetInt32ResultFromBytes( OperateResult<byte[]> result, IByteTransform byteTransform )
        {
            return GetResultFromBytes( result, m => byteTransform.TransInt32( m, 0 ) );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<uint> GetUInt32ResultFromBytes( OperateResult<byte[]> result, IByteTransform byteTransform )
        {
            return GetResultFromBytes( result, m => byteTransform.TransUInt32( m, 0 ) );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<long> GetInt64ResultFromBytes( OperateResult<byte[]> result, IByteTransform byteTransform )
        {
            return GetResultFromBytes( result, m => byteTransform.TransInt64( m, 0 ) );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<ulong> GetUInt64ResultFromBytes( OperateResult<byte[]> result, IByteTransform byteTransform )
        {
            return GetResultFromBytes( result, m => byteTransform.TransUInt64( m, 0 ) );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<float> GetSingleResultFromBytes( OperateResult<byte[]> result, IByteTransform byteTransform )
        {
            return GetResultFromBytes( result, m => byteTransform.TransSingle( m, 0 ) );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<double> GetDoubleResultFromBytes( OperateResult<byte[]> result, IByteTransform byteTransform )
        {
            return GetResultFromBytes( result, m => byteTransform.TransDouble( m, 0 ) );
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<string> GetStringResultFromBytes( OperateResult<byte[]> result, IByteTransform byteTransform )
        {
            return GetResultFromBytes( result, m => byteTransform.TransString( m, 0, m.Length, Encoding.ASCII ) );
        }


    }
}
