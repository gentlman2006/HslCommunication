using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication.Profinet.Panasonic;
using HslCommunication;

namespace HslCommunication_Net45.Test.Profinet.Panasonic
{
    [TestClass]
    public class PanasonicMewtocolTest
    {
        [TestMethod]
        public void BuildReadCommandTest( )
        {
            OperateResult<byte[]> read = PanasonicMewtocol.BuildReadCommand( 0xEE, "X1", 10 );
            Assert.IsTrue( read.IsSuccess, "Build read command failed" );

            string command = Encoding.ASCII.GetString( read.Content );
            string corrent = "%EE#RCCX00010010";

            Assert.IsTrue( corrent == command.Substring( 0, command.Length - 3 ), "data is not same" );
        }

    }
}
