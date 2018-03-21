using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatRoom
{
    /// <summary>
    /// 扩展实现的账户信息，记录唯一标记，ip地址，上线时间，名字
    /// </summary>
    public class NetAccount
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// Ip地址
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 上线时间
        /// </summary>
        public string OnlineTime { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 字符串标识形式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "[" + Ip + "] : " + Name;
        }
    }



    /// <summary>
    /// 消息类
    /// </summary>
    public class NetMessage
    {
        /// <summary>
        /// 谁发的消息
        /// </summary>
        public string FromName { get; set; }
        /// <summary>
        /// 什么时候发的消息
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 发送了什么内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 内容的类型，一般都是string
        /// </summary>
        public string Type { get; set; }
    }
}
