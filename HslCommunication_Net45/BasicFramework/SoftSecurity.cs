using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HslCommunication.BasicFramework
{

    /**********************************************************************************
     * 
     *    Create Date:2017-05-03 16:52:48
     *    Author By Richard.Hu
     * 
     * 
     **********************************************************************************/


    /// <summary>
    /// 字符串加密解密相关的自定义类
    /// </summary>
    public static class SoftSecurity
    {
        /// <summary>
        /// 加密数据，采用对称加密的方式
        /// </summary>
        /// <param name="pToEncrypt">待加密的数据</param>
        /// <returns>加密后的数据</returns>
        internal static string MD5Encrypt(string pToEncrypt)
        {
            return MD5Encrypt(pToEncrypt, "zxcvBNMM");
        }


        /// <summary>
        /// 加密数据，采用对称加密的方式
        /// </summary>
        /// <param name="pToEncrypt">待加密的数据</param>
        /// <param name="Password">密钥，长度为8，英文或数字</param>
        /// <returns>加密后的数据</returns>
        public static string MD5Encrypt(string pToEncrypt, string Password)
        {
            string aisdhaisdhwdb = Password;
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            des.Key = Encoding.ASCII.GetBytes(aisdhaisdhwdb);
            des.IV = Encoding.ASCII.GetBytes(aisdhaisdhwdb);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }

        /// <summary>
        /// 解密过程，使用的是对称的加密
        /// </summary>
        /// <param name="pToDecrypt">等待解密的字符</param>
        /// <returns>返回原密码，如果解密失败，返回‘解密失败’</returns>
        internal static string MD5Decrypt(string pToDecrypt)
        {
            return MD5Decrypt(pToDecrypt, "zxcvBNMM");
        }

        /// <summary>
        /// 解密过程，使用的是对称的加密
        /// </summary>
        /// <param name="pToDecrypt">等待解密的字符</param>
        /// <param name="password">密钥，长度为8，英文或数字</param>
        /// <returns>返回原密码，如果解密失败，返回‘解密失败’</returns>
        public static string MD5Decrypt(string pToDecrypt, string password)
        {
            if (pToDecrypt == "") return pToDecrypt;
            string zxcawrafdgegasd = password;
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            des.Key = Encoding.ASCII.GetBytes(zxcawrafdgegasd);
            des.IV = Encoding.ASCII.GetBytes(zxcawrafdgegasd);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            cs.Dispose();
            return Encoding.Default.GetString(ms.ToArray());
        }
    }
}
