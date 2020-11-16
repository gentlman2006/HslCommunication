using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.Profinet.Melsec;
using HslCommunication;

namespace HslCommunication_Net45.Test.Documentation.Samples.Profinet
{
    public class melsecTest
    {
        public void ClassTest( )
        {
            #region Usage

            // 实例化对象，指定PLC的ip地址和端口号
            MelsecMcNet melsecMc = new MelsecMcNet( "192.168.1.110", 6000 );
            // 举例读取D100的值
            short D100 = melsecMc.ReadInt16( "D100" ).Content;

            #endregion
        }

        public void ClassTest2( )
        {
            #region Usage2

            // 实例化对象，指定PLC的ip地址和端口号
            MelsecMcNet melsecMc = new MelsecMcNet( "192.168.1.110", 6000 );

            // 连接对象
            OperateResult connect = melsecMc.ConnectServer( );
            if (!connect.IsSuccess)
            {
                Console.WriteLine( "connect failed:" + connect.Message );
                return;
            }

            // 举例读取D100的值
            short D100 = melsecMc.ReadInt16( "D100" ).Content;

            melsecMc.ConnectClose( );

            #endregion
        }


        public void ReadExample( )
        {
            #region ReadExample1


            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 此处以D寄存器作为示例
            short short_D1000 = melsec_net.ReadInt16( "D1000" ).Content;         // 读取D1000的short值 
            ushort ushort_D1000 = melsec_net.ReadUInt16( "D1000" ).Content;      // 读取D1000的ushort值
            int int_D1000 = melsec_net.ReadInt32( "D1000" ).Content;             // 读取D1000-D1001组成的int数据
            uint uint_D1000 = melsec_net.ReadUInt32( "D1000" ).Content;          // 读取D1000-D1001组成的uint数据
            float float_D1000 = melsec_net.ReadFloat( "D1000" ).Content;         // 读取D1000-D1001组成的float数据
            long long_D1000 = melsec_net.ReadInt64( "D1000" ).Content;           // 读取D1000-D1003组成的long数据
            ulong ulong_D1000 = melsec_net.ReadUInt64( "D1000" ).Content;        // 读取D1000-D1003组成的long数据
            double double_D1000 = melsec_net.ReadDouble( "D1000" ).Content;      // 读取D1000-D1003组成的double数据
            string str_D1000 = melsec_net.ReadString( "D1000", 10 ).Content;     // 读取D1000-D1009组成的条码数据

            // 读取数组
            short[] short_D1000_array = melsec_net.ReadInt16( "D1000", 10 ).Content;         // 读取D1000的short值 
            ushort[] ushort_D1000_array = melsec_net.ReadUInt16( "D1000", 10 ).Content;      // 读取D1000的ushort值
            int[] int_D1000_array = melsec_net.ReadInt32( "D1000", 10 ).Content;             // 读取D1000-D1001组成的int数据
            uint[] uint_D1000_array = melsec_net.ReadUInt32( "D1000", 10 ).Content;          // 读取D1000-D1001组成的uint数据
            float[] float_D1000_array = melsec_net.ReadFloat( "D1000", 10 ).Content;         // 读取D1000-D1001组成的float数据
            long[] long_D1000_array = melsec_net.ReadInt64( "D1000", 10 ).Content;           // 读取D1000-D1003组成的long数据
            ulong[] ulong_D1000_array = melsec_net.ReadUInt64( "D1000", 10 ).Content;        // 读取D1000-D1003组成的long数据
            double[] double_D1000_array = melsec_net.ReadDouble( "D1000", 10 ).Content;      // 读取D1000-D1003组成的double数据

            #endregion
        }

