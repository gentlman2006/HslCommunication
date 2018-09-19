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
    public partial class FormUdpNet : Form
    {
        public FormUdpNet( )
        {
            InitializeComponent( );
        }

        private void FormClient_Load( object sender, EventArgs e )
        {
            panel2.Enabled = false;
            textBox3.Text = Guid.Empty.ToString( );


            if (!Program.ShowAuthorInfomation)
            {
                label2.Visible = false;
                linkLabel1.Visible = false;
                label20.Visible = false;
            }
        }



        private NetUdpClient udpClient = null;

        private void button1_Click( object sender, EventArgs e )
        {
            try
            {
                udpClient = new NetUdpClient( textBox1.Text, int.Parse( textBox2.Text ) );
                udpClient.Token = new Guid( textBox3.Text );

                panel2.Enabled = true;
                button1.Enabled = false;
            }
            catch (Exception ex)
            {
                HslCommunication.BasicFramework.SoftBasic.ShowExceptionMessage( ex );
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
                udpClient.SendMessage( handle, textBox4.Text );
            }
            

        }

        private void button4_Click( object sender, EventArgs e )
        {
            // 清空
            textBox4.Clear( );
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
