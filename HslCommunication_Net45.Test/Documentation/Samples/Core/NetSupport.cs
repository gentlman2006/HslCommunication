using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using HslCommunication.Core;
using System.IO;

namespace HslCommunication_Net45.Test.Documentation.Samples.Core
{
    public class NetSupportExample
    {

        public void ReadBytesFromSocketExample1( )
        {
            #region ReadBytesFromSocketExample1

            // 创建socket
            Socket socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            socket.Connect( IPAddress.Parse( "192.168.0.7" ), 1000 );

            // 准备接收指定长度的数据，假设为20个长度
            byte[] receive = NetSupport.ReadBytesFromSocket( socket, 20 );

            // 根据需要选择是否关闭连接
            socket.Close( );

            // 接下来就可以对receive进行操作了

            #endregion

        }

        public void ReadBytesFromSocketExample2( )
        {
            #region ReadBytesFromSocketExample2

            // 创建socket
            Socket socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            socket.Connect( IPAddress.Parse( "192.168.0.7" ), 1000 );

            // 准备接收指定长度的数据，假设为10000个长度，然后输出进度
            Action<long, long> report = ( long rece, long totle ) =>
             {
                 Console.WriteLine( "总数据量：" + totle + "  当前接收字节数：" + rece );
             };
            byte[] receive = NetSupport.ReadBytesFromSocket( socket, 10000, report, false, false );

            // 根据需要选择是否关闭连接
            socket.Close( );

            // 接下来就可以对receive进行操作了

            #endregion

        }

        public void ReadBytesFromSocketExample3( )
        {
            #region ReadBytesFromSocketExample3

            // 创建socket
            Socket socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            socket.Connect( IPAddress.Parse( "192.168.0.7" ), 1000 );

            // 准备接收指定长度的数据，假设为4个长度，然后输出进度
            byte[] head = NetSupport.ReadBytesFromSocket( socket, 4);
            int length = BitConverter.ToInt32( head, 0 );

            byte[] content = NetSupport.ReadBytesFromSocket( socket, length );

            // 根据需要选择是否关闭连接
            socket.Close( );

            // 接下来就可以对content进行操作了

            #endregion

        }

        public void WriteStreamFromSocketExample( )
        {
            #region WriteStreamFromSocketExample

            // 创建socket
            Socket socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            socket.Connect( IPAddress.Parse( "192.168.0.7" ), 1000 );

            // 准备接收指定长度的数据，假设为1234567个长度，然后输出进度
            Action<long, long> report = ( long rece, long totle ) =>
            {
                Console.WriteLine( "总数据量：" + totle + "  当前接收字节数：" + rece );
            };

            // 获取文件流
            Stream stream = new FileStream( "D:\\123.txt", FileMode.Create );
            NetSupport.WriteStreamFromSocket( socket, stream, 1234567, report, false );
            stream.Dispose( );
            socket.Close( );

            // 上述的代码是从套接字接收了1234567长度的字节，然后写入到了文件中


            #endregion
        }

        public void WriteSocketFromStreamExample()
        {
            #region WriteSocketFromStreamExample

            // 创建socket
            Socket socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            socket.Connect( IPAddress.Parse( "192.168.0.7" ), 1000 );

            // 准备接收指定长度的数据，假设为1234567个长度，然后输出进度
            Action<long, long> report = ( long rece, long totle ) =>
            {
                Console.WriteLine( "总数据量：" + totle + "  当前发送字节数：" + rece );
            };

            // 获取文件流
            Stream stream = new FileStream( "D:\\123.txt", FileMode.Open );
            NetSupport.WriteSocketFromStream( socket, stream, 1234567, report, false );
            stream.Dispose( );
            socket.Close( );

            // 上述的代码是从文件中读取数据内容，然后写入socket发送到远程




            #endregion
        }

    }
}
