// 版权所有 © 2020 CefSharp 作者。保留所有权利。
//
// 使用此源代码的许可信息可以在 LICENSE 文件中找到。

// 注意：CefSharp.Core 命名空间中的类已被隐藏，以防止用户直接使用它们。


using System;
using System.Threading.Tasks;

using CefSharp.Internals;

namespace CefSharp
{
    /// <summary>
    /// 全局的CEF方法通过此类暴露。例如，CefInitalize 对应于 Cef.Initialize
    /// CEF API 文档 https://magpcss.org/ceforum/apidocs3/projects/(default)/(_globals).html
    /// 这个类不能被继承。
    /// </summary>
    public static class Cef
    {
        /// <summary>
        /// 当<see cref="Cef.Shutdown"/>方法被调用时，此事件会被触发，
        /// 在关闭逻辑执行之前。
        /// </summary>
        /// <remarks>
        /// 该事件将在与<see cref="Cef.Shutdown"/>方法相同的线程上被调用。
        /// </remarks>
        public static event EventHandler ShutdownStarted;

        public static TaskFactory UIThreadTaskFactory
        {
            get { return Core.Cef.UIThreadTaskFactory; }
        }
        public static TaskFactory IOThreadTaskFactory
        {
            get { return Core.Cef.IOThreadTaskFactory; }
        }
        public static TaskFactory FileThreadTaskFactory
        {
            get { return Core.Cef.FileThreadTaskFactory; }
        }

        public static void AddDisposable(IDisposable item)
        {
            Core.Cef.AddDisposable(item);
        }

        public static void RemoveDisposable(IDisposable item)
        {
            Core.Cef.RemoveDisposable(item);
        }

        /// <summary>获取表示CefSharp是否初始化的值。</summary>
        /// <value>如果CefSharp已初始化则为true；否则为false。</value>

        public static bool IsInitialized
        {
            get { return Core.Cef.IsInitialized; }
        }

        /// <summary>获取表示CefSharp是否初始化的值。</summary>
        /// <value>如果CefSharp已初始化则为true；否则为false。</value>

        public static bool IsShutdown
        {
            get { return Core.Cef.IsShutdown; }
        }

        /// <summary>获取当前使用的CefSharp版本。</summary>
        /// <value>CefSharp版本。</value>
        public static string CefSharpVersion
        {
            get { return Core.Cef.CefSharpVersion; }
        }

        /// <summary>获取当前使用的CEF版本。</summary>
        /// <value>CEF版本。</value>
        public static string CefVersion
        {
            get { return Core.Cef.CefVersion; }
        }


        /// <summary>获取当前使用的Chromium版本。</summary>
        /// <value>Chromium版本。</value>
        public static string ChromiumVersion
        {
            get { return Core.Cef.ChromiumVersion; }
        }

        /// <summary>
        /// 获取当前使用的CEF版本的Git提交哈希值。
        /// </summary>
        /// <value>Git提交哈希值</value>
        public static string CefCommitHash
        {
            get { return Core.Cef.CefCommitHash; }
        }

        /// <summary>
        /// 将指定的url解析为其组成部分。
        /// 使用GURL解析URL。GURL是Google的URL解析库。
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>如果URL为空或无效，则返回null。</returns>
        public static UrlParts ParseUrl(string url)
        {
            return Core.Cef.ParseUrl(url);
        }


        /// <summary>
        /// 使用用户提供的设置初始化 CefSharp。
        /// 需要注意的是，Initialize 和 Shutdown <strong>必须</strong> 在您的主线程（通常是UI线程）上调用。
        /// 如果在不同的线程上调用它们，您的应用程序将会挂起。有关详细信息，请参阅 Cef.Shutdown() 的文档。
        /// </summary>
        /// <param name="settings">CefSharp 的配置设置。</param>
        /// <returns>如果成功则返回 true；否则返回 false。</returns>
        public static bool Initialize(CefSettingsBase settings)
        {
            using (settings)
            {
                return Core.Cef.Initialize(settings.settings);
            }
        }

