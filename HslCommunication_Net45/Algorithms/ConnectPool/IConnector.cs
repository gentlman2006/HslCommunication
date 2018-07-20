using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Algorithms.ConnectPool
{
    /// <summary>
    /// 连接池的接口，连接池的管理对象必须实现此接口
    /// </summary>
    /// <remarks>为了使用完整的连接池功能，需要先实现本接口，然后配合<see cref="ConnectPool{TConnector}"/>来使用</remarks>
    /// <example>
    /// 下面举例实现一个modbus的连接池对象
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Algorithms\ConnectPool.cs" region="IConnector Example" title="IConnector示例" />
    /// </example>
    public interface IConnector
    {

        /// <summary>
        /// 指示当前的连接是否在使用用
        /// </summary>
        bool IsConnectUsing { get; set; }


        /// <summary>
        /// 唯一的GUID码
        /// </summary>
        string GuidToken { get; set; }


        /// <summary>
        /// 最新一次使用的时间
        /// </summary>
        DateTime LastUseTime { get; set; }


        /// <summary>
        /// 打开连接
        /// </summary>
        void Open( );


        /// <summary>
        /// 关闭并释放
        /// </summary>
        void Close( );

    }
}
