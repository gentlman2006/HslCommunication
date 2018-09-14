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
    /// <remarks>
    /// <note type="important">
    /// 序号生成器软件，当获取序列号，清空序列号操作后，会自动的将ID号存储到本地的文件中，存储方式采用乐观并发模型实现。
    /// </note>
    /// </remarks>
    /// <example>
    /// 此处举个例子，也是Demo程序的源代码，包含了2个按钮的示例和瞬间调用100万次的性能示例。
    /// <note type="tip">百万次调用的实际耗时取决于计算机的性能，不同的计算机的表现存在差异，比如作者的：i5-4590cpu,内存ddr3-8G表示差不多在800毫秒左右</note>
    /// <code lang="cs" source="TestProject\HslCommunicationDemo\FormSeqCreate.cs" region="FormSeqCreate" title="示例代码" />
    /// </example>
    public sealed class SoftNumericalOrder : SoftFileSaveBase
    {

        #region Constructor

        /// <summary>
        /// 实例化一个流水号生成的对象
        /// </summary>
        /// <param name="textHead">流水号的头文本</param>
        /// <param name="timeFormate">流水号带的时间信息</param>
        /// <param name="numberLength">流水号数字的标准长度，不够补0</param>
        /// <param name="fileSavePath">流水号存储的文本位置</param>
        public SoftNumericalOrder( string textHead, string timeFormate, int numberLength, string fileSavePath )
        {
            LogHeaderText = "SoftNumericalOrder";
            TextHead = textHead;
            TimeFormate = timeFormate;
            NumberLength = numberLength;
            FileSavePath = fileSavePath;
            LoadByFile( );

            AsyncCoordinator = new HslAsyncCoordinator( ( ) =>
               {
                   if (!string.IsNullOrEmpty( FileSavePath ))
                   {
                       using (System.IO.StreamWriter sw = new System.IO.StreamWriter( FileSavePath, false, Encoding.Default ))
                       {
                           sw.Write( CurrentIndex );
                       }
                   }
               } );

        }

        #endregion

        #region Private Member

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

        #endregion

        #region Public Method


        /// <summary>
        /// 获取流水号的值
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToSaveString( )
        {
            return CurrentIndex.ToString( );
        }


        /// <summary>
        /// 加载流水号
        /// </summary>
        /// <param name="content">源字符串信息</param>
        public override void LoadByString( string content )
        {
            CurrentIndex = Convert.ToInt64( content );
        }

        /// <summary>
        /// 清除流水号计数，进行重新计数
        /// </summary>
        public void ClearNumericalOrder( )
        {
            Interlocked.Exchange( ref CurrentIndex, 0 );
            AsyncCoordinator.StartOperaterInfomation( );
        }

        /// <summary>
        /// 获取流水号数据
        /// </summary>
        /// <returns>新增计数后的信息</returns>
        public string GetNumericalOrder( )
        {
            long number = Interlocked.Increment( ref CurrentIndex );
            AsyncCoordinator.StartOperaterInfomation( );
            if (string.IsNullOrEmpty( TimeFormate ))
            {
                return TextHead + number.ToString( ).PadLeft( NumberLength, '0' );
            }
            else
            {
                return TextHead + DateTime.Now.ToString( TimeFormate ) + number.ToString( ).PadLeft( NumberLength, '0' );
            }
        }

        /// <summary>
        /// 获取流水号数据
        /// </summary>
        /// <param name="textHead">指定一个新的文本头</param>
        /// <returns>带头信息的计数后的信息</returns>
        public string GetNumericalOrder( string textHead )
        {
            long number = Interlocked.Increment( ref CurrentIndex );
            AsyncCoordinator.StartOperaterInfomation( );
            if (string.IsNullOrEmpty( TimeFormate ))
            {
                return textHead + number.ToString( ).PadLeft( NumberLength, '0' );
            }
            else
            {
                return textHead + DateTime.Now.ToString( TimeFormate ) + number.ToString( ).PadLeft( NumberLength, '0' );
            }
        }

        /// <summary>
        /// 单纯的获取数字形式的流水号
        /// </summary>
        /// <returns>新增计数后的信息</returns>
        public long GetLongOrder( )
        {
            long number = Interlocked.Increment( ref CurrentIndex );
            AsyncCoordinator.StartOperaterInfomation( );
            return number;
        }

        #endregion

        #region High Performance Save

        /// <summary>
        /// 高性能存储块
        /// </summary>
        private HslAsyncCoordinator AsyncCoordinator = null;



        #endregion


    }


    /// <summary>
    /// 一个简单的不持久化的序号自增类，采用线程安全实现，并允许指定最大数字，到达后清空从指定数开始
    /// </summary>
    public sealed class SoftIncrementCount : IDisposable
    {
        #region Constructor

        /// <summary>
        /// 实例化一个自增信息的对象，包括最大值
        /// </summary>
        /// <param name="max">数据的最大值，必须指定</param>
        /// <param name="start">数据的起始值，默认为0</param>
        public SoftIncrementCount( long max, long start = 0 )
        {
            this.start = start;
            this.max = max;
            current = start;
            hybirdLock = new SimpleHybirdLock( );
        }

        #endregion

        #region Private Member

        private long start = 0;
        private long current = 0;
        private long max = long.MaxValue;
        private SimpleHybirdLock hybirdLock;

        #endregion

        #region Public Method

        /// <summary>
        /// 获取自增信息
        /// </summary>
        /// <returns>计数自增后的值</returns>
        public long GetCurrentValue( )
        {
            long value = 0;
            hybirdLock.Enter( );

            value = current;
            current++;
            if (current > max)
            {
                current = 0;
            }

            hybirdLock.Leave( );
            return value;
        }


        #endregion
        
        #region IDisposable Support

        private bool disposedValue = false; // 要检测冗余调用

        void Dispose( bool disposing )
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。

                    hybirdLock.Dispose( );
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。


                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~SoftIncrementCount() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        /// <summary>
        /// 释放当前对象所占用的资源
        /// </summary>
        public void Dispose( )
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose( true );
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
