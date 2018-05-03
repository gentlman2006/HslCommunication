using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PushNetServer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main( )
        {
            HslCommunication.BasicFramework.SoftMail.MailSystem163.MailSendAddress = "hsl200909@163.com";
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles( );
            Application.SetCompatibleTextRenderingDefault( false );
            Application.Run( new FormServer( ) );
        }

        private static void CurrentDomain_UnhandledException( object sender, UnhandledExceptionEventArgs e )
        {
            if (e.ExceptionObject is Exception ex)
            {
                HslCommunication.BasicFramework.SoftMail.MailSystem163.SendMail( ex );
            }
        }
    }
}
