using HslCommunication.Core.Net;
using HslCommunication.LogNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace HslCommunication.ModBus
{
    /// <summary>
    /// 异形Modbus-Tcp客户端，支持侦听来自服务器的连接，然后再请求服务器数据，支持是否启用注册包功能
    /// </summary>
    public class ModbusTcpAlien : NetworkServerBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个异形的Modbus-Tcp客户端，需要指定客户端的站号信息 
        /// </summary>
        public ModbusTcpAlien( byte station )
        {
            this.station = station;
        }

        #endregion

        /// <summary>
        /// 登录的回调方法
        /// </summary>
        /// <param name="obj">传入的异步对象</param>
        protected override void ThreadPoolLogin( object obj )
        {
            if(obj is Socket socket)
            {
                // 登录验证
                OperateResult<byte[]> read = Receive( socket, 4 );

            }
        }


        #region Object Override

        /// <summary>
        /// 获取本对象的字符串表示形式
        /// </summary>
        /// <returns></returns>
        public override string ToString( )
        {
            return base.ToString( );
        }


        #endregion
        
        #region Private Member


        private byte station = 0x01;                     // Modbus-Tcp客户端的站号
        private ILogNet logNet;                          // 日志存储


        #endregion

    }
}
