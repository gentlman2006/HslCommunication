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
    public class SoftZippedTest
    {
        [TestMethod]
        public void SoftZipped1Test( )
        {
            byte[] b1 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12, 0x36, 0xF2, 0x27 };
            byte[] b2 = SoftZipped.CompressBytes( b1 );
            byte[] b3 = SoftZipped.Decompress( b2 );

            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( b1, b3 ) );

        }
    }
}
