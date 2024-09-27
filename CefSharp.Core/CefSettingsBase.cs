//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

//注意：CefSharp.Core 命名空间中的类已对 intellisnse 隐藏，因此用户不会直接使用它们

using CefSharp.Internals;
using System;
using System.Collections.Generic;

namespace CefSharp
{
    /// <summary>
    ///初始化设置。其中许多设置和其他设置也可以使用命令行开关进行配置。
    ///WPF/WinForms/OffScreen 都有自己的 CefSettings 实现来设置
    ///相关设置例如OffScreen 开始时音频静音。
    /// </summary>
    public abstract class CefSettingsBase : IDisposable
    {
        private bool disposed = false;
        internal Core.CefSettingsBase settings = new Core.CefSettingsBase();

#if NETCOREAPP
        public CefSettingsBase() : base()
        {
            if(!System.IO.File.Exists(BrowserSubprocessPath))
            {
                if(Initializer.LibCefLoaded)
                {
                    BrowserSubprocessPath = Initializer.BrowserSubProcessPath;
                }
            }
        }
#endif
        /// <summary>
        ///释放非托管 CefSettingsBase 实例。
        ///一般情况下你不需要调用这个
        ///非托管资源将在调用 <see cref="Cef.Initialize(CefSettingsBase)"/> （或其中一个重载）后被释放。
        /// </summary>
        public void Dispose()
        {
            disposed = true;
            settings = null;
        }

        /// <summary>
        /// 获取一个值，该值指示 CefSettings 是否已被释放。
        /// </summary>
        public bool IsDisposed
        {
            get { return disposed; }
        }

        /// <summary>
        ///将海关方案添加到此集合中。
        /// </summary>
        public IEnumerable<CefCustomScheme> CefCustomSchemes
        {
            get { return settings.CefCustomSchemes; }
        }

        /// <summary>
        /// 将自定义命令行参数添加到此集合中，它们将添加到 OnBeforeCommandLineProcessing 中。这
        ///CefSettings.CommandLineArgsDisabled 值可用于以空命令行对象启动。中指定的任何值
        ///相当于命令行参数的 CefSettings 将在调用此方法之前设置。
        /// </summary>
        public CommandLineArgDictionary CefCommandLineArgs
        {
            get { return settings.CefCommandLineArgs; }
        }

        /// <summary>
        /// 设置为 true 可禁用使用标准 CEF 和 Chromium 命令行参数的浏览器进程功能配置。
        /// 仍可使用 CEF 数据结构或通过添加到 CefCommandLineArgs 来指定配置。
        /// </summary>
        public bool CommandLineArgsDisabled
        {
            get { return settings.CommandLineArgsDisabled; }
            set { settings.CommandLineArgsDisabled = value; }
        }

        /// <summary>
        ///设置为 true 可通过控制浏览器进程主（UI）线程消息泵调度
        ///IBrowserProcessHandler.OnScheduleMessagePumpWork 回调。建议将此选项与
        ///Cef.DoMessageLoopWork() 函数，用于 CEF 消息循环必须集成到现有应用程序消息中的情况
        ///循环（请参阅 Cef.DoMessageLoopWork 上的其他注释和警告）。不建议大多数用户启用此选项；
        ///禁用此选项并使用 MultiThreadedMessageLoop （默认）（如果可能）。
        /// </summary>
        public bool ExternalMessagePump
        {
            get { return settings.ExternalMessagePump; }
            set { settings.ExternalMessagePump = value; }
        }

        /// <summary>
        ///设置为 true 以使浏览器进程消息循环在单独的线程中运行。如果为 false，则 CefDoMessageLoopWork()
        ///函数必须从您的应用程序消息循环中调用。此选项仅在 Windows 上受支持。默认值为
        ///真的。
        /// </summary>
        public bool MultiThreadedMessageLoop
        {
            get { return settings.MultiThreadedMessageLoop; }
            set { settings.MultiThreadedMessageLoop = value; }
        }

        /// <summary>
        ///将为子进程启动的单独可执行文件的路径。默认情况下使用浏览器进程可执行文件。
        ///有关详细信息，请参阅 Cef.ExecuteProcess() 上的注释。如果该值非空，则它必须是绝对路径。
        ///也可以使用“browser-subprocess-path”命令行开关进行配置。
        ///默认使用提供的 CefSharp.BrowserSubprocess.exe 实例
        /// </summary>
        public string BrowserSubprocessPath
        {
            get { return settings.BrowserSubprocessPath; }
            set { settings.BrowserSubprocessPath = value; }
        }

