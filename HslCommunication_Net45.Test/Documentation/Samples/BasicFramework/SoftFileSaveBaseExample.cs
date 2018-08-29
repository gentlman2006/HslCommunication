using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication_Net45.Test.Documentation.Samples.BasicFramework
{
    #region SoftFileSaveBase1

    // 要想使用该类进行一些数据的存储，必须继承实现下面的方法。


    /// <summary>
    /// 这是一个软件的配置信息存储功能的示例，假设我要存储两个数据，服务器的ip地址和端口号，一个string，一个int
    /// </summary>
    public class SoftSettings : HslCommunication.BasicFramework.SoftFileSaveBase
    {
        /// <summary>
        /// 必须重写这个方法，返回的数据就真正保存到txt的数据
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToSaveString( )
        {
            // 此处举例采用xml格式存储
            System.Xml.Linq.XElement element = new System.Xml.Linq.XElement( "Settings" );

            element.Add( new System.Xml.Linq.XElement( nameof( IpAddress ), IpAddress ) );
            element.Add( new System.Xml.Linq.XElement( nameof( Port ), Port.ToString() ) );

            return element.ToString( );
        }

        /// <summary>
        /// 必须重写这个方法，这个方法里应当包含怎么解析文件的数据
        /// </summary>
        /// <param name="content"></param>
        public override void LoadByString( string content )
        {
            // 上面的存储是使用xml的方式的，所以此处解析也要对照
            System.Xml.Linq.XElement element = System.Xml.Linq.XElement.Parse( content );

            IpAddress = element.Element( nameof( IpAddress ) ).Value;
            Port = int.Parse(element.Element( nameof( Port ) ).Value);
        }



        // 这里就是我们实际需要存储和解析的属性了

        /// <summary>
        /// IP地址数据
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// 端口数据
        /// </summary>
        public int Port { get; set; }
    }

    #endregion

    #region SoftFileSaveBase2

    /// <summary>
    /// 这是一个软件的配置信息存储功能的示例，假设我要存储两个数据，服务器的ip地址和端口号，一个string，一个int，存储效果为加密
    /// </summary>
    public class SoftSettings2 : HslCommunication.BasicFramework.SoftFileSaveBase
    {
        /// <summary>
        /// 必须重写这个方法，返回的数据就真正保存到txt的数据
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToSaveString( )
        {
            // 此处举例采用xml格式存储
            System.Xml.Linq.XElement element = new System.Xml.Linq.XElement( "Settings" );

            element.Add( new System.Xml.Linq.XElement( nameof( IpAddress ), IpAddress ) );
            element.Add( new System.Xml.Linq.XElement( nameof( Port ), Port.ToString( ) ) );

            return element.ToString( );
        }

        /// <summary>
        /// 必须重写这个方法，这个方法里应当包含怎么解析文件的数据
        /// </summary>
        /// <param name="content"></param>
        public override void LoadByString( string content )
        {
            // 上面的存储是使用xml的方式的，所以此处解析也要对照
            System.Xml.Linq.XElement element = System.Xml.Linq.XElement.Parse( content );

            IpAddress = element.Element( nameof( IpAddress ) ).Value;
            Port = int.Parse( element.Element( nameof( Port ) ).Value );
        }


        public override void SaveToFile( )
        {
            // 采用了des加密存储了数据
            base.SaveToFile( m => HslCommunication.BasicFramework.SoftSecurity.MD5Encrypt( m, "12345678" ) );
        }

        public override void LoadByFile( )
        {
            // 采用了des加密解密了数据
            base.LoadByFile( m => HslCommunication.BasicFramework.SoftSecurity.MD5Decrypt( m, "12345678" ) );
        }

        // 这里就是我们实际需要存储和解析的属性了

        /// <summary>
        /// IP地址数据
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// 端口数据
        /// </summary>
        public int Port { get; set; }
    }

    #endregion

    public class Example2
    {
        public Example2( )
        {
            #region Example


            // 初始化，需要加载一次数据
            SoftSettings softSettings = new SoftSettings( );
            softSettings.FileSavePath = "settings.txt";
            softSettings.LoadByFile( );


            // 当你修改了属性的数据后，就需要调用保存一次，才会保存成功
            softSettings.IpAddress = "192.168.0.100";
            softSettings.Port = 1000;
            softSettings.SaveToFile( );


            #endregion
        }
    }

}
