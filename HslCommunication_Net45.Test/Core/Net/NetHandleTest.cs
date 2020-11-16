using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication;


namespace HslCommunication_Net45.Test.Core.Net
{
    /// <summary>
    /// NetHandle的测试类对象
    /// </summary>
    [TestClass]
    public class NetHandleTest
    {
        /// <summary>
        /// 实例化的方法测试
        /// </summary>
        [TestMethod]
        public void IsCreateSuccess( )
        {
            NetHandle netHandle = new NetHandle( 1, 1, 1 );
            Assert.AreEqual<byte>( 1, netHandle.CodeMajor );
            Assert.AreEqual<byte>( 1, netHandle.CodeMinor );
            Assert.AreEqual<ushort>( 1, netHandle.CodeIdentifier );
            Assert.AreEqual( 16842753, netHandle.CodeValue );



            netHandle = new NetHandle( 16842753 );
            Assert.AreEqual<byte>( 1, netHandle.CodeMajor );
            Assert.AreEqual<byte>( 1, netHandle.CodeMinor );
            Assert.AreEqual<ushort>( 1, netHandle.CodeIdentifier );
            Assert.AreEqual( 16842753, netHandle.CodeValue );
        }

        /// <summary>
        /// 增加的一个方法测试
        /// </summary>
        [TestMethod]
        public void AddTest( )
        {
            NetHandle netHandle = new NetHandle( 1, 1, 1 );


            netHandle++;
            Assert.AreEqual<byte>( netHandle.CodeMajor, 1 );
            Assert.AreEqual<byte>( netHandle.CodeMinor, 1 );
            Assert.AreEqual<ushort>( netHandle.CodeIdentifier, 2 );

            Assert.AreEqual( 16842754, netHandle.CodeValue );
        }

        /// <summary>
        /// 减小的一个方法测试
        /// </summary>
        [TestMethod]
        public void SubTest( )
        {
            NetHandle netHandle = new NetHandle( 1, 1, 1 );


            netHandle--;
            Assert.AreEqual<byte>( netHandle.CodeMajor, 1 );
            Assert.AreEqual<byte>( netHandle.CodeMinor, 1 );
            Assert.AreEqual<ushort>( netHandle.CodeIdentifier, 0 );

            Assert.AreEqual( 16842752, netHandle.CodeValue );
        }

    }
}
