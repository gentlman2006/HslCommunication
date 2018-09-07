using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication;
using HslCommunication.ModBus;

namespace HslCommunication_Net45.Test.Documentation.Samples.Modbus
{
    public class ModbusTcpNetExample
    {

        #region Example1
        
        // 本类支持的读写操作提供了非常多的重载方法，总有你想要的方法
        private ModbusTcpNet modbus = new ModbusTcpNet( "192.168.0.1" );   // 实例化



        private void CoilExample()
        {
            // 读取线圈示例
            bool coil100 = modbus.ReadCoil( "100" ).Content;

            // 判断是否读取成功
            OperateResult<bool> result_coil100 = modbus.ReadCoil( "100" );
            if(result_coil100.IsSuccess)
            {
                // success
                bool value = result_coil100.Content;
            }
            else
            {
                // failed
            }


            // 假设读取站号10的线圈100的值
            bool coil_station_ten_100 = modbus.ReadCoil( "s=10;100" ).Content;



            // =============================================================================================
            // 写入也是同理，线圈100写通
            modbus.WriteCoil( "100", true );

            // 站号10的线圈写通
            modbus.WriteCoil( "s=10;100", true );

            // 想要判断是否写入成功
            if(modbus.WriteCoil( "s=10;100", true ).IsSuccess)
            {
                // success
            }
            else
            {
                // failed
            }



            // ===========================================================================================
            // 批量读写也是类似，批量的读取
            bool[] coil10_19 = modbus.ReadCoil( "100", 10 ).Content;

            // 写入也是同理
            modbus.WriteCoil( "100", new bool[] { true, false, true, false, false, false, true, false, false, false } );
            

            // 离散输入的数据读取同理
        }


        private void RegisterExample( )
        {
            // 读取寄存器100的值
            short register100 = modbus.ReadInt16( "100" ).Content;

            // 批量读取寄存器100-109的值
            short[] register100_109 = modbus.ReadInt16( "100", 10 ).Content;

            // 写入寄存器100的值，注意，一定要强制转换short类型
            modbus.Write( "100", (short)123 );

            // 批量写
            modbus.Write( "100", new short[] { 123, -123, 4244 } );


            // ==============================================================================================
            // 以下是一些常规的操作，不再对是否成功的结果进行判断
            // 读取操作

            bool coil100 = modbus.ReadCoil( "100" ).Content;   // 读取线圈100的通断
            short short100 = modbus.ReadInt16( "100" ).Content; // 读取寄存器100的short值
            ushort ushort100 = modbus.ReadUInt16( "100" ).Content; // 读取寄存器100的ushort值
            int int100 = modbus.ReadInt32( "100" ).Content;      // 读取寄存器100-101的int值
            uint uint100 = modbus.ReadUInt32( "100" ).Content;   // 读取寄存器100-101的uint值
            float float100 = modbus.ReadFloat( "100" ).Content; // 读取寄存器100-101的float值
            long long100 = modbus.ReadInt64( "100" ).Content;    // 读取寄存器100-103的long值
            ulong ulong100 = modbus.ReadUInt64( "100" ).Content; // 读取寄存器100-103的ulong值
            double double100 = modbus.ReadDouble( "100" ).Content; // 读取寄存器100-103的double值
            string str100 = modbus.ReadString( "100", 5 ).Content;// 读取100到104共10个字符的字符串
            
            // 写入操作
            modbus.WriteCoil( "100", true );// 写入线圈100为通
            modbus.Write( "100", (short)12345 );// 写入寄存器100为12345
            modbus.Write( "100", (ushort)45678 );// 写入寄存器100为45678
            modbus.Write( "100", 123456789 );// 写入寄存器100-101为123456789
            modbus.Write( "100", (uint)123456778 );// 写入寄存器100-101为123456778
            modbus.Write( "100", 123.456 );// 写入寄存器100-101为123.456
            modbus.Write( "100", 12312312312414L );//写入寄存器100-103为一个大数据
            modbus.Write( "100", 12634534534543656UL );// 写入寄存器100-103为一个大数据
            modbus.Write( "100", 123.456d );// 写入寄存器100-103为一个双精度的数据
            modbus.Write( "100", "K123456789" );

            // ===============================================================================================
            // 读取输入寄存器
            short input_short100 = modbus.ReadInt16( "x=4;100" ).Content; // 读取寄存器100的short值
            ushort input_ushort100 = modbus.ReadUInt16( "x=4;100" ).Content; // 读取寄存器100的ushort值
            int input_int100 = modbus.ReadInt32( "x=4;100" ).Content;      // 读取寄存器100-101的int值
            uint input_uint100 = modbus.ReadUInt32( "x=4;100" ).Content;   // 读取寄存器100-101的uint值
            float input_float100 = modbus.ReadFloat( "x=4;100" ).Content; // 读取寄存器100-101的float值
            long input_long100 = modbus.ReadInt64( "x=4;100" ).Content;    // 读取寄存器100-103的long值
            ulong input_ulong100 = modbus.ReadUInt64( "x=4;100" ).Content; // 读取寄存器100-103的ulong值
            double input_double100 = modbus.ReadDouble( "x=4;100" ).Content; // 读取寄存器100-103的double值
            string input_str100 = modbus.ReadString( "x=4;100", 5 ).Content;// 读取100到104共10个字符的字符串
        }





