using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication.BasicFramework;

namespace HslCommunicationDemo
{
    public partial class FormMail : Form
    {
        public FormMail( )
        {
            InitializeComponent( );
        }

        private void FormMail_Load( object sender, EventArgs e )
        {
            textBox3.Text = "<html><body style=\"background-color:PowderBlue;\"><h1>Look! Styles and colors</h1><p style=\"font-family:verdana;color:red\">This text is in Verdana and red</p><p style=\"font-family:times;color:green\">This text is in Times and green</p><p style=\"font-size:30px\">This text is 30 pixels high</p></body></html> ";
        }

        private void button3_Click( object sender, EventArgs e )
        {
            try
            {
                SoftMail.MailSystem163.SendMail( textBox5.Text, textBox1.Text, textBox4.Text );
                MessageBox.Show( "发送成功！" );
            }
            catch (Exception ex)
            {
                SoftBasic.ShowExceptionMessage( ex );
            }
        }

        private void button1_Click( object sender, EventArgs e )
        {
            try
            {
                SoftMail.MailSystem163.SendMail( textBox6.Text, textBox2.Text, textBox3.Text, true );
                MessageBox.Show( "发送成功！" );
            }
            catch (Exception ex)
            {
                SoftBasic.ShowExceptionMessage( ex );
            }
        }
    }
}
