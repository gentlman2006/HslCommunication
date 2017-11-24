using HslCommunication.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



/************************************************************************************************
 * 
 *    版权声明：Copyright © 2017 Richard.Hu
 *    
 *    时间：2017年11月18日 10:20:15
 * 
 *************************************************************************************************/





namespace HslCommunication.ModBus
{
    /// <summary>
    /// ModBus的功能码
    /// </summary>
    public enum ModBusFunctionMask
    {
        /// <summary>
        /// 读线圈
        /// </summary>
        ReadCoil = 1,
        /// <summary>
        /// 读离散量
        /// </summary>
        ReadDiscrete = 2,
        /// <summary>
        /// 读保持型寄存器
        /// </summary>
        ReadRegister = 3,
        /// <summary>
        /// 写单个线圈
        /// </summary>
        WriteOneCoil = 5,
        /// <summary>
        /// 写单个寄存器
        /// </summary>
        WriteOneRegister = 6,
        /// <summary>
        /// 写多个线圈
        /// </summary>
        WriteCoil = 0x0F,
        /// <summary>
        /// 写多个寄存器
        /// </summary>
        WriteRegister = 0x10,
    }









    /// <summary>
    /// ModBusTcp的客户端，可以方便的实现指定地点的数据读取和写入
    /// </summary>
    public class ModBusTcpClient : DoubleModeNetBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个ModBusTcp的客户端，需要指定服务器的地址及端口，默认端口为502
        /// </summary>
        /// <param name="ipAddress">服务器的IP地址</param>
        /// <param name="port">服务器的端口</param>
        /// <param name="station">客户端的站号，可以用来标识不同的客户端，默认255</param>
        public ModBusTcpClient(string ipAddress, int port = 502, byte station = 0xFF)
        {
            serverEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
            simpleHybird = new SimpleHybirdLock();
            this.station = station;
        }


        #endregion

        #region Private Field
        
        private ushort messageId = 1;                       // 消息头
        private byte station = 0;                           // ModBus的客户端站号
        private SimpleHybirdLock simpleHybird;              // 消息头递增的同步锁
        private int receiveTimeOut = 5000;                  // 接收Modbus数据的超时时间

        #endregion

        #region Private Method

        private ushort GetMessageId()
        {
            ushort result = 0;
            simpleHybird.Enter();
            result = messageId;
            if (messageId == ushort.MaxValue)
            {
                messageId = 0;
            }
            else
            {
                messageId++;
            }
            simpleHybird.Leave();
            return result;
        }

        private byte[] BuildReadCommand(ModBusFunctionMask mask, ushort address, ushort length)
        {
            ushort messageId = GetMessageId();
            byte[] buffer = new byte[12];
            buffer[0] = (byte)(messageId / 256);
            buffer[1] = (byte)(messageId % 256);
            buffer[5] = 0x06;
            buffer[6] = station;
            buffer[7] = (byte)mask;
            buffer[8] = (byte)(address / 256);
            buffer[9] = (byte)(address % 256);
            buffer[10] = (byte)(length / 256);
            buffer[11] = (byte)(length % 256);
            return buffer;
        }


        private byte[] BuildWriteOneCoil(ushort address, bool value)
        {
            ushort messageId = GetMessageId();
            byte[] buffer = new byte[12];
            buffer[0] = (byte)(messageId / 256);
            buffer[1] = (byte)(messageId % 256);
            buffer[5] = 0x06;
            buffer[6] = station;
            buffer[7] = (byte)ModBusFunctionMask.WriteOneCoil;
            buffer[8] = (byte)(address / 256);
            buffer[9] = (byte)(address % 256);
            if (value) buffer[10] = 0xFF;
            return buffer;
        }

        private byte[] BuildWriteOneRegister(ushort address, byte[] data)
        {
            ushort messageId = GetMessageId();
            byte[] buffer = new byte[12];
            buffer[0] = (byte)(messageId / 256);
            buffer[1] = (byte)(messageId % 256);
            buffer[5] = 0x06;
            buffer[6] = station;
            buffer[7] = (byte)ModBusFunctionMask.WriteOneRegister;
            buffer[8] = (byte)(address / 256);
            buffer[9] = (byte)(address % 256);
            buffer[10] = data[1];
            buffer[11] = data[0];
            return buffer;
        }



        private byte[] BuildWriteCoil(ushort address, bool[] value)
        {
            byte[] data = BasicFramework.SoftBasic.BoolArrayToByte(value);
            if (data == null) data = new byte[0];

            ushort messageId = GetMessageId();
            byte[] buffer = new byte[13 + data.Length];
            buffer[0] = (byte)(messageId / 256);
            buffer[1] = (byte)(messageId % 256);
            buffer[4] = (byte)((buffer.Length - 6) / 256);
            buffer[5] = (byte)((buffer.Length - 6) % 256);
            buffer[6] = station;
            buffer[7] = (byte)ModBusFunctionMask.WriteCoil;
            buffer[8] = (byte)(address / 256);
            buffer[9] = (byte)(address % 256);
            buffer[10] = (byte)(value.Length / 256);
            buffer[11] = (byte)(value.Length % 256);

            buffer[12] = (byte)(data.Length);

            data.CopyTo(buffer, 13);
            return buffer;
        }

