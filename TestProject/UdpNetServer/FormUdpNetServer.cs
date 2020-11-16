using HslCommunication.Enthernet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication.Core.Net;
using HslCommunication;

namespace UdpNetServer
{
    public partial class FormUdpNetServer : Form
    {
        public FormUdpNetServer( )
        {
            InitializeComponent( );
        }

        private void FormUdpNetServer_Load( object sender, EventArgs e )
        {
            textBox3.Text = Guid.Empty.ToString( );
        }


        #region Simplify Net

        private NetUdpServer udpNetServer;

        private void Start()
        {
            try
            {
                udpNetServer = new NetUdpServer( );
                udpNetServer.ReceiveCacheLength = int.Parse( textBox4.Text );
                udpNetServer.Token = new Guid( textBox3.Text );
                udpNetServer.AcceptString += UdpNetServer_AcceptString;
                udpNetServer.LogNet = new HslCommunication.LogNet.LogNetSingle( Application.StartupPath + @"\Logs\log.txt" );
                udpNetServer.LogNet.BeforeSaveToFile += LogNet_BeforeSaveToFile;
                udpNetServer.ServerStart( int.Parse( textBox1.Text ) );
                userButton1.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show( "创建失败：" + ex.Message );
            }
        }

        private void UdpNetServer_AcceptString( AppSession session, NetHandle handle, string value )
        {
            if (InvokeRequired)
            {
                BeginInvoke( new Action<AppSession, NetHandle, string>( UdpNetServer_AcceptString ), session, handle, value );
                return;
            }

            textBox2.AppendText( $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}]：" + handle + " 值：" + value + Environment.NewLine );
        }

        private void LogNet_BeforeSaveToFile( object sender, HslCommunication.LogNet.HslEventArgs e )
        {
            if (InvokeRequired)
            {
                BeginInvoke( new Action<object, HslCommunication.LogNet.HslEventArgs>( LogNet_BeforeSaveToFile ), sender, e );
                return;
            }

            textBox2.AppendText( e.HslMessage.ToString( ) + Environment.NewLine );
        }
        


        #endregion
        

        private void userButton1_Click_1( object sender, EventArgs e )
        {
            Start( );
        }
    }
}
