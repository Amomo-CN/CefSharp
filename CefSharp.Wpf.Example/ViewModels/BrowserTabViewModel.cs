// Copyright © 2013 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using CefSharp.Example;

// 命名空间声明，包含示例WPF应用的视图模型
namespace CefSharp.Wpf.Example.ViewModels
{
    // 实现INotifyPropertyChanged接口的浏览器标签页视图模型类
    public class BrowserTabViewModel : INotifyPropertyChanged
    {
        // 用户将访问的网页地址
        private string address;
        public string Address
        {
            get { return address; }
            // 属性更改通知，用于界面更新
            set { Set(ref address, value); }
        }

        // 可编辑的地址栏文本，区别于实际地址用于输入验证或预处理
        private string addressEditable;
        public string AddressEditable
        {
            get { return addressEditable; }
            set { Set(ref addressEditable, value); }
        }

        // 浏览器输出的消息，如日志信息
        private string outputMessage;
        public string OutputMessage
        {
            get { return outputMessage; }
            set { Set(ref outputMessage, value); }
        }

        // 当前页面的状态消息，例如加载状态
        private string statusMessage;
        public string StatusMessage
        {
            get { return statusMessage; }
            set { Set(ref statusMessage, value); }
        }





        // 页面标题
        private string title;
        public string Title
        {
            get { return title; }
            set { Set(ref title, value); }
        }

        // WPF Web浏览器控件实例
        private IWpfWebBrowser webBrowser;
        public IWpfWebBrowser WebBrowser
        {
            get { return webBrowser; }
            set { Set(ref webBrowser, value); }
        }

        // JavaScript执行结果，可为任意类型
        private object evaluateJavaScriptResult;
        public object EvaluateJavaScriptResult
        {
            get { return evaluateJavaScriptResult; }
            set { Set(ref evaluateJavaScriptResult, value); }
        }

        // 是否显示侧边栏的标志
        private bool showSidebar;
        public bool ShowSidebar
        {
            get { return showSidebar; }
            set { Set(ref showSidebar, value); }
        }

        // 是否显示下载信息的标志
        private bool showDownloadInfo;
        public bool ShowDownloadInfo
        {
            get { return showDownloadInfo; }
            set { Set(ref showDownloadInfo, value); }
        }

        // 上一次下载操作的描述
        private string lastDownloadAction;
        public string LastDownloadAction
        {
            get { return lastDownloadAction; }
            set { Set(ref lastDownloadAction, value); }
        }

        // 下载项详细信息
        private DownloadItem downloadItem;
        public DownloadItem DownloadItem
        {
            get { return downloadItem; }
            set { Set(ref downloadItem, value); }
        }

        // 控制是否启用旧式绑定模式的标志
        private bool legacyBindingEnabled;

        // 属性更改事件，用于界面数据绑定
        public event PropertyChangedEventHandler PropertyChanged;

        // 获取或设置是否启用旧式绑定模式的属性
        public bool LegacyBindingEnabled
        {
            get { return legacyBindingEnabled; }
            set { Set(ref legacyBindingEnabled, value); }
        }

        // 命令属性，用于页面跳转操作
        public ICommand GoCommand { get; private set; }

        // 命令属性，用于返回主页操作
        public ICommand HomeCommand { get; private set; }

        // 命令属性，用于执行指定的JavaScript代码
        public ICommand ExecuteJavaScriptCommand { get; private set; }

        // 命令属性，用于评估JavaScript表达式并获取结果
        public ICommand EvaluateJavaScriptCommand { get; private set; }

        // 命令属性，用于显示开发者工具
        public ICommand ShowDevToolsCommand { get; private set; }

        // 命令属性，用于关闭开发者工具
        public ICommand CloseDevToolsCommand { get; private set; }

        // 命令属性，用于进行JavaScript绑定压力测试
        public ICommand JavascriptBindingStressTest { get; private set; }

