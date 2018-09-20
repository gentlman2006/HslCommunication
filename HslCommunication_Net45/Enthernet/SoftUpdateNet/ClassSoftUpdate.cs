using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using HslCommunication.Core;
using HslCommunication.Core.Net;

namespace HslCommunication.Enthernet
{

    /// <summary>
    /// 用于服务器支持软件全自动更新升级的类
    /// </summary>
    public sealed class NetSoftUpdateServer : NetworkServerBase
    {

        #region Constructor

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        /// <param name="updateExeFileName">更新程序的名称</param>
        public NetSoftUpdateServer( string updateExeFileName = "软件自动更新.exe" )
        {
            this.updateExeFileName = updateExeFileName;
        }

        #endregion

        #region Private Member

        private string m_FilePath = @"C:\HslCommunication";
        private string updateExeFileName;                     // 软件更新的声明

        #endregion

        /// <summary>
        /// 系统升级时客户端所在的目录，默认为C:\HslCommunication
        /// </summary>
        public string FileUpdatePath
        {
            get { return m_FilePath; }
            set { m_FilePath = value; }
        }


     
        /// <summary>
        /// 系统的登录方法
        /// </summary>
        /// <param name="obj">接收的socket对象</param>
        protected override void ThreadPoolLogin(object obj)
        {
            Socket socket = obj as Socket;
            try
            {
                OperateResult<byte[]> receive = Receive( socket, 4 );
                if (!receive.IsSuccess) return;

                byte[] ReceiveByte = receive.Content;
                int Protocol = BitConverter.ToInt32(ReceiveByte, 0);

                if (Protocol == 0x1001 || Protocol == 0x1002)
                {
                    // 安装系统和更新系统
                    if (Protocol == 0x1001)
                    {
                        LogNet?.WriteInfo(ToString(), StringResources.Language.SystemInstallOperater + ((IPEndPoint)socket.RemoteEndPoint).Address.ToString());
                    }
                    else
                    {
                        LogNet?.WriteInfo( ToString( ), StringResources.Language.SystemUpdateOperater + ((IPEndPoint)socket.RemoteEndPoint).Address.ToString());
                    }
                    if (Directory.Exists(FileUpdatePath))
                    {
                        string[] files = Directory.GetFiles(FileUpdatePath);
                        
                        List<string> Files = new List<string>(files);
                        for (int i = Files.Count - 1; i >= 0; i--)
                        {
                            FileInfo finfo = new FileInfo(Files[i]);
                            if (finfo.Length > 200000000)
                            {
                                Files.RemoveAt(i);
                            }
                            if (Protocol == 0x1002)
                            {
                                if (finfo.Name == this.updateExeFileName)
                                {
                                    Files.RemoveAt(i);
                                }
                            }
                        }
                        files = Files.ToArray();

                        socket.BeginReceive(new byte[4], 0, 4, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);

                        socket.Send(BitConverter.GetBytes(files.Length));
                        for (int i = 0; i < files.Length; i++)
                        {
                            // 传送数据包含了本次数据大小，文件数据大小，文件名（带后缀）
                            FileInfo finfo = new FileInfo(files[i]);
                            string fileName = finfo.Name;
                            byte[] ByteName = Encoding.Unicode.GetBytes(fileName);

                            int First = 4 + 4 + ByteName.Length;
                            byte[] FirstSend = new byte[First];

                            FileStream fs = new FileStream(files[i], FileMode.Open, FileAccess.Read);

                            Array.Copy(BitConverter.GetBytes(First), 0, FirstSend, 0, 4);
                            Array.Copy(BitConverter.GetBytes((int)fs.Length), 0, FirstSend, 4, 4);
                            Array.Copy(ByteName, 0, FirstSend, 8, ByteName.Length);

                            socket.Send(FirstSend);
                            Thread.Sleep(10);

                            byte[] buffer = new byte[4096];
                            int sended = 0;
                            while (sended < fs.Length)
                            {
                                int count = fs.Read(buffer, 0, 4096);
                                socket.Send(buffer, 0, count, SocketFlags.None);
                                sended += count;
                            }

                            fs.Close();
                            fs.Dispose();

                            Thread.Sleep(20);
                        }
                    }
                }
                else
                {
                    // 兼容原先版本的更新，新的验证方式无需理会
                    socket.Send(BitConverter.GetBytes(10000f));
                    Thread.Sleep(20);
                    socket?.Close();
                }
            }
            catch (Exception ex)
            {
                Thread.Sleep(20);
                socket?.Close();
                LogNet?.WriteException( ToString( ), StringResources.Language.FileSendClientFailed, ex);
            }
        }


        private void ReceiveCallBack(IAsyncResult ir)
        {
            if (ir.AsyncState is Socket socket)
            {
                try
                {
                    socket.EndReceive(ir);
                }
                catch(Exception ex)
                {
                    LogNet?.WriteException( ToString( ), ex);
                }
                finally
                {
                    socket?.Close();
                    socket = null;
                }
            }
        }


        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString( )
        {
            return "NetSoftUpdateServer";
        }


        #endregion

    }
}
