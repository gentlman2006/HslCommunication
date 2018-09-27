using HslCommunication.Profinet.Melsec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication;
using HslCommunication.Core.Net;
using HslCommunication.ModBus;
using HslCommunication.Profinet.Siemens;
using System.Net.Sockets;
using HslCommunication.BasicFramework;
using HslCommunication.Core;

namespace HslCommunication_Net45.Test.Documentation.Samples.Core
{
    public class NetworkDeviceBase
    {

        #region IDataTransfer Example

        public class DataMy : IDataTransfer
        {
            // 根据对应的设备选择对应的实例化
            // 三菱 RegularByteTransform
            // 西门子 ReverseBytesTransform
            // Modbus及欧姆龙 ReverseWordTransform
            private IByteTransform byteTransform = new RegularByteTransform( );

            public ushort ReadCount => 5;


            public short temperature = 0;  // 温度
            public float press = 0f;       // 压力
            public int others = 0;         // 自定义的其他信息



            public void ParseSource( byte[] Content )
            {
                temperature = byteTransform.TransInt16( Content, 0 );
                press = byteTransform.TransSingle( Content, 2 );
                others = byteTransform.TransInt32( Content, 6 );
            }

            public byte[] ToSource( )
            {
                byte[] buffer = new byte[10];
                byteTransform.TransByte( temperature ).CopyTo( buffer, 0 );
                byteTransform.TransByte( press ).CopyTo( buffer, 2 );
                byteTransform.TransByte( others ).CopyTo( buffer, 6 );
                return buffer;
            }
        }

        #endregion


        public void ReadCustomerExample( )
        {
            #region ReadCustomerExample

            MelsecMcNet melsec = new MelsecMcNet( "192.168.0.100", 6000 );
            OperateResult<DataMy> read = melsec.ReadCustomer<DataMy>( "M100" );
            if (read.IsSuccess)
            {
                // success
                DataMy data = read.Content;
            }
            else
            {
                // failed
                Console.WriteLine( "读取失败：" + read.Message );
            }

            #endregion
        }

        public async void ReadCustomerAsyncExample( )
        {
            #region ReadCustomerAsyncExample

            MelsecMcNet melsec = new MelsecMcNet( "192.168.0.100", 6000 );
            OperateResult<DataMy> read = await melsec.ReadCustomerAsync<DataMy>( "M100" );
            if (read.IsSuccess)
            {
                // success
                DataMy data = read.Content;
            }
            else
            {
                // failed
                Console.WriteLine( "读取失败：" + read.Message );
            }

            #endregion
        }

        public void WriteCustomerExample( )
        {
            #region WriteCustomerExample

            MelsecMcNet melsec = new MelsecMcNet( "192.168.0.100", 6000 );

            DataMy dataMy = new DataMy( );
            dataMy.temperature = 20;
            dataMy.press = 123.456f;
            dataMy.others = 1234232132;

            OperateResult write = melsec.WriteCustomer( "M100" ,dataMy );
            if (write.IsSuccess)
            {
                // success
                Console.WriteLine( "写入成功！" );
            }
            else
            {
                // failed
                Console.WriteLine( "读取失败：" + write.Message );
            }

            #endregion
        }

        public async void WriteCustomerAsyncExample( )
        {
            #region WriteCustomerAsyncExample

            MelsecMcNet melsec = new MelsecMcNet( "192.168.0.100", 6000 );

            DataMy dataMy = new DataMy( );
            dataMy.temperature = 20;
            dataMy.press = 123.456f;
            dataMy.others = 1234232132;

            OperateResult write = await melsec.WriteCustomerAsync( "M100", dataMy );
            if (write.IsSuccess)
            {
                // success
                Console.WriteLine( "写入成功！" );
            }
            else
            {
                // failed
                Console.WriteLine( "读取失败：" + write.Message );
            }

            #endregion
        }

        public void ReadInt16( )
        {
            #region ReadInt16

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            short d100 = melsec_net.ReadInt16( "D100" ).Content;


            // 如果需要判断是否读取成功
            OperateResult<short> R_d100 = melsec_net.ReadInt16( "D100" );
            if (R_d100.IsSuccess)
            {
                // success
                short value = R_d100.Content;
            }
            else
            {
                // failed
            }

            #endregion
        }

        public async void ReadInt16Async( )
        {
            #region ReadInt16Async

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            short d100 = (await melsec_net.ReadInt16Async( "D100" )).Content;


            // 如果需要判断是否读取成功
            OperateResult<short> R_d100 = await melsec_net.ReadInt16Async( "D100" );
            if (R_d100.IsSuccess)
            {
                // success
                short value = R_d100.Content;
            }
            else
            {
                // failed
            }

            #endregion
        }

