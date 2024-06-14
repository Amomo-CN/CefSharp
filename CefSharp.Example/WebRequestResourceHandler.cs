//版权所有 © 2016 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace CefSharp.Example
{
    /// <summary>
    /// 一个简单的 <see cref="ResourceHandler"/> 使用 <see cref="WebRequest.Create(string)"/> 来满足请求。
    ///</摘要>
    ///<备注>
    ///如果您想查看示例，此示例并未涵盖所有情况，例如 POST 请求
    ///展开后请提交拉取请求。
    /// </remarks>
    public class WebRequestResourceHandler : ResourceHandler
    {
        public override CefReturnValue ProcessRequestAsync(IRequest request, ICallback callback)
        {
            //生成一个任务并立即返回 CefReturnValue.ContinueAsync
            Task.Run(async () =>
            {
                using (callback)
                {
                    //创建标头的克隆，以便我们可以修改它
                    var headers = new NameValueCollection(request.Headers);

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(request.Url);
                    httpWebRequest.UserAgent = headers["User-Agent"];
                    httpWebRequest.Accept = headers["Accept"];
                    httpWebRequest.Method = request.Method;
                    httpWebRequest.Referer = request.ReferrerUrl;

                    //这些标头必须通过适当的属性进行设置。
                    headers.Remove("User-Agent");
                    headers.Remove("Accept");

                    httpWebRequest.Headers.Add(headers);

                    //TODO: 处理发送数据
                    var postData = request.PostData;

                    var httpWebResponse = await httpWebRequest.GetResponseAsync() as HttpWebResponse;

                    // 获取与响应关联的流。
                    var receiveStream = httpWebResponse.GetResponseStream();

                    var contentType = new ContentType(httpWebResponse.ContentType);
                    var mimeType = contentType.MediaType;
                    var charSet = contentType.CharSet;
                    var statusCode = httpWebResponse.StatusCode;

                    var memoryStream = new MemoryStream();
                    receiveStream.CopyTo(memoryStream);
                    receiveStream.Dispose();
                    httpWebResponse.Dispose();

                    //将流位置重置为 0，以便可以将流复制到底层非托管缓冲区中
                    memoryStream.Position = 0;

                    ResponseLength = memoryStream.Length;
                    MimeType = mimeType;
                    Charset = charSet ?? "UTF-8";
                    StatusCode = (int)statusCode;
                    Stream = memoryStream;
                    AutoDisposeStream = true;

                    callback.Continue();
                }
            });

            return CefReturnValue.ContinueAsync;
        }
    }
}
