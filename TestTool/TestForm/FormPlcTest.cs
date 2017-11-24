using HslCommunication;
using HslCommunication.Profinet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestTool.TestForm
{
    public partial class FormPlcTest : Form
    {
        public FormPlcTest()
        {
            InitializeComponent();
        }

        private void FormPlcTest_Load(object sender, EventArgs e)
        {
            // 窗口的载入方法
            MelsecNetInitialization();


        }

        /// <summary>
        /// 在文本框中追加一条新的字符串信息，该方法是线程安全的，同时允许从前台和后台进行调用显示
        /// </summary>
        /// <param name="str"></param>
        private void TextBoxAppendStringLine(string str)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action<string>(TextBoxAppendStringLine), str);
                return;
            }

            textBox1.AppendText($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {str}{Environment.NewLine}");
        }


        #region 三菱PLC数据测试篇


        private MelsecNet melsec_net = new MelsecNet();
        private void MelsecNetInitialization()
        {
            //初始化
            melsec_net.PLCIpAddress = System.Net.IPAddress.Parse("192.168.0.7");  // PLC的IP地址
            melsec_net.PortRead = 6000;                                           // 端口
            melsec_net.PortWrite = 6001;                                          // 写入端口，最好和读取分开
            melsec_net.NetworkNumber = 0;                                         // 网络号
            melsec_net.NetworkStationNumber = 0;                                  // 网络站号
            melsec_net.ConnectTimeout = 500;                                      // 连接超时时间
        }

        private void userButton1_Click(object sender, EventArgs e)
        {
            // M100-M104读取显示
            OperateResult<byte[]> read = melsec_net.ReadFromPLC(MelsecDataType.M, 100, 5);
            if (read.IsSuccess)
            {
                //成功读取，True代表通，False代表不通
                bool M100 = read.Content[0] == 1;
                bool M101 = read.Content[1] == 1;
                bool M102 = read.Content[2] == 1;
                bool M103 = read.Content[3] == 1;
                bool M104 = read.Content[4] == 1;
                // 显示
                TextBoxAppendStringLine($"M100:{M100} M101:{M101} M102:{M102} M103:{M103} M104:{M104}");
            }
            else
            {
                //失败读取，显示失败信息
                MessageBox.Show(read.ToMessageShowString());
            }
        }
        private void userButton3_Click(object sender, EventArgs e)
        {
            // M100-M104 写入测试 此处写入后M100:通 M101:断 M102:断 M103:通 M104:通
            bool[] values = new bool[] { true, false, false, true, true };
            OperateResult write = melsec_net.WriteIntoPLC(MelsecDataType.M, 200, values);
            if (write.IsSuccess)
            {
                TextBoxAppendStringLine("写入成功");
            }
            else
            {
                MessageBox.Show(write.ToMessageShowString());
            }
        }
        

        private void userButton2_Click(object sender, EventArgs e)
        {
            // D100-D104读取
            OperateResult<byte[]> read = melsec_net.ReadFromPLC(MelsecDataType.D, 100, 5);
            if (read.IsSuccess)
            {
                // 成功读取，提取各自的值，此处的值有个前提假设，假设PLC上的数据是有符号的数据，表示-32768-32767
                short D100 = melsec_net.GetShortFromBytes(read.Content, 0);
                short D101 = melsec_net.GetShortFromBytes(read.Content, 1);
                short D102 = melsec_net.GetShortFromBytes(read.Content, 2);
                short D103 = melsec_net.GetShortFromBytes(read.Content, 3);
                short D104 = melsec_net.GetShortFromBytes(read.Content, 4);
                TextBoxAppendStringLine("D100:" + D100);
                TextBoxAppendStringLine("D101:" + D101);
                TextBoxAppendStringLine("D102:" + D102);
                TextBoxAppendStringLine("D103:" + D103);
                TextBoxAppendStringLine("D104:" + D104);
                //================================================================================
                //这两种方式一样的，如果是无符号的，则使用 ushort D100 = BitConverter.ToUInt16(read.Content, 0);//0-65535
                //short D100 = BitConverter.ToInt16(read.Content, 0);
                //short D101 = BitConverter.ToInt16(read.Content, 2);
                //short D102 = BitConverter.ToInt16(read.Content, 4);
                //short D103 = BitConverter.ToInt16(read.Content, 6);
                //short D104 = BitConverter.ToInt16(read.Content, 8);
            }
            else
            {
                //失败读取
                MessageBox.Show(read.ToMessageShowString());
            }

        }

        private void userButton4_Click(object sender, EventArgs e)
        {

            short[] values = new short[5] { 1335, 8765, 1234, 4567,-2563 };
            // D100为1234,D101为8765,D102为1234,D103为4567,D104为-2563
            OperateResult write = melsec_net.WriteIntoPLC(MelsecDataType.D, 6000, values);
            if (write.IsSuccess)
            {
                //成功写入
                TextBoxAppendStringLine("写入成功");
            }
            else
            {
                MessageBox.Show(write.ToMessageShowString());
            }
        }




        #endregion








        #region 西门子篇二S7通讯协议读取


        private SiemensTcpNet siemensTcpNet = new SiemensTcpNet(SiemensPLCS.S1200)
        {
            PLCIpAddress = System.Net.IPAddress.Parse("192.168.1.195")
        };


        private void userButton9_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {


                OperateResult<byte[]> read = siemensTcpNet.ReadFromPLC("M100", 6);
                if (read.IsSuccess)
                {
                    TextBoxAppendStringLine(HslCommunication.BasicFramework.SoftBasic.ByteToHexString(read.Content));
                }
                else
                {
                    MessageBox.Show(read.ToMessageShowString());
                    if (read.Content != null) textBox1.Text = HslCommunication.BasicFramework.SoftBasic.ByteToHexString(read.Content);
                }
            }
        }

        private void userButton11_Click(object sender, EventArgs e)
        {
            // 1500PLC测试代码

            int type = 2;
            while (type < 256)
            {
                siemensTcpNet.SetPlcType((byte)type);
                OperateResult<byte[]> read = siemensTcpNet.ReadFromPLC("M100", 5);
                if (read.IsSuccess)
                {
                    MessageBox.Show("访问成功！1500PLC TYPE:" + type);
                    break;
                }
                type++;
            }

        }


        private void userButton7_Click(object sender, EventArgs e)
        {
            OperateResult write = siemensTcpNet.WriteIntoPLC("M100", new byte[] { 0xFF, 0x3C, 0x0F });
            if(write.IsSuccess)
            {
                TextBoxAppendStringLine("写入成功");
            }
            else
            {
                MessageBox.Show(write.ToMessageShowString());
            }
        }

        private void userButton6_Click(object sender, EventArgs e)
        {
            // 位写入测试
            for (int i = 0; i < 10; i++)
            {

                OperateResult write = siemensTcpNet.WriteIntoPLC("M100.7", false);
                if (write.IsSuccess)

                {
                    TextBoxAppendStringLine("写入成功");
                }
                else
                {
                    MessageBox.Show(write.ToMessageShowString());

                }
            }
        }



        private void userButton10_Click(object sender, EventArgs e)
        {
            OperateResult<byte[]> read = siemensTcpNet.ReadFromPLC(
                new string[] { "M100", "M150", "M200", "I300" },
                new ushort[] { 1, 4, 3, 1});
            
            if(read.IsSuccess)
            {
                byte value1 = read.Content[0];
                int value2 = siemensTcpNet.GetIntFromBytes(read.Content, 1);
                short value3 = siemensTcpNet.GetShortFromBytes(read.Content, 5);
                byte value4 = read.Content[8];

                TextBoxAppendStringLine($"[{value1},{value2}, {value3}, {value4}]");
            }
            else
            {
                MessageBox.Show(read.ToMessageShowString());
            }
        }




        #endregion

        private void userButton12_Click(object sender, EventArgs e)
        {
            // 条码写入测试
            OperateResult write = siemensTcpNet.WriteAsciiStringIntoPLC("M110", "AKDQWDNADC", 10);
            if (write.IsSuccess)

            {
                TextBoxAppendStringLine("写入成功");
            }
            else
            {
                MessageBox.Show(write.ToMessageShowString());

            }
        }

        private void userButton13_Click(object sender, EventArgs e)
        {
            OperateResult<byte[]> read = siemensTcpNet.ReadFromPLC("M116", 10);
            if (read.IsSuccess)
            {
                TextBoxAppendStringLine(HslCommunication.BasicFramework.SoftBasic.ByteToHexString(read.Content));
            }
            else
            {
                MessageBox.Show(read.ToMessageShowString());
                if(read.Content!=null)textBox1.Text = HslCommunication.BasicFramework.SoftBasic.ByteToHexString(read.Content);
            }
        }

        private void userButton14_Click(object sender, EventArgs e)
        {
            bool[] data = new bool[10]
            {
                false,    // M200.0 = false
                false,    // M200.1 = false
                false,    // M200.2 = false
                true,     // M200.3 = true
                true,     // M200.4 = true
                false,    // M200.5 = false
                true,     // M200.6 = true
                false,    // M200.7 = false
                true,     // M201.0 = true
                false     // M201.1 = false
            };
            data[6] = true;
            OperateResult write = siemensTcpNet.WriteIntoPLC("M200", data);
            if (write.IsSuccess)

            {
                TextBoxAppendStringLine("写入成功");
            }
            else
            {
                MessageBox.Show(write.ToMessageShowString());

            }
        }

        private void userButton15_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                OperateResult<byte[]> read = siemensTcpNet.ReadFromPLC(
                    new string[] { "M100", "M150", "M200", "I300" },
                    new ushort[] { 1, 4, 3, 1 });

                if (read.IsSuccess)
                {
                    byte value1 = read.Content[0];
                    int value2 = siemensTcpNet.GetIntFromBytes(read.Content, 1);
                    short value3 = siemensTcpNet.GetShortFromBytes(read.Content, 5);
                    byte value4 = read.Content[8];

                    TextBoxAppendStringLine($"[{value1},{value2}, {value3}, {value4}]");
                }
                else
                {
                    MessageBox.Show(read.ToMessageShowString());
                }

            }
        }

        private void userButton16_Click(object sender, EventArgs e)
        {
            siemensTcpNet.ConnectServer();
        }
    }
}
