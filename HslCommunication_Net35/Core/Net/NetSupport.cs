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

    }

    #endregion
    
}
