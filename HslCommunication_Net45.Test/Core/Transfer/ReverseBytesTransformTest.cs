using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication.Core;

namespace HslCommunication_Net45.Test.Core.Transfer
{
    [TestClass]
    public class ReverseBytesTransformTest
    {
        #region Constructor

        public ReverseBytesTransformTest( )
        {
            byteTransform = new ReverseBytesTransform( );
        }

        private ReverseBytesTransform byteTransform;

        #endregion

        #region Bool Test

        [TestMethod]
        public void BoolTransferTest( )
        {
            byte[] data = new byte[2] { 0x01, 0x00 };
            Assert.AreEqual( true, byteTransform.TransBool( data, 0 ) );
            Assert.AreEqual( false, byteTransform.TransBool( data, 1 ) );
        }

        [TestMethod]
        public void ByteToBoolArrayTransferTest( )
        {
            byte[] data = new byte[2] { 0xA3, 0x46 };
            bool[] array = byteTransform.TransBool( data, 1, 1 );
            Assert.AreEqual( false, array[7] );
            Assert.AreEqual( true, array[6] );
            Assert.AreEqual( false, array[5] );
            Assert.AreEqual( false, array[4] );
            Assert.AreEqual( false, array[3] );
            Assert.AreEqual( true, array[2] );
            Assert.AreEqual( true, array[1] );
            Assert.AreEqual( false, array[0] );
        }


        [TestMethod]
        public void BoolArrayToByteTransferTest( )
        {
            byte[] data = new byte[2] { 0xA3, 0x46 };
            bool[] buffer = new bool[] { true, true, false, false, false, true, false, true, false, true, true, false, false, false, true, false };


            byte[] value = byteTransform.TransByte( buffer );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( data, value ) );
        }

        #endregion

        #region Int16 Test

        [TestMethod]
        public void BytesToInt16TransferTest( )
        {
            byte[] data = new byte[4];
            BitConverter.GetBytes( (short)1234 ).CopyTo( data, 0 );
            BitConverter.GetBytes( (short)-9876 ).CopyTo( data, 2 );
            Array.Reverse( data, 0, 2 );
            Array.Reverse( data, 2, 2 );


            short[] array = byteTransform.TransInt16( data, 0, 2 );
            Assert.AreEqual<short>( 1234, array[0] );
            Assert.AreEqual<short>( -9876, array[1] );
        }

