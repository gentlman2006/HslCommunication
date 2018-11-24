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
        }

        private void button1_Click( object sender, EventArgs e )
        {
            version = new HslCommunication.BasicFramework.SystemVersion( textBox1.Text );
            File.WriteAllBytes( "version.txt",Encoding.Default.GetBytes( textBox1.Text ) );

            MessageBox.Show( "更新成功" );
        }

        private HslCommunication.BasicFramework.SystemVersion version = new HslCommunication.BasicFramework.SystemVersion( "1.0.1" );

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
            softUpdateServer.LogNet = new HslCommunication.LogNet.LogNetSingle( "logs.txt" );
            softUpdateServer.LogNet.BeforeSaveToFile += LogNet_BeforeSaveToFile;
            softUpdateServer.ServerStart( 18468 );
        }

        private void LogNet_BeforeSaveToFile( object sender, HslCommunication.LogNet.HslEventArgs e )
        {
            Invoke( new Action( ( ) =>
             {
                 textBox2.AppendText( e.HslMessage.ToString( ) + Environment.NewLine );
             } ) );
        }

        #endregion

        private void Form1_Shown( object sender, EventArgs e )
        {
            NetStart( );
            NetStart2( );
        }
    }
}
