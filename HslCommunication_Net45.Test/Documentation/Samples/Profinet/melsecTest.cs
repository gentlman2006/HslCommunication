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

    }
}
