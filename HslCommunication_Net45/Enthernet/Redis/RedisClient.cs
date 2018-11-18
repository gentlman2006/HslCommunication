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
        /// <param name="password">密码，如果服务器没有设置，密码设置为null</param>
        public RedisClient( string ipAddress, int port, string password )
        {
            IpAddress = ipAddress;
            Port = port;
            ReceiveTimeOut = 30000;
            this.password = password;
        }

        /// <summary>
        /// 实例化一个客户端对象，需要手动指定Ip地址和端口
        /// </summary>
        /// <param name="password">密码，如果服务器没有设置，密码设置为null</param>
        public RedisClient( string password )
        {
            ReceiveTimeOut = 30000;
            this.password = password;
        }

        #endregion

        #region Override

        /// <summary>
        /// 如果设置了密码，对密码进行验证
        /// </summary>
        /// <param name="socket">网络的套接字服务</param>
        /// <returns>是否成功的对象</returns>
        protected override OperateResult InitializationOnConnect( Socket socket )
        {
            if(!string.IsNullOrEmpty( this.password ))
            {
                byte[] command = PackCommand( new string[] { "AUTH", this.password } );

                OperateResult<byte[]> read = ReadFromCoreServer( socket, command );
                if (!read.IsSuccess) return read;

                string msg = Encoding.UTF8.GetString( read.Content );
                if (!msg.StartsWith( "+OK" )) return new OperateResult( msg );

                return OperateResult.CreateSuccessResult( );
            }
            return base.InitializationOnConnect( socket );
        }

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
            if (ReceiveTimeOut < 0) return OperateResult.CreateSuccessResult( new byte[0] );

            // 接收数据信息
            return ReceiveCommand( socket );
        }
        

        private OperateResult<byte[]> ReceiveCommandLine(Socket socket )
        {
            List<byte> bufferArray = new List<byte>( );
            try
            {
                // 接收到\n为止
                while (true)
                {
                    byte[] head = NetSupport.ReadBytesFromSocket( socket, 1 );
                    bufferArray.AddRange( head );
                    if (head[0] == '\n') break;
                }

                // 指令头已经接收完成
                return OperateResult.CreateSuccessResult(  bufferArray.ToArray( ) );
            }
            catch(Exception ex)
            {
                return new OperateResult<byte[]>( ex.Message );
            }
        }

        private OperateResult<byte[]> ReceiveCommandString( Socket socket, int length )
        {
            try
            {
                List<byte> bufferArray = new List<byte>( );
                bufferArray.AddRange(NetSupport.ReadBytesFromSocket( socket, length ));
                while (true)
                {
                    byte[] head = NetSupport.ReadBytesFromSocket( socket, 1 );
                    bufferArray.AddRange( head );
                    if (head[0] == '\n') break;
                }

                return OperateResult.CreateSuccessResult( bufferArray.ToArray() );
            }
            catch (Exception ex)
            {
                return new OperateResult<byte[]>( ex.Message );
            }
        }

        private OperateResult<int> GetNumberFromCommandLine( byte[] commandLine )
        {
            try
            {
                string command = Encoding.UTF8.GetString( commandLine ).TrimEnd( '\r', '\n' );
                return OperateResult.CreateSuccessResult( Convert.ToInt32(command.Substring( 1 ) ));
            }
            catch (Exception ex)
            {
                return new OperateResult<int>( ex.Message );
            }
        }
        
        private OperateResult<string> GetStringFromCommandLine( byte[] commandLine )
        {
            try
            {
                if (commandLine[0] != '$') return new OperateResult<string>( Encoding.UTF8.GetString( commandLine ) );

                // 先找到换行符
                int index_start = -1;
                int index_end = -1;
                // 下面的判断兼容windows系统及linux系统
                for (int i = 0; i < commandLine.Length; i++)
                {
                    if(commandLine[i] == '\r' || commandLine[i] == '\n')
                    {
                        index_start = i;
                    }

                    if(commandLine[i] == '\n')
                    {
                        index_end = i;
                        break;
                    }
                }

                int length = Convert.ToInt32( Encoding.UTF8.GetString( commandLine, 1, index_start - 1 ) );
                if (length < 0) return new OperateResult<string>( "(nil) None Value" );

                return OperateResult.CreateSuccessResult( Encoding.UTF8.GetString( commandLine, index_end + 1, length ) );
            }
            catch (Exception ex)
            {
                return new OperateResult<string>( ex.Message );
            }
        }

        private OperateResult<string[]> GetStringsFromCommandLine( byte[] commandLine )
        {
            try
            {
                List<string> lists = new List<string>( );
                if (commandLine[0] != '*') return new OperateResult<string[]>( Encoding.UTF8.GetString( commandLine ) );

                int index = 0;
                for (int i = 0; i < commandLine.Length; i++)
                {
                    if (commandLine[i] == '\r' || commandLine[i] == '\n')
                    {
                        index = i;
                        break;
                    }
                }

                int length = Convert.ToInt32( Encoding.UTF8.GetString( commandLine, 1, index - 1 ) );
                for (int i = 0; i < length; i++)
                {
                    // 提取所有的字符串内容
                    int index_end = -1;
                    for (int j = index; j < commandLine.Length; j++)
                    {
                        if (commandLine[j] == '\n')
                        {
                            index_end = j;
                            break;
                        }
                    }
                    index = index_end + 1;
                    // 寻找子字符串
                    int index_start = -1;
                    for (int j = index; j < commandLine.Length; j++)
                    {
                        if (commandLine[j] == '\r' || commandLine[j] == '\n')
                        {
                            index_start = j;
                            break;
                        }
                    }
                    int stringLength = Convert.ToInt32( Encoding.UTF8.GetString( commandLine, index + 1, index_start - index - 1 ) );
                    for (int j = index; j < commandLine.Length; j++)
                    {
                        if (commandLine[j] == '\n')
                        {
                            index_end = j;
                            break;
                        }
                    }
                    index = index_end + 1;

                    lists.Add( Encoding.UTF8.GetString( commandLine, index, stringLength ) );
                    index = index + stringLength;
                }


                return OperateResult.CreateSuccessResult( lists.ToArray( ) );
            }
            catch (Exception ex)
            {
                return new OperateResult<string[]>( ex.Message );
            }
        }

        /// <summary>
        /// 从网络接收一条redis消息
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <returns>接收的结果对象</returns>
        protected OperateResult<byte[]> ReceiveCommand(Socket socket )
        {
            List<byte> bufferArray = new List<byte>( );
            OperateResult<byte[]> readCommandLine = ReceiveCommandLine( socket );
            if (!readCommandLine.IsSuccess) return readCommandLine;

            bufferArray.AddRange( readCommandLine.Content );
            if (readCommandLine.Content[0] == '+' || readCommandLine.Content[0] == '-' || readCommandLine.Content[0] == ':')
            {
                // 状态回复，错误回复，整数回复
                return OperateResult.CreateSuccessResult( bufferArray.ToArray( ) );
            }
            else if (readCommandLine.Content[0] == '$')
            {
                // 批量回复，允许最大512M字节
                OperateResult<int> lengthResult = GetNumberFromCommandLine( readCommandLine.Content );
                if (!lengthResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( lengthResult );

                if(lengthResult.Content < 0) return OperateResult.CreateSuccessResult( bufferArray.ToArray( ) );

                // 接收字符串信息
                OperateResult<byte[]> receiveContent = ReceiveCommandString( socket, lengthResult.Content );
                if (!receiveContent.IsSuccess) return receiveContent;
                
                bufferArray.AddRange( receiveContent.Content );
                return OperateResult.CreateSuccessResult( bufferArray.ToArray( ) );
            }
            else if (readCommandLine.Content[0] == '*')
            {
                // 多参数的回复
                OperateResult<int> lengthResult = GetNumberFromCommandLine( readCommandLine.Content );
                if (!lengthResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( lengthResult );

                for (int i = 0; i < lengthResult.Content; i++)
                {
                    OperateResult<byte[]> receiveCommand = ReceiveCommand( socket );
                    if (!receiveCommand.IsSuccess) return receiveCommand;

                    bufferArray.AddRange( receiveCommand.Content );
                }

                return OperateResult.CreateSuccessResult( bufferArray.ToArray( ) );
            }
            else
            {
                return new OperateResult<byte[]>( "Not Supported HeadCode: " + readCommandLine.Content[0] );
            }
        }

        #endregion

        #region Command Support

        private byte[] PackCommand( string[] commands )
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

        #region Key Write Read

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
            
            return GetStringFromCommandLine( read.Content );
        }


        /// <summary>
        /// 自定义的指令交互方法，该指令用空格分割，举例：LTRIM AAAAA 0 999 就是收缩列表，GET AAA 就是获取键值，需要对返回的数据进行二次分析
        /// </summary>
        /// <param name="command">举例：LTRIM AAAAA 0 999 就是收缩列表，GET AAA 就是获取键值</param>
        /// <returns>从服务器返回的结果数据对象</returns>
        public OperateResult<string> ReadCustomer( string command )
        {
            byte[] byteCommand = PackCommand( command.Split( ' ' ));

            OperateResult<byte[]> read = ReadFromCoreServer( byteCommand );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );
            
            return OperateResult.CreateSuccessResult( Encoding.UTF8.GetString( read.Content ) );
        }

        #endregion

        #region List Operate

        /// <summary>
        /// 获取列表的数据长度
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>是否成功的数据长度对象</returns>
        public OperateResult<int> GetListLength(string key )
        {
            byte[] command = PackCommand( new string[] { "LLEN", key } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return GetNumberFromCommandLine( read.Content );
        }
        
        /// <summary>
        /// 获取数组的制定的索引的位置
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="index">索引位置</param>
        /// <returns>结果数据对象</returns>
        public OperateResult<string> ReadListByIndex( string key, long index )
        {
            byte[] command = PackCommand( new string[] { "LINDEX", key, index.ToString( ) } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return GetStringFromCommandLine( read.Content );
        }

        /// <summary>
        /// 移除并返回列表 key 的头元素。
        /// </summary>
        /// <param name="key">关键字信息</param>
        /// <returns>结果数据对象</returns>
        public OperateResult<string> ListLeftPop( string key )
        {
            byte[] command = PackCommand( new string[] { "LPOP", key } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return GetStringFromCommandLine( read.Content );
        }
        
        /// <summary>
        /// 数组的左侧插入一个数据，如果数组不存在，就创建一个数组
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <returns>是否插入数据成功</returns>
        public OperateResult ListLeftPush( string key, string value )
        {
            byte[] command = PackCommand( new string[] { "LPUSH", key, value } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return read;

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult( msg );

            return OperateResult.CreateSuccessResult( );
        }
        
        /// <summary>
        /// 数组的左侧插入一个数据，如果数组不存在，就返回失败
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <returns>是否插入数据成功</returns>
        public OperateResult ListLeftPushX( string key, string value )
        {
            byte[] command = PackCommand( new string[] { "LPUSHX", key, value } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return read;

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( "+OK" )) return new OperateResult( msg );

            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 获取数组数据里指定范围的数据信息
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="start">开始的索引</param>
        /// <param name="end">结束的索引</param>
        /// <returns>结果数组对象</returns>
        public OperateResult<string[]> ListRange(string key, long start, long end )
        {
            byte[] command = PackCommand( new string[] { "LRANGE", key, start.ToString( ), end.ToString( ) } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string[]>( read );

            return GetStringsFromCommandLine( read.Content );
        }

        /// <summary>
        /// 设置数组的某一个索引的数据信息，当 index 参数超出范围，或对一个空列表( key 不存在)进行 LSET 时，返回一个错误。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="index">索引位置</param>
        /// <param name="value">值</param>
        /// <returns>是否插入数据成功</returns>
        public OperateResult ListSet( string key, long index, string value )
        {
            byte[] command = PackCommand( new string[] { "LSET", key.ToString( ), index.ToString( ), value } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return read;

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( "+OK" )) return new OperateResult( msg );

            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 对一个列表进行修剪(trim)，就是说，让列表只保留指定区间内的元素，不在指定区间之内的元素都将被删除。
        /// 举个例子，执行命令 LTRIM list 0 2 ，表示只保留列表 list 的前三个元素，其余元素全部删除。
        /// 下标( index)参数 start 和 stop 都以 0 为底，也就是说，以 0 表示列表的第一个元素，以 1 表示列表的第二个元素，以此类推。
        /// 你也可以使用负数下标，以 -1 表示列表的最后一个元素， -2 表示列表的倒数第二个元素，以此类推。
        /// 当 key 不是列表类型时，返回一个错误。
        /// </summary>
        /// <param name="key">关键字信息</param>
        /// <param name="start">起始的索引信息</param>
        /// <param name="end">结束的索引信息</param>
        /// <returns>是否成功的结果对象</returns>
        public OperateResult ListTrim( string key, long start, long end )
        {
            byte[] command = PackCommand( new string[] { "LTRIM", key, start.ToString( ), end.ToString( ) } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return read;

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( "+OK" )) return new OperateResult( msg );

            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 移除并返回列表 key 的尾部元素。
        /// </summary>
        /// <param name="key">关键字信息</param>
        /// <returns>结果数据对象</returns>
        public OperateResult<string> ListRightPop( string key )
        {
            byte[] command = PackCommand( new string[] { "RPOP", key } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return GetStringFromCommandLine( read.Content );
        }

        /// <summary>
        /// 命令 RPOPLPUSH 在一个原子时间内，执行以下两个动作：
        /// 1. 将列表 source 中的最后一个元素( 尾元素)弹出，并返回给客户端。
        /// 2. 将 source 弹出的元素插入到列表 destination ，作为 destination 列表的的头元素。
        /// 举个例子，你有两个列表 source 和 destination ， source 列表有元素 a, b, c ， destination 列表有元素 x, y, z ，执行 RPOPLPUSH source destination 之后， source 列表包含元素 a, b ， destination 列表包含元素 c, x, y, z ，并且元素 c 会被返回给客户端。
        /// 如果 source 不存在，值 nil 被返回，并且不执行其他动作。
        /// 如果 source 和 destination 相同，则列表中的表尾元素被移动到表头，并返回该元素，可以把这种特殊情况视作列表的旋转( rotation)操作。
        /// </summary>
        /// <param name="key1">第一个关键字</param>
        /// <param name="key2">第二个关键字</param>
        /// <returns>返回的移除的对象</returns>
        public OperateResult<string> ListRightPopLeftPush( string key1, string key2 )
        {
            byte[] command = PackCommand( new string[] { "RPOPLPUSH", key1, key2 } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return GetStringFromCommandLine( read.Content );
        }

        /// <summary>
        /// 将一个或多个值 value 插入到列表 key 的表尾(最右边)。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <returns>是否插入数据成功</returns>
        public OperateResult ListRightPush( string key, string value )
        {
            byte[] command = PackCommand( new string[] { "RPUSH", key, value } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return read;

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( "+OK" )) return new OperateResult( msg );

            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 数组的右侧插入一个数据，如果数组不存在，就返回失败
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <returns>是否插入数据成功</returns>
        public OperateResult ListRightPushX( string key, string value )
        {
            byte[] command = PackCommand( new string[] { "RPUSHX", key, value } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return read;

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( "+OK" )) return new OperateResult( msg );

            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 将信息 message 发送到指定的频道 channel
        /// </summary>
        /// <param name="channel">频道，和关键字不是一回事</param>
        /// <param name="message">消息</param>
        /// <returns>是否发送成功的结果对象</returns>
        public OperateResult<int> Publish(string channel, string message )
        {
            byte[] command = PackCommand( new string[] { "PUBLISH", channel, message } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );
            
            if (read.Content[0] != ':') return new OperateResult<int>( Encoding.UTF8.GetString( read.Content ) );

            return GetNumberFromCommandLine( read.Content );

        }

        #endregion

        #region Private Member

        private string password = string.Empty;                 // 密码信息

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
