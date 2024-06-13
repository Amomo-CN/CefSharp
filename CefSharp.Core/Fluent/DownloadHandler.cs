//版权所有 © 2021 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System.IO;

namespace CefSharp.Fluent
{
    /// <summary>
    ///在下载开始之前调用以响应用户启动的操作
    ///（例如 alt + 链接单击或返回 `Content-Disposition: 的链接单击：
    ///来自服务器的附件`响应）。
    ///</摘要>
    ///<param name="chromiumWebBrowser">ChromiumWebBrowser 控件</param>
    ///<param name="browser">浏览器实例</param>
    ///<param name="url">为目标下载网址</param>
    ///<param name="requestMethod">是目标方法（GET、POST等）</param>
    ///<returns>返回 true 继续下载，返回 false 取消下载。</returns>
    public delegate bool CanDownloadDelegate(IWebBrowser chromiumWebBrowser, IBrowser browser, string url, string requestMethod);

    /// <summary>
    ///下载开始之前调用。
    ///</摘要>
    ///<param name="chromiumWebBrowser">ChromiumWebBrowser 控件</param>
    ///<param name="browser">浏览器实例</param>
    ///<param name="downloadItem">代表正在下载的文件</param>
    ///<param name="callback">用于异步继续下载的回调接口</param>
    ///<returns>返回 true 并执行 <paramref name="callback"/>
    ///异步或在此方法中继续或取消下载。
    ///返回 false 继续默认处理（使用 Alloy 样式取消，
    ///下载 Chrome 风格的架子）。</returns>
    public delegate bool OnBeforeDownloadDelegate(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback);

    /// <summary>
    ///当下载的状态或进度信息已更新时调用。这可能在 <see cref="IDownloadHandler.OnBeforeDownload"/> 之前和之后调用多次。
    ///</摘要>
    ///<param name="chromiumWebBrowser">ChromiumWebBrowser 控件</param>
    ///<param name="browser">浏览器实例</param>
    ///<param name="downloadItem">代表正在下载的文件</param>
    ///<param name="callback">用于取消/暂停/恢复进程的回调</param>
    public delegate void OnDownloadUpdatedDelegate(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback);

    /// <summary>
    ///<see cref="DownloadHandlerBuilder"/> 使用的 <see cref="IDownloadHandler"/> 实现
    ///提供创建 <see cref="IDownloadHandler"/> 的流畅方法。
    ///</摘要>
    public class DownloadHandler : Handler.DownloadHandler
    {
        private CanDownloadDelegate canDownload;
        private OnBeforeDownloadDelegate onBeforeDownload;
        private OnDownloadUpdatedDelegate onDownloadUpdated;

        ///<摘要>
        ///创建一个新的 DownloadHandler 构建器
        ///</摘要>
        ///<returns>Fluent DownloadHandler Builder</returns>
        public static DownloadHandlerBuilder Create()
        {
            return new DownloadHandlerBuilder();
        }

        /// <summary>
        ///创建一个新的 <see cref="IDownloadHandler"/> 实例
        ///其中所有下载都会自动下载到指定文件夹。
        ///不向用户显示任何对话框。
        ///</摘要>
        ///<param name="folder">下载文件的文件夹。</param>
        ///<param name="downloadUpdated">用于下载更新、跟踪进度、完成等的可选委托</param>
        ///<returns><see cref="IDownloadHandler"/> 实例。</returns>     
        public static IDownloadHandler UseFolder(string folder, OnDownloadUpdatedDelegate downloadUpdated = null)
        {
            return Create()
                .OnBeforeDownload((chromiumWebBrowser, browser, item, callback) =>
                {
                    using (callback)
                    {
                        var path = Path.Combine(folder, item.SuggestedFileName);

                        callback.Continue(path, showDialog: false);
                    }

                    return true;
                })
                .OnDownloadUpdated(downloadUpdated)
                .Build();
        }

        /// <summary>
        ///创建一个新的 <see cref="IDownloadHandler"/> 实例
        ///其中向用户显示默认的“另存为”对话框。
        ///</摘要>
        ///<param name="downloadUpdated">用于下载更新、跟踪进度、完成等的可选委托</param>
        ///<returns><see cref="IDownloadHandler"/> 实例。</returns>
        public static IDownloadHandler AskUser(OnDownloadUpdatedDelegate downloadUpdated = null)
        {
            return Create()
                .OnBeforeDownload((chromiumWebBrowser, browser, item, callback) =>
                {
                    using (callback)
                    {
                        callback.Continue("", showDialog: true);
                    }

                    return true;
                })
                .OnDownloadUpdated(downloadUpdated)
                .Build();
        }

        /// <summary>
        /// 使用 <see cref="Create"/> 创建 Fluent 构建器的新实例
        ///</摘要>
        internal DownloadHandler()
        {

        }

        internal void SetCanDownload(CanDownloadDelegate action)
        {
            canDownload = action;
        }

        internal void SetOnBeforeDownload(OnBeforeDownloadDelegate action)
        {
            onBeforeDownload = action;
        }

        internal void SetOnDownloadUpdated(OnDownloadUpdatedDelegate action)
        {
            onDownloadUpdated = action;
        }

        /// <inheritdoc/>
        protected override bool CanDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, string url, string requestMethod)
        {
            return canDownload?.Invoke(chromiumWebBrowser, browser, url, requestMethod) ?? true;
        }

        /// <inheritdoc/>
        protected override bool OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            return onBeforeDownload?.Invoke(chromiumWebBrowser, browser, downloadItem, callback) ?? false;
        }

        /// <inheritdoc/>
        protected override void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            onDownloadUpdated?.Invoke(chromiumWebBrowser, browser, downloadItem, callback);
        }
    }
}
