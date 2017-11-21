using HslCommunication.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.BasicFramework
{

    /****************************************************************************
     * 
     *    日期： 2017年6月28日 18:34:14
     *    功能： 一个泛型的数组缓冲类，支持高性能存储
     * 
     ***************************************************************************/


    /// <summary>
    /// 内存队列的基类
    /// </summary>
    public abstract class SoftCacheArrayBase
    {
        /// <summary>
        /// 字节数据流
        /// </summary>
        protected byte[] DataBytes = null;
        /// <summary>
        /// 数据的长度
        /// </summary>
        public int ArrayLength { get; protected set; }
        /// <summary>
        /// 数据数组变动时的数据锁
        /// </summary>
        protected SimpleHybirdLock HybirdLock = new SimpleHybirdLock();


        /// <summary>
        /// 用于从保存的数据对象初始化的
        /// </summary>
        /// <param name="dataSave"></param>
        /// <exception cref="NullReferenceException"></exception>
        public virtual void LoadFromBytes(byte[] dataSave)
        {

        }

        /// <summary>
        /// 获取原本的数据字节
        /// </summary>
        /// <returns></returns>
        public byte[] GetAllData()
        {
            byte[] result = new byte[DataBytes.Length];
            DataBytes.CopyTo(result, 0);
            return result;
        }
    }

    /// <summary>
    /// 一个内存队列缓存的类，数据类型为Int64
    /// </summary>
    public sealed class SoftCacheArrayLong : SoftCacheArrayBase
    {
        /// <summary>
        /// 数据的本身面貌
        /// </summary>
        private long[] DataArray = null;
        /// <summary>
        /// 实例化一个数据对象
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="defaultValue"></param>
        public SoftCacheArrayLong(int capacity, int defaultValue)
        {
            if (capacity < 10) capacity = 10;
            ArrayLength = capacity;
            DataArray = new long[capacity];
            DataBytes = new byte[capacity * 8];

            if (defaultValue != 0)
            {
                for (int i = 0; i < capacity; i++)
                {
                    DataArray[i] = defaultValue;
                }
            }
        }


        /// <summary>
        /// 用于从保存的数据对象初始化的
        /// </summary>
        /// <param name="dataSave"></param>
        /// <exception cref="NullReferenceException"></exception>
        public override void LoadFromBytes(byte[] dataSave)
        {
            int capacity = dataSave.Length / 8;
            ArrayLength = capacity;
            DataArray = new long[capacity];
            DataBytes = new byte[capacity * 8];

            for (int i = 0; i < capacity; i++)
            {
                DataArray[i] = BitConverter.ToInt64(dataSave, i * 8);
            }
        }

        /// <summary>
        /// 线程安全的添加数据
        /// </summary>
        /// <param name="value">值</param>
        public void AddValue(long value)
        {
            HybirdLock.Enter();
            //进入混合锁模式
            for (int i = 0; i < ArrayLength - 1; i++)
            {
                DataArray[i] = DataArray[i + 1];
            }
            DataArray[ArrayLength - 1] = value;

            for (int i = 0; i < ArrayLength; i++)
            {
                BitConverter.GetBytes(DataArray[i]).CopyTo(DataBytes, 8 * i);
            }

            HybirdLock.Leave();


        }


    }



    /// <summary>
    /// 一个内存队列缓存的类，数据类型为Int32
    /// </summary>
    public sealed class SoftCacheArrayInt : SoftCacheArrayBase
    {
        /// <summary>
        /// 数据的本身面貌
        /// </summary>
        private int[] DataArray = null;

        /// <summary>
        /// 实例化一个数据对象
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="defaultValue"></param>
        public SoftCacheArrayInt(int capacity, int defaultValue)
        {
            if (capacity < 10) capacity = 10;
            ArrayLength = capacity;
            DataArray = new int[capacity];
            DataBytes = new byte[capacity * 4];

            if (defaultValue != 0)
            {
                for (int i = 0; i < capacity; i++)
                {
                    DataArray[i] = defaultValue;
                }
            }
        }


        /// <summary>
        /// 用于从保存的数据对象初始化的
        /// </summary>
        /// <param name="dataSave"></param>
        /// <exception cref="NullReferenceException"></exception>
        public override void LoadFromBytes(byte[] dataSave)
        {
            int capacity = dataSave.Length / 4;
            ArrayLength = capacity;
            DataArray = new int[capacity];
            DataBytes = new byte[capacity * 4];

            for (int i = 0; i < capacity; i++)
            {
                DataArray[i] = BitConverter.ToInt32(dataSave, i * 4);
            }
        }

        /// <summary>
        /// 线程安全的添加数据
        /// </summary>
        /// <param name="value">值</param>
        public void AddValue(int value)
        {
            HybirdLock.Enter();
            //进入混合锁模式
            for (int i = 0; i < ArrayLength - 1; i++)
            {
                DataArray[i] = DataArray[i + 1];
            }
            DataArray[ArrayLength - 1] = value;

            for (int i = 0; i < ArrayLength; i++)
            {
                BitConverter.GetBytes(DataArray[i]).CopyTo(DataBytes, 4 * i);
            }

            HybirdLock.Leave();


        }

        /// <summary>
        /// 安全的获取数组队列
        /// </summary>
        /// <returns></returns>
        public int[] GetIntArray()
        {
            int[] result = null;

            HybirdLock.Enter();
            result = new int[ArrayLength];
            //进入混合锁模式
            for (int i = 0; i < ArrayLength; i++)
            {
                result[i] = DataArray[i];
            }
            HybirdLock.Leave();

            return result;
        }
    }
}
