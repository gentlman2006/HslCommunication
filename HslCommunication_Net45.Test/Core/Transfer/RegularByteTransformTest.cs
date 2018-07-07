using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication;
using HslCommunication.Core;


namespace HslCommunication_Net45.Test.Core.Transfer
{
    [TestClass]
    public class RegularByteTransformTest
    {
        #region Constructor

        public RegularByteTransformTest( )
        {
            byteTransform = new RegularByteTransform( );
        }

        private RegularByteTransform byteTransform;

        #endregion


        [TestMethod]
        public void BoolTransferTest( )
        {
            byte[] data = new byte[2] { 0x01, 0x00 };
            Assert.AreEqual( true, byteTransform.TransBool( data, 0 ) );

            Assert.AreEqual( false, byteTransform.TransBool( data, 1 ) ); 
        }




    }
}
