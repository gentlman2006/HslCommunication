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
            timeStart = DateTime.Now;
            threadOnline = 3;
            // 开了三条线程进行测试读取
            new System.Threading.Thread( new System.Threading.ThreadStart( ThreadReadFromPlc ) ) { IsBackground = true }.Start( );
            new System.Threading.Thread( new System.Threading.ThreadStart( ThreadWriteFromPlc ) ) { IsBackground = true }.Start( );
            new System.Threading.Thread( new System.Threading.ThreadStart( ThreadReadFromPlc ) ) { IsBackground = true }.Start( );
        }

        private int readSuccess = 0;
        private int readFailed = 0;
        private DateTime timeStart = DateTime.Now;
        private int threadOnline = 3;

        private void ThreadReadFromPlc( )
        {
            while (true)
            {
                HslCommunication.OperateResult<short> read = client.ReadShortRegister( 100 );
                if (read.IsSuccess)
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

        private void ThreadWriteFromPlc( )
        {
            while (true)
            {
                HslCommunication.OperateResult read = client.WriteRegister( 100, (short)1234 );
                if (read.IsSuccess)
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

        private void Finish()
        {
            if (System.Threading.Interlocked.Decrement( ref threadOnline ) == 0)
            {
                TimeSpan ts = DateTime.Now - timeStart;

                Invoke( new Action( ( ) =>
                {
                    label5.Text = (3000f / ts.TotalSeconds).ToString( );
                } ) );
            }
        }

        private Action action_update;

        private void ShowUpdate( )
        {
            if(InvokeRequired && IsHandleCreated)
            {
                Invoke( action_update );
                return;
            }

            label_success.Text = readSuccess.ToString( );
            label_failed.Text = readFailed.ToString( );
        }
    }
}
