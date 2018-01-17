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

# HslCommunication ![Build status](https://ci.appveyor.com/api/projects/status/oajkgccisoe98gip?svg=true) [![NuGet Status](https://img.shields.io/nuget/v/HslCommunication.svg)](https://www.nuget.org/packages/HslCommunication/) [![Gitter](https://badges.gitter.im/Join%20Chat.svg)](http://shang.qq.com/wpa/qunwpa?idkey=2278cb9c2e0c04fc305c43e41acff940499a34007dfca9e83a7291e726f9c4e8)

## 版权声明
本组件版权归Richard.Hu所有
## 授权协议
使用请遵循LGPL-3.0协议说明，除了协议中已经规定的内容外，附加下面三个条款（与原协议如有冲突以附加条款为准）：

* 允许用户使用本工具库（从NuGet下载）集成到自己的项目中作为闭源软件一部分，只需要声明版权出处并出具一份LGPL-3.0的授权协议即可。
* 禁止对源代码做出修改，禁止复制中间的代码及参考思路开发出类似的组件库。
* 源代码仅作为个人学习使用。

## NuGet安装
说明：NuGet为稳定版本，组件的使用必须从NuGet下载，此处发布的项目有可能为还没有通过编译的测试版，NuGet安装如下：

Install-Package HslCommunication

## 运行环境
* .Net Framework环境下：支持.Net 3.5及以上环境，功能最完善。
* .Net Standard环境下：.Net 2.0以上，目前仅仅实现PLC读写，modbus tcp读写，日志记录。

## 联系作者
* 技术支持QQ群：[592132877](http://shang.qq.com/wpa/qunwpa?idkey=2278cb9c2e0c04fc305c43e41acff940499a34007dfca9e83a7291e726f9c4e8)
* 邮箱地址：hsl200909@163.com

## 项目目标
本项目的目标在于开发一个.Net下大多数软件系统都会包含了基础类库功能，实现一些常用的数据通信，日志记录等等类，以及版本类。

## 项目介绍
完整的项目介绍地址： [http://www.cnblogs.com/dathlin/p/7703805.html](http://www.cnblogs.com/dathlin/p/7703805.html)

* [日志记录功能](http://www.cnblogs.com/dathlin/p/7691693.html)
* [同步网络通讯功能](http://www.cnblogs.com/dathlin/p/7697782.html)
* [文件管理引擎](http://www.cnblogs.com/dathlin/p/7746113.html)
* [异步网络通讯功能](http://www.cnblogs.com/dathlin/p/8097897.html)
* [三菱及西门子PLC访问](http://www.cnblogs.com/dathlin/p/7469679.html)
* 邮件功能使用
* [流水号生成器](http://www.cnblogs.com/dathlin/p/7811489.html)
* [软件注册码功能](http://www.cnblogs.com/dathlin/p/7832315.html)
* [ModBus Tcp服务器开发](http://www.cnblogs.com/dathlin/p/7782315.html)
* [ModBus Tcp客户端开发](http://www.cnblogs.com/dathlin/p/7885368.html)
* 多线程任务功能
* [CRC16校验](http://www.cnblogs.com/dathlin/p/7821808.html)
* [常用控件库](http://www.cnblogs.com/dathlin/p/8150516.html)


## 二次扩展
* 基于本组件开发的一个CS架构的项目模版，该模版采用本组件进行开发，完成了账户管理，角色管理，个人文件管理，头像机制，自动升级，完善的日志等等功能。项目地址为：[https://github.com/dathlin/ClientServerProject](https://github.com/dathlin/ClientServerProject)

* 基于本组件开发的一个Modbus Tcp测试工具，可以方便的测试客户端和服务端功能。[https://github.com/dathlin/ModBusTcpTools](https://github.com/dathlin/ModBusTcpTools)

* 基于本组件开发的一个西门子PLC后台读取显示数据，并推送给在线客户端显示实时曲线的示例项目。[https://github.com/dathlin/RemoteMonitor](https://github.com/dathlin/RemoteMonitor)

* 基于本组件开发的一个文件管理引擎，实现服务器端文件存储，客户端进行文件上传，下载，删除，查看文件信息等等。[https://github.com/dathlin/FileManagment](https://github.com/dathlin/FileManagment)

* 基于本组件开发的一个局域网多人聊天的程序，支持在线客户端信息查看，服务器强制关闭客户端。[https://github.com/dathlin/NetChatRoom](https://github.com/dathlin/NetChatRoom)

## 代码贡献
热烈欢迎对本项目的代码提出改进意见，可以发起Pull Request，对于代码量贡献较多的小伙伴，会有额外的组件使用权，并在特别感谢里写明。

## 特别感谢
* 混合锁及可序列化异常类，读写锁，并发模型部分代码及思路参考《CLR Via C#》，感谢作者Jeffrey Richter