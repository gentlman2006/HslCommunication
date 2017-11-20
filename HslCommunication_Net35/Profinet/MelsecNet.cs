using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace HslCommunication.Profinet
{
    /// <summary>
    /// 三菱网络类基类，提供三菱基础数据和方法
    /// </summary>
    public class MelsecNetBase : PlcNetBase
    {
        /// <summary>
        /// 获取或设置PLC所在的网络号，一般都为0
        /// </summary>
        public byte NetworkNumber { get; set; } = 0x00;
        /// <summary>
        /// 获取或设置PLC所在网络的站号，一般都为0
        /// </summary>
        public byte NetworkStationNumber { get; set; } = 0x00;
        /// <summary>
        /// 当读取位数据时，进行相对的转化
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        protected byte[] ReceiveBytesTranslate(byte[] bytes)
        {
            byte[] result = new byte[bytes.Length * 2];

            for (int i = 0; i < bytes.Length; i++)
            {
                result[i * 2 + 0] = (byte)(bytes[i] >> 4);//取高字节
                result[i * 2 + 1] = (byte)(bytes[i] & 0x0f);//取低字节
            }
            return result;
        }
        /// <summary>
        /// 根据读取的数据类型和长度返回最终生成的数据
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        protected int GetLengthFromPlcType(MelsecDataType type, int length)
        {
            if (type.DataType == 1)
            {
                return length + 2;
            }
            else
            {
                return length * 2 + 2;
            }
        }

        /// <summary>
        /// 将BOOL变量数组转化成可写入PLC的字节数据
        /// </summary>
        /// <param name="data">bool数组</param>
        /// <returns>可写入PLC的字节数据</returns>
        protected byte[] BoolTranslateBytes(bool[] data)
        {
            byte[] temp = new byte[data.Length / 2];
            for (int i = 0; i < data.Length / 2; i++)
            {
                if (data[i * 2 + 0]) temp[i] += 0x10;
                if (data[i * 2 + 1]) temp[i] += 0x01;
            }
            return temp;
        }
        /// <summary>
        /// 根据字节数据进行补齐偶数位
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <returns>补齐后的数据</returns>
        protected byte[] SingularTurnEven(byte[] bytes)
        {
            if (bytes.Length % 2 == 1)
            {
                //补齐末尾数据
                byte[] temp2 = new byte[bytes.Length + 1];
                Array.Copy(bytes, 0, temp2, 0, bytes.Length);
                bytes = temp2;
            }
            return bytes;
        }
        /// <summary>
        /// 根据类型地址长度确认需要读取的指令头
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="address">起始地址</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        protected byte[] GetReadCommand(MelsecDataType type, ushort address, ushort length)
        {
            byte[] _PLCCommand = new byte[21];
            //默认信息----注意：高低字节交错
            _PLCCommand[0] = 0x50;//副标题
            _PLCCommand[1] = 0x00;
            _PLCCommand[2] = NetworkNumber;//网络号
            _PLCCommand[3] = 0xFF;//PLC编号
            _PLCCommand[4] = 0xFF;//目标模块IO编号
            _PLCCommand[5] = 0x03;
            _PLCCommand[6] = NetworkStationNumber;//目标模块站号
            _PLCCommand[7] = 0x0C;//请求数据长度
            _PLCCommand[8] = 0x00;
            _PLCCommand[9] = 0x0A;//CPU监视定时器
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x01;//批量读取数据命令
            _PLCCommand[12] = 0x04;
            _PLCCommand[13] = type.DataType;//一点为单位成批读取
            _PLCCommand[14] = 0x00;
            _PLCCommand[15] = (byte)(address % 256); ;//起始地址的地位
            _PLCCommand[16] = (byte)(address / 256);
            _PLCCommand[17] = 0x00;
            _PLCCommand[18] = type.DataCode;//指明读取的数据
            _PLCCommand[19] = (byte)(length % 256);//软元件长度的地位
            _PLCCommand[20] = (byte)(length / 256);
            return _PLCCommand;
        }
        /// <summary>
        /// 根据类型地址以及需要写入的数据来生成指令头
        /// </summary>
        /// <param name="type"></param>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected byte[] GetWriteCommand(MelsecDataType type, ushort address, byte[] data)
        {
            byte[] _PLCCommand = new byte[21 + data.Length];

            //默认信息----注意：高低字节交错
            _PLCCommand[0] = 0x50;//副标题
            _PLCCommand[1] = 0x00;
            _PLCCommand[2] = NetworkNumber;//网络号
            _PLCCommand[3] = 0xFF;//PLC编号
            _PLCCommand[4] = 0xFF;//目标模块IO编号
            _PLCCommand[5] = 0x03;
            _PLCCommand[6] = NetworkStationNumber;//目标模块站号

            _PLCCommand[7] = (byte)((_PLCCommand.Length - 9) % 256);//请求数据长度
            _PLCCommand[8] = (byte)((_PLCCommand.Length - 9) / 256); ;
            _PLCCommand[9] = 0x0A;//CPU监视定时器
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x01;//批量读取数据命令
            _PLCCommand[12] = 0x14;
            _PLCCommand[13] = type.DataType;//一点为单位成批读取
            _PLCCommand[14] = 0x00;
            _PLCCommand[15] = (byte)(address % 256); ;//起始地址的地位
            _PLCCommand[16] = (byte)(address / 256);
            _PLCCommand[17] = 0x00;
            _PLCCommand[18] = type.DataCode;//指明写入的数据

            if (type.DataType == 1)
            {
                _PLCCommand[19] = (byte)(data.Length * 2 % 256);//软元件长度的地位
                _PLCCommand[20] = (byte)(data.Length * 2 / 256);
            }
            else
            {
                _PLCCommand[19] = (byte)(data.Length / 2 % 256);//软元件长度的地位
                _PLCCommand[20] = (byte)(data.Length / 2 / 256);
            }

            Array.Copy(data, 0, _PLCCommand, 21, data.Length);
            return _PLCCommand;
        }
    }


    /// <summary>
    /// 数据访问类，用于计算机和三菱PLC的以太网模块通讯的类
    /// 通讯协议为基于以太网的QnA兼容3E帧协议
    /// 只支持同步访问方式，必须先配置网络参数
    /// </summary>
    public sealed class MelsecNet : MelsecNetBase
    {
        /// <summary>
        /// 从三菱PLC中读取想要的数据，返回读取结果
        /// </summary>
        /// <param name="type">想要读取的数据类型</param>
        /// <param name="address">读取数据的起始地址</param>
        /// <param name="length">读取的数据长度，字最大值960，位最大值7168</param>
        /// <returns>带数据头的字节数组</returns>
        public OperateResult<byte[]> ReadFromPLC(MelsecDataType type, ushort address, ushort length)
        {
            OperateResult<byte[]> result = new OperateResult<byte[]>();
            //获取指令
            byte[] _PLCCommand = GetReadCommand(type, address, length);


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
                result.Message = StringResources.ConnectedFailed; ;
                socket.Close();
                return result;
            }
            byte[] DataHead = null;
            try
            {
                socket.Send(_PLCCommand);
                //先接收满9个数据
                int NeedReceived = 9;
                DataHead = NetSupport.ReadBytesFromSocket(socket, NeedReceived);
                NeedReceived = BitConverter.ToUInt16(DataHead, 7);
                DataHead = NetSupport.ReadBytesFromSocket(socket, NeedReceived);
                //获取读取代码
                result.ErrorCode = BitConverter.ToUInt16(DataHead, 0);
                result.Content = new byte[DataHead.Length - 2];
                Array.Copy(DataHead, 2, result.Content, 0, DataHead.Length - 2);
                if (type.DataType == 1) result.Content = ReceiveBytesTranslate(result.Content);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = StringResources.SocketIOException + ex.Message;
                socket.Close();
                return result;
            }
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            socket = null;
            if (result.ErrorCode > 0) result.IsSuccess = false;
            return result;
        }



        /// <summary>
        /// 向PLC写入ASCII编码字符串数据，针对W,D的方式，数据为字符串
        /// </summary>
        /// <param name="type">写入的数据类型</param>
        /// <param name="address">初始地址</param>
        /// <param name="data">字符串数据信息</param>
        /// <returns>结果</returns>
        public OperateResult WriteAsciiStringIntoPLC(MelsecDataType type, ushort address, string data)
        {
            byte[] temp = Encoding.ASCII.GetBytes(data);
            temp = SingularTurnEven(temp);
            return WriteIntoPLC(type, address, temp);
        }
        /// <summary>
        /// 向PLC写入指定长度ASCII编码字符串数据，超过截断，不够补0，针对W,D的方式，数据为字符串
        /// </summary>
        /// <param name="type">写入的数据类型</param>
        /// <param name="address">初始地址</param>
        /// <param name="data">字符串数据信息</param>
        /// <param name="length">指定写入长度，必须为偶数</param>
        /// <returns>结果</returns>
        public OperateResult WriteAsciiStringIntoPLC(MelsecDataType type, ushort address, string data, int length)
        {
            byte[] temp = Encoding.ASCII.GetBytes(data);
            //补位到指定长度
            temp = ManageBytesLength(temp, length);
            //偶数位补齐
            temp = SingularTurnEven(temp);
            //写入
            return WriteIntoPLC(type, address, temp);
        }
        /// <summary>
        /// 使用Unicode编码向PLC写入字符串数据，针对W,D的方式，数据为字符串
        /// </summary>
        /// <param name="type">写入的数据类型</param>
        /// <param name="address">初始地址</param>
        /// <param name="data">字符串数据信息</param>
        /// <returns>结果</returns>
        public OperateResult WriteUnicodeStringIntoPLC(MelsecDataType type, ushort address, string data)
        {
            byte[] temp = Encoding.Unicode.GetBytes(data);
            return WriteIntoPLC(type, address, temp);
        }

        /// <summary>
        /// 使用Unicode编码向PLC写入指定长度的字符串数据，针对W,D的方式，数据为字符串
        /// </summary>
        /// <param name="type">写入的数据类型</param>
        /// <param name="address">初始地址</param>
        /// <param name="data">字符串数据信息</param>
        /// <param name="length">指定字符串的长度</param>
        /// <returns>结果</returns>
        public OperateResult WriteUnicodeStringIntoPLC(MelsecDataType type, ushort address, string data, int length)
        {
            byte[] temp = Encoding.Unicode.GetBytes(data);
            //扩充指定长度的数据
            temp = ManageBytesLength(temp, length * 2);
            return WriteIntoPLC(type, address, temp);
        }


        /// <summary>
        /// 向PLC写入数据，针对X,Y,M,L的方式，数据为通断的信号
        /// data数组长度必须为偶数，否则报错
        /// </summary>
        /// <param name="type">写入的数据类型</param>
        /// <param name="address">初始地址</param>
        /// <param name="data">通断信号的数组</param>
        /// <exception cref="Exception"></exception>
        /// <returns>结果</returns>
        public OperateResult WriteIntoPLC(MelsecDataType type, ushort address, bool[] data)
        {
            if (data.Length % 2 == 1)
            {
                throw new Exception("数据的数组长度必须为2的倍数");
            }
            //转换成实际的数据
            byte[] temp = BoolTranslateBytes(data);
            return WriteIntoPLC(type, address, temp);
        }
        /// <summary>
        /// 向PLC写入数据，针对D和W的方式，数据格式为无符号的ushort数组
        /// </summary>
        /// <param name="type">写入的数据类型</param>
        /// <param name="address">初始地址</param>
        /// <param name="data">无符号的ushort数组</param>
        /// <returns>结果</returns>
        public OperateResult WriteIntoPLC(MelsecDataType type, ushort address, ushort[] data)
        {
            byte[] temp = GetBytesFromArray(data,false);
            return WriteIntoPLC(type, address, temp);
        }
        /// <summary>
        /// 向PLC写入数据，针对D和W的方式，数据格式为有符号的short数组
        /// </summary>
        /// <param name="type">写入的数据类型</param>
        /// <param name="address">初始地址</param>
        /// <param name="data">有符号的short数组</param>
        /// <returns>结果</returns>
        public OperateResult WriteIntoPLC(MelsecDataType type, ushort address, short[] data)
        {
            byte[] temp = GetBytesFromArray(data,false);
            return WriteIntoPLC(type, address, temp);
        }

        /// <summary>
        /// 向PLC写入数据，数据格式为原始的字节类型
        /// </summary>
        /// <param name="type">写入的数据类型</param>
        /// <param name="address">初始地址</param>
        /// <param name="data">原始的字节数据</param>
        /// <returns>结果</returns>
        public OperateResult WriteIntoPLC(MelsecDataType type, ushort address, byte[] data)
        {
            OperateResult<byte[]> result = new OperateResult<byte[]>();
            byte[] _PLCCommand = GetWriteCommand(type, address, data);

            Array.Copy(data, 0, _PLCCommand, 21, data.Length);

            //测试指令数据
            //string test = ByteToHexString(_PLCCommand);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            HslTimeOut timeout = new HslTimeOut()
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
                socket.Close();
                return result;
            }
            byte[] DataHead = null;
            try
            {
                socket.Send(_PLCCommand);
                //先接收满9个数据
                int NeedReceived = 9;
                DataHead = NetSupport.ReadBytesFromSocket(socket, NeedReceived);
                NeedReceived = BitConverter.ToUInt16(DataHead, 7);
                DataHead = NetSupport.ReadBytesFromSocket(socket, NeedReceived);
                result.ErrorCode = BitConverter.ToUInt16(DataHead, 0);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = StringResources.SocketIOException + ex.Message;
                Thread.Sleep(10);
                socket.Close();
                return result;
            }
            socket.Shutdown(SocketShutdown.Both);
            Thread.Sleep(10);
            socket.Close();
            socket = null;
            if (result.ErrorCode > 0) result.IsSuccess = false;
            return result;
        }
    }


    /// <summary>
    /// 异步的多PLC请求类，可以同时并发的向多个PLC发起请求，该类使用非阻塞的高性能方式实现，需要好好测试
    /// </summary>
    public sealed class MelsecNetMultiAsync : MelsecNetBase
    {
        /// <summary>
        /// 实例化一个多PLC请求的对象
        /// </summary>
        /// <param name="networkNumber">要求所有网络号一致</param>
        /// <param name="networkStationNumber">要求所有站号一致</param>
        /// <param name="type">要求所有读取类型一致</param>
        /// <param name="address">要求所有起始地址一致</param>
        /// <param name="length">要求所有读取数据长度一致</param>
        /// <param name="timeout">连接PLC过程中的超时时间</param>
        /// <param name="readCycle">每次请求访问的间隔时间</param>
        /// <param name="allAddress">所有等待访问的PLC的IP地址</param>
        /// <param name="portMain">访问PLC时的主端口</param>
        /// <param name="portBackup">访问PLC时的备用端口</param>
        public MelsecNetMultiAsync(
            byte networkNumber, 
            byte networkStationNumber, 
            MelsecDataType type,
            ushort address,
            ushort length,
            int timeout,
            int readCycle,
            IPAddress[] allAddress,
            int portMain,
            int portBackup)
        {
            NetworkNumber = networkNumber;
            NetworkStationNumber = networkStationNumber;
            MelsecType = type;
            Address = address;
            Length = length;
            Timeout = timeout;
            ReadCycle = readCycle;
            //初始化数据对象
            EveryMachineLength = GetLengthFromPlcType(type, length);
            BytesResult = new byte[EveryMachineLength * allAddress.Length];

            PlcStates = new PlcStateOne[allAddress.Length];
            for (int i = 0; i < PlcStates.Length; i++)
            {
                PlcStates[i] = new PlcStateOne()
                {
                    Index = i,
                    PlcDataHead = new byte[9],
                    PlcDataContent = new byte[EveryMachineLength],
                    LengthDataContent = 0,
                    LengthDataHead = 0,
                    IsConnect = false,
                    PlcIpAddress = allAddress[i],
                    PortMain = portMain,
                    PortBackup = portBackup,
                };
            }
            //启动线程
            Thread thread = new Thread(new ThreadStart(ThreadDealWithTimeout))
            {
                IsBackground = true
            };
            thread.Start();
        }


        /*********************************************************************************************
         * 
         *    拷贝的数据方式仍然需要修改
         * 
         * 
         *********************************************************************************************/

            
        private MelsecDataType MelsecType = MelsecDataType.D;
        private ushort Address = 0;
        private ushort Length = 0;
        /// <summary>
        /// 超时时间
        /// </summary>
        private int Timeout = 600;
        /// <summary>
        /// 访问周期
        /// </summary>
        private int ReadCycle = 1000;
        /// <summary>
        /// 系统的连接状态，0未连接，1连接中
        /// </summary>
        private int ConnectStatus = 0;
        /// <summary>
        /// 用于存储结果的字节数组
        /// </summary>
        private byte[] BytesResult = null;
        /// <summary>
        /// 每台设备的长度
        /// </summary>
        private int EveryMachineLength = 0;
        /// <summary>
        /// 接收到所有的PLC数据时候触发
        /// </summary>
        public event IEDelegate<byte[]> OnReceivedData = null;

        private PlcStateOne[] PlcStates = null;
        /// <summary>
        /// 维护超时连接的线程方法
        /// </summary>
        private void ThreadDealWithTimeout()
        {
            LogNet?.WriteInfo("waitting one second");
            Thread.Sleep(1000);// 刚启动的时候进行休眠一小会
            LogNet?.WriteInfo("begining recyle for reading plc");
            while (true)
            {
                DateTime firstTime = DateTime.Now;// 连接时间

                TimerCallBack();// 启动访问PLC

                while ((DateTime.Now - firstTime).TotalMilliseconds < Timeout)
                {
                    // 超时时间等待
                }
                // 连接超时处理
                for (int i = 0; i < PlcStates.Length; i++)
                {
                    if (!PlcStates[i].IsConnect) PlcStates[i].WorkSocket.Close();
                }

                while ((DateTime.Now - firstTime).TotalMilliseconds < ReadCycle)
                {
                    // 等待下次连接
                }

            }
        }
        private void TimerCallBack()
        {
            // 如果已经连接就返回，此处采用原子操作实现
            if (Interlocked.CompareExchange(ref ConnectStatus, 1, 0) == 0)
            {
                m_ac = new AsyncCoordinator();
                for (int i = 0; i < PlcStates.Length; i++)
                {
                    PlcStates[i].IsConnect = false;
                    PlcStates[i].WorkSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    PlcStates[i].WorkSocket.BeginConnect(new IPEndPoint(PlcStates[i].PlcIpAddress, PlcStates[i].GetPort()), new AsyncCallback(PlcConnectCallBack), PlcStates[i]);
                    PlcStates[i].LengthDataHead = 0;
                    PlcStates[i].LengthDataContent = 0;

                    m_ac.AboutToBegin(1);
                }
                // 启动检查连接情况
                m_ac.AllBegun(AllDown);
            }
        }

        private byte[] ConnectWrongHead = new byte[] { 0x00, 0x01 };

        private void PlcConnectCallBack(IAsyncResult ar)
        {
            if(ar.AsyncState is PlcStateOne stateone)
            {
                try
                {
                    stateone.WorkSocket.EndConnect(ar);
                    stateone.WorkSocket.BeginReceive(
                        stateone.PlcDataHead,
                        stateone.LengthDataHead,
                        stateone.PlcDataHead.Length - stateone.LengthDataHead,
                        SocketFlags.None,
                        new AsyncCallback(PlcHeadReceiveCallBack),
                        stateone);
                    stateone.IsConnect = true;// 指示访问成功
                }
                catch(Exception ex)
                {
                    LogNet?.WriteException("connect failed", ex);
                    // 访问失败
                    stateone.WorkSocket.Close();
                    // 初始化数据
                    Array.Copy(ConnectWrongHead, 0, BytesResult, stateone.Index * EveryMachineLength, 2);
                    // 更换端口
                    stateone.ChangePort();
                    // 结束任务
                    m_ac.JustEnded();
                }
            }
        }

        private void PlcHeadReceiveCallBack(IAsyncResult ar)
        {
            if (ar.AsyncState is PlcStateOne stateone)
            {
                try
                {
                    stateone.LengthDataHead += stateone.WorkSocket.EndReceive(ar);
                    if (stateone.LengthDataHead < stateone.PlcDataHead.Length)
                    {
                        // 继续接收头格式
                        stateone.WorkSocket.BeginReceive(
                        stateone.PlcDataHead,
                        stateone.LengthDataHead,
                        stateone.PlcDataHead.Length - stateone.LengthDataHead,
                        SocketFlags.None,
                        new AsyncCallback(PlcHeadReceiveCallBack),
                        stateone);
                    }
                    else
                    {
                        // 计算接下来的接收长度，最少还有2个长度的字节数据
                        int NeedReceived = BitConverter.ToUInt16(stateone.PlcDataHead, 7);
                        stateone.PlcDataContent = new byte[NeedReceived];
                        // 接收内容
                        stateone.WorkSocket.BeginReceive(
                        stateone.PlcDataContent,
                        0,
                        stateone.PlcDataContent.Length - stateone.LengthDataContent,
                        SocketFlags.None,
                        new AsyncCallback(PlcContentReceiveCallBack),
                        stateone);
                    }
                }
                catch(Exception ex)
                {
                    LogNet?.WriteException("Head receive", ex);
                    // 由于未知原因，数据接收失败
                    stateone.WorkSocket.Close();
                    // 初始化数据
                    Array.Copy(ConnectWrongHead, 0, BytesResult, stateone.Index * EveryMachineLength, 2);
                    // 结束任务
                    m_ac.JustEnded();
                }
            }
        }

        private void PlcContentReceiveCallBack(IAsyncResult ar)
        {
            if (ar.AsyncState is PlcStateOne stateone)
            {
                try
                {
                    stateone.LengthDataContent += stateone.WorkSocket.EndReceive(ar);
                    if (stateone.LengthDataHead < stateone.PlcDataHead.Length)
                    {
                        // 继续接内容格式
                        stateone.WorkSocket.BeginReceive(
                        stateone.PlcDataContent,
                        stateone.LengthDataContent,
                        stateone.PlcDataContent.Length - stateone.LengthDataContent,
                        SocketFlags.None,
                        new AsyncCallback(PlcContentReceiveCallBack),
                        stateone);
                    }
                    else
                    {
                        // 所有内容接收完成
                        int error = BitConverter.ToUInt16(stateone.PlcDataContent, 0);
                        if (error > 0)
                        {
                            Array.Copy(ConnectWrongHead, 0, BytesResult, stateone.Index * EveryMachineLength, 2);
                        }
                        else
                        {
                            Array.Copy(stateone.PlcDataContent, 0, BytesResult, stateone.Index * EveryMachineLength, stateone.PlcDataContent.Length);
                        }
                        // 关闭连接
                        stateone.WorkSocket.Close();
                        // 结束任务
                        m_ac.JustEnded();
                    }
                }
                catch(Exception ex)
                {
                    LogNet?.WriteException("Data receive", ex);
                    // 由于未知原因，数据接收失败
                    stateone.WorkSocket.Close();
                    // 初始化数据
                    Array.Copy(ConnectWrongHead, 0, BytesResult, stateone.Index * EveryMachineLength, 2);
                    // 结束任务
                    m_ac.JustEnded();
                }
            }
        }

        private void AllDown(CoordinationStatus status)
        {
            // 此处没有取消和超时状态，直接完成
            if (status == CoordinationStatus.AllDone)
            {
                Interlocked.Exchange(ref ConnectStatus, 0);
                LogNet?.WriteDebug("All bytes read complete.");
                OnReceivedData?.Invoke(BytesResult.ToArray());
            }
        }

        private AsyncCoordinator m_ac = new AsyncCoordinator();
    }
    





    /// <summary>
    /// 三菱PLC的数据类型，此处只包含了几个常用的类型
    /// X,Y,M,D,W,L
    /// </summary>
    public class MelsecDataType
    {
        /// <summary>
        /// 如果您清楚类型代号，可以根据值进行扩展
        /// </summary>
        /// <param name="code">数据类型的代号</param>
        /// <param name="type">0或1，默认为0</param>
        public MelsecDataType(byte code, byte type)
        {
            DataCode = code;
            if (type < 2) DataType = type;
        }
        /// <summary>
        /// 类型的代号值
        /// </summary>
        public byte DataCode { get; private set; } = 0x00;
        /// <summary>
        /// 数据的类型，0代表按字，1代表按位
        /// </summary>
        public byte DataType { get; private set; } = 0x00;

        /// <summary>
        /// X输入寄存器
        /// </summary>
        public readonly static MelsecDataType X = new MelsecDataType(0x9C, 0x01);
        /// <summary>
        /// Y输出寄存器
        /// </summary>
        public readonly static MelsecDataType Y = new MelsecDataType(0x9D, 0x01);
        /// <summary>
        /// M中间寄存器
        /// </summary>
        public readonly static MelsecDataType M = new MelsecDataType(0x90, 0x01);
        /// <summary>
        /// D数据寄存器
        /// </summary>
        public readonly static MelsecDataType D = new MelsecDataType(0xA8, 0x00);
        /// <summary>
        /// W链接寄存器
        /// </summary>
        public readonly static MelsecDataType W = new MelsecDataType(0xB4, 0x00);
        /// <summary>
        /// L锁存继电器
        /// </summary>
        public readonly static MelsecDataType L = new MelsecDataType(0x92, 0x01);
    }
}
