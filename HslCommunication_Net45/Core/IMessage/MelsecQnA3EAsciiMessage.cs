using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core.IMessage
{


    /// <summary>
    /// 基于MC协议的Qna兼容3E帧协议的ASCII通讯消息机制
    /// </summary>
    public class MelsecQnA3EAsciiMessage : INetMessage
    {

        /// <summary>
        /// 消息头的指令长度
        /// </summary>
        public int ProtocolHeadBytesLength
        {
            get
            {
                return 18;
            }
        }


        /// <summary>
        /// 从当前的头子节文件中提取出接下来需要接收的数据长度
        /// </summary>
        /// <returns>返回接下来的数据内容长度</returns>
        public int GetContentLengthByHeadBytes( )
        {
            byte[] buffer = new byte[4];
            buffer[0] = HeadBytes[14];
            buffer[1] = HeadBytes[15];
            buffer[2] = HeadBytes[16];
            buffer[3] = HeadBytes[17];
            return Convert.ToInt32( Encoding.ASCII.GetString( buffer ), 16 );
        }


        /// <summary>
        /// 检查头子节的合法性
        /// </summary>
        /// <param name="token">特殊的令牌，有些特殊消息的验证</param>
        /// <returns>是否成功的结果</returns>
        public bool CheckHeadBytesLegal( byte[] token )
        {
            if (HeadBytes == null) return false;

            if (HeadBytes[0] == (byte)'D' && HeadBytes[1] == (byte)'0' && HeadBytes[2] == (byte)'0' && HeadBytes[3] == (byte)'0')
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
        /// <returns>消息标识</returns>
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
