// 版权声明，指出CefSharp项目的作者及源代码使用的BSD风格许可协议
// 使用者需遵守LICENSE文件中的条款

using System; // 引入基础类库，提供基本的类型和命名空间
using System.Windows; // 引入WPF类库，提供窗口和UI元素相关功能
using System.Windows.Controls; // 引入WPF控件类库，用于处理控件和布局
using System.Windows.Media; // 引入WPF媒体类库，用于处理图形和颜色

// 命名空间定义，组织相关的类和接口
namespace CefSharp.Wpf.Example.Handlers
{
    // 自定义DisplayHandler类，继承自CefSharp.Handler.DisplayHandler，处理全屏模式切换
    public class DisplayHandler : CefSharp.Handler.DisplayHandler
    {
        // 存储父容器，用于在全屏模式下恢复布局
        private Grid parent;

        // 存储全屏窗口实例，用于在退出全屏模式时关闭
        private Window fullScreenWindow;

        // 重写OnFullscreenModeChange方法，处理全屏模式切换事件
        // 参数：
        // - chromiumWebBrowser：CefSharp中的Web浏览器实例
        // - browser：浏览器对象
        // - fullscreen：是否进入全屏模式
        protected override void OnFullscreenModeChange(IWebBrowser chromiumWebBrowser, IBrowser browser, bool fullscreen)
        {
            var webBrowser = (ChromiumWebBrowser)chromiumWebBrowser;

            // 在UI线程上执行操作，确保与界面交互的安全
            webBrowser.Dispatcher.BeginInvoke((Action)(() =>
            {
                if (fullscreen)
                {
                    // 在这个示例中，父容器是Grid，如果您的父容器是其他类型的控件，请相应地更新代码
                    parent = (Grid)VisualTreeHelper.GetParent(webBrowser);

                    // 注意：如果ChromiumWebBrowser实例没有直接引用DataContext（例如BrowserTabViewModel）
                    // 则在退出全屏模式时，数据绑定可能不会更新，导致问题（如浏览器重新加载Url）
                    parent.Children.Remove(webBrowser);

                    // 创建全屏窗口，设置无边框和最大化状态
                    fullScreenWindow = new Window
                    {
                        WindowStyle = WindowStyle.None,
                        WindowState = WindowState.Maximized,
                        // 设置ChromiumWebBrowser为全屏窗口的内容
                        Content = webBrowser
                    };

                    // 显示全屏窗口
                    fullScreenWindow.ShowDialog();
                }
                else
                {
                    // 退出全屏模式时，清空全屏窗口内容
                    fullScreenWindow.Content = null;

                    // 将ChromiumWebBrowser添加回原来的父容器
                    parent.Children.Add(webBrowser);

                    // 关闭全屏窗口
                    fullScreenWindow.Close();
                    fullScreenWindow = null;
                    parent = null;
                }
            }));
        }
    }
}