using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication;

namespace HslCommunication_Net45.Test.Profinet.Melsec
{
    [TestClass]
    public class MelsecFxSerialTest
    {
        [TestMethod]
        public void BuildWriteBoolPacketTest( )
        {
            // 测试生成的指令是否是正确的
            byte[] corrent = new byte[] { 0x02, 0x37, 0x31, 0x33, 0x30, 0x35, 0x03, 0x30, 0x33 };
            OperateResult<byte[]> operateResult = HslCommunication.Profinet.Melsec.MelsecFxSerial.BuildWriteBoolPacket( "Y23", true );
            Assert.IsTrue( operateResult.IsSuccess, "Y23指令生成失败" );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( corrent, operateResult.Content ), "Y23指令校验失败" );

            corrent = new byte[] { 0x02, 0x37, 0x31, 0x31, 0x30, 0x38, 0x03, 0x30, 0x34 }; // 02 37 31 31 30 38 03 30 34
            operateResult = HslCommunication.Profinet.Melsec.MelsecFxSerial.BuildWriteBoolPacket( "M17", true );
            Assert.IsTrue( operateResult.IsSuccess, "M17指令生成失败" );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( corrent, operateResult.Content ), "M17指令校验失败" );

            corrent = new byte[] { 0x02, 0x38, 0x31, 0x31, 0x30, 0x38, 0x03, 0x30, 0x35 };
            operateResult = HslCommunication.Profinet.Melsec.MelsecFxSerial.BuildWriteBoolPacket( "M17", false );
            Assert.IsTrue( operateResult.IsSuccess, "M17指令生成失败" );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( corrent, operateResult.Content ), "M17指令校验失败" );
        }

        [TestMethod]
        public void BuildReadWordCommandTest( )
        {
            byte[] corrent = new byte[] { 0x02, 0x30, 0x30, 0x30, 0x41, 0x30, 0x30, 0x32, 0x03, 0x36, 0x36 };
            OperateResult<byte[],int> operateResult = HslCommunication.Profinet.Melsec.MelsecFxSerial.BuildReadBoolCommand( "Y0", 16 );
            Assert.IsTrue( operateResult.IsSuccess, "Y0指令生成失败" );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( corrent, operateResult.Content1 ), "Y23指令校验失败" );

            corrent = new byte[] { 0x02, 0x30, 0x31, 0x30, 0x46, 0x36, 0x30, 0x34, 0x03, 0x37, 0x34 };
            OperateResult<byte[]> read = HslCommunication.Profinet.Melsec.MelsecFxSerial.BuildReadWordCommand( "D123", 2 );
            Assert.IsTrue( operateResult.IsSuccess, "D123指令生成失败" );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( corrent, read.Content ), "D123指令校验失败" );
        }

        [TestMethod]
        public void BuildReadBoolCommandTest( )
        {
            byte[] corrent = new byte[] { 0x02, 0x30, 0x30, 0x31, 0x30, 0x31, 0x30, 0x31, 0x03, 0x35, 0x36 }; // 02 30 30 31 30 31 30 31 03 35 36
            OperateResult<byte[], int> operateResult = HslCommunication.Profinet.Melsec.MelsecFxSerial.BuildReadBoolCommand( "M14", 1 );
            Assert.IsTrue( operateResult.IsSuccess, "M14指令生成失败" );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( corrent, operateResult.Content1 ), "M14指令校验失败" );
;
        }

        [TestMethod]
        public void BuildWriteWordCommandTest( )
        {
            byte[] corrent = new byte[] { 0x02, 0x31, 0x31, 0x30, 0x46, 0x36, 0x30, 0x32, 0x30, 0x43, 0x30, 0x30, 0x03, 0x34, 0x36 };
            OperateResult<byte[]> operateResult = HslCommunication.Profinet.Melsec.MelsecFxSerial.BuildWriteWordCommand( "D123", BitConverter.GetBytes( (ushort)12 ) );
            Assert.IsTrue( operateResult.IsSuccess, "D123写入指令生成失败" );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( corrent, operateResult.Content ), "123写入指令校验失败 : " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( operateResult.Content, ' ' ) );
            
        }

        [TestMethod]
        public void ExtractActualBoolDataTest( )
        {
            OperateResult<bool[]> operate = HslCommunication.Profinet.Melsec.MelsecFxSerial.ExtractActualBoolData(
                HslCommunication.BasicFramework.SoftBasic.HexStringToBytes( "02 30 32 03 36 35" ), 1, 7 );

            Assert.IsTrue( operate.IsSuccess, "bool数组指定解析失败" );
            Assert.IsTrue( operate.Content[0] );
            Assert.IsFalse( operate.Content[1] );
            Assert.IsFalse( operate.Content[2] );
            Assert.IsFalse( operate.Content[3] );
            Assert.IsFalse( operate.Content[4] );
            Assert.IsFalse( operate.Content[5] );
            Assert.IsFalse( operate.Content[6] );
        }

    }
}
