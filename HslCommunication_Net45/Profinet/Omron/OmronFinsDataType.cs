using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.Omron
{
    /// <summary>
    /// 欧姆龙的Fins协议的数据类型
    /// </summary>
    public class OmronFinsDataType
    {
        /// <summary>
        /// 实例化一个Fins的数据类型
        /// </summary>
        /// <param name="bitCode">进行位操作的指令</param>
        /// <param name="wordCode">进行字操作的指令</param>
        public OmronFinsDataType( byte bitCode, byte wordCode )
        {
            BitCode = bitCode;
            WordCode = wordCode;
        }



        /// <summary>
        /// 进行位操作的指令
        /// </summary>
        public byte BitCode { get; private set; }

        /// <summary>
        /// 进行字操作的指令
        /// </summary>
        public byte WordCode { get; private set; }



        /// <summary>
        /// DM Area
        /// </summary>
        public static readonly OmronFinsDataType DM = new OmronFinsDataType( 0x02, 0x82 );
        /// <summary>
        /// CIO Area
        /// </summary>
        public static readonly OmronFinsDataType CIO = new OmronFinsDataType( 0x30, 0xB0 );
        /// <summary>
        /// Work Area
        /// </summary>
        public static readonly OmronFinsDataType WR = new OmronFinsDataType( 0x31, 0xB1 );
        /// <summary>
        /// Holding Bit Area
        /// </summary>
        public static readonly OmronFinsDataType HR = new OmronFinsDataType( 0x32, 0xB2 );
        /// <summary>
        /// Auxiliary Bit Area
        /// </summary>
        public static readonly OmronFinsDataType AR = new OmronFinsDataType( 0x33, 0xB3 );
    }
}
