using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication.Profinet.Siemens;

namespace HslCommunicationDemo
{
    public partial class FormLoad : Form
    {
        public FormLoad( )
        {
            InitializeComponent( );
        }

        private void button1_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormSiemens form = new FormSiemens( SiemensPLCS.S1200 ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }


        private void button6_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormModbus form = new FormModbus())
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button4_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormMelsecBinary form = new FormMelsecBinary( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button2_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormSiemens form = new FormSiemens( SiemensPLCS.S1500 ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button3_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormSiemens form = new FormSiemens( SiemensPLCS.S300 ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button5_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormSiemens form = new FormSiemens( SiemensPLCS.S200Smart ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
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

        private void linkLabel3_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            try
            {
                System.Diagnostics.Process.Start( linkLabel3.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button7_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormMelsecAscii form = new FormMelsecAscii( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void linkLabel2_Click( object sender, EventArgs e )
        {
            HslCommunication.BasicFramework.FormSupport form = new HslCommunication.BasicFramework.FormSupport( );
            form.ShowDialog( );
        }

        private void button8_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormSimplifyNet form = new FormSimplifyNet( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button9_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormUdpNet form = new FormUdpNet( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button10_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormSiemensFW form = new FormSiemensFW( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button11_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormOmron form = new FormOmron( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button12_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormFileClient form = new FormFileClient( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button14_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormComplexNet form = new FormComplexNet( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button13_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormMelsec1EBinary form = new FormMelsec1EBinary( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button15_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormLogNet form = new FormLogNet( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button16_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormBasicControl form = new FormBasicControl( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button17_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormGauge form = new FormGauge( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button18_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormCurve form = new FormCurve( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button19_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormModbusAlien form = new FormModbusAlien( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void FormLoad_Load( object sender, EventArgs e )
        {
            label3.Text = "库版本：" + HslCommunication.BasicFramework.SoftBasic.FrameworkVersion.ToString( );
        }

        private void button20_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormPieChart form = new FormPieChart( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button21_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormModbusRtu form = new FormModbusRtu( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button22_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormPushNet form = new FormPushNet( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button23_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (Robot.FormEfort form = new Robot.FormEfort( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button24_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormSerialDebug form = new FormSerialDebug( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button25_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (Algorithms.FourierTransform form = new Algorithms.FourierTransform( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button26_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormSeqCreate form = new FormSeqCreate( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button27_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormRegister form = new FormRegister( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button28_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormMail form = new FormMail( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }

        private void button29_Click( object sender, EventArgs e )
        {
            Hide( );
            System.Threading.Thread.Sleep( 200 );
            using (FormSimplifyNetAlien form = new FormSimplifyNetAlien( ))
            {
                form.ShowDialog( );
            }
            System.Threading.Thread.Sleep( 200 );
            Show( );
        }
    }
}