        #endregion

        private void ReadExample( )
        {
            #region ReadExample1

            ModbusTcpNet modbus = new ModbusTcpNet( "192.168.0.1" );   // 实例化

            // 假设100存储了short的报警，101,102存储了float的温度，103，104存储了int的产量
            OperateResult<byte[]> read = modbus.Read( "100", 5 );
            if(read.IsSuccess)
            {
                // 共计10个字节的结果内容
                short alarm = modbus.ByteTransform.TransInt16( read.Content, 0 );
                float temp = modbus.ByteTransform.TransSingle( read.Content, 2 );
                int product = modbus.ByteTransform.TransInt32( read.Content, 6 );
            }
            else
            {
                // failed
            }


            #endregion
        }

        private void WriteExample( )
        {
            #region WriteExample1

            ModbusTcpNet modbus = new ModbusTcpNet( "192.168.0.1" );   // 实例化

            // 假设100存储了short的报警，101,102存储了float的温度，103，104存储了int的产量
            byte[] buffer = new byte[10];
            modbus.ByteTransform.TransByte( (short)1 ).CopyTo( buffer, 0 );
            modbus.ByteTransform.TransByte( 123.456f ).CopyTo( buffer, 2 );
            modbus.ByteTransform.TransByte( 45678922 ).CopyTo( buffer, 6 );

            OperateResult write = modbus.Write( "100", buffer );
            if (write.IsSuccess)
            {
                // success
            }
            else
            {
                // failed
            }


            #endregion
        }
    }

    public class ModbusRtuExample
    {

        #region Example2

        // 本类支持的读写操作提供了非常多的重载方法，总有你想要的方法
        private ModbusRtu modbus = new ModbusRtu( );   // 实例化



        private void CoilExample( )
        {
            // 读取线圈示例
            bool coil100 = modbus.ReadCoil( "100" ).Content;

            // 判断是否读取成功
            OperateResult<bool> result_coil100 = modbus.ReadCoil( "100" );
            if (result_coil100.IsSuccess)
            {
                // success
                bool value = result_coil100.Content;
            }
            else
            {
                // failed
            }


            // 假设读取站号10的线圈100的值
            bool coil_station_ten_100 = modbus.ReadCoil( "s=10;100" ).Content;



            // =============================================================================================
            // 写入也是同理，线圈100写通
            modbus.WriteCoil( "100", true );

            // 站号10的线圈写通
            modbus.WriteCoil( "s=10;100", true );

            // 想要判断是否写入成功
            if (modbus.WriteCoil( "s=10;100", true ).IsSuccess)
            {
                // success
            }
            else
            {
                // failed
            }



            // ===========================================================================================
            // 批量读写也是类似，批量的读取
            bool[] coil10_19 = modbus.ReadCoil( "100", 10 ).Content;

            // 写入也是同理
            modbus.WriteCoil( "100", new bool[] { true, false, true, false, false, false, true, false, false, false } );


            // 离散输入的数据读取同理
        }


