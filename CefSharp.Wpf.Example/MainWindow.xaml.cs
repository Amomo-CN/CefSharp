// 版权声明：此段代码的版权属于The CefSharp Authors，且遵循BSD风格许可协议，具体条款可在LICENSE文件中查阅。
// 使用此源代码需遵守该协议。

// 引入必要的命名空间，包括系统基础类库、CefSharp示例相关的类库、WPF相关控制及视图模型类库等。
using System; // 基础类型和公共语言运行库的类
using System.Collections.Generic; // 泛型集合
using System.Collections.ObjectModel; // 可观察的集合类型，常用于UI数据绑定
using System.IO; // 文件和流操作
using System.Runtime.InteropServices; // 用于平台调用服务
using System.Threading; // 线程管理
using System.Threading.Tasks; // 异步编程模型

using System.Windows; // WPF基本元素和特性
using System.Windows.Input; // 输入相关类，如命令绑定
using System.Windows.Threading;

using CefSharp.Example; // CefSharp示例项目提供的类
using CefSharp.Example.Handlers; // 示例中的处理器类
using CefSharp.Wpf.Example.Controls; // 示例中自定义的WPF控件
using CefSharp.Wpf.Example.ViewModels; // 示例中浏览器标签页的视图模型

using Microsoft.Win32; // 提供访问Windows注册表的类

// 定义命名空间，包含WPF示例应用的主要逻辑。
namespace CefSharp.Wpf.Example
{
    // 定义MainWindow类，它是WPF窗口的扩展，用于展示浏览器功能。
    public partial class MainWindow : Window
    {

        // 常量定义，表示新增标签页时默认打开的URL。
        private const string DefaultUrlForAddedTabs = "https://www.google.com";

        // 定义一个公开的可观察集合，用于存储浏览器标签页的视图模型。
        public ObservableCollection<BrowserTabViewModel> BrowserTabs { get; set; }

