using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication.ModBus;

namespace TestTool.TestForm
{
    public partial class FormSimenceModbusTcp : Form
    {
        public FormSimenceModbusTcp( )
        {
            InitializeComponent( );
        }



        private ModBusTcpClient client;                                   // 与PLC通信的对象


        private void FormSimenceModbusTcp_Load( object sender, EventArgs e )
        {
            client = new ModBusTcpClient( "192.168.1.195" );

            action_update = new Action( ShowUpdate );
        }

        private void userButton1_Click( object sender, EventArgs e )
        {
            // 连接PLC
            if (client.ConnectServer( ).IsSuccess)
            {
                userButton1.Enabled = false;
                userButton2.Enabled = true;
            }
            else
            {
                MessageBox.Show( "连接失败" );
            }
        }

        private void userButton2_Click( object sender, EventArgs e )
        {
            // 断开PLC
            client.ConnectClose( );
            userButton1.Enabled = true;
            userButton2.Enabled = false;
        }

        private void userButton3_Click( object sender, EventArgs e )
        {
            // 开了三条线程进行测试读取
            new System.Threading.Thread( new System.Threading.ThreadStart( ThreadReadFromPlc ) ) { IsBackground = true }.Start( );
            new System.Threading.Thread( new System.Threading.ThreadStart( ThreadReadFromPlc ) ) { IsBackground = true }.Start( );
            new System.Threading.Thread( new System.Threading.ThreadStart( ThreadReadFromPlc ) ) { IsBackground = true }.Start( );
        }

        private int readSuccess = 0;
        private int readFailed = 0;

        private void ThreadReadFromPlc( )
        {
            for (int i = 0; i < 1000; i++)
            {
                if(client.ReadShortRegister(200).IsSuccess)
                {
                    readSuccess++;
                }
                else
                {
                    readFailed++;
                }

                ShowUpdate( );
            }
        }

        private Action action_update;

        private void ShowUpdate( )
        {
            if(InvokeRequired)
            {
                Invoke( action_update );
                return;
            }

            label_success.Text = readSuccess.ToString( );
            label_failed.Text = readFailed.ToString( );
        }
    }
}
