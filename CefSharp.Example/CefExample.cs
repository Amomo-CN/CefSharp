//版权所有 © 2014 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using CefSharp.Example.Proxy;
using CefSharp.SchemeHandler;

namespace CefSharp.Example
{
    public static class CefExample
    {
        //TODO:在 https://github.com/chromiumembedded/cef/issues/2685 之后恢复
        //已修复。
        public const string ExampleDomain = "cefsharp.example";
        public const string BaseUrl = "https://" + ExampleDomain;
        public const string DefaultUrl = BaseUrl + "/home.html";//滴滴 打开主页
#if NETCOREAPP
        public const string BindingTestUrl = BaseUrl + "/BindingTestNetCore.html";
#else
        public const string BindingTestUrl = BaseUrl + "/BindingTest.html";
#endif
        public const string BindingTestNetCoreUrl = BaseUrl + "/BindingTestNetCore.html";
        public const string BindingTestSingleUrl = BaseUrl + "/BindingTestSingle.html";
        public const string BindingTestsAsyncTaskUrl = BaseUrl + "/BindingTestsAsyncTask.html";
        public const string LegacyBindingTestUrl = BaseUrl + "/LegacyBindingTest.html";
        public const string PostMessageTestUrl = BaseUrl + "/PostMessageTest.html";
        public const string PopupTestUrl = BaseUrl + "/PopupTest.html";
        public const string TooltipTestUrl = BaseUrl + "/TooltipTest.html";
        public const string BasicSchemeTestUrl = BaseUrl + "/SchemeTest.html";
        public const string ResponseFilterTestUrl = BaseUrl + "/ResponseFilterTest.html";
        public const string DraggableRegionTestUrl = BaseUrl + "/DraggableRegionTest.html";
        public const string DragDropCursorsTestUrl = BaseUrl + "/DragDropCursorsTest.html";
        public const string CssAnimationTestUrl = BaseUrl + "/CssAnimationTest.html";
        public const string CdmSupportTestUrl = BaseUrl + "/CdmSupportTest.html";
        public const string HelloWorldUrl = BaseUrl + "/HelloWorld.html";
        public const string BindingApiCustomObjectNameTestUrl = BaseUrl + "/BindingApiCustomObjectNameTest.html";
        public const string TestResourceUrl = "http://test/resource/load";
        public const string RenderProcessCrashedUrl = "http://processcrashed";
        public const string TestUnicodeResourceUrl = "http://test/resource/loadUnicode";
        public const string PopupParentUrl = "http://www.w3schools.com/jsref/tryit.asp?filename=tryjsref_win_close";
        public const string ChromeInternalUrls = "chrome://chrome-urls";
        public const string ChromeNetInternalUrls = "chrome://net-internals";
        public const string ChromeProcessInternalUrls = "chrome://process-internals";

        // 在调试实际的子流程时使用，以使该项目内的断点等工作。  
        private static readonly bool DebuggingSubProcess = Debugger.IsAttached;

        public static void Init(CefSettingsBase settings, IBrowserProcessHandler browserProcessHandler)
        {
            // 设置 Google API 密钥，用于无 GPS 的地理定位请求。  看 http://www.chromium.org/developers/how-tos/api-keys
            // Environment.SetEnvironmentVariable("GOOGLE_API_KEY", "");
            // Environment.SetEnvironmentVariable("GOOGLE_DEFAULT_CLIENT_ID", "");
            // Environment.SetEnvironmentVariable("GOOGLE_DEFAULT_CLIENT_SECRET", "");

            //Chromium 命令行参数
            //http://peter.sh/experiments/chromium-command-line-switches/
            //NOTE:并非全部与“CefSharp”相关，仅供参考。
            //CEF特定命令行参数
            //https://bitbucket.org/chromiumembedded/cef/src/master/libcef/common/cef_switches.cc?fileviewer=file-view-default

            //**重要**：对于启用/禁用的命令行参数，例如disable-gpu，指定值“0”，例如
            //settings.CefCommandLineArgs.Add("disable-gpu", "0");不会有任何效果，因为第二个参数被忽略。

            //**重要**：使用命令行参数时，如果您遇到以下情况，Chromium 版本之间的行为可能会发生变化
            //升级到新版本后出现的问题，您应该删除所有自定义命令行参数并添加测试它们
            //隔离以确定它们是否引起问题。
            //此处显示的命令行参数仅供参考，未经测试以确认它们适用于每个版本

            settings.RemoteDebuggingPort = 8088;
            //缓存数据在磁盘上的存储位置。如果为空，内存缓存将用于某些功能，临时磁盘缓存将用于其他功能。
            //如果指定了缓存路径，HTML5 数据库（例如 localStorage）只会跨会话持久保存。
            settings.RootCachePath = Path.GetFullPath("cache");
            //如果非空，则 CachePath 必须等于 RootCachePath 或者是 RootCachePath 的子级
            //我们正在使用一个子文件夹。
            //
            settings.CachePath = Path.GetFullPath("cache\\global");
            //settings.UserAgent = "CefSharp Browser" + Cef.CefSharpVersion; // 用户代理示例
            //settings.CefCommandLineArgs.Add("renderer-startup-dialog");
            //settings.CefCommandLineArgs.Add("disable-site-isolation-trials");
            //settings.CefCommandLineArgs.Add("enable-media-stream"); //启用 WebRTC
            //settings.CefCommandLineArgs.Add("no-proxy-server"); //不要使用代理服务器，始终进行直接连接。覆盖传递的任何其他代理服务器标志。
            //settings.CefCommandLineArgs.Add("allow-running-insecure-content"); //默认情况下，https 页面无法从 http URL 运行 JavaScript 或 CSS。这提供了一个覆盖来获取旧的不安全行为。仅适用于 47 及以上。
            //https://peter.sh/experiments/chromium-command-line-switches/#disable-site-isolation-trials
            //settings.CefCommandLineArgs.Add("disable-site-isolation-trials");
            //NOTE: 在进程中运行网络服务不是 CEF 官方支持的
            //它可能适用于新版本，也可能不适用于新版本。
            //settings.CefCommandLineArgs.Add("enable-features", "CastMediaRouteProvider,NetworkServiceInProcess");

            //settings.CefCommandLineArgs.Add("enable-logging"); //为渲染器进程启用日志记录（将使用 cmd 提示符打开并输出调试消息 -与设置 LogSeverity = LogSeverity.Verbose 结合使用；）
            //settings.LogSeverity = LogSeverity.Verbose; // 需要启用日志记录来输出消息

            //settings.CefCommandLineArgs.Add("disable-extensions"); //可以禁用扩展支持
            //settings.CefCommandLineArgs.Add("disable-pdf-extension"); //PDF 扩展名可以专门禁用

            //音频播放示例
            //settings.CefCommandLineArgs["autoplay-policy"] = "no-user-gesture-required";

            //NOTE: 为了获得 OSR 最佳性能，您应该在禁用 GPU 的情况下运行：
            // `--disable-gpu --disable-gpu-compositing --enable-begin-frame-scheduling`
            // （您将失去 WebGL 支持，但会提高 FPS 并减少 CPU 使用率）。
            // http://magpcss.org/ceforum/viewtopic.php?f=6&t=13271#p27075
            //https://bitbucket.org/chromiumembedded/cef/commits/e3c1d8632eb43c1c2793d71639f3f5695696a5e8

            //NOTE: 以下函数将设置所有三个参数
            //settings.SetOffScreenRenderingBestPerformanceArgs();
            //settings.CefCommandLineArgs.Add("disable-gpu");
            //settings.CefCommandLineArgs.Add("disable-gpu-compositing");
            //settings.CefCommandLineArgs.Add("enable-begin-frame-scheduling");

            //settings.CefCommandLineArgs.Add("disable-gpu-vsync"); //禁用垂直同步

            // 以下选项控制所有框架的可访问性状态。
            //仅当 IBrowserHost.SetAccessibilityState 调用未设置可访问性状态时，这些选项才会生效。
            // --force-renderer-accessibility enables browser accessibility.
            // --disable-renderer-accessibility completely disables browser accessibility.
            //settings.CefCommandLineArgs.Add("force-renderer-accessibility");
            //settings.CefCommandLineArgs.Add("disable-renderer-accessibility");

            //启用未捕获的异常处理程序
            settings.UncaughtExceptionStackSize = 10;

            //禁用 WebAssembly
            //settings.JavascriptFlags = "--noexpose_wasm";

            //离屏渲染（WPF/离屏）
            if (settings.WindowlessRenderingEnabled)
            {
                //禁用直接组合进行测试 https://github.com/cefsharp/CefSharp/issues/1634
                //settings.CefCommandLineArgs.Add("disable-direct-composition");

                // 启用此功能后，DevTools 似乎无法工作
                // http://magpcss.org/ceforum/viewtopic.php?f=6&t=14095
                //settings.CefCommandLineArgs.Add("enable-begin-frame-scheduling");
            }

            var proxy = ProxyConfig.GetProxyInformation();
            switch (proxy.AccessType)
            {
                case InternetOpenType.Direct:
                    {
                        //不要使用代理服务器，始终进行直接连接。
                        settings.CefCommandLineArgs.Add("no-proxy-server");
                        break;
                    }
                case InternetOpenType.Proxy:
                    {
                        settings.CefCommandLineArgs.Add("proxy-server", proxy.ProxyAddress);
                        break;
                    }
                case InternetOpenType.PreConfig:
                    {
                        settings.CefCommandLineArgs.Add("proxy-auto-detect");
                        break;
                    }
            }

            //settings.LogSeverity = LogSeverity.Verbose;

            //实验设置见 https://github.com/chromiumembedded/cef/issues/2969
            //欲了解详情
            //settings.ChromeRuntime = true;

            if (DebuggingSubProcess)
            {
                var architecture = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
#if NETCOREAPP
                settings.BrowserSubprocessPath = Path.GetFullPath("..\\..\\..\\..\\..\\..\\CefSharp.BrowserSubprocess\\bin.netcore\\" + architecture + "\\Debug\\netcoreapp3.1\\CefSharp.BrowserSubprocess.exe");
#else
                settings.BrowserSubprocessPath = Path.GetFullPath("..\\..\\..\\..\\CefSharp.BrowserSubprocess\\bin\\" + architecture + "\\Debug\\CefSharp.BrowserSubprocess.exe");
#endif
            }

            settings.CookieableSchemesList = "custom";

            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = CefSharpSchemeHandlerFactory.SchemeName,
                SchemeHandlerFactory = new CefSharpSchemeHandlerFactory(),
                IsSecure = true, //使用与“https”URL 相同的安全规则进行处理
            });

            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = "https",
                SchemeHandlerFactory = new CefSharpSchemeHandlerFactory(),
                DomainName = ExampleDomain
            });

            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = CefSharpSchemeHandlerFactory.SchemeNameTest,
                SchemeHandlerFactory = new CefSharpSchemeHandlerFactory(),
                IsSecure = true //使用与“https”URL 相同的安全规则进行处理
            });

            //您可以使用 http/https 方案 -最好注册特定域
            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = "https",
                SchemeHandlerFactory = new CefSharpSchemeHandlerFactory(),
                DomainName = "cefsharp.com",
                IsSecure = true //使用与“https”URL 相同的安全规则进行处理
            });

            const string cefSharpExampleResourcesFolder =
