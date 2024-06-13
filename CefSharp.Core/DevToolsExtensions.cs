//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System.Collections.Generic;
using System.Threading.Tasks;
using CefSharp.DevTools;
using CefSharp.Internals;
using CefSharp.Web;

namespace CefSharp
{
    /// <summary>
    /// 通过 <see cref="IBrowserHost"/> 访问 DevTools 的扩展
    ///</摘要>
    public static class DevToolsExtensions
    {
        /// <summary>
        /// 通过 DevTools 协议执行方法调用。这是一个更加结构化的
        ///SendDevToolsMessage 的版本。
        ///有关详细信息，请参阅 https://chromedevtools.github.io/devtools-protocol/上的 DevTools 协议文档
        ///支持的方法和预期的 <paramref name="parameters"/> JSON 消息格式。
        ///请参阅 SendDevToolsMessage 文档以获取其他使用信息。
        ///</摘要>
        ///<param name="browserHost">浏览器主机</param>
        ///<param name="messageId">是唯一标识消息的增量编号（传递 0 以分配下一个编号
        ///自动根据之前的值）</param>
        ///<param name="method">是方法名称</param>
        ///<param name="parameters">是表示为 <see cref="JsonString"/> 的方法参数，
        ///可能为空。</param>
        ///<returns>如果在 CEF UI 线程上调用并且消息是，则返回分配的消息 ID
        ///成功提交验证，否则0</returns>
        public static int ExecuteDevToolsMethod(this IBrowserHost browserHost, int messageId, string method, JsonString parameters)
        {
            WebBrowserExtensions.ThrowExceptionIfBrowserHostNull(browserHost);

            var json = parameters == null ? null : parameters.Json;

            return browserHost.ExecuteDevToolsMethod(messageId, method, json);
        }

        /// <summary>
        ///通过 DevTools 协议执行方法调用。这是一个更加结构化的
        ///SendDevToolsMessage 的版本。 <see cref="ExecuteDevToolsMethod"/> 只能在
        ///CEF UI Thread，该方法可以在任何线程上调用。
        ///有关详细信息，请参阅 https://chromedevtools.github.io/devtools-protocol/上的 DevTools 协议文档
        ///支持的方法和预期的 <paramref name="parameters"/> 字典内容。
        ///请参阅 SendDevToolsMessage 文档以获取其他使用信息。
        ///</摘要>
        ///<param name="browser">浏览器实例</param>
        ///<param name="messageId">是唯一标识消息的增量编号（传递 0 以分配下一个编号
        ///自动根据之前的值）</param>
        ///<param name="method">是方法名称</param>
        ///<param name="parameters">是以字典形式表示的方法参数，
        ///可能为空。</param>
        ///<returns>返回一个可以等待获取分配的消息Id的任务。如果消息是
        ///提交验证失败，该值为0。</returns>
        public static Task<int> ExecuteDevToolsMethodAsync(this IBrowser browser, int messageId, string method, IDictionary<string, object> parameters = null)
        {
            WebBrowserExtensions.ThrowExceptionIfBrowserNull(browser);

            var browserHost = browser.GetHost();

            WebBrowserExtensions.ThrowExceptionIfBrowserHostNull(browserHost);

            if (CefThread.CurrentlyOnUiThread)
            {
                return Task.FromResult(browserHost.ExecuteDevToolsMethod(messageId, method, parameters));
            }

            if (CefThread.CanExecuteOnUiThread)
            {
                return CefThread.ExecuteOnUiThread(() =>
                {
                    return browserHost.ExecuteDevToolsMethod(messageId, method, parameters);
                });
            }

            //CEF 返回 0 表示失败，我们也会这样做。
            return Task.FromResult(0);
        }

