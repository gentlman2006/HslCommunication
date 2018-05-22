using System;
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
        }
        




        #region Private Member

        private SerialPort SP_ReadData = null;                    // 串口交互的核心

        #endregion

        private void button1_Click( object sender, EventArgs e )
        {
            if (!int.TryParse( textBox2.Text, out int baudRate ))
            {
                MessageBox.Show( "波特率输入错误！" );
                return;
            }

            if (!int.TryParse( textBox16.Text, out int dataBits ))
            {
                MessageBox.Show( "数据位输入错误！" );
                return;
            }

            if (!int.TryParse( textBox17.Text, out int stopBits ))
            {
                MessageBox.Show( "停止位输入错误！" );
                return;
            }


            SP_ReadData = new SerialPort( );
            SP_ReadData.PortName = textBox1.Text;
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
            System.Threading.Thread.Sleep( 20 );
            byte[] buffer = new byte[SP_ReadData.BytesToRead];
            SP_ReadData.Read( buffer, 0, SP_ReadData.BytesToRead );

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
                     textBox6.AppendText( "[" + DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss.fff" ) + "]   " + msg + Environment.NewLine );
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
                    MessageBox.Show( "CRC校验码输入失败！" );
                    return;
                }
            }

            if(checkBox3.Checked)
            {
                // 显示发送信息
                if(checkBox4.Checked)
                {
                    textBox6.AppendText( "[" + DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss.fff" ) + "]   " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( send,' ' ) + Environment.NewLine );
                }
                else
                {
                    textBox6.AppendText( HslCommunication.BasicFramework.SoftBasic.ByteToHexString( send ) + Environment.NewLine );
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
    }
}