        // 构造函数，初始化MainWindow。
        public MainWindow()
        {
            // 调用基类的初始化方法，加载XAML定义的界面元素。
            InitializeComponent();
            // 设置窗口的数据上下文为自身，便于数据绑定。
            DataContext = this;

            // 初始化BrowserTabs集合，用于存放标签页数据。
            BrowserTabs = new ObservableCollection<BrowserTabViewModel>();

            // 绑定系统命令“新建”至OpenNewTab方法，用于新建标签页。
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, OpenNewTab));
            // 绑定系统命令“关闭”至CloseTab方法，用于关闭标签页。
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseTab));

            // 绑定CefSharp自定义命令至相应的方法，用于执行特定操作。
            CommandBindings.Add(new CommandBinding(CefSharpCommands.Exit, Exit)); // 退出应用
            CommandBindings.Add(new CommandBinding(CefSharpCommands.OpenTabCommand, OpenTabCommandBinding)); // 打开新标签页
            CommandBindings.Add(new CommandBinding(CefSharpCommands.PrintTabToPdfCommand, PrintToPdfCommandBinding)); // 打印当前标签页为PDF
            CommandBindings.Add(new CommandBinding(CefSharpCommands.CustomCommand, CustomCommandBinding)); // 自定义命令处理

            // 加载完成事件，当窗口加载完毕时触发。
            Loaded += MainWindowLoaded;

            // 获取当前进程的体系结构（32位或64位），并将其添加到窗口标题中以标识。
            var bitness = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
            Title += " - " + bitness;


        }

        // 方法：关闭当前选中的标签页。
        private void CloseTab(object sender, ExecutedRoutedEventArgs e)
        {
            // 检查是否有标签页存在。
            if (BrowserTabs.Count > 0)
            {
                // 获取触发此事件的原始元素。
                var originalSource = (FrameworkElement)e.OriginalSource;

                // 定义一个变量用来存储要关闭的标签页视图模型。
                BrowserTabViewModel browserViewModel;

                // 判断触发事件的元素是否为主窗口本身。
                if (originalSource is MainWindow)
                {
                    // 如果是，直接从集合中移除当前选中的标签页，并获取其视图模型。
                    browserViewModel = BrowserTabs[TabControl.SelectedIndex];
                    BrowserTabs.RemoveAt(TabControl.SelectedIndex);
                }
                else
                {
                    // 如果不是，则从触发事件元素的数据上下文中获取视图模型，并从集合中移除。
                    browserViewModel = (BrowserTabViewModel)originalSource.DataContext;
                    BrowserTabs.Remove(browserViewModel);
                }

                // 释放关闭标签页所对应的Web浏览器资源。
                browserViewModel.WebBrowser.Dispose();
            }
        }

        // 此方法响应“新建”命令，用于打开一个新的浏览器标签页。
        private void OpenNewTab(object sender, ExecutedRoutedEventArgs e)
        {
            // 调用CreateNewTab方法创建新标签页，使用默认URL。
            CreateNewTab();

            // 设置新创建的标签页为当前选中项。
            TabControl.SelectedIndex = TabControl.Items.Count - 1;
        }




        // 当MainWindow加载完成时，这个方法会被自动调用。
        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            // 调用CreateNewTab方法来创建一个新的浏览器标签页。
            // 参数包括默认的URL（由CefExample.DefaultUrl提供）和一个布尔值true，表示侧边栏应该被显示。
            //CreateNewTab(CefExample.DefaultUrl, true);//滴滴 默认打开了主页 不需要 - -

            // 创建新的浏览器标签页
            CreateNewTab("http://192.168.10.173:8088/", false); // APS
            CreateNewTab("http://192.168.10.209:8080/webroot/decision/v10/entry/access/734b3268-55f8-4e48-9c03-0fa78a8fd17d?width=2160&height=1078", false); // 报表 齐套分析1
            CreateNewTab("http://192.168.100.216:8081/U9C/mvc/main/index", false); //U9


            // 初始化已激活标签页的字典  不用了 因为下面已经实现了强制自动刷新
            //var activatedTabs = new Dictionary<int, bool>();

            //// 创建一个定时器
            //var timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromMilliseconds(150); // 设置定时器间隔为100毫秒
            //timer.Tick += (s, args) =>
            //{
            //    // 检查每个标签页
            //    for (int i = 0; i < TabControl.Items.Count; i++)
            //    {
            //        // 如果这个标签页还没有被激活过
            //        if (!activatedTabs.ContainsKey(i))
            //        {

            //            // 激活这个标签页
            //            TabControl.SelectedIndex = i;

            //            // 将这个标签页添加到已激活标签页的字典中
            //            activatedTabs[i] = true;

            //            // 退出循环，因为我们只需要在每个Tick中激活一个标签页
            //            break;
            //        }
            //    }
            //};
            //timer.Start(); // 启动定时器
        }


        // 此方法用于创建一个新的浏览器标签页，并将其添加到BrowserTabs集合中。
        // 它接受三个参数，其中url是新标签页将要加载的网页地址，默认是DefaultUrlForAddedTabs，
        // showSideBar是一个布尔值，指示是否在新标签页中显示侧边栏，默认不显示，
        // legacyBindingEnabled也是一个布尔值，指明是否启用旧式的数据绑定模式，默认不启用。
        private void CreateNewTab(string url = DefaultUrlForAddedTabs, bool showSideBar = false, bool legacyBindingEnabled = false)
        {
            // 实例化一个新的BrowserTabViewModel对象，传入指定的URL，并设置ShowSidebar和LegacyBindingEnabled属性。
            // BrowserTabViewModel是代表一个浏览器标签页的视图模型类，包含标签页的所有逻辑和数据。
            var newTabViewModel = new BrowserTabViewModel(url) { ShowSidebar = showSideBar, LegacyBindingEnabled = legacyBindingEnabled };


            // 将新创建的标签页设置为当前选中项。
            TabControl.SelectedIndex = BrowserTabs.Count;
            // 立即加载标签的内容
            newTabViewModel.WebBrowser.Load(url);


            // 将新创建的BrowserTabViewModel对象添加到BrowserTabs集合中。
            // BrowserTabs是一个集合，用于存储所有已打开的浏览器标签页的视图模型。
            BrowserTabs.Add(newTabViewModel);


            TabControl.UpdateLayout();// 强制更新布局，确保内容立即显示
        }
        // 当自定义命令绑定被触发时，此方法将被执行。
        private void CustomCommandBinding(object sender, ExecutedRoutedEventArgs e)
        {
            // 获取命令参数的字符串表示。
            var param = e.Parameter.ToString();

            // 检查是否有至少一个浏览器标签页。
            if (BrowserTabs.Count > 0)
            {
                // 获取触发事件的UI元素。
                var originalSource = (FrameworkElement)e.OriginalSource;

                // 避免重复代码，创建BrowserTabViewModel实例。
                BrowserTabViewModel browserViewModel;

                // 如果原始源是MainWindow，取TabControl当前选中的BrowserTabViewModel。
                if (originalSource is MainWindow)
                {
                    browserViewModel = BrowserTabs[TabControl.SelectedIndex];
                }
                // 如果原始源是其他元素，从其DataContext获取BrowserTabViewModel。
                else
                {
                    browserViewModel = (BrowserTabViewModel)originalSource.DataContext;
                }

                // 根据命令参数执行相应操作。
                if (param == "CustomRequest")
                {
                    // 加载自定义请求示例。
                    browserViewModel.LoadCustomRequestExample();
                }
                else if (param == "OpenDevTools")
                {
                    // 显示浏览器的开发者工具。
                    browserViewModel.WebBrowser.ShowDevTools();
                }
                else if (param == "ZoomIn")
                {
                    // 执行浏览器的放大命令。
                    var cmd = browserViewModel.WebBrowser.ZoomInCommand;
                    cmd.Execute(null);
                }
                else if (param == "ZoomOut")
                {
                    // 执行浏览器的缩小命令。
                    var cmd = browserViewModel.WebBrowser.ZoomOutCommand;
                    cmd.Execute(null);
                }
                else if (param == "ZoomReset")
                {
                    // 重置浏览器的缩放级别。
                    var cmd = browserViewModel.WebBrowser.ZoomResetCommand;
                    cmd.Execute(null);
                }
                else if (param == "ToggleAudioMute")
                {
                    // 切换浏览器的音频静音状态。
                    var cmd = browserViewModel.WebBrowser.ToggleAudioMuteCommand;
                    cmd.Execute(null);
                }
                else if (param == "ClearHttpAuthCredentials")
                {
                    // 获取浏览器的Host对象，如果可用且未被销毁，清除HTTP身份验证凭据。
                    var browserHost = browserViewModel.WebBrowser.GetBrowserHost();
                    if (browserHost != null && !browserHost.IsDisposed)
                    {
                        var requestContext = browserHost.RequestContext;
                        requestContext.ClearHttpAuthCredentials(); // 立即清除身份验证信息。
                        requestContext.ClearHttpAuthCredentialsAsync().ContinueWith(x => // 异步清除，等待完成并打印结果。
                        {
                            Console.WriteLine("请求上下文。返回了ClearHttpAuthCredentials" + x.Result);
                        });
                    }
                }

                else if (param == "LoadExtension")
                {
                    // 获取当前浏览器实例
                    var browser = browserViewModel.WebBrowser;

                    // 检查浏览器地址是否以http或https开头，因为示例扩展仅适用于这些方案
                    if (browser.Address.StartsWith("http"))
                    {
                        // 获取浏览器宿主的请求上下文
                        var requestContext = browser.GetBrowserHost().RequestContext;

                        // 设置扩展目录路径
                        var dir = Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\CefSharp.Example\Extensions");
                        // 获取完整路径
                        dir = Path.GetFullPath(dir);

                        // 确保扩展目录存在，否则抛出异常
                        if (!Directory.Exists(dir))
                        {
                            throw new DirectoryNotFoundException("无法找到示例扩展文件夹 - " + dir);
                        }

                        // 初始化扩展处理器，用于处理扩展加载和与浏览器的交互
                        var extensionHandler = new ExtensionHandler
                        {
                            LoadExtensionPopup = (url) =>
                            {
                                // 在UI线程中弹出一个新的窗口承载扩展内容
                                Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    var extensionWindow = new Window();
                                    var extensionBrowser = new ChromiumWebBrowser(url);
                                    extensionWindow.Content = extensionBrowser;
                                    extensionWindow.Show();
                                }));
                            },
                            GetActiveBrowser = (extension, isIncognito) =>
                            {
                                // 返回当前活动的浏览器实例，供扩展使用
                                return browser.BrowserCore;
                            }
                        };

                        // 从指定目录加载扩展
                        requestContext.LoadExtensionsFromDirectory(dir, extensionHandler);
                    }
                    else
                    {
                        // 显示消息框提示用户扩展仅支持http(s)协议
                        MessageBox.Show("示例扩展仅适用于http(s)协议，请加载不同网站后重试", "无法加载扩展");
                    }
                }

                else if (param == "ToggleSidebar")
                {
                    // 切换浏览器侧边栏的显示状态
                    browserViewModel.ShowSidebar = !browserViewModel.ShowSidebar;
                }

                else if (param == "ToggleDownloadInfo")
                {
                    // 切换下载信息的显示状态
                    browserViewModel.ShowDownloadInfo = !browserViewModel.ShowDownloadInfo;
                }

                else if (param == "ResizeHackTests")
                {
                    // 执行一个可能导致崩溃的测试模拟窗口大小调整问题
                    ReproduceWasResizedCrashAsync();
                }

                else if (param == "AsyncJsbTaskTests")
                {
                    // 开启并发任务执行模式，用于测试异步JavaScript绑定任务
                    CefSharpSettings.ConcurrentTaskExecution = true;
                    // 创建新标签页并加载测试URL
                    CreateNewTab(CefExample.BindingTestsAsyncTaskUrl, true);
                    // 将焦点切换到新创建的标签页
                    TabControl.SelectedIndex = TabControl.Items.Count - 1;
                }

                else if (param == "LegacyBindingTest")
                {
                    // 创建新标签页并加载旧式绑定测试页面，启用旧式绑定模式
                    CreateNewTab(CefExample.LegacyBindingTestUrl, true, legacyBindingEnabled: true);
                    // 将焦点切换到新创建的标签页
                    TabControl.SelectedIndex = TabControl.Items.Count - 1;
                }

                // 注意：根据需要添加更多命令处理逻辑
                // 示例：
                // else if (param == "CustomRequest123")
                // {
                //     browserViewModel.LoadCustomRequestExample();
                // }
            }
        }
        // 方法：打印到PDF命令绑定
        private async void PrintToPdfCommandBinding(object sender, ExecutedRoutedEventArgs e)
        {
            // 检查是否有打开的浏览器标签
            if (BrowserTabs.Count > 0)
            {
                // 获取触发事件的源元素
                var originalSource = (FrameworkElement)e.OriginalSource;

                // 获取BrowserTabViewModel实例
                BrowserTabViewModel browserViewModel;
                if (originalSource is MainWindow)
                {
                    // 如果源是主窗口，获取当前选中的BrowserTabViewModel
                    browserViewModel = BrowserTabs[TabControl.SelectedIndex];
                }
                else
                {
                    // 否则，从数据上下文中获取BrowserTabViewModel
                    browserViewModel = (BrowserTabViewModel)originalSource.DataContext;
                }

                // 创建一个保存文件对话框
                var dialog = new SaveFileDialog
                {
                    // 默认扩展名为.pdf
                    DefaultExt = ".pdf",
                    // 过滤器只允许选择PDF文件
                    Filter = "Pdf documents (.pdf)|*.pdf"
                };

                // 如果用户点击保存
                if (dialog.ShowDialog() == true)
                {
                    // 打印到PDF并等待结果
                    var success = await browserViewModel.WebBrowser.PrintToPdfAsync(
                        dialog.FileName, // 保存的文件名
                        new PdfPrintSettings
                        {
                            // 使用自定义页边距
                            MarginType = CefPdfPrintMarginType.Custom,
                            // 边距设置
                            MarginBottom = 0.01,
                            MarginTop = 0.01,
                            MarginLeft = 0.01,
                            MarginRight = 0.01,
                        });

                    // 如果打印成功
                    if (success)
                    {
                        // 显示消息框通知用户PDF已保存
                        MessageBox.Show("PDF已保存到 " + dialog.FileName);
                    }
                    else
                    {
                        // 否则，显示错误消息提示无法保存PDF
                        MessageBox.Show("无法保存PDF，请检查您是否拥有对的写入权限 " + dialog.FileName);
                    }
                }
            }
        }

        // 方法：打开新标签页命令绑定
        private void OpenTabCommandBinding(object sender, ExecutedRoutedEventArgs e)
        {
            // 获取传递的URL参数
            var url = e.Parameter.ToString();

            // 检查URL是否为空
            if (string.IsNullOrEmpty(url))
            {
                // 抛出异常，提示提供有效的URL参数
                throw new Exception("请为绑定提供有效的命令参数");
            }

            // 创建新标签页并加载URL
            CreateNewTab(url, true);

            // 将焦点切换到新创建的标签页
            TabControl.SelectedIndex = TabControl.Items.Count - 1;
        }

        // 方法：退出程序
        private void Exit(object sender, ExecutedRoutedEventArgs e)
        {
            // 关闭窗口，退出程序
            Close();
        }

        // 方法：关闭指定的浏览器标签页
        private void CloseTab(BrowserTabViewModel browserViewModel)
        {
            // 从集合中移除并释放BrowserTabViewModel
            if (BrowserTabs.Remove(browserViewModel))
            {
                browserViewModel.WebBrowser?.Dispose();
            }
        }

        // 方法：模拟导致崩溃的窗口大小调整
        private void ReproduceWasResizedCrashAsync()
        {
            // 创建两个新的标签页
            CreateNewTab();
            CreateNewTab();

            // 设置窗口状态为正常
            WindowState = WindowState.Normal;

            // 异步任务模拟崩溃
            Task.Run(() =>
            {
                try
                {
                    // 初始化随机数生成器
                    var random = new Random();

                    // 循环20次
                    for (int i = 0; i < 2; i++)
                    {
                        // 循环150次
                        for (int j = 0; j < 50; j++)
                        {
                            // 在主线程上执行操作
                            Dispatcher.Invoke(new Action(() =>
                            {
                                // 随机调整窗口宽度和高度
                                var newWidth = Width + (i % 2 == 0 ? -5 : 5);
                                var newHeight = Height + (i % 2 == 0 ? -5 : 5);
                                // 限制窗口尺寸范围
                                if (newWidth < 500 || newWidth > 1500)
                                {
                                    newWidth = 1000;
                                }
                                if (newHeight < 500 || newHeight > 1500)
                                {
                                    newHeight = 1000;
                                }
                                // 更新窗口尺寸
                                Width = newWidth;
                                Height = newHeight;

                                // 获取未选中的标签页索引列表
                                var indexes = new List<int>();
                                for (int k = 0; k < TabControl.Items.Count; k++)
                                {
                                    if (TabControl.SelectedIndex != k)
                                    {
                                        indexes.Add(k);
                                    }
                                }

                                // 随机选择一个未选中的标签页
                                TabControl.SelectedIndex = indexes[random.Next(0, indexes.Count)];

                                // 有一定概率关闭并创建新的标签页
                                if (random.Next(0, 5) == 0)
                                {
                                    // 不关闭第一个标签页
                                    CloseTab(BrowserTabs[Math.Max(1, TabControl.SelectedIndex)]);
                                    CreateNewTab();
                                }
                            }));

                            // 随机睡眠一段时间
                            Thread.Sleep(random.Next(1, 11));
                        }
                    }
                }
                catch (TaskCanceledException) { } // 防止在VS调试时中断
            });
        }
    }
}
