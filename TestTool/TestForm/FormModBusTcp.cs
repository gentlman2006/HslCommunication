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

            busTcpClient.ConnectServer();
        }
        
        private void FormModBusTcp_FormClosing(object sender, FormClosingEventArgs e)
        {
            tcpServer?.ServerClose();
            busTcpClient?.ConnectClose();
        }

        #endregion

        private void userButton2_Click(object sender, EventArgs e)
        {
            byte[] data = HslCommunication.BasicFramework.SoftBasic.HexStringToBytes("00 00 00 00 00 06 00 03 00 00 00 03");

            HslCommunication.OperateResult<byte[]> read = busTcpClient.ReadFromServerCore(data);
            if(read.IsSuccess)
            {
                // 获取结果，并转化为Hex字符串，方便显示
                string result = HslCommunication.BasicFramework.SoftBasic.ByteToHexString(read.Content, ' ');
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
        private void userButton11_Click(object sender, EventArgs e)
        {
            HslCommunication.OperateResult write = busTcpClient.WriteOneCoil(0, true);
            if (write.IsSuccess)
            {
                // 写入成功
                textBox1.Text = "写入成功";
            }
            else
            {
                MessageBox.Show(write.ToMessageShowString());
            }
        }
        private void userButton12_Click(object sender, EventArgs e)
        {
            // 0x00为高位，0x10为低位
            HslCommunication.OperateResult write = busTcpClient.WriteOneRegister(0, 0x00, 0x10);
            if (write.IsSuccess)
            {
                // 写入成功
                textBox1.Text = "写入成功";
            }
            else
            {
                MessageBox.Show(write.ToMessageShowString());
            }
        }
        private void userButton13_Click(object sender, EventArgs e)
        {
            // 线圈0为True，线圈1为false，线圈2为true.....等等，以此类推，数组长度多少，就写入多少线圈
            bool[] value = new bool[] { true, false, true, true, false, false };
            HslCommunication.OperateResult write = busTcpClient.WriteCoil(0, value);
            if (write.IsSuccess)
            {
                // 写入成功
                textBox1.Text = "写入成功";
            }
            else
            {
                MessageBox.Show(write.ToMessageShowString());
            }
        }
        private void userButton14_Click(object sender, EventArgs e)
        {
            ushort[] value = new ushort[] { 46789, 467, 12345 };
            HslCommunication.OperateResult write = busTcpClient.WriteRegister(0, value);
            if (write.IsSuccess)
            {
                // 写入成功
                textBox1.Text = "写入成功";
            }
            else
            {
                MessageBox.Show(write.ToMessageShowString());
            }
        }

        private void userButton15_Click(object sender, EventArgs e)
        {
            int value = 12345678;// 等待写入的一个数据
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);  // 这个是必须的

            HslCommunication.OperateResult write = busTcpClient.WriteRegister(0, buffer);
            if (write.IsSuccess)
            {
                // 写入成功
                textBox1.Text = "写入成功";
            }
            else
            {
                MessageBox.Show(write.ToMessageShowString());
            }
        }


        private ModBusTcpClient busTcpClient = new ModBusTcpClient("192.168.1.195", 502, 0xFF);   // 站号255



        #endregion

        private void userButton8_Click(object sender,EventArgs e)
        {
            HslCommunication.OperateResult<byte[]> read = busTcpClient.ReadDiscrete(0, 10);
            if(read.IsSuccess)
            {
                // 共返回2个字节，以下展示手动处理位，分别获取10和线圈的通断情况
                bool coil_0 = (read.Content[0] & 0x01) == 0x01;
                bool coil_1 = (read.Content[0] & 0x02) == 0x02;
                bool coil_2 = (read.Content[0] & 0x04) == 0x04;
                bool coil_3 = (read.Content[0] & 0x08) == 0x08;
                bool coil_4 = (read.Content[0] & 0x10) == 0x10;
                bool coil_5 = (read.Content[0] & 0x20) == 0x20;
                bool coil_6 = (read.Content[0] & 0x40) == 0x40;
                bool coil_7 = (read.Content[0] & 0x80) == 0x80;
                bool coil_8 = (read.Content[1] & 0x01) == 0x01;
                bool coil_9 = (read.Content[1] & 0x02) == 0x02;
            }
            else
            {
                MessageBox.Show(read.ToMessageShowString());
            }
        }

        private void userButton9_Click(object sender, EventArgs e)
        {
            HslCommunication.OperateResult<byte[]> read = busTcpClient.ReadDiscrete(0, 10);
            if (read.IsSuccess)
            {
                // 共返回2个字节，一次性获取所有节点的通断
                bool[] result = HslCommunication.BasicFramework.SoftBasic.ByteToBoolArray(read.Content, 10);
                bool coil_0 = result[0];
                bool coil_1 = result[1];
                bool coil_2 = result[2];
                bool coil_3 = result[3];
                bool coil_4 = result[4];
                bool coil_5 = result[5];
                bool coil_6 = result[6];
                bool coil_7 = result[7];
                bool coil_8 = result[8];
                bool coil_9 = result[9];
            }
            else
            {
                MessageBox.Show(read.ToMessageShowString());
            }
        }

        private void userButton10_Click(object sender, EventArgs e)
        {
            HslCommunication.OperateResult<byte[]> read = busTcpClient.ReadRegister(0, 10);
            if (read.IsSuccess)
            {
                // 共返回20个字节，每个数据2个字节，高位在前，低位在后
                // 在数据解析前需要知道里面到底存了什么类型的数据，所以需要进行一些假设：
                // 前两个字节是short数据类型
                byte[] buffer = new byte[2];
                buffer[0] = read.Content[1];
                buffer[1] = read.Content[0];

                short value1 = BitConverter.ToInt16(buffer, 0);
                // 接下来的2个字节是ushort类型
                buffer = new byte[2];
                buffer[0] = read.Content[3];
                buffer[1] = read.Content[2];

                ushort value2 = BitConverter.ToUInt16(buffer, 0);
                // 接下来的4个字节是int类型
                buffer = new byte[4];
                buffer[0] = read.Content[7];
                buffer[1] = read.Content[6];
                buffer[2] = read.Content[5];
                buffer[3] = read.Content[4];

                int value3 = BitConverter.ToInt32(buffer, 0);
                // 接下来的4个字节是float类型
                buffer = new byte[4];
                buffer[0] = read.Content[11];
                buffer[1] = read.Content[10];
                buffer[2] = read.Content[9];
                buffer[3] = read.Content[8];

                float value4 = BitConverter.ToSingle(buffer, 0);
                // 接下来的全部字节，共8个字节是规格信息
                string speci = Encoding.ASCII.GetString(read.Content, 12, 8);

                // 已经提取完所有的数据
            }
            else
            {
                MessageBox.Show(read.ToMessageShowString());
            }
        }

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
                if(!busTcpClient.WriteOneRegister(0, value).IsSuccess)
                    textBox2.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "失败" + Environment.NewLine);

                textBox2.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + Environment.NewLine);
                HslCommunication.OperateResult<short> read = busTcpClient.ReadShortRegister(30);

                textBox2.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + Environment.NewLine);

                System.Threading.Thread.Sleep(10);
                if (read.Content == value)
                {
                    busTcpClient.WriteOneRegister(0, 0);
                    textBox2.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + Environment.NewLine);
                }
                else
                {
                    textBox2.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "失败" + Environment.NewLine);
                }

                textBox2.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + busTcpClient.ReadBoolCoil(0).Content + Environment.NewLine);
            }
        }

        private void userButton6_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                HslCommunication.OperateResult result = busTcpClient.WriteRegister(0, new short[] { 0, 0, 0, 0 });
                textBox2.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + result.IsSuccess + Environment.NewLine);
            }
        }
        private void userButton30_Click(object sender, EventArgs e)
        {
            // 读取操作
            bool coil100 = busTcpClient.ReadBoolCoil(100).Content;   // 读取线圈100的通断
            short short100 = busTcpClient.ReadShortRegister(100).Content; // 读取寄存器100的short值
            ushort ushort100 = busTcpClient.ReadUShortRegister(100).Content; // 读取寄存器100的ushort值
            int int100 = busTcpClient.ReadIntRegister(100).Content;      // 读取寄存器100-101的int值
            uint uint100 = busTcpClient.ReadUIntRegister(100).Content;   // 读取寄存器100-101的uint值
            float float100 = busTcpClient.ReadFloatRegister(100).Content; // 读取寄存器100-101的float值
            long long100 = busTcpClient.ReadLongRegister(100).Content;    // 读取寄存器100-103的long值
            ulong ulong100 = busTcpClient.ReadULongRegister(100).Content; // 读取寄存器100-103的ulong值
            double double100 = busTcpClient.ReadDoubleRegister(100).Content; // 读取寄存器100-103的double值
            string str100 = busTcpClient.ReadStringRegister(100, 5).Content;// 读取100到104共10个字符的字符串

            // 写入操作
            busTcpClient.WriteOneCoil(100, true);// 写入线圈100为通
            busTcpClient.WriteRegister(100, (short)12345);// 写入寄存器100为12345
            busTcpClient.WriteRegister(100, (ushort)45678);// 写入寄存器100为45678
            busTcpClient.WriteRegister(100, 123456789);// 写入寄存器100-101为123456789
            busTcpClient.WriteRegister(100, (uint)123456778);// 写入寄存器100-101为123456778
            busTcpClient.WriteRegister(100, 123.456);// 写入寄存器100-101为123.456
            busTcpClient.WriteRegister(100, 12312312312414L);//写入寄存器100-103为一个大数据
            busTcpClient.WriteRegister(100, 12634534534543656UL);// 写入寄存器100-103为一个大数据
            busTcpClient.WriteRegister(100, 123.456d);// 写入寄存器100-103为一个双精度的数据
            busTcpClient.WriteRegister(100, "K123456789");
            
        }
    }
}