        public void ReadInt16Array( )
        {
            #region ReadInt16Array

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            short[] d100_109_value = melsec_net.ReadInt16( "D100", 10 ).Content;

            // 如果需要判断是否读取成功
            OperateResult<short[]> R_d100_109_value = melsec_net.ReadInt16( "D100", 10 );
            if (R_d100_109_value.IsSuccess)
            {
                // success
                short value_d100 = R_d100_109_value.Content[0];
                short value_d101 = R_d100_109_value.Content[1];
                short value_d102 = R_d100_109_value.Content[2];
                short value_d103 = R_d100_109_value.Content[3];
                short value_d104 = R_d100_109_value.Content[4];
                short value_d105 = R_d100_109_value.Content[5];
                short value_d106 = R_d100_109_value.Content[6];
                short value_d107 = R_d100_109_value.Content[7];
                short value_d108 = R_d100_109_value.Content[8];
                short value_d109 = R_d100_109_value.Content[9];
            }
            else
            {
                // failed
            }


            #endregion
        }

        public async void ReadInt16ArrayAsync( )
        {
            #region ReadInt16ArrayAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            short[] d100_109_value = (await melsec_net.ReadInt16Async( "D100", 10 )).Content;

            // 如果需要判断是否读取成功
            OperateResult<short[]> R_d100_109_value = await melsec_net.ReadInt16Async( "D100", 10 );
            if (R_d100_109_value.IsSuccess)
            {
                // success
                short value_d100 = R_d100_109_value.Content[0];
                short value_d101 = R_d100_109_value.Content[1];
                short value_d102 = R_d100_109_value.Content[2];
                short value_d103 = R_d100_109_value.Content[3];
                short value_d104 = R_d100_109_value.Content[4];
                short value_d105 = R_d100_109_value.Content[5];
                short value_d106 = R_d100_109_value.Content[6];
                short value_d107 = R_d100_109_value.Content[7];
                short value_d108 = R_d100_109_value.Content[8];
                short value_d109 = R_d100_109_value.Content[9];
            }
            else
            {
                // failed
            }


            #endregion
        }

        public void ReadUInt16( )
        {
            #region ReadUInt16

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            ushort d100 = melsec_net.ReadUInt16( "D100" ).Content;

            // 如果需要判断是否读取成功
            OperateResult<ushort> R_d100 = melsec_net.ReadUInt16( "D100" );
            if (R_d100.IsSuccess)
            {
                // success
                ushort value = R_d100.Content;
            }
            else
            {
                // failed
            }

            #endregion
        }

        public async void ReadUInt16Async( )
        {
            #region ReadUInt16Async

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            ushort d100 = (await melsec_net.ReadUInt16Async( "D100" )).Content;

            // 如果需要判断是否读取成功
            OperateResult<ushort> R_d100 = await melsec_net.ReadUInt16Async( "D100" );
            if (R_d100.IsSuccess)
            {
                // success
                ushort value = R_d100.Content;
            }
            else
            {
                // failed
            }

            #endregion
        }

        public void ReadUInt16Array( )
        {
            #region ReadUInt16Array

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            ushort[] d100_109 = melsec_net.ReadUInt16( "D100", 10 ).Content;

            // 如果需要判断是否读取成功
            OperateResult<ushort[]> R_d100_109 = melsec_net.ReadUInt16( "D100", 10 );
            if (R_d100_109.IsSuccess)
            {
                // success
                ushort value_d100 = R_d100_109.Content[0];
                ushort value_d101 = R_d100_109.Content[1];
                ushort value_d102 = R_d100_109.Content[2];
                ushort value_d103 = R_d100_109.Content[3];
                ushort value_d104 = R_d100_109.Content[4];
                ushort value_d105 = R_d100_109.Content[5];
                ushort value_d106 = R_d100_109.Content[6];
                ushort value_d107 = R_d100_109.Content[7];
                ushort value_d108 = R_d100_109.Content[8];
                ushort value_d109 = R_d100_109.Content[9];
            }
            else
            {
                // failed
            }


            #endregion
        }

        public async void ReadUInt16ArrayAsync( )
        {
            #region ReadUInt16ArrayAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            ushort[] d100_109 = (await melsec_net.ReadUInt16Async( "D100", 10 )).Content;

            // 如果需要判断是否读取成功
            OperateResult<ushort[]> R_d100_109 = await melsec_net.ReadUInt16Async( "D100", 10 );
            if (R_d100_109.IsSuccess)
            {
                // success
                ushort value_d100 = R_d100_109.Content[0];
                ushort value_d101 = R_d100_109.Content[1];
                ushort value_d102 = R_d100_109.Content[2];
                ushort value_d103 = R_d100_109.Content[3];
                ushort value_d104 = R_d100_109.Content[4];
                ushort value_d105 = R_d100_109.Content[5];
                ushort value_d106 = R_d100_109.Content[6];
                ushort value_d107 = R_d100_109.Content[7];
                ushort value_d108 = R_d100_109.Content[8];
                ushort value_d109 = R_d100_109.Content[9];
            }
            else
            {
                // failed
            }


            #endregion
        }

