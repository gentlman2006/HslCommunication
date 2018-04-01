## Summary
本篇文章主要讲解如何创建一个常规的ModbusTcp服务器，以及如何定制实现自己的特殊功能，这一切都是非常简单并且高效的。

This article mainly explains how to create a regular ModbusTcp server, 
and how to customize and implement your own special functions. All this is very simple and efficient.

## Contact
QQ Group : [592132877](http://shang.qq.com/wpa/qunwpa?idkey=2278cb9c2e0c04fc305c43e41acff940499a34007dfca9e83a7291e726f9c4e8)

Email: hsl200909@163.com

## Prepare
如果你需要一个测试的客户端，那么可以使用本项目提供的客户端项目进行测试，当然也可以选择其他的客户端。

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


    label11.Text = "write time：" + DateTime.Now.ToString( ) + " befor-value：" + befor + " after-value：" + after;
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