using HslCommunication;
using HslCommunication.Enthernet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HslCommunicationDemo
{
    public partial class FormFileClient : Form
    {
        public FormFileClient( )
        {
            InitializeComponent( );
        }

        private void FormFileClient_Load( object sender, EventArgs e )
        {
            textBox15.Text = Guid.Empty.ToString( );
        }



        #region Intergration File Client


        private IntegrationFileClient integrationFileClient;                 // 客户端的核心引擎

        private void IntegrationFileClientInitialization( )
        {

            if(!System.Net.IPAddress.TryParse(textBox1.Text,out System.Net.IPAddress ipAddress))
            {
                MessageBox.Show( "Ip输入异常，请重新输入！" );
                return;
            }


            if(!int.TryParse(textBox2.Text,out int port))
            {
                MessageBox.Show( "Ip输入异常，请重新输入！" );
                return;
            }


            // 定义连接服务器的一些属性，超时时间，IP及端口信息
            integrationFileClient = new IntegrationFileClient( )
            {
                ConnectTimeOut = 5000,
                ServerIpEndPoint = new System.Net.IPEndPoint( ipAddress, port ),
                Token = new Guid( textBox15.Text ),                                         // 指定一个令牌
                LogNet = new HslCommunication.LogNet.LogNetSingle( Application.StartupPath + "\\Logs\\log.txt" ),
            };

            // 创建本地文件存储的路径
            string path = Application.StartupPath + @"\Files";
            if (!System.IO.Directory.Exists( path ))
            {
                System.IO.Directory.CreateDirectory( path );
            }

            button1.Enabled = false;
            panel2.Enabled = true;
        }

        private void button1_Click( object sender, EventArgs e )
        {
            IntegrationFileClientInitialization( );
        }


        #endregion

        #region Upload File


        /*************************************************************************************************
         * 
         *   一条指令即可完成文件的上传操作，上传模式有三种
         *   1. 指定本地的完整路径的文件名
         *   2. 将流（stream）中的数据上传到服务器
         *   3. 将bitmap图片数据上传到服务器
         * 
         ********************************************************************************************/

        private void button2_Click( object sender, EventArgs e )
        {
            // 选择文件
            using (OpenFileDialog ofd = new OpenFileDialog( ))
            {
                if (ofd.ShowDialog( ) == DialogResult.OK)
                {
                    textBox3.Text = ofd.FileName;
                }
            }
        }

        private void button3_Click( object sender, EventArgs e )
        {
            // 开始上传
            if (!string.IsNullOrEmpty( textBox3.Text ))
            {
                if(!System.IO.File.Exists( textBox3.Text ))
                {
                    MessageBox.Show( "选择的文件不存在，退出！" );
                    return;
                }

                // 点击开始上传，此处按照实际项目需求放到了后台线程处理，事实上这种耗时的操作就应该放到后台线程
                System.Threading.Thread thread = new System.Threading.Thread( new System.Threading.ParameterizedThreadStart( (ThreadUploadFile) ) );
                thread.IsBackground = true;
                thread.Start( textBox3.Text );
                button3.Enabled = false;
                progressBar1.Value = 0;
            }
        }

        private void ThreadUploadFile( object filename )
        {
            if (filename is string fileName)
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo( fileName );
                // 开始正式上传，关于三级分类，下面只是举个例子，上传成功后去服务器端寻找文件就能明白
                OperateResult result = integrationFileClient.UploadFile(
                    fileName,                   // 需要上传的原文件的完整路径，上传成功还需要个条件，该文件不能被占用
                    fileInfo.Name,              // 在服务器存储的文件名，带后缀，一般设置为原文件的文件名，当然您也可以重新设置名字
                    "Files",                    // 第一级分类，指示文件存储的类别，对应在服务器端存储的路径不一致
                    "Personal",                 // 第二级分类，指示文件存储的类别，对应在服务器端存储的路径不一致
                    "Admin",                    // 第三级分类，指示文件存储的类别，对应在服务器端存储的路径不一致
                    "这个文件非常重要",        // 这个文件的额外描述文本，可以为空（""）
                    "张三",                     // 文件的上传人，当然你也可以不使用，可以为空（""）
                    UpdateReportProgress         // 文件上传时的进度报告，如果你不需要，指定为NULL就行，一般文件比较大，带宽比较小，都需要进度提示
                    );

                // 切换到UI前台显示结果
                Invoke( new Action<OperateResult>( operateResult =>
                {
                    button3.Enabled = true;
                    if (result.IsSuccess)
                    {
                        MessageBox.Show( "文件上传成功！" );
                    }
                    else
                    {
                        // 失败原因多半来自网络异常，还有文件不存在，分类名称填写异常
                        MessageBox.Show( "文件上传失败：" + result.ToMessageShowString( ) );
                    }
                } ), result );
            }
        }

        /// <summary>
        /// 用于更新上传进度的方法，该方法是线程安全的
        /// </summary>
        /// <param name="sended">已经上传的字节数</param>
        /// <param name="totle">总字节数</param>
        private void UpdateReportProgress( long sended, long totle )
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke( new Action<long, long>( UpdateReportProgress ), sended, totle );
                return;
            }


            // 此处代码是安全的
            int value = (int)(sended * 100L / totle);
            progressBar1.Value = value;
        }



        #endregion

        #region Download File


        /*************************************************************************************************
         * 
         *   一条指令即可完成文件的下载操作，下载模式有三种
         *   1. 指定需要下载的文件名（带后缀）
         *   2. 将服务器上的数据下载到流（stream）中
         *   3. 将服务器上的数据下载到bitmap图片中
         * 
         ********************************************************************************************/

        private void button4_Click( object sender, EventArgs e )
        {
            // 点击开始下载，此处按照实际项目需求放到了后台线程处理，事实上这种耗时的操作就应该放到后台线程
            System.Threading.Thread thread = new System.Threading.Thread( new System.Threading.ParameterizedThreadStart( (ThreadDownloadFile) ) );
            thread.IsBackground = true;
            thread.Start( textBox_download_fileName.Text );
            progressBar2.Value = 0;
        }


        private void ThreadDownloadFile( object filename )
        {
            if (filename is string fileName)
            {
                OperateResult result = integrationFileClient.DownloadFile(
                    fileName,                   // 文件在服务器上保存的名称，举例123.txt
                    "Files",                    // 第一级分类，指示文件存储的类别，对应在服务器端存储的路径不一致
                    "Personal",                 // 第二级分类，指示文件存储的类别，对应在服务器端存储的路径不一致
                    "Admin",                    // 第三级分类，指示文件存储的类别，对应在服务器端存储的路径不一致
                    DownloadReportProgress,     // 文件下载的时候的进度报告，友好的提示下载进度信息
                    Application.StartupPath + @"\Files\" + filename // 下载后在文本保存的路径，也可以直接下载到 MemoryStream 的数据流中，或是bitmap中，或是手动选择存储路径
                    );

                // 切换到UI前台显示结果
                Invoke( new Action<OperateResult>( operateResult =>
                {
                    if (result.IsSuccess)
                    {
                        MessageBox.Show( "文件下载成功！" );
                    }
                    else
                    {
                        // 失败原因多半来自网络异常，还有文件不存在，分类名称填写异常
                        MessageBox.Show( "文件下载失败：" + result.ToMessageShowString( ) );
                    }
                } ), result );
            }
        }

        /// <summary>
        /// 用于更新文件下载进度的方法，该方法是线程安全的
        /// </summary>
        /// <param name="receive">已经接收的字节数</param>
        /// <param name="totle">总字节数</param>
        private void DownloadReportProgress( long receive, long totle )
        {
            if (progressBar2.InvokeRequired)
            {
                progressBar2.Invoke( new Action<long, long>( DownloadReportProgress ), receive, totle );
                return;
            }


            // 此处代码是线程安全的
            int value = (int)(receive * 100L / totle);
            progressBar2.Value = value;
        }


        #endregion

        #region Delete Files

        private void button5_Click( object sender, EventArgs e )
        {
            // 文件的删除不需要放在后台线程，前台即可处理，无论多少大的文件，无论该文件是否在下载中，都是很快删除的
            OperateResult result = integrationFileClient.DeleteFile( "123.txt", "Files", "Personal", "Admin" );
            if (result.IsSuccess)
            {
                MessageBox.Show( "文件删除成功！" );
            }
            else
            {
                // 删除失败的原因除了一般的网络问题，还有因为服务器的文件不存在，会在Message里有显示。
                MessageBox.Show( "文件删除失败，原因：" + result.ToMessageShowString( ) );
            }
        }


        #endregion




    }
}
