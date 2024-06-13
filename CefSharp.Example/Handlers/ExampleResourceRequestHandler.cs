//版权所有 © 2019 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。
using System;
using System.IO;
using System.Text;
using CefSharp.Example.Filters;
using CefSharp.Handler;
using CefSharp.ResponseFilter;

namespace CefSharp.Example.Handlers
{
    /// <summary>
    /// ExampleResourceRequestHandler 演示了您可以执行的一些功能
    ///使用 <see cref="ResourceRequestHandler"/>
    ///</摘要>
    ///<备注>
    ///<see cref="ResourceRequestHandler"/> 表示单个资源请求
    /// </remarks>
    public class ExampleResourceRequestHandler : ResourceRequestHandler
    {
        private MemoryStream memoryStream;

        protected override CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            Uri url;
            if (Uri.TryCreate(request.Url, UriKind.Absolute, out url) == false)
            {
                //如果我们无法解析 Uri，则取消请求
                //避免在此处抛出任何异常，因为我们被非托管代码调用
                return CefReturnValue.Cancel;
            }

            //Referer 设置方法示例
            //设置任何标头时应该同样有效

            //对于此示例，仅在使用我们的自定义方案时设置 Referer
            if (url.Scheme == CefSharpSchemeHandlerFactory.SchemeName)
            {
                //现在使用它自己的方法设置引荐来源网址（之前在标题中设置）
                request.SetReferrer("http://google.com", ReferrerPolicy.Default);
            }

            //在每个请求中设置 User-Agent 的示例。
            //var headers = request.Headers;

            //var userAgent = headers["用户代理"];
            //headers["User-Agent"] = userAgent + " CefSharp";

            //请求头=标题;

            //注意：如果您不想实现此方法，则返回 false 是默认行为
            //我们还建议您显式处理回调，因为它包装了非托管资源。
            //callback.Dispose();
            //返回假；
            //注意：以异步方式执行回调时需要检查它是否已处理
            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    if (request.Method == "POST")
                    {
                        using (var postData = request.PostData)
                        {
                            if (postData != null)
                            {
                                var elements = postData.Elements;

                                var charSet = request.GetCharSet();

                                foreach (var element in elements)
                                {
                                    if (element.Type == PostDataElementType.Bytes)
                                    {
                                        var body = element.GetBody(charSet);
                                    }
                                }
                            }
                        }
                    }

                    //注意Redirect只需设置请求Url
                    //if (request.Url.StartsWith("https://www.google.com", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    request.Url = "https://github.com/";
                    //}

                    //异步方式回调
                    //callback.Continue(true);
                    //返回CefReturnValue.ContinueAsync;
                }
            }

            return CefReturnValue.Continue;
        }

        protected override void OnResourceRedirect(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
        {
            //如何重定向的示例 -需要在第二遍中检查 `newUrl`
            //if (request.Url.StartsWith("https://www.google.com", StringComparison.OrdinalIgnoreCase) && !newUrl.Contains("github"))
            //{
            //    newUrl = "https://github.com";
            //}
        }

        protected override bool OnProtocolExecution(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            return request.Url.StartsWith("mailto");
        }

        protected override bool OnResourceResponse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            //注意：您不能修改响应，只能修改请求
            //您现在可以访问标头
            //var headers = response.Headers;

            return false;
        }

        protected override IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            var url = new Uri(request.Url);
            if (url.Scheme == CefSharpSchemeHandlerFactory.SchemeName)
            {
                if (request.Url.Equals(CefExample.ResponseFilterTestUrl, StringComparison.OrdinalIgnoreCase))
                {
                    return new FindReplaceResponseFilter("REPLACE_THIS_STRING", "这是替换后的字符串!");
                }

                if (request.Url.Equals("custom://cefsharp/assets/js/jquery.js", StringComparison.OrdinalIgnoreCase))
                {
                    return new AppendResponseFilter(System.Environment.NewLine + "//CefSharp 附加了此评论.");
                }

                //只需要我们的定制方案
                memoryStream = new MemoryStream();
                return new StreamResponseFilter(memoryStream);
            }

            //return new PassThruResponseFilter();
            return null;
        }

        protected override void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            var url = new Uri(request.Url);
            if (url.Scheme == CefSharpSchemeHandlerFactory.SchemeName && memoryStream != null)
            {
                //TODO: 对这里的数据做一些事情
                var data = memoryStream.ToArray();
                var dataLength = data.Length;
                //NOTE: 您可能需要根据请求使用不同的编码
                var dataAsUtf8String = Encoding.UTF8.GetString(data);
            }
        }

        protected override void Dispose()
        {
            memoryStream?.Dispose();
            memoryStream = null;

            base.Dispose();
        }
    }
}
