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
            textBox1.Text = HslCommunication.BasicFramework.SoftBasic.ByteToHexString(HslCommunication.Core.NetSupport.CommandBytes(1001,
                new HslCommunication.NetHandle(1, 1, 21), new Guid("1275BB9A-14B2-4A96-9673-B0AF0463D474"), null));
        }

        private void userButton3_Click(object sender, EventArgs e)
        {
            using (TestTool.TestForm.FormModBusTcp form = new TestForm.FormModBusTcp())
            {
                Hide();
                form.ShowDialog();
                Show();
            }
        }

        private void userButton4_Click(object sender, EventArgs e)
        {
            textBox1.Text = DateTime.Now.Ticks.ToString() ;
        }

        private void userButton5_Click(object sender, EventArgs e)
        {
            using (TestTool.TestForm.FormJsonTest form = new TestForm.FormJsonTest())
            {
                form.ShowDialog();
            }
        }

        private void userButton6_Click(object sender, EventArgs e)
        {
            using (TestTool.TestForm.FormSeqTest fst = new TestForm.FormSeqTest())
            {
                fst.ShowDialog();
            }
        }

        private void userButton7_Click(object sender, EventArgs e)
        {
            using (TestTool.TestForm.FormCRCTest fst = new TestForm.FormCRCTest())
            {
                fst.ShowDialog();
            }
        }

        private void userButton8_Click(object sender, EventArgs e)
        {
            using (TestTool.TestForm.FormRegisterTest form = new TestForm.FormRegisterTest())
            {
                form.ShowDialog();
            }
        }

        private void userButton9_Click(object sender, EventArgs e)
        {
            using (TestTool.TestForm.FormFileTest form = new TestForm.FormFileTest())
            {
                form.ShowDialog();
            }
        }

        private void userButton10_Click(object sender, EventArgs e)
        {
            MessageBox.Show("1234"); 
            return;
            HslCommunication.OperateResult<string> result = GetInformation("D:\\123.txt");
            if(result.IsSuccess)
            {
                MessageBox.Show(result.Content);
            }
            else
            {
                MessageBox.Show("读取失败：" + result.Message);
            }
        }


        /// <summary>
        /// 输入一个文件名，输出文件名的内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private HslCommunication.OperateResult<string> GetInformation(string fileName)
        {
            HslCommunication.OperateResult<string> result = new HslCommunication.OperateResult<string>();

            try
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(fileName, Encoding.UTF8))
                {
                    result.Content = sr.ReadToEnd();
                    result.IsSuccess = true;
                }
            }
            catch(Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("1234");
        }

        private void userButton11_Click(object sender, EventArgs e)
        {
            userButton10.Enabled = false;
            button1.Enabled = false;
        }

        private void userButton12_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox2.Text, out int value))
            {
                userVerticalProgress1.Value = value;
            }
        }
    }
}
