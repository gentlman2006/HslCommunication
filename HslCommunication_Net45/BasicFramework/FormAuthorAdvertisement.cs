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
    /// 显示作者一些信息的类，应当在软件系统中提供一处链接显示原作者信息，或者以彩蛋方式实现
    /// </summary>
    public partial class FormAuthorAdvertisement : Form
    {
        /// <summary>
        /// 实例化一个窗口，显示作者的相关信息
        /// </summary>
        public FormAuthorAdvertisement()
        {
            InitializeComponent();
        }

        private void FormAuthorAdvertisement_Load(object sender, EventArgs e)
        {
            label4.Text = "本框架基础主要提供了应用程序可能会包含了常用的类，账户类，授权类，消息类，版本类，" +
                "日志类，还有常用的窗口，包含了C#大量的委托的使用，减去新建系统开发重复的基础工作，提升效率。" +
                "本框架也包含了一套完整的网络通信解决方案，具有非常高的灵活性来为您的系统提供稳定的信息传递，" +
                "而且用于通信的数据全部经过DES加密，公钥私钥仅内部可见，保证了安全性。";
            label7.Text = "左边为我个人的公众号，本人提供软件定制服务，主要包含中小型的软件系统和个人使用的文件处理工具，"+
                "具体案例可以参照公众号展示说明。";
            toolStripStatusLabel4.Text = SoftBasic.FrameworkVersion.ToString();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "http://www.cnblogs.com/dathlin/");
        }
    }
}
