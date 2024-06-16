// 版权声明，指出代码由CefSharp Authors创作，保留所有权利。
// 使用此源代码的许可遵循BSD风格的许可证，详细条款见LICENSE文件。

using System.Windows; // 引入WPF框架的基本类型

using CefSharp.Example; // 引入CefSharp示例项目的相关类
using CefSharp.Example.Handlers; // 引入CefSharp示例项目的处理器类
using CefSharp.Wpf.Example.Handlers; // 引入CefSharp WPF示例项目的处理器类

namespace CefSharp.Wpf.Example // 定义命名空间CefSharp.Wpf.Example
{
    // 公共部分类App，继承自WPF应用程序的Application类
    public partial class App : Application
    {
        // 在应用程序启动时执行的事件处理程序
        protected override void OnStartup(StartupEventArgs e)
        {
            // 注释掉的代码用于禁用触笔和触摸支持，启用指针支持
            //System.AppContext.SetSwitch("Switch.System.Windows.Input.Stylus.DisableStylusAndTouchSupport", true);
            System.AppContext.SetSwitch("Switch.System.Windows.Input.Stylus.EnablePointerSupport", true);

            // 检查是否处于调试模式，如果不是，显示警告消息
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                MessageBox.Show("当在Visual Studio之外运行此示例时，" +
                                "请确保以`Release`模式编译。", "警告");
            }
#endif

            // 定义是否使用多线程消息循环
            const bool multiThreadedMessageLoop = true;

            // 根据多线程消息循环的设置，创建相应的浏览器进程处理器
            IBrowserProcessHandler browserProcessHandler;

            if (multiThreadedMessageLoop)
            {
                browserProcessHandler = new BrowserProcessHandler();
            }
            else
            {
                browserProcessHandler = new WpfBrowserProcessHandler(Dispatcher);
            }

            // 创建Cef设置对象
            var settings = new CefSettings();
            // 设置是否使用多线程消息循环
            settings.MultiThreadedMessageLoop = multiThreadedMessageLoop;
            // 设置外部消息泵的使用情况
            settings.ExternalMessagePump = !multiThreadedMessageLoop;
            // 启用Chrome运行时
            settings.ChromeRuntime = true;

            // 初始化CefSharp，传入设置和浏览器进程处理器
            CefExample.Init(settings, browserProcessHandler: browserProcessHandler);

            // 调用基类的OnStartup方法，继续应用程序的启动流程
            base.OnStartup(e);
        }
    }
}

/*
这段代码定义了一个名为App的公共部分类，继承自WPF应用程序的Application类。
在OnStartup方法中，首先检查是否处于调试模式，如果不是，则显示一个警告消息。
然后，根据是否使用多线程消息循环，创建相应的浏览器进程处理器。
接着，创建CefSettings对象，并设置是否使用多线程消息循环和外部消息泵的使用情况。
最后，调用CefExample.Init方法，传入设置和浏览器进程处理器，初始化Cef
*/