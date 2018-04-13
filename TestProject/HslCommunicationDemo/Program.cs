using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HslCommunicationDemo
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main( )
        {



            Application.EnableVisualStyles( );
            Application.SetCompatibleTextRenderingDefault( false );


            System.Threading.ThreadPool.SetMaxThreads( 2000, 800 );
            Application.Run( new FormLoad( ) );
        }
    }
}