        /// <summary>
        ///通过 DevTools 协议执行方法调用。这是一个更加结构化的
        ///SendDevToolsMessage 的版本。 <see cref="ExecuteDevToolsMethod"/> 只能在
        ///CEF UI Thread，该方法可以在任何线程上调用。
        ///有关详细信息，请参阅 https://chromedevtools.github.io/devtools-protocol/上的 DevTools 协议文档
        ///支持的方法和预期的 <paramref name="parameters"/> 字典内容。
        ///请参阅 SendDevToolsMessage 文档以获取其他使用信息。
        ///</摘要>
        ///<param name="chromiumWebBrowser">ChromiumWebBrowser 实例</param>
        ///<param name="messageId">是唯一标识消息的增量编号（传递 0 以分配下一个编号
        ///自动根据之前的值）</param>
        ///<param name="method">是方法名称</param>
        ///<param name="parameters">是以字典形式表示的方法参数，
        ///可能为空。</param>
        ///<returns>返回一个可以等待获取分配的消息Id的任务。如果消息是
        ///提交验证失败，该值为0。</returns>
        public static Task<int> ExecuteDevToolsMethodAsync(this IChromiumWebBrowserBase chromiumWebBrowser, int messageId, string method, IDictionary<string, object> parameters = null)
        {
            var browser = chromiumWebBrowser.BrowserCore;

            return browser.ExecuteDevToolsMethodAsync(messageId, method, parameters);
        }

        ///<摘要>
        ///获取 chromiumWebBrowser 的 DevTools 客户端的新实例
        ///实例。
        ///</摘要>
        ///<param name="chromiumWebBrowser">chromiumWebBrowser 实例</param>
        ///<returns>DevToolsClient</returns>
        public static DevToolsClient GetDevToolsClient(this IChromiumWebBrowserBase chromiumWebBrowser)
        {
            var browser = chromiumWebBrowser.BrowserCore;

            return browser.GetDevToolsClient();
        }

        /// <summary>
        ///获取 DevTools 客户端的新实例 
        ///</摘要>
        ///<param name="browser">IBrowser 实例</param>
        ///<returns>DevToolsClient</returns>    
        public static DevToolsClient GetDevToolsClient(this IBrowser browser)
        {
            WebBrowserExtensions.ThrowExceptionIfBrowserNull(browser);

            var browserHost = browser.GetHost();

            WebBrowserExtensions.ThrowExceptionIfBrowserHostNull(browserHost);

            var devToolsClient = new DevToolsClient(browser);

            var observerRegistration = browserHost.AddDevToolsMessageObserver(devToolsClient);

            devToolsClient.SetDevToolsObserverRegistration(observerRegistration);

            return devToolsClient;
        }

        /// <summary>
        /// 使用 DevTools 协议设置主机的文档内容。
        ///</摘要>
        ///<param name="chromiumWebBrowser">ChromiumWebBrowser 实例</param>
        ///<param name="html">html</param>
        ///<returns>可以等待以确定内容是否已成功更新的任务。 </returns>
        public static Task<bool> SetMainFrameDocumentContentAsync(this IChromiumWebBrowserBase chromiumWebBrowser, string html)
        {
            var browser = chromiumWebBrowser.BrowserCore;

            return browser.SetMainFrameDocumentContentAsync(html);
        }

        /// <summary>
        ///使用 DevTools 协议设置主机的文档内容。
        ///</摘要>
        ///<param name="browser">浏览器实例</param>
        ///<param name="html">html</param>
        ///<returns>可以等待以确定内容是否已成功更新的任务。 </returns>
        public static async Task<bool> SetMainFrameDocumentContentAsync(this IBrowser browser, string html)
        {
            WebBrowserExtensions.ThrowExceptionIfBrowserNull(browser);

            using (var client = browser.GetDevToolsClient())
            {
                var response = await client.Page.GetFrameTreeAsync().ConfigureAwait(false);

                var frames = response.FrameTree;
                var mainFrame = frames.Frame;

                var setContentResponse = await client.Page.SetDocumentContentAsync(mainFrame.Id, html).ConfigureAwait(false);

                return setContentResponse.Success;
            }
        }
    }
}
