using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using HslCommunication.Profinet.Siemens;

namespace TestTool.TestForm
{
    public partial class FormMultiConnect : Form
    {
        public FormMultiConnect( )
        {
            InitializeComponent( );
        }

        private void button1_Click( object sender, EventArgs e )
        {
            maxConnection = int.Parse( textBox1.Text );
            for (int i = 0; i < maxConnection; i++)
            {
                new Thread( new ThreadStart( TheadConnection ) ) { IsBackground = true }.Start( );
            }
        }

        private void TheadConnection()
        {
            SiemensS7Net siemensS7 = new SiemensS7Net( SiemensPLCS.S1200 );
            siemensS7.ConnectTimeOut = 10000;

            siemensS7.ConnectServer( );
            siemensS7.ConnectClose( );
        }

        private int maxConnection = 0;
    }
}
