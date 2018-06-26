using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication.Profinet;
using HslCommunication;
using HslCommunication.ModBus;
using System.Threading;

namespace ModbusTcpServer
{
    public partial class FormModbus : Form
    {
        public FormModbus( )
        {
            InitializeComponent( );
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

        private void FormSiemens_Load( object sender, EventArgs e )
        {
            panel2.Enabled = false;
            
        }


        private System.Windows.Forms.Timer timerSecond;

        private void FormSiemens_FormClosing( object sender, FormClosingEventArgs e )
        {

        }

        /// <summary>
        /// 统一的读取结果的数据解析，显示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="address"></param>
        /// <param name="textBox"></param>
        private void readResultRender<T>( T result, string address, TextBox textBox )
        {
            textBox.AppendText( DateTime.Now.ToString( "[HH:mm:ss] " ) + $"[{address}] {result}{Environment.NewLine}" );
        }

        /// <summary>
        /// 统一的数据写入的结果显示
        /// </summary>
        /// <param name="result"></param>
        /// <param name="address"></param>
        private void writeResultRender( string address )
        {
            MessageBox.Show( DateTime.Now.ToString( "[HH:mm:ss] " ) + $"[{address}] 写入成功" );
        }


        #region Server Start


        private HslCommunication.ModBus.ModbusTcpServer busTcpServer;

        private void button1_Click( object sender, EventArgs e )
        {
            if (!int.TryParse( textBox2.Text, out int port ))
            {
                MessageBox.Show( "端口输入不正确！" );
                return;
            }


            try
            {

                busTcpServer = new HslCommunication.ModBus.ModbusTcpServer( );                       // 实例化对象
                busTcpServer.LogNet = new HslCommunication.LogNet.LogNetSingle( "logs.txt" );        // 配置日志信息
                busTcpServer.LogNet.BeforeSaveToFile += LogNet_BeforeSaveToFile;
                busTcpServer.OnDataReceived += BusTcpServer_OnDataReceived;
                busTcpServer.ServerStart( port );

                button1.Enabled = false;
                panel2.Enabled = true;
                button4.Enabled = true;

                timerSecond = new System.Windows.Forms.Timer( );
                timerSecond.Interval = 1000;
                timerSecond.Tick += TimerSecond_Tick;
                timerSecond.Start( );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void TimerSecond_Tick( object sender, EventArgs e )
        {
            label15.Text = busTcpServer.OnlineCount.ToString( ) ;
        }

        private void BusTcpServer_OnDataReceived( HslCommunication.ModBus.ModbusTcpServer tcpServer, byte[] modbus )
        {
            if (!checkBox1.Checked) return;

            if (InvokeRequired)
            {
                BeginInvoke( new Action<HslCommunication.ModBus.ModbusTcpServer,byte[]>( BusTcpServer_OnDataReceived ), tcpServer, modbus );
                return;
            }

            textBox1.AppendText( "接收数据：" + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( modbus, ' ' ) + Environment.NewLine );
        }

        /// <summary>
        /// 当有日志记录的时候，触发，将日志信息也在主界面进行输出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogNet_BeforeSaveToFile( object sender, HslCommunication.LogNet.HslEventArgs e )
        {
            if (InvokeRequired)
            {
                Invoke( new Action<object, HslCommunication.LogNet.HslEventArgs>( LogNet_BeforeSaveToFile ), sender, e );
                return;
            }

            textBox1.AppendText( e.HslMessage.ToString( ) + Environment.NewLine );
        }



        #endregion

        #region 单数据读取测试


        private void button_read_bool_Click( object sender, EventArgs e )
        {
            // 读取线圈bool变量
            readResultRender( busTcpServer.ReadCoil( textBox3.Text  ), textBox3.Text, textBox4 );
        }
        
        private void button6_Click( object sender, EventArgs e )
        {
            // 读取离散bool变量
            readResultRender( busTcpServer.ReadDiscrete( textBox3.Text  ), textBox3.Text, textBox4 );
        }

        private void button_read_short_Click( object sender, EventArgs e )
        {
            // 读取short变量
            readResultRender( busTcpServer.ReadInt16( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_ushort_Click( object sender, EventArgs e )
        {
            // 读取ushort变量
            readResultRender( busTcpServer.ReadUInt16( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_int_Click( object sender, EventArgs e )
        {
            // 读取int变量
            readResultRender( busTcpServer.ReadInt32( textBox3.Text ), textBox3.Text, textBox4 );
        }
        private void button_read_uint_Click( object sender, EventArgs e )
        {
            // 读取uint变量
            readResultRender( busTcpServer.ReadUInt32( textBox3.Text ), textBox3.Text, textBox4 );
        }
        private void button_read_long_Click( object sender, EventArgs e )
        {
            // 读取long变量
            readResultRender( busTcpServer.ReadInt64( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_ulong_Click( object sender, EventArgs e )
        {
            // 读取ulong变量
            readResultRender( busTcpServer.ReadUInt64( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_float_Click( object sender, EventArgs e )
        {
            // 读取float变量
            readResultRender( busTcpServer.ReadFloat( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_double_Click( object sender, EventArgs e )
        {
            // 读取double变量
            readResultRender( busTcpServer.ReadDouble( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_string_Click( object sender, EventArgs e )
        {
            // 读取字符串
            readResultRender( busTcpServer.ReadString( textBox3.Text, ushort.Parse( textBox5.Text ) ), textBox3.Text, textBox4 );
        }


        #endregion

        #region 单数据写入测试


        private void button24_Click( object sender, EventArgs e )
        {
            // bool写入
            try
            {
                busTcpServer.WriteCoil( textBox8.Text, bool.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button7_Click( object sender, EventArgs e )
        {
            // 离散bool写入
            try
            {
                busTcpServer.WriteDiscrete( textBox8.Text, bool.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button22_Click( object sender, EventArgs e )
        {
            // short写入
            try
            {
                busTcpServer.Write( textBox8.Text, short.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button21_Click( object sender, EventArgs e )
        {
            // ushort写入
            try
            {
                busTcpServer.Write(textBox8.Text, ushort.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }


        private void button20_Click( object sender, EventArgs e )
        {
            // int写入
            try
            {
                busTcpServer.Write( textBox8.Text, int.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button19_Click( object sender, EventArgs e )
        {
            // uint写入
            try
            {
                busTcpServer.Write( textBox8.Text , uint.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button18_Click( object sender, EventArgs e )
        {
            // long写入
            try
            {
                busTcpServer.Write( textBox8.Text, long.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button17_Click( object sender, EventArgs e )
        {
            // ulong写入
            try
            {
                busTcpServer.Write(textBox8.Text , ulong.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button16_Click( object sender, EventArgs e )
        {
            // float写入
            try
            {
                busTcpServer.Write( textBox8.Text, float.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button15_Click( object sender, EventArgs e )
        {
            // double写入
            try
            {
                busTcpServer.Write( textBox8.Text, double.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }


        private void button14_Click( object sender, EventArgs e )
        {
            // string写入
            try
            {
                busTcpServer.Write( textBox8.Text, textBox7.Text );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }



        #endregion

        private void button2_Click( object sender, EventArgs e )
        {
            // 点击数据监视
            ModBusMonitorAddress monitorAddress = new ModBusMonitorAddress( );
            monitorAddress.Address = ushort.Parse( textBox6.Text );
            monitorAddress.OnChange += MonitorAddress_OnChange;
            monitorAddress.OnWrite += MonitorAddress_OnWrite;
            busTcpServer.AddSubcription( monitorAddress );
            button2.Enabled = false;
        }

        private void MonitorAddress_OnWrite( ModBusMonitorAddress monitor, short value )
        {
            // 当有客户端写入时就触发
        }

        private void MonitorAddress_OnChange( ModBusMonitorAddress monitor, short befor, short after )
        {
            // 当该地址的值更改的时候触发
            if (InvokeRequired)
            {
                BeginInvoke( new Action<ModBusMonitorAddress, short, short>( MonitorAddress_OnChange ), monitor, befor, after );
                return;
            }

            textBox9.Text = after.ToString( );

            label11.Text = "写入时间：" + DateTime.Now.ToString( ) + " 修改前：" + befor + " 修改后：" + after;
        }

        private void linkLabel2_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            HslCommunication.BasicFramework.FormSupport form = new HslCommunication.BasicFramework.FormSupport( );
            form.ShowDialog( );
        }

        private void button3_Click( object sender, EventArgs e )
        {
            if (busTcpServer == null)
            {
                MessageBox.Show( "必须先启动服务器！" );
                return;
            }
            // 信任客户端配置
            using (FormTrustedClient form = new FormTrustedClient( busTcpServer ))
            {
                form.ShowDialog( );
            }
        }


        private void Test1( )
        {
            bool Coil100 = busTcpServer.ReadCoil( "100" );                  // 读线圈100的值
            bool[] Coil100_109 = busTcpServer.ReadCoil( "100", 10 );        // 读线圈数组
            short Short100 = busTcpServer.ReadInt16( "100" );               // 读取寄存器值
            ushort UShort100 = busTcpServer.ReadUInt16( "100" );            // 读取寄存器ushort值
            int Int100 = busTcpServer.ReadInt32( "100" );                   // 读取寄存器int值
            uint UInt100 = busTcpServer.ReadUInt32( "100" );                // 读取寄存器uint值
            float Float100 = busTcpServer.ReadFloat( "100" );               // 读取寄存器Float值
            long Long100 = busTcpServer.ReadInt64( "100" );                 // 读取寄存器long值
            ulong ULong100 = busTcpServer.ReadUInt64( "100" );              // 读取寄存器ulong值
            double Double100 = busTcpServer.ReadDouble( "100" );            // 读取寄存器double值

            busTcpServer.WriteCoil( "100", true );                          // 写线圈的通断
            busTcpServer.Write( "100", (short)5 );                          // 写入short值
            busTcpServer.Write( "100", (ushort)45678 );                     // 写入ushort值
            busTcpServer.Write( "100", 12345667 );                          // 写入int值
            busTcpServer.Write( "100", (uint)12312312 );                    // 写入uint值
            busTcpServer.Write( "100", 123.456f );                          // 写入float值
            busTcpServer.Write( "100", 1231231231233L );                    // 写入long值
            busTcpServer.Write( "100", 1212312313UL );                      // 写入ulong值
            busTcpServer.Write( "100", 123.456d );                          // 写入double值
        }

        private void button4_Click( object sender, EventArgs e )
        {
            // 连接异形客户端
            using (FormInputAlien form = new FormInputAlien( ))
            {
                if (form.ShowDialog( ) == DialogResult.OK)
                {
                    OperateResult connect = busTcpServer.ConnectHslAlientClient( form.IpAddress, form.Port, form.DTU );
                    if (connect.IsSuccess)
                    {
                        MessageBox.Show( "连接成功！" );
                    }
                    else
                    {
                        MessageBox.Show( "连接失败！原因：" + connect.Message );
                    }
                }
            }
        }

        private void button5_Click( object sender, EventArgs e )
        {
            // 启动串口
            if (busTcpServer != null)
            {
                try
                {
                    busTcpServer.StartSerialPort( textBox10.Text );
                    button5.Enabled = false;
                }
                catch(Exception ex)
                {
                    MessageBox.Show( "串口服务器启动失败：" + ex.Message );
                }
            }
            else
            {
                MessageBox.Show( "请先启动Tcp服务器：" );
            }
        }


    }
}
