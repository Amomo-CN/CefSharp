// 版权声明信息，指明源代码的使用许可遵循BSD风格的许可证，具体条款见LICENSE文件。

using System.Windows;          // 引入WPF基础类型和UI元素的命名空间

using CefSharp.Enums;         // 引入CefSharp库中枚举类型的命名空间

using Microsoft.Windows.Input.TouchKeyboard; // 引入Windows触摸键盘管理的命名空间

namespace CefSharp.Wpf.Example   // 命名空间声明，属于CefSharp.Wpf.Example项目
{
    ///
    /// <summary>
    /// TouchKeyboardWin10MainWindow类提供了一个基本的仅限Windows 10环境下的示例，
    /// 展示如何在WPF应用中显示屏幕（虚拟）键盘。
    ///</summary>
    ///
    public partial class TouchKeyboardWin10MainWindow : Window  // 类定义，继承自Window类
    {
        // 私有字段，用于管理触摸键盘事件
        private TouchKeyboardEventManager touchKeyboardEventManager;

        // 构造函数，初始化窗口及事件处理
        public TouchKeyboardWin10MainWindow()
        {
            // 调用基类的初始化方法以加载XAML定义的UI
            InitializeComponent();

            // 订阅浏览器的VirtualKeyboardRequested事件
            Browser.VirtualKeyboardRequested += BrowserVirtualKeyboardRequested;
            // 订阅浏览器初始化状态改变的事件
            Browser.IsBrowserInitializedChanged += BrowserIsBrowserInitializedChanged;
        }

        // 浏览器初始化状态变更时的事件处理方法
        private void BrowserIsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // 判断浏览器是否已初始化完成
            if ((bool)e.NewValue)
            {
                // 获取浏览器宿主句柄
                var browserHost = Browser.GetBrowserHost();
                // 使用宿主句柄创建TouchKeyboardEventManager实例
                touchKeyboardEventManager = new TouchKeyboardEventManager(browserHost.GetWindowHandle());
            }
            else
            {
                // 若浏览器未初始化或已销毁，释放TouchKeyboardEventManager资源
                if (touchKeyboardEventManager != null)
                {
                    touchKeyboardEventManager.Dispose();
                }
            }
        }

        // 虚拟键盘请求事件的处理方法
        private void BrowserVirtualKeyboardRequested(object sender, VirtualKeyboardRequestedEventArgs e)
        {
            // 通过TouchKeyboardEventManager获取输入面板实例
            var inputPane = touchKeyboardEventManager.GetInputPane();
            // 根据TextInputMode决定是否显示虚拟键盘
            if (e.TextInputMode == TextInputMode.None)
            {
                // 当不需要输入时尝试隐藏键盘
                inputPane.TryHide();
            }
            else
            {
                // 否则，尝试显示键盘
                inputPane.TryShow();
            }
        }
    }
}

/*
这段代码定义了一个名为TouchKeyboardWin10MainWindow的WPF窗口类
用于演示如何在WPF应用中显示屏幕（虚拟）键盘。
该类继承自Window类，并实现了一个私有字段touchKeyboardEventManager，
用于管理触摸键盘事件。
构造函数中，调用了基类的初始化方法以加载XAML定义的UI，
并订阅了浏览器的VirtualKeyboardRequested事件和浏览器初始化状态改变的事件。*/