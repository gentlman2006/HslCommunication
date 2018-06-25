using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.ModBus
{
    /// <summary>
    /// Modbus协议地址格式，可以携带站号，功能码，地址信息
    /// </summary>
    public class ModbusAddress
    {
        /// <summary>
        /// 实例化一个默认的对象
        /// </summary>
        public ModbusAddress( )
        {
            Station = -1;
            Function = ModbusInfo.ReadRegister;
            Address = 0;
        }

        /// <summary>
        /// 实例化一个默认的对象，使用默认的地址初始化
        /// </summary>
        public ModbusAddress( string address )
        {
            Station = -1;
            Function = ModbusInfo.ReadRegister;
            Address = 0;
            AnalysisAddress( address );
        }

        /// <summary>
        /// 站号信息
        /// </summary>
        public int Station { get; set; }

        /// <summary>
        /// 功能码
        /// </summary>
        public byte Function { get; set; }

        /// <summary>
        /// 起始地址
        /// </summary>
        public ushort Address { get; set; }


        /// <summary>
        /// 解析Modbus的地址码
        /// </summary>
        /// <param name="address"></param>
        public void AnalysisAddress( string address )
        {
            if (address.IndexOf( ';' ) < 0)
            {
                // 正常地址，功能码03
                Address = ushort.Parse( address );
            }
            else
            {
                // 带功能码的地址
                string[] list = address.Split( ';' );
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i][0] == 's' || list[i][0] == 'S')
                    {
                        // 站号信息
                        this.Station = byte.Parse( list[i].Substring( 2 ) );
                    }
                    else if (list[i][0] == 'x' || list[i][0] == 'X')
                    {
                        this.Function = byte.Parse( list[i].Substring(2) );
                    }
                    else
                    {
                        this.Address = ushort.Parse( list[i] );
                    }
                }
            }
        }

        /// <summary>
        /// 创建一个读取的字节对象
        /// </summary>
        /// <param name="station"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] CreateReadBytes( byte station, ushort length )
        {
            byte[] buffer = new byte[6];
            buffer[0] = this.Station < 0 ? station : (byte)this.Station;
            buffer[1] = Function;
            buffer[2] = BitConverter.GetBytes( this.Address )[1];
            buffer[3] = BitConverter.GetBytes( this.Address )[0];
            buffer[4] = BitConverter.GetBytes( length )[1];
            buffer[5] = BitConverter.GetBytes( length )[0];
            return buffer;
        }



        /// <summary>
        /// 地址新增指定的数
        /// </summary>
        /// <param name="value"></param>
        /// <returns>新增后的地址信息</returns>
        public ModbusAddress AddressAdd( int value )
        {
            return new ModbusAddress( )
            {
                Station = this.Station,
                Function = this.Function,
                Address = (ushort)(this.Address + value),
            };
        }


        /// <summary>
        /// 地址新增1
        /// </summary>
        /// <returns>新增后的地址信息</returns>
        public ModbusAddress AddressAdd( )
        {
            return AddressAdd( 1 );
        }


        /// <summary>
        /// 获取本对象的字符串表示形式
        /// </summary>
        /// <returns></returns>
        public override string ToString( )
        {
            StringBuilder sb = new StringBuilder( );
            if (Station >= 0) sb.Append( "s=" + Station + ";" );
            if (Function >= 1) sb.Append( "x=" + Function + ";" );
            sb.Append( Address.ToString( ) );

            return sb.ToString( );
        }

    }
}
