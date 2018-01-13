using HslCommunication.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace HslCommunication.LogNet
{
    /// <summary>
    /// 日志存储类的基类，提供一些基础的服务
    /// </summary>    
    public abstract class LogNetBase : IDisposable
    {
        #region 私有字段


        private HslMessageDegree m_messageDegree = HslMessageDegree.DEBUG;

        private Queue<HslMessageItem> m_WaitForSave = new Queue<HslMessageItem>();

        private SimpleHybirdLock m_simpleHybirdLock = new SimpleHybirdLock();
        /// <summary>
        /// 文件读写锁
        /// </summary>
        protected SimpleHybirdLock m_fileSaveLock = new SimpleHybirdLock();

        private int m_SaveStatus = 0;



        #endregion

        #region 事件存储处

        /// <summary>
        /// 在存储到文件的时候将会触发的事件
        /// </summary>
        public event EventHandler<HslEventArgs> BeforeSaveToFile = null;

        private void OnBeforeSaveToFile(HslEventArgs args)
        {
            BeforeSaveToFile?.Invoke(this, args);
        }

        #endregion

        #region 公开的属性

        /// <summary>
        /// 日志存储模式，1:单文件，2:按大小存储，3:按时间存储
        /// </summary>
        public int LogSaveMode { get; protected set; }



        #endregion

        #region 消息记录方法

        /// <summary>
        /// 写入一条调试信息
        /// </summary>
        /// <param name="text"></param>
        public void WriteDebug(string text)
        {
            RecordMessage(HslMessageDegree.DEBUG, text);
        }

        /// <summary>
        /// 写入一条调试信息
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">文本内容</param>
        public void WriteDebug(string keyWord, string text)
        {
            if (string.IsNullOrEmpty(keyWord))
            {
                WriteDebug(text);
            }
            else
            {
                WriteDebug(keyWord + " : " + text);
            }
        }

        /// <summary>
        /// 写入一条普通信息
        /// </summary>
        /// <param name="text">文本内容</param>
        public void WriteInfo(string text)
        {
            RecordMessage(HslMessageDegree.INFO, text);
        }

        /// <summary>
        /// 写入一条普通信息
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">文本内容</param>
        public void WriteInfo(string keyWord, string text)
        {
            if (string.IsNullOrEmpty(keyWord))
            {
                WriteInfo(text);
            }
            else
            {
                WriteInfo(keyWord + " : " + text);
            }
        }

        /// <summary>
        /// 写入一条警告信息
        /// </summary>
        /// <param name="text">文本内容</param>
        public void WriteWarn(string text)
        {
            RecordMessage(HslMessageDegree.WARN, text);
        }

        /// <summary>
        /// 写入一条警告信息
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">文本内容</param>
        public void WriteWarn(string keyWord, string text)
        {
            if (string.IsNullOrEmpty(keyWord))
            {
                WriteWarn(text);
            }
            else
            {
                WriteWarn(keyWord + " : " + text);
            }
        }


        /// <summary>
        /// 写入一条错误消息
        /// </summary>
        /// <param name="text">文本内容</param>
        public void WriteError(string text)
        {
            RecordMessage(HslMessageDegree.ERROR, text);
        }

        /// <summary>
        /// 写入一条错误消息
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">文本内容</param>
        public void WriteError(string keyWord, string text)
        {
            if (string.IsNullOrEmpty(keyWord))
            {
                WriteError(text);
            }
            else
            {
                WriteError(keyWord + " : " + text);
            }
        }

        /// <summary>
        /// 写入一条致命错误信息
        /// </summary>
        /// <param name="text">文本内容</param>
        public void WriteFatal(string text)
        {
            RecordMessage(HslMessageDegree.FATAL, text);
        }


        /// <summary>
        /// 写入一条致命错误信息
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">文本内容</param>
        public void WriteFatal(string keyWord, string text)
        {
            if (string.IsNullOrEmpty(keyWord))
            {
                WriteFatal(text);
            }
            else
            {
                WriteFatal(keyWord + " : " + text);
            }
        }

        /// <summary>
        /// 写入一条异常信息
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="ex"></param>
        public void WriteException(string keyWord, Exception ex)
        {
            RecordMessage(HslMessageDegree.FATAL, LogNetManagment.GetSaveStringFromException(keyWord, ex));
        }

        /// <summary>
        /// 写入一条异常信息
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">内容</param>
        /// <param name="ex">异常</param>
        public void WriteException(string keyWord, string text, Exception ex)
        {
            if (string.IsNullOrEmpty(keyWord))
            {
                WriteException(text, ex);
            }
            else
            {
                WriteException(keyWord + " : " + text, ex);
            }
        }

        /// <summary>
        /// 记录一条自定义的消息
        /// </summary>
        /// <param name="degree"></param>
        /// <param name="text"></param>
        public void RecordMessage(HslMessageDegree degree, string text)
        {
            WriteToFile(degree, text);
        }

        /// <summary>
        /// 写入一条解释性的消息，不需要带有回车键
        /// </summary>
        /// <param name="description"></param>
        public void WriteDescrition(string description)
        {
            if (string.IsNullOrEmpty(description)) return;

            // 和上面的文本之间追加一行空行
            StringBuilder stringBuilder = new StringBuilder("\u0002");
            stringBuilder.Append(Environment.NewLine);
            stringBuilder.Append("\u0002/");

            int count = 118 - CalculateStringOccupyLength(description);
            if (count >= 8)
            {
                int count_1 = (count - 8) / 2;
                AppendCharToStringBuilder(stringBuilder, '*', count_1);
                stringBuilder.Append("   ");
                stringBuilder.Append(description);
                stringBuilder.Append("   ");
                if (count % 2 == 0)
                {
                    AppendCharToStringBuilder(stringBuilder, '*', count_1);
                }
                else
                {
                    AppendCharToStringBuilder(stringBuilder, '*', count_1 + 1);
                }
            }
            else if (count >= 2)
            {
                int count_1 = (count - 2) / 2;
                AppendCharToStringBuilder(stringBuilder, '*', count_1);
                stringBuilder.Append(description);
                if (count % 2 == 0)
                {
                    AppendCharToStringBuilder(stringBuilder, '*', count_1);
                }
                else
                {
                    AppendCharToStringBuilder(stringBuilder, '*', count_1 + 1);
                }
            }
            else
            {
                stringBuilder.Append(description);
            }

            stringBuilder.Append("/");

            stringBuilder.Append(Environment.NewLine);

            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolSaveText), stringBuilder.ToString());
        }




        /// <summary>
        /// 写入一条换行符
        /// </summary>
        public void WriteNewLine()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolSaveText), "\u0002" + Environment.NewLine);
        }

        /// <summary>
        /// 设置日志的存储等级，高于该等级的才会被存储
        /// </summary>
        /// <param name="degree"></param>
        public void SetMessageDegree(HslMessageDegree degree)
        {
            m_messageDegree = degree;
        }

        #endregion

        #region 文件读写块




        private void WriteToFile(HslMessageDegree degree, string text)
        {
            // 过滤事件
            if (degree <= m_messageDegree)
            {
                // 需要记录数据
                HslMessageItem item = GetHslMessageItem(degree, text);

                HslEventArgs args = new HslEventArgs()
                {
                    HslMessage = item,
                };

                AddItemToCache(item);
                // 触发事件
                OnBeforeSaveToFile(args);
            }

        }


        private void AddItemToCache(HslMessageItem item)
        {
            m_simpleHybirdLock.Enter();

            m_WaitForSave.Enqueue(item);

            m_simpleHybirdLock.Leave();

            StartSaveFile();
        }


        private void StartSaveFile()
        {
            if (Interlocked.CompareExchange(ref m_SaveStatus, 1, 0) == 0)
            {
                //启动存储
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolSaveFile), null);
            }
        }

        private HslMessageItem GetAndRemoveLogItem()
        {
            HslMessageItem result = null;

            m_simpleHybirdLock.Enter();

            result = m_WaitForSave.Count > 0 ? m_WaitForSave.Dequeue() : null;

            m_simpleHybirdLock.Leave();

            return result;
        }

        private void ThreadPoolSaveFile(object obj)
        {
            // 获取需要存储的日志
            HslMessageItem current = GetAndRemoveLogItem();
            // 进入文件操作的锁
            m_fileSaveLock.Enter();


            // 获取要存储的文件名称
            string LogSaveFileName = GetFileSaveName();

            if (!string.IsNullOrEmpty(LogSaveFileName))
            {
                // 保存
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(LogSaveFileName, true, Encoding.UTF8);
                    while (current != null)
                    {
                        sw.Write(HslMessageFormate(current));
                        sw.Write(Environment.NewLine);
                        current = GetAndRemoveLogItem();
                    }
                }
                catch (Exception ex)
                {
                    AddItemToCache(current);
                    AddItemToCache(new HslMessageItem()
                    {
                        Degree = HslMessageDegree.FATAL,
                        Text = LogNetManagment.GetSaveStringFromException("LogNetSelf", ex),
                    });
                }
                finally
                {
                    sw?.Dispose();
                }
            }


            // 释放锁
            m_fileSaveLock.Leave();

            Interlocked.Exchange(ref m_SaveStatus, 0);

            // 再次检测锁是否释放完成
            if (m_WaitForSave.Count > 0)
            {
                StartSaveFile();
            }
        }

        private string HslMessageFormate(HslMessageItem hslMessage)
        {
            StringBuilder stringBuilder = new StringBuilder("\u0002");
            stringBuilder.Append("[");
            stringBuilder.Append(LogNetManagment.GetDegreeDescription(hslMessage.Degree));
            stringBuilder.Append("] ");

            stringBuilder.Append(hslMessage.Time.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            stringBuilder.Append(" thread:[");
            stringBuilder.Append(hslMessage.ThreadId.ToString("D2"));
            stringBuilder.Append("] ");

            stringBuilder.Append(hslMessage.Text);

            return stringBuilder.ToString();
        }

        private void ThreadPoolSaveText(object obj)
        {
            // 进入文件操作的锁
            m_fileSaveLock.Enter();

            //获取要存储的文件名称
            string LogSaveFileName = GetFileSaveName();

            if (!string.IsNullOrEmpty(LogSaveFileName))
            {
                // 保存
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(LogSaveFileName, true, Encoding.UTF8);
                    string str = obj as string;
                    sw.Write(str);
                }
                catch (Exception ex)
                {
                    AddItemToCache(new HslMessageItem()
                    {
                        Degree = HslMessageDegree.FATAL,
                        Text = LogNetManagment.GetSaveStringFromException("LogNetSelf", ex),
                    });
                }
                finally
                {
                    sw?.Dispose();
                }
            }

            // 释放锁
            m_fileSaveLock.Leave();
        }

        #endregion

        #region 辅助方法




        /// <summary>
        /// 获取要存储的文件的名称
        /// </summary>
        /// <returns></returns>
        protected virtual string GetFileSaveName()
        {
            return string.Empty;
        }

        /// <summary>
        /// 返回检查的路径名称，将会包含反斜杠
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected string CheckPathEndWithSprit(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                if (!filePath.EndsWith(@"\"))
                {
                    return filePath + @"\";
                }
            }
            return filePath;
        }



        private HslMessageItem GetHslMessageItem(HslMessageDegree degree, string text)
        {
            return new HslMessageItem()
            {
                Degree = degree,
                Text = text,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Time = DateTime.Now,
            };
        }


        private int CalculateStringOccupyLength(string str)
        {
            if (string.IsNullOrEmpty(str)) return 0;
            int result = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] >= 0x4e00 && str[i] <= 0x9fbb)
                {
                    result += 2;
                }
                else
                {
                    result += 1;
                }
            }
            return result;
        }

        private void AppendCharToStringBuilder(StringBuilder sb, char c, int count)
        {
            for (int i = 0; i < count; i++)
            {
                sb.Append(c);
            }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。

                    m_simpleHybirdLock.Dispose();
                    m_WaitForSave.Clear();
                    m_fileSaveLock.Dispose();

                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                m_simpleHybirdLock = null;
                m_WaitForSave = null;
                m_fileSaveLock = null;
                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~LogNetBase() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }


        // 添加此代码以正确实现可处置模式。
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
