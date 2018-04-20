using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestTool.TestForm
{
    public partial class FormCRCTest : Form
    {
        public FormCRCTest( )
        {
            InitializeComponent( );
        }

        private void userButton1_Click( object sender, EventArgs e )
        {
            byte[] data = new byte[] { 0xFF, 0xA0 };
            byte[] result = HslCommunication.Serial.SoftCRC16.CRC16( data );
            textBox1.AppendText( HslCommunication.BasicFramework.SoftBasic.ByteToHexString( result ) + Environment.NewLine );
        }

        private void userButton2_Click( object sender, EventArgs e )
        {
            byte[] data = new byte[] { 0xFF, 0xA0, 0x40, 0x38 };
            bool result = HslCommunication.Serial.SoftCRC16.CheckCRC16( data );
            textBox1.AppendText( result.ToString( ) + Environment.NewLine );
        }

        private void userButton3_Click( object sender, EventArgs e )
        {
            byte[] data = new byte[] { 0xFF, 0xA0 };
            byte[] result = HslCommunication.Serial.SoftCRC16.CRC16( data, 0x80, 0x05 );
            textBox1.AppendText( HslCommunication.BasicFramework.SoftBasic.ByteToHexString( result ) + Environment.NewLine );
        }

        private void userButton4_Click( object sender, EventArgs e )
        {
            byte[] data = new byte[] { 0xFF, 0xA0, 0x06, 0xED };
            bool result = HslCommunication.Serial.SoftCRC16.CheckCRC16( data, 0x80, 0x05 );
            textBox1.AppendText( result.ToString( ) + Environment.NewLine );
        }

        private void userButton5_Click( object sender, EventArgs e )
        {
            byte[] data = new byte[] { 0x01, 0x06, 0x00, 0x0C, 0x01, 0xDD, 0x88 };
            bool result = HslCommunication.Serial.SoftCRC16.CheckCRC16( data );
            if (result)
            {
                // 计数器清零成功
            }
            else
            {
                // 计数器清零失败
            }


            data = new byte[] { 0x01, 0x03, 0x00, 0x01, 0x30, 0x18 };
            bool result2 = HslCommunication.Serial.SoftCRC16.CheckCRC16( data );
            ;
        }

        private void userButton6_Click( object sender, EventArgs e )
        {
            byte[] data = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x64, 0x44, 0x21 };
            bool result = HslCommunication.Serial.SoftCRC16.CheckCRC16( data );
            textBox1.AppendText( result.ToString( ) + Environment.NewLine );
        }
    }
}
