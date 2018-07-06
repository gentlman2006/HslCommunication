using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace HslCommunication.BasicFramework
{
    /// <summary>
    /// 一个简单通用的消息队列
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class SoftMsgQueue<T> : SoftFileSaveBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        public SoftMsgQueue()
        {
            LogHeaderText = "SoftMsgQueue<" + typeof(T).ToString() + ">";
        }

        #endregion


        /// <summary>
        /// 所有临时存储的数据
        /// </summary>
        private Queue<T> all_items = new Queue<T>();


        private int m_Max_Cache = 200;
        /// <summary>
        /// 临时消息存储的最大条数，必须大于10
        /// </summary>
        public int MaxCache
        {
            get { return m_Max_Cache; }
            set { if (value > 10) m_Max_Cache = value; }
        }

        /// <summary>
        /// 获取最新添加进去的数据
        /// </summary>
        public T CurrentItem
        {
            get
            {
                if (all_items.Count > 0)
                {
                    return all_items.Peek();
                }
                else
                {
                    return default(T);
                }
            }
        }

        /// <summary>
        /// 将集合进行锁定
        /// </summary>
        private object lock_queue = new object();

        /// <summary>
        /// 新增一条数据
        /// </summary>
        public void AddNewItem(T item)
        {
            lock (lock_queue)
            {
                while (all_items.Count >= m_Max_Cache)
                {
                    all_items.Dequeue();
                }
                all_items.Enqueue(item);
            }
        }
        /// <summary>
        /// 获取存储字符串
        /// </summary>
        /// <returns></returns>
        public override string ToSaveString()
        {
            return JArray.FromObject(all_items).ToString();
        }
        /// <summary>
        /// 获取加载字符串
        /// </summary>
        /// <param name="content"></param>
        public override void LoadByString(string content)
        {
            JArray array = JArray.Parse(content);
            all_items = (Queue<T>)array.ToObject(typeof(Queue<T>));
        }

    }

    /// <summary>
    /// 系统的消息类，用来发送消息，和确认消息的
    /// </summary>
    public class MessageBoard
    {
        /// <summary>
        /// 发送方名称
        /// </summary>
        public string NameSend { get; set; } = "";
        /// <summary>
        /// 接收方名称
        /// </summary>
        public string NameReceive { get; set; } = "";
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 发送的消息内容
        /// </summary>
        public string Content { get; set; } = "";
        /// <summary>
        /// 消息是否已经被查看
        /// </summary>
        public bool HasViewed { get; set; }
    }
}
