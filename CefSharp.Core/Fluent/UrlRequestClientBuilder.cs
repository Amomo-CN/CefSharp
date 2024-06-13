//版权所有 © 2021 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System.IO;

namespace CefSharp.Fluent
{
    /// <summary>
    ///流畅的 UrlRequestClient 构建器
    /// </summary>
    public class UrlRequestClientBuilder
    {
        private UrlRequestClient client = new UrlRequestClient();

        /// <summary>
        ///详细信息请参见 <see cref="IUrlRequestClient.GetAuthCredentials(bool, string, int, string, string, IAuthCallback)"/>
        ///</摘要>
        ///<param name="func">当 <see cref="IUrlRequestClient.GetAuthCredentials(bool, string, int, string, string, IAuthCallback)"/> 时执行的函数
        ///称为 </param>
        ///<返回>
        ///Fluent Builder，调用<see cref="UrlRequestClientBuilder.Build"/>创建
        ///一个新的 <see cref="IUrlRequestClient"/> 实例
        /// </returns>
        public UrlRequestClientBuilder GetAuthCredentials(GetAuthCredentialsDelegate func)
        {
            client.SetGetAuthCredentials(func);

            return this;
        }

        /// <summary>
        ///有关详细信息，请参阅 <see cref="IUrlRequestClient.OnDownloadData(IUrlRequest, Stream)"/>。
        ///</摘要>
        ///<param name="action">当 <see cref="IUrlRequestClient.OnDownloadData(IUrlRequest, Stream)"/> 时执行的操作
        ///被调用</param>
        ///<返回>
        ///Fluent Builder，调用<see cref="UrlRequestClientBuilder.Build"/>创建
        ///一个新的 <see cref="IUrlRequestClient"/> 实例
        /// </returns>
        public UrlRequestClientBuilder OnDownloadData(OnDownloadDataDelegate action)
        {
            client.SetOnDownloadData(action);

            return this;
        }

        /// <summary>
        ///有关详细信息，请参阅 <see cref="IUrlRequestClient.OnDownloadProgress(IUrlRequest, long, long)"/>。
        ///</摘要>
        ///<param name="action">当 <see cref="IUrlRequestClient.OnDownloadProgress(IUrlRequest, long, long)"/> 时执行的操作
        ///被调用</param>
        ///<返回>
        ///Fluent Builder，调用<see cref="UrlRequestClientBuilder.Build"/>创建
        ///一个新的 <see cref="IUrlRequestClient"/> 实例
        /// </returns>
        public UrlRequestClientBuilder OnDownloadProgress(OnDownloadProgressDelegate action)
        {
            client.SetOnDownloadProgress(action);

            return this;
        }

        /// <summary>
        ///有关详细信息，请参阅 <see cref="IUrlRequestClient.OnRequestComplete"/>。
        ///</摘要>
        ///<param name="action">当 <see cref="IUrlRequestClient.OnRequestComplete(IUrlRequest)"/> 时执行的操作
        ///被调用</param>
        ///<返回>
        ///Fluent Builder，调用<see cref="UrlRequestClientBuilder.Build"/>创建
        ///一个新的 <see cref="IUrlRequestClient"/> 实例
        /// </returns>
        public UrlRequestClientBuilder OnRequestComplete(OnRequestCompleteDelegate action)
        {
            client.SetOnRequestComplete(action);

            return this;
        }

        /// <summary>
        ///详细信息请参见 <see cref="IUrlRequestClient.OnUploadProgress(IUrlRequest, long, long)"/>。
        ///</摘要>
        ///<param name="action">当 <see cref="IUrlRequestClient.OnUploadProgress(IUrlRequest, long, long)"/> 时执行的操作
        ///被调用</param>
        ///<返回>
        ///Fluent Builder，调用<see cref="UrlRequestClientBuilder.Build"/>创建
        ///一个新的 <see cref="IUrlRequestClient"/> 实例
        /// </returns>
        public UrlRequestClientBuilder OnUploadProgress(OnUploadProgressDelegate action)
        {
            client.SetOnUploadProgress(action);

            return this;
        }

        /// <summary>
        ///创建一个 <see cref="IUrlRequestClient"/> 实例
        ///</摘要>
        ///<returns> 一个 <see cref="IUrlRequestClient"/> 实例</returns>
        public IUrlRequestClient Build()
        {
            return client;
        }
    }
}
