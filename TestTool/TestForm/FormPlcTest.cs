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

            textBox1.AppendText($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] {str}{Environment.NewLine}");
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

            // 如果需要长连接，就取消下面这行代码的注释，对于数据读写的代码，没有影响
            // melsec_net.ConnectServer();                                        // 切换长连接，这行代码可以放在其他任何地方
            // melsec_net.ConnectClose();                                         // 关闭长连接，并切换为短连接，在系统退出时可以调用
        }

        private void userButton1_Click(object sender, EventArgs e)
        {
            // M100-M104读取显示
            OperateResult<byte[]> read = melsec_net.ReadFromPLC("M100", 5);

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


        private void userButton20_Click(object sender, EventArgs e)
        {
            // M200-M209读取显示
            OperateResult<byte[]> read = melsec_net.ReadFromPLC("M200", 10);
            if (read.IsSuccess)
            {
                // 成功读取，True代表通，False代表不通
                bool M200 = read.Content[0] == 1;
                bool M201 = read.Content[1] == 1;
                bool M202 = read.Content[2] == 1;
                bool M203 = read.Content[3] == 1;
                bool M204 = read.Content[4] == 1;
                bool M205 = read.Content[5] == 1;
                bool M206 = read.Content[6] == 1;
                bool M207 = read.Content[7] == 1;
                bool M208 = read.Content[8] == 1;
                bool M209 = read.Content[9] == 1;
                // 显示
            }
            else
            {
                //失败读取，显示失败信息
                MessageBox.Show(read.ToMessageShowString());
            }
        }

        private void userButton21_Click(object sender, EventArgs e)
        {
            // X100-X10F读取显示
            OperateResult<byte[]> read = melsec_net.ReadFromPLC("X200", 16);
            if (read.IsSuccess)
            {
                // 成功读取，True代表通，False代表不通
                bool X200 = read.Content[0] == 1;
                bool X201 = read.Content[1] == 1;
                bool X202 = read.Content[2] == 1;
                bool X203 = read.Content[3] == 1;
                bool X204 = read.Content[4] == 1;
                bool X205 = read.Content[5] == 1;
                bool X206 = read.Content[6] == 1;
                bool X207 = read.Content[7] == 1;
                bool X208 = read.Content[8] == 1;
                bool X209 = read.Content[9] == 1;
                bool X20A = read.Content[10] == 1;
                bool X20B = read.Content[11] == 1;
                bool X20C = read.Content[12] == 1;
                bool X20D = read.Content[13] == 1;
                bool X20E = read.Content[14] == 1;
                bool X20F = read.Content[15] == 1;
                // 显示
            }
            else
            {
                //失败读取，显示失败信息
                MessageBox.Show(read.ToMessageShowString());
            }
        }
        private void userButton22_Click(object sender, EventArgs e)
        {
            // 读取M100是否通，十进制地址
            bool M100 = melsec_net.ReadBoolFromPLC("M100").Content;
            // 读取X1A0是否通，十六进制地址
            bool X1A0 = melsec_net.ReadBoolFromPLC("X1A0").Content;
            // 读取Y1A0是否通，十六进制地址
            bool Y1A0 = melsec_net.ReadBoolFromPLC("Y1A0").Content;
            // 读取B1A0是否通，十六进制地址
            bool B1A0 = melsec_net.ReadBoolFromPLC("B1A0").Content;
            // 读取D1000的short值  ,W3C0,R3C0 效果是一样的
            short short_D1000 = melsec_net.ReadShortFromPLC("D1000").Content;
            // 读取D1000的ushort值
            ushort ushort_D1000 = melsec_net.ReadUShortFromPLC("D1000").Content;
            // 读取D1000-D1001组成的int数据
            int int_D100 = melsec_net.ReadIntFromPLC("D1000").Content;
            // 读取D1000-D1001组成的float数据
            float float_D1000 = melsec_net.ReadFloatFromPLC("D1000").Content;
            // 读取D1000-D1003组成的long数据
            long long_D1000 = melsec_net.ReadLongFromPLC("D1000").Content;
            // 读取D1000-D1003组成的double数据
            double double_D1000 = melsec_net.ReadDoubleFromPLC("D1000").Content;
            // 读取D1000-D1009组成的条码数据
            string str_D1000 = melsec_net.ReadStringFromPLC("D1000", 10).Content;


            // 写入M100为通
            melsec_net.WriteIntoPLC("M100", true);
            // 写入Y1A0为通
            melsec_net.WriteIntoPLC("Y1A0", true);
            // 写入X1A0为通
            melsec_net.WriteIntoPLC("X1A0", true);
            // 写入B1A0为通
            melsec_net.WriteIntoPLC("B1A0", true);
            // 写入D1000  short值  ,W3C0,R3C0 效果是一样的
            melsec_net.WriteIntoPLC("D1000", (short)1234);
            // 写入D1000  ushort值
            melsec_net.WriteIntoPLC("D1000", (ushort)45678);
            // 写入D1000  int值
            melsec_net.WriteIntoPLC("D1000", 1234566);
            // 写入D1000  uint值
            melsec_net.WriteIntoPLC("D1000", (uint)1234566);
            // 写入D1000  float值
            melsec_net.WriteIntoPLC("D1000", 123.456f);
            // 写入D1000  double值
            melsec_net.WriteIntoPLC("D1000", 123.456d);
            // 写入D1000  long值
            melsec_net.WriteIntoPLC("D1000", 123456661235123534L);
            // 写入D1000  string值
            melsec_net.WriteAsciiStringIntoPLC("D1000", "K123456789");
        }

        private void userButton23_Click(object sender, EventArgs e)
        {
            byte[] buffer = HslCommunication.BasicFramework.SoftBasic.HexStringToBytes("50 00 00 FF FF 03 00 0D 00 0A 00 01 14 01 00 64 00 00 90 01 00 10");
            // 直接使用报文进行
            OperateResult<byte[]> operate = melsec_net.ReadFromServerCore(buffer);
            if(operate.IsSuccess)
            {
                // 返回PLC的报文反馈，需要自己对报文进行结果分析
                MessageBox.Show(HslCommunication.BasicFramework.SoftBasic.ByteToHexString(operate.Content));
            }
            else
            {
                // 网络原因导致的失败
                MessageBox.Show(operate.ToMessageShowString());
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
        private void userButton17_Click(object sender, EventArgs e)
        {
            bool[] values = new bool[] { true,false ,true};
            OperateResult write = melsec_net.WriteIntoPLC("M100", values);
            if (write.IsSuccess)
            {
                TextBoxAppendStringLine("写入成功");
            }
            else
            {
                MessageBox.Show(write.ToMessageShowString());
            }

        }

        private void userButton19_Click(object sender, EventArgs e)
        {
            OperateResult write = melsec_net.WriteIntoPLC("M100", true);
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
            float[] values = new float[] { 123.456f };
            byte[] buffer = new byte[values.Length * 4];
            for (int i = 0; i < values.Length; i++)
            {
                BitConverter.GetBytes(values[i]).CopyTo(buffer, i * 4);
            }
            OperateResult write = melsec_net.WriteIntoPLC("D1000", buffer);
            if(write.IsSuccess)
            {
                MessageBox.Show("写入成功！");
            }
            else
            {
                MessageBox.Show("写入失败！" + write.ToMessageShowString());
            }




            // short[] values = new short[5] { 1335, 8765, 1234, 4567,-2563 };
            // D100为1234,D101为8765,D102为1234,D103为4567,D104为-2563
            write = melsec_net.WriteIntoPLC(MelsecDataType.D, 6000, values);
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
            for (int i = 0; i < 10; i++)
            {
                OperateResult write = siemensTcpNet.WriteIntoPLC("M100", new byte[] { 0xFF, 0x3C, 0x0F });
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

        private void userButton18_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                TextBoxAppendStringLine(siemensTcpNet.ReadShortFromPLC("M200").Content.ToString()); 

            }
        }


    }
}
