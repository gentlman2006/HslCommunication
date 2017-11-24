using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using HslCommunication.Core;

namespace HslCommunication.Profinet
{
    /*************************************************************************
     * 
     *    支持生成使用的PLC设备的通信的及基础类，提供相关的操作
     * 
     *    Support the generation of PLC used in communications equipment and base classes, provide related operation
     * 
     *************************************************************************/


    internal class PlcStateOne
    {
        /*********************************************************************************
         * 
         *    并发异步访问PLC的状态类
         * 
         *    应该包含地址，传送状态，以及连接成功功能
         * 
         ********************************************************************************/
        /// <summary>
        /// 在数组中的位置
        /// </summary>
        public int Index { get; set; }


        private bool IsMainPort = true;
        public int GetPort()
        {
            return IsMainPort ? PortMain : PortBackup;
        }
        public void ChangePort()
        {
            IsMainPort = !IsMainPort;
        }


        /// <summary>
        /// 主要访问的PLC端口
        /// </summary>
        public int PortMain { get; set; }
        /// <summary>
        /// 备用访问的PLC端口
        /// </summary>
        public int PortBackup { get; set; }



        /// <summary>
        /// PLC的IP地址
        /// </summary>
        public IPAddress PlcIpAddress { get; set; }
        /// <summary>
        /// 用于工作的套接字对象
        /// </summary>
        public Socket WorkSocket { get; set; }
        /// <summary>
        /// 连接是否成功
        /// </summary>
        public bool IsConnect { get; set; }
        /// <summary>
        /// 头子节
        /// </summary>
        public byte[] PlcDataHead { get; set; }
        /// <summary>
        /// 头子节接收长度
        /// </summary>
        public int LengthDataHead { get; set; }

        /// <summary>
        /// 接收的PLC实际数据
        /// </summary>
        public byte[] PlcDataContent { get; set; }
        /// <summary>
        /// 实际数据的接收长度
        /// </summary>
        public int LengthDataContent { get; set; }
    }



    

    /// <summary>
    /// 以太网模块访问的基类
    /// </summary>
    public class PlcNetBase : DoubleModeNetBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        public PlcNetBase()
        {
        }

        #endregion

        #region Private Members
        
        private int m_PortRead = 1000;                                // 读取的端口号
        private int m_PortReadBackup = -1;                            // 备用的端口号
        private bool m_IsPortNormal = true;                           // 端口号切换使用

        #endregion

        #region DoubleModeNetBase Override
        
        /// <summary>
        /// 获取服务器的连接
        /// </summary>
        /// <returns></returns>
        protected override IPEndPoint GetIPEndPoint()
        {
            return new IPEndPoint(PLCIpAddress, GetPort());
        }
        

        #endregion



        /// <summary>
        /// 获取访问的端口号
        /// </summary>
        /// <returns></returns>
        protected int GetPort()
        {
            if (m_PortReadBackup <= 0) return m_PortRead;
            return m_IsPortNormal ? m_PortRead : m_PortReadBackup;
        }
        /// <summary>
        /// 更换端口号
        /// </summary>
        protected void ChangePort()
        {
            m_IsPortNormal = !m_IsPortNormal;
        }

        /// <summary>
        /// 读取数据的端口，默认为1000
        /// </summary>
        public int PortRead
        {
            get { return m_PortRead; }
            set { m_PortRead = value; }
        }
        /// <summary>
        /// 读取数据的备用端口，默认为-1(不切换)，当一次请求失败时，将会自动切换端口
        /// </summary>
        public int PortReadBackup
        {
            get { return m_PortReadBackup; }
            set { m_PortReadBackup = value; }
        }

        /// <summary>
        /// 写入数据端口，默认为1001
        /// </summary>
        public int PortWrite { get; set; } = 1001;

        private IPAddress m_PlcIpAddress = IPAddress.Parse("192.168.0.2");
        /// <summary>
        /// 远程PLC的IP地址，默认为192.168.0.2
        /// </summary>
        public IPAddress PLCIpAddress
        {
            get { return m_PlcIpAddress; }
            set { m_PlcIpAddress = value; }
        }


        /// <summary>
        /// 追加字节数据的头部空字节
        /// </summary>
        /// <param name="bytes">实际数据</param>
        /// <param name="length">追加的长度</param>
        /// <returns></returns>
        protected byte[] AddVoidHead(byte[] bytes, int length)
        {
            if (bytes == null)
            {
                return new byte[length];
            }
            else
            {
                byte[] result = new byte[bytes.Length + length];
                bytes.CopyTo(result, length);
                return result;
            }
        }


        /// <summary>
        /// 控制字节长度，超出选择截断，不够补零
        /// </summary>
        /// <param name="bytes">字节数据</param>
        /// <param name="length">最终需要的目标长度</param>
        /// <returns>处理后的数据</returns>
        protected byte[] ManageBytesLength(byte[] bytes, int length)
        {
            if (bytes == null) return null;
            byte[] temp = new byte[length];
            if (length > bytes.Length)
            {
                Array.Copy(bytes, 0, temp, 0, bytes.Length);
            }
            else
            {
                Array.Copy(bytes, 0, temp, 0, length);
            }
            return temp;
        }


        /// <summary>
        /// 根据数据的数组返回真实的数据字节
        /// </summary>
        /// <param name="array"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        protected byte[] GetBytesFromArray(short[] array, bool reverse)
        {
            byte[] temp = new byte[array.Length * 2];
            for (int i = 0; i < array.Length; i++)
            {
                if (!reverse)
                {
                    BitConverter.GetBytes(array[i]).CopyTo(temp, 2 * i);
                }
                else
                {
                    byte[] buffer = BitConverter.GetBytes(array[i]);
                    Array.Reverse(buffer);
                    buffer.CopyTo(temp, 2 * i);
                }
            }
            return temp;
        }

        /// <summary>
        /// 根据数据的数组返回真实的数据字节
        /// </summary>
        /// <param name="array"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        protected byte[] GetBytesFromArray(int[] array, bool reverse)
        {
            byte[] temp = new byte[array.Length * 4];
            for (int i = 0; i < array.Length; i++)
            {
                if (!reverse)
                {
                    BitConverter.GetBytes(array[i]).CopyTo(temp, 4 * i);
                }
                else
                {
                    byte[] buffer = BitConverter.GetBytes(array[i]);
                    Array.Reverse(buffer);
                    buffer.CopyTo(temp, 4 * i);
                }
            }
            return temp;
        }

        /// <summary>
        /// 根据数据的数据返回真实的数据字节
        /// </summary>
        /// <param name="array"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        protected byte[] GetBytesFromArray(ushort[] array, bool reverse)
        {
            byte[] temp = new byte[array.Length * 2];
            for (int i = 0; i < array.Length; i++)
            {
                if (!reverse)
                {
                    BitConverter.GetBytes(array[i]).CopyTo(temp, 2 * i);
                }
                else
                {
                    byte[] buffer = BitConverter.GetBytes(array[i]);
                    Array.Reverse(buffer);
                    buffer.CopyTo(temp, 2 * i);
                }
            }
            return temp;
        }

        /// <summary>
        /// 根据数据的数组返回真实的数据字节
        /// </summary>
        /// <param name="array"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        protected byte[] GetBytesFromArray(uint[] array, bool reverse)
        {
            byte[] temp = new byte[array.Length * 4];
            for (int i = 0; i < array.Length; i++)
            {
                if (!reverse)
                {
                    BitConverter.GetBytes(array[i]).CopyTo(temp, 4 * i);
                }
                else
                {
                    byte[] buffer = BitConverter.GetBytes(array[i]);
                    Array.Reverse(buffer);
                    buffer.CopyTo(temp, 4 * i);
                }
            }
            return temp;
        }

        /// <summary>
        /// 根据底层的数据情况返回转换后的short数组
        /// </summary>
        /// <param name="bytes">真实的数据</param>
        /// <returns></returns>
        public virtual short[] GetArrayFromBytes(byte[] bytes)
        {
            short[] temp = new short[bytes.Length / 2];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = BitConverter.ToInt16(bytes, i * 2);
            }
            return temp;
        }

        /// <summary>
        /// 根据底层的数据情况返回转换后的int数组
        /// </summary>
        /// <param name="bytes">真实的数据</param>
        /// <returns></returns>
        public virtual int[] GetIntArrayFromBytes(byte[] bytes)
        {
            int[] temp = new int[bytes.Length / 4];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = BitConverter.ToInt32(bytes, i * 4);
            }
            return temp;
        }

        /// <summary>
        /// 从指定的字节数据中提取指定位置的short值
        /// </summary>
        /// <param name="content">读取的字节数组</param>
        /// <param name="index">索引</param>
        /// <returns>实际值</returns>
        public virtual short GetShortFromBytes(byte[] content, int index)
        {
            return BitConverter.ToInt16(content, index * 2);
        }
    }
}
