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
    public class LogNetSingle : LogNetBase, ILogNet
    {
        private string m_fileName = string.Empty;

        /// <summary>
        /// 实例化一个单文件日志的对象
        /// </summary>
        /// <param name="filePath"></param>
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
        /// <returns></returns>
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
        /// 获取存储的文件的名称
        /// </summary>
        /// <returns></returns>
        protected override string GetFileSaveName()
        {
            return m_fileName;
        }



        /// <summary>
        /// 获取所有的日志文件数组，对于单日志文件来说就只有一个
        /// </summary>
        /// <returns></returns>
        public string[] GetExistLogFileNames()
        {
            return new string[]
            {
                m_fileName,
            };
        }
    }
}
