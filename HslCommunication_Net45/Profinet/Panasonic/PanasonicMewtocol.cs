using HslCommunication.Core;
using HslCommunication.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.Panasonic
{
    /// <summary>
    /// 松下PLC的数据交互协议，采用Mewtocol协议通讯
    /// </summary>
    [Obsolete("还没有完成，无法使用")]
    public class PanasonicMewtocol : SerialDeviceBase<RegularByteTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化一个默认的松下PLC通信对象，默认站号为1
        /// </summary>
        /// <param name="station">站号信息，默认为1</param>
        public PanasonicMewtocol( int station = 1 )
        {

        }

        #endregion

        #region Public Properties
        
        /// <summary>
        /// 设备的目标站号
        /// </summary>
        public ushort Station { get; set; }

        #endregion

        #region ReadBool

        /// <summary>
        /// X1,X10
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public OperateResult<bool[]> ReadBool(string address, ushort length )
        {
            return new OperateResult<bool[]>( );
        }

        #endregion

        #region Bulid Read Command

        private OperateResult<byte[]> BuildReadBool(string address, ushort length )
        {
            StringBuilder sb = new StringBuilder( );
            sb.Append( "%" );
            sb.Append( Station.ToString( "D2" ) );
            sb.Append( "$" );
            sb.Append( "RCP" );


            return OperateResult.CreateSuccessResult( Encoding.ASCII.GetBytes( sb.ToString( ) ) );
        }

        /// <summary>
        /// 解析数据地址，解析出地址类型，起始地址，DB块的地址
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns>解析出地址类型，起始地址，DB块的地址</returns>
        private static OperateResult<string, ushort> AnalysisAddress( string address )
        {
            var result = new OperateResult<string, ushort>( );
            try
            {
                result.Content2 = 0;
                if (address[0] == 'X' || address[0] == 'x')
                {
                    result.Content1 = "X";
                    result.Content2 = ushort.Parse( address.Substring( 1 ) );
                }
                else if(address[0] == 'Y' || address[0] == 'y')
                {
                    result.Content1 = "Y";
                    result.Content2 = ushort.Parse( address.Substring( 1 ) );
                }
                else if (address[0] == 'R' || address[0] == 'r')
                {
                    result.Content1 = "R";
                    result.Content2 = ushort.Parse( address.Substring( 1 ) );
                }
                else if (address[0] == 'T' || address[0] == 't')
                {
                    result.Content1 = "T";
                    result.Content2 = ushort.Parse( address.Substring( 1 ) );
                }
                else if (address[0] == 'C' || address[0] == 'c')
                {
                    result.Content1 = "C";
                    result.Content2 = ushort.Parse( address.Substring( 1 ) );
                }
                else if (address[0] == 'L' || address[0] == 'l')
                {
                    result.Content1 = "L";
                    result.Content2 = ushort.Parse( address.Substring( 1 ) );
                }
                else if (address[0] == 'D' || address[0] == 'D')
                {
                    result.Content1 = "D";
                    result.Content2 = ushort.Parse( address.Substring( 1 ) );
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }

            result.IsSuccess = true;
            return result;
        }

        #endregion

    }
}
