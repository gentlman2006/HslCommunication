using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core;

namespace HslCommunication
{
    /// <summary>
    /// 一个工业物联网的底层架构框架，专注于底层的技术通信及跨平台，跨语言通信功能，实现各种主流的PLC数据读写，实现modbus的各种协议读写等等，
    /// 支持快速搭建工业上位机软件，组态软件，SCADA软件，工厂MES系统，助力企业工业4.0腾飞，实现智能制造，智慧工厂的目标。
    /// <br /><br />
    /// 本组件免费开源，使用之前请认真的阅读本API文档，对于本文档中警告部分的内容务必理解，部署生产之前请详细测试，如果在测试的过程中，
    /// 发现了BUG，或是有问题的地方，欢迎联系作者进行修改，或是直接在github上进行提问。统一声明：对于操作设备造成的任何损失，作者概不负责。
    /// <br /><br />
    /// 官方网站：<a href="http://www.hslcommunication.cn/">http://www.hslcommunication.cn/</a>，包含组件的在线API地址以及一个MES DEMO的项目展示。
    /// <br /><br />
    /// <note type="important">
    /// 本组件的目标是集成一个框架，统一所有的设备读写方法，抽象成统一的接口<see cref="IReadWriteNet"/>，对于上层操作只需要关注地址，读取类型即可，另一个目标是使用本框架轻松实现C#后台+C#客户端+web浏览器+android手机的全方位功能实现。
    /// </note>
    /// <br /><br />
    /// 本库提供了C#版本和java版本和python版本，java，python版本的使用和C#几乎是一模一样的，都是可以相互通讯的。
    /// </summary>
    /// <remarks>
    /// 本软件著作权归Richard.Hu所有，开源项目地址：<a href="https://github.com/dathlin/HslCommunication">https://github.com/dathlin/HslCommunication</a>  开源协议：LGPL-3.0
    /// <br />
    /// 博客地址：<a href="https://www.cnblogs.com/dathlin/p/7703805.html">https://www.cnblogs.com/dathlin/p/7703805.html</a>
    /// <br />
    /// 打赏请扫码：<br />
    /// <img src="https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/support.png" />
    /// </remarks>
    /// <revisionHistory>
    ///     <revision date="2017-10-21" version="3.7.10" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>正式发布库到互联网上去。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-10-21" version="3.7.11" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>添加xml文档</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-10-31" version="3.7.12" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>重新设计西门子的数据读取机制，提供一个更改类型的方法。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-11-06" version="3.7.13" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>提供一个ModBus的服务端引擎。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-11-07" version="3.7.14" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>紧急修复了西门子批量访问时出现的BUG。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-11-12" version="3.7.15" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>完善CRC16校验码功能，完善数据库辅助类方法。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-11-13" version="3.7.16" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>西门子访问类，提供一个批量bool数据写入，但该写入存在安全隐患，具体见博客。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-11-21" version="4.0.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>与3.X版本不兼容，谨慎升级。如果要升级，主要涉及的代码包含PLC的数据访问和同步数据通信。</item>
    ///             <item>删除了2个类，OperateResultBytes和OperateResultString类，提供了更加强大方便的泛型继承类，多达10个泛型参数。地址见http://www.cnblogs.com/dathlin/p/7865682.html</item>
    ///             <item>将部分类从HslCommunication命名空间下移动到HslCommunication.Core下面。</item>
    ///             <item>提供了一个通用的ModBus TCP的客户端类，方便和服务器交互。</item>
    ///             <item>完善了HslCommunication.BasicFramework.SoftBaisc下面的辅助用的静态方法，提供了一些方便的数据转化，在上面进行公开。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-11-24" version="4.0.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>更新了三菱的读取接口，提供了一个额外的字符串表示的方式，OperateResult&lt;byte[]&gt; read =  melsecNet.ReadFromPLC("M100", 5);</item>
    ///             <item>更新了西门子的数据访问类和modbus tcp类提供双模式运行，按照之前版本的写法是默认模式，每次请求重新创建网络连接，新增模式二，在代码里先进行连接服务器方法，自动切换到模式二，每次请求都共用一个网络连接，内部已经同步处理，加速数据访问，如果访问失败，自动在下次请求是重新连接，如果调用关闭连接服务器，自动切换到模式一。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-11-25" version="4.0.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复Modbus tcp批量写入寄存器时，数据解析异常的BUG。</item>
    ///             <item>三菱访问器新增长连接模式。</item>
    ///             <item>三菱访问器支持单个M写入，在数组中指定一个就行。</item>
    ///             <item>三菱访问器提供了float[]数组写入的API。</item>
    ///             <item>三菱访问器支持F报警器，B链接继电器，S步进继电器，V边沿继电器，R文件寄存器读写，不过还需要大面积测试。</item>
    ///             <item>三菱访问器的读写地址支持字符串形式传入。</item>
    ///             <item>其他的细节优化。</item>
    ///             <item>感谢 hwdq0012 网友的测试和建议。</item>
    ///             <item>感谢 吃饱睡好 好朋友的测试</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-11-27" version="4.0.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>三菱，西门子，Modbus tcp客户端内核优化重构。</item>
    ///             <item>三菱，西门子，Modbus tcp客户端提供统一的报文测试方法，该方法也是通信核心，所有API都是基于此扩展起来的。</item>
    ///             <item>三菱，西门子，Modbus tcp客户端提供了一些便捷的读写API，详细参见对应博客。</item>
    ///             <item>三菱的地址区分十进制和十六进制。</item>
    ///             <item>优化三菱的位读写操作。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-11-28" version="4.1.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复西门子读取的地址偏大会出现异常的BUG。</item>
    ///             <item>完善统一了所有三菱，西门子，modbus客户端类的读写方法，已经更新到博客。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-02" version="4.1.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>完善日志记录，提供关键字记录操作。</item>
    ///             <item>三菱，西门子，modbus tcp客户端提供自定义数据读写。</item>
    ///             <item>modbus tcp服务端提供数据池功能，并支持数据订阅操作。</item>
    ///             <item>提供一个纵向的进度控件。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-04" version="4.1.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>完善Modbus tcp服务器端的数据订阅功能。</item>
    ///             <item>进度条控件支持水平方向和垂直方向两个模式。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-05" version="4.1.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>进度条控件修复初始颜色为空的BUG。</item>
    ///             <item>进度条控件文本锯齿修复。</item>
    ///             <item>按钮控件无法使用灰色按钮精灵破解。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-13" version="4.1.4" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>modbus tcp提供读取short数组的和ushort数组方法。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-13" version="4.1.5" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复流水号生成器无法生成不带日期格式的流水号BUG。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-18" version="4.1.6" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>OperateResult成功时，消息为成功。</item>
    ///             <item>数据库辅助类API添加，方便的读取聚合函数。</item>
    ///             <item>日志类分析工具界面，显示文本微调。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-25" version="4.1.7" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>进度条控件新增一个新的属性对象，是否使用动画。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-27" version="4.1.8" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>新增一个饼图控件。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-28" version="4.1.9" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>饼图显示优化，新增是否显示百分比的选择。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-31" version="4.2.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>新增一个仪表盘控件。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-01-03" version="4.2.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>饼图控件新增一个是否显示占比很小的信息文本。</item>
    ///             <item>新增一个旋转开关控件。</item>
    ///             <item>新增一个信号灯控件。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-01-05" version="4.2.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复modbus tcp客户端读取 float, int, long,的BUG。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-01-08" version="4.2.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复modbus tcp客户端读取某些特殊设备会读取不到数据的BUG。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-01-15" version="4.2.4" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>双模式的网络基类中新增一个读取超时的时间设置，如果为负数，那么就不验证返回。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-01-24" version="4.3.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>信号灯控件显示优化。</item>
    ///             <item>Modbus Tcp服务端类修复内存暴涨问题。</item>
    ///             <item>winfrom客户端提供一个曲线控件，方便显示实时数据，多曲线数据。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-02-05" version="4.3.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>优化modbus tcp客户端的访问类，支持服务器返回错误信息。</item>
    ///             <item>优化曲线控件，支持横轴文本显示，支持辅助线标记，详细见对应博客。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-02-22" version="4.3.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>曲线控件最新时间显示BUG修复。</item>
    ///             <item>Modbus tcp错误码BUG修复。</item>
    ///             <item>三菱访问类完善long类型读写。</item>
    ///             <item>西门子访问类支持1500系列，支持读取订货号。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-03-05" version="4.3.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>曲线控件增加一个新的属性，图标标题。</item>
    ///             <item>Modbus tcp服务器端的读写BUG修复。</item>
    ///             <item>西门子访问类重新支持200smart。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-03-07" version="4.3.4" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>Json组件更新至11.0.1版本。</item>
    ///             <item>紧急修复日志类的BeforeSaveToFile事件在特殊情况的触发BUG。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-03-19" version="4.3.5" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复Modbus-tcp服务器接收异常的BUG。</item>
    ///             <item>修复SoftBasic.ByteTo[U]ShortArray两个方法异常。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-04-05" version="5.0.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>网络核心层重新开发，完全的基于异步IO实现。</item>
    ///             <item>所有双模式客户端类进行代码重构，接口统一。</item>
    ///             <item>完善并扩充OperateResult对象的类型支持。</item>
    ///             <item>提炼一些基础的更加通用的接口方法，在SoftBasic里面。</item>
    ///             <item>支持欧姆龙PLC的数据交互。</item>
    ///             <item>支持三菱的1E帧数据格式。</item>
    ///             <item>不兼容升级，谨慎操作。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-04-10" version="5.0.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>OperateResult静态方法扩充。</item>
    ///             <item>文件引擎提升缓存空间到100K，加速文件传输。</item>
    ///             <item>三菱添加读取单个bool数据。</item>
    ///             <item>Modbus-tcp客户端支持配置起始地址不是0的服务器。</item>
    ///             <item>其他代码优化。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-04-14" version="5.0.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>ComplexNet服务器代码精简优化，移除客户端的在线信息维护代码。</item>
    ///             <item>西门子访问类第一次握手信号18字节改为0x02。</item>
    ///             <item>更新JSON组件到11.0.2版本。</item>
    ///             <item>日志存储类优化，支持过滤存储特殊关键字的日志。</item>
    ///             <item>Demo项目新增控件介绍信息。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-04-20" version="5.0.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复Modbus-Tcp服务器的空异常。</item>
    ///             <item>修复西门子类写入float，double，long数据异常。</item>
    ///             <item>修复modbus-tcp客户端读写字符串颠倒异常。</item>
    ///             <item>修复三菱多读取数据字节的问题。</item>
    ///             <item>双模式客户端新增异形客户端模式，变成了三模式客户端。</item>
    ///             <item>提供异形modbus服务器和客户端Demo方便测试。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-04-25" version="5.0.4" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>Modbus-tcp服务器同时支持RTU数据交互。</item>
    ///             <item>异形客户端新增在线监测，自动剔除访问异常设备。</item>
    ///             <item>modbus-tcp支持读取输入点。</item>
    ///             <item>所有客户端设备的连接超时判断增加休眠，降低CPU负载。</item>
    ///             <item>西门子批量读取上限为19个数组。</item>
    ///             <item>其他小幅度的代码优化。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-04-30" version="5.0.5" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>Modbus相关的代码优化。</item>
    ///             <item>新增Modbus-Rtu客户端模式，配合服务器的串口支持，已经可以实现电脑本机的通讯测试了。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-05-04" version="5.0.6" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>提炼数据转换基类，优化代码，修复WordReverse类对字符串的BUG，相当于修复modbus和omron读写字符串的异常。</item>
    ///             <item>新增一个全新的功能类，数据的推送类，轻量级的高效的订阅发布数据信息。具体参照Demo。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-05-07" version="5.0.7" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>Modbus服务器提供在线客户端数量属性。</item>
    ///             <item>所有服务器基类添加端口缓存。</item>
    ///             <item>双模式客户端完善连接失败，请求超时的消息提示。</item>
    ///             <item>修复双模式客户端某些特殊情况下的头子节NULL异常。</item>
    ///             <item>修复三菱交互类的ASCII协议下的写入数据异常。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-05-12" version="5.0.8" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>新增一个埃夫特机器人的数据访问类。</item>
    ///             <item>双模式客户端的长连接支持延迟连接操作，通过一个新方法完成。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-05-21" version="5.0.9" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>优化ComplexNet客户端的代码。</item>
    ///             <item>更新埃夫特机器人的读取机制到最新版。</item>
    ///             <item>Modbus Rtu及串口基类支持接收超时时间设置，不会一直卡死。</item>
    ///             <item>Modbus Tcp及Rtu都支持带功能码输入，比如读取100地址，等同于03X100。（注意：该多功能地址仅仅适用于Read及相关的方法</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-05-22" version="5.0.10" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>Modbus Tcp及Rtu支持手动更改站号。也就是支持动态站号调整。</item>
    ///             <item>修复上个版本遗留的Modbus在地址偏移情况下会多减1的BUG。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-06-05" version="5.1.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>Modbus服务器支持串口发送数据时也会触发消息接收。</item>
    ///             <item>IReadWriteNet接口新增Read(string address,ushort length)方法。</item>
    ///             <item>提炼统一的设备基类，支持Read方法及其扩展的子方法。</item>
    ///             <item>修复埃夫特机器人的读取BUG。</item>
    ///             <item>三菱PLC支持读取定时器，计数器的值，地址格式为"T100"，"C100"。</item>
    ///             <item>新增快速离散的傅立叶频谱变换算法，并在Demo中测试三种周期信号。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-06-16" version="5.1.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复西门子fetch/write协议对db块，定时器，计数器读写的BUG。</item>
    ///             <item>埃夫特机器人修复tostring()的方法。</item>
    ///             <item>modbus客户端新增两个属性，指示是否字节颠倒和字符串颠倒，根据不同的服务器配置。</item>
    ///             <item>IReadWriteNet接口补充几个数组读取的方法。</item>
    ///             <item>新增一个全新的连接池功能类，详细请参见 https://www.cnblogs.com/dathlin/p/9191211.html </item>
    ///             <item>其他的小bug修复，细节优化。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-06-27" version="5.1.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>IByteTransform接口新增bool[]数组转换的2个方法。</item>
    ///             <item>Modbus Server类新增离散输入数据池和输入寄存器数据池，可以在服务器端读写，在客户端读。</item>
    ///             <item>Modbus Tcp及Modbus Rtu及java的modbus tcp支持富地址表示，比如"s=2;100"为站号2的地址100信息。</item>
    ///             <item>Modbus Server修复一个偶尔出现多次异常下线的BUG。</item>
    ///             <item>其他注释修正。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-07-13" version="5.1.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>Modbus服务器新增数据大小端配置。</item>
    ///             <item>Modbus服务器支持数据存储本地及从本地加载。</item>
    ///             <item>修复modbus服务器边界读写bug。</item>
    ///             <item>ByteTransformBase的double转换bug修复。</item>
    ///             <item>修复ReverseWordTransform批量字节转换时隐藏的一些bug。</item>
    ///             <item>SoftBasic移除2个数据转换的方法。</item>
    ///             <item>修复modbus写入单个寄存器的高地位倒置的bug。</item>
    ///             <item>修复串口通信过程中字节接收不完整的异常。包含modbus服务器和modbus-rtu。</item>
    ///             <item>添加了.net 4.5项目，并且其他项目源代码引用该项目。添加了单元测试，逐步新增测试方法。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-07-27" version="5.2.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>项目新增api文档，提供离线版和在线版，文档提供了一些示例代码。</item>
    ///             <item>modbus-rtu新增批量的数组读取方法。</item>
    ///             <item>modbus-rtu公开ByteTransform属性，方便的进行数据转换。</item>
    ///             <item>SoftMail删除发送失败10次不能继续发送的机制。</item>
    ///             <item>modbus server新增站号属性，站号不对的话，不响应rtu反馈。</item>
    ///             <item>modbus server修复读取65524和65535地址提示越界的bug。</item>
    ///             <item>Demo项目提供了tcp/ip的调试工具。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-08-08" version="5.2.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>API文档中西门子FW协议示例代码修复。</item>
    ///             <item>modbus-rtu修复读取线圈和输入线圈的值错误的bug。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-08-23" version="5.2.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>Demo中三菱A-1E帧，修复bool读取显示失败的BUG。</item>
    ///             <item>数据订阅类客户端连接上服务器后，服务器立即推送一次。</item>
    ///             <item>串口设备基类代码提炼，提供了多种数据类型的读写支持。</item>
    ///             <item>仪表盘新增属性IsBigSemiCircle，设置为true之后，仪表盘可显示大于半圆的视图。</item>
    ///             <item>提供了一个新的三菱串口类，用于采集FX系列的PLC，MelsecFxSerial</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-08-24" version="5.2.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复双模式基类的一个bug，支持不接受反馈数据。</item>
    ///             <item>修复三菱串口类的读写bug，包括写入位，和读取字和位。</item>
    ///             <item>相关代码重构优化。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-09-08" version="5.3.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>串口基类接收数据优化，保证接收一次完整的数据内容。</item>
    ///             <item>新增一个容器罐子的控件，可以调整背景颜色。</item>
    ///             <item>OperateResult成功时的错误码调整为0。</item>
    ///             <item>修复modbus-tcp及modbus-rtu读取coil及discrete的1个位时解析异常的bug。</item>
    ///             <item>授权类公开一个属性，终极秘钥的属性，感谢 洛阳-LYG 的建议。</item>
    ///             <item>修复transbool方法在特殊情况下的bug</item>
    ///             <item>NetworkDeviceBase 写入的方法设置为了虚方法，允许子类进行重写。</item>
    ///             <item>SoftBasic: 新增三个字节处理的方法，移除前端字节，移除后端字节，移除两端字节。</item>
    ///             <item>新增串口应用的LRC校验方法。还未实际测试。</item>
    ///             <item>Siemens的s7协议支持V区自动转换，方便数据读取。</item>
    ///             <item>新增ab plc的类AllenBradleyNet，已测试读写，bool写入仍存在一点问题。</item>
    ///             <item>新增modbus-Ascii类，该类库还未仔细测试。</item>
    ///             <item>埃夫特机器人更新，适配最新版本数据采集。</item>
    ///             <item>其他的代码优化，重构精简</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-09-10" version="5.3.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复埃夫特机器人读取数据的bug，已测试通过。</item>
    ///             <item>ByteTransform数据转换层新增一个DataFormat属性，可选ABCD,BADC,CDAB,DCBA</item>
    ///             <item>三个modbus协议均适配了ByteTransform并提供了直接修改的属性，默认ABCD</item>
    ///             <item>注意：如果您的旧项目使用的Modbus类，请务必重新测试适配。给你带来的不便，敬请谅解。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-09-21" version="5.3.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>所有显示字符串支持中英文，支持切换，默认为系统语言。</item>
    ///             <item>Json组件依赖设置为不依赖指定版本。</item>
    ///             <item>modbus-ascii类库测试通过。</item>
    ///             <item>新增松下的plc串口读写类，还未测试。</item>
    ///             <item>西门子s7类写入byte数组长度不受限制，原先大概250个字节左右。</item>
    ///             <item>demo界面进行了部分的中英文适配。</item>
    ///             <item>OperateResult类新增了一些额外的构造方法。</item>
    ///             <item>SoftBasic新增了几个字节数组操作相关的通用方法。</item>
    ///             <item>其他大量的细节的代码优化，重构。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-09-27" version="5.3.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>DeviceNet层添加异步的API，支持async+await调用。</item>
    ///             <item>java修复西门子的写入成功却提示失败的bug。</item>
    ///             <item>java代码重构，和C#基本保持一致。</item>
    ///             <item>python版本发布，支持三菱，西门子，欧姆龙，modbus，数据订阅，同步访问。</item>
    ///             <item>其他的代码优化，重构精简。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-10-20" version="5.4.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>python和java的代码优化，完善，添加三菱A-1E类。</item>
    ///             <item>修复仪表盘控件，最大值小于0会产生的特殊Bug。</item>
    ///             <item>NetSimplifyClient: 提供高级.net的异步版本方法。</item>
    ///             <item>serialBase: 新增初始化和结束的保护方法，允许重写实现额外的操作。</item>
    ///             <item>softBuffer: 添加一个线程安全的buffer内存读写。</item>
    ///             <item>添加西门子ppi协议类，针对s7-200，需要最终测试。</item>
    ///             <item>Panasonic: 修复松下plc的读取读取数据异常。</item>
    ///             <item>修复fx协议批量读取bool时意外的Bug。</item>
    ///             <item>NetSimplifyClient: 新增带用户int数据返回的读取接口。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-10-24" version="5.4.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>新增一个温度采集模块的类，基于modbus-rtu实现，阿尔泰科技发展有限公司的DAM3601模块。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-10-25" version="5.4.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>三菱的mc协议新增支持读取ZR文件寄存器功能。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-10-30" version="5.4.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复AB PLC的bool和byte写入失败的bug，感谢 北京-XLang 提供的思路。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-11-1" version="5.5.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>新增西门子PPI通讯类库，支持200，200smart等串口通信，感谢 合肥-加劲 和 江阴-  ∮溪风-⊙_⌒ 的测试</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-11-5" version="5.5.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>新增三菱计算机链接协议通讯库，支持485组网，有效距离达50米，感谢珠海-刀客的测试。</item>
    ///             <item>串口协议的基类提供了检测当前串口是否处于打开的方法接口。</item>
    ///             <item>西门子S7协议新增槽号为3的s7-400的PLC选项，等待测试。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-11-9" version="5.5.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>西门子PPI写入bool方法名重载到了Write方法里。</item>
    ///             <item>松下写入bool方法名重载到了Write方法里。</item>
    ///             <item>修复CRC16验证码在某些特殊情况下的溢出bug。</item>
    ///             <item>西门子类添加槽号和机架号属性，只针对400PLC有效，初步测试可读写。</item>
    ///             <item>ab plc支持对数组的读写操作，支持数组长度为0-246，超过246即失败。</item>
    ///             <item>三菱的编程口协议修复某些特殊情况读取失败，却提示成功的bug。</item>
    ///             <item>串口基类提高缓存空间到4096，并在数据交互时捕获COM口的异常。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-11-16" version="5.6.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复欧姆龙的数据格式错误，修改为CDAB。</item>
    ///             <item>新增一个瓶子的控件。</item>
    ///             <item>新增一个管道的控件。</item>
    ///             <item>初步新增一个redis的类，初步实现了读写关键字。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-11-21" version="5.6.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>AB PLC读取数组过长时提示错误信息。</item>
    ///             <item>正式发布redis客户端，支持一些常用的操作，并提供一个浏览器。博客：https://www.cnblogs.com/dathlin/p/9998013.html </item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-11-24" version="5.6.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>曲线控件的曲线支持隐藏其中的一条或是多条曲线，可以用来实现手动选择显示曲线的功能。</item>
    ///             <item>Redis功能块代码优化，支持通知服务器进行数据快照保存，包括同步异步。</item>
    ///             <item>Redis新增订阅客户端类，可以实现订阅一个或是多个频道数据。</item>
    ///         </list>
    ///     </revision>
    /// </revisionHistory>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute( )]
    public class NamespaceDoc
    {

    }
}
