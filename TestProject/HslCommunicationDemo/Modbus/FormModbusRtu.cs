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

namespace HslCommunicationDemo
{
    public partial class FormModbusRtu : Form
    {
        public FormModbusRtu( )
        {
            InitializeComponent( );
        }


        private ModbusRtu busRtuClient = null;

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
            userCurve1.SetLeftCurve( "A", new float[0], Color.Tomato );
            comboBox1.SelectedIndex = 0;
        }

        private void FormSiemens_FormClosing( object sender, FormClosingEventArgs e )
        {
            isThreadRun = false;
        }

        /// <summary>
        /// 统一的读取结果的数据解析，显示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="address"></param>
        /// <param name="textBox"></param>
        private void readResultRender<T>( OperateResult<T> result, string address, TextBox textBox )
        {
            if (result.IsSuccess)
            {
                textBox.AppendText( DateTime.Now.ToString( "[HH:mm:ss] " ) + $"[{address}] {result.Content}{Environment.NewLine}" );
            }
            else
            {
                MessageBox.Show( DateTime.Now.ToString( "[HH:mm:ss] " ) + $"[{address}] 读取失败{Environment.NewLine}原因：{result.ToMessageShowString( )}" );
            }
        }

        /// <summary>
        /// 统一的数据写入的结果显示
        /// </summary>
        /// <param name="result"></param>
        /// <param name="address"></param>
        private void writeResultRender( OperateResult result, string address )
        {
            if (result.IsSuccess)
            {
                MessageBox.Show( DateTime.Now.ToString( "[HH:mm:ss] " ) + $"[{address}] 写入成功" );
            }
            else
            {
                MessageBox.Show( DateTime.Now.ToString( "[HH:mm:ss] " ) + $"[{address}] 写入失败{Environment.NewLine}原因：{result.ToMessageShowString( )}" );
            }
        }


        #region Connect And Close



