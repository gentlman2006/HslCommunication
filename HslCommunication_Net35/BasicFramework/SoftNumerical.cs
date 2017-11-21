using HslCommunication.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HslCommunication.BasicFramework
{
    /********************************************************************************************
     * 
     *    一个高度灵活的流水号生成的类，允许根据指定规则加上时间数据进行生成
     * 
     *    根据保存机制进行优化，必须做到好并发量
     * 
     ********************************************************************************************/




    /// <summary>
    /// 一个用于自动流水号生成的类，必须指定保存的文件，实时保存来确认安全
    /// </summary>
    public sealed class SoftNumericalOrder : SoftFileSaveBase
    {
        /// <summary>
        /// 实例化一个流水号生成的对象
        /// </summary>
        /// <param name="textHead">流水号的头文本</param>
        /// <param name="timeFormate">流水号带的时间信息</param>
        /// <param name="numberLength">流水号数字的标准长度，不够补0</param>
        /// <param name="fileSavePath">流水号存储的文本位置</param>
        public SoftNumericalOrder(string textHead, string timeFormate,int numberLength, string fileSavePath)
        {
            TextHead = textHead;
            TimeFormate = timeFormate;
            NumberLength = numberLength;
            FileSavePath = fileSavePath;
            LoadByFile();

            AsyncCoordinator = new HslAsyncCoordinator(() =>
              {
                  if (!string.IsNullOrEmpty(FileSavePath))
                  {
                      using (System.IO.StreamWriter sw = new System.IO.StreamWriter(FileSavePath, false, Encoding.Default))
                      {
                          sw.Write(CurrentIndex);
                      }
                  }
              });

        }

        
        /// <summary>
        /// 当前的生成序列号
        /// </summary>
        private long CurrentIndex = 0;
        /// <summary>
        /// 流水号的文本头
        /// </summary>
        private string TextHead = string.Empty;
        /// <summary>
        /// 时间格式默认年月日
        /// </summary>
        private string TimeFormate = "yyyyMMdd";
        /// <summary>
        /// 流水号数字应该显示的长度
        /// </summary>
        private int NumberLength = 5;
        /// <summary>
        /// 获取流水号的值
        /// </summary>
        /// <returns></returns>
        public override string ToSaveString()
        {
            return CurrentIndex.ToString();
        }
        /// <summary>
        /// 加载流水号
        /// </summary>
        /// <param name="content"></param>
        public override void LoadByString(string content)
        {
            CurrentIndex = Convert.ToInt64(content);
        }

        /// <summary>
        /// 清除流水号计数，进行重新计数
        /// </summary>
        public void ClearNumericalOrder()
        {
            Interlocked.Exchange(ref CurrentIndex, 0);
            AsyncCoordinator.StartOperaterInfomation();
        }

        /// <summary>
        /// 获取流水号数据
        /// </summary>
        /// <returns></returns>
        public string GetNumericalOrder()
        {
            long number = Interlocked.Increment(ref CurrentIndex);
            AsyncCoordinator.StartOperaterInfomation();
            return TextHead + DateTime.Now.ToString(TimeFormate) + number.ToString().PadLeft(NumberLength, '0');
        }
        /// <summary>
        /// 获取流水号数据
        /// </summary>
        /// <param name="textHead">指定一个新的文本头</param>
        /// <returns></returns>
        public string GetNumericalOrder(string textHead)
        {
            long number = Interlocked.Increment(ref CurrentIndex);
            AsyncCoordinator.StartOperaterInfomation();
            return textHead + DateTime.Now.ToString(TimeFormate) + number.ToString().PadLeft(NumberLength, '0');
        }



        #region 高性能存储块

        /// <summary>
        /// 高性能存储块
        /// </summary>
        private HslAsyncCoordinator AsyncCoordinator { get; set; }



        #endregion
        

    }
}
