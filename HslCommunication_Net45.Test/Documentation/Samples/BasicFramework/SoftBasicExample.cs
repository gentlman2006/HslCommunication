using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using Newtonsoft.Json.Linq;

namespace HslCommunication_Net45.Test.Documentation.Samples.BasicFramework
{
    public class SoftBasicExample
    {
        public void CalculateFileMD5Example( )
        {
            #region CalculateFileMD5Example

            try
            {
                string md5 = SoftBasic.CalculateFileMD5( "D:\\123.txt" );

                Console.WriteLine( md5 );
            }
            catch(Exception ex)
            {
                Console.WriteLine( "failed : " + ex.Message );
            }
            
            #endregion
            
        }

        public void CalculateStreamMD5Example1( )
        {
            #region CalculateStreamMD5Example1

            try
            {
                // stream 可以是文件流，网络流，内存流
                Stream stream = File.OpenRead( "D:\\123.txt" );

                string md5 = SoftBasic.CalculateStreamMD5( stream );

                Console.WriteLine( md5 );
            }
            catch (Exception ex)
            {
                Console.WriteLine( "failed : " + ex.Message );
            }

            #endregion
        }

        public void CalculateStreamMD5Example2( )
        {
            #region CalculateStreamMD5Example2

            try
            {
                Bitmap bitmap = new Bitmap( 100, 100 );

                string md5 = SoftBasic.CalculateStreamMD5( bitmap );

                Console.WriteLine( md5 );
            }
            catch (Exception ex)
            {
                Console.WriteLine( "failed : " + ex.Message );
            }

            #endregion
        }

        public void GetSizeDescriptionExample( )
        {
            #region GetSizeDescriptionExample

            string size = SoftBasic.GetSizeDescription( 1234254123 );

            // 1.15 Gb
            Console.WriteLine( size );


            #endregion
        }

        public void AddArrayDataExample( )
        {
            #region AddArrayDataExample

            int[] old = new int[5] { 1234, 1235, 1236, 1237, 1238 };
            int[] tmp = new int[2] { 456, 457 };


            SoftBasic.AddArrayData( ref old, tmp, 6 );
            foreach(var m in old)
            {
                Console.Write( m + " " );
            }

            // 输出 1235, 1236, 1237, 1238, 456, 457

            #endregion
        }

        public void ArrayExpandToLengthExample( )
        {
            #region ArrayExpandToLengthExample


            int[] old = new int[5] { 1234, 1235, 1236, 1237, 1238 };
            old = SoftBasic.ArrayExpandToLength( old, 8 );

            foreach (var m in old)
            {
                Console.Write( m + " " );
            }

            // 输出 1234, 1235, 1236, 1237, 1238, 0, 0, 0 

            #endregion
        }

        public void ArrayExpandToLengthEvenExample( )
        {
            #region ArrayExpandToLengthEvenExample

            int[] old = new int[5] { 1234, 1235, 1236, 1237, 1238 };
            old = SoftBasic.ArrayExpandToLengthEven( old );

            foreach (var m in old)
            {
                Console.Write( m + " " );
            }

            // 输出 1234, 1235, 1236, 1237, 1238, 0 


            #endregion
        }

        public void IsTwoBytesEquelExample1( )
        {
            #region IsTwoBytesEquelExample1

            byte[] b1 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12, 0x36, 0xF2, 0x27 };
            byte[] b2 = new byte[] { 0x12, 0xC6, 0x25, 0x3C, 0x42, 0x85, 0x5B, 0x05, 0x12, 0x87 };

            Console.WriteLine( SoftBasic.IsTwoBytesEquel( b1, 3, b2, 5, 4 ) );

            // 输出 true


            #endregion
        }