        public void ReadInt32( )
        {
            #region ReadInt32

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            int d100 = melsec_net.ReadInt32( "D100" ).Content;

            // 如果需要判断是否读取成功
            OperateResult<int> R_d100 = melsec_net.ReadInt32( "D100" );
            if (R_d100.IsSuccess)
            {
                // success
                int value = R_d100.Content;
            }
            else
            {
                // failed
            }


            #endregion
        }

        public async void ReadInt32Async( )
        {
            #region ReadInt32Async

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            int d100 = (await melsec_net.ReadInt32Async( "D100" )).Content;

            // 如果需要判断是否读取成功
            OperateResult<int> R_d100 = await melsec_net.ReadInt32Async( "D100" );
            if (R_d100.IsSuccess)
            {
                // success
                int value = R_d100.Content;
            }
            else
            {
                // failed
            }


            #endregion
        }

        public void ReadInt32Array( )
        {
            #region ReadInt32Array

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            int[] d100_119 = melsec_net.ReadInt32( "D100", 10 ).Content;

            // 如果需要判断是否读取成功

            OperateResult<int[]> R_d100_119 = melsec_net.ReadInt32( "D100", 10 );
            if (R_d100_119.IsSuccess)
            {
                // success
                int value_d100 = R_d100_119.Content[0];
                int value_d102 = R_d100_119.Content[1];
                int value_d104 = R_d100_119.Content[2];
                int value_d106 = R_d100_119.Content[3];
                int value_d108 = R_d100_119.Content[4];
                int value_d110 = R_d100_119.Content[5];
                int value_d112 = R_d100_119.Content[6];
                int value_d114 = R_d100_119.Content[7];
                int value_d116 = R_d100_119.Content[8];
                int value_d118 = R_d100_119.Content[9];
            }
            else
            {
                // failed
            }


            #endregion
        }

        public async void ReadInt32ArrayAsync( )
        {
            #region ReadInt32ArrayAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            int[] d100_119 = (await melsec_net.ReadInt32Async( "D100", 10 )).Content;

            // 如果需要判断是否读取成功

            OperateResult<int[]> R_d100_119 = await melsec_net.ReadInt32Async( "D100", 10 );
            if (R_d100_119.IsSuccess)
            {
                // success
                int value_d100 = R_d100_119.Content[0];
                int value_d102 = R_d100_119.Content[1];
                int value_d104 = R_d100_119.Content[2];
                int value_d106 = R_d100_119.Content[3];
                int value_d108 = R_d100_119.Content[4];
                int value_d110 = R_d100_119.Content[5];
                int value_d112 = R_d100_119.Content[6];
                int value_d114 = R_d100_119.Content[7];
                int value_d116 = R_d100_119.Content[8];
                int value_d118 = R_d100_119.Content[9];
            }
            else
            {
                // failed
            }


            #endregion
        }

        public void ReadUInt32( )
        {
            #region ReadUInt32

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            uint d100 = melsec_net.ReadUInt32( "D100" ).Content;

            // 如果需要判断是否读取成功
            OperateResult<uint> R_d100 = melsec_net.ReadUInt32( "D100" );
            if (R_d100.IsSuccess)
            {
                // success
                uint value = R_d100.Content;
            }
            else
            {
                // failed
            }


            #endregion
        }

        public async void ReadUInt32Async( )
        {
            #region ReadUInt32Async

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            uint d100 = (await melsec_net.ReadUInt32Async( "D100" )).Content;

            // 如果需要判断是否读取成功
            OperateResult<uint> R_d100 = await melsec_net.ReadUInt32Async( "D100" );
            if (R_d100.IsSuccess)
            {
                // success
                uint value = R_d100.Content;
            }
            else
            {
                // failed
            }


            #endregion
        }

        public void ReadUInt32Array( )
        {
            #region ReadUInt32Array

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            uint[] d100_119 = melsec_net.ReadUInt32( "D100", 10 ).Content;

            // 如果需要判断是否读取成功

            OperateResult<uint[]> R_d100_119 = melsec_net.ReadUInt32( "D100", 10 );
            if (R_d100_119.IsSuccess)
            {
                uint value_d100 = R_d100_119.Content[0];
                uint value_d102 = R_d100_119.Content[1];
                uint value_d104 = R_d100_119.Content[2];
                uint value_d106 = R_d100_119.Content[3];
                uint value_d108 = R_d100_119.Content[4];
                uint value_d110 = R_d100_119.Content[5];
                uint value_d112 = R_d100_119.Content[6];
                uint value_d114 = R_d100_119.Content[7];
                uint value_d116 = R_d100_119.Content[8];
                uint value_d118 = R_d100_119.Content[9];
            }
            else
            {
                // failed
            }


            #endregion
        }


