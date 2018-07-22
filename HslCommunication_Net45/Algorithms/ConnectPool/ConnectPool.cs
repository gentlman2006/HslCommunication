using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.ModBus;

namespace HslCommunication.Algorithms.ConnectPool
{
    /// <summary>
    /// 一个连接池管理器，负责维护多个可用的连接，并且自动清理，扩容
    /// </summary>
    /// <typeparam name="TConnector">管理的连接类，需要支持IConnector接口</typeparam>
    /// <remarks>
    /// 需要先实现 <see cref="IConnector"/> 接口的对象，然后就可以实现真正的连接池了，理论上可以实现任意的连接对象，包括modbus连接对象，各种PLC连接对象，数据库连接对象，redis连接对象，SimplifyNet连接对象等等。下面的示例就是modbus-tcp的实现
    /// <note type="warning">要想真正的支持连接池访问，还需要服务器支持一个端口的多连接操作，三菱PLC的端口就不支持，如果要测试示例代码的连接池对象，需要使用本组件的<see cref="ModbusTcpServer"/>来创建服务器对象</note>
    /// </remarks>
    /// <example>
    /// 下面举例实现一个modbus的连接池对象，先实现接口化的操作
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Algorithms\ConnectPool.cs" region="IConnector Example" title="IConnector示例" />
    /// 然后就可以实现真正的连接池了
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Algorithms\ConnectPool.cs" region="ConnectPoolExample" title="ConnectPool示例" />
    /// </example>
    public class ConnectPool<TConnector> where TConnector : IConnector
    {
        #region Constructor

        /// <summary>
        /// 实例化一个连接池对象，需要指定如果创建新实例的方法
        /// </summary>
        /// <param name="createConnector">创建连接对象的委托</param>
        public ConnectPool( Func<TConnector> createConnector )
        {
            this.CreateConnector = createConnector;
            hybirdLock = new HslCommunication.Core.SimpleHybirdLock( );
            connectors = new List<TConnector>( );

            timerCheck = new System.Threading.Timer( TimerCheckBackground, null, 10000, 30000 );
        }

        #endregion

        #region Public Method


        /// <summary>
        /// 获取可用的对象
        /// </summary>
        /// <returns>可用的连接对象</returns>
        public TConnector GetAvailableConnector( )
        {
            while (!canGetConnector)
            {
                System.Threading.Thread.Sleep( 100 );
            }

            TConnector result = default( TConnector );
            hybirdLock.Enter( );

            for (int i = 0; i < connectors.Count; i++)
            {
                if (!connectors[i].IsConnectUsing)
                {
                    connectors[i].IsConnectUsing = true;
                    result = connectors[i];
                    break;
                }
            }

            if (result == null)
            {
                // 创建新的连接
                result = CreateConnector( );
                result.IsConnectUsing = true;
                result.LastUseTime = DateTime.Now;
                result.Open( );
                connectors.Add( result );
                usedConnector = connectors.Count;

                if (usedConnector == maxConnector) canGetConnector = false;
            }


            result.LastUseTime = DateTime.Now;

            hybirdLock.Leave( );

            return result;
        }

        /// <summary>
        /// 使用完之后需要通知管理器
        /// </summary>
        /// <param name="connector">连接对象</param>
        public void ReturnConnector( TConnector connector )
        {
            hybirdLock.Enter( );

            int index = connectors.IndexOf( connector );
            if (index != -1)
            {
                connectors[index].IsConnectUsing = false;
            }

            hybirdLock.Leave( );
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 获取或设置最大的连接数
        /// </summary>
        public int MaxConnector
        {
            get { return maxConnector; }
            set { maxConnector = value; }
        }


        /// <summary>
        /// 获取或设置连接过期的时间，单位秒，默认30秒
        /// </summary>
        public int ConectionExpireTime
        {
            get { return expireTime; }
            set { expireTime = value; }
        }


        /// <summary>
        /// 当前已经使用的连接数
        /// </summary>
        public int UsedConnector
        {
            get { return usedConnector; }
        }

        #endregion

        #region Clear Timer


        private void TimerCheckBackground( object obj )
        {
            // 清理长久不用的连接对象
            hybirdLock.Enter( );

            for (int i = connectors.Count - 1; i >= 0; i--)
            {
                if ((DateTime.Now - connectors[i].LastUseTime).TotalSeconds > expireTime && !connectors[i].IsConnectUsing)
                {
                    // 10分钟未使用了，就要删除掉
                    connectors[i].Close( );
                    connectors.RemoveAt( i );
                }
            }

            usedConnector = connectors.Count;
            if (usedConnector < connectors.Count) canGetConnector = true;

            hybirdLock.Leave( );
        }

        #endregion

        #region Private Member

        private Func<TConnector> CreateConnector = null;                   // 创建新的连接对象的委托
        private int maxConnector = 10;                                     // 最大的连接数
        private int usedConnector = 0;                                     // 已经使用的连接
        private int expireTime = 30;                                       // 连接的过期时间，单位秒
        private bool canGetConnector = true;                               // 是否可以获取连接
        private System.Threading.Timer timerCheck = null;                  // 对象列表检查的时间间隔
        private HslCommunication.Core.SimpleHybirdLock hybirdLock = null;  // 列表操作的锁
        private List<TConnector> connectors = null;                        // 所有连接的列表

        #endregion
    }
}
