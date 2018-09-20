using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HslCommunication.Robot.EFORT
{

    /// <summary>
    /// 埃夫特机器人对应型号为ER7B-C10，此协议为定制版，使用前请测试
    /// </summary>
    public class ER7BC10 : NetworkDoubleBase<EFORTMessage, RegularByteTransform>
    {

        #region Constructor

        /// <summary>
        /// 实例化一个默认的对象，并指定IP地址和端口号，端口号通常为8008
        /// </summary>
        /// <param name="ipAddress">Ip地址</param>
        /// <param name="port">端口号</param>
        public ER7BC10( string ipAddress, int port )
        {
            IpAddress = ipAddress;
            Port = port;

            hybirdLock = new SimpleHybirdLock( );                                 // 实例化一个数据锁
        }

        #endregion

        #region Request Create

        /// <summary>
        /// 获取发送的消息的命令
        /// </summary>
        /// <returns>字节数组命令</returns>
        public byte[] GetReadCommand()
        {
            byte[] command = new byte[38];

            Encoding.ASCII.GetBytes( "MessageHead" ).CopyTo( command, 0 );
            BitConverter.GetBytes( (ushort)command.Length ).CopyTo( command, 16 );
            BitConverter.GetBytes( (ushort)1001 ).CopyTo( command, 18 );
            BitConverter.GetBytes( GetHeartBeat( ) ).CopyTo( command, 20 );
            Encoding.ASCII.GetBytes( "MessageTail" ).CopyTo( command, 22);

            return command;
        }

        private ushort GetHeartBeat( )
        {
            ushort result = 0;
            hybirdLock.Enter( );

            result = (ushort)heartbeat;
            heartbeat++;
            if (heartbeat > ushort.MaxValue)
            {
                heartbeat = 0;
            }

            hybirdLock.Leave( );
            return result;
        }

        #endregion

        #region Read Support


        /// <summary>
        /// 读取机器人的详细信息
        /// </summary>
        /// <returns>结果数据信息</returns>
        public OperateResult<EfortData> Read( )
        {
            OperateResult<byte[]> read = ReadFromCoreServer( GetReadCommand( ) );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<EfortData>( read );

            if (read.Content.Length < 788)
                return new OperateResult<EfortData>( string.Format( StringResources.Language.DataLengthIsNotEnough, 788, read.Content.Length ) );

            // 开始解析数据
            EfortData efortData = new EfortData( );
            efortData.PacketStart = Encoding.ASCII.GetString( read.Content, 0, 16 ).Trim( );
            efortData.PacketOrders = BitConverter.ToUInt16( read.Content, 18 );
            efortData.PacketHeartbeat = BitConverter.ToUInt16( read.Content, 20 );
            efortData.ErrorStatus = read.Content[22];
            efortData.HstopStatus = read.Content[23];
            efortData.AuthorityStatus = read.Content[24];
            efortData.ServoStatus = read.Content[25];
            efortData.AxisMoveStatus = read.Content[26];
            efortData.ProgMoveStatus = read.Content[27];
            efortData.ProgLoadStatus = read.Content[28];
            efortData.ProgHoldStatus = read.Content[29];
            efortData.ModeStatus = BitConverter.ToUInt16( read.Content, 30 );
            efortData.SpeedStatus = BitConverter.ToUInt16( read.Content, 32 );

            for (int i = 0; i < 32; i++)
            {
                efortData.IoDOut[i] = read.Content[34 + i];
            }
            for (int i = 0; i < 32; i++)
            {
                efortData.IoDIn[i] = read.Content[66 + i];
            }
            for (int i = 0; i < 32; i++)
            {
                efortData.IoIOut[i] = BitConverter.ToInt32( read.Content, 100 + 4 * i );
            }
            for (int i = 0; i < 32; i++)
            {
                efortData.IoIIn[i] = BitConverter.ToInt32( read.Content, 228 + 4 * i );
            }

            efortData.ProjectName = Encoding.ASCII.GetString( read.Content, 356, 32 ).Trim( '\u0000' );
            efortData.ProgramName = Encoding.ASCII.GetString( read.Content, 388, 32 ).Trim( '\u0000' );
            efortData.ErrorText = Encoding.ASCII.GetString( read.Content, 420, 128 ).Trim( '\u0000' );

            for (int i = 0; i < 7; i++)
            {
                efortData.DbAxisPos[i] = BitConverter.ToSingle( read.Content, 548 + 4 * i );
            }

            for (int i = 0; i < 6; i++)
            {
                efortData.DbCartPos[i] = BitConverter.ToSingle( read.Content, 576 + 4 * i );
            }

            for (int i = 0; i < 7; i++)
            {
                efortData.DbAxisSpeed[i] = BitConverter.ToSingle( read.Content, 600 + 4 * i );
            }

            for (int i = 0; i < 7; i++)
            {
                efortData.DbAxisAcc[i] = BitConverter.ToSingle( read.Content, 628 + 4 * i );
            }

            for (int i = 0; i < 7; i++)
            {
                efortData.DbAxisAccAcc[i] = BitConverter.ToSingle( read.Content, 656 + 4 * i );
            }
            
            for (int i = 0; i < 7; i++)
            {
                efortData.DbAxisTorque[i] = BitConverter.ToSingle( read.Content, 684 + 4 * i );
            }

            for (int i = 0; i < 7; i++)
            {
                efortData.DbAxisDirCnt[i] = BitConverter.ToInt32( read.Content, 712 + 4 * i );
            }

            for (int i = 0; i < 7; i++)
            {
                efortData.DbAxisTime[i] = BitConverter.ToInt32( read.Content, 740 + 4 * i );
            }


            efortData.DbDeviceTime = BitConverter.ToInt32( read.Content, 768 );
            efortData.PacketEnd = Encoding.ASCII.GetString( read.Content, 772, 16 ).Trim( );


            return OperateResult.CreateSuccessResult( efortData );
        }




        #endregion

        #region Private Member

        private int heartbeat = 0;
        private SimpleHybirdLock hybirdLock;             // 心跳值的锁

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString( )
        {
            return $"ER7BC10 Robot[{IpAddress}:{Port}]";
        }

        #endregion
    }
}
