using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.ModBus;

namespace HslCommunication_Net45.Test.Documentation.Samples.Modbus
{
    #region ModbusTcpServerExample
    
    public class ModbusTcpServerExample
    {
        public ModbusTcpServerExample( )
        {
            // 简单的创建一个modbus的服务器
            modbusServer = new ModbusTcpServer( );
            modbusServer.ServerStart( 502 );
        }


        private ModbusTcpServer modbusServer = null;


        public void Example1( )
        {
            
            modbusServer.WriteCoil( "100", true );                              // 往线圈100地址写入true
            modbusServer.WriteCoil( "100", new bool[] { true, false, true } );  // 往线圈100-102的地址写入true,false,true
            bool coil_100 = modbusServer.ReadCoil( "100" );                     // 读取线圈100的值
            bool[] coil_100_102 = modbusServer.ReadCoil( "100", 3 );            // 读取线圈100-102的值


            
            modbusServer.WriteDiscrete( "100", true );                                 // 往离散输入100地址写入true
            modbusServer.WriteDiscrete( "100", new bool[] { true, false, true } );     // 往离散输入100-102的地址写入true,false,true
            bool discrete_100 = modbusServer.ReadDiscrete( "100" );                    // 读取离散输入100的值
            bool[] discrete_100_102 = modbusServer.ReadDiscrete( "100", 3 );           // 读取离散输入100-102的值


            
            modbusServer.Write( "100", (short)1234 );                              // 往寄存器100写入1234值
            modbusServer.Write( "100", new short[] { 1234, -1234, 567 } );         // 往寄存器100-102写入1234,-1234,567
            short reg_100 = modbusServer.ReadInt16( "100" );                       // 读取寄存器100的值
            short[] reg_100_102 = modbusServer.ReadInt16( "100", 3 );              // 读取寄存器100-102的值


            
            modbusServer.Write( "x=4;100", (short)1234 );                           // 往输入寄存器100写入1234值
            modbusServer.Write( "x=4;100", new short[] { 1234, -1234, 567 } );      // 往输入寄存器00-102写入1234,-1234,567
            short intReg_100 = modbusServer.ReadInt16( "x=4;100" );                 // 读取输入寄存器100的值
            short[] intReg_100_102 = modbusServer.ReadInt16( "x=4;100", 3 );        // 读取输入寄存器100-102的值


            // 寄存器其他的数据类型写入，请参照Write方法的重载，支持short,ushort,int,uint,long,ulong,float,double,string类型数据的读写

        }


        public void Example2( )
        {
            // 监视某一地址的数据变化，初始化的时候调用一次即可
            // 例如我要监视寄存器地址100的值，当有modbus的客户端来更改值的时候就触发，服务器端更改值不触发
            ModBusMonitorAddress monitorAddress = new ModBusMonitorAddress( );
            monitorAddress.Address = 100;
            monitorAddress.OnChange += ( ModBusMonitorAddress busMonitorAddress, short oldValue, short newValue ) =>
            {
                Console.WriteLine( "地址" + busMonitorAddress.Address + " 值发送了更改，原值：" + oldValue + " 现在的值" + newValue );
            };

            modbusServer.AddSubcription( monitorAddress );
        }


        public void Example3( )
        {
            // 启动modbus-rtu访问，通常指定COM口及波特率
            modbusServer.StartSerialPort( "COM1", 9600 );
        }

        public void Example4( )
        {
            // 仅允许指定的ip地址进行连接
            modbusServer.SetTrustedIpAddress( new List<string>( )
            {
                "192.168.0.100",
                "192.168.0.101",
                "192.168.0.102"
            } );
        }

        public void Example5( )
        {
            // 异形连接，modbus服务器运行在本地，客户端运行在云端，客户端需要对服务器进行读写
            // 更多的信息请点击 https://www.cnblogs.com/dathlin/p/8934266.html
            HslCommunication.OperateResult connect = modbusServer.ConnectHslAlientClient( "117.48.203.204", 12345, "12345678901" );
            if (connect.IsSuccess)
            {
                Console.WriteLine( "success!" );
            }
            else
            {
                Console.WriteLine( "failed:" + connect.Message );
            }
        }

        public void Example6( )
        {
            // 显示客户端发送的命令消息，这个事件调用一次即可。
            modbusServer.OnDataReceived += ( ModbusTcpServer server, byte[] data ) =>
            {
                Console.WriteLine( "Receive:" + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( data ) );
            };

            // 此处仅仅用于显示，提取指令信息可以实现更加复杂的功能。
        }
        
    }

    #endregion
}