        /// <summary>
        /// 使用用户提供的设置初始化 CefSharp。
        /// 重要提示：Initialize 和 Shutdown <strong>必须</strong> 在您的主应用程序线程（通常是UI线程）上被调用。
        /// 如果在不同的线程上进行调用，您的应用程序将会挂起。更多详情，请参阅 Cef.Shutdown() 的文档。
        /// </summary>
        /// <param name="settings">CefSharp 的配置设置。</param>
        /// <param name="performDependencyCheck">检查所有相关依赖是否就绪，如果有缺失则抛出异常。</param>
        /// <returns>如果操作成功则返回 true；否则返回 false。</returns>
        public static bool Initialize(CefSettingsBase settings, bool performDependencyCheck)
        {
            using (settings)
            {
                return Core.Cef.Initialize(settings.settings, performDependencyCheck);
            }
        }

        /// <summary
        /// 使用用户提供的设置初始化 CefSharp。
        /// 需要注意的是，Initialize/Shutdown（初始化/关闭）<strong>必须</strong>在主应用程序线程（通常是 UI 线程）上调用。
        /// 应用程序线程（通常是用户界面线程）上调用。如果在不同的
        /// 线程上调用，应用程序将挂起。有关详细信息，请参阅 Cef.Shutdown() 文档。
        /// </summary
        /// <param name="settings">CefSharp 配置设置。
        /// <param name="performDependencyCheck">检查所有相关的依赖项是否可用，如果缺少任何依赖项，则抛出异常</param>。
        /// <param name="browserProcessHandler"> 浏览器进程特定功能的处理程序。如果不想处理这些事件，则为空</param>。
        /// <returns>如果成功则为 true；否则为 false.</returns>
        public static bool Initialize(CefSettingsBase settings, bool performDependencyCheck, IBrowserProcessHandler browserProcessHandler)
        {
            using (settings)
            {
                return Core.Cef.Initialize(settings.settings, performDependencyCheck, browserProcessHandler);
            }
        }

        /// <summary
        /// 使用用户提供的设置初始化 CefSharp。
        /// 需要注意的是，Initialize/Shutdown（初始化/关闭）<strong>必须</strong>在主应用程序线程（通常是 UI 线程）上调用。
        /// 应用程序线程（通常是用户界面线程）上调用。如果在不同的
        /// 线程上调用，应用程序将挂起。有关详细信息，请参阅 Cef.Shutdown() 文档。
        /// </summary
        /// <param name="settings">CefSharp 配置设置。
        /// <param name="performDependencyCheck">检查所有可用的相关依赖项，如果缺少任何依赖项，则抛出异常</param>。
        /// <param name="cefApp">执行此接口以提供处理程序实现。如果不希望处理这些事件，则为空</param>。
        /// <returns>如果成功则为 true；否则为 false.</returns>
        public static bool Initialize(CefSettingsBase settings, bool performDependencyCheck, IApp cefApp)
        {
            using (settings)
            {
                return Core.Cef.Initialize(settings.settings, performDependencyCheck, cefApp);
            }
        }

        /// <summary
        /// 使用用户提供的设置初始化 CefSharp。该方法允许您等待
        /// <see cref="IBrowserProcessHandler.OnContextInitialized"/> 被调用后再继续。
        /// 需要注意的是，Initialize 和 Shutdown <strong>MUST</strong> 必须在主应用程序线程（通常是 UI 界面线程）上调用。
        /// 应用程序线程（通常是用户界面线程）上调用。如果在不同的
        /// 线程上调用，应用程序将挂起。详情请参见 Cef.Shutdown() 文档。
        /// </summary
        /// <param name="settings">CefSharp 配置设置。
        /// <param name="performDependencyCheck">检查所有相关的依赖项是否可用，如果缺少任何依赖项，则抛出异常</param>。
        /// <param name="browserProcessHandler"> 浏览器进程特定功能的处理程序。如果不想处理这些事件，则为空</param>。
        /// <returns>返回一个可等待的任务，成功则为 true，否则为 false。如果为 false，则检查日志文件以查找可能的错误</returns>。
        /// <备注
        /// 如果成功，则在调用 <see cref="IBrowserProcessHandler.OnContextInitialized"/> 时任务将成功完成。
        /// 如果成功，则将在 CEF UI 线程上同步继续。
        /// </remarks
        public static Task<bool> InitializeAsync(CefSettingsBase settings, bool performDependencyCheck = true, IBrowserProcessHandler browserProcessHandler = null)
        {
            using (settings)
            {
                try
                {
                    //忽略结果，Task将被设置在Core.Cef.Initialize中
                    Core.Cef.Initialize(settings.settings, performDependencyCheck, browserProcessHandler);
                }
                catch (Exception ex)
                {
                    GlobalContextInitialized.SetException(ex);
                }
            }

            return GlobalContextInitialized.Task;
        }

