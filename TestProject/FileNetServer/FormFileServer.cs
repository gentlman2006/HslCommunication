using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication.Enthernet;

namespace FileNetServer
{
    public partial class FormFileServer : Form
    {
        public FormFileServer( )
        {
            InitializeComponent( );
        }

        private void FormFileServer_Load( object sender, EventArgs e )
        {
            textBox2.Text = Guid.Empty.ToString( );
            textBox9.Text = Guid.Empty.ToString( );

            textBox3.Text = Application.StartupPath + "\\AdvancedFiles";
            textBox4.Text = Application.StartupPath + "\\FileTemp";

            textBox8.Text = Application.StartupPath + "\\UltimateFiles";
        }

        private void linkLabel1_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            try
            {
                System.Diagnostics.Process.Start( linkLabel1.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void linkLabel2_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            HslCommunication.BasicFramework.FormSupport form = new HslCommunication.BasicFramework.FormSupport( );
            form.ShowDialog( );
        }

        #region Advanced Server

        private AdvancedFileServer advancedFileServer;

        private void AdvancedFileServerStart( )
        {
            // textBox1.Text为端口号信息
            if (!int.TryParse( textBox1.Text, out int port ))
            {
                MessageBox.Show( "Advanced文件服务器引擎的端口号输入异常" );
            }

            advancedFileServer = new AdvancedFileServer( );
            advancedFileServer.FilesDirectoryPath = textBox3.Text;                 // 所有文件存储的路径
            advancedFileServer.FilesDirectoryPathTemp = textBox4.Text;             // 临时文件的目录
            advancedFileServer.Token = new Guid( textBox2.Text );                  // 令牌
            advancedFileServer.LogNet = new HslCommunication.LogNet.LogNetSingle( Application.StartupPath + "\\Logs\\AdvancedLog.txt" );  // 设置日志
            advancedFileServer.LogNet.BeforeSaveToFile += LogNet1_BeforeSaveToFile;
            advancedFileServer.ServerStart( port );
        }

        private void LogNet1_BeforeSaveToFile( object sender, HslCommunication.LogNet.HslEventArgs e )
        {
            if(InvokeRequired)
            {
                Invoke( new Action<object, HslCommunication.LogNet.HslEventArgs>( LogNet1_BeforeSaveToFile ), sender, e );
                return;
            }

            textBox5.AppendText( e.HslMessage.ToString( ) + Environment.NewLine );
        }
        private void button1_Click( object sender, EventArgs e )
        {
            try
            {
                // 启动文件服务器
                AdvancedFileServerStart( );
                button1.Enabled = false;
            }
            catch(Exception ex)
            {
                HslCommunication.BasicFramework.SoftBasic.ShowExceptionMessage( ex );
            }
        }

        #endregion

        #region Ultimate Server



        private UltimateFileServer ultimateFileServer;

        private void UltimateFileServerStart( )
        {
            if (!int.TryParse( textBox10.Text, out int port ))
            {
                MessageBox.Show( "Advanced文件服务器引擎的端口号输入异常" );
            }

            ultimateFileServer = new UltimateFileServer( );
            ultimateFileServer.FilesDirectoryPath = textBox8.Text;                   // 设置文件的存储路径
            ultimateFileServer.Token = new Guid( textBox9.Text );                    // 令牌
            ultimateFileServer.LogNet = new HslCommunication.LogNet.LogNetSingle( Application.StartupPath + "\\Logs\\UltimateLog.txt" );  // 日志
            ultimateFileServer.LogNet.BeforeSaveToFile += LogNet2_BeforeSaveToFile;
            ultimateFileServer.ServerStart( port );
        }

        private void LogNet2_BeforeSaveToFile( object sender, HslCommunication.LogNet.HslEventArgs e )
        {
            if (InvokeRequired)
            {
                Invoke( new Action<object, HslCommunication.LogNet.HslEventArgs>( LogNet2_BeforeSaveToFile ), sender, e );
                return;
            }

            textBox6.AppendText( e.HslMessage.ToString( ) + Environment.NewLine );
        }
        private void button2_Click( object sender, EventArgs e )
        {
            try
            {
                // 启动服务器
                UltimateFileServerStart( );
                button2.Enabled = false;
            }
            catch (Exception ex)
            {
                HslCommunication.BasicFramework.SoftBasic.ShowExceptionMessage( ex );
            }
        }



        #endregion


    }
}