        public async void ReadUInt32ArrayAsync( )
        {
            #region ReadUInt32ArrayAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            uint[] d100_119 = (await melsec_net.ReadUInt32Async( "D100", 10 )).Content;

            // 如果需要判断是否读取成功

            OperateResult<uint[]> R_d100_119 = await melsec_net.ReadUInt32Async( "D100", 10 );
            if (R_d100_119.IsSuccess)
            {
                uint value_d100 = R_d100_119.Content[0];
                uint value_d102 = R_d100_119.Content[1];
                uint value_d104 = R_d100_119.Content[2];
                uint value_d106 = R_d100_119.Content[3];
                uint value_d108 = R_d100_119.Content[4];
                uint value_d110 = R_d100_119.Content[5];
                uint value_d112 = R_d100_119.Content[6];
                uint value_d114 = R_d100_119.Content[7];
                uint value_d116 = R_d100_119.Content[8];
                uint value_d118 = R_d100_119.Content[9];
            }
            else
            {
                // failed
            }


            #endregion
        }

        public void ReadFloat( )
        {
            #region ReadFloat

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            float d100 = melsec_net.ReadFloat( "D100" ).Content;

            // 如果需要判断是否读取成功
            OperateResult<float> R_d100 = melsec_net.ReadFloat( "D100" );
            if (R_d100.IsSuccess)
            {
                // success
                float value = R_d100.Content;
            }
            else
            {
                // failed
            }

            #endregion
        }


        public async void ReadFloatAsync( )
        {
            #region ReadFloatAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            float d100 = (await melsec_net.ReadFloatAsync( "D100" )).Content;

            // 如果需要判断是否读取成功
            OperateResult<float> R_d100 = await melsec_net.ReadFloatAsync( "D100" );
            if (R_d100.IsSuccess)
            {
                // success
                float value = R_d100.Content;
            }
            else
            {
                // failed
            }

            #endregion
        }

        public void ReadFloatArray( )
        {
            #region ReadFloatArray

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            float[] d100_119 = melsec_net.ReadFloat( "D100", 10 ).Content;

            // 如果需要判断是否读取成功

            OperateResult<float[]> R_d100_119 = melsec_net.ReadFloat( "D100", 10 );
            if (R_d100_119.IsSuccess)
            {
                float value_d100 = R_d100_119.Content[0];
                float value_d102 = R_d100_119.Content[1];
                float value_d104 = R_d100_119.Content[2];
                float value_d106 = R_d100_119.Content[3];
                float value_d108 = R_d100_119.Content[4];
                float value_d110 = R_d100_119.Content[5];
                float value_d112 = R_d100_119.Content[6];
                float value_d114 = R_d100_119.Content[7];
                float value_d116 = R_d100_119.Content[8];
                float value_d118 = R_d100_119.Content[9];
            }
            else
            {
                // failed
            }


            #endregion
        }

        public async void ReadFloatArrayAsync( )
        {
            #region ReadFloatArrayAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            float[] d100_119 = (await melsec_net.ReadFloatAsync( "D100", 10 )).Content;

            // 如果需要判断是否读取成功

            OperateResult<float[]> R_d100_119 = await melsec_net.ReadFloatAsync( "D100", 10 );
            if (R_d100_119.IsSuccess)
            {
                float value_d100 = R_d100_119.Content[0];
                float value_d102 = R_d100_119.Content[1];
                float value_d104 = R_d100_119.Content[2];
                float value_d106 = R_d100_119.Content[3];
                float value_d108 = R_d100_119.Content[4];
                float value_d110 = R_d100_119.Content[5];
                float value_d112 = R_d100_119.Content[6];
                float value_d114 = R_d100_119.Content[7];
                float value_d116 = R_d100_119.Content[8];
                float value_d118 = R_d100_119.Content[9];
            }
            else
            {
                // failed
            }


            #endregion
        }
        
        public void ReadInt64( )
        {
            #region ReadInt64

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            long d100 = melsec_net.ReadInt64( "D100" ).Content;

            // 如果需要判断是否读取成功
            OperateResult<long> R_d100 = melsec_net.ReadInt64( "D100" );
            if (R_d100.IsSuccess)
            {
                // success
                double value = R_d100.Content;
            }
            else
            {
                // failed
            }

            #endregion
        }

        public async void ReadInt64Async( )
        {
            #region ReadInt64Async

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            long d100 = (await melsec_net.ReadInt64Async( "D100" )).Content;

            // 如果需要判断是否读取成功
            OperateResult<long> R_d100 = await melsec_net.ReadInt64Async( "D100" );
            if (R_d100.IsSuccess)
            {
                // success
                double value = R_d100.Content;
            }
            else
            {
                // failed
            }

            #endregion
        }

