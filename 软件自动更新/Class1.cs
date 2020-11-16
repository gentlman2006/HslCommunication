using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Microsoft.Win32;

namespace 软件自动更新
{
    /// <summary>
    /// 软件的更新类
    /// </summary>
    public class SystemUpdate
    {
        /// <summary>
        /// 软件名称
        /// </summary>
        public string SoftName { get; set; } = "";
        /// <summary>
        /// 软件的头描述
        /// </summary>
        public string SoftNameHead { get; set; } = "";
        /// <summary>
        /// 软件的服务器地址
        /// </summary>
        public string ServerIp { get; set; } = "";
        /// <summary>
        /// 服务器的更新端口号，默认为13141
        /// </summary>
        public int ServerPort { get; set; } = 13141;
        /// <summary>
        /// 用于通信验证的网络令牌
        /// </summary>
        public Guid KeyToken { get; set; } = Guid.Empty;


        public static SystemUpdate 设备管理系统 = new SystemUpdate()
        {
            SoftName = "设备管理系统",
            SoftNameHead="",
            ServerIp="117.48.203.204",//171.188.0.182
        };
        public static SystemUpdate 清泉设备管理系统 = new SystemUpdate()
        {
            SoftName = "设备管理系统",
            SoftNameHead = "清泉",
            ServerIp = "171.188.58.7",
        };
        public static SystemUpdate 工程胎硫化机上位机系统 = new SystemUpdate()
        {
            SoftName = "硫化机上位机客户端",
            SoftNameHead = "",
            ServerIp="10.1.124.9",
        };
        public static SystemUpdate 车胎硫化机上位机系统 = new SystemUpdate()
        {
            SoftName= "硫化机上位机客户端",
            SoftNameHead="车胎",
            ServerIp= "171.188.54.177",
        };
        public static SystemUpdate 质量管控系统 = new SystemUpdate()
        {
            SoftName= "质量管控系统客户端",
            SoftNameHead="",
            ServerIp="10.1.63.81",//内测版本暂时的服务器
        };
        public static SystemUpdate 项目管理系统 = new SystemUpdate()
        {
            SoftName = "项目管理系统",
            SoftNameHead = "",
            ServerIp = "10.1.63.37",
            ServerPort = 14578,
        };
        public static SystemUpdate CS项目基础模版 = new SystemUpdate()
        {
            SoftName = "软件系统客户端模版",
            SoftNameHead = "",
            ServerIp = "117.48.203.204",
            ServerPort = 17538,
        };
        public static SystemUpdate Demo项目 = new SystemUpdate( )
        {
            SoftName = "HslCommunicationDemo",
            SoftNameHead = "",
            ServerIp = "118.24.36.220",
            ServerPort = 18468,
        };
    }


    public class Class1
    {
        public static SystemUpdate CurrentSystem = SystemUpdate.Demo项目;
    }
    


}
