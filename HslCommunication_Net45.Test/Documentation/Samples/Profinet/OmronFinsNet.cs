using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication;
using HslCommunication.Profinet.Omron;

namespace HslCommunication_Net45.Test.Documentation.Samples.Profinet
{
    public class OmronFinsNetExample
    {


        public void ClassTest( )
        {
            #region Usage

            // 实例化对象，指定PLC的ip地址和端口号
            OmronFinsNet omronFinsNet = new OmronFinsNet( "192.168.1.110", 6000 );
            omronFinsNet.SA1 = 10;
            omronFinsNet.DA1 = 110;
            // 举例读取D100的值
            short D100 = omronFinsNet.ReadInt16( "D100" ).Content;

            #endregion
        }

        public void ClassTest2( )
        {
            #region Usage2

            // 实例化对象，指定PLC的ip地址和端口号
            OmronFinsNet omronFinsNet = new OmronFinsNet( "192.168.1.110", 6000 );
            omronFinsNet.SA1 = 10;
            omronFinsNet.DA1 = 110;

            // 连接对象
            OperateResult connect = omronFinsNet.ConnectServer( );
            if (!connect.IsSuccess)
            {
                Console.WriteLine( "connect failed:" + connect.Message );
                return;
            }

            // 举例读取D100的值
            short D100 = omronFinsNet.ReadInt16( "D100" ).Content;

            omronFinsNet.ConnectClose( );

            #endregion
        }


        public void ReadExample( )
        {
            #region ReadExample1


            OmronFinsNet omronFinsNet = new OmronFinsNet( "192.168.1.110", 6000 );
            omronFinsNet.SA1 = 10;
            omronFinsNet.DA1 = 110;

            // 此处以D寄存器作为示例
            bool D100_7 = omronFinsNet.ReadBool( "D100.7" ).Content;  // 读取D100.7是否通断，注意D100.0等同于D100
            short short_D100 = omronFinsNet.ReadInt16( "D100" ).Content; // 读取D100组成的字
            ushort ushort_D100 = omronFinsNet.ReadUInt16( "D100" ).Content; // 读取D100组成的无符号的值
            int int_D100 = omronFinsNet.ReadInt32( "D100" ).Content;         // 读取D100-D101组成的有符号的数据
            uint uint_D100 = omronFinsNet.ReadUInt32( "D100" ).Content;      // 读取D100-D101组成的无符号的值
            float float_D100 = omronFinsNet.ReadFloat( "D100" ).Content;   // 读取D100-D101组成的单精度值
            long long_D100 = omronFinsNet.ReadInt64( "D100" ).Content;      // 读取D100-D103组成的大数据值
            ulong ulong_D100 = omronFinsNet.ReadUInt64( "D100" ).Content;   // 读取D100-D103组成的无符号大数据
            double double_D100 = omronFinsNet.ReadDouble( "D100" ).Content; // 读取D100-D103组成的双精度值
            string str_D100 = omronFinsNet.ReadString( "D100", 5 ).Content;// 读取D100-D104组成的ASCII字符串数据

            // 读取数组
            short[] short_D1000_array = omronFinsNet.ReadInt16( "D1000", 10 ).Content;         // 读取D1000的short值 
            ushort[] ushort_D1000_array = omronFinsNet.ReadUInt16( "D1000", 10 ).Content;      // 读取D1000的ushort值
            int[] int_D1000_array = omronFinsNet.ReadInt32( "D1000", 10 ).Content;             // 读取D1000-D1001组成的int数据
            uint[] uint_D1000_array = omronFinsNet.ReadUInt32( "D1000", 10 ).Content;          // 读取D1000-D1001组成的uint数据
            float[] float_D1000_array = omronFinsNet.ReadFloat( "D1000", 10 ).Content;         // 读取D1000-D1001组成的float数据
            long[] long_D1000_array = omronFinsNet.ReadInt64( "D1000", 10 ).Content;           // 读取D1000-D1003组成的long数据
            ulong[] ulong_D1000_array = omronFinsNet.ReadUInt64( "D1000", 10 ).Content;        // 读取D1000-D1003组成的long数据
            double[] double_D1000_array = omronFinsNet.ReadDouble( "D1000", 10 ).Content;      // 读取D1000-D1003组成的double数据

            #endregion
        }

