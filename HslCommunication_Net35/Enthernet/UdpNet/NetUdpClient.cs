using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HslCommunication.Enthernet.UdpNet
{
    /// <summary>
    /// UDP客户端的类，只负责发送数据到服务器，该数据经过封装
    /// </summary>
    public class NetUdpClient : HslCommunication.Core.Net.NetworkBase
    {

        private IPEndPoint ServerEndPoint = null;


        /// <summary>
        /// 实例化对象，
        /// </summary>
        /// <param name="endpoint"></param>
        public NetUdpClient( IPEndPoint endpoint )
        {
            ServerEndPoint = endpoint;
            CoreSocket = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
        }


        /// <summary>
        /// 发送字节数据到服务器
        /// </summary>
        /// <param name="customer">用户自定义的标记数据</param>
        /// <param name="data">字节数据</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public void SendMessage( NetHandle customer, byte[] data )
        {
            CoreSocket.SendTo( HslProtocol.CommandBytes( customer, Token, data ), ServerEndPoint );
        }


        /// <summary>
        /// 发送字符串数据到服务器
        /// </summary>
        /// <param name="customer">用户自定义的标记数据</param>
        /// <param name="data">字符串数据</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public void SendMessage( NetHandle customer, string data )
        {
            CoreSocket.SendTo( HslProtocol.CommandBytes( customer, Token, data ), ServerEndPoint );
        }
    }
}
