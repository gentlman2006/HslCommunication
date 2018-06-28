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
     \\\/      \\\/              \\\\\\\\\/             \\\\\\\\\\\\\/         Present by Richard.Hu
</pre>

# HslCommunication.dll
![Build status](https://img.shields.io/badge/Build-Success-green.svg) [![NuGet Status](https://img.shields.io/nuget/v/HslCommunication.svg)](https://www.nuget.org/packages/HslCommunication/) ![NuGet Download](https://img.shields.io/nuget/dt/HslCommunication.svg) [![Gitter](https://badges.gitter.im/Join%20Chat.svg)](http://shang.qq.com/wpa/qunwpa?idkey=2278cb9c2e0c04fc305c43e41acff940499a34007dfca9e83a7291e726f9c4e8) [![NetFramework](https://img.shields.io/badge/Language-C%23%207.0-orange.svg)](https://blogs.msdn.microsoft.com/dotnet/2016/08/24/whats-new-in-csharp-7-0/) [![Visual Studio](https://img.shields.io/badge/Visual%20Studio-2017-red.svg)](https://www.visualstudio.com/zh-hans/) ![License status](https://img.shields.io/badge/License-LGPL3.0-yellow.svg) ![copyright status](https://img.shields.io/badge/CopyRight-Richard.Hu-brightgreen.svg) 

# HslCommunication.jar
![Build status](https://img.shields.io/badge/Build-Success-green.svg) ![License status](https://img.shields.io/badge/License-LGPL3.0-yellow.svg) ![NetFramework](https://img.shields.io/badge/Language-java-orange.svg) ![JDK status](https://img.shields.io/badge/JDK-1.8.0-green.svg) ![IDE status](https://img.shields.io/badge/Intellij%20Idea-2018.1-red.svg) ![copyright status](https://img.shields.io/badge/CopyRight-Richard.Hu-brightgreen.svg) 

## CopyRight
本组件版权归Richard.Hu所有 [ (C) 2018 Richard.Hu, All Rights Reserved ]

## Version Declaration
Version 5.X 的计划：
* 提升组件的稳定性，尤其是在高并发的情况下
* 扩展其他的PLC和设备通讯支持
* 文档手册的编写
* Java版本的支持


## License
使用请遵循LGPL-3.0协议说明，除了协议中已经规定的内容外，附加下面三个条款（与原协议如有冲突以附加条款为准）：

* 允许用户使用本工具库（从NuGet下载）集成到自己的项目中作为闭源软件一部分，只需要声明版权出处并出具一份LGPL-3.0的授权协议即可。
* 禁止复制中间的代码及参考思路开发出类似的组件库。
* 源代码仅作为个人学习使用。

## Install From NuGet
说明：NuGet为稳定版本，支持在线升级，组件的使用最好从NuGet下载，此处发布的项目有可能为还没有通过编译的测试版，NuGet安装如下：
```
Install-Package HslCommunication
```

## Environment
* IDE: **Visual Studio 2017** 必须这个版本及以上，不然会语法报错
* .Net Framework环境下：支持.Net 3.5及以上环境，功能最完善。
* .Net Standard环境下：.Net 2.0以上，目前仅仅实现PLC读写，modbus tcp读写，日志记录。
* java环境下：**Intellij Idea 2018.1**

## Contact
* 技术支持QQ群：[592132877](http://shang.qq.com/wpa/qunwpa?idkey=2278cb9c2e0c04fc305c43e41acff940499a34007dfca9e83a7291e726f9c4e8)
* 邮箱地址(Email)：hsl200909@163.com

## Project Target
本项目的目标在于开发一个.Net及java下大多数软件系统都会包含了基础类库功能，实现一些常用的数据通信，日志记录等等类，以及版本类，网络通讯类，PLC数据访问类。并且实现C#和java无缝通信集成。

The goal of this project is to develop a .Net and java. Most software systems will include the basic class library functions, implement some common data communications, log records, etc., as well as version classes, network communications, and PLC data access classes. And to achieve seamless integration of C# and java communications.

## HslCommunication.dll Summary 
完整的项目介绍地址： [http://www.cnblogs.com/dathlin/p/7703805.html](http://www.cnblogs.com/dathlin/p/7703805.html)

* [日志记录功能](http://www.cnblogs.com/dathlin/p/7691693.html)
* [同步网络通讯功能 **NetSimplify**](http://www.cnblogs.com/dathlin/p/7697782.html)
* [文件管理引擎](http://www.cnblogs.com/dathlin/p/7746113.html)
* [异步网络通讯功能 **NetComplex**](http://www.cnblogs.com/dathlin/p/8097897.html)
* [三菱及西门子PLC访问](http://www.cnblogs.com/dathlin/p/7469679.html)
* [西门子PLC访问](http://www.cnblogs.com/dathlin/p/8685855.html)
* [欧姆龙PLC访问](http://www.cnblogs.com/dathlin/p/8723961.html)
* [邮件功能使用](http://www.cnblogs.com/dathlin/p/8463613.html)
* [流水号生成器](http://www.cnblogs.com/dathlin/p/7811489.html)
* [软件注册码功能](http://www.cnblogs.com/dathlin/p/7832315.html)
* [数据订阅推送 **NetPush**](http://www.cnblogs.com/dathlin/p/8992315.html)
* [ModBus 服务器开发，包含Tcp服务器和Rtu服务器](http://www.cnblogs.com/dathlin/p/7782315.html)
* [ModBus Tcp客户端开发](http://www.cnblogs.com/dathlin/p/7885368.html)
* [ModBus Rtu客户端开发](http://www.cnblogs.com/dathlin/p/8974215.html)
* [异形Modbus Tcp客户端开发，侦听模式客户端](http://www.cnblogs.com/dathlin/p/8934266.html)
* 多线程任务功能
* [CRC16校验](http://www.cnblogs.com/dathlin/p/7821808.html)
* [常用控件库](http://www.cnblogs.com/dathlin/p/8150516.html)
* [连接池使用](https://www.cnblogs.com/dathlin/p/9191211.html)


**Detail Introduction**

* [How to communicate with melsec plc](https://github.com/dathlin/HslCommunication/blob/master/docs/Melsec.md)
* [How to communicate with siemens plc](https://github.com/dathlin/HslCommunication/blob/master/docs/Siemens.md)
* [How to communicate with omron plc](https://github.com/dathlin/HslCommunication/blob/master/docs/Omron.md)
* [How to communicate with modbus-tcp device](https://github.com/dathlin/HslCommunication/blob/master/docs/ModbusTcp.md)
* [How to build you own modbus server, include tcp and rtu mode](https://github.com/dathlin/HslCommunication/blob/master/docs/ModbusServer.md)

## HslCommunication.jar Summary 
本组件提供java版本，为.net版本的阉割版，除去了所有的服务器功能代码，保留了绝大多数的客户端功能代码，方便的和PLC，设备进行数据交互，和C#程序进行数据交互，本jar组件适用用安卓开发，方便搭建一个.net 服务器 + windows 客户端 + asp.net 客户端 + j2ee 客户端 + java 客户端 + android 客户端，未来可能支持python，目前的java功能代码如下：

* [三菱PLC的数据交互](https://www.cnblogs.com/dathlin/p/9176069.html)
* [西门子PLC的数据交互](https://www.cnblogs.com/dathlin/p/9196129.html)
* Modbus Tcp 客户端开发
* 同步网络通讯功能 **NetSimplify**
* 异步网络通讯功能 **NetComplex**
* 数据订阅推送 **NetPush**


## Second Extensions Project (内含几个Demo)
* 基于本组件开发的一个CS架构的项目模版，该模版采用本组件进行开发，完成了账户管理，角色管理，个人文件管理，头像机制，自动升级，完善的日志等等功能。项目地址为：[https://github.com/dathlin/ClientServerProject](https://github.com/dathlin/ClientServerProject)

* 基于本组件开发的一个局域网多人聊天的程序，支持在线客户端信息查看，服务器强制关闭客户端。[https://github.com/dathlin/NetChatRoom](https://github.com/dathlin/NetChatRoom)

* 基于本组件开发的一个西门子PLC后台读取显示数据，并推送给在线客户端（包括window程序，asp.net网站，Android程序）显示实时曲线的示例项目。并实现远程操作。[https://github.com/dathlin/RemoteMonitor](https://github.com/dathlin/RemoteMonitor)

* **ModbusTcpServer项目** 基于本组件开发的一个Modbus Tcp服务器工具，可以方便的快速搭建一个性能可靠稳定的服务器。

* **FileNetServer项目** 基于本组件开发的一个文件管理引擎，实现服务器端文件存储，支持客户端进行文件上传，下载，删除，查看文件信息等等。

* **HslCommunicationDemo项目** 基于本组件开发的一个西门子，三菱，欧姆龙，Modbus-Tcp，SimplifyNet，ComplexNet，FileNet等的通讯测试工具，方便的进行读写测试，不需要额外的编写代码。

* **HslSharp项目** 基于本组件深度定制实现的OPC UA网关服务器，实现基于三菱，西门子，欧姆龙，modbus等HslCommunication支持的协议创建可配置的OPC UA服务器。下载地址：[HSLSharp.zip](https://github.com/dathlin/HslCommunication/raw/master/Download/HSLSharp.zip)

## Contribution
热烈欢迎对本项目的代码提出改进意见，可以发起Pull Request，对于代码量贡献较多的小伙伴，会有额外的组件使用权，并在特别感谢里写明。

## Thanks
* 混合锁及可序列化异常类，读写锁，并发模型部分代码及思路参考《CLR Via C#》，感谢作者Jeffrey Richter
* 感谢 **CKernal** 推送的Qna兼容1E帧协议的三菱代码
* 感谢 **ligihtdev** 打赏支持
* 感谢 **Wzhigang** 打赏支持
* 感谢 **Running...** 打赏支持
* 感谢 **生意王(2940280678)** 打赏支持


## 创作不易，感谢打赏
If this library really helps you, you can support me by AliPay. Please choose the amount according to your actual ability.

![打赏](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/support.png)

## HslCommunicationDemo
v5.1.1 function, support
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