        /// <summary>
        ///运行 CEF 消息循环。使用此功能代替应用程序 -
        ///提供消息循环以获得性能和CPU之间的最佳平衡
        ///用法。该函数只能在主应用程序线程上调用，并且
        ///仅当使用 Cef.Initialize() 调用时
        ///CefSettings.MultiThreadedMessageLoop 值为 false。该功能将
        ///阻塞，直到系统收到退出消息。
        /// </summary>
        public static void RunMessageLoop()
        {
            Core.Cef.RunMessageLoop();
        }

        /// <summary>
        ///退出通过调用 Cef.RunMessageLoop() 启动的 CEF 消息循环。
        ///该函数只能在主应用程序线程上调用
        ///如果使用了 Cef.RunMessageLoop()。
        /// </summary>
        public static void QuitMessageLoop()
        {
            Core.Cef.QuitMessageLoop();
        }

        /// <summary>
        ///执行CEF消息循环处理的单次迭代。这个函数是
        ///提供用于必须将 CEF 消息循环集成到
        ///现有应用程序消息循环。不建议使用该功能
        ///对于大多数用户；如果可能的话，使用 CefSettings.MultiThreadedMessageLoop （默认）。
        ///使用此函数时必须注意平衡性能
        ///防止 CPU 使用率过高。建议启用
        ///使用时的CefSettings.ExternalMessagePump选项
        ///这个函数使IBrowserProcessHandler.OnScheduleMessagePumpWork()
        ///回调可以促进调度过程。这个函数应该只是
        ///仅在调用 Cef.Initialize() 时才在主应用程序线程上调用
        ///CefSettings.MultiThreadedMessageLoop 值为 false。这个功能
        ///不会阻塞。
        /// </summary>
        public static void DoMessageLoopWork()
        {
            Core.Cef.DoMessageLoopWork();
        }

        /// <summary>
        ///应从应用程序入口点函数调用此函数来执行辅助进程。
        ///它可用于从浏览器客户端可执行文件运行辅助进程（默认行为）或
        ///来自 CefSettings.browser_subprocess_path 值指定的单独可执行文件。
        ///如果为浏览器进程调用（由无“type”命令行值标识），它将立即返回值 -1。
        ///如果调用一个可识别的辅助进程，它将阻塞，直到该进程退出，然后返回进程退出代码。
        ///|应用程序|参数可能为空。 |windows_sandbox_info|参数仅在 Windows 上使用，并且可以为 NULL（有关详细信息，请参阅 cef_sandbox_win.h）。
        /// </summary>
        public static int ExecuteProcess()
        {
            return Core.Cef.ExecuteProcess();
        }

        ///<summary>添加一个条目到跨域白名单</summary>
        ///<param name="sourceOrigin">目标协议/域允许访问的源。</param>
        ///<param name="targetProtocol">允许访问源源的目标协议。</param>
        ///<param name="targetDomain">允许访问源源的可选目标域。</param>
        ///<param name="allowTargetSubdomains">如果设置为 true 将允许 blah.example.com（如果 
        ///<paramref name="targetDomain"/> 设置为 example.com
        ///</参数>
        ///<returns>无效或无法访问白名单则返回 false </returns>
        ///<备注>
        ///同源策略限制不同来源的脚本托管方式
        ///（方案+域+端口）可以通信。默认情况下，脚本只能访问
        ///同源资源。托管在 HTTP 和 HTTPS 方案上的脚本
        ///（但没有其他方案）可以使用“Access-Control-Allow-Origin”标头
        ///允许跨域请求。例如，https://source.example.com 可以使
        ///XMLHttpRequest 请求 http://target.example.com 如果
        ///http://target.example.com 请求返回“Access-Control-Allow-Origin:
        ///https://source.example.com”响应标头。
        ///
        ///脚本位于单独的框架或 iframe 中，并由同一协议托管
        ///如果两个页面都设置了域名后缀，则可以执行跨源JavaScript
        ///document.domain值改为相同的域后缀。例如，
        ///scheme://foo.example.com 和scheme://bar.example.com 可以使用以下方式进行通信
        ///如果两个域都设置了 document.domain="example.com"，则 JavaScript。
        ///
        ///此方法用于允许访问否则会违反的源
        ///同源策略。托管在完全合格的脚本下
        ///<paramref name="sourceOrigin"/> URL（如http://www.example.com）将被允许访问
        ///托管在指定 <paramref name="targetProtocol"/> 和 <paramref name="targetDomain"/> 上的所有资源。
        ///如果 <paramref name="targetDomain"/> 非空且 <paramref name="allowTargetSubdomains"/> 如果仅 false
        ///将允许精确的域匹配。如果 <paramref name="targetDomain"/> 包含顶部-
        ///级别域组件（如“example.com”）和 <paramref name="allowTargetSubdomains"/> 是
        ///将允许真正的子域匹配。如果 <paramref name="targetDomain"/> 为空并且
        ///<paramref name="allowTargetSubdomains"/> 如果为 true，则所有域和 IP 地址都将是
        ///允许。
        ///
        ///该方法不能用于绕过本地或显示的限制
        ///孤立的方案。有关更多信息，请参阅 <see cref="CefCustomScheme"/> 上的评论
        ///信息。
        ///
        ///该函数可以在任何线程上调用。如果 <paramref name="sourceOrigin"/> 返回 false
        ///无效或无法访问白名单。
        ///</备注>   
        public static bool AddCrossOriginWhitelistEntry(
       string sourceOrigin,
       string targetProtocol,
       string targetDomain,
       bool allowTargetSubdomains)
        {
            return Core.Cef.AddCrossOriginWhitelistEntry(
                sourceOrigin,
                targetProtocol,
                targetDomain,
                allowTargetSubdomains);
        }

        /// <summary>从跨源白名单中删除条目</summary>
        ///<param name="sourceOrigin">目标协议/域允许访问的源。</param>
        ///<param name="targetProtocol">允许访问源源的目标协议。</param>
        ///<param name="targetDomain">允许访问源源的可选目标域。</param>
        ///<param name="allowTargetSubdomains">如果设置为 true 将允许 blah.example.com（如果
        ///<paramref name="targetDomain"/> 设置为 example.com
        ///</参数>
        ///<备注>
        ///从跨域访问白名单中删除一条条目。如果返回 false
        ///<paramref name="sourceOrigin"/> 无效或无法访问白名单。
        /// </remarks>
        public static bool RemoveCrossOriginWhitelistEntry(string sourceOrigin,
            string targetProtocol,
            string targetDomain,
            bool allowTargetSubdomains)

        {
            return Core.Cef.RemoveCrossOriginWhitelistEntry(
                sourceOrigin,
                targetProtocol,
                targetDomain,
                allowTargetSubdomains);
        }

        /// <summary>移除跨域访问白名单中的所有条目。</summary>
        ///<备注>
        ///移除跨域访问白名单中的所有条目。如果返回 false
        ///白名单无法访问。
        /// </remarks>
        public static bool ClearCrossOriginWhitelist()
        {
            return Core.Cef.ClearCrossOriginWhitelist();
        }