        [TestMethod]
        public void Int16ToBytesTransferTest( )
        {
            byte[] data = new byte[4];
            BitConverter.GetBytes( (short)1234 ).CopyTo( data, 0 );
            BitConverter.GetBytes( (short)-9876 ).CopyTo( data, 2 );
            Array.Reverse( data, 0, 2 );
            Array.Reverse( data, 2, 2 );

            byte[] buffer = byteTransform.TransByte( new short[] { 1234, -9876 } );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( data, buffer ) );
        }

        #endregion

        #region UInt16 Test

        [TestMethod]
        public void BytesToUInt16TransferTest( )
        {
            byte[] data = new byte[4];
            BitConverter.GetBytes( (ushort)1234 ).CopyTo( data, 0 );
            BitConverter.GetBytes( (ushort)54321 ).CopyTo( data, 2 );
            Array.Reverse( data, 0, 2 );
            Array.Reverse( data, 2, 2 );

            ushort[] array = byteTransform.TransUInt16( data, 0, 2 );
            Assert.AreEqual<ushort>( 1234, array[0] );
            Assert.AreEqual<ushort>( 54321, array[1] );
        }


        [TestMethod]
        public void UInt16ToBytesTransferTest( )
        {
            byte[] data = new byte[4];
            BitConverter.GetBytes( (ushort)1234 ).CopyTo( data, 0 );
            BitConverter.GetBytes( (ushort)54321 ).CopyTo( data, 2 );
            Array.Reverse( data, 0, 2 );
            Array.Reverse( data, 2, 2 );

            byte[] buffer = byteTransform.TransByte( new ushort[] { 1234, 54321 } );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( data, buffer ) );
        }


        #endregion

        #region Int32 Test


        [TestMethod]
        public void BytesToInt32TransferTest( )
        {
            byte[] data = new byte[8];
            BitConverter.GetBytes( 12345678 ).CopyTo( data, 0 );
            BitConverter.GetBytes( -9876654 ).CopyTo( data, 4 );
            Array.Reverse( data, 0, 4 );
            Array.Reverse( data, 4, 4 );

            int[] array = byteTransform.TransInt32( data, 0, 2 );
            Assert.AreEqual<int>( 12345678, array[0] );
            Assert.AreEqual<int>( -9876654, array[1] );
        }

        [TestMethod]
        public void Int32ToBytesTransferTest( )
        {
            byte[] data = new byte[8];
            BitConverter.GetBytes( 12345678 ).CopyTo( data, 0 );
            BitConverter.GetBytes( -9876654 ).CopyTo( data, 4 );
            Array.Reverse( data, 0, 4 );
            Array.Reverse( data, 4, 4 );

            byte[] buffer = byteTransform.TransByte( new int[] { 12345678, -9876654 } );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( data, buffer ) );
        }

        #endregion

        #region UInt32 Test


        [TestMethod]
        public void BytesToUInt32TransferTest( )
        {
            byte[] data = new byte[8];
            BitConverter.GetBytes( (uint)12345678 ).CopyTo( data, 0 );
            BitConverter.GetBytes( (uint)9876654 ).CopyTo( data, 4 );
            Array.Reverse( data, 0, 4 );
            Array.Reverse( data, 4, 4 );

            uint[] array = byteTransform.TransUInt32( data, 0, 2 );
            Assert.AreEqual<uint>( 12345678, array[0] );
            Assert.AreEqual<uint>( 9876654, array[1] );
        }


        [TestMethod]
        public void UInt32ToBytesTransferTest( )
        {
            byte[] data = new byte[8];
            BitConverter.GetBytes( (uint)12345678 ).CopyTo( data, 0 );
            BitConverter.GetBytes( (uint)9876654 ).CopyTo( data, 4 );
            Array.Reverse( data, 0, 4 );
            Array.Reverse( data, 4, 4 );

            byte[] buffer = byteTransform.TransByte( new uint[] { 12345678, 9876654 } );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( data, buffer ) );
        }

        #endregion

        #region Int64 Test


        [TestMethod]
        public void BytesToInt64TransferTest( )
        {
            byte[] data = new byte[16];
            BitConverter.GetBytes( 12345678911234L ).CopyTo( data, 0 );
            BitConverter.GetBytes( -987665434123245L ).CopyTo( data, 8 );
            Array.Reverse( data, 0, 8 );
            Array.Reverse( data, 8, 8 );

            long[] array = byteTransform.TransInt64( data, 0, 2 );
            Assert.AreEqual<long>( 12345678911234L, array[0] );
            Assert.AreEqual<long>( -987665434123245L, array[1] );
        }

        [TestMethod]
        public void Int64ToBytesTransferTest( )
        {
            byte[] data = new byte[16];
            BitConverter.GetBytes( 12345678911234L ).CopyTo( data, 0 );
            BitConverter.GetBytes( -987665434123245L ).CopyTo( data, 8 );
            Array.Reverse( data, 0, 8 );
            Array.Reverse( data, 8, 8 );

            byte[] buffer = byteTransform.TransByte( new long[] { 12345678911234L, -987665434123245L } );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( data, buffer ) );
        }

        #endregion

        #region UInt64 Test


        [TestMethod]
        public void BytesToUInt64TransferTest( )
        {
            byte[] data = new byte[16];
            BitConverter.GetBytes( 1234567812123334123UL ).CopyTo( data, 0 );
            BitConverter.GetBytes( 92353421232423213UL ).CopyTo( data, 8 );
            Array.Reverse( data, 0, 8 );
            Array.Reverse( data, 8, 8 );

            ulong[] array = byteTransform.TransUInt64( data, 0, 2 );
            Assert.AreEqual<ulong>( 1234567812123334123UL, array[0] );
            Assert.AreEqual<ulong>( 92353421232423213UL, array[1] );
        }


        [TestMethod]
        public void UInt64ToBytesTransferTest( )
        {
            byte[] data = new byte[16];
            BitConverter.GetBytes( 1234567812123334123UL ).CopyTo( data, 0 );
            BitConverter.GetBytes( 92353421232423213UL ).CopyTo( data, 8 );
            Array.Reverse( data, 0, 8 );
            Array.Reverse( data, 8, 8 );

            byte[] buffer = byteTransform.TransByte( new ulong[] { 1234567812123334123UL, 92353421232423213UL } );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( data, buffer ) );
        }


        #endregion

        #region Float Test


        [TestMethod]
        public void BytesToFloatTransferTest( )
        {
            byte[] data = new byte[8];
            BitConverter.GetBytes( 123.456f ).CopyTo( data, 0 );
            BitConverter.GetBytes( -0.001234f ).CopyTo( data, 4 );
            Array.Reverse( data, 0, 4 );
            Array.Reverse( data, 4, 4 );

            float[] array = byteTransform.TransSingle( data, 0, 2 );
            Assert.AreEqual<float>( 123.456f, array[0] );
            Assert.AreEqual<float>( -0.001234f, array[1] );
        }

        [TestMethod]
        public void FloatToBytesTransferTest( )
        {
            byte[] data = new byte[8];
            BitConverter.GetBytes( 123.456f ).CopyTo( data, 0 );
            BitConverter.GetBytes( -0.001234f ).CopyTo( data, 4 );
            Array.Reverse( data, 0, 4 );
            Array.Reverse( data, 4, 4 );

            byte[] buffer = byteTransform.TransByte( new float[] { 123.456f, -0.001234f } );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( data, buffer ) );
        }


        #endregion

        #region Double Test


        [TestMethod]
        public void BytesToDoubleTransferTest( )
        {
            byte[] data = new byte[16];
            BitConverter.GetBytes( 123.456789D ).CopyTo( data, 0 );
            BitConverter.GetBytes( -0.00000123D ).CopyTo( data, 8 );
            Array.Reverse( data, 0, 8 );
            Array.Reverse( data, 8, 8 );

            double[] array = byteTransform.TransDouble( data, 0, 2 );
            Assert.AreEqual<double>( 123.456789D, array[0] );
            Assert.AreEqual<double>( -0.00000123D, array[1] );
        }

        [TestMethod]
        public void DoubleToBytesTransferTest( )
        {
            byte[] data = new byte[16];
            BitConverter.GetBytes( 123.456789D ).CopyTo( data, 0 );
            BitConverter.GetBytes( -0.00000123D ).CopyTo( data, 8 );
            Array.Reverse( data, 0, 8 );
            Array.Reverse( data, 8, 8 );

            byte[] buffer = byteTransform.TransByte( new double[] { 123.456789D, -0.00000123D } );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( data, buffer ) );
        }

        #endregion

        #region String Test


        [TestMethod]
        public void BytesToStringTransferTest( )
        {
            byte[] data = Encoding.ASCII.GetBytes( "ABCDEFG5" );

            string str = byteTransform.TransString( data, 0, 8, Encoding.ASCII );
            Assert.AreEqual<string>( "ABCDEFG5", str );
        }


        [TestMethod]
        public void StringToBytesTransferTest( )
        {
            byte[] data = Encoding.ASCII.GetBytes( "ABCDEFG5" );

            byte[] buffer = byteTransform.TransByte( "ABCDEFG5", Encoding.ASCII );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( data, buffer ) );
        }

        #endregion
    }
}