        /// <summary>
        ///全局浏览器缓存数据在磁盘上的存储位置。如果这个值非空，那么它一定是
        ///绝对路径，必须等于 CefSettings.RootCachePath 或者是 CefSettings.RootCachePath 的子目录（如果 RootCachePath 是
        ///为空则默认为该值）。如果该值为空，则浏览器将以“隐身模式”创建，其中
        ///内存缓存用于存储，不会将数据持久保存到磁盘。 HTML5 数据库（例如 localStorage）仅
        ///如果指定了缓存路径，则跨会话持久化。可以通过以下方式覆盖单个 RequestContext 实例
        ///RequestContextSettings.CachePath 值。
        /// </summary>
        public string CachePath
        {
            get { return settings.CachePath; }
            set { settings.CachePath = value; }
        }

        /// <summary>
        ///所有 CefSettings.CachePath 和 RequestContextSettings.CachePath 值必须具有的共同根目录。如果这
        ///值为空且 CefSettings.CachePath 非空则默认为 CefSettings.CachePath 值。
        ///如果该值非空，则它必须是绝对路径。  未能正确设置该值可能会导致沙箱
        ///阻止对 CachePath 目录的读/写访问。注意：CefSharp 不实现 CHROMIUM SANDBOX。一个非空
        ///RootCachePath 可以在您想要浏览器的情况下与空的 CefSettings.CachePath 结合使用
        ///附加到在“隐身模式”下创建的全局RequestContext（默认）以及使用自定义创建的实例
        ///RequestContext 使用基于磁盘的缓存。
        /// </summary>
        public string RootCachePath
        {
            get { return settings.RootCachePath; }
            set { settings.RootCachePath = value; }
        }

        /// <summary>
        /// 设置为 true 以完全忽略 SSL 证书错误。不建议这样做。
        /// </summary>
        public bool IgnoreCertificateErrors
        {
            get { return settings.CefCommandLineArgs.ContainsKey("ignore-certificate-errors"); }
            set
            {
                if (value)
                {
                    if (!settings.CefCommandLineArgs.ContainsKey("ignore-certificate-errors"))
                    {
                        settings.CefCommandLineArgs.Add("ignore-certificate-errors");
                    }
                }
                else
                {
                    if (settings.CefCommandLineArgs.ContainsKey("ignore-certificate-errors"))
                    {
                        settings.CefCommandLineArgs.Remove("ignore-certificate-errors");
                    }
                }
            }
        }

        /// <summary>
        ///将传递给 WebKit 的区域设置字符串。如果为空，将使用默认区域设置“en-US”。也可配置使用
        ///“lang”命令行开关。
        /// </summary>
        public string Locale
        {
            get { return settings.Locale; }
            set { settings.Locale = value; }
        }

        /// <summary>
        ///语言环境目录的完全限定路径。如果该值为空，则区域设置目录必须位于
        ///模块目录。如果该值非空，则它必须是绝对路径。也可以使用“locales-dir-path”进行配置
        ///命令行开关。
        /// </summary>
        public string LocalesDirPath
        {
            get { return settings.LocalesDirPath; }
            set { settings.LocalesDirPath = value; }
        }

        /// <summary>
        ///资源目录的完全限定路径。如果该值为空，则 cef.pak 和/或 devtools_resources.pak 文件
        ///必须位于模块目录中。还可以使用“resources-dir-path”命令行开关进行配置。
        /// </summary>
        public string ResourcesDirPath
        {
            get { return settings.ResourcesDirPath; }
            set { settings.ResourcesDirPath = value; }
        }

        /// <summary>
        ///用于调试日志的目录和文件名。如果为空，将使用默认日志文件名和位置。在 Windows 上
        ///“debug.log”文件将被写入主可执行目录中。也可以使用“log-file”命令行进行配置
        ///转变。
        /// </summary>
        public string LogFile
        {
            get { return settings.LogFile; }
            set { settings.LogFile = value; }
        }

        /// <summary>
        ///日志严重性。仅记录此严重级别或更高级别的消息。当设置为
        ///<see cref="CefSharp.LogSeverity.Disable"/> 不会将任何消息写入日志文件，但致命消息仍会写入
        ///输出到标准错误。还可以使用“log-severity”命令行开关进行配置，其值为“verbose”、“info”、“warning”、
        ///“错误”、“致命”、“错误报告”或“禁用”。
        /// </summary>
        public CefSharp.LogSeverity LogSeverity
        {
            get { return settings.LogSeverity; }
            set { settings.LogSeverity = value; }
        }

