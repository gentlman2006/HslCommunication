using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Serial
{
    /// <summary>
    /// 用于LRC验证的类，提供了标准的验证方法
    /// </summary>
    public class SoftLRC
    {
        /// <summary>
        /// 获取对应的数据的LRC校验码
        /// </summary>
        /// <param name="value">需要校验的数据，不包含LRC字节</param>
        /// <returns>返回带LRC校验码的字节数组，可用于串口发送</returns>
        public static byte[] LRC( byte[] value )
        {
            if (value == null) return null;

            int sum = 0;
            for (int i = 0; i < value.Length; i++)
            {
                sum += value[i];
            }

            sum = sum % 256;
            sum = 256 - sum;

            byte[] LRC = new byte[] { (byte)sum };
            return BasicFramework.SoftBasic.SpliceTwoByteArray( value, LRC );
        }


        /// <summary>
        /// 检查对应的数据是否符合LRC的验证
        /// </summary>
        /// <param name="value">等待校验的数据，是否正确</param>
        /// <returns>是否校验成功</returns>
        public static bool CheckLRC( byte[] value )
        {
            if (value == null) return false;

            int length = value.Length;
            byte[] buf = new byte[length - 1];
            Array.Copy( value, 0, buf, 0, buf.Length );

            byte[] LRCbuf = LRC( buf );
            if (LRCbuf[length - 1] == value[length - 1])
            {
                return true;
            }
            return false;
        }
    }
}