        private void button1_Click( object sender, EventArgs e )
        {
            if(!int.TryParse(textBox2.Text,out int baudRate ))
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


            if (!byte.TryParse(textBox15.Text,out byte station))
            {
                MessageBox.Show( "站号输入不正确！" );
                return;
            }

            busRtuClient?.Close( );
            busRtuClient = new ModbusRtu( station );
            busRtuClient.AddressStartWithZero = checkBox1.Checked;
            busRtuClient.IsMultiWordReverse = checkBox2.Checked;
            busRtuClient.IsStringReverse = checkBox3.Checked;


            try
            {
                busRtuClient.SerialPortInni( sp =>
                 {
                     sp.PortName = textBox1.Text;
                     sp.BaudRate = baudRate;
                     sp.DataBits = dataBits;
                     sp.StopBits = stopBits == 0 ? System.IO.Ports.StopBits.None : (stopBits == 1 ? System.IO.Ports.StopBits.One : System.IO.Ports.StopBits.Two);
                     sp.Parity = comboBox1.SelectedIndex == 0 ? System.IO.Ports.Parity.None : (comboBox1.SelectedIndex == 1 ? System.IO.Ports.Parity.Odd : System.IO.Ports.Parity.Even);
                 } );
                busRtuClient.Open( );

                button2.Enabled = true;
                button1.Enabled = false;
                panel2.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button2_Click( object sender, EventArgs e )
        {
            // 断开连接
            busRtuClient.Close( );
            button2.Enabled = false;
            button1.Enabled = true;
            panel2.Enabled = false;
        }







        #endregion

        #region 单数据读取测试


        private void button_read_bool_Click( object sender, EventArgs e )
        {
            // 读取bool变量
            readResultRender( busRtuClient.ReadCoil( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button4_Click_1( object sender, EventArgs e )
        {
            // 离散输入读取
            readResultRender( busRtuClient.ReadDiscrete( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_short_Click( object sender, EventArgs e )
        {
            // 读取short变量
            readResultRender( busRtuClient.ReadInt16( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_ushort_Click( object sender, EventArgs e )
        {
            // 读取ushort变量
            readResultRender( busRtuClient.ReadUInt16( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_int_Click( object sender, EventArgs e )
        {
            // 读取int变量
            readResultRender( busRtuClient.ReadInt32(  textBox3.Text ), textBox3.Text, textBox4 );
        }
        private void button_read_uint_Click( object sender, EventArgs e )
        {
            // 读取uint变量
            readResultRender( busRtuClient.ReadUInt32( textBox3.Text ), textBox3.Text, textBox4 );
        }
        private void button_read_long_Click( object sender, EventArgs e )
        {
            // 读取long变量
            readResultRender( busRtuClient.ReadInt64( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_ulong_Click( object sender, EventArgs e )
        {
            // 读取ulong变量
            readResultRender( busRtuClient.ReadUInt64( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_float_Click( object sender, EventArgs e )
        {
            // 读取float变量
            readResultRender( busRtuClient.ReadFloat( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_double_Click( object sender, EventArgs e )
        {
            // 读取double变量
            readResultRender( busRtuClient.ReadDouble( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_string_Click( object sender, EventArgs e )
        {
            // 读取字符串
            readResultRender( busRtuClient.ReadString( textBox3.Text , ushort.Parse( textBox5.Text ) ), textBox3.Text, textBox4 );
        }


        #endregion

        #region 单数据写入测试


        private void button24_Click( object sender, EventArgs e )
        {
            // bool写入
            try
            {
                writeResultRender( busRtuClient.WriteCoil( textBox8.Text, bool.Parse( textBox7.Text ) ), textBox8.Text );
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
                writeResultRender( busRtuClient.Write( textBox8.Text , short.Parse( textBox7.Text ) ), textBox8.Text );
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
                writeResultRender( busRtuClient.Write( textBox8.Text , ushort.Parse( textBox7.Text ) ), textBox8.Text );
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
                writeResultRender( busRtuClient.Write( textBox8.Text , int.Parse( textBox7.Text ) ), textBox8.Text );
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
                writeResultRender( busRtuClient.Write( textBox8.Text , uint.Parse( textBox7.Text ) ), textBox8.Text );
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
                writeResultRender( busRtuClient.Write( textBox8.Text , long.Parse( textBox7.Text ) ), textBox8.Text );
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
                writeResultRender( busRtuClient.Write( textBox8.Text , ulong.Parse( textBox7.Text ) ), textBox8.Text );
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
                writeResultRender( busRtuClient.Write( textBox8.Text , float.Parse( textBox7.Text ) ), textBox8.Text );
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
                writeResultRender( busRtuClient.Write( textBox8.Text , double.Parse( textBox7.Text ) ), textBox8.Text );
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
                writeResultRender( busRtuClient.Write( textBox8.Text , textBox7.Text ), textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }




        #endregion

        #region 批量读取测试

        private void button25_Click( object sender, EventArgs e )
        {
            try
            {
                OperateResult<byte[]> read = busRtuClient.Read( textBox6.Text , ushort.Parse( textBox9.Text ) );
                if (read.IsSuccess)
                {
                    textBox10.Text = "结果：" + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( read.Content );
                }
                else
                {
                    MessageBox.Show( "读取失败：" + read.ToMessageShowString( ) );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show( "读取失败：" + ex.Message );
            }
        }



        #endregion

        #region 报文读取测试


        private void button26_Click( object sender, EventArgs e )
        {
            try
            {
                OperateResult<byte[]> read = busRtuClient.ReadBase( HslCommunication.Serial.SoftCRC16.CRC16(HslCommunication.BasicFramework.SoftBasic.HexStringToBytes( textBox13.Text )) );
                if (read.IsSuccess)
                {
                    textBox11.Text = "结果：" + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( read.Content );
                }
                else
                {
                    MessageBox.Show( "读取失败：" + read.ToMessageShowString( ) );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show( "读取失败：" + ex.Message );
            }
        }


        #endregion

        #region 定时器读取测试

        // 外加曲线显示

        private Thread thread = null;              // 后台读取的线程
        private int timeSleep = 300;               // 读取的间隔
        private bool isThreadRun = false;          // 用来标记线程的运行状态

        private void button27_Click( object sender, EventArgs e )
        {
            // 启动后台线程，定时读取PLC中的数据，然后在曲线控件中显示

            if (!isThreadRun)
            {
                if (!int.TryParse( textBox14.Text, out timeSleep ))
                {
                    MessageBox.Show( "间隔时间格式输入错误！" );
                    return;
                }
                button27.Text = "停止";
                isThreadRun = true;
                thread = new Thread( ThreadReadServer );
                thread.IsBackground = true;
                thread.Start( );
            }
            else
            {
                button27.Text = "启动";
                isThreadRun = false;
            }
        }

        private void ThreadReadServer()
        {
            while (isThreadRun)
            {
                Thread.Sleep( timeSleep );

                try
                {
                    OperateResult<short> read = busRtuClient.ReadInt16( textBox12.Text );
                    if (read.IsSuccess)
                    {
                        // 显示曲线
                        if (isThreadRun) Invoke( new Action<short>( AddDataCurve ), read.Content );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show( "读取失败：" + ex.Message );
                }

            }
        }


        private void AddDataCurve( short data )
        {
            userCurve1.AddCurveData( "A", data );
        }


        #endregion

        #region 压力测试

        private void button4_Click( object sender, EventArgs e )
        {
            PressureTest2( );
        }

        private int thread_status = 0;
        private int failed = 0;
        private DateTime thread_time_start = DateTime.Now;
        // 压力测试，开3个线程，每个线程进行读写操作，看使用时间
        private void PressureTest2( )
        {
            thread_status = 3;
            failed = 0;
            thread_time_start = DateTime.Now;
            new Thread( new ThreadStart( thread_test2 ) ) { IsBackground = true, }.Start( );
            new Thread( new ThreadStart( thread_test2 ) ) { IsBackground = true, }.Start( );
            new Thread( new ThreadStart( thread_test2 ) ) { IsBackground = true, }.Start( );
            button3.Enabled = false;
        }

        private void thread_test2( )
        {
            int count = 500;
            while (count > 0)
            {
                if (!busRtuClient.Write( "100", (short)1234 ).IsSuccess) failed++;
                if (!busRtuClient.ReadInt16( "100" ).IsSuccess) failed++;
                count--;
            }
            thread_end( );
        }

        private void thread_end( )
        {
            if (Interlocked.Decrement( ref thread_status ) == 0)
            {
                // 执行完成
                Invoke( new Action( ( ) =>
                {
                    button3.Enabled = true;
                    MessageBox.Show( "耗时：" + (DateTime.Now - thread_time_start).TotalSeconds + Environment.NewLine + "失败次数：" + failed );
                } ) );
            }
        }
        
        #endregion

        #region Test Function


        private void Test1()
        {
            OperateResult<bool[]> read = busRtuClient.ReadCoil( "100", 10 );
            if(read.IsSuccess)
            {
                bool coil_100 = read.Content[0];
                // and so on 
                bool coil_109 = read.Content[9];
            }
            else
            {
                // failed
                string err = read.Message;
            }
        }


        private void Test2()
        {
            bool[] values = new bool[] { true, false, false, false, true, true, false, true, false, false };
            OperateResult write = busRtuClient.WriteCoil( "100", values );
            if (write.IsSuccess)
            {
                // success
            }
            else
            {
                // failed
                string err = write.Message;
            }

            HslCommunication.Core.IByteTransform ByteTransform = new HslCommunication.Core.ReverseWordTransform( );
        }


        #endregion

        
    }
}
