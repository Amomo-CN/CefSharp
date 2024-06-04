// 版权声明，指出CefSharp的源代码使用BSD风格的许可协议，具体条款可在LICENSE文件中找到

// 引入System.Windows命名空间，包含WPF框架的基础类型和特性
using System.Windows;

// 命名空间声明，此代码属于CefSharp.Wpf.Example项目
namespace CefSharp.Wpf.Example
{
    /// <summary>
    /// ChangeParentWindow.xaml 的交互逻辑类说明。
    /// 此窗口用于演示如何改变ChromiumWebBrowser控件的父容器。
    ///</summary>
    public partial class ChangeParentWindow : Window
    {
        // 私有字段，用于存储ChromiumWebBrowser实例
        private ChromiumWebBrowser browser;
        // 私有字段，用于存储新创建的Window实例
        private Window newWindow;

        // 构造函数，初始化组件并设置StaticBrowser的内容为一个新的ChromiumWebBrowser实例，初始加载"http://www.msn.net"
        public ChangeParentWindow()
        {
            InitializeComponent();

            StaticBrowser.Child = new ChromiumWebBrowser("http://www.msn.net");
        }

        // 添加浏览器按钮点击事件处理方法，创建或重用ChromiumWebBrowser实例，并在一个新窗口中显示
        private void OnAddBrowser(object sender, RoutedEventArgs e)
        {
            // 如果browser尚未创建，则创建一个新的ChromiumWebBrowser实例
            if (browser == null)
            {
                browser = new ChromiumWebBrowser("http://www.msn.net");
            }

            // 如果newWindow尚未创建，则创建一个新的Window实例，设置其内容为browser，并居中显示
            if (newWindow == null)
            {
                newWindow = new Window
                {
                    Owner = this, // 设置新窗口的拥有者为当前窗口
                    Content = browser,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner // 确保新窗口在当前窗口中心打开
                };

                newWindow.Show(); // 显示新窗口
            }
        }

        // 移除浏览器按钮点击事件处理方法，关闭新窗口并将browser移回原始位置
        private void OnRemoveBrowser(object sender, RoutedEventArgs e)
        {
            // 如果newWindow存在，则清空其内容，关闭窗口，并将引用置为null
            if (newWindow != null)
            {
                newWindow.Content = null;
                newWindow.Close();
                newWindow = null;
            }

            // 将browser重新设置为BrowserSite容器的内容
            BrowserSite.Child = browser;
        }
    }
}