// 版权所有 © 2013 CefSharp 作者。保留所有权利。
//
// 本源代码的使用受BSD风格许可证的管辖，该许可证可以在LICENSE文件中找到。

using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CefSharp.Example;
using CefSharp.Example.Handlers;
using CefSharp.Example.JavascriptBinding;
using CefSharp.Example.ModelBinding;
using CefSharp.Example.PostMessage;
using CefSharp.Fluent;
using CefSharp.Wpf.Example.Handlers;
using CefSharp.Wpf.Example.ViewModels;
using CefSharp.Wpf.Experimental;
using CefSharp.Wpf.Experimental.Accessibility;

namespace CefSharp.Wpf.Example.Views
{
    public partial class BrowserTabView : UserControl
    {
        // 存储可拖拽区域，如果我们有一个 - 用于击中测试
        private Region region;

        public BrowserTabView()
        {
            InitializeComponent();

            // 在DataContext更改时触发事件
            DataContextChanged += OnDataContextChanged;

            // 使用PopupMouseTransform来启用鼠标变换
            browser.UsePopupMouseTransform();

            // 设置浏览器的背景颜色
            // browser.BrowserSettings.BackgroundColor = Cef.ColorSetARGB(0, 255, 255, 255);

            // 请删除以下注释以使用Experimental WpfImeKeyboardHandler。
            // browser.WpfKeyboardHandler = new CefSharp.Wpf.Experimental.WpfImeKeyboardHandler(browser);

            // 请删除以下注释以指定CompositionUnderline的颜色。
            // var transparent = Colors.Transparent;
            // var black = Colors.Black;
            // ImeHandler.ColorBKCOLOR = Cef.ColorSetARGB(transparent.A, transparent.R, transparent.G, transparent.B);
            // ImeHandler.ColorUNDERLINE = Cef.ColorSetARGB(black.A, black.R, black.G, black.B);

            // 设置请求处理器
            browser.RequestHandler = new ExampleRequestHandler();

            // 测试处理器允许所有权限
            browser.PermissionHandler = new ExamplePermissionHandler();

            // 创建绑定选项
            var bindingOptions = new BindingOptions()
            {
                Binder = BindingOptions.DefaultBinder.Binder,
                MethodInterceptor = new MethodInterceptorLogger(), // 截获从js到.net的方法调用并记录它
#if !NETCOREAPP
                PropertyInterceptor = new PropertyInterceptorLogger()
#endif
            };

            // 若要使用ResolveObject并绑定具有isAsync:false的对象，我们必须在浏览器初始化之前将CefSharpSettings.WcfEnabled设置为true。
#if !NETCOREAPP
            CefSharpSettings.WcfEnabled = true;
#endif

            // 如果您在javascript中调用CefSharp.BindObjectAsync并传递未绑定的对象名称，则ResolveObject将被调用，您可以然后注册它
            browser.JavascriptObjectRepository.ResolveObject += (sender, e) =>
            {
                var repo = e.ObjectRepository;

                // 当JavascriptObjectRepository.Settings.LegacyBindingEnabled = true时
                // 这个事件将被raised with ObjectName == Legacy，因此您可以绑定遗留对象
#if NETCOREAPP
                if (e.ObjectName == "Legacy")
                {
                    repo.Register("boundAsync", new AsyncBoundObject(), options: bindingOptions);
                }
                else
                {
                    if (e.ObjectName == "boundAsync")
                    {
                        repo.Register("boundAsync", new AsyncBoundObject(), options: bindingOptions);
                    }
                    else if (e.ObjectName == "boundAsync2")
                    {
                        repo.Register("boundAsync2", new AsyncBoundObject(), options: bindingOptions);
                    }
                }
#else
                if (e.ObjectName == "Legacy")
                {
                    repo.Register("bound", new BoundObject(), isAsync: false, options: BindingOptions.DefaultBinder);
                    repo.Register("boundAsync", new AsyncBoundObject(), isAsync: true, options: bindingOptions);
                }
                else
                {
                    if (e.ObjectName == "bound")
                    {
                        repo.Register("bound", new BoundObject(), isAsync: false, options: bindingOptions);
                    }
                    else if (e.ObjectName == "boundAsync")
                    {
                        repo.Register("boundAsync", new AsyncBoundObject(), isAsync: true, options: bindingOptions);
                    }
                    else if (e.ObjectName == "boundAsync2")
                    {
                        repo.Register("boundAsync2", new AsyncBoundObject(), isAsync: true, options: bindingOptions);
                    }
                }
#endif
            };

            browser.JavascriptObjectRepository.ObjectBoundInJavascript += (sender, e) =>
            {
                var name = e.ObjectName;

                Debug.WriteLine($"Object {e.ObjectName} was bound successfully.");
            };

            // 设置显示处理器
            browser.DisplayHandler = new DisplayHandler();

            // 设置生命周期处理器
            // browser.LifeSpanHandler = CefSharp.Wpf.Experimental.LifeSpanHandler
            //     .Create()
            //     .OnPopupCreated((ctrl, targetUrl, targetFrameName, windowInfo) =>
            //     {
            //         var windowX = (windowInfo.X == int.MinValue) ? double.NaN : windowInfo.X;
            //         var windowY = (windowInfo.Y == int.MinValue) ? double.NaN : windowInfo.Y;
            //         var windowWidth = (windowInfo.Width == int.MinValue) ? double.NaN : windowInfo.Width;
            //         var windowHeight = (windowInfo.Height == int.MinValue) ? double.NaN : windowInfo.Height;

            //         var popup = new System.Windows.Window
            //         {
            //             Left = windowX,
            //             Top = windowY,
            //             Width = windowWidth,
            //             Height = windowHeight,
            //             Content = ctrl,
            //             Owner = Window.GetWindow(browser),
            //             Title = targetFrameName
            //         };

            //         popup.Closed += (o, e) =>
            //         {
            //             var w = o as System.Windows.Window;
            //             if (w != null && w.Content is IWebBrowser)
            //             {
            //                 (w.Content as IWebBrowser)?.Dispose();
            //                 w.Content = null;
            //             }
            //         };
            //     })
            //     .OnPopupBrowserCreated((ctrl, browser) =>
            //     {
            //         ctrl.Dispatcher.Invoke(() =>
            //         {
            //             var owner = System.Windows.Window.GetWindow(ctrl);

            //             if (owner != null && owner.Content == ctrl)
            //             {
            //                 owner.Show();
            //             }
            //         });
            //     })
            //     .OnPopupDestroyed((ctrl, popupBrowser) =>
            //     {
            //         // 如果浏览器已被disposed，则不需要删除tab
            //         if (!ctrl.IsDisposed)
            //         {
            //             var owner = System.Windows.Window.GetWindow(ctrl);

            //             if (owner != null && owner.Content == ctrl)
            //             {
            //                 owner.Close();
            //             }
            //         }
            //     }).Build();

            // 设置菜单处理器
            browser.MenuHandler = new MenuHandler(addDevtoolsMenuItems: true);

            // 启用实验性Accessibility支持
            browser.AccessibilityHandler = new AccessibilityHandler(browser);
            browser.IsBrowserInitializedChanged += (sender, args) =>
            {
                if ((bool)args.NewValue)
                {
                    // Uncomment to enable support
                    // browser.GetBrowserHost().SetAccessibilityState(CefState.Enabled);
                }
            };

            // 设置下载处理器
            browser.DownloadHandler = DownloadHandler
                .Create()
                .CanDownload((chromiumWebBrowser, browser, url, requestMethod) =>
                {
                    // 允许所有下载
                    return true;
                })
                .OnBeforeDownload((chromiumWebBrowser, browser, downloadItem, callback) =>
                {
                    UpdateDownloadAction("OnBeforeDownload", downloadItem);

                    // 显示对话框提示用户保存
                    callback.Continue("", showDialog: true);

                    return true;

                }).OnDownloadUpdated((chromiumWebBrowser, browser, downloadItem, callback) =>
                {
                    UpdateDownloadAction("OnDownloadUpdated", downloadItem);
                })
                .Build();

            // 设置音频处理器
            browser.AudioHandler = new CefSharp.Handler.AudioHandler();

            // 设置JavaScript对话框处理器
            browser.JsDialogHandler = new Handlers.JsDialogHandler();

            // 读取嵌入式位图到内存流，然后注册它作为资源，以便可以加载自定义://cefsharp/images/beach.jpg
            var beachImageStream = new MemoryStream();
            CefSharp.Example.Properties.Resources.beach.Save(beachImageStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            browser.RegisterResourceHandler(CefExample.BaseUrl + "/images/beach.jpg", beachImageStream, Cef.GetMimeType("jpg"));

            // 创建拖拽处理器
            var dragHandler = new DragHandler();
            dragHandler.RegionsChanged += OnDragHandlerRegionsChanged;

            // 设置拖拽处理器
            browser.DragHandler = dragHandler;

            // 设置资源处理器工厂
            // browser.ResourceHandlerFactory = new InMemorySchemeAndResourceHandlerFactory();

            // 可以指定自定义的RequestContext来共享设置在一组ChromiumWebBrowsers中
            // browser.RequestContext = new RequestContext(new RequestContextHandler());

            // 注意 - 这对于这个示例非常重要，因为默认页面将不会加载否则
            // browser.RequestContext.RegisterSchemeHandlerFactory(CefSharpSchemeHandlerFactory.SchemeName, null, new CefSharpSchemeHandlerFactory());
            // browser.RequestContext.RegisterSchemeHandlerFactory("https", "cefsharp.example", new CefSharpSchemeHandlerFactory());

            // 可以在RequestContext上设置首选项，仍然需要在CEF UI线程上调用。
            // Cef.UIThreadTaskFactory.StartNew(delegate
            // {
            //     string errorMessage;
            //     // 使用这个来检查设置首选项是否在您的代码中工作

            //     var success = browser.RequestContext.SetPreference("webkit.webprefs.minimum_font_size", 24, out errorMessage);
            // });             

            // 设置渲染进程消息处理器
            browser.RenderProcessMessageHandler = new RenderProcessMessageHandler();

            // 设置加载错误事件处理器
            browser.LoadError += (sender, args) =>
            {
                // 中止是通常安全忽略的
                // 动作如启动下载将触发中止错误
                // 不需要用户采取任何操作。
                if (args.ErrorCode == CefErrorCode.Aborted)
                {
                    return;
                }

                // 不显示外部协议的错误，我们允许OS处理在OnProtocolExecution()中。
                if (args.ErrorCode == CefErrorCode.UnknownUrlScheme && args.Frame.Url.StartsWith("mailto"))
                {
                    return;
                }

                // 显示加载错误消息。
                var errorHtml = string.Format("<html><body><h2>Failed to load URL {0} with error {1} ({2}).</h2></body></html>",
                                              args.FailedUrl, args.ErrorText, args.ErrorCode);

                _ = args.Browser.SetMainFrameDocumentContentAsync(errorHtml);

                // AddressChanged不是为失败的Url调用的，因此我们需要手动更新Url文本框
                Dispatcher.InvokeAsync(() =>
                {
                    var viewModel = (BrowserTabViewModel)this.DataContext;
                    viewModel.AddressEditable = args.FailedUrl;
                });
            };

            // 注册测试资源
            CefExample.RegisterTestResources(browser);

            // 设置JavaScript消息接收事件处理器
            browser.JavascriptMessageReceived += OnBrowserJavascriptMessageReceived;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // TODO：理想情况下，我们可以直接绑定这个，而不需要使用代码behind
            var viewModel = e.NewValue as BrowserTabViewModel;

            if (viewModel != null)
            {

                browser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = viewModel.LegacyBindingEnabled;
            }
        }

        // 当从浏览器JavaScript收到消息时触发的事件处理方法
        private void OnBrowserJavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
        {
            // 复杂对象最初被表达为IDictionary（实际上是一个ExpandoObject，因此您可以使用dynamic）
            // 检查接收到的消息是否为ExpandoObject类型（一种动态类型，常用于表示复杂且不确定结构的对象）
            if (typeof(System.Dynamic.ExpandoObject).IsAssignableFrom(e.Message.GetType()))
            {

                // 您可以使用dynamic来访问属性或者，您可以使用内置的Model Binder将其转换为自定义模型
                // dynamic msg = e.Message;


                var msg = e.ConvertMessageTo<PostMessageExample>();

                // 检查消息类型是否为"Update"
                if (msg.Type == "Update")
                {
                    // 分别获取回调函数、消息类型和数据属性
                    var callback = msg.Callback;
                    var type = msg.Type; // 这里再次确认类型，实际逻辑中可能不需要这行
                    var property = msg.Data.Property;

                    // 异步执行回调函数，并传递消息类型作为参数
                    callback.ExecuteAsync(type);
                }
            }
            // 如果接收到的消息是一个整数类型
            else if (e.Message is int)
            {
                // 在浏览器环境中异步执行JavaScript函数PostMessageIntTestCallback，并传入接收到的整数值
                e.Frame.ExecuteJavaScriptAsync("PostMessageIntTestCallback(" + (int)e.Message + ")");
            }
        }

        // 更新下载操作的方法，接收一个字符串类型的下载动作和一个DownloadItem对象作为参数
        private void UpdateDownloadAction(string downloadAction, DownloadItem downloadItem)
        {
            // 使用Dispatcher来确保在UI线程上执行更新操作，防止跨线程访问UI元素引发异常
            this.Dispatcher.InvokeAsync(() =>
            {
                // 获取当前控件的数据上下文，假设它是一个BrowserTabViewModel实例
                var viewModel = (BrowserTabViewModel)this.DataContext;
                // 更新LastDownloadAction属性为传入的下载动作字符串
                viewModel.LastDownloadAction = downloadAction;
                // 更新DownloadItem属性为传入的DownloadItem对象
                viewModel.DownloadItem = downloadItem;
            });
        }

        // 浏览器控件鼠标左键按下的事件处理方法
        private void OnBrowserMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 获取鼠标点击位置相对于浏览器控件的坐标点
            var point = e.GetPosition(browser);

            // 判断该坐标点是否在region区域内
            if (region.IsVisible((float)point.X, (float)point.Y))
            {
                // 获取当前控件所在的窗口
                var window = Window.GetWindow(this);
                // 触发窗口的拖动移动行为
                window.DragMove();

                // 标记事件已处理，防止其他处理程序继续处理此事件
                e.Handled = true;
            }
        }

        // 当拖动处理区域发生变化时调用的事件处理方法
        private void OnDragHandlerRegionsChanged(Region region)
        {
            // 检查传入的区域是否非空
            if (region != null)
            {
                // 如果当前类成员变量this.region还未被初始化（即为null）
                // 这通常意味着事件处理器是首次被配置或“连接”（wire up）
                if (this.region == null)
                {
                    // 绑定浏览器的PreviewMouseLeftButtonDown事件到OnBrowserMouseLeftButtonDown方法
                    // 这将在浏览器上鼠标左键按下时触发相应的处理逻辑
                    browser.PreviewMouseLeftButtonDown += OnBrowserMouseLeftButtonDown;
                }

                // 更新当前类成员变量this.region为新的区域
                this.region = region;
            }
        }

        // 当TextBox获得键盘焦点时触发的事件处理程序
        private void OnTextBoxGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // 将事件参数转换为TextBox对象
            var textBox = (TextBox)sender;
            // 选中TextBox中的所有文本
            textBox.SelectAll();
        }

        // 当TextBox捕获鼠标时触发的事件处理程序
        private void OnTextBoxGotMouseCapture(object sender, MouseEventArgs e)
        {
            // 将事件参数转换为TextBox对象
            var textBox = (TextBox)sender;
            // 选中TextBox中的所有文本
            textBox.SelectAll();
        }
    }
}
