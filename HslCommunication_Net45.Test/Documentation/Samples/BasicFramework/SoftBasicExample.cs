using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;

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

            #endregion
        }

        public void AddArrayDataExample( )
        {
            #region AddArrayDataExample

            #endregion
        }

        public void ArrayExpandToLengthExample( )
        {
            #region ArrayExpandToLengthExample

            #endregion
        }

        public void ArrayExpandToLengthEvenExample( )
        {
            #region ArrayExpandToLengthEvenExample


            #endregion
        }

        public void IsTwoBytesEquelExample1( )
        {
            #region IsTwoBytesEquelExample1



            #endregion
        }

        public void IsTwoBytesEquelExample2( )
        {
            #region IsTwoBytesEquelExample2



            #endregion
        }

        public void IsTwoTokenEquelExample( )
        {
            #region IsTwoTokenEquelExample


            #endregion
        }

        public void GetEnumValuesExample( )
        {
            #region GetEnumValuesExample

            #endregion
        }

        public void GetValueFromJsonObjectExample( )
        {
            #region GetValueFromJsonObjectExample


            #endregion
        }

        public void JsonSetValueExample( )
        {
            #region JsonSetValueExample

            #endregion
        }

        public void GetExceptionMessageExample1( )
        {
            #region GetExceptionMessageExample1

            #endregion
        }

        public void GetExceptionMessageExample2( )
        {
            #region GetExceptionMessageExample2

            #endregion
        }

        public void ByteToHexStringExample1( )
        {
            #region ByteToHexStringExample1

            #endregion
        }

        public void ByteToHexStringExample2( )
        {
            #region ByteToHexStringExample2



            #endregion
        }
    }
}
