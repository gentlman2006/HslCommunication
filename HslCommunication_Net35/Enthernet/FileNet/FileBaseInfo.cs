using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Enthernet
{
    /// <summary>
    /// 文件的基础信息
    /// </summary>
    public class FileBaseInfo
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 文件的标识，注释
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 文件上传人的名称
        /// </summary>
        public string Upload { get; set; }
    }
}
