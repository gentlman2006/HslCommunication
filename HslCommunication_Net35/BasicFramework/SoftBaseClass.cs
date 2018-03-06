using HslCommunication.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HslCommunication.BasicFramework
{
    
    /*****************************************************************************
     * 
     *    一些类的基类，实现一些共同的基础功能
     *    
     *    Create Date : 2017-05-03 17:45:37
     *    
     *    
     *    2018年3月6日 21:38:37
     *    修改：提炼接口，完善注释和分块
     * 
     * 
     *****************************************************************************/



    /// <summary>
    /// 支持字符串信息加载存储的接口，定义了几个通用的方法
    /// </summary>
    public interface ISoftFileSaveBase
    {
        /// <summary>
        /// 获取需要保存的数据，需要重写实现
        /// </summary>
        /// <returns>需要存储的信息</returns>
        string ToSaveString();

        /// <summary>
        /// 从字符串加载数据，需要重写实现
        /// </summary>
        /// <param name="content">字符串数据</param>
        void LoadByString(string content);


        /// <summary>
        /// 不使用解密方法从文件读取数据
        /// </summary>
        void LoadByFile();


        /// <summary>
        /// 不使用加密方法保存数据到文件
        /// </summary>
        void SaveToFile();


        /// <summary>
        /// 文件路径的存储
        /// </summary>
        string FileSavePath { get; set; }
    }


    /// <summary>
    /// 文件存储功能的基类，包含了文件存储路径，存储方法等
    /// </summary>
    public class SoftFileSaveBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个文件存储的基类
        /// </summary>
        public SoftFileSaveBase()
        {
            HybirdLock = new SimpleHybirdLock();
        }

        #endregion

        #region Private Member

        private SimpleHybirdLock HybirdLock;                   // 文件存储的同步锁

        #endregion

        #region Protect Member


        /// <summary>
        /// 在日志保存时的标记当前调用类的信息
        /// </summary>
        protected string LogHeaderText { get; set; }

        #endregion

        #region Save Load String


        /// <summary>
        /// 获取需要保存的数据，需要重写实现
        /// </summary>
        /// <returns>需要存储的信息</returns>
        public virtual string ToSaveString()
        {
            return string.Empty;
        }


        /// <summary>
        /// 从字符串加载数据，需要重写实现
        /// </summary>
        /// <param name="content">字符串数据</param>
        public virtual void LoadByString(string content)
        {

        }

        #endregion

        #region Save Load File


        /// <summary>
        /// 不使用解密方法从文件读取数据
        /// </summary>
        public virtual void LoadByFile()
        {
            LoadByFile(m => m);
        }



        /// <summary>
        /// 使用用户自定义的解密方法从文件读取数据
        /// </summary>
        /// <param name="decrypt">用户自定义的解密方法</param>
        public void LoadByFile(Converter<string, string> decrypt)
        {
            if (FileSavePath != "")
            {
                if (File.Exists(FileSavePath))
                {
                    HybirdLock.Enter();
                    try
                    {
                        using (StreamReader sr = new StreamReader(FileSavePath, Encoding.Default))
                        {
                            LoadByString(decrypt(sr.ReadToEnd()));
                        }
                    }
                    catch(Exception ex)
                    {
                        ILogNet?.WriteException(LogHeaderText,StringResources.FileLoadFailed, ex);
                    }
                    finally
                    {
                        HybirdLock.Leave();
                    }
                }
            }
        }



        /// <summary>
        /// 不使用加密方法保存数据到文件
        /// </summary>
        public virtual void SaveToFile()
        {
            SaveToFile(m => m);
        }


        /// <summary>
        /// 使用用户自定义的加密方法保存数据到文件
        /// </summary>
        /// <param name="encrypt">用户自定义的加密方法</param>
        public void SaveToFile(Converter<string, string> encrypt)
        {
            if (FileSavePath != "")
            {
                HybirdLock.Enter();
                try
                {
                    using (StreamWriter sw = new StreamWriter(FileSavePath, false, Encoding.Default))
                    {
                        sw.Write(encrypt(ToSaveString()));
                        sw.Flush();
                    }
                }
                catch (Exception ex)
                {
                    ILogNet?.WriteException(LogHeaderText,StringResources.FileSaveFailed, ex);
                }
                finally
                {
                    HybirdLock.Leave();
                }
            }
        }


        #endregion



        /// <summary>
        /// 文件存储的路径
        /// </summary>
        public string FileSavePath { get; set; }

        /// <summary>
        /// 日志记录类
        /// </summary>
        public LogNet.ILogNet ILogNet { get; set; } 


    }


}
