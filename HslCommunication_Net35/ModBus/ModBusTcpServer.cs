using HslCommunication.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace HslCommunication.ModBus
{



    /***********************************************************************************************
     * 
     *    日期：2017年11月3日 23:09:14
     *    说明：一个支持ModBus Tcp协议的服务器总站，支持来自所有的设备的信息汇总
     *          需要接收到来自设备的数据，区分设备数据有两个方法
     *          1.使用ModBus的
     * 
     ***********************************************************************************************/





    /// <summary>
    /// ModBus Tcp协议的服务器端，支持所有发送到此端口的数据，并对所有的数据提供一个中心处理方法，读取该服务器数据将返回无效数据
    /// </summary>
    public class ModBusTcpServer : NetServerBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个Modbus Tcp的服务器，支持数据读写操作
        /// </summary>
        public ModBusTcpServer()
        {
            Coils = new bool[65536];
            Register = new byte[65536 * 2];
            hybirdLockCoil = new SimpleHybirdLock();
            hybirdLockRegister = new SimpleHybirdLock();
            LogHeaderText = "ModBusTcpServer";

            subscriptions = new List<ModBusMonitorAddress>();
            subcriptionHybirdLock = new SimpleHybirdLock();
        }

        #endregion


        #region Public Members


        /// <summary>
        /// 接收到数据的时候就行触发
        /// </summary>
        public event IEDelegate<byte[]> OnDataReceived;


        #endregion

        #region Data Pool

        // 数据池，用来模拟真实的Modbus数据区块
        private bool[] Coils;                       // 线圈池
        private SimpleHybirdLock hybirdLockCoil;    // 线圈池的同步锁
        private byte[] Register;                    // 寄存器池
        private SimpleHybirdLock hybirdLockRegister;// 寄存器池同步锁

        #endregion

        #region Coil Read Write

        /// <summary>
        /// 读取地址的线圈的通断情况
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public bool ReadCoil(ushort address)
        {
            bool result = false;
            hybirdLockCoil.Enter();
            result = Coils[address];
            hybirdLockCoil.Leave();
            return result;
        }

        /// <summary>
        /// 批量读取地址的线圈的通断情况
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">读取长度</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public bool[] ReadCoil(ushort address, ushort length)
        {
            bool[] result = new bool[length];
            hybirdLockCoil.Enter();
            for (int i = address; i < address + length; i++)
            {
                if (i < ushort.MaxValue)
                {
                    result[i - address] = Coils[i];
                }
            }
            hybirdLockCoil.Leave();
            return result;
        }

        /// <summary>
        /// 写入线圈的通断值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">是否通断</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void WriteCoil(ushort address, bool data)
        {
            hybirdLockCoil.Enter();
            Coils[address] = data;
            hybirdLockCoil.Leave();
        }

        /// <summary>
        /// 写入线圈数组的通断值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">是否通断</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void WriteCoil(ushort address, bool[] data)
        {
            if (data == null) return;

            hybirdLockCoil.Enter();
            for (int i = address; i < address + data.Length; i++)
            {
                if (i < ushort.MaxValue)
                {
                    Coils[i] = data[i - address];
                }
            }
            hybirdLockCoil.Leave();
        }


        #endregion

        #region Register Read

        /// <summary>
        /// 读取自定义的寄存器的值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数据长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public byte[] ReadRegister(ushort address, ushort length)
        {
            byte[] buffer = new byte[length * 2];
            hybirdLockRegister.Enter();
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Register[address * 2 + i];
            }
            hybirdLockRegister.Leave();
            return buffer;
        }


        /// <summary>
        /// 读取一个寄存器的值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public short ReadShortRegister(ushort address)
        {
            byte[] buffer = new byte[2];
            hybirdLockRegister.Enter();
            buffer[0] = Register[address * 2 + 1];
            buffer[1] = Register[address * 2 + 0];
            hybirdLockRegister.Leave();
            return BitConverter.ToInt16(buffer, 0);
        }

        /// <summary>
        /// 批量读取寄存器的值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">读取的short长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public short[] ReadShortRegister(ushort address, ushort length)
        {
            short[] result = new short[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadShortRegister((ushort)(address + i));
            }
            return result;
        }

        /// <summary>
        /// 读取一个寄存器的值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public ushort ReadUShortRegister(ushort address)
        {
            byte[] buffer = new byte[2];
            hybirdLockRegister.Enter();
            buffer[0] = Register[address * 2 + 1];
            buffer[1] = Register[address * 2 + 0];
            hybirdLockRegister.Leave();
            return BitConverter.ToUInt16(buffer, 0);
        }

        /// <summary>
        /// 批量读取寄存器的值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">读取长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public ushort[] ReadUShortRegister(ushort address, ushort length)
        {
            ushort[] result = new ushort[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadUShortRegister((ushort)(address + i));
            }
            return result;
        }

        /// <summary>
        /// 读取两个寄存器组成的int值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public int ReadIntRegister(ushort address)
        {
            byte[] buffer = new byte[4];
            hybirdLockRegister.Enter();
            buffer[0] = Register[address * 2 + 3];
            buffer[1] = Register[address * 2 + 2];
            buffer[2] = Register[address * 2 + 1];
            buffer[3] = Register[address * 2 + 0];
            hybirdLockRegister.Leave();
            return BitConverter.ToInt32(buffer, 0);
        }


        /// <summary>
        /// 批量读取寄存器组成的int值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public int[] ReadIntRegister(ushort address, ushort length)
        {
            int[] result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadIntRegister((ushort)(address + 2 * i));
            }
            return result;
        }


        /// <summary>
        /// 读取两个寄存器组成的uint值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public uint ReadUIntRegister(ushort address)
        {
            byte[] buffer = new byte[4];
            hybirdLockRegister.Enter();
            buffer[0] = Register[address * 2 + 3];
            buffer[1] = Register[address * 2 + 2];
            buffer[2] = Register[address * 2 + 1];
            buffer[3] = Register[address * 2 + 0];
            hybirdLockRegister.Leave();
            return BitConverter.ToUInt32(buffer, 0);
        }


        /// <summary>
        /// 批量读取寄存器组成的uint值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public uint[] ReadUIntRegister(ushort address, ushort length)
        {
            uint[] result = new uint[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadUIntRegister((ushort)(address + 2 * i));
            }
            return result;
        }

        /// <summary>
        /// 读取两个寄存器组成的float值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public float ReadFloatRegister(ushort address)
        {
            byte[] buffer = new byte[4];
            hybirdLockRegister.Enter();
            buffer[0] = Register[address * 2 + 3];
            buffer[1] = Register[address * 2 + 2];
            buffer[2] = Register[address * 2 + 1];
            buffer[3] = Register[address * 2 + 0];
            hybirdLockRegister.Leave();
            return BitConverter.ToSingle(buffer, 0);
        }

        /// <summary>
        /// 批量读取寄存器组成的float值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public float[] ReadFloatRegister(ushort address, ushort length)
        {
            float[] result = new float[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadFloatRegister((ushort)(address + 2 * i));
            }
            return result;
        }


        /// <summary>
        /// 读取四个寄存器组成的long值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public long ReadLongRegister(ushort address)
        {
            byte[] buffer = new byte[8];
            hybirdLockRegister.Enter();
            buffer[0] = Register[address * 2 + 7];
            buffer[1] = Register[address * 2 + 6];
            buffer[2] = Register[address * 2 + 5];
            buffer[3] = Register[address * 2 + 4];
            buffer[4] = Register[address * 2 + 3];
            buffer[5] = Register[address * 2 + 2];
            buffer[6] = Register[address * 2 + 1];
            buffer[7] = Register[address * 2 + 0];
            hybirdLockRegister.Leave();
            return BitConverter.ToInt64(buffer, 0);
        }


        /// <summary>
        /// 批量读取寄存器组成的long值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public long[] ReadLongRegister(ushort address, ushort length)
        {
            long[] result = new long[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadLongRegister((ushort)(address + 4 * i));
            }
            return result;
        }

        /// <summary>
        /// 读取四个寄存器组成的ulong值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public ulong ReadULongRegister(ushort address)
        {
            byte[] buffer = new byte[8];
            hybirdLockRegister.Enter();
            buffer[0] = Register[address * 2 + 7];
            buffer[1] = Register[address * 2 + 6];
            buffer[2] = Register[address * 2 + 5];
            buffer[3] = Register[address * 2 + 4];
            buffer[4] = Register[address * 2 + 3];
            buffer[5] = Register[address * 2 + 2];
            buffer[6] = Register[address * 2 + 1];
            buffer[7] = Register[address * 2 + 0];
            hybirdLockRegister.Leave();
            return BitConverter.ToUInt64(buffer, 0);
        }


        /// <summary>
        /// 批量读取寄存器组成的ulong值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public ulong[] ReadULongRegister(ushort address, ushort length)
        {
            ulong[] result = new ulong[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadULongRegister((ushort)(address + 4 * i));
            }
            return result;
        }

        /// <summary>
        /// 读取四个寄存器组成的double值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public double ReadDoubleRegister(ushort address)
        {
            byte[] buffer = new byte[8];
            hybirdLockRegister.Enter();
            buffer[0] = Register[address * 2 + 7];
            buffer[1] = Register[address * 2 + 6];
            buffer[2] = Register[address * 2 + 5];
            buffer[3] = Register[address * 2 + 4];
            buffer[4] = Register[address * 2 + 3];
            buffer[5] = Register[address * 2 + 2];
            buffer[6] = Register[address * 2 + 1];
            buffer[7] = Register[address * 2 + 0];
            hybirdLockRegister.Leave();
            return BitConverter.ToDouble(buffer, 0);
        }

        /// <summary>
        /// 批量读取寄存器组成的double值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public double[] ReadDoubleRegister(ushort address, ushort length)
        {
            double[] result = new double[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadDoubleRegister((ushort)(address + 4 * i));
            }
            return result;
        }

        /// <summary>
        /// 读取ASCII字符串，长度为寄存器数量，最终的字符串长度为这个值的2倍
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">寄存器长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public string ReadStringRegister(ushort address, ushort length)
        {
            string str = string.Empty;
            hybirdLockRegister.Enter();
            str = Encoding.ASCII.GetString(Register, address * 2, length * 2);
            hybirdLockRegister.Leave();
            return str;
        }





        #endregion

        #region Register Write

        /// <summary>
        /// 写入寄存器数据，指定字节数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="high">高位数据</param>
        /// <param name="low">地位数据</param>
        public void WriteRegister(ushort address, byte high, byte low)
        {
            hybirdLockRegister.Enter();
            Register[address * 2 + 0] = high;
            Register[address * 2 + 1] = low;
            hybirdLockRegister.Leave();
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">数据值</param>
        public void WriteRegister(ushort address, short data)
        {
            byte[] buffer = BitConverter.GetBytes(data);
            WriteRegister(address, buffer[1], buffer[0]);
        }

        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">数据值</param>
        public void WriteRegister(ushort address, short[] data)
        {
            if (data == null) return;
            for (ushort i = 0; i < data.Length; i++)
            {
                WriteRegister((ushort)(address + i), data[i]);
            }
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">数据值</param>
        public void WriteRegister(ushort address, ushort data)
        {
            byte[] buffer = BitConverter.GetBytes(data);
            WriteRegister(address, buffer[1], buffer[0]);
        }

        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">数据值</param>
        public void WriteRegister(ushort address, ushort[] data)
        {
            if (data == null) return;
            for (ushort i = 0; i < data.Length; i++)
            {
                WriteRegister((ushort)(address + i), data[i]);
            }
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">数据值</param>
        public void WriteRegister(ushort address, int data)
        {
            byte[] buffer = BitConverter.GetBytes(data);
            hybirdLockRegister.Enter();
            Register[address * 2 + 3] = buffer[0];
            Register[address * 2 + 2] = buffer[1];
            Register[address * 2 + 1] = buffer[2];
            Register[address * 2 + 0] = buffer[3];
            hybirdLockRegister.Leave();
        }

        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">数据值</param>
        public void WriteRegister(ushort address, int[] data)
        {
            if (data == null) return;
            for (ushort i = 0; i < data.Length; i++)
            {
                WriteRegister((ushort)(address + i * 2), data[i]);
            }
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">数据值</param>
        public void WriteRegister(ushort address, uint data)
        {
            byte[] buffer = BitConverter.GetBytes(data);
            hybirdLockRegister.Enter();
            Register[address * 2 + 3] = buffer[0];
            Register[address * 2 + 2] = buffer[1];
            Register[address * 2 + 1] = buffer[2];
            Register[address * 2 + 0] = buffer[3];
            hybirdLockRegister.Leave();
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">数据值</param>
        public void WriteRegister(ushort address, uint[] data)
        {
            if (data == null) return;
            for (ushort i = 0; i < data.Length; i++)
            {
                WriteRegister((ushort)(address + i * 2), data[i]);
            }
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">数据值</param>
        public void WriteRegister(ushort address, float data)
        {
            byte[] buffer = BitConverter.GetBytes(data);
            hybirdLockRegister.Enter();
            Register[address * 2 + 3] = buffer[0];
            Register[address * 2 + 2] = buffer[1];
            Register[address * 2 + 1] = buffer[2];
            Register[address * 2 + 0] = buffer[3];
            hybirdLockRegister.Leave();
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">数据值</param>
        public void WriteRegister(ushort address, float[] data)
        {
            if (data == null) return;
            for (ushort i = 0; i < data.Length; i++)
            {
                WriteRegister((ushort)(address + i * 2), data[i]);
            }
        }

        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">数据值</param>
        public void WriteRegister(ushort address, long data)
        {
            byte[] buffer = BitConverter.GetBytes(data);
            hybirdLockRegister.Enter();
            Register[address * 2 + 7] = buffer[0];
            Register[address * 2 + 6] = buffer[1];
            Register[address * 2 + 5] = buffer[2];
            Register[address * 2 + 4] = buffer[3];
            Register[address * 2 + 3] = buffer[4];
            Register[address * 2 + 2] = buffer[5];
            Register[address * 2 + 1] = buffer[6];
            Register[address * 2 + 0] = buffer[7];
            hybirdLockRegister.Leave();
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">数据值</param>
        public void WriteRegister(ushort address, long[] data)
        {
            if (data == null) return;
            for (ushort i = 0; i < data.Length; i++)
            {
                WriteRegister((ushort)(address + i * 4), data[i]);
            }
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">数据值</param>
        public void WriteRegister(ushort address, ulong data)
        {
            byte[] buffer = BitConverter.GetBytes(data);
            hybirdLockRegister.Enter();
            Register[address * 2 + 7] = buffer[0];
            Register[address * 2 + 6] = buffer[1];
            Register[address * 2 + 5] = buffer[2];
            Register[address * 2 + 4] = buffer[3];
            Register[address * 2 + 3] = buffer[4];
            Register[address * 2 + 2] = buffer[5];
            Register[address * 2 + 1] = buffer[6];
            Register[address * 2 + 0] = buffer[7];
            hybirdLockRegister.Leave();
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">数据值</param>
        public void WriteRegister(ushort address, ulong[] data)
        {
            if (data == null) return;
            for (ushort i = 0; i < data.Length; i++)
            {
                WriteRegister((ushort)(address + i * 4), data[i]);
            }
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">数据值</param>
        public void WriteRegister(ushort address, double data)
        {
            byte[] buffer = BitConverter.GetBytes(data);
            hybirdLockRegister.Enter();
            Register[address * 2 + 7] = buffer[0];
            Register[address * 2 + 6] = buffer[1];
            Register[address * 2 + 5] = buffer[2];
            Register[address * 2 + 4] = buffer[3];
            Register[address * 2 + 3] = buffer[4];
            Register[address * 2 + 2] = buffer[5];
            Register[address * 2 + 1] = buffer[6];
            Register[address * 2 + 0] = buffer[7];
            hybirdLockRegister.Leave();
        }

        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">数据值</param>
        public void WriteRegister(ushort address, double[] data)
        {
            if (data == null) return;
            for (ushort i = 0; i < data.Length; i++)
            {
                WriteRegister((ushort)(address + i * 4), data[i]);
            }
        }


        #endregion

        #region NetServer Override


        /// <summary>
        /// 当接收到了新的请求的时候执行的操作
        /// </summary>
        /// <param name="obj"></param>
        protected override void ThreadPoolLogin(object obj)
        {
            // 为了提高系统的响应能力，采用异步来实现，即时有数万台设备接入也能应付
            if (obj is Socket socket)
            {
                ModBusState state = new ModBusState()
                {
                    WorkSocket = socket,
                };

                try
                {
                    state.WorkSocket.BeginReceive(state.HeadByte, 0, 6, SocketFlags.None, new AsyncCallback(ModbusHeadReveiveCallback), state);
                }
                catch (Exception ex)
                {
                    state = null;
                    LogNet?.WriteException(LogHeaderText, "头子节接收失败！", ex);
                }
            }
        }


        #endregion


        #region Private Method


        private void ModbusHeadReveiveCallback(IAsyncResult ar)
        {
            if (ar.AsyncState is ModBusState state)
            {
                try
                {
                    state.HeadByteReceivedLength += state.WorkSocket.EndReceive(ar);
                }
                catch (Exception ex)
                {
                    // 关闭连接，记录日志
                    state.WorkSocket?.Close();
                    state = null;
                    LogNet?.WriteException(LogHeaderText, "头子节接收失败！", ex);
                    return;
                }


                if (state.HeadByteReceivedLength < state.HeadByte.Length)
                {
                    // 数据不够，继续接收
                    state.WorkSocket.BeginReceive(state.HeadByte, state.HeadByteReceivedLength, state.HeadByte.Length - state.HeadByteReceivedLength,
                        SocketFlags.None, new AsyncCallback(ModbusHeadReveiveCallback), state);
                    return;
                }


                // 准备接收的数据长度
                int ContentLength = state.HeadByte[4] * 256 + state.HeadByte[5];
                // 第一次过滤，过滤掉不是Modbus Tcp协议的
                if (state.HeadByte[2] == 0x00 &&
                    state.HeadByte[3] == 0x00 &&
                    ContentLength < 300)
                {
                    // 头子节接收完成
                    state.Content = new byte[ContentLength];
                    state.ContentReceivedLength = 0;
                    // 开始接收内容
                    state.WorkSocket.BeginReceive(state.Content, state.ContentReceivedLength, state.Content.Length - state.ContentReceivedLength,
                        SocketFlags.None, new AsyncCallback(ModbusDataReveiveCallback), state);
                }
                else
                {
                    // 关闭连接，记录日志
                    state.WorkSocket?.Close();
                    state = null;
                    LogNet?.WriteDebug(LogHeaderText, "Received Bytes, but is was not modbus tcp protocols");
                }
               
            }
        }


        private void ModbusDataReveiveCallback(IAsyncResult ar)
        {
            if (ar.AsyncState is ModBusState state)
            {
                try
                {
                    state.ContentReceivedLength += state.WorkSocket.EndReceive(ar);
                }
                catch (Exception ex)
                {
                    // 关闭连接，记录日志
                    state.WorkSocket?.Close();
                    state = null;
                    LogNet?.WriteException(LogHeaderText, "内容数据接收失败！", ex);
                    return;
                }

                if (state.ContentReceivedLength < state.Content.Length)
                {
                    // 数据不够，继续接收
                    state.WorkSocket.BeginReceive(state.Content, state.ContentReceivedLength, state.Content.Length - state.ContentReceivedLength,
                        SocketFlags.None, new AsyncCallback(ModbusDataReveiveCallback), state);
                    return;
                }


                // 数据接收完成
                // 内容接收完成，所有的数据接收结束
                byte[] data = new byte[state.HeadByte.Length + state.Content.Length];
                state.HeadByte.CopyTo(data, 0);
                state.Content.CopyTo(data, state.HeadByte.Length);

                state.Clear();



                if (data[7] == 0x01 ||
                    data[7] == 0x02 ||
                    data[7] == 0x03)
                {
                    if (data.Length != 0x0C)
                    {
                        // 指令长度验证错误，关闭网络连接
                        state.WorkSocket?.Close();
                        state = null;
                        LogNet?.WriteWarn(LogHeaderText, "Command length check failed！");
                        return;
                    }
                }



                // 需要回发消息
                byte[] copy = null;
                switch (data[7])
                {
                    case 0x01:
                        {
                            // 线圈读取
                            int address = data[8] * 256 + data[9];
                            int length = data[10] * 256 + data[11];
                            if (length > 2048) length = 2048;         // 线圈读取应该小于2048个
                            bool[] read = ReadCoil((ushort)address, (ushort)length);
                            byte[] buffer = BasicFramework.SoftBasic.BoolArrayToByte(read);
                            copy = new byte[9 + buffer.Length];
                            Array.Copy(data, 0, copy, 0, 8);
                            copy[4] = (byte)((copy.Length - 6) / 256);
                            copy[5] = (byte)((copy.Length - 6) % 256);
                            copy[8] = (byte)buffer.Length;
                            Array.Copy(buffer, 0, copy, 9, buffer.Length);
                            break;
                        }
                    case 0x02:
                        {
                            // 离散值读取，本服务器无效
                            copy = new byte[9];
                            Array.Copy(data, 0, copy, 0, 8);
                            copy[4] = 0x00;
                            copy[5] = 0x03;
                            break;
                        }
                    case 0x03:
                        {
                            // 寄存器读取
                            int address = data[8] * 256 + data[9];
                            int length = data[10] * 256 + data[11];
                            if (length > 127) length = 127;          // 寄存器最大读取范围为127个
                            byte[] buffer = ReadRegister((ushort)address, (ushort)length);
                            copy = new byte[9 + buffer.Length];
                            Array.Copy(data, 0, copy, 0, 8);
                            copy[4] = (byte)((copy.Length - 6) / 256);
                            copy[5] = (byte)((copy.Length - 6) % 256);
                            copy[8] = (byte)buffer.Length;
                            Array.Copy(buffer, 0, copy, 9, buffer.Length);
                            break;
                        }
                    case 0x05:
                        {
                            // 写单个线圈
                            int address = data[8] * 256 + data[9];
                            if (data[10] == 0xFF && data[11] == 0x00)
                            {
                                WriteCoil((ushort)address, true);
                            }
                            else if (data[10] == 0x00 && data[11] == 0x00)
                            {
                                WriteCoil((ushort)address, false);
                            }
                            copy = new byte[12];
                            Array.Copy(data, 0, copy, 0, 12);
                            copy[4] = 0x00;
                            copy[5] = 0x06;
                            break;
                        }
                    case 0x06:
                        {
                            // 写单个寄存器
                            ushort address = (ushort)(data[8] * 256 + data[9]);
                            short ValueOld = ReadShortRegister(address);
                            // 写入到寄存器
                            WriteRegister(address, data[10], data[11]);
                            short ValueNew = ReadShortRegister(address);
                            // 触发写入请求
                            OnRegisterBeforWrite(address, ValueOld, ValueNew);

                            copy = new byte[12];
                            Array.Copy(data, 0, copy, 0, 12);
                            copy[4] = 0x00;
                            copy[5] = 0x06;
                            break;
                        }
                    case 0x0F:
                        {
                            // 写多个线圈
                            int address = data[8] * 256 + data[9];
                            int length = data[10] * 256 + data[11];
                            byte[] buffer = new byte[data.Length - 13];
                            Array.Copy(data, 13, buffer, 0, buffer.Length);
                            bool[] value = BasicFramework.SoftBasic.ByteToBoolArray(buffer, length);
                            WriteCoil((ushort)address, value);
                            copy = new byte[12];
                            Array.Copy(data, 0, copy, 0, 12);
                            copy[4] = 0x00;
                            copy[5] = 0x06;
                            break;
                        }
                    case 0x10:
                        {
                            // 写多个寄存器
                            int address = data[8] * 256 + data[9];
                            int length = data[10] * 256 + data[11];
                            byte[] buffer = new byte[data.Length - 13];
                            for (int i = 0; i < length; i++)
                            {
                                if ((2 * i + 14) < data.Length)
                                {
                                    short ValueOld = ReadShortRegister((ushort)(address + i));
                                    WriteRegister((ushort)(address + i), data[2 * i + 13], data[2 * i + 14]);
                                    short ValueNew = ReadShortRegister((ushort)(address + i));
                                    // 触发写入请求
                                    OnRegisterBeforWrite((ushort)(address + i), ValueOld, ValueNew);
                                }
                            }
                            copy = new byte[12];
                            Array.Copy(data, 0, copy, 0, 12);
                            copy[4] = 0x00;
                            copy[5] = 0x06;
                            break;
                        }
                    default:
                        {
                            if (IsStarted) LogNet?.WriteWarn(LogHeaderText, "Unknown Function Code:" + data[7]);
                            copy = new byte[12];
                            Array.Copy(data, 0, copy, 0, 12);
                            copy[4] = 0x00;
                            copy[5] = 0x06;
                            break;
                        }
                }

                
                
                try
                {
                    // 管他是什么，先开始数据接收
                    // state.WorkSocket?.Close();
                    state.WorkSocket.BeginReceive(state.HeadByte, 0, 6, SocketFlags.None, new AsyncCallback(ModbusHeadReveiveCallback), state);
                }
                catch (Exception ex)
                {
                    state.WorkSocket?.Close();
                    state = null;
                    LogNet?.WriteException(LogHeaderText, "Send exception:", ex);
                    return;
                }


                // 回发数据，先获取发送锁
                state.hybirdLock.Enter();
                state.WorkSocket.BeginSend(copy, 0, size: copy.Length, socketFlags: SocketFlags.None, callback: new AsyncCallback(DataSendCallBack), state: state);


                // 通知处理消息
                if (IsStarted) OnDataReceived?.Invoke(data);


            }
        }


        private void DataSendCallBack(IAsyncResult ar)
        {
            if (ar.AsyncState is ModBusState state)
            {
                state.hybirdLock.Leave();
                try
                {
                    state.WorkSocket.EndSend(ar);
                }
                catch (Exception ex)
                {
                    state.WorkSocket?.Close();
                    state = null;
                    LogNet?.WriteException(LogHeaderText, "内容数据回发失败！", ex);
                }
            }
        }

        #endregion
        
        #region Subscription Support

        // 本服务器端支持指定地址的数据订阅器，目前仅支持寄存器操作

        private List<ModBusMonitorAddress> subscriptions;     // 数据订阅集合
        private SimpleHybirdLock subcriptionHybirdLock;       // 集合锁

        /// <summary>
        /// 新增一个数据监视的任务
        /// </summary>
        /// <param name="monitor"></param>
        public void AddSubcription(ModBusMonitorAddress monitor)
        {
            subcriptionHybirdLock.Enter();
            subscriptions.Add(monitor);
            subcriptionHybirdLock.Leave();
        }

        /// <summary>
        /// 移除一个数据监视的任务
        /// </summary>
        /// <param name="monitor"></param>
        public void RemoveSubcrption(ModBusMonitorAddress monitor)
        {
            subcriptionHybirdLock.Enter();
            subscriptions.Remove(monitor);
            subcriptionHybirdLock.Leave();
        }


        /// <summary>
        /// 在数据变更后，进行触发是否产生订阅
        /// </summary>
        /// <param name="address"></param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        private void OnRegisterBeforWrite(ushort address, short before, short after)
        {
            subcriptionHybirdLock.Enter();

            for (int i = 0; i < subscriptions.Count; i++)
            {
                if (subscriptions[i].Address == address)
                {
                    subscriptions[i].SetValue(after);
                    if (before != after)
                    {
                        subscriptions[i].SetChangeValue(before, after);
                    }
                    break;
                }
            }

            subcriptionHybirdLock.Leave();
        }

        #endregion

    }


    /// <summary>
    /// 服务器端提供的数据监视服务
    /// </summary>
    public class ModBusMonitorAddress
    {
        /// <summary>
        /// 本次数据监视的地址
        /// </summary>
        public ushort Address { get; set; }
        /// <summary>
        /// 数据写入时触发的事件
        /// </summary>
        public event Action<ModBusMonitorAddress, short> OnWrite;
        /// <summary>
        /// 数据改变时触发的事件
        /// </summary>
        public event Action<ModBusMonitorAddress, short, short> OnChange;

        /// <summary>
        /// 强制设置触发事件
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(short value)
        {
            OnWrite?.Invoke(this, value);
        }

        /// <summary>
        /// 强制设置触发值变更事件
        /// </summary>
        /// <param name="before">变更前的值</param>
        /// <param name="after">变更后的值</param>
        public void SetChangeValue(short before, short after)
        {
            if (before != after)
            {
                OnChange?.Invoke(this, before, after);
            }
        }
    }



    /// <summary>
    /// ModBus的异步状态信息
    /// </summary>
    internal class ModBusState
    {
        #region Constructor

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        public ModBusState()
        {
            hybirdLock = new SimpleHybirdLock();
            ConnectTime = DateTime.Now;
            HeadByte = new byte[6];
        }

        #endregion

        /// <summary>
        /// 连接的时间
        /// </summary>
        public DateTime ConnectTime { get; private set; }




        /// <summary>
        /// 工作套接字
        /// </summary>
        public Socket WorkSocket = null;
        /// <summary>
        /// 消息头的缓存
        /// </summary>
        public byte[] HeadByte = new byte[6];

        /// <summary>
        /// 消息头的接收长度
        /// </summary>
        public int HeadByteReceivedLength = 0;

        /// <summary>
        /// 内容数据缓存
        /// </summary>
        public byte[] Content = null;

        /// <summary>
        /// 内容数据接收长度
        /// </summary>
        public int ContentReceivedLength = 0;

        /// <summary>
        /// 回发信息的同步锁
        /// </summary>
        internal SimpleHybirdLock hybirdLock;

        /// <summary>
        /// 清除原先的接收状态
        /// </summary>
        public void Clear()
        {
            Array.Clear(HeadByte, 0, 6);
            HeadByteReceivedLength = 0;
            Content = null;
            ContentReceivedLength = 0;
        }
    }
}
