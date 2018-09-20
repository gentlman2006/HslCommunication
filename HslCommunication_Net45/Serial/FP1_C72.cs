using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.IO.Ports;
using System.Threading;

namespace FP1_F72
{
    class FP1_C72
    {
        private SerialPort serialPort1 = new SerialPort( );
        private bool portOpened = false;

        private string tobcc(string s) //帧校验函数FCS
        {
            int t = 0;
            char[] chars = s.ToCharArray();
            t = chars[0];
            for (int i = 1; i < s.Length; i++)
            {
                t ^= (char)chars[i];
            }
            return intToHex(t / 16) + intToHex(t % 16);
        }


        public string intToHex(int value)
        {
            if (value < 0 || value > 15)
            {
                return "";
            }
            if (value < 10)
                return value.ToString();
            char ret = 'A';
            ret += (char)(value - 10);
            return ret.ToString();
        }

        public string BCDStrToHexStr(string bcdstr)
        {
            string rets = "";
            if(bcdstr.Length <=2)
                return bcdstr;
            for (int i = bcdstr.Length; i > 0; i-=2)
                if(i==1 )
                  rets += bcdstr.Substring(0, 1);
                else
                  rets += bcdstr.Substring(i - 2, 2);
            return rets;
        }

        public string IntToHexStr(int value)
        {
            return Convert.ToString(value, 16).PadLeft(4,'0');

        }

        public int HexStrToInt(string hexStr)
        {
            return Convert.ToInt32(hexStr, 16);
        }

        /// <summary>
        /// 字符串颠倒
        /// </summary>
        /// <param name="str">输入串</param>
        /// <returns>颠倒串</returns>
        public string ReverseStr(string str)
        {
            int i = str.Length;
            string rets="";
            for (int j = 0; j < i; j++)
                rets += str[i - j - 1];
            return rets;
        }

       /// <summary>
       /// 十六进制串转换为二进制串
       /// </summary>
        /// <param name="hexStr">十六进制串</param>
        /// <returns>二进制串</returns>
        public string HexStrToBitStr(string hexStr)
        {
            string ret = "", tmp;
            
            for (int i = 0; i < hexStr.Length; i++)
            {
                switch (hexStr.Substring(i, 1))
                {
                    case "0":
                        tmp = "0000";
                        break;
                    case "1":
                        tmp = "0001";
                        break;
                    case "2":
                        tmp = "0010";
                        break;
                    case "3":
                        tmp = "0011";
                        break;
                    case "4":
                        tmp = "0100";
                        break;
                    case "5":
                        tmp = "0101";
                        break;
                    case "6":
                        tmp = "0110";
                        break;
                    case "7":
                        tmp = "0111";
                        break;
                    case "8":
                        tmp = "1000";
                        break;
                    case "9":
                        tmp = "1001";
                        break;
                    case "A":
                        tmp = "1010";
                        break;
                    case "B":
                        tmp = "1011";
                        break;
                    case "C":
                        tmp = "1100";
                        break;
                    case "D":
                        tmp = "1101";
                        break;
                    case "E":
                        tmp = "1110";
                        break;
                    case "F":
                        tmp = "1111";
                        break;
                    default:
                        tmp = "0000";
                        break;

                }
                ret = ret + tmp;
            }
            return ret;
        }

        

