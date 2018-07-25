using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication.ModBus;
using HslCommunication;

namespace HslCommunication_Net45.Test.Modbus
{
    [TestClass]
    public class ModbusTest
    {

        private ModbusTcpServer tcpServer;
        private ModbusTcpNet modbusTcp;

        
        public ModbusTest( )
        {
            tcpServer = new ModbusTcpServer( );
            tcpServer.ServerStart( 502 );

            modbusTcp = new ModbusTcpNet( "127.0.0.1" );
            modbusTcp.ConnectServer( );
        }


        [TestMethod]
        public void CoilTest( )
        {
            tcpServer.WriteCoil( "1234", true );
            OperateResult<bool> opValue = modbusTcp.ReadCoil( "1234" );

            if (!opValue.IsSuccess)
            {
                Assert.Fail( "Coil ReadFailed" );
                return;
            }

            if (!opValue.Content)
            {
                Assert.Fail( "Coil Value Failed" );
            }
        }


        public void CoilArrayTest1( )
        {
            bool[] buffer = new bool[] { true, false, false, true, true, false };
            modbusTcp.WriteCoil( "1000", buffer );

            bool[] value = modbusTcp.ReadCoil( "1000", 6 ).Content;

            for (int i = 0; i < buffer.Length; i++)
            {
                if(buffer[i]!=value[i])
                {
                    Assert.Fail( "Coil Array Value Failed" );
                }
            }
        }


        public void CoilArrayTest2( )
        {
            bool[] buffer = new bool[] { true, false, false, true, true, false };
            tcpServer.WriteCoil( "1100", buffer );

            bool[] value = modbusTcp.ReadCoil( "1100", 6 ).Content;

            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] != value[i])
                {
                    Assert.Fail( "Coil Array Value Failed" );
                }
            }
        }


        public void RegisterByteTest( )
        {

        }


    }
}
