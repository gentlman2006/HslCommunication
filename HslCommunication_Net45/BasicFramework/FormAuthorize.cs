using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HslCommunication.BasicFramework
{
    /// <summary>
    /// 用来测试版软件授权的窗口
    /// </summary>
    public partial class FormAuthorize : Form
    {
        /// <summary>
        /// 实例化授权注册窗口
        /// </summary>
        /// <param name="authorize"></param>
        /// <param name="aboutCode">提示关于怎么获取注册码的信息</param>
        /// <param name="encrypt">加密的方法</param>
        public FormAuthorize(SoftAuthorize authorize,string aboutCode,Func<string,string> encrypt)
        {
            InitializeComponent();
            softAuthorize = authorize;
            AboutCode = aboutCode;
            Encrypt = encrypt;
        }

        private SoftAuthorize softAuthorize = null;

        private void FormAuthorize_Load(object sender, EventArgs e)
        {
            textBox1.Text = softAuthorize.GetMachineCodeString();
        }

        private void userButton1_Click(object sender, EventArgs e)
        {
            if (softAuthorize.CheckAuthorize(textBox2.Text, Encrypt))
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("注册码不正确");
            }
        }

        Func<string, string> Encrypt = null;

        private string AboutCode { get; set; } = "";


        private void linkLabel1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(AboutCode);
        }
    }
}
