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
    public class SoftBasicTest
    {
        [TestMethod]
        public void GetSizeDescriptionTest( )
        {
            long value = 123564357;
            Assert.AreEqual( "117.84 Mb", SoftBasic.GetSizeDescription( value ) );
            value = 345;
            Assert.AreEqual( "345 B", SoftBasic.GetSizeDescription( value ) );
            value = 453268;
            Assert.AreEqual( "442.64 Kb", SoftBasic.GetSizeDescription( value ) );
            value = 452342343424;
            Assert.AreEqual( "421.28 Gb", SoftBasic.GetSizeDescription( value ) );
        }

        [TestMethod]
        public void AddArrayDataTest( )
        {
            byte[] b1 = new byte[] { 0x13, 0xA6, 0x15, 0x85 };
            byte[] b2 = new byte[] { 0x5B, 0x05, 0x12 };
            byte[] b3 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12 };
            byte[] b4 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B };

            SoftBasic.AddArrayData( ref b1, b2, 7 );
            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( b1, b3 ) );
        }

        [TestMethod]
        public void AddArrayDataTest2( )
        {
            byte[] b1 = new byte[] { 0x13, 0xA6, 0x15, 0x85 };
            byte[] b2 = new byte[] { 0x5B, 0x05, 0x12 };
            byte[] b3 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12 };
            byte[] b4 = new byte[] { 0x15, 0x85, 0x5B, 0x05, 0x12 };
            
            SoftBasic.AddArrayData( ref b1, b2, 5 );
            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( b1, b4 ) );
        }

        [TestMethod]
        public void ArrayExpandToLengthTest( )
        {
            byte[] b1 = new byte[] { 0x13, 0xA6, 0x15, 0x85 };
            byte[] b2 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x00, 0x00, 0x00 };

            byte[] b3 = SoftBasic.ArrayExpandToLength( b1, 7 );
            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( b2, b3 ) );
        }

        [TestMethod]
        public void ArrayExpandToLengthEvenTest( )
        {
            byte[] b1 = new byte[] { 0x13, 0xA6, 0x15 };
            byte[] b2 = new byte[] { 0x13, 0xA6, 0x15, 0x00 };

            byte[] b3 = SoftBasic.ArrayExpandToLengthEven( b1 );
            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( b2, b3 ) );

            b3 = SoftBasic.ArrayExpandToLengthEven( b3 );
            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( b2, b3 ) );
        }

        [TestMethod]
        public void IsTwoBytesEquelTest1( )
        {
            byte[] b1 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12, 0x36, 0xF2, 0x27 };
            byte[] b2 = new byte[] { 0x12, 0xC6, 0x25, 0x3C, 0x42, 0x85, 0x5B, 0x05, 0x12, 0x87 };

            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( b1, 3, b2, 5, 4 ) );
        }


        [TestMethod]
        public void IsTwoBytesEquelTest2( )
        {
            byte[] b1 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12, 0x36, 0xF2, 0x27 };
            byte[] b2 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12, 0x36, 0xF2, 0x27 };

            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( b1, b2) );
        }

        [TestMethod]
        public void IsTwoBytesEquelTest3( )
        {
            byte[] b1 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x13, 0x36, 0xF2, 0x27 };
            byte[] b2 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12, 0x36, 0xF2, 0x27 };

            Assert.IsFalse( SoftBasic.IsTwoBytesEquel( b1, b2 ) );
        }

        [TestMethod]
        public void IsByteTokenEquelTest( )
        {
            Guid guid = new Guid( "56b79cac-91e8-460f-95ce-72b39e19185e" );
            byte[] b2 = new byte[32];
            guid.ToByteArray( ).CopyTo( b2, 12 );

            Assert.IsTrue( SoftBasic.IsByteTokenEquel( b2, guid ) );
        }

        [TestMethod]
        public void IsTwoTokenEquelTest( )
        {
            Guid guid1 = new Guid( "56b79cac-91e8-460f-95ce-72b39e19185e" );
            Guid guid2 = new Guid( "56b79cac-91e8-460f-95ce-72b39e19185e" );

            Assert.IsTrue( SoftBasic.IsTwoTokenEquel( guid1, guid2 ) );
        }


    }



}
