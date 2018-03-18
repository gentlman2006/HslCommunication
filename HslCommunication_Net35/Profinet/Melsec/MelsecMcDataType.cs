using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.Melsec
{
    /// <summary>
    /// 三菱PLC的数据类型，此处包含了几个常用的类型
    /// </summary>
    public class MelsecMcDataType
    {
        /// <summary>
        /// 如果您清楚类型代号，可以根据值进行扩展
        /// </summary>
        /// <param name="code">数据类型的代号</param>
        /// <param name="type">0或1，默认为0</param>
        public MelsecMcDataType( byte code, byte type )
        {
            DataCode = code;
            if (type < 2) DataType = type;
        }
        /// <summary>
        /// 类型的代号值
        /// </summary>
        public byte DataCode { get; private set; } = 0x00;
        /// <summary>
        /// 数据的类型，0代表按字，1代表按位
        /// </summary>
        public byte DataType { get; private set; } = 0x00;

        /// <summary>
        /// X输入寄存器
        /// </summary>
        public readonly static MelsecMcDataType X = new MelsecMcDataType( 0x9C, 0x01 );
        /// <summary>
        /// Y输出寄存器
        /// </summary>
        public readonly static MelsecMcDataType Y = new MelsecMcDataType( 0x9D, 0x01 );
        /// <summary>
        /// M中间寄存器
        /// </summary>
        public readonly static MelsecMcDataType M = new MelsecMcDataType( 0x90, 0x01 );
        /// <summary>
        /// D数据寄存器
        /// </summary>
        public readonly static MelsecMcDataType D = new MelsecMcDataType( 0xA8, 0x00 );
        /// <summary>
        /// W链接寄存器
        /// </summary>
        public readonly static MelsecMcDataType W = new MelsecMcDataType( 0xB4, 0x00 );
        /// <summary>
        /// L锁存继电器
        /// </summary>
        public readonly static MelsecMcDataType L = new MelsecMcDataType( 0x92, 0x01 );
        /// <summary>
        /// F报警器
        /// </summary>
        public readonly static MelsecMcDataType F = new MelsecMcDataType( 0x93, 0x01 );
        /// <summary>
        /// V边沿继电器
        /// </summary>
        public readonly static MelsecMcDataType V = new MelsecMcDataType( 0x94, 0x01 );
        /// <summary>
        /// B链接继电器
        /// </summary>
        public readonly static MelsecMcDataType B = new MelsecMcDataType( 0xA0, 0x01 );
        /// <summary>
        /// R文件寄存器
        /// </summary>
        public readonly static MelsecMcDataType R = new MelsecMcDataType( 0xAF, 0x00 );
        /// <summary>
        /// S步进继电器
        /// </summary>
        public readonly static MelsecMcDataType S = new MelsecMcDataType( 0x98, 0x01 );
    }
}
