using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication;
using HslCommunication.Profinet.Melsec;

namespace HslCommunication_Net45.Test.Profinet.Melsec
{
    [TestClass]
    public class MelsecFxLinksTest
    {
        [TestMethod]
        public void AccTest( )
        {
            string msg = "00FFBR3ABCD";
            string msg2 = "00FFBR3ABCDBD";

            string cal = MelsecFxLinks.CalculateAcc( msg );

            Assert.IsTrue( msg2 == cal );
        }

        [TestMethod]
        public void buildCommandTest( )
        {
            byte[] right = new byte[] { 0x05, 0x30, 0x35, 0x46, 0x46, 0x42, 0x52, 0x41, 0x58, 0x30, 0x30, 0x34, 0x30, 0x30, 0x35, 0x34, 0x37 };

            OperateResult<byte[]> command = MelsecFxLinks.BuildReadCommand( 5, "X40", 5, true, true, 10 );

            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( command.Content, right ), HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ') );
        }

        [TestMethod]
        public void BuildWriteBoolCommandTest( )
        {
            byte[] right = new byte[] { 0x05, 0x30, 0x30, 0x46, 0x46, 0x42, 0x57, 0x30, 0x4D, 0x30, 0x39, 0x30, 0x33, 0x30, 0x35, 0x30, 0x31, 0x31, 0x30, 0x31, 0x32, 0x36 };
            OperateResult<byte[]> command = MelsecFxLinks.BuildWriteBoolCommand( 0, "M903", new bool[] { false, true, true, false, true }, true, 0 );

            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( command.Content, right ), HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
        }

        [TestMethod]
        public void BuildWriteWordCommandTest( )
        {
            byte[] right = new byte[] { 0x05, 0x30, 0x30, 0x46, 0x46, 0x57, 0x57, 0x30, 0x4D, 0x30, 0x36, 0x34, 0x30, 0x30, 0x32, 0x32, 0x33, 0x34, 0x37, 0x41, 0x42, 0x39, 0x36, 0x30, 0x35 };
            OperateResult<byte[]> command = MelsecFxLinks.BuildWriteByteCommand( 0, "M640", new byte[] { 0x47, 0x23, 0x96, 0xAB }, true, 0 );

            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( command.Content, right ), HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
        }

        [TestMethod]
        public void BuildStartCommandTest( )
        {
            byte[] right = new byte[] { 0x05, 0x30, 0x35, 0x46, 0x46, 0x52, 0x52, 0x30, 0x43, 0x35 };
            OperateResult<byte[]> command = MelsecFxLinks.BuildStart( 5, true, 0 );

            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( command.Content, right ), HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
        }

        [TestMethod]
        public void BuildStopCommandTest( )
        {
            byte[] right = new byte[] { 0x05, 0x30, 0x30, 0x46, 0x46, 0x52, 0x53, 0x30, 0x43, 0x31 };
            OperateResult<byte[]> command = MelsecFxLinks.BuildStop( 0, true, 0 );

            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( command.Content, right ), HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
        }
    }
}
