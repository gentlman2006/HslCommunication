using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestTool
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void userButton1_Click(object sender, EventArgs e)
        {
            using (TestTool.TestForm.FormPlcTest form = new TestForm.FormPlcTest())
            {
                form.ShowDialog();
            }
        }

        private void userButton2_Click(object sender, EventArgs e)
        {
            textBox1.Text = HslCommunication.BasicFramework.SoftBasic.ByteToHexString(HslCommunication.NetSupport.CommandBytes(1001,
                new HslCommunication.NetHandle(1, 1, 21), new Guid("1275BB9A-14B2-4A96-9673-B0AF0463D474"), null));
        }

        private void userButton3_Click(object sender, EventArgs e)
        {
            using (TestTool.TestForm.FormModBusTcp form = new TestForm.FormModBusTcp())
            {
                form.ShowDialog();
            }
        }
    }
}
