[中文](https://github.com/dathlin/HslCommunication) | [English](https://github.com/dathlin/HslCommunication/blob/master/docs/English.md)
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
![Build status](https://img.shields.io/badge/Build-Success-green.svg) ![License status](https://img.shields.io/badge/License-LGPL3.0-yellow.svg) ![NetFramework](https://img.shields.io/badge/Language-java-orange.svg) ![JDK status](https://img.shields.io/badge/JDK-1.8.0-green.svg) ![IDE status](https://img.shields.io/badge/Intellij%20Idea-2018.4-red.svg) ![copyright status](https://img.shields.io/badge/CopyRight-Richard.Hu-brightgreen.svg) 

# HslCommunication.py
![Build status](https://img.shields.io/badge/Build-Success-green.svg) ![License status](https://img.shields.io/badge/License-LGPL3.0-yellow.svg) ![NetFramework](https://img.shields.io/badge/python-3.6-orange.svg) ![IDE status](https://img.shields.io/badge/Visual%20Studio-Code-red.svg) ![copyright status](https://img.shields.io/badge/CopyRight-Richard.Hu-brightgreen.svg) 
## CopyRight
本组件版权归Richard.Hu所有

## Official Website
唯一官网：[http://www.hslcommunication.cn/](http://www.hslcommunication.cn/)

## License
使用请遵循LGPL-3.0协议说明，除了协议中已经规定的内容外，附加下面三个条款（与原协议如有冲突以附加条款为准）：

* 允许用户使用本工具库（从NuGet下载）集成到自己的项目中作为闭源软件一部分，只需要声明版权出处并出具一份LGPL-3.0的授权协议即可。
* 禁止复制中间的代码及参考思路开发出类似的组件库。
* 源代码仅作为个人学习使用。

## 什么是HSL？
这是一个基于工业物联网，计算机通讯的架构实现，集成了工业软件开发的大部分的基础功能实现，比如三菱PLC通讯，西门子PLC通讯，欧姆龙PLC通讯，modbus通讯，
这些通讯全部进行了多语言的实现，当然，主打的 .net 库的功能集成还更加的强大，除此之外，还实现了跨程序，跨语言，跨平台的通讯，让你不再纠结于使用windows还是
linux系统，实现了日志功能，流水号生成功能，邮件发送功能，傅立叶变换功能，等等，将来会集成更多的工业环境常见功能的实现。

为了不让工业4.0只停留在口号上，万丈高楼平地起，而基石就是HSL。

## HSL能干什么？
HSL能将工业生产现场的设备进行万物互联，将数据在底层自由的传输，无论是主动的还是被动的，无论你的采集系统是什么（通常采集系统为windows电脑，或是嵌入式系统，或是基于linux的盒子），
都可以实现数据的随意传输，方便快速实现强大，实时，高响应的健壮系统，无论您是构建C/S系统，还是B/S系统，还是C-B-S-A（集成桌面客户端，浏览器，安卓）混合系统，都是快速而且低成本的实现，
只要拥有了工业现场的一手数据，即可以搭建强大的实时监视功能的软件，生产报表及自动化排产的软件，各种工艺参数历史追踪的软件，基于数据经验的机器学习软件，以及全功能等等。

**顺便聊聊** ，传统的工业模式都是采购现成的工业软件，包括上位机软件及MES系统，而忽视了自身的研发能力。对于一些行业标准的功能软件来说，比如ERP系统，财务软件，这些可以直接采购即可，
但是对于上位机及MES系统而言，各个企业的实际需求千差万别，难以有通用的场景，而目前的现状都是花大钱办小事，所以在此处，给出一条面向未来的模式实现：对于生产企业而言，
基于HSL开发企业级MES系统实现，作为数据的核心仓库中心，及业务逻辑处理中心；对于设备供应商而言，基于HSL开发上位机软件系统，快速且方便的将数据分发至客户的MES系统，进行协同工作。

**关于企业合作：** 欢迎企业客户联系合作，包括开发新的协议支持，培训及指导上位机软件及MES系统的开发，指导工厂智能化升级改造及信息化团队建设。

## Install From NuGet
说明：NuGet为稳定版本，支持在线升级，组件的使用最好从NuGet下载，此处发布的项目有可能为还没有通过编译的测试版，NuGet安装如下：
```
Install-Package HslCommunication
```

## Environment
* IDE: **Visual Studio 2017** 必须这个版本及以上，不然会语法报错
* .Net Framework环境下：支持.Net 3.5及以上环境，功能最完善。
* .Net Standard环境下：.Net 2.0以上，目前仅仅实现PLC读写，modbus tcp读写，日志记录。
* java环境下：**Intellij Idea 2018.4**
* python: **Visual Studio Code**

## Contact
* 工业软件交流QQ群：[592132877](http://shang.qq.com/wpa/qunwpa?idkey=2278cb9c2e0c04fc305c43e41acff940499a34007dfca9e83a7291e726f9c4e8)
* 邮箱地址(Email)：hsl200909@163.com
* 技术支持VIP群（打赏超过100RMB可加入）：[838185568](http://shang.qq.com/wpa/qunwpa?idkey=eee02ce1acde63c6316cbb380a80e033a14170ab7ca981f1cac83e0b657c8860)

## HslCommunication.dll Summary 
完整的项目介绍地址： [http://www.cnblogs.com/dathlin/p/7703805.html](http://www.cnblogs.com/dathlin/p/7703805.html)

**HslCommunicationDemo** 通过下面的Demo界面可以大概清楚本项目支持的功能:
![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/demo.png)

* [日志记录功能](http://www.cnblogs.com/dathlin/p/7691693.html)
* [同步网络通讯功能 **NetSimplify**](http://www.cnblogs.com/dathlin/p/7697782.html)
* [文件管理引擎](http://www.cnblogs.com/dathlin/p/7746113.html)
* [异步网络通讯功能 **NetComplex**](http://www.cnblogs.com/dathlin/p/8097897.html)
* [三菱PLC以太网访问](http://www.cnblogs.com/dathlin/p/7469679.html)
* [三菱PLC串口访问](https://www.cnblogs.com/dathlin/p/9536467.html)
* [西门子PLC访问](http://www.cnblogs.com/dathlin/p/8685855.html)
* 西门子PPI访问
* [欧姆龙PLC访问](http://www.cnblogs.com/dathlin/p/8723961.html)
* [AB PLC访问](https://www.cnblogs.com/dathlin/p/9607929.html)
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
* [Redis读写](https://www.cnblogs.com/dathlin/p/9998013.html)


## HslCommunication.jar Summary 
本组件提供java版本，为.net版本的阉割版，除去了所有的服务器功能代码，保留了部分的客户端功能代码，方便的和PLC，设备进行数据交互，和C#程序进行数据交互，本jar组件适用用安卓开发，方便搭建一个.net 服务器 + windows 客户端 + asp.net 客户端 + j2ee 客户端 + java 客户端 + android 客户端，目前的java功能代码如下：

* [三菱PLC的数据交互](https://www.cnblogs.com/dathlin/p/9176069.html)
* [西门子PLC的数据交互](https://www.cnblogs.com/dathlin/p/9196129.html)
* 欧姆龙PLC的数据交互
* Modbus Tcp 客户端开发
* 同步网络通讯功能 **NetSimplify**
* 异步网络通讯功能 **NetComplex**
* 数据订阅推送 **NetPush**

## HslCommunication.py Summary 
本组件提供python版本，为.net版本的阉割版，除去了所有的服务器功能代码，保留了部分的客户端功能代码，方便的和PLC，设备进行数据交互，和C#程序进行数据交互，适用于跨平台运行，主要功能如下：

* [三菱PLC的数据交互](https://www.cnblogs.com/dathlin/p/9745147.html)
* [西门子PLC的数据交互](https://www.cnblogs.com/dathlin/p/9713921.html)
* 欧姆龙PLC的数据交互
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
* **SharpNodeSettings项目** 数据网关项目，基于可配置的文件创建的数据中心，支持redis公开和opc ua公开。[https://github.com/dathlin/SharpNodeSettings](https://github.com/dathlin/SharpNodeSettings)
* **MES** 系统，初步的mes系统的界面，目前仍然在开发中。[http://118.24.36.220:8081/](http://118.24.36.220:8081/)

## Contribution
热烈欢迎对本项目的代码提出改进意见，可以发起Pull Request，对于代码量贡献较多的小伙伴，会有额外的组件使用权，并在特别感谢里写明。

## Thanks
* 混合锁及可序列化异常类，读写锁，并发模型部分代码及思路参考《CLR Via C#》，感谢作者Jeffrey Richter

## 创作不易，感谢打赏
If this library really helps you, you can support me by AliPay. Please choose the amount according to your actual ability.
企业用户打赏及需要发票的打赏请专门联系作者（将在官网显示出来），本开源项目的发展和完善需要大家共同的支持。

![打赏](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/support.png)

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

## AB PLC测试界面 [ AllenBradley PLC Communication ]
![Picture](https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/ab1.png)

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
