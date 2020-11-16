using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication.BasicFramework;

namespace HslCommunication_Net45.Test.BasicFramework
{
    [TestClass]
    public class SoftBufferTest
    {
        [TestMethod]
        public void SoftBuffer1( )
        {
            SoftBuffer softBuffer = new SoftBuffer( 1000 );

            byte[] b1 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12, 0x36, 0xF2, 0x27 };
            softBuffer.SetBytes( b1, 367 );

            byte[] b2 = softBuffer.GetBytes( 367, b1.Length );
            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( b1, b2 ), "第一次" + SoftBasic.ByteToHexString( b2 ) );

            byte[] b3 = new byte[] { 0x12, 0xC6, 0x25, 0x3C, 0x42, 0x85, 0x5B, 0x05, 0x12, 0x87 };
            softBuffer.SetBytes( b3, 367 + b1.Length );

            byte[] b4 = softBuffer.GetBytes( 367 + b1.Length, b3.Length );
            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( b3, b4 ), "第二次" );

            byte[] b5 = SoftBasic.SpliceTwoByteArray( b1, b3 );
            byte[] b6 = softBuffer.GetBytes( 367, b1.Length + b3.Length );
            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( b5, b6 ), "第三次" );
        }

        [TestMethod]
        public void Int16Test( )
        {
            SoftBuffer softBuffer = new SoftBuffer( 1000 );
            softBuffer.SetValue( new short[] { 123, -123, 24567 }, 328 );

            short[] read = softBuffer.GetInt16( 328, 3 );
            Assert.IsTrue( read[0] == 123 && read[1] == -123 && read[2] == 24567 );
        }

        [TestMethod]
        public void UInt16Test( )
        {
            SoftBuffer softBuffer = new SoftBuffer( 1000 );
            softBuffer.SetValue( new ushort[] { 123, 42123, 24567 }, 328 );

            ushort[] read = softBuffer.GetUInt16( 328, 3 );
            Assert.IsTrue( read[0] == 123 && read[1] == 42123 && read[2] == 24567 );
        }

        [TestMethod]
        public void Int32Test( )
        {
            SoftBuffer softBuffer = new SoftBuffer( 1000 );
            softBuffer.SetValue( new int[] { 123456, -12345, 231412 }, 328 );

            int[] read = softBuffer.GetInt32( 328, 3 );
            Assert.IsTrue( read[0] == 123456 && read[1] == -12345 && read[2] == 231412 );
        }

        [TestMethod]
        public void UInt32Test( )
        {
            SoftBuffer softBuffer = new SoftBuffer( 1000 );
            softBuffer.SetValue( new uint[] { 123, 42123, 24567 }, 328 );

            uint[] read = softBuffer.GetUInt32( 328, 3 );
            Assert.IsTrue( read[0] == 123 && read[1] == 42123 && read[2] == 24567 );
        }

        [TestMethod]
        public void Int64Test( )
        {
            SoftBuffer softBuffer = new SoftBuffer( 1000 );
            softBuffer.SetValue( new long[] { 123456, -12345, 231412 }, 328 );

            long[] read = softBuffer.GetInt64( 328, 3 );
            Assert.IsTrue( read[0] == 123456 && read[1] == -12345 && read[2] == 231412 );
        }

        [TestMethod]
        public void UInt64Test( )
        {
            SoftBuffer softBuffer = new SoftBuffer( 1000 );
            softBuffer.SetValue( new ulong[] { 123, 42123, 24567 }, 328 );

            ulong[] read = softBuffer.GetUInt64( 328, 3 );
            Assert.IsTrue( read[0] == 123 && read[1] == 42123 && read[2] == 24567 );
        }

        [TestMethod]
        public void SingleTest( )
        {
            SoftBuffer softBuffer = new SoftBuffer( 1000 );
            softBuffer.SetValue( new float[] { 123456f, -12345f, 231412f }, 328 );

            float[] read = softBuffer.GetSingle( 328, 3 );
            Assert.IsTrue( read[0] == 123456f && read[1] == -12345f && read[2] == 231412f );
        }

        [TestMethod]
        public void DoubleTest( )
        {
            SoftBuffer softBuffer = new SoftBuffer( 1000 );
            softBuffer.SetValue( new double[] { 123456d, -12345d, 231412d }, 328 );

            double[] read = softBuffer.GetDouble( 328, 3 );
            Assert.IsTrue( read[0] == 123456d && read[1] == -12345d && read[2] == 231412d );
        }
    }
}
