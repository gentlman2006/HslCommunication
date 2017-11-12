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
        public FormCRCTest()
        {
            InitializeComponent();
        }

        private void userButton1_Click(object sender, EventArgs e)
        {
            byte[] data = new byte[] { 0xFF, 0xA0 };
            byte[] result = HslCommunication.Serial.SoftCRC16.CRC16(data);
            textBox1.AppendText(HslCommunication.BasicFramework.SoftBasic.ByteToHexString(result) + Environment.NewLine);
        }

        private void userButton2_Click(object sender, EventArgs e)
        {
            byte[] data = new byte[] { 0xFF, 0xA0, 0x40, 0x38 };
            bool result = HslCommunication.Serial.SoftCRC16.CheckCRC16(data);
            textBox1.AppendText(result.ToString() + Environment.NewLine);
        }

        private void userButton3_Click(object sender, EventArgs e)
        {
            byte[] data = new byte[] { 0xFF, 0xA0 };
            byte[] result = HslCommunication.Serial.SoftCRC16.CRC16(data, 0x80, 0x05);
            textBox1.AppendText(HslCommunication.BasicFramework.SoftBasic.ByteToHexString(result) + Environment.NewLine);
        }

        private void userButton4_Click(object sender, EventArgs e)
        {
            byte[] data = new byte[] { 0xFF, 0xA0, 0x06, 0xED };
            bool result = HslCommunication.Serial.SoftCRC16.CheckCRC16(data, 0x80, 0x05);
            textBox1.AppendText(result.ToString() + Environment.NewLine);
        }
    }
}
