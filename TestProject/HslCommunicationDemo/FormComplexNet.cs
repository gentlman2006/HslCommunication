using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication.Enthernet;
using HslCommunication;
using System.Net;

namespace HslCommunicationDemo
{
    public partial class FormComplexNet : Form
    {
        public FormComplexNet( )
        {
            InitializeComponent( );
        }

        private void FormComplexNet_Load( object sender, EventArgs e )
        {
            button2.Enabled = false;
            textBox3.Text = Guid.Empty.ToString( );
        }


        private NetComplexClient complexClient = null;


        private void button1_Click( object sender, EventArgs e )
        {

            if(!IPAddress.TryParse(textBox1.Text,out IPAddress address))
            {
                MessageBox.Show( "IP地址填写不正确" );
                return;
            }

            if(!int.TryParse(textBox2.Text,out int port))
            {
                MessageBox.Show( "port填写不正确" );
                return;
            }

            // 连接 connect
            complexClient = new NetComplexClient( );
            complexClient.ClientAlias = textBox9.Text;
            complexClient.EndPointServer = new IPEndPoint( address, port );
            complexClient.Token = new Guid( textBox3.Text );
            complexClient.AcceptString += ComplexClient_AcceptString;
            complexClient.AcceptByte += ComplexClient_AcceptByte;
            complexClient.ClientStart( );

            button1.Enabled = false;

            panel2.Enabled = true;
        }

        private void ComplexClient_AcceptByte( HslCommunication.Core.Net.AppSession session, NetHandle handle, byte[] data )
        {
            // 接收字节数据，
            ShowTextInfo( $"[{session.IpEndPoint}] [{handle}] {HslCommunication.BasicFramework.SoftBasic.ByteToHexString( data )}" );
        }

        private void ComplexClient_AcceptString( HslCommunication.Core.Net.AppSession session, NetHandle handle, string data )
        {
            // 接收字符串
            ShowTextInfo( $"[{session.IpEndPoint}] [{handle}] {data}" );
        }

        private void button2_Click( object sender, EventArgs e )
        {
            // 断开连接 disconnect
            complexClient.ClientClose( );
        }

        private void ShowTextInfo( string text )
        {
            if (InvokeRequired)
            {
                Invoke( new Action<string>( ShowTextInfo ) );
                return;
            }

            textBox2.AppendText( text + Environment.NewLine );
        }
    }
}
