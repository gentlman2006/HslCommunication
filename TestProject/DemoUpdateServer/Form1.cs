using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using HslCommunication;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;

namespace DemoUpdateServer
{
    public partial class Form1 : Form
    {
        public Form1( )
        {
            InitializeComponent( );
        }

        private void Form1_Load( object sender, EventArgs e )
        {
            // 加载版本号
            if (File.Exists( "version.txt" ))
            {
                textBox1.Text = Encoding.Default.GetString( File.ReadAllBytes( "version.txt" ) );
                version = new HslCommunication.BasicFramework.SystemVersion( textBox1.Text );
            }

            if(!Directory.Exists( Application.StartupPath + @"\Demo" ))
            {
                Directory.CreateDirectory( Application.StartupPath + @"\Demo" );
            }

            lognet = new HslCommunication.LogNet.LogNetSingle( "logs.txt" );
            lognet.BeforeSaveToFile += LogNet_BeforeSaveToFile;
        }

        private void button1_Click( object sender, EventArgs e )
        {
            version = new HslCommunication.BasicFramework.SystemVersion( textBox1.Text );
            File.WriteAllBytes( "version.txt",Encoding.Default.GetBytes( textBox1.Text ) );

            MessageBox.Show( "更新成功" );
        }

        private HslCommunication.BasicFramework.SystemVersion version = new HslCommunication.BasicFramework.SystemVersion( "1.0.1" );
        private HslCommunication.LogNet.ILogNet lognet;
        private Random random = new Random( );

        #region 同步网络中心，用来请求版本号信息

        private HslCommunication.Enthernet.NetSimplifyServer simplifyServer;

        private void NetStart( )
        {
            simplifyServer = new HslCommunication.Enthernet.NetSimplifyServer( );
            simplifyServer.ReceiveStringEvent += SimplifyServer_ReceiveStringEvent;
            simplifyServer.ServerStart( 18467 );
        }

        private void SimplifyServer_ReceiveStringEvent( HslCommunication.Core.Net.AppSession arg1, NetHandle handle, string msg )
        {
            if(handle == 1)
            {
                simplifyServer.SendMessage( arg1, handle, version.ToString( ) );
                string address = GetAddressByIp( arg1.IpAddress );
                lognet.WriteInfo( $"{arg1.IpAddress.PadRight( 15 )} [{address}] Run Application" );
            }
            else if(handle == 2)
            {
                simplifyServer.SendMessage( arg1, random.Next( 10000 ), "这是一条测试的数据：" + random.Next( 10000 ).ToString( ) );
            }
            else
            {
                simplifyServer.SendMessage( arg1, handle, msg );
            }
        }


        #endregion

        #region UpdateServer

        private HslCommunication.Enthernet.NetSoftUpdateServer softUpdateServer;

        private void NetStart2( )
        {
            softUpdateServer = new HslCommunication.Enthernet.NetSoftUpdateServer( );
            softUpdateServer.FileUpdatePath = Application.StartupPath + @"\Demo";
            softUpdateServer.LogNet = lognet;
            softUpdateServer.ServerStart( 18468 );
        }

        private void LogNet_BeforeSaveToFile( object sender, HslCommunication.LogNet.HslEventArgs e )
        {
            Invoke( new Action( ( ) =>
             {
                 if (e.HslMessage.Degree != HslCommunication.LogNet.HslMessageDegree.FATAL)
                 {
                     textBox2.AppendText( e.HslMessage.ToString( ) + Environment.NewLine );
                 }
             } ) );
        }

        #endregion

        private void Form1_Shown( object sender, EventArgs e )
        {
            NetStart( );
            NetStart2( );
        }

        private void button2_Click( object sender, EventArgs e )
        {
            MessageBox.Show( GetAddressByIp( "47.92.5.140" ) );
        }

        private string GetAddressByIp(string ip )
        {
            try
            {
                WebClient webClient = new WebClient( );

                webClient.Encoding = Encoding.UTF8;
                webClient.Headers.Add( "Accept", "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*" );
                webClient.Headers.Add( "Accept-Language", "zh-cn" );//ja-jp
                webClient.Headers.Add( "User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71" );
                webClient.Headers.Add( "Content-Type", "text/html" );
                webClient.Headers.Add( "Content-Type", "image/jpeg" );
                //webClient.Headers.Add("Connection", "Keep-Alive");
                webClient.Headers.Add( "Accept-Encoding", "gzip,deflate" );

                byte[] data = webClient.DownloadData( "http://www.ip138.com/ips138.asp?ip=" + ip + "&action=2" );
                webClient.Dispose( );

                string result = Encoding.Default.GetString( data );

                Match match = Regex.Match( result, "<ul class=\"ul1\"><li>[^<]+" );
                if (match == null)
                {
                    return string.Empty;
                }

                return match.Value.Substring( 25 );
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
