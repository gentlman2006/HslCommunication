using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HslCommunication.LogNet
{
    /// <summary>
    /// 日志查看器的窗口类，用于分析统计日志数据
    /// </summary>
    public partial class FormLogNetView : Form
    {
        /// <summary>
        /// 实例化一个日志查看器的窗口
        /// </summary>
        public FormLogNetView()
        {
            InitializeComponent();
        }

        private void FormLogNetView_Load(object sender, EventArgs e)
        {

        }

        private void userButton1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fDialog = new OpenFileDialog())
            {
                fDialog.Filter = "日志文件(*.txt)|*.txt";
                if (fDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = fDialog.FileName;
                    DealWithFileName(fDialog.FileName);
                }
            }

        }

        private void DealWithFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;
            if (!System.IO.File.Exists(fileName))
            {
                MessageBox.Show("文件不存在！");
                return;
            }
            try
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(fileName, Encoding.UTF8))
                {
                    try
                    {
                        logNetAnalysisControl1.SetLogNetSource(sr.ReadToEnd());
                    }
                    catch (Exception ex)
                    {
                        BasicFramework.SoftBasic.ShowExceptionMessage(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                BasicFramework.SoftBasic.ShowExceptionMessage(ex);
            }
        }


        

        private void logNetAnalysisControl1_Load(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", "https://github.com/dathlin/C-S-");
            }
            catch
            {

            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