        ///<摘要>
        ///返回全局 cookie 管理器。默认情况下，如果指定，数据将存储在 CefSettings.CachePath 中，否则存储在内存中。
        ///使用该方法相当于调用Cef.GetGlobalRequestContext().GetCookieManager()
        ///cookie 管理器存储是以异步方式创建的，而此方法可能返回 cookie 管理器实例，
        ///在获取/写入 cookie 之前可能会有短暂的延迟。
        ///要确保 cookie 管理器已初始化，请使用以下方法之一
        ///-调用 ICompletionCallback.OnComplete 后访问 ICookieManager
        ///-在 IBrowserProcessHandler.OnContextInitialized 中访问 ICookieManager 实例。
        ///-使用 ChromiumWebBrowser BrowserInitialized (OffScreen) 或 IsBrowserInitializedChanged (WinForms/WPF) 事件。
        ///</摘要>
        ///<param name="callback">如果非 NULL，它将在管理器存储初始化后在 CEF UI 线程上异步执行。</param>
        ///<returns>A 全局 cookie 管理器，如果 RequestContext 尚未初始化，则返回 null。</returns>
        public static ICookieManager GetGlobalCookieManager(ICompletionCallback callback = null)
        {
            return Core.Cef.GetGlobalCookieManager(callback);
        }

        ///<摘要>
        ///在调用 Cef.Shutdown 之前调用，这将处理任何剩余的
        ///ChromiumWebBrowser 实例。在 WPF 中，这是从 Dispatcher.ShutdownStarted 使用的
        ///释放 ChromiumWebBrowser 实例持有的非托管资源。
        ///一般来说，你不需要自己调用它。
        ///</摘要>
        public static void PreShutdown()
        {
            Core.Cef.PreShutdown();
        }

        ///<摘要>
        ///关闭 CefSharp 和底层 CEF 基础设施。该方法多次调用是安全的；只会
        ///在第一次调用时关闭 CEF（所有后续调用将被忽略）。
        ///应在应用程序主线程上调用此方法，以在应用程序退出之前关闭 CEF 浏览器进程。
        ///如果您使用 CefSharp.OffScreen，那么您必须在应用程序退出之前显式调用此函数，否则它将挂起。
        ///该方法必须在与Initialize 相同的线程上调用。如果您没有显式调用 Shutdown，则 CefSharp.Wpf 和 CefSharp.WinForms
        ///版本将尽力为您调用 Shutdown，如果您的应用程序在关闭时遇到问题，则显式调用。
        ///</摘要>  
        public static void Shutdown()
        {
            ShutdownStarted?.Invoke(null, EventArgs.Empty);
            ShutdownStarted = null;

            Core.Cef.Shutdown();
        }

        /// <summary>
        ///此方法只应由高级用户使用，如果您不确定，请使用 Cef.Shutdown()。
        ///应在主应用程序线程上调用此函数来关闭
        ///应用程序退出之前的 CEF 浏览器进程。这个方法只是简单的获取一个锁
        ///并调用本机CefShutdown方法，仅检查IsInitialized。所有 ChromiumWeb 浏览器
        ///在调用此方法之前必须释放实例。如果调用此方法导致崩溃
        ///或挂起，那么您可能挂在某些非托管资源上或尚未关闭所有浏览器
        ///实例
        /// </summary>
        public static void ShutdownWithoutChecks()
        {
            ShutdownStarted?.Invoke(null, EventArgs.Empty);
            ShutdownStarted = null;

            Core.Cef.ShutdownWithoutChecks();
        }

        /// <summary>
        /// 清除在全局请求上下文中注册的所有方案处理程序工厂。
        ///出错时返回 false。该函数可以在浏览器进程中的任何线程上调用。
        ///使用此函数相当于调用Cef.GetGlobalRequestContext().ClearSchemeHandlerFactories()。
        ///</摘要>
        ///<returns>出错时返回 false。</returns>
        public static bool ClearSchemeHandlerFactories()
        {
            return Core.Cef.ClearSchemeHandlerFactories();
        }

        /// <summary>
        ///如果在指定的 CEF 线程上调用，则返回 true。
        /// </summary>
        /// <returns>如果在指定线程上调用则返回 true.</returns>
        public static bool CurrentlyOnThread(CefThreadIds threadId)
        {
            return Core.Cef.CurrentlyOnThread(threadId);
        }

