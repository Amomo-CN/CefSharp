// 引入System.Threading.Tasks命名空间，支持异步操作
using System.Threading.Tasks;
// 引入System.Windows命名空间，包含WPF的基本UI元素和特性
using System.Windows;
// 引入System.Windows.Controls命名空间，包含控件类
using System.Windows.Controls;

// 命名空间声明，属于CefSharp.Wpf.Example项目
namespace CefSharp.Wpf.Example
{
    // 交互逻辑类，与TestWindow.xaml窗体关联
    ///
    /// <summary>
    /// 提供TestWindow.xaml窗体的用户界面交互逻辑
    ///</summary>
    ///
    public partial class SpawnBrowsersWindow : Window
    {
        // 是否正在运行测试标志
        bool isRunning;
        // 请求取消操作的标志
        bool requestCancel;
        // 最大速度常量，用于计算延迟时间
        private const int MAX_SPEED = 1000;

        // 构造函数，初始化窗体和列表项
        public SpawnBrowsersWindow()
        {
            InitializeComponent();
            // 循环添加100个列表项
            for (int i = 1; i <= 100; i++)
            {
                itemList.Items.Add("Item " + i);
            }
        }

        // 列表项选择改变时的事件处理方法
        private async void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 如果容器已有浏览器，则先销毁
            if (browserContainer.Content is ChromiumWebBrowser)
            {
                ChromiumWebBrowser oldBrowser = browserContainer.Content as ChromiumWebBrowser;
                browserContainer.Content = null;
                oldBrowser.Dispose();
            }
            // 稍作延迟
            await Task.Delay(10);

            // 创建新浏览器并设置地址
            ChromiumWebBrowser browser = new ChromiumWebBrowser()
            {
                Address = "http://www.google.com"
            };
            // 将新浏览器放入容器
            browserContainer.Content = browser;
            // 注释掉的代码用于清空页面内容
            //browser.ExecuteScriptAsync("document.body.innerHTML = '';");
        }

        // 开始/停止测试按钮点击事件处理方法
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // 判断是否正在运行，决定是开始还是停止测试
            if (isRunning)
            {
                StartStopTest(false);
            }
            else
            {
                StartStopTest(true);

                // 遍历列表项并模拟测试
                for (int i = 0; i < itemList.Items.Count; i++)
                {
                    itemList.SelectedIndex = i;
                    // 根据滑块值延迟
                    await Task.Delay((int)(MAX_SPEED * speedSlider.Value));
                    // 检查是否请求取消
                    if (requestCancel)
                    {
                        break;
                    }
                }

                // 测试结束后重置状态
                StartStopTest(false);
            }
        }

        // 开始/停止测试的方法
        private void StartStopTest(bool isStart)
        {
            // 更新请求取消和运行状态
            requestCancel = !isStart;
            isRunning = isStart;
            // 更新按钮文本
            btnTest.Content = isStart ? "Cancel" : "Test";
            // 禁用或启用列表项选择
            itemList.IsEnabled = !isRunning;
        }

        // 滑块值改变时的事件处理方法
        private void speedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // 更新速度标签文本
            speedLabel.Content = string.Format("{0}ms", (int)(MAX_SPEED * speedSlider.Value));
        }
    }
}

/*

这段代码定义了一个名为SpawnBrowsersWindow的WPF窗口类
用于演示动态创建和销毁ChromiumWebBrowser实例的功能
以及通过滑块控制测试速度和开始/停止测试
窗体包括一个列表项选择、一个开始/停止按钮、一个速度调整滑块，以及相应的逻辑处理方法。
*/