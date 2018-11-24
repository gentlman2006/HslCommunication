using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
#if !NETSTANDARD2_0
using System.Windows.Forms;
#endif
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Drawing;

namespace HslCommunication.BasicFramework
{
    /// <summary>
    /// 一个软件基础类，提供常用的一些静态方法 ->
    /// A software-based class that provides some common static methods
    /// </summary>
    public class SoftBasic
    {
        #region MD5 Calculate


        /// <summary>
        /// 获取文件的md5码 -> Get the MD5 code of the file
        /// </summary>
        /// <param name="filePath">文件的路径，既可以是完整的路径，也可以是相对的路径 -> The path to the file</param>
        /// <returns>Md5字符串</returns>
        /// <example>
        /// 下面举例实现获取一个文件的md5码
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="CalculateFileMD5Example" title="CalculateFileMD5示例" />
        /// </example>
        public static string CalculateFileMD5( string filePath )
        {
            string str_md5 = string.Empty;
            using (FileStream fs = new FileStream( filePath, FileMode.Open, FileAccess.Read ))
            {
                str_md5 = CalculateStreamMD5( fs );
            }
            return str_md5;
        }

        /// <summary>
        /// 获取数据流的md5码 -> Get the MD5 code for the data stream
        /// </summary>
        /// <param name="stream">数据流，可以是内存流，也可以是文件流</param>
        /// <returns>Md5字符串</returns>
        /// <example>
        /// 下面举例实现获取一个流的md5码
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="CalculateStreamMD5Example1" title="CalculateStreamMD5示例" />
        /// </example>
        public static string CalculateStreamMD5( Stream stream )
        {
            MD5 md5 = new MD5CryptoServiceProvider( );
            byte[] bytes_md5 = md5.ComputeHash( stream );
            return BitConverter.ToString( bytes_md5 ).Replace( "-", "" );
        }


#if !NETSTANDARD2_0
        /// <summary>
        /// 获取内存图片的md5码 -> Get the MD5 code of the memory picture
        /// </summary>
        /// <param name="bitmap">内存图片</param>
        /// <returns>Md5字符串</returns>
        /// <example>
        /// 下面举例实现获取一个图像的md5码
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="CalculateStreamMD5Example2" title="CalculateStreamMD5示例" />
        /// </example>
        public static string CalculateStreamMD5(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, bitmap.RawFormat);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes_md5 = md5.ComputeHash(ms);
            ms.Dispose();
            return BitConverter.ToString(bytes_md5).Replace("-", "");
        }
#endif

        #endregion

        #region DataSize Format


        /// <summary>
        /// 从一个字节大小返回带单位的描述
        /// </summary>
        /// <param name="size">实际的大小值</param>
        /// <returns>最终的字符串值</returns>
        /// <example>
        /// 比如说我们获取了文件的长度，这个长度可以来自于本地，也可以来自于数据库查询
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="GetSizeDescriptionExample" title="GetSizeDescription示例" />
        /// </example>
        public static string GetSizeDescription( long size )
        {
            if (size < 1000)
            {
                return size + " B";
            }
            else if (size < 1000 * 1000)
            {
                float data = (float)size / 1024;
                return data.ToString( "F2" ) + " Kb";
            }
            else if (size < 1000 * 1000 * 1000)
            {
                float data = (float)size / 1024 / 1024;
                return data.ToString( "F2" ) + " Mb";
            }
            else
            {
                float data = (float)size / 1024 / 1024 / 1024;
                return data.ToString( "F2" ) + " Gb";
            }
        }


        #endregion

        #region Array Expaned

