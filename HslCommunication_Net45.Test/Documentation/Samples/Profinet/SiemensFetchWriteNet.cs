using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.Profinet.Siemens;
using HslCommunication;

namespace HslCommunication_Net45.Test.Documentation.Samples.Profinet
{
    public class SiemensFetchWriteNetExample
    {


        public void ClassTest( )
        {
            #region Usage

            // 实例化对象，指定PLC的ip地址
            SiemensFetchWriteNet siemens = new SiemensFetchWriteNet( " 192.168.1.110", 2000 );
            // 举例读取M100的值
            short M100 = siemens.ReadInt16( "M100" ).Content;

            #endregion
        }

        public void ClassTest2( )
        {
            #region Usage2

            // 实例化对象，指定PLC的ip地址和端口号
            SiemensFetchWriteNet siemens = new SiemensFetchWriteNet( " 192.168.1.110", 2000 );

            // 连接对象
            OperateResult connect = siemens.ConnectServer( );
            if (!connect.IsSuccess)
            {
                Console.WriteLine( "connect failed:" + connect.Message );
                return;
            }

            // 举例读取M100的值
            short M100 = siemens.ReadInt16( "M100" ).Content;

            siemens.ConnectClose( );

            #endregion
        }


        public void ReadExample( )
        {
            #region ReadExample1


            SiemensFetchWriteNet siemensTcpNet = new SiemensFetchWriteNet( " 192.168.1.110", 2000 );

            // 此处以M100寄存器作为示例
            byte byte_M100 = siemensTcpNet.ReadByte( "M100" ).Content; // 读取M100的值
            short short_M100 = siemensTcpNet.ReadInt16( "M100" ).Content; // 读取M100-M101组成的字
            ushort ushort_M100 = siemensTcpNet.ReadUInt16( "M100" ).Content; // 读取M100-M101组成的无符号的值
            int int_M100 = siemensTcpNet.ReadInt32( "M100" ).Content;         // 读取M100-M103组成的有符号的数据
            uint uint_M100 = siemensTcpNet.ReadUInt32( "M100" ).Content;      // 读取M100-M103组成的无符号的值
            float float_M100 = siemensTcpNet.ReadFloat( "M100" ).Content;   // 读取M100-M103组成的单精度值
            long long_M100 = siemensTcpNet.ReadInt64( "M100" ).Content;      // 读取M100-M107组成的大数据值
            ulong ulong_M100 = siemensTcpNet.ReadUInt64( "M100" ).Content;   // 读取M100-M107组成的无符号大数据
            double double_M100 = siemensTcpNet.ReadDouble( "M100" ).Content; // 读取M100-M107组成的双精度值
            string str_M100 = siemensTcpNet.ReadString( "M100", 10 ).Content;// 读取M100-M109组成的ASCII字符串数据

            // 读取数组
            short[] short_M100_array = siemensTcpNet.ReadInt16( "M100", 10 ).Content; // 读取M100-M101组成的字
            ushort[] ushort_M100_array = siemensTcpNet.ReadUInt16( "M100", 10 ).Content; // 读取M100-M101组成的无符号的值
            int[] int_M100_array = siemensTcpNet.ReadInt32( "M100", 10 ).Content;         // 读取M100-M103组成的有符号的数据
            uint[] uint_M100_array = siemensTcpNet.ReadUInt32( "M100", 10 ).Content;      // 读取M100-M103组成的无符号的值
            float[] float_M100_array = siemensTcpNet.ReadFloat( "M100", 10 ).Content;   // 读取M100-M103组成的单精度值
            long[] long_M100_array = siemensTcpNet.ReadInt64( "M100", 10 ).Content;      // 读取M100-M107组成的大数据值
            ulong[] ulong_M100_array = siemensTcpNet.ReadUInt64( "M100", 10 ).Content;   // 读取M100-M107组成的无符号大数据
            double[] double_M100_array = siemensTcpNet.ReadDouble( "M100", 10 ).Content; // 读取M100-M107组成的双精度值

            #endregion
        }

