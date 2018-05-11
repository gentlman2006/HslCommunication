using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication.Robot.EFORT;
using HslCommunication;

namespace HslCommunicationDemo.Robot
{
    public partial class FormEfort : Form
    {
        public FormEfort( )
        {
            InitializeComponent( );
        }

        private void FormEfort_Load( object sender, EventArgs e )
        {
            panel2.Enabled = false;
            button2.Enabled = false;
        }

        private void linkLabel1_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            try
            {
                System.Diagnostics.Process.Start( linkLabel1.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }




        private void RenderRobotData(EfortData efortData)
        {
            textBox4.Text = efortData.PacketStart;
            textBox6.Text = efortData.PacketOrders.ToString( );
            textBox7.Text = efortData.PacketHeartbeat.ToString( );
            textBox5.Text = efortData.PacketEnd.ToString( );

            if(efortData.ErrorStatus == 1)
            {
                label13.BackColor = Color.Red;
                label12.BackColor = SystemColors.Control;
            }
            else
            {
                label13.BackColor = SystemColors.Control;
                label12.BackColor = Color.Red;
            }

            if(efortData.HstopStatus == 1)
            {
                label15.BackColor = Color.Red;
                label14.BackColor = SystemColors.Control;
            }
            else
            {
                label15.BackColor = SystemColors.Control;
                label14.BackColor = Color.Red;
            }

            if(efortData.AuthorityStatus == 1)
            {
                label18.BackColor = Color.Red;
                label17.BackColor = SystemColors.Control;
            }
            else
            {
                label18.BackColor = SystemColors.Control;
                label17.BackColor = Color.Red;
            }

            if(efortData.ServoStatus == 1)
            {
                label22.BackColor = Color.Red;
                label21.BackColor = SystemColors.Control;
            }
            else
            {
                label22.BackColor = SystemColors.Control;
                label21.BackColor = Color.Red;
            }

            if(efortData.AxisMoveStatus == 1)
            {
                label25.BackColor = Color.Red;
                label24.BackColor = SystemColors.Control;
            }
            else
            {
                label25.BackColor = SystemColors.Control;
                label24.BackColor = Color.Red;
            }

            if(efortData.ProgMoveStatus == 1)
            {
                label28.BackColor = Color.Red;
                label27.BackColor = SystemColors.Control;
            }
            else
            {
                label28.BackColor = SystemColors.Control;
                label27.BackColor = Color.Red;
            }

            if(efortData.ProgLoadStatus == 1)
            {
                label31.BackColor = Color.Red;
                label30.BackColor = SystemColors.Control;
            }
            else
            {
                label31.BackColor = SystemColors.Control;
                label30.BackColor = Color.Red;
            }

            if(efortData.ProgHoldStatus == 1)
            {
                label34.BackColor = Color.Red;
                label33.BackColor = SystemColors.Control;
            }
            else
            {
                label34.BackColor = SystemColors.Control;
                label33.BackColor = Color.Red;
            }

            if(efortData.ModeStatus == 1)
            {
                label39.BackColor = Color.Red;
                label37.BackColor = SystemColors.Control;
                label36.BackColor = SystemColors.Control;
            }
            else if(efortData.ModeStatus == 2)
            {
                label39.BackColor = SystemColors.Control;
                label37.BackColor = Color.Red;
                label36.BackColor = SystemColors.Control;
            }
            else
            {
                label39.BackColor = SystemColors.Control;
                label37.BackColor = SystemColors.Control;
                label36.BackColor = Color.Red;
            }

            label40.Text = efortData.SpeedStatus.ToString( ) + " %";
            label46.Text = efortData.ProjectName;
            label48.Text = efortData.ProgramName;


            textBox8.Text = GetStringFromArray( efortData.IoDOut );
            textBox9.Text = GetStringFromArray( efortData.IoDIn );
            textBox10.Text = GetStringFromArray( efortData.IoIOut );
            textBox11.Text = GetStringFromArray( efortData.IoIIn );
            textBox12.Text = efortData.ErrorText;

            textBox13.Text = efortData.DbAxisPos[0].ToString( );
            textBox20.Text = efortData.DbAxisPos[1].ToString( );
            textBox24.Text = efortData.DbAxisPos[2].ToString( );
            textBox28.Text = efortData.DbAxisPos[3].ToString( );
            textBox32.Text = efortData.DbAxisPos[4].ToString( );
            textBox36.Text = efortData.DbAxisPos[5].ToString( );
            textBox40.Text = efortData.DbAxisPos[6].ToString( );

            textBox16.Text = efortData.DbCartPos[0].ToString( );
            textBox17.Text = efortData.DbCartPos[1].ToString( );
            textBox21.Text = efortData.DbCartPos[2].ToString( );
            textBox25.Text = efortData.DbCartPos[3].ToString( );
            textBox29.Text = efortData.DbCartPos[4].ToString( );
            textBox33.Text = efortData.DbCartPos[5].ToString( );

            textBox14.Text = efortData.DbAxisSpeed[0].ToString( );
            textBox19.Text = efortData.DbAxisSpeed[1].ToString( );
            textBox23.Text = efortData.DbAxisSpeed[2].ToString( );
            textBox27.Text = efortData.DbAxisSpeed[3].ToString( );
            textBox31.Text = efortData.DbAxisSpeed[4].ToString( );
            textBox35.Text = efortData.DbAxisSpeed[5].ToString( );
            textBox39.Text = efortData.DbAxisSpeed[6].ToString( );


            textBox15.Text = efortData.DbAxisTorque[0].ToString( );
            textBox18.Text = efortData.DbAxisTorque[0].ToString( );
            textBox22.Text = efortData.DbAxisTorque[0].ToString( );
            textBox26.Text = efortData.DbAxisTorque[0].ToString( );
            textBox30.Text = efortData.DbAxisTorque[0].ToString( );
            textBox34.Text = efortData.DbAxisTorque[0].ToString( );
            textBox38.Text = efortData.DbAxisTorque[0].ToString( );
        }

        private string GetStringFromArray(Array array)
        {
            StringBuilder sb = new StringBuilder( "[" );
            foreach (var item in array)
            {
                sb.Append( item.ToString( ) + "," );
            }
            sb.Append( "]" );
            return sb.ToString( );
        }












        private ER7BC10 efortRobot;

        private void button1_Click( object sender, EventArgs e )
        {
            try
            {
                // 连接
                efortRobot = new ER7BC10( textBox1.Text, int.Parse( textBox2.Text ) );
                efortRobot.ConnectTimeOut = 2000;

                OperateResult connect = efortRobot.ConnectServer( );
                if(connect.IsSuccess)
                {
                    MessageBox.Show( "连接成功" );
                    button1.Enabled = false;
                    button2.Enabled = true;
                    panel2.Enabled = true;
                }
                else
                {
                    MessageBox.Show( "连接失败" );
                }
            }
            catch(Exception ex)
            {
                HslCommunication.BasicFramework.SoftBasic.ShowExceptionMessage( ex );
            }
        }

        private void button2_Click( object sender, EventArgs e )
        {
            efortRobot?.ConnectClose( );
            button2.Enabled = false;
            panel2.Enabled = false;
            button1.Enabled = true;
            timer?.Stop( );
            button3.Enabled = true;
        }

        private void button_read_short_Click( object sender, EventArgs e )
        {
            // 刷新数据
            OperateResult<EfortData> read = efortRobot.Read( );
            if(!read.IsSuccess)
            {
                MessageBox.Show( "读取失败！" + read.Message );
            }
            else
            {
                RenderRobotData( read.Content );
            }
        }

        private Timer timer;

        private void button3_Click( object sender, EventArgs e )
        {
            try
            {
                // 定时读取
                timer = new Timer( );
                timer.Interval = int.Parse( textBox3.Text );
                timer.Tick += Timer_Tick;
                timer.Start( );
                button3.Enabled = false;
            }
            catch(Exception ex)
            {
                // 因为有可能时间文本的格式输入错误
                HslCommunication.BasicFramework.SoftBasic.ShowExceptionMessage( ex );
            }
        }

        private void Timer_Tick( object sender, EventArgs e )
        {
            OperateResult<EfortData> read = efortRobot.Read( );
            if(read.IsSuccess)
            {
                RenderRobotData( read.Content );
            }
        }
    }
}
