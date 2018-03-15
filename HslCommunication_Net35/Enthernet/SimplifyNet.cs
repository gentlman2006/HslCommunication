using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core;

namespace HslCommunication.Enthernet
{


    /// <summary>
    /// 简单客户端的类
    /// </summary>
    public sealed class SimplifyNetClient : NetworkDoubleBase<HslMessage, RegularByteTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        public SimplifyNetClient( )
        {

        }



        #endregion



        #region Object Override

        /// <summary>
        /// 获取本对象的字符串表示形式
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString( )
        {
            return "SimplifyNetClient";
        }

        #endregion
    }
}