        /// <summary>
        /// 一个通用的数组新增个数方法，会自动判断越界情况，越界的情况下，会自动的截断或是填充 -> 
        /// A common array of new methods, will automatically determine the cross-border situation, in the case of cross-border, will be automatically truncated or filled
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="array">原数据</param>
        /// <param name="data">等待新增的数据</param>
        /// <param name="max">原数据的最大值</param>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="AddArrayDataExample" title="AddArrayData示例" />
        /// </example>
        public static void AddArrayData<T>( ref T[] array, T[] data, int max )
        {
            if (data == null) return;           // 数据为空
            if (data.Length == 0) return;       // 数据长度为空

            if (array.Length == max)
            {
                for (int i = 0; i < array.Length - data.Length; i++)
                {
                    array[i] = array[i + 1];
                }

                for (int i = 0; i < data.Length; i++)
                {
                    array[array.Length - data.Length + i] = data[i];
                }
            }
            else
            {
                if ((array.Length + data.Length) > max)
                {
                    T[] tmp = new T[max];
                    for (int i = 0; i < (max - data.Length); i++)
                    {
                        tmp[i] = array[i + (array.Length - max + data.Length)];
                    }
                    for (int i = 0; i < data.Length; i++)
                    {
                        tmp[tmp.Length - data.Length + i] = data[i];
                    }
                    // 更新数据
                    array = tmp;
                }
                else
                {
                    T[] tmp = new T[array.Length + data.Length];
                    for (int i = 0; i < array.Length; i++)
                    {
                        tmp[i] = array[i];
                    }
                    for (int i = 0; i < data.Length; i++)
                    {
                        tmp[tmp.Length - data.Length + i] = data[i];
                    }

                    array = tmp;
                }
            }
        }

        /// <summary>
        /// 将一个数组进行扩充到指定长度，或是缩短到指定长度 ->
        /// Extend an array to a specified length, or shorten to a specified length or fill
        /// </summary>
        /// <typeparam name="T">数组的类型</typeparam>
        /// <param name="data">原先数据的数据</param>
        /// <param name="length">新数组的长度</param>
        /// <returns>新数组长度信息</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ArrayExpandToLengthExample" title="ArrayExpandToLength示例" />
        /// </example>
        public static T[] ArrayExpandToLength<T>( T[] data, int length )
        {
            if (data == null) return new T[length];

            if (data.Length == length) return data;

            T[] buffer = new T[length];

            Array.Copy( data, buffer, Math.Min( data.Length, buffer.Length ) );

            return buffer;
        }


        /// <summary>
        /// 将一个数组进行扩充到偶数长度 ->
        /// Extend an array to even lengths
        /// </summary>
        /// <typeparam name="T">数组的类型</typeparam>
        /// <param name="data">原先数据的数据</param>
        /// <returns>新数组长度信息</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ArrayExpandToLengthEvenExample" title="ArrayExpandToLengthEven示例" />
        /// </example>
        public static T[] ArrayExpandToLengthEven<T>( T[] data )
        {
            if (data == null) return new T[0];

            if (data.Length % 2 == 1)
            {
                return ArrayExpandToLength( data, data.Length + 1 );
            }
            else
            {
                return data;
            }
        }

        #endregion

        #region Byte Array compare

