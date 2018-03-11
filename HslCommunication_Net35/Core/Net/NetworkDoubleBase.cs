using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace HslCommunication.Core
{


    /// <summary>
    /// 支持长连接，短连接两个模式的通用客户端基类
    /// </summary>
    public class NetworkDoubleBase<TNetMessage> : NetworkBase where TNetMessage : INetMessage, new()
    {
        #region Private Member

        /// <summary>
        /// 是否处于长连接的状态
        /// </summary>
        private bool IsPersistentConn { get; set; } = false;
        /// <summary>
        /// 指示长连接的套接字是否处于错误的状态
        /// </summary>
        private bool IsSocketErrorState { get; set; } = false;
        /// <summary>
        /// IP地址
        /// </summary>
        private string ipAddress = "127.0.0.1";
        private int port = 10000;

        #endregion



        #region Receive Message

        /// <summary>
        /// 接收一条完成的数据，使用异步接收完成，包含了指令头信息
        /// </summary>
        /// <param name="socket">已经打开的网络套接字</param>
        /// <returns>数据的接收结果对象</returns>
        protected OperateResult<TNetMessage> ReceiveMessage(Socket socket)
        {
            TNetMessage netMsg = new TNetMessage();
            OperateResult<TNetMessage> result = new OperateResult<TNetMessage>();

            // 接收指令头
            OperateResult<byte[]> headResult = Receive(socket, netMsg.ProtocolHeadBytesLength);
            if (!headResult.IsSuccess)
            {
                socket.Close();
                result.CopyErrorFromOther(headResult);
                return result;
            }

            netMsg.HeadBytes = headResult.Content;

            if (!netMsg.CheckHeadBytesLegal(Token.ToByteArray()))
            {
                // 令牌校验失败
                socket.Close();
                LogNet?.WriteError(ToString(), StringResources.TokenCheckFailed);
                result.Message = StringResources.TokenCheckFailed;
                return result;
            }

            int contentLength = netMsg.GetContentLengthByHeadBytes();
            if (contentLength == 0)
            {
                netMsg.ContentBytes = new byte[0];
            }
            else
            {
                OperateResult<byte[]> contentResult = Receive(socket, contentLength);
                if (!headResult.IsSuccess)
                {
                    socket.Close();
                    result.CopyErrorFromOther(contentResult);
                    return result;
                }
            }

            result.Content = netMsg;
            result.IsSuccess = true;
            return result;
        }

        #endregion

        #region Core Communication

        /***************************************************************************************
         * 
         *    主要的数据交互分为4步
         *    1. 连接服务器，或是获取到旧的使用的网络信息
         *    2. 发送数据信息
         *    3. 接收反馈的数据信息
         *    4. 关闭网络连接，如果是短连接的话
         * 
         **************************************************************************************/


        public OperateResult<byte[],byte[]> ReadFromCoreServer(byte[] send)
        {
            Socket socket = null;
            if(IsPersistentConn)
            {
                // 长连接模式
            }
            else
            {
                // 短连接模式
            }
        }


        #endregion

    }


}
