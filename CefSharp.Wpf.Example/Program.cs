// 版权声明，指出CefSharp的源代码使用BSD风格的许可协议，具体条款可在LICENSE文件中找到

// 引入System命名空间，包含基础类型和类
using System;

// 命名空间声明，此代码属于CefSharp.Wpf.Example项目
namespace CefSharp.Wpf.Example
{
    // 定义一个静态类Program，作为WPF应用程序的入口点
    public static class Program
    {
        // 标记为STAThread，指示该线程需要使用单线程公寓状态（STA），这是WPF应用程序所必需的
        [STAThread]
        // 主方法，应用程序的起点，接收命令行参数
        public static int Main(string[] args)
        {
            // 创建一个新的App实例，这是WPF应用程序的主类
            var application = new App();

            // 初始化应用程序组件
            application.InitializeComponent();

            // 运行应用程序，返回应用程序的退出代码
            return application.Run();
        }
    }
}
/*这段代码是CefSharp WPF示例项目的主程序入口点。它使用[STAThread]属性标记Main方法
以确保线程使用单线程公寓状态（STA），这是运行WPF应用程序所必需的。Main方法创建一个App实例
初始化组件，然后运行应用程序。App.Run()方法会启动消息循环，直到应用程序关闭，此时返回应用程序的退出代码。
*/