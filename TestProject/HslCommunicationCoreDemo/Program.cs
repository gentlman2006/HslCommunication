using System;
using HslCommunication.Profinet.Melsec;
using HslCommunication.ModBus;
using HslCommunication;
using HslCommunication.Profinet.AllenBradley;
using HslCommunication.Profinet.Siemens;
using System.Threading.Tasks;
using HslCommunication.Enthernet;
using HslCommunication.Enthernet.Redis;

namespace HslCommunicationCoreDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Console.WriteLine( System.Globalization.CultureInfo.CurrentCulture.ToString() );


            //NetSimplifyClient AccountSimplifyClient = new NetSimplifyClient( "127.0.0.1", 23456 );
            //OperateResult<NetHandle,string> read = AccountSimplifyClient.ReadCustomerFromServer( 1, "" );
            //if (read.IsSuccess)
            //{
            //    Console.WriteLine( "Handle:" + read.Content1 );
            //    Console.WriteLine( read.Content2 );
            //}
            //else
            //{
            //    Console.WriteLine( "失败：" + read.Message );
            //}

            RedisSubscribe subscribe = new RedisSubscribe( "127.0.0.1", 6379, new string[] { "WareHouse:HuiBo" } );
            subscribe.CreatePush( ( m, n ) =>
            {
                Console.WriteLine( DateTime.Now.ToString() + "  Key: " + m );
                Console.WriteLine( n );
            } );

            Console.ReadLine( );
        }






        static void MelsecTest( )
        {
            MelsecMcAsciiNet melsec = new MelsecMcAsciiNet( "192.168.1.192", 6000 );
            HslCommunication.OperateResult<short> read = melsec.ReadInt16( "D100" );
            if (read.IsSuccess)
            {
                Console.WriteLine( read.Content );
            }
            else
            {
                Console.WriteLine( read.Message );
            }
        }
    }
}
