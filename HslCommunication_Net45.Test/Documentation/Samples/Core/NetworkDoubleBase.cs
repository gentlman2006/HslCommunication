using HslCommunication.Profinet.Melsec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication;
using HslCommunication.Core.Net;
using HslCommunication.ModBus;
using HslCommunication.Profinet.Siemens;
using System.Net.Sockets;
using HslCommunication.BasicFramework;
using HslCommunication.Core;

namespace HslCommunication_Net45.Test.Documentation.Samples.Core
{


    public class NetworkDoubleBaseTest
    {
        public void ConnectServer( )
        {
            MelsecMcNet client = new MelsecMcNet( );

            #region Connect1

            // client为设备的连接对象，简单的启动连接
            client.ConnectServer( );

            #endregion

            #region Connect2

            // client为设备的连接对象，如果想知道是否连接成功
            OperateResult connect = client.ConnectServer( );
            if (connect.IsSuccess)
            {
                Console.WriteLine( "connect success" );
            }
            else
            {
                // do something
                Console.WriteLine( "connect failed" );
            }

            #endregion


        }

        public void ConnectServer2( )
        {
            MelsecMcNet client = new MelsecMcNet( );


            AlienSession session = new AlienSession( );
            #region AlienConnect1

            // session对象由 NetworkAlienClient 类创建
            client.ConnectServer( session );

            #endregion

            #region AlienConnect2

            // session对象由 NetworkAlienClient 类创建
            OperateResult connect = client.ConnectServer( session );
            if (connect.IsSuccess)
            {
                Console.WriteLine( "connect success" );
            }
            else
            {
                // do something
                Console.WriteLine( "connect failed" );
            }

            #endregion
        }

        public void ConnectCloseExample( )
        {
            MelsecMcNet client = new MelsecMcNet( );

            #region ConnectCloseExample

            client.ConnectClose( );

            #endregion
        }

        public void InitializationOnConnectExample( )
        {
            MelsecMcNet client = new MelsecMcNet( );

            #region InitializationOnConnectExample

            

            #endregion
        }


        public void ByteTransformExample( )
        {
            MelsecMcNet client = new MelsecMcNet( );

            #region ByteTransform

            // 假设buffer是client从设备读取的数据内容
            byte[] buffer = new byte[8];
            // 转化为4个short
            short[] short_value = client.ByteTransform.TransInt16( buffer, 0, 4 );
            // 转化为2个float
            float[] float_value = client.ByteTransform.TransSingle( buffer, 0, 2 );
            // 其他的类型转换是类似的

            #endregion

        }

        public void ConnectTimeOutExample( )
        {
            MelsecMcNet client = new MelsecMcNet( );

            #region ConnectTimeOutExample

            // 设置连接的超时时间，单位 毫秒
            client.ConnectTimeOut = 1000;
            // 1秒没有连接上的时候就自动返回失败
            client.ConnectServer( );

            #endregion
        }

        public void ReceiveTimeOutExample( )
        {
            MelsecMcNet client = new MelsecMcNet( );

            #region ReceiveTimeOutExample

            // 设置反馈的超时时间，单位 毫秒
            client.ReceiveTimeOut = 1000;
            // 1秒没有接收到就自动返回失败，此处的地址示例是modbus的地址，对于读取也是一样的
            OperateResult write = client.Write( "100", 123 );

            #endregion

        }

        public void SetPersistentConnectionExample( )
        {
            MelsecMcNet client = new MelsecMcNet( );

            #region SetPersistentConnectionExample

            client.SetPersistentConnection( );        // 设置长连接，但是不立即连接，等到第一次读写的时候再启动连接。
            System.Threading.Thread.Sleep( 10000 );   // 休眠10秒



            // 调用write时才是真正的启动连接，后续的读写操作重复利用该连接，此处的地址示例是modbus的地址，对于读取也是一样的
            OperateResult write = client.Write( "100", 123 );

            #endregion
            
        }

        public void IpAddressExample( )
        {
            #region IpAddressExample

            ModbusTcpNet modbus = new ModbusTcpNet( "192.168.0.100" );
            // 读取线圈100的值
            bool coil_ip100 = modbus.ReadCoil( "100" ).Content;
            // 切换ip地址
            modbus.IpAddress = "192.168.0.101";
            bool coil_ip101 = modbus.ReadCoil( "100" ).Content;

            #endregion
        }

        
        public void ReadFromCoreServerExample1( )
        {
            #region ReadFromCoreServerExample1

            ModbusTcpNet modbus = new ModbusTcpNet( );
            MelsecMcNet melsec = new MelsecMcNet( );
            SiemensS7Net siemens = new SiemensS7Net( SiemensPLCS.S1200 );

            // 创建并连接一个socket
            Socket socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            socket.Connect( System.Net.IPAddress.Parse( "192.168.0.100" ), 1000 );

            // 叠加读写操作
            OperateResult<byte[]> read_modbus = modbus.ReadFromCoreServer( socket, SoftBasic.HexStringToBytes( "00 00 00 00 00 06 00 03 00 00 00 03" ) );
            OperateResult<byte[]> read_melsec = melsec.ReadFromCoreServer( socket, SoftBasic.HexStringToBytes( "50 00 00 FF FF 03 00 0D 00 0A 00 01 14 01 00 64 00 00 90 01 00 10" ) );
            OperateResult<byte[]> read_siemens = siemens.ReadFromCoreServer( socket, SoftBasic.HexStringToBytes( "03 00 00 24 02 F0 80 32 01 00 00 00 01 00 0E 00 05 05 01 12 0A 10 02 00 01 00 00 83 00 03 20 00 04 00 08 3B" ) );


            socket.Close( );

            #endregion
        }

        public void ReadFromCoreServerExample2( )
        {
            #region ReadFromCoreServerExample2

            ModbusTcpNet modbus = new ModbusTcpNet( "192.168.0.100" );

            // 此处举例实现特殊的modbus功能码
            OperateResult<byte[]> read = modbus.ReadFromCoreServer( SoftBasic.HexStringToBytes( "0x00 0x00 0x00 0x00 0x00 0x03 0x01 0x09 0x01" ) );
            if (read.IsSuccess)
            {
                // 成功，开始解析从服务器返回的数据，是一条完整的报文信息
                Console.WriteLine( SoftBasic.ByteToHexString( read.Content, ' ' ) );
            }
            else
            {
                // 失败
            }

            #endregion
        }


    }
}
