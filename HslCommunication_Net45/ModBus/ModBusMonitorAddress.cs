using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.ModBus
{
    /// <summary>
    /// 服务器端提供的数据监视服务
    /// </summary>
    public class ModBusMonitorAddress
    {
        /// <summary>
        /// 本次数据监视的地址
        /// </summary>
        public ushort Address { get; set; }

        /// <summary>
        /// 数据写入时触发的事件
        /// </summary>
        public event Action<ModBusMonitorAddress, short> OnWrite;

        /// <summary>
        /// 数据改变时触发的事件
        /// </summary>
        public event Action<ModBusMonitorAddress, short, short> OnChange;

        /// <summary>
        /// 强制设置触发事件
        /// </summary>
        /// <param name="value"></param>
        public void SetValue( short value )
        {
            OnWrite?.Invoke( this, value );
        }

        /// <summary>
        /// 强制设置触发值变更事件
        /// </summary>
        /// <param name="before">变更前的值</param>
        /// <param name="after">变更后的值</param>
        public void SetChangeValue( short before, short after )
        {
            if (before != after)
            {
                OnChange?.Invoke( this, before, after );
            }
        }
    }


}
