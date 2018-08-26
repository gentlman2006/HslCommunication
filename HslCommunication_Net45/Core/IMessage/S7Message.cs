using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core.IMessage
{

    /// <summary>
    /// 西门子S7协议的消息解析规则
    /// </summary>
    public class S7Message : INetMessage
    {
        /// <summary>
        /// 西门子头字节的长度
        /// </summary>
        public int ProtocolHeadBytesLength
        {
            get { return 4; }
        }


        /// <summary>
        /// 头子节的数据
        /// </summary>
        public byte[] HeadBytes { get; set; }


        /// <summary>
        /// 内容字节的数据
        /// </summary>
        public byte[] ContentBytes { get; set; }


        /// <summary>
        /// 检查头子节是否合法的判断
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns>是否合法的</returns>
        public bool CheckHeadBytesLegal(byte[] token)
        {
            if (HeadBytes == null) return false;

            if (HeadBytes[0] == 0x03 && HeadBytes[1] == 0x00)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 获取剩余的内容长度
        /// </summary>
        /// <returns>数据内容长度</returns>
        public int GetContentLengthByHeadBytes()
        {
            if (HeadBytes?.Length >= 4)
            {
                return HeadBytes[2] * 256 + HeadBytes[3] - 4;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取消息号，此处无效
        /// </summary>
        /// <returns>消息标识</returns>
        public int GetHeadBytesIdentity()
        {
            return 0;
        }


        /// <summary>
        /// 发送的字节信息
        /// </summary>
        public byte[] SendBytes { get; set; }
    }
}
