using HslCommunication.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace HslCommunication.Enthernet.Redis
{
    /// <summary>
    /// 提供了redis辅助类的一些方法
    /// </summary>
    public class RedisHelper
    {
        #region Socket Helper


        /// <summary>
        /// 接收一行命令数据
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <returns>带有结果对象的数据信息</returns>
        public static OperateResult<byte[]> ReceiveCommandLine( Socket socket )
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
                return OperateResult.CreateSuccessResult( bufferArray.ToArray( ) );
            }
            catch (Exception ex)
            {
                return new OperateResult<byte[]>( ex.Message );
            }
        }

        /// <summary>
        /// 接收一行字符串的信息
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="length">字符串的长度</param>
        /// <returns>带有结果对象的数据信息</returns>
        public static OperateResult<byte[]> ReceiveCommandString( Socket socket, int length )
        {
            try
            {
                List<byte> bufferArray = new List<byte>( );
                bufferArray.AddRange( NetSupport.ReadBytesFromSocket( socket, length ) );
                while (true)
                {
                    byte[] head = NetSupport.ReadBytesFromSocket( socket, 1 );
                    bufferArray.AddRange( head );
                    if (head[0] == '\n') break;
                }

                return OperateResult.CreateSuccessResult( bufferArray.ToArray( ) );
            }
            catch (Exception ex)
            {
                return new OperateResult<byte[]>( ex.Message );
            }
        }


        /// <summary>
        /// 从网络接收一条redis消息
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <returns>接收的结果对象</returns>
        public static OperateResult<byte[]> ReceiveCommand( Socket socket )
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

                if (lengthResult.Content < 0) return OperateResult.CreateSuccessResult( bufferArray.ToArray( ) );

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

        #region Prase Helper

        /// <summary>
        /// 将字符串数组打包成一个redis的报文信息
        /// </summary>
        /// <param name="commands">字节数据信息</param>
        /// <returns>结果报文信息</returns>
        public static byte[] PackStringCommand( string[] commands )
        {
            StringBuilder sb = new StringBuilder( );
            sb.Append( '*' );
            sb.Append( commands.Length.ToString( ) );
            sb.Append( "\r\n" );
            for (int i = 0; i < commands.Length; i++)
            {
                sb.Append( '$' );
                sb.Append( Encoding.UTF8.GetBytes( commands[i] ).Length.ToString( ) );
                sb.Append( "\r\n" );
                sb.Append( commands[i] );
                sb.Append( "\r\n" );
            }
            return Encoding.UTF8.GetBytes( sb.ToString( ) );
        }

        /// <summary>
        /// 从原始的结果数据对象中提取出数字数据
        /// </summary>
        /// <param name="commandLine">原始的字节数据</param>
        /// <returns>带有结果对象的数据信息</returns>
        public static OperateResult<int> GetNumberFromCommandLine( byte[] commandLine )
        {
            try
            {
                string command = Encoding.UTF8.GetString( commandLine ).TrimEnd( '\r', '\n' );
                return OperateResult.CreateSuccessResult( Convert.ToInt32( command.Substring( 1 ) ) );
            }
            catch (Exception ex)
            {
                return new OperateResult<int>( ex.Message );
            }
        }

        /// <summary>
        /// 从结果的数据对象里提取字符串的信息
        /// </summary>
        /// <param name="commandLine">原始的字节数据</param>
        /// <returns>带有结果对象的数据信息</returns>
        public static OperateResult<string> GetStringFromCommandLine( byte[] commandLine )
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
                    if (commandLine[i] == '\r' || commandLine[i] == '\n')
                    {
                        index_start = i;
                    }

                    if (commandLine[i] == '\n')
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

        /// <summary>
        /// 从redis的结果数据中分析出所有的字符串信息
        /// </summary>
        /// <param name="commandLine">结果数据</param>
        /// <returns>带有结果对象的数据信息</returns>
        public static OperateResult<string[]> GetStringsFromCommandLine( byte[] commandLine )
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
                    if (commandLine[index] == '$')
                    {
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
                        if (stringLength >= 0)
                        {
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
                        else
                        {
                            lists.Add( null );
                        }
                    }
                    else
                    {
                        int index_start = -1;
                        for (int j = index; j < commandLine.Length; j++)
                        {
                            if (commandLine[j] == '\r' || commandLine[j] == '\n')
                            {
                                index_start = j;
                                break;
                            }
                        }
                        lists.Add( Encoding.UTF8.GetString( commandLine, index, index_start - index - 1 ) );
                    }
                }
                
                return OperateResult.CreateSuccessResult( lists.ToArray( ) );
            }
            catch (Exception ex)
            {
                return new OperateResult<string[]>( ex.Message );
            }
        }


        #endregion
    }
}
