using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication.Core;
using HslCommunication.BasicFramework;

namespace HslCommunication_Net45.Test.Core.Transfer
{
    [TestClass]
    public class ReverseWordTransformTest
    {
        #region Constructor

        public ReverseWordTransformTest( )
        {
            byteTransform = new ReverseWordTransform( );
        }

        protected ReverseWordTransform byteTransform;


        /// <summary>
        /// 按照字节错位的方法
        /// </summary>
        /// <param name="buffer">实际的字节数据</param>
        /// <param name="index">起始字节位置</param>
        /// <param name="length">数据长度</param>
        /// <param name="isReverse">是否按照字来反转</param>
        /// <returns></returns>
        private byte[] ReverseBytesByWord( byte[] buffer, int index, int length, DataFormat dataFormat )
        {
            byte[] tmp = new byte[length];

            for (int i = 0; i < length; i++)
            {
                tmp[i] = buffer[index + i];
            }
            
            if (tmp.Length == 4)
            {
                
                if (dataFormat == DataFormat.CDAB)
                {
                    byte a = tmp[0];
                    tmp[0] = tmp[1];
                    tmp[1] = a;


                    byte b = tmp[2];
                    tmp[2] = tmp[3];
                    tmp[3] = b;
                }
                else if (dataFormat == DataFormat.BADC)
                {
                    byte a = tmp[0];
                    tmp[0] = tmp[2];
                    tmp[2] = a;


                    byte b = tmp[1];
                    tmp[1] = tmp[3];
                    tmp[3] = b;
                }
                else if(dataFormat == DataFormat.ABCD)
                {
                    byte a = tmp[0];
                    tmp[0] = tmp[3];
                    tmp[3] = a;

                    byte b = tmp[1];
                    tmp[1] = tmp[2];
                    tmp[2] = b;
                }
            }
            else if (tmp.Length == 8)
            {
                if (dataFormat == DataFormat.CDAB)
                {
                    byte a = tmp[0];
                    tmp[0] = tmp[1];
                    tmp[1] = a;


                    byte b = tmp[2];
                    tmp[2] = tmp[3];
                    tmp[3] = b;

                    a = tmp[4];
                    tmp[4] = tmp[5];
                    tmp[5] = a;

                    a = tmp[6];
                    tmp[6] = tmp[7];
                    tmp[7] = a;
                }
                else if (dataFormat == DataFormat.BADC)
                {
                    byte a = tmp[0];
                    tmp[0] = tmp[6];
                    tmp[6] = a;


                    a = tmp[1];
                    tmp[1] = tmp[7];
                    tmp[7] = a;

                    a = tmp[2];
                    tmp[2] = tmp[4];
                    tmp[4] = a;

                    a = tmp[3];
                    tmp[3] = tmp[5];
                    tmp[5] = a;
                }
                else if (dataFormat == DataFormat.ABCD)
                {
                    byte a = tmp[0];
                    tmp[0] = tmp[7];
                    tmp[7] = a;

                    a = tmp[1];
                    tmp[1] = tmp[6];
                    tmp[6] = a;

                    a = tmp[2];
                    tmp[2] = tmp[5];
                    tmp[5] = a;

                    a = tmp[3];
                    tmp[3] = tmp[4];
                    tmp[4] = a;
                }
            }
            else
            {
                for (int i = 0; i < length / 2; i++)
                {
                    byte b = tmp[i * 2 + 0];
                    tmp[i * 2 + 0] = tmp[i * 2 + 1];
                    tmp[i * 2 + 1] = b;
                }
            }

            return tmp;
        }

        private byte[] ReverseBytesByWord( byte[] buffer, DataFormat dataFormat )
        {
            return ReverseBytesByWord( buffer, 0, buffer.Length, dataFormat );
        }

        #endregion

        #region ReverseByteByWordTest

        [TestMethod]
        public void ReverseBytesByWordTest1( )
        {
            byte[] data = new byte[4] { 0x46, 0x38, 0xA0, 0xB0 };
            byte[] buffer = ReverseBytesByWord( data, DataFormat.ABCD );

            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( new byte[] { 0xB0, 0xA0, 0x38, 0x46  }, buffer ) );
        }

