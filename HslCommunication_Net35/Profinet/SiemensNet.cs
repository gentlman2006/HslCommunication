using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;



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
        public OperateResultBytes ReadFromPLC(SiemensDataType type, ushort address, ushort lengh)
        {
            OperateResultBytes result = new OperateResultBytes();

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
                result.Message= StringResources.ConnectedFailed;
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
        public OperateResultBytes WriteAsciiStringIntoPLC(SiemensDataType type, ushort address, string data)
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
        public OperateResultBytes WriteAsciiStringIntoPLC(SiemensDataType type, ushort address, string data, int length)
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
        public OperateResultBytes WriteUnicodeStringIntoPLC(SiemensDataType type, ushort address, string data)
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
        public OperateResultBytes WriteUnicodeStringIntoPLC(SiemensDataType type, ushort address, string data, int length)
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
        public OperateResultBytes WriteIntoPLC(SiemensDataType type, ushort address, short[] data)
        {
            return WriteIntoPLC(type, address, GetBytesFromArray(data,false));
        }

        /// <summary>
        /// 向PLC中写入数据，返回值说明
        /// </summary>
        /// <param name="type">要写入的数据类型</param>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <exception cref="ArgumentNullException">写入的数据不能为空</exception>
        /// <returns>返回写入结果</returns>
        public OperateResultBytes WriteIntoPLC(SiemensDataType type, ushort address, byte[] data)
        {
            var result = new OperateResultBytes();
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
        /// <summary>
        /// 实例化一个数据通信的对象，需要指定访问哪种Plc
        /// </summary>
        /// <param name="siemens"></param>
        public SiemensTcpNet(SiemensPLCS siemens)
        {
            PortRead = 102;
            PortWrite = 102;
            CurrentPlc = siemens;
            
            switch (siemens)
            {
                case SiemensPLCS.S1200: plcHead1[18] = 1; break;
                case SiemensPLCS.S300: plcHead1[18] = 2; break;
                default: plcHead1[18] = 3; break;
            }
        }


        /// <summary>
        /// 可以手动设置PLC类型，用来测试原本不支持的数据访问功能
        /// </summary>
        /// <param name="type"></param>
        public void SetPlcType(byte type)
        {
            plcHead1[18] = type;
        }



        private bool ReceiveBytesFromSocket(Socket socket, out byte[] data)
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
            catch
            {
                socket?.Close();
                data = null;
                return false;
            }
        }

        private bool SendBytesToSocket(Socket socket, byte[] data)
        {
            try
            {
                if (data != null)
                {
                    socket.Send(data);
                }
                return true;
            }
            catch
            {
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

        private bool InitilizationConnect(Socket socket,OperateResult result)
        {
            // 发送初始化信息
            if(!SendBytesToSocket(socket,plcHead1))
            {
                result.Message = "初始化信息发送失败";
                return false;
            }

            if(!ReceiveBytesFromSocket(socket,out byte[] head1))
            {
                result.Message = "初始化信息接收失败";
                return false;
            }

            if(!SendBytesToSocket(socket,plcHead2))
            {
                result.Message = "初始化信息发送失败";
                return false;
            }

            if(!ReceiveBytesFromSocket(socket,out byte[] head2))
            {
                result.Message = "初始化信息接收失败";
                return false;
            }

            return true;
        }


        /// <summary>
        /// 从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <param name="count">读取的数量，以字节为单位</param>
        /// <returns></returns>
        public OperateResultBytes ReadFromPLC(string address, ushort count)
        {
            return ReadFromPLC(new string[] { address }, new ushort[] { count });
        }


        /// <summary>
        /// 一次性从PLC获取所有的数据，按照先后顺序返回一个统一的Buffer，需要按照顺序处理，两个数组长度必须一致
        /// </summary>
        /// <param name="address">起始地址数组</param>
        /// <param name="count">数据长度数组</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public OperateResultBytes ReadFromPLC(string[] address, ushort[] count)
        {
            if (address == null) throw new NullReferenceException("address");
            if (count == null) throw new NullReferenceException("count");
            if (address.Length != count.Length) throw new Exception("两个参数的个数不统一");

            OperateResultBytes result = new OperateResultBytes();

            if (!CreateSocketAndConnect(out Socket socket, new IPEndPoint(PLCIpAddress, PortRead), result))
            {
                return result;
            }

            if (!InitilizationConnect(socket, result))
            {
                return result;
            }


            // 分批次进行读取，计算总批次
            int times = address.Length / 255;
            if (address.Length % 255 > 0)
            {
                times++;
            }

            // 缓存所有批次的结果
            List<byte[]> arrays_bytes = new List<byte[]>();

            for (int jj = 0; jj < times; jj++)
            {
                // 计算本批次需要读取的数据
                int startIndex = jj * 255;
                int readCount = address.Length - startIndex;
                if (readCount > 255)
                {
                    readCount = 255;
                }

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

                int receiveCount = 0;

                for (int ii = 0; ii < readCount; ii++)
                {
                    //==============================================================================================
                    // 实际应该接收多少数据
                    receiveCount += count[ii + 255 * jj];

                    // 填充数据
                    if (!AnalysisAddress(address[ii + 255 * jj], out byte type, out int startAddress, out ushort dbAddress, result))
                    {
                        socket?.Close();
                        return result;
                    }



                    //===========================================================================================
                    // 读取地址的前缀
                    _PLCCommand[19 + ii * 12] = 0x12;
                    _PLCCommand[20 + ii * 12] = 0x0A;
                    _PLCCommand[21 + ii * 12] = 0x10;
                    _PLCCommand[22 + ii * 12] = 0x02;
                    // 访问数据的个数
                    _PLCCommand[23 + ii * 12] = (byte)(count[ii + 255 * jj] / 256);
                    _PLCCommand[24 + ii * 12] = (byte)(count[ii + 255 * jj] % 256);
                    // DB块编号，如果访问的是DB块的话
                    _PLCCommand[25 + ii * 12] = (byte)(dbAddress / 256);
                    _PLCCommand[26 + ii * 12] = (byte)(dbAddress % 256);
                    // 访问数据类型
                    _PLCCommand[27 + ii * 12] = type;
                    // 偏移位置
                    _PLCCommand[28 + ii * 12] = (byte)(startAddress / 256 / 256);
                    _PLCCommand[29 + ii * 12] = (byte)(startAddress / 256);
                    _PLCCommand[30 + ii * 12] = (byte)(startAddress % 256);
                }


                if (!SendBytesToSocket(socket, _PLCCommand))
                {
                    result.Message = "发送读取信息失败";
                    return result;
                }

                if (!ReceiveBytesFromSocket(socket, out byte[] content))
                {
                    result.Message = "接收信息失败";
                    return result;
                }

                if (content.Length < 21 || content[20] != readCount)
                {
                    socket?.Close();
                    result.Message = "数据块长度校验失败";
                    result.Content = content;
                    return result;
                }

                // 分次读取成功
                byte[] buffer = new byte[receiveCount];
                int kk = 0;
                int ll = 0;
                for (int ii = 21; ii < content.Length; ii++)
                {
                    if (content[ii] == 0xFF &&
                        content[ii + 1] == 0x04)
                    {
                        // 有数据
                        Array.Copy(content, ii + 4, buffer, ll, count[kk + 255 * jj]);
                        ii += count[kk + 255 * jj] + 3;
                        ll += count[kk + 255 * jj];
                        kk++;
                    }
                }
                arrays_bytes.Add(buffer);
            }
            
            if (arrays_bytes.Count == 1)
            {
                result.Content = arrays_bytes[0];
            }
            else
            {
                int length = 0;
                int offset = 0;

                // 获取长度并生成缓冲数据
                arrays_bytes.ForEach(m => length += m.Length);
                result.Content = new byte[length];

                // 复制数据操作
                arrays_bytes.ForEach(m =>
                {
                    m.CopyTo(result.Content, offset);
                    offset += m.Length;
                });
                arrays_bytes.Clear();
            }
            result.IsSuccess = true;
            socket?.Close();
            // 所有的数据接收完成，进行返回
            return result;
        }

        /// <summary>
        /// 将数据写入到PLC数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100</param>
        /// <param name="data">写入的数据，长度根据data的长度来指示</param>
        /// <returns></returns>
        public OperateResult WriteIntoPLC(string address, byte[] data)
        {
            OperateResult result = new OperateResult();
            if (!CreateSocketAndConnect(out Socket socket, new IPEndPoint(PLCIpAddress, GetPort()), result))
            {
                ChangePort();
                return result;
            }

            // 发送初始化信息
            if (!InitilizationConnect(socket, result))
            {
                return result;
            }

            if (data == null) data = new byte[0];

            if (!AnalysisAddress(address, out byte type, out int startAddress, out ushort dbAddress, result))
            {
                socket?.Close();
                return result;
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
            _PLCCommand[28] = (byte)(startAddress / 256 / 256);;
            _PLCCommand[29] = (byte)(startAddress / 256);
            _PLCCommand[30] = (byte)(startAddress % 256);
            // 按字写入
            _PLCCommand[31] = 0x00;
            _PLCCommand[32] = 0x04;
            // 按位计算的长度
            _PLCCommand[33] = (byte)(data.Length * 8 / 256);
            _PLCCommand[34] = (byte)(data.Length * 8 % 256);

            data.CopyTo(_PLCCommand, 35);

            if(!SendBytesToSocket(socket,_PLCCommand))
            {
                result.Message = "发送写入信息失败";
                return result;
            }

            if(!ReceiveBytesFromSocket(socket,out byte[] content))
            {
                result.Message = "接收信息失败";
                return result;
            }

            if (content[content.Length - 1] != 0xFF)
            {
                // 写入异常
                socket?.Close();
                result.Message = "写入数据异常";
                return result;
            }

            socket?.Close();
            result.IsSuccess = true;
            return result;
        }

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


        /// <summary>
        /// 向PLC中写入bool数组，返回值说明，比如你写入M100,那么data[0]对应M100.0
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, bool[] data)
        {
            int length = data.Length % 8 == 0 ? data.Length / 8 : data.Length / 8 + 1;
            byte[] buffer = new byte[length];

            for (int i = 0; i < data.Length; i++)
            {
                int index = i / 8;
                int offect = i % 8;

                if (data[i]) buffer[index] += (byte)(Math.Pow(2, offect));
            }

            return WriteIntoPLC(address, buffer);
        }

        /// <summary>
        /// 向PLC中写入short数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, short[] data)
        {
            return WriteIntoPLC(address, GetBytesFromArray(data,true));
        }

        /// <summary>
        /// 向PLC中写入ushort数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, ushort[] data)
        {
            return WriteIntoPLC(address, GetBytesFromArray(data,true));
        }        

        /// <summary>
        /// 向PLC中写入int数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, int[] data)
        {
            return WriteIntoPLC(address, GetBytesFromArray(data,true));
        }

        /// <summary>
        /// 向PLC中写入uint数据，返回值说明
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="data">要写入的实际数据</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteIntoPLC(string address, uint[] data)
        {
            return WriteIntoPLC(address, GetBytesFromArray(data,true));
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
            if (!CreateSocketAndConnect(out Socket socket, new IPEndPoint(PLCIpAddress, GetPort()), result))
            {
                ChangePort();
                return result;
            }

            // 发送初始化信息
            if (!InitilizationConnect(socket, result))
            {
                return result;
            }
            

            byte[] buffer = new byte[1];
            buffer[0] = data ? (byte)0x01 : (byte)0x00;
            

            if(!AnalysisAddress(address,out byte type,out int startAddress,out ushort dbAddress,result))
            {
                socket?.Close();
                return result;
            }

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

            if(!SendBytesToSocket(socket,_PLCCommand))
            {
                result.Message = "发送写入信息失败";
                return result;
            }

            if(!ReceiveBytesFromSocket(socket,out byte[] content))
            {
                result.Message = "接收信息失败";
                return result;
            }

            if (content[content.Length - 1] != 0xFF)
            {
                // 写入异常
                socket?.Close();
                result.Message = "写入数据异常";
                return result;
            }

            socket?.Close();
            result.IsSuccess = true;
            return result;
        }

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

        //private byte[] plcHead1 = new byte[22]
        //{
        //        0x03,  // 报文头
        //        0x00,  
        //        0x00,  // 数据长度
        //        0x16,
        //        0x11,
        //        0xE0,
        //        0x00,
        //        0x00,
        //        0x00,
        //        0x01,
        //        0x00,
        //        0xC1,
        //        0x02,
        //        0x01,
        //        0x00,
        //        0xC2,
        //        0x02,
        //        0x01,
        //        0x01,  // 指示cpu
        //        0xC0,
        //        0x01,
        //        0x09
        //};

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

        //private byte[] plcHead2 = new byte[25]
        //{
        //        0x03,
        //        0x00,
        //        0x00,
        //        0x19,
        //        0x02,
        //        0xF0,
        //        0x80,
        //        0x32,
        //        0x01,
        //        0x00,
        //        0x00,
        //        0xFF,
        //        0xFF,
        //        0x00,
        //        0x08,
        //        0x00,
        //        0x00,
        //        0xF0,
        //        0x00,
        //        0x00,
        //        0x01,
        //        0x00,
        //        0x01,
        //        0x07,
        //        0x80
        //};

        private SiemensPLCS CurrentPlc = SiemensPLCS.S1200;
    }



}
