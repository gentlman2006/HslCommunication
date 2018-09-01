using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace HslCommunication.BasicFramework
{
    /// <summary>
    /// 软件授权类
    /// </summary>
    public class SoftAuthorize : SoftFileSaveBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个软件授权类
        /// </summary>
        public SoftAuthorize()
        {
            machine_code = GetInfo();
            LogHeaderText = "SoftAuthorize";
        }

        #endregion

        #region Static Members

        /// <summary>
        /// 注册码描述文本
        /// </summary>
        public static readonly string TextCode = "Code";

        #endregion

        #region Private Members

        /// <summary>
        /// 最终的注册秘钥信息，注意是只读的。
        /// </summary>
        /// <remarks>
        /// 时间：2018年9月1日 23:01:54，来自 洛阳-LYG 的建议，公开了本属性信息，只读。
        /// </remarks>
        public string FinalCode { get; private set; } = "";
        /// <summary>
        /// 是否正式发行版，是的话就取消授权
        /// </summary>
        public bool IsReleaseVersion { get; set; } = false;
        /// <summary>
        /// 指示是否加载过文件信息
        /// </summary>
        private bool HasLoadByFile { get; set; } = false;


        private string machine_code = "";

        #endregion

        #region Public Members


        /// <summary>
        /// 指示系统是否处于试用运行
        /// </summary>
        public bool IsSoftTrial { get; set; } = false;



        #endregion

        #region Public Method


        /// <summary>
        /// 获取本机的机器码
        /// </summary>
        /// <returns>机器码字符串</returns>
        public string GetMachineCodeString()
        {
            return machine_code;
        }

        /// <summary>
        /// 获取需要保存的数据内容
        /// </summary>
        /// <returns>实际保存的内容</returns>
        public override string ToSaveString()
        {
            JObject json = new JObject
            {
                { TextCode, new JValue(FinalCode) }
            };
            return json.ToString();
        }
        /// <summary>
        /// 从字符串加载数据
        /// </summary>
        /// <param name="content">文件存储的数据</param>
        public override void LoadByString(string content)
        {
            JObject json = JObject.Parse(content);
            FinalCode = SoftBasic.GetValueFromJsonObject(json, TextCode, FinalCode);
            HasLoadByFile = true;
        }
        /// <summary>
        /// 使用特殊加密算法加密数据
        /// </summary>
        public override void SaveToFile()
        {
            SaveToFile(m => SoftSecurity.MD5Encrypt(m));
        }
        /// <summary>
        /// 使用特殊解密算法解密数据
        /// </summary>
        public override void LoadByFile()
        {
            LoadByFile(m => SoftSecurity.MD5Decrypt(m));
        }



        /// <summary>
        /// 检查该注册码是否是正确的注册码
        /// </summary>
        /// <param name="code">注册码信息</param>
        /// <param name="encrypt">数据加密的方法，必须用户指定</param>
        /// <returns>是否注册成功</returns>
        public bool CheckAuthorize(string code, Func<string, string> encrypt)
        {
            if (code != encrypt(GetMachineCodeString()))
            {
                return false;
            }
            else
            {
                FinalCode = code;
                SaveToFile();
                return true;
            }
        }

        /// <summary>
        /// 检测授权是否成功
        /// </summary>
        /// <param name="encrypt">数据加密的方法，必须用户指定</param>
        /// <returns>是否成功授权</returns>
        public bool IsAuthorizeSuccess(Func<string, string> encrypt)
        {
            if (IsReleaseVersion) return true;

            if (encrypt(GetMachineCodeString()) == FinalCode)
            {
                return true;
            }
            else
            {
                FinalCode = "";
                SaveToFile();
                return false;
            }
        }


        #endregion

        #region Static Method


        /// <summary>
        /// 获取本计算机唯一的机器码
        /// </summary>
        /// <returns>字符串形式的机器码</returns>
        public static string GetInfo()
        {
            string unique = "";
            // 获取处理器信息
            ManagementClass cimobject = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = cimobject.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                unique += mo.Properties["ProcessorId"].Value.ToString();
            }
            // 获取硬盘ID  
            ManagementClass cimobject1 = new ManagementClass("Win32_DiskDrive");
            ManagementObjectCollection moc1 = cimobject1.GetInstances();
            foreach (ManagementObject mo in moc1)
            {
                unique += (string)mo.Properties["Model"].Value;
                break;
            }

            // 获取BIOS
            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("Select SerialNumber From Win32_BIOS");
            ManagementObjectCollection moc2 = searcher.Get();

            if (moc2.Count > 0)
            {
                foreach (ManagementObject share in moc2)
                {
                    unique += share["SerialNumber"].ToString();
                }
            }

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            return SoftBasic.ByteToHexString(md5.ComputeHash(Encoding.Unicode.GetBytes(unique))).Substring(0, 25);
        }


        #endregion
    }
}
