//版权所有 © 2021 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。
using System.IO;

namespace CefSharp.Fluent
{
    /// <summary>
    ///当浏览器需要用户的凭据时在 CEF IO 线程上调用。
    ///只有浏览器进程发起的请求才会调用此方法。 
    ///</摘要>
    ///<param name="isProxy">表示该主机是否为代理服务器</param>
    ///<param name="host">主机名。</param>
    ///<param name="port">端口号。</param>
    ///<param name="realm">领域</param>
    ///<param name="scheme">方案</param>
    ///<param name="callback">是认证信息的回调</param>
    ///<返回>
    ///返回true继续请求，当认证信息可用时调用<see cref="IAuthCallback.Continue(string, string)"/>。
    ///如果请求有关联的浏览器/框架，则返回 false 将导致调用 <see cref="IRequestHandler.GetAuthCredentials"/> 
    ///与该浏览器关联的 <see cref="IRequestHandler"/>（如果有）。
    ///否则，返回 false 将立即取消请求。
    /// </returns>
    public delegate bool GetAuthCredentialsDelegate(bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback);

    /// <summary>
    ///当读取响应的某些部分时调用。如果在请求上设置了 <see cref="UrlRequestFlags.NoDownloadData"/> 标志，则不会调用此方法。 
    ///</摘要>
    ///<param name="request">请求</param>
    ///<param name="data">包含自上次调用以来接收到的字节的流。不能在该方法的范围之外使用。 </参数>
    public delegate void OnDownloadDataDelegate(IUrlRequest request, Stream data);

    /// <summary>
    ///通知客户端下载进度。
    ///</摘要>
    ///<param name="request">请求</param>
    ///<param name="current">表示调用之前接收到的字节数</param>
    ///<param name="total">是响应的预期总大小（如果未确定，则为 -1）。</param>
    public delegate void OnDownloadProgressDelegate(IUrlRequest request, long current, long total);

    /// <summary>
    ///通知客户端请求已完成。
    ///使用 <see cref="IUrlRequest.RequestStatus"/> 属性来确定是否
    ///请求是否成功。
    ///</摘要>
    ///<param name="request">请求</param>
    public delegate void OnRequestCompleteDelegate(IUrlRequest request);

    /// <summary>
    ///通知客户端上传进度。
    ///仅当请求上设置了 UR_FLAG_REPORT_UPLOAD_PROGRESS 标志时才会调用此方法。
    ///</摘要>
    ///<param name="request">请求</param>
    ///<param name="current">表示到目前为止发送的字节数。</param>
    ///<param name="total">是上传数据的总大小（如果启用分块上传，则为-1）。</param>
    public delegate void OnUploadProgressDelegate(IUrlRequest request, long current, long total);

    /// <summary>
    /// 流畅的 UrlRequestClient
    /// </summary>
    public class UrlRequestClient : CefSharp.UrlRequestClient
    {
        private GetAuthCredentialsDelegate getAuthCredentials;
        private OnDownloadDataDelegate onDownloadData;
        private OnDownloadProgressDelegate onDownloadProgress;
        private OnRequestCompleteDelegate onRequestComplete;
        private OnUploadProgressDelegate onUploadProgress;

        /// <summary>
        ///创建一个新的 UrlRequestClient 构建器
        ///</摘要>
        ///<returns>Fluent UrlRequestClient Builder</returns>
        public static UrlRequestClientBuilder Create()
        {
            return new UrlRequestClientBuilder();
        }

        /// <summary>
        /// 使用 <see cref="Create"/> 创建 Fluent 构建器的新实例
        /// </summary>
        internal UrlRequestClient()
        {

        }

        internal void SetGetAuthCredentials(GetAuthCredentialsDelegate func)
        {
            getAuthCredentials = func;
        }

        internal void SetOnDownloadData(OnDownloadDataDelegate action)
        {
            onDownloadData = action;
        }

        internal void SetOnDownloadProgress(OnDownloadProgressDelegate action)
        {
            onDownloadProgress = action;
        }

        internal void SetOnRequestComplete(OnRequestCompleteDelegate action)
        {
            onRequestComplete = action;
        }

        internal void SetOnUploadProgress(OnUploadProgressDelegate action)
        {
            onUploadProgress = action;
        }

        /// <inheritdoc/>
        protected override bool GetAuthCredentials(bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            return getAuthCredentials?.Invoke(isProxy, host, port, realm, scheme, callback) ?? false;
        }

        /// <inheritdoc/>
        protected override void OnDownloadData(IUrlRequest request, Stream data)
        {
            onDownloadData?.Invoke(request, data);
        }

        /// <inheritdoc/>
        protected override void OnDownloadProgress(IUrlRequest request, long current, long total)
        {
            onDownloadProgress?.Invoke(request, current, total);
        }

        /// <inheritdoc/>
        protected override void OnRequestComplete(IUrlRequest request)
        {
            onRequestComplete?.Invoke(request);
        }

        /// <inheritdoc/>
        protected override void OnUploadProgress(IUrlRequest request, long current, long total)
        {
            onUploadProgress?.Invoke(request, current, total);
        }
    }
}
