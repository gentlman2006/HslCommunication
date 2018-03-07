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
    public partial class FormLogNetTest : Form
    {
        public FormLogNetTest()
        {
            InitializeComponent();
        }

        private void FormLogNetTest_Load(object sender, EventArgs e)
        {
            // 该source可以是本地读取的文件，也可以是网络发送过来的数据
            //string source = Encoding.UTF8.GetString(System.IO.File.ReadAllBytes("123.txt"));   // 传入路径
            //logNetAnalysisControl1.SetLogNetSource(source);

            logNet.BeforeSaveToFile += LogNet_BeforeSaveToFile;
        }

        private void LogNet_BeforeSaveToFile( object sender, HslCommunication.LogNet.HslEventArgs e )
        {
            // 显示日志信息
            if (IsHandleCreated && InvokeRequired)
            {
                BeginInvoke( new Action<object, HslCommunication.LogNet.HslEventArgs>( LogNet_BeforeSaveToFile ), sender, e );
                return;
            }

            textBox1.AppendText( FormatLogInfo( e.HslMessage ) );
        }

        private string FormatLogInfo( HslCommunication.LogNet.HslMessageItem item )
        {
            return $"[{item.Degree}] {item.Time.ToString( "yyyy-MM-dd HH:mm:ss.fff" )} Thread[{item.ThreadId.ToString( "D2" )}] {item.Text}{Environment.NewLine}";
        }

        private HslCommunication.LogNet.ILogNet logNet = new HslCommunication.LogNet.LogNetSingle(Application.StartupPath + "\\Logs\\123.txt");
        

        private void userButton1_Click(object sender, EventArgs e)
        {
            // 一般日志写入
            logNet.WriteDebug("调试信息");
            logNet.WriteInfo("一般信息");
            logNet.WriteWarn("警告信息");
            logNet.WriteError("错误信息");
            logNet.WriteFatal("致命信息");
            logNet.WriteException(null, new IndexOutOfRangeException());

            // 带有关键字的写入，关键字建议为方法名或是类名，方便分析的时候归类搜索
            logNet.WriteDebug("userButton1_Click", "调试信息");
            logNet.WriteInfo("TestForm", "一般信息");
            logNet.WriteWarn("随便什么", "警告信息");
            logNet.WriteError("userButton1_Click", "错误信息");
            logNet.WriteFatal("userButton1_Click", "致命信息");
            logNet.WriteException("userButton1_Click", new IndexOutOfRangeException());
        }
    }
}
