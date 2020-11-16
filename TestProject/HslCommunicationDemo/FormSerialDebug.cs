﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HslCommunicationDemo
{
    public partial class FormSerialDebug : Form
    {
        public FormSerialDebug( )
        {
            InitializeComponent( );
        }

        private void FormSerialDebug_Load( object sender, EventArgs e )
        {
            panel2.Enabled = false;

            comboBox1.SelectedIndex = 0;

            comboBox2.DataSource = SerialPort.GetPortNames( );
            try
            {
                comboBox2.SelectedIndex = 0;
            }
            catch
            {
                comboBox2.Text = "COM3";
            }

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
                Text = "串口调试助手";
                label2.Text = "博客地址：";
                label4.Text = "使用协议：";
                label20.Text = "作者：Richard Hu";
                label5.Text = "串口，无协议";
                label1.Text = "Com口：";
                label3.Text = "波特率:";
                label22.Text = "数据位:";
                label23.Text = "停止位:";
                label24.Text = "奇偶：";
                button1.Text = "打开串口";
                button2.Text = "关闭串口";
                label6.Text = "数据发送区：";
                checkBox1.Text = "是否使用二进制通信";
                checkBox2.Text = "是否自动计算校验码";
                label7.Text = "数据接收区：";
                checkBox3.Text = "是否显示发送数据";
                checkBox4.Text = "是否显示时间";
                button3.Text = "发送数据";
                comboBox1.DataSource = new string[] { "无", "奇", "偶" };
            }
            else
            {
                Text = "Serial Debug Tools";
                label2.Text = "Blogs:";
                label4.Text = "Protocols:";
                label20.Text = "Author:Richard Hu";
                label5.Text = "Serial,no protocol";
                label1.Text = "Com:";
                label3.Text = "Baud rate:";
                label22.Text = "Data bits:";
                label23.Text = "Stop bits:";
                label24.Text = "parity:";
                button1.Text = "Open";
                button2.Text = "Close";
                label6.Text = "Data sending Area:";
                checkBox1.Text = "Whether to use binary communication";
                checkBox2.Text = "Whether to automatically calculate the check code";
                label7.Text = "Data receiving Area:";
                checkBox3.Text = "Whether to display send data";
                checkBox4.Text = "Whether to show time";
                button3.Text = "Send Data";
                comboBox1.DataSource = new string[] { "None", "Odd", "Even" };
            }
        }

        // 01 10 00 64 00 10 20 00 00 00 01 00 02 00 03 00 04 00 05 00 06 00 07 00 08 00 09 00 0A 00 0B 00 0C 00 0D 00 0E 00 0F



        #region Private Member

        private SerialPort SP_ReadData = null;                    // 串口交互的核心

        #endregion

        private void button1_Click( object sender, EventArgs e )
        {
            if (!int.TryParse( textBox2.Text, out int baudRate ))
            {
                MessageBox.Show( Program.Language == 1 ? "波特率输入错误！" : "Baud rate input error" );
                return;
            }

            if (!int.TryParse( textBox16.Text, out int dataBits ))
            {
                MessageBox.Show( Program.Language == 1 ? "数据位输入错误！" : "Data bits input error" );
                return;
            }

            if (!int.TryParse( textBox17.Text, out int stopBits ))
            {
                MessageBox.Show( Program.Language == 1 ? "停止位输入错误！" : "Stop bits input error" );
                return;
            }


            SP_ReadData = new SerialPort( );
            SP_ReadData.PortName = comboBox2.Text;
            SP_ReadData.BaudRate = baudRate;
            SP_ReadData.DataBits = dataBits;
            SP_ReadData.StopBits = stopBits == 0 ? StopBits.None : (stopBits == 1 ? StopBits.One : StopBits.Two);
            SP_ReadData.Parity = comboBox1.SelectedIndex == 0 ? Parity.None : (comboBox1.SelectedIndex == 1 ? Parity.Odd : Parity.Even);

            try
            {
                SP_ReadData.DataReceived += SP_ReadData_DataReceived;
                SP_ReadData.Open( );
                button1.Enabled = false;
                button2.Enabled = true;

                panel2.Enabled = true;
            }
            catch(Exception ex)
            {
                HslCommunication.BasicFramework.SoftBasic.ShowExceptionMessage( ex );
            }
        }

        private void SP_ReadData_DataReceived( object sender, SerialDataReceivedEventArgs e )
        {
            // 接收数据
            byte[] buffer = null;
            byte[] data = new byte[2048];
            int receiveCount = 0;
            while (true)
            {
                System.Threading.Thread.Sleep( 20 );
                if(SP_ReadData.BytesToRead < 1)
                {
                    buffer = new byte[receiveCount];
                    Array.Copy( data, 0, buffer, 0, receiveCount );
                    break;
                }

                receiveCount += SP_ReadData.Read( data, receiveCount, SP_ReadData.BytesToRead );
            }

            if (receiveCount == 0) return;

            Invoke( new Action( ( ) =>
             {
                 string msg = string.Empty;
                 if(checkBox1.Checked)
                 {
                     msg = HslCommunication.BasicFramework.SoftBasic.ByteToHexString( buffer, ' ' );
                 }
                 else
                 {
                     msg = Encoding.ASCII.GetString( buffer );
                 }
                 

                 if(checkBox4.Checked)
                 {
                     textBox6.AppendText( "[" + DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss.fff" ) + (Program.Language == 1 ? "][收]   " : "][R]   ") + msg + Environment.NewLine );
                 }
                 else
                 {
                     textBox6.AppendText( msg + Environment.NewLine );
                 }

             } ) );
        }

        private void button3_Click( object sender, EventArgs e )
        {
            // 发送数据
            byte[] send = null;
            if(checkBox1.Checked)
            {
                send = HslCommunication.BasicFramework.SoftBasic.HexStringToBytes( textBox5.Text );
            }
            else
            {
                send = Encoding.ASCII.GetBytes( textBox5.Text );
            }

            if(checkBox2.Checked)
            {
                try
                {
                    send = HslCommunication.Serial.SoftCRC16.CRC16( send, Convert.ToByte( textBox3.Text, 16 ), Convert.ToByte( textBox4.Text, 16 ) );
                }
                catch
                {
                    MessageBox.Show( Program.Language == 1 ? "CRC校验码输入失败！" : "CRC check code input failed" );
                    return;
                }
            }

            if(checkBox3.Checked)
            {
                // 显示发送信息
                if(checkBox4.Checked)
                {
                    textBox6.AppendText( "[" + DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss.fff" ) + (Program.Language == 1 ? "][发]   " : "][S]   ") + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( send,' ' ) + Environment.NewLine );
                }
                else
                {
                    textBox6.AppendText( HslCommunication.BasicFramework.SoftBasic.ByteToHexString( send, ' ' ) + Environment.NewLine );
                }
            }
            SP_ReadData?.Write( send, 0, send.Length );
        }

        private void textBox5_KeyDown( object sender, KeyEventArgs e )
        {
            // 按下 ENTER 键的时候自动发送
            if(e.KeyCode == Keys.Enter)
            {
                button3.PerformClick( );
            }
        }

        private void button2_Click( object sender, EventArgs e )
        {
            // 关闭串口
            try
            {
                SP_ReadData.Close( );
                button2.Enabled = false;
                button1.Enabled = true;

                panel2.Enabled = false;
            }
            catch(Exception ex)
            {
                HslCommunication.BasicFramework.SoftBasic.ShowExceptionMessage( ex );
            }
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