        /// <summary>
        /// PortOpen--打开串口
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="baudrate">波特率</param>
        /// <param name="databit">数据位</param>
        /// <param name="stopbit">停止位</param>
        /// <param name="oddEven">奇偶位</param>
        /// <param name="errMsg">错误信息反馈</param>
        /// <returns>正确为true 否则为false</returns>
        public bool PortOpen(int port,int baudrate,int databit,int stopbit,int oddEven,ref string errMsg )
        {
            serialPort1.PortName = "COM"+port;
            serialPort1.BaudRate = baudrate;
            string rets="",errs="";
            switch (oddEven)
            {
                case 0:
                    serialPort1.Parity = Parity.None;
                    break;
                case 1:
                    serialPort1.Parity = Parity.Odd;
                    break;
                case 2:
                    serialPort1.Parity = Parity.Even;
                    break;
                case 3:
                    serialPort1.Parity = Parity.Mark;
                    break;
                case 4:
                    serialPort1.Parity = Parity.Space;
                    break;
            }
           
            switch (stopbit)
            {
                case 0:
                    serialPort1.StopBits = StopBits.None ;
                    break;
                case 1:
                    serialPort1.StopBits = StopBits.One;
                    break;
                case 2:
                    serialPort1.StopBits = StopBits.OnePointFive;
                    break;
                case 3:
                    serialPort1.StopBits = StopBits.Two;
                    break;
            }
            serialPort1.DataBits = databit ;
            try{
                serialPort1.Open();
                if (!SendPlcCmd("%EE#RT", ref rets, ref errs))
                {
                    errMsg = "PLC未连接上，请检查连接线路或通讯参数设置是否正常！";
                    serialPort1.Close();
                    return false;
                }
            }catch(Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
            portOpened = true;
            return true;
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        /// <returns></returns>
        public bool PortClose()
        {
            if (portOpened)
                serialPort1.Close();
            portOpened = false;
            return true;
        }

        /// <summary>
        /// PLC 错误信息
        /// </summary>
        /// <param name="errcode">错误编码</param>
        /// <returns>错误信息</returns>
        private string GetPlcErrInfo(int errcode)
        {
            switch (errcode)
            {
                case 21:
                    return "NACK 错误:远程单元无法被正确识别，或者发生了数据错误.";
                case 22:
                    return "WACK 错误: 用于远程单元的接收缓冲区已满. ";
                case 23:
                    return "多重端口错误:远程单元编号(01 至16)设置与本地单元重复. ";
                case 24:
                    return "传输格式错误:试图发送不符合传输格式的数据或者某一帧数据溢出或发生了数据错误. ";
                case 25:
                    return "硬件错误: 传输系统硬件停止操作.";
                case 26:
                    return "单元号错误: 远程单元的编号设置超出01 至63 的范围. ";
                case 27:
                    return "不支持错误: 接收方数据帧溢出. 试图在不同的模块之间发送不同帧长度的数据.";
                case 28:
                    return "无应答错误:远程单元不存在(超时) .";
                case 29:
                    return "缓冲区关闭错误:试图发送或接收处于关闭状态的缓冲区,持续处于传输禁止状态.";
                case 30:
                    return "超时错误: ";
                case 40:
                    return "BCC 错误: 在指令数据中发生传输错误. ";
                case 41:
                    return "格式错误:所发送的指令信息不符合传输格式. 例: 指令中的数据项目过多或不足. 缺少'#'符号及'目标站号' .";
                case 42:
                    return "不支持错误: 发送了一个未被支持的指令. 向未被支持的目标站发送了指令. ";
                case 43:
                    return "处理步骤错误:在处于传输请求信息挂起时,发送了其他指令. ";
                case 50:
                    return "链接设置错误: 设置了实际不存在的链接编号. ";
                case 51:
                    return "同时操作错误:当向其他单元发出指令时,本地单元的传输缓冲区已满 .";
                case 52:
                    return "传输禁止错误: 无法向其他单元传输. ";
                case 53:
                    return "忙错误:在接收到指令时,正在处理其他指令. ";
                case 60:
                    return " 参数错误: 在指令中包含有无法使用的代码,或者代码没有附带区域指定参数(X,Y,D, 等以外.)指令中的代码带有非法的功能指定参数(0,1,2,等). ";
                case 61:
                    return "数据错误: 触点编号,区域编号,数据代码格式(BCD,hex等)上溢出, 下溢出以及区域指定错误.";
                case 62:
                    return "寄存器错误: 过多记录数据在未记录状态下的操作（监控记录、跟踪记录等。) 当记录溢出时，将进行重新记录。";
                case 63:
                    return "PLC 模式错误: 当一条指令发出时，运行模式不能够对指令进行处理。";
                case 65:
                    return "保护错误: 在存储保护状态下执行写操作到程序区域或系统寄存器。";
                case 66:
                    return "地址错误: 地址（程序地址、绝对地址等）数据编码形式（BCD、hex 等）、上溢、下溢或指定范围错误。";
                case 67:
                    return "丢失数据错误: 要读的数据不存在。（ 读取没有写入注释寄存区的数据。）";
                case 20:
                default:
                    return "未定义.";
            }
        }

        /// <summary>
        /// 发送PLC通信命令
        /// </summary>
        /// <param name="cmdstr">命令串</param>
        /// <param name="retStr">反馈串</param>
        /// <param name="errStr">错误信息</param>
        /// <returns>true-正确执行,false-执行错误</returns>
        private  bool SendPlcCmd(string cmdstr,ref string retStr, ref string errStr)
        {
            string ms = "";
            int i = 0;
            //if (!portOpened)
            //{
            //    errStr = "Port not Opened!";
            //    return false;
            //}

            try
            {
                int DataLength = serialPort1.BytesToRead;
                if (DataLength > 0)
                    serialPort1.DiscardInBuffer();
                serialPort1.Write(cmdstr + tobcc(cmdstr) + (char)13);
                Thread.Sleep(100);
                DataLength = serialPort1.BytesToRead;
                if (DataLength > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    byte[] ds = new byte[DataLength+1];
                    while (i < DataLength)
                    {
                        int len = serialPort1.Read(ds, 0, DataLength+1);
                        sb.Append(Encoding.ASCII.GetString(ds, 0, len));
                        i += len;
                    }
                    ms = sb.ToString();
                    if (ms.Length > 5)
                    {
                        if (ms.Substring(0, 1) == "%" && ms.Substring(3, 1) == "!")
                        {
                                i = System.Convert.ToInt16(ms.Substring(4, 2));
                                errStr = GetPlcErrInfo(i);
                                return false;
                        }
                    }
                }
                else
                {
                    errStr = "没数据返回!";
                    return false;
                }
            }
            catch (Exception ex)
            {
                errStr =  ex.Message;
                return false;
            }
            retStr = ms;
            return true;

        }

        /// <summary>
        /// 读单触点
        /// </summary>
        /// <param name="code">触点代码:X,Y,R,T,C,L</param>
        /// <param name="address">触点编号,长度为4:3个BCD+1个HEX</param>
        /// <param name="retstr">信息反馈,return为true时retstr的第7个字节为触点状态:'0'-OFF,'1'-ON</param>
        /// <param name="errstr">错误反馈</param>
        /// <returns>true-正确执行,false-执行错误</returns>
        public bool ReadContactPoint(char code,string address,ref string  retstr,ref string errstr)
        {
            string sendstr;
            sendstr = "%EE#RCS";
            sendstr +=(code + address);
            return SendPlcCmd(sendstr,ref retstr,ref errstr);
        }

        /// <summary>
        /// 读多触点
        /// </summary>
        /// <param name="addressM">触点信息:个数(n)+[触点代码(1字符)+触点编号(4字符)]+...+[触点代码(1字符)+触点编号(4字符)],总共1+5*n个字符</param>
        /// <param name="retstr">信息反馈,return为true时从retstr的第7个字节开始依次为触点状态:'0'-OFF,'1'-ON</param>
        /// <param name="errstr">错误信息</param>
        /// <returns>true-正确执行,false-执行错误</returns>
        public bool ReadMultiContactPoint( string addressM, ref string retstr, ref string errstr)
        {
            string sendstr;
            sendstr = "%EE#RCP";
            sendstr +=   addressM;
            return SendPlcCmd(sendstr, ref retstr, ref errstr);
        }

        /// <summary>
        /// 读数据
        /// </summary>
        /// <param name="code">数据代码:WX,WY,WR,WL,SV,EV,DT,LD,FL,IX,IY等</param>
        /// <param name="addstart">起始数据编码</param>
        /// <param name="addend">结束数据编码</param>
        /// <param name="retstr">反馈信息</param>
        /// <param name="errstr">错误信息</param>
        /// <returns>true-正确执行,false-执行错误</returns>
        public bool ReadWordContactPoint( string code, string addstart, string addend,ref string retstr, ref string errstr)
        {
            string sendstr;
            switch(code)
            {
                case "WX":
                case "WY":
                case "WR":
                case "WL":
                    sendstr = "%EE#RCC"+code.Substring(1,1);
                    break;
                case "DT":
                case "FL":
                case "LD":
                    sendstr = "%EE#RD"+code.Substring(0,1)+"0";
                    //addstart = "0" + addstart;
                    addend = "0" + addend;
                    break;
                case "SV":
                    sendstr = "%EE#RS";
                    break;
                case "EV":
                    sendstr = "%EE#RK";
                    break;
                case "IX":
                case "IY":
                case "ID":
                    sendstr = "%EE#RD";
                    addstart = code;//? 说明书上是一个字符，有点疑问
                    addend = "000000000";
                    break;
                default :
                    errstr = "代码参数（code ）输入错误！";
                    return false;
            }
            sendstr += ( addstart + addend);
            return SendPlcCmd(sendstr, ref retstr, ref errstr);
        }

        /// <summary>
        /// 写单触点
        /// </summary>
        /// <param name="code">触点代码:Y,R,L</param>
        /// <param name="address">触点编号,长度为4:3个BCD+1个HEX</param>
        /// <param name="value">写入值:"0"-OFF,"1"-ON</param>
        /// <param name="retstr">信息反馈</param>
        /// <param name="errstr">错误信息反馈</param>
        /// <returns>true-正确执行,false-执行错误</returns>
        public bool WriteContactPoint( char code, string address, string value,ref string retstr, ref string errstr)
        {
            string sendstr;
            sendstr = "%EE#WCS";
            if (address.Length != 4)
            {
                errstr = "参数 address输入不合法!";
                return false;
            }
            if (value != "0" && value != "1")
            {
                errstr = "参数 value输入不合法!";
                return false;         
            }
            sendstr += (code + address+value) ;
            return SendPlcCmd(sendstr, ref retstr, ref errstr);
        }

        /// <summary>
        /// 写多触点
        /// </summary>
        /// <param name="addressM">触点信息:[触点代码(1字符)+触点编号(4字符)]+...+[触点代码(1字符)+触点编号(4字符)],总共5*n个字符</param>
        /// <param name="valueM">写入值(1个字符)*n</param>
        /// <param name="retstr">命令反馈信息</param>
        /// <param name="errstr">错误反馈信息</param>
        /// <returns>true-正确执行,false-执行错误</returns>
        public bool WriteMulteContactPoint(string addressM, string valueM, ref string retstr, ref string errstr)
        {
            string sendstr;
            sendstr = "%EE#WCP";
            if (valueM.Length * 5 != addressM.Length || addressM.Length % 5 != 0)
            {
                errstr = "Parament addressM or valueM error!";
                return false;
            }
            sendstr += valueM.Length.ToString();
            for (int i=0;i<valueM.Length ;i++)
            sendstr +=  (addressM.Substring(i+5,5) + valueM.Substring(i,1));
            return SendPlcCmd(sendstr, ref retstr, ref errstr);
        }

        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="code">数据代码:WX,WY,WR,WL,SV,EV,DT,LD,FL,IX,IY等</param>
        /// <param name="addstart">起始数据编码</param>
        /// <param name="addend">结束数据编码</param>
        /// <param name="valueM">4字节BCD*n</param>
        /// <param name="retstr">反馈信息</param>
        /// <param name="errstr">错误信息</param>
        /// <returns>true-正确执行,false-执行错误</returns>
        public bool WriteWordContactPoint( string  code, string addstart, string addend, string valueM,ref string retstr, ref string errstr)
        {
            string sendstr;
            switch (code)
            {
                case "WX":
                    errstr = "外部输入信号拒绝写！";
                    return false;
                case "WY":
                case "WR":
                case "WL":
                    sendstr = "%EE#WCC" + code.Substring(1, 1);
                    break;
                case "DT":
                case "FL":
                case "LD":
                    sendstr = "%EE#WD" + code.Substring(0, 1) + "0";
                    addend = "0" + addend;
                    break;
                case "SV":
                    sendstr = "%EE#WS";
                    break;
                case "EV":
                    sendstr = "%EE#WK";
                    break;
                case "IX":
                case "IY":
                case "ID":
                    sendstr = "%EE#WD";
                    addstart = code;//? 说明书上是一个字符，有点疑问
                    addend = "000000000";
                    break;
                default:
                    errstr = "代码参数（code ）输入错误！";
                    return false;
            }

            sendstr += ( addstart + addend + valueM);
            return SendPlcCmd(sendstr, ref retstr, ref errstr);
        }

        /// <summary>
        /// 设置触点监视对象
        /// </summary>
        /// <param name="addressM">[触点代码(1字符)+触点编号(4字符)]*n,触点代码:X,Y,R,T,C,L</param>
        /// <param name="retstr">命令反馈信息</param>
        /// <param name="errstr">错误反馈信息</param>
        /// <returns>true-正确执行,false-执行错误</returns>
        public bool MonitorContactPoint(string addressM, ref string retstr, ref string errstr)
        {
            string sendstr,rets="",errs="";
            sendstr = "%EE#MCFFFFF";
            if (!SendPlcCmd(sendstr, ref rets, ref errs))
            {
                errstr = errs;
                retstr = rets;
                return false;
            }
            if (addressM.Length % 5 != 0)
            {
                errstr = "函数MonitorContactPoint（参数：addressM ） 输入有误！";
                return false;
            }
            sendstr = "%EE#MC" + addressM;
            return SendPlcCmd(sendstr, ref retstr, ref errstr);
        }

        /// <summary>
        /// 设置数据监视对象
        /// </summary>
        /// <param name="addressM">[数据代码(1字符)+数据编码(5字符BCD)]*n,触点代码:D,L,F,S,K</param>
        /// <param name="retstr">命令反馈信息</param>
        /// <param name="errstr">错误反馈信息</param>
        /// <returns>true-正确执行,false-执行错误</returns>
        public bool MonitorWordData(string addressM, ref string retstr, ref string errstr)
        {
            string sendstr, rets = "", errs = "";
            sendstr = "%EE#MDFFFFFF";
            if (!SendPlcCmd(sendstr, ref rets, ref errs))
            {
                errstr = errs;
                retstr = rets;
                return false;
            }
            if (addressM.Length % 6 != 0)
            {
                errstr = "函数MonitorWordData（参数：addressM ）输入有误！";
                return false;
            }
            sendstr = "%EE#MD" + addressM;
            return SendPlcCmd(sendstr, ref retstr, ref errstr);
        }

        /// <summary>
        /// 字符串颠倒
        /// </summary>
        /// <param name="str">输入串</param>
        /// <returns>颠倒串</returns>
        private  string ReverseString(string str)
        {
            StringBuilder strBuild = new StringBuilder();
            for (int i = str.Length - 1; i >= 0; i--)
            {
                strBuild.Append(str[i]);
            }
            return strBuild.ToString();
        } 

        /// <summary>
        /// 读取监视数据
        /// </summary>
        /// <param name="retstr">反馈串</param>
        /// <param name="errstr">错误信息</param>
        /// <returns>true-正确执行,false-执行错误</returns>
        public bool GetMonitorData(ref string retstr, ref string errstr)
        {
            string sendstr,rets="",errs="";
            sendstr = "%EE#MG";
            if(!SendPlcCmd(sendstr, ref rets, ref errs))
            {
                errstr = errs;
                retstr = rets;
                return false;
            }
            retstr = rets;
            return true;
        }
        
    }


}