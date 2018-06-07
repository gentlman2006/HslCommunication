package HslCommunication.Core.Transfer;

public interface IByteTransform {



    /**
     * 从缓存中提取出bool结果
     * @param buffer 缓存数据
     * @param index 索引位置
     * @return boolean值
     */
    boolean TransBool( byte[] buffer, int index );

    /**
     * 缓存中提取byte结果
     * @param buffer 缓存数据
     * @param index 索引位置
     * @return byte对象
     */
    byte TransByte( byte[] buffer, int index );

    /**
     * 从缓存中提取byte数组结果
     * @param buffer 缓存数据
     * @param index 索引位置
     * @param length 读取的数组长度
     * @return
     */
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
    /// <param name="length">读取的数组长度</param>
    /// <returns>int数组对象</returns>
    int[] TransInt32( byte[] buffer, int index, int length );



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
    /// <param name="length">读取的数组长度</param>
    /// <returns>long数组对象</returns>
    long[] TransInt64( byte[] buffer, int index, int length );



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
    /// <param name="length">读取的数组长度</param>
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
    /// <param name="buffer">缓存对象</param>
    /// <param name="index">索引位置</param>
    /// <param name="length">读取的数组长度</param>
    /// <returns></returns>
    double[] TransDouble( byte[] buffer, int index, int length );

    /// <summary>
    /// 从缓存中提取string结果，使用指定的编码
    /// </summary>
    /// <param name="buffer">缓存对象</param>
    /// <param name="index">索引位置</param>
    /// <param name="length">byte数组长度</param>
    /// <param name="encoding">字符串的编码</param>
    /// <returns>string对象</returns>
    String TransString( byte[] buffer, int index, int length, String encoding );




    /// <summary>
    /// bool变量转化缓存数据
    /// </summary>
    /// <param name="value">等待转化的数据</param>
    /// <returns>buffer数据</returns>
    byte[] TransByte( boolean value );

    /// <summary>
    /// bool数组变量转化缓存数据
    /// </summary>
    /// <param name="values">等待转化的数组</param>
    /// <returns>buffer数据</returns>
    byte[] TransByte( boolean[] values );

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
    /// 使用指定的编码字符串转化缓存数据
    /// </summary>
    /// <param name="value">等待转化的数据</param>
    /// <param name="encoding">字符串的编码方式</param>
    /// <returns>buffer数据</returns>
    byte[] TransByte( String value, String encoding );


}
