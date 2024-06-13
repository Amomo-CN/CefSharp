//版权所有 © 2012 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Threading.Tasks;
using CefSharp.Handler;

namespace CefSharp.Example.Handlers
{
    /// <summary>
    /// <see cref="RequestHandler"/> 提供了一个基类供您继承 
    ///你只需要实现与你相关的方法。 
    ///如果您实现 IRequestHandler 接口，您将需要
    ///实现每个方法
    /// </summary>
    public class ExampleRequestHandler : RequestHandler
    {
        public static readonly string VersionNumberString = String.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}",
            Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion);

        protected override bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            return false;
        }

        protected override bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        protected override bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            //NOTE: 我们还建议您将回调包装在 using 语句中或显式执行回调。Dispose 因为回调包装了非托管资源。

            //例子#1
            //返回true并稍后调用IRequestCallback.Continue()以继续或取消请求。
            //在此实例中，我们将使用一个任务，通常您会调用 UI 线程并向用户显示一个对话框
            //您可以将 IWebBrowser 参数转换为 ChromiumWebBrowser 以轻松访问
            //控制，您可以从那里调用 UI 线程，应该采用异步方式
            Task.Run(() =>
            {
                //NOTE:以异步方式执行回调时，需要检查它是否已处理
                if (!callback.IsDisposed)
                {
                    using (callback)
                    {
                        //我们将允许来自 badssl.com 的过期证书
                        if (requestUrl.ToLower().Contains("https://expired.badssl.com/"))
                        {
                            callback.Continue(true);
                        }
                        else
                        {
                            callback.Continue(false);
                        }
                    }
                }
            });

            return true;

            //例子#2
            //执行回调并返回true立即允许无效证书
            //callback.Continue(true); //回调一旦执行就会释放它自己
            //返回真；

            //例子#3
            //默认行为返回false（立即取消请求）
            //callback.Dispose(); //处理回调
            //返回假；
        }

        protected override bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            //NOTE:我们还建议您显式处理回调，因为它包装了非托管资源。

            //Example #1
            //生成一个任务来执行我们的回调并返回 true；
            //典型用法是调用 UI 线程来打开用户名/密码对话框
            //然后使用响应的用户名/密码执行回调
            //您可以将 IWebBrowser 参数转换为 ChromiumWebBrowser 以轻松访问
            //控制，您可以从那里调用 UI 线程，应该采用异步方式
            //在浏览器中加载https://httpbin.org/basic-auth/cefsharp/passwd进行测试
            Task.Run(() =>
            {
                using (callback)
                {
                    if (originUrl.Contains("https://httpbin.org/basic-auth/"))
                    {
                        var parts = originUrl.Split('/');
                        var username = parts[parts.Length - 2];
                        var password = parts[parts.Length - 1];
                        callback.Continue(username, password);
                    }
                }
            });

            return true;

            //例子#2
            //返回false取消请求
            //callback.Dispose();
            //返回假；
        }

        protected override void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status, int errorCode, string errorMessage)
        {
            // TODO:在此处添加您自己的代码，以处理渲染进程因某种原因终止的情况。
            chromiumWebBrowser.Load(CefExample.RenderProcessCrashedUrl);
        }

        protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            //NOTE:在大多数情况下，您会检查 request.Url 并仅处理您感兴趣的请求
            if (request.Url.ToLower().StartsWith("https://cefsharp.example")
                || request.Url.ToLower().StartsWith(CefSharpSchemeHandlerFactory.SchemeName)
                || request.Url.ToLower().StartsWith("mailto:")
                || request.Url.ToLower().StartsWith("https://googlechrome.github.io/samples/service-worker/"))
            {
                return new ExampleResourceRequestHandler();
            }

            return null;
        }
    }
}