        /// <summary>
        ///初始化 V8 JavaScript 引擎时将使用的自定义标志。使用自定义标志的后果可能不是
        ///经过充分测试。也可以使用“js-flags”命令行开关进行配置。
        /// </summary>
        public string JavascriptFlags
        {
            get { return settings.JavascriptFlags; }
            set { settings.JavascriptFlags = value; }
        }

        /// <summary>
        /// 值，该值将作为默认 User-Agent 字符串的 product 部分插入。如果 Chromium 产品版本为空
        ///将被使用。如果指定了 UserAgent，则将忽略此值。也可使用 “user-agent-product” 命令进行配置 -线路开关。
        /// </summary>
        public string UserAgentProduct
        {
            get { return settings.UserAgentProduct; }
            set { settings.UserAgentProduct = value; }
        }

        /// <summary>
        ///设置为 1024 到 65535 之间的值以在指定端口上启用远程调试。例如，如果指定 8080
        ///远程调试 URL 将为 http://localhost:8080。可以从任何 CEF 或 Chrome 浏览器窗口远程调试 CEF。还
        ///使用“远程调试端口”命令行开关进行配置。
        /// </summary>
        public int RemoteDebuggingPort
        {
            get { return settings.RemoteDebuggingPort; }
            set { settings.RemoteDebuggingPort = value; }
        }

        /// <summary>
        ///为未捕获的异常捕获的堆栈跟踪帧的数量。指定一个正值以启用
        ///CefRenderProcessHandler。 OnUncaughtException() 回调。指定 0（默认值），OnUncaughtException() 将不会
        ///被调用。还可以使用“uncaught-exception-stack-size”命令行开关进行配置。
        /// </summary>
        public int UncaughtExceptionStackSize
        {
            get { return settings.UncaughtExceptionStackSize; }
            set { settings.UncaughtExceptionStackSize = value; }
        }

        /// <summary>
        ///将作为 User-Agent HTTP 标头返回的值。如果为空，将使用默认的用户代理字符串。还
        ///使用“user-agent”命令行开关进行配置。
        /// </summary>
        public string UserAgent
        {
            get { return settings.UserAgent; }
            set { settings.UserAgent = value; }
        }

        /// <summary>
        ///设置为 true (1) 以启用无窗口（离屏）渲染支持。如果应用程序不使用，请勿启用该值
        ///无窗口渲染，因为它可能会降低某些系统上的渲染性能。
        /// </summary>
        public bool WindowlessRenderingEnabled
        {
            get { return settings.WindowlessRenderingEnabled; }
            set { settings.WindowlessRenderingEnabled = value; }
        }

        /// <summary>
        ///使用全局cookie时默认保留会话cookie（没有过期日期或有效期的cookie）
        ///经理将此值设置为 true。会话 cookie 通常是暂时的，大多数 Web 浏览器不会持久存在
        ///他们。还必须指定 CachePath 值才能启用此功能。也可以使用“persist-session-cookies”进行配置
        ///命令行开关。可以通过以下方式覆盖单个 RequestContext 实例
        ///RequestContextSettings.PersistSessionCookies 值。
        /// </summary>
        public bool PersistSessionCookies
        {
            get { return settings.PersistSessionCookies; }
            set { settings.PersistSessionCookies = value; }
        }

        /// <summary>
        /// 逗号分隔的有序语言代码列表，没有任何空格，将在“Accept-Language”HTTP 标头中使用。
        /// 可以使用 CefSettings.AcceptLanguageList 值进行全局设置。如果两个值都为空，则将使用“en-US,en”。
        /// 
        /// </summary>
        public string AcceptLanguageList
        {
            get { return settings.AcceptLanguageList; }
            set { settings.AcceptLanguageList = value; }
        }

        /// <summary>
        ///加载文档之前以及未指定文档颜色时浏览器使用的背景颜色。阿尔法
        ///组件必须是完全不透明 (0xFF) 或完全透明 (0x00)。如果 alpha 分量完全不透明，则 RGB
        ///组件将用作背景颜色。如果 alpha 组件对于 WinForms 浏览器来说是完全透明的，那么
        ///使用不透明白色的默认值。如果 Alpha 组件对于无窗口 (WPF/OffScreen) 浏览器完全透明
        ///然后将启用透明绘画。
        /// </summary>
        public UInt32 BackgroundColor
        {
            get { return settings.BackgroundColor; }
            set { settings.BackgroundColor = value; }
        }

