using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace HslCommunication.Core
{

    #region 多线程同步协调类


    /// <summary>
    /// 线程的协调逻辑状态
    /// </summary>
    internal enum CoordinationStatus
    {
        /// <summary>
        /// 所有项完成
        /// </summary>
        AllDone,
        /// <summary>
        /// 超时
        /// </summary>
        Timeout,
        /// <summary>
        /// 任务取消
        /// </summary>
        Cancel
    }

    /// <summary>
    /// 一个线程协调逻辑类，详细参考书籍《CLR Via C#》page:681
    /// 这个类可惜没有报告进度的功能
    /// </summary>
    internal sealed class AsyncCoordinator
    {
        private int m_opCount = 1;
        private int m_statusReported = 0;
        private Action<CoordinationStatus> m_callback;
        private System.Threading.Timer m_timer;

        /// <summary>
        /// 每次的操作任务开始前必须调用该方法
        /// </summary>
        /// <param name="opsToAdd"></param>
        public void AboutToBegin(int opsToAdd = 1) => Interlocked.Add(ref m_opCount, opsToAdd);
        /// <summary>
        /// 在一次任务处理好操作之后，必须调用该方法
        /// </summary>
        public void JustEnded()
        {
            if (Interlocked.Decrement(ref m_opCount) == 0)
            {
                ReportStatus(CoordinationStatus.AllDone);
            }
        }
        /// <summary>
        /// 该方法必须在发起所有的操作之后调用
        /// </summary>
        /// <param name="callback">回调方法</param>
        /// <param name="timeout">超时时间</param>
        public void AllBegun(Action<CoordinationStatus> callback, int timeout = Timeout.Infinite)
        {
            m_callback = callback;
            if (timeout != Timeout.Infinite)
            {
                m_timer = new System.Threading.Timer(TimeExpired, null, timeout, Timeout.Infinite);
            }
            JustEnded();//修正一开始设置的初始值
        }
        /// <summary>
        /// 超时的方法
        /// </summary>
        /// <param name="o"></param>
        private void TimeExpired(object o) => ReportStatus(CoordinationStatus.Timeout);
        /// <summary>
        /// 取消任务的执行
        /// </summary>
        public void Cancel() => ReportStatus(CoordinationStatus.Cancel);
        /// <summary>
        /// 生成一次报告
        /// </summary>
        /// <param name="status">报告的状态</param>
        private void ReportStatus(CoordinationStatus status)
        {
            //只报告一次的限制
            if (Interlocked.Exchange(ref m_statusReported, 1) == 0)
            {
                m_callback(status);
            }
        }

        /// <summary>
        /// 乐观的并发方法模型，具体参照《CLR Via C#》page:686
        /// </summary>
        /// <param name="target">唯一的目标数据</param>
        /// <param name="change">修改数据的算法</param>
        /// <returns></returns>
        public static int Maxinum(ref int target, Func<int, int> change)
        {
            int currentVal = target, startVal, desiredVal;
            do
            {
                startVal = currentVal;//设置值
                //以下为业务逻辑，允许实现非常复杂的设置
                desiredVal = change(startVal);

                currentVal = Interlocked.CompareExchange(ref target, desiredVal, startVal);
            }
            while (startVal != currentVal);//更改失败就强制更新
            return desiredVal;
        }
    }


    #endregion

    #region 乐观并发模型的协调类


    /// <summary>
    /// 一个用于高性能，乐观并发模型控制操作的类，允许一个方法(隔离方法)的安全单次执行
    /// </summary>
    public sealed class HslAsyncCoordinator
    {
        /// <summary>
        /// 实例化一个对象，需要传入隔离执行的方法
        /// </summary>
        /// <param name="operater">隔离执行的方法</param>
        public HslAsyncCoordinator(Action operater)
        {
            action = operater;
        }
        /// <summary>
        /// 操作状态，0是未操作，1是操作中
        /// </summary>
        private int OperaterStatus = 0;
        /// <summary>
        /// 需要操作的次数
        /// </summary>
        private long Target = 0;
        /// <summary>
        /// 启动线程池执行隔离方法
        /// </summary>
        public void StartOperaterInfomation()
        {
            Interlocked.Increment(ref Target);
            if (Interlocked.CompareExchange(ref OperaterStatus, 1, 0) == 0)
            {
                //启动保存
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolOperater), null);
            }
        }

        private Action action = null;

        private void ThreadPoolOperater(object obj)
        {
            long currentVal = Target, startVal;
            long desiredVal = 0;
            do
            {
                startVal = currentVal;//设置值
                // 以下为业务逻辑，允许实现非常复杂的设置
                action?.Invoke();
                // 需要清零值的时候必须用下面的原子操作
                currentVal = Interlocked.CompareExchange(ref Target, desiredVal, startVal);
            }
            while (startVal != currentVal);// 更改失败就强制更新

            // 退出保存状态
            Interlocked.Exchange(ref OperaterStatus, 0);
            // 最终状态确认
            if (Target != desiredVal) StartOperaterInfomation();
        }
    }



    #endregion

    #region 高性能的读写锁

    // 一个高性能的读写锁，由《CLR Via C#》作者Jeffrey Richter提供

    /// <summary>
    /// 一个高性能的读写锁，支持写锁定，读灵活，读时写锁定，写时读锁定
    /// </summary>
    public sealed class HslReadWriteLock : IDisposable
    {
        #region Lock State Management
#if false
              private struct BitField {
                 private int m_mask, m_1, m_startBit;
                 public BitField(int startBit, int numBits) {
                    m_startBit = startBit;
                    m_mask = unchecked((int)((1 << numBits) - 1) << startBit);
                    m_1 = unchecked((int)1 << startBit);
                 }
                 public void Increment(ref int value) { value += m_1; }
                 public void Decrement(ref int value) { value -= m_1; }
                 public void Decrement(ref int value, int amount) { value -= m_1 * amount; }
                 public int Get(int value) { return (value & m_mask) >> m_startBit; }
                 public int Set(int value, int fieldValue) { return (value & ~m_mask) | (fieldValue << m_startBit); }
              }

              private static BitField s_state = new BitField(0, 3);
              private static BitField s_readersReading = new BitField(3, 9);
              private static BitField s_readersWaiting = new BitField(12, 9);
              private static BitField s_writersWaiting = new BitField(21, 9);
              private static OneManyLockStates State(int value) { return (OneManyLockStates)s_state.Get(value); }
              private static void State(ref int ls, OneManyLockStates newState) {
                 ls = s_state.Set(ls, (int)newState);
              }
#endif
        private enum OneManyLockStates
        {
            Free = 0x00000000,
            OwnedByWriter = 0x00000001,
            OwnedByReaders = 0x00000002,
            OwnedByReadersAndWriterPending = 0x00000003,
            ReservedForWriter = 0x00000004,
        }

        private const int c_lsStateStartBit = 0;
        private const int c_lsReadersReadingStartBit = 3;
        private const int c_lsReadersWaitingStartBit = 12;
        private const int c_lsWritersWaitingStartBit = 21;

        // Mask = unchecked((int) ((1 << numBits) - 1) << startBit);
        private const int c_lsStateMask = unchecked((int)((1 << 3) - 1) << c_lsStateStartBit);
        private const int c_lsReadersReadingMask = unchecked((int)((1 << 9) - 1) << c_lsReadersReadingStartBit);
        private const int c_lsReadersWaitingMask = unchecked((int)((1 << 9) - 1) << c_lsReadersWaitingStartBit);
        private const int c_lsWritersWaitingMask = unchecked((int)((1 << 9) - 1) << c_lsWritersWaitingStartBit);
        private const int c_lsAnyWaitingMask = c_lsReadersWaitingMask | c_lsWritersWaitingMask;

        // FirstBit = unchecked((int) 1 << startBit);
        private const int c_ls1ReaderReading = unchecked((int)1 << c_lsReadersReadingStartBit);
        private const int c_ls1ReaderWaiting = unchecked((int)1 << c_lsReadersWaitingStartBit);
        private const int c_ls1WriterWaiting = unchecked((int)1 << c_lsWritersWaitingStartBit);

        private static OneManyLockStates State(int ls) { return (OneManyLockStates)(ls & c_lsStateMask); }
        private static void SetState(ref int ls, OneManyLockStates newState)
        {
            ls = (ls & ~c_lsStateMask) | ((int)newState);
        }

        private static int NumReadersReading(int ls) { return (ls & c_lsReadersReadingMask) >> c_lsReadersReadingStartBit; }
        private static void AddReadersReading(ref int ls, int amount) { ls += (c_ls1ReaderReading * amount); }

        private static int NumReadersWaiting(int ls) { return (ls & c_lsReadersWaitingMask) >> c_lsReadersWaitingStartBit; }
        private static void AddReadersWaiting(ref int ls, int amount) { ls += (c_ls1ReaderWaiting * amount); }

        private static int NumWritersWaiting(int ls) { return (ls & c_lsWritersWaitingMask) >> c_lsWritersWaitingStartBit; }
        private static void AddWritersWaiting(ref int ls, int amount) { ls += (c_ls1WriterWaiting * amount); }

        private static bool AnyWaiters( int ls ) { return (ls & c_lsAnyWaitingMask) != 0; }

        private static string DebugState(int ls)
        {
            return string.Format(CultureInfo.InvariantCulture,
               "State={0}, RR={1}, RW={2}, WW={3}", State(ls),
               NumReadersReading(ls), NumReadersWaiting(ls), NumWritersWaiting(ls));
        }

        /// <summary>
        /// 返回本对象的描述字符串
        /// </summary>
        /// <returns>对象的描述字符串</returns>
        public override string ToString() { return DebugState(m_LockState); }
        #endregion

        #region State Fields
        private int m_LockState = (int)OneManyLockStates.Free;

        // Readers wait on this if a writer owns the lock
        private Semaphore m_ReadersLock = new Semaphore(0, int.MaxValue);

        // Writers wait on this if a reader owns the lock
        private Semaphore m_WritersLock = new Semaphore(0, int.MaxValue);
        #endregion

        #region Construction
        /// <summary>
        /// 实例化一个读写锁的对象
        /// </summary>
        public HslReadWriteLock() : base() { }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                m_WritersLock.Close(); m_WritersLock = null;
                m_ReadersLock.Close(); m_ReadersLock = null;
                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~HslReadWriteLock() {
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

        #region Writer members
        private bool m_exclusive;

        /// <summary>
        /// 根据读写情况请求锁
        /// </summary>
        /// <param name="exclusive">True为写请求，False为读请求</param>
        public void Enter(bool exclusive)
        {
            if (exclusive)
            {
                while (WaitToWrite(ref m_LockState)) m_WritersLock.WaitOne();
            }
            else
            {
                while (WaitToRead(ref m_LockState)) m_ReadersLock.WaitOne();
            }
            m_exclusive = exclusive;
        }

        private static bool WaitToWrite(ref int target)
        {
            int start, current = target;
            bool wait;
            do
            {
                start = current;
                int desired = start;
                wait = false;

                switch (State(desired))
                {
                    case OneManyLockStates.Free:  // If Free -> OBW, return
                    case OneManyLockStates.ReservedForWriter: // If RFW -> OBW, return
                        SetState(ref desired, OneManyLockStates.OwnedByWriter);
                        break;

                    case OneManyLockStates.OwnedByWriter:  // If OBW -> WW++, wait & loop around
                        AddWritersWaiting(ref desired, 1);
                        wait = true;
                        break;

                    case OneManyLockStates.OwnedByReaders: // If OBR or OBRAWP -> OBRAWP, WW++, wait, loop around
                    case OneManyLockStates.OwnedByReadersAndWriterPending:
                        SetState(ref desired, OneManyLockStates.OwnedByReadersAndWriterPending);
                        AddWritersWaiting(ref desired, 1);
                        wait = true;
                        break;
                    default:
                        Debug.Assert(false, "Invalid Lock state");
                        break;
                }
                current = Interlocked.CompareExchange(ref target, desired, start);
            } while (start != current);
            return wait;
        }

        /// <summary>
        /// 释放锁，将根据锁状态自动区分读写锁
        /// </summary>
        public void Leave()
        {
            int wakeup;
            if (m_exclusive)
            {
                Debug.Assert((State(m_LockState) == OneManyLockStates.OwnedByWriter) && (NumReadersReading(m_LockState) == 0));
                // Pre-condition:  Lock's state must be OBW (not Free/OBR/OBRAWP/RFW)
                // Post-condition: Lock's state must become Free or RFW (the lock is never passed)

                // Phase 1: Release the lock
                wakeup = DoneWriting(ref m_LockState);
            }
            else
            {
                var s = State(m_LockState);
                Debug.Assert((State(m_LockState) == OneManyLockStates.OwnedByReaders) || (State(m_LockState) == OneManyLockStates.OwnedByReadersAndWriterPending));
                // Pre-condition:  Lock's state must be OBR/OBRAWP (not Free/OBW/RFW)
                // Post-condition: Lock's state must become unchanged, Free or RFW (the lock is never passed)

                // Phase 1: Release the lock
                wakeup = DoneReading(ref m_LockState);
            }

            // Phase 2: Possibly wake waiters
            if (wakeup == -1) m_WritersLock.Release();
            else if (wakeup > 0) m_ReadersLock.Release(wakeup);
        }

        // Returns -1 to wake a writer, +# to wake # readers, or 0 to wake no one
        private static int DoneWriting(ref int target)
        {
            int start, current = target;
            int wakeup = 0;
            do
            {
                int desired = (start = current);

                // We do this test first because it is commonly true & 
                // we avoid the other tests improving performance
                if (!AnyWaiters(desired))
                {
                    SetState(ref desired, OneManyLockStates.Free);
                    wakeup = 0;
                }
                else if (NumWritersWaiting(desired) > 0)
                {
                    SetState(ref desired, OneManyLockStates.ReservedForWriter);
                    AddWritersWaiting(ref desired, -1);
                    wakeup = -1;
                }
                else
                {
                    wakeup = NumReadersWaiting(desired);
                    Debug.Assert(wakeup > 0);
                    SetState(ref desired, OneManyLockStates.OwnedByReaders);
                    AddReadersWaiting(ref desired, -wakeup);
                    // RW=0, RR=0 (incremented as readers enter)
                }
                current = Interlocked.CompareExchange(ref target, desired, start);
            } while (start != current);
            return wakeup;
        }
        #endregion

        #region Reader members
        private static bool WaitToRead(ref int target)
        {
            int start, current = target;
            bool wait;
            do
            {
                int desired = (start = current);
                wait = false;

                switch (State(desired))
                {
                    case OneManyLockStates.Free:  // If Free->OBR, RR=1, return
                        SetState(ref desired, OneManyLockStates.OwnedByReaders);
                        AddReadersReading(ref desired, 1);
                        break;

                    case OneManyLockStates.OwnedByReaders: // If OBR -> RR++, return
                        AddReadersReading(ref desired, 1);
                        break;

                    case OneManyLockStates.OwnedByWriter:  // If OBW/OBRAWP/RFW -> RW++, wait, loop around
                    case OneManyLockStates.OwnedByReadersAndWriterPending:
                    case OneManyLockStates.ReservedForWriter:
                        AddReadersWaiting(ref desired, 1);
                        wait = true;
                        break;

                    default:
                        Debug.Assert(false, "Invalid Lock state");
                        break;
                }
                current = Interlocked.CompareExchange(ref target, desired, start);
            } while (start != current);
            return wait;
        }

        // Returns -1 to wake a writer, +# to wake # readers, or 0 to wake no one
        private static int DoneReading(ref int target)
        {
            int start, current = target;
            int wakeup;
            do
            {
                int desired = (start = current);
                AddReadersReading(ref desired, -1);  // RR--
                if (NumReadersReading(desired) > 0)
                {
                    // RR>0, no state change & no threads to wake
                    wakeup = 0;
                }
                else if (!AnyWaiters(desired))
                {
                    SetState(ref desired, OneManyLockStates.Free);
                    wakeup = 0;
                }
                else
                {
                    Debug.Assert(NumWritersWaiting(desired) > 0);
                    SetState(ref desired, OneManyLockStates.ReservedForWriter);
                    AddWritersWaiting(ref desired, -1);
                    wakeup = -1;   // Wake 1 writer
                }
                current = Interlocked.CompareExchange(ref target, desired, start);
            } while (start != current);
            return wakeup;
        }

        #endregion
    }



    #endregion

    #region 简单的混合锁

    /// <summary>
    /// 一个简单的混合线程同步锁，采用了基元用户加基元内核同步构造实现
    /// </summary>
    /// <example>
    /// 以下演示常用的锁的使用方式，还包含了如何优雅的处理异常锁
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\ThreadLock.cs" region="SimpleHybirdLockExample1" title="SimpleHybirdLock示例" />
    /// </example>
    public sealed class SimpleHybirdLock : IDisposable
    {

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                m_waiterLock.Close();

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~SimpleHybirdLock() {
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

        /// <summary>
        /// 基元用户模式构造同步锁
        /// </summary>
        private int m_waiters = 0;
        /// <summary>
        /// 基元内核模式构造同步锁
        /// </summary>
        private AutoResetEvent m_waiterLock = new AutoResetEvent(false);

        /// <summary>
        /// 获取锁
        /// </summary>
        public void Enter()
        {
            if (Interlocked.Increment(ref m_waiters) == 1) return;//用户锁可以使用的时候，直接返回，第一次调用时发生
            //当发生锁竞争时，使用内核同步构造锁
            m_waiterLock.WaitOne();
        }

        /// <summary>
        /// 离开锁
        /// </summary>
        public void Leave()
        {
            if (Interlocked.Decrement(ref m_waiters) == 0) return;//没有可用的锁的时候
            m_waiterLock.Set();
        }

        /// <summary>
        /// 获取当前锁是否在等待当中
        /// </summary>
        public bool IsWaitting => m_waiters != 0;
    }


    #endregion

    #region 多线程并发处理数据的类
    

    /*******************************************************************************
     * 
     *    创建日期：2017年7月6日 08:30:56
     *    
     * 
     *******************************************************************************/
     

    /// <summary>
    /// 一个用于多线程并发处理数据的模型类，适用于处理数据量非常庞大的情况
    /// </summary>
    /// <typeparam name="T">等待处理的数据类型</typeparam>
    public sealed class SoftMultiTask<T>
    {
        /// <summary>
        /// 实例化一个数据处理对象
        /// </summary>
        /// <param name="dataList">数据处理列表</param>
        /// <param name="operater">数据操作方法，应该是相对耗时的任务</param>
        /// <param name="threadCount">需要使用的线程数</param>
        public SoftMultiTask(T[] dataList, Func<T, bool> operater, int threadCount = 10)
        {
            m_dataList = dataList ?? throw new ArgumentNullException("dataList");
            m_operater = operater ?? throw new ArgumentNullException("operater");
            if (threadCount < 1) throw new ArgumentException( "threadCount can not less than 1", "threadCount");
            m_threadCount = threadCount;
            //增加任务处理
            Interlocked.Add(ref m_opCount, dataList.Length);
            //增加线程处理
            Interlocked.Add(ref m_opThreadCount, threadCount);
        }



        /// <summary>
        /// 操作总数，判定操作是否完成
        /// </summary>
        private int m_opCount = 0;
        /// <summary>
        /// 判断是否所有的线程是否处理完成
        /// </summary>
        private int m_opThreadCount = 1;
        /// <summary>
        /// 准备启动的处理数据的线程数量
        /// </summary>
        private int m_threadCount = 10;
        /// <summary>
        /// 指示多线程处理是否在运行中，防止冗余调用
        /// </summary>
        private int m_runStatus = 0;

        /// <summary>
        /// 列表数据
        /// </summary>
        private T[] m_dataList = null;
        /// <summary>
        /// 需要操作的方法
        /// </summary>
        private Func<T, bool> m_operater = null;

        /// <summary>
        /// 一个双参数委托
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ex"></param>
        public delegate void MultiInfo(T item, Exception ex);
        /// <summary>
        /// 用于报告进度的委托，当finish等于count时，任务完成
        /// </summary>
        /// <param name="finish">已完成操作数量</param>
        /// <param name="count">总数量</param>
        /// <param name="success">成功数量</param>
        /// <param name="failed">失败数量</param>
        public delegate void MultiInfoTwo(int finish, int count, int success, int failed);

        /// <summary>
        /// 异常发生时事件
        /// </summary>
        public event MultiInfo OnExceptionOccur;
        /// <summary>
        /// 报告处理进度时发生
        /// </summary>
        public event MultiInfoTwo OnReportProgress;


        /// <summary>
        /// 已处理完成数量，无论是否异常
        /// </summary>
        private int m_finishCount = 0;
        /// <summary>
        /// 处理完成并实现操作数量
        /// </summary>
        private int m_successCount = 0;
        /// <summary>
        /// 处理过程中异常数量
        /// </summary>
        private int m_failedCount = 0;


        /// <summary>
        /// 用于触发事件的混合线程锁
        /// </summary>
        private SimpleHybirdLock HybirdLock = new SimpleHybirdLock();

        /// <summary>
        /// 指示处理状态是否为暂停状态
        /// </summary>
        private bool m_isRunningStop = false;
        /// <summary>
        /// 指示系统是否需要强制退出
        /// </summary>
        private bool m_isQuit = false;
        /// <summary>
        /// 在发生错误的时候是否强制退出后续的操作
        /// </summary>
        private bool m_isQuitAfterException = false;


        #region Start Stop Method
        /// <summary>
        /// 启动多线程进行数据处理
        /// </summary>
        public void StartOperater()
        {
            if (Interlocked.CompareExchange(ref m_runStatus, 0, 1) == 0)
            {
                for (int i = 0; i < m_threadCount; i++)
                {
                    Thread thread = new Thread(new ThreadStart(ThreadBackground));
                    thread.IsBackground = true;
                    thread.Start();
                }
                JustEnded();
            }
        }

        /// <summary>
        /// 暂停当前的操作
        /// </summary>
        public void StopOperater()
        {
            if (m_runStatus == 1)
            {
                m_isRunningStop = true;
            }
        }

        /// <summary>
        /// 恢复暂停的操作
        /// </summary>
        public void ResumeOperater()
        {
            m_isRunningStop = false;
        }

        /// <summary>
        /// 直接手动强制结束操作
        /// </summary>
        public void EndedOperater()
        {
            if (m_runStatus == 1)
            {
                m_isQuit = true;
            }
        }
        /// <summary>
        /// 在发生错误的时候是否强制退出后续的操作
        /// </summary>
        public bool IsQuitAfterException
        {
            get
            {
                return m_isQuitAfterException;
            }
            set
            {
                m_isQuitAfterException = value;
            }
        }

        #endregion




        private void ThreadBackground()
        {
            while (true)
            {
                // 检测是否处于暂停的状态
                while (m_isRunningStop)
                {
                    ;
                }
                // 提取处理的任务
                int index = Interlocked.Decrement(ref m_opCount);
                if (index < 0)
                {
                    // 任务完成
                    break;
                }
                else
                {
                    T item = m_dataList[index];
                    bool result = false;
                    bool isException = false;
                    try
                    {
                        if (!m_isQuit) result = m_operater(item);
                    }
                    catch (Exception ex)
                    {
                        isException = true;
                        // 此处必须吞噬所有异常
                        OnExceptionOccur?.Invoke(item, ex);

                        // 是否需要退出处理
                        if (m_isQuitAfterException) EndedOperater();
                    }
                    finally
                    {
                        // 保证了报告进度时数据的正确性
                        HybirdLock.Enter();

                        if (result) m_successCount++;
                        if (isException) m_failedCount++;
                        m_finishCount++;
                        OnReportProgress?.Invoke(m_finishCount, m_dataList.Length, m_successCount, m_failedCount);

                        HybirdLock.Leave();
                    }
                }
            }
            JustEnded();
        }
        private void JustEnded()
        {
            if (Interlocked.Decrement(ref m_opThreadCount) == 0)
            {
                // 数据初始化
                m_finishCount = 0;
                m_failedCount = 0;
                m_successCount = 0;
                Interlocked.Exchange(ref m_opCount, m_dataList.Length);
                Interlocked.Exchange(ref m_opThreadCount, m_threadCount + 1);

                // 状态复位
                Interlocked.Exchange(ref m_runStatus, 0);
                m_isRunningStop = false;
                m_isQuit = false;
            }
        }
    }


    #endregion

    #region 双检锁

#if NET451 || NETSTANDARD2_0

    /// <summary>
    /// 一个双检锁的示例，适合一些占内存的静态数据对象，获取的时候才实例化真正的对象
    /// </summary>
    internal sealed class Singleton
    {
        private static object m_lock = new object();

        private static Singleton SValue = null;

        public Singleton()
        {

        }

        public static Singleton GetSingleton()
        {
            if (SValue != null) return SValue;

            Monitor.Enter(m_lock);
            if (SValue == null)
            {
                Singleton temp = new Singleton();
                Volatile.Write(ref SValue, temp);

                //上述编译不通过，简单的使用下述过程
                SValue = new Singleton();
            }
            Monitor.Exit(m_lock);
            return SValue;
        }
    }

#endif


    #endregion

    #region 高级混合锁


#if NET451 || NETSTANDARD2_0


    /// <summary>
    /// 一个高级的混合线程同步锁，采用了基元用户加基元内核同步构造实现，并包含了自旋和线程所有权
    /// </summary>
    internal sealed class AdvancedHybirdLock : IDisposable
    {

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        void Dispose( bool disposing )
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                m_waiterLock.Close( );

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~SimpleHybirdLock() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose( )
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose( true );
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// 基元用户模式构造同步锁
        /// </summary>
        private int m_waiters = 0;
        /// <summary>
        /// 基元内核模式构造同步锁
        /// </summary>
        private AutoResetEvent m_waiterLock = new AutoResetEvent( false );
        /// <summary>
        /// 控制自旋的一个字段
        /// </summary>
        private int m_spincount = 4000;
        /// <summary>
        /// 指出哪个线程拥有锁
        /// </summary>
        private int m_owningThreadId = 0;
        /// <summary>
        /// 指示锁拥有了多少次
        /// </summary>
        private int m_recursion = 0;

        /// <summary>
        /// 获取锁
        /// </summary>
        public void Enter( )
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (threadId == m_owningThreadId)
            {
                m_recursion++;
                return;//如果调用线程已经拥有锁，就返回
            }
            //SpinWait spinwait

            if (Interlocked.Increment( ref m_waiters ) == 1) return;//用户锁可以使用的时候，直接返回，第一次调用时发生
            //当发生锁竞争时，使用内核同步构造锁
            m_waiterLock.WaitOne( );
        }

        /// <summary>
        /// 离开锁
        /// </summary>
        public void Leave( )
        {
            if (Interlocked.Decrement( ref m_waiters ) == 0) return;//没有可用的锁的时候
            m_waiterLock.Set( );
        }

    }

#endif

    #endregion
}