        public void ReadExample2( )
        {
            #region ReadExample2

            SiemensFetchWriteNet siemens = new SiemensFetchWriteNet( " 192.168.1.110", 2000 );

            OperateResult<byte[]> read = siemens.Read( "M100", 8 );
            if (read.IsSuccess)
            {
                float temp = siemens.ByteTransform.TransInt16( read.Content, 0 ) / 10f;
                float press = siemens.ByteTransform.TransInt16( read.Content, 2 ) / 100f;
                int count = siemens.ByteTransform.TransInt32( read.Content, 6 );

                // do something
            }
            else
            {
                // failed
            }


            #endregion
        }

        public void WriteExample( )
        {
            #region WriteExample1

            SiemensFetchWriteNet siemens = new SiemensFetchWriteNet( " 192.168.1.110", 2000 );

            // 此处以M100寄存器作为示例
            siemens.Write( "M100", (short)1234 );                // 写入M100  short值  ,W3C0,R3C0 效果是一样的
            siemens.Write( "M100", (ushort)45678 );              // 写入M100  ushort值
            siemens.Write( "M100", 1234566 );                    // 写入M100  int值
            siemens.Write( "M100", (uint)1234566 );               // 写入M100  uint值
            siemens.Write( "M100", 123.456f );                    // 写入M100  float值
            siemens.Write( "M100", 123.456d );                    // 写入M100  double值
            siemens.Write( "M100", 123456661235123534L );          // 写入M100  long值
            siemens.Write( "M100", 523456661235123534UL );          // 写入M100  ulong值
            siemens.Write( "M100", "K123456789" );                // 写入M100  string值

            // 读取数组
            siemens.Write( "M100", new short[] { 123, 3566, -123 } );                // 写入M100  short值  ,W3C0,R3C0 效果是一样的
            siemens.Write( "M100", new ushort[] { 12242, 42321, 12323 } );              // 写入M100  ushort值
            siemens.Write( "M100", new int[] { 1234312312, 12312312, -1237213 } );                    // 写入M100  int值
            siemens.Write( "M100", new uint[] { 523123212, 213, 13123 } );               // 写入M100  uint值
            siemens.Write( "M100", new float[] { 123.456f, 35.3f, -675.2f } );                    // 写入M100  float值
            siemens.Write( "M100", new double[] { 12343.542312d, 213123.123d, -231232.53432d } );                    // 写入M100  double值
            siemens.Write( "M100", new long[] { 1231231242312, 34312312323214, -1283862312631823 } );          // 写入M100  long值
            siemens.Write( "M100", new ulong[] { 1231231242312, 34312312323214, 9731283862312631823 } );          // 写入M100  ulong值

            #endregion
        }

        public void WriteExample2( )
        {
            #region WriteExample2

            SiemensFetchWriteNet siemens = new SiemensFetchWriteNet( " 192.168.1.110", 2000 );

            // 拼凑数据，这样的话，一次通讯就完成数据的全部写入
            byte[] buffer = new byte[8];
            siemens.ByteTransform.TransByte( (short)1234 ).CopyTo( buffer, 0 );
            siemens.ByteTransform.TransByte( (short)2100 ).CopyTo( buffer, 2 );
            siemens.ByteTransform.TransByte( 12353423 ).CopyTo( buffer, 4 );

            OperateResult write = siemens.Write( "M100", buffer );
            if (write.IsSuccess)
            {
                // success
            }
            else
            {
                // failed
            }

            // 上面的功能等同于三个数据分别写入，下面的性能更差点，因为进行了三次通讯，而且每次还要判断是否写入成功
            //siemens.Write( "M100", (short)1234 );
            //siemens.Write( "M100", (short)2100 );
            //siemens.Write( "M100", 12353423 );

            #endregion
        }

        


    }
}
