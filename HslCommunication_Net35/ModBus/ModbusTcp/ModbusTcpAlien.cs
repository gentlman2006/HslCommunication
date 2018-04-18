using HslCommunication.Core.Net;
using HslCommunication.LogNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using HslCommunication.Core.IMessage;

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

                // 0x48 0x73 0x6E 0x00 0x17 0x31 0x32 0x33 0x34 0x35 0x36 0x37 0x38 0x39 0x30 0x31 0x14 0x45 0x7E 0x1A 0xA0 0xAA 0xC0 0xA8 0x00 0x01 0x17 0x10
                // +------------+ +--+ +--+ +----------------------------------------------------+ +---------------------------+ +-----------------+ +-------+
                //   固定消息头  备用 长度          DTU码 12345678901 (唯一标识)                    登录密码(不受信的排除)    Ip:192.168.0.1       端口10000

                // 返回
                // 0x48 0x73 0x6E 0x00 0x01 0x00
                // +------------+ +--+ +--+ +--+
                //   固定消息头  备用 长度 结果代码

                // 结果代码 
                // 0x00: 登录成功 
                // 0x01: DTU重复登录 
                // 0x02: DTU禁止登录
                // 0x03: 密码验证失败 
                // 0x04: DTU

                OperateResult<AlienMessage> check = ReceiveMessage( socket, 5000, new AlienMessage( ) );
                if (!check.IsSuccess) return;


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
        

    }
}
