## Summary
本篇文章主要讲解如何与西门子PLC进行数据交互，有两个协议可供个选择，S7协议和Fetch/Write协议，S7不需要在PLC端进行
做参数配置，而Fetch/Write协议需要在PLC端的网络模块进行一些参数的配置，两个读写方式几乎是一致的，只是实例化的时候有些区别，
所以本文下面的读写代码对两个协议都是适用的。

This article mainly explains how to interact with Siemens PLC data. There are two protocols available for selection. 
S7 protocol and Fetch/Write protocol, S7 does not need to configure parameters on the PLC side, 
and Fetch/Write protocol needs to be on the PLC side. The network module carries out the configuration of some parameters.
The two reading and writing modes are almost the same, but there are some differences in the instantiation.
Therefore, the following reading and writing code of this article is applicable to both protocols.

## Contact
QQ Group : [592132877](http://shang.qq.com/wpa/qunwpa?idkey=2278cb9c2e0c04fc305c43e41acff940499a34007dfca9e83a7291e726f9c4e8)

Email: hsl200909@163.com

## Prepare
如果使用了Fetch/Write协议，那么需要参考下面的图片信息进行配置PLC信息参数

If you use the Fetch/Write protocol, you need to refer to the following picture information to configure the PLC information parameters.

![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/siemens/siemens1.jpg)
![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/siemens/siemens2.jpg)
![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/siemens/siemens3.jpg)
![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/siemens/siemens4.jpg)
![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/siemens/siemens5.jpg)
![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/siemens/siemens6.jpg)


## Instantiation
**1. Add namespace**

```
using HslCommunication.Profinet.Siemens;
using HslCommunication;

```

**2. Declare**
```
private SiemensS7Net siemensTcpNet = null;
```
if you want to us Fetch/Write to communicate
```
private SiemensFetchWriteNet siemensFWNet = null;
```

**3. Instantiation**

```

// specify plc type ip address
siemensTcpNet = new SiemensS7Net( siemensPLCS ,"192.168.0.100");
siemensTcpNet.ConnectServer();

```

If we want to know whether connectd, we can do like this

```

OperateResult connect = siemensTcpNet.ConnectServer( );
if (connect.IsSuccess)
{
    // success
}
else
{
    // failed
}

```

**4. Close the connection when the program exits**

```

 siemensTcpNet.ConnectClose( );

```

## Exchange Data

**1. Read Or Write Bool Data**
```
OperateResult<bool> read = siemensTcpNet.ReadBool( "M100.4" );
if (read.IsSuccess)
{
    bool m100_4 = read.Content;
}
else
{
    // failed
    string err = read.Message;
}
```
```
OperateResult write = siemensTcpNet.Write( "M100.4", true );
if(write.IsSuccess)
{
    // success
}
else
{
    // failed
    string err = write.Message;
}
```
> Fetch/Write Not Support Bit Operate

**2. Read M100 Include Many Data Types**
```
byte m100_byte = siemensTcpNet.ReadByte( "M100" ).Content;
short m100_short = siemensTcpNet.ReadInt16( "M100" ).Content;
ushort m100_ushort = siemensTcpNet.ReadUInt16( "M100" ).Content;
int m100_int = siemensTcpNet.ReadInt32( "M100" ).Content;
uint m100_uint = siemensTcpNet.ReadUInt32( "M100" ).Content;
float m100_float = siemensTcpNet.ReadFloat( "M100" ).Content;
double m100_double = siemensTcpNet.ReadDouble( "M100" ).Content;
// need to specify the text length
string m100_string = siemensTcpNet.ReadString( "M100", 10 ).Content;
```

**3. Write M100 Include Many Data Types**
```
siemensTcpNet.Write( "M100", (short)5 );
siemensTcpNet.Write( "M100", (ushort)5 );
siemensTcpNet.Write( "M100", 5 );
siemensTcpNet.Write( "M100", (uint)5 );
siemensTcpNet.Write( "M100", (long)5 );
siemensTcpNet.Write( "M100", (ulong)5 );
siemensTcpNet.Write( "M100", 5f );
siemensTcpNet.Write( "M100", 5d );
siemensTcpNet.Write( "M100", "12345678" );
```


> Not only  'M' dataType, but also 'I','Q','DB', **DB block should like this:"DB20.30"**

> This library alse support write array values.

**4. Read complex data, for example, M100-M119 contains all data you want**

Data name | Data section | Data type | Data Length
-|-|-|-
count | M100-M103 | int | 4-byte
temp | M104-M107 | float | 4-byte
name1 | M108-M109 | short | 2-byte
barcode | M110-M119 | string | 10-byte

So we can do like this

```

OperateResult<byte[]> read = siemensTcpNet.Read( "M100", 20 );
if(read.IsSuccess)
{
    int count = siemensTcpNet.ByteTransform.TransInt32( read.Content, 0 );
    float temp = siemensTcpNet.ByteTransform.TransSingle( read.Content, 4 );
    short name1 = siemensTcpNet.ByteTransform.TransInt16( read.Content, 8 );
    string barcode = Encoding.ASCII.GetString( read.Content, 10, 10 );
}


```




**5. Implementing custom type reads and writes**

We found the code above is awkward and we want to improve.

First, Inherit and implement interface methods

```

public class UserType : HslCommunication.IDataTransfer
{
    #region IDataTransfer

    private HslCommunication.Core.IByteTransform ByteTransform = new HslCommunication.Core.ReverseBytesTransform( );


    public ushort ReadCount => 20;

    public void ParseSource( byte[] Content )
    {
        int count = ByteTransform.TransInt32( Content, 0 );
        float temp = ByteTransform.TransSingle( Content, 4 );
        short name1 = ByteTransform.TransInt16( Content, 8 );
        string barcode = Encoding.ASCII.GetString( Content, 10, 10 );
    }

    public byte[] ToSource( )
    {
        byte[] buffer = new byte[20];
        ByteTransform.TransByte( count ).CopyTo( buffer, 0 );
        ByteTransform.TransByte( temp ).CopyTo( buffer, 4 );
        ByteTransform.TransByte( name1 ).CopyTo( buffer, 8 );
        Encoding.ASCII.GetBytes( barcode ).CopyTo( buffer, 10 );
        return buffer;
    }


    #endregion


    #region Public Data

    public int count { get; set; }

    public float temp { get; set; }

    public short name1 { get; set; }

    public string barcode { get; set; }

    #endregion
}

```

So we can do like this

```

OperateResult<UserType> read = siemensTcpNet.ReadCustomer<UserType>( "M100" );
if (read.IsSuccess)
{
    UserType value = read.Content;
}
// write value
siemensTcpNet.WriteCustomer( "M100", new UserType( ) );

```


## Supported data types and examples
Data Name | Data Type | Address Format | Example Address
-|-|-|-
M | byte | Decimal | M100, M200
I | byte | Decimal | I100, I200
Q | byte | Decimal | Q100, Q200
DB | byte | Decimal | DB100.20, DB200.100.4
T | byte | Decimal | T100, T200
C | byte | Decimal | C100, C200


## Log Support

This component can also achieve log output. Here is an example. The specific log function refers to the logbook.

```
siemensTcpNet.LogNet = new HslCommunication.LogNet.LogNetSingle( Application.StartupPath + "\\Logs.txt" );
```



## Others
For more details, you can download source code, refer to HslCommunicationDemo project