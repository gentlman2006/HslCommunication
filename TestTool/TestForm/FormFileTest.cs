using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication.Enthernet;
using HslCommunication;

namespace TestTool.TestForm
{
    public partial class FormFileTest : Form
    {
        public FormFileTest()
        {
            InitializeComponent();
        }

        private void FormFileTest_Load(object sender, EventArgs e)
        {

        }


        #region 服务器端代码


        private UltimateFileServer ultimateFileServer;                                            // 引擎对象

        private void UltimateFileServerInitialization()
        {
            ultimateFileServer = new UltimateFileServer();                                        // 实例化对象
            ultimateFileServer.FilesDirectoryPath = Application.StartupPath + @"\UltimateFile";   // 所有文件存储的基础路径
            ultimateFileServer.ServerStart(34567);                                                // 启动一个端口的引擎
        }




        #endregion



        #region 客户端核心引擎

        private IntegrationFileClient integrationFileClient;



        #endregion

        #region 上传文件块

        private void userButton3_Click(object sender, EventArgs e)
        {
            // 点击后进行文件选择
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = ofd.FileName;
                }
            }

        }


        private void userButton2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                // 点击开始上传，此处按照实际项目需求放到了后台线程处理，事实上这种耗时的操作就应该放到后台线程
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart((ThreadUploadFile));
                thread.IsBackground = true;
                thread.Start(textBox1.Text);
                userButton2.Enabled = false;
            }
        }

        private void ThreadUploadFile(object filename)
        {
            if(filename is string fileName)
            {
                // 开始正式上传

            }
        }




        #endregion

    }
}
