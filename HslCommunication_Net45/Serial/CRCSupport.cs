using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Serial
{
    /// <summary>
    /// 用于CRC16验证的类，提供了标准的验证方法
    /// </summary>
    /// <remarks>
    /// 本类提供了几个静态的方法，用来进行CRC16码的计算和验证的，多项式码可以自己指定配置，但是预置的寄存器为0xFF 0xFF
    /// </remarks>
    /// <example>
    /// 先演示如何校验一串数据的CRC码
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Serial\SoftCRC16.cs" region="Example1" title="SoftCRC16示例" />
    /// 然后下面是如何生成你自己的CRC校验码
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Serial\SoftCRC16.cs" region="Example2" title="SoftCRC16示例" />
    /// </example>
    public class SoftCRC16
    {

        /// <summary>
        /// 来校验对应的接收数据的CRC校验码，默认多项式码为0xA001
        /// </summary>
        /// <param name="value">需要校验的数据，带CRC校验码</param>
        /// <returns>返回校验成功与否</returns>
        public static bool CheckCRC16(byte[] value )
        {
            return CheckCRC16( value, 0xA0, 0x01);
        }

        /// <summary>
        /// 指定多项式码来校验对应的接收数据的CRC校验码
        /// </summary>
        /// <param name="value">需要校验的数据，带CRC校验码</param>
        /// <param name="CH">多项式码高位</param>
        /// <param name="CL">多项式码低位</param>
        /// <returns>返回校验成功与否</returns>
        public static bool CheckCRC16(byte[] value, byte CH, byte CL)
        {
            if (value == null) return false;
            if (value.Length < 2) return false;

            int length = value.Length;
            byte[] buf = new byte[length - 2];
            Array.Copy(value, 0, buf, 0, buf.Length);

            byte[] CRCbuf = CRC16(buf, CH, CL);
            if (CRCbuf[length - 2] == value[length - 2] &&
                CRCbuf[length - 1] == value[length - 1])
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 获取对应的数据的CRC校验码，默认多项式码为0xA001
        /// </summary>
        /// <param name="value">需要校验的数据，不包含CRC字节</param>
        /// <returns>返回带CRC校验码的字节数组，可用于串口发送</returns>
        public static byte[] CRC16(byte[] value )
        {
            return CRC16( value, 0xA0, 0x01);
        }

        /// <summary>
        /// 通过指定多项式码来获取对应的数据的CRC校验码
        /// </summary>
        /// <param name="value">需要校验的数据，不包含CRC字节</param>
        /// <param name="CL">多项式码地位</param>
        /// <param name="CH">多项式码高位</param>
        /// <returns>返回带CRC校验码的字节数组，可用于串口发送</returns>
        public static byte[] CRC16(byte[] value, byte CH, byte CL)
        {
            byte[] buf = new byte[value.Length + 2];
            value.CopyTo(buf, 0);

            byte CRC16Lo;
            byte CRC16Hi;           // CRC寄存器                   
            byte SaveHi;
            byte SaveLo;
            byte[] tmpData;
            int Flag;

            // 预置寄存器
            CRC16Lo = 0xFF;
            CRC16Hi = 0xFF;

            tmpData = value;
            for (int i = 0; i < tmpData.Length; i++)
            {
                CRC16Lo = (byte)(CRC16Lo ^ tmpData[i]); // 每一个数据与CRC寄存器低位进行异或，结果返回CRC寄存器                 
                for (Flag = 0; Flag <= 7; Flag++)
                {
                    SaveHi = CRC16Hi;
                    SaveLo = CRC16Lo;
                    CRC16Hi = (byte)(CRC16Hi >> 1);      // 高位右移一位                     
                    CRC16Lo = (byte)(CRC16Lo >> 1);      // 低位右移一位                     
                    if ((SaveHi & 0x01) == 0x01) // 如果高位字节最后一位为1    
                    {
                        // 则低位字节右移后前面补1                     
                        CRC16Lo = (byte)(CRC16Lo | 0x80);
                    }
                    // 否则自动补0                     


                    // 如果最低位为1，则将CRC寄存器与预设的固定值进行异或运算
                    if ((SaveLo & 0x01) == 0x01)
                    {
                        CRC16Hi = (byte)(CRC16Hi ^ CH);
                        CRC16Lo = (byte)(CRC16Lo ^ CL);
                    }
                }
            }

            buf[buf.Length - 2] = CRC16Lo;
            buf[buf.Length - 1] = CRC16Hi;

            // 返回最终带有CRC校验码结尾的信息
            return buf;
        }


    }
}
