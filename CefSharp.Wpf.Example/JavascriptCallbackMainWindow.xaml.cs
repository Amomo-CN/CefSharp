// 导入System.Threading.Tasks命名空间，用于异步编程
using System.Threading.Tasks;
// 导入System.Windows命名空间，包含WPF的基本组件和类
using System.Windows;
// 导入CefSharp.Example.JavascriptBinding命名空间，包含JavaScript回调绑定对象
using CefSharp.Example.JavascriptBinding;

// 命名空间声明，此代码属于CefSharp.Wpf.Example项目
namespace CefSharp.Wpf.Example
{
    // 交互逻辑类，用于JavascriptCallbackMainWindow.xaml窗口
    public partial class JavascriptCallbackMainWindow : Window
    {
        // 私有字段，用于存储两个JavaScript回调绑定对象
        private JavascriptCallbackBoundObject boundObjectOne;
        private JavascriptCallbackBoundObject boundObjectTwo;

        // 构造函数，初始化组件并注册JavaScript回调对象
        public JavascriptCallbackMainWindow()
        {
            InitializeComponent();

            // 创建两个JavaScript回调绑定对象，分别与BrowserOne和BrowserTwo关联
            boundObjectOne = new JavascriptCallbackBoundObject(BrowserOne);
            boundObjectTwo = new JavascriptCallbackBoundObject(BrowserTwo);

            // 如果是.NET Core环境，直接注册JavaScript对象仓库
            #if NETCOREAPP
                BrowserOne.JavascriptObjectRepository.Register("boundObject", boundObjectOne);
                BrowserTwo.JavascriptObjectRepository.Register("boundObject", boundObjectTwo);
            // 否则，对于非.NET Core环境，注册JavaScript对象仓库，第三个参数为false表示不启用异步调用
            #else
                BrowserOne.JavascriptObjectRepository.Register("boundObject", boundObjectOne, false);
                BrowserTwo.JavascriptObjectRepository.Register("boundObject", boundObjectTwo, false);
            #endif
        }

        // 立即执行回调方法点击事件处理程序
        private void ExecuteCallbackImmediatelyClick(object sender, RoutedEventArgs e)
        {
            // 调用两个绑定对象的RunCallback方法
            boundObjectOne.RunCallback();
            boundObjectTwo.RunCallback();

            // 重新加载两个浏览器
            BrowserOne.Reload();
            BrowserTwo.Reload();
        }

        // 三秒后执行回调方法点击事件处理程序
        private void ExecuteCallbackInThreeSeconds(object sender, RoutedEventArgs e)
        {
            // 更改两个浏览器的地址
            BrowserOne.Address = "custom://cefsharp/SchemeTest.html";
            BrowserTwo.Address = "custom://cefsharp/SchemeTest.html";

            // 延迟3秒后执行任务
            Task.Delay(3000).ContinueWith(t =>
            {
                // 调用两个绑定对象的RunCallback方法
                boundObjectOne.RunCallback();
                boundObjectTwo.RunCallback();
            });
        }
    }
}

/*
这段代码定义了一个WPF窗口类JavascriptCallbackMainWindow
用于展示如何在CefSharp（Chromium Embedded Framework）中使用JavaScript回调
窗口中有两个ChromiumWebBrowser控件（BrowserOne和BrowserTwo）
并各自绑定了一个JavascriptCallbackBoundObject实例，用于处理JavaScript回调
ExecuteCallbackImmediatelyClick方法立即执行回调并重新加载浏览器
而ExecuteCallbackInThreeSeconds方法在3秒后执行回调
代码中还使用了条件编译指令#if NETCOREAPP来区分.NET Core和非.NET Core环境下的注册方法
*/