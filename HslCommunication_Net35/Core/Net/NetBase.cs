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
        internal static void ThreadPoolCheckConnect( HslTimeOut timeout, int millisecond )
        {
            while (!timeout.IsSuccessful)
            {
                if ((DateTime.Now - timeout.StartTime).TotalMilliseconds > millisecond)
                {
                    // 连接超时或是验证超时
                    if (!timeout.IsSuccessful) timeout.WorkSocket?.Close( );
                    break;
                }
                Thread.Sleep( 100 );
            }
        }

        internal static void ThreadPoolCheckTimeOut( object obj )
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
                            timeout.Operator?.Invoke( );
                            timeout.WorkSocket?.Close( );
                        }
                        break;
                    }
                }
            }
        }
        

        /// <summary>
        /// 判断两个数据的令牌是否相等
        /// </summary>
        /// <param name="head">头指令</param>
        /// <param name="token">令牌</param>
        /// <returns></returns>
        internal static bool CheckTokenEquel( byte[] head, Guid token )
        {
            return IsTwoBytesEquel( head, 12, token.ToByteArray( ), 0, 16 );
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
        internal static bool IsTwoBytesEquel( byte[] b1, int start1, byte[] b2, int start2, int length )
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
        public static byte[] ReadBytesFromSocket( Socket socket, int receive )
        {
            return ReadBytesFromSocket( socket, receive, null, false, false );
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
        public static byte[] ReadBytesFromSocket( Socket socket, int receive, Action<long, long> report, bool reportByPercent, bool response )
        {
            byte[] bytes_receive = new byte[receive];
            int count_receive = 0;
            long percent = 0;
            while (count_receive < receive)
            {
                // 分割成2KB来接收数据
                int receive_length = (receive - count_receive) >= SocketBufferSize ? SocketBufferSize : (receive - count_receive);
                count_receive += socket.Receive( bytes_receive, count_receive, receive_length, SocketFlags.None );
                if (reportByPercent)
                {
                    long percentCurrent = (long)count_receive * 100 / receive;
                    if (percent != percentCurrent)
                    {
                        percent = percentCurrent;
                        // 报告进度
                        report?.Invoke( count_receive, receive );
                    }
                }
                else
                {
                    // 报告进度
                    report?.Invoke( count_receive, receive );
                }
                // 回发进度
                if (response) socket.Send( BitConverter.GetBytes( (long)count_receive ) );
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
        internal static void WriteStreamFromSocket( Socket socket, Stream stream, long receive, Action<long, long> report, bool reportByPercent )
        {
            byte[] buffer = new byte[SocketBufferSize];
            long count_receive = 0;
            long percent = 0;
            while (count_receive < receive)
            {
                // 分割成4KB来接收数据
                int current = socket.Receive( buffer, 0, SocketBufferSize, SocketFlags.None );
                count_receive += current;
                stream.Write( buffer, 0, current );
                if (reportByPercent)
                {
                    long percentCurrent = count_receive * 100 / receive;
                    if (percent != percentCurrent)
                    {
                        percent = percentCurrent;
                        // 报告进度
                        report?.Invoke( count_receive, receive );
                    }
                }
                else
                {
                    // 报告进度
                    report?.Invoke( count_receive, receive );
                }
                // 回发进度
                socket.Send( BitConverter.GetBytes( count_receive ) );
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
        internal static void WriteSocketFromStream( Socket socket, Stream stream, long length, Action<long, long> report, bool reportByPercent )
        {
            byte[] buffer = new byte[SocketBufferSize];
            long count_send = 0;
            stream.Position = 0;
            long percent = 0;

            while (count_send < length)
            {
                int count = stream.Read( buffer, 0, SocketBufferSize );
                count_send += count;
                socket.Send( buffer, 0, count, SocketFlags.None );

                while (count_send != BitConverter.ToInt64( ReadBytesFromSocket( socket, 8 ), 0 )) ;

                long received = count_send;

                if (reportByPercent)
                {
                    long percentCurrent = received * 100 / length;
                    if (percent != percentCurrent)
                    {
                        percent = percentCurrent;
                        // 报告进度
                        report?.Invoke( received, length );
                    }
                }
                else
                {
                    // 报告进度
                    report?.Invoke( received, length );
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
        internal static void CheckSendBytesReceived( Socket socket, long length, Action<long, long> report, bool reportByPercent )
        {
            long remoteNeedReceive = 0;
            long percent = 0;
            // 确认服务器的数据是否接收完成
            while (remoteNeedReceive < length)
            {
                remoteNeedReceive = BitConverter.ToInt64( ReadBytesFromSocket( socket, 8 ), 0 );
                if (reportByPercent)
                {
                    long percentCurrent = remoteNeedReceive * 100 / length;
                    if (percent != percentCurrent)
                    {
                        percent = percentCurrent;
                        // 报告进度
                        report?.Invoke( remoteNeedReceive, length );
                    }
                }
                else
                {
                    // 报告进度
                    report?.Invoke( remoteNeedReceive, length );
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
        public static byte[] CommandBytes( int command, int customer, Guid token, byte[] data )
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
                data = HslSecurity.ByteEncrypt( data );
                if (data.Length > 10240)
                {
                    // 10K以上的数据，进行数据压缩
                    data = HslZipped.CompressBytes( data );
                    _zipped = HslCommunicationCode.Hsl_Protocol_Zipped;
                }
                _temp = new byte[HslCommunicationCode.HeadByteLength + data.Length];
                _sendLength = data.Length;
            }
            BitConverter.GetBytes( command ).CopyTo( _temp, 0 );
            BitConverter.GetBytes( customer ).CopyTo( _temp, 4 );
            BitConverter.GetBytes( _zipped ).CopyTo( _temp, 8 );
            token.ToByteArray( ).CopyTo( _temp, 12 );
            BitConverter.GetBytes( _sendLength ).CopyTo( _temp, 28 );
            if (_sendLength > 0)
            {
                Array.Copy( data, 0, _temp, 32, _sendLength );
            }
            return _temp;
        }


        /// <summary>
        /// 解析接收到数据，先解压缩后进行解密
        /// </summary>
        /// <param name="head"></param>
        /// <param name="content"></param>
        internal static byte[] CommandAnalysis( byte[] head, byte[] content )
        {
            if (content != null)
            {
                int _zipped = BitConverter.ToInt32( head, 8 );
                // 先进行解压
                if (_zipped == HslCommunicationCode.Hsl_Protocol_Zipped)
                {
                    content = HslZipped.Decompress( content );
                }
                // 进行解密
                return HslSecurity.ByteDecrypt( content );
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
        internal static byte[] CommandBytes( int customer, Guid token, byte[] data )
        {
            return CommandBytes( HslCommunicationCode.Hsl_Protocol_User_Bytes, customer, token, data );
        }


        /// <summary>
        /// 获取发送字节数据的实际数据，带指令头
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="token"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static byte[] CommandBytes( int customer, Guid token, string data )
        {
            if (data == null) return CommandBytes( HslCommunicationCode.Hsl_Protocol_User_String, customer, token, null );
            else return CommandBytes( HslCommunicationCode.Hsl_Protocol_User_String, customer, token, Encoding.Unicode.GetBytes( data ) );
        }



        /// <summary>
        /// 根据字符串及指令头返回数据信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static byte[] GetBytesFromString( int code, string data )
        {
            int length = string.IsNullOrEmpty( data ) ? 0 : Encoding.Unicode.GetBytes( data ).Length;
            byte[] result = new byte[length + 8];
            BitConverter.GetBytes( code ).CopyTo( result, 0 );
            BitConverter.GetBytes( length ).CopyTo( result, 4 );
            if (length > 0)
            {
                Encoding.Unicode.GetBytes( data ).CopyTo( result, 8 );
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
        internal void ThreadPoolCheckConnect( object obj )
        {
            if (obj is HslTimeOut timeout)
            {
                NetSupport.ThreadPoolCheckConnect( timeout, ConnectTimeout );
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
        public delegate void IEDelegate( );
        /// <summary>
        /// 带一个参数的委托
        /// </summary>
        /// <typeparam name="T1">T1</typeparam>
        /// <param name="object1"></param>
        public delegate void IEDelegate<T1>( T1 object1 );
        /// <summary>
        /// 带二个参数的委托
        /// </summary>
        /// <typeparam name="T1">T1</typeparam>
        /// <typeparam name="T2">T2</typeparam>
        /// <param name="object1">object1</param>
        /// <param name="object2">object2</param>
        public delegate void IEDelegate<T1, T2>( T1 object1, T2 object2 );
        /// <summary>
        /// 带三个参数的委托
        /// </summary>
        /// <typeparam name="T1">T1</typeparam>
        /// <typeparam name="T2">T2</typeparam>
        /// <typeparam name="T3">T3</typeparam>
        /// <param name="object1">object1</param>
        /// <param name="object2">object2</param>
        /// <param name="object3">object3</param>
        public delegate void IEDelegate<T1, T2, T3>( T1 object1, T2 object2, T3 object3 );


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
            socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            var timeout = new HslTimeOut( )
            {
                WorkSocket = socket
            };
            ThreadPool.QueueUserWorkItem( new WaitCallback( ThreadPoolCheckConnect ), timeout );

            try
            {
                socket.Connect( iPEndPoint );
                timeout.IsSuccessful = true;
                return true;
            }
            catch (Exception ex)
            {
                result.Message = CombineExceptionString( StringResources.ConnectedFailed, ex.Message );
                LogNet?.WriteError( LogHeaderText, CombineExceptionString( StringResources.ConnectedFailed, ex.Message ) );
                socket?.Close( );
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
                bytes = NetSupport.ReadBytesFromSocket( socket, length, receiveStatus, reportByPercent, response );
                // 指示成功
                // if (checkTimeOut) timeout.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                socket?.Close( );
                result.Message = CombineExceptionString( exceptionMessage, ex.Message );
                LogNet?.WriteException( LogHeaderText, exceptionMessage, ex );
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
                socket.Send( send );
            }
            catch (Exception ex)
            {
                result.Message = CombineExceptionString( exceptionMessage, ex.Message );
                LogNet?.WriteException( LogHeaderText, exceptionMessage, ex );
                socket?.Close( );
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
                NetSupport.CheckSendBytesReceived( socket, length, report, true );
                return true;
            }
            catch (Exception ex)
            {
                result.Message = CombineExceptionString( exceptionMessage, ex.Message );
                LogNet?.WriteException( LogHeaderText, exceptionMessage, ex );
                socket?.Close( );
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
            if (NetSupport.CheckTokenEquel( head, KeyToken ))
            {
                return true;
            }
            else
            {
                result.Message = StringResources.TokenCheckFailed;
                try
                {
                    LogNet?.WriteWarn( LogHeaderText, StringResources.TokenCheckFailed + " Ip:" + socket.RemoteEndPoint.AddressFamily.ToString( ) );
                }
                catch
                {
                    // 多半因为socket异常关系导致，不再记录日志
                }
                socket?.Close( );
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
                using (FileStream fs = new FileStream( filename, FileMode.Open, FileAccess.Read ))
                {
                    NetSupport.WriteSocketFromStream( socket, fs, filelength, report, true );
                }
                return true;
            }
            catch (Exception ex)
            {
                socket?.Close( );
                LogNet?.WriteException( LogHeaderText, exceptionMessage, ex );
                result.Message = CombineExceptionString( exceptionMessage, ex.Message );
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
                using (FileStream fs = new FileStream( filename, FileMode.Create, FileAccess.Write ))
                {
                    NetSupport.WriteStreamFromSocket( socket, fs, receive, report, true );
                }

                return true;
            }
            catch (Exception ex)
            {
                result.Message = CombineExceptionString( exceptionMessage, ex.Message );
                LogNet?.WriteException( LogHeaderText, exceptionMessage, ex );
                socket?.Close( );
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
        protected string CombineExceptionString( string message, string exception )
        {
            return $"{message + Environment.NewLine}原因：{exception}";
        }

        #endregion
    }

    #endregion

    #region 服务器客户端共同的基类

    /// <summary>
    /// 客户端服务端都拥有的一些方法
    /// </summary>
    public abstract class NetShareBase : NetBase
    {
        #region 异步发送数据块

        /// <summary>
        /// 发送数据的方法
        /// </summary>
        /// <param name="stateOne">通信用的核心对象</param>
        /// <param name="content"></param>
        internal void SendBytesAsync( AsyncStateOne stateOne, byte[] content )
        {
            if (content == null) return;
            try
            {

                // 启用另外一个套接字传送
                AsyncStateSend state = new AsyncStateSend( )
                {
                    WorkSocket = stateOne.WorkSocket,
                    Content = content,
                    AlreadySendLength = 0,
                    HybirdLockSend = stateOne.HybirdLockSend,
                };

                state.HybirdLockSend.Enter( );

                state.WorkSocket.BeginSend(
                    state.Content,
                    state.AlreadySendLength,
                    state.Content.Length - state.AlreadySendLength,
                    SocketFlags.None,
                    new AsyncCallback( SendCallBack ),
                    state );
            }
            catch (ObjectDisposedException ex)
            {
                // 不操作
                stateOne.HybirdLockSend.Leave( );
            }
            catch (Exception ex)
            {
                stateOne.HybirdLockSend.Leave( );
                if (!ex.Message.Contains( StringResources.SocketRemoteCloseException ))
                {
                    LogNet?.WriteException( LogHeaderText, StringResources.SocketSendException, ex );
                }
            }
        }

        /// <summary>
        /// 发送回发方法
        /// </summary>
        /// <param name="ar"></param>
        internal void SendCallBack( IAsyncResult ar )
        {
            if (ar.AsyncState is AsyncStateSend stateone)
            {
                try
                {
                    stateone.AlreadySendLength += stateone.WorkSocket.EndSend( ar );
                    if (stateone.AlreadySendLength < stateone.Content.Length)
                    {
                        // 继续发送
                        stateone.WorkSocket.BeginSend( stateone.Content,
                        stateone.AlreadySendLength,
                        stateone.Content.Length - stateone.AlreadySendLength,
                        SocketFlags.None, new AsyncCallback( SendCallBack ), stateone );
                    }
                    else
                    {
                        stateone.HybirdLockSend.Leave( );
                        // 发送完成
                        stateone = null;
                    }
                }
                catch (ObjectDisposedException ex)
                {
                    stateone.HybirdLockSend.Leave( );
                    // 不处理
                    stateone = null;
                }
                catch (Exception ex)
                {
                    LogNet?.WriteException( LogHeaderText, StringResources.SocketEndSendException, ex );
                    stateone.HybirdLockSend.Leave( );
                    stateone = null;
                }
            }
        }

        #endregion

        #region 异步数据接收块

        /// <summary>
        /// 重新开始接收下一次的数据传递
        /// </summary>
        /// <param name="receive">网络状态</param>
        /// <param name="isProcess">是否触发数据处理</param>
        internal void ReBeginReceiveHead( AsyncStateOne receive, bool isProcess )
        {
            try
            {
                byte[] head = receive.BytesHead, Content = receive.BytesContent;
                receive.Clear( );
                receive.WorkSocket.BeginReceive( receive.BytesHead, receive.AlreadyReceivedHead, receive.BytesHead.Length - receive.AlreadyReceivedHead,
                    SocketFlags.None, new AsyncCallback( HeadReceiveCallback ), receive );
                // 检测是否需要数据处理
                if (isProcess)
                {
                    // 校验令牌
                    if (NetSupport.CheckTokenEquel( head, KeyToken ))
                    {
                        Content = NetSupport.CommandAnalysis( head, Content );
                        int protocol = BitConverter.ToInt32( head, 0 );
                        int customer = BitConverter.ToInt32( head, 4 );
                        // 转移到数据中心处理
                        DataProcessingCenter( receive, protocol, customer, Content );
                    }
                    else
                    {
                        // 应该关闭网络通信
                        LogNet?.WriteWarn( LogHeaderText, StringResources.TokenCheckFailed );
                    }
                }
            }
            catch(Exception ex)
            {
                SocketReceiveException( receive, ex );
                LogNet?.WriteException( LogHeaderText, ex );
            }
        }

        /// <summary>
        /// 指令头接收方法
        /// </summary>
        /// <param name="ar"></param>
        internal void HeadReceiveCallback( IAsyncResult ar )
        {
            if (ar.AsyncState is AsyncStateOne receive)
            {
                try
                {
                    receive.AlreadyReceivedHead += receive.WorkSocket.EndReceive( ar );
                }
                catch (ObjectDisposedException ex)
                {
                    // 不需要处理
                    return;
                }
                catch (SocketException ex)
                {
                    // 已经断开连接了
                    SocketReceiveException( receive, ex );
                    LogNet?.WriteException( LogHeaderText, ex );
                    return;
                }
                catch (Exception ex)
                {
                    // 其他乱七八糟的异常重新启用接收数据
                    ReBeginReceiveHead( receive, false );
                    LogNet?.WriteException( LogHeaderText, StringResources.SocketEndReceiveException, ex );
                    return;
                }

                if (receive.AlreadyReceivedHead < receive.BytesHead.Length)
                {
                    // 仍需要接收
                    receive.WorkSocket.BeginReceive( receive.BytesHead, receive.AlreadyReceivedHead, receive.BytesHead.Length - receive.AlreadyReceivedHead,
                        SocketFlags.None, new AsyncCallback( HeadReceiveCallback ), receive );
                }
                else
                {
                    // 接收完毕，准备接收内容
                    int receive_length = BitConverter.ToInt32( receive.BytesHead, receive.BytesHead.Length - 4 );
                    receive.BytesContent = new byte[receive_length];

                    if (receive_length > 0)
                    {
                        int receiveSize = (receive.BytesContent.Length - receive.AlreadyReceivedContent) > SegmentationLength ? SegmentationLength : (receive.BytesContent.Length - receive.AlreadyReceivedContent);
                        receive.WorkSocket.BeginReceive( receive.BytesContent, receive.AlreadyReceivedContent, receiveSize,
                            SocketFlags.None, new AsyncCallback( ContentReceiveCallback ), receive );
                    }
                    else
                    {
                        // 处理数据并重新启动接收
                        ReBeginReceiveHead( receive, true );
                    }
                }
            }
        }


        /// <summary>
        /// 接收出错的时候进行处理
        /// </summary>
        /// <param name="receive"></param>
        /// <param name="ex"></param>
        internal virtual void SocketReceiveException( AsyncStateOne receive, Exception ex )
        {

        }


        /// <summary>
        /// 数据内容接收方法
        /// </summary>
        /// <param name="ar"></param>
        internal void ContentReceiveCallback( IAsyncResult ar )
        {
            if (ar.AsyncState is AsyncStateOne receive)
            {
                try
                {
                    receive.AlreadyReceivedContent += receive.WorkSocket.EndReceive( ar );
                }
                catch (ObjectDisposedException ex)
                {
                    //不需要处理
                    return;
                }
                catch (SocketException ex)
                {
                    //已经断开连接了
                    SocketReceiveException( receive, ex );
                    LogNet?.WriteException( LogHeaderText, ex );
                    return;
                }
                catch (Exception ex)
                {
                    //其他乱七八糟的异常重新启用接收数据
                    ReBeginReceiveHead( receive, false );
                    LogNet?.WriteException( LogHeaderText, StringResources.SocketEndReceiveException, ex );
                    return;
                }

                if (receive.AlreadyReceivedContent < receive.BytesContent.Length)
                {
                    int receiveSize = (receive.BytesContent.Length - receive.AlreadyReceivedContent) > SegmentationLength ? SegmentationLength : (receive.BytesContent.Length - receive.AlreadyReceivedContent);
                    //仍需要接收
                    receive.WorkSocket.BeginReceive( receive.BytesContent, receive.AlreadyReceivedContent, receiveSize, SocketFlags.None, new AsyncCallback( ContentReceiveCallback ), receive );
                }
                else
                {
                    //处理数据并重新启动接收
                    ReBeginReceiveHead( receive, true );
                }

            }
        }


        #endregion

        #region 同步方式的高级数据发送

        /// <summary>
        /// [自校验] 发送字节数据并确认对方接收完成数据，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="headcode">头指令</param>
        /// <param name="customer">用户指令</param>
        /// <param name="send">发送的数据</param>
        /// <param name="result">用于返回的结果</param>
        /// <param name="sendReport">发送的进度报告</param>
        /// <param name="failedString">失败时存储的额外描述信息</param>
        /// <returns></returns>
        protected bool SendBaseAndCheckReceive(
            Socket socket,
            int headcode,
            int customer,
            byte[] send,
            OperateResult result,
            Action<long, long> sendReport = null,
            string failedString = null
            )
        {
            // 数据处理
            send = NetSupport.CommandBytes( headcode, customer, KeyToken, send );

            // 发送数据
            if (!SendBytesToSocket(
                socket,                                                // 套接字
                send,                                                  // 发送的字节数据
                result,                                                // 结果信息对象
                failedString                                           // 异常附加对象
                ))
            {
                send = null;
                return false;
            }

            // 确认对方是否接收完成
            int remoteReceive = send.Length - HslCommunicationCode.HeadByteLength;

            if (!CheckRomoteReceived(
                socket,                                                // 套接字
                remoteReceive,                                         // 对方需要接收的长度
                sendReport,                                            // 发送进度报告
                result,                                                // 结果信息对象
                failedString                                           // 异常附加信息
                ))
            {
                send = null;
                return false;
            }

            // 对方接收成功
            send = null;
            return true;
        }

        /// <summary>
        /// [自校验] 发送字节数据并确认对方接收完成数据，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="customer">用户指令</param>
        /// <param name="send">发送的数据</param>
        /// <param name="result">用于返回的结果</param>
        /// <param name="sendReport">发送的进度报告</param>
        /// <param name="failedString">异常时记录到日志的附加信息</param>
        /// <returns></returns>
        protected bool SendBytesAndCheckReceive(
            Socket socket,
            int customer,
            byte[] send,
            OperateResult result,
            Action<long, long> sendReport = null,
            string failedString = null
            )
        {
            if (SendBaseAndCheckReceive(
                socket,                                           // 套接字
                HslCommunicationCode.Hsl_Protocol_User_Bytes,     // 指示字节数组
                customer,                                         // 用户数据
                send,                                             // 发送数据，该数据还要经过处理
                result,                                           // 结果消息对象
                sendReport,                                       // 发送的进度报告
                failedString                                      // 错误的额外描述
                ))
            {
                return true;
            }
            else
            {
                LogNet?.WriteError( LogHeaderText, failedString );
                return false;
            }
        }

        /// <summary>
        /// [自校验] 直接发送字符串数据并确认对方接收完成数据，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="customer">用户指令</param>
        /// <param name="send">发送的数据</param>
        /// <param name="result">用于返回的结果</param>
        /// <param name="sendReport">发送的进度报告</param>
        /// <param name="failedString"></param>
        /// <returns></returns>
        protected bool SendStringAndCheckReceive(
            Socket socket,
            int customer,
            string send,
            OperateResult result,
            Action<long, long> sendReport = null,
            string failedString = null
            )
        {
            byte[] data = string.IsNullOrEmpty( send ) ? null : Encoding.Unicode.GetBytes( send );

            if (!SendBaseAndCheckReceive(
                socket,                                           // 套接字
                HslCommunicationCode.Hsl_Protocol_User_String,    // 指示字符串数据
                customer,                                         // 用户数据
                data,                                             // 字符串的数据
                result,                                           // 结果消息对象
                sendReport,                                       // 发送的进度报告
                failedString                                      // 错误的额外描述
                ))
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// [自校验] 将文件数据发送至套接字，具体发送细节将在继承类中实现，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="filename">文件名称，文件必须存在</param>
        /// <param name="servername">远程端的文件名称</param>
        /// <param name="filetag">文件的额外标签</param>
        /// <param name="fileupload">文件的上传人</param>
        /// <param name="result">操作结果对象</param>
        /// <param name="sendReport">发送进度报告</param>
        /// <param name="failedString"></param>
        /// <returns></returns>
        protected bool SendFileAndCheckReceive(
            Socket socket,
            string filename,
            string servername,
            string filetag,
            string fileupload,
            OperateResult result,
            Action<long, long> sendReport = null,
            string failedString = null
            )
        {
            // 发送文件名，大小，标签
            FileInfo info = new FileInfo( filename );

            if (!File.Exists( filename ))
            {
                // 如果文件不存在
                if (!SendStringAndCheckReceive( socket, 0, "", result, null, failedString )) return false;
                else
                {
                    result.Message = "找不到该文件，请重新确认文件！";
                    socket?.Close( );
                    return false;
                }
            }

            // 文件存在的情况
            Newtonsoft.Json.Linq.JObject json = new Newtonsoft.Json.Linq.JObject
            {
                { "FileName", new Newtonsoft.Json.Linq.JValue(servername) },
                { "FileSize", new Newtonsoft.Json.Linq.JValue(info.Length) },
                { "FileTag", new Newtonsoft.Json.Linq.JValue(filetag) },
                { "FileUpload", new Newtonsoft.Json.Linq.JValue(fileupload) }
            };

            if (!SendStringAndCheckReceive( socket, 1, json.ToString( ), result, null, failedString )) return false;


            if (!SendFileStreamToSocket( socket, filename, info.Length, result, sendReport, failedString ))
            {
                return false;
            }

            // 检查接收
            // if (!CheckRomoteReceived(socket, info.Length, sendReport, result, failedString))
            // {
            //    return false;
            // }

            return true;
        }

        /// <summary>
        /// [自校验] 将流数据发送至套接字，具体发送细节将在继承类中实现，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="stream">文件名称，文件必须存在</param>
        /// <param name="servername">远程端的文件名称</param>
        /// <param name="filetag">文件的额外标签</param>
        /// <param name="fileupload">文件的上传人</param>
        /// <param name="result">操作结果对象</param>
        /// <param name="sendReport">发送进度报告</param>
        /// <param name="failedString"></param>
        /// <returns></returns>
        protected bool SendFileAndCheckReceive(
            Socket socket,
            Stream stream,
            string servername,
            string filetag,
            string fileupload,
            OperateResult result,
            Action<long, long> sendReport = null,
            string failedString = null
            )
        {
            // 文件存在的情况
            Newtonsoft.Json.Linq.JObject json = new Newtonsoft.Json.Linq.JObject
            {
                { "FileName", new Newtonsoft.Json.Linq.JValue(servername) },
                { "FileSize", new Newtonsoft.Json.Linq.JValue(stream.Length) },
                { "FileTag", new Newtonsoft.Json.Linq.JValue(filetag) },
                { "FileUpload", new Newtonsoft.Json.Linq.JValue(fileupload) }
            };

            if (!SendStringAndCheckReceive( socket, 1, json.ToString( ), result, null, failedString )) return false;


            try
            {
                NetSupport.WriteSocketFromStream( socket, stream, stream.Length, sendReport, true );
            }
            catch (Exception ex)
            {
                socket?.Close( );
                LogNet?.WriteException( LogHeaderText, failedString, ex );
                result.Message = CombineExceptionString( failedString, ex.Message );
                return false;
            }

            // 检查接收
            // if (!CheckRomoteReceived(socket, info.Length, sendReport, result, failedString))
            // {
            //    return false;
            // }

            return true;
        }


        #endregion

        #region 同步方式的高级数据接收

        /// <summary>
        /// [自校验] 接收一条完整的同步数据，包含头子节和内容字节，基础的数据，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="head">头子节</param>
        /// <param name="content">内容字节</param>
        /// <param name="result">结果</param>
        /// <param name="receiveReport">接收进度反馈</param>
        /// <param name="failedString">失败时用于显示的字符串</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">result</exception>
        protected bool ReceiveAndCheckBytes(
            Socket socket,
            out byte[] head,
            out byte[] content,
            OperateResult result,
            Action<long, long> receiveReport,
            string failedString = null
            )
        {
            // 30秒超时接收验证
            HslTimeOut hslTimeOut = new HslTimeOut( )
            {
                DelayTime = 30000,
                IsSuccessful = false,
                StartTime = DateTime.Now,
                WorkSocket = socket,
            };

            ThreadPool.QueueUserWorkItem( new WaitCallback( NetSupport.ThreadPoolCheckTimeOut ), hslTimeOut );

            // 接收头
            if (!ReceiveBytesFromSocket(
                socket,                                     // 套接字
                out head,                                   // 接收的头指令
                HslCommunicationCode.HeadByteLength,        // 头指令长度
                result,                                     // 结果消息对象
                null,                                       // 不报告进度
                false,                                      // 报告是否按照百分比报告
                false,                                      // 不回发接收长度
                true,                                       // 检查是否超时
                failedString                                // 异常时的附加文本描述
                ))
            {
                hslTimeOut.IsSuccessful = true;
                content = null;
                return false;
            }

            hslTimeOut.IsSuccessful = true;

            // 检查令牌
            if (!CheckTokenPermission( socket, head, KeyToken, result ))
            {
                content = null;
                return false;
            }

            // 接收内容
            int contentLength = BitConverter.ToInt32( head, 28 );
            if (!ReceiveBytesFromSocket(
                socket,                                     // 套接字
                out content,                                // 内容字节
                contentLength,                              // 内容数据长度
                result,                                     // 结果消息对象
                receiveReport,                              // 接收进度报告委托
                true,                                       // 按照百分比进行报告数据
                true,                                       // 回发已经接收的数据长度
                false,                                      // 不进行超时检查
                failedString                                // 异常时附加的文本描述
                ))
            {
                head = null;
                return false;
            }

            content = NetSupport.CommandAnalysis( head, content );
            return true;
        }



        /// <summary>
        /// [自校验] 从网络中接收一个字符串数据，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="customer">接收的用户数据</param>
        /// <param name="receive">接收的字节数据</param>
        /// <param name="result">结果信息对象</param>
        /// <param name="receiveReport">接收数据时的进度报告</param>
        /// <param name="failedString">失败时记录日志的字符串</param>
        /// <returns></returns>
        protected bool ReceiveStringFromSocket(
            Socket socket,
            out int customer,
            out string receive,
            OperateResult result,
            Action<long, long> receiveReport,
            string failedString = null
            )
        {
            if (!ReceiveAndCheckBytes( socket, out byte[] head, out byte[] content, result, receiveReport, failedString ))
            {
                customer = 0;
                receive = null;
                return false;
            }

            // check
            if (BitConverter.ToInt32( head, 0 ) != HslCommunicationCode.Hsl_Protocol_User_String)
            {
                customer = 0;
                receive = null;
                result.Message = "数据头校验失败！";
                LogNet?.WriteError( LogHeaderText, "数据头校验失败！" );
                socket?.Close( );
                return false;
            }

            // 分析数据
            customer = BitConverter.ToInt32( head, 4 );
            receive = Encoding.Unicode.GetString( content );

            return true;
        }

        /// <summary>
        /// [自校验] 从网络中接收一串字节数据，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="customer">接收的用户数据</param>
        /// <param name="data">接收的字节数据</param>
        /// <param name="result">结果信息对象</param>
        /// <param name="failedString">失败时记录日志的字符串</param>
        /// <returns></returns>
        protected bool ReceiveContentFromSocket(
            Socket socket,
            out int customer,
            out byte[] data,
            OperateResult result,
            string failedString = null
            )
        {
            if (!ReceiveAndCheckBytes( socket, out byte[] head, out byte[] content, result, null, failedString ))
            {
                result = null;
                customer = 0;
                data = null;
                return false;
            }

            // check
            if (BitConverter.ToInt32( head, 0 ) != HslCommunicationCode.Hsl_Protocol_User_Bytes)
            {
                customer = 0;
                data = null;
                result.Message = "数据头校验失败！";
                LogNet?.WriteError( LogHeaderText, "数据头校验失败！" );
                socket?.Close( );
                return false;
            }

            // 分析数据
            customer = BitConverter.ToInt32( head, 4 );
            data = content;

            return true;
        }


        /// <summary>
        /// [自校验] 从套接字中接收文件头信息
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="filename"></param>
        /// <param name="size"></param>
        /// <param name="filetag"></param>
        /// <param name="fileupload"></param>
        /// <param name="result"></param>
        /// <param name="failedString"></param>
        /// <returns></returns>
        protected bool ReceiveFileHeadFromSocket(
            Socket socket,
            out string filename,
            out long size,
            out string filetag,
            out string fileupload,
            OperateResult result,
            string failedString = null
            )
        {
            // 先接收文件头信息
            if (!ReceiveStringFromSocket( socket, out int customer, out string filehead, result, null, failedString ))
            {
                filename = null;
                size = 0;
                filetag = null;
                fileupload = null;
                return false;
            }

            // 判断文件是否存在
            if (customer == 0)
            {
                LogNet?.WriteWarn( LogHeaderText, "对方文件不存在，无法接收！" );
                result.Message = StringResources.FileNotExist;
                filename = null;
                size = 0;
                filetag = null;
                fileupload = null;
                socket?.Close( );
                return false;
            }

            // 提取信息
            Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse( filehead );
            filename = SoftBasic.GetValueFromJsonObject( json, "FileName", "" );
            size = SoftBasic.GetValueFromJsonObject( json, "FileSize", 0L );
            filetag = SoftBasic.GetValueFromJsonObject( json, "FileTag", "" );
            fileupload = SoftBasic.GetValueFromJsonObject( json, "FileUpload", "" );

            return true;
        }


        /// <summary>
        /// [自校验] 从网络中接收一个文件，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="savename">接收文件后保存的文件名</param>
        /// <param name="filename">文件在对方电脑上的文件名</param>
        /// <param name="size">文件大小</param>
        /// <param name="filetag">文件的标识</param>
        /// <param name="fileupload">文件的上传人</param>
        /// <param name="result">结果信息对象</param>
        /// <param name="receiveReport">接收进度报告</param>
        /// <param name="failedString">失败时的记录日志字符串</param>
        /// <returns></returns>
        protected bool ReceiveFileFromSocket(
            Socket socket,
            string savename,
            out string filename,
            out long size,
            out string filetag,
            out string fileupload,
            OperateResult result,
            Action<long, long> receiveReport,
            string failedString = null
            )
        {
            // 先接收文件头信息
            if (!ReceiveFileHeadFromSocket(
                socket,
                out filename,
                out size,
                out filetag,
                out fileupload,
                result,
                failedString
                ))
            {
                return false;
            }

            //// 先接收文件头信息
            //if (!ReceiveStringFromSocket(socket, out int customer, out string filehead, result, null, failedString))
            //{
            //    filename = null;
            //    size = 0;
            //    filetag = null;
            //    fileupload = null;
            //    return false;
            //}

            //// 判断文件是否存在
            //if (customer == 0)
            //{
            //    LogNet?.WriteWarn("对方文件不存在，无法接收！");
            //    result.Message = StringResources.FileNotExist;
            //    filename = null;
            //    size = 0;
            //    filetag = null;
            //    fileupload = null;
            //    socket?.Close();
            //    return false;
            //}

            //// 提取信息
            //Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(filehead);
            //filename = SoftBasic.GetValueFromJsonObject(json, "FileName", "");
            //size = SoftBasic.GetValueFromJsonObject(json, "FileSize", 0L);
            //filetag = SoftBasic.GetValueFromJsonObject(json, "FileTag", "");
            //fileupload = SoftBasic.GetValueFromJsonObject(json, "FileUpload", "");

            // 接收文件消息
            if (!ReceiveFileSteamFromSocket( socket, savename, size, receiveReport, result, failedString ))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// [自校验] 从网络中接收一个文件，写入数据流，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="stream">等待写入的数据流</param>
        /// <param name="filename">文件在对方电脑上的文件名</param>
        /// <param name="size">文件大小</param>
        /// <param name="filetag">文件的标识</param>
        /// <param name="fileupload">文件的上传人</param>
        /// <param name="result">结果信息对象</param>
        /// <param name="receiveReport">接收进度报告</param>
        /// <param name="failedString">失败时的记录日志字符串</param>
        /// <returns></returns>
        protected bool ReceiveFileFromSocket(
            Socket socket,
            Stream stream,
            out string filename,
            out long size,
            out string filetag,
            out string fileupload,
            OperateResult result,
            Action<long, long> receiveReport,
            string failedString = null
            )
        {
            // 先接收文件头信息
            if (!ReceiveFileHeadFromSocket(
                socket,
                out filename,
                out size,
                out filetag,
                out fileupload,
                result,
                failedString
                ))
            {
                return false;
            }


            try
            {
                NetSupport.WriteStreamFromSocket( socket, stream, size, receiveReport, true );
                return true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                socket?.Close( );
                return false;
            }
        }


        #endregion

        #region 删除文件的操作

        /// <summary>
        /// 删除文件的操作
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        protected bool DeleteFileByName( string filename )
        {
            try
            {
                if (!File.Exists( filename )) return true;
                File.Delete( filename );
                return true;
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( LogHeaderText, "delete file failed:" + filename, ex );
                return false;
            }
        }

        #endregion

        #region Protect Method

        /// <summary>
        /// 预处理文件夹的名称，除去文件夹名称最后一个'\'，如果有的话
        /// </summary>
        /// <param name="folder">文件夹名称</param>
        /// <returns></returns>
        protected string PreprocessFolderName( string folder )
        {
            if (folder.EndsWith( @"\" ))
            {
                return folder.Substring( 0, folder.Length - 1 );
            }
            else
            {
                return folder;
            }
        }

        #endregion

        #region 虚方法


        /// <summary>
        /// 数据处理中心，应该继承重写
        /// </summary>
        /// <param name="receive">连接状态</param>
        /// <param name="protocol">协议头</param>
        /// <param name="customer">用户自定义</param>
        /// <param name="content">数据内容</param>
        internal virtual void DataProcessingCenter( AsyncStateOne receive, int protocol, int customer, byte[] content )
        {

        }

        #endregion
    }


    #endregion

    #region 服务器类基类


    /*******************************************************************************
     * 
     *    网络通信类的服务器基础类，提供服务器运行所有相关的方法和功能
     *
     *    Network Communications Server base class provides methods and features related to server running all
     * 
     *******************************************************************************/



    /// <summary>
    /// 所有服务器类对象的基类，添加了一些服务器属性
    /// </summary>
    public class NetServerBase : NetShareBase
    {
        //服务器端的类，提供一个启动的方法，提供两个接收数据的事件
        /// <summary>
        /// 服务器引擎是否启动
        /// </summary>
        public bool IsStarted { get; protected set; } = false;

        /// <summary>
        /// 异步传入的连接申请请求
        /// </summary>
        /// <param name="iar"></param>
        protected void AsyncAcceptCallback( IAsyncResult iar )
        {
            //还原传入的原始套接字
            if (iar.AsyncState is Socket server_socket)
            {
                Socket client = null;
                try
                {
                    // 在原始套接字上调用EndAccept方法，返回新的套接字
                    client = server_socket.EndAccept( iar );
                    ThreadPool.QueueUserWorkItem( new WaitCallback( ThreadPoolLogin ), client );
                }
                catch (ObjectDisposedException ex)
                {
                    // 服务器关闭时候触发的异常，不进行记录
                    return;
                }
                catch (Exception ex)
                {
                    // 有可能刚连接上就断开了，那就不管
                    client?.Close( );
                    LogNet?.WriteException( LogHeaderText, StringResources.SocketAcceptCallbackException, ex );
                }

                // 如果失败，尝试启动三次
                int i = 0;
                while (i < 3)
                {
                    try
                    {
                        server_socket.BeginAccept( new AsyncCallback( AsyncAcceptCallback ), server_socket );
                        break;
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep( 1000 );
                        LogNet?.WriteException( LogHeaderText, StringResources.SocketReAcceptCallbackException, ex );
                        i++;
                    }
                }

                if (i >= 3)
                {
                    LogNet?.WriteError( LogHeaderText, StringResources.SocketReAcceptCallbackException );
                    // 抛出异常，终止应用程序
                    throw new Exception( StringResources.SocketReAcceptCallbackException );
                }
            }
        }

        /// <summary>
        /// 用于登录的回调方法
        /// </summary>
        /// <param name="obj">socket对象</param>
        protected virtual void ThreadPoolLogin( object obj )
        {
            Socket socket = obj as Socket;
            socket?.Close( );
        }


        /// <summary>
        /// 服务器启动时额外的初始化信息
        /// </summary>
        protected virtual void StartInitialization( )
        {

        }


        /// <summary>
        /// 启动服务器的引擎
        /// </summary>
        /// <param name="port">指定一个端口号</param>
        public virtual void ServerStart( int port )
        {
            if (!IsStarted)
            {
                StartInitialization( );

                WorkSocket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
                WorkSocket.Bind( new IPEndPoint( IPAddress.Any, port ) );
                WorkSocket.Listen( 500 );//单次允许最后请求500个，足够小型系统应用了
                WorkSocket.BeginAccept( new AsyncCallback( AsyncAcceptCallback ), WorkSocket );
                IsStarted = true;

                LogNet?.WriteNewLine( );
                LogNet?.WriteInfo( LogHeaderText, StringResources.NetEngineStart );
            }
        }


        /// <summary>
        /// 服务器关闭的时候需要做的事情
        /// </summary>
        protected virtual void CloseAction( )
        {

        }

        /// <summary>
        /// 关闭服务器的引擎
        /// </summary>
        public virtual void ServerClose( )
        {
            if (IsStarted)
            {
                CloseAction( );
                WorkSocket?.Close( );
                IsStarted = false;
                LogNet?.WriteInfo( LogHeaderText, StringResources.NetEngineClose );
            }
        }

    }

    #endregion

    #region 文件服务器基类


    /// <summary>
    /// 文件服务器的基类，提供了对文件的基本操作
    /// </summary>
    public class FileServerBase : NetServerBase
    {
        #region 文件的标记块
        /// <summary>
        /// 所有文件操作的词典锁
        /// </summary>
        internal Dictionary<string, FileMarkId> m_dictionary_files_marks = new Dictionary<string, FileMarkId>( );
        /// <summary>
        /// 词典的锁
        /// </summary>
        private SimpleHybirdLock hybirdLock = new SimpleHybirdLock( );

        /// <summary>
        /// 获取当前文件的读写锁，如果没有会自动创建
        /// </summary>
        /// <param name="filename">完整的文件路径</param>
        /// <returns>读写锁</returns>
        internal FileMarkId GetFileMarksFromDictionaryWithFileName( string filename )
        {
            FileMarkId fileMarkId = null;
            hybirdLock.Enter( );

            // lock operator
            if (m_dictionary_files_marks.ContainsKey( filename ))
            {
                fileMarkId = m_dictionary_files_marks[filename];
            }
            else
            {
                fileMarkId = new FileMarkId( LogNet, filename );
                m_dictionary_files_marks.Add( filename, fileMarkId );
            }

            hybirdLock.Leave( );
            return fileMarkId;
        }

        #endregion

        #region 接收信息头数据

        /// <summary>
        /// 接收本次操作的信息头数据
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="command">命令</param>
        /// <param name="fileName">文件名</param>
        /// <param name="factory">第一大类</param>
        /// <param name="group">第二大类</param>
        /// <param name="id">第三大类</param>
        /// <param name="result">结果对象</param>
        /// <param name="failedString">失败的字符串</param>
        /// <returns></returns>
        protected bool ReceiveInformationHead(
            Socket socket,
            out int command,
            out string fileName,
            out string factory,
            out string group,
            out string id,
            OperateResult result,
            string failedString = null
            )
        {
            // 先接收文件名
            if (!ReceiveStringFromSocket( socket, out command, out fileName, result, null, failedString ))
            {
                factory = null;
                group = null;
                id = null;
                return false;
            }

            // 接收Factory
            if (!ReceiveStringFromSocket( socket, out int command1, out factory, result, null, failedString ))
            {
                group = null;
                id = null;
                return false;
            }

            // 接收Group
            if (!ReceiveStringFromSocket( socket, out int command2, out group, result, null, failedString ))
            {
                id = null;
                return false;
            }

            // 最后接收id
            if (!ReceiveStringFromSocket( socket, out int command3, out id, result, null, failedString ))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取一个随机的文件名，由GUID码和随机数字组成
        /// </summary>
        /// <returns></returns>
        protected string CreateRandomFileName( )
        {
            return Guid.NewGuid( ).ToString( "N" ) + m_random.Next( 1000, 10000 ).ToString( );
        }

        /// <summary>
        /// 返回服务器的绝对路径
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="group"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected string ReturnAbsoluteFilePath( string factory, string group, string id )
        {
            string result = m_FilesDirectoryPath;
            if (!string.IsNullOrEmpty( factory )) result += "\\" + factory;
            if (!string.IsNullOrEmpty( group )) result += "\\" + group;
            if (!string.IsNullOrEmpty( id )) result += "\\" + id;
            return result;
        }


        /// <summary>
        /// 返回服务器的绝对路径
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="group"></param>
        /// <param name="id"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected string ReturnAbsoluteFileName( string factory, string group, string id, string fileName )
        {
            return ReturnAbsoluteFilePath( factory, group, id ) + "\\" + fileName;
        }


        /// <summary>
        /// 返回相对路径的名称
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="group"></param>
        /// <param name="id"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected string ReturnRelativeFileName( string factory, string group, string id, string fileName )
        {
            string result = "";
            if (!string.IsNullOrEmpty( factory )) result += factory + "\\";
            if (!string.IsNullOrEmpty( group )) result += group + "\\";
            if (!string.IsNullOrEmpty( id )) result += id + "\\";
            return result + fileName;
        }


        #endregion

        #region 定时清除标记块

        //private Timer timer;

        //private void ClearDict(object obj)
        //{
        //    hybirdLock.Enter();

        //    List<string> waitRemove = new List<string>();
        //    foreach(var m in m_dictionary_files_marks)
        //    {
        //        if(m.Value.CanClear())
        //        {
        //            waitRemove.Add(m.Key);
        //        }
        //    }

        //    foreach(var m in waitRemove)
        //    {
        //        m_dictionary_files_marks.Remove(m);
        //    }

        //    waitRemove.Clear();
        //    waitRemove = null;

        //    hybirdLock.Leave();
        //}
        #endregion

        #region 临时文件复制块

        /// <summary>
        /// 移动一个文件到新的文件去
        /// </summary>
        /// <param name="fileNameOld"></param>
        /// <param name="fileNameNew"></param>
        /// <returns></returns>
        protected bool MoveFileToNewFile( string fileNameOld, string fileNameNew )
        {
            try
            {
                FileInfo info = new FileInfo( fileNameNew );
                if (!Directory.Exists( info.DirectoryName ))
                {
                    Directory.CreateDirectory( info.DirectoryName );
                }

                if (File.Exists( fileNameNew ))
                {
                    File.Delete( fileNameNew );
                }

                File.Move( fileNameOld, fileNameNew );
                return true;
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( LogHeaderText, "Move a file to new file failed:", ex );
                return false;
            }
        }



        /// <summary>
        /// 删除文件并回发确认信息，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="fullname"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected bool DeleteFileAndCheck(
            Socket socket,
            string fullname,
            OperateResult result
            )
        {
            // 删除文件，如果失败，重复三次
            int customer = 0;
            int times = 0;
            while (times < 3)
            {
                times++;
                if (DeleteFileByName( fullname ))
                {
                    customer = 1;
                    break;
                }
                else
                {
                    Thread.Sleep( 500 );
                }
            }

            // 回发消息
            if (!SendStringAndCheckReceive(
                socket,                                                                // 网络套接字
                customer,                                                              // 是否移动成功
                "成功",                                                                // 字符串数据
                result,                                                                // 结果数据对象
                null,                                                                  // 不进行报告
                "回发删除结果错误" )                                                    // 发送错误时的数据
                )
            {
                return false;
            }

            return true;
        }



        #endregion

        #region File Upload Event

        // 文件的上传事件


        #endregion

        #region Override Method



        /// <summary>
        /// 服务器启动时的操作
        /// </summary>
        protected override void StartInitialization( )
        {
            if (string.IsNullOrEmpty( FilesDirectoryPath ))
            {
                throw new ArgumentNullException( "FilesDirectoryPath", "No saved path is specified" );
            }

            CheckFolderAndCreate( );
            base.StartInitialization( );
        }

        #endregion

        #region Protect Method



        /// <summary>
        /// 检查文件夹是否存在，不存在就创建
        /// </summary>
        protected virtual void CheckFolderAndCreate( )
        {
            if (!Directory.Exists( FilesDirectoryPath ))
            {
                Directory.CreateDirectory( FilesDirectoryPath );
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 文件所存储的路径
        /// </summary>
        public string FilesDirectoryPath
        {
            get { return m_FilesDirectoryPath; }
            set { m_FilesDirectoryPath = PreprocessFolderName( value ); }
        }


        /// <summary>
        /// 获取文件夹的所有文件列表
        /// </summary>
        /// <param name="factory">第一大类</param>
        /// <param name="group">第二大类</param>
        /// <param name="id">第三大类</param>
        /// <returns></returns>
        public virtual string[] GetDirectoryFiles( string factory, string group, string id )
        {
            if (string.IsNullOrEmpty( FilesDirectoryPath )) return new string[0];

            string absolutePath = ReturnAbsoluteFilePath( factory, group, id );

            // 如果文件夹不存在
            if (!Directory.Exists( absolutePath )) return new string[0];
            // 返回文件列表
            return Directory.GetFiles( absolutePath );
        }

        /// <summary>
        /// 获取文件夹的所有文件夹列表
        /// </summary>
        /// <param name="factory">第一大类</param>
        /// <param name="group">第二大类</param>
        /// <param name="id">第三大类</param>
        /// <returns></returns>
        public string[] GetDirectories( string factory, string group, string id )
        {
            if (string.IsNullOrEmpty( FilesDirectoryPath )) return new string[0];

            string absolutePath = ReturnAbsoluteFilePath( factory, group, id );

            // 如果文件夹不存在
            if (!Directory.Exists( absolutePath )) return new string[0];
            // 返回文件列表
            return Directory.GetDirectories( absolutePath );
        }

        #endregion

        #region Private Members

        private string m_FilesDirectoryPath = null;      // 文件的存储路径
        private Random m_random = new Random( );          // 随机生成的文件名

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
        public DoubleModeNetBase( )
        {
            serverInterfaceLock = new SimpleHybirdLock( );
        }

        #endregion

        #region Protect Members
        
        /// <summary>
        /// 服务器的连接端口
        /// </summary>
        protected IPEndPoint serverEndPoint = null;
        

        #endregion

        #region Private Members

        private int receiveBackTimeOut = 10000;                   // 读取数据的时候指示超时检查时间，如果为负数，则不接收服务器的返回
        private bool isCommunicationError = false;                // 指示上次通信的时候是否发生了通信错误
        private SimpleHybirdLock serverInterfaceLock;             // 与服务器交互时候的网络锁
        private bool isSocketInitialization = false;              // 指示连接模式，false为短连接请求，true为长连接请求

        #endregion

        #region Public Members

        /// <summary>
        /// 读取数据的时候指示超时检查时间，默认10秒，如果为负数，则不接收服务器的返回
        /// </summary>
        public int ReceiveBackTimeOut
        {
            get
            {
                return receiveBackTimeOut;
            }
            set
            {
                receiveBackTimeOut = value;
            }
        }

        #endregion

        #region Connect And Close

        /// <summary>
        /// 切换短连接模式到长连接模式，后面的每次请求都共享一个通道
        /// </summary>
        /// <returns>返回连接结果，如果失败的话（也即IsSuccess为False），包含失败信息</returns>
        public OperateResult ConnectServer( )
        {
            isSocketInitialization = true;
            OperateResult result = new OperateResult( );

            // 重新连接之前，先将旧的数据进行清空
            WorkSocket?.Close( );

            if (!CreateSocketAndConnect( out Socket socket, GetIPEndPoint( ), result ))
            {
                // 创建失败
                socket = null;
            }
            else
            {
                // 创建成功
                WorkSocket = socket;

                // 发送初始化数据
                if (!InitilizationOnConnect( socket, result ))
                {
                    // 初始化失败，重新标记连接失败
                    WorkSocket?.Close( );
                    LogNet?.WriteDebug( LogHeaderText, "Initializate Connection Failed !" );
                }
                else
                {
                    // 初始化成功
                    result.IsSuccess = true;
                    LogNet?.WriteDebug( LogHeaderText, StringResources.NetEngineStart );
                }
            }

            return result;
        }




        /// <summary>
        /// 在长连接模式下，断开服务器的连接，并切换到短连接模式
        /// </summary>
        /// <returns>关闭连接，不需要查看IsSuccess属性查看</returns>
        public OperateResult ConnectClose( )
        {
            OperateResult result = new OperateResult( );
            isSocketInitialization = false;

            // 额外操作
            result.IsSuccess = ExtraOnDisconnect( WorkSocket, result );
            // 关闭信息
            WorkSocket?.Close( );
            WorkSocket = null;

            LogNet?.WriteDebug( LogHeaderText, StringResources.NetEngineClose );
            return result;
        }


        #endregion

        #region Operate Result Transfer


        /// <summary>
        /// 将指定的OperateResult类型转化
        /// </summary>
        /// <param name="read">结果对象</param>
        /// <returns></returns>
        protected OperateResult<bool> GetBoolResultFromBytes( OperateResult<byte[]> read )
        {
            if (read == null) return null;
            OperateResult<bool> result = new OperateResult<bool>( );
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
        protected OperateResult<byte> GetByteResultFromBytes( OperateResult<byte[]> read )
        {
            if (read == null) return null;
            OperateResult<byte> result = new OperateResult<byte>( );
            if (read.IsSuccess)
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
        protected OperateResult<short> GetInt16ResultFromBytes( OperateResult<byte[]> read, bool reverse = false )
        {
            if (read == null) return null;
            OperateResult<short> result = new OperateResult<short>( );
            if (read.IsSuccess)
            {
                if (reverse) Array.Reverse( read.Content );
                result.Content = BitConverter.ToInt16( read.Content, 0 );
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
        protected OperateResult<ushort> GetUInt16ResultFromBytes( OperateResult<byte[]> read, bool reverse = false )
        {
            OperateResult<ushort> result = new OperateResult<ushort>( );
            if (read.IsSuccess)
            {
                if (reverse) Array.Reverse( read.Content );
                result.Content = BitConverter.ToUInt16( read.Content, 0 );
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
        protected OperateResult<int> GetInt32ResultFromBytes( OperateResult<byte[]> read, bool reverse = false )
        {
            OperateResult<int> result = new OperateResult<int>( );
            if (read.IsSuccess)
            {
                if (reverse) Array.Reverse( read.Content );
                result.Content = BitConverter.ToInt32( read.Content, 0 );
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
        protected OperateResult<uint> GetUInt32ResultFromBytes( OperateResult<byte[]> read, bool reverse = false )
        {
            OperateResult<uint> result = new OperateResult<uint>( );
            if (read.IsSuccess)
            {
                if (reverse) Array.Reverse( read.Content );
                result.Content = BitConverter.ToUInt32( read.Content, 0 );
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
        protected OperateResult<long> GetInt64ResultFromBytes( OperateResult<byte[]> read, bool reverse = false )
        {
            OperateResult<long> result = new OperateResult<long>( );
            if (read.IsSuccess)
            {
                if (reverse) Array.Reverse( read.Content );
                result.Content = BitConverter.ToInt64( read.Content, 0 );
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
        protected OperateResult<ulong> GetUInt64ResultFromBytes( OperateResult<byte[]> read, bool reverse = false )
        {
            OperateResult<ulong> result = new OperateResult<ulong>( );
            if (read.IsSuccess)
            {
                if (reverse) Array.Reverse( read.Content );
                result.Content = BitConverter.ToUInt64( read.Content, 0 );
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
        protected OperateResult<float> GetFloatResultFromBytes( OperateResult<byte[]> read, bool reverse = false )
        {
            OperateResult<float> result = new OperateResult<float>( );
            if (read.IsSuccess)
            {
                if (reverse) Array.Reverse( read.Content );
                result.Content = BitConverter.ToSingle( read.Content, 0 );
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
        protected OperateResult<double> GetDoubleResultFromBytes( OperateResult<byte[]> read, bool reverse = false )
        {
            OperateResult<double> result = new OperateResult<double>( );
            if (read.IsSuccess)
            {
                if (reverse) Array.Reverse( read.Content );
                result.Content = BitConverter.ToDouble( read.Content, 0 );
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
        protected OperateResult<string> GetStringResultFromBytes( OperateResult<byte[]> read )
        {
            OperateResult<string> result = new OperateResult<string>( );
            if (read.IsSuccess)
            {
                result.Content = Encoding.ASCII.GetString( read.Content );
            }
            result.IsSuccess = read.IsSuccess;
            result.Message = read.Message;
            result.ErrorCode = read.ErrorCode;

            return result;
        }

        #endregion

        #region Extra Ini And Close


        /// <summary>
        /// 连接上服务器后需要进行的初始化操作
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="result">结果数据对象</param>
        /// <returns></returns>
        protected virtual bool InitilizationOnConnect( Socket socket, OperateResult result )
        {
            return true;
        }

        /// <summary>
        /// 在将要和服务器进行断开的情况下额外的操作
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="result">结果对象</param>
        /// <returns></returns>
        protected virtual bool ExtraOnDisconnect( Socket socket, OperateResult result )
        {
            return true;
        }



        #endregion

        #region Core Connect

        /// <summary>
        /// 获取最终连接的网络点
        /// </summary>
        /// <returns></returns>
        protected virtual IPEndPoint GetIPEndPoint( )
        {
            return serverEndPoint;
        }


        /// <summary>
        /// 获取主要工作的网络服务
        /// </summary>
        /// <returns></returns>
        protected Socket GetWorkSocket( out OperateResult connect )
        {
            if (WorkSocket == null || isCommunicationError)
            {
                connect = ConnectServer( );
            }
            else
            {
                connect = new OperateResult( )
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
        protected bool CreateSocketByDoubleMode( out Socket socket, OperateResult result )
        {
            if (!isSocketInitialization)
            {
                // 短连接模式，重新创建网络连接
                if (!CreateSocketAndConnect( out socket, GetIPEndPoint( ), result ))
                {
                    socket = null;
                    return false;
                }

                // 连接后初始化操作，如有需要，在派生类中要重写
                if (!InitilizationOnConnect( socket, result ))
                {
                    return false;
                }

                return true;
            }
            else
            {
                // 长连接模式，重新利用原先的套接字，如果这个套接字被Close了，会重新连接，如果仍是失败，下次调用重新连接
                socket = GetWorkSocket( out OperateResult connect );
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
        protected virtual bool ReceiveResponse( Socket socket, out byte[] response, OperateResult result )
        {
            response = null;
            return true;
        }


        /// <summary>
        /// 发送网数据到网络上去，并接收来自网络的反馈，接收反馈超时时间
        /// </summary>
        /// <param name="send"></param>
        /// <param name="receive"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected bool SendCommandAndReceiveResponse( byte[] send, out byte[] receive, OperateResult result )
        {
            // string str = SoftBasic.ByteToHexString(send, ' ');

            serverInterfaceLock.Enter( );                        // 进入读取的锁

            // 创建或重复利用一个网络接收器
            if (!CreateSocketByDoubleMode( out Socket socket, result ))
            {
                isCommunicationError = true;
                serverInterfaceLock.Leave( );
                receive = null;
                return false;
            }

            // 发送数据到网络服务
            if (!SendBytesToSocket( socket, send, result ))
            {
                isCommunicationError = true;
                serverInterfaceLock.Leave( );
                receive = null;
                return false;
            }

            // 判断是否需要进行接收服务器的返回
            if (ReceiveBackTimeOut >= 0)
            {
                // 进行10秒接收超时的机制
                HslTimeOut hslTimeOut = new HslTimeOut
                {
                    WorkSocket = socket,
                    DelayTime = ReceiveBackTimeOut                                                       // 10秒内必须接收到数据
                };

                if (ReceiveBackTimeOut < int.MaxValue)
                {
                    ThreadPool.QueueUserWorkItem( new WaitCallback( ThreadPoolCheckConnect ), hslTimeOut );
                }

                if (!ReceiveResponse( socket, out byte[] response, result ))
                {
                    isCommunicationError = true;
                    serverInterfaceLock.Leave( );
                    receive = null;
                    hslTimeOut.IsSuccessful = true;
                    return false;
                }
                hslTimeOut.IsSuccessful = true;                                                         // 退出超时的算法
                receive = response;                                                                     // 接收到数据
            }
            else
            {
                receive = null;
            }

            isCommunicationError = false;
            if (!isSocketInitialization) socket.Close( );                                                // 如果是短连接就关闭连接
            serverInterfaceLock.Leave( );                                                                // 离开读取的锁

            return true;
        }


        #endregion

        #region Read Server Core

        /// <summary>
        /// 读写服务器的核心方法，直接发送基础报文，接收服务器的报文返回，可用于测试报文是否正确及二次扩展成自己的API
        /// </summary>
        /// <param name="send">发送的原始字节数据</param>
        /// <returns>读取结果，如果失败，还带有失败信息</returns>
        public OperateResult<byte[]> ReadFromServerCore( byte[] send )
        {
            OperateResult<byte[]> result = new OperateResult<byte[]>( );

            if (!SendCommandAndReceiveResponse( send, out byte[] response, result ))
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
        protected byte[] GetBytesFromArray( short[] data, bool reverse )
        {
            if (data == null) return null;

            byte[] buffer = new byte[data.Length * 2];
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes( data[i] );
                if (reverse)
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
        protected byte[] GetBytesFromArray( ushort[] data, bool reverse )
        {
            if (data == null) return null;

            byte[] buffer = new byte[data.Length * 2];
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes( data[i] );
                if (reverse)
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
        protected byte[] GetBytesFromArray( int[] data, bool reverse )
        {
            if (data == null) return null;

            byte[] buffer = new byte[data.Length * 4];
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes( data[i] );
                if (reverse)
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
        protected byte[] GetBytesFromArray( uint[] data, bool reverse )
        {
            if (data == null) return null;

            byte[] buffer = new byte[data.Length * 4];
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes( data[i] );
                if (reverse)
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
        protected byte[] GetBytesFromArray( float[] data, bool reverse )
        {
            if (data == null) return null;

            byte[] buffer = new byte[data.Length * 4];
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes( data[i] );
                if (reverse)
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
        protected byte[] GetBytesFromArray( long[] data, bool reverse )
        {
            if (data == null) return null;

            byte[] buffer = new byte[data.Length * 8];
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes( data[i] );
                if (reverse)
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
        protected byte[] GetBytesFromArray( ulong[] data, bool reverse )
        {
            if (data == null) return null;

            byte[] buffer = new byte[data.Length * 8];
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes( data[i] );
                if (reverse)
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
        protected byte[] GetBytesFromArray( double[] data, bool reverse )
        {
            if (data == null) return null;

            byte[] buffer = new byte[data.Length * 8];
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes( data[i] );
                if (reverse)
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