        /// <summary>
        /// 获取全局请求上下文。确保完成后丢弃该对象。
        ///较早可能访问 IRequestContext 的位置是 IBrowserProcessHandler.OnContextInitialized。
        ///替代使用 ChromiumWebBrowser BrowserInitialized (OffScreen) 或 IsBrowserInitializedChanged (WinForms/WPF) 事件。
        /// </summary>
        /// <returns>返回全局请求上下文，如果 RequestContext 尚未初始化，则返回 null.</returns>
        public static IRequestContext GetGlobalRequestContext()
        {
            return Core.Cef.GetGlobalRequestContext();
        }

        /// <summary>
        ///辅助函数（CefColorSetARGB 宏的包装）结合了
        ///将 4 个颜色分量转换为 uint32 以便与 BackgroundColor 属性一起使用
        /// </summary>
        /// <param name="a">Alpha</param>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        /// <returns>返回颜色.</returns>
        public static UInt32 ColorSetARGB(UInt32 a, UInt32 r, UInt32 g, UInt32 b)
        {
            return Core.Cef.ColorSetARGB(a, r, g, b);
        }

        /// <summary>
        /// 崩溃报告是使用名为的 INI 样式配置文件配置的
        ///crash_reporter.cfg。该文件必须放置在
        ///主应用程序可执行文件。文件内容如下：
        ///
        /// # 注释以井号字符开头，并且必须独占一行。
        ///
        ///[配置]
        ///ProductName=<“prod”崩溃键的值；默认为“cef”>
        ///ProductVersion=<“ver”崩溃键的值；默认为CEF版本>
        ///AppName=<仅限Windows；用于存储崩溃的应用程序特定文件夹名称组件
        ///          信息;默认为“CEF”>
        ///ExternalHandler=<仅限Windows；要使用的外部处理程序 exe 的名称
        ///而不是重新启动主exe；默认为空>
        ///ServerURL=<崩溃服务器URL;默认为空>
        ///RateLimitEnabled=<如果上传应限制速率则为 True；默认为 true>
        ///MaxUploadsPerDay=<每 24 小时最大上传次数，如果启用速率限制则使用；
        ///默认为5>
        ///MaxDatabaseSizeInMb=<崩溃报告磁盘使用总量大于此值
        ///将导致旧报告被删除；默认为20>
        ///MaxDatabaseAgeInDays=<早于此值的崩溃报告将被删除；
        ///默认为5>
        ///
        ///[崩溃键]
        ///my_key1=<小|中|大>
        ///my_key2=<小|中|大>
        ///
        ///配置部分：
        ///
        ///如果设置了“ProductName”和/或“ProductVersion”，则指定的值
        ///将包含在故障转储元数据中。 
        ///
        ///如果在 Windows 上设置了“AppName”，则崩溃报告信息（指标、
        ///数据库和转储）将存储在本地磁盘上
        ///“C:\Users\[CurrentUser]\AppData\Local\[AppName]\User Data”文件夹。 
        ///
        ///如果在 Windows 上设置了“ExternalHandler”，则指定的 exe 将被
        ///作为 crashpad-handler 启动而不是重新启动主进程
        ///EXE文件。该值可以是绝对路径或相对于主exe的路径
        ///目录。 
        ///
        ///如果设置了“ServerURL”，则崩溃将作为多部分 POST 上传
        ///请求到指定的 URL。否则，报告将仅存储在本地
        ///在磁盘上。
        ///
        ///如果“RateLimitEnabled”设置为 true，则崩溃报告上传将按速率进行
        ///限制如下：
        ///1. 如果“MaxUploadsPerDay”设置为正值，则最多
        ///每 24 小时内将上传指定数量的崩溃。
        ///2. 如果由于网络或服务器错误导致崩溃上传失败，则
        ///将应用最多 24 小时的增量退避延迟
        ///重试。
        ///3. 如果应用了退避延迟并且“MaxUploadsPerDay”> 1，则
        ///“MaxUploadsPerDay”值将减少到1，直到客户端
        ///重新启动。这有助于避免当网络或
        ///服务器错误已解决。
        ///
        ///如果“MaxDatabaseSizeInMb”设置为正值，则崩溃报告存储
        ///磁盘上的大小将限制为该大小（以兆字节为单位）。例如，在 Windows 上
        ///每个转储大约为 600KB，因此“MaxDatabaseSizeInMb”值为 20 相当于
        ///磁盘上存储了大约 34 个崩溃报告。
        ///
        ///如果“MaxDatabaseAgeInDays”设置为正值，则崩溃报告较旧
        ///超过指定年龄（以天为单位）的将被删除。
        ///
        ///CrashKeys 部分：
        ///
        ///可以指定任意数量的崩溃键以供应用程序使用。碰撞
        ///键值将根据指定的大小被截断（小 = 63 字节，
        ///中 = 252 字节，大 = 1008 字节）。崩溃键的值可以设置
        ///从任何线程或进程使用 Cef.SetCrashKeyValue 函数。这些
        ///键/值对将与故障转储一起发送到故障服务器
        ///文件。中值和大值将被分块提交。例如，
        ///如果你的键名为“mykey”，那么该值将被分解为有序的
        ///块并使用名为“mykey-1”、“mykey-2”等的键提交。
        /// </summary>
        /// <returns>如果启用了崩溃报告，则返回 true.</returns>
        public static bool CrashReportingEnabled
        {
            get { return Core.Cef.CrashReportingEnabled; }
        }

