using HslCommunication.Profinet.Melsec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication;
using HslCommunication.Core.Net;

namespace HslCommunication_Net45.Test.Documentation.Samples.Core
{


    public class NetworkDoubleBaseTest
    {
        public void ConnectServer( )
        {
            MelsecMcNet client = new MelsecMcNet( );

            #region Connect1

            // client为设备的连接对象，简单的启动连接
            client.ConnectServer( );

            #endregion

            #region Connect2

            // client为设备的连接对象，如果想知道是否连接成功
            OperateResult connect = client.ConnectServer( );
            if (connect.IsSuccess)
            {
                Console.WriteLine( "connect success" );
            }
            else
            {
                // do something
                Console.WriteLine( "connect failed" );
            }

            #endregion


        }

        public void ConnectServer2( )
        {
            MelsecMcNet client = new MelsecMcNet( );


            AlienSession session = new AlienSession( );
            #region AlienConnect1

            // session对象由 NetworkAlienClient 类创建
            client.ConnectServer( session );

            #endregion

            #region AlienConnect2

            // session对象由 NetworkAlienClient 类创建
            OperateResult connect = client.ConnectServer( session );
            if (connect.IsSuccess)
            {
                Console.WriteLine( "connect success" );
            }
            else
            {
                // do something
                Console.WriteLine( "connect failed" );
            }

            #endregion
        }

        public void ConnectCloseExample( )
        {
            MelsecMcNet client = new MelsecMcNet( );

            #region ConnectCloseExample

            client.ConnectClose( );

            #endregion
        }

        public void InitializationOnConnectExample( )
        {
            MelsecMcNet client = new MelsecMcNet( );

            #region InitializationOnConnectExample

            

            #endregion
        }


        public void ByteTransformExample( )
        {
            MelsecMcNet client = new MelsecMcNet( );

            #region ByteTransform

            // 假设buffer是client从设备读取的数据内容
            byte[] buffer = new byte[8];
            // 转化为4个short
            short[] short_value = client.ByteTransform.TransInt16( buffer, 0, 4 );
            // 转化为2个float
            float[] float_value = client.ByteTransform.TransSingle( buffer, 0, 2 );
            // 其他的类型转换是类似的

            #endregion

        }

        public void ConnectTimeOutExample( )
        {
            MelsecMcNet client = new MelsecMcNet( );

            #region ConnectTimeOutExample

            // 设置连接的超时时间，单位 毫秒
            client.ConnectTimeOut = 1000;
            // 1秒没有连接上的时候就自动返回失败
            client.ConnectServer( );

            #endregion
        }

        public void ReceiveTimeOutExample( )
        {
            MelsecMcNet client = new MelsecMcNet( );

            #region ReceiveTimeOutExample

            // 设置反馈的超时时间，单位 毫秒
            client.ReceiveTimeOut = 1000;
            // 1秒没有接收到就自动返回失败，此处的地址示例是modbus的地址，对于读取也是一样的
            OperateResult write = client.Write( "100", 123 );

            #endregion

        }

        public void SetPersistentConnectionExample( )
        {
            MelsecMcNet client = new MelsecMcNet( );

            #region SetPersistentConnectionExample

            client.SetPersistentConnection( );        // 设置长连接，但是不立即连接，等到第一次读写的时候再启动连接。
            System.Threading.Thread.Sleep( 10000 );   // 休眠10秒



            // 调用write时才是真正的启动连接，后续的读写操作重复利用该连接，此处的地址示例是modbus的地址，对于读取也是一样的
            OperateResult write = client.Write( "100", 123 );

            #endregion


        }
    }
}
