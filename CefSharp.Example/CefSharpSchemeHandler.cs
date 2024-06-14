//版权所有 © 2012 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CefSharp.Example
{
    internal class CefSharpSchemeHandler : ResourceHandler
    {
        public override CefReturnValue ProcessRequestAsync(IRequest request, ICallback callback)
        {
            var uri = new Uri(request.Url);
            var fileName = uri.AbsolutePath;

            Task.Run(() =>
            {
                using (callback)
                {
                    Stream stream = null;

                    if (string.Equals(fileName, "/PostDataTest.html", StringComparison.OrdinalIgnoreCase))
                    {
                        var postDataElement = request.PostData.Elements.FirstOrDefault();
                        stream = ResourceHandler.GetMemoryStream("发送数据: " + (postDataElement == null ? "无效的" : postDataElement.GetBody()), Encoding.UTF8);
                    }

                    if (string.Equals(fileName, "/PostDataAjaxTest.html", StringComparison.OrdinalIgnoreCase))
                    {
                        var postData = request.PostData;
                        if (postData == null)
                        {
                            stream = ResourceHandler.GetMemoryStream("发送数据：空", Encoding.UTF8);
                        }
                        else
                        {
                            var postDataElement = postData.Elements.FirstOrDefault();
                            stream = ResourceHandler.GetMemoryStream("发送数据： " + (postDataElement == null ? "无效的" : postDataElement.GetBody()), Encoding.UTF8);
                        }
                    }

                    if (stream == null)
                    {
                        callback.Cancel();
                    }
                    else
                    {
                        //将流位置重置为 0，以便可以将流复制到底层非托管缓冲区中
                        stream.Position = 0;
                        //填充响应值 -不再需要实现 GetResponseHeaders（除非您需要执行重定向）
                        ResponseLength = stream.Length;
                        MimeType = "text/html";
                        StatusCode = (int)HttpStatusCode.OK;
                        Stream = stream;

                        callback.Continue();
                    }
                }
            });

            return CefReturnValue.ContinueAsync;
        }
    }
}
