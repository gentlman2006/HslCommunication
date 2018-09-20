using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HslCommunication.LogNet
{
    /// <summary>
    /// 单日志文件对象
    /// </summary>
    /// <remarks>
    /// 此日志实例化需要指定一个完整的文件路径，当需要记录日志的时候调用方法，会使得日志越来越大，对于写入的性能没有太大影响，但是会影响文件读取。
    /// </remarks>
    public class LogNetSingle : LogNetBase, ILogNet
    {
        #region Private Member

        private string m_fileName = string.Empty;

        #endregion

        #region Constructor
        
        /// <summary>
        /// 实例化一个单文件日志的对象
        /// </summary>
        /// <param name="filePath">文件的路径</param>
        /// <exception cref="FileNotFoundException"></exception>
        public LogNetSingle(string filePath) 
        {
            LogSaveMode = LogNetManagment.LogSaveModeBySingleFile;

            m_fileName = filePath;

            FileInfo fileInfo = new FileInfo(filePath);
            if(!Directory.Exists(fileInfo.DirectoryName))
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 单日志文件允许清空日志内容
        /// </summary>
        public void ClearLog()
        {
            m_fileSaveLock.Enter();
            if (!string.IsNullOrEmpty(m_fileName))
            {
                File.Create(m_fileName).Dispose();
            }
            m_fileSaveLock.Leave();
        }

        /// <summary>
        /// 获取单日志文件的所有保存记录
        /// </summary>
        /// <returns>字符串信息</returns>
        public string GetAllSavedLog()
        {
            string result = string.Empty;
            m_fileSaveLock.Enter();
            if (!string.IsNullOrEmpty(m_fileName))
            {
                if (File.Exists(m_fileName))
                {
                    StreamReader stream = new StreamReader(m_fileName, Encoding.UTF8);
                    result = stream.ReadToEnd();
                    stream.Dispose();
                }
            }
            m_fileSaveLock.Leave();
            return result;
        }
        
        /// <summary>
        /// 获取所有的日志文件数组，对于单日志文件来说就只有一个
        /// </summary>
        /// <returns>字符串数组，包含了所有的存在的日志数据</returns>
        public string[] GetExistLogFileNames( )
        {
            return new string[]
            {
                m_fileName,
            };
        }

        #endregion

        #region LogNetBase Override

        /// <summary>
        /// 获取存储的文件的名称
        /// </summary>
        /// <returns>字符串数据</returns>
        protected override string GetFileSaveName()
        {
            return m_fileName;
        }
        
        #endregion

    }
}