        [TestMethod]
        public void ReverseBytesByWordTest2( )
        {
            byte[] data = new byte[4] { 0x46, 0x38, 0xA0, 0xB0 };
            byte[] buffer = ReverseBytesByWord( data, DataFormat.BADC );

            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( new byte[] { 0xA0, 0xB0, 0x46, 0x38  }, buffer ) );
        }

        [TestMethod]
        public void ReverseBytesByWordTest3( )
        {
            byte[] data = new byte[4] { 0x46, 0x38, 0xA0, 0xB0 };
            byte[] buffer = ReverseBytesByWord( data, DataFormat.CDAB );

            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( new byte[] { 0x38, 0x46, 0xB0, 0xA0 }, buffer ) );
        }

        [TestMethod]
        public void ReverseBytesByWordTest4( )
        {
            byte[] data = new byte[4] { 0x46, 0x38, 0xA0, 0xB0 };
            byte[] buffer = ReverseBytesByWord( data, DataFormat.DCBA );

            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( new byte[] { 0x46, 0x38, 0xA0, 0xB0 }, buffer ) );
        }

        [TestMethod]
        public void ReverseBytesByWordTest5( )
        {
            byte[] data = new byte[8] { 0x46, 0x38, 0xA0, 0xB0, 0xFF, 0x3D, 0xC1, 0x08 };
            byte[] buffer = ReverseBytesByWord( data, DataFormat.DCBA );

            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( new byte[] { 0x46, 0x38, 0xA0, 0xB0, 0xFF, 0x3D, 0xC1, 0x08 }, buffer ),
                "Data:"+SoftBasic.ByteToHexString(buffer)+ " Actual:"+SoftBasic.ByteToHexString( new byte[] { 0x08, 0xC1, 0x3D, 0xFF, 0xB0, 0xA0, 0x38, 0x46 } ) );
        }


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
            data = SoftBasic.BytesReverseByWord( data );


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
            data = SoftBasic.BytesReverseByWord( data );

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
            data = SoftBasic.BytesReverseByWord( data );

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
            data = SoftBasic.BytesReverseByWord( data );

            byte[] buffer = byteTransform.TransByte( new ushort[] { 1234, 54321 } );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( data, buffer ) );
        }


        #endregion

        #region Int32 Test


        [TestMethod]
        public void BytesToInt32TransferTest( )
        {
            byte[] data = new byte[8];
            ReverseBytesByWord( BitConverter.GetBytes( 12345678 ), byteTransform.DataFormat ).CopyTo( data, 0 );
            ReverseBytesByWord( BitConverter.GetBytes( -9876654 ), byteTransform.DataFormat ).CopyTo( data, 4 );

            int[] array = byteTransform.TransInt32( data, 0, 2 );
            Assert.AreEqual<int>( 12345678, array[0] );
            Assert.AreEqual<int>( -9876654, array[1] );
        }

        [TestMethod]
        public void Int32ToBytesTransferTest( )
        {
            byte[] data = new byte[8];
            ReverseBytesByWord( BitConverter.GetBytes( 12345678 ), byteTransform.DataFormat ).CopyTo( data, 0 );
            ReverseBytesByWord( BitConverter.GetBytes( -9876654 ), byteTransform.DataFormat ).CopyTo( data, 4 );

            byte[] buffer = byteTransform.TransByte( new int[] { 12345678, -9876654 } );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( data, buffer ) );
        }

        #endregion

        #region UInt32 Test


        [TestMethod]
        public void BytesToUInt32TransferTest( )
        {
            byte[] data = new byte[8];
            ReverseBytesByWord( BitConverter.GetBytes( (uint)12345678 ), byteTransform.DataFormat ).CopyTo( data, 0 );
            ReverseBytesByWord( BitConverter.GetBytes( (uint)9876654 ), byteTransform.DataFormat ).CopyTo( data, 4 );

            uint[] array = byteTransform.TransUInt32( data, 0, 2 );
            Assert.AreEqual<uint>( 12345678, array[0] );
            Assert.AreEqual<uint>( 9876654, array[1] );
        }


        [TestMethod]
        public void UInt32ToBytesTransferTest( )
        {
            byte[] data = new byte[8];
            ReverseBytesByWord( BitConverter.GetBytes( (uint)12345678 ), byteTransform.DataFormat ).CopyTo( data, 0 );
            ReverseBytesByWord( BitConverter.GetBytes( (uint)9876654 ), byteTransform.DataFormat ).CopyTo( data, 4 );

            byte[] buffer = byteTransform.TransByte( new uint[] { 12345678, 9876654 } );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( data, buffer ) );
        }

        #endregion

        #region Int64 Test


        [TestMethod]
        public void BytesToInt64TransferTest( )
        {
            byte[] data = new byte[16];
            ReverseBytesByWord( BitConverter.GetBytes( 12345678911234L ), byteTransform.DataFormat ).CopyTo( data, 0 );
            ReverseBytesByWord( BitConverter.GetBytes( -987665434123245L ), byteTransform.DataFormat ).CopyTo( data, 8 );

            long[] array = byteTransform.TransInt64( data, 0, 2 );
            Assert.AreEqual<long>( 12345678911234L, array[0] );
            Assert.AreEqual<long>( -987665434123245L, array[1] );
        }

        [TestMethod]
        public void Int64ToBytesTransferTest( )
        {
            byte[] data = new byte[16];
            ReverseBytesByWord( BitConverter.GetBytes( 12345678911234L ), byteTransform.DataFormat ).CopyTo( data, 0 );
            ReverseBytesByWord( BitConverter.GetBytes( -987665434123245L ), byteTransform.DataFormat ).CopyTo( data, 8 );

            byte[] buffer = byteTransform.TransByte( new long[] { 12345678911234L, -987665434123245L } );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( data, buffer ) );
        }

        #endregion

        #region UInt64 Test


        [TestMethod]
        public void BytesToUInt64TransferTest( )
        {
            byte[] data = new byte[16];
            ReverseBytesByWord( BitConverter.GetBytes( 1234567812123334123UL ), byteTransform.DataFormat ).CopyTo( data, 0 );
            ReverseBytesByWord( BitConverter.GetBytes( 92353421232423213UL ), byteTransform.DataFormat ).CopyTo( data, 8 );

            ulong[] array = byteTransform.TransUInt64( data, 0, 2 );
            Assert.AreEqual<ulong>( 1234567812123334123UL, array[0] );
            Assert.AreEqual<ulong>( 92353421232423213UL, array[1] );
        }


        [TestMethod]
        public void UInt64ToBytesTransferTest( )
        {
            byte[] data = new byte[16];
            ReverseBytesByWord( BitConverter.GetBytes( 1234567812123334123UL ), byteTransform.DataFormat ).CopyTo( data, 0 );
            ReverseBytesByWord( BitConverter.GetBytes( 92353421232423213UL ), byteTransform.DataFormat ).CopyTo( data, 8 );

            byte[] buffer = byteTransform.TransByte( new ulong[] { 1234567812123334123UL, 92353421232423213UL } );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( data, buffer ) );
        }


        #endregion

        #region Float Test


        [TestMethod]
        public void BytesToFloatTransferTest( )
        {
            byte[] data = new byte[8];
            ReverseBytesByWord( BitConverter.GetBytes( 123.456f ), byteTransform.DataFormat ).CopyTo( data, 0 );
            ReverseBytesByWord( BitConverter.GetBytes( -0.001234f ), byteTransform.DataFormat ).CopyTo( data, 4 );

            float[] array = byteTransform.TransSingle( data, 0, 2 );
            Assert.AreEqual<float>( 123.456f, array[0] );
            Assert.AreEqual<float>( -0.001234f, array[1] );
        }

        [TestMethod]
        public void FloatToBytesTransferTest( )
        {
            byte[] data = new byte[8];
            ReverseBytesByWord( BitConverter.GetBytes( 123.456f ), byteTransform.DataFormat ).CopyTo( data, 0 );
            ReverseBytesByWord( BitConverter.GetBytes( -0.001234f ), byteTransform.DataFormat ).CopyTo( data, 4 );

            byte[] buffer = byteTransform.TransByte( new float[] { 123.456f, -0.001234f } );
            Assert.IsTrue( HslCommunication.BasicFramework.SoftBasic.IsTwoBytesEquel( data, buffer ) );
        }


        #endregion

        #region Double Test


        [TestMethod]
        public void BytesToDoubleTransferTest( )
        {
            byte[] data = new byte[16];
            ReverseBytesByWord( BitConverter.GetBytes( 123.456789D ), byteTransform.DataFormat ).CopyTo( data, 0 );
            ReverseBytesByWord( BitConverter.GetBytes( -0.00000123D ), byteTransform.DataFormat ).CopyTo( data, 8 );

            double[] array = byteTransform.TransDouble( data, 0, 2 );
            Assert.AreEqual<double>( 123.456789D, array[0] );
            Assert.AreEqual<double>( -0.00000123D, array[1] );
        }

        [TestMethod]
        public void DoubleToBytesTransferTest( )
        {
            byte[] data = new byte[16];
            ReverseBytesByWord( BitConverter.GetBytes( 123.456789D ), byteTransform.DataFormat ).CopyTo( data, 0 );
            ReverseBytesByWord( BitConverter.GetBytes( -0.00000123D ), byteTransform.DataFormat ).CopyTo( data, 8 );

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
