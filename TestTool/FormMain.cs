using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using HslCommunication.BasicFramework;
using System.Net.Mail;

namespace TestTool
{
    public partial class FormMain : Form
    {
        public FormMain( )
        {
            InitializeComponent( );
        }

        private void userButton1_Click( object sender, EventArgs e )
        {
            using (TestTool.TestForm.FormPlcTest form = new TestForm.FormPlcTest( ))
            {
                form.ShowDialog( );
            }
        }

        private void userButton2_Click( object sender, EventArgs e )
        {
            //textBox1.Text = HslCommunication.BasicFramework.SoftBasic.ByteToHexString( HslCommunication.Core.NetSupport.CommandBytes( 1001,
            //    new HslCommunication.NetHandle( 1, 1, 21 ), new Guid( "1275BB9A-14B2-4A96-9673-B0AF0463D474" ), null ) );
        }

        private void userButton3_Click( object sender, EventArgs e )
        {
            using (TestTool.TestForm.FormModBusTcp form = new TestForm.FormModBusTcp( ))
            {
                Hide( );
                form.ShowDialog( );
                Show( );
            }
        }

        private void userButton4_Click( object sender, EventArgs e )
        {
            textBox1.Text = DateTime.Now.Ticks.ToString( );
        }

        private void userButton5_Click( object sender, EventArgs e )
        {
            using (TestTool.TestForm.FormJsonTest form = new TestForm.FormJsonTest( ))
            {
                form.ShowDialog( );
            }
        }

        private void userButton6_Click( object sender, EventArgs e )
        {
            using (TestTool.TestForm.FormSeqTest fst = new TestForm.FormSeqTest( ))
            {
                fst.ShowDialog( );
            }
        }

        private void userButton7_Click( object sender, EventArgs e )
        {
            using (TestTool.TestForm.FormCRCTest fst = new TestForm.FormCRCTest( ))
            {
                fst.ShowDialog( );
            }
        }

        private void userButton8_Click( object sender, EventArgs e )
        {
            using (TestTool.TestForm.FormRegisterTest form = new TestForm.FormRegisterTest( ))
            {
                form.ShowDialog( );
            }
        }

        private void userButton9_Click( object sender, EventArgs e )
        {
            using (TestTool.TestForm.FormFileTest form = new TestForm.FormFileTest( ))
            {
                form.ShowDialog( );
            }
        }

        private void userButton10_Click( object sender, EventArgs e )
        {
            textBox1.Text = random.Next( 10000000 ).ToString( );
            return;
            HslCommunication.OperateResult<string> result = GetInformation( "D:\\123.txt" );
            if (result.IsSuccess)
            {
                MessageBox.Show( result.Content );
            }
            else
            {
                MessageBox.Show( "读取失败：" + result.Message );
            }
        }


        /// <summary>
        /// 输入一个文件名，输出文件名的内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private HslCommunication.OperateResult<string> GetInformation( string fileName )
        {
            HslCommunication.OperateResult<string> result = new HslCommunication.OperateResult<string>( );

            try
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader( fileName, Encoding.UTF8 ))
                {
                    result.Content = sr.ReadToEnd( );
                    result.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        Random random = new Random( );
        private void button1_Click( object sender, EventArgs e )
        {
            textBox1.Text = random.Next( 10000000 ).ToString( );

            List<string> data = new List<string>( );
            List<int> ints = new List<int>( );
            for (int i = 0; i < 8; i++)
            {
                data.Add( random.Next( 100, 999 ).ToString( ) );
                ints.Add( random.Next( 0, 5 ) );
            }

        }

        private void userButton11_Click( object sender, EventArgs e )
        {
            userButton10.Enabled = false;
            button1.Enabled = false;
        }

        private void userButton12_Click( object sender, EventArgs e )
        {
            if (int.TryParse( textBox2.Text, out int value ))
            {
            }
        }

        private void userButton13_Click( object sender, EventArgs e )
        {
            // 日志查看器
            //using (HslCommunication.LogNet.FormLogNetView form = new HslCommunication.LogNet.FormLogNetView())
            //{
            //    form.ShowDialog();
            //}
            TestForm.FormLogNetTest form = new TestForm.FormLogNetTest( );
            form.ShowDialog( );
        }








        private void userButton14_Click( object sender, EventArgs e )
        {
            string json = JObject.FromObject( new AA( )
            {
                asdasdasdasd = "123123123",
                adasdasd = 1234,
            } ).ToString( );


            AA aa = GetT<AA>( json );
            ;
        }

        private T GetT<T>( string json )
        {
            JObject jObject = JObject.Parse( json );
            string dataType = jObject["DataType"].ToObject<string>( );

            Type type = Type.GetType( "TestTool." + dataType );
            object obj = jObject.ToObject( type );
            return (T)obj;
        }

        private void userButton15_Click( object sender, EventArgs e )
        {
            TestForm.FormControls form = new TestForm.FormControls( );
            form.ShowDialog( );
            form.Dispose( );
        }

        private void userButton16_Click( object sender, EventArgs e )
        {
            TestForm.FormControlCollection form = new TestForm.FormControlCollection( );
            form.ShowDialog( );
            form.Dispose( );
        }


        private float[] GetV( int count )
        {
            float[] values = new float[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = random.Next( 101 );
            }
            return values;
        }

        private void userButton17_Click( object sender, EventArgs e )
        {
            MessageBox.Show( HslCommunication.BasicFramework.SoftBasic.ByteToHexString( BitConverter.GetBytes( 12.34f ) ) );
            return;

            MessageBox.Show( "第一：" + (-1).ToString( "X4" ) + Environment.NewLine +
                "第二：" + (14).ToString( "X4" ) );

            userCurve1.SetLeftCurve( "123", GetV( 100 ), Color.DodgerBlue );
        }

        private void userButton18_Click( object sender, EventArgs e )
        {
            TestForm.FormCurve form = new TestForm.FormCurve( );
            form.ShowDialog( );
            form.Dispose( );
        }

        private void userButton19_Click( object sender, EventArgs e )
        {
            TestForm.FormSimenceModbusTcp form = new TestForm.FormSimenceModbusTcp( );
            form.ShowDialog( );
            form.Dispose( );
        }

        private void userButton20_Click( object sender, EventArgs e )
        {
            // 邮件发送
            // SoftMail.MailSystem163.SendMail( "hsl200909@163.com", "重要信息", "这是一条重要的文本" );
            SoftMail MailSystem163 = new SoftMail(
            mail =>
            {
                mail.Host = "mail.jinkosolar.com";//使用163的SMTP服务器发送邮件
                mail.UseDefaultCredentials = true;
                mail.EnableSsl = true;
                mail.Port = 465;
                mail.DeliveryMethod = SmtpDeliveryMethod.Network;
                mail.Credentials = new System.Net.NetworkCredential( "mes_notice", "TPYdga9=" );//密码zxcvbnm1234
            },
            "mes_notice@jinkosolar.com",
            "hsl200909@163.com"
            );

            bool send = MailSystem163.SendMail( "测试", "随便一条数据" );
            if (send) MessageBox.Show( "发送成功！" );
            else MessageBox.Show( "发送失败！" );
        }

        private void userButton21_Click( object sender, EventArgs e )
        {
            using (TestForm.FormMultiConnect form = new TestForm.FormMultiConnect( ))
            {
                form.ShowDialog( );
            }
        }
    }

    public class AA
    {
        public AA()
        {
            DataType = "AA";
        }


        public int adasdasd { get; set; }

        public string asdasdasdasd { get; set; }

        public string DataType { get; set; }
    }
}