        private void RegisterExample( )
        {
            // 读取寄存器100的值
            short register100 = modbus.ReadInt16( "100" ).Content;

            // 批量读取寄存器100-109的值
            short[] register100_109 = modbus.ReadInt16( "100", 10 ).Content;

            // 写入寄存器100的值，注意，一定要强制转换short类型
            modbus.Write( "100", (short)123 );

            // 批量写
            modbus.Write( "100", new short[] { 123, -123, 4244 } );


            // ==============================================================================================
            // 以下是一些常规的操作，不再对是否成功的结果进行判断
            // 读取操作

            bool coil100 = modbus.ReadCoil( "100" ).Content;   // 读取线圈100的通断
            short short100 = modbus.ReadInt16( "100" ).Content; // 读取寄存器100的short值
            ushort ushort100 = modbus.ReadUInt16( "100" ).Content; // 读取寄存器100的ushort值
            int int100 = modbus.ReadInt32( "100" ).Content;      // 读取寄存器100-101的int值
            uint uint100 = modbus.ReadUInt32( "100" ).Content;   // 读取寄存器100-101的uint值
            float float100 = modbus.ReadFloat( "100" ).Content; // 读取寄存器100-101的float值
            long long100 = modbus.ReadInt64( "100" ).Content;    // 读取寄存器100-103的long值
            ulong ulong100 = modbus.ReadUInt64( "100" ).Content; // 读取寄存器100-103的ulong值
            double double100 = modbus.ReadDouble( "100" ).Content; // 读取寄存器100-103的double值
            string str100 = modbus.ReadString( "100", 5 ).Content;// 读取100到104共10个字符的字符串

            // 写入操作
            modbus.WriteCoil( "100", true );// 写入线圈100为通
            modbus.Write( "100", (short)12345 );// 写入寄存器100为12345
            modbus.Write( "100", (ushort)45678 );// 写入寄存器100为45678
            modbus.Write( "100", 123456789 );// 写入寄存器100-101为123456789
            modbus.Write( "100", (uint)123456778 );// 写入寄存器100-101为123456778
            modbus.Write( "100", 123.456 );// 写入寄存器100-101为123.456
            modbus.Write( "100", 12312312312414L );//写入寄存器100-103为一个大数据
            modbus.Write( "100", 12634534534543656UL );// 写入寄存器100-103为一个大数据
            modbus.Write( "100", 123.456d );// 写入寄存器100-103为一个双精度的数据
            modbus.Write( "100", "K123456789" );

            // ===============================================================================================
            // 读取输入寄存器
            short input_short100 = modbus.ReadInt16( "x=4;100" ).Content; // 读取寄存器100的short值
            ushort input_ushort100 = modbus.ReadUInt16( "x=4;100" ).Content; // 读取寄存器100的ushort值
            int input_int100 = modbus.ReadInt32( "x=4;100" ).Content;      // 读取寄存器100-101的int值
            uint input_uint100 = modbus.ReadUInt32( "x=4;100" ).Content;   // 读取寄存器100-101的uint值
            float input_float100 = modbus.ReadFloat( "x=4;100" ).Content; // 读取寄存器100-101的float值
            long input_long100 = modbus.ReadInt64( "x=4;100" ).Content;    // 读取寄存器100-103的long值
            ulong input_ulong100 = modbus.ReadUInt64( "x=4;100" ).Content; // 读取寄存器100-103的ulong值
            double input_double100 = modbus.ReadDouble( "x=4;100" ).Content; // 读取寄存器100-103的double值
            string input_str100 = modbus.ReadString( "x=4;100", 5 ).Content;// 读取100到104共10个字符的字符串
        }





        #endregion

        private void ReadExample( )
        {
            #region ReadExample2

            ModbusRtu modbus = new ModbusRtu( );   // 实例化
            // 此处忽略初始化
            // modbus.SerialPortInni( "COM3" );

            // 假设100存储了short的报警，101,102存储了float的温度，103，104存储了int的产量
            OperateResult<byte[]> read = modbus.Read( "100", 5 );
            if (read.IsSuccess)
            {
                // 共计10个字节的结果内容
                short alarm = modbus.ByteTransform.TransInt16( read.Content, 0 );
                float temp = modbus.ByteTransform.TransSingle( read.Content, 2 );
                int product = modbus.ByteTransform.TransInt32( read.Content, 6 );
            }
            else
            {
                // failed
            }


            #endregion
        }

        private void WriteExample( )
        {
            #region WriteExample2

            ModbusRtu modbus = new ModbusRtu( );   // 实例化
            // 此处忽略初始化
            // modbus.SerialPortInni( "COM3" );

            // 假设100存储了short的报警，101,102存储了float的温度，103，104存储了int的产量
            byte[] buffer = new byte[10];
            modbus.ByteTransform.TransByte( (short)1 ).CopyTo( buffer, 0 );
            modbus.ByteTransform.TransByte( 123.456f ).CopyTo( buffer, 2 );
            modbus.ByteTransform.TransByte( 45678922 ).CopyTo( buffer, 6 );

            OperateResult write = modbus.Write( "100", buffer );
            if (write.IsSuccess)
            {
                // success
            }
            else
            {
                // failed
            }


            #endregion
        }
    }
}
