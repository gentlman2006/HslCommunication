using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core;
using HslCommunication;
using HslCommunication.Core.Net;
using System.Net.Sockets;

namespace HslCommunication.ModBus.ModbusTcp
{
    public class ModbusTcpServer : NetworkServerBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个Modbus Tcp的服务器，支持数据读写操作
        /// </summary>
        public ModbusTcpServer( )
        {
            Coils = new bool[65536];
            Register = new byte[65536 * 2];
            hybirdLockCoil = new SimpleHybirdLock( );
            hybirdLockRegister = new SimpleHybirdLock( );

            subscriptions = new List<ModBusMonitorAddress>( );
            subcriptionHybirdLock = new SimpleHybirdLock( );
            byteTransform = new ReverseWordTransform( );
        }

        #endregion

        #region Private Method


        /// <summary>
        /// 判断操作线圈或是寄存器的是否发生了越界
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数据长度</param>
        /// <returns>越界返回<c>True</c>，否则返回<c>False</c></returns>
        private bool IsAddressOverBoundary( ushort address, ushort length )
        {
            if ((address + length) >= ushort.MaxValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Public Members


        /// <summary>
        /// 接收到数据的时候就行触发
        /// </summary>
        public event Action<byte[]> OnDataReceived;


        #endregion

        #region Data Pool

        // 数据池，用来模拟真实的Modbus数据区块
        private bool[] Coils;                         // 线圈池
        private SimpleHybirdLock hybirdLockCoil;      // 线圈池的同步锁
        private byte[] Register;                      // 寄存器池
        private SimpleHybirdLock hybirdLockRegister;  // 寄存器池同步锁
        private IByteTransform byteTransform;

        #endregion

        #region Coil Read Write



        /// <summary>
        /// 读取地址的线圈的通断情况
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public bool ReadCoil( ushort address )
        {
            bool result = false;
            hybirdLockCoil.Enter( );
            result = Coils[address];
            hybirdLockCoil.Leave( );
            return result;
        }

        /// <summary>
        /// 批量读取地址的线圈的通断情况
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">读取长度</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public bool[] ReadCoil( ushort address, ushort length )
        {
            bool[] result = new bool[length];
            hybirdLockCoil.Enter( );
            for (int i = address; i < address + length; i++)
            {
                if (i < ushort.MaxValue)
                {
                    result[i - address] = Coils[i];
                }
            }
            hybirdLockCoil.Leave( );
            return result;
        }

        /// <summary>
        /// 写入线圈的通断值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">是否通断</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void WriteCoil( ushort address, bool data )
        {
            hybirdLockCoil.Enter( );
            Coils[address] = data;
            hybirdLockCoil.Leave( );
        }

        /// <summary>
        /// 写入线圈数组的通断值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">是否通断</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void WriteCoil( ushort address, bool[] data )
        {
            if (data == null) return;

            hybirdLockCoil.Enter( );
            for (int i = address; i < address + data.Length; i++)
            {
                if (i < ushort.MaxValue)
                {
                    Coils[i] = data[i - address];
                }
            }
            hybirdLockCoil.Leave( );
        }


        #endregion

        #region Register Read

        /// <summary>
        /// 读取自定义的寄存器的值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数据长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>byte数组值</returns>
        public byte[] ReadRegister( ushort address, ushort length )
        {
            byte[] buffer = new byte[length * 2];
            hybirdLockRegister.Enter( );
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Register[address * 2 + i];
            }
            hybirdLockRegister.Leave( );
            return buffer;
        }


        /// <summary>
        /// 读取一个寄存器的值，返回类型short
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>short值</returns>
        public short ReadInt16( ushort address )
        {
            return byteTransform.TransInt16( ReadRegister( address, 1 ), 0 );
        }

        /// <summary>
        /// 批量读取寄存器的值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">读取的short长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>short数组值</returns>
        public short[] ReadInt16( ushort address, ushort length )
        {
            short[] result = new short[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadInt16( (ushort)(address + i) );
            }
            return result;
        }

        /// <summary>
        /// 读取一个寄存器的值，返回类型为ushort
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>ushort值</returns>
        public ushort ReadUInt16( ushort address )
        {
            return byteTransform.TransUInt16( ReadRegister( address, 1 ), 0 );
        }

        /// <summary>
        /// 批量读取寄存器的值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">读取长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>ushort数组</returns>
        public ushort[] ReadUInt16( ushort address, ushort length )
        {
            ushort[] result = new ushort[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadUInt16( (ushort)(address + i) );
            }
            return result;
        }

        /// <summary>
        /// 读取两个寄存器组成的int值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>int值</returns>
        public int ReadInt32( ushort address )
        {
            return byteTransform.TransInt32( ReadRegister( address, 2 ), 0 );
        }


        /// <summary>
        /// 批量读取寄存器组成的int值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>int数组</returns>
        public int[] ReadInt32( ushort address, ushort length )
        {
            int[] result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadInt32( (ushort)(address + 2 * i) );
            }
            return result;
        }


        /// <summary>
        /// 读取两个寄存器组成的uint值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public uint ReadUInt32( ushort address )
        {
            return byteTransform.TransUInt32( ReadRegister( address, 2 ), 0 );
        }


        /// <summary>
        /// 批量读取寄存器组成的uint值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public uint[] ReadUInt32( ushort address, ushort length )
        {
            uint[] result = new uint[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadUInt32( (ushort)(address + 2 * i) );
            }
            return result;
        }

        /// <summary>
        /// 读取两个寄存器组成的float值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public float ReadFloat( ushort address )
        {
            return byteTransform.TransSingle( ReadRegister( address, 2 ), 0 );
        }

        /// <summary>
        /// 批量读取寄存器组成的float值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public float[] ReadFloat( ushort address, ushort length )
        {
            float[] result = new float[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadFloat( (ushort)(address + 2 * i) );
            }
            return result;
        }


        /// <summary>
        /// 读取四个寄存器组成的long值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public long ReadLong( ushort address )
        {
            return byteTransform.TransInt64( ReadRegister( address, 4 ), 0 );
        }


        /// <summary>
        /// 批量读取寄存器组成的long值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public long[] ReadLong( ushort address, ushort length )
        {
            long[] result = new long[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadLong( (ushort)(address + 4 * i) );
            }
            return result;
        }

        /// <summary>
        /// 读取四个寄存器组成的ulong值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public ulong ReadULong( ushort address )
        {
            return byteTransform.TransUInt64( ReadRegister( address, 4 ), 0 );
        }


        /// <summary>
        /// 批量读取寄存器组成的ulong值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public ulong[] ReadULong( ushort address, ushort length )
        {
            ulong[] result = new ulong[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadULong( (ushort)(address + 4 * i) );
            }
            return result;
        }

        /// <summary>
        /// 读取四个寄存器组成的double值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public double ReadDouble( ushort address )
        {
            return byteTransform.TransDouble( ReadRegister( address, 4 ), 0 );
        }

        /// <summary>
        /// 批量读取寄存器组成的double值
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public double[] ReadDouble( ushort address, ushort length )
        {
            double[] result = new double[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadDouble( (ushort)(address + 4 * i) );
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
        public string ReadString( ushort address, ushort length )
        {
            string str = string.Empty;
            hybirdLockRegister.Enter( );
            str = Encoding.ASCII.GetString( Register, address * 2, length * 2 );
            hybirdLockRegister.Leave( );
            return str;
        }





        #endregion

        #region Register Write


        /// <summary>
        /// 写入寄存器数据，指定字节数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">字节数据</param>
        public void WriteRegister( ushort address, byte[] value )
        {
            hybirdLockRegister.Enter( );
            for (int i = 0; i < value.Length; i++)
            {
                Register[address * 2 + i] = value[i];
            }
            hybirdLockRegister.Leave( );
        }


        /// <summary>
        /// 写入寄存器数据，指定字节数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="high">高位数据</param>
        /// <param name="low">地位数据</param>
        public void WriteRegister( ushort address, byte high, byte low )
        {
            WriteRegister( address, new byte[] { high, low } );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">数据值</param>
        public void WriteRegister( ushort address, short value )
        {
            WriteRegister( address, byteTransform.TransByte( value ) );
        }

        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">数据值</param>
        public void WriteRegister( ushort address, short[] value )
        {
            if (value == null) return;
            WriteRegister( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">数据值</param>
        public void WriteRegister( ushort address, ushort value )
        {
            WriteRegister( address, byteTransform.TransByte( value ) );
        }

        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">数据值</param>
        public void WriteRegister( ushort address, ushort[] value )
        {
            if (value == null) return;
            WriteRegister( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">数据值</param>
        public void WriteRegister( ushort address, int value )
        {
            WriteRegister( address, byteTransform.TransByte( value ) );
        }

        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">数据值</param>
        public void WriteRegister( ushort address, int[] value )
        {
            if (value == null) return;
            WriteRegister( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">数据值</param>
        public void WriteRegister( ushort address, uint value )
        {
            WriteRegister( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">数据值</param>
        public void WriteRegister( ushort address, uint[] value )
        {
            if (value == null) return;
            WriteRegister( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">数据值</param>
        public void WriteRegister( ushort address, float value )
        {
            WriteRegister( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">数据值</param>
        public void WriteRegister( ushort address, float[] value )
        {
            if (value == null) return;
            WriteRegister( address, byteTransform.TransByte( value ) );
        }

        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">数据值</param>
        public void WriteRegister( ushort address, long value )
        {
            WriteRegister( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">数据值</param>
        public void WriteRegister( ushort address, long[] value )
        {
            if (value == null) return;
            WriteRegister( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">数据值</param>
        public void WriteRegister( ushort address, ulong value )
        {
            WriteRegister( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">数据值</param>
        public void WriteRegister( ushort address, ulong[] value )
        {
            if (value == null) return;
            WriteRegister( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">数据值</param>
        public void WriteRegister( ushort address, double value )
        {
            WriteRegister( address, byteTransform.TransByte( value ) );
        }

        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">数据值</param>
        public void WriteRegister( ushort address, double[] value )
        {
            if (value == null) return;
            WriteRegister( address, byteTransform.TransByte( value ) );
        }


        #endregion

        #region NetServer Override


        /// <summary>
        /// 当接收到了新的请求的时候执行的操作
        /// </summary>
        /// <param name="obj"></param>
        protected override void ThreadPoolLogin( object obj )
        {
            // 为了提高系统的响应能力，采用异步来实现，即时有数万台设备接入也能应付
            if (obj is Socket socket)
            {
                ModBusState state = new ModBusState( )
                {
                    WorkSocket = socket,
                };

                try
                {
                    state.IpEndPoint = (System.Net.IPEndPoint)socket.RemoteEndPoint;
                    state.IpAddress = state.IpEndPoint.Address.ToString( );
                }
                catch (Exception ex)
                {
                    LogNet?.WriteException( ToString( ), "Ip信息获取失败", ex );
                }

                LogNet?.WriteDebug( ToString( ), $"客户端 [ {state.IpEndPoint} ] 上线" );

                try
                {
                    state.WorkSocket.BeginReceive( state.HeadByte, 0, 6, SocketFlags.None, new AsyncCallback( ModbusHeadReveiveCallback ), state );
                }
                catch (Exception ex)
                {
                    state = null;
                    state.WorkSocket.Close( );
                    LogNet?.WriteException( ToString(), $"客户端 [ {state.IpEndPoint} ] 头子节接收失败！", ex );
                }
            }
        }


        #endregion

        #region Private Method


        private void ModbusHeadReveiveCallback( IAsyncResult ar )
        {
            if (ar.AsyncState is ModBusState state)
            {
                try
                {
                    int receiveCount = state.WorkSocket.EndReceive( ar );
                    if (receiveCount == 0)
                    {
                        state.WorkSocket?.Close( );
                        state = null;
                        LogNet?.WriteDebug( ToString( ), $"客户端 [ {state.IpEndPoint} ] 下线" );
                        return;
                    }
                    else
                    {
                        state.HeadByteReceivedLength += receiveCount;
                    }
                }
                catch (Exception ex)
                {
                    // 关闭连接，记录日志
                    state.WorkSocket?.Close( );
                    state = null;
                    LogNet?.WriteException( ToString(), $"客户端 [ {state.IpEndPoint} ] 异常下线，消息子节接收失败！", ex );
                    return;
                }

                try
                {
                    if (state.HeadByteReceivedLength < state.HeadByte.Length)
                    {
                        // 数据不够，继续接收
                        state.WorkSocket.BeginReceive( state.HeadByte, state.HeadByteReceivedLength, state.HeadByte.Length - state.HeadByteReceivedLength,
                            SocketFlags.None, new AsyncCallback( ModbusHeadReveiveCallback ), state );
                        return;
                    }
                }
                catch(Exception ex)
                {
                    state.WorkSocket?.Close( );
                    state = null;
                    LogNet?.WriteException( ToString( ), $"客户端 [ {state.IpEndPoint} ] 异常下线，再次启动接收失败！", ex );
                    return;
                }


                // 准备接收的数据长度
                int ContentLength = state.HeadByte[4] * 256 + state.HeadByte[5];
                // 第一次过滤，过滤掉不是Modbus Tcp协议的
                if (state.HeadByte[2] == 0x00 &&
                    state.HeadByte[3] == 0x00 &&
                    ContentLength < 300)
                {
                    try
                    {
                        // 头子节接收完成
                        state.Content = new byte[ContentLength];
                        state.ContentReceivedLength = 0;
                        // 开始接收内容
                        state.WorkSocket.BeginReceive( state.Content, state.ContentReceivedLength, state.Content.Length - state.ContentReceivedLength,
                            SocketFlags.None, new AsyncCallback( ModbusDataReveiveCallback ), state );
                    }
                    catch(Exception ex)
                    {
                        state.WorkSocket?.Close( );
                        state = null;
                        LogNet?.WriteException( ToString( ), $"客户端 [ {state.IpEndPoint} ] 异常下线，启动内容接收失败！", ex );
                        return;
                    }
                }
                else
                {
                    // 关闭连接，记录日志
                    state.WorkSocket?.Close( );
                    state = null;
                    LogNet?.WriteDebug( ToString(), $"客户端 [ {state.IpEndPoint} ] 下线，不是标准的Modbus协议！" );
                }
            }
        }


        private void ModbusDataReveiveCallback( IAsyncResult ar )
        {
            if (ar.AsyncState is ModBusState state)
            {
                try
                {
                    state.ContentReceivedLength += state.WorkSocket.EndReceive( ar );
                    if (state.ContentReceivedLength < state.Content.Length)
                    {
                        // 数据不够，继续接收
                        state.WorkSocket.BeginReceive( state.Content, state.ContentReceivedLength, state.Content.Length - state.ContentReceivedLength,
                            SocketFlags.None, new AsyncCallback( ModbusDataReveiveCallback ), state );
                        return;
                    }
                }
                catch (Exception ex)
                {
                    // 关闭连接，记录日志
                    state.WorkSocket?.Close( );
                    state = null;
                    LogNet?.WriteException( ToString(), $"客户端 [ {state.IpEndPoint} ] 下线，内容数据接收失败！", ex );
                    return;
                }
                

                // 数据接收完成
                // 内容接收完成，所有的数据接收结束
                byte[] data = new byte[state.HeadByte.Length + state.Content.Length];
                state.HeadByte.CopyTo( data, 0 );
                state.Content.CopyTo( data, state.HeadByte.Length );

                state.Clear( );



                if (data[7] == 0x01 ||
                    data[7] == 0x02 ||
                    data[7] == 0x03)
                {
                    if (data.Length != 0x0C)
                    {
                        // 指令长度验证错误，关闭网络连接
                        state.WorkSocket?.Close( );
                        state = null;
                        LogNet?.WriteWarn( LogHeaderText, "Command length check failed！" );
                        return;
                    }
                }



                // 需要回发消息
                byte[] copy = null;
                try
                {
                    switch (data[7])
                    {
                        case 0x01:
                            {
                                // 线圈读取
                                int address = data[8] * 256 + data[9];
                                int length = data[10] * 256 + data[11];
                                if (length > 2048) length = 2048;         // 线圈读取应该小于2048个
                                bool[] read = ReadCoil( (ushort)address, (ushort)length );
                                byte[] buffer = BasicFramework.SoftBasic.BoolArrayToByte( read );
                                copy = new byte[9 + buffer.Length];
                                Array.Copy( data, 0, copy, 0, 8 );
                                copy[4] = (byte)((copy.Length - 6) / 256);
                                copy[5] = (byte)((copy.Length - 6) % 256);
                                copy[8] = (byte)buffer.Length;
                                Array.Copy( buffer, 0, copy, 9, buffer.Length );
                                break;
                            }
                        case 0x03:
                            {
                                // 寄存器读取
                                int address = data[8] * 256 + data[9];
                                int length = data[10] * 256 + data[11];
                                if (length > 127) length = 127;          // 寄存器最大读取范围为127个
                                byte[] buffer = ReadRegister( (ushort)address, (ushort)length );
                                copy = new byte[9 + buffer.Length];
                                Array.Copy( data, 0, copy, 0, 8 );
                                copy[4] = (byte)((copy.Length - 6) / 256);
                                copy[5] = (byte)((copy.Length - 6) % 256);
                                copy[8] = (byte)buffer.Length;
                                Array.Copy( buffer, 0, copy, 9, buffer.Length );
                                break;
                            }
                        case 0x05:
                            {
                                // 写单个线圈
                                int address = data[8] * 256 + data[9];
                                if (data[10] == 0xFF && data[11] == 0x00)
                                {
                                    WriteCoil( (ushort)address, true );
                                }
                                else if (data[10] == 0x00 && data[11] == 0x00)
                                {
                                    WriteCoil( (ushort)address, false );
                                }
                                copy = new byte[12];
                                Array.Copy( data, 0, copy, 0, 12 );
                                copy[4] = 0x00;
                                copy[5] = 0x06;
                                break;
                            }
                        case 0x06:
                            {
                                // 写单个寄存器
                                ushort address = (ushort)(data[8] * 256 + data[9]);
                                short ValueOld = ReadShortRegister( address );
                                // 写入到寄存器
                                WriteRegister( address, data[10], data[11] );
                                short ValueNew = ReadShortRegister( address );
                                // 触发写入请求
                                OnRegisterBeforWrite( address, ValueOld, ValueNew );

                                copy = new byte[12];
                                Array.Copy( data, 0, copy, 0, 12 );
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
                                Array.Copy( data, 13, buffer, 0, buffer.Length );
                                bool[] value = BasicFramework.SoftBasic.ByteToBoolArray( buffer, length );
                                WriteCoil( (ushort)address, value );
                                copy = new byte[12];
                                Array.Copy( data, 0, copy, 0, 12 );
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

                                // 为了使服务器的数据订阅更加的准确，决定将设计改为等待所有的数据写入完成后，再统一触发订阅，2018年3月4日 20:56:47
                                MonitorAddress[] addresses = new MonitorAddress[length];
                                for (int i = 0; i < length; i++)
                                {
                                    if ((2 * i + 14) < data.Length)
                                    {
                                        short ValueOld = ReadShortRegister( (ushort)(address + i) );
                                        WriteRegister( (ushort)(address + i), data[2 * i + 13], data[2 * i + 14] );
                                        short ValueNew = ReadShortRegister( (ushort)(address + i) );
                                        // 触发写入请求
                                        addresses[i] = new MonitorAddress( )
                                        {
                                            Address = (ushort)(address + i),
                                            ValueOrigin = ValueOld,
                                            ValueNew = ValueNew
                                        };
                                    }
                                }

                                // 所有数据都更改完成后，再触发消息
                                for (int i = 0; i < addresses.Length; i++)
                                {
                                    OnRegisterBeforWrite( addresses[i].Address, addresses[i].ValueOrigin, addresses[i].ValueNew );
                                }

                                copy = new byte[12];
                                Array.Copy( data, 0, copy, 0, 12 );
                                copy[4] = 0x00;
                                copy[5] = 0x06;
                                break;
                            }
                        default:
                            {
                                copy = new byte[9];
                                Array.Copy( data, 0, copy, 0, 8 );
                                copy[4] = 0x00;
                                copy[5] = 0x03;
                                copy[7] = (byte)(data[7] + 0x80);
                                copy[8] = 0x01;  // 不支持的功能码
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    state.WorkSocket?.Close( );
                    state = null;
                    LogNet?.WriteException( LogHeaderText, ex );
                    return;
                }



                try
                {
                    // 管他是什么，先开始数据接收
                    // state.WorkSocket?.Close();
                    state.WorkSocket.BeginReceive( state.HeadByte, 0, 6, SocketFlags.None, new AsyncCallback( ModbusHeadReveiveCallback ), state );
                }
                catch (Exception ex)
                {
                    state.WorkSocket?.Close( );
                    state = null;
                    LogNet?.WriteException( LogHeaderText, "Send exception:", ex );
                    return;
                }


                // 回发数据，先获取发送锁
                state.hybirdLock.Enter( );
                state.WorkSocket.BeginSend( copy, 0, size: copy.Length, socketFlags: SocketFlags.None, callback: new AsyncCallback( DataSendCallBack ), state: state );


                // 通知处理消息
                if (IsStarted) OnDataReceived?.Invoke( data );


            }
        }


        private void DataSendCallBack( IAsyncResult ar )
        {
            if (ar.AsyncState is ModBusState state)
            {
                state.hybirdLock.Leave( );
                try
                {
                    state.WorkSocket.EndSend( ar );
                }
                catch (Exception ex)
                {
                    state.WorkSocket?.Close( );
                    state = null;
                    LogNet?.WriteException( LogHeaderText, "内容数据回发失败！", ex );
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
        public void AddSubcription( ModBusMonitorAddress monitor )
        {
            subcriptionHybirdLock.Enter( );
            subscriptions.Add( monitor );
            subcriptionHybirdLock.Leave( );
        }

        /// <summary>
        /// 移除一个数据监视的任务
        /// </summary>
        /// <param name="monitor"></param>
        public void RemoveSubcrption( ModBusMonitorAddress monitor )
        {
            subcriptionHybirdLock.Enter( );
            subscriptions.Remove( monitor );
            subcriptionHybirdLock.Leave( );
        }


        /// <summary>
        /// 在数据变更后，进行触发是否产生订阅
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        private void OnRegisterBeforWrite( ushort address, short before, short after )
        {
            subcriptionHybirdLock.Enter( );
            for (int i = 0; i < subscriptions.Count; i++)
            {
                if (subscriptions[i].Address == address)
                {
                    subscriptions[i].SetValue( after );
                    if (before != after)
                    {
                        subscriptions[i].SetChangeValue( before, after );
                    }
                }
            }
            subcriptionHybirdLock.Leave( );
        }

        #endregion


    }
}
