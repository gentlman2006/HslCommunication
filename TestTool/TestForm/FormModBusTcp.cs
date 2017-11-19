using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication.ModBus;

namespace TestTool.TestForm
{
    public partial class FormModBusTcp : Form
    {
        public FormModBusTcp()
        {
            InitializeComponent();

            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
        }



        private bool m_IsModBusStart { get; set; } = false;
        private ModBusTcpServer tcpServer;
        private long m_ReceivedTimes { get; set; }


        private void userButton1_Click(object sender, EventArgs e)
        {
            if (!m_IsModBusStart)
            {
                m_IsModBusStart = true;
                tcpServer = new ModBusTcpServer(); // 实例化服务器接收对象
                tcpServer.LogNet = new HslCommunication.LogNet.LogNetSingle(Application.StartupPath + @"\Logs\log.txt"); // 设置日志文件
                tcpServer.OnDataReceived += TcpServer_OnDataReceived; // 关联数据接收方法
                tcpServer.ServerStart(51234); // 绑定端口
                timer.Start(); // 启动服务
            }
        }


        private void TcpServer_OnDataReceived(byte[] object1)
        {
            m_ReceivedTimes++;
            BeginInvoke(new Action<byte[]>(ShowModbusData), object1);
        }


        private void ShowModbusData(byte[] modbus)
        {
            textBox1.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " :" +
                HslCommunication.BasicFramework.SoftBasic.ByteToHexString(modbus) + Environment.NewLine);
        }



        #region 统计每秒数据接收次数

        private Timer timer = new Timer();
        private long times_old = 0;
        private void Timer_Tick(object sender, EventArgs e)
        {
            long times = m_ReceivedTimes - times_old;
            label1.Text = times.ToString();
            times_old = m_ReceivedTimes;
        }

        private void FormModBusTcp_Load(object sender, EventArgs e)
        {

        }
        
        private void FormModBusTcp_FormClosing(object sender, FormClosingEventArgs e)
        {
            tcpServer?.ServerClose();
        }

        #endregion

        private void userButton2_Click(object sender, EventArgs e)
        {
            byte[] data = HslCommunication.BasicFramework.SoftBasic.HexStringToBytes(textBox3.Text);

            textBox2.AppendText(HslCommunication.BasicFramework.SoftBasic.ByteToHexString(data,'_') + Environment.NewLine);
        }
    }
}
