using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core.IMessage;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using HslCommunication.BasicFramework;
using HslCommunication.Enthernet;

namespace HslCommunication.Core.Net
{
    /// <summary>
    /// 包含了主动异步接收的方法实现和文件类异步读写的实现
    /// </summary>
    public class NetworkXBase : NetworkBase
    {
        #region Constractor

        /// <summary>
        /// 默认的无参构造方法
        /// </summary>
        public NetworkXBase()
        {

        }

        #endregion

        #region Protect Member


        #endregion

        #region Send Bytes Async

        /// <summary>
        /// 发送数据的方法
        /// </summary>
        /// <param name="session">通信用的核心对象</param>
        /// <param name="content">完整的字节信息</param>
        internal void SendBytesAsync( AppSession session, byte[] content )
        {
            if (content == null) return;
            try
            {
                // 进入发送数据的锁，然后开启异步的数据发送
                session.HybirdLockSend.Enter( );

                // 启用另外一个网络封装对象进行发送数据
                AsyncStateSend state = new AsyncStateSend( )
                {
                    WorkSocket = session.WorkSocket,
                    Content = content,
                    AlreadySendLength = 0,
                    HybirdLockSend = session.HybirdLockSend,
                };

                state.WorkSocket.BeginSend(
                    state.Content,
                    state.AlreadySendLength,
                    state.Content.Length - state.AlreadySendLength,
                    SocketFlags.None,
                    new AsyncCallback( SendCallBack ),
                    state );
            }
            catch (ObjectDisposedException)
            {
                // 不操作
                session.HybirdLockSend.Leave( );
            }
            catch (Exception ex)
            {
                session.HybirdLockSend.Leave( );
                if (!ex.Message.Contains( StringResources.Language.SocketRemoteCloseException ))
                {
                    LogNet?.WriteException( ToString( ), StringResources.Language.SocketSendException, ex );
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
                        SocketFlags.None,
                        new AsyncCallback( SendCallBack ),
                        stateone );
                    }
                    else
                    {
                        stateone.HybirdLockSend.Leave( );
                        // 发送完成
                        stateone = null;
                    }
                }
                catch (ObjectDisposedException)
                {
                    stateone.HybirdLockSend.Leave( );
                    // 不处理
                    stateone = null;
                }
                catch (Exception ex)
                {
                    LogNet?.WriteException( ToString( ), StringResources.Language.SocketEndSendException, ex );
                    stateone.HybirdLockSend.Leave( );
                    stateone = null;
                }
            }
        }

        #endregion

        #region Receive Bytes Async

        /// <summary>
        /// 重新开始接收下一次的数据传递
        /// </summary>
        /// <param name="session">网络状态</param>
        /// <param name="isProcess">是否触发数据处理</param>
        internal void ReBeginReceiveHead( AppSession session, bool isProcess )
        {
            try
            {
                byte[] head = session.BytesHead, Content = session.BytesContent;
                session.Clear( );
                session.WorkSocket.BeginReceive( session.BytesHead, session.AlreadyReceivedHead, session.BytesHead.Length - session.AlreadyReceivedHead,
                    SocketFlags.None, new AsyncCallback( HeadBytesReceiveCallback ), session );
                // 检测是否需要数据处理
                if (isProcess)
                {
                    // 校验令牌
                    if (CheckRemoteToken( head ))
                    {
                        Content = HslProtocol.CommandAnalysis( head, Content );
                        int protocol = BitConverter.ToInt32( head, 0 );
                        int customer = BitConverter.ToInt32( head, 4 );
                        // 转移到数据中心处理
                        DataProcessingCenter( session, protocol, customer, Content );
                    }
                    else
                    {
                        // 应该关闭网络通信
                        LogNet?.WriteWarn( ToString( ), StringResources.Language.TokenCheckFailed );
                        AppSessionRemoteClose( session );
                    }
                }
            }
            catch (Exception ex)
            {
                SocketReceiveException( session, ex );
                LogNet?.WriteException( ToString( ), ex );
            }
        }

        /// <summary>
        /// 指令头接收方法
        /// </summary>
        /// <param name="ar">异步状态信息</param>
        protected void HeadBytesReceiveCallback( IAsyncResult ar )
        {
            if (ar.AsyncState is AppSession session)
            {
                try
                {
                    int receiveCount = session.WorkSocket.EndReceive( ar );
                    if (receiveCount == 0)
                    {
                        // 断开了连接，需要做个处理，一个是直接关闭，另一个是触发下线
                        AppSessionRemoteClose( session );
                        return;
                    }
                    else
                    {
                        session.AlreadyReceivedHead += receiveCount;
                    }
                }
                catch (ObjectDisposedException)
                {
                    // 不需要处理，来自服务器主动关闭
                    return;
                }
                catch (SocketException ex)
                {
                    // 已经断开连接了
                    SocketReceiveException( session, ex );
                    LogNet?.WriteException( ToString( ), ex );
                    return;
                }
                catch (Exception ex)
                {
                    // 其他乱七八糟的异常重新启用接收数据
                    ReBeginReceiveHead( session, false );
                    LogNet?.WriteException( ToString( ), StringResources.Language.SocketEndReceiveException, ex );
                    return;
                }


                if (session.AlreadyReceivedHead < session.BytesHead.Length)
                {
                    try
                    {
                        // 仍需要接收
                        session.WorkSocket.BeginReceive( session.BytesHead, session.AlreadyReceivedHead, session.BytesHead.Length - session.AlreadyReceivedHead,
                            SocketFlags.None, new AsyncCallback( HeadBytesReceiveCallback ), session );
                    }
                    catch (Exception ex)
                    {
                        SocketReceiveException( session, ex );
                        LogNet?.WriteException( ToString( ), ex );
                    }
                }
                else
                {
                    // 接收完毕，校验令牌
                    if (!CheckRemoteToken( session.BytesHead ))
                    {
                        LogNet?.WriteWarn( ToString( ), StringResources.Language.TokenCheckFailed );
                        AppSessionRemoteClose( session );
                        return;
                    }

                    int receive_length = BitConverter.ToInt32( session.BytesHead, session.BytesHead.Length - 4 );


                    session.BytesContent = new byte[receive_length];

                    if (receive_length > 0)
                    {
                        try
                        {
                            int receiveSize = session.BytesContent.Length - session.AlreadyReceivedContent;
                            session.WorkSocket.BeginReceive( session.BytesContent, session.AlreadyReceivedContent, receiveSize,
                                SocketFlags.None, new AsyncCallback( ContentReceiveCallback ), session );
                        }
                        catch (Exception ex)
                        {
                            SocketReceiveException( session, ex );
                            LogNet?.WriteException( ToString( ), ex );
                        }
                    }
                    else
                    {
                        // 处理数据并重新启动接收
                        ReBeginReceiveHead( session, true );
                    }
                }
            }
        }




        /// <summary>
        /// 数据内容接收方法
        /// </summary>
        /// <param name="ar"></param>
        private void ContentReceiveCallback( IAsyncResult ar )
        {
            if (ar.AsyncState is AppSession receive)
            {
                try
                {
                    receive.AlreadyReceivedContent += receive.WorkSocket.EndReceive( ar );
                }
                catch (ObjectDisposedException)
                {
                    //不需要处理
                    return;
                }
                catch (SocketException ex)
                {
                    //已经断开连接了
                    SocketReceiveException( receive, ex );
                    LogNet?.WriteException( ToString( ), ex );
                    return;
                }
                catch (Exception ex)
                {
                    //其他乱七八糟的异常重新启用接收数据
                    ReBeginReceiveHead( receive, false );
                    LogNet?.WriteException( ToString( ), StringResources.Language.SocketEndReceiveException, ex );
                    return;
                }


                if (receive.AlreadyReceivedContent < receive.BytesContent.Length)
                {
                    int receiveSize = receive.BytesContent.Length - receive.AlreadyReceivedContent;
                    try
                    {
                        //仍需要接收
                        receive.WorkSocket.BeginReceive( receive.BytesContent, receive.AlreadyReceivedContent, receiveSize, SocketFlags.None, new AsyncCallback( ContentReceiveCallback ), receive );
                    }
                    catch (Exception ex)
                    {
                        ReBeginReceiveHead( receive, false );
                        LogNet?.WriteException( ToString( ), StringResources.Language.SocketEndReceiveException, ex );
                    }
                }
                else
                {
                    //处理数据并重新启动接收
                    ReBeginReceiveHead( receive, true );
                }

            }
        }


        #endregion

        #region Token Check

        /// <summary>
        /// 检查当前的头子节信息的令牌是否是正确的
        /// </summary>
        /// <param name="headBytes">头子节数据</param>
        /// <returns>令牌是验证成功</returns>
        protected bool CheckRemoteToken( byte[] headBytes )
        {
            return SoftBasic.IsByteTokenEquel( headBytes, Token );
        }


        #endregion

        #region Special Bytes Send

        /// <summary>
        /// [自校验] 发送字节数据并确认对方接收完成数据，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="headcode">头指令</param>
        /// <param name="customer">用户指令</param>
        /// <param name="send">发送的数据</param>
        /// <returns>是否发送成功</returns>
        protected OperateResult SendBaseAndCheckReceive( Socket socket, int headcode, int customer, byte[] send )
        {
            // 数据处理
            send = HslProtocol.CommandBytes( headcode, customer, Token, send );
            
            // 发送数据
            OperateResult sendResult = Send( socket, send );
            if(!sendResult.IsSuccess) return sendResult;
            
            // 检查对方接收完成
            OperateResult<long> checkResult = ReceiveLong( socket );
            if(!checkResult.IsSuccess) return checkResult;

            // 检查长度接收
            if (checkResult.Content != send.Length)
            {
                socket?.Close();
                return new OperateResult( StringResources.Language.CommandLengthCheckFailed );
            }

            return checkResult;
        }


        /// <summary>
        /// [自校验] 发送字节数据并确认对方接收完成数据，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="customer">用户指令</param>
        /// <param name="send">发送的数据</param>
        /// <returns>是否发送成功</returns>
        protected OperateResult SendBytesAndCheckReceive( Socket socket, int customer, byte[] send )
        {
            return SendBaseAndCheckReceive( socket, HslProtocol.ProtocolUserBytes, customer, send );
        }


        /// <summary>
        /// [自校验] 直接发送字符串数据并确认对方接收完成数据，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="customer">用户指令</param>
        /// <param name="send">发送的数据</param>
        /// <returns>是否发送成功</returns>
        protected OperateResult SendStringAndCheckReceive( Socket socket, int customer, string send )
        {
            byte[] data = string.IsNullOrEmpty( send ) ? null : Encoding.Unicode.GetBytes( send );

            return SendBaseAndCheckReceive( socket, HslProtocol.ProtocolUserString, customer, data );
        }



        /// <summary>
        /// [自校验] 将文件数据发送至套接字，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="filename">完整的文件路径</param>
        /// <param name="filelength">文件的长度</param>
        /// <param name="report">进度报告器</param>
        /// <returns>是否发送成功</returns>
        protected OperateResult SendFileStreamToSocket( Socket socket, string filename, long filelength, Action<long, long> report = null )
        {
            try
            {
                OperateResult result = null;
                using (FileStream fs = new FileStream( filename, FileMode.Open, FileAccess.Read ))
                {
                    result = SendStream( socket, fs, filelength, report, true );
                }
                return result;
            }
            catch (Exception ex)
            {
                socket?.Close( );
                LogNet?.WriteException( ToString( ), ex );
                return new OperateResult( ex.Message );
            }
        }


        /// <summary>
        /// [自校验] 将文件数据发送至套接字，具体发送细节将在继承类中实现，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="filename">文件名称，文件必须存在</param>
        /// <param name="servername">远程端的文件名称</param>
        /// <param name="filetag">文件的额外标签</param>
        /// <param name="fileupload">文件的上传人</param>
        /// <param name="sendReport">发送进度报告</param>
        /// <returns>是否发送成功</returns>
        protected OperateResult SendFileAndCheckReceive(
            Socket socket,
            string filename,
            string servername,
            string filetag,
            string fileupload,
            Action<long, long> sendReport = null
            )
        {
            // 发送文件名，大小，标签
            FileInfo info = new FileInfo( filename );

            if (!File.Exists( filename ))
            {
                // 如果文件不存在
                OperateResult stringResult = SendStringAndCheckReceive( socket, 0, "" );
                if (!stringResult.IsSuccess) return stringResult;

                socket?.Close( );
                return new OperateResult( StringResources.Language.FileNotExist );
            }

            // 文件存在的情况
            Newtonsoft.Json.Linq.JObject json = new Newtonsoft.Json.Linq.JObject
            {
                { "FileName", new Newtonsoft.Json.Linq.JValue(servername) },
                { "FileSize", new Newtonsoft.Json.Linq.JValue(info.Length) },
                { "FileTag", new Newtonsoft.Json.Linq.JValue(filetag) },
                { "FileUpload", new Newtonsoft.Json.Linq.JValue(fileupload) }
            };
            
            // 先发送文件的信息到对方
            OperateResult sendResult = SendStringAndCheckReceive( socket, 1, json.ToString( ) );
            if (!sendResult.IsSuccess) return sendResult;
            
            // 最后发送
            return SendFileStreamToSocket( socket, filename, info.Length, sendReport );
        }



        /// <summary>
        /// [自校验] 将流数据发送至套接字，具体发送细节将在继承类中实现，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="stream">文件名称，文件必须存在</param>
        /// <param name="servername">远程端的文件名称</param>
        /// <param name="filetag">文件的额外标签</param>
        /// <param name="fileupload">文件的上传人</param>
        /// <param name="sendReport">发送进度报告</param>
        /// <returns></returns>
        protected OperateResult SendFileAndCheckReceive(
            Socket socket,
            Stream stream,
            string servername,
            string filetag,
            string fileupload,
            Action<long, long> sendReport = null
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
            
            // 发送文件信息
            OperateResult fileResult = SendStringAndCheckReceive( socket, 1, json.ToString( ) );
            if (!fileResult.IsSuccess) return fileResult;
            
            return SendStream( socket, stream, stream.Length, sendReport, true );
        }
        
        #endregion

        #region Special Bytes Receive

        /// <summary>
        /// [自校验] 接收一条完整的同步数据，包含头子节和内容字节，基础的数据，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="timeout">超时时间设置，如果为负数，则不检查超时</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">result</exception>
        protected OperateResult<byte[], byte[]> ReceiveAndCheckBytes( Socket socket, int timeout )
        {
            // 30秒超时接收验证
            HslTimeOut hslTimeOut = new HslTimeOut( )
            {
                DelayTime = timeout,
                IsSuccessful = false,
                StartTime = DateTime.Now,
                WorkSocket = socket,
            };

            if (timeout > 0) ThreadPool.QueueUserWorkItem( new WaitCallback( ThreadPoolCheckTimeOut ), hslTimeOut );
            
            // 接收头指令
            OperateResult<byte[]> headResult = Receive( socket, HslProtocol.HeadByteLength );
            if (!headResult.IsSuccess)
            {
                hslTimeOut.IsSuccessful = true;
                return OperateResult.CreateFailedResult<byte[],byte[]>( headResult );
            }
            hslTimeOut.IsSuccessful = true;
            
            // 检查令牌
            if (!CheckRemoteToken( headResult.Content ))
            {
                socket?.Close( );
                return new OperateResult<byte[], byte[]>( StringResources.Language.TokenCheckFailed );
            }

            int contentLength = BitConverter.ToInt32( headResult.Content, HslProtocol.HeadByteLength - 4 );
            // 接收内容
            OperateResult<byte[]> contentResult = Receive( socket, contentLength );
            if (!contentResult.IsSuccess) return OperateResult.CreateFailedResult<byte[], byte[]>( contentResult );

            // 返回成功信息
            OperateResult checkResult = SendLong( socket, HslProtocol.HeadByteLength + contentLength );
            if(!checkResult.IsSuccess) return OperateResult.CreateFailedResult<byte[], byte[]>( checkResult );

            byte[] head = headResult.Content;
            byte[] content = contentResult.Content;
            content = HslProtocol.CommandAnalysis( head, content );
            return OperateResult.CreateSuccessResult( head, content );
        }

        /// <summary>
        /// [自校验] 从网络中接收一个字符串数据，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <returns></returns>
        protected OperateResult<int, string> ReceiveStringContentFromSocket( Socket socket )
        {
            OperateResult<byte[], byte[]> receive = ReceiveAndCheckBytes( socket, 10000 );
            if (!receive.IsSuccess) return OperateResult.CreateFailedResult<int, string>( receive );

            // 检查是否是字符串信息
            if (BitConverter.ToInt32( receive.Content1, 0 ) != HslProtocol.ProtocolUserString)
            {
                LogNet?.WriteError( ToString( ), StringResources.Language.CommandHeadCodeCheckFailed );
                socket?.Close( );
                return new OperateResult<int, string>( StringResources.Language.CommandHeadCodeCheckFailed );
            }

            if (receive.Content2 == null) receive.Content2 = new byte[0];
            // 分析数据
            return OperateResult.CreateSuccessResult( BitConverter.ToInt32( receive.Content1, 4 ), Encoding.Unicode.GetString( receive.Content2 ) );
        }



        /// <summary>
        /// [自校验] 从网络中接收一串字节数据，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <returns></returns>
        protected OperateResult<int, byte[]> ReceiveBytesContentFromSocket( Socket socket )
        {
            OperateResult<byte[], byte[]> receive = ReceiveAndCheckBytes( socket, 10000 );
            if (!receive.IsSuccess) return OperateResult.CreateFailedResult<int, byte[]>( receive );

            // 检查是否是字节信息
            if (BitConverter.ToInt32( receive.Content1, 0 ) != HslProtocol.ProtocolUserBytes)
            {
                LogNet?.WriteError( ToString( ), StringResources.Language.CommandHeadCodeCheckFailed );
                socket?.Close( );
                return new OperateResult<int, byte[]>( StringResources.Language.CommandHeadCodeCheckFailed );
            }

            // 分析数据
            return OperateResult.CreateSuccessResult( BitConverter.ToInt32( receive.Content1, 4 ), receive.Content2 );
        }


        /// <summary>
        /// [自校验] 从套接字中接收文件头信息
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        protected OperateResult<FileBaseInfo> ReceiveFileHeadFromSocket( Socket socket )
        {
            // 先接收文件头信息
            OperateResult<int, string> receiveString = ReceiveStringContentFromSocket( socket );
            if (!receiveString.IsSuccess) return OperateResult.CreateFailedResult<FileBaseInfo>( receiveString );
            
            // 判断文件是否存在
            if (receiveString.Content1 == 0)
            {
                socket?.Close( );
                LogNet?.WriteWarn( ToString( ), StringResources.Language.FileRemoteNotExist );
                return new OperateResult<FileBaseInfo>( StringResources.Language.FileNotExist );
            }

            OperateResult<FileBaseInfo> result = new OperateResult<FileBaseInfo>
            {
                Content = new FileBaseInfo( )
            };
            try
            {
                // 提取信息
                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse( receiveString.Content2 );
                result.Content.Name = SoftBasic.GetValueFromJsonObject( json, "FileName", "" );
                result.Content.Size = SoftBasic.GetValueFromJsonObject( json, "FileSize", 0L );
                result.Content.Tag = SoftBasic.GetValueFromJsonObject( json, "FileTag", "" );
                result.Content.Upload = SoftBasic.GetValueFromJsonObject( json, "FileUpload", "" );
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                socket?.Close( );
                result.Message = "Extra，" + ex.Message;
            }

            return result;
        }

        /// <summary>
        /// [自校验] 从网络中接收一个文件，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="savename">接收文件后保存的文件名</param>
        /// <param name="receiveReport">接收进度报告</param>
        /// <returns></returns>
        protected OperateResult<FileBaseInfo> ReceiveFileFromSocket( Socket socket, string savename, Action<long, long> receiveReport )
        {
            // 先接收文件头信息
            OperateResult<FileBaseInfo> fileResult = ReceiveFileHeadFromSocket( socket );
            if (!fileResult.IsSuccess) return fileResult;

            try
            {
                using (FileStream fs = new FileStream( savename, FileMode.Create, FileAccess.Write ))
                {
                    WriteStream( socket, fs, fileResult.Content.Size, receiveReport, true );
                }
                return fileResult;
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), ex );
                socket?.Close( );
                return new OperateResult<FileBaseInfo>( )
                {
                    Message = ex.Message
                };
            }
        }


        /// <summary>
        /// [自校验] 从网络中接收一个文件，写入数据流，如果结果异常，则结束通讯，参数顺序文件名，文件大小，文件标识，上传人
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="stream">等待写入的数据流</param>
        /// <param name="receiveReport">接收进度报告</param>
        /// <returns></returns>
        protected OperateResult<FileBaseInfo> ReceiveFileFromSocket( Socket socket, Stream stream, Action<long, long> receiveReport )
        {
            // 先接收文件头信息
            OperateResult<FileBaseInfo> fileResult = ReceiveFileHeadFromSocket( socket );
            if (!fileResult.IsSuccess) return fileResult;

            try
            {
                WriteStream( socket, stream, fileResult.Content.Size, receiveReport, true );
                return fileResult;
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), ex );
                socket?.Close( );
                return new OperateResult<FileBaseInfo>( )
                {
                    Message = ex.Message
                };
            }
        }

        #endregion

        #region File Operate

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
                LogNet?.WriteException( ToString( ), "delete file failed:" + filename, ex );
                return false;
            }
        }

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

        #region Virtual Method


        /// <summary>
        /// 数据处理中心，应该继承重写
        /// </summary>
        /// <param name="session">连接状态</param>
        /// <param name="protocol">协议头</param>
        /// <param name="customer">用户自定义</param>
        /// <param name="content">数据内容</param>
        internal virtual void DataProcessingCenter( AppSession session, int protocol, int customer, byte[] content )
        {

        }

        /// <summary>
        /// 接收出错的时候进行处理
        /// </summary>
        /// <param name="session">会话内容</param>
        /// <param name="ex">异常信息</param>
        internal virtual void SocketReceiveException( AppSession session, Exception ex )
        {

        }


        /// <summary>
        /// 当远端的客户端关闭连接时触发
        /// </summary>
        /// <param name="session"></param>
        internal virtual void AppSessionRemoteClose( AppSession session )
        {

        }

        #endregion

        #region Receive Long And Send Long

        /// <summary>
        /// 从网络中接收Long数据
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        private OperateResult<long> ReceiveLong( Socket socket )
        {
            OperateResult<byte[]> read = Receive( socket, 8 );
            if (read.IsSuccess)
            {
                return OperateResult.CreateSuccessResult( BitConverter.ToInt64( read.Content, 0 ) );
            }
            else
            {
                return new OperateResult<long>( )
                {
                    Message = read.Message,
                };
            }
        }

        /// <summary>
        /// 将long数据发送到套接字
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private OperateResult SendLong( Socket socket, long value )
        {
            return Send( socket, BitConverter.GetBytes( value ) );
        }


        #endregion

        #region StreamWriteRead


        /// <summary>
        /// 发送一个流的所有数据到网络套接字
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="stream"></param>
        /// <param name="receive"></param>
        /// <param name="report"></param>
        /// <param name="reportByPercent"></param>
        /// <returns></returns>
        protected OperateResult SendStream( Socket socket, Stream stream, long receive, Action<long, long> report, bool reportByPercent )
        {
            byte[] buffer = new byte[102400]; // 100K的数据缓存池
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
                OperateResult write = SendBytesAndCheckReceive( socket, read.Content, newBuffer );
                if (!write.IsSuccess)
                {
                    return new OperateResult( )
                    {
                        Message = write.Message,
                    };
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

        /// <summary>
        /// 从套接字中接收所有的数据然后写入到流当中去
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="stream"></param>
        /// <param name="totalLength"></param>
        /// <param name="report"></param>
        /// <param name="reportByPercent"></param>
        /// <returns></returns>
        protected OperateResult WriteStream( Socket socket, Stream stream, long totalLength, Action<long, long> report, bool reportByPercent )
        {
            long count_receive = 0;
            long percent = 0;
            while (count_receive < totalLength)
            {
                // 先从流中异步接收数据
                OperateResult<int,byte[]> read = ReceiveBytesContentFromSocket( socket );
                if (!read.IsSuccess)
                {
                    return new OperateResult( )
                    {
                        Message = read.Message,
                    };
                }
                count_receive += read.Content1;
                
                // 开始写入文件流
                OperateResult write = WriteStream( stream, read.Content2 );
                if (!write.IsSuccess)
                {
                    return new OperateResult( )
                    {
                        Message = write.Message,
                    };
                }

                // 报告进度
                if (reportByPercent)
                {
                    long percentCurrent = count_receive * 100 / totalLength;
                    if (percent != percentCurrent)
                    {
                        percent = percentCurrent;
                        report?.Invoke( count_receive, totalLength );
                    }
                }
                else
                {
                    report?.Invoke( count_receive, totalLength );
                }
                
            }

            return OperateResult.CreateSuccessResult( );
        }


        #endregion

        #region Object Override

        /// <summary>
        /// 获取本对象的字符串表示形式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "NetworkXBase";
        }


        #endregion
    }
}
