using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HslCommunication.LogNet
{


    #region 自定义的消息


    /// <summary>
    /// 带有存储消息的事件
    /// </summary>
    public class HslEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public HslMessageItem HslMessage { get; set; }

    }


    #endregion

    #region 日志文件的输出模式

    /// <summary>
    /// 日志文件输出模式
    /// </summary>
    public enum GenerateMode
    {
        /// <summary>
        /// 按每个小时生成日志文件
        /// </summary>
        ByEveryHour = 1,
        /// <summary>
        /// 按每天生成日志文件
        /// </summary>
        ByEveryDay = 2,
        /// <summary>
        /// 按每个周生成日志文件
        /// </summary>
        ByEveryWeek = 3,
        /// <summary>
        /// 按每个月生成日志文件
        /// </summary>
        ByEveryMonth = 4,
        /// <summary>
        /// 按每季度生成日志文件
        /// </summary>
        ByEverySeason = 5,
        /// <summary>
        /// 按每年生成日志文件
        /// </summary>
        ByEveryYear = 6,
    }


    #endregion
    
    #region 消息等级

    /// <summary>
    /// 记录消息的等级
    /// </summary>
    public enum HslMessageDegree
    {
        /// <summary>
        /// 一条消息都不记录
        /// </summary>
        None = 1,
        /// <summary>
        /// 记录致命等级及以上日志的消息
        /// </summary>
        FATAL = 2,
        /// <summary>
        /// 记录异常等级及以上日志的消息
        /// </summary>
        ERROR = 3,
        /// <summary>
        /// 记录警告等级及以上日志的消息
        /// </summary>
        WARN = 4,
        /// <summary>
        /// 记录信息等级及以上日志的消息
        /// </summary>
        INFO = 5,
        /// <summary>
        /// 记录调试等级及以上日志的信息
        /// </summary>
        DEBUG = 6
    }

    #endregion

    #region MyRegion

    /// <summary>
    /// 单个日志的记录信息
    /// </summary>
    public class HslMessageItem
    {
        private static long IdNumber = 0;


        /// <summary>
        /// 默认的无参构造器
        /// </summary>
        public HslMessageItem()
        {
            Id = Interlocked.Increment(ref IdNumber);
        }

        /// <summary>
        /// 单个记录信息的标识ID，程序重新运行时清空
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// 消息的等级
        /// </summary>
        public HslMessageDegree Degree { get; set; } = HslMessageDegree.DEBUG;

        /// <summary>
        /// 线程ID
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// 消息文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 消息发生的事件
        /// </summary>
        public DateTime Time { get; set; }



    }

    #endregion


}