        public void ReadInt64Array( )
        {
            #region ReadInt64Array

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            long[] d100_139 = melsec_net.ReadInt64( "D100", 10 ).Content;

            // 如果需要判断是否读取成功

            OperateResult<long[]> R_d100_139 = melsec_net.ReadInt64( "D100", 10 );
            if (R_d100_139.IsSuccess)
            {
                long value_d100 = R_d100_139.Content[0];
                long value_d104 = R_d100_139.Content[1];
                long value_d108 = R_d100_139.Content[2];
                long value_d112 = R_d100_139.Content[3];
                long value_d116 = R_d100_139.Content[4];
                long value_d120 = R_d100_139.Content[5];
                long value_d124 = R_d100_139.Content[6];
                long value_d128 = R_d100_139.Content[7];
                long value_d132 = R_d100_139.Content[8];
                long value_d136 = R_d100_139.Content[9];
            }
            else
            {
                // failed
            }


            #endregion
        }
        
        public async void ReadInt64ArrayAsync( )
        {
            #region ReadInt64ArrayAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            long[] d100_139 = (await melsec_net.ReadInt64Async( "D100", 10 )).Content;

            // 如果需要判断是否读取成功

            OperateResult<long[]> R_d100_139 = await melsec_net.ReadInt64Async( "D100", 10 );
            if (R_d100_139.IsSuccess)
            {
                long value_d100 = R_d100_139.Content[0];
                long value_d104 = R_d100_139.Content[1];
                long value_d108 = R_d100_139.Content[2];
                long value_d112 = R_d100_139.Content[3];
                long value_d116 = R_d100_139.Content[4];
                long value_d120 = R_d100_139.Content[5];
                long value_d124 = R_d100_139.Content[6];
                long value_d128 = R_d100_139.Content[7];
                long value_d132 = R_d100_139.Content[8];
                long value_d136 = R_d100_139.Content[9];
            }
            else
            {
                // failed
            }


            #endregion
        }

        public void ReadUInt64( )
        {
            #region ReadUInt64

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            ulong d100 = melsec_net.ReadUInt64( "D100" ).Content;

            // 如果需要判断是否读取成功
            OperateResult<ulong> R_d100 = melsec_net.ReadUInt64( "D100" );
            if (R_d100.IsSuccess)
            {
                // success
                double value = R_d100.Content;
            }
            else
            {
                // failed
            }

            #endregion
        }

        public async void ReadUInt64Async( )
        {
            #region ReadUInt64Async

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            ulong d100 = (await melsec_net.ReadUInt64Async( "D100" )).Content;

            // 如果需要判断是否读取成功
            OperateResult<ulong> R_d100 = await melsec_net.ReadUInt64Async( "D100" );
            if (R_d100.IsSuccess)
            {
                // success
                double value = R_d100.Content;
            }
            else
            {
                // failed
            }

            #endregion
        }

        public void ReadUInt64Array( )
        {
            #region ReadUInt64Array

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            ulong[] d100_139 = melsec_net.ReadUInt64( "D100", 10 ).Content;

            // 如果需要判断是否读取成功

            OperateResult<ulong[]> R_d100_139 = melsec_net.ReadUInt64( "D100", 10 );
            if (R_d100_139.IsSuccess)
            {
                ulong value_d100 = R_d100_139.Content[0];
                ulong value_d104 = R_d100_139.Content[1];
                ulong value_d108 = R_d100_139.Content[2];
                ulong value_d112 = R_d100_139.Content[3];
                ulong value_d116 = R_d100_139.Content[4];
                ulong value_d120 = R_d100_139.Content[5];
                ulong value_d124 = R_d100_139.Content[6];
                ulong value_d128 = R_d100_139.Content[7];
                ulong value_d132 = R_d100_139.Content[8];
                ulong value_d136 = R_d100_139.Content[9];
            }
            else
            {
                // failed
            }


            #endregion
        }

        public async void ReadUInt64ArrayAsync( )
        {
            #region ReadUInt64ArrayAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            ulong[] d100_139 = (await melsec_net.ReadUInt64Async( "D100", 10 )).Content;

            // 如果需要判断是否读取成功

            OperateResult<ulong[]> R_d100_139 = await melsec_net.ReadUInt64Async( "D100", 10 );
            if (R_d100_139.IsSuccess)
            {
                ulong value_d100 = R_d100_139.Content[0];
                ulong value_d104 = R_d100_139.Content[1];
                ulong value_d108 = R_d100_139.Content[2];
                ulong value_d112 = R_d100_139.Content[3];
                ulong value_d116 = R_d100_139.Content[4];
                ulong value_d120 = R_d100_139.Content[5];
                ulong value_d124 = R_d100_139.Content[6];
                ulong value_d128 = R_d100_139.Content[7];
                ulong value_d132 = R_d100_139.Content[8];
                ulong value_d136 = R_d100_139.Content[9];
            }
            else
            {
                // failed
            }


            #endregion
        }

        public void ReadDouble( )
        {
            #region ReadDouble

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            double d100 = melsec_net.ReadDouble( "D100" ).Content;

            // 如果需要判断是否读取成功
            OperateResult<double> R_d100 = melsec_net.ReadDouble( "D100" );
            if (R_d100.IsSuccess)
            {
                // success
                double value = R_d100.Content;
            }
            else
            {
                // failed
            }

            #endregion
        }

