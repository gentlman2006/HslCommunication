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
    public partial class FormThreadTest : Form
    {
        public FormThreadTest()
        {
            InitializeComponent();
        }

        private void userButton1_Click(object sender, EventArgs e)
        {
            string[] files = System.IO.Directory.GetFiles("D:\\files", "*.txt");
            HslCommunication.Core.SoftMultiTask<string> softMultiTask = new HslCommunication.Core.SoftMultiTask<string>(files, DealWithFile, 10);
            softMultiTask.OnExceptionOccur += SoftMultiTask_OnExceptionOccur; // 如果DealWithFile捕获了异常，就注释掉本行代码
            softMultiTask.OnReportProgress += SoftMultiTask_OnReportProgress;
        }

        private void SoftMultiTask_OnReportProgress(int finish, int count, int success, int failed)
        {
            // 报告进度，这个方法是在后台的，需要委托显示
            if(InvokeRequired)
            {
                Invoke(new Action<int, int, int, int>(SoftMultiTask_OnReportProgress), finish, count, success, failed);
                return;
            }

            // 每个任务有3种状态，返回成功，返回失败，发生异常
            // finish 是完成个数，包含成功和失败和异常
            // count 是总的任务个数
            // success 是执行方法返回成功的个数
            // failed 是发生异常个数

            
        }

        private void SoftMultiTask_OnExceptionOccur(string item, Exception ex)
        {
            // 方法DealWithFile(string fileName)出现异常，并且没有被捕获才会执行这里的代码
        }

        /// <summary>
        /// 不需要添加异常捕获，添加了的话，softMultiTask就不需要触发异常处理了
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool DealWithFile(string fileName)
        {
            // 此处模拟了一个相对耗时的任务，比如上传文件到云端，解析文件的数据，从云端下载文件之类的
            System.Threading.Thread.Sleep(100);
            return true;
        }
    }
}
