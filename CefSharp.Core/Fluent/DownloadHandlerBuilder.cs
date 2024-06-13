//版权所有 © 2021 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

namespace CefSharp.Fluent
{
    /// <summary>
    /// Fluent DownloadHandler 生成器
    /// </summary>
    public class DownloadHandlerBuilder
    {
        private readonly DownloadHandler handler = new DownloadHandler();

        /// <summary>
        ///有关详细信息，请参阅 <see cref="IDownloadHandler.CanDownload(IWebBrowser, IBrowser, string, string)"/>。
        ///</摘要>
        ///<param name="action">当 <see cref="IDownloadHandler.CanDownload(IWebBrowser, IBrowser, string, string)"/> 时执行的操作
        ///被调用</param>
        ///<返回>
        ///Fluent Builder，调用<see cref="Build"/>创建
        ///一个新的 <see cref="IDownloadHandler"/> 实例
        /// </returns>
        public DownloadHandlerBuilder CanDownload(CanDownloadDelegate action)
        {
            handler.SetCanDownload(action);

            return this;
        }

        /// <summary>
        ///有关详细信息，请参阅 <see cref="IDownloadHandler.OnBeforeDownload(IWebBrowser, IBrowser, DownloadItem, IBeforeDownloadCallback)"/>。
        ///</摘要>
        ///<param name="action">当 <see cref="IDownloadHandler.OnBeforeDownload(IWebBrowser, IBrowser, DownloadItem, IBeforeDownloadCallback)"/> 时执行的操作
        ///被调用</param>
        ///<返回>
        ///Fluent Builder，调用<see cref="Build"/>创建
        ///一个新的 <see cref="IDownloadHandler"/> 实例
        /// </returns>
        public DownloadHandlerBuilder OnBeforeDownload(OnBeforeDownloadDelegate action)
        {
            handler.SetOnBeforeDownload(action);

            return this;
        }

        /// <summary>
        ///有关详细信息，请参阅 <see cref="IDownloadHandler.OnDownloadUpdated(IWebBrowser, IBrowser, DownloadItem, IDownloadItemCallback)"/>。
        ///</摘要>
        ///<param name="action">当 <see cref="IDownloadHandler.OnDownloadUpdated(IWebBrowser, IBrowser, DownloadItem, IDownloadItemCallback)"/> 时执行的操作
        ///被调用</param>
        ///<返回>
        ///Fluent Builder，调用<see cref="DownloadHandlerBuilder.Build"/>创建
        ///一个新的 <see cref="IDownloadHandler"/> 实例
        /// </returns>
        public DownloadHandlerBuilder OnDownloadUpdated(OnDownloadUpdatedDelegate action)
        {
            handler.SetOnDownloadUpdated(action);

            return this;
        }

        /// <summary>
        ///创建一个 <see cref="IDownloadHandler"/> 实例
        ///</摘要>
        ///<returns> 一个 <see cref="IDownloadHandler"/> 实例</returns>
        public IDownloadHandler Build()
        {
            return handler;
        }
    }
}
