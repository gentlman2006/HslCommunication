using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HslCommunication_Net45.Test.BasicFramework
{
    [TestClass]
    public class SoftSecurityTest
    {
        [TestMethod]
        public void SoftSecurity( )
        {
            string content = "ansidhqiwk还是得阿斯达asihdISHDIHAS$%&@#*@#$*()";

            string encode = HslCommunication.BasicFramework.SoftSecurity.MD5Encrypt( content, "asdfuioA" );
            string content2 = HslCommunication.BasicFramework.SoftSecurity.MD5Decrypt( encode, "asdfuioA" );


            if (content != content2)
            {
                Assert.Fail( "加密解密失败" );
            }

        }



    }
}
