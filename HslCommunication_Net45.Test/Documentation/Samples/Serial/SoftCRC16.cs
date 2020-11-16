using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HslCommunication_Net45.Test.Documentation.Samples.Serial
{
    public class SoftCRC16Example
    {

        public void Example( )
        {

            // 01 03 00 00 00 02 C4 0B
            // 01 03 04 00 00 00 00 FA 33

            #region Example1

            // 进行CRC校验，例如从modbus接收的数据 01 03 00 00 00 02 C4 0B

            bool check = HslCommunication.Serial.SoftCRC16.CheckCRC16(
                HslCommunication.BasicFramework.SoftBasic.HexStringToBytes( "01 03 00 00 00 02 C4 0B" ) );

            if (check)
            {
                Console.WriteLine( "check success!" );       // 此处success
            }
            else
            {
                Console.WriteLine( "check failed!" );
            }

            // 上述的代码是使用了多项式码 A0 01，检验成功，如果您的多项式不是这个，比如 B8 08 那么就需要按照如下的方式
            check = HslCommunication.Serial.SoftCRC16.CheckCRC16(
                HslCommunication.BasicFramework.SoftBasic.HexStringToBytes( "01 03 00 00 00 02 C4 0B" ), 0xB8, 0x08 );

            if (check)
            {
                Console.WriteLine( "check success!" );
            }
            else
            {
                Console.WriteLine( "check failed!" );    // 此处failed
            }

            #endregion

            #region Example2

            // 计算CRC码，比如我要给"01 03 00 00 00 02"增加crc校验

            byte[] buffer = HslCommunication.Serial.SoftCRC16.CRC16(
                HslCommunication.BasicFramework.SoftBasic.HexStringToBytes( "01 03 00 00 00 02" ) );

            // buffer 就是 "01 03 00 00 00 02 C4 0B" 然后就可以发送到modbus的串口了

            // 如果需要自己指定多项式码 B8 08
            buffer = HslCommunication.Serial.SoftCRC16.CRC16(
                HslCommunication.BasicFramework.SoftBasic.HexStringToBytes( "01 03 00 00 00 02" ), 0xB8, 0x08 );

            #endregion

        }

    }
}
