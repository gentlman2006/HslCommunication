using System;
using HslCommunication.Profinet.Melsec;
using HslCommunication.ModBus;
using HslCommunication;
using HslCommunication.Profinet.AllenBradley;
using HslCommunication.Profinet.Siemens;
using System.Threading.Tasks;
using HslCommunication.Enthernet;
using HslCommunication.Enthernet.Redis;
using HslCommunication.BasicFramework;
using HslCommunication.Core;

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

            //RedisSubscribe subscribe = new RedisSubscribe( "127.0.0.1", 6379, new string[] { "WareHouse:HuiBo" } );
            //subscribe.CreatePush( ( m, n ) =>
            //{
            //    Console.WriteLine( DateTime.Now.ToString() + "  Key: " + m );
            //    Console.WriteLine( n );
            //} );

            //RedisClient redisClient = new RedisClient( "47.92.5.140", 6379, "" );
            //redisClient.SetPersistentConnection( );

            //MelsecMcNet melsecMc = new MelsecMcNet( "192.168.8.12", 6001 );
            //melsecMc.SetPersistentConnection( );

            //while (true)
            //{
            //    System.Threading.Thread.Sleep( 300 );
            //    OperateResult<int> read = melsecMc.ReadInt32( "D100" );
            //    if (!read.IsSuccess) {
            //        Console.WriteLine( DateTime.Now.ToString( ) + "  PLC Read Failed: " + read.Message );
            //        continue;
            //    };

            //    OperateResult write = redisClient.WriteKey( "Test", read.Content.ToString( ) );
            //    if (write.IsSuccess)
            //    {
            //        Console.WriteLine( DateTime.Now.ToString( ) + "  Write Success!" );
            //    }
            //    else
            //    {
            //        Console.WriteLine( DateTime.Now.ToString( ) + "  Write Failed : " + write.Message );
            //    }
            //}


            SimpleHybirdLock hybirdLock = new SimpleHybirdLock( );

            int[] buffer = new int[1000];
            DateTime start = DateTime.Now;
            for (int i = 0; i < 100000; i++)
            {
                hybirdLock.Enter( );

                for (int j = 0; j < buffer.Length - 1; j++)
                {
                    buffer[j] = buffer[j + 1];
                }

                buffer[999] = i;

                hybirdLock.Leave( );
            }

            Console.WriteLine( (DateTime.Now - start).TotalMilliseconds );



            start = DateTime.Now;
            for (int i = 0; i < 100000; i++)
            {
                hybirdLock.Enter( );

                int[] newbuffer = new int[1000];
                Array.Copy( buffer, 0, newbuffer, 0, 999 );
                newbuffer[999] = i;
                buffer = newbuffer;

                hybirdLock.Leave( );
            }

            Console.WriteLine( (DateTime.Now - start).TotalMilliseconds );


            SharpList<int> sharpList = new SharpList<int>( 1000, true );
            start = DateTime.Now;
            for (int i = 0; i < 100000; i++)
            {
                sharpList.Add( i );
            }
            Console.WriteLine( (DateTime.Now - start).TotalMilliseconds );

            int[] data = sharpList.ToArray( );

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
