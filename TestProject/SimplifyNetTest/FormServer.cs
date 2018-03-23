using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication.Enthernet;
using HslCommunication.Core.Net;

namespace SimplifyNetTest
{
    public partial class FormServer : Form
    {
        public FormServer()
        {
            InitializeComponent( );
        }

        private void FormServer_Load( object sender, EventArgs e )
        {
            
        }

        #region Simplify Net

        private NetSimplifyServer simplifyServer;

        private void Start()
        {
            try
            {
                simplifyServer = new NetSimplifyServer( );
                simplifyServer.Token = Guid.Empty;
                simplifyServer.ReceiveStringEvent += SimplifyServer_ReceiveStringEvent;
                simplifyServer.LogNet = new HslCommunication.LogNet.LogNetSingle( Application.StartupPath + @"\Logs\log.txt" );
                simplifyServer.LogNet.BeforeSaveToFile += LogNet_BeforeSaveToFile;
                simplifyServer.ServerStart( int.Parse( textBox1.Text ) );
            }
            catch(Exception ex )
            {
                MessageBox.Show( "创建失败：" + ex.Message );
            }
        }

        private void LogNet_BeforeSaveToFile( object sender, HslCommunication.LogNet.HslEventArgs e )
        {
            if(InvokeRequired)
            {
                BeginInvoke( new Action<object, HslCommunication.LogNet.HslEventArgs>( LogNet_BeforeSaveToFile), sender, e );
                return;
            }

            textBox2.AppendText( e.HslMessage.ToString( ) + Environment.NewLine );
        }

        private void SimplifyServer_ReceiveStringEvent( AppSession session, HslCommunication.NetHandle handle, string value )
        {
            if (handle == 1)
            {
                simplifyServer.SendMessage( session, handle, "这是测试信号" );
            }
            else if (handle < 100)
            {
                simplifyServer.SendMessage( session, handle, "这是测试信号：" + handle );
            }
            else
            {

            }
        }


        #endregion

        private void userButton1_Click( object sender, EventArgs e )
        {
            Start( );
        }
    }
}
