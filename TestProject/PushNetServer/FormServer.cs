using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication.Enthernet;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using HslCommunication;

namespace PushNetServer
{
    public partial class FormServer : Form
    {
        public FormServer()
        {
            InitializeComponent( );
        }

        private void Form1_Load( object sender, EventArgs e )
        {
            panel2.Enabled = false;

            timerOneSecond = new Timer( );
            timerOneSecond.Interval = 1000;
            timerOneSecond.Tick += TimerOneSecond_Tick;

            textBox3.Text = Guid.Empty.ToString( );
        }


        private NetPushServer pushServer;

        private void button1_Click( object sender, EventArgs e )
        {
            try
            {
                // 启动服务
                pushServer = new NetPushServer( );
                pushServer.Token = new Guid( textBox3.Text );                                    // 支持令牌
                pushServer.LogNet = new HslCommunication.LogNet.LogNetSingle( "log.txt" );       // 支持日志
                pushServer.ServerStart( int.Parse( textBox2.Text ) );
                button1.Enabled = false;
                panel2.Enabled = true;
                timerOneSecond.Start( );
            }
            catch (Exception ex)
            {
                HslCommunication.BasicFramework.SoftBasic.ShowExceptionMessage( ex );
            }
        }

        private void button2_Click( object sender, EventArgs e )
        {
            // 普通的数据推送
            pushServer.PushString( textBox5.Text, textBox4.Text );
        }


        // ==============================================================================================


        private Timer timerOneSecond;


        private void TimerOneSecond_Tick( object sender, EventArgs e )
        {
            label2.Text = pushServer.OnlineCount.ToString( );
        }

        // ===========================================================================================

        private Timer timer随机数;
        private void button3_Click( object sender, EventArgs e )
        {
            try
            {
                // 定时推送随机数
                timer随机数 = new Timer( );
                timer随机数.Interval = int.Parse( textBox6.Text );
                min = int.Parse( textBox7.Text );
                max = int.Parse( textBox8.Text );

                timer随机数.Tick += Timer随机数_Tick;
                timer随机数.Start( );
                button3.Enabled = false;

            }
            catch (Exception ex)
            {
                HslCommunication.BasicFramework.SoftBasic.ShowExceptionMessage( ex );
            }
        }

        private int min = 0;
        private int max = 1000;
        private Random random = new Random( );

        private void Timer随机数_Tick( object sender, EventArgs e )
        {
            pushServer.PushString( textBox1.Text, random.Next( min, max ).ToString( ) );
        }



        // ========================================================================================================

        private Timer timer时间;

        private void button4_Click( object sender, EventArgs e )
        {
            try
            {
                // 时间推送
                timer时间 = new Timer( );
                timer时间.Interval = int.Parse( textBox11.Text );
                timer时间.Tick += Timer时间_Tick;
                timer时间.Start( );
                button4.Enabled = false;
            }
            catch (Exception ex)
            {
                HslCommunication.BasicFramework.SoftBasic.ShowExceptionMessage( ex );
            }
        }

        private void Timer时间_Tick( object sender, EventArgs e )
        {
            pushServer.PushString( textBox12.Text, DateTime.Now.ToString( ) );
        }

        // ========================================================================================================

        private Timer timerJson;

        private void button5_Click( object sender, EventArgs e )
        {
            try
            {
                timerJson = new Timer( );
                timerJson.Interval = int.Parse( textBox13.Text );
                timerJson.Tick += TimerJson_Tick;
                timerJson.Start( );
                button5.Enabled = false;
            }
            catch (Exception ex)
            {
                HslCommunication.BasicFramework.SoftBasic.ShowExceptionMessage( ex );
            }
        }

        private void TimerJson_Tick( object sender, EventArgs e )
        {
            JObject json = new JObject( );
            json.Add( "value1", new JValue( random.Next( 1000, 9999 ) ) );
            json.Add( "value2", new JValue( Math.Round( random.NextDouble( ), 6 ) * 1000 ));
            json.Add( "value3", new JValue( Guid.NewGuid( ).ToString( ) ) );
            json.Add( "value4", new JValue( DateTime.Now ) );

            pushServer.PushString( textBox14.Text, json.ToString( ) );
        }

        // ==============================================================================================

        private Timer timerXml;

        private void button6_Click( object sender, EventArgs e )
        {
            try
            {
                timerXml = new Timer( );
                timerXml.Interval = int.Parse( textBox17.Text );
                timerXml.Tick += TimerXml_Tick; ;
                timerXml.Start( );
                button6.Enabled = false;
            }
            catch (Exception ex)
            {
                HslCommunication.BasicFramework.SoftBasic.ShowExceptionMessage( ex );
            }
        }

        private void TimerXml_Tick( object sender, EventArgs e )
        {
            XElement element = new XElement( "Data" );
            element.SetElementValue( "value1", random.Next( 1000, 9999 ) );
            element.SetElementValue( "value2", (Math.Round( random.NextDouble( ), 6 ) * 1000 ).ToString() );
            element.SetElementValue( "value3", Guid.NewGuid( ).ToString( ) );
            element.SetElementValue( "value4", DateTime.Now.ToString( "O" ) );

            pushServer.PushString( textBox18.Text, element.ToString( ) );
        }

        private void button7_Click( object sender, EventArgs e )
        {
            try
            {
                OperateResult create = pushServer.CreatePushRemote( textBox9.Text, int.Parse( textBox10.Text ), textBox15.Text );

                if(create.IsSuccess)
                {
                    MessageBox.Show( "创建成功！" );
                }
                else
                {
                    MessageBox.Show( "创建失败！" + create.Message );
                }
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
                MessageBox.Show( ex.Message );
            }
        }
    }
}
