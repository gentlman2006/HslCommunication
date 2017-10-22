using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace 软件自动更新
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Text = "更新软件---" + Class1.CurrentSystem.SoftNameHead + Class1.CurrentSystem.SoftName;
        }

        private bool CanWindowClose { get; set; } = true;
        private bool IsSystemDownLoad { get; set; } = false;

        private string FilePath = @"C:\" + Class1.CurrentSystem.SoftNameHead + Class1.CurrentSystem.SoftName;
        public IPEndPoint ServerIpEndPoint { get; set; } =
            new IPEndPoint(IPAddress.Parse(Class1.CurrentSystem.ServerIp), Class1.CurrentSystem.ServerPort);





        private void Form2_Shown(object sender, EventArgs e)
        {
            string NowPath = Application.StartupPath;

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (NowPath != desktop)
            {
                if (File.Exists(NowPath + @"\" + Class1.CurrentSystem.SoftName + ".exe"))
                {
                    FilePath = NowPath;
                    textBox1.Text = FilePath;
                    textBox1.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectCallback), null);
                }
                else
                {
                    IsSystemDownLoad = true;
                    textBox1.Text = FilePath;
                }
            }
            else
            {
                IsSystemDownLoad = true;
                textBox1.Text = FilePath;
            }
        }

        private void ConnectCallback(object obj)
        {
            Thread.Sleep(400);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(ServerIpEndPoint);
            }
            catch
            {
                Invoke(new Action(() =>
                {
                    MessageBox.Show("连接服务器失败，稍后重试。");
                }));
                return;
            }

            try
            {
                //控制版本号的文件最后一个写入，来确保所有文件更新成功
                string ExecuteFileName = string.Empty;
                byte[] ExecuteFileContent = null;
                //先请求令牌，为了兼容原来旧的服务，令牌舍弃
                //socket.Send(Class1.CurrentSystem.KeyToken.ToByteArray());

                int Command = 0x1002;

                if(IsSystemDownLoad)
                {
                    Command = 0x1001;
                }

                socket.Send(BitConverter.GetBytes(Command));


                CanWindowClose = false;
                int _ReceiveLenght = 4;
                int ReceivedLenght = 0;
                byte[] ReceiveByte = new byte[_ReceiveLenght];
                while (ReceivedLenght < _ReceiveLenght)
                {
                    int temp = socket.Receive(ReceiveByte, ReceivedLenght, _ReceiveLenght - ReceivedLenght, SocketFlags.None);
                    ReceivedLenght += temp;
                }

                //接收的文件个数
                int FileCount = BitConverter.ToInt32(ReceiveByte, 0);

                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }

                Invoke(new Action(() =>
                {
                    label6.Text = "文件数:" + FileCount;
                }));

                int ReceivedFiles = 0;

                for (int i = 0; i < FileCount; i++)
                {
                    _ReceiveLenght = 8;
                    ReceivedLenght = 0;
                    ReceiveByte = new byte[_ReceiveLenght];

                    //分三次接收数据
                    while (ReceivedLenght < _ReceiveLenght)
                    {
                        int temp = socket.Receive(ReceiveByte, ReceivedLenght, _ReceiveLenght - ReceivedLenght, SocketFlags.None);
                        ReceivedLenght += temp;
                    }
                    int FileNameLenght = BitConverter.ToInt32(ReceiveByte, 0) - 8;
                    int FileLenght = BitConverter.ToInt32(ReceiveByte, 4);

                    _ReceiveLenght = FileNameLenght;
                    ReceivedLenght = 0;
                    ReceiveByte = new byte[_ReceiveLenght];


                    while (ReceivedLenght < _ReceiveLenght)
                    {
                        int temp = socket.Receive(ReceiveByte, ReceivedLenght, _ReceiveLenght - ReceivedLenght, SocketFlags.None);
                        ReceivedLenght += temp;
                    }
                    string fileName = Encoding.Unicode.GetString(ReceiveByte);

                    Invoke(new Action(() =>
                    {
                        label3.Text = "文件名:" + StringShort(fileName);
                        label4.Text = "大小:" + SizeToString(FileLenght);
                        progressBar1.Value = 100 * (ReceivedFiles + 1) / FileCount;
                        progressBar2.Value = 0;
                        progressBar2.Maximum = FileLenght;
                        label7.Text = "当前:" + (ReceivedFiles + 1);
                    }));

                    Thread.Sleep(100);
                    _ReceiveLenght = FileLenght;
                    ReceivedLenght = 0;
                    ReceiveByte = new byte[_ReceiveLenght];


                    //文件选择分割成4K数据进行接收
                    while (ReceivedLenght < _ReceiveLenght)
                    {
                        int 本次接收 = (_ReceiveLenght - ReceivedLenght) > 4096 ? 4096 : _ReceiveLenght - ReceivedLenght;
                        int temp = socket.Receive(ReceiveByte, ReceivedLenght, 本次接收, SocketFlags.None);
                        ReceivedLenght += temp;

                        Invoke(new Action(() =>
                        {
                            progressBar2.Value = ReceivedLenght;
                        }));
                    }
                    //数据保存

                    string FullName = FilePath + @"\" + fileName;

                    if (FullName.Contains(Class1.CurrentSystem.SoftName + ".exe") && string.IsNullOrEmpty(ExecuteFileName))
                    {
                        ExecuteFileName = FullName;
                        ExecuteFileContent = ReceiveByte;
                    }
                    else
                    {
                        try
                        {
                            File.WriteAllBytes(FullName, ReceiveByte);
                        }
                        catch
                        {
                            //事实上在此处如果出现异常，将会导致应用程序出现无法预知的失败
                            throw;
                        }
                    }

                    Thread.Sleep(100);

                    ReceivedFiles++;
                }


                if (!string.IsNullOrEmpty(ExecuteFileName) && ExecuteFileContent != null)
                {
                    try
                    {
                        File.WriteAllBytes(ExecuteFileName, ExecuteFileContent);
                    }
                    catch
                    {
                        //此处异常没有影响，重新运行程序时，将重新更新
                        throw new Exception("更新软件时出现了异常，请稍后重试！");
                    }
                }


                socket.Send(BitConverter.GetBytes(1));
                Thread.Sleep(20);
                socket.Close();

                Thread.Sleep(500);
                BeginInvoke(new Action(() =>
                {
                    panel1.Location = new Point(2, 2);
                    label_info.Text = "更新成功!";
                    panel1.Visible = true;
                }));


                Thread.Sleep(500);

                if (IsSystemDownLoad)
                {
                    string temp = @"ECHO OFF
ECHO Set WshShell = Wscript.CreateObject(""Wscript.Shell"") >%temp%\tmp.vbs
CMD /c ""ECHO ^Set MyLink = WshShell.CreateShortcut(""" +
Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\" +
Class1.CurrentSystem.SoftNameHead + Class1.CurrentSystem.SoftName + @".lnk"")"" >>%temp%\tmp.vbs""
ECHO MyLink.TargetPath = """ + FilePath + @"\" + Class1.CurrentSystem.SoftName + @".exe"" >>%temp%\tmp.vbs
ECHO MyLink.Save >>%temp%\tmp.vbs
cscript /nologo %temp%\tmp.vbs
DEL /q /s %temp%\tmp.vbs 2>nul 1>nul";
                    string FullName = FilePath + @"\生成快捷方式.cmd";
                    try
                    {
                        File.WriteAllBytes(FullName, Encoding.Default.GetBytes(temp));
                        //启动生成快捷方式
                        System.Diagnostics.Process.Start(FullName);
                    }
                    catch
                    {

                    }
                }

                //启动指定的应用程序
                System.Diagnostics.Process.Start(FilePath + @"\" + Class1.CurrentSystem.SoftName + ".exe");
                BeginInvoke(new Action(() =>
                {
                    CanWindowClose = true;
                    Close();
                }));
            }
            catch (Exception ex)
            {
                BeginInvoke(new Action(() =>
                {
                    MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
                }));
            }
            finally
            {
                CanWindowClose = true;
            }
        }
        private string SizeToString(long size)
        {
            if (size < 1024)
            {
                return size + " B";
            }
            else if (size < 1024 * 1024)
            {
                return ((float)size / 1024).ToString("F1") + " K";
            }
            else
            {
                return ((float)size / (1024 * 1024)).ToString("F1") + " M";
            }
        }
        private string StringShort(string str)
        {
            if (str.Length > 30)
            {
                return str.Substring(0, 20) + "...";
            }
            else
            {
                return str;
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CanWindowClose)
            {
                e.Cancel = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = dialog.SelectedPath;

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FilePath = textBox1.Text;
            textBox1.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectCallback), null);
        }

        /// <summary>
        /// 该方法弃用
        /// </summary>
        private void CheckUpdate()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(ServerIpEndPoint);
            }
            catch
            {
                Invoke(new Action(() =>
                {
                    MessageBox.Show("连接服务器失败，稍后重试。\r\n请检查网络状况，或是服务器被关闭。");
                }));
                socket.Close();
                return;
            }

            try
            {
                int Command = 0x1000;
                socket.Send(BitConverter.GetBytes(Command));


                int _ReceiveLenght = 4;
                int ReceivedLenght = 0;
                byte[] ReceiveByte = new byte[_ReceiveLenght];
                while (ReceivedLenght < _ReceiveLenght)
                {
                    int temp = socket.Receive(ReceiveByte, ReceivedLenght, _ReceiveLenght - ReceivedLenght, SocketFlags.None);
                    ReceivedLenght += temp;
                }

                float ServerVersion = BitConverter.ToSingle(ReceiveByte, 0);


                BeginInvoke(new Action(() =>
                {
                    panel1.Location = new Point(2, 2);
                    label_info.Text = "更新成功!";
                    panel1.Visible = true;
                }));

            }
            catch (Exception ex)
            {
                BeginInvoke(new Action(() =>
                {
                    MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
                }));
            }
            finally
            {
                socket.Close();
            }
        }


    }


}