        public void ReadExample2( )
        {
            #region ReadExample2

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            OperateResult<byte[]> read = melsec_net.Read( "D100", 4 );
            if(read.IsSuccess)
            {
                float temp = melsec_net.ByteTransform.TransInt16( read.Content, 0 ) / 10f;
                float press = melsec_net.ByteTransform.TransInt16( read.Content, 2 ) / 100f;
                int count = melsec_net.ByteTransform.TransInt32( read.Content, 2 );

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

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 此处以D寄存器作为示例
            melsec_net.Write( "D1000", (short)1234 );                // 写入D1000  short值  ,W3C0,R3C0 效果是一样的
            melsec_net.Write( "D1000", (ushort)45678 );              // 写入D1000  ushort值
            melsec_net.Write( "D1000", 1234566 );                    // 写入D1000  int值
            melsec_net.Write( "D1000", (uint)1234566 );               // 写入D1000  uint值
            melsec_net.Write( "D1000", 123.456f );                    // 写入D1000  float值
            melsec_net.Write( "D1000", 123.456d );                    // 写入D1000  double值
            melsec_net.Write( "D1000", 123456661235123534L );          // 写入D1000  long值
            melsec_net.Write( "D1000", 523456661235123534UL );          // 写入D1000  ulong值
            melsec_net.Write( "D1000", "K123456789" );                // 写入D1000  string值

            // 读取数组
            melsec_net.Write( "D1000", new short[] { 123, 3566, -123 } );                // 写入D1000  short值  ,W3C0,R3C0 效果是一样的
            melsec_net.Write( "D1000", new ushort[] { 12242, 42321, 12323 } );              // 写入D1000  ushort值
            melsec_net.Write( "D1000", new int[] { 1234312312, 12312312, -1237213 } );                    // 写入D1000  int值
            melsec_net.Write( "D1000", new uint[] { 523123212, 213,13123 } );               // 写入D1000  uint值
            melsec_net.Write( "D1000", new float[] { 123.456f, 35.3f, -675.2f } );                    // 写入D1000  float值
            melsec_net.Write( "D1000", new double[] { 12343.542312d, 213123.123d, -231232.53432d } );                    // 写入D1000  double值
            melsec_net.Write( "D1000", new long[] { 1231231242312,34312312323214,-1283862312631823 } );          // 写入D1000  long值
            melsec_net.Write( "D1000", new ulong[] { 1231231242312, 34312312323214, 9731283862312631823 } );          // 写入D1000  ulong值

            #endregion
        }

        public void WriteExample2( )
        {
            #region WriteExample2

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 拼凑数据，这样的话，一次通讯就完成数据的全部写入
            byte[] buffer = new byte[8];
            melsec_net.ByteTransform.TransByte( (short)1234 ).CopyTo( buffer, 0 );
            melsec_net.ByteTransform.TransByte( (short)2100 ).CopyTo( buffer, 2 );
            melsec_net.ByteTransform.TransByte( 12353423 ).CopyTo( buffer, 4 );

            OperateResult write = melsec_net.Write( "D100", buffer );
            if (write.IsSuccess)
            {
                // success
            }
            else
            {
                // failed
            }

            // 上面的功能等同于三个数据分别写入，下面的性能更差点，因为进行了三次通讯，而且每次还要判断是否写入成功
            //melsec_net.Write( "D100", (short)1234 );
            //melsec_net.Write( "D100", (short)2100 );
            //melsec_net.Write( "D100", 12353423 );

            #endregion
        }


        public void ReadBool( )
        {
            #region ReadBool

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            bool X1 = melsec_net.ReadBool( "X1" ).Content;
            bool[] X1_10 = melsec_net.ReadBool( "X1", 10 ).Content;

            // 如果需要判断是否读取成功
            OperateResult<bool> R_X1 = melsec_net.ReadBool( "X1" );
            if (R_X1.IsSuccess)
            {
                // success
                bool value = R_X1.Content;
            }
            else
            {
                // failed
            }


            OperateResult<bool[]> R_X1_10 = melsec_net.ReadBool( "X1", 10 );
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

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的写入，没有仔细校验的方式
            melsec_net.Write( "M100", true );
            melsec_net.Write( "M100", new bool[] { true, false, true, false } );

            // 如果需要判断是否读取成功
            OperateResult write1 = melsec_net.Write( "M100", true );
            if (write1.IsSuccess)
            {
                // success
            }
            else
            {
                // failed
            }


            OperateResult write2 = melsec_net.Write( "M100", new bool[] { true, false, true, false } );
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
