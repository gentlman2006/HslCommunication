using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core.IMessage;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace HslCommunication.Core.Net
{
    /// <summary>
    /// 包含了主动异步接收的方法实现和文件类异步读写的实现
    /// </summary>
    public class NetworkXBase<TNetMessage> : NetworkBase where TNetMessage : INetMessage,new()
    {
        #region Constractor

        public NetworkXBase()
        {
            netMessage = new TNetMessage( );
        }

        #endregion

        #region Private Member

        private TNetMessage netMessage;                             // 消息解析规则

        #endregion


        #region 主动异步接收的方法块



        protected void BeginReceiveMessage()
        {

        }


        #endregion

        private OperateResult<long> ReceiveLong(Socket socket)
        {
            OperateResult<byte[]> read = Receive( socket, 8 );
            if(read.IsSuccess)
            {
                return OperateResult.CreateSuccessResult( BitConverter.ToInt64( read.Content, 8 ) );
            }
            else
            {
                return new OperateResult<long>( )
                {
                    Message = read.Message,
                };
            }
        }

        private OperateResult SendLong( Socket socket, long value )
        {
            return Send( socket, BitConverter.GetBytes( value ) );
        }



        #region StreamWriteRead




        /// <summary>
        /// 发送一个流的所有数据到网络套接字
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="stream"></param>
        /// <param name="report"></param>
        /// <param name="reportByPercent"></param>
        /// <returns></returns>
        protected OperateResult SendStream( Socket socket, Stream stream, long receive, Action<long, long> report, bool reportByPercent )
        {
            byte[] buffer = new byte[1024];
            long SendTotal = 0;
            long percent = 0;
            stream.Position = 0;
            while (SendTotal < receive)
            {
                // 先从流中接收数据
                OperateResult<int> read = ReadStream( stream, buffer );
                if (!read.IsSuccess) return new OperateResult( )
                {
                    Message = read.Message,
                };
                else
                {
                    SendTotal += read.Content;
                }

                // 然后再异步写到socket中
                byte[] newBuffer = new byte[read.Content];
                Array.Copy( buffer, 0, newBuffer, 0, newBuffer.Length );
                OperateResult write = Send( socket, buffer );
                if(!write.IsSuccess)
                {
                    return new OperateResult( )
                    {
                        Message = read.Message,
                    };
                }

                // 确认对方接收
                while (true)
                {
                    OperateResult<long> check = ReceiveLong( socket );
                    if(!check.IsSuccess)
                    {
                        return new OperateResult( )
                        {
                            Message = read.Message,
                        };
                    }

                    if(check.Content == SendTotal)
                    {
                        break;
                    }
                }

                // 报告进度
                if (reportByPercent)
                {
                    long percentCurrent = SendTotal * 100 / receive;
                    if (percent != percentCurrent)
                    {
                        percent = percentCurrent;
                        report?.Invoke( SendTotal, receive );
                    }
                }
                else
                {
                    // 报告进度
                    report?.Invoke( SendTotal, receive );
                }
            }

            return OperateResult.CreateSuccessResult( );
        }


        protected OperateResult WriteStream( Socket socket, Stream stream, long receive, Action<long, long> report, bool reportByPercent )
        {
            byte[] buffer = new byte[1024];
            long count_receive = 0;
            long percent = 0;
            while (count_receive < receive)
            {
                // 先从流中异步接收数据
                int receiveCount = (receive - count_receive) > 1024 ? 1024 : (int)(receive - count_receive);
                OperateResult<byte[]> read = Receive( socket, receiveCount );
                if(!read.IsSuccess)
                {
                    return new OperateResult( )
                    {
                        Message = read.Message,
                    };
                }
                count_receive += receiveCount;
                // 开始写入文件流
                OperateResult write = WriteStream( stream, read.Content );
                if(!write.IsSuccess)
                {
                    return new OperateResult( )
                    {
                        Message = write.Message,
                    };
                }

                // 报告进度
                if (reportByPercent)
                {
                    long percentCurrent = count_receive * 100 / receive;
                    if (percent != percentCurrent)
                    {
                        percent = percentCurrent;
                        report?.Invoke( count_receive, receive );
                    }
                }
                else
                {
                    report?.Invoke( count_receive, receive );
                }

                // 回发进度
                OperateResult check = SendLong(socket, count_receive )
                socket.Send( BitConverter.GetBytes( count_receive ) );
            }
            buffer = null;
        }


        #endregion


    }
}
