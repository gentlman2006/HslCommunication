using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace HslCommunication.Enthernet.Redis
{
    /// <summary>
    /// 这是一个redis的客户端类，支持读取，写入，发布订阅，但是不支持订阅，如果需要订阅，请使用另一个类
    /// </summary>
    public class RedisClient : NetworkDoubleBase<HslMessage, RegularByteTransform>
    {
        #region Constructor


        /// <summary>
        /// 实例化一个客户端的对象，用于和服务器通信
        /// </summary>
        /// <param name="ipAddress">服务器的ip地址</param>
        /// <param name="port">服务器的端口号</param>
        public RedisClient( string ipAddress, int port )
        {
            IpAddress = ipAddress;
            Port = port;
        }

        /// <summary>
        /// 实例化一个客户端对象，需要手动指定Ip地址和端口
        /// </summary>
        public RedisClient( )
        {

        }

        #endregion

        #region Override

        /// <summary>
        /// 在其他指定的套接字上，使用报文来通讯，传入需要发送的消息，返回一条完整的数据指令
        /// </summary>
        /// <param name="socket">指定的套接字</param>
        /// <param name="send">发送的完整的报文信息</param>
        /// <remarks>
        /// 无锁的基于套接字直接进行叠加协议的操作。
        /// </remarks>
        /// <example>
        /// 假设你有一个自己的socket连接了设备，本组件可以直接基于该socket实现modbus读取，三菱读取，西门子读取等等操作，前提是该服务器支持多协议，虽然这个需求听上去比较变态，但本组件支持这样的操作。
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="ReadFromCoreServerExample1" title="ReadFromCoreServer示例" />
        /// </example>
        /// <returns>接收的完整的报文信息</returns>
        public override OperateResult<byte[]> ReadFromCoreServer( Socket socket, byte[] send )
        {
            OperateResult sendResult = Send( socket, send );
            if (!sendResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( sendResult );

            string tmp = BasicFramework.SoftBasic.ByteToHexString( send, ' ' );
            // 接收超时时间大于0时才允许接收远程的数据
            if (ReceiveTimeOut >= 0)
            {
                // 接收数据信息
                return ReceiveRedisMsg( socket );
            }
            else
            {
                // Not need receive
                return OperateResult.CreateSuccessResult( new byte[0] );
            }
        }

        /// <summary>
        /// 从Redis服务器接收一条数据信息
        /// </summary>
        /// <param name="socket">套接字信息</param>
        /// <returns>接收的结果对象</returns>
        protected OperateResult<byte[]> ReceiveRedisMsg( Socket socket )
        {
            List<byte> bufferArray = new List<byte>( );
            try
            {
                //byte[] buffer1 = new byte[2048];
                //int count1 = socket.Receive( buffer1 );
                //for (int i = 0; i < count1; i++)
                //{
                //    bufferArray.Add( buffer1[i] );
                //}
                //return OperateResult.CreateSuccessResult( bufferArray.ToArray( ) );


                byte[] head = NetSupport.ReadBytesFromSocket( socket, 1 );
                bufferArray.AddRange( head );

                if(head[0] == '-' || head[0] == '+' || head[0] == ':')
                {
                    // 接收到\n为止
                    while (true)
                    {
                        head = NetSupport.ReadBytesFromSocket( socket, 1 );
                        bufferArray.AddRange( head );
                        if (head[0] == '\n') break;
                    }
                }
                else if (head[0] == '$')
                {
                    // 接收到两次\n为止
                    int receiveCount = 0;
                    while (true)
                    {
                        head = NetSupport.ReadBytesFromSocket( socket, 1 );
                        bufferArray.AddRange( head );
                        if (head[0] == '\n')
                        {
                            if(receiveCount == 0)
                            {
                                if (bufferArray[1] == '-') break;
                            }
                            receiveCount++;
                            if (receiveCount == 2) break;
                        }
                    }
                }
                else
                {
                    byte[] buffer = new byte[2048];
                    int count = socket.Receive( buffer );
                    for (int i = 0; i < count; i++)
                    {
                        bufferArray.Add( buffer[i] );
                    }
                }

                return OperateResult.CreateSuccessResult( bufferArray.ToArray( ) );
            }
            catch(Exception ex)
            {
                return new OperateResult<byte[]>( ex.Message );
            }
        }


        #endregion

        #region Command Support

        public byte[] PackCommand( string[] commands )
        {
            StringBuilder sb = new StringBuilder( );
            sb.Append( '*' );
            sb.Append( commands.Length.ToString() );
            sb.Append( "\r\n" );
            for (int i = 0; i < commands.Length; i++)
            {
                sb.Append( '$' );
                sb.Append( Encoding.UTF8.GetBytes(commands[i]).Length.ToString( ) );
                sb.Append( "\r\n" );
                sb.Append( commands[i] );
                sb.Append( "\r\n" );
            }
            return Encoding.UTF8.GetBytes( sb.ToString( ) );
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 针对服务器里的
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数据值</param>
        /// <returns>是否写入成功</returns>
        public OperateResult WriteKey(string key, string value )
        {
            byte[] command = PackCommand( new string[] { "SET", key, value } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return read;

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith("+OK")) return new OperateResult( msg );

            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 读取服务器的键值数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>读取的结果成功对象</returns>
        public OperateResult<string> ReadKey(string key )
        {
            byte[] command = PackCommand( new string[] { "GET", key } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if(!msg.StartsWith("$")) return new OperateResult<string>( msg );

            
            return OperateResult.CreateSuccessResult( msg );
        }

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString( )
        {
            return $"RedisClient[{IpAddress}:{Port}]";
        }

        #endregion
    }
}
