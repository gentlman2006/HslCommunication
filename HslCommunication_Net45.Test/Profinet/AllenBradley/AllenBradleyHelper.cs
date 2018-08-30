using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication.Profinet.AllenBradley;

namespace HslCommunication_Net45.Test.Profinet.AllenBradley
{
    [TestClass]
    public class AllenBradleyHelperTest
    {

        [TestMethod]
        public void PackRequsetReadTest( )
        {
            byte[] corrent = new byte[] { 0x4c, 0x05, 0x91, 0x08, 0x53, 0x74, 0x61, 0x72, 0x74, 0x5f, 0x69, 0x6e, 0x01, 0x00 };

            byte[] buffer = AllenBradleyHelper.PackRequsetRead( "Start_in" );
            if (!HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( buffer, corrent ))
            {
                Assert.Fail( "指令失败：" + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( buffer ) );
            }
        }
    }
}
