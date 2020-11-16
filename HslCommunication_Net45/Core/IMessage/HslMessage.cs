using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core.IMessage
{

    /// <summary>
    /// 本组件系统使用的默认的消息规则，说明解析和反解析规则的
    /// </summary>
    public class HslMessage : INetMessage
    {
        /// <summary>
        /// 本协议的消息头长度
        /// </summary>
        public int ProtocolHeadBytesLength
        {
            get { return 32; }
        }


        /// <summary>
        /// 头子节信息
        /// </summary>
        public byte[] HeadBytes { get ; set; }

        /// <summary>
        /// 内容字节信息
        /// </summary>
        public byte[] ContentBytes { get ; set ; }


        /// <summary>
        /// 检查接收的数据是否合法
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns>是否合法</returns>
        public bool CheckHeadBytesLegal(byte[] token)
        {
            if (HeadBytes == null) return false;

            if (HeadBytes?.Length>=32)
            {
                return BasicFramework.SoftBasic.IsTwoBytesEquel(HeadBytes, 12, token, 0, 16);
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
        public int GetContentLengthByHeadBytes()
        {
            if (HeadBytes?.Length >= 32)
            {
                return BitConverter.ToInt32(HeadBytes, 28);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取头子节里的特殊标识
        /// </summary>
        /// <returns>标识信息</returns>
        public int GetHeadBytesIdentity()
        {
            if (HeadBytes?.Length >= 32)
            {
                return BitConverter.ToInt32(HeadBytes, 4);
            }
            else
            {
                return 0;
            }
        }


        /// <summary>
        /// 发送的字节信息
        /// </summary>
        public byte[] SendBytes { get; set; }
    }
}