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

        /// <summary>
        /// 获取一个枚举类型的所有枚举值，可直接应用于组合框数据
        /// </summary>
        /// <typeparam name="TEnum">枚举的类型值</typeparam>
        /// <returns>枚举值数组</returns>
        public static TEnum[] GetEnumValues<TEnum>() where TEnum : struct
        {
            return (TEnum[])Enum.GetValues(typeof(TEnum));
        }



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

        /// <summary>
        /// 显示一个完整的错误信息
        /// </summary>
        /// <param name="ex"></param>
        /// <exception cref="NullReferenceException"></exception>
        public static void ShowExceptionMessage(Exception ex)
        {
            MessageBox.Show(GetExceptionMessage(ex));
        }
        /// <summary>
        /// 获取一个异常的完整错误信息
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string GetExceptionMessage(Exception ex)
        {
            return StringResources.ExceptionMessage + ex.Message + Environment.NewLine +
                StringResources.ExceptionStackTrace + ex.StackTrace + Environment.NewLine +
                StringResources.ExceptopnTargetSite + ex.TargetSite;
        }

        /// <summary>
        /// 字节数据转化成16进制表示的字符串
        /// </summary>
        /// <param name="InBytes">字节数组</param>
        /// <returns>返回的字符串</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string ByteToHexString(byte[] InBytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte InByte in InBytes)
            {
                sb.Append(string.Format("{0:X2}", InByte));
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
        /// 设置或获取系统框架的版本号
        /// </summary>
        public static SystemVersion FrameworkVersion { get; set; } = new SystemVersion("1.0.2");

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
    }

}