        /// <summary>
        ///关联支持的以逗号分隔的方案列表
        ///ICookieManager。如果 CookieableSchemesExcludeDefaults 为 false
        ///默认方案（“http”、“https”、“ws”和“wss”）也将受到支持。
        ///指定 CookieableSchemesList 值和设置
        ///CookieableSchemesExcludeDefaults 为 true 将禁用所有加载
        ///并为此管理器保存 cookie。可以被覆盖
        ///对于各个 RequestContext 实例，通过
        ///RequestContextSettings.CookieableSchemesList 和
        ///RequestContextSettings.CookieableSchemesExcludeDefaults 值。
        /// </summary>
        public string CookieableSchemesList
        {
            get { return settings.CookieableSchemesList; }
            set { settings.CookieableSchemesList = value; }
        }

        /// <summary>
        ///如果 CookieableSchemesExcludeDefaults 为 false
        ///默认方案（“http”、“https”、“ws”和“wss”）也将受到支持。
        ///指定 CookieableSchemesList 值和设置
        ///CookieableSchemesExcludeDefaults 为 true 将禁用所有加载
        ///并为此管理器保存 cookie。可以被覆盖
        ///对于各个 RequestContext 实例，通过
        ///RequestContextSettings.CookieableSchemesList 和
        ///RequestContextSettings.CookieableSchemesExcludeDefaults 值。
        /// </summary>
        public bool CookieableSchemesExcludeDefaults
        {
            get { return settings.CookieableSchemesExcludeDefaults; }
            set { settings.CookieableSchemesExcludeDefaults = value; }
        }

        /// <summary>
        ///使用提供的设置注册自定义方案。
        ///</摘要>
        ///<param name="scheme">CefCustomScheme，提供有关方案的详细信息。</param>
        public void RegisterScheme(CefCustomScheme scheme)
        {
            settings.RegisterScheme(scheme);
        }

        /// <summary>
        ///设置命令行参数以禁用 GPU 加速。 WebGL 将使用
        ///软件渲染
        /// </summary>
        public void DisableGpuAcceleration()
        {
            if (!settings.CefCommandLineArgs.ContainsKey("disable-gpu"))
            {
                settings.CefCommandLineArgs.Add("disable-gpu");
            }
        }

        /// <summary>
        ///设置命令行参数以启用打印预览参见
        /// https://github.com/chromiumembedded/cef/issues/123/add-support-for-print-preview 了解详情。
        /// </summary>
        public void EnablePrintPreview()
        {
            if (!settings.CefCommandLineArgs.ContainsKey("enable-print-preview"))
            {
                settings.CefCommandLineArgs.Add("enable-print-preview");
            }
        }

        /// <summary>
        ///设置命令行参数以获得最佳 OSR（离屏和 WPF）渲染性能 软件渲染将用于 WebGL，请查看源代码
        ///确定哪些标志最适合您的要求。
        /// </summary>
        public void SetOffScreenRenderingBestPerformanceArgs()
        {
            //使用软件渲染和合成（禁用 GPU）来提高 FPS
            //并降低了 CPU 使用率。
            // 看 https://github.com/chromiumembedded/cef/issues/1257 了解详情。
            if (!settings.CefCommandLineArgs.ContainsKey("disable-gpu"))
            {
                settings.CefCommandLineArgs.Add("disable-gpu");
            }

            if (!settings.CefCommandLineArgs.ContainsKey("disable-gpu-compositing"))
            {
                settings.CefCommandLineArgs.Add("disable-gpu-compositing");
            }

            //同步所有进程之间的帧速率。这导致
            //通过避免生成额外的帧来减少 CPU 使用率
            //否则将被丢弃。可以在浏览器中设置帧率
            //创建时间通过 IBrowserSettings.WindowlessFrameRate 或更改
            //动态使用 IBrowserHost.SetWindowlessFrameRate。在cefclient中
            //可以使用`--off-screen-frame-rate=XX`通过命令行进行设置。
            //有关详细信息，请参阅 https://github.com/chromiumembedded/cef/issues/1368。
            if (!settings.CefCommandLineArgs.ContainsKey("enable-begin-frame-scheduling"))
            {
                settings.CefCommandLineArgs.Add("enable-begin-frame-scheduling");
            }
        }
    }
}
