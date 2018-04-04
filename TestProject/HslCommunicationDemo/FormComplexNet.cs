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

            try
            {
                // 连接 connect
                complexClient = new NetComplexClient( );
                complexClient.ClientAlias = textBox9.Text;
                complexClient.EndPointServer = new IPEndPoint( address, port );
                complexClient.Token = new Guid( textBox3.Text );
                complexClient.AcceptString += ComplexClient_AcceptString;
                complexClient.AcceptByte += ComplexClient_AcceptByte;
                complexClient.ClientStart( );

                button1.Enabled = false;
                button2.Enabled = true;
                panel2.Enabled = true;
            }
            catch(Exception ex)
            {
                HslCommunication.BasicFramework.SoftBasic.ShowExceptionMessage( ex );
            }
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
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void ShowTextInfo( string text )
        {
            if (InvokeRequired)
            {
                Invoke( new Action<string>( ShowTextInfo ),text );
                return;
            }

            textBox8.AppendText( text + Environment.NewLine );
        }

        private void button3_Click( object sender, EventArgs e )
        {
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


            if(!int.TryParse(textBox6.Text,out int count))
            {
                MessageBox.Show( "数据发送次数输入异常" );
                return;
            }

            for (int i = 0; i < count; i++)
            {
                complexClient.Send( handle, textBox4.Text );
            }
        }
    }
}
