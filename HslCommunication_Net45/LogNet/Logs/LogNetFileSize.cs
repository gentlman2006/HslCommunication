using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HslCommunication.LogNet
{
    /// <summary>
    /// 根据文件的大小来存储日志信息
    /// </summary>
    public class LogNetFileSize : LogNetBase, ILogNet
    {

        #region 构造方法

        /// <summary>
        /// 实例化一个根据文件大小生成新文件的
        /// </summary>
        /// <param name="filePath">日志文件的保存路径</param>
        /// <param name="fileMaxSize">每个日志文件的最大大小，默认2M</param>
        public LogNetFileSize(string filePath, int fileMaxSize = 2 * 1024 * 1024)
        {
            m_filePath = filePath;
            m_fileMaxSize = fileMaxSize;


            LogSaveMode = LogNetManagment.LogSaveModeByFileSize;

            m_filePath = CheckPathEndWithSprit(m_filePath);
        }




        #endregion






        /// <summary>
        /// 当前正在存储的文件名称
        /// </summary>
        private string m_fileName = string.Empty;

        private string m_filePath = string.Empty;

        private int m_fileMaxSize = 2 * 1024 * 1024; //2M

        private int m_CurrentFileSize = 0;


        /// <summary>
        /// 获取需要保存的日志文件
        /// </summary>
        /// <returns></returns>
        protected override string GetFileSaveName()
        {
            //路径没有设置则返回空
            if (string.IsNullOrEmpty(m_filePath)) return string.Empty;

            if(string.IsNullOrEmpty(m_fileName))
            {
                //加载文件名称
                m_fileName = GetLastAccessFileName();
            }
            
            if(File.Exists(m_fileName))
            {
                FileInfo fileInfo = new FileInfo(m_fileName);

                if (fileInfo.Length > m_fileMaxSize)
                {
                    //新生成文件
                    m_fileName = GetDefaultFileName();
                }
            }

            return m_fileName;
        }



        /// <summary>
        /// 获取之前保存的日志文件
        /// </summary>
        /// <returns></returns>
        private string GetLastAccessFileName()
        {
            foreach (var m in GetExistLogFileNames())
            {
                FileInfo fileInfo = new FileInfo(m);
                if (fileInfo.Length < m_fileMaxSize)
                {
                    m_CurrentFileSize = (int)fileInfo.Length;
                    return m;
                }
            }

            //返回一个新的默认当前时间的日志名称
            return GetDefaultFileName();
        }


        /// <summary>
        /// 获取一个新的默认的文件名称
        /// </summary>
        /// <returns></returns>
        private string GetDefaultFileName()
        {
            //返回一个新的默认当前时间的日志名称
            return m_filePath + LogNetManagment.LogFileHeadString + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
        }


        /// <summary>
        /// 返回所有的日志文件
        /// </summary>
        /// <returns></returns>
        public string[] GetExistLogFileNames()
        {
            if (!string.IsNullOrEmpty(m_filePath))
            {
                return Directory.GetFiles(m_filePath, LogNetManagment.LogFileHeadString + "*.txt");
            }
            else
            {
                return new string[] { };
            }
        }
    }
}