        public async void ReadDoubleAsync( )
        {
            #region ReadDoubleAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            double d100 = (await melsec_net.ReadDoubleAsync( "D100" )).Content;

            // 如果需要判断是否读取成功
            OperateResult<double> R_d100 = await melsec_net.ReadDoubleAsync( "D100" );
            if (R_d100.IsSuccess)
            {
                // success
                double value = R_d100.Content;
            }
            else
            {
                // failed
            }

            #endregion
        }

        public void ReadDoubleArray( )
        {
            #region ReadDoubleArray

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            double[] d100_139 = melsec_net.ReadDouble( "D100", 10 ).Content;

            // 如果需要判断是否读取成功

            OperateResult<double[]> R_d100_139 = melsec_net.ReadDouble( "D100", 10 );
            if (R_d100_139.IsSuccess)
            {
                double value_d100 = R_d100_139.Content[0];
                double value_d104 = R_d100_139.Content[1];
                double value_d108 = R_d100_139.Content[2];
                double value_d112 = R_d100_139.Content[3];
                double value_d116 = R_d100_139.Content[4];
                double value_d120 = R_d100_139.Content[5];
                double value_d124 = R_d100_139.Content[6];
                double value_d128 = R_d100_139.Content[7];
                double value_d132 = R_d100_139.Content[8];
                double value_d136 = R_d100_139.Content[9];
            }
            else
            {
                // failed
            }


            #endregion
        }

        public async void ReadDoubleArrayAsync( )
        {
            #region ReadDoubleArrayAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            double[] d100_139 = (await melsec_net.ReadDoubleAsync( "D100", 10 )).Content;

            // 如果需要判断是否读取成功

            OperateResult<double[]> R_d100_139 = await melsec_net.ReadDoubleAsync( "D100", 10 );
            if (R_d100_139.IsSuccess)
            {
                double value_d100 = R_d100_139.Content[0];
                double value_d104 = R_d100_139.Content[1];
                double value_d108 = R_d100_139.Content[2];
                double value_d112 = R_d100_139.Content[3];
                double value_d116 = R_d100_139.Content[4];
                double value_d120 = R_d100_139.Content[5];
                double value_d124 = R_d100_139.Content[6];
                double value_d128 = R_d100_139.Content[7];
                double value_d132 = R_d100_139.Content[8];
                double value_d136 = R_d100_139.Content[9];
            }
            else
            {
                // failed
            }


            #endregion
        }

        public void ReadString( )
        {
            #region ReadString

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            string d100_value = melsec_net.ReadString( "D100", 5 ).Content;

            // 如果需要判断是否读取成功
            OperateResult<string> R_d100_value = melsec_net.ReadString( "D100", 5 );
            if (R_d100_value.IsSuccess)
            {
                // success
                string value = R_d100_value.Content;
            }
            else
            {
                // failed
            }

            #endregion
        }

        public async void ReadStringAsync( )
        {
            #region ReadStringAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 以下是简单的读取，没有仔细校验的方式
            string d100_value = (await melsec_net.ReadStringAsync( "D100", 5 )).Content;

            // 如果需要判断是否读取成功
            OperateResult<string> R_d100_value = await melsec_net.ReadStringAsync( "D100", 5 );
            if (R_d100_value.IsSuccess)
            {
                // success
                string value = R_d100_value.Content;
            }
            else
            {
                // failed
            }

            #endregion
        }

