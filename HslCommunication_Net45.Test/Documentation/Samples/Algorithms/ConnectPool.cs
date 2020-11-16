using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Algorithms.ConnectPool;
using HslCommunication.ModBus;
using HslCommunication;

namespace HslCommunication_Net45.Test.Documentation.Samples.Algorithms
{
    #region ConnectPoolExample
    public class ConnectPoolExample
    {
        public ConnectPoolExample( )
        {
            connectPool = new ConnectPool<ModbusConnector>( ( ) => new ModbusConnector( "192.168.0.100", 502 ) );
            connectPool.MaxConnector = 10;   // 允许同时存在10个连接对象
        }

        private ConnectPool<ModbusConnector> connectPool = null;

        /// <summary>
        /// 现在可以从任意的线程来调用本方法了，性能非常的高
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>结果对象</returns>
        public OperateResult<short> ReadInt16( string address )
        {
            ModbusConnector connector = connectPool.GetAvailableConnector( );
            OperateResult<short> read = connector.ModbusTcp.ReadInt16( address );
            connectPool.ReturnConnector( connector );
            return read;
        }


        /// <summary>
        /// 现在可以从任意的线程来调用本方法了，性能非常的高
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns>结果对象</returns>
        public OperateResult Write( string address ,short value)
        {
            ModbusConnector connector = connectPool.GetAvailableConnector( );
            OperateResult write = connector.ModbusTcp.Write( address, value );
            connectPool.ReturnConnector( connector );
            return write;
        }

        // 其他的接口实现类似
    }

    #endregion


    #region IConnector Example


    /// <summary>
    /// 此处示例实现一个modbus-tcp连接对象，事实上这里可以实现任何的连接对象，PLC的，数据库的，redis的等等操作
    /// </summary>
    public class ModbusConnector : IConnector
    {

        private ModbusTcpNet modbusTcp = null;

        public ModbusConnector( string ipAddress, int port )
        {
            modbusTcp = new ModbusTcpNet( ipAddress, port, 0x01 );   // 默认站号1
        }


        public ModbusTcpNet ModbusTcp
        {
            get { return modbusTcp; }
        } 

        public bool IsConnectUsing { get; set; }


        public string GuidToken { get; set; }


        public DateTime LastUseTime { get; set; }


        public void Close( )
        {
            modbusTcp.ConnectClose( );
        }


        public void Open( )
        {
            modbusTcp.SetPersistentConnection( );
        }
    }


    #endregion

}
