using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using HslCommunication.Core;



/****************************************************************************
 * 
 *    日期：2017年10月31日 14:57:37
 *    修改：西门子通信篇二的报文信息更新，读取数组更加快速
 *          提供了一个方法，可以手动修改PLC类型参数
 * 
 * 
 ****************************************************************************/








namespace HslCommunication.Profinet
{
    /// <summary>
    /// 数据访问类，用于计算机和西门子PLC的以太网模块通讯的类
    /// 通讯协议为基于以太网的FETCH/WRITE协议
    /// 想要访问成功，必须先配置PLC的网络
    /// </summary>
    public sealed class SiemensNet : PlcNetBase
    {
        /// <summary>
        /// 从西门子PLC中读取想要的数据，返回结果类说明
        /// </summary>
        /// <param name="type">想要读取的数据类型</param>
        /// <param name="address">读取数据的起始地址</param>
        /// <param name="lengh">读取的数据长度</param>
        /// <returns>返回读取结果</returns>
        public OperateResult<byte[]> ReadFromPLC(SiemensDataType type, ushort address, ushort lengh)
        {
            OperateResult<byte[]> result = new OperateResult<byte[]>();

            byte[] _PLCCommand = new byte[16];
            _PLCCommand[0] = 0x53;
            _PLCCommand[1] = 0x35;
            _PLCCommand[2] = 0x10;
            _PLCCommand[3] = 0x01;
            _PLCCommand[4] = 0x03;
            _PLCCommand[5] = 0x05;
            _PLCCommand[6] = 0x03;
            _PLCCommand[7] = 0x08;

            //指定数据区
            _PLCCommand[8] = type.DataCode;
            _PLCCommand[9] = 0x00;

            //指定数据地址
            _PLCCommand[10] = (byte)(address / 256);
            _PLCCommand[11] = (byte)(address % 256);

            //指定数据长度
            _PLCCommand[12] = (byte)(lengh / 256);
            _PLCCommand[13] = (byte)(lengh % 256);

            _PLCCommand[14] = 0xff;
            _PLCCommand[15] = 0x02;


            //超时验证的线程池技术
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            HslTimeOut timeout = new HslTimeOut()
            {
                WorkSocket = socket
            };
            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolCheckConnect), timeout);
                socket.Connect(new IPEndPoint(PLCIpAddress, GetPort()));
                timeout.IsSuccessful = true;
            }
            catch
            {
                ChangePort();
                result.Message = StringResources.ConnectedFailed;
                Thread.Sleep(10);
                socket?.Close();
                return result;
            }
            try
            {
                //连接成功，发送数据
                socket.Send(_PLCCommand);
                byte[] rec_head = NetSupport.ReadBytesFromSocket(socket, 16);
                if (rec_head[8] != 0x00)
                {
                    result.ErrorCode = rec_head[8];
                    Thread.Sleep(10);
                    socket?.Close();
                    return result;
                }
                result.Content = NetSupport.ReadBytesFromSocket(socket, lengh);
                result.IsSuccess = true;
                result.Message = "读取成功";
            }
            catch (Exception ex)
            {
                result.Message = StringResources.SocketIOException + ex.Message;
                Thread.Sleep(10);
                socket?.Close();
                return result;
            }
            Thread.Sleep(10);
            socket?.Close();
            //所有的数据接收完成，进行返回
            return result;
        }

        /// <summary>
        /// 向PLC中写入字符串，编码格式为ASCII
        /// </summary>
        /// <param name="type">要写入的数据类型</param>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回读取结果</returns>
        public OperateResult<byte[]> WriteAsciiStringIntoPLC(SiemensDataType type, ushort address, string data)
        {
            byte[] temp = Encoding.ASCII.GetBytes(data);
            return WriteIntoPLC(type, address, temp);
        }

        /// <summary>
        /// 向PLC中写入指定长度的字符串,超出截断，不够补0，编码格式为ASCII
        /// </summary>
        /// <param name="type">要写入的数据类型</param>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>返回读取结果</returns>
        public OperateResult<byte[]> WriteAsciiStringIntoPLC(SiemensDataType type, ushort address, string data, int length)
        {
            byte[] temp = Encoding.ASCII.GetBytes(data);
            temp = ManageBytesLength(temp, length);
            return WriteIntoPLC(type, address, temp);
        }

        /// <summary>
        /// 向PLC中写入字符串，编码格式为Unicode
        /// </summary>
        /// <param name="type">要写入的数据类型</param>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回读取结果</returns>
        public OperateResult<byte[]> WriteUnicodeStringIntoPLC(SiemensDataType type, ushort address, string data)
        {
            byte[] temp = Encoding.Unicode.GetBytes(data);
            return WriteIntoPLC(type, address, temp);
        }

        /// <summary>
        /// 向PLC中写入指定长度的字符串,超出截断，不够补0，编码格式为Unicode
        /// </summary>
        /// <param name="type">要写入的数据类型</param>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>返回读取结果</returns>
        public OperateResult<byte[]> WriteUnicodeStringIntoPLC(SiemensDataType type, ushort address, string data, int length)
        {
            byte[] temp = Encoding.Unicode.GetBytes(data);
            temp = ManageBytesLength(temp, length * 2);
            return WriteIntoPLC(type, address, temp);
        }

        /// <summary>
        /// 向PLC中写入short数据，返回值说明
        /// </summary>
        /// <param name="type">要写入的数据类型</param>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <exception cref="ArgumentNullException">写入的数据不能为空</exception>
        /// <returns>返回写入结果</returns>
        public OperateResult<byte[]> WriteIntoPLC(SiemensDataType type, ushort address, short[] data)
        {
            return WriteIntoPLC(type, address, GetBytesFromArray(data, false));
        }

        /// <summary>
        /// 向PLC中写入数据，返回值说明
        /// </summary>
        /// <param name="type">要写入的数据类型</param>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <exception cref="ArgumentNullException">写入的数据不能为空</exception>
        /// <returns>返回写入结果</returns>
        public OperateResult<byte[]> WriteIntoPLC(SiemensDataType type, ushort address, byte[] data)
        {
            var result = new OperateResult<byte[]>();
            byte[] _PLCCommand = new byte[16 + data.Length];
            _PLCCommand[0] = 0x53;
            _PLCCommand[1] = 0x35;
            _PLCCommand[2] = 0x10;
            _PLCCommand[3] = 0x01;
            _PLCCommand[4] = 0x03;
            _PLCCommand[5] = 0x03;
            _PLCCommand[6] = 0x03;
            _PLCCommand[7] = 0x08;

            //指定数据区
            _PLCCommand[8] = type.DataCode;
            _PLCCommand[9] = 0x00;

            //指定数据地址
            _PLCCommand[10] = (byte)(address / 256);
            _PLCCommand[11] = (byte)(address % 256);

            //指定数据长度
            _PLCCommand[12] = (byte)(data.Length / 256);
            _PLCCommand[13] = (byte)(data.Length % 256);

            _PLCCommand[14] = 0xff;
            _PLCCommand[15] = 0x02;

            //放置数据
            Array.Copy(data, 0, _PLCCommand, 16, data.Length);


            //超时验证的线程池技术
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var timeout = new HslTimeOut()
            {
                WorkSocket = socket
            };
            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolCheckConnect), timeout);
                socket.Connect(new IPEndPoint(PLCIpAddress, PortWrite));
                timeout.IsSuccessful = true;
            }
            catch
            {
                result.Message = StringResources.ConnectedFailed;
                Thread.Sleep(10);
                socket?.Close();
                return result;
            }
            try
            {
                //连接成功，发送数据
                socket.Send(_PLCCommand);
                byte[] rec_head = NetSupport.ReadBytesFromSocket(socket, 16);
                //分析数据
                if (rec_head[8] != 0x00)
                {
                    result.ErrorCode = rec_head[8];
                    Thread.Sleep(10);
                    socket?.Close();
                    return result;
                }
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                result.IsSuccess = true;
                result.Message = "写入成功";
            }
            catch (Exception ex)
            {
                result.Message = StringResources.SocketIOException + ex.Message;
                Thread.Sleep(10);
                socket?.Close();
                return result;
            }
            return result;
        }
    }

    /// <summary>
    /// 西门子的数据类型
    /// </summary>
    public sealed class SiemensDataType
    {
        /// <summary>
        /// 如果您清楚类型代号，可以根据值进行扩展
        /// </summary>
        /// <param name="code"></param>
        public SiemensDataType(byte code)
        {
            DataCode = code;
        }
        /// <summary>
        /// 类型的代号值
        /// </summary>
        public byte DataCode { get; set; } = 0x00;
        /// <summary>
        /// M寄存器
        /// </summary>
        public readonly static SiemensDataType M = new SiemensDataType(0x02);
        /// <summary>
        /// I寄存器
        /// </summary>
        public readonly static SiemensDataType I = new SiemensDataType(0x03);
        /// <summary>
        /// Q寄存器
        /// </summary>
        public readonly static SiemensDataType Q = new SiemensDataType(0x04);

    }





    /// <summary>
    /// 西门子的PLC类型，目前支持的访问类型
    /// </summary>
    public enum SiemensPLCS
    {
        /// <summary>
        /// 1200系列
        /// </summary>
        S1200 = 1,
        /// <summary>
        /// 300系列
        /// </summary>
        S300 = 2,
    }

    /// <summary>
    /// 一个直接使用Tcp协议连接西门子PLC的类，只需要在plc侧配置IP即可，该类在西门子1200上经过测试，1500暂时不支持
    /// </summary>
    public sealed class SiemensTcpNet : PlcNetBase
    {
        #region Constructor


        /// <summary>
        /// 实例化一个数据通信的对象，需要指定访问哪种Plc
        /// </summary>
        /// <param name="siemens"></param>
        public SiemensTcpNet(SiemensPLCS siemens)
        {
            PortRead = 102;
            PortWrite = 102;
            CurrentPlc = siemens;
            LogHeaderText = "SiemensTcpNet";

            switch (siemens)
            {
                case SiemensPLCS.S1200: plcHead1[18] = 1; break;
                case SiemensPLCS.S300: plcHead1[18] = 2; break;
                default: plcHead1[18] = 3; break;
            }
        }

        #endregion

        #region DoubleModeNetBase Override

        /// <summary>
        /// 连接操作
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected override bool InitilizationOnConnect(Socket socket, OperateResult result)
        {
            return InitilizationConnect(socket, result);
        }

        /// <summary>
        /// 接收服务器的时候的数据反馈
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="response"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected override bool ReceiveResponse(Socket socket, out byte[] response, OperateResult result)
        {
            try
            {
                // 先接收4个字节的数据
                byte[] head = NetSupport.ReadBytesFromSocket(socket, 4);
                int receive = head[2] * 256 + head[3];
                response = new byte[receive];
                head.CopyTo(response, 0);
                NetSupport.ReadBytesFromSocket(socket, receive - 4).CopyTo(response, 4);
                return true;
            }
            catch(Exception ex)
            {
                socket?.Close();
                response = null;
                result.Message = ex.Message;
                return false;
            }
        }

        
        #endregion

        #region Build Command

        /// <summary>
        /// 生成一个读取字数据指令头的通用方法
        /// </summary>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <param name="command"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool BuildReadCommand(string[] address, ushort[] count, out byte[] command, OperateResult result)
        {
            if (address == null) throw new NullReferenceException("address");
            if (count == null) throw new NullReferenceException("count");
            if (address.Length != count.Length) throw new Exception("两个参数的个数不统一");
            if (count.Length > 255) throw new Exception("读取的数组数量不允许大于255");

            int readCount = count.Length;

            byte[] _PLCCommand = new byte[19 + readCount * 12];
            // 报文头
            _PLCCommand[0] = 0x03;
            _PLCCommand[1] = 0x00;
            // 长度
            _PLCCommand[2] = (byte)(_PLCCommand.Length / 256);
            _PLCCommand[3] = (byte)(_PLCCommand.Length % 256);
            // 固定
            _PLCCommand[4] = 0x02;
            _PLCCommand[5] = 0xF0;
            _PLCCommand[6] = 0x80;
            _PLCCommand[7] = 0x32;
            // 命令：发
            _PLCCommand[8] = 0x01;
            // 标识序列号
            _PLCCommand[9] = 0x00;
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x00;
            _PLCCommand[12] = 0x01;
            // 命令数据总长度
            _PLCCommand[13] = (byte)((_PLCCommand.Length - 17) / 256);
            _PLCCommand[14] = (byte)((_PLCCommand.Length - 17) % 256);

            _PLCCommand[15] = 0x00;
            _PLCCommand[16] = 0x00;

            // 命令起始符
            _PLCCommand[17] = 0x04;
            // 读取数据块个数
            _PLCCommand[18] = (byte)readCount;


            for (int ii = 0; ii < readCount; ii++)
            {
                // 填充数据
                if (!AnalysisAddress(address[ii], out byte type, out int startAddress, out ushort dbAddress, result))
                {
                    command = null;
                    return false;
                }
                
                //===========================================================================================
                // 读取地址的前缀
                _PLCCommand[19 + ii * 12] = 0x12;
                _PLCCommand[20 + ii * 12] = 0x0A;
                _PLCCommand[21 + ii * 12] = 0x10;
                _PLCCommand[22 + ii * 12] = 0x02;
                // 访问数据的个数
                _PLCCommand[23 + ii * 12] = (byte)(count[ii] / 256);
                _PLCCommand[24 + ii * 12] = (byte)(count[ii] % 256);
                // DB块编号，如果访问的是DB块的话
                _PLCCommand[25 + ii * 12] = (byte)(dbAddress / 256);
                _PLCCommand[26 + ii * 12] = (byte)(dbAddress % 256);
                // 访问数据类型
                _PLCCommand[27 + ii * 12] = type;
                // 偏移位置
                _PLCCommand[28 + ii * 12] = (byte)(startAddress / 256 / 256 % 256);
                _PLCCommand[29 + ii * 12] = (byte)(startAddress / 256 % 256);
                _PLCCommand[30 + ii * 12] = (byte)(startAddress % 256);
            }
            command = _PLCCommand;

            return true;
        }

        /// <summary>
        /// 生成一个位读取数据指令头的通用方法
        /// </summary>
        /// <param name="address"></param>
        /// <param name="command"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool BuildBitReadCommand(string address, out byte[] command, OperateResult result)
        {
            byte[] _PLCCommand = new byte[31];
            // 报文头
            _PLCCommand[0] = 0x03;
            _PLCCommand[1] = 0x00;
            // 长度
            _PLCCommand[2] = (byte)(_PLCCommand.Length / 256);
            _PLCCommand[3] = (byte)(_PLCCommand.Length % 256);
            // 固定
            _PLCCommand[4] = 0x02;
            _PLCCommand[5] = 0xF0;
            _PLCCommand[6] = 0x80;
            _PLCCommand[7] = 0x32;
            // 命令：发
            _PLCCommand[8] = 0x01;
            // 标识序列号
            _PLCCommand[9] = 0x00;
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x00;
            _PLCCommand[12] = 0x01;
            // 命令数据总长度
            _PLCCommand[13] = (byte)((_PLCCommand.Length - 17) / 256);
            _PLCCommand[14] = (byte)((_PLCCommand.Length - 17) % 256);

            _PLCCommand[15] = 0x00;
            _PLCCommand[16] = 0x00;

            // 命令起始符
            _PLCCommand[17] = 0x04;
            // 读取数据块个数
            _PLCCommand[18] = 0x01;


            // 填充数据
            if (!AnalysisAddress(address, out byte type, out int startAddress, out ushort dbAddress, result))
            {
                command = null;
                return false;
            }

            //===========================================================================================
            // 读取地址的前缀
            _PLCCommand[19] = 0x12;
            _PLCCommand[20] = 0x0A;
            _PLCCommand[21] = 0x10;
            // 读取的数据时位
            _PLCCommand[22] = 0x01;
            // 访问数据的个数
            _PLCCommand[23] = 0x00;
            _PLCCommand[24] = 0x01;
            // DB块编号，如果访问的是DB块的话
            _PLCCommand[25] = (byte)(dbAddress / 256);
            _PLCCommand[26] = (byte)(dbAddress % 256);
            // 访问数据类型
            _PLCCommand[27] = type;
            // 偏移位置
            _PLCCommand[28] = (byte)(startAddress / 256 / 256 % 256);
            _PLCCommand[29] = (byte)(startAddress / 256 % 256);
            _PLCCommand[30] = (byte)(startAddress % 256);

            command = _PLCCommand;

            return true;
        }



        private bool BuildWriteByteCommand(string address, byte[] data,out byte[] command, OperateResult result)
        {
            if (data == null) data = new byte[0];

            if (!AnalysisAddress(address, out byte type, out int startAddress, out ushort dbAddress, result))
            {
                command = null;
                return false;
            }

            byte[] _PLCCommand = new byte[35 + data.Length];
            _PLCCommand[0] = 0x03;
            _PLCCommand[1] = 0x00;
            // 长度
            _PLCCommand[2] = (byte)((35 + data.Length) / 256);
            _PLCCommand[3] = (byte)((35 + data.Length) % 256);
            // 固定
            _PLCCommand[4] = 0x02;
            _PLCCommand[5] = 0xF0;
            _PLCCommand[6] = 0x80;
            _PLCCommand[7] = 0x32;
            // 命令 发
            _PLCCommand[8] = 0x01;
            // 标识序列号
            _PLCCommand[9] = 0x00;
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x00;
            _PLCCommand[12] = 0x01;
            // 固定
            _PLCCommand[13] = 0x00;
            _PLCCommand[14] = 0x0E;
            // 写入长度+4
            _PLCCommand[15] = (byte)((4 + data.Length) / 256);
            _PLCCommand[16] = (byte)((4 + data.Length) % 256);
            // 命令起始符
            _PLCCommand[17] = 0x05;
            // 写入数据块个数
            _PLCCommand[18] = 0x01;
            // 固定，返回数据长度
            _PLCCommand[19] = 0x12;
            _PLCCommand[20] = 0x0A;
            _PLCCommand[21] = 0x10;
            // 写入方式，1是按位，2是按字
            _PLCCommand[22] = 0x02;
            // 写入数据的个数
            _PLCCommand[23] = (byte)(data.Length / 256);
            _PLCCommand[24] = (byte)(data.Length % 256);
            // DB块编号，如果访问的是DB块的话
            _PLCCommand[25] = (byte)(dbAddress / 256);
            _PLCCommand[26] = (byte)(dbAddress % 256);
            // 写入数据的类型
            _PLCCommand[27] = type;
            // 偏移位置
            _PLCCommand[28] = (byte)(startAddress / 256 / 256 % 256); ;
            _PLCCommand[29] = (byte)(startAddress / 256 % 256);
            _PLCCommand[30] = (byte)(startAddress % 256);
            // 按字写入
            _PLCCommand[31] = 0x00;
            _PLCCommand[32] = 0x04;
            // 按位计算的长度
            _PLCCommand[33] = (byte)(data.Length * 8 / 256);
            _PLCCommand[34] = (byte)(data.Length * 8 % 256);

            data.CopyTo(_PLCCommand, 35);

            command = _PLCCommand;

            return true;
        }
        

        private bool BuildWriteBitCommand(string address, bool data,out byte[] command, OperateResult result)
        {
            if (!AnalysisAddress(address, out byte type, out int startAddress, out ushort dbAddress, result))
            {
                command = null;
                return false;
            }

            byte[] buffer = new byte[1];
            buffer[0] = data ? (byte)0x01 : (byte)0x00;

            byte[] _PLCCommand = new byte[35 + buffer.Length];
            _PLCCommand[0] = 0x03;
            _PLCCommand[1] = 0x00;
            // 长度
            _PLCCommand[2] = (byte)((35 + buffer.Length) / 256);
            _PLCCommand[3] = (byte)((35 + buffer.Length) % 256);
            // 固定
            _PLCCommand[4] = 0x02;
            _PLCCommand[5] = 0xF0;
            _PLCCommand[6] = 0x80;
            _PLCCommand[7] = 0x32;
            // 命令 发
            _PLCCommand[8] = 0x01;
            // 标识序列号
            _PLCCommand[9] = 0x00;
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x00;
            _PLCCommand[12] = 0x01;
            // 固定
            _PLCCommand[13] = 0x00;
            _PLCCommand[14] = 0x0E;
            // 写入长度+4
            _PLCCommand[15] = (byte)((4 + buffer.Length) / 256);
            _PLCCommand[16] = (byte)((4 + buffer.Length) % 256);
            // 命令起始符
            _PLCCommand[17] = 0x05;
            // 写入数据块个数
            _PLCCommand[18] = 0x01;
            _PLCCommand[19] = 0x12;
            _PLCCommand[20] = 0x0A;
            _PLCCommand[21] = 0x10;
            // 写入方式，1是按位，2是按字
            _PLCCommand[22] = 0x01;
            // 写入数据的个数
            _PLCCommand[23] = (byte)(buffer.Length / 256);
            _PLCCommand[24] = (byte)(buffer.Length % 256);
            // DB块编号，如果访问的是DB块的话
            _PLCCommand[25] = (byte)(dbAddress / 256);
            _PLCCommand[26] = (byte)(dbAddress % 256);
            // 写入数据的类型
            _PLCCommand[27] = type;
            // 偏移位置
            _PLCCommand[28] = (byte)(startAddress / 256 / 256);
            _PLCCommand[29] = (byte)(startAddress / 256);
            _PLCCommand[30] = (byte)(startAddress % 256);
            // 按位写入
            _PLCCommand[31] = 0x00;
            _PLCCommand[32] = 0x03;
            // 按位计算的长度
            _PLCCommand[33] = (byte)(buffer.Length / 256);
            _PLCCommand[34] = (byte)(buffer.Length % 256);

            buffer.CopyTo(_PLCCommand, 35);

            command = _PLCCommand;

            return true;
        }



        #endregion

        #region Public Method

        /// <summary>
        /// 可以手动设置PLC类型，用来测试原本不支持的数据访问功能
        /// </summary>
        /// <param name="type"></param>
        public void SetPlcType(byte type)
        {
            plcHead1[18] = type;
        }

        #endregion

        #region Private Method


        private bool ReceiveBytesFromSocket(Socket socket, out byte[] data, OperateResult result)
        {
            try
            {
                // 先接收4个字节的数据
                byte[] head = NetSupport.ReadBytesFromSocket(socket, 4);
                int receive = head[2] * 256 + head[3];
                data = new byte[receive];
                head.CopyTo(data, 0);
                NetSupport.ReadBytesFromSocket(socket, receive - 4).CopyTo(data, 4);
                return true;
            }
            catch(Exception ex)
            {
                socket?.Close();
                data = null;
                LogNet?.WriteException(LogHeaderText, "Receive Data Failed: ", ex);
                result.Message = "初始化信息接收失败：" + ex.Message;
                return false;
            }
        }

        private bool SendBytesToSocket(Socket socket, byte[] data, OperateResult result)
        {
            try
            {
                if (data != null)
                {
                    socket.Send(data);
                }
                return true;
            }
            catch(Exception ex)
            {
                result.Message = "初始化信息发送失败：" + ex.Message;
                LogNet?.WriteException(LogHeaderText, "Send Data Failed: ", ex);
                socket?.Close();
                return false;
            }
        }


        private int CalculateAddressStarted(string address)
        {
            if (address.IndexOf('.') < 0)
            {
                return Convert.ToInt32(address) * 8;
            }
            else
            {
                string[] temp = address.Split('.');
                return Convert.ToInt32(temp[0]) * 8 + Convert.ToInt32(temp[1]);
            }
        }

        /// <summary>
        /// 解析数据地址
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="type">类型</param>
        /// <param name="startAddress">其实地址</param>
        /// <param name="dbAddress">DB块地址</param>
        /// <param name="result">结果数据对象</param>
        /// <returns></returns>
        private bool AnalysisAddress(string address, out byte type, out int startAddress, out ushort dbAddress, OperateResult result)
        {
            try
            {
                dbAddress = 0;
                if (address[0] == 'I')
                {
                    type = 0x81;
                    startAddress = CalculateAddressStarted(address.Substring(1));
                }
                else if (address[0] == 'Q')
                {
                    type = 0x82;
                    startAddress = CalculateAddressStarted(address.Substring(1));
                }
                else if (address[0] == 'M')
                {
                    type = 0x83;
                    startAddress = CalculateAddressStarted(address.Substring(1));
                }
                else if (address[0] == 'D' || address.Substring(0, 2) == "DB")
                {
                    type = 0x84;
                    string[] adds = address.Split('.');
                    if (address[1] == 'B')
                    {
                        dbAddress = Convert.ToUInt16(adds[0].Substring(2));
                    }
                    else
                    {
                        dbAddress = Convert.ToUInt16(adds[0].Substring(1));
                    }

                    startAddress = CalculateAddressStarted(address.Substring(address.IndexOf('.') + 1));
                }
                else
                {
                    result.Message = "不支持的数据类型";
                    type = 0;
                    startAddress = 0;
                    dbAddress = 0;
                    return false;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                type = 0;
                startAddress = 0;
                dbAddress = 0;
                return false;
            }
            return true;
        }

        private bool InitilizationConnect(Socket socket, OperateResult result)
        {
            // 发送初始化信息
            if (!SendBytesToSocket(socket, plcHead1, result))
            {
                return false;
            }

            if (!ReceiveBytesFromSocket(socket, out byte[] head1, result))
            {
                return false;
            }

            if (!SendBytesToSocket(socket, plcHead2, result))
            {
                return false;
            }

            if (!ReceiveBytesFromSocket(socket, out byte[] head2, result))
            {
                return false;
            }

            return true;
        }



        #endregion

        #region Customer Support

        /// <summary>
        /// 读取自定义的数据类型，只要规定了写入和解析规则
        /// </summary>
        /// <typeparam name="T">类型名称</typeparam>
        /// <param name="address">起始地址</param>
        /// <returns></returns>
        public OperateResult<T> ReadFromPLC<T>(string address) where T : IDataTransfer, new()
        {
            OperateResult<T> result = new OperateResult<T>();
            T Content = new T();
            OperateResult<byte[]> read = ReadFromPLC(address, Content.ReadCount);
            if(read.IsSuccess)
            {
                Content.ParseSource(read.Content);
                result.Content = Content;
                result.IsSuccess = true;
            }
            else
            {
                result.ErrorCode = read.ErrorCode;
                result.Message = read.Message;
            }
            return result;
        }

        /// <summary>
        /// 写入自定义的数据类型到PLC去，只要规定了生成字节的方法即可
        /// </summary>
        /// <typeparam name="T">自定义类型</typeparam>
        /// <param name="address">起始地址</param>
        /// <param name="data">实例对象</param>
        /// <returns></returns>
        public OperateResult WriteIntoPLC<T>(string address , T data) where T : IDataTransfer, new()
        {
            return WriteIntoPLC(address, data.ToSource());
        }


        #endregion

        #region Read Support


        /// <summary>
        /// 从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <param name="count">读取的数量，以字节为单位</param>
        /// <returns></returns>
        public OperateResult<byte[]> ReadFromPLC(string address, ushort count)
        {
            return ReadFromPLC(new string[] { address }, new ushort[] { count });
        }

        /// <summary>
        /// 从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，以位为单位
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        private OperateResult<byte[]> ReadBitFromPLC(string address)
        {
            OperateResult<byte[]> result = new OperateResult<byte[]>();
            if (!BuildBitReadCommand(address, out byte[] command, result))
            {
                return result;
            }

            OperateResult<byte[]> read = ReadFromServerCore(command);
            if(read.IsSuccess)
            {
                int receiveCount = 1;

                if (read.Content.Length >= 21 && read.Content[20] == 1)
                {
                    // 分析结果
                    byte[] buffer = new byte[receiveCount];

                    if (22 < read.Content.Length)
                    {
                        if (read.Content[21] == 0xFF &&
                            read.Content[22] == 0x03)
                        {
                            // 有数据
                            buffer[0] = read.Content[25];
                        }
                    }

                    result.Content = buffer;
                    result.IsSuccess = true;
                }
                else
                {
                    result.ErrorCode = read.ErrorCode;
                    result.Message = "数据块长度校验失败";
                }
            }
            else
            {
                result.ErrorCode = read.ErrorCode;
                result.Message = read.Message;
            }

            return result;
        }


        /// <summary>
        /// 一次性从PLC获取所有的数据，按照先后顺序返回一个统一的Buffer，需要按照顺序处理，两个数组长度必须一致
        /// </summary>
        /// <param name="address">起始地址数组</param>
        /// <param name="count">数据长度数组</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public OperateResult<byte[]> ReadFromPLC(string[] address, ushort[] count)
        {
            OperateResult<byte[]> result = new OperateResult<byte[]>();
            if (!BuildReadCommand(address, count, out byte[] command, result))
            {
                return result;
            }

            OperateResult<byte[]> read = ReadFromServerCore(command);
            if(read.IsSuccess)
            {
                int receiveCount = 0;
                for (int i = 0; i < count.Length; i++)
                {
                    receiveCount += count[i];
                }

                if (read.Content.Length >= 21 && read.Content[20] == count.Length)
                {
                    // 分析结果
                    byte[] buffer = new byte[receiveCount];
                    int kk = 0;
                    int ll = 0;
                    for (int ii = 21; ii < read.Content.Length; ii++)
                    {
                        if ((ii + 1) < read.Content.Length)
                        {
                            if (read.Content[ii] == 0xFF &&
                                read.Content[ii + 1] == 0x04)
                            {
                                // 有数据
                                Array.Copy(read.Content, ii + 4, buffer, ll, count[kk]);
                                ii += count[kk] + 3;
                                ll += count[kk];
                                kk++;
                            }
                        }
                    }

                    result.Content = buffer;
                    result.IsSuccess = true;
                }
                else
                {
                    result.ErrorCode = read.ErrorCode;
                    result.Message = "数据块长度校验失败";
                }
            }
            else
            {
                result.ErrorCode = read.ErrorCode;
                result.Message = read.Message;
            }

            return result;
        }


        /// <summary>
        /// 读取指定地址的bool数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<bool> ReadBoolFromPLC(string address)
        {
            return GetBoolResultFromBytes(ReadBitFromPLC(address));
        }


        /// <summary>
        /// 读取指定地址的byte数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<byte> ReadByteFromPLC(string address)
        {
            return GetByteResultFromBytes(ReadFromPLC(address, 1));
        }


        /// <summary>
        /// 读取指定地址的short数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<short> ReadShortFromPLC(string address)
        {
            return GetInt16ResultFromBytes(ReadFromPLC(address, 2), true);
        }


        /// <summary>
        /// 读取指定地址的ushort数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<ushort> ReadUShortFromPLC(string address)
        {
            return GetUInt16ResultFromBytes(ReadFromPLC(address, 2), true);
        }

        /// <summary>
        /// 读取指定地址的int数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<int> ReadIntFromPLC(string address)
        {
            return GetInt32ResultFromBytes(ReadFromPLC(address, 4), true);
        }

        /// <summary>
        /// 读取指定地址的uint数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<uint> ReadUIntFromPLC(string address)
        {
            return GetUInt32ResultFromBytes(ReadFromPLC(address, 4), true);
        }

        /// <summary>
        /// 读取指定地址的float数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<float> ReadFloatFromPLC(string address)
        {
            return GetFloatResultFromBytes(ReadFromPLC(address, 4), true);
        }

        /// <summary>
        /// 读取指定地址的long数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<long> ReadLongFromPLC(string address)
        {
            return GetInt64ResultFromBytes(ReadFromPLC(address, 8), true);
        }

        /// <summary>
        /// 读取指定地址的ulong数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<ulong> ReadULongFromPLC(string address)
        {
            return GetUInt64ResultFromBytes(ReadFromPLC(address, 8), true);
        }

        /// <summary>
        /// 读取指定地址的double数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <returns></returns>
        public OperateResult<double> ReadDoubleFromPLC(string address)
        {
            return GetDoubleResultFromBytes(ReadFromPLC(address, 8), true);
        }

        /// <summary>
        /// 读取地址地址的String数据
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <param name="length">字符串长度</param>
        /// <returns></returns>
        public OperateResult<string> ReadStringFromPLC(string address, ushort length)
        {
            return GetStringResultFromBytes(ReadFromPLC(address, length));
        }




        #endregion

        #region Write Base
        

        /// <summary>
        /// 将数据写入到PLC数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <param name="data">写入的数据，长度根据data的长度来指示</param>
        /// <returns></returns>
        public OperateResult WriteIntoPLC(string address, byte[] data)
        {
            OperateResult result = new OperateResult();

            // 生成指令
            if (!BuildWriteByteCommand(address, data, out byte[] command, result))
            {
                return result;
            }

            OperateResult<byte[]> write = ReadFromServerCore(command);
            if (write.IsSuccess)
            {
                if (write.Content[write.Content.Length - 1] != 0xFF)
                {
                    // 写入异常
                    result.Message = "写入数据异常，代号为：" + write.Content[write.Content.Length - 1].ToString();
                }
                else
                {
                    result.IsSuccess = true;  // 写入成功
                }
            }
            else
            {
                result.ErrorCode = write.ErrorCode;
                result.Message = write.Message;
            }
            return result;
        }
        

        /// <summary>
        /// 写入PLC的一个位，例如"M100.6"，"I100.7"，"Q100.0"，"DB20.100.0"，如果只写了"M100"默认为"M100.0
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public OperateResult WriteIntoPLC(string address, bool data)
        {
            OperateResult result = new OperateResult();

            // 生成指令
            if (!BuildWriteBitCommand(address, data, out byte[] command, result))
            {
                return result;
            }

            OperateResult<byte[]> write = ReadFromServerCore(command);
            if (write.IsSuccess)
            {
                if (write.Content[write.Content.Length - 1] != 0xFF)
                {
                    // 写入异常
                    result.Message = "写入数据异常，代号为：" + write.Content[write.Content.Length - 1].ToString();
                }
                else
                {
                    result.IsSuccess = true;  // 写入成功
                }
            }
            else
            {
                result.ErrorCode = write.ErrorCode;
                result.Message = write.Message;
            }
            return result;
        }


        #endregion

        #region Write String



        /// <summary>
        /// 向PLC中写入字符串，编码格式为ASCII
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回读取结果</returns>
        public OperateResult WriteAsciiStringIntoPLC(string address, string data)
        {
            byte[] temp = Encoding.ASCII.GetBytes(data);
            return WriteIntoPLC(address, temp);
        }

        /// <summary>
        /// 向PLC中写入指定长度的字符串,超出截断，不够补0，编码格式为ASCII
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>返回读取结果</returns>
        public OperateResult WriteAsciiStringIntoPLC(string address, string data, int length)
        {
            byte[] temp = Encoding.ASCII.GetBytes(data);
            temp = ManageBytesLength(temp, length);
            return WriteIntoPLC(address, temp);
        }

        /// <summary>
        /// 向PLC中写入字符串，编码格式为Unicode
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回读取结果</returns>
        public OperateResult WriteUnicodeStringIntoPLC(string address, string data)
        {
            byte[] temp = Encoding.Unicode.GetBytes(data);
            return WriteIntoPLC(address, temp);
        }

        /// <summary>
        /// 向PLC中写入指定长度的字符串,超出截断，不够补0，编码格式为Unicode
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>返回读取结果</returns>
        public OperateResult WriteUnicodeStringIntoPLC(string address, string data, int length)
        {
            byte[] temp = Encoding.Unicode.GetBytes(data);
            temp = ManageBytesLength(temp, length * 2);
            return WriteIntoPLC(address, temp);
        }

        #endregion

        #region Write bool[]
        
        /// <summary>
        /// 向PLC中写入bool数组，返回值说明，比如你写入M100,那么data[0]对应M100.0
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据，长度为8的倍数</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, bool[] data)
        {
            return WriteIntoPLC(address, BasicFramework.SoftBasic.BoolArrayToByte(data));
        }


        #endregion

        #region Write Byte

        /// <summary>
        /// 向PLC中写入byte数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns></returns>
        public OperateResult WriteIntoPLC(string address,byte data)
        {
            return WriteIntoPLC(address, new byte[] { data });
        }

        #endregion

        #region Write Short

        /// <summary>
        /// 向PLC中写入short数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, short[] data)
        {
            return WriteIntoPLC(address, GetBytesFromArray(data, true));
        }

        /// <summary>
        /// 向PLC中写入short数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, short data)
        {
            return WriteIntoPLC(address, new short[] { data });
        }

        #endregion

        #region Write UShort


        /// <summary>
        /// 向PLC中写入ushort数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, ushort[] data)
        {
            return WriteIntoPLC(address, GetBytesFromArray(data, true));
        }


        /// <summary>
        /// 向PLC中写入ushort数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, ushort data)
        {
            return WriteIntoPLC(address, new ushort[] { data });
        }


        #endregion

        #region Write Int
        
        /// <summary>
        /// 向PLC中写入int数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, int[] data)
        {
            return WriteIntoPLC(address, GetBytesFromArray(data, true));
        }

        /// <summary>
        /// 向PLC中写入int数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, int data)
        {
            return WriteIntoPLC(address, new int[] { data });
        }

        #endregion

        #region Write UInt
        
        /// <summary>
        /// 向PLC中写入uint数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, uint[] data)
        {
            return WriteIntoPLC(address, GetBytesFromArray(data, true));
        }

        /// <summary>
        /// 向PLC中写入uint数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, uint data)
        {
            return WriteIntoPLC(address, new uint[] { data });
        }

        #endregion

        #region Write Float
        
        /// <summary>
        /// 向PLC中写入float数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, float[] data)
        {
            return WriteIntoPLC(address, GetBytesFromArray(data, true));
        }

        /// <summary>
        /// 向PLC中写入float数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, float data)
        {
            return WriteIntoPLC(address, new float[] { data });
        }


        #endregion

        #region Write Long
        
        /// <summary>
        /// 向PLC中写入long数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, long[] data)
        {
            return WriteIntoPLC(address, GetBytesFromArray(data, true));
        }

        /// <summary>
        /// 向PLC中写入long数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, long data)
        {
            return WriteIntoPLC(address, new long[] { data });
        }
        
        #endregion

        #region Write ULong
        
        /// <summary>
        /// 向PLC中写入ulong数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, ulong[] data)
        {
            return WriteIntoPLC(address, GetBytesFromArray(data, true));
        }

        /// <summary>
        /// 向PLC中写入ulong数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, ulong data)
        {
            return WriteIntoPLC(address, new ulong[] { data });
        }
        
        #endregion

        #region Write Double
        
        /// <summary>
        /// 向PLC中写入double数组，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, double[] data)
        {
            return WriteIntoPLC(address, GetBytesFromArray(data, true));
        }

        /// <summary>
        /// 向PLC中写入double数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, double data)
        {
            return WriteIntoPLC(address, new double[] { data });
        }
        
        #endregion

        #region PLC Net Base Override


        /// <summary>
        /// 从返回的西门子数组中获取short数组数据，已经内置高地位转换
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public override short[] GetArrayFromBytes(byte[] bytes)
        {
            short[] temp = new short[bytes.Length / 2];
            for (int i = 0; i < temp.Length; i++)
            {
                byte[] buffer = new byte[2];
                buffer[0] = bytes[i * 2 + 1];
                buffer[1] = bytes[i * 2];
                temp[i] = BitConverter.ToInt16(buffer, 0);
            }
            return temp;
        }

        /// <summary>
        /// 从返回的西门子数组中获取int数组数据，已经内置高地位转换
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public override int[] GetIntArrayFromBytes(byte[] bytes)
        {
            int[] temp = new int[bytes.Length / 4];
            for (int i = 0; i < temp.Length; i++)
            {
                byte[] buffer = new byte[4];
                buffer[0] = bytes[i * 4 + 3];
                buffer[1] = bytes[i * 4 + 2];
                buffer[2] = bytes[i * 4 + 1];
                buffer[3] = bytes[i * 4 + 0];
                temp[i] = BitConverter.ToInt32(buffer, 0);
            }
            return temp;
        }

        /// <summary>
        /// 根据索引位转换获取short数据
        /// </summary>
        /// <param name="content"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override short GetShortFromBytes(byte[] content, int index)
        {
            byte[] buffer = new byte[2];
            buffer[0] = content[index + 1];
            buffer[1] = content[index + 0];
            return BitConverter.ToInt16(buffer, 0);
        }


        #endregion

        #region Public Method


        /// <summary>
        /// 根据索引位转换获取float数据
        /// </summary>
        /// <param name="content"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetFloatFromBytes(byte[] content, int index)
        {
            byte[] buffer = new byte[4];
            buffer[0] = content[index + 3];
            buffer[1] = content[index + 2];
            buffer[2] = content[index + 1];
            buffer[3] = content[index + 0];
            return BitConverter.ToSingle(buffer, 0);
        }

        /// <summary>
        /// 根据索引位转换获取int数据
        /// </summary>
        /// <param name="content"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetIntFromBytes(byte[] content, int index)
        {
            byte[] buffer = new byte[4];
            buffer[0] = content[index + 3];
            buffer[1] = content[index + 2];
            buffer[2] = content[index + 1];
            buffer[3] = content[index + 0];
            return BitConverter.ToInt32(buffer, 0);
        }

        #endregion

        #region Private Members


        private byte[] plcHead1 = new byte[22]
        {
                0x03,  // 报文头
                0x00,
                0x00,  // 数据长度
                0x16,
                0x11,
                0xE0,
                0x00,
                0x00,
                0x00,
                0x01,
                0x00,
                0xC1,
                0x02,
                0x10,
                0x00,
                0xC2,
                0x02,
                0x03,
                0x01,  // 指示cpu
                0xC0,
                0x01,
                0x0A
        };
        private byte[] plcHead2 = new byte[25]
        {
                0x03,
                0x00,
                0x00,
                0x19,
                0x02,
                0xF0,
                0x80,
                0x32,
                0x01,
                0x00,
                0x00,
                0xCC,
                0xC1,
                0x00,
                0x08,
                0x00,
                0x00,
                0xF0,
                0x00,
                0x00,
                0x01,
                0x00,
                0x01,
                0x03,
                0xC0
        };
        
        private SiemensPLCS CurrentPlc = SiemensPLCS.S1200;
        
        #endregion
    }



}
