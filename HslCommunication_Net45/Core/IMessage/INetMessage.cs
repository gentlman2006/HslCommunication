using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core.IMessage
{
    /// <summary>
    /// 本系统的消息类，包含了各种解析规则，数据信息提取规则
    /// </summary>
    public interface INetMessage
    {
        /// <summary>
        /// 消息头的指令长度
        /// </summary>
        int ProtocolHeadBytesLength { get; }


        /// <summary>
        /// 从当前的头子节文件中提取出接下来需要接收的数据长度
        /// </summary>
        /// <returns>返回接下来的数据内容长度</returns>
        int GetContentLengthByHeadBytes();


        /// <summary>
        /// 检查头子节的合法性
        /// </summary>
        /// <param name="token">特殊的令牌，有些特殊消息的验证</param>
        /// <returns>是否成功的结果</returns>
        bool CheckHeadBytesLegal(byte[] token);


        /// <summary>
        /// 获取头子节里的消息标识
        /// </summary>
        /// <returns>消息标识</returns>
        int GetHeadBytesIdentity();


        /// <summary>
        /// 消息头字节
        /// </summary>
        byte[] HeadBytes { get; set; }


        /// <summary>
        /// 消息内容字节
        /// </summary>
        byte[] ContentBytes { get; set; }


        /// <summary>
        /// 发送的字节信息
        /// </summary>
        byte[] SendBytes { get; set; }
    }
    

}