#if !NETCOREAPP
                @"..\..\..\..\CefSharp.Example\Resources";
#else
                @"..\..\..\..\..\..\CefSharp.Example\Resources";
#endif

            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = "localfolder",
                SchemeHandlerFactory = new FolderSchemeHandlerFactory(rootFolder: cefSharpExampleResourcesFolder,
                                                                    schemeName: "localfolder", //可选参数无方案名称检查是否为空
                                                                    hostName: "cefsharp", //可选参数无主机名检查是否为空
                                                                    defaultPage: "home.html") //可选参数默认为index.html
            });

            //必须在调用 Cef.Initialized 之前设置
            CefSharpSettings.FocusedNodeChangedEnabled = true;

            //异步 Javascript 绑定 -方法在 TaskScheduler.Default 上排队。
            //当您有返回 Task<T> 的方法时，将其设置为 true
            //CefSharpSettings.ConcurrentTaskExecution = true;

            //旧版绑定行为 -与版本 57 及更低版本中的 Javascript 绑定相同
            //有关详细信息，请参阅问题 https://github.com/cefsharp/CefSharp/issues/1203
            //CefSharpSettings.LegacyJavascriptBindingEnabled = true;

            //如果父进程关闭则退出子进程
            //目前这是可选的
            //https://github.com/cefsharp/CefSharp/pull/2375/
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

            //NOTE: 在调用 Cef.Initialize 之前设置此项以使用用户名和密码指定代理
            //一组不能在运行时更改。如果您需要在运行时（动态）更改代理，那么
            //看 https://github.com/cefsharp/CefSharp/wiki/General-Usage#proxy-resolution
            //CefSharpSettings.Proxy = new ProxyOptions(ip: "127.0.0.1", port: "8080", username: "cefsharp", password: "123");

            bool performDependencyCheck = !DebuggingSubProcess;

            if (!Cef.Initialize(settings, performDependencyCheck: performDependencyCheck, browserProcessHandler: browserProcessHandler))
            {
                throw new Exception("无法初始化Cef");
            }

            Cef.AddCrossOriginWhitelistEntry(BaseUrl, "https", "cefsharp.com", false);
        }

        public static void RegisterTestResources(IWebBrowser browser)
        {
            if (browser.ResourceRequestHandlerFactory == null)
            {
                browser.ResourceRequestHandlerFactory = new ResourceRequestHandlerFactory();
            }

            var handler = browser.ResourceRequestHandlerFactory as ResourceRequestHandlerFactory;

            if (handler != null)
            {
                const string renderProcessCrashedBody = "<html><body><h1>渲染进程崩溃</h1><p>您看到此消息是因为渲染进程已崩溃</p></body></html>";
                handler.RegisterHandler(RenderProcessCrashedUrl, ResourceHandler.GetByteArray(renderProcessCrashedBody, Encoding.UTF8));

                const string responseBody = "<html><body><h1>成功</h1><p>该文档是从 System.IO.Stream 加载的</p></body></html>";
                handler.RegisterHandler(TestResourceUrl, ResourceHandler.GetByteArray(responseBody, Encoding.UTF8));

                const string unicodeResponseBody = "<html><body>整体满意度</body></html>";
                handler.RegisterHandler(TestUnicodeResourceUrl, ResourceHandler.GetByteArray(unicodeResponseBody, Encoding.UTF8));
            }
        }
    }
}
