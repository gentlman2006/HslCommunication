package HslCommunication.Profinet.Melsec;

import HslCommunication.Core.Types.OperateResultExTwo;
import HslCommunication.Utilities;

/**
 * 所有三菱通讯类的通用辅助工具类，包含了一些通用的静态方法，可以使用本类来获取一些原始的报文信息。详细的操作参见例子
 */
public class MelsecHelper {

    /**
     * 解析A1E协议数据地址
     * @param address 数据地址
     * @return 解析值
     */
    public static OperateResultExTwo<MelsecA1EDataType, Short> McA1EAnalysisAddress( String address )
    {
        OperateResultExTwo<MelsecA1EDataType, Short> result = new OperateResultExTwo<MelsecA1EDataType, Short>();
        try {
            switch (address.charAt(0)) {
                case 'X':
                case 'x': {
                    result.Content1 = MelsecA1EDataType.X;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecA1EDataType.X.getFromBase());
                    break;
                }
                case 'Y':
                case 'y': {
                    result.Content1 = MelsecA1EDataType.Y;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecA1EDataType.Y.getFromBase());
                    break;
                }
                case 'M':
                case 'm': {
                    result.Content1 = MelsecA1EDataType.M;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecA1EDataType.M.getFromBase());
                    break;
                }
                case 'S':
                case 's': {
                    result.Content1 = MelsecA1EDataType.S;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecA1EDataType.S.getFromBase());
                    break;
                }
                case 'D':
                case 'd': {
                    result.Content1 = MelsecA1EDataType.D;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecA1EDataType.D.getFromBase());
                    break;
                }
                case 'R':
                case 'r': {
                    result.Content1 = MelsecA1EDataType.R;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecA1EDataType.R.getFromBase());
                    break;
                }
                default:
                    throw new Exception("输入的类型不支持，请重新输入");
            }
        } catch (Exception ex) {
            result.Message = "地址格式填写错误：" + ex.getMessage();
            return result;
        }

        result.IsSuccess = true;
        return result;
    }

    /**
     * 解析数据地址
     * @param address 数据地址
     * @return 解析值
     */
    public static OperateResultExTwo<MelsecMcDataType, Short> McAnalysisAddress( String address )
    {
        OperateResultExTwo<MelsecMcDataType, Short> result = new OperateResultExTwo<MelsecMcDataType, Short>();
        try {
            switch (address.charAt(0)) {
                case 'M':
                case 'm': {
                    result.Content1 = MelsecMcDataType.M;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.M.getFromBase());
                    break;
                }
                case 'X':
                case 'x': {
                    result.Content1 = MelsecMcDataType.X;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.X.getFromBase());
                    break;
                }
                case 'Y':
                case 'y': {
                    result.Content1 = MelsecMcDataType.Y;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.Y.getFromBase());
                    break;
                }
                case 'D':
                case 'd': {
                    result.Content1 = MelsecMcDataType.D;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.D.getFromBase());
                    break;
                }
                case 'W':
                case 'w': {
                    result.Content1 = MelsecMcDataType.W;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.W.getFromBase());
                    break;
                }
                case 'L':
                case 'l': {
                    result.Content1 = MelsecMcDataType.L;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.L.getFromBase());
                    break;
                }
                case 'F':
                case 'f': {
                    result.Content1 = MelsecMcDataType.F;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.F.getFromBase());
                    break;
                }
                case 'V':
                case 'v': {
                    result.Content1 = MelsecMcDataType.V;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.V.getFromBase());
                    break;
                }
                case 'B':
                case 'b': {
                    result.Content1 = MelsecMcDataType.B;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.B.getFromBase());
                    break;
                }
                case 'R':
                case 'r': {
                    result.Content1 = MelsecMcDataType.R;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.R.getFromBase());
                    break;
                }
                case 'S':
                case 's': {
                    result.Content1 = MelsecMcDataType.S;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.S.getFromBase());
                    break;
                }
                case 'Z':
                case 'z': {
                    result.Content1 = MelsecMcDataType.Z;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.Z.getFromBase());
                    break;
                }
                case 'T':
                case 't': {
                    result.Content1 = MelsecMcDataType.T;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.T.getFromBase());
                    break;
                }
                case 'C':
                case 'c': {
                    result.Content1 = MelsecMcDataType.C;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecMcDataType.C.getFromBase());
                    break;
                }
                default:
                    throw new Exception("输入的类型不支持，请重新输入");
            }
        } catch (Exception ex) {
            result.Message = "地址格式填写错误：" + ex.getMessage();
            return result;
        }

        result.IsSuccess = true;
        return result;
    }


    /**
     * 从字节构建一个ASCII格式的地址字节
     * @param value 字节信息
     * @return ASCII格式的地址
     */
    public static byte[] BuildBytesFromData(byte value )
    {
        return Utilities.getBytes(String.format("%02x",value),"ASCII");
    }


    /**
     * 从short数据构建一个ASCII格式地址字节
     * @param value short值
     * @return ASCII格式的地址
     */
    public static byte[] BuildBytesFromData( short value )
    {
        return Utilities.getBytes(String.format("%04x",value),"ASCII");
    }

    /**
     * 从int数据构建一个ASCII格式地址字节
     * @param value int值
     * @return ASCII格式的地址
     */
    public static byte[] BuildBytesFromData( int value )
    {
        return Utilities.getBytes(String.format("%04x",value),"ASCII");
    }


    /**
     * 从三菱的地址中构建MC协议的6字节的ASCII格式的地址
     * @param address 三菱地址
     * @param type 三菱的数据类型
     * @return 6字节的ASCII格式的地址
     */
    public static byte[] BuildBytesFromAddress( int address, MelsecMcDataType type )
    {
        return Utilities.getBytes(String.format(type.getFromBase() == 10 ? "%06d" : "%06x",address),"ASCII");
    }


    /**
     * 从字节数组构建一个ASCII格式的地址字节
     * @param value 字节信息
     * @return ASCII格式的地址
     */
    public static byte[] BuildBytesFromData( byte[] value )
    {
        byte[] buffer = new byte[value.length * 2];
        for (int i = 0; i < value.length; i++)
        {
            byte[] data = BuildBytesFromData( value[i] );
            buffer[2*i+0] = data[0];
            buffer[2*i+1] = data[1];
        }
        return buffer;
    }


    /**
     * 将0，1，0，1的字节数组压缩成三菱格式的字节数组来表示开关量的
     * @param value 原始的数据字节
     * @return 压缩过后的数据字节
     */
    public static byte[] TransBoolArrayToByteData( byte[] value )
    {
        int length = value.length % 2 == 0 ? value.length / 2 : (value.length / 2) + 1;
        byte[] buffer = new byte[length];

        for (int i = 0; i < length; i++)
        {
            if (value[i * 2 + 0] != 0x00) buffer[i] += 0x10;
            if ((i * 2 + 1) < value.length)
            {
                if (value[i * 2 + 1] != 0x00) buffer[i] += 0x01;
            }
        }

        return buffer;
    }


    /**
     * 计算Fx协议指令的和校验信息
     * @param data 字节数据
     * @return 校验之后的数据
     */
    public static byte[] FxCalculateCRC( byte[] data )
    {
        int sum = 0;
        for (int i = 1; i < data.length - 2; i++)
        {
            sum += data[i];
        }
        return BuildBytesFromData( (byte)sum );
    }


    /**
     * 检查指定的和校验是否是正确的
     * @param data 字节数据
     * @return 是否成功
     */
    public static boolean CheckCRC( byte[] data )
    {
        byte[] crc = FxCalculateCRC( data );
        if (crc[0] != data[data.length - 2]) return false;
        if (crc[1] != data[data.length - 1]) return false;
        return true;
    }

}
