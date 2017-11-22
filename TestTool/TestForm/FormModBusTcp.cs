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
            comboBox1.DataSource = HslCommunication.BasicFramework.SoftBasic.GetEnumValues<HslCommunication.ModBus.ModBusFunctionMask>();
        }
        
        private void FormModBusTcp_FormClosing(object sender, FormClosingEventArgs e)
        {
            tcpServer?.ServerClose();
        }

        #endregion

        private void userButton2_Click(object sender, EventArgs e)
        {
            byte[] data = HslCommunication.BasicFramework.SoftBasic.HexStringToBytes(textBox3.Text);

            HslCommunication.OperateResult<byte[]> read = busTcpClient.ReadFromModBusServer(data);
            if(read.IsSuccess)
            {
                textBox2.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " :" +
                HslCommunication.BasicFramework.SoftBasic.ByteToHexString(read.Content, ' ') + Environment.NewLine);
            }
            else
            {
                MessageBox.Show(read.ToMessageShowString());
            }
        }

        private void userButton3_Click(object sender, EventArgs e)
        {
            // 00 00 00 00 00 06 FF 03 00 00 00 10

            HslCommunication.OperateResult<byte[]> read = null;

            ModBusFunctionMask mask = (ModBusFunctionMask)comboBox1.SelectedItem;

            switch(mask)
            {
                case ModBusFunctionMask.ReadCoil:
                    {
                        // 读线圈
                        read = busTcpClient.ReadCoil(ushort.Parse(textBox4.Text), ushort.Parse(textBox5.Text));
                        break;
                    }
                case ModBusFunctionMask.ReadDiscrete:
                    {
                        // 读离散值
                        read = busTcpClient.ReadDiscrete(ushort.Parse(textBox4.Text), ushort.Parse(textBox5.Text));
                        break;
                    }
                case ModBusFunctionMask.ReadRegister:
                    {
                        // 读寄存器
                        read = busTcpClient.ReadRegister(ushort.Parse(textBox4.Text), ushort.Parse(textBox5.Text));
                        break;
                    }
                default:break;
            }

            if (read != null)
            {
                if (read.IsSuccess)
                {
                    textBox2.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " :" +
                    HslCommunication.BasicFramework.SoftBasic.ByteToHexString(read.Content, ' ') + Environment.NewLine);
                }
                else
                {
                    MessageBox.Show(read.ToMessageShowString());
                }
            }
        }



        #region ModBus Tcp 客户端块


        private ModBusTcpClient busTcpClient = new ModBusTcpClient("192.168.1.195", 502, 0xFF);   // 站号255



        #endregion



        private void userButton4_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox6.Text))
            {
                HslCommunication.OperateResult result = busTcpClient.WriteOneRegister(0, short.Parse(textBox6.Text));
                MessageBox.Show(result.IsSuccess ? "写入成功！" : "写入失败");
            }
        }

        private void userButton5_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox6.Text))
            {
                short value = short.Parse(textBox6.Text);
                textBox2.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + Environment.NewLine);
                HslCommunication.OperateResult result = busTcpClient.WriteOneRegister(0, value);


                textBox2.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + Environment.NewLine);
                HslCommunication.OperateResult<byte[]> read = busTcpClient.ReadRegister(30, 1);

                textBox2.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + Environment.NewLine);

                if ((read.Content[0] * 256 + read.Content[1]) == value)
                {
                    busTcpClient.WriteOneRegister(0, 0);
                    textBox2.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + Environment.NewLine);
                }
            }
        }
    }
}
