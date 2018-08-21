using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;







namespace HslCommunication.Core.Address
{
    /// <summary>
    /// 所有设备通信类的地址基础类
    /// </summary>
    public class DeviceAddressBase
    {
        /// <summary>
        /// 起始地址
        /// </summary>
        public ushort Address { get; set; }


        /// <summary>
        /// 解析字符串的地址
        /// </summary>
        /// <param name="address">地址信息</param>
        public virtual void AnalysisAddress( string address )
        {
            Address = ushort.Parse( address );
        }


        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串数据</returns>
        public override string ToString( )
        {
            return Address.ToString( );
        }

    }
}