        public void ReadExample2( )
        {
            #region ReadExample2

            OmronFinsNet omronFinsNet = new OmronFinsNet( "192.168.1.110", 6000 );
            omronFinsNet.SA1 = 10;
            omronFinsNet.DA1 = 110;

            OperateResult<byte[]> read = omronFinsNet.Read( "D100", 4 );
            if (read.IsSuccess)
            {
                float temp = omronFinsNet.ByteTransform.TransInt16( read.Content, 0 ) / 10f;
                float press = omronFinsNet.ByteTransform.TransInt16( read.Content, 2 ) / 100f;
                int count = omronFinsNet.ByteTransform.TransInt32( read.Content, 2 );

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

            OmronFinsNet omronFinsNet = new OmronFinsNet( "192.168.1.110", 6000 );
            omronFinsNet.SA1 = 10;
            omronFinsNet.DA1 = 110;

            // 此处以D寄存器作为示例
            omronFinsNet.Write( "D1000", (short)1234 );                // 写入D1000  short值  ,W3C0,R3C0 效果是一样的
            omronFinsNet.Write( "D1000", (ushort)45678 );              // 写入D1000  ushort值
            omronFinsNet.Write( "D1000", 1234566 );                    // 写入D1000  int值
            omronFinsNet.Write( "D1000", (uint)1234566 );               // 写入D1000  uint值
            omronFinsNet.Write( "D1000", 123.456f );                    // 写入D1000  float值
            omronFinsNet.Write( "D1000", 123.456d );                    // 写入D1000  double值
            omronFinsNet.Write( "D1000", 123456661235123534L );          // 写入D1000  long值
            omronFinsNet.Write( "D1000", 523456661235123534UL );          // 写入D1000  ulong值
            omronFinsNet.Write( "D1000", "K123456789" );                // 写入D1000  string值

            // 读取数组
            omronFinsNet.Write( "D1000", new short[] { 123, 3566, -123 } );                // 写入D1000  short值  ,W3C0,R3C0 效果是一样的
            omronFinsNet.Write( "D1000", new ushort[] { 12242, 42321, 12323 } );              // 写入D1000  ushort值
            omronFinsNet.Write( "D1000", new int[] { 1234312312, 12312312, -1237213 } );                    // 写入D1000  int值
            omronFinsNet.Write( "D1000", new uint[] { 523123212, 213, 13123 } );               // 写入D1000  uint值
            omronFinsNet.Write( "D1000", new float[] { 123.456f, 35.3f, -675.2f } );                    // 写入D1000  float值
            omronFinsNet.Write( "D1000", new double[] { 12343.542312d, 213123.123d, -231232.53432d } );                    // 写入D1000  double值
            omronFinsNet.Write( "D1000", new long[] { 1231231242312, 34312312323214, -1283862312631823 } );          // 写入D1000  long值
            omronFinsNet.Write( "D1000", new ulong[] { 1231231242312, 34312312323214, 9731283862312631823 } );          // 写入D1000  ulong值

            #endregion
        }

        public void WriteExample2( )
        {
            #region WriteExample2

            OmronFinsNet omronFinsNet = new OmronFinsNet( "192.168.1.110", 6000 );
            omronFinsNet.SA1 = 10;
            omronFinsNet.DA1 = 110;

            // 拼凑数据，这样的话，一次通讯就完成数据的全部写入
            byte[] buffer = new byte[8];
            omronFinsNet.ByteTransform.TransByte( (short)1234 ).CopyTo( buffer, 0 );
            omronFinsNet.ByteTransform.TransByte( (short)2100 ).CopyTo( buffer, 2 );
            omronFinsNet.ByteTransform.TransByte( 12353423 ).CopyTo( buffer, 4 );

            OperateResult write = omronFinsNet.Write( "D100", buffer );
            if (write.IsSuccess)
            {
                // success
            }
            else
            {
                // failed
            }

            // 上面的功能等同于三个数据分别写入，下面的性能更差点，因为进行了三次通讯，而且每次还要判断是否写入成功
            //omronFinsNet.Write( "D100", (short)1234 );
            //omronFinsNet.Write( "D100", (short)2100 );
            //omronFinsNet.Write( "D100", 12353423 );

            #endregion
        }


        public void ReadBool( )
        {
            #region ReadBool

            OmronFinsNet omronFinsNet = new OmronFinsNet( "192.168.1.110", 6000 );
            omronFinsNet.SA1 = 10;
            omronFinsNet.DA1 = 110;

            // 以下是简单的读取，没有仔细校验的方式
            bool X1 = omronFinsNet.ReadBool( "D100.1" ).Content;
            bool[] X1_10 = omronFinsNet.ReadBool( "D100.1", 10 ).Content;

            // 如果需要判断是否读取成功
            OperateResult<bool> R_X1 = omronFinsNet.ReadBool( "D100.1" );
            if (R_X1.IsSuccess)
            {
                // success
                bool value = R_X1.Content;
            }
            else
            {
                // failed
            }


            OperateResult<bool[]> R_X1_10 = omronFinsNet.ReadBool( "D100.1", 10 );
            if (R_X1_10.IsSuccess)
            {
                // success
                bool x1 = R_X1_10.Content[0];
                bool x2 = R_X1_10.Content[1];
                bool x3 = R_X1_10.Content[2];
                bool x4 = R_X1_10.Content[3];
                bool x5 = R_X1_10.Content[4];
                bool x6 = R_X1_10.Content[5];
                bool x7 = R_X1_10.Content[6];
                bool x8 = R_X1_10.Content[7];
                bool x9 = R_X1_10.Content[8];
                bool xa = R_X1_10.Content[9];
            }
            else
            {
                // failed
            }


            #endregion
        }

        public void WriteBool( )
        {
            #region WriteBool

            OmronFinsNet omronFinsNet = new OmronFinsNet( "192.168.1.110", 6000 );
            omronFinsNet.SA1 = 10;
            omronFinsNet.DA1 = 110;

            // 以下是简单的写入，没有仔细校验的方式
            omronFinsNet.Write( "D100.1", true );
            omronFinsNet.Write( "D100.1", new bool[] { true, false, true, false } );

            // 如果需要判断是否读取成功
            OperateResult write1 = omronFinsNet.Write( "D100.1", true );
            if (write1.IsSuccess)
            {
                // success
            }
            else
            {
                // failed
            }


            OperateResult write2 = omronFinsNet.Write( "D100.1", new bool[] { true, false, true, false } );
            if (write2.IsSuccess)
            {
                // success
            }
            else
            {
                // failed
            }


            #endregion
        }

    }
}
