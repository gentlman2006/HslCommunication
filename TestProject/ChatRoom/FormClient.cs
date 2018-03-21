using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication;
using HslCommunication.Enthernet;
using HslCommunication.BasicFramework;
using Newtonsoft.Json.Linq;
using HslCommunication.Core.Net;

namespace ChatRoom
{
    public partial class FormClient : Form
    {
        public FormClient()
        {
            InitializeComponent();


            FormLogin login = new FormLogin();
            if(login.ShowDialog() != DialogResult.OK)
            {
                Dispose();
                Application.Exit();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Net_Socket_Client_Initialization();
        }

        private void FormClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            net_socket_client.ClientClose();
        }


        /// <summary>
        /// 登录的用户名字
        /// </summary>
        public static string LoginName { get; set; }


        #region 客户端网络块


        private NetworkComplexClient net_socket_client = new NetworkComplexClient( );

        private void Net_Socket_Client_Initialization()
        {
            try
            {
                net_socket_client.Token = new Guid("91625bad-d581-44ab-b121-ffff5bcb83fb");          // 设置令牌，必须与连接的服务器令牌一致
                net_socket_client.EndPointServer = new System.Net.IPEndPoint(
                    System.Net.IPAddress.Parse("127.0.0.1"),12345);                                     // 连接的服务器的地址，必须和服务器端的信息对应
                net_socket_client.ClientAlias = LoginName;                                              // 传入账户名
                net_socket_client.AcceptString += Net_socket_client_AcceptString;                       // 接收到字符串信息时触发
                net_socket_client.ClientStart();
            }
            catch (Exception ex)
            {
                SoftBasic.ShowExceptionMessage(ex);
            }
        }

        /// <summary>
        /// 接收到服务器的字节数据的回调方法
        /// </summary>
        /// <param name="state">网络连接对象</param>
        /// <param name="customer">用户自定义的指令头，用来区分数据用途</param>
        /// <param name="data">数据</param>
        private void Net_socket_client_AcceptString(AppSession state, NetHandle customer, string data)
        {
            // 我们规定
            // 1 是系统消息，
            // 2 是用户发送的消息
            // 3 客户端在线信息
            // 4 退出指令
            // 当你的消息头种类很多以后，可以在一个统一的类中心进行规定
            if (customer == 1)
            {
                ShowSystemMsg(data);
            }
            else if(customer == 2)
            {
                ShowMsg(data);
            }
            else if(customer == 3)
            {
                ShowOnlineClient(data);
            }
            else if(customer == 4)
            {
                // 退出系统
                QuitSystem( );
            }
        }




        #endregion

        #region 窗口方法

        private void ShowMsg(string msgJson)
        {
            if (textBox2.InvokeRequired)
            {
                textBox2.Invoke(new Action<string>(ShowMsg), msgJson);
                return;
            }

            NetMessage msg = JObject.Parse(msgJson).ToObject<NetMessage>();
            if (msg.Type == "string")
            {
                textBox2.AppendText(msg.FromName + msg.Time.ToString("[ HH:mm:ss ]") + Environment.NewLine + msg.Content + Environment.NewLine);
            }
        }


        private void ShowSystemMsg(string msg)
        {
            if (textBox1.InvokeRequired)
            {
                textBox1.Invoke(new Action<string>(ShowSystemMsg), msg);
                return;
            }

            textBox1.AppendText(DateTime.Now.ToString() + " " + msg + Environment.NewLine);
        }


        private void ShowOnlineClient(string json)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(ShowOnlineClient), json);
                return;
            }


            listBox1.DataSource = JArray.Parse(json).ToObject<NetAccount[]>();
        }


        private void QuitSystem()
        {
            if (InvokeRequired)
            {
                Invoke( new Action( QuitSystem ) );
                return;
            }

            Close( );
        }


        #endregion


        // 发送消息
        private void userButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox3.Text)) return;

            net_socket_client.Send(2, textBox3.Text);
            textBox3.Clear();
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                userButton1.PerformClick();
            }
        }
    } 
}
