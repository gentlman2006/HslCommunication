using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core.IMessage
{
    /// <summary>
    /// 用于欧姆龙通信的Fins协议的消息解析规则
    /// </summary>
    public class FinsMessage : INetMessage
    {
        /// <summary>
        /// 消息头的指令长度
        /// </summary>
        public int ProtocolHeadBytesLength
        {
            get { return 8; }
        }


        /// <summary>
        /// 从当前的头子节文件中提取出接下来需要接收的数据长度
        /// </summary>
        /// <returns>返回接下来的数据内容长度</returns>
        public int GetContentLengthByHeadBytes( )
        {
            byte[] buffer = new byte[4];
            buffer[0] = HeadBytes[7];
            buffer[1] = HeadBytes[6];
            buffer[2] = HeadBytes[5];
            buffer[3] = HeadBytes[4];
            return BitConverter.ToInt32( buffer, 0 );
        }


        /// <summary>
        /// 检查头子节的合法性
        /// </summary>
        /// <param name="token">特殊的令牌，有些特殊消息的验证</param>
        /// <returns>是否成功的结果</returns>
        public bool CheckHeadBytesLegal( byte[] token )
        {
            if (HeadBytes == null) return false;

            if (HeadBytes[0] == 0x46 && HeadBytes[1] == 0x49 && HeadBytes[2] == 0x4E && HeadBytes[3] == 0x53)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 获取头子节里的消息标识
        /// </summary>
        /// <returns>消息id</returns>
        public int GetHeadBytesIdentity( )
        {
            return 0;
        }


        /// <summary>
        /// 消息头字节
        /// </summary>
        public byte[] HeadBytes { get; set; }


        /// <summary>
        /// 消息内容字节
        /// </summary>
        public byte[] ContentBytes { get; set; }


        /// <summary>
        /// 发送的字节信息
        /// </summary>
        public byte[] SendBytes { get; set; }
    }
}
