using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication.Enthernet;

namespace ComplexNetServer
{
    public partial class FormServer : Form
    {
        public FormServer( )
        {
            InitializeComponent( );
        }

        private void FormServer_Load( object sender, EventArgs e )
        {
            textBox3.Text = Guid.Empty.ToString( );
        }


        #region Complex Server


        private NetComplexServer complexServer = null;

        private void ComplexServerStart(int port)
        {
            complexServer = new NetComplexServer( );
            complexServer.LogNet = new HslCommunication.LogNet.LogNetDateTime( Application.StartupPath + "\\Logs", HslCommunication.LogNet.GenerateMode.ByEveryDay );
            complexServer.LogNet.BeforeSaveToFile += LogNet_BeforeSaveToFile;
            complexServer.Token = new Guid( textBox3.Text );
            complexServer.AcceptString += ComplexServer_AcceptString;
            complexServer.AcceptByte += ComplexServer_AcceptByte;
            complexServer.ClientOnline += ComplexServer_ClientOnline;
            complexServer.ClientOffline += ComplexServer_ClientOffline;
            complexServer.ServerStart( port );
        }

        private void ComplexServer_ClientOffline( HslCommunication.Core.Net.AppSession session, string reason )
        {
            // 下线
            ShowTextInfo( $"[{session.IpEndPoint}] Offline" );
        }

        private void ComplexServer_ClientOnline( HslCommunication.Core.Net.AppSession session )
        {
            // 上线
            ShowTextInfo( $"[{session.IpEndPoint}] Online" );
        }

        private void ComplexServer_AcceptByte( HslCommunication.Core.Net.AppSession session, HslCommunication.NetHandle handle, byte[] data )
        {
            // 接收字节数据，
            ShowTextInfo( $"[{session.IpEndPoint}] [{handle}] {HslCommunication.BasicFramework.SoftBasic.ByteToHexString(data)}" );

            // 也可以回发客户端信息，选择发送的session即可
        }

        private void ComplexServer_AcceptString( HslCommunication.Core.Net.AppSession session, HslCommunication.NetHandle handle, string data )
        {
            // 接收字符串
            ShowTextInfo( $"[{session.IpEndPoint}] [{handle}] {data}" );

            // 举个例子，当handle==1时，回发一串信息
            // for example , when handle == 1. return text
            if(handle == 1)
            {
                complexServer.Send( session, handle, "This is test Text" );
            }
        }

        private void LogNet_BeforeSaveToFile( object sender, HslCommunication.LogNet.HslEventArgs e )
        {
            // 显示日志信息
            ShowTextInfo( e.HslMessage.ToString( ) );
        }



        #endregion

        private void ShowTextInfo(string text)
        {
            if(InvokeRequired)
            {
                Invoke( new Action<string>( ShowTextInfo ) );
                return;
            }

            textBox2.AppendText( text + Environment.NewLine );
        }

        private void userButton1_Click( object sender, EventArgs e )
        {
            if(!int.TryParse(textBox1.Text,out int port))
            {
                MessageBox.Show( "端口号输入异常！" );
                return;
            }

            try
            {
                ComplexServerStart( port );
                userButton1.Enabled = false;
                panel1.Enabled = true;
            }
            catch(Exception ex)
            {
                HslCommunication.BasicFramework.SoftBasic.ShowExceptionMessage( ex );
            }
        }
    }
}
