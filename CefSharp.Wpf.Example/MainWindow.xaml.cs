// 版权归2011年CefSharp作者所有。所有权利保留。
//
// 使用此源代码受BSD风格的许可协议约束，可在LICENSE文件中找到。
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

using CefSharp;
using CefSharp.Example;
using CefSharp.Example.Handlers;
using CefSharp.Wpf.Example;
using CefSharp.Wpf.Example.Controls;
using CefSharp.Wpf.Example.ViewModels;

using Microsoft.Win32;

namespace CefSharp.Wpf.Example
{
    public partial class MainWindow : Window
    {
        // 用于存储浏览器标签页的集合
        public ObservableCollection<BrowserTabViewModel> BrowserTabs { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // 初始化BrowserTabs集合
            BrowserTabs = new ObservableCollection<BrowserTabViewModel>();

            // 添加命令绑定：新建标签页
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, OpenNewTab));
            // 添加命令绑定：关闭标签页
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseTab));

            // 添加自定义CefSharp命令绑定
            CommandBindings.Add(new CommandBinding(CefSharpCommands.Exit, Exit));
            CommandBindings.Add(new CommandBinding(CefSharpCommands.OpenTabCommand, OpenTabCommandBinding));
            CommandBindings.Add(new CommandBinding(CefSharpCommands.PrintTabToPdfCommand, PrintToPdfCommandBinding));
            CommandBindings.Add(new CommandBinding(CefSharpCommands.CustomCommand, CustomCommandBinding));

            // 窗口加载完成后执行的事件
            Loaded += MainWindowLoaded;

            // 根据运行时架构添加标题信息
            var bitness = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
            Title += " - " + bitness;
        }

        // 关闭标签页的方法
        private void CloseTab(object sender, ExecutedRoutedEventArgs e)
        {
            if (BrowserTabs.Count > 0)
            {
                // 获取事件源
                var originalSource = (FrameworkElement)e.OriginalSource;

                // 获取要关闭的浏览器标签页
                BrowserTabViewModel browserViewModel;

                if (originalSource is MainWindow)
                {
                    // 如果事件源是主窗口，获取当前选中的标签页
                    browserViewModel = BrowserTabs[TabControl.SelectedIndex];
                    BrowserTabs.RemoveAt(TabControl.SelectedIndex);
                }
                else
                {
                    // 如果事件源是标签页，直接从集合中移除
                    browserViewModel = (BrowserTabViewModel)originalSource.DataContext;
                    BrowserTabs.Remove(browserViewModel);
                }

                // 销毁Web浏览器实例
                browserViewModel.WebBrowser.Dispose();
            }
        }

        // 新建标签页的方法
        private void OpenNewTab(object sender, ExecutedRoutedEventArgs e)
        {
            // 创建新标签页
            CreateNewTab();

            // 选择新创建的标签页
            TabControl.SelectedIndex = TabControl.Items.Count - 1;
        }

        // 窗口加载完成后的事件处理
        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            // 创建第一个标签页
            CreateNewTab(CefExample.DefaultUrl, true);
        }

        // 创建新标签页的方法
        private void CreateNewTab(string url = DefaultUrlForAddedTabs, bool showSideBar = false, bool legacyBindingEnabled = false)
        {
            // 添加新的浏览器标签页到集合
            BrowserTabs.Add(new BrowserTabViewModel(url) { ShowSidebar = showSideBar, LegacyBindingEnabled = legacyBindingEnabled });
        }

        // 处理自定义命令的方法
        private void CustomCommandBinding(object sender, ExecutedRoutedEventArgs e)
        {
            // 获取命令参数
            var param = e.Parameter.ToString();

            // 如果有标签页
            if (BrowserTabs.Count > 0)
            {
                // 获取事件源
                var originalSource = (FrameworkElement)e.OriginalSource;

                // 获取对应的浏览器标签页
                BrowserTabViewModel browserViewModel;

                if (originalSource is MainWindow)
                {
                    browserViewModel = BrowserTabs[TabControl.SelectedIndex];
                }
                else
                {
                    browserViewModel = (BrowserTabViewModel)originalSource.DataContext;
                }

                // 根据参数执行相应操作
                switch (param)
                {
                    case "CustomRequest":
                        browserViewModel.LoadCustomRequestExample();
                        break;
                    case "OpenDevTools":
                        browserViewModel.WebBrowser.ShowDevTools();
                        break;
                    case "ZoomIn":
                        var cmd = browserViewModel.WebBrowser.ZoomInCommand;
                        cmd.Execute(null);
                        break;
                    case "ZoomOut":
                        cmd = browserViewModel.WebBrowser.ZoomOutCommand;
                        cmd.Execute(null);
                        break;
                    case "ZoomReset":
                        cmd = browserViewModel.WebBrowser.ZoomResetCommand;
                        cmd.Execute(null);
                        break;
                    case "ToggleAudioMute":
                        cmd = browserViewModel.WebBrowser.ToggleAudioMuteCommand;
                        cmd.Execute(null);
                        break;
                    case "ClearHttpAuthCredentials":
                        var browserHost = browserViewModel.WebBrowser.GetBrowserHost();
                        if (browserHost != null && !browserHost.IsDisposed)
                        {
                            var requestContext = browserHost.RequestContext;
                            requestContext.ClearHttpAuthCredentials();
                            requestContext.ClearHttpAuthCredentialsAsync().ContinueWith(x =>
                            {
                                Console.WriteLine("RequestContext.ClearHttpAuthCredentials returned " + x.Result);
                            });
                        }
                        break;
                    case "LoadExtension":
                        var browser = browserViewModel.WebBrowser;
                        // 扩展仅支持http(s)协议
                        if (browser.Address.StartsWith("http"))
                        {
                            var requestContext = browser.GetBrowserHost().RequestContext;

                            var dir = Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\CefSharp.Example\Extensions");
                            dir = Path.GetFullPath(dir);
                            if (!Directory.Exists(dir))
                            {
                                throw new DirectoryNotFoundException("无法找到示例扩展文件夹 - " + dir);
                            }

                            var extensionHandler = new ExtensionHandler
                            {
                                LoadExtensionPopup = (url) =>
                                {
                                    Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        var extensionWindow = new Window();

                                        var extensionBrowser = new ChromiumWebBrowser(url);
                                        //extensionBrowser.IsBrowserInitializedChanged += (s, args) =>
                                        //{
                                        //    extensionBrowser.ShowDevTools();
                                        //};

                                        extensionWindow.Content = extensionBrowser;

                                        extensionWindow.Show();
                                    }));
                                },
                                GetActiveBrowser = (extension, isIncognito) =>
                                {
                                    // 返回扩展将作用的活动浏览器
                                    return browser.BrowserCore;
                                }
                            };

                            requestContext.LoadExtensionsFromDirectory(dir, extensionHandler);
                        }
                        else
                        {
                            MessageBox.Show("示例扩展仅支持http(s)协议，请加载其他网站后重试", "无法加载扩展");
                        }
                        break;
                    case "ToggleSidebar":
                        browserViewModel.ShowSidebar = !browserViewModel.ShowSidebar;
                        break;
                    case "ToggleDownloadInfo":
                        browserViewModel.ShowDownloadInfo = !browserViewModel.ShowDownloadInfo;
                        break;
                    case "ResizeHackTests":
                        ReproduceWasResizedCrashAsync();
                        break;
                    case "AsyncJsbTaskTests":
                        // 启用并发任务执行后，所有测试将通过ConcurrentMethodQueueRunner进行
                        CefSharpSettings.ConcurrentTaskExecution = true;

                        CreateNewTab(CefExample.BindingTestsAsyncTaskUrl, true);

                        TabControl.SelectedIndex = TabControl.Items.Count - 1;
                        break;
                    case "LegacyBindingTest":
                        CreateNewTab(CefExample.LegacyBindingTestUrl, true, legacyBindingEnabled: true);

                        TabControl.SelectedIndex = TabControl.Items.Count - 1;
                        break;

                        // 随着需要添加更多情况
                        //case "CustomRequest123":
                        //    browserViewModel.LoadCustomRequestExample();
                        //    break;
                }
            }
        }
    }
}// 将网页内容打印为PDF文件
private async void PrintToPdfCommandBinding(object sender, ExecutedRoutedEventArgs e)
{
    if (BrowserTabs.Count > 0)
    {
        var originalSource = (FrameworkElement)e.OriginalSource;

        BrowserTabViewModel browserViewModel;

        if (originalSource is MainWindow)
        {
            browserViewModel = BrowserTabs[TabControl.SelectedIndex];
        }
        else
        {
            browserViewModel = (BrowserTabViewModel)originalSource.DataContext;
        }

        // 显示保存文件对话框，设置默认扩展名为.pdf，过滤器为Pdf文档
        var dialog = new SaveFileDialog
        {
            DefaultExt = ".pdf",
            Filter = "Pdf documents (.pdf)|*.pdf"
        };

        // 如果用户选择了一个文件名并点击"保存"
        if (dialog.ShowDialog() == true)
        {
            // 尝试将网页内容保存为PDF
            var success = await browserViewModel.WebBrowser.PrintToPdfAsync(dialog.FileName, new PdfPrintSettings
            {
                // 设置自定义边距
                MarginType = CefPdfPrintMarginType.Custom,
                MarginBottom = 0.01,
                MarginTop = 0.01,
                MarginLeft = 0.01,
                MarginRight = 0.01,
            });

            // 如果保存成功
            if (success)
            {
                // 显示消息框提示用户PDF已保存
                MessageBox.Show("Pdf was saved to " + dialog.FileName);
            }
            else
            {
                // 显示消息框提示用户无法保存PDF，可能是因为没有写入权限
                MessageBox.Show("Unable to save Pdf, check you have write permissions to " + dialog.FileName);
            }
        }
    }
}

