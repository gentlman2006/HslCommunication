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
    }
}