        /// <summary>
        /// 从崩溃元数据中设置或清除特定的键值对。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public static void SetCrashKeyValue(string key, string value)
        {
            Core.Cef.SetCrashKeyValue(key, value);
        }

        /// <summary>
        /// 获取当前日志级别。
        ///当 <see cref="CefSettingsBase.LogSeverity"/> 设置为 <see cref="LogSeverity.Disable"/> 时
        ///不会将任何消息写入日志文件，但 FATAL 消息仍会输出到 stderr。
        ///当禁用日志记录时，此方法将返回 <see cref="LogSeverity.Fatal"/>。
        /// </summary>
        /// <returns>当前日志级别</returns>
        public static LogSeverity GetMinLogLevel()
        {
            var severity = Core.Cef.GetMinLogLevel();

            //手动将int转成enum
            //值不匹配（这是 CEF/Chromium 中的差异）实现
            //我们需要手动处理它，
            //https://github.com/chromiumembedded/cef/blob/2a64387259cf14412e24c3267c8a1eb3b99a54e3/include/base/cef_logging.h#L186
            //const LogSeverity LOG_VERBOSE = -1;
            //const LogSeverity LOG_INFO = 0;
            //const LogSeverity LOG_WARNING = 1;
            //const LogSeverity LOG_ERROR = 2;
            //const LogSeverity LOG_FATAL = 3;

            if (severity == -1)
            {
                return LogSeverity.Verbose;
            }

            if (severity == 0)
            {
                return LogSeverity.Info;
            }

            if (severity == 1)
            {
                return LogSeverity.Warning;
            }

            if (severity == 2)
            {
                return LogSeverity.Error;
            }

            if (severity == 3)
            {
                return LogSeverity.Fatal;
            }

            //没有匹配的类型，以枚举形式返回整数值
            return (LogSeverity)severity;
        }

        /// <summary>
        ///返回指定文件扩展名的 mime 类型，如果未知，则返回空字符串。
        /// </summary>
        /// <param name="extension">file extension</param>
        /// <returns>返回指定文件扩展名的 mime 类型，如果未知则返回空字符串.</returns>
        public static string GetMimeType(string extension)
        {
            return Core.Cef.GetMimeType(extension);
        }

        /// <summary>
        ///WaitForBrowsersToClose 默认不启用，调用此方法
        ///在 Cef.Initialize 之前启用。如果你没有调用 Cef.Initialize
        ///明确地那么应该在创建第一个之前调用它
        ///ChromiumWebBrowser 实例。
        /// </summary>
        public static void EnableWaitForBrowsersToClose()
        {
            Core.Cef.EnableWaitForBrowsersToClose();
        }

