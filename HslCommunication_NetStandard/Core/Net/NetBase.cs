using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using HslCommunication.BasicFramework;
using HslCommunication.Enthernet;
using HslCommunication.LogNet;

namespace HslCommunication.Core
{
    /*******************************************************************************
     * 
     *    网络通信类的基础类，提供所有相关的基础方法和功能
     *
     *    Network communication base class of the class, provides the basis of all relevant methods and functions
     * 
     *******************************************************************************/

    #region 网络传输辅助类

    /// <summary>
    /// 静态的方法支持类，提供一些网络的静态支持
    /// </summary>
    public static class NetSupport
    {
        /// <summary>
        /// Socket传输中的缓冲池大小
        /// </summary>
        internal const int SocketBufferSize = 4096;

        /// <summary>
        /// 检查是否超时的静态方法
        /// </summary>
        /// <param name="timeout">数据封送对象</param>
        /// <param name="millisecond">超时的时间</param>
        internal static void ThreadPoolCheckConnect(HslTimeOut timeout, int millisecond)
        {
            while (!timeout.IsSuccessful)
            {
                if ((DateTime.Now - timeout.StartTime).TotalMilliseconds > millisecond)
                {
                    // 连接超时或是验证超时
                    if (!timeout.IsSuccessful) timeout.WorkSocket?.Close();
                    break;
                }
                Thread.Sleep(100);
            }
        }

        internal static void ThreadPoolCheckTimeOut(object obj)
        {
            if (obj is HslTimeOut timeout)
            {
                while (!timeout.IsSuccessful)
                {
                    if ((DateTime.Now - timeout.StartTime).TotalMilliseconds > timeout.DelayTime)
                    {
                        // 连接超时或是验证超时
                        if (!timeout.IsSuccessful)
                        {
                            timeout.Operator?.Invoke();
                            timeout.WorkSocket?.Close();
                        }
                        break;
                    }
                }
            }
        }



        /// <summary>
        /// 判断两个字节是否相同
        /// </summary>
        /// <param name="b1">第一个字节</param>
        /// <param name="b2">第二个字节</param>
        /// <returns></returns>
        internal static bool IsTwoBytesEquel(byte[] b1, byte[] b2)
        {
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 判断两个数据的令牌是否相等
        /// </summary>
        /// <param name="head">头指令</param>
        /// <param name="token">令牌</param>
        /// <returns></returns>
        internal static bool CheckTokenEquel(byte[] head, Guid token)
        {
            return IsTwoBytesEquel(head, 12, token.ToByteArray(), 0, 16);
        }


        /// <summary>
        /// 判断两个字节的指定部分是否相同
        /// </summary>
        /// <param name="b1">第一个字节</param>
        /// <param name="start1">第一个字节的起始位置</param>
        /// <param name="b2">第二个字节</param>
        /// <param name="start2">第二个字节的起始位置</param>
        /// <param name="length">校验的长度</param>
        /// <returns>返回是否相等</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        internal static bool IsTwoBytesEquel(byte[] b1, int start1, byte[] b2, int start2, int length)
        {
            if (b1 == null || b2 == null) return false;
            for (int i = 0; i < length; i++)
            {
                if (b1[i + start1] != b2[i + start2])
                {
                    return false;
                }
            }
            return true;
        }



        /// <summary>
        /// 读取socket数据的基础方法，只适合用来接收指令头，或是同步数据
        /// </summary>
        /// <param name="socket">通信对象</param>
        /// <param name="receive">接收的长度</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        public static byte[] ReadBytesFromSocket(Socket socket, int receive)
        {
            return ReadBytesFromSocket(socket, receive, null, false, false);
        }


        /// <summary>
        /// 读取socket数据的基础方法，只适合用来接收指令头，或是同步数据
        /// </summary>
        /// <param name="socket">通信对象</param>
        /// <param name="receive">接收的长度</param>
        /// <param name="report">用于报告接收进度的对象</param>
        /// <param name="reportByPercent">是否按照百分比报告进度</param>
        /// <param name="response">是否回发接收数据长度</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        public static byte[] ReadBytesFromSocket(Socket socket, int receive, Action<long, long> report, bool reportByPercent, bool response)
        {
            byte[] bytes_receive = new byte[receive];
            int count_receive = 0;
            long percent = 0;
            while (count_receive < receive)
            {
                // 分割成2KB来接收数据
                int receive_length = (receive - count_receive) >= SocketBufferSize ? SocketBufferSize : (receive - count_receive);
                count_receive += socket.Receive(bytes_receive, count_receive, receive_length, SocketFlags.None);
                if (reportByPercent)
                {
                    long percentCurrent = (long)count_receive * 100 / receive;
                    if (percent != percentCurrent)
                    {
                        percent = percentCurrent;
                        // 报告进度
                        report?.Invoke(count_receive, receive);
                    }
                }
                else
                {
                    // 报告进度
                    report?.Invoke(count_receive, receive);
                }
                // 回发进度
                if (response) socket.Send(BitConverter.GetBytes((long)count_receive));
            }
            return bytes_receive;
        }


        /// <summary>
        /// 从socket套接字读取数据并写入流中，必然报告进度
        /// </summary>
        /// <param name="socket">通信对象</param>
        /// <param name="stream">stream</param>
        /// <param name="receive">接收的长度</param>
        /// <param name="report">用于报告接收进度的对象</param>
        /// <param name="reportByPercent">是否按照百分比报告进度</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        internal static void WriteStreamFromSocket(Socket socket, Stream stream, long receive, Action<long, long> report, bool reportByPercent)
        {
            byte[] buffer = new byte[SocketBufferSize];
            long count_receive = 0;
            long percent = 0;
            while (count_receive < receive)
            {
                // 分割成4KB来接收数据
                int current = socket.Receive(buffer, 0, SocketBufferSize, SocketFlags.None);
                count_receive += current;
                stream.Write(buffer, 0, current);
                if (reportByPercent)
                {
                    long percentCurrent = count_receive * 100 / receive;
                    if (percent != percentCurrent)
                    {
                        percent = percentCurrent;
                        // 报告进度
                        report?.Invoke(count_receive, receive);
                    }
                }
                else
                {
                    // 报告进度
                    report?.Invoke(count_receive, receive);
                }
                // 回发进度
                socket.Send(BitConverter.GetBytes(count_receive));
            }
            buffer = null;
        }




        /// <summary>
        /// 读取流并将数据写入socket
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="socket">连接的套接字</param>
        /// <param name="length">返回的文件长度</param>
        /// <param name="report">发送的进度报告</param>
        /// <param name="reportByPercent"></param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        internal static void WriteSocketFromStream(Socket socket, Stream stream, long length, Action<long, long> report, bool reportByPercent)
        {
            byte[] buffer = new byte[SocketBufferSize];
            long count_send = 0;
            stream.Position = 0;
            long percent = 0;

            while (count_send < length)
            {
                int count = stream.Read(buffer, 0, SocketBufferSize);
                count_send += count;
                socket.Send(buffer, 0, count, SocketFlags.None);

                while (count_send != BitConverter.ToInt64(ReadBytesFromSocket(socket, 8), 0)) ;

                long received = count_send;

                if (reportByPercent)
                {
                    long percentCurrent = received * 100 / length;
                    if (percent != percentCurrent)
                    {
                        percent = percentCurrent;
                        // 报告进度
                        report?.Invoke(received, length);
                    }
                }
                else
                {
                    // 报告进度
                    report?.Invoke(received, length);
                }

                // 双重接收验证
                if (count == 0)
                {
                    break;
                }
            }
            buffer = null;
        }


        /// <summary>
        /// 检查数据是否接收完成并报告进度
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="length">对方需要接收的长度</param>
        /// <param name="report">报告的委托方法</param>
        /// <param name="reportByPercent">是否按照百分比报告进度</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        internal static void CheckSendBytesReceived(Socket socket, long length, Action<long, long> report, bool reportByPercent)
        {
            long remoteNeedReceive = 0;
            long percent = 0;
            // 确认服务器的数据是否接收完成
            while (remoteNeedReceive < length)
            {
                remoteNeedReceive = BitConverter.ToInt64(ReadBytesFromSocket(socket, 8), 0);
                if (reportByPercent)
                {
                    long percentCurrent = remoteNeedReceive * 100 / length;
                    if (percent != percentCurrent)
                    {
                        percent = percentCurrent;
                        // 报告进度
                        report?.Invoke(remoteNeedReceive, length);
                    }
                }
                else
                {
                    // 报告进度
                    report?.Invoke(remoteNeedReceive, length);
                }
            }
        }


        /// <summary>
        /// 生成终极传送指令的方法，所有的数据均通过该方法出来
        /// </summary>
        /// <param name="command">命令头</param>
        /// <param name="customer">自用自定义</param>
        /// <param name="token">令牌</param>
        /// <param name="data">字节数据</param>
        /// <returns></returns>
        public static byte[] CommandBytes(int command, int customer, Guid token, byte[] data)
        {
            byte[] _temp = null;
            int _zipped = HslCommunicationCode.Hsl_Protocol_NoZipped;
            int _sendLength = 0;
            if (data == null)
            {
                _temp = new byte[HslCommunicationCode.HeadByteLength];
            }
            else
            {
                // 加密
                data = HslSecurity.ByteEncrypt(data);
                if (data.Length > 10240)
                {
                    // 10K以上的数据，进行数据压缩
                    data = HslZipped.CompressBytes(data);
                    _zipped = HslCommunicationCode.Hsl_Protocol_Zipped;
                }
                _temp = new byte[HslCommunicationCode.HeadByteLength + data.Length];
                _sendLength = data.Length;
            }
            BitConverter.GetBytes(command).CopyTo(_temp, 0);
            BitConverter.GetBytes(customer).CopyTo(_temp, 4);
            BitConverter.GetBytes(_zipped).CopyTo(_temp, 8);
            token.ToByteArray().CopyTo(_temp, 12);
            BitConverter.GetBytes(_sendLength).CopyTo(_temp, 28);
            if (_sendLength > 0)
            {
                Array.Copy(data, 0, _temp, 32, _sendLength);
            }
            return _temp;
        }


        /// <summary>
        /// 解析接收到数据，先解压缩后进行解密
        /// </summary>
        /// <param name="head"></param>
        /// <param name="content"></param>
        internal static byte[] CommandAnalysis(byte[] head, byte[] content)
        {
            if (content != null)
            {
                int _zipped = BitConverter.ToInt32(head, 8);
                // 先进行解压
                if (_zipped == HslCommunicationCode.Hsl_Protocol_Zipped)
                {
                    content = HslZipped.Decompress(content);
                }
                // 进行解密
                return HslSecurity.ByteDecrypt(content);
            }
            else
            {
                return null;
            }
        }
        

        /// <summary>
        /// 获取发送字节数据的实际数据，带指令头
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="token"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static byte[] CommandBytes(int customer, Guid token, byte[] data)
        {
            return CommandBytes(HslCommunicationCode.Hsl_Protocol_User_Bytes, customer, token, data);
        }


        /// <summary>
        /// 获取发送字节数据的实际数据，带指令头
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="token"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static byte[] CommandBytes(int customer, Guid token, string data)
        {
            if (data == null) return CommandBytes(HslCommunicationCode.Hsl_Protocol_User_String, customer, token, null);
            else return CommandBytes(HslCommunicationCode.Hsl_Protocol_User_String, customer, token, Encoding.Unicode.GetBytes(data));
        }



        /// <summary>
        /// 根据字符串及指令头返回数据信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static byte[] GetBytesFromString(int code, string data)
        {
            int length = string.IsNullOrEmpty(data) ? 0 : Encoding.Unicode.GetBytes(data).Length;
            byte[] result = new byte[length + 8];
            BitConverter.GetBytes(code).CopyTo(result, 0);
            BitConverter.GetBytes(length).CopyTo(result, 4);
            if (length > 0)
            {
                Encoding.Unicode.GetBytes(data).CopyTo(result, 8);
            }
            return result;
        }


    }

    #endregion

    #region 网络通信的基类


    /// <summary>
    /// 一个网络通信类的基础类
    /// </summary>
    public abstract class NetBase
    {
        #region 受保护的属性


        /// <summary>
        /// 用于通信工作的核心对象
        /// </summary>
        internal Socket WorkSocket { get; set; }
        /// <summary>
        /// 分次接收的数据长度
        /// </summary>
        internal int SegmentationLength { get; set; } = 1024;

        /// <summary>
        /// 检查超时的子线程
        /// </summary>
        /// <param name="obj"></param>
        internal void ThreadPoolCheckConnect(object obj)
        {
            if (obj is HslTimeOut timeout)
            {
                NetSupport.ThreadPoolCheckConnect(timeout, ConnectTimeout);
            }
        }

        /// <summary>
        /// 在日志保存时的标记当前调用类的信息
        /// </summary>
        protected string LogHeaderText { get; set; }

        #endregion

        #region 公开的属性


        /// <summary>
        /// 网络访问中的超时时间，单位：毫秒，默认值5000
        /// </summary>
        public int ConnectTimeout { get; set; } = 5000;

        /// <summary>
        /// 当前对象的身份令牌，用来在网络通信中双向认证的依据
        /// </summary>
        public Guid KeyToken { get; set; } = Guid.Empty;



        /// <summary>
        /// 不带参数的委托
        /// </summary>
        public delegate void IEDelegate();
        /// <summary>
        /// 带一个参数的委托
        /// </summary>
        /// <typeparam name="T1">T1</typeparam>
        /// <param name="object1"></param>
        public delegate void IEDelegate<T1>(T1 object1);
        /// <summary>
        /// 带二个参数的委托
        /// </summary>
        /// <typeparam name="T1">T1</typeparam>
        /// <typeparam name="T2">T2</typeparam>
        /// <param name="object1">object1</param>
        /// <param name="object2">object2</param>
        public delegate void IEDelegate<T1, T2>(T1 object1, T2 object2);
        /// <summary>
        /// 带三个参数的委托
        /// </summary>
        /// <typeparam name="T1">T1</typeparam>
        /// <typeparam name="T2">T2</typeparam>
        /// <typeparam name="T3">T3</typeparam>
        /// <param name="object1">object1</param>
        /// <param name="object2">object2</param>
        /// <param name="object3">object3</param>
        public delegate void IEDelegate<T1, T2, T3>(T1 object1, T2 object2, T3 object3);


        /// <summary>
        /// 日志保存类，默认为空
        /// </summary>
        public ILogNet LogNet { get; set; }



        #endregion

        #region 同步数据的七个基本操作

        /****************************************************************************
         * 
         *    1. 创建并连接套接字
         *    2. 接收指定长度的字节数据
         *    3. 发送字节数据到套接字
         *    4. 检查对方是否接收完成
         *    5. 检查头子节令牌是否通过
         *    6. 将文件流写入套接字
         *    7. 从套接字接收文件流
         * 
         ****************************************************************************/




        /// <summary>
        /// 创建socket对象并尝试连接终结点，如果异常，则结束通信
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="iPEndPoint">网络终结点</param>
        /// <param name="result">结果对象</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        protected bool CreateSocketAndConnect(
            out Socket socket,
            IPEndPoint iPEndPoint,
            OperateResult result
            )
        {
            // create the socket object
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var timeout = new HslTimeOut()
            {
                WorkSocket = socket
            };
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolCheckConnect), timeout);

            try
            {
                socket.Connect(iPEndPoint);
                timeout.IsSuccessful = true;
                return true;
            }
            catch (Exception ex)
            {
                result.Message = CombineExceptionString(StringResources.ConnectedFailed, ex.Message);
                LogNet?.WriteError(LogHeaderText, CombineExceptionString(StringResources.ConnectedFailed, ex.Message));
                socket?.Close();
                socket = null;
                return false;
            }
        }


        /// <summary>
        /// 仅仅接收一定长度的字节数据，如果异常，则结束通信
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="bytes">字节数据</param>
        /// <param name="length">长度</param>
        /// <param name="result">结果对象</param>
        /// <param name="receiveStatus">接收状态</param>
        /// <param name="reportByPercent">是否根据百分比报告进度</param>
        /// <param name="response">是否回发进度</param>
        /// <param name="checkTimeOut">是否进行超时检查</param>
        /// <param name="exceptionMessage">假设发生异常，应该携带什么信息</param>
        /// <returns></returns>
        protected bool ReceiveBytesFromSocket(
            Socket socket,
            out byte[] bytes,
            int length,
            OperateResult result,
            Action<long, long> receiveStatus,
            bool reportByPercent,
            bool response,
            bool checkTimeOut,
            string exceptionMessage = null
            )
        {
            //HslTimeOut timeout = null;
            //if (checkTimeOut)
            //{
            //    timeout = new HslTimeOut()
            //    {
            //        WorkSocket = socket,
            //    };
            //    ThreadPool.QueueUserWorkItem(ThreadPoolCheckConnect, timeout);
            //}

            try
            {
                bytes = NetSupport.ReadBytesFromSocket(socket, length, receiveStatus, reportByPercent, response);
                // 指示成功
                // if (checkTimeOut) timeout.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                socket?.Close();
                result.Message = CombineExceptionString(exceptionMessage, ex.Message);
                LogNet?.WriteException(LogHeaderText, exceptionMessage, ex);
                bytes = null; // 这个内存清除的很重要
                return false;
            }
            return true;
        }

        /// <summary>
        /// 仅仅将数据发送到socket对象上去，如果异常，则结束通信
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="send"></param>
        /// <param name="result"></param>
        /// <param name="exceptionMessage"></param>
        /// <returns></returns>
        protected bool SendBytesToSocket(
            Socket socket,
            byte[] send,
            OperateResult result,
            string exceptionMessage = null
            )
        {
            try
            {
                socket.Send(send);
            }
            catch (Exception ex)
            {
                result.Message = CombineExceptionString(exceptionMessage, ex.Message);
                LogNet?.WriteException(LogHeaderText, exceptionMessage, ex);
                socket?.Close();
                send = null;
                return false;
            }
            return true;
        }


        /// <summary>
        /// 确认对方是否已经接收完成数据，如果异常，则结束通信
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="length"></param>
        /// <param name="report"></param>
        /// <param name="result"></param>
        /// <param name="exceptionMessage"></param>
        /// <returns></returns>
        protected bool CheckRomoteReceived(
            Socket socket,
            long length,
            Action<long, long> report,
            OperateResult result,
            string exceptionMessage = null
            )
        {
            try
            {
                NetSupport.CheckSendBytesReceived(socket, length, report, true);
                return true;
            }
            catch (Exception ex)
            {
                result.Message = CombineExceptionString(exceptionMessage, ex.Message);
                LogNet?.WriteException(LogHeaderText, exceptionMessage, ex);
                socket?.Close();
                return false;
            }
        }


        /// <summary>
        /// 检查令牌是否正确，如果不正确，结束网络通信
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="head">头子令</param>
        /// <param name="token">令牌</param>
        /// <param name="result">结果对象</param>
        /// <returns></returns>
        protected bool CheckTokenPermission(
            Socket socket,
            byte[] head,
            Guid token,
            OperateResult result
            )
        {
            if (NetSupport.CheckTokenEquel(head, KeyToken))
            {
                return true;
            }
            else
            {
                result.Message = StringResources.TokenCheckFailed;
                LogNet?.WriteWarn(LogHeaderText, StringResources.TokenCheckFailed + " Ip:" + socket.RemoteEndPoint.AddressFamily.ToString());
                socket?.Close();
                head = null;
                return false;
            }
        }

        /// <summary>
        /// 将文件数据发送至套接字，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="filename"></param>
        /// <param name="filelength"></param>
        /// <param name="result"></param>
        /// <param name="report"></param>
        /// <param name="exceptionMessage"></param>
        /// <returns></returns>
        protected bool SendFileStreamToSocket(
            Socket socket,
            string filename,
            long filelength,
            OperateResult result,
            Action<long, long> report = null,
            string exceptionMessage = null
            )
        {
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    NetSupport.WriteSocketFromStream(socket, fs, filelength, report, true);
                }
                return true;
            }
            catch (Exception ex)
            {
                socket?.Close();
                LogNet?.WriteException(LogHeaderText, exceptionMessage, ex);
                result.Message = CombineExceptionString(exceptionMessage, ex.Message);
                filelength = 0;
                return false;
            }
        }


        /// <summary>
        /// 从套接字中接收一个文件数据，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="filename"></param>
        /// <param name="receive"></param>
        /// <param name="report"></param>
        /// <param name="result"></param>
        /// <param name="exceptionMessage"></param>
        /// <returns></returns>
        protected bool ReceiveFileSteamFromSocket(
            Socket socket,
            string filename,
            long receive,
            Action<long, long> report,
            OperateResult result,
            string exceptionMessage = null
            )
        {
            try
            {
                using(FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    NetSupport.WriteStreamFromSocket(socket, fs, receive, report, true);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                result.Message = CombineExceptionString(exceptionMessage, ex.Message);
                LogNet?.WriteException(LogHeaderText, exceptionMessage, ex);
                socket?.Close();
                return false;
            }
        }



        #endregion

        #region protect method

        /// <summary>
        /// 获取错误的用于显示的信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected string CombineExceptionString(string message, string exception)
        {
            return $"{message + Environment.NewLine}原因：{exception}";
        }

        #endregion
    }

    #endregion
    
    

    #region 双模式客户端基类

    /// <summary>
    /// 双模式客户端基类，允许实现两种模式，短连接和长连接的机制
    /// </summary>
    public abstract class DoubleModeNetBase : NetBase
    {
        #region Constructor

        /// <summary>
        /// 构造函数
        /// </summary>
        public DoubleModeNetBase()
        {
            serverInterfaceLock = new SimpleHybirdLock();
        }

        #endregion

        #region Protect Members

        /// <summary>
        /// 与服务器交互时候的网络锁
        /// </summary>
        protected SimpleHybirdLock serverInterfaceLock;
        /// <summary>
        /// 指示连接模式，false为短连接请求，true为长连接请求
        /// </summary>
        protected bool isSocketInitialization = false;
        /// <summary>
        /// 服务器的连接端口
        /// </summary>
        protected IPEndPoint serverEndPoint = null;

        #endregion

        #region Connect And Close

        /// <summary>
        /// 切换短连接模式到长连接模式，后面的每次请求都共享一个通道
        /// </summary>
        /// <returns>返回连接结果，如果失败的话，包含失败信息</returns>
        public OperateResult ConnectServer()
        {
            isSocketInitialization = true;
            OperateResult result = new OperateResult();

            WorkSocket?.Close();

            if (!CreateSocketAndConnect(out Socket socket, GetIPEndPoint(), result))
            {
                // 创建失败
                socket = null;
            }
            else
            {
                // 创建成功
                WorkSocket = socket;

                // 发送初始化数据
                if (!InitilizationOnConnect(socket, result))
                {
                    // 初始化失败，重新标记连接失败
                    WorkSocket?.Close();
                    LogNet?.WriteDebug(LogHeaderText, "Initializate Connection Failed !");
                }
                else
                {
                    // 初始化成功
                    result.IsSuccess = true;
                    LogNet?.WriteDebug(LogHeaderText, StringResources.NetEngineStart);
                }
            }
            
            return result;
        }




        /// <summary>
        /// 在长连接模式下，断开服务器的连接，并切换到短连接模式
        /// </summary>
        public OperateResult ConnectClose()
        {
            OperateResult result = new OperateResult();
            isSocketInitialization = false;

            // 额外操作
            result.IsSuccess = ExtraOnDisconnect(WorkSocket, result);
            // 关闭信息
            WorkSocket?.Close();
            WorkSocket = null;

            LogNet?.WriteDebug(LogHeaderText, StringResources.NetEngineClose);
            return result;
        }


        #endregion

        #region Read Data From Server


        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="read">结果对象</param>
        /// <returns></returns>
        protected OperateResult<bool> GetBoolResultFromBytes(OperateResult<byte[]> read)
        {
            if (read == null) return null;
            OperateResult<bool> result = new OperateResult<bool>();
            if (read.IsSuccess)
            {
                result.Content = read.Content[0] != 0x00;
            }
            result.IsSuccess = read.IsSuccess;
            result.Message = read.Message;
            result.ErrorCode = read.ErrorCode;
            return result;
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="read">结果对象</param>
        /// <returns></returns>
        protected OperateResult<byte> GetByteResultFromBytes(OperateResult<byte[]> read)
        {
            if (read == null) return null;
            OperateResult<byte> result = new OperateResult<byte>();
            if(read.IsSuccess)
            {
                result.Content = read.Content[0];
            }
            result.IsSuccess = read.IsSuccess;
            result.Message = read.Message;
            result.ErrorCode = read.ErrorCode;
            return result;
        }


        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="read">结果对象</param>
        /// <param name="reverse">是否需要反转结果</param>
        /// <returns></returns>
        protected OperateResult<short> GetInt16ResultFromBytes(OperateResult<byte[]> read, bool reverse = false)
        {
            if (read == null) return null;
            OperateResult<short> result = new OperateResult<short>();
            if (read.IsSuccess)
            {
                if (reverse) Array.Reverse(read.Content);
                result.Content = BitConverter.ToInt16(read.Content, 0);
            }
            result.IsSuccess = read.IsSuccess;
            result.Message = read.Message;
            result.ErrorCode = read.ErrorCode;

            return result;
        }


        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="read">结果对象</param>
        /// <param name="reverse">是否需要反转结果</param>
        /// <returns></returns>
        protected OperateResult<ushort> GetUInt16ResultFromBytes(OperateResult<byte[]> read, bool reverse = false)
        {
            OperateResult<ushort> result = new OperateResult<ushort>();
            if (read.IsSuccess)
            {
                if (reverse) Array.Reverse(read.Content);
                result.Content = BitConverter.ToUInt16(read.Content, 0);
            }
            result.IsSuccess = read.IsSuccess;
            result.Message = read.Message;
            result.ErrorCode = read.ErrorCode;

            return result;
        }


        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="read">结果对象</param>
        /// <param name="reverse">是否需要反转结果</param>
        /// <returns></returns>
        protected OperateResult<int> GetInt32ResultFromBytes(OperateResult<byte[]> read, bool reverse = false)
        {
            OperateResult<int> result = new OperateResult<int>();
            if (read.IsSuccess)
            {
                if (reverse) Array.Reverse(read.Content);
                result.Content = BitConverter.ToInt32(read.Content, 0);
            }
            result.IsSuccess = read.IsSuccess;
            result.Message = read.Message;
            result.ErrorCode = read.ErrorCode;

            return result;
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="read">结果对象</param>
        /// <param name="reverse">是否需要反转结果</param>
        /// <returns></returns>
        protected OperateResult<uint> GetUInt32ResultFromBytes(OperateResult<byte[]> read, bool reverse = false)
        {
            OperateResult<uint> result = new OperateResult<uint>();
            if (read.IsSuccess)
            {
                if (reverse) Array.Reverse(read.Content);
                result.Content = BitConverter.ToUInt32(read.Content, 0);
            }
            result.IsSuccess = read.IsSuccess;
            result.Message = read.Message;
            result.ErrorCode = read.ErrorCode;

            return result;
        }


        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="read">结果对象</param>
        /// <param name="reverse">是否需要反转结果</param>
        /// <returns></returns>
        protected OperateResult<long> GetInt64ResultFromBytes(OperateResult<byte[]> read, bool reverse = false)
        {
            OperateResult<long> result = new OperateResult<long>();
            if (read.IsSuccess)
            {
                if (reverse) Array.Reverse(read.Content);
                result.Content = BitConverter.ToInt64(read.Content, 0);
            }
            result.IsSuccess = read.IsSuccess;
            result.Message = read.Message;
            result.ErrorCode = read.ErrorCode;

            return result;
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="read">结果对象</param>
        /// <param name="reverse">是否需要反转结果</param>
        /// <returns></returns>
        protected OperateResult<ulong> GetUInt64ResultFromBytes(OperateResult<byte[]> read, bool reverse = false)
        {
            OperateResult<ulong> result = new OperateResult<ulong>();
            if (read.IsSuccess)
            {
                if (reverse) Array.Reverse(read.Content);
                result.Content = BitConverter.ToUInt64(read.Content, 0);
            }
            result.IsSuccess = read.IsSuccess;
            result.Message = read.Message;
            result.ErrorCode = read.ErrorCode;

            return result;
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="read">结果对象</param>
        /// <param name="reverse">是否需要反转结果</param>
        /// <returns></returns>
        protected OperateResult<float> GetFloatResultFromBytes(OperateResult<byte[]> read, bool reverse = false)
        {
            OperateResult<float> result = new OperateResult<float>();
            if (read.IsSuccess)
            {
                if (reverse) Array.Reverse(read.Content);
                result.Content = BitConverter.ToSingle(read.Content, 0);
            }
            result.IsSuccess = read.IsSuccess;
            result.Message = read.Message;
            result.ErrorCode = read.ErrorCode;

            return result;
        }


        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="read">结果对象</param>
        /// <param name="reverse">是否需要反转结果</param>
        /// <returns></returns>
        protected OperateResult<double> GetDoubleResultFromBytes(OperateResult<byte[]> read, bool reverse = false)
        {
            OperateResult<double> result = new OperateResult<double>();
            if (read.IsSuccess)
            {
                if (reverse) Array.Reverse(read.Content);
                result.Content = BitConverter.ToDouble(read.Content, 0);
            }
            result.IsSuccess = read.IsSuccess;
            result.Message = read.Message;
            result.ErrorCode = read.ErrorCode;

            return result;
        }

        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="read">结果对象</param>
        /// <returns></returns>
        protected OperateResult<string> GetStringResultFromBytes(OperateResult<byte[]> read)
        {
            OperateResult<string> result = new OperateResult<string>();
            if (read.IsSuccess)
            {
                result.Content = Encoding.ASCII.GetString(read.Content);
            }
            result.IsSuccess = read.IsSuccess;
            result.Message = read.Message;
            result.ErrorCode = read.ErrorCode;

            return result;
        }

        #endregion

        #region Virtual Method


        /// <summary>
        /// 连接上服务器后需要进行的初始化操作
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="result">结果数据对象</param>
        /// <returns></returns>
        protected virtual bool InitilizationOnConnect(Socket socket, OperateResult result)
        {
            return true;
        }

        /// <summary>
        /// 在将要和服务器进行断开的情况下额外的操作
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="result">结果对象</param>
        /// <returns></returns>
        protected virtual bool ExtraOnDisconnect(Socket socket, OperateResult result)
        {
            return true;
        }



        #endregion

        #region Core Connect

        /// <summary>
        /// 获取最终连接的网络点
        /// </summary>
        /// <returns></returns>
        protected virtual IPEndPoint GetIPEndPoint()
        {
            return serverEndPoint;
        }


        /// <summary>
        /// 获取主要工作的网络服务
        /// </summary>
        /// <returns></returns>
        protected Socket GetWorkSocket(out OperateResult connect)
        {
            if (WorkSocket == null || !WorkSocket.Connected)
            {
                connect = ConnectServer();
            }
            else
            {
                connect = new OperateResult()
                {
                    IsSuccess = true,
                };
            }
            return WorkSocket;
        }

        /// <summary>
        /// 根据模式创建新的网络连接，或者时利用旧的网络服务
        /// </summary>
        /// <param name="socket">新生成的网络请求</param>
        /// <param name="result">结果信息</param>
        /// <returns></returns>
        protected bool CreateSocketByDoubleMode(out Socket socket, OperateResult result)
        {
            if (!isSocketInitialization)
            {
                // 短连接模式，重新创建网络连接
                if (!CreateSocketAndConnect(out socket, GetIPEndPoint(), result))
                {
                    socket = null;
                    return false;
                }

                // 连接后初始化操作，如有需要，在派生类中要重写
                if(!InitilizationOnConnect(socket, result))
                {
                    return false;
                }

                return true;
            }
            else
            {
                // 长连接模式，重新利用原先的套接字，如果这个套接字被Close了，会重新连接，如果仍是失败，下次调用重新连接
                socket = GetWorkSocket(out OperateResult connect);
                if (!connect.IsSuccess)
                {
                    result.Message = connect.Message;
                    return false;
                }
                return true;
            }
        }


        /// <summary>
        /// 接收来自服务器的响应数据，该方法需要在派生类中重写
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="response"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool ReceiveResponse(Socket socket, out byte[] response, OperateResult result)
        {
            response = null;
            return true;
        }


        /// <summary>
        /// 发送网数据到网络上去，并接收来自网络的反馈，接收反馈超时时间，5秒钟
        /// </summary>
        /// <param name="send"></param>
        /// <param name="receive"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected bool SendCommandAndReceiveResponse(byte[] send, out byte[] receive, OperateResult result)
        {
            // string str = SoftBasic.ByteToHexString(send, ' ');

            serverInterfaceLock.Enter();                        // 进入读取的锁

            // 创建或重复利用一个网络接收器
            if (!CreateSocketByDoubleMode(out Socket socket, result))
            {
                serverInterfaceLock.Leave();
                receive = null;
                return false;
            }
            
            // 发送数据到网络服务
            if (!SendBytesToSocket(socket, send, result))
            {
                serverInterfaceLock.Leave();
                receive = null;
                return false;
            }

            // 进行10秒接收超时的机制
            HslTimeOut hslTimeOut = new HslTimeOut
            {
                WorkSocket = socket,
                DelayTime = 10000                       // 10秒内必须接收到数据
            };
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolCheckConnect), hslTimeOut);

            if (!ReceiveResponse(socket, out byte[] response, result))
            {
                serverInterfaceLock.Leave();
                receive = null;
                hslTimeOut.IsSuccessful = true;
                return false;
            }
            hslTimeOut.IsSuccessful = true;                     // 退出超时的算法

            receive = response;                                 // 接收到数据
            if (!isSocketInitialization) socket.Close();       // 如果是短连接就关闭连接

            serverInterfaceLock.Leave();                        // 离开读取的锁
            
            return true;
        }


        #endregion

        #region Read Server Core

        /// <summary>
        /// 读写服务器的核心方法，直接发送基础报文，接收服务器的报文返回，可用于测试报文是否正确及二次扩展成自己的API
        /// </summary>
        /// <param name="send">发送的原始字节数据</param>
        /// <returns></returns>
        public OperateResult<byte[]> ReadFromServerCore(byte[] send)
        {
            OperateResult<byte[]> result = new OperateResult<byte[]>();

            if (!SendCommandAndReceiveResponse(send, out byte[] response, result))
            {
                // 失败
                return result;
            }
            else
            {
                // 成功
                result.Content = response;
                result.IsSuccess = true;
                result.Message = "Success";
                return result;
            }
        }


        #endregion

        #region Data Transfer

        /// <summary>
        /// 根据short[]进行转换到byte[]数组
        /// </summary>
        /// <param name="data"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        protected byte[] GetBytesFromArray(short[] data, bool reverse)
        {
            if (data == null) return null;

            byte[] buffer = new byte[data.Length * 2];
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(data[i]);
                if(reverse)
                {
                    buffer[2 * i + 1] = temp[0];
                    buffer[2 * i + 0] = temp[1];
                }
                else
                {
                    buffer[2 * i + 0] = temp[0];
                    buffer[2 * i + 1] = temp[1];
                }
            }

            return buffer;
        }

        /// <summary>
        /// 根据ushort[]进行转换到byte[]数组
        /// </summary>
        /// <param name="data"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        protected byte[] GetBytesFromArray(ushort[] data, bool reverse)
        {
            if (data == null) return null;

            byte[] buffer = new byte[data.Length * 2];
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(data[i]);
                if(reverse)
                {
                    buffer[2 * i + 1] = temp[0];
                    buffer[2 * i + 0] = temp[1];
                }
                else
                {
                    buffer[2 * i + 0] = temp[0];
                    buffer[2 * i + 1] = temp[1];
                }
            }

            return buffer;
        }

        /// <summary>
        /// int[]进行转换到byte[]数组
        /// </summary>
        /// <param name="data"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        protected byte[] GetBytesFromArray(int[] data, bool reverse)
        {
            if (data == null) return null;

            byte[] buffer = new byte[data.Length * 4];
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(data[i]);
                if(reverse)
                {
                    buffer[4 * i + 3] = temp[0];
                    buffer[4 * i + 2] = temp[1];
                    buffer[4 * i + 1] = temp[2];
                    buffer[4 * i + 0] = temp[3];
                }
                else
                {
                    buffer[4 * i + 0] = temp[0];
                    buffer[4 * i + 1] = temp[1];
                    buffer[4 * i + 2] = temp[2];
                    buffer[4 * i + 3] = temp[3];
                }
            }

            return buffer;
        }


        /// <summary>
        /// uint[]进行转换到byte[]数组
        /// </summary>
        /// <param name="data"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        protected byte[] GetBytesFromArray(uint[] data, bool reverse)
        {
            if (data == null) return null;

            byte[] buffer = new byte[data.Length * 4];
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(data[i]);
                if(reverse)
                {
                    buffer[4 * i + 3] = temp[0];
                    buffer[4 * i + 2] = temp[1];
                    buffer[4 * i + 1] = temp[2];
                    buffer[4 * i + 0] = temp[3];
                }
                else
                {
                    buffer[4 * i + 0] = temp[0];
                    buffer[4 * i + 1] = temp[1];
                    buffer[4 * i + 2] = temp[2];
                    buffer[4 * i + 3] = temp[3];
                }
            }

            return buffer;
        }

        /// <summary>
        /// float[]进行转换到byte[]数组
        /// </summary>
        /// <param name="data"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        protected byte[] GetBytesFromArray(float[] data, bool reverse)
        {
            if (data == null) return null;

            byte[] buffer = new byte[data.Length * 4];
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(data[i]);
                if(reverse)
                {
                    buffer[4 * i + 3] = temp[0];
                    buffer[4 * i + 2] = temp[1];
                    buffer[4 * i + 1] = temp[2];
                    buffer[4 * i + 0] = temp[3];
                }
                else
                {
                    buffer[4 * i + 0] = temp[0];
                    buffer[4 * i + 1] = temp[1];
                    buffer[4 * i + 2] = temp[2];
                    buffer[4 * i + 3] = temp[3];
                }
            }

            return buffer;
        }

        /// <summary>
        /// long[]进行转换到byte[]数组
        /// </summary>
        /// <param name="data"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        protected byte[] GetBytesFromArray(long[] data, bool reverse)
        {
            if (data == null) return null;

            byte[] buffer = new byte[data.Length * 8];
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(data[i]);
                if(reverse)
                {
                    buffer[8 * i + 7] = temp[0];
                    buffer[8 * i + 6] = temp[1];
                    buffer[8 * i + 5] = temp[2];
                    buffer[8 * i + 4] = temp[3];
                    buffer[8 * i + 3] = temp[4];
                    buffer[8 * i + 2] = temp[5];
                    buffer[8 * i + 1] = temp[6];
                    buffer[8 * i + 0] = temp[7];
                }
                else
                {
                    buffer[8 * i + 0] = temp[0];
                    buffer[8 * i + 1] = temp[1];
                    buffer[8 * i + 2] = temp[2];
                    buffer[8 * i + 3] = temp[3];
                    buffer[8 * i + 4] = temp[4];
                    buffer[8 * i + 5] = temp[5];
                    buffer[8 * i + 6] = temp[6];
                    buffer[8 * i + 7] = temp[7];
                }
            }

            return buffer;
        }

        /// <summary>
        /// long[]进行转换到byte[]数组
        /// </summary>
        /// <param name="data"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        protected byte[] GetBytesFromArray(ulong[] data, bool reverse)
        {
            if (data == null) return null;

            byte[] buffer = new byte[data.Length * 8];
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(data[i]);
                if(reverse)
                {
                    buffer[8 * i + 7] = temp[0];
                    buffer[8 * i + 6] = temp[1];
                    buffer[8 * i + 5] = temp[2];
                    buffer[8 * i + 4] = temp[3];
                    buffer[8 * i + 3] = temp[4];
                    buffer[8 * i + 2] = temp[5];
                    buffer[8 * i + 1] = temp[6];
                    buffer[8 * i + 0] = temp[7];
                }
                else
                {
                    buffer[8 * i + 0] = temp[0];
                    buffer[8 * i + 1] = temp[1];
                    buffer[8 * i + 2] = temp[2];
                    buffer[8 * i + 3] = temp[3];
                    buffer[8 * i + 4] = temp[4];
                    buffer[8 * i + 5] = temp[5];
                    buffer[8 * i + 6] = temp[6];
                    buffer[8 * i + 7] = temp[7];
                }
            }

            return buffer;
        }

        /// <summary>
        /// long[]进行转换到byte[]数组
        /// </summary>
        /// <param name="data"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        protected byte[] GetBytesFromArray(double[] data, bool reverse)
        {
            if (data == null) return null;

            byte[] buffer = new byte[data.Length * 8];
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(data[i]);
                if(reverse)
                {
                    buffer[8 * i + 7] = temp[0];
                    buffer[8 * i + 6] = temp[1];
                    buffer[8 * i + 5] = temp[2];
                    buffer[8 * i + 4] = temp[3];
                    buffer[8 * i + 3] = temp[4];
                    buffer[8 * i + 2] = temp[5];
                    buffer[8 * i + 1] = temp[6];
                    buffer[8 * i + 0] = temp[7];
                }
                else
                {
                    buffer[8 * i + 0] = temp[0];
                    buffer[8 * i + 1] = temp[1];
                    buffer[8 * i + 2] = temp[2];
                    buffer[8 * i + 3] = temp[3];
                    buffer[8 * i + 4] = temp[4];
                    buffer[8 * i + 5] = temp[5];
                    buffer[8 * i + 6] = temp[6];
                    buffer[8 * i + 7] = temp[7];
                }
            }

            return buffer;
        }


        #endregion
    }

    #endregion

}
