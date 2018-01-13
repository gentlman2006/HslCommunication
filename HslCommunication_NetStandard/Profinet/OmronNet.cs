using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core;

namespace HslCommunication.Profinet
{

    /***************************************************************************************************
     * 
     *    暂时计划添加Omron的通讯库，但是取决于测试设备
     * 
     *************************************************************************************************/




    /// <summary>
    /// 欧姆龙Plc的通讯类，基于Fins Tcp协议通讯
    /// </summary>
    internal class OmronNet : DoubleModeNetBase
    {
        #region Constructor
        
        /// <summary>
        /// 实例化一个新的对象，需要指定PLC的ip地址和端口号
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public OmronNet(string ipAddress,int port)
        {
            LogHeaderText = "OmronNet";

        }

        #endregion








    }
}
