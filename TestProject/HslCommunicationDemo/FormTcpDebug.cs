using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace HslCommunicationDemo
{
    public partial class FormTcpDebug : Form
    {
        public FormTcpDebug( )
        {
            InitializeComponent( );
        }

        private void FormTcpDebug_Load( object sender, EventArgs e )
        {
            panel2.Enabled = false;
        }


        private Socket socketCore = null;
        private bool connectSuccess = false;
        private byte[] buffer = new byte[2048];

        private void button1_Click( object sender, EventArgs e )
        {
            // 连接服务器
            try
            {
                socketCore?.Close( );
                socketCore = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
                connectSuccess = false;
                new System.Threading.Thread( ( ) =>
                {
                    System.Threading.Thread.Sleep( 2000 );
                    if (!connectSuccess) socketCore?.Close( );
                } ).Start( );
                socketCore.Connect( System.Net.IPAddress.Parse( textBox1.Text ), int.Parse( textBox2.Text ) );
                connectSuccess = true;

                socketCore.BeginReceive( buffer, 0, 2048, SocketFlags.None, new AsyncCallback( ReceiveCallBack ), socketCore );
                button1.Enabled = false;
                button2.Enabled = true;
                panel2.Enabled = true;

                MessageBox.Show( "连接成功！" );
            }
            catch(Exception ex)
            {
                MessageBox.Show( "连接失败！" + Environment.NewLine + ex.Message );
            }
        }


        private void button2_Click( object sender, EventArgs e )
        {
            socketCore?.Close( );
            button1.Enabled = true;
            button2.Enabled = false;
            panel2.Enabled = false;
        }

        private void ReceiveCallBack( IAsyncResult ar )
        {
            try
            {
                int length = socketCore.EndReceive( ar );
                socketCore.BeginReceive( buffer, 0, 2048, SocketFlags.None, new AsyncCallback( ReceiveCallBack ), socketCore );

                if (length == 0) return;

                byte[] data = new byte[length];
                Array.Copy( buffer, 0, data, 0, length );
                Invoke( new Action( ( ) =>
                {
                    string msg = string.Empty;
                    if (checkBox1.Checked)
                    {
                        msg = HslCommunication.BasicFramework.SoftBasic.ByteToHexString( data, ' ' );
                    }
                    else
                    {
                        msg = Encoding.ASCII.GetString( data );
                    }


                    if (checkBox4.Checked)
                    {
                        textBox6.AppendText( "[" + DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss.fff" ) + "]   " + msg + Environment.NewLine );
                    }
                    else
                    {
                        textBox6.AppendText( msg + Environment.NewLine );
                    }

                } ) );
            }
            catch(ObjectDisposedException)
            {

            }
            catch (Exception ex)
            {
                Invoke( new Action( ( ) =>
                {
                    MessageBox.Show( "服务器断开连接。" );
                    panel2.Enabled = false;
                    button1.Enabled = true;
                    button2.Enabled = false;
                } ) );
            }
        }

        private void button3_Click( object sender, EventArgs e )
        {
            // 发送数据
            byte[] send = null;
            if (checkBox1.Checked)
            {
                send = HslCommunication.BasicFramework.SoftBasic.HexStringToBytes( textBox5.Text );
            }
            else
            {
                send = Encoding.ASCII.GetBytes( textBox5.Text );
            }
            

            if (checkBox3.Checked)
            {
                // 显示发送信息
                if (checkBox4.Checked)
                {
                    textBox6.AppendText( "[" + DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss.fff" ) + "]   " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( send, ' ' ) + Environment.NewLine );
                }
                else
                {
                    textBox6.AppendText( HslCommunication.BasicFramework.SoftBasic.ByteToHexString( send, ' ' ) + Environment.NewLine );
                }
            }
            try
            {
                socketCore?.Send( send, 0, send.Length, SocketFlags.None );
            }
            catch(Exception ex)
            {
                HslCommunication.BasicFramework.SoftBasic.ShowExceptionMessage( ex );
            }
        }
    }
}
