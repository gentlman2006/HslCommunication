using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication;

namespace HslCommunication_Net45.Test.Core.Net
{
    [TestClass]
    public class HslProtocolTest
    {

        /// <summary>
        /// 指令的构建以及分析测试
        /// </summary>
        [TestMethod]
        public void CommandBuildAndAnalysis( )
        {
            Guid token = Guid.NewGuid( );

            byte[] b1 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12, 0x36, 0xF2, 0x27 };
            
        }

    }
}
