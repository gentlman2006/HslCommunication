using System;
using HslCommunication.Profinet.Melsec;
using HslCommunication.ModBus;
using HslCommunication;
using HslCommunication.Profinet.AllenBradley;

namespace HslCommunicationCoreDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Console.WriteLine( System.Globalization.CultureInfo.CurrentCulture.ToString() );
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
