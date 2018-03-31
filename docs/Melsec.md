## Summary
本篇文章主要讲解如何与三菱PLC进行数据交互，使用协议MC协议，目前支持Qna兼容3E帧的二进制通讯和ASCII通讯，
更多的协议帧支持以后再说，在和PLC通讯前，需要现在PLC侧进行网络参数的配置，配置成功后才能保证通讯成功。

This article mainly explains how to interact with Mitsubishi PLC data, using the protocol MC protocol, 
currently supports Qna compatible 3E frame binary communication and ASCII communication, 
more protocol frame support will be said later, before communication with PLC, 
need PLC side Configure the network parameters. After the configuration is successful, the communication can be guaranteed.

## Contact
QQ Group : [592132877](http://shang.qq.com/wpa/qunwpa?idkey=2278cb9c2e0c04fc305c43e41acff940499a34007dfca9e83a7291e726f9c4e8)
Email: hsl200909@163.com

## Prepare
在三菱PLC侧进行相关的参数配置，步骤参照下面的图片

![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/melsec/melsec2.png)
![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/melsec/melsec1.png)


## Instantiation
1. Add namespace
<pre>
<code>
using HslCommunication.Profinet.Melsec;
using HslCommunication;
</code>
</pre>

2. Declare
<pre>
<code>
private MelsecMcNet melsec_net = null;
</code>
</pre>

3. Instantiation
<pre>
<code>
 // specify plc ip address and port
 melsec_net = new MelsecMcNet("192.168.0.110",2000);
 melsec_net.ConnectServer();
</code>
</pre>

If we want to know whether connectd, we can do like this
<pre>
<code>
OperateResult connect = melsec_net.ConnectServer( );
if (connect.IsSuccess)
{
    // success
}
else
{
    // failed
}
</code>
</pre>

4. Closed when exsis programe
<pre>
<code>
 melsec_net.ConnectClose( );
</code>
</pre>

## Exchange Data

Show some examples read from plc and write into plc

1. Read M100 to M109
<pre>
<code>
OperateResult<bool[]> read = melsec_net.ReadBool( "M100", 10 );
if(read.IsSuccess)
{
    bool m100 = read.Content[0];
    // and so on
    bool m109 = read.Content[9];
}
else
{
    // failed
}
</code>
</pre>

2. Write M100 to M109
<pre>
<code>
bool[] values = new bool[] { true, false, true, true, false, true, false, true, true, false };
OperateResult read = melsec_net.Write( "M100", values );
if (read.IsSuccess)
{
    // success
}
else
{
    // failed
}
</code>
</pre>

Not only  'M' dataType, but also 'X','Y','L','F','V','B','S' 

3. Read D100 Include Many Data Types
<pre>
<code>
short d100_short = melsec_net.ReadInt16( "D100" ).Content;
ushort d100_ushort = melsec_net.ReadUInt16( "D100" ).Content;
int d100_int = melsec_net.ReadInt32( "D100" ).Content;
uint d100_uint = melsec_net.ReadUInt32( "D100" ).Content;
long d100_long = melsec_net.ReadInt64( "D100" ).Content;
ulong d100_ulong = melsec_net.ReadUInt64( "D100" ).Content;
float d100_float = melsec_net.ReadFloat( "D100" ).Content;
double d100_double = melsec_net.ReadDouble( "D100" ).Content;
// need to specify the text length
string d100_string = melsec_net.ReadString( "D100", 10 ).Content;
</code>
</pre>

4. Write D100 Include Many Data Types
<pre>
<code>
melsec_net.Write( "D100", (short)5 );
melsec_net.Write( "D100", (ushort)5 );
melsec_net.Write( "D100", 5 );
melsec_net.Write( "D100", (uint)5 );
melsec_net.Write( "D100", (long)5 );
melsec_net.Write( "D100", (ulong)5 );
melsec_net.Write( "D100", 5f );
melsec_net.Write( "D100", 5d );
// length should Multiples of 2 
melsec_net.Write( "D100", "12345678" );
</code>
</pre>

Not only  'D' dataType, but also 'D','W','R'

