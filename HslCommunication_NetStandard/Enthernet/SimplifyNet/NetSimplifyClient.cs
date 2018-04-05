using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core.Net;
using HslCommunication.Core.IMessage;
using HslCommunication.Core;

namespace HslCommunication.Enthernet
{


    /// <summary>
    /// 异步访问数据的客户端类，用于向服务器请求一些确定的数据信息
    /// </summary>
    public class NetSimplifyClient : NetworkDoubleBase<HslMessage, RegularByteTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化一个客户端的对象，用于和服务器通信
        /// </summary>
        public NetSimplifyClient( string ipAddress, int port )
        {
            IpAddress = ipAddress;
            Port = port;
        }

        /// <summary>
        /// 实例化一个客户端对象，需要手动指定Ip地址和端口
        /// </summary>
        public NetSimplifyClient( )
        {

        }

        #endregion


        /// <summary>
        /// 客户端向服务器进行请求，请求字符串数据
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">发送数据</param>
        /// <returns></returns>
        public OperateResult<string> ReadFromServer(NetHandle customer,string send = null)
        {
            var result = new OperateResult<string>( );
            var data = string.IsNullOrEmpty( send ) ? new byte[0] : Encoding.Unicode.GetBytes( send );
            var temp = ReadFromServerBase( HslProtocol.ProtocolUserString, customer, data);
            result.IsSuccess = temp.IsSuccess;
            result.ErrorCode = temp.ErrorCode;
            result.Message = temp.Message;
            if (temp.IsSuccess)
            {
                result.Content = Encoding.Unicode.GetString( temp.Content );
            }
            temp = null;
            return result;
        }


        /// <summary>
        /// 客户端向服务器进行请求，请求字节数据
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">发送的字节内容</param>
        /// <returns></returns>
        public OperateResult<byte[]> ReadFromServer(NetHandle customer,byte[] send)
        {
            return ReadFromServerBase( HslProtocol.ProtocolUserBytes, customer, send);
        }

        /// <summary>
        /// 需要发送的底层数据
        /// </summary>
        /// <param name="headcode">数据的指令头</param>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">需要发送的底层数据</param>
        /// <returns></returns>
        private OperateResult<byte[]> ReadFromServerBase(int headcode,int customer,byte[] send)
        {
            var read = ReadFromCoreServer( HslProtocol.CommandBytes( headcode, customer, Token, send ) );
            if(!read.IsSuccess)
            {
                return read;
            }

            byte[] headBytes = new byte[HslProtocol.HeadByteLength];
            byte[] contentBytes = new byte[read.Content.Length - HslProtocol.HeadByteLength];

            Array.Copy( read.Content, 0, headBytes, 0, HslProtocol.HeadByteLength );
            if(contentBytes.Length>0)
            {
                Array.Copy( read.Content, HslProtocol.HeadByteLength, contentBytes, 0, read.Content.Length - HslProtocol.HeadByteLength );
            }

            contentBytes = HslProtocol.CommandAnalysis( headBytes, contentBytes );
            return OperateResult.CreateSuccessResult( contentBytes );
        }


        #region Object Override

        /// <summary>
        /// 获取本对象的字符串表示形式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString( );
        }


        #endregion

    }


}
