using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication.Enthernet.Redis;
using HslCommunication;

namespace HslCommunicationDemo
{
    #region FormSimplifyNet


    public partial class FormRedisClient : Form
    {
        public FormRedisClient( )
        {
            InitializeComponent( );
        }

        private void FormClient_Load( object sender, EventArgs e )
        {
            panel2.Enabled = false;
            textBox3.Text = Guid.Empty.ToString( );
            button2.Enabled = false;

            Language( Program.Language );


            if (!Program.ShowAuthorInfomation)
            {
                label2.Visible = false;
                linkLabel1.Visible = false;
                label20.Visible = false;
            }
        }

        private void Language( int language )
        {
            if (language == 1)
            {
                Text = "Redis网络客户端";
                label2.Text = "博客地址：";
                label4.Text = "使用协议：";
                label1.Text = "Ip地址：";
                label3.Text = "端口号：";
                button1.Text = "连接";
                button2.Text = "断开连接";
                label6.Text = "令牌：";
                button5.Text = "启动短连接";
                button6.Text = "压力测试";
                label7.Text = "指令头：";
                label8.Text = "耗时";
                label9.Text = "数据：";
                button3.Text = "写入";
                label10.Text = "次数：";
                label11.Text = "耗时：";
                button4.Text = "读取";
                label12.Text = "接收：";
                label5.Text = "Hsl协议";
                label20.Text = "作者：Richard Hu";
            }
            else
            {
                Text = "Redis Client Test";
                label2.Text = "Blogs:";
                label4.Text = "Protocols:";
                label1.Text = "Ip:";
                label3.Text = "Port:";
                button1.Text = "Connect";
                button2.Text = "Disconnect";
                label6.Text = "Token:";
                button5.Text = "Start a short connection";
                button6.Text = "Pressure test";
                label7.Text = "Command:";
                label8.Text = "Take:";
                label9.Text = "Data:";
                button3.Text = "Write";
                label10.Text = "Times:";
                label11.Text = "Take:";
                button4.Text = "Read";
                label12.Text = "Receive:";
                label5.Text = "Hsl protocol";
                label20.Text = "Author:Richard Hu";
            }
        }

        private RedisClient redisClient = new RedisClient( );

        private void button1_Click( object sender, EventArgs e )
        {
            // 连接
            redisClient.IpAddress = textBox1.Text;
            redisClient.Port = int.Parse( textBox2.Text );
            redisClient.Token = new Guid( textBox3.Text );
            OperateResult connect = redisClient.ConnectServer( );

            if(connect.IsSuccess)
            {
                button1.Enabled = false;
                button2.Enabled = true;
                panel2.Enabled = true;
                button5.Enabled = false;
                MessageBox.Show( StringResources.Language.ConnectServerSuccess );
            }
            else
            {
                MessageBox.Show( StringResources.Language.ConnectedFailed + connect.ToMessageShowString( ) );
            }
        }

        private void button2_Click( object sender, EventArgs e )
        {
            // 断开连接
            button5.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = false;
            panel2.Enabled = false;

            redisClient.ConnectClose( );
        }

        private int status = 1;
        private void button5_Click( object sender, EventArgs e )
        {
            if (status == 1)
            {
                // 启动短连接
                redisClient.IpAddress = textBox1.Text;
                redisClient.Port = int.Parse( textBox2.Text );

                button1.Enabled = false;
                button2.Enabled = false;
                panel2.Enabled = true;
                status = 2;
                button5.Text = Program.Language == 1 ? "重新选择连接" : "Choose again";
            }
            else
            {
                status = 1;
                button1.Enabled = true;
                panel2.Enabled = false;
                button5.Text = Program.Language == 1 ? "启动短连接" : "Start a short connection";
            }
        }

        private void button3_Click( object sender, EventArgs e )
        {
            // 写入关键字
            DateTime start = DateTime.Now;
            int count = int.Parse( textBox6.Text );
            for (int i = 0; i < count; i++)
            {
                OperateResult write = redisClient.WriteKey( textBox5.Text, textBox4.Text );
                if (write.IsSuccess)
                {
                    textBox7.Text = (DateTime.Now - start).TotalMilliseconds.ToString( "F2" );
                    MessageBox.Show( "success" );
                }
                else
                {
                    textBox7.Text = (DateTime.Now - start).TotalMilliseconds.ToString( "F2" );
                    MessageBox.Show( Program.Language == 1 ? "写入失败：" : "Write Failed:" + write.ToMessageShowString( ) );
                }

            }
            
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

        private void button4_Click_1( object sender, EventArgs e )
        {
            // 读关键字
            DateTime start = DateTime.Now;
            int count = int.Parse( textBox9.Text );
            for (int i = 0; i < count; i++)
            {
                OperateResult<string> read = redisClient.ReadKey( textBox11.Text);
                if (read.IsSuccess)
                {
                    textBox10.Text = read.Content;
                }
                else
                {
                    textBox10.Text = Program.Language == 1 ? "读取失败：" : "Read Failed:" + read.ToMessageShowString( );
                }

            }
            textBox8.Text = (DateTime.Now - start).TotalMilliseconds.ToString( "F2" );
        }
    }


    #endregion
}
