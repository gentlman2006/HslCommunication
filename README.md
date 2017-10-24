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

## 联系作者
* 技术支持QQ群：[592132877](http://shang.qq.com/wpa/qunwpa?idkey=2278cb9c2e0c04fc305c43e41acff940499a34007dfca9e83a7291e726f9c4e8)
* 邮箱地址：hsl200909@163.com

## 项目目标
本项目的目标在于开发一个.Net下大多数软件系统都会包含了基础类库功能，实现一些常用的数据通信，日志记录等等类，以及版本类。

## 项目介绍
[http://www.cnblogs.com/dathlin/p/7703805.html](http://www.cnblogs.com/dathlin/p/7703805.html)

## 二次扩展
基于本组件开发的一个CS架构的项目模版，该模版采用本组件进行开发，完成了账户管理，角色管理，个人文件管理，头像机制，自动升级，完善的日志等等功能。项目地址为：[https://github.com/dathlin/ClientServerProject](https://github.com/dathlin/ClientServerProject)

## 代码贡献
热烈欢迎对本项目的代码提出改进意见，可以发起Pull Request，对于代码量贡献较多的小伙伴，会有额外的组件使用权，并在特别感谢里写明。

## 特别感谢
* 混合锁及可序列化异常类，读写锁，并发模型部分代码及思路参考《CLR Via C#》，感谢作者Jeffrey Richter