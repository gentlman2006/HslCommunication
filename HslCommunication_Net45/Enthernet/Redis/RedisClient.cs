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
            if (!string.IsNullOrEmpty( this.password ))
            {
                byte[] command = RedisHelper.PackStringCommand( new string[] { "AUTH", this.password } );

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
            return RedisHelper.ReceiveCommand( socket );
        }
        
        #endregion

        #region Customer

        /// <summary>
        /// 自定义的指令交互方法，该指令用空格分割，举例：LTRIM AAAAA 0 999 就是收缩列表，GET AAA 就是获取键值，需要对返回的数据进行二次分析
        /// </summary>
        /// <param name="command">举例：LTRIM AAAAA 0 999 就是收缩列表，GET AAA 就是获取键值</param>
        /// <returns>从服务器返回的结果数据对象</returns>
        public OperateResult<string> ReadCustomer( string command )
        {
            byte[] byteCommand = RedisHelper.PackStringCommand( command.Split( ' ' ) );

            OperateResult<byte[]> read = ReadFromCoreServer( byteCommand );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return OperateResult.CreateSuccessResult( Encoding.UTF8.GetString( read.Content ) );
        }

        #endregion

        #region Key Operate

        /// <summary>
        /// 删除给定的一个或多个 key 。不存在的 key 会被忽略。
        /// </summary>
        /// <param name="keys">关键字</param>
        /// <returns>被删除 key 的数量。</returns>
        public OperateResult<int> DeleteKey( string[] keys )
        {
            List<string> list = new List<string>( );
            list.Add( "DEL" );
            list.AddRange( keys );

            byte[] command = RedisHelper.PackStringCommand( list.ToArray( ) );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 删除给定的一个或多个 key 。不存在的 key 会被忽略。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>被删除 key 的数量。</returns>
        public OperateResult<int> DeleteKey( string key )
        {
            return DeleteKey( new string[] { key } );
        }

        /// <summary>
        /// 检查给定 key 是否存在。若 key 存在，返回 1 ，否则返回 0 。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>若 key 存在，返回 1 ，否则返回 0 。</returns>
        public OperateResult<int> ExistsKey( string key )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "EXISTS", key } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 为给定 key 设置生存时间，当 key 过期时(生存时间为 0 )，它会被自动删除。设置成功返回 1 。当 key 不存在或者不能为 key 设置生存时间时，返回 0 。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>
        /// 设置成功返回 1 。当 key 不存在或者不能为 key 设置生存时间时，返回 0 。
        /// </returns>
        public OperateResult<int> ExpireKey( string key )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "EXPIRE", key } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 查找所有符合给定模式 pattern 的 key 。
        /// * 匹配数据库中所有 key。
        /// h?llo 匹配 hello ， hallo 和 hxllo 等。
        /// h[ae]llo 匹配 hello 和 hallo ，但不匹配 hillo 。
        /// </summary>
        /// <param name="pattern">给定模式</param>
        /// <returns>符合给定模式的 key 列表。</returns>
        public OperateResult<string[]> ReadAllKeys( string pattern )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "KEYS", pattern } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string[]>( read );

            return RedisHelper.GetStringsFromCommandLine( read.Content );
        }

        /// <summary>
        /// 将当前数据库的 key 移动到给定的数据库 db 当中。
        /// 如果当前数据库(源数据库)和给定数据库(目标数据库)有相同名字的给定 key ，或者 key 不存在于当前数据库，那么 MOVE 没有任何效果。
        /// 因此，也可以利用这一特性，将 MOVE 当作锁(locking)原语(primitive)。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="db">数据块</param>
        /// <returns>是否移动成功</returns>
        public OperateResult MoveKey( string key, int db )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "MOVE", key, db.ToString( ) } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return read;

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( "+OK" )) return new OperateResult( msg );

            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 移除给定 key 的生存时间，将这个 key 从『易失的』(带生存时间 key )转换成『持久的』(一个不带生存时间、永不过期的 key )。
        /// 当生存时间移除成功时，返回 1 .
        /// 如果 key 不存在或 key 没有设置生存时间，返回 0 。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>
        /// 当生存时间移除成功时，返回 1 .
        /// 如果 key 不存在或 key 没有设置生存时间，返回 0 。
        /// </returns>
        public OperateResult<int> PersistKey( string key )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "PERSIST", key } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 从当前数据库中随机返回(不删除)一个 key 。
        /// 当数据库不为空时，返回一个 key 。
        /// 当数据库为空时，返回 nil 。
        /// </summary>
        /// <returns>
        /// 当数据库不为空时，返回一个 key 。
        /// 当数据库为空时，返回 nil 。
        /// </returns>
        public OperateResult<string> ReadRandomKey( )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "RANDOMKEY" } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return RedisHelper.GetStringFromCommandLine( read.Content );
        }

        /// <summary>
        /// 将 key 改名为 newkey 。
        /// 当 key 和 newkey 相同，或者 key 不存在时，返回一个错误。
        /// 当 newkey 已经存在时， RENAME 命令将覆盖旧值。
        /// </summary>
        /// <param name="key1">旧的key</param>
        /// <param name="key2">新的key</param>
        /// <returns>
        /// 改名成功时提示 OK ，失败时候返回一个错误。
        /// </returns>
        public OperateResult RenameKey( string key1, string key2 )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "RENAME", key1, key2 } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return read;

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( "+OK" )) return new OperateResult( msg );

            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 返回 key 所储存的值的类型。none (key不存在)，string (字符串)，list (列表)，set (集合)，zset (有序集)，hash (哈希表)
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>类型</returns>
        public OperateResult<string> ReadKeyType( string key )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "TYPE", key } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( "+" )) return new OperateResult<string>( msg );

            return OperateResult.CreateSuccessResult( msg.Substring( 1 ).TrimEnd( '\r', '\n' ) );
        }



        #endregion

        #region String Operate

        /// <summary>
        /// 如果 key 已经存在并且是一个字符串， APPEND 命令将 value 追加到 key 原来的值的末尾。
        /// 如果 key 不存在， APPEND 就简单地将给定 key 设为 value ，就像执行 SET key value 一样。
        /// 返回追加 value 之后， key 中字符串的长度。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数值</param>
        /// <returns>
        /// 追加 value 之后， key 中字符串的长度。
        /// </returns>
        public OperateResult<int> AppendKey( string key, string value )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "APPEND", key, value } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 将 key 中储存的数字值减一。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 DECR 操作。
        /// 如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
        /// 本操作的值限制在 64 位(bit)有符号数字表示之内。
        /// 返回执行 DECR 命令之后 key 的值。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>执行 DECR 命令之后 key 的值。</returns>
        public OperateResult<int> DecrementKey( string key )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "DECR", key } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 将 key 所储存的值减去减量 decrement 。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 DECR 操作。
        /// 如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
        /// 本操作的值限制在 64 位(bit)有符号数字表示之内。
        /// 返回减去 decrement 之后， key 的值。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">操作的值</param>
        /// <returns>返回减去 decrement 之后， key 的值。</returns>
        public OperateResult<int> DecrementKey( string key, long value )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "DECR", key, value.ToString( ) } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 返回 key 所关联的字符串值。如果 key 不存在那么返回特殊值 nil 。
        /// 假如 key 储存的值不是字符串类型，返回一个错误，因为 GET 只能用于处理字符串值。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>当 key 不存在时，返回 nil ，否则，返回 key 的值。</returns>
        public OperateResult<string> ReadKey( string key )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "GET", key } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return RedisHelper.GetStringFromCommandLine( read.Content );
        }

        /// <summary>
        /// 返回 key 中字符串值的子字符串，字符串的截取范围由 start 和 end 两个偏移量决定(包括 start 和 end 在内)。
        /// 负数偏移量表示从字符串最后开始计数， -1 表示最后一个字符， -2 表示倒数第二个，以此类推。
        /// 返回截取得出的子字符串。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="start">截取开始的位置</param>
        /// <param name="end">截取结束的位置</param>
        /// <returns>返回截取得出的子字符串。</returns>
        public OperateResult<string> ReadKeyRange( string key, int start, int end )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "GETRANGE", key, start.ToString( ), end.ToString( ) } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return RedisHelper.GetStringFromCommandLine( read.Content );
        }

        /// <summary>
        /// 将给定 key 的值设为 value ，并返回 key 的旧值(old value)。当 key 存在但不是字符串类型时，返回一个错误。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">新的值</param>
        /// <returns>返回给定 key 的旧值。当 key 没有旧值时，也即是， key 不存在时，返回 nil 。</returns>
        public OperateResult<string> ReadAndWriteKey( string key, string value )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "GETSET", key, value } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return RedisHelper.GetStringFromCommandLine( read.Content );
        }

        /// <summary>
        /// 将 key 中储存的数字值增一。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 INCR 操作。
        /// 如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
        /// 返回执行 INCR 命令之后 key 的值。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>返回执行 INCR 命令之后 key 的值。</returns>
        public OperateResult<int> IncrementKey( string key )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "INCR", key } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 将 key 所储存的值加上增量 increment 。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 INCR 操作。
        /// 如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">增量数据</param>
        /// <returns>加上 increment 之后， key 的值。</returns>
        public OperateResult<int> IncrementKey( string key, long value )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "INCRBY", key, value.ToString( ) } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 将 key 所储存的值加上增量 increment 。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 INCR 操作。
        /// 如果命令执行成功，那么 key 的值会被更新为（执行加法之后的）新值，并且新值会以字符串的形式返回给调用者
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">增量数据</param>
        /// <returns>执行命令之后 key 的值。</returns>
        public OperateResult<string> IncrementKey( string key, float value )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "INCRBYFLOAT", key, value.ToString( ) } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return RedisHelper.GetStringFromCommandLine( read.Content );
        }


        /// <summary>
        /// 返回所有(一个或多个)给定 key 的值。
        /// 如果给定的 key 里面，有某个 key 不存在，那么这个 key 返回特殊值 null 。因此，该命令永不失败。
        /// </summary>
        /// <param name="keys">关键字数组</param>
        /// <returns>一个包含所有给定 key 的值的列表。</returns>
        public OperateResult<string[]> ReadKey( string[] keys )
        {
            List<string> list = new List<string>( );
            list.Add( "MGET" );
            list.AddRange( keys );
            byte[] command = RedisHelper.PackStringCommand( list.ToArray( ) );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string[]>( read );

            return RedisHelper.GetStringsFromCommandLine( read.Content );
        }

        /// <summary>
        /// 同时设置一个或多个 key-value 对。
        /// 如果某个给定 key 已经存在，那么 MSET 会用新值覆盖原来的旧值，如果这不是你所希望的效果，请考虑使用 MSETNX 命令：它只会在所有给定 key 都不存在的情况下进行设置操作。
        /// </summary>
        /// <param name="keys">关键字数组</param>
        /// <param name="values">值数组</param>
        /// <returns>总是返回 OK (因为 MSET 不可能失败)</returns>
        public OperateResult WriteKey( string[] keys, string[] values )
        {
            if (keys == null) throw new ArgumentNullException( "keys" );
            if (values == null) throw new ArgumentNullException( "values" );
            if (keys.Length != values.Length) throw new ArgumentException( "Two arguement not same length" );

            List<string> list = new List<string>( );
            list.Add( "MSET" );
            for (int i = 0; i < keys.Length; i++)
            {
                list.Add( keys[i] );
                list.Add( values[i] );
            }

            byte[] command = RedisHelper.PackStringCommand( list.ToArray( ) );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return read;

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( "+OK" )) return new OperateResult( msg );

            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 将字符串值 value 关联到 key 。
        /// 如果 key 已经持有其他值， SET 就覆写旧值，无视类型。
        /// 对于某个原本带有生存时间（TTL）的键来说， 当 SET 命令成功在这个键上执行时， 这个键原有的 TTL 将被清除。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数据值</param>
        /// <returns> SET 在设置操作成功完成时，才返回 OK 。</returns>
        public OperateResult WriteKey( string key, string value )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "SET", key, value } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return read;

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( "+OK" )) return new OperateResult( msg );

            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 将值 value 关联到 key ，并将 key 的生存时间设为 seconds (以秒为单位)。如果 key 已经存在， SETEX 命令将覆写旧值。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数值</param>
        /// <param name="seconds">生存时间，单位秒</param>
        /// <returns>设置成功时返回 OK 。当 seconds 参数不合法时，返回一个错误。</returns>
        public OperateResult WriteExpireKey( string key, string value, long seconds )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "SETEX", key, seconds.ToString( ), value } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return read;

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( "+OK" )) return new OperateResult( msg );

            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 将 key 的值设为 value ，当且仅当 key 不存在。若给定的 key 已经存在，则 SETNX 不做任何动作。设置成功，返回 1 。设置失败，返回 0 。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数据值</param>
        /// <returns>设置成功，返回 1 。设置失败，返回 0 。</returns>
        public OperateResult<int> WriteKeyIfNotExists( string key, string value )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "SETNX", key, value.ToString( ) } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 用 value 参数覆写(overwrite)给定 key 所储存的字符串值，从偏移量 offset 开始。不存在的 key 当作空白字符串处理。返回被 SETRANGE 修改之后，字符串的长度。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数值</param>
        /// <param name="offset">起始的偏移量</param>
        /// <returns>被 SETRANGE 修改之后，字符串的长度。</returns>
        public OperateResult<int> WriteKeyRange( string key, string value, int offset )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "SETRANGE", key, offset.ToString( ), value.ToString( ) } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 返回 key 所储存的字符串值的长度。当 key 储存的不是字符串值时，返回一个错误。返回符串值的长度。当 key 不存在时，返回 0 。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>字符串值的长度。当 key 不存在时，返回 0 。</returns>
        public OperateResult<int> ReadKeyLength( string key )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "STRLEN", key } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        #endregion

        #region List Operate

        /// <summary>
        /// 将值 value 插入到列表 key 当中，位于值 pivot 之前。
        /// 当 pivot 不存在于列表 key 时，不执行任何操作。
        /// 当 key 不存在时， key 被视为空列表，不执行任何操作。
        /// 如果 key 不是列表类型，返回一个错误。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数值</param>
        /// <param name="pivot">原先的值</param>
        /// <returns>
        /// 如果命令执行成功，返回插入操作完成之后，列表的长度。
        /// 如果没有找到 pivot ，返回 -1 。
        /// 如果 key 不存在或为空列表，返回 0 。
        /// </returns>
        public OperateResult<int> ListInsertBefore( string key, string value, string pivot )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "LINSERT", key, "BEFORE", pivot, value } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 将值 value 插入到列表 key 当中，位于值 pivot 之后。
        /// 当 pivot 不存在于列表 key 时，不执行任何操作。
        /// 当 key 不存在时， key 被视为空列表，不执行任何操作。
        /// 如果 key 不是列表类型，返回一个错误。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数值</param>
        /// <param name="pivot">原先的值</param>
        /// <returns>
        /// 如果命令执行成功，返回插入操作完成之后，列表的长度。
        /// 如果没有找到 pivot ，返回 -1 。
        /// 如果 key 不存在或为空列表，返回 0 。
        /// </returns>
        public OperateResult<int> ListInsertAfter( string key, string value, string pivot )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "LINSERT", key, "AFTER", pivot, value } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 返回列表 key 的长度。如果 key 不存在，则 key 被解释为一个空列表，返回 0 .如果 key 不是列表类型，返回一个错误。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>列表 key 的长度。</returns>
        public OperateResult<int> GetListLength( string key )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "LLEN", key } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 返回列表 key 中，下标为 index 的元素。下标(index)参数 start 和 stop 都以 0 为底，也就是说，以 0 表示列表的第一个元素，以 1 表示列表的第二个元素，以此类推。
        /// 你也可以使用负数下标，以 -1 表示列表的最后一个元素， -2 表示列表的倒数第二个元素，以此类推。如果 key 不是列表类型，返回一个错误。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="index">索引位置</param>
        /// <returns>列表中下标为 index 的元素。如果 index 参数的值不在列表的区间范围内(out of range)，返回 nil 。</returns>
        public OperateResult<string> ReadListByIndex( string key, long index )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "LINDEX", key, index.ToString( ) } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return RedisHelper.GetStringFromCommandLine( read.Content );
        }

        /// <summary>
        /// 移除并返回列表 key 的头元素。列表的头元素。当 key 不存在时，返回 nil 。
        /// </summary>
        /// <param name="key">关键字信息</param>
        /// <returns>列表的头元素。</returns>
        public OperateResult<string> ListLeftPop( string key )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "LPOP", key } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return RedisHelper.GetStringFromCommandLine( read.Content );
        }

        /// <summary>
        /// 将一个或多个值 value 插入到列表 key 的表头，如果 key 不存在，一个空列表会被创建并执行 LPUSH 操作。当 key 存在但不是列表类型时，返回一个错误。返回执行 LPUSH 命令后，列表的长度。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <returns>执行 LPUSH 命令后，列表的长度。</returns>
        public OperateResult<int> ListLeftPush( string key, string value )
        {
            return ListLeftPush( key, new string[] { value } );
        }

        /// <summary>
        /// 将一个或多个值 value 插入到列表 key 的表头，如果 key 不存在，一个空列表会被创建并执行 LPUSH 操作。当 key 存在但不是列表类型时，返回一个错误。返回执行 LPUSH 命令后，列表的长度。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="values">值</param>
        /// <returns>执行 LPUSH 命令后，列表的长度。</returns>
        public OperateResult<int> ListLeftPush( string key, string[] values )
        {
            List<string> list = new List<string>( );
            list.Add( "LPUSH" );
            list.Add( key );
            list.AddRange( values );

            byte[] command = RedisHelper.PackStringCommand( list.ToArray( ) );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 将值 value 插入到列表 key 的表头，当且仅当 key 存在并且是一个列表。和 LPUSH 命令相反，当 key 不存在时， LPUSHX 命令什么也不做。
        /// 返回LPUSHX 命令执行之后，表的长度。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <returns>是否插入数据成功</returns>
        public OperateResult ListLeftPushX( string key, string value )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "LPUSHX", key, value } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return read;

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( "+OK" )) return new OperateResult( msg );

            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 返回列表 key 中指定区间内的元素，区间以偏移量 start 和 stop 指定。
        /// 下标(index)参数 start 和 stop 都以 0 为底，也就是说，以 0 表示列表的第一个元素，以 1 表示列表的第二个元素，以此类推。
        /// 你也可以使用负数下标，以 -1 表示列表的最后一个元素， -2 表示列表的倒数第二个元素，以此类推。
        /// 返回一个列表，包含指定区间内的元素。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="start">开始的索引</param>
        /// <param name="stop">结束的索引</param>
        /// <returns>返回一个列表，包含指定区间内的元素。</returns>
        public OperateResult<string[]> ListRange( string key, long start, long stop )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "LRANGE", key, start.ToString( ), stop.ToString( ) } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string[]>( read );

            return RedisHelper.GetStringsFromCommandLine( read.Content );
        }

        /// <summary>
        /// 根据参数 count 的值，移除列表中与参数 value 相等的元素。count 的值可以是以下几种：
        /// count > 0 : 从表头开始向表尾搜索，移除与 value 相等的元素，数量为 count 。
        /// count &lt; 0 : 从表尾开始向表头搜索，移除与 value 相等的元素，数量为 count 的绝对值。
        /// count = 0 : 移除表中所有与 value 相等的值。
        /// 返回被移除的数量。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="count">移除参数</param>
        /// <param name="value">匹配的值</param>
        /// <returns>被移除元素的数量。因为不存在的 key 被视作空表(empty list)，所以当 key 不存在时， LREM 命令总是返回 0 。</returns>
        public OperateResult<int> ListRemoveElementMatch( string key, long count, string value )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "LREM", key, count.ToString( ), value } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 设置数组的某一个索引的数据信息，当 index 参数超出范围，或对一个空列表( key 不存在)进行 LSET 时，返回一个错误。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="index">索引位置</param>
        /// <param name="value">值</param>
        /// <returns>操作成功返回 ok ，否则返回错误信息。</returns>
        public OperateResult ListSet( string key, long index, string value )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "LSET", key.ToString( ), index.ToString( ), value } );

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
            byte[] command = RedisHelper.PackStringCommand( new string[] { "LTRIM", key, start.ToString( ), end.ToString( ) } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return read;

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( "+OK" )) return new OperateResult( msg );

            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 移除并返回列表 key 的尾元素。当 key 不存在时，返回 nil 。
        /// </summary>
        /// <param name="key">关键字信息</param>
        /// <returns>列表的尾元素。</returns>
        public OperateResult<string> ListRightPop( string key )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "RPOP", key } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return RedisHelper.GetStringFromCommandLine( read.Content );
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
            byte[] command = RedisHelper.PackStringCommand( new string[] { "RPOPLPUSH", key1, key2 } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return RedisHelper.GetStringFromCommandLine( read.Content );
        }

        /// <summary>
        /// 将一个或多个值 value 插入到列表 key 的表尾(最右边)。
        /// 如果 key 不存在，一个空列表会被创建并执行 RPUSH 操作。当 key 存在但不是列表类型时，返回一个错误。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <returns>返回执行 RPUSH 操作后，表的长度。</returns>
        public OperateResult ListRightPush( string key, string value )
        {
            return ListRightPush( key, new string[] { value } );
        }

        /// <summary>
        /// 将一个或多个值 value 插入到列表 key 的表尾(最右边)。
        /// 如果有多个 value 值，那么各个 value 值按从左到右的顺序依次插入到表尾：比如对一个空列表 mylist 执行 RPUSH mylist a b c ，得出的结果列表为 a b c ，
        /// 如果 key 不存在，一个空列表会被创建并执行 RPUSH 操作。当 key 存在但不是列表类型时，返回一个错误。
        /// 返回执行 RPUSH 操作后，表的长度。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="values">值</param>
        /// <returns>返回执行 RPUSH 操作后，表的长度。</returns>
        public OperateResult ListRightPush( string key, string[] values )
        {
            List<string> list = new List<string>( );
            list.Add( "RPUSH" );
            list.Add( key );
            list.AddRange( values );

            byte[] command = RedisHelper.PackStringCommand( list.ToArray( ) );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return read;

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 将值 value 插入到列表 key 的表尾，当且仅当 key 存在并且是一个列表。
        /// 和 RPUSH 命令相反，当 key 不存在时， RPUSHX 命令什么也不做。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <returns>RPUSHX 命令执行之后，表的长度。</returns>
        public OperateResult<int> ListRightPushX( string key, string value )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "RPUSHX", key, value } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        #endregion

        #region Hash Operate

        /// <summary>
        /// 删除哈希表 key 中的一个或多个指定域，不存在的域将被忽略。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="field">域</param>
        /// <returns>被成功移除的域的数量，不包括被忽略的域。</returns>
        public OperateResult<int> DeleteHashKey( string key, string field )
        {
            return DeleteHashKey( key, new string[] { field } );
        }

        /// <summary>
        /// 删除哈希表 key 中的一个或多个指定域，不存在的域将被忽略。返回被成功移除的域的数量，不包括被忽略的域。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="fields">所有的域</param>
        /// <returns>返回被成功移除的域的数量，不包括被忽略的域。</returns>
        public OperateResult<int> DeleteHashKey( string key, string[] fields )
        {
            List<string> list = new List<string>( );
            list.Add( "HDEL" );
            list.Add( key );
            list.AddRange( fields );

            byte[] command = RedisHelper.PackStringCommand( list.ToArray( ) );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 查看哈希表 key 中，给定域 field 是否存在。如果哈希表含有给定域，返回 1 。
        /// 如果哈希表不含有给定域，或 key 不存在，返回 0 。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="field">域</param>
        /// <returns>如果哈希表含有给定域，返回 1 。如果哈希表不含有给定域，或 key 不存在，返回 0 。</returns>
        public OperateResult<int> ExistsHashKey( string key, string field )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "HEXISTS", key, field } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 返回哈希表 key 中给定域 field 的值。当给定域不存在或是给定 key 不存在时，返回 nil 
        /// </summary>
        /// <param name="key">关键值</param>
        /// <param name="field">域</param>
        /// <returns>
        /// 给定域的值。
        /// 当给定域不存在或是给定 key 不存在时，返回 nil 。
        /// </returns>
        public OperateResult<string> ReadHashKey( string key, string field )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "HGET", key, field } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return RedisHelper.GetStringFromCommandLine( read.Content );
        }

        /// <summary>
        /// 返回哈希表 key 中，所有的域和值。在返回值里，紧跟每个域名(field name)之后是域的值(value)，所以返回值的长度是哈希表大小的两倍。
        /// </summary>
        /// <param name="key">关键值</param>
        /// <returns>
        /// 以列表形式返回哈希表的域和域的值。
        /// 若 key 不存在，返回空列表。
        /// </returns>
        public OperateResult<string[]> ReadHashKeyAll( string key )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "HGETALL", key } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string[]>( read );

            return RedisHelper.GetStringsFromCommandLine( read.Content );
        }

        /// <summary>
        /// 为哈希表 key 中的域 field 的值加上增量 increment 。增量也可以为负数，相当于对给定域进行减法操作。
        /// 如果 key 不存在，一个新的哈希表被创建并执行 HINCRBY 命令。返回执行 HINCRBY 命令之后，哈希表 key 中域 field 的值。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="field">域</param>
        /// <param name="value">增量值</param>
        /// <returns>返回执行 HINCRBY 命令之后，哈希表 key 中域 field 的值。</returns>
        public OperateResult<int> IncrementHashKey( string key, string field, long value )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "HINCRBY", key, field, value.ToString( ) } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 为哈希表 key 中的域 field 的值加上增量 increment 。增量也可以为负数，相当于对给定域进行减法操作。
        /// 如果 key 不存在，一个新的哈希表被创建并执行 HINCRBY 命令。返回执行 HINCRBY 命令之后，哈希表 key 中域 field 的值。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="field">域</param>
        /// <param name="value">增量值</param>
        /// <returns>返回执行 HINCRBY 命令之后，哈希表 key 中域 field 的值。</returns>
        public OperateResult<int> IncrementHashKey( string key, string field, float value )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "HINCRBYFLOAT", key, field, value.ToString( ) } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 返回哈希表 key 中的所有域。当 key 不存在时，返回一个空表。
        /// </summary>
        /// <param name="key">关键值</param>
        /// <returns>
        /// 一个包含哈希表中所有域的表。
        /// 当 key 不存在时，返回一个空表。
        /// </returns>
        public OperateResult<string[]> ReadHashKeys( string key )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "HKEYS", key } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string[]>( read );

            return RedisHelper.GetStringsFromCommandLine( read.Content );
        }

        /// <summary>
        /// 删除哈希表 key 中的一个或多个指定域，不存在的域将被忽略。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>被成功移除的域的数量，不包括被忽略的域。</returns>
        public OperateResult<int> ReadHashKeyLength( string key )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "HLEN", key } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 返回哈希表 key 中，一个或多个给定域的值。如果给定的域不存在于哈希表，那么返回一个 nil 值。因为不存在的 key 被当作一个空哈希表来处理，所以对一个不存在的 key 进行 HMGET 操作将返回一个只带有 nil 值的表。
        /// </summary>
        /// <param name="key">关键值</param>
        /// <param name="fields">指定的域</param>
        /// <returns>
        /// 一个包含多个给定域的关联值的表，表值的排列顺序和给定域参数的请求顺序一样。
        /// </returns>
        public OperateResult<string[]> ReadHashKey( string key, string[] fields )
        {
            List<string> list = new List<string>( );
            list.Add( "HMGET" );
            list.Add( key );
            list.AddRange( fields );

            byte[] command = RedisHelper.PackStringCommand( list.ToArray( ) );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string[]>( read );

            return RedisHelper.GetStringsFromCommandLine( read.Content );
        }

        /// <summary>
        /// 将哈希表 key 中的域 field 的值设为 value 。
        /// 如果 key 不存在，一个新的哈希表被创建并进行 HSET 操作。
        /// 如果域 field 已经存在于哈希表中，旧值将被覆盖。
        /// 如果 field 是哈希表中的一个新建域，并且值设置成功，返回 1 。
        /// 如果哈希表中域 field 已经存在且旧值已被新值覆盖，返回 0 。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="field">域</param>
        /// <param name="value">数据值</param>
        /// <returns>
        /// 如果 field 是哈希表中的一个新建域，并且值设置成功，返回 1 。
        /// 如果哈希表中域 field 已经存在且旧值已被新值覆盖，返回 0 。
        /// </returns>
        public OperateResult<int> WriteHashKey( string key, string field, string value )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "HSET", key, field, value } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 同时将多个 field-value (域-值)对设置到哈希表 key 中。
        /// 此命令会覆盖哈希表中已存在的域。
        /// 如果 key 不存在，一个空哈希表被创建并执行 HMSET 操作。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="fields">域</param>
        /// <param name="values">数据值</param>
        /// <returns>
        /// 如果命令执行成功，返回 OK 。
        /// 当 key 不是哈希表(hash)类型时，返回一个错误
        /// </returns>
        public OperateResult WriteHashKey( string key, string[] fields, string[] values )
        {
            if (fields == null) throw new ArgumentNullException( "fields" );
            if (values == null) throw new ArgumentNullException( "values" );
            if (fields.Length != values.Length) throw new ArgumentException( "Two arguement not same length" );

            List<string> list = new List<string>( );
            list.Add( "HMSET" );
            list.Add( key );
            for (int i = 0; i < fields.Length; i++)
            {
                list.Add( fields[i] );
                list.Add( values[i] );
            }

            byte[] command = RedisHelper.PackStringCommand( list.ToArray( ) );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return read;

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( "+OK" )) return new OperateResult( msg );

            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 将哈希表 key 中的域 field 的值设置为 value ，当且仅当域 field 不存在。若域 field 已经存在，该操作无效。
        /// 设置成功，返回 1 。如果给定域已经存在且没有操作被执行，返回 0 。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="field">域</param>
        /// <param name="value">数据值</param>
        /// <returns>设置成功，返回 1 。如果给定域已经存在且没有操作被执行，返回 0 。</returns>
        public OperateResult<int> WriteHashKeyNx( string key, string field, string value )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "HSETNX", key, field, value } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( ":" )) return new OperateResult<int>( msg );

            return RedisHelper.GetNumberFromCommandLine( read.Content );
        }

        /// <summary>
        /// 返回哈希表 key 中的所有域。当 key 不存在时，返回一个空表。
        /// </summary>
        /// <param name="key">关键值</param>
        /// <returns>
        /// 一个包含哈希表中所有域的表。
        /// 当 key 不存在时，返回一个空表。
        /// </returns>
        public OperateResult<string[]> ReadHashValues( string key )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "HVALS", key } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string[]>( read );

            return RedisHelper.GetStringsFromCommandLine( read.Content );
        }



        #endregion

        #region Server Operate

        /// <summary>
        /// SAVE 命令执行一个同步保存操作，将当前 Redis 实例的所有数据快照(snapshot)以 RDB 文件的形式保存到硬盘。
        /// </summary>
        /// <returns>保存成功时返回 OK 。</returns>
        public OperateResult Save( )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "SAVE" } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return read;

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( "+OK" )) return new OperateResult( msg );

            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 在后台异步(Asynchronously)保存当前数据库的数据到磁盘。
        /// BGSAVE 命令执行之后立即返回 OK ，然后 Redis fork 出一个新子进程，原来的 Redis 进程(父进程)继续处理客户端请求，而子进程则负责将数据保存到磁盘，然后退出。
        /// </summary>
        /// <returns>反馈信息。</returns>
        public OperateResult SaveAsync( )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "SAVE" } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return read;

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( "+OK" )) return new OperateResult( msg );

            return OperateResult.CreateSuccessResult( );
        }

        #endregion

        #region Publish

        /// <summary>
        /// 将信息 message 发送到指定的频道 channel
        /// </summary>
        /// <param name="channel">频道，和关键字不是一回事</param>
        /// <param name="message">消息</param>
        /// <returns>是否发送成功的结果对象</returns>
        public OperateResult<int> Publish( string channel, string message )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "PUBLISH", channel, message } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int>( read );

            if (read.Content[0] != ':') return new OperateResult<int>( Encoding.UTF8.GetString( read.Content ) );

            return RedisHelper.GetNumberFromCommandLine( read.Content );

        }

        #endregion

        #region DB Block

        /// <summary>
        /// 切换到指定的数据库，数据库索引号 index 用数字值指定，以 0 作为起始索引值。默认使用 0 号数据库。
        /// </summary>
        /// <param name="db">索引值</param>
        /// <returns>是否切换成功</returns>
        public OperateResult SelectDB( int db )
        {
            byte[] command = RedisHelper.PackStringCommand( new string[] { "SELECT", db.ToString( ) } );

            OperateResult<byte[]> read = ReadFromCoreServer( command );
            if (!read.IsSuccess) return read;

            string msg = Encoding.UTF8.GetString( read.Content );
            if (!msg.StartsWith( "+OK" )) return new OperateResult( msg );

            return OperateResult.CreateSuccessResult( );
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
