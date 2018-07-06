using System;

namespace HslCommunication.LogNet
{
    /// <summary>
    /// 一个通用的日志接口
    /// </summary>
    public interface ILogNet : IDisposable
    {
        /// <summary>
        /// 文件存储模式，1:单文件，2:根据大小，3:根据时间
        /// </summary>
        int LogSaveMode { get; }

        /// <summary>
        /// 存储之前引发的事件，允许额外的操作
        /// </summary>
        event EventHandler<HslEventArgs> BeforeSaveToFile;

        /// <summary>
        /// 自定义的消息记录
        /// </summary>
        /// <param name="degree">消息等级</param>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">日志内容</param>
        void RecordMessage(HslMessageDegree degree, string keyWord, string text);

        /// <summary>
        /// 写入一条调试日志
        /// </summary>
        /// <param name="text">日志内容</param>
        void WriteDebug(string text);

        /// <summary>
        /// 写入一条调试日志
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">日志内容</param>
        void WriteDebug(string keyWord, string text);

        /// <summary>
        /// 写入一条解释性的信息
        /// </summary>
        /// <param name="description"></param>
        void WriteDescrition(string description);

        /// <summary>
        /// 写入一条错误日志
        /// </summary>
        /// <param name="text">日志内容</param>
        void WriteError(string text);

        /// <summary>
        /// 写入一条错误日志
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">日志内容</param>
        void WriteError(string keyWord, string text);

        /// <summary>
        /// 写入一条异常信息
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="ex">异常</param>
        void WriteException(string keyWord, Exception ex);

        /// <summary>
        /// 写入一条异常信息
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">内容</param>
        /// <param name="ex">异常</param>
        void WriteException(string keyWord, string text, Exception ex);

        /// <summary>
        /// 写入一条致命日志
        /// </summary>
        /// <param name="text">日志内容</param>
        void WriteFatal(string text);

        /// <summary>
        /// 写入一条致命日志
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">日志内容</param>
        void WriteFatal(string keyWord, string text);


        /// <summary>
        /// 写入一条信息日志
        /// </summary>
        /// <param name="text">日志内容</param>
        void WriteInfo(string text);

        /// <summary>
        /// 写入一条信息日志
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">日志内容</param>
        void WriteInfo(string keyWord, string text);

        /// <summary>
        /// 写入一行换行符
        /// </summary>
        void WriteNewLine();

        /// <summary>
        /// 写入一条警告日志
        /// </summary>
        /// <param name="text">日志内容</param>
        void WriteWarn(string text);

        /// <summary>
        /// 写入一条警告日志
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">日志内容</param>
        void WriteWarn(string keyWord, string text);

        /// <summary>
        /// 设置日志的存储等级，高于该等级的才会被存储
        /// </summary>
        /// <param name="degree"></param>
        void SetMessageDegree(HslMessageDegree degree);

        /// <summary>
        /// 获取已存在的日志文件名称
        /// </summary>
        /// <returns></returns>
        string[] GetExistLogFileNames();

        /// <summary>
        /// 过滤掉指定的关键字的日志，该信息不存储，但仍然触发BeforeSaveToFile事件
        /// </summary>
        /// <param name="keyword"></param>
        void FiltrateKeyword( string keyword );
    }
}