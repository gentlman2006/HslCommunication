using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication.ModBus;
using HslCommunication;

namespace HslCommunication_Net45.Test.Modbus
{
    /// <summary>
    /// 主要是测试报文的生成是否正确
    /// </summary>
    [TestClass]
    public class ModbusTcpTest
    {
        [TestMethod]
        public void BuildReadCoilCommandTest( )
        {
            ModbusTcpNet modbusTcp = new ModbusTcpNet( "127.0.0.1" );

            OperateResult<byte[]> command = modbusTcp.BuildReadCoilCommand( "100", 6 );
            Assert.IsTrue( command.IsSuccess, "command create failed" );

            if(command.Content[2] == 0x00 &&
                command.Content[3] == 0x00 &&
                command.Content[4] == 0x00 &&
                command.Content[5] == 0x06 &&
                command.Content[6] == 0x01 &&
                command.Content[7] == 0x01 &&
                command.Content[8] == 0x00 &&
                command.Content[9] == 0x64 &&
                command.Content[10] == 0x00 &&
                command.Content[11] == 0x06)
            {

            }
            else
            {
                Assert.Fail( "command check failed : " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
            }
        }


        [TestMethod]
        public void BuildReadDiscreteCommandTest( )
        {
            ModbusTcpNet modbusTcp = new ModbusTcpNet( "127.0.0.1" );

            OperateResult<byte[]> command = modbusTcp.BuildReadDiscreteCommand( "s=2;100", 10 );
            Assert.IsTrue( command.IsSuccess, "command create failed" );

            if (command.Content[2] == 0x00 &&
                command.Content[3] == 0x00 &&
                command.Content[4] == 0x00 &&
                command.Content[5] == 0x06 &&
                command.Content[6] == 0x02 &&
                command.Content[7] == 0x02 &&
                command.Content[8] == 0x00 &&
                command.Content[9] == 0x64 &&
                command.Content[10] == 0x00 &&
                command.Content[11] == 0x0A)
            {

            }
            else
            {
                Assert.Fail( "command check failed : " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
            }
        }

        [TestMethod]
        public void BuildReadRegisterCommandTest1( )
        {
            ModbusTcpNet modbusTcp = new ModbusTcpNet( "127.0.0.1" );

            OperateResult<byte[]> command = modbusTcp.BuildReadRegisterCommand( "s=2;123", 10 );
            Assert.IsTrue( command.IsSuccess, "command create failed" );

            if (command.Content[2] == 0x00 &&
                command.Content[3] == 0x00 &&
                command.Content[4] == 0x00 &&
                command.Content[5] == 0x06 &&
                command.Content[6] == 0x02 &&
                command.Content[7] == 0x03 &&
                command.Content[8] == 0x00 &&
                command.Content[9] == 0x7B &&
                command.Content[10] == 0x00 &&
                command.Content[11] == 0x0A)
            {

            }
            else
            {
                Assert.Fail( "command check failed : " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
            }
        }


        [TestMethod]
        public void BuildReadRegisterCommandTest2( )
        {
            ModbusTcpNet modbusTcp = new ModbusTcpNet( "127.0.0.1" );

            OperateResult<byte[]> command = modbusTcp.BuildReadRegisterCommand( "x=4;s=2;123", 10 );
            Assert.IsTrue( command.IsSuccess, "command create failed" );

            if (command.Content[2] == 0x00 &&
                command.Content[3] == 0x00 &&
                command.Content[4] == 0x00 &&
                command.Content[5] == 0x06 &&
                command.Content[6] == 0x02 &&
                command.Content[7] == 0x04 &&
                command.Content[8] == 0x00 &&
                command.Content[9] == 0x7B &&
                command.Content[10] == 0x00 &&
                command.Content[11] == 0x0A)
            {

            }
            else
            {
                Assert.Fail( "command check failed : " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
            }
        }

        [TestMethod]
        public void BuildWriteOneCoilCommandTest1( )
        {
            ModbusTcpNet modbusTcp = new ModbusTcpNet( "127.0.0.1" );

            OperateResult<byte[]> command = modbusTcp.BuildWriteOneCoilCommand( "123", true );
            Assert.IsTrue( command.IsSuccess, "command create failed" );

            if (command.Content[2] == 0x00 &&
                command.Content[3] == 0x00 &&
                command.Content[4] == 0x00 &&
                command.Content[5] == 0x06 &&
                command.Content[6] == 0x01 &&
                command.Content[7] == 0x05 &&
                command.Content[8] == 0x00 &&
                command.Content[9] == 0x7B &&
                command.Content[10] == 0xFF &&
                command.Content[11] == 0x00)
            {

            }
            else
            {
                Assert.Fail( "command check failed : " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
            }
        }

        [TestMethod]
        public void BuildWriteOneCoilCommandTest2( )
        {
            ModbusTcpNet modbusTcp = new ModbusTcpNet( "127.0.0.1" );

            OperateResult<byte[]> command = modbusTcp.BuildWriteOneCoilCommand( "s=2;123", false );
            Assert.IsTrue( command.IsSuccess, "command create failed" );

            if (command.Content[2] == 0x00 &&
                command.Content[3] == 0x00 &&
                command.Content[4] == 0x00 &&
                command.Content[5] == 0x06 &&
                command.Content[6] == 0x02 &&
                command.Content[7] == 0x05 &&
                command.Content[8] == 0x00 &&
                command.Content[9] == 0x7B &&
                command.Content[10] == 0x00 &&
                command.Content[11] == 0x00)
            {

            }
            else
            {
                Assert.Fail( "command check failed : " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
            }
        }

        [TestMethod]
        public void BuildWriteOneRegisterCommandTest( )
        {
            ModbusTcpNet modbusTcp = new ModbusTcpNet( "127.0.0.1" );

            OperateResult<byte[]> command = modbusTcp.BuildWriteOneRegisterCommand( "s=2;123", new byte[] { 0x01, 0x10 } );
            Assert.IsTrue( command.IsSuccess, "command create failed" );

            if (command.Content[2] == 0x00 &&
                command.Content[3] == 0x00 &&
                command.Content[4] == 0x00 &&
                command.Content[5] == 0x06 &&
                command.Content[6] == 0x02 &&
                command.Content[7] == 0x06 &&
                command.Content[8] == 0x00 &&
                command.Content[9] == 0x7B &&
                command.Content[10] == 0x01 &&
                command.Content[11] == 0x10)
            {

            }
            else
            {
                Assert.Fail( "command check failed : " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
            }
        }

        [TestMethod]
        public void BuildWriteCoilCommandTest( )
        {
            ModbusTcpNet modbusTcp = new ModbusTcpNet( "127.0.0.1" );

            OperateResult<byte[]> command = modbusTcp.BuildWriteCoilCommand( "s=2;123", new bool[] { true, false, false, true } );
            Assert.IsTrue( command.IsSuccess, "command create failed" );

            if (command.Content[2] == 0x00 &&
                command.Content[3] == 0x00 &&
                command.Content[4] == 0x00 &&
                command.Content[5] == 0x08 &&
                command.Content[6] == 0x02 &&
                command.Content[7] == 0x0F &&
                command.Content[8] == 0x00 &&
                command.Content[9] == 0x7B &&
                command.Content[10] == 0x00 &&
                command.Content[11] == 0x04 &&
                command.Content[12] == 0x01 &&
                command.Content[13] == 0x09)
            {

            }
            else
            {
                Assert.Fail( "command check failed : " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
            }
        }

        [TestMethod]
        public void BuildWriteRegisterCommandTest( )
        {
            ModbusTcpNet modbusTcp = new ModbusTcpNet( "127.0.0.1" );

            OperateResult<byte[]> command = modbusTcp.BuildWriteRegisterCommand( "s=2;123", new byte[] { 0x12, 0x34, 0x56, 0x78 } );
            Assert.IsTrue( command.IsSuccess, "command create failed" );

            if (command.Content[2] == 0x00 &&
                command.Content[3] == 0x00 &&
                command.Content[4] == 0x00 &&
                command.Content[5] == 0x0B &&
                command.Content[6] == 0x02 &&
                command.Content[7] == 0x10 &&
                command.Content[8] == 0x00 &&
                command.Content[9] == 0x7B &&
                command.Content[10] == 0x00 &&
                command.Content[11] == 0x02 &&
                command.Content[12] == 0x04 &&
                command.Content[13] == 0x12 &&
                command.Content[14] == 0x34 &&
                command.Content[15] == 0x56 &&
                command.Content[16] == 0x78)
            {

            }
            else
            {
                Assert.Fail( "command check failed : " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
            }
        }
    }
}
