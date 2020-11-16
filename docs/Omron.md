## Summary
本篇文章主要讲解如何与欧姆龙PLC进行数据交互，采用FINS协议来完成，可以很简单的实现数据交互，如果发现写入数据失败，
那么需要在PLC上设置成可以写入的状态。

This article mainly explains how to interact with Omron PLC data, using the FINS protocol to complete the data exchange can be very simple, 
if it is found that writing data fails, then need to be set in the PLC can be written into the state.

## Contact
QQ Group : [592132877](http://shang.qq.com/wpa/qunwpa?idkey=2278cb9c2e0c04fc305c43e41acff940499a34007dfca9e83a7291e726f9c4e8)

Email: hsl200909@163.com

## Prepare

You should know the IP address of the PLC and the IP address of the computer


## Instantiation
**1. Add namespace**

```
using HslCommunication.Profinet.Omron;
using HslCommunication;

```

**2. Declare**
```
private OmronFinsNet omronFinsNet = null;
```

**3. Instantiation**

```

// specify ip address and port
omronFinsNet = new OmronFinsNet( "192.168.0.100", 6000 );
// specify plc destination unit address, always 0x00
omronFinsNet.DA2 = 0x00;
// specify plc destination node address, this is test
omronFinsNet.DA1 = 0x0B;
// specify Source node address, this is test
omronFinsNet.SA1 = 0x0C;

omronFinsNet.ConnectServer( ); 

```

If we want to know whether connectd, we can do like this

```

OperateResult connect = omronFinsNet.ConnectServer( );
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

 omronFinsNet.ConnectClose( );

```

## Exchange Data

**1. Read Or Write Bool Data**
```
OperateResult<bool> read = omronFinsNet.ReadBool( "D100.4" );
if (read.IsSuccess)
{
    bool D100_4 = read.Content;
}
else
{
    // failed
    string err = read.Message;
}
```
```
OperateResult write = omronFinsNet.Write( "D100.4", true );
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
> Also support bit array read/write

**2. Read D100 Include Many Data Types**
```
short m100_short = omronFinsNet.ReadInt16( "D100" ).Content;
ushort m100_ushort = omronFinsNet.ReadUInt16( "D100" ).Content;
int m100_int = omronFinsNet.ReadInt32( "D100" ).Content;
uint m100_uint = omronFinsNet.ReadUInt32( "D100" ).Content;
float m100_float = omronFinsNet.ReadFloat( "D100" ).Content;
double m100_double = omronFinsNet.ReadDouble( "D100" ).Content;
// need to specify the text length
string m100_string = omronFinsNet.ReadString( "D100", 5 ).Content;
```

**3. Write D100 Include Many Data Types**
```
omronFinsNet.Write( "D100", (short)5 );
omronFinsNet.Write( "D100", (ushort)5 );
omronFinsNet.Write( "D100", 5 );
omronFinsNet.Write( "D100", (uint)5 );
omronFinsNet.Write( "D100", (long)5 );
omronFinsNet.Write( "D100", (ulong)5 );
omronFinsNet.Write( "D100", 5f );
omronFinsNet.Write( "D100", 5d );
omronFinsNet.Write( "D100", "12345678" );
```


> Not only  'D' dataType, but also 'C','A','W', 'H'

> This library alse support write array values.

**4. Read complex data, for example, D100-D109 contains all data you want**

Data name | Data section | Data type | Data Length
-|-|-|-
count | D100-D101 | int | 4-byte
temp | D102-D103 | float | 4-byte
name1 | D104 | short | 2-byte
barcode | D105-D109 | string | 10-byte

So we can do like this

```

OperateResult<byte[]> read = omronFinsNet.Read( "D100", 10 );
if(read.IsSuccess)
{
    int count = omronFinsNet.ByteTransform.TransInt32( read.Content, 0 );
    float temp = omronFinsNet.ByteTransform.TransSingle( read.Content, 4 );
    short name1 = omronFinsNet.ByteTransform.TransInt16( read.Content, 8 );
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

OperateResult<UserType> read = omronFinsNet.ReadCustomer<UserType>( "D100" );
if (read.IsSuccess)
{
    UserType value = read.Content;
}
// write value
omronFinsNet.WriteCustomer( "D100", new UserType( ) );

```


## Supported data types and examples
Data Name | Data Type | Address Format | Example Address
-|-|-|-
DM | short | Decimal | D100, D200
CIO | short | Decimal | C100, C200
WR | short | Decimal | W100, W200
HR | short | Decimal | H100, H200
AR | short | Decimal | A100, A200


## Log Support

This component can also achieve log output. Here is an example. The specific log function refers to the logbook.

```
omronFinsNet.LogNet = new HslCommunication.LogNet.LogNetSingle( Application.StartupPath + "\\Logs.txt" );
```



## Others
For more details, you can download source code, refer to HslCommunicationDemo project