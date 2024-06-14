//版权所有 © 2019 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.IO;

namespace CefSharp.Example
{
    public class UrlRequestClient : IUrlRequestClient
    {
        private readonly Action<IUrlRequest, byte[]> completeAction;
        private readonly MemoryStream responseBody = new MemoryStream();

        public UrlRequestClient(Action<IUrlRequest, byte[]> completeAction)
        {
            this.completeAction = completeAction;
        }

        bool IUrlRequestClient.GetAuthCredentials(bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            return true;
        }

        void IUrlRequestClient.OnDownloadData(IUrlRequest request, Stream data)
        {
            data.CopyTo(responseBody);
        }

        void IUrlRequestClient.OnDownloadProgress(IUrlRequest request, long current, long total)
        {

        }

        void IUrlRequestClient.OnRequestComplete(IUrlRequest request)
        {
            this?.completeAction(request, responseBody.ToArray());
        }

        void IUrlRequestClient.OnUploadProgress(IUrlRequest request, long current, long total)
        {

        }
    }
}
