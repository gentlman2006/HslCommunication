using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication.Core;
using HslCommunication;

namespace HslCommunication_Net45.Test.Core.Transfer
{
    [TestClass]
    public class ByteTransformHelperTest
    {
        #region Constructor

        /// <summary>
        /// constructor
        /// </summary>
        public ByteTransformHelperTest( )
        {
            byteTransform = new RegularByteTransform( );
        }

        private IByteTransform byteTransform;

        #endregion


        [TestMethod]
        public void GetBoolResultFromBytesTest( )
        {
            OperateResult<bool> result = ByteTransformHelper.GetBoolResultFromBytes(
                OperateResult.CreateSuccessResult( new byte[] { 0x01, 0x00 } ), byteTransform );

            Assert.IsTrue( result.IsSuccess && result.Content );
        }


        [TestMethod]
        public void GetByteResultFromBytesTest( )
        {
            OperateResult<byte> result = ByteTransformHelper.GetByteResultFromBytes(
                OperateResult.CreateSuccessResult( new byte[] { 0x21, 0xA0 } ), byteTransform );

            Assert.IsTrue( result.IsSuccess && result.Content == 0x21 );
        }



        [TestMethod]
        public void GetInt16ResultFromBytesTest( )
        {
            OperateResult<short> result = ByteTransformHelper.GetInt16ResultFromBytes(
                OperateResult.CreateSuccessResult( BitConverter.GetBytes( (short)12345 ) ), byteTransform );

            Assert.IsTrue( result.IsSuccess && result.Content == 12345 );
        }


        [TestMethod]
        public void GetUInt16ResultFromBytesTest( )
        {
            OperateResult<ushort> result = ByteTransformHelper.GetUInt16ResultFromBytes(
               OperateResult.CreateSuccessResult( BitConverter.GetBytes( (ushort)52345 ) ), byteTransform );

            Assert.IsTrue( result.IsSuccess && result.Content == 52345 );
        }


        [TestMethod]
        public void GetInt32ResultFromBytesTest( )
        {
            OperateResult<int> result = ByteTransformHelper.GetInt32ResultFromBytes(
               OperateResult.CreateSuccessResult( BitConverter.GetBytes( 523451234 ) ), byteTransform );

            Assert.IsTrue( result.IsSuccess && result.Content == 523451234 );
        }


        [TestMethod]
        public void GetUInt32ResultFromBytesTest( )
        {
            OperateResult<uint> result = ByteTransformHelper.GetUInt32ResultFromBytes(
               OperateResult.CreateSuccessResult( BitConverter.GetBytes( (uint)523451234 ) ), byteTransform );

            Assert.IsTrue( result.IsSuccess && result.Content == 523451234 );
        }


        [TestMethod]
        public void GetInt64ResultFromBytesTest( )
        {
            OperateResult<long> result = ByteTransformHelper.GetInt64ResultFromBytes(
               OperateResult.CreateSuccessResult( BitConverter.GetBytes( 5234512311231234L ) ), byteTransform );

            Assert.IsTrue( result.IsSuccess && result.Content == 5234512311231234L );
        }

        [TestMethod]
        public void GetUInt64ResultFromBytesTest( )
        {
            OperateResult<ulong> result = ByteTransformHelper.GetUInt64ResultFromBytes(
               OperateResult.CreateSuccessResult( BitConverter.GetBytes( 5234512311231234UL ) ), byteTransform );

            Assert.IsTrue( result.IsSuccess && result.Content == 5234512311231234UL );
        }


        [TestMethod]
        public void GetSingleResultFromBytesTest( )
        {
            OperateResult<float> result = ByteTransformHelper.GetSingleResultFromBytes(
               OperateResult.CreateSuccessResult( BitConverter.GetBytes( 123.456f ) ), byteTransform );

            Assert.IsTrue( result.IsSuccess && result.Content == 123.456f );
        }


        [TestMethod]
        public void GetDoubleResultFromBytesTest( )
        {
            OperateResult<double> result = ByteTransformHelper.GetDoubleResultFromBytes(
               OperateResult.CreateSuccessResult( BitConverter.GetBytes( 123.456d ) ), byteTransform );

            Assert.IsTrue( result.IsSuccess && result.Content == 123.456d );
        }

        [TestMethod]
        public void GetStringResultFromBytesTest( )
        {
            OperateResult<string> result = ByteTransformHelper.GetStringResultFromBytes(
              OperateResult.CreateSuccessResult( Encoding.ASCII.GetBytes( "asdqw123JSHDUA" ) ), byteTransform );

            Assert.IsTrue( result.IsSuccess && result.Content == "asdqw123JSHDUA" );
        }
    }
}
