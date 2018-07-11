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

namespace HslCommunicationDemo
{
    public partial class FormSimplifyNet : Form
    {
        public FormSimplifyNet( )
        {
            InitializeComponent( );
        }

        private void FormClient_Load( object sender, EventArgs e )
        {
            panel2.Enabled = false;
            textBox3.Text = Guid.Empty.ToString( );
            button2.Enabled = false;
        }



        private NetSimplifyClient simplifyClient = new NetSimplifyClient( );

        private void button1_Click( object sender, EventArgs e )
        {
            // 连接
            simplifyClient.IpAddress = textBox1.Text;
            simplifyClient.Port = int.Parse( textBox2.Text );
            simplifyClient.Token = new Guid( textBox3.Text );
            OperateResult connect = simplifyClient.ConnectServer( );

            if(connect.IsSuccess)
            {
                button1.Enabled = false;
                button2.Enabled = true;
                panel2.Enabled = true;
                button5.Enabled = false;
                MessageBox.Show( "连接成功！" );
            }
            else
            {
                MessageBox.Show( "连接失败" + connect.ToMessageShowString( ) );
            }
        }

        private void button2_Click( object sender, EventArgs e )
        {
            // 断开连接
            button5.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = false;
            panel2.Enabled = false;

            simplifyClient.ConnectClose( );
        }

        private void button5_Click( object sender, EventArgs e )
        {
            if (button5.Text != "重新选择连接")
            {
                // 启动短连接
                simplifyClient.IpAddress = textBox1.Text;
                simplifyClient.Port = int.Parse( textBox2.Text );
                simplifyClient.Token = new Guid( textBox3.Text );

                button1.Enabled = false;
                button2.Enabled = false;
                panel2.Enabled = true;

                button5.Text = "重新选择连接";
            }
            else
            {
                button1.Enabled = true;
                button5.Text = "启动短连接";
            }
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


            int count = int.Parse( textBox6.Text );
            DateTime start = DateTime.Now;
            for (int i = 0; i < count; i++)
            {
                OperateResult<string> read = simplifyClient.ReadFromServer( handle, textBox4.Text );
                if (read.IsSuccess)
                {
                    textBox8.AppendText( read.Content + Environment.NewLine );
                }
                else
                {
                    MessageBox.Show( "读取失败：" + read.ToMessageShowString( ) );
                }
            }

            textBox7.Text = (DateTime.Now - start).TotalMilliseconds.ToString( "F2" );

        }

        private void button4_Click( object sender, EventArgs e )
        {
            // 清空
            textBox8.Clear( );
        }

        private void linkLabel1_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            try
            {
                System.Diagnostics.Process.Start( linkLabel1.Text );
            }
            catch (Exception ex)
            {
                HslCommunication.BasicFramework.SoftBasic.ShowExceptionMessage( ex );
            }
        }
    }
}
