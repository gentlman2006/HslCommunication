[中文](https://github.com/dathlin/HslCommunication)|[English](https://github.com/dathlin/HslCommunication/blob/master/docs/English.md)
<pre>
             ///\      ///\             /////////\              ///\
            //\\/      //\/           //\\\\\\\\//\            //\\/
           //\/       //\/          //\\/       \\/           //\/
          //\/       //\/           \//\                     //\/
         /////////////\/             \//////\               //\/
        //\\\\\\\\\//\/               \\\\\//\             //\/
       //\/       //\/                     \//\           //\/
      //\/       //\/           ///\      //\\/          //\/       //\   
     ///\      ///\/            \/////////\\/           /////////////\/
     \\\/      \\\/              \\\\\\\\\/             \\\\\\\\\\\\\/             Present by Richard.Hu
</pre>

# HslCommunication.dll
![Build status](https://img.shields.io/badge/Build-Success-green.svg) [![NuGet Status](https://img.shields.io/nuget/v/HslCommunication.svg)](https://www.nuget.org/packages/HslCommunication/) ![NuGet Download](https://img.shields.io/nuget/dt/HslCommunication.svg) [![Gitter](https://badges.gitter.im/Join%20Chat.svg)](http://shang.qq.com/wpa/qunwpa?idkey=2278cb9c2e0c04fc305c43e41acff940499a34007dfca9e83a7291e726f9c4e8) [![NetFramework](https://img.shields.io/badge/Language-C%23%207.0-orange.svg)](https://blogs.msdn.microsoft.com/dotnet/2016/08/24/whats-new-in-csharp-7-0/) [![Visual Studio](https://img.shields.io/badge/Visual%20Studio-2017-red.svg)](https://www.visualstudio.com/zh-hans/) ![License status](https://img.shields.io/badge/License-LGPL3.0-yellow.svg) ![copyright status](https://img.shields.io/badge/CopyRight-Richard.Hu-brightgreen.svg) 

# HslCommunication.jar
![Build status](https://img.shields.io/badge/Build-Success-green.svg) ![License status](https://img.shields.io/badge/License-LGPL3.0-yellow.svg) ![NetFramework](https://img.shields.io/badge/Language-java-orange.svg) ![JDK status](https://img.shields.io/badge/JDK-1.8.0-green.svg) ![IDE status](https://img.shields.io/badge/Intellij%20Idea-2018.1-red.svg) ![copyright status](https://img.shields.io/badge/CopyRight-Richard.Hu-brightgreen.svg) 

## CopyRight
(C) 2018 Richard.Hu, All Rights Reserved

## Official Website
[http://www.hslcommunication.cn/](http://www.hslcommunication.cn/)

## License LGPL3.0

## Install From NuGet
```
Install-Package HslCommunication
```

## Environment
* IDE: **Visual Studio 2017** 
* .Net Framework: support .Net 3.5 and .Net 4.5 full-featured
* .Net Standard: removeed controls and serial function
* java: **Intellij Idea 2018.1** just device read and communication with C#
* python: **Visual Studio Code** just device read and communication with C#

## Contact
* QQ Group: [592132877](http://shang.qq.com/wpa/qunwpa?idkey=2278cb9c2e0c04fc305c43e41acff940499a34007dfca9e83a7291e726f9c4e8) There are many like-minded friends in the group
* Email: hsl200909@163.com


## HslCommunication.dll Summary
* [How to communicate with melsec plc](https://github.com/dathlin/HslCommunication/blob/master/docs/Melsec.md)
* [How to communicate with siemens plc](https://github.com/dathlin/HslCommunication/blob/master/docs/Siemens.md)
* [How to communicate with omron plc](https://github.com/dathlin/HslCommunication/blob/master/docs/Omron.md)
* [How to communicate with modbus-tcp device](https://github.com/dathlin/HslCommunication/blob/master/docs/ModbusTcp.md)
* [How to build you own modbus server, include tcp and rtu mode](https://github.com/dathlin/HslCommunication/blob/master/docs/ModbusServer.md)

## HslCommunication.jar Summary
This component provides Java version, for the. NET version of the castrated version, the removal of all the server function code, 
retaining the vast majority of client function code, convenient and PLC, device for data interaction, 
and C # Program for data interaction, this jar component is suitable for Android development, 
easy to build a. NET Server + Windows Client + ASP. NET client + EE client + Java Client + Android client, 
may support Python in the future, the current Java function code is as follows:

## Support
If this library really helps you, you can support me by AliPay. Please choose the amount according to your actual ability.

![打赏](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/support.png)

## HslCommunicationDemo
v5.2.3 function, support
![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/demo.png)

## Controls
This library include some controls render upside picture. u can easily use them
![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/controls.png)

## 三菱测试界面 [ Mitsubishi PLC Communication ]
Using MC protocol, Qna 3E, Include binary and ascii
![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/Melsec1.png)

## 西门子测试界面 [ Siemens PLC Communication ]
Using S7 protocol And Fetch/Write protocol
![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/siemens1.png)

## 欧姆测试界面 [ Omron PLC Communication ]
Using Fins-tcp protocol
![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/Omron.png)

## Modbus-Tcp 客户端服务器 [ Modbus-tcp Communication ]
Client, using read/write coils and register, read discrete input , read register input
![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/Modbus1.png)

Server, you can build your own modbus-tcp server easily
![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/Modbus2.png)

## Simplify Net 测试演示 [ Based on Tcp/Ip ]
Communicaion with multi-computers , client can exchange data with server easily, include server side ,client side

![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/Simlify1.png)
![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/Simlify2.png)

## Udp Net 测试演示 [ Base on Udp/Ip ]
Communicaion with multi-computers , client can send a large of data to server, include server side ,client side

![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/Udp1.png)
![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/Udp2.png)

## File Net 测试演示 [ Base on Tcp/Ip ]
Communicaion with multi-computers , client can exchange File with server easily, include server side ,client side

![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/File1.png)
![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/File2.png)
