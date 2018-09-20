using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core;
using HslCommunication;
using HslCommunication.Core.Net;
using System.Net.Sockets;
using HslCommunication.Core.IMessage;

#if !NETSTANDARD2_0
using System.IO.Ports;
#endif

namespace HslCommunication.ModBus
{
    /// <summary>
    /// Modbus的虚拟服务器，同时支持Tcp和Rtu的机制，支持线圈，离散输入，寄存器和输入寄存器的读写操作，可以用来当做系统的数据交换池
    /// </summary>
    /// <remarks>
    /// 可以基于本类实现一个功能复杂的modbus服务器，在传统的.NET版本里，还支持modbus-rtu指令的收发，.NET Standard版本服务器不支持rtu操作。服务器支持的数据池如下：
    /// <list type="number">
    /// <item>线圈，功能码对应01，05，15</item>
    /// <item>离散输入，功能码对应02</item>
    /// <item>寄存器，功能码对应03，06，16</item>
    /// <item>输入寄存器，功能码对应04</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// 读写的地址格式为富文本地址，具体请参照下面的示例代码。
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Modbus\ModbusTcpServer.cs" region="ModbusTcpServerExample" title="ModbusTcpServer示例" />
    /// </example>
    public class ModbusTcpServer : NetworkServerBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个Modbus Tcp的服务器，支持数据读写操作
        /// </summary>
        public ModbusTcpServer( )
        {
            Coils = new bool[DataPoolLength];
            InputCoils = new bool[DataPoolLength];
            Register = new byte[DataPoolLength * 2];
            InputRegister = new byte[DataPoolLength * 2];
            hybirdLockCoil = new SimpleHybirdLock( );
            hybirdLockInput = new SimpleHybirdLock( );
            hybirdLockRegister = new SimpleHybirdLock( );
            hybirdLockInputR = new SimpleHybirdLock( );
            lock_trusted_clients = new SimpleHybirdLock( );

            subscriptions = new List<ModBusMonitorAddress>( );
            subcriptionHybirdLock = new SimpleHybirdLock( );
            byteTransform = new ReverseWordTransform( );

#if !NETSTANDARD2_0
            serialPort = new SerialPort( );
#endif
        }

        #endregion

        #region Public Members


        /// <summary>
        /// 接收到数据的时候就行触发
        /// </summary>
        public event Action<ModbusTcpServer, byte[]> OnDataReceived;

        /// <summary>
        /// 当前在线的客户端的数量
        /// </summary>
        public int OnlineCount => onlineCount;

        /// <summary>
        /// 获取或设置数据解析的格式，默认ABCD，可选BADC，CDAB，DCBA格式
        /// </summary>
        /// <remarks>
        /// 对于Int32,UInt32,float,double,Int64,UInt64类型来说，存在多地址的电脑情况，需要和服务器进行匹配
        /// </remarks>
        public DataFormat DataFormat
        {
            get { return byteTransform.DataFormat; }
            set { byteTransform.DataFormat = value; }
        }

        /// <summary>
        /// 字符串数据是否按照字来反转
        /// </summary>
        public bool IsStringReverse
        {
            get { return byteTransform.IsStringReverse; }
            set { byteTransform.IsStringReverse = value; }
        }


        /// <summary>
        /// 获取或设置服务器的站号信息，对于rtu模式，只有站号对了，才会反馈回数据信息。默认为1。
        /// </summary>
        public int Station
        {
            get { return station; }
            set { station = value; }
        }

        #endregion

        #region Data Pool

        // 数据池，用来模拟真实的Modbus数据区块
        private bool[] Coils;                         // 线圈池
        private SimpleHybirdLock hybirdLockCoil;      // 线圈池的同步锁
        private bool[] InputCoils;                    // 只读的输入线圈
        private SimpleHybirdLock hybirdLockInput;     // 只读输入线圈的同步锁
        private byte[] Register;                      // 寄存器池
        private SimpleHybirdLock hybirdLockRegister;  // 寄存器池同步锁
        private byte[] InputRegister;                 // 输入寄存器
        private SimpleHybirdLock hybirdLockInputR;    // 输入寄存器的同步锁
        private ReverseWordTransform byteTransform;
        private const int DataPoolLength = 65536;     // 数据的长度
        private int station = 1;                      // 服务器的站号数据，对于tcp无效，对于rtu来说，如果小于0，则忽略站号信息

        #endregion

        #region Data Persistence

        /// <summary>
        /// 将本系统的数据池数据存储到指定的文件
        /// </summary>
        /// <param name="path">文件的路径</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="System.IO.PathTooLongException"></exception>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <example>
        /// modbusTcpServer.SaveDataPool("data.txt"); // 在当前的程序目录下存储数据池信息，当程序关闭的时候需要进行存储
        /// </example>
        public void SaveDataPool( string path )
        {
            byte[] buffer = new byte[DataPoolLength * 6];
            // 存储线圈
            hybirdLockCoil.Enter( );
            for (int i = 0; i < DataPoolLength; i++)
            {
                if (Coils[i]) buffer[i] = 0x01;
            }
            hybirdLockCoil.Leave( );

            // 存储离散信息
            hybirdLockInput.Enter( );
            for (int i = 0; i < DataPoolLength; i++)
            {
                if (InputCoils[i]) buffer[DataPoolLength + i] = 0x01;
            }
            hybirdLockInput.Leave( );

            // 存储寄存器
            hybirdLockRegister.Enter( );
            Array.Copy( Register, 0, buffer, DataPoolLength * 2, DataPoolLength * 2 );
            hybirdLockRegister.Leave( );

            // 存储输入寄存器
            hybirdLockInputR.Enter( );
            Array.Copy( InputRegister, 0, buffer, DataPoolLength * 4, DataPoolLength * 2 );
            hybirdLockInputR.Leave( );


            System.IO.File.WriteAllBytes( path, buffer );
        }

        /// <summary>
        /// 从文件加载数据池信息
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="System.IO.PathTooLongException"></exception>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <example>
        /// modbusTcpServer.LoadDataPool("data.txt"); // 从当前的程序目录下为文件加载数据池信息，当程序打开的时候需要进行加载
        /// </example>
        public void LoadDataPool( string path )
        {
            byte[] buffer = System.IO.File.ReadAllBytes( path );
            if (buffer.Length < DataPoolLength * 6) throw new Exception( "File is not correct" );

            // 线圈数据加载
            hybirdLockCoil.Enter( );
            for (int i = 0; i < DataPoolLength; i++)
            {
                if (buffer[i] == 0x01) Coils[i] = true;
            }
            hybirdLockCoil.Leave( );

            // 离散寄存器信息加载
            hybirdLockInput.Enter( );
            for (int i = 0; i < DataPoolLength; i++)
            {
                if (buffer[DataPoolLength + i] == 0x01) InputCoils[i] = true;
            }
            hybirdLockInput.Leave( );

            hybirdLockRegister.Enter( );
            Array.Copy( buffer, DataPoolLength * 2, Register, 0, DataPoolLength * 2 );
            hybirdLockRegister.Leave( );

            hybirdLockInputR.Enter( );
            Array.Copy( buffer, DataPoolLength * 4, InputRegister, 0, DataPoolLength * 2 );
            hybirdLockInputR.Leave( );
        }

        #endregion

        #region Coil Read Write



        /// <summary>
        /// 读取地址的线圈的通断情况
        /// </summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public bool ReadCoil( string address )
        {
            ushort add = ushort.Parse( address );
            bool result = false;
            hybirdLockCoil.Enter( );
            result = Coils[add];
            hybirdLockCoil.Leave( );
            return result;
        }


        /// <summary>
        /// 批量读取地址的线圈的通断情况
        /// </summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <param name="length">读取长度</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public bool[] ReadCoil( string address, ushort length )
        {
            ushort add = ushort.Parse( address );
            bool[] result = new bool[length];
            hybirdLockCoil.Enter( );
            for (int i = add; i < add + length; i++)
            {
                if (i <= ushort.MaxValue)
                {
                    result[i - add] = Coils[i];
                }
            }
            hybirdLockCoil.Leave( );
            return result;
        }

        /// <summary>
        /// 写入线圈的通断值
        /// </summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <param name="data">是否通断</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void WriteCoil( string address, bool data )
        {
            ushort add = ushort.Parse( address );
            hybirdLockCoil.Enter( );
            Coils[add] = data;
            hybirdLockCoil.Leave( );
        }

        /// <summary>
        /// 写入线圈数组的通断值
        /// </summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <param name="data">是否通断</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void WriteCoil( string address, bool[] data )
        {
            if (data == null) return;
            ushort add = ushort.Parse( address );

            hybirdLockCoil.Enter( );
            for (int i = add; i < add + data.Length; i++)
            {
                if (i <= ushort.MaxValue)
                {
                    Coils[i] = data[i - add];
                }
            }
            hybirdLockCoil.Leave( );
        }


        #endregion

        #region Discrete Read Write


        /// <summary>
        /// 读取地址的离散线圈的通断情况
        /// </summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public bool ReadDiscrete( string address )
        {
            bool result = false;
            ushort add = ushort.Parse( address );
            hybirdLockInput.Enter( );
            result = InputCoils[add];
            hybirdLockInput.Leave( );
            return result;
        }


        /// <summary>
        /// 批量读取地址的离散线圈的通断情况
        /// </summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <param name="length">读取长度</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public bool[] ReadDiscrete( string address, ushort length )
        {
            bool[] result = new bool[length];
            ushort add = ushort.Parse( address );
            hybirdLockInput.Enter( );
            for (int i = add; i < add + length; i++)
            {
                if (i <= ushort.MaxValue)
                {
                    result[i - add] = InputCoils[i];
                }
            }
            hybirdLockInput.Leave( );
            return result;
        }

        /// <summary>
        /// 写入离散线圈的通断值
        /// </summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <param name="data">是否通断</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void WriteDiscrete( string address, bool data )
        {
            ushort add = ushort.Parse( address );
            hybirdLockInput.Enter( );
            InputCoils[add] = data;
            hybirdLockInput.Leave( );
        }

        /// <summary>
        /// 写入离散线圈数组的通断值
        /// </summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <param name="data">是否通断</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void WriteDiscrete( string address, bool[] data )
        {
            if (data == null) return;
            ushort add = ushort.Parse( address );
            hybirdLockInput.Enter( );
            for (int i = add; i < add + data.Length; i++)
            {
                if (i <= ushort.MaxValue)
                {
                    InputCoils[i] = data[i - add];
                }
            }
            hybirdLockInput.Leave( );
        }


        #endregion

        #region Register Read

        /// <summary>
        /// 读取自定义的寄存器的值
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="length">数据长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>byte数组值</returns>
        public byte[] ReadRegister( string address, ushort length )
        {
            byte[] buffer = new byte[length * 2];

            ModbusAddress mAddress = new ModbusAddress( address );

            if (mAddress.Function == ModbusInfo.ReadRegister)
            {
                hybirdLockRegister.Enter( );
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = Register[mAddress.Address * 2 + i];
                }
                hybirdLockRegister.Leave( );
            }
            else if (mAddress.Function == ModbusInfo.ReadInputRegister)
            {
                hybirdLockInputR.Enter( );
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = InputRegister[mAddress.Address * 2 + i];
                }
                hybirdLockInputR.Leave( );
            }
            return buffer;
        }


        /// <summary>
        /// 读取一个寄存器的值，返回类型short
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>short值</returns>
        public short ReadInt16( string address )
        {
            return byteTransform.TransInt16( ReadRegister( address, 1 ), 0 );
        }

        /// <summary>
        /// 批量读取寄存器的值
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="length">读取的short长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>short数组值</returns>
        public short[] ReadInt16( string address, ushort length )
        {
            short[] result = new short[length];
            ModbusAddress mAddress = new ModbusAddress( address );
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadInt16( mAddress.AddressAdd( i ).ToString( ) );
            }
            return result;
        }

        /// <summary>
        /// 读取一个寄存器的值，返回类型为ushort
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>ushort值</returns>
        public ushort ReadUInt16( string address )
        {
            return byteTransform.TransUInt16( ReadRegister( address, 1 ), 0 );
        }

        /// <summary>
        /// 批量读取寄存器的值
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="length">读取长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>ushort数组</returns>
        public ushort[] ReadUInt16( string address, ushort length )
        {
            ushort[] result = new ushort[length];
            ModbusAddress mAddress = new ModbusAddress( address );
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadUInt16( mAddress.AddressAdd( i ).ToString( ) );
            }
            return result;
        }

        /// <summary>
        /// 读取两个寄存器组成的int值
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>int值</returns>
        public int ReadInt32( string address )
        {
            return byteTransform.TransInt32( ReadRegister( address, 2 ), 0 );
        }


        /// <summary>
        /// 批量读取寄存器组成的int值
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="length">数组长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>int数组</returns>
        public int[] ReadInt32( string address, ushort length )
        {
            int[] result = new int[length];
            ModbusAddress mAddress = new ModbusAddress( address );
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadInt32( mAddress.AddressAdd( i * 2 ).ToString( ) );
            }
            return result;
        }


        /// <summary>
        /// 读取两个寄存器组成的uint值
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>uint值</returns>
        public uint ReadUInt32( string address )
        {
            return byteTransform.TransUInt32( ReadRegister( address, 2 ), 0 );
        }


        /// <summary>
        /// 批量读取寄存器组成的uint值
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="length">数组长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>uint数组</returns>
        public uint[] ReadUInt32( string address, ushort length )
        {
            uint[] result = new uint[length];
            ModbusAddress mAddress = new ModbusAddress( address );
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadUInt32( mAddress.AddressAdd( i * 2 ).ToString( ) );
            }
            return result;
        }

        /// <summary>
        /// 读取两个寄存器组成的float值
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>float值</returns>
        public float ReadFloat( string address )
        {
            return byteTransform.TransSingle( ReadRegister( address, 2 ), 0 );
        }

        /// <summary>
        /// 批量读取寄存器组成的float值
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="length">数组长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>float数组</returns>
        public float[] ReadFloat( string address, ushort length )
        {
            float[] result = new float[length];
            ModbusAddress mAddress = new ModbusAddress( address );
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadFloat( mAddress.AddressAdd( i * 2 ).ToString( ) );
            }
            return result;
        }


        /// <summary>
        /// 读取四个寄存器组成的long值
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>long值</returns>
        public long ReadInt64( string address )
        {
            return byteTransform.TransInt64( ReadRegister( address, 4 ), 0 );
        }


        /// <summary>
        /// 批量读取寄存器组成的long值
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="length">数组长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>long数组</returns>
        public long[] ReadInt64( string address, ushort length )
        {
            long[] result = new long[length];
            ModbusAddress mAddress = new ModbusAddress( address );
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadInt64( mAddress.AddressAdd( i * 4 ).ToString( ) );
            }
            return result;
        }

        /// <summary>
        /// 读取四个寄存器组成的ulong值
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>ulong值</returns>
        public ulong ReadUInt64( string address )
        {
            return byteTransform.TransUInt64( ReadRegister( address, 4 ), 0 );
        }


        /// <summary>
        /// 批量读取寄存器组成的ulong值
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="length">数组长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>ulong数组</returns>
        public ulong[] ReadUInt64( string address, ushort length )
        {
            ulong[] result = new ulong[length];
            ModbusAddress mAddress = new ModbusAddress( address );
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadUInt64( mAddress.AddressAdd( i * 4 ).ToString( ) );
            }
            return result;
        }

        /// <summary>
        /// 读取四个寄存器组成的double值
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>double值</returns>
        public double ReadDouble( string address )
        {
            return byteTransform.TransDouble( ReadRegister( address, 4 ), 0 );
        }

        /// <summary>
        /// 批量读取寄存器组成的double值
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="length">数组长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>double数组</returns>
        public double[] ReadDouble( string address, ushort length )
        {
            double[] result = new double[length];
            ModbusAddress mAddress = new ModbusAddress( address );
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadDouble( mAddress.AddressAdd( i * 4 ).ToString( ) );
            }
            return result;
        }

        /// <summary>
        /// 读取ASCII字符串，长度为寄存器数量，最终的字符串长度为这个值的2倍
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="length">寄存器长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>字符串信息</returns>
        public string ReadString( string address, ushort length )
        {
            string str = string.Empty;
            ModbusAddress mAddress = new ModbusAddress( address );
            if (mAddress.Function == ModbusInfo.ReadRegister)
            {
                hybirdLockRegister.Enter( );
                str = byteTransform.TransString( Register, mAddress.Address * 2, length * 2, Encoding.ASCII );
                hybirdLockRegister.Leave( );
            }
            else if (mAddress.Function == ModbusInfo.ReadInputRegister)
            {
                hybirdLockInputR.Enter( );
                str = byteTransform.TransString( InputRegister, mAddress.Address * 2, length * 2, Encoding.ASCII );
                hybirdLockInputR.Leave( );
            }

            return str;
        }





        #endregion

        #region Register Write


        /// <summary>
        /// 写入寄存器数据，指定字节数据
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="value">字节数据</param>
        public void Write( string address, byte[] value )
        {
            ModbusAddress mAddress = new ModbusAddress( address );
            if (mAddress.Function == ModbusInfo.ReadRegister)
            {
                hybirdLockRegister.Enter( );
                for (int i = 0; i < value.Length; i++)
                {
                    Register[mAddress.Address * 2 + i] = value[i];
                }
                hybirdLockRegister.Leave( );
            }
            else if (mAddress.Function == ModbusInfo.ReadInputRegister)
            {
                hybirdLockInputR.Enter( );
                for (int i = 0; i < value.Length; i++)
                {
                    InputRegister[mAddress.Address * 2 + i] = value[i];
                }
                hybirdLockInputR.Leave( );
            }
        }


        /// <summary>
        /// 写入寄存器数据，指定字节数据
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="high">高位数据</param>
        /// <param name="low">地位数据</param>
        public void Write( string address, byte high, byte low )
        {
            Write( address, new byte[] { high, low } );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="value">数据值</param>
        public void Write( string address, short value )
        {
            Write( address, byteTransform.TransByte( value ) );
        }

        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="value">数据值</param>
        public void Write( string address, short[] value )
        {
            if (value == null) return;
            Write( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="value">数据值</param>
        public void Write( string address, ushort value )
        {
            Write( address, byteTransform.TransByte( value ) );
        }

        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="value">数据值</param>
        public void Write( string address, ushort[] value )
        {
            if (value == null) return;
            Write( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="value">数据值</param>
        public void Write( string address, int value )
        {
            Write( address, byteTransform.TransByte( value ) );
        }

        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="value">数据值</param>
        public void Write( string address, int[] value )
        {
            if (value == null) return;
            Write( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="value">数据值</param>
        public void Write( string address, uint value )
        {
            Write( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="value">数据值</param>
        public void Write( string address, uint[] value )
        {
            if (value == null) return;
            Write( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="value">数据值</param>
        public void Write( string address, float value )
        {
            Write( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="value">数据值</param>
        public void Write( string address, float[] value )
        {
            if (value == null) return;
            Write( address, byteTransform.TransByte( value ) );
        }

        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="value">数据值</param>
        public void Write( string address, long value )
        {
            Write( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="value">数据值</param>
        public void Write( string address, long[] value )
        {
            if (value == null) return;
            Write( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="value">数据值</param>
        public void Write( string address, ulong value )
        {
            Write( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="value">数据值</param>
        public void Write( string address, ulong[] value )
        {
            if (value == null) return;
            Write( address, byteTransform.TransByte( value ) );
        }


        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数据
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="value">数据值</param>
        public void Write( string address, double value )
        {
            Write( address, byteTransform.TransByte( value ) );
        }

        /// <summary>
        /// 往Modbus服务器中的指定寄存器写入数组
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="value">数据值</param>
        public void Write( string address, double[] value )
        {
            if (value == null) return;
            Write( address, byteTransform.TransByte( value ) );
        }

        /// <summary>
        /// 往Mobus服务器中的指定寄存器写入字符串
        /// </summary>
        /// <param name="address">其实地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="value">ASCII编码的字符串的信息</param>
        public void Write( string address, string value )
        {
            byte[] buffer = byteTransform.TransByte( value, Encoding.ASCII );
            buffer = BasicFramework.SoftBasic.ArrayExpandToLengthEven( buffer );
            Write( address, buffer );
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
                    LogNet?.WriteException( ToString( ), StringResources.Language.GetClientIpaddressFailed, ex );
                }

                if (IsTrustedClientsOnly)
                {
                    // 检查受信任的情况
                    if (!CheckIpAddressTrusted( state.IpAddress ))
                    {
                        // 客户端不被信任，退出
                        LogNet?.WriteDebug( ToString( ), string.Format( StringResources.Language.ClientDisableLogin, state.IpEndPoint ) );
                        state.WorkSocket.Close( );
                        return;
                    }
                }

                LogNet?.WriteDebug( ToString( ), string.Format( StringResources.Language.ClientOnlineInfo, state.IpEndPoint ) );

                try
                {
                    state.WorkSocket.BeginReceive( state.HeadByte, 0, 6, SocketFlags.None, new AsyncCallback( ModbusHeadReveiveCallback ), state );
                    System.Threading.Interlocked.Increment( ref onlineCount );
                }
                catch (Exception ex)
                {
                    state.WorkSocket?.Close( );
                    LogNet?.WriteException( ToString( ), $"客户端 [ {state.IpEndPoint} ] 头子节接收失败！", ex );
                    state = null;
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
                        if (state.IsModbusOffline( ))
                        {
                            LogNet?.WriteDebug( ToString( ), string.Format( StringResources.Language.ClientOfflineInfo, state.IpEndPoint ) );
                            System.Threading.Interlocked.Decrement( ref onlineCount );
                        }
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
                    if (state.IsModbusOffline( ))
                    {
                        LogNet?.WriteException( ToString( ), string.Format( StringResources.Language.ClientOfflineInfo, state.IpEndPoint ), ex );
                        System.Threading.Interlocked.Decrement( ref onlineCount );
                    }
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
                catch (Exception ex)
                {
                    state.WorkSocket?.Close( );
                    if (state.IsModbusOffline( ))
                    {
                        LogNet?.WriteException( ToString( ), string.Format( StringResources.Language.ClientOfflineInfo, state.IpEndPoint ), ex );
                        System.Threading.Interlocked.Decrement( ref onlineCount );
                    }
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
                    catch (Exception ex)
                    {
                        state.WorkSocket?.Close( );
                        if (state.IsModbusOffline( ))
                        {
                            LogNet?.WriteException( ToString( ), string.Format( StringResources.Language.ClientOfflineInfo, state.IpEndPoint ), ex );
                            System.Threading.Interlocked.Decrement( ref onlineCount );
                        }
                        return;
                    }
                }
                else
                {
                    // 关闭连接，记录日志
                    state.WorkSocket?.Close( );
                    if (state.IsModbusOffline( ))
                    {
                        LogNet?.WriteWarn( ToString( ), string.Format( StringResources.Language.ClientOfflineInfo, state.IpEndPoint ) + StringResources.Language.ModbusMatchFailed );
                        System.Threading.Interlocked.Decrement( ref onlineCount );
                    }
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
                    if (state.IsModbusOffline( ))
                    {
                        LogNet?.WriteException( ToString( ), string.Format( StringResources.Language.ClientOfflineInfo, state.IpEndPoint ), ex );
                        System.Threading.Interlocked.Decrement( ref onlineCount );
                    }
                    return;
                }


                // 数据接收完成
                // 内容接收完成，所有的数据接收结束
                byte[] data = new byte[state.HeadByte.Length + state.Content.Length];
                state.HeadByte.CopyTo( data, 0 );
                state.Content.CopyTo( data, state.HeadByte.Length );

                state.Clear( );


                byte[] modbusCore = ModbusTcpTransModbusCore( data );

                if (!CheckModbusMessageLegal( modbusCore ))
                {
                    // 指令长度验证错误，关闭网络连接
                    state.WorkSocket?.Close( );
                    if (state.IsModbusOffline( ))
                    {
                        LogNet?.WriteError( ToString( ), string.Format( StringResources.Language.ClientOfflineInfo, state.IpEndPoint ) );
                        System.Threading.Interlocked.Decrement( ref onlineCount );
                    }
                    return;
                }


                // 需要回发消息
                byte[] copy = ModbusCoreTransModbusTcp( ReadFromModbusCore( modbusCore ), data );


                try
                {
                    // 管他是什么，先开始数据接收
                    // state.WorkSocket?.Close();
                    state.WorkSocket.BeginReceive( state.HeadByte, 0, 6, SocketFlags.None, new AsyncCallback( ModbusHeadReveiveCallback ), state );
                }
                catch (Exception ex)
                {
                    state.WorkSocket?.Close( );
                    if (state.IsModbusOffline( ))
                    {
                        LogNet?.WriteException( ToString( ), string.Format( StringResources.Language.ClientOfflineInfo, state.IpEndPoint ), ex );
                        System.Threading.Interlocked.Decrement( ref onlineCount );
                    }
                    return;
                }


                // 回发数据，先获取发送锁
                state.hybirdLock.Enter( );
                try
                {
                    state.WorkSocket.BeginSend( copy, 0, size: copy.Length, socketFlags: SocketFlags.None, callback: new AsyncCallback( DataSendCallBack ), state: state );
                }
                catch (Exception ex)
                {
                    state.WorkSocket?.Close( );
                    state.hybirdLock.Leave( );
                    if (state.IsModbusOffline( ))
                    {
                        LogNet?.WriteException( ToString( ), string.Format( StringResources.Language.ClientOfflineInfo, state.IpEndPoint ), ex );
                        System.Threading.Interlocked.Decrement( ref onlineCount );
                    }
                    return;
                }

                // 通知处理消息
                if (IsStarted) OnDataReceived?.Invoke( this, data );
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
                    if (state.IsModbusOffline( ))
                    {
                        LogNet?.WriteException( ToString( ), string.Format( StringResources.Language.ClientOfflineInfo, state.IpEndPoint ), ex );
                        state = null;
                        System.Threading.Interlocked.Decrement( ref onlineCount );
                    }
                }
            }
        }

        #endregion

        #region Function Process Center

        /// <summary>
        /// 创建特殊的功能标识，然后返回该信息
        /// </summary>
        /// <param name="modbusCore">modbus核心报文</param>
        /// <param name="error">错误码</param>
        /// <returns>携带错误码的modbus报文</returns>
        private byte[] CreateExceptionBack( byte[] modbusCore, byte error )
        {
            byte[] buffer = new byte[3];
            buffer[0] = modbusCore[0];
            buffer[1] = (byte)(modbusCore[1] + 0x80);
            buffer[2] = error;
            return buffer;
        }

        /// <summary>
        /// 创建返回消息
        /// </summary>
        /// <param name="modbusCore">modbus核心报文</param>
        /// <param name="content">返回的实际数据内容</param>
        /// <returns>携带内容的modbus报文</returns>
        private byte[] CreateReadBack( byte[] modbusCore, byte[] content )
        {
            byte[] buffer = new byte[3 + content.Length];
            buffer[0] = modbusCore[0];
            buffer[1] = modbusCore[1];
            buffer[2] = (byte)content.Length;
            Array.Copy( content, 0, buffer, 3, content.Length );
            return buffer;
        }

        /// <summary>
        /// 创建写入成功的反馈信号
        /// </summary>
        /// <param name="modbus">modbus核心报文</param>
        /// <returns>携带成功写入的信息</returns>
        private byte[] CreateWriteBack( byte[] modbus )
        {
            byte[] buffer = new byte[6];
            Array.Copy( modbus, 0, buffer, 0, 6 );
            return buffer;
        }


        private byte[] ReadCoilBack( byte[] modbus )
        {
            try
            {
                ushort address = byteTransform.TransUInt16( modbus, 2 );
                ushort length = byteTransform.TransUInt16( modbus, 4 );

                // 越界检测
                if ((address + length) > ushort.MaxValue + 1)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeOverBound );
                }

                // 地址长度检测
                if (length > 2040)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeQuantityOver );
                }

                bool[] read = ReadCoil( address.ToString( ), length );
                byte[] buffer = BasicFramework.SoftBasic.BoolArrayToByte( read );
                return CreateReadBack( modbus, buffer );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), StringResources.Language.ModbusTcpReadCoilException, ex );
                return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeReadWriteException );
            }
        }

        private byte[] ReadDiscreteBack( byte[] modbus )
        {
            try
            {
                ushort address = byteTransform.TransUInt16( modbus, 2 );
                ushort length = byteTransform.TransUInt16( modbus, 4 );

                // 越界检测
                if ((address + length) > ushort.MaxValue + 1)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeOverBound );
                }

                // 地址长度检测
                if (length > 2040)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeQuantityOver );
                }

                bool[] read = ReadDiscrete( address.ToString( ), length );
                byte[] buffer = BasicFramework.SoftBasic.BoolArrayToByte( read );
                return CreateReadBack( modbus, buffer );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), StringResources.Language.ModbusTcpReadCoilException, ex );
                return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeReadWriteException );
            }
        }


        private byte[] ReadRegisterBack( byte[] modbus )
        {
            try
            {
                ushort address = byteTransform.TransUInt16( modbus, 2 );
                ushort length = byteTransform.TransUInt16( modbus, 4 );

                // 越界检测
                if ((address + length) > ushort.MaxValue + 1)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeOverBound );
                }

                // 地址长度检测
                if (length > 127)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeQuantityOver );
                }

                byte[] buffer = ReadRegister( address.ToString( ), length );
                return CreateReadBack( modbus, buffer );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), StringResources.Language.ModbusTcpReadRegisterException, ex );
                return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeReadWriteException );
            }
        }

        private byte[] ReadInputRegisterBack( byte[] modbus )
        {
            try
            {
                ushort address = byteTransform.TransUInt16( modbus, 2 );
                ushort length = byteTransform.TransUInt16( modbus, 4 );

                // 越界检测
                if ((address + length) > ushort.MaxValue + 1)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeOverBound );
                }

                // 地址长度检测
                if (length > 127)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeQuantityOver );
                }

                byte[] buffer = ReadRegister( "x=4;" + address.ToString( ), length );
                return CreateReadBack( modbus, buffer );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), StringResources.Language.ModbusTcpReadRegisterException, ex );
                return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeReadWriteException );
            }
        }

        private byte[] WriteOneCoilBack( byte[] modbus )
        {
            try
            {
                ushort address = byteTransform.TransUInt16( modbus, 2 );

                if (modbus[4] == 0xFF && modbus[5] == 0x00)
                {
                    WriteCoil( address.ToString( ), true );
                }
                else if (modbus[4] == 0x00 && modbus[5] == 0x00)
                {
                    WriteCoil( address.ToString( ), false );
                }
                return CreateWriteBack( modbus );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), StringResources.Language.ModbusTcpWriteCoilException, ex );
                return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeReadWriteException );
            }
        }



        private byte[] WriteOneRegisterBack( byte[] modbus )
        {
            try
            {
                ushort address = byteTransform.TransUInt16( modbus, 2 );
                short ValueOld = ReadInt16( address.ToString( ) );
                // 写入到寄存器
                Write( address.ToString( ), modbus[4], modbus[5] );
                short ValueNew = ReadInt16( address.ToString( ) );
                // 触发写入请求
                OnRegisterBeforWrite( address, ValueOld, ValueNew );

                return CreateWriteBack( modbus );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), StringResources.Language.ModbusTcpWriteRegisterException, ex );
                return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeReadWriteException );
            }
        }

        private byte[] WriteCoilsBack( byte[] modbus )
        {
            try
            {
                ushort address = byteTransform.TransUInt16( modbus, 2 );
                ushort length = byteTransform.TransUInt16( modbus, 4 );

                if ((address + length) > ushort.MaxValue + 1)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeOverBound );
                }

                if (length > 2040)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeQuantityOver );
                }

                byte[] buffer = new byte[modbus.Length - 7];
                Array.Copy( modbus, 7, buffer, 0, buffer.Length );
                bool[] value = BasicFramework.SoftBasic.ByteToBoolArray( buffer, length );
                WriteCoil( address.ToString( ), value );
                return CreateWriteBack( modbus );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), StringResources.Language.ModbusTcpWriteCoilException, ex );
                return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeReadWriteException );
            }
        }


        private byte[] WriteRegisterBack( byte[] modbus )
        {
            try
            {
                ushort address = byteTransform.TransUInt16( modbus, 2 );
                ushort length = byteTransform.TransUInt16( modbus, 4 );

                if ((address + length) > ushort.MaxValue + 1)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeOverBound );
                }

                if (length > 127)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeQuantityOver );
                }

                byte[] buffer = new byte[modbus.Length - 7];

                // 为了使服务器的数据订阅更加的准确，决定将设计改为等待所有的数据写入完成后，再统一触发订阅，2018年3月4日 20:56:47
                MonitorAddress[] addresses = new MonitorAddress[length];
                for (ushort i = 0; i < length; i++)
                {
                    short ValueOld = ReadInt16( (address + i).ToString( ) );
                    Write( (address + i).ToString( ), modbus[2 * i + 7], modbus[2 * i + 8] );
                    short ValueNew = ReadInt16( (address + i).ToString( ) );
                    // 触发写入请求
                    addresses[i] = new MonitorAddress( )
                    {
                        Address = (ushort)(address + i),
                        ValueOrigin = ValueOld,
                        ValueNew = ValueNew
                    };
                }

                // 所有数据都更改完成后，再触发消息
                for (int i = 0; i < addresses.Length; i++)
                {
                    OnRegisterBeforWrite( addresses[i].Address, addresses[i].ValueOrigin, addresses[i].ValueNew );
                }

                return CreateWriteBack( modbus );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), StringResources.Language.ModbusTcpWriteRegisterException, ex );
                return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeReadWriteException );
            }
        }


        #endregion

        #region Subscription Support

        // 本服务器端支持指定地址的数据订阅器，目前仅支持寄存器操作

        private List<ModBusMonitorAddress> subscriptions;     // 数据订阅集合
        private SimpleHybirdLock subcriptionHybirdLock;       // 集合锁

        /// <summary>
        /// 新增一个数据监视的任务，针对的是寄存器
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

        #region Trust Client Only

        private List<string> TrustedClients = null;              // 受信任的客户端
        private bool IsTrustedClientsOnly = false;               // 是否启用仅仅受信任的客户端登录
        private SimpleHybirdLock lock_trusted_clients;           // 受信任的客户端的列表

        /// <summary>
        /// 设置并启动受信任的客户端登录并读写，如果为null，将关闭对客户端的ip验证
        /// </summary>
        /// <param name="clients">受信任的客户端列表</param>
        public void SetTrustedIpAddress( List<string> clients )
        {
            lock_trusted_clients.Enter( );
            if (clients != null)
            {
                TrustedClients = clients.Select( m =>
                 {
                     System.Net.IPAddress iPAddress = System.Net.IPAddress.Parse( m );
                     return iPAddress.ToString( );
                 } ).ToList( );
                IsTrustedClientsOnly = true;
            }
            else
            {
                TrustedClients = new List<string>( );
                IsTrustedClientsOnly = false;
            }
            lock_trusted_clients.Leave( );
        }

        /// <summary>
        /// 检查该Ip地址是否是受信任的
        /// </summary>
        /// <param name="ipAddress">Ip地址信息</param>
        /// <returns>是受信任的返回<c>True</c>，否则返回<c>False</c></returns>
        private bool CheckIpAddressTrusted( string ipAddress )
        {
            if (IsTrustedClientsOnly)
            {
                bool result = false;
                lock_trusted_clients.Enter( );
                for (int i = 0; i < TrustedClients.Count; i++)
                {
                    if (TrustedClients[i] == ipAddress)
                    {
                        result = true;
                        break;
                    }
                }
                lock_trusted_clients.Leave( );
                return result;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取受信任的客户端列表
        /// </summary>
        /// <returns></returns>
        public string[] GetTrustedClients( )
        {
            string[] result = new string[0];
            lock_trusted_clients.Enter( );
            if (TrustedClients != null)
            {
                result = TrustedClients.ToArray( );
            }
            lock_trusted_clients.Leave( );
            return result;
        }


        #endregion

        #region Modbus Core Logic


        /// <summary>
        /// 检测当前的Modbus接收的指定是否是合法的
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <returns>是否合格</returns>
        private bool CheckModbusMessageLegal( byte[] buffer )
        {
            try
            {
                if (buffer[1] == ModbusInfo.ReadCoil ||
                    buffer[1] == ModbusInfo.ReadDiscrete ||
                    buffer[1] == ModbusInfo.ReadRegister ||
                    buffer[1] == ModbusInfo.ReadInputRegister ||
                    buffer[1] == ModbusInfo.WriteOneCoil ||
                    buffer[1] == ModbusInfo.WriteOneRegister)
                {
                    if (buffer.Length != 0x06)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else if (
                    buffer[1] == ModbusInfo.WriteCoil ||
                    buffer[1] == ModbusInfo.WriteRegister)
                {
                    if (buffer.Length < 7)
                    {
                        return false;
                    }
                    else
                    {
                        if (buffer[6] == (buffer.Length - 7))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), ex );
                return false;
            }
        }


        /// <summary>
        /// Modbus核心数据交互方法
        /// </summary>
        /// <param name="modbusCore">核心的Modbus报文</param>
        /// <returns>进行数据交互之后的结果</returns>
        private byte[] ReadFromModbusCore( byte[] modbusCore )
        {
            byte[] buffer = null;

            switch (modbusCore[1])
            {
                case ModbusInfo.ReadCoil:
                    {
                        buffer = ReadCoilBack( modbusCore ); break;
                    }
                case ModbusInfo.ReadDiscrete:
                    {
                        buffer = ReadDiscreteBack( modbusCore ); break;
                    }
                case ModbusInfo.ReadRegister:
                    {
                        buffer = ReadRegisterBack( modbusCore ); break;
                    }
                case ModbusInfo.ReadInputRegister:
                    {
                        buffer = ReadInputRegisterBack( modbusCore ); break;
                    }
                case ModbusInfo.WriteOneCoil:
                    {
                        buffer = WriteOneCoilBack( modbusCore ); break;
                    }
                case ModbusInfo.WriteOneRegister:
                    {
                        buffer = WriteOneRegisterBack( modbusCore ); break;
                    }
                case ModbusInfo.WriteCoil:
                    {
                        buffer = WriteCoilsBack( modbusCore ); break;
                    }
                case ModbusInfo.WriteRegister:
                    {
                        buffer = WriteRegisterBack( modbusCore ); break;
                    }
                default:
                    {
                        buffer = CreateExceptionBack( modbusCore, ModbusInfo.FunctionCodeNotSupport ); break;
                    }
            }

            return buffer;
        }


        /// <summary>
        /// 将Modbus-Tcp的报文转换成核心的Modbus报文，就是移除报文头
        /// </summary>
        /// <param name="modbusTcp"></param>
        /// <returns></returns>
        private byte[] ModbusTcpTransModbusCore( byte[] modbusTcp )
        {
            byte[] buffer = new byte[modbusTcp.Length - 6];

            Array.Copy( modbusTcp, 6, buffer, 0, buffer.Length );

            return buffer;
        }


        /// <summary>
        /// 根据Modbus数据信息生成Modbus-Tcp数据信息
        /// </summary>
        /// <param name="modbusData"></param>
        /// <param name="modbusOrigin"></param>
        /// <returns></returns>
        private byte[] ModbusCoreTransModbusTcp( byte[] modbusData, byte[] modbusOrigin )
        {
            byte[] buffer = new byte[modbusData.Length + 6];
            buffer[0] = modbusOrigin[0];
            buffer[1] = modbusOrigin[1];

            modbusData.CopyTo( buffer, 6 );
            buffer[5] = (byte)modbusData.Length;

            return buffer;
        }

        /// <summary>
        /// 将Modbus-Rtu的报文转成核心的Modbus报文，移除了CRC校验
        /// </summary>
        /// <param name="modbusRtu"></param>
        /// <returns></returns>
        private byte[] ModbusRtuTransModbusCore( byte[] modbusRtu )
        {
            byte[] buffer = new byte[modbusRtu.Length - 2];

            Array.Copy( modbusRtu, 0, buffer, 0, buffer.Length );

            return buffer;
        }

#if !NETSTANDARD2_0

        /// <summary>
        /// 根据Modbus数据信息生成Modbus-Rtu数据信息
        /// </summary>
        /// <param name="modbusData"></param>
        /// <returns></returns>
        private byte[] ModbusCoreTransModbusRtu( byte[] modbusData )
        {
            return Serial.SoftCRC16.CRC16( modbusData );
        }

#endif

        #endregion

        #region Serial Support

#if !NETSTANDARD2_0

        private SerialPort serialPort;            // 核心的串口对象

        /// <summary>
        /// 使用默认的参数进行初始化串口，9600波特率，8位数据位，无奇偶校验，1位停止位
        /// </summary>
        /// <param name="com">串口信息</param>
        public void StartSerialPort( string com )
        {
            StartSerialPort( com, 9600 );
        }


        /// <summary>
        /// 使用默认的参数进行初始化串口，8位数据位，无奇偶校验，1位停止位
        /// </summary>
        /// <param name="com">串口信息</param>
        /// <param name="baudRate">波特率</param>
        public void StartSerialPort( string com, int baudRate )
        {
            StartSerialPort( sp =>
            {
                sp.PortName = com;
                sp.BaudRate = baudRate;
                sp.DataBits = 8;
                sp.Parity = Parity.None;
                sp.StopBits = StopBits.One;
            } );
        }

        /// <summary>
        /// 使用自定义的初始化方法初始化串口的参数
        /// </summary>
        /// <param name="inni">初始化信息的委托</param>
        public void StartSerialPort( Action<SerialPort> inni )
        {
            if (!serialPort.IsOpen)
            {
                inni?.Invoke( serialPort );

                serialPort.ReadBufferSize = 1024;
                serialPort.ReceivedBytesThreshold = 1;
                serialPort.Open( );
                serialPort.DataReceived += SerialPort_DataReceived;
            }
        }



        /// <summary>
        /// 关闭串口
        /// </summary>
        public void CloseSerialPort( )
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close( );
            }
        }


        /// <summary>
        /// 接收到串口数据的时候触发
        /// </summary>
        /// <param name="sender">串口对象</param>
        /// <param name="e">消息</param>
        private void SerialPort_DataReceived( object sender, SerialDataReceivedEventArgs e )
        {
            int rCount = 0;
            byte[] buffer = new byte[1024];
            byte[] receive = null;

            while(true)
            {
                System.Threading.Thread.Sleep( 20 );            // 此处做个微小的延时，等待数据接收完成
                int count = serialPort.Read( buffer, rCount, serialPort.BytesToRead );
                rCount += count;
                if(count == 0) break;

                receive = new byte[rCount];
                Array.Copy( buffer, 0, receive, 0, count );
            }

            if(receive == null) return;
            
            if (receive.Length < 3)
            {
                LogNet?.WriteError( ToString( ), $"Uknown Data：" + BasicFramework.SoftBasic.ByteToHexString( receive, ' ' ) );
                return;
            }

            if (Serial.SoftCRC16.CheckCRC16( receive ))
            {
                byte[] modbusCore = ModbusRtuTransModbusCore( receive );


                if (!CheckModbusMessageLegal( modbusCore ))
                {
                    // 指令长度验证错误，关闭网络连接
                    LogNet?.WriteError( ToString( ), $"Receive Nosense Modbus-rtu : " + BasicFramework.SoftBasic.ByteToHexString( receive, ' ' ) );
                    return;
                }

                // 验证站号是否一致
                if(station >= 0 && station != modbusCore[0])
                {
                    LogNet?.WriteError( ToString( ), $"Station not match Modbus-rtu : " + BasicFramework.SoftBasic.ByteToHexString( receive, ' ' ) );
                    return;
                }

                // LogNet?.WriteError( ToString( ), $"Success：" + BasicFramework.SoftBasic.ByteToHexString( receive, ' ' ) );
                // 需要回发消息
                byte[] copy = ModbusCoreTransModbusRtu( ReadFromModbusCore( modbusCore ) );

                serialPort.Write( copy, 0, copy.Length );

                if (IsStarted) OnDataReceived?.Invoke( this, receive );
            }
            else
            {
                LogNet?.WriteWarn( "CRC Check Failed : " + BasicFramework.SoftBasic.ByteToHexString( receive, ' ' ) );
            }
        }

#endif



        #endregion

        #region Private Member

        private int onlineCount = 0;               // 在线的客户端的数量

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString( )
        {
            return $"ModbusTcpServer[{Port}]";
        }

        #endregion
    }
}
