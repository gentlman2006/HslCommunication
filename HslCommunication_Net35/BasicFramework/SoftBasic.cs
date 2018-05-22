using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Drawing;

namespace HslCommunication.BasicFramework
{
    /// <summary>
    /// 一个软件基础类，提供常用的一些静态方法
    /// </summary>
    public class SoftBasic
    {
        #region MD5码计算块


        /// <summary>
        /// 获取文件的md5码
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string CalculateFileMD5(string filePath)
        {
            string str_md5 = string.Empty;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                str_md5 = CalculateStreamMD5(fs);
            }
            return str_md5;
        }

        /// <summary>
        /// 获取数据流的md5码
        /// </summary>
        /// <param name="stream">数据流，可以是内存流，也可以是文件流</param>
        /// <returns></returns>
        public static string CalculateStreamMD5(Stream stream)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes_md5 = md5.ComputeHash(stream);
            return BitConverter.ToString(bytes_md5).Replace("-", "");
        }

        /// <summary>
        /// 获取内存图片的md5码
        /// </summary>
        /// <param name="bitmap">内存图片</param>
        /// <returns></returns>
        public static string CalculateStreamMD5(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, bitmap.RawFormat);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes_md5 = md5.ComputeHash(ms);
            ms.Dispose();
            return BitConverter.ToString(bytes_md5).Replace("-", "");
        }

        #endregion

        #region 数据大小相关


        /// <summary>
        /// 从一个字节大小返回带单位的描述
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GetSizeDescription(long size)
        {
            if (size < 1000)
            {
                return size + " B";
            }
            else if (size < 1000 * 1000)
            {
                float data = (float)size / 1024;
                return data.ToString("F2") + " Kb";
            }
            else if (size < 1000 * 1000 * 1000)
            {
                float data = (float)size / 1024 / 1024;
                return data.ToString("F2") + " Mb";
            }
            else
            {
                float data = (float)size / 1024 / 1024 / 1024;
                return data.ToString("F2") + " Gb";
            }
        }


        #endregion

        #region 数组处理方法

        /// <summary>
        /// 一个通用的数组新增个数方法，会自动判断越界情况，越界的情况下，会自动的截断或是填充
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="array">原数据</param>
        /// <param name="data">等待新增的数据</param>
        /// <param name="max">原数据的最大值</param>
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
        /// 将一个数组进行扩充到指定长度，或是缩短到指定长度
        /// </summary>
        /// <typeparam name="T">数组的类型</typeparam>
        /// <param name="data">原先数据的数据</param>
        /// <param name="length">新数组的长度</param>
        /// <returns>新数组长度信息</returns>
        public static T[] ArrayExpandToLength<T>( T[] data, int length )
        {
            if (data == null) return new T[length];

            if (data.Length == length) return data;

            T[] buffer = new T[length];

            Array.Copy( data, buffer, Math.Min( data.Length, buffer.Length ) );

            return buffer;
        }


        /// <summary>
        /// 将一个数组进行扩充到偶数长度
        /// </summary>
        /// <typeparam name="T">数组的类型</typeparam>
        /// <param name="data">原先数据的数据</param>
        /// <returns>新数组长度信息</returns>
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

        #region 数组比较

        /// <summary>
        /// 判断两个字节的指定部分是否相同
        /// </summary>
        /// <param name="b1">第一个字节</param>
        /// <param name="start1">第一个字节的起始位置</param>
        /// <param name="b2">第二个字节</param>
        /// <param name="start2">第二个字节的起始位置</param>
        /// <param name="length">校验的长度</param>
        /// <returns>返回是否相等</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static bool IsTwoBytesEquel(byte[] b1, int start1, byte[] b2, int start2, int length)
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
        /// 判断两个数据的令牌是否相等
        /// </summary>
        /// <param name="head">字节数据</param>
        /// <param name="token">GUID数据</param>
        /// <returns>返回是否相等</returns>
        public static bool IsByteTokenEquel(byte[] head, Guid token)
        {
            return IsTwoBytesEquel(head, 12, token.ToByteArray(), 0, 16);
        }


        /// <summary>
        /// 判断两个数据的令牌是否相等
        /// </summary>
        /// <param name="token1">第一个令牌</param>
        /// <param name="token2">第二个令牌</param>
        /// <returns>返回是否相等</returns>
        public static bool IsTwoTokenEquel(Guid token1, Guid token2)
        {
            return IsTwoBytesEquel(token1.ToByteArray(), 0, token2.ToByteArray(), 0, 16);
        }



        #endregion

        #region 枚举相关块


        /// <summary>
        /// 获取一个枚举类型的所有枚举值，可直接应用于组合框数据
        /// </summary>
        /// <typeparam name="TEnum">枚举的类型值</typeparam>
        /// <returns>枚举值数组</returns>
        public static TEnum[] GetEnumValues<TEnum>() where TEnum : struct
        {
            return (TEnum[])Enum.GetValues(typeof(TEnum));
        }

        #endregion

        #region JSON数据提取相关块

        /// <summary>
        /// 一个泛型方法，提供json对象的数据读取
        /// </summary>
        /// <typeparam name="T">读取的泛型</typeparam>
        /// <param name="json">json对象</param>
        /// <param name="value_name">值名称</param>
        /// <param name="default_value">默认值</param>
        /// <returns></returns>
        public static T GetValueFromJsonObject<T>(JObject json, string value_name, T default_value)
        {
            if (json.Property(value_name) != null)
            {
                return json.Property(value_name).Value.Value<T>();
            }
            else
            {
                return default_value;
            }
        }



        /// <summary>
        /// 一个泛型方法，提供json对象的数据写入
        /// </summary>
        /// <typeparam name="T">写入的泛型</typeparam>
        /// <param name="json">json对象</param>
        /// <param name="property">值名称</param>
        /// <param name="value">值数据</param>
        public static void JsonSetValue<T>(JObject json, string property, T value)
        {
            if (json.Property(property) != null)
            {
                json.Property(property).Value = new JValue(value);
            }
            else
            {
                json.Add(property, new JValue(value));
            }
        }


        #endregion

        #region 异常错误信息格式化

        /// <summary>
        /// 显示一个完整的错误信息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <exception cref="NullReferenceException"></exception>
        public static void ShowExceptionMessage(Exception ex)
        {
            MessageBox.Show(GetExceptionMessage(ex));
        }


        /// <summary>
        /// 显示一个完整的错误信息，和额外的字符串描述信息
        /// </summary>
        /// <param name="extraMsg">额外的描述信息</param>
        /// <param name="ex">异常对象</param>
        /// <exception cref="NullReferenceException"></exception>
        public static void ShowExceptionMessage(string extraMsg, Exception ex)
        {
            MessageBox.Show(GetExceptionMessage(extraMsg, ex));
        }


        /// <summary>
        /// 获取一个异常的完整错误信息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <returns>完整的字符串数据</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string GetExceptionMessage(Exception ex)
        {
            return StringResources.ExceptionMessage + ex.Message + Environment.NewLine +
                StringResources.ExceptionStackTrace + ex.StackTrace + Environment.NewLine +
                StringResources.ExceptopnTargetSite + ex.TargetSite;
        }

        /// <summary>
        /// 获取一个异常的完整错误信息，和额外的字符串描述信息
        /// </summary>
        /// <param name="extraMsg">额外的信息</param>
        /// <param name="ex">异常对象</param>
        /// <returns>完整的字符串数据</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string GetExceptionMessage(string extraMsg, Exception ex)
        {
            if (string.IsNullOrEmpty(extraMsg))
            {
                return GetExceptionMessage(ex);
            }
            else
            {
                return extraMsg + Environment.NewLine + GetExceptionMessage(ex);
            }
        }


        #endregion

        #region Hex字符串和Byte[]相互转化块


        /// <summary>
        /// 字节数据转化成16进制表示的字符串
        /// </summary>
        /// <param name="InBytes">字节数组</param>
        /// <returns>返回的字符串</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string ByteToHexString(byte[] InBytes)
        {
            return ByteToHexString(InBytes, (char)0);
        }

        /// <summary>
        /// 字节数据转化成16进制表示的字符串
        /// </summary>
        /// <param name="InBytes">字节数组</param>
        /// <param name="segment">分割符</param>
        /// <returns>返回的字符串</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string ByteToHexString(byte[] InBytes, char segment)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte InByte in InBytes)
            {
                if (segment == 0) sb.Append(string.Format("{0:X2}", InByte));
                else sb.Append(string.Format("{0:X2}{1}", InByte, segment));
            }

            if (segment != 0 && sb.Length > 1 && sb[sb.Length - 1] == segment)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }



        /// <summary>
        /// 字符串数据转化成16进制表示的字符串
        /// </summary>
        /// <param name="InString">输入的字符串数据</param>
        /// <returns>返回的字符串</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string ByteToHexString(string InString)
        {
            return ByteToHexString(Encoding.Unicode.GetBytes(InString));
        }

        /// <summary>
        /// 将16进制的字符串转化成Byte数据，将检测每2个字符转化，也就是说，中间可以是任意字符
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(string hex)
        {
            hex = hex.ToUpper();
            List<char> data = new List<char>()
            {
                '0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'
            };

            MemoryStream ms = new MemoryStream();

            for (int i = 0; i < hex.Length; i++)
            {
                if ((i + 1) < hex.Length)
                {
                    if (data.Contains(hex[i]) && data.Contains(hex[i + 1]))
                    {
                        // 这是一个合格的字节数据
                        ms.WriteByte((byte)(data.IndexOf(hex[i]) * 16 + data.IndexOf(hex[i + 1])));
                        i++;
                    }
                }
            }

            byte[] result = ms.ToArray();
            ms.Dispose();
            return result;
        }

        #endregion

        #region Bool[]数组和byte[]相互转化块


        /// <summary>
        /// 将bool数组转换到byte数组
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static byte[] BoolArrayToByte(bool[] array)
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
        /// 从Byte数组中提取位数组
        /// </summary>
        /// <param name="InBytes">原先的字节数组</param>
        /// <param name="length">想要转换的长度，如果超出自动会缩小到数组最大长度</param>
        /// <returns></returns>
        public static bool[] ByteToBoolArray(byte[] InBytes, int length)
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

        #region byte[]数组和short,ushort相互转化


        /*******************************************************************************************************
         * 
         *    2018年3月19日 10:57:11
         *    感谢：毛毛虫 提供的BUG报告 316664767@qq.com
         * 
         ********************************************************************************************************/

        /// <summary>
        /// 从byte数组中提取出short数组，并指定是否需要高地位置换
        /// </summary>
        /// <param name="InBytes"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        public static short[] ByteToShortArray( byte[] InBytes, bool reverse )
        {
            if (InBytes == null) return null;

            short[] array = new short[InBytes.Length / 2];
            for (int i = 0; i < array.Length; i++)
            {
                byte[] temp = new byte[2];

                if (reverse)
                {
                    temp[0] = InBytes[2 * i + 1];
                    temp[1] = InBytes[2 * i + 0];
                }
                else
                {
                    temp[0] = InBytes[2 * i + 0];
                    temp[1] = InBytes[2 * i + 1];
                }
                array[i] = BitConverter.ToInt16( temp, 0 );
            }

            return array;
        }

        /// <summary>
        /// 从byte数组中提取出ushort数组，并指定是否需要高地位置换
        /// </summary>
        /// <param name="InBytes"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        public static ushort[] ByteToUShortArray( byte[] InBytes, bool reverse )
        {
            if (InBytes == null) return null;

            ushort[] array = new ushort[InBytes.Length / 2];
            for (int i = 0; i < array.Length; i++)
            {
                byte[] temp = new byte[2];

                if (reverse)
                {
                    temp[0] = InBytes[2 * i + 1];
                    temp[1] = InBytes[2 * i + 0];
                }
                else
                {
                    temp[0] = InBytes[2 * i + 0];
                    temp[1] = InBytes[2 * i + 1];
                }
                array[i] = BitConverter.ToUInt16( temp, 0 );
            }

            return array;
        }

        #endregion

        #region 基础框架块

        /// <summary>
        /// 设置或获取系统框架的版本号
        /// </summary>
        public static SystemVersion FrameworkVersion { get; set; } = new SystemVersion("5.0.10");


        #endregion

        #region 深度克隆对象

        /// <summary>
        /// 使用序列化反序列化深度克隆一个对象
        /// </summary>
        /// <param name="oringinal"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static object DeepClone(object oringinal)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter()
                {
                    Context = new System.Runtime.Serialization.StreamingContext(System.Runtime.Serialization.StreamingContextStates.Clone)
                };
                formatter.Serialize(stream, oringinal);
                stream.Position = 0;
                return formatter.Deserialize(stream);
            }
        }


        #endregion

        #region 获取唯一的一串字符串

        /// <summary>
        /// 获取一串唯一的随机字符串，长度为20，由Guid码和4位数的随机数组成，保证字符串的唯一性
        /// </summary>
        /// <returns>随机字符串数据</returns>
        public static string GetUniqueStringByGuidAndRandom()
        {
            Random random = new Random();
            return Guid.NewGuid().ToString("N") + random.Next(1000, 10000);
        }

        #endregion
    }

}