        private byte[] BuildWriteRegister(ushort address, byte[] data)
        {
            if (data == null) data = new byte[0];


            ushort messageId = GetMessageId();
            byte[] buffer = new byte[13 + data.Length];
            buffer[0] = (byte)(messageId / 256);
            buffer[1] = (byte)(messageId % 256);
            buffer[4] = (byte)((buffer.Length - 6) / 256);
            buffer[5] = (byte)((buffer.Length - 6) % 256);
            buffer[6] = station;
            buffer[7] = (byte)ModBusFunctionMask.WriteRegister;
            buffer[8] = (byte)(address / 256);
            buffer[9] = (byte)(address % 256);
            buffer[10] = (byte)(data.Length / 2 / 256);
            buffer[11] = (byte)(data.Length / 2 % 256);

            buffer[12] = (byte)(data.Length);

            data.CopyTo(buffer, 13);

            return buffer;
        }

        #endregion

        #region Public 

        /// <summary>
        /// 接收数据的延时时间
        /// </summary>
        public int ReceiveTimeOut
        {
            get
            {
                return receiveTimeOut;
            }
            set
            {
                receiveTimeOut = value;
            }
        }

        #endregion

        #region Public Method
        
     
        /// <summary>
        /// 读写ModBus服务器的基础方法，支持任意的数据操作
        /// </summary>
        /// <param name="send">发送的字节数据</param>
        /// <returns></returns>
        public OperateResult<byte[]> ReadFromModBusServer(byte[] send)
        {
            OperateResult<byte[]> result = new OperateResult<byte[]>();

            System.Net.Sockets.Socket socket = null;
            if (!isSocketInitialization)
            {
                if (!CreateSocketAndConnect(out socket, GetIPEndPoint(), result))
                {
                    socket = null;
                    return result;
                }
            }
            else
            {
                socket = GetWorkSocket(out OperateResult connect);
                if(!connect.IsSuccess)
                {
                    result.Message = connect.Message;
                    return result;
                }
            }
            

            serverInterfaceLock.Enter();

            if (!SendBytesToSocket(socket, send, result, "发送数据到服务器失败"))
            {
                serverInterfaceLock.Leave();
                return result;
            }


            HslTimeOut hslTimeOut = new HslTimeOut();
            hslTimeOut.WorkSocket = socket;
            hslTimeOut.DelayTime = 5000;                       // 5秒内必须接收到数据
            try
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(
                    ThreadPoolCheckConnect), hslTimeOut);
                byte[] head = NetSupport.ReadBytesFromSocket(socket, 6);

                int length = head[4] * 256 + head[5];
                byte[] data = NetSupport.ReadBytesFromSocket(socket, length);