// 打开指定URL的新标签页
private void OpenTabCommandBinding(object sender, ExecutedRoutedEventArgs e)
{
    var url = e.Parameter.ToString();

    // 检查URL是否为空
    if (string.IsNullOrEmpty(url))
    {
        throw new Exception("Please provide a valid command parameter for binding");
    }

    // 创建新标签页并加载指定URL
    CreateNewTab(url, true);

    // 选择新创建的标签页
    TabControl.SelectedIndex = TabControl.Items.Count - 1;
}

// 关闭应用程序
private void Exit(object sender, ExecutedRoutedEventArgs e)
{
    // 关闭窗口
    Close();
}

// 关闭指定的浏览器标签页
private void CloseTab(BrowserTabViewModel browserViewModel)
{
    // 从集合中移除浏览器视图模型，并销毁Web浏览器实例
    if (BrowserTabs.Remove(browserViewModel))
    {
        browserViewModel.WebBrowser?.Dispose();
    }
}

// 重现可能导致崩溃的尺寸调整问题
private void ReproduceWasResizedCrashAsync()
{
    // 创建两个新标签页
    CreateNewTab();
    CreateNewTab();

    // 设置窗口状态为正常
    WindowState = WindowState.Normal;

    // 异步任务来模拟尺寸调整和标签页切换
    Task.Run(() =>
    {
        try
        {
            var random = new Random();

            // 循环20次
            for (int i = 0; i < 20; i++)
            {
                // 循环150次
                for (int j = 0; j < 150; j++)
                {
                    // 在主线程上执行尺寸调整和标签页切换
                    Dispatcher.Invoke(new Action(() =>
                    {
                        // 随机调整宽度
                        var newWidth = Width + (i % 2 == 0 ? -5 : 5);
                        // 随机调整高度
                        var newHeight = Height + (i % 2 == 0 ? -5 : 5);

                        // 确保宽度和高度在500到1500之间
                        if (newWidth < 500 || newWidth > 1500)
                        {
                            newWidth = 1000;
                        }
                        if (newHeight < 500 || newHeight > 1500)
                        {
                            newHeight = 1000;
                        }

                        // 设置新的尺寸
                        Width = newWidth;
                        Height = newHeight;

                        // 获取所有非当前选中的标签页索引
                        var indexes = new List<int>();
                        for (int k = 0; k < TabControl.Items.Count; k++)
                        {
                            if (TabControl.SelectedIndex != k)
                            {
                                indexes.Add(k);
                            }
                        }

                        // 随机选择一个非当前选中的标签页
                        TabControl.SelectedIndex = indexes[random.Next(0, indexes.Count)];

                        // 随机概率关闭一个标签页并创建新标签页
                        if (random.Next(0, 5) == 0)
                        {
                            CloseTab(BrowserTabs[Math.Max(1, TabControl.SelectedIndex)]); // 不关闭第一个标签页
                            CreateNewTab();
                        }
                    }));

                    // 随机睡眠时间
                    Thread.Sleep(random.Next(1, 11));
                }
            }
        }
        catch (TaskCanceledException) { } // 防止VS停止时引发异常
    });
}
