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
    public partial class FormRegisterTest : Form
    {
        public FormRegisterTest()
        {
            InitializeComponent();
        }

        private HslCommunication.BasicFramework.SoftAuthorize softAuthorize = null;

        private void FormRegisterTest_Load(object sender, EventArgs e)
        {
            softAuthorize = new HslCommunication.BasicFramework.SoftAuthorize();
            //softAuthorize.FileSavePath = Application.StartupPath + @"\Authorize.txt"; // 设置存储激活码的文件，该存储是加密的
            //softAuthorize.LoadByFile();

            // 检测激活码是否正确，没有文件，或激活码错误都算作激活失败
            //if (!softAuthorize.IsAuthorizeSuccess(AuthorizeEncrypted))
            //{
            //    // 显示注册窗口
            //    using (HslCommunication.BasicFramework.FormAuthorize form =
            //        new HslCommunication.BasicFramework.FormAuthorize(
            //            softAuthorize,
            //            "请联系XXX获取激活码",
            //            AuthorizeEncrypted))
            //    {
            //        if (form.ShowDialog() != DialogResult.OK)
            //        {
            //            // 授权失败，退出
            //            Close();
            //        }
            //    }
            //}

            // 此处示例程序的机器码为：2E4C8EB0EBB8C4551C49AC277
            // 直接进行判断，允不允许运行
            if (!softAuthorize.CheckAuthorize("B384A9552ACFABF3CF839FB8A7CEAB123A264457BA0C176AE13F412CDD76C338", AuthorizeEncrypted))
            {
                // 检测授权失败
                Close();
            }



            textBox1.Text = softAuthorize.GetMachineCodeString();
        }


        /// <summary>
        /// 一个自定义的加密方法，传入一个原始数据，返回一个加密结果
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        private string AuthorizeEncrypted(string origin)
        {
            // 此处使用了组件支持的DES对称加密技术
            return HslCommunication.BasicFramework.SoftSecurity.MD5Encrypt(origin, "12345678");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 生成注册码
            textBox2.Text = AuthorizeEncrypted(softAuthorize.GetMachineCodeString());
            // 测试的注册码
            // B384A9552ACFABF3CF839FB8A7CEAB123A264457BA0C176AE13F412CDD76C338
        }
    }
}
