using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication.Profinet;
using System.Threading;
using HslCommunication.Profinet.Melsec;
using HslCommunication;

namespace HslCommunicationDemo
{
    public partial class FormMelsecBinary : Form
    {
        public FormMelsecBinary()
        {
            InitializeComponent( );
            melsec_net = new MelsecMcNet( );
        }


        private MelsecMcNet melsec_net = null;

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
            // 连接
            if (!System.Net.IPAddress.TryParse( textBox1.Text, out System.Net.IPAddress address ))
            {
                MessageBox.Show( "Ip地址输入不正确！" );
                return;
            }

            melsec_net.IpAddress = textBox1.Text;

            if(!int.TryParse(textBox2.Text,out int port))
            {
                MessageBox.Show( "端口输入格式不正确！" );
                return;
            }

            melsec_net.Port = port;

            melsec_net.ConnectClose( );

            try
            {
                OperateResult connect = melsec_net.ConnectServer( );
                if (connect.IsSuccess)
                {
                    MessageBox.Show( "连接成功！" );
                    button2.Enabled = true;
                    button1.Enabled = false;
                    panel2.Enabled = true;
                }
                else
                {
                    MessageBox.Show( "连接失败！" );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button2_Click( object sender, EventArgs e )
        {
            // 断开连接
            melsec_net.ConnectClose( );
            button2.Enabled = false;
            button1.Enabled = true;
            panel2.Enabled = false;
        }







        #endregion

        #region 单数据读取测试


        private void button_read_bool_Click( object sender, EventArgs e )
        {
            // 读取bool变量
            readResultRender( melsec_net.ReadBool( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_short_Click( object sender, EventArgs e )
        {
            // 读取short变量
            readResultRender( melsec_net.ReadInt16( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_ushort_Click( object sender, EventArgs e )
        {
            // 读取ushort变量
            readResultRender( melsec_net.ReadUInt16( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_int_Click( object sender, EventArgs e )
        {
            // 读取int变量
            readResultRender( melsec_net.ReadInt32( textBox3.Text ), textBox3.Text, textBox4 );
        }
        private void button_read_uint_Click( object sender, EventArgs e )
        {
            // 读取uint变量
            readResultRender( melsec_net.ReadUInt32( textBox3.Text ), textBox3.Text, textBox4 );
        }
        private void button_read_long_Click( object sender, EventArgs e )
        {
            // 读取long变量
            readResultRender( melsec_net.ReadInt64( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_ulong_Click( object sender, EventArgs e )
        {
            // 读取ulong变量
            readResultRender( melsec_net.ReadUInt64( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_float_Click( object sender, EventArgs e )
        {
            // 读取float变量
            readResultRender( melsec_net.ReadFloat( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_double_Click( object sender, EventArgs e )
        {
            // 读取double变量
            readResultRender( melsec_net.ReadDouble( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_string_Click( object sender, EventArgs e )
        {
            // 读取字符串
            readResultRender( melsec_net.ReadString( textBox3.Text, ushort.Parse( textBox5.Text ) ), textBox3.Text, textBox4 );
        }


        #endregion

        #region 单数据写入测试


        private void button24_Click( object sender, EventArgs e )
        {
            // bool写入
            try
            {
                writeResultRender( melsec_net.Write( textBox8.Text,new bool[] { bool.Parse( textBox7.Text ) } ), textBox8.Text );
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
                writeResultRender( melsec_net.Write( textBox8.Text, short.Parse( textBox7.Text ) ), textBox8.Text );
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
                writeResultRender( melsec_net.Write( textBox8.Text, ushort.Parse( textBox7.Text ) ), textBox8.Text );
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
                writeResultRender( melsec_net.Write( textBox8.Text, int.Parse( textBox7.Text ) ), textBox8.Text );
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
                writeResultRender( melsec_net.Write( textBox8.Text, uint.Parse( textBox7.Text ) ), textBox8.Text );
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
                writeResultRender( melsec_net.Write( textBox8.Text, long.Parse( textBox7.Text ) ), textBox8.Text );
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
                writeResultRender( melsec_net.Write( textBox8.Text, ulong.Parse( textBox7.Text ) ), textBox8.Text );
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
                writeResultRender( melsec_net.Write( textBox8.Text, float.Parse( textBox7.Text ) ), textBox8.Text );
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
                writeResultRender( melsec_net.Write( textBox8.Text, double.Parse( textBox7.Text ) ), textBox8.Text );
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
                writeResultRender( melsec_net.Write( textBox8.Text, textBox7.Text ), textBox8.Text );
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
                OperateResult<byte[]> read = melsec_net.Read( textBox6.Text, ushort.Parse( textBox9.Text ) );
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
                OperateResult<byte[]> read = melsec_net.ReadFromCoreServer( HslCommunication.BasicFramework.SoftBasic.HexStringToBytes( textBox13.Text ) );
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
                    OperateResult<short> read = melsec_net.ReadInt16( textBox12.Text );
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

        #region 测试使用

        private void test1()
        {
            OperateResult<bool[]> read = melsec_net.ReadBool( "M100", 10 );
            if(read.IsSuccess)
            {
                bool m100 = read.Content[0];
                // and so on
                bool m109 = read.Content[9];
            }
            else
            {
                // failed
            }
        }

        private void test2( )
        {
            bool[] values = new bool[] { true, false, true, true, false, true, false, true, true, false };
            OperateResult read = melsec_net.Write( "M100", values );
            if (read.IsSuccess)
            {
                // success
            }
            else
            {
                // failed
            }
        }


        private void test3( )
        {
            short d100_short = melsec_net.ReadInt16( "D100" ).Content;
            ushort d100_ushort = melsec_net.ReadUInt16( "D100" ).Content;
            int d100_int = melsec_net.ReadInt32( "D100" ).Content;
            uint d100_uint = melsec_net.ReadUInt32( "D100" ).Content;
            long d100_long = melsec_net.ReadInt64( "D100" ).Content;
            ulong d100_ulong = melsec_net.ReadUInt64( "D100" ).Content;
            float d100_float = melsec_net.ReadFloat( "D100" ).Content;
            double d100_double = melsec_net.ReadDouble( "D100" ).Content;
            // need to specify the text length
            string d100_string = melsec_net.ReadString( "D100", 10 ).Content;
        }
        private void test4( )
        {
            melsec_net.Write( "D100", (short)5 );
            melsec_net.Write( "D100", (ushort)5 );
            melsec_net.Write( "D100", 5 );
            melsec_net.Write( "D100", (uint)5 );
            melsec_net.Write( "D100", (long)5 );
            melsec_net.Write( "D100", (ulong)5 );
            melsec_net.Write( "D100", 5f );
            melsec_net.Write( "D100", 5d );
            // length should Multiples of 2 
            melsec_net.Write( "D100", "12345678" );
        }


        private void test5( )
        {
            OperateResult<byte[]> read = melsec_net.Read( "D100", 10 );
            if(read.IsSuccess)
            {
                int count = melsec_net.ByteTransform.TransInt32( read.Content, 0 );
                float temp = melsec_net.ByteTransform.TransSingle( read.Content, 4 );
                short name1 = melsec_net.ByteTransform.TransInt16( read.Content, 8 );
                string barcode = Encoding.ASCII.GetString( read.Content, 10, 10 );
            }
        }

        private void test6( )
        {
            OperateResult<UserType> read = melsec_net.ReadCustomer<UserType>( "D100" );
            if (read.IsSuccess)
            {
                UserType value = read.Content;
            }
            // write value
            melsec_net.WriteCustomer( "D100", new UserType( ) );

            melsec_net.LogNet = new HslCommunication.LogNet.LogNetSingle( Application.StartupPath + "\\Logs.txt" );

        }

        // private MelsecMcAsciiNet melsec_ascii_net = null;

        #endregion

        #region 压力测试

        private int thread_status = 0;
        private int failed = 0;
        private DateTime thread_time_start = DateTime.Now;
        // 压力测试，开3个线程，每个线程进行读写操作，看使用时间
        private void button3_Click( object sender, EventArgs e )
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
                if (!melsec_net.Write( "D100", (short)1234 ).IsSuccess) failed++;
                if (!melsec_net.ReadInt16( "D100" ).IsSuccess) failed++;
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


    }

    public class UserType : HslCommunication.IDataTransfer
    {
        #region IDataTransfer

        private HslCommunication.Core.IByteTransform ByteTransform = new HslCommunication.Core.RegularByteTransform();


        public ushort ReadCount => 10;

        public void ParseSource( byte[] Content )
        {
            int count = ByteTransform.TransInt32( Content, 0 );
            float temp = ByteTransform.TransSingle( Content, 4 );
            short name1 = ByteTransform.TransInt16( Content, 8 );
            string barcode = Encoding.ASCII.GetString( Content, 10, 10 );
        }

        public byte[] ToSource( )
        {
            byte[] buffer = new byte[20];
            ByteTransform.TransByte( count ).CopyTo( buffer, 0 );
            ByteTransform.TransByte( temp ).CopyTo( buffer, 4 );
            ByteTransform.TransByte( name1 ).CopyTo( buffer, 8 );
            Encoding.ASCII.GetBytes( barcode ).CopyTo( buffer, 10 );
            return buffer;
        }


        #endregion


        #region Public Data

        public int count { get; set; }

        public float temp { get; set; }

        public short name1 { get; set; }

        public string barcode { get; set; }

        #endregion
    }
}
