using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication
{
    internal class HslSecurity
    {

        #region Encryption and decryption

        /*******************************************************************************
         * 
         *    用于加密解密的方法，为了性能考虑，使用了相对简单的加密解密方式，紧紧对当前的程序集开放
         *
         *    Method for encryption and decryption, for performance reasons, using relatively simple encryption and decryption
         * 
         *******************************************************************************/
         

        /// <summary>
        /// 加密方法，只对当前的程序集开放
        /// </summary>
        /// <param name="enBytes">等待加密的数据</param>
        /// <returns>加密后的字节数据</returns>
        internal static byte[] ByteEncrypt(byte[] enBytes)
        {
            if (enBytes == null) return null;
            byte[] result = new byte[enBytes.Length];
            for (int i = 0; i < enBytes.Length; i++)
            {
                result[i] = (byte)(enBytes[i] ^ 0xB5);
            }
            return result;
        }
        /// <summary>
        /// 解密方法，只对当前的程序集开放
        /// </summary>
        /// <param name="deBytes">等待解密的数据</param>
        /// <returns>解密后的字节数据</returns>
        internal static byte[] ByteDecrypt(byte[] deBytes)
        {
            return ByteEncrypt(deBytes);
        }


        #endregion




    }
}
