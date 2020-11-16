## Summary
��ƪ������Ҫ������δ���һ�������Modbus��������������Tcp��������Rtu���������Լ���ζ���ʵ���Լ������⹦�ܣ���һ�ж��Ƿǳ��򵥲��Ҹ�Ч�ġ�

This article mainly explains how to create a regular Modbus server, Including Tcp and Rtu mode.
and how to customize and implement your own special functions. All this is very simple and efficient.

## Contact
QQ Group : [592132877](http://shang.qq.com/wpa/qunwpa?idkey=2278cb9c2e0c04fc305c43e41acff940499a34007dfca9e83a7291e726f9c4e8)

Email: hsl200909@163.com

## Prepare
�������Ҫһ�����ԵĿͻ��ˣ���ô����ʹ�ñ���Ŀ�ṩ�Ŀͻ�����Ŀ���в��ԣ���ȻҲ����ѡ�������Ŀͻ��ˡ�

If you need a test client, you can use the client project provided by this project to test, of course, you can also select other clients.

## Instantiation
**1. Add namespace**

```
using HslCommunication.Modbus;
using HslCommunication;

```

**2. Declare**
```
private HslCommunication.ModBus.ModbusTcpServer busTcpServer;
```

## Create a new simple server
We only need 2 lines to complete the required function.
```
try
{
    busTcpServer = new HslCommunication.ModBus.ModbusTcpServer( );
    busTcpServer.ServerStart( 502 );
}
catch (Exception ex)
{
    MessageBox.Show( ex.Message );
}
```

## Log Support
```
busTcpServer.LogNet = new HslCommunication.LogNet.LogNetSingle( "logs.txt" );
```

## Special handling of data on the client
At some point, we need to manually process the data sent by the client, which can be processed according to the following code.

First of all, we need to bind an event.
```
busTcpServer.OnDataReceived += BusTcpServer_OnDataReceived;
```

Then we implement this method. The following example shows the received data directly in hexadecimal format.

```
private void BusTcpServer_OnDataReceived( byte[] modbus )
{
    string hexText = HslCommunication.BasicFramework.SoftBasic.ByteToHexString(modbus);
}

```

## Modbus Rtu Support

```
    private void button5_Click( object sender, EventArgs e )
    {
        // ��������
        if (busTcpServer != null)
        {
            try
            {
                busTcpServer.StartSerialPort( "COM4" );
            }
            catch(Exception ex)
            {
                MessageBox.Show( "���ڷ���������ʧ�ܣ�" + ex.Message );
            }
        }
        else
        {
            MessageBox.Show( "��������Tcp��������" );
        }
    }
```



## Basic Read Write

```
    bool Coil100 = busTcpServer.ReadCoil( 100 );                  // ����Ȧ100��ֵ
    bool[] Coil100_109 = busTcpServer.ReadCoil( 100, 10 );        // ����Ȧ����
    short Short100 = busTcpServer.ReadInt16( 100 );               // ��ȡ�Ĵ���ֵ
    ushort UShort100 = busTcpServer.ReadUInt16( 100 );            // ��ȡ�Ĵ���ushortֵ
    int Int100 = busTcpServer.ReadInt32( 100 );                   // ��ȡ�Ĵ���intֵ
    uint UInt100 = busTcpServer.ReadUInt32( 100 );                // ��ȡ�Ĵ���uintֵ
    float Float100 = busTcpServer.ReadFloat( 100 );               // ��ȡ�Ĵ���Floatֵ
    long Long100 = busTcpServer.ReadInt64( 100 );                 // ��ȡ�Ĵ���longֵ
    ulong ULong100 = busTcpServer.ReadUInt64( 100 );              // ��ȡ�Ĵ���ulongֵ
    double Double100 = busTcpServer.ReadDouble( 100 );            // ��ȡ�Ĵ���doubleֵ

    busTcpServer.WriteCoil( 100, true );                          // д��Ȧ��ͨ��
    busTcpServer.Write( 100, (short)5 );                          // д��shortֵ
    busTcpServer.Write( 100, (ushort)45678 );                     // д��ushortֵ
    busTcpServer.Write( 100, 12345667 );                          // д��intֵ
    busTcpServer.Write( 100, (uint)12312312 );                    // д��uintֵ
    busTcpServer.Write( 100, 123.456f );                          // д��floatֵ
    busTcpServer.Write( 100, 1231231231233L );                    // д��longֵ
    busTcpServer.Write( 100, 1212312313UL );                      // д��ulongֵ
    busTcpServer.Write( 100, 123.456d );                          // д��doubleֵ
```


## Monitor the data of one address
```

private void button2_Click( object sender, EventArgs e )
{
    // click to add a monitor
    ModBusMonitorAddress monitorAddress = new ModBusMonitorAddress( );
    monitorAddress.Address = ushort.Parse( 100 );
    monitorAddress.OnChange += MonitorAddress_OnChange;
    monitorAddress.OnWrite += MonitorAddress_OnWrite;
    busTcpServer.AddSubcription( monitorAddress );
}

private void MonitorAddress_OnWrite( ModBusMonitorAddress monitor, short value )
{
    // Occurs when a client writes data
}

private void MonitorAddress_OnChange( ModBusMonitorAddress monitor, short befor, short after )
{
    // Occurs when the data value changes
    if(InvokeRequired)
    {
        BeginInvoke( new Action<ModBusMonitorAddress, short, short>( MonitorAddress_OnChange ), monitor, befor, after );
        return;
    }


    label11.Text = "write time��" + DateTime.Now.ToString( ) + " befor-value��" + befor + " after-value��" + after;
}

```

## Limit client login
We can also restrict special clients from logging in, specifying trusted clients via ip addresses.
```
modbusTcpServer.SetTrustedIpAddress( new List<string>( "192.168.0.100","192.168.0.101" ) );

```

If you want to lift the restriction.
```
modbusTcpServer.SetTrustedIpAddress( null );

```