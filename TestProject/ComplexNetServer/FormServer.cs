using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication;
using HslCommunication.Enthernet;
using HslCommunication.LogNet;

namespace ComplexNetServer
{
    #region NetComplexServer

    public partial class FormServer : Form
    {
        public FormServer( )
        {
            InitializeComponent( );

        }

        private void FormServer_Load( object sender, EventArgs e )
        {
            textBox3.Text = Guid.Empty.ToString( );

            logNet = new HslCommunication.LogNet.LogNetDateTime( Application.StartupPath + "\\Logs", HslCommunication.LogNet.GenerateMode.ByEveryDay );
            logNet.BeforeSaveToFile += LogNet_BeforeSaveToFile;
        }


        #region Complex Server

        private ILogNet logNet = null;
        private NetComplexServer complexServer = null;

        private void ComplexServerStart(int port)
        {
            complexServer = new NetComplexServer( );
            complexServer.LogNet = logNet;
            complexServer.Token = new Guid( textBox3.Text );
            complexServer.AcceptString += ComplexServer_AcceptString;
            complexServer.AcceptByte += ComplexServer_AcceptByte;
            complexServer.ClientOnline += ComplexServer_ClientOnline;
            complexServer.ClientOffline += ComplexServer_ClientOffline;
            complexServer.AllClientsStatusChange += ComplexServer_AllClientsStatusChange;
            complexServer.ServerStart( port );
        }

        private void ComplexServer_AllClientsStatusChange( int count )
        {
            Invoke( new Action<int>( m =>
             {
                 label11.Text = count.ToString( );
             } ), count );
        }

        private void ComplexServer_ClientOffline( HslCommunication.Core.Net.AppSession session, string reason )
        {
            // 下线触发
        }

        private void ComplexServer_ClientOnline( HslCommunication.Core.Net.AppSession session )
        {
            // 上线触发
        }

        private void ComplexServer_AcceptByte( HslCommunication.Core.Net.AppSession session, NetHandle handle, byte[] data )
        {
            // 接收字节数据，
            ShowTextInfo( $"[{session.IpEndPoint}] [{handle}] {HslCommunication.BasicFramework.SoftBasic.ByteToHexString(data)}" );

            // 也可以回发客户端信息，选择发送的session即可
        }

        private void ComplexServer_AcceptString( HslCommunication.Core.Net.AppSession session, NetHandle handle, string data )
        {
            // 接收字符串
            logNet.WriteInfo( $"[{session.IpEndPoint}] [{handle}] {data}" );

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
                Invoke( new Action<string>( ShowTextInfo ), text );
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
        

        private void button1_Click( object sender, EventArgs e )
        {
            // 广播数据，此处其实也可以对指定的客户端进行广播数据，需要知道AppSession对象，通常的做法的建立一个房间号引擎，
            // 缓存登录到该房间号的AppSession对象，这样就可以实现针对这个房间号的客户端进行广播数据了。


            // Broadcast data, in fact, broadcast data can also be specified for the specified client, you need to know the AppSession object, 
            // the usual practice to create a room number engine, cache the AppSession object logged in to the room number, 
            // so that you can achieve this room number The client broadcasts data.


            // 数据发送
            NetHandle handle = new NetHandle( );
            if (textBox5.Text.IndexOf( '.' ) >= 0)
            {
                string[] values = textBox5.Text.Split( '.' );
                handle = new NetHandle( byte.Parse( values[0] ), byte.Parse( values[1] ), ushort.Parse( values[2] ) );
            }
            else
            {
                handle = int.Parse( textBox5.Text );
            }


            complexServer.SendAllClients( handle, textBox4.Text );
        }
    }

    #endregion
}
