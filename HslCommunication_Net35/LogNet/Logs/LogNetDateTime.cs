using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace HslCommunication.LogNet
{
    /// <summary>
    /// 一个日志组件，可以根据时间来区分不同的文件存储
    /// </summary>
    public class LogNetDateTime : LogNetBase, ILogNet
    {

        #region 构造方法

        /// <summary>
        /// 实例化一个根据时间存储的日志组件
        /// </summary>
        /// <param name="filePath">文件存储的路径</param>
        /// <param name="generateMode">存储文件的间隔</param>
        public LogNetDateTime(string filePath, GenerateMode generateMode = GenerateMode.ByEveryYear)
        {
            m_filePath = filePath;
            m_generateMode = generateMode;

            LogSaveMode = LogNetManagment.LogSaveModeByDateTime;

            m_filePath = CheckPathEndWithSprit(m_filePath);
        }


        #endregion



        /// <summary>
        /// 当前正在存储的文件名称
        /// </summary>
        private string m_fileName = string.Empty;

        /// <summary>
        /// 文件的路径
        /// </summary>
        private string m_filePath = string.Empty;

        /// <summary>
        /// 文件的存储模式，默认按照年份来存储
        /// </summary>
        private GenerateMode m_generateMode = GenerateMode.ByEveryYear;


        /// <summary>
        /// 获取需要保存的日志文件
        /// </summary>
        /// <returns></returns>
        protected override string GetFileSaveName()
        {
            if (string.IsNullOrEmpty(m_filePath)) return string.Empty;

            switch(m_generateMode)
            {
                case GenerateMode.ByEveryHour:
                    {
                        return m_filePath + LogNetManagment.LogFileHeadString + DateTime.Now.ToString("yyyyMMdd_HH") + ".txt";
                    }
                case GenerateMode.ByEveryDay:
                    {
                        return m_filePath + LogNetManagment.LogFileHeadString + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                    }
                case GenerateMode.ByEveryWeek:
                    {
                        GregorianCalendar gc = new GregorianCalendar( );
                        int weekOfYear = gc.GetWeekOfYear( DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday );
                        return m_filePath + LogNetManagment.LogFileHeadString + DateTime.Now.Year + "_W" + weekOfYear + ".txt";
                    }
                case GenerateMode.ByEveryMonth:
                    {
                        return m_filePath + LogNetManagment.LogFileHeadString + DateTime.Now.ToString("yyyy_MM") + ".txt";
                    }
                case GenerateMode.ByEverySeason:
                    {
                        return m_filePath + LogNetManagment.LogFileHeadString + DateTime.Now.Year + "_Q" + (DateTime.Now.Month / 3 + 1) + ".txt";
                    }
                case GenerateMode.ByEveryYear:
                    {
                        return m_filePath + LogNetManagment.LogFileHeadString + DateTime.Now.Year + ".txt";
                    }
                default:return string.Empty;
            }
        }



        /// <summary>
        /// 获取所有的文件夹中的日志文件
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