                byte[] buffer = new byte[6 + length];
                head.CopyTo(buffer, 0);
                data.CopyTo(buffer, 6);
                hslTimeOut.IsSuccessful = true;
                result.Content = buffer;
            }
            catch (Exception ex)
            {
                socket?.Close();
                result.Message = "从服务器接收结果数据的时候发生错误：" + ex.Message;
                serverInterfaceLock.Leave();
                return result;
            }

            if (!isSocketInitialization) socket?.Close();

            serverInterfaceLock.Leave();

            result.IsSuccess = true;
            return result;
        }


        


        /// <summary>
        /// 读取服务器的线圈，对应功能码0x01
        /// </summary>
        /// <param name="function"></param>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private OperateResult<byte[]> ReadModBusBase(ModBusFunctionMask function, ushort address, ushort length)
        {
            OperateResult<byte[]> resultBytes = ReadFromModBusServer(BuildReadCommand(function, address, length));
            if (resultBytes.IsSuccess)
            {
                // 二次数据处理
                if (resultBytes.Content?.Length >= 9)
                {
                    byte[] buffer = new byte[resultBytes.Content.Length - 9];
                    Array.Copy(resultBytes.Content, 9, buffer, 0, buffer.Length);
                    resultBytes.Content = buffer;
                }
                else
                {
                    // 数据异常
                    resultBytes.IsSuccess = false;
                    resultBytes.Message = "数据异常";
                }
            }
            return resultBytes;
        }

        /// <summary>
        /// 读取服务器的线圈，对应功能码0x01
        /// </summary>
        /// <param name="address">读取的起始地址</param>
        /// <param name="length">读取的数据长度</param>
        /// <returns></returns>
        public OperateResult<byte[]> ReadCoil(ushort address, ushort length)
        {
            return ReadModBusBase(ModBusFunctionMask.ReadCoil, address, length);
        }

        /// <summary>
        /// 读取服务器的离散量，对应的功能码0x02
        /// </summary>
        /// <param name="address">读取的起始地址</param>
        /// <param name="length">读取的数据长度</param>
        /// <returns></returns>
        public OperateResult<byte[]> ReadDiscrete(ushort address, ushort length)
        {
            return ReadModBusBase(ModBusFunctionMask.ReadDiscrete, address, length);
        }

        /// <summary>
        /// 读取服务器的寄存器，对应的功能码0x03
        /// </summary>
        /// <param name="address">读取的起始地址</param>
        /// <param name="length">读取的数据长度</param>
        /// <returns></returns>
        public OperateResult<byte[]> ReadRegister(ushort address, ushort length)
        {
            return ReadModBusBase(ModBusFunctionMask.ReadRegister, address, length);
        }


        /// <summary>
        /// 写单个线圈，对应的功能码0x05
        /// </summary>
        /// <param name="address">写入的起始地址</param>
        /// <param name="value">False还是True，代表线圈断和通</param>
        /// <returns></returns>
        public OperateResult WriteOneCoil(ushort address, bool value)
        {
            return ReadFromModBusServer(BuildWriteOneCoil(address, value));
        }

        /// <summary>
        /// 写入一个寄存器的数据，对应的功能码0x06
        /// </summary>
        /// <param name="address">写入的起始地址</param>
        /// <param name="value">有符号的数据值</param>
        /// <returns></returns>
        public OperateResult WriteOneRegister(ushort address, short value)
        {
            return ReadFromModBusServer(BuildWriteOneRegister(address, BitConverter.GetBytes(value)));
        }

        /// <summary>
        /// 写入一个寄存器的数据，对应的功能码0x06
        /// </summary>
        /// <param name="address">写入的起始地址</param>
        /// <param name="value">无符号的数据值</param>
        /// <returns></returns>
        public OperateResult WriteOneRegister(ushort address, ushort value)
        {
            return ReadFromModBusServer(BuildWriteOneRegister(address, BitConverter.GetBytes(value)));
        }

        /// <summary>
        /// 写入一个寄存器的数据，对应的功能码0x06
        /// </summary>
        /// <param name="address">写入的起始地址</param>
        /// <param name="High">高位的数据</param>
        /// <param name="Low">地位的数据</param>
        /// <returns></returns>
        public OperateResult WriteOneRegister(ushort address, byte High, byte Low)
        {
            return ReadFromModBusServer(BuildWriteOneRegister(address, new byte[] { Low, High }));
        }


        /// <summary>
        /// 写线圈数组，线圈数组不能大于2040个，对应的功能码0x0F
        /// </summary>
        /// <param name="address">写入的起始地址</param>
        /// <param name="value">线圈的数组</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public OperateResult WriteCoil(ushort address, bool[] value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (value.Length > 2040) throw new ArgumentOutOfRangeException("value", "长度不能大于2040。");
            return ReadFromModBusServer(BuildWriteCoil(address, value));
        }

        /// <summary>
        /// 写多个寄存器，寄存器的个数不能大于128个，对应功能码0x10
        /// </summary>
        /// <param name="address">写入的起始地址</param>
        /// <param name="value">字节数组</param>
        /// <returns></returns>
        public OperateResult WriteRegister(ushort address, byte[] value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (value.Length > 255) throw new ArgumentOutOfRangeException("value", "长度不能大于255。");
            return ReadFromModBusServer(BuildWriteRegister(address, value));
        }

        /// <summary>
        /// 写多个寄存器，寄存器的个数不能大于128个，对应功能码0x10
        /// </summary>
        /// <param name="address">写入的起始地址</param>
        /// <param name="value">short数组</param>
        /// <returns></returns>
        public OperateResult WriteRegister(ushort address, short[] value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (value.Length > 128) throw new ArgumentOutOfRangeException("value", "长度不能大于128。");

            byte[] buffer = new byte[value.Length * 2];
            for (int i = 0; i < value.Length; i++)
            {
                buffer[2 * i + 0] = BitConverter.GetBytes(value[i])[1];
                buffer[2 * i + 1] = BitConverter.GetBytes(value[i])[0];
            }

            return ReadFromModBusServer(BuildWriteRegister(address, buffer));
        }

        /// <summary>
        /// 写多个寄存器，寄存器的个数不能大于128个，对应功能码0x10
        /// </summary>
        /// <param name="address">写入的起始地址</param>
        /// <param name="value">ushort数组</param>
        /// <returns></returns>
        public OperateResult WriteRegister(ushort address, ushort[] value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (value.Length > 128) throw new ArgumentOutOfRangeException("value", "长度不能大于128。");

            byte[] buffer = new byte[value.Length * 2];
            for (int i = 0; i < value.Length; i++)
            {
                buffer[2 * i + 0] = BitConverter.GetBytes(value[i])[1];
                buffer[2 * i + 1] = BitConverter.GetBytes(value[i])[0];
            }

            return ReadFromModBusServer(BuildWriteRegister(address, buffer));
        }

        #endregion

        #region Object Override

        /// <summary>
        /// 判断实例是否为同一个
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// 用作特定类型的哈希函数
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 获取文本表示的形式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"ModBusTcpClient[{serverEndPoint}]";
        }

        #endregion


    }
}