        /// <summary>
        /// 判断两个字节的指定部分是否相同 ->
        /// Determines whether the specified portion of a two-byte is the same
        /// </summary>
        /// <param name="b1">第一个字节</param>
        /// <param name="start1">第一个字节的起始位置</param>
        /// <param name="b2">第二个字节</param>
        /// <param name="start2">第二个字节的起始位置</param>
        /// <param name="length">校验的长度</param>
        /// <returns>返回是否相等</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="IsTwoBytesEquelExample1" title="IsTwoBytesEquel示例" />
        /// </example>
        public static bool IsTwoBytesEquel( byte[] b1, int start1, byte[] b2, int start2, int length )
        {
            if (b1 == null || b2 == null) return false;
            for (int i = 0; i < length; i++)
            {
                if (b1[i + start1] != b2[i + start2])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 判断两个字节的指定部分是否相同 ->
        /// Determines whether the specified portion of a two-byte is the same
        /// </summary>
        /// <param name="b1">第一个字节</param>
        /// <param name="b2">第二个字节</param>
        /// <returns>返回是否相等</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="IsTwoBytesEquelExample2" title="IsTwoBytesEquel示例" />
        /// </example>
        public static bool IsTwoBytesEquel( byte[] b1, byte[] b2)
        {
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            return IsTwoBytesEquel( b1, 0, b2, 0, b1.Length );
        }


        /// <summary>
        /// 判断两个数据的令牌是否相等 ->
        /// Determines whether the tokens of two data are equal
        /// </summary>
        /// <param name="head">字节数据</param>
        /// <param name="token">GUID数据</param>
        /// <returns>返回是否相等</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="IsTwoTokenEquelExample" title="IsByteTokenEquel示例" />
        /// </example>
        public static bool IsByteTokenEquel( byte[] head, Guid token )
        {
            return IsTwoBytesEquel( head, 12, token.ToByteArray( ), 0, 16 );
        }


        /// <summary>
        /// 判断两个数据的令牌是否相等 ->
        /// Determines whether the tokens of two data are equal
        /// </summary>
        /// <param name="token1">第一个令牌</param>
        /// <param name="token2">第二个令牌</param>
        /// <returns>返回是否相等</returns>
        public static bool IsTwoTokenEquel( Guid token1, Guid token2 )
        {
            return IsTwoBytesEquel( token1.ToByteArray( ), 0, token2.ToByteArray( ), 0, 16 );
        }



        #endregion

        #region Enum About


        /// <summary>
        /// 获取一个枚举类型的所有枚举值，可直接应用于组合框数据 ->
        /// Gets all the enumeration values of an enumeration type that can be applied directly to the combo box data
        /// </summary>
        /// <typeparam name="TEnum">枚举的类型值</typeparam>
        /// <returns>枚举值数组</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="GetEnumValuesExample" title="GetEnumValues示例" />
        /// </example>
        public static TEnum[] GetEnumValues<TEnum>( ) where TEnum : struct
        {
            return (TEnum[])Enum.GetValues( typeof( TEnum ) );
        }

        #endregion

        #region JSON Data Get

        /// <summary>
        /// 一个泛型方法，提供json对象的数据读取 ->
        /// A generic method that provides data read for a JSON object
        /// </summary>
        /// <typeparam name="T">读取的泛型</typeparam>
        /// <param name="json">json对象</param>
        /// <param name="value_name">值名称</param>
        /// <param name="default_value">默认值</param>
        /// <returns>值对象</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="GetValueFromJsonObjectExample" title="GetValueFromJsonObject示例" />
        /// </example>
        public static T GetValueFromJsonObject<T>( JObject json, string value_name, T default_value )
        {
            if (json.Property( value_name ) != null)
            {
                return json.Property( value_name ).Value.Value<T>( );
            }
            else
            {
                return default_value;
            }
        }



        /// <summary>
        /// 一个泛型方法，提供json对象的数据写入 ->
        /// A generic method that provides data writing to a JSON object
        /// </summary>
        /// <typeparam name="T">写入的泛型</typeparam>
        /// <param name="json">json对象</param>
        /// <param name="property">值名称</param>
        /// <param name="value">值数据</param>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="JsonSetValueExample" title="JsonSetValue示例" />
        /// </example>
        public static void JsonSetValue<T>( JObject json, string property, T value )
        {
            if (json.Property( property ) != null)
            {
                json.Property( property ).Value = new JValue( value );
            }
            else
            {
                json.Add( property, new JValue( value ) );
            }
        }


        #endregion

        #region Exception Message Format

#if !NETSTANDARD2_0

        /// <summary>
        /// 显示一个完整的错误信息 ->
        /// Displays a complete error message
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <remarks>调用本方法可以显示一个异常的详细信息</remarks>
        /// <exception cref="NullReferenceException"></exception>
        public static void ShowExceptionMessage( Exception ex )
        {
            MessageBox.Show( GetExceptionMessage( ex ) );
        }


        /// <summary>
        /// 显示一个完整的错误信息，和额外的字符串描述信息 ->
        /// Displays a complete error message, and additional string description information
        /// </summary>
        /// <param name="extraMsg">额外的描述信息</param>
        /// <remarks>调用本方法可以显示一个异常的详细信息</remarks>
        /// <param name="ex">异常对象</param>
        /// <exception cref="NullReferenceException"></exception>
        public static void ShowExceptionMessage( string extraMsg, Exception ex )
        {
            MessageBox.Show( GetExceptionMessage( extraMsg, ex ) );
        }

#endif

        /// <summary>
        /// 获取一个异常的完整错误信息 ->
        /// Gets the complete error message for an exception
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <returns>完整的字符串数据</returns>
        /// <remarks>获取异常的完整信息</remarks>
        /// <exception cref="NullReferenceException">ex不能为空</exception>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="GetExceptionMessageExample1" title="GetExceptionMessage示例" />
        /// </example>
        public static string GetExceptionMessage( Exception ex )
        {
            return StringResources.Language.ExceptionMessage + ex.Message + Environment.NewLine +
                StringResources.Language.ExceptionStackTrace + ex.StackTrace + Environment.NewLine +
                StringResources.Language.ExceptopnTargetSite + ex.TargetSite;
        }

        /// <summary>
        /// 获取一个异常的完整错误信息，和额外的字符串描述信息 ->
        /// Gets the complete error message for an exception, and additional string description information
        /// </summary>
        /// <param name="extraMsg">额外的信息</param>
        /// <param name="ex">异常对象</param>
        /// <returns>完整的字符串数据</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="GetExceptionMessageExample2" title="GetExceptionMessage示例" />
        /// </example>
        public static string GetExceptionMessage( string extraMsg, Exception ex )
        {
            if (string.IsNullOrEmpty( extraMsg ))
            {
                return GetExceptionMessage( ex );
            }
            else
            {
                return extraMsg + Environment.NewLine + GetExceptionMessage( ex );
            }
        }


        #endregion

        #region Hex string and Byte[] transform


        /// <summary>
        /// 字节数据转化成16进制表示的字符串 ->
        /// Byte data into a string of 16 binary representations
        /// </summary>
        /// <param name="InBytes">字节数组</param>
        /// <returns>返回的字符串</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ByteToHexStringExample1" title="ByteToHexString示例" />
        /// </example>
        public static string ByteToHexString( byte[] InBytes )
        {
            return ByteToHexString( InBytes, (char)0 );
        }

        /// <summary>
        /// 字节数据转化成16进制表示的字符串 ->
        /// Byte data into a string of 16 binary representations
        /// </summary>
        /// <param name="InBytes">字节数组</param>
        /// <param name="segment">分割符</param>
        /// <returns>返回的字符串</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ByteToHexStringExample2" title="ByteToHexString示例" />
        /// </example>
        public static string ByteToHexString( byte[] InBytes, char segment )
        {
            StringBuilder sb = new StringBuilder( );
            foreach (byte InByte in InBytes)
            {
                if (segment == 0) sb.Append( string.Format( "{0:X2}", InByte ) );
                else sb.Append( string.Format( "{0:X2}{1}", InByte, segment ) );
            }

            if (segment != 0 && sb.Length > 1 && sb[sb.Length - 1] == segment)
            {
                sb.Remove( sb.Length - 1, 1 );
            }
            return sb.ToString( );
        }



        /// <summary>
        /// 字符串数据转化成16进制表示的字符串 ->
        /// String data into a string of 16 binary representations
        /// </summary>
        /// <param name="InString">输入的字符串数据</param>
        /// <returns>返回的字符串</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string ByteToHexString( string InString )
        {
            return ByteToHexString( Encoding.Unicode.GetBytes( InString ) );
        }


        private static List<char> hexCharList = new List<char>( )
            {
                '0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'
            };

        /// <summary>
        /// 将16进制的字符串转化成Byte数据，将检测每2个字符转化，也就是说，中间可以是任意字符 ->
        /// Converts a 16-character string into byte data, which will detect every 2 characters converted, that is, the middle can be any character
        /// </summary>
        /// <param name="hex">十六进制的字符串，中间可以是任意的分隔符</param>
        /// <returns>转换后的字节数组</returns>
        /// <remarks>参数举例：AA 01 34 A8</remarks>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="HexStringToBytesExample" title="HexStringToBytes示例" />
        /// </example>
        public static byte[] HexStringToBytes( string hex )
        {
            hex = hex.ToUpper( );

            MemoryStream ms = new MemoryStream( );

            for (int i = 0; i < hex.Length; i++)
            {
                if ((i + 1) < hex.Length)
                {
                    if (hexCharList.Contains( hex[i] ) && hexCharList.Contains( hex[i + 1] ))
                    {
                        // 这是一个合格的字节数据
                        ms.WriteByte( (byte)(hexCharList.IndexOf( hex[i] ) * 16 + hexCharList.IndexOf( hex[i + 1] )) );
                        i++;
                    }
                }
            }

            byte[] result = ms.ToArray( );
            ms.Dispose( );
            return result;
        }

        #endregion

        #region Byte Reverse By Word

        /// <summary>
        /// 将byte数组按照双字节进行反转，如果为单数的情况，则自动补齐 ->
        /// Reverses the byte array by double byte, or if the singular is the case, automatically
        /// </summary>
        /// <param name="inBytes">输入的字节信息</param>
        /// <returns>反转后的数据</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="BytesReverseByWord" title="BytesReverseByWord示例" />
        /// </example>
        public static byte[] BytesReverseByWord(byte[] inBytes )
        {
            if (inBytes == null) return null;
            byte[] buffer = ArrayExpandToLengthEven( inBytes );

            for (int i = 0; i < buffer.Length / 2; i++)
            {
                byte tmp = buffer[i * 2 + 0];
                buffer[i * 2 + 0] = buffer[i * 2 + 1];
                buffer[i * 2 + 1] = tmp;
            }

            return buffer;
        }

        #endregion

        #region Byte[] and AsciiByte[] transform

        /// <summary>
        /// 将原始的byte数组转换成ascii格式的byte数组 ->
        /// Converts the original byte array to an ASCII-formatted byte array
        /// </summary>
        /// <param name="inBytes">等待转换的byte数组</param>
        /// <returns>转换后的数组</returns>
        public static byte[] BytesToAsciiBytes( byte[] inBytes )
        {
            return Encoding.ASCII.GetBytes( ByteToHexString( inBytes ) );
        }

        /// <summary>
        /// 将ascii格式的byte数组转换成原始的byte数组 ->
        /// Converts an ASCII-formatted byte array to the original byte array
        /// </summary>
        /// <param name="inBytes">等待转换的byte数组</param>
        /// <returns>转换后的数组</returns>
        public static byte[] AsciiBytesToBytes( byte[] inBytes )
        {
            return HexStringToBytes( Encoding.ASCII.GetString( inBytes ) );
        }


        #endregion

        #region Bool[] and byte[] transform


        /// <summary>
        /// 将bool数组转换到byte数组 ->
        /// Converting a bool array to a byte array
        /// </summary>
        /// <param name="array">bool数组</param>
        /// <returns>转换后的字节数组</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="BoolArrayToByte" title="BoolArrayToByte示例" />
        /// </example>
        public static byte[] BoolArrayToByte( bool[] array )
        {
            if (array == null) return null;

            int length = array.Length % 8 == 0 ? array.Length / 8 : array.Length / 8 + 1;
            byte[] buffer = new byte[length];

            for (int i = 0; i < array.Length; i++)
            {
                int index = i / 8;
                int offect = i % 8;

                byte temp = 0;
                switch (offect)
                {
                    case 0: temp = 0x01; break;
                    case 1: temp = 0x02; break;
                    case 2: temp = 0x04; break;
                    case 3: temp = 0x08; break;
                    case 4: temp = 0x10; break;
                    case 5: temp = 0x20; break;
                    case 6: temp = 0x40; break;
                    case 7: temp = 0x80; break;
                    default: break;
                }

                if (array[i]) buffer[index] += temp;
            }

            return buffer;
        }

        /// <summary>
        /// 从Byte数组中提取位数组，length代表位数 ->
        /// Extracts a bit array from a byte array, length represents the number of digits
        /// </summary>
        /// <param name="InBytes">原先的字节数组</param>
        /// <param name="length">想要转换的长度，如果超出自动会缩小到数组最大长度</param>
        /// <returns>转换后的bool数组</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ByteToBoolArray" title="ByteToBoolArray示例" />
        /// </example> 
        public static bool[] ByteToBoolArray( byte[] InBytes, int length )
        {
            if (InBytes == null) return null;

            if (length > InBytes.Length * 8) length = InBytes.Length * 8;
            bool[] buffer = new bool[length];

            for (int i = 0; i < length; i++)
            {
                int index = i / 8;
                int offect = i % 8;

                byte temp = 0;
                switch (offect)
                {
                    case 0: temp = 0x01; break;
                    case 1: temp = 0x02; break;
                    case 2: temp = 0x04; break;
                    case 3: temp = 0x08; break;
                    case 4: temp = 0x10; break;
                    case 5: temp = 0x20; break;
                    case 6: temp = 0x40; break;
                    case 7: temp = 0x80; break;
                    default: break;
                }

                if ((InBytes[index] & temp) == temp)
                {
                    buffer[i] = true;
                }
            }

            return buffer;
        }


        #endregion

        #region Byte[] Splice

        /// <summary>
        /// 拼接2个字节数组成一个数组 ->
        /// Splicing 2 bytes to to an array
        /// </summary>
        /// <param name="bytes1">数组一</param>
        /// <param name="bytes2">数组二</param>
        /// <returns>拼接后的数组</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="SpliceTwoByteArray" title="SpliceTwoByteArray示例" />
        /// </example> 
        public static byte[] SpliceTwoByteArray( byte[] bytes1, byte[] bytes2 )
        {
            if (bytes1 == null && bytes2 == null) return null;
            if (bytes1 == null) return bytes2;
            if (bytes2 == null) return bytes1;

            byte[] buffer = new byte[bytes1.Length + bytes2.Length];
            bytes1.CopyTo( buffer, 0 );
            bytes2.CopyTo( buffer, bytes1.Length );
            return buffer;
        }

        /// <summary>
        /// 将一个byte数组的前面指定位数移除，返回新的一个数组 ->
        /// Removes the preceding specified number of bits in a byte array, returning a new array
        /// </summary>
        /// <param name="value">字节数组</param>
        /// <param name="length">等待移除的长度</param>
        /// <returns>新的数据</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="BytesArrayRemoveBegin" title="BytesArrayRemoveBegin示例" />
        /// </example> 
        public static byte[] BytesArrayRemoveBegin( byte[] value, int length )
        {
            return BytesArrayRemoveDouble( value, length, 0 );
        }

        /// <summary>
        /// 将一个byte数组的后面指定位数移除，返回新的一个数组 ->
        /// Removes the specified number of digits after a byte array, returning a new array
        /// </summary>
        /// <param name="value">字节数组</param>
        /// <param name="length">等待移除的长度</param>
        /// <returns>新的数据</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="BytesArrayRemoveLast" title="BytesArrayRemoveLast示例" />
        /// </example> 
        public static byte[] BytesArrayRemoveLast( byte[] value, int length )
        {
            return BytesArrayRemoveDouble( value, 0, length );
        }

        /// <summary>
        /// 将一个byte数组的前后移除指定位数，返回新的一个数组 ->
        /// Removes a byte array before and after the specified number of bits, returning a new array
        /// </summary>
        /// <param name="value">字节数组</param>
        /// <param name="leftLength">前面的位数</param>
        /// <param name="rightLength">后面的位数</param>
        /// <returns>新的数据</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="BytesArrayRemoveDouble" title="BytesArrayRemoveDouble示例" />
        /// </example> 
        public static byte[] BytesArrayRemoveDouble( byte[] value, int leftLength, int rightLength )
        {
            if (value == null) return null;
            if (value.Length <= (leftLength + rightLength)) return new byte[0];

            byte[] buffer = new byte[value.Length - leftLength - rightLength];
            Array.Copy( value, leftLength, buffer, 0, buffer.Length );

            return buffer;
        }

        #endregion

        #region Basic Framework

        /// <summary>
        /// 设置或获取系统框架的版本号 ->
        /// Set or get the version number of the system framework
        /// </summary>
        /// <remarks>
        /// 当你要显示本组件框架的版本号的时候，就可以用这个属性来显示
        /// </remarks>
        public static SystemVersion FrameworkVersion { get; set; } = new SystemVersion( "5.6.2" );


        #endregion

        #region Deep Clone

        /// <summary>
        /// 使用序列化反序列化深度克隆一个对象，该对象需要支持序列化特性 ->
        /// Cloning an object with serialization deserialization depth that requires support for serialization attributes
        /// </summary>
        /// <param name="oringinal">源对象，支持序列化</param>
        /// <returns>新的一个实例化的对象</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="NonSerializedAttribute"></exception>
        /// <remarks>
        /// <note type="warning">
        /// <paramref name="oringinal"/> 参数必须实现序列化的特性
        /// </note>
        /// </remarks>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="DeepClone" title="DeepClone示例" />
        /// </example>
        public static object DeepClone( object oringinal )
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream( ))
            {
                BinaryFormatter formatter = new BinaryFormatter( )
                {
                    Context = new System.Runtime.Serialization.StreamingContext( System.Runtime.Serialization.StreamingContextStates.Clone )
                };
                formatter.Serialize( stream, oringinal );
                stream.Position = 0;
                return formatter.Deserialize( stream );
            }
        }


        #endregion

        #region Unique String Get

        /// <summary>
        /// 获取一串唯一的随机字符串，长度为20，由Guid码和4位数的随机数组成，保证字符串的唯一性 ->
        /// Gets a string of unique random strings with a length of 20, consisting of a GUID code and a 4-digit random number to guarantee the uniqueness of the string
        /// </summary>
        /// <returns>随机字符串数据</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="GetUniqueStringByGuidAndRandom" title="GetUniqueStringByGuidAndRandom示例" />
        /// </example>
        public static string GetUniqueStringByGuidAndRandom( )
        {
            Random random = new Random( );
            return Guid.NewGuid( ).ToString( "N" ) + random.Next( 1000, 10000 );
        }

        #endregion
    }

}