        /// <summary>
        ///确保所有 ChromiumWebBrowser 实例已被调用的 Helper 方法
        ///已关闭/已处置，应在 Cef.Shutdown 之前调用。
        ///处置所有剩余的 ChromiumWebBrowser 实例
        ///然后等待 CEF 释放其剩余的 CefBrowser 实例。
        ///最后有 50 毫秒的小延迟，以允许 CEF 完成其清理。
        ///仅应在 MultiThreadedMessageLoop = true 时调用；
        ///（CEF 集成到主消息循环时尚未测试）。
        ///</摘要>
        public static void WaitForBrowsersToClose()
        {
            Core.Cef.WaitForBrowsersToClose();
        }


        /// <summary>
        /// 确保所有 ChromiumWebBrowser 实例已被调用的辅助方法
        ///已关闭/已处置，应在 Cef.Shutdown 之前调用。
        ///处置所有剩余的 ChromiumWebBrowser 实例
        ///然后等待 CEF 释放其剩余的 CefBrowser 实例。
        ///最后有 50 毫秒的小延迟，以允许 CEF 完成其清理。
        ///仅应在 MultiThreadedMessageLoop = true 时调用；
        ///（CEF 集成到主消息循环时尚未测试）。
        /// </summary>
        /// <param name="timeoutInMiliseconds">超时时间（以毫秒为单位）.</param>
        public static void WaitForBrowsersToClose(int timeoutInMiliseconds)
        {
            Core.Cef.WaitForBrowsersToClose(timeoutInMiliseconds);
        }

        /// <summary>
        /// 在指定线程上发布延迟执行的操作。
        ///</摘要>
        ///<param name="threadId">线程 ID</param>
        ///<param name="action">要执行的操作</param>
        ///<param name="delayInMs">延迟毫秒</param>
        ///<返回>布尔</返回>
        public static bool PostDelayedAction(CefThreadIds threadId, Action action, int delayInMs)
        {
            return Core.Cef.PostDelayedAction(threadId, action, delayInMs);
        }

        /// <summary>
        ///发布一个要在指定线程上执行的操作。
        ///</摘要>
        ///<param name="threadId">线程 ID</param>
        ///<param name="action">要执行的操作</param>
        ///<返回>布尔</返回>
        public static bool PostAction(CefThreadIds threadId, Action action)
        {
            return Core.Cef.PostAction(threadId, action);
        }

        /// <summary>
        ///指示当前操作系统版本是否匹配或高于 Windows 10 版本。
        ///未针对 Windows 10 表现的应用程序会返回 false，即使当前操作系统版本是 Windows 10。
        ///要清单 Windows 10 的应用程序，请参阅 https://learn.microsoft.com/en-us/windows/win32/sysinfo/targeting-your-application-at-windows-8-1。
        ///</摘要>
        ///<returns>如果当前操作系统版本匹配或大于 Windows 10 版本，则为 True；否则为 false。</returns>
        public static bool IsWindows10OrGreater()
        {
            return Core.Cef.IsWindows10OrGreaterEx();
        }

        /// <summary>
        ///如果当前操作系统版本匹配或高于 Windows 10 版本，则此方法不执行任何操作。
        ///未针对 Windows 10 显示的应用程序将抛出 <see cref="ApplicationException"/>，即使当前操作系统版本是 Windows 10。
        ///要清单 Windows 10 的应用程序，请参阅 https://learn.microsoft.com/en-us/windows/win32/sysinfo/targeting-your-application-at-windows-8-1。
        ///</摘要>
        ///<异常 cref="ApplicationException"></异常>
        public static void AssertIsWindows10OrGreater()
        {
            if (!IsWindows10OrGreater())
                throw new ApplicationException("当前操作系统版本低于 Windows 10。未针对 Windows 10 清单的应用程序会引发此异常，即使当前操作系统版本是 Windows 10。要针对 Windows 10 清单应用程序，请参阅 https://learn.microsoft.com/en-us/windows/win32/sysinfo/targeting-your-application-at-windows-8-1.");
        }
    }
}
