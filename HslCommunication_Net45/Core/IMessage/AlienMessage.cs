using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core.IMessage
{

    /// <summary>
    /// 异形消息对象，用于异形客户端的注册包接收以及验证使用
    /// </summary>
    public class AlienMessage : INetMessage
    {
        /// <summary>
        /// 本协议的消息头长度
        /// </summary>
        public int ProtocolHeadBytesLength
        {
            get { return 5; }
        }


        /// <summary>
        /// 头子节信息
        /// </summary>
        public byte[] HeadBytes { get; set; }

        /// <summary>
        /// 内容字节信息
        /// </summary>
        public byte[] ContentBytes { get; set; }


        /// <summary>
        /// 检查接收的数据是否合法
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns>是否合法</returns>
        public bool CheckHeadBytesLegal( byte[] token )
        {
            if (HeadBytes == null) return false;

            if (HeadBytes[0] == 0x48 &&
                HeadBytes[1] == 0x73 &&
                HeadBytes[2] == 0x6E)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 从头子节信息中解析出接下来需要接收的数据长度
        /// </summary>
        /// <returns>接下来的数据长度</returns>
        public int GetContentLengthByHeadBytes( )
        {
            return HeadBytes[4];
        }

        /// <summary>
        /// 获取头子节里的特殊标识
        /// </summary>
        /// <returns>标识信息</returns>
        public int GetHeadBytesIdentity( )
        {
             return 0;
        }


        /// <summary>
        /// 发送的字节信息
        /// </summary>
        public byte[] SendBytes { get; set; }


    }
}
