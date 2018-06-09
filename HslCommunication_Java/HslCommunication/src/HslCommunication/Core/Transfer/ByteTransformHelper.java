package HslCommunication.Core.Transfer;

import HslCommunication.BasicFramework.SoftBasic;
import HslCommunication.Core.Types.OperateResultExOne;

public class ByteTransformHelper {

    /// <summary>
    /// 结果转换操作的基础方法，需要支持类型，及转换的委托
    /// </summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <param name="result">源</param>
    /// <param name="translator"></param>
    /// <returns></returns>
    public static <TResult> OperateResultExOne<TResult> GetResultFromBytes<TResult>( OperateResultExOne<byte[]> result, Function<byte[], TResult> translator )
    {
        OperateResultExOne<TResult> tmp = new OperateResultExOne<TResult>( );
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
            tmp.Message = "数据转化失败，源数据：" + SoftBasic.ByteToHexString( result.Content ) + " 消息：" + ex.Message;
        }

        return tmp;

    }



    /// <summary>
    /// 将指定的OperateResult类型转化
    /// </summary>
    /// <param name="result">原始的类型</param>
    /// <param name="byteTransform">数据转换方法</param>
    /// <returns>转化后的类型</returns>
    public static OperateResultExOne<Boolean> GetBoolResultFromBytes( OperateResultExOne<byte[]> result, IByteTransform byteTransform )
    {
        return GetResultFromBytes( result, m -> byteTransform.TransBool( m, 0 ) );
    }

    /// <summary>
    /// 将指定的OperateResult类型转化
    /// </summary>
    /// <param name="result">原始的类型</param>
    /// <param name="byteTransform">数据转换方法</param>
    /// <returns>转化后的类型</returns>
    public static OperateResultExOne<Byte> GetByteResultFromBytes( OperateResultExOne<byte[]> result, IByteTransform byteTransform )
    {
        return GetResultFromBytes( result, m -> byteTransform.TransByte( m, 0 ) );
    }

    /// <summary>
    /// 将指定的OperateResult类型转化
    /// </summary>
    /// <param name="result">原始的类型</param>
    /// <param name="byteTransform">数据转换方法</param>
    /// <returns>转化后的类型</returns>
    public static OperateResultExOne<Short> GetInt16ResultFromBytes( OperateResultExOne<byte[]> result, IByteTransform byteTransform )
    {
        return GetResultFromBytes( result, m -> byteTransform.TransInt16( m, 0 ) );
    }



    /// <summary>
    /// 将指定的OperateResult类型转化
    /// </summary>
    /// <param name="result">原始的类型</param>
    /// <param name="byteTransform">数据转换方法</param>
    /// <returns>转化后的类型</returns>
    public static OperateResultExOne<Integer> GetInt32ResultFromBytes( OperateResultExOne<byte[]> result, IByteTransform byteTransform )
    {
        return GetResultFromBytes( result, (byte[] m) -> byteTransform.TransInt32( m, 0 ) );
    }


    /// <summary>
    /// 将指定的OperateResult类型转化
    /// </summary>
    /// <param name="result">原始的类型</param>
    /// <param name="byteTransform">数据转换方法</param>
    /// <returns>转化后的类型</returns>
    public static OperateResultExOne<Long> GetInt64ResultFromBytes( OperateResultExOne<byte[]> result, IByteTransform byteTransform )
    {
        return GetResultFromBytes( result, m -> byteTransform.TransInt64( m, 0 ) );
    }

    /// <summary>
    /// 将指定的OperateResult类型转化
    /// </summary>
    /// <param name="result">原始的类型</param>
    /// <param name="byteTransform">数据转换方法</param>
    /// <returns>转化后的类型</returns>
    public static OperateResultExOne<Float> GetSingleResultFromBytes( OperateResultExOne<byte[]> result, IByteTransform byteTransform )
    {
        return GetResultFromBytes( result, m -> byteTransform.TransSingle( m, 0 ) );
    }

    /// <summary>
    /// 将指定的OperateResult类型转化
    /// </summary>
    /// <param name="result">原始的类型</param>
    /// <param name="byteTransform">数据转换方法</param>
    /// <returns>转化后的类型</returns>
    public static OperateResultExOne<Double> GetDoubleResultFromBytes( OperateResultExOne<byte[]> result, IByteTransform byteTransform )
    {
        return GetResultFromBytes( result, m -> byteTransform.TransDouble( m, 0 ) );
    }

    /// <summary>
    /// 将指定的OperateResult类型转化
    /// </summary>
    /// <param name="result">原始的类型</param>
    /// <param name="byteTransform">数据转换方法</param>
    /// <returns>转化后的类型</returns>
    public static OperateResultExOne<String> GetStringResultFromBytes( OperateResultExOne<byte[]> result, IByteTransform byteTransform )
    {
        return GetResultFromBytes( result, m -> byteTransform.TransString( m, 0, m.Length, Encoding.ASCII ) );
    }

}
