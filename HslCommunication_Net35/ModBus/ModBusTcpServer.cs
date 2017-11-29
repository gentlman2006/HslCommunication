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
                    state.WorkSocket.BeginReceive(state.HeadByte, 0, 6, SocketFlags.None, new AsyncCallback(HeadReveiveCallBack), state);
                }
                catch (Exception ex)
                {
                    if(IsStarted) LogNet?.WriteException("头子节接收失败！", ex);
                }
            }
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
        public void WriteCoil(ushort address,bool data)
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
        /// 读取一个寄存器的值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public short ReadShortRegister(ushort address)
        {
            byte[] buffer = new byte[2];
            hybirdLockRegister.Enter();
            buffer[0] = Register[address + 1];
            buffer[1] = Register[address];
            hybirdLockRegister.Leave();
            return BitConverter.ToInt16(buffer, 0);
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
            buffer[0] = Register[address + 1];
            buffer[1] = Register[address];
            hybirdLockRegister.Leave();
            return BitConverter.ToUInt16(buffer, 0);
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
            buffer[0] = Register[address + 3];
            buffer[1] = Register[address + 2];
            buffer[2] = Register[address + 1];
            buffer[3] = Register[address + 0];
            hybirdLockRegister.Leave();
            return BitConverter.ToInt32(buffer, 0);
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
            buffer[0] = Register[address + 3];
            buffer[1] = Register[address + 2];
            buffer[2] = Register[address + 1];
            buffer[3] = Register[address + 0];
            hybirdLockRegister.Leave();
            return BitConverter.ToUInt32(buffer, 0);
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
            buffer[0] = Register[address + 3];
            buffer[1] = Register[address + 2];
            buffer[2] = Register[address + 1];
            buffer[3] = Register[address + 0];
            hybirdLockRegister.Leave();
            return BitConverter.ToSingle(buffer, 0);
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
            buffer[0] = Register[address + 7];
            buffer[1] = Register[address + 6];
            buffer[2] = Register[address + 5];
            buffer[3] = Register[address + 4];
            buffer[4] = Register[address + 3];
            buffer[5] = Register[address + 2];
            buffer[6] = Register[address + 1];
            buffer[7] = Register[address + 0];
            hybirdLockRegister.Leave();
            return BitConverter.ToInt64(buffer, 0);
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
            buffer[0] = Register[address + 7];
            buffer[1] = Register[address + 6];
            buffer[2] = Register[address + 5];
            buffer[3] = Register[address + 4];
            buffer[4] = Register[address + 3];
            buffer[5] = Register[address + 2];
            buffer[6] = Register[address + 1];
            buffer[7] = Register[address + 0];
            hybirdLockRegister.Leave();
            return BitConverter.ToUInt64(buffer, 0);
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
            buffer[0] = Register[address + 7];
            buffer[1] = Register[address + 6];
            buffer[2] = Register[address + 5];
            buffer[3] = Register[address + 4];
            buffer[4] = Register[address + 3];
            buffer[5] = Register[address + 2];
            buffer[6] = Register[address + 1];
            buffer[7] = Register[address + 0];
            hybirdLockRegister.Leave();
            return BitConverter.ToDouble(buffer, 0);
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
            str = Encoding.ASCII.GetString(Register, address, length * 2);
            hybirdLockRegister.Leave();
            return str;
        }





        #endregion

        #region Register Write


        public void WriteRegister(ushort address, short data)
        {

        }

        #endregion

        #region Private Method


        private void HeadReveiveCallBack(IAsyncResult ar)
        {
            if (ar.AsyncState is ModBusState state)
            {
                try
                {
                    int count = state.WorkSocket.EndReceive(ar);

                    state.HeadByteReceivedLength += count;
                    if (state.HeadByteReceivedLength < state.HeadByte.Length)
                    {
                        // 数据不够，继续接收
                        state.WorkSocket.BeginReceive(state.HeadByte, state.HeadByteReceivedLength, state.HeadByte.Length - state.HeadByteReceivedLength,
                            SocketFlags.None, new AsyncCallback(HeadReveiveCallBack), state);
                    }
                    else
                    {
                        // 第一次过滤，过滤掉不是Modbus Tcp协议的
                        if (state.HeadByte[2] == 0x00 &&
                            state.HeadByte[3] == 0x00)
                        {
                            // 头子节接收完成
                            int ContentLength = state.HeadByte[4] * 256 + state.HeadByte[5];
                            state.Content = new byte[ContentLength];
                            
                            // 开始接收内容
                            state.WorkSocket.BeginReceive(state.Content, state.ContentReceivedLength, state.Content.Length - state.ContentReceivedLength,
                                SocketFlags.None, new AsyncCallback(ContentReveiveCallBack), state);
                        }
                        else
                        {
                            // 关闭连接，记录日志
                            state.WorkSocket?.Close();
                            state = null;
                            if (IsStarted) LogNet?.WriteDebug("Received Bytes, but is was not modbus tcp");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 关闭连接，记录日志
                    state.WorkSocket?.Close();
                    state = null;
                    if(IsStarted) LogNet?.WriteException("头子节接收失败！", ex);
                }
            }
        }


        private void ContentReveiveCallBack(IAsyncResult ar)
        {
            if (ar.AsyncState is ModBusState state)
            {
                try
                {
                    int count = state.WorkSocket.EndReceive(ar);

                    state.ContentReceivedLength += count;
                    if (state.ContentReceivedLength < state.Content.Length)
                    {
                        // 数据不够，继续接收
                        state.WorkSocket.BeginReceive(state.Content, state.ContentReceivedLength, state.Content.Length - state.ContentReceivedLength,
                            SocketFlags.None, new AsyncCallback(ContentReveiveCallBack), state);
                    }
                    else
                    {
                        // 内容接收完成，所有的数据接收结束
                        byte[] data = new byte[state.HeadByte.Length + state.Content.Length];
                        state.HeadByte.CopyTo(data, 0);
                        state.Content.CopyTo(data, state.HeadByte.Length);

                        state.Clear();

                        // 重新启动接收
                        state.WorkSocket.BeginReceive(state.HeadByte, state.HeadByteReceivedLength, state.HeadByte.Length, SocketFlags.None, new AsyncCallback(HeadReveiveCallBack), state);

                        // 需要回发消息
                        byte[] copy = null;
                        if (data[7] < 0x05)
                        {
                            // 读取数据的情况，返回的数据长度为0
                            copy = new byte[9];
                            Array.Copy(data, 0, copy, 0, 8);
                            copy[4] = 0x00;
                            copy[5] = 0x03;
                        }
                        else
                        {
                            // 数据写入情况，返回写入指令，去掉数据值
                            copy = new byte[12];
                            Array.Copy(data, 0, copy, 0, 12);
                            copy[4] = 0x00;
                            copy[5] = 0x06;
                        }

                        // 回发数据
                        state.WorkSocket.BeginSend(copy, 0, size: copy.Length, socketFlags: SocketFlags.None, callback: new AsyncCallback(DataSendCallBack), state: state);
                        
                        // 通知处理消息
                        
                        if(IsStarted) OnDataReceived?.Invoke(data);
                    }
                }
                catch (Exception ex)
                {
                    // 关闭连接，记录日志
                    state.WorkSocket?.Close();
                    state = null;
                    if(IsStarted) LogNet?.WriteException("头子节接收失败！", ex);
                }
            }
        }


        private void DataSendCallBack(IAsyncResult ar)
        {
            if(ar is ModBusState state)
            {
                try
                {
                    state.WorkSocket.EndSend(ar);
                }
                catch(Exception ex)
                {
                    state.WorkSocket?.Close();
                    state = null;
                    if(IsStarted) LogNet?.WriteException("头子节接收失败！", ex);
                }
            }
        }
        
        #endregion

        #region Private Feilds
        


        #endregion

    }






    /// <summary>
    /// ModBus的异步状态信息
    /// </summary>
    internal class ModBusState
    {
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
