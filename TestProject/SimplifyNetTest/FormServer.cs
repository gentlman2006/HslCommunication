﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication.Enthernet;
using HslCommunication.Core.Net;
using HslCommunication;

namespace SimplifyNetTest
{
    public partial class FormServer : Form
    {
        public FormServer()
        {
            InitializeComponent( );
        }

        private void FormServer_Load( object sender, EventArgs e )
        {
            textBox3.Text = Guid.Empty.ToString( );
        }

        #region Simplify Net

        private NetSimplifyServer simplifyServer;
        private Timer timerSecond;

        private void Start()
        {
            try
            {
                simplifyServer = new NetSimplifyServer( );                                          // 实例化
                simplifyServer.Token = new Guid( textBox3.Text );                                   // 设置令牌
                simplifyServer.ReceiveStringEvent += SimplifyServer_ReceiveStringEvent;             // 接收字符串的时候触发
                simplifyServer.LogNet = new HslCommunication.LogNet.LogNetSingle( Application.StartupPath + @"\Logs\log.txt" );
                simplifyServer.LogNet.BeforeSaveToFile += LogNet_BeforeSaveToFile;                  // 日志保存前先显示出来
                simplifyServer.ServerStart( int.Parse( textBox1.Text ) );                           // 启动服务
                userButton1.Enabled = false;


                timerSecond = new Timer( );                            // 这个定时器的功能是每秒更新在线的客户端数量
                timerSecond.Interval = 1000;
                timerSecond.Tick += TimerSecond_Tick;
                timerSecond.Start( );
            }
            catch(Exception ex )
            {
                MessageBox.Show( "创建失败：" + ex.Message );
            }
        }

        private void TimerSecond_Tick( object sender, EventArgs e )
        {
            label6.Text = simplifyServer.ClientCount.ToString( );
        }

        private void LogNet_BeforeSaveToFile( object sender, HslCommunication.LogNet.HslEventArgs e )
        {
            if(InvokeRequired)
            {
                BeginInvoke( new Action<object, HslCommunication.LogNet.HslEventArgs>( LogNet_BeforeSaveToFile), sender, e );
                return;
            }

            textBox2.AppendText( e.HslMessage.ToString( ) + Environment.NewLine );
        }

        private void SimplifyServer_ReceiveStringEvent( AppSession session, NetHandle handle, string value )
        {
            if (handle == 1)
            {
                // 当收到客户端发来的信号1的时候进行操作的消息
                simplifyServer.SendMessage( session, handle, "这是测试信号：" + value );
            }
            else if (handle < 100)
            {
                simplifyServer.SendMessage( session, handle, "这是测试信号：" + handle );
            }
            else
            {
                simplifyServer.SendMessage( session, handle, "不支持的消息" );
            }


            // 显示出来，谁发的，发了什么
            textBox2.Invoke( new Action( ( ) => {
                textBox2.AppendText( $"{session} [{handle}] {value}" + Environment.NewLine );
            } ) );

        }

        private void userButton1_Click( object sender, EventArgs e )
        {
            // 点击了启动服务的按钮
            Start( );
        }


        #endregion



        private void userButton2_Click( object sender, EventArgs e )
        {
            // 连接异形客户端
            using (FormInputAlien form = new FormInputAlien( ))
            {
                if (form.ShowDialog( ) == DialogResult.OK)
                {
                    OperateResult connect = simplifyServer.ConnectHslAlientClient( form.IpAddress, form.Port, form.DTU );
                    if (connect.IsSuccess)
                    {
                        MessageBox.Show( "连接成功！" );
                    }
                    else
                    {
                        MessageBox.Show( "连接失败！原因：" + connect.Message );
                    }
                }
            }
        }
    }
}
