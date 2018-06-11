using System;
using HslCommunication.Profinet.Melsec;

namespace HslCommunicationCoreDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            MelsecTest( );

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