        public void IsTwoBytesEquelExample2( )
        {
            #region IsTwoBytesEquelExample2


            byte[] b1 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12, 0x36, 0xF2, 0x27 };
            byte[] b2 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12, 0x36, 0xF2, 0x27 };

            Console.WriteLine( SoftBasic.IsTwoBytesEquel( b1, b2 ) );

            // 输出 true

            #endregion
        }

        public void IsTwoTokenEquelExample( )
        {
            #region IsTwoTokenEquelExample

            Guid guid = new Guid( "56b79cac-91e8-460f-95ce-72b39e19185e" );
            byte[] b2 = new byte[32];
            guid.ToByteArray( ).CopyTo( b2, 12 );

            Console.WriteLine( SoftBasic.IsByteTokenEquel( b2, guid ) );

            // 输出 true

            #endregion
        }

        public void GetEnumValuesExample( )
        {
            #region GetEnumValuesExample

            System.IO.FileMode[] modes = SoftBasic.GetEnumValues<System.IO.FileMode>( );

            foreach (var m in modes)
            {
                Console.WriteLine( m );
            }

            // 输出
            // Append
            // Create
            // CreateNew
            // Open
            // OpenOrCreate
            // Truncate

            #endregion
        }

        public void GetValueFromJsonObjectExample( )
        {
            #region GetValueFromJsonObjectExample

            JObject json = new JObject( );
            json.Add( "A", new JValue( "Abcdea234a" ) );

            Console.WriteLine( "Abcdea234a", SoftBasic.GetValueFromJsonObject( json, "A", "" ) );

            // 输出 true

            #endregion
        }

        public void JsonSetValueExample( )
        {
            #region JsonSetValueExample

            JObject json = new JObject( );
            json.Add( "A", new JValue( "Abcdea234a" ) );

            SoftBasic.JsonSetValue( json, "B", "1234" );
            // json  A:Abcdea234a B:1234


            #endregion
        }

        public void GetExceptionMessageExample1( )
        {
            #region GetExceptionMessageExample1

            try
            {
                int i = 0;
                int j = 10 / i;
            }
            catch(Exception ex)
            {
                Console.WriteLine( SoftBasic.GetExceptionMessage( ex ) );
            }

            #endregion
        }

        public void GetExceptionMessageExample2( )
        {
            #region GetExceptionMessageExample2

            try
            {
                int i = 0;
                int j = 10 / i;
            }
            catch (Exception ex)
            {
                Console.WriteLine( "Msg", SoftBasic.GetExceptionMessage( ex ) );
            }

            #endregion
        }

        public void ByteToHexStringExample1( )
        {
            #region ByteToHexStringExample1

            byte[] b1 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12, 0x36, 0xF2, 0x27 };
            Console.WriteLine( SoftBasic.ByteToHexString( b1, ' ' ) );

            // 输出 "13 A6 15 85 5B 05 12 36 F2 27";

            #endregion
        }

        public void ByteToHexStringExample2( )
        {
            #region ByteToHexStringExample2

            byte[] b1 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12, 0x36, 0xF2, 0x27 };

            Console.WriteLine( SoftBasic.ByteToHexString( b1 ) );

            // 输出 "13A615855B051236F227";

            #endregion
        }

        public void HexStringToBytesExample( )
        {
            #region HexStringToBytesExample
            
            // str无论是下面哪种情况，都是等效的
            string str = "13 A6 15 85 5B 05 12 36 F2 27";
            //string str = "13-A6-15-85-5B-05-12-36-F2-27";
            //string str = "13A615855B051236F227";
            //string str = "13_A6_15_85_5B_05_12_36_F2_27";

            byte[] b1 = SoftBasic.HexStringToBytes( str );
            // b1 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12, 0x36, 0xF2, 0x27 };

           


            #endregion
        }

        public void BoolArrayToByteExample( )
        {
            #region BoolArrayToByte

            bool[] values = new bool[] { true, false, false, true, true, true, false, true, false, false, false, true, false, true, false, false };


            byte[] buffer = SoftBasic.BoolArrayToByte( values ) ;

            // 结果如下
            // buffer = new byte[2] { 0xB9, 0x28 };


            #endregion
        }

        public void ByteToBoolArrayExample( )
        {
            #region ByteToBoolArray

            byte[] buffer = new byte[2] { 0xB9, 0x28 };
            bool[] result = SoftBasic.ByteToBoolArray( buffer, 15 );

            // 结果如下
            // result = new bool[] { true, false, false, true, true, true, false, true, false, false, false, true, false, true, false };

            #endregion
        }

        public void SpliceTwoByteArrayExample( )
        {
            #region SpliceTwoByteArray
            
            byte[] b1 = new byte[] { 0x13, 0xA6, 0x15, 0x85 };
            byte[] b2 = new byte[] { 0x5B, 0x05, 0x12 };

            byte[] buffer = SoftBasic.SpliceTwoByteArray( b1, b2 );

            // buffer 的值就是 new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12 };

            #endregion
        }

        public void BytesArrayRemoveBeginExample( )
        {
            #region BytesArrayRemoveBegin

            byte[] b1 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12 };

            byte[] buffer = SoftBasic.BytesArrayRemoveBegin( b1, 3 );


            // buffer 的值就是b1移除了前三个字节 new byte[] { 0x85, 0x5B, 0x05, 0x12 };

            #endregion
        }

        public void BytesArrayRemoveLastExample( )
        {
            #region BytesArrayRemoveLast

            byte[] b1 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12 };

            byte[] buffer = SoftBasic.BytesArrayRemoveLast( b1, 4 );

            // buffer 的值就是b1移除了后四个字节 new byte[] { 0x13, 0xA6, 0x15 };

            #endregion
        }


        public void BytesArrayRemoveDoubleExample( )
        {
            #region BytesArrayRemoveDouble

            byte[] b1 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12 };

            byte[] buffer = SoftBasic.BytesArrayRemoveDouble( b1, 1, 3 );

            // buffer的值就是移除了第一个字节数据和最后两个字节数据的新值 new byte[] { 0xA6, 0x15, 0x85 };

            #endregion
        }

        public void DeepCloneExample( )
        {
            #region DeepClone


            SystemVersion version1 = new SystemVersion( "1.2.3" );
            SystemVersion version2 = (SystemVersion)SoftBasic.DeepClone( version1 );

            // 这两个版本号的值是一致的，但是属于不同的对象

            #endregion
        }
        
        public void GetUniqueStringByGuidAndRandomExample( )
        {

            #region GetUniqueStringByGuidAndRandom

            string uid = SoftBasic.GetUniqueStringByGuidAndRandom( );

            // 例子，随机的一串数字，重复概率几乎为0，长度为36位字节
            // ed28ea220cd34fea9fdd07a926be757d4562

            #endregion
        }

        public void BytesReverseByWordExample( )
        {
            #region BytesReverseByWord

            byte[] b1 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05, 0x12 };


            byte[] buffer = SoftBasic.BytesReverseByWord( b1 );

            // buffer的值就为 = new byte[] { 0xA6, 0x13, 0x85, 0x15, 0x05, 0x5B, 0x00, 0x12 };

            // 再举个例子

            byte[] b2 = new byte[] { 0x13, 0xA6, 0x15, 0x85, 0x5B, 0x05 };
            
            byte[] buffer2 = SoftBasic.BytesReverseByWord( b1 );

            // buffer2的值就是 = new byte[] { 0xA6, 0x13, 0x85, 0x15, 0x05, 0x5B };

            #endregion
        }
    }
}
