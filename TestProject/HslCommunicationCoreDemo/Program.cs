using System;
using HslCommunication.Profinet.Melsec;
using HslCommunication.ModBus;
using HslCommunication;

namespace HslCommunicationCoreDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //MelsecTest( );

            ModbusTcpNet modbusTcp = new ModbusTcpNet( "127.0.0.1" );
            //modbusTcp.ConnectServer( );

            Console.WriteLine( modbusTcp.ReadInt16( "100" ).Content );
            
            Console.WriteLine( HslCommunication.BasicFramework.SoftBasic.ByteToHexString( modbusTcp.Read( "100", 200 ).Content, ' ' ) );

            //modbusTcp.ConnectClose( );
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
