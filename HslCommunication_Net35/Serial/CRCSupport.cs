using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Serial
{
    /// <summary>
    /// 用于CRC16验证的类，提供了标准的验证方法
    /// </summary>
    public class HslCRC
    {
        /// <summary>
        /// 校验对应的接收数据的CRC校验码
        /// </summary>
        /// <param name="data">需要校验的数据，带CRC校验码</param>
        /// <returns>返回校验成功与否</returns>
        public static bool CheckCRC16(byte[] data)
        {
            int length = data.Length;
            byte[] buf = new byte[length - 2];
            Array.Copy(data, 0, buf, 0, buf.Length);

            byte[] CRCbuf = CRC16(buf);
            if (CRCbuf[length - 2] == data[length - 2] &&
                CRCbuf[length - 1] == data[length - 1])
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取对应的数据的CRC校验码
        /// </summary>
        /// <param name="data">需要校验的数据，不包含CRC字节</param>
        /// <returns>返回带CRC校验码的字节数组，可用于串口发送</returns>
        public static byte[] CRC16(byte[] data)
        {
            byte[] buf = new byte[data.Length + 2];
            data.CopyTo(buf, 0);


            byte CRC16Lo;
            byte CRC16Hi;   //CRC寄存器             
            byte CL; byte CH;       //多项式码0xA001             
            byte SaveHi;
            byte SaveLo;
            byte[] tmpData;
            int Flag;

            //预置寄存器
            CRC16Lo = 0xFF;
            CRC16Hi = 0xFF;

            //预设固定值
            CL = 0x01;
            CH = 0xA0;
            tmpData = data;
            for (int i = 0; i < tmpData.Length; i++)
            {
                CRC16Lo = (byte)(CRC16Lo ^ tmpData[i]); //每一个数据与CRC寄存器低位进行异或，结果返回CRC寄存器                 
                for (Flag = 0; Flag <= 7; Flag++)
                {
                    SaveHi = CRC16Hi;
                    SaveLo = CRC16Lo;
                    CRC16Hi = (byte)(CRC16Hi >> 1);      //高位右移一位                     
                    CRC16Lo = (byte)(CRC16Lo >> 1);      //低位右移一位                     
                    if ((SaveHi & 0x01) == 0x01) //如果高位字节最后一位为1    
                    {
                        //则低位字节右移后前面补1                     
                        CRC16Lo = (byte)(CRC16Lo | 0x80);
                    }
                    //否则自动补0                     


                    //如果最低位为1，则将CRC寄存器与预设的固定值进行异或运算
                    if ((SaveLo & 0x01) == 0x01)
                    {
                        CRC16Hi = (byte)(CRC16Hi ^ CH);
                        CRC16Lo = (byte)(CRC16Lo ^ CL);
                    }
                }
            }


            buf[buf.Length - 2] = CRC16Lo;
            buf[buf.Length - 1] = CRC16Hi;
            //byte[] ReturnData = new byte[2];            
            //ReturnData[0] = CRC16Hi;       //CRC高位             
            //ReturnData[1] = CRC16Lo;       //CRC低位             
            //return ReturnData;     
            return buf;

        }



        private bool CheckResponse(byte[] response)
        {
            //Perform a basic CRC check:
            byte[] CRC = GetCRC(response);
            if (CRC[0] == response[response.Length - 2] && CRC[1] == response[response.Length - 1])
                return true;
            else
                return false;
        }
        private byte[] GetCRC(byte[] message)
        {
            //Function expects a modbus message of any length as well as a 2 byte CRC array in which to 
            //return the CRC values:

            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < message.Length; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    //向右移一位，高位置0
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);


                    if (CRCLSB == 1)
                    {
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                    }
                }
            }
            byte[] ByteTemp = new byte[2];
            ByteTemp[1] = CRCHigh = (byte)(CRCFull / 256);
            ByteTemp[0] = CRCLow = (byte)(CRCFull % 256);
            return ByteTemp;
        }
    }
}
