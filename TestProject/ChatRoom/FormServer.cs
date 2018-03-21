using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication;
using HslCommunication.Core.Net;
using HslCommunication.Enthernet;
using Newtonsoft.Json.Linq;

namespace ChatRoom
{
    public partial class FormServer : Form
    {
        public FormServer()
        {
            InitializeComponent();
        }

        private void FormServer_Load(object sender, EventArgs e)
        {
            ComplexServerInitialization();
        }


        #region 核心网络服务相关


        private NetworkComplexServer complexServer;

        private void ComplexServerInitialization()
        {
            complexServer = new NetworkComplexServer( );                                             // 实例化
            complexServer.Token = new Guid("91625bad-d581-44ab-b121-ffff5bcb83fb");          // 设置令牌，提升安全性
            complexServer.LogNet = new HslCommunication.LogNet.LogNetSingle("log.txt");         // 设置日志记录，如果不需要，可以删除
            complexServer.ClientOnline += ComplexServer_ClientOnline;                           // 客户端上线时触发
            complexServer.ClientOffline += ComplexServer_ClientOffline;                         // 客户端下线时触发
            complexServer.AllClientsStatusChange += ComplexServer_AllClientsStatusChange;       // 只要有客户端上线或下线就触发
            complexServer.AcceptString += ComplexServer_AcceptString;                           // 客户端发来消息时触发
            complexServer.ServerStart(12345);                                                   // 启动服务，需要选择一个端口
        }

        private void ComplexServer_AllClientsStatusChange(string object1)
        {
            
        }

        private void ComplexServer_AcceptString(AppSession object1, NetHandle object2, string object3)
        {
            // 我们规定
            // 1 是系统消息，
            // 2 是用户发送的消息
            // 3 客户端在线信息
            // 4 强制客户端下线
            // 当你的消息头种类很多以后，可以在一个统一的类中心进行规定
            if (object2 == 2)
            {
                // 来自客户端的消息，就只有这么一种情况
                NetMessage msg = new NetMessage()
                {
                    FromName = object1.LoginAlias,
                    Time = DateTime.Now,
                    Type = "string",
                    Content = object3,
                };

                // 群发出去
                complexServer.SendAllClients(2, JObject.FromObject(msg).ToString());
            }
        }

        private void ComplexServer_ClientOffline( AppSession object1, string object2)
        {
            // 客户端下线，发送消息给客户端
            complexServer.SendAllClients(1, object1.IpAddress + " " + object1.LoginAlias + " : " + object2);
            // 发送在线信息
            complexServer.SendAllClients(3, RemoveOnLine(object1.ClientUniqueID));

            // 在主界面显示信息
            ShowMsg(object1.IpAddress + " " + object1.LoginAlias + " : " + object2);
            ShowOnlineClient( );
        }

        private void ComplexServer_ClientOnline( AppSession object1 )
        {
            // 客户端上线，发送消息给客户端
            complexServer.SendAllClients(1, object1.IpAddress + " " + object1.LoginAlias + " : 上线");
            // 发送在线信息
            NetAccount account = new NetAccount()
            {
                Guid = object1.ClientUniqueID,
                Ip = object1.IpAddress,
                Name = object1.LoginAlias,
                OnlineTime = DateTime.Now.ToString(),
            };
            complexServer.SendAllClients(3,  AddOnLine(account));

            // 在主界面显示信息
            ShowMsg(object1.IpAddress + " " + object1.LoginAlias + " : 上线");
            ShowOnlineClient( );
        }


        #endregion

        #region 在线客户端信息实现块

        private List<NetAccount> all_accounts = new List<NetAccount>();
        private object obj_lock = new object();

        // 新增一个用户账户到在线客户端
        private string AddOnLine(NetAccount item)
        {
            string result = string.Empty;
            lock(obj_lock)
            {
                all_accounts.Add(item);
                result = JArray.FromObject(all_accounts).ToString();
            }
            return result;
        }

        // 移除在线账户并返回相应的在线信息
        private string RemoveOnLine(string guid)
        {
            string result = string.Empty;
            lock (obj_lock)
            {
                for (int i = 0; i < all_accounts.Count; i++)
                {
                    if(all_accounts[i].Guid == guid)
                    {
                        all_accounts.RemoveAt(i);
                        break;
                    }
                }
                result = JArray.FromObject(all_accounts).ToString();
            }
            return result;
        }


        #endregion

        #region 窗口相关

        private void ShowMsg(string msg)
        {
            if(textBox1.InvokeRequired)
            {
                textBox1.Invoke(new Action<string>(ShowMsg), msg);
                return;
            }

            textBox1.AppendText(DateTime.Now.ToString() + " " + msg + Environment.NewLine);
        }


        private void ShowOnlineClient()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(ShowOnlineClient));
                return;
            }

            lock(obj_lock)
            {
                listBox1.DataSource = all_accounts.ToArray( );
                label4.Text = all_accounts.Count.ToString();
            }
        }


        #endregion

        private void userButton1_Click(object sender, EventArgs e)
        {
            // 服务器发送系统消息到客户端
            if(!string.IsNullOrEmpty(textBox2.Text))
            {
                // 来自客户端的消息，就只有这么一种情况
                NetMessage msg = new NetMessage()
                {
                    FromName = "系统",
                    Time = DateTime.Now,
                    Type = "string",
                    Content = textBox2.Text,
                };

                // 群发出去
                complexServer.SendAllClients(2, JObject.FromObject(msg).ToString());
            }
        }

        private void userButton2_Click( object sender, EventArgs e )
        {
            // 关闭指定的客户端，指定用户名称即可
            if (!string.IsNullOrEmpty( textBox3.Text ))
            {
                complexServer.SendClientByAlias( textBox3.Text, 4, "" );
            }
        }

        private void userButton3_Click( object sender, EventArgs e )
        {
            // 关闭所有的在线客户端，不指定名称
            complexServer.SendAllClients( 4, "" );
        }

        private void userButton4_Click( object sender, EventArgs e )
        {
            FormClient client = new FormClient( );
            client.Show( );
        }
    }



}