        // 构造函数，初始化视图模型并设置地址、命令及默认行为
        public BrowserTabViewModel(string address)
        {
            // 初始化地址和可编辑地址
            Address = address;
            AddressEditable = Address;

            WebBrowser = new ChromiumWebBrowser();

            // 定义Go命令，当地址非空或空白时可执行页面跳转
            GoCommand = new RelayCommand(Go, () => !string.IsNullOrWhiteSpace(Address));

            // 定义HomeCommand命令，点击后将地址重置为默认URL
            HomeCommand = new RelayCommand(() => AddressEditable = Address = CefExample.DefaultUrl);

            // 执行JavaScript代码的命令，需要传入非空字符串
            ExecuteJavaScriptCommand = new RelayCommand<string>(ExecuteJavaScript, s => !string.IsNullOrWhiteSpace(s));

            // 评估JavaScript表达式的命令，同样要求非空字符串输入
            EvaluateJavaScriptCommand = new RelayCommand<string>(EvaluateJavaScript, s => !string.IsNullOrWhiteSpace(s));

            // 显示开发者工具的命令
            ShowDevToolsCommand = new RelayCommand(() => webBrowser.ShowDevTools());

            // 关闭开发者工具的命令
            CloseDevToolsCommand = new RelayCommand(() => webBrowser.CloseDevTools());

            // JavaScript绑定压力测试命令，加载特定测试页并在加载完成后10秒刷新页面
            JavascriptBindingStressTest = new RelayCommand(() =>
            {
                WebBrowser.Load(CefExample.BindingTestUrl);
                WebBrowser.LoadingStateChanged += (e, args) =>
                {
                    if (!args.IsLoading)
                    {
                        Task.Delay(10000).ContinueWith(t =>
                        {
                            if (WebBrowser != null)
                            {
                                WebBrowser.Reload();
                            }
                        });
                    }
                };
            });

            // 监听属性变化事件，以便在属性更新时执行相应操作
            PropertyChanged += OnPropertyChanged;

            // 设置输出消息为当前使用的CefSharp版本信息
            var version = string.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}", Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion);
            OutputMessage = version;
        }

        // 异步方法，评估JavaScript表达式并处理结果或异常
        private async void EvaluateJavaScript(string s)
        {
            try
            {
                var response = await webBrowser.EvaluateScriptAsync(s);
                // 如果脚本执行成功且结果为回调，则进一步执行回调
                if (response.Success && response.Result is IJavascriptCallback)
                {
                    response = await ((IJavascriptCallback)response.Result).ExecuteAsync("这是来自EvaluateJavaScript的回调");
                }

                // 显示执行结果或错误信息
                EvaluateJavaScriptResult = response.Success ? (response.Result?.ToString() ?? "null") : response.Message;
            }
            catch (Exception e)
            {
                // 弹窗显示执行JavaScript时的错误信息
                MessageBox.Show($"JavaScript评估时出错: {e.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 执行JavaScript代码的方法，不等待结果直接执行
        private void ExecuteJavaScript(string s)
        {
            try
            {
                webBrowser.ExecuteScriptAsync(s);
            }
            catch (Exception e)
            {
                // 弹窗显示执行JavaScript时的错误信息
                MessageBox.Show($"执行JavaScript时出错: {e.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 属性变更事件处理器，响应不同属性的变更
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Address":
                    // 地址更改时同步更新可编辑地址
                    AddressEditable = Address;
                    break;

                case "WebBrowser":
                    // Web浏览器实例变更时，注册控制台消息和状态消息的事件处理
                    if (WebBrowser != null)
                    {
                        WebBrowser.ConsoleMessage += OnWebBrowserConsoleMessage;
                        WebBrowser.StatusMessage += OnWebBrowserStatusMessage;
                    }
                    break;
            }
        }

        // 控制台消息事件处理方法
        private void OnWebBrowserConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            // 更新输出消息为最新的控制台日志
            OutputMessage = e.Message;
        }

        // 状态消息事件处理方法
        private void OnWebBrowserStatusMessage(object sender, StatusMessageEventArgs e)
        {
            // 更新状态消息
            StatusMessage = e.Value;
        }

        // 跳转到用户输入的地址
        private void Go()
        {
            Address = AddressEditable;

            // 将焦点设置回Web浏览器
            Keyboard.Focus(WebBrowser);
        }

        // 加载自定义请求示例，演示带有POST数据的页面加载
        public void LoadCustomRequestExample()
        {
            var postData = System.Text.Encoding.Default.GetBytes("test=123&data=456");

            // 使用POST数据加载指定页面
            WebBrowser.LoadUrlWithPostData("https://cefsharp.com/PostDataTest.html", postData);
        }

        // 设置属性值并触发通知的辅助方法
        protected void Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null, [CallerMemberName] string caller = null)
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            // 输出到控制台，使用中文提示并添加关键词 "滴滴-"

            Debug.WriteLine($"滴滴-{propertyName} 属性在 {DateTime.Now} 被 {caller} 函数修改。");
        }

    }
}
