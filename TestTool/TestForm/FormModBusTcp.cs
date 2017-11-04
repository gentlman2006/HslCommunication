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
        }



        private bool m_IsModBusStart { get; set; } = false;
        private ModBusTcpServer tcpServer;


        private void userButton1_Click(object sender, EventArgs e)
        {
            if (!m_IsModBusStart)
            {
                tcpServer = new ModBusTcpServer();
                tcpServer.OnDataReceived += TcpServer_OnDataReceived;
                tcpServer.ServerStart(51234);
            }
        }


        private void TcpServer_OnDataReceived(byte[] object1)
        {
            BeginInvoke(new Action<byte[]>(ShowModbusData), object1);
        }


        private void ShowModbusData(byte[] modbus)
        {
            textBox1.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " : " +
                HslCommunication.BasicFramework.SoftBasic.ByteToHexString(modbus) + Environment.NewLine);
        }

        private void FormModBusTcp_FormClosing(object sender, FormClosingEventArgs e)
        {
            tcpServer?.ServerClose();
        }

    }
}