        public async void WriteAsync( )
        {
            #region WriteAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            await melsec_net.WriteAsync( "D100", (short)123 );

            // 如果想要判断是否写入成功
            OperateResult write = await melsec_net.WriteAsync( "D100", (short)123 );
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

        public void WriteInt16( )
        {
            #region WriteInt16

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            melsec_net.Write( "D100", (short)123 );

            // 如果想要判断是否写入成功
            OperateResult write = melsec_net.Write( "D100", (short)123 );
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

        public async void WriteInt16Async( )
        {
            #region WriteInt16Async

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            await melsec_net.WriteAsync( "D100", (short)123 );

            // 如果想要判断是否写入成功
            OperateResult write = await melsec_net.WriteAsync( "D100", (short)123 );
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

        public void WriteInt16Array( )
        {
            #region WriteInt16Array

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            melsec_net.Write( "D100", new short[] { 123, -342, 3535 } );

            // 如果想要判断是否写入成功
            OperateResult write = melsec_net.Write( "D100", new short[] { 123, -342, 3535 } );
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
        public async void WriteInt16ArrayAsync( )
        {
            #region WriteInt16ArrayAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            await melsec_net.WriteAsync( "D100", new short[] { 123, -342, 3535 } );

            // 如果想要判断是否写入成功
            OperateResult write = await melsec_net.WriteAsync( "D100", new short[] { 123, -342, 3535 } );
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

        public void WriteUInt16( )
        {
            #region WriteUInt16

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            melsec_net.Write( "D100", (ushort)123 );

            // 如果想要判断是否写入成功
            OperateResult write = melsec_net.Write( "D100", (ushort)123 );
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

        public async void WriteUInt16Async( )
        {
            #region WriteUInt16Async

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            await melsec_net.WriteAsync( "D100", (ushort)123 );

            // 如果想要判断是否写入成功
            OperateResult write = await melsec_net.WriteAsync( "D100", (ushort)123 );
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

        public void WriteUInt16Array( )
        {
            #region WriteUInt16Array

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            melsec_net.Write( "D100", new ushort[] { 123, 342, 3535 } );

            // 如果想要判断是否写入成功
            OperateResult write = melsec_net.Write( "D100", new ushort[] { 123, 342, 3535 } );
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

        public async void WriteUInt16ArrayAsync( )
        {
            #region WriteUInt16ArrayAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            await melsec_net.WriteAsync( "D100", new ushort[] { 123, 342, 3535 } );

            // 如果想要判断是否写入成功
            OperateResult write = await melsec_net.WriteAsync( "D100", new ushort[] { 123, 342, 3535 } );
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


        public void WriteInt32( )
        {
            #region WriteInt32

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            melsec_net.Write( "D100", 123 );

            // 如果想要判断是否写入成功
            OperateResult write = melsec_net.Write( "D100", 123 );
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

        public async void WriteInt32Async( )
        {
            #region WriteInt32Async

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            await melsec_net.WriteAsync( "D100", 123 );

            // 如果想要判断是否写入成功
            OperateResult write = await melsec_net.WriteAsync( "D100", 123 );
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

        public void WriteInt32Array( )
        {
            #region WriteInt32Array

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            melsec_net.Write( "D100", new int[] { 123, 342, -3535, 1235234 } );

            // 如果想要判断是否写入成功
            OperateResult write = melsec_net.Write( "D100", new int[] { 123, 342, -3535, 1235234 } );
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

        public async void WriteInt32ArrayAsync( )
        {
            #region WriteInt32ArrayAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            await melsec_net.WriteAsync( "D100", new int[] { 123, 342, -3535, 1235234 } );

            // 如果想要判断是否写入成功
            OperateResult write = await melsec_net.WriteAsync( "D100", new int[] { 123, 342, -3535, 1235234 } );
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

        public void WriteUInt32( )
        {
            #region WriteUInt32

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            melsec_net.Write( "D100", (uint)123 );

            // 如果想要判断是否写入成功
            OperateResult write = melsec_net.Write( "D100", (uint)123 );
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

        public async void WriteUInt32Async( )
        {
            #region WriteUInt32Async

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            await melsec_net.WriteAsync( "D100", (uint)123 );

            // 如果想要判断是否写入成功
            OperateResult write = await melsec_net.WriteAsync( "D100", (uint)123 );
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

        public void WriteUInt32Array( )
        {
            #region WriteUInt32Array

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            melsec_net.Write( "D100", new uint[] { 123, 342, 3535, 1235234 } );

            // 如果想要判断是否写入成功
            OperateResult write = melsec_net.Write( "D100", new uint[] { 123, 342, 3535, 1235234 } );
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

        public async void WriteUInt32ArrayAsync( )
        {
            #region WriteUInt32ArrayAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            await melsec_net.WriteAsync( "D100", new uint[] { 123, 342, 3535, 1235234 } );

            // 如果想要判断是否写入成功
            OperateResult write = await melsec_net.WriteAsync( "D100", new uint[] { 123, 342, 3535, 1235234 } );
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
        
        public void WriteFloat( )
        {
            #region WriteFloat

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            melsec_net.Write( "D100", 123.456f );

            // 如果想要判断是否写入成功
            OperateResult write = melsec_net.Write( "D100", 123.456f );
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

        public async void WriteFloatAsync( )
        {
            #region WriteFloatAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            await melsec_net.WriteAsync( "D100", 123.456f );

            // 如果想要判断是否写入成功
            OperateResult write = await melsec_net.WriteAsync( "D100", 123.456f );
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

        public void WriteFloatArray( )
        {
            #region WriteFloatArray

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            melsec_net.Write( "D100", new float[] { 123f, 342.23f, 0.001f, -123.34f } );

            // 如果想要判断是否写入成功
            OperateResult write = melsec_net.Write( "D100", new float[] { 123f, 342.23f, 0.001f, -123.34f } );
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

        public async void WriteFloatArrayAsync( )
        {
            #region WriteFloatArrayAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            await melsec_net.WriteAsync( "D100", new float[] { 123f, 342.23f, 0.001f, -123.34f } );

            // 如果想要判断是否写入成功
            OperateResult write = await melsec_net.WriteAsync( "D100", new float[] { 123f, 342.23f, 0.001f, -123.34f } );
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

        public void WriteInt64( )
        {
            #region WriteInt64

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            melsec_net.Write( "D100", 12334242354L );

            // 如果想要判断是否写入成功
            OperateResult write = melsec_net.Write( "D100", 12334242354L );
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

        public async void WriteInt64Async( )
        {
            #region WriteInt64Async

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            await melsec_net.WriteAsync( "D100", 12334242354L );

            // 如果想要判断是否写入成功
            OperateResult write = await melsec_net.WriteAsync( "D100", 12334242354L );
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

        public void WriteInt64Array( )
        {
            #region WriteInt64Array

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            melsec_net.Write( "D100", new long[] { 123, 342, -352312335, 1235234 } );

            // 如果想要判断是否写入成功
            OperateResult write = melsec_net.Write( "D100", new long[] { 123, 342, -352312335, 1235234 } );
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

        public async void WriteInt64ArrayAsync( )
        {
            #region WriteInt64ArrayAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            await melsec_net.WriteAsync( "D100", new long[] { 123, 342, -352312335, 1235234 } );

            // 如果想要判断是否写入成功
            OperateResult write = await melsec_net.WriteAsync( "D100", new long[] { 123, 342, -352312335, 1235234 } );
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

        public void WriteUInt64( )
        {
            #region WriteUInt64

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            melsec_net.Write( "D100", 12334242354UL );

            // 如果想要判断是否写入成功
            OperateResult write = melsec_net.Write( "D100", 12334242354UL );
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


        public async void WriteUInt64Async( )
        {
            #region WriteUInt64Async

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            await melsec_net.WriteAsync( "D100", 12334242354UL );

            // 如果想要判断是否写入成功
            OperateResult write = await melsec_net.WriteAsync( "D100", 12334242354UL );
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

        public void WriteUInt64Array( )
        {
            #region WriteUInt64Array

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            melsec_net.Write( "D100", new ulong[] { 123, 342, 352312335, 1235234 } );

            // 如果想要判断是否写入成功
            OperateResult write = melsec_net.Write( "D100", new ulong[] { 123, 342, 352312335, 1235234 } );
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
        
        public async void WriteUInt64ArrayAsync( )
        {
            #region WriteUInt64ArrayAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            await melsec_net.WriteAsync( "D100", new ulong[] { 123, 342, 352312335, 1235234 } );

            // 如果想要判断是否写入成功
            OperateResult write = await melsec_net.WriteAsync( "D100", new ulong[] { 123, 342, 352312335, 1235234 } );
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


        public void WriteDouble( )
        {
            #region WriteDouble

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            melsec_net.Write( "D100", 123.456d );

            // 如果想要判断是否写入成功
            OperateResult write = melsec_net.Write( "D100", 123.456d );
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

        public async void WriteDoubleAsync( )
        {
            #region WriteDoubleAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            await melsec_net.WriteAsync( "D100", 123.456d );

            // 如果想要判断是否写入成功
            OperateResult write = await melsec_net.WriteAsync( "D100", 123.456d );
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

        public void WriteDoubleArray( )
        {
            #region WriteDoubleArray

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            melsec_net.Write( "D100", new double[] { 123d, 342.23d, 0.001d, -123.34d } );

            // 如果想要判断是否写入成功
            OperateResult write = melsec_net.Write( "D100", new double[] { 123d, 342.23d, 0.001d, -123.34d } );
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


        public async void WriteDoubleArrayAsync( )
        {
            #region WriteDoubleArrayAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            await melsec_net.WriteAsync( "D100", new double[] { 123d, 342.23d, 0.001d, -123.34d } );

            // 如果想要判断是否写入成功
            OperateResult write = await melsec_net.WriteAsync( "D100", new double[] { 123d, 342.23d, 0.001d, -123.34d } );
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

        public void WriteString( )
        {
            #region WriteString

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            melsec_net.Write( "D100", "ABCDEFGH" );

            // 如果想要判断是否写入成功
            OperateResult write = melsec_net.Write( "D100", "ABCDEFGH" );
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


        public async void WriteStringAsync( )
        {
            #region WriteStringAsync

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            await melsec_net.WriteAsync( "D100", "ABCDEFGH" );

            // 如果想要判断是否写入成功
            OperateResult write = await melsec_net.WriteAsync( "D100", "ABCDEFGH" );
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

        public void WriteString2( )
        {
            #region WriteString2

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            melsec_net.Write( "D100", "ABCDEFGH", 10 );

            // 如果想要判断是否写入成功
            OperateResult write = melsec_net.Write( "D100", "ABCDEFGH", 10 );
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


        public async void WriteString2Async( )
        {
            #region WriteString2Async

            MelsecMcNet melsec_net = new MelsecMcNet( "192.168.0.100", 6000 );

            // 简单的写入
            await melsec_net.WriteAsync( "D100", "ABCDEFGH", 10 );

            // 如果想要判断是否写入成功
            OperateResult write = await melsec_net.WriteAsync( "D100", "ABCDEFGH", 10 );
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
