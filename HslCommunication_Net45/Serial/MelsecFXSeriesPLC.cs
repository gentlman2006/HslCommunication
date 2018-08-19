using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace ApolloStudio
{
    class MelsecFXSeries
    {
        #region 报文生成
        /// <summary>
        /// 字符串按格式截取
        /// </summary>
        /// <param name="str"></param>
        /// <param name="fmt"></param>
        /// <param name="sep"></param>
        /// <returns></returns>
        public static string ExtractString(string str, string fmt, string sep)
        {
            string str_t = str;

            int index = str.IndexOf(fmt);
            if (index != -1)
            {
                index += fmt.Length;
                str_t = str_t.Substring(index, str_t.Length - index);
            }
            else
                return "";

            index = str_t.IndexOf(sep);
            if (index != -1)
                str_t = str_t.Substring(0, index);

            return str_t.Trim();
        }

        /// <summary>
        /// D区写float型数据
        /// </summary>
        /// <param name="uAddress"></param>
        /// <returns></returns>
        public static string Write_D_Float(UInt32 uAddress, float fData)
        {
            byte[] uSend = new byte[] { 0x02, 0x31, 0x30, 0x30, 0x30, 0x30, 0x30, 0x34, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x03, 0x30, 0x30 };

            uAddress = uAddress * 2 + 0x1000;
            UInt32 uTmp = uAddress & 0x000f;
            uSend[5] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uAddress >> 4) & 0x000f;
            uSend[4] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uAddress >> 8) & 0x000f;
            uSend[3] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uAddress >> 12) & 0x000f;
            uSend[2] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));

            try
            {
                byte[] be_t = BitConverter.GetBytes(fData);
                string sg = be_t[3].ToString("X2") + be_t[2].ToString("X2") + be_t[1].ToString("X2") + be_t[0].ToString("X2");
                byte[] be = Encoding.ASCII.GetBytes(sg);
                uSend[8] = be[6];
                uSend[9] = be[7];
                uSend[10] = be[4];
                uSend[11] = be[5];
                uSend[12] = be[2];
                uSend[13] = be[3];
                uSend[14] = be[0];
                uSend[15] = be[1];
            }
            catch (Exception e) { throw e; }

            UInt32 uSum = 0;
            for (int i = 1; i < 17; i++)
                uSum = uSum + (byte)uSend[i];
            uTmp = uSum & 0x000f;
            uSend[18] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uSum >> 4) & 0x000f;
            uSend[17] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));

            return Encoding.ASCII.GetString(uSend);
        }

        /// <summary>
        /// D区写int型数据
        /// </summary>
        /// <param name="uAddress"></param>
        /// <returns></returns>
        public static string Write_D_Int(UInt32 uAddress, int iData)
        {
            byte[] uSend = new byte[] { 0x02, 0x31, 0x30, 0x30, 0x30, 0x30, 0x30, 0x34, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x03, 0x30, 0x30 };

            uAddress = uAddress * 2 + 0x1000;
            UInt32 uTmp = uAddress & 0x000f;
            uSend[5] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uAddress >> 4) & 0x000f;
            uSend[4] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uAddress >> 8) & 0x000f;
            uSend[3] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uAddress >> 12) & 0x000f;
            uSend[2] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));

            try
            {
                byte[] be = Encoding.ASCII.GetBytes(iData.ToString("X8"));
                uSend[8] = be[6];
                uSend[9] = be[7];
                uSend[10] = be[4];
                uSend[11] = be[5];
                uSend[12] = be[2];
                uSend[13] = be[3];
                uSend[14] = be[0];
                uSend[15] = be[1];
            }
            catch (Exception e) { throw e; }

            UInt32 uSum = 0;
            for (int i = 1; i < 17; i++)
                uSum = uSum + (byte)uSend[i];
            uTmp = uSum & 0x000f;
            uSend[18] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uSum >> 4) & 0x000f;
            uSend[17] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));

            return Encoding.ASCII.GetString(uSend);
        }

        /// <summary>
        /// D区写short型数据
        /// </summary>
        /// <param name="uAddress"></param>
        /// <returns></returns>
        public static string Write_D_Short(UInt32 uAddress, short sData)
        {
            byte[] uSend = new byte[] { 0x02, 0x31, 0x30, 0x30, 0x30, 0x30, 0x30, 0x32, 0x30, 0x30, 0x30, 0x30, 0x03, 0x30, 0x30 };

            uAddress = uAddress * 2 + 0x1000;
            UInt32 uTmp = uAddress & 0x000f;
            uSend[5] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uAddress >> 4) & 0x000f;
            uSend[4] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uAddress >> 8) & 0x000f;
            uSend[3] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uAddress >> 12) & 0x000f;
            uSend[2] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));

            try
            {
                byte[] be = Encoding.ASCII.GetBytes(sData.ToString("X4"));
                uSend[8] = be[2];
                uSend[9] = be[3];
                uSend[10] = be[0];
                uSend[11] = be[1];
            }
            catch (Exception e) { throw e; }

            UInt32 uSum = 0;
            for (int i = 1; i < 13; i++)
                uSum = uSum + (byte)uSend[i];
            uTmp = uSum & 0x000f;
            uSend[14] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uSum >> 4) & 0x000f;
            uSend[13] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));

            return Encoding.ASCII.GetString(uSend);
        }

        /// <summary>
        /// 开关量置位（true）/复位（false）
        /// </summary>
        /// <returns></returns>
        public static string ForceOnOff(string uArea, UInt32 uAddress, bool OnOff)
        {
            byte[] uSend;
            if (OnOff)
                uSend = new byte[] { 0x02, 0x37, 0x30, 0x30, 0x30, 0x30, 0x03, 0x30, 0x30 };
            else
                uSend = new byte[] { 0x02, 0x38, 0x30, 0x30, 0x30, 0x30, 0x03, 0x30, 0x30 };

            uint uAddress_y = uAddress;
            if (uArea == "M")
                uAddress_y = uAddress_y % 8;
            else if (uArea == "S")
                uAddress_y = uAddress_y % 8;
            else if (uArea == "X")
            {
                uAddress_y = uAddress_y - (uAddress_y / 100) * 20;
                uAddress_y = uAddress_y - (uAddress_y / 10) * 2;
                uAddress_y = uAddress_y % 8;
            }
            else if (uArea == "Y")
            {
                uAddress_y = uAddress_y - (uAddress_y / 100) * 20;
                uAddress_y = uAddress_y - (uAddress_y / 10) * 2;
                uAddress_y = uAddress_y % 8;
            }

            if (uArea == "M")
                uAddress = uAddress / 8 + 0x0100;
            else if (uArea == "S")
                uAddress = uAddress / 8 + 0x0000;
            else if (uArea == "X")
            {
                uAddress = uAddress - (uAddress / 100) * 20;
                uAddress = uAddress - (uAddress / 10) * 2;
                uAddress = uAddress / 8 + 0x0080;
            }
            else if (uArea == "Y")
            {
                uAddress = uAddress - (uAddress / 100) * 20;
                uAddress = uAddress - (uAddress / 10) * 2;
                uAddress = uAddress / 8 + 0x00A0;
            }
            uAddress = uAddress * 8 + uAddress_y;

            UInt32 uTmp = uAddress & 0x000f;
            uSend[3] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uAddress >> 4) & 0x000f;
            uSend[2] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uAddress >> 8) & 0x000f;
            uSend[5] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uAddress >> 12) & 0x000f;
            uSend[4] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));

            UInt32 uSum = 0;
            for (int i = 1; i < 7; i++)
                uSum = uSum + (byte)uSend[i];
            uTmp = uSum & 0x000f;
            uSend[8] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uSum >> 4) & 0x000f;
            uSend[7] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));

            return Encoding.ASCII.GetString(uSend);
        }

        /// <summary>
        /// 生成三菱PLC读取命令
        /// </summary>
        /// <param name="uArea">区域</param>
        /// <param name="uAddress">地址</param>
        /// <param name="uCount">涉及到的地址的数量</param>
        /// <returns></returns>
        public static string GenerateMitsubishiPlcReadCommand(string uArea, UInt32 uAddress, UInt32 uCount)
        {
            byte[] uSend = new byte[] { 0x02, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x03, 0x30, 0x30 };

            if (uArea == "D")
                uCount = uCount * 2;
            else if (uArea == "M") { }
            else if (uArea == "S") { }
            else if (uArea == "X") { }
            else if (uArea == "Y") { }
            if ((uCount / 16) >= 10)
                uSend[6] = (byte)(uCount / 16 + 0x41 - 10);
            else
                uSend[6] = (byte)(uCount / 16 + 0x30);
            if ((uCount % 16) >= 10)
                uSend[7] = (byte)((uCount % 16) + 0x41 - 10);
            else
                uSend[7] = (byte)((uCount % 16) + 0x30);

            if (uArea == "D")
                uAddress = uAddress * 2 + 0x1000;
            else if (uArea == "M")
                uAddress = uAddress / 8 + 0x0100;
            else if (uArea == "S")
                uAddress = uAddress / 8 + 0x0000;
            else if (uArea == "X")
            {
                uAddress = uAddress - (uAddress / 100) * 20;
                uAddress = uAddress - (uAddress / 10) * 2;
                uAddress = uAddress / 8 + 0x0080;
            }
            else if (uArea == "Y")
            {
                uAddress = uAddress - (uAddress / 100) * 20;
                uAddress = uAddress - (uAddress / 10) * 2;
                uAddress = uAddress / 8 + 0x00A0;
            }
            UInt32 uTmp = uAddress & 0x000f;
            uSend[5] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uAddress >> 4) & 0x000f;
            uSend[4] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uAddress >> 8) & 0x000f;
            uSend[3] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uAddress >> 12) & 0x000f;
            uSend[2] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));

            UInt32 uSum = 0;
            for (int i = 1; i < 9; i++)
                uSum = uSum + (byte)uSend[i];
            uTmp = uSum & 0x000f;
            uSend[10] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uSum >> 4) & 0x000f;
            uSend[9] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));

            return Encoding.ASCII.GetString(uSend);
        }

        /// <summary>
        /// 三菱PLC读D区short型数据
        /// </summary>
        /// <param name="uData"></param>
        /// <returns></returns>
        public static short DecodeMitsubishiPlcData_D_Short(byte[] uData)
        {
            try
            {
                byte[] be = new byte[] { uData[3], uData[4], uData[1], uData[2] };
                return Convert.ToInt16(Encoding.ASCII.GetString(be), 16);
            }
            catch { return 0; }
        }

        /// <summary>
        /// 三菱PLC读D区int型数据
        /// </summary>
        /// <param name="uData"></param>
        /// <returns></returns>
        public static int DecodeMitsubishiPlcData_D_Int(byte[] uData)
        {
            try
            {
                byte[] be = new byte[] { uData[7], uData[8], uData[5], uData[6], uData[3], uData[4], uData[1], uData[2] };
                return Convert.ToInt32(Encoding.ASCII.GetString(be), 16);
            }
            catch { return 0; }
        }

        /// <summary>
        /// 三菱PLC读D区float型数据
        /// </summary>
        /// <param name="uData"></param>
        /// <returns></returns>
        public static float DecodeMitsubishiPlcData_D_Float(byte[] uData)
        {
            try
            {
                byte[] be = new byte[] { uData[7], uData[8], uData[5], uData[6], uData[3], uData[4], uData[1], uData[2] };
                string sg = Encoding.ASCII.GetString(be);
                byte[] be_t = new byte[4];
                for (int i = 0; i < 4; i++)
                    be_t[i] = Convert.ToByte(sg.Substring((3 - i) * 2, 2), 16);
                return BitConverter.ToSingle(be_t, 0);
            }
            catch { return 0; }
        }

        /// <summary>
        /// 三菱PLC读D区string型数据
        /// </summary>
        /// <param name="uData"></param>
        /// <param name="uCount"></param>
        /// <returns></returns>
        public static string DecodeMitsubishiPlcData_D_String(byte[] uData, UInt32 uCount)
        {
            try
            {
                string sg = "";
                for (int i = 1; i <= uCount; i++)
                {
                    byte[] be = new byte[] { uData[i * 4 - 1], uData[i * 4], uData[i * 4 - 3], uData[i * 4 - 2] };
                    byte[] be_t = new byte[] { Convert.ToByte(Encoding.ASCII.GetString(be), 16) };
                    sg += Encoding.ASCII.GetString(be_t);
                }
                return sg;
            }
            catch { return ""; }
        }

        /// <summary>
        /// 三菱PLC读M、S、X、Y区数据（均为bool型）
        /// </summary>
        /// <param name="uData"></param>
        /// <param name="uArea"></param>
        /// <param name="uAddress"></param>
        /// <returns></returns>
        public static bool DecodeMitsubishiPlcData_M_S_X_Y(byte[] uData, string uArea, UInt32 uAddress)
        {
            try
            {
                byte[] be = new byte[] { uData[1], uData[2] };
                string str = Convert.ToString(Convert.ToByte(Encoding.ASCII.GetString(be), 16), 2);
                while (true)
                {
                    if (str.Length >= 8)
                        break;
                    str = "0" + str;
                }
                if (uArea == "M")
                    uAddress = uAddress % 8;
                else if (uArea == "S")
                    uAddress = uAddress % 8;
                else if (uArea == "X")
                {
                    uAddress = uAddress - (uAddress / 100) * 20;
                    uAddress = uAddress - (uAddress / 10) * 2;
                    uAddress = uAddress % 8;
                }
                else if (uArea == "Y")
                {
                    uAddress = uAddress - (uAddress / 100) * 20;
                    uAddress = uAddress - (uAddress / 10) * 2;
                    uAddress = uAddress % 8;
                }
                return str.Substring(7 - (int)uAddress, 1) == "1" ? true : false;
            }
            catch { return false; }
        }

        #endregion

        #region 数据操作
        /// <summary>
        /// 写入FX PLC数据
        /// </summary>
        /// <param name="sp">串口实例</param>
        /// <param name="dataType">写入数据地址类型（M,S,X,Y,D）</param>
        /// <param name="address">偏移地址</param>
        /// <param name="value">写入的数据值</param>
        /// <param name="valueType">写入的数据值类型</param>
        /// <returns></returns>
        public static bool FX_Series_Write(SerialPort sp, string dataType, uint address, object value, string valueType)
        {
            if (!sp.IsOpen) return false;
            try
            {
                if (dataType == "M" || dataType == "X" || dataType == "Y" || dataType == "S")
                {
                    sp.Write(ForceOnOff(dataType, address, (bool)value));
                    Thread.Sleep(20);
                    int r = sp.ReadByte();
                    if (r == 6)//成功
                        return true;
                    else if (r == 21)//拒收
                        return false;
                    else//无效
                        return false;
                }
                else if (dataType == "D")
                {
                    if (valueType == "float")
                    {
                        sp.Write(Write_D_Float(address, (float)value));
                        Thread.Sleep(20);
                        int r = sp.ReadByte();
                        if (r == 6)//成功
                            return true;
                        else if (r == 21)//拒收
                            return false;
                        else//无效
                            return false;
                    }
                    else if (valueType == "int")
                    {
                        sp.Write(Write_D_Int(address, (int)value));
                        Thread.Sleep(20);
                        int r = sp.ReadByte();
                        if (r == 6)//成功
                            return true;
                        else if (r == 21)//拒收
                            return false;
                        else//无效
                            return false;
                    }
                    else if (valueType == "short")
                    {
                        sp.Write(Write_D_Short(address, (short)value));
                        Thread.Sleep(20);
                        int r = sp.ReadByte();
                        if (r == 6)//成功
                            return true;
                        else if (r == 21)//拒收
                            return false;
                        else//无效
                            return false;
                    }
                    else return false;
                }
                else return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 读取FX PLC数据
        /// </summary>
        /// <param name="sp">串口实例</param>
        /// <param name="dataType">读取数据地址类型（M,S,X,Y,D）</param>
        /// <param name="address">偏移地址</param>
        /// <param name="readLength">读取的数据长度</param>
        /// <param name="valueType">读取的值类型</param>
        /// <returns></returns>
        public static object FX_Series_Read(SerialPort sp, string dataType, uint address, uint readLength, string valueType)
        {
            if (!sp.IsOpen) return null;
            try
            {
                if (dataType == "M" || dataType == "X" || dataType == "Y" || dataType == "S")
                {
                    sp.Write(GenerateMitsubishiPlcReadCommand(dataType, address, 1));
                    byte[] br = new byte[144];
                    int b = 0, i = 0;
                    while (true)
                    {
                        b = sp.ReadByte();
                        if (b == 3)
                        {
                            sp.ReadByte();
                            sp.ReadByte();
                            break;
                        }
                        br[i] = (byte)b;
                        i++;
                    }
                    if (br[0] == 2)
                    {
                        return DecodeMitsubishiPlcData_M_S_X_Y(br, dataType, address);
                    }
                    else return null;
                }
                else if (dataType == "D")
                {
                    if (valueType == "int" || valueType == "float") sp.Write(GenerateMitsubishiPlcReadCommand(dataType, address, 2));
                    else if (valueType == "short") sp.Write(GenerateMitsubishiPlcReadCommand(dataType, address, 1));
                    else if (valueType == "string") sp.Write(GenerateMitsubishiPlcReadCommand(dataType, address, readLength));
                    byte[] br = new byte[144];
                    int b = 0, i = 0;
                    while (true)
                    {
                        b = sp.ReadByte();
                        if (b == 3)
                        {
                            sp.ReadByte();
                            sp.ReadByte();
                            break;
                        }
                        br[i] = (byte)b;
                        i++;
                    }
                    if (br[0] == 2)
                    {
                        if (valueType == "int") return DecodeMitsubishiPlcData_D_Int(br);
                        else if (valueType == "float") return DecodeMitsubishiPlcData_D_Float(br);
                        else if (valueType == "short") return DecodeMitsubishiPlcData_D_Short(br);
                        else if (valueType == "string") return DecodeMitsubishiPlcData_D_String(br, readLength);
                        else return null;
                    }
                    else return null;
                }
                else return null;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
