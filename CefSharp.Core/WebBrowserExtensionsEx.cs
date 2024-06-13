//版权所有 © 2021 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using CefSharp.Internals;
using CefSharp.ModelBinding;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CefSharp
{
    /// <summary>
    /// 扩展的WebBrowserExtensions
    /// </summary>
    public static class WebBrowserExtensionsEx
    {
        /// <summary>
        ///检索当前的 <see cref="NavigationEntry"/>。包含类似信息
        ///<see cref="NavigationEntry.HttpStatusCode"/> 和 <see cref="NavigationEntry.SslStatus"/>
        ///</摘要>
        ///<param name="browser">此方法扩展的 ChromiumWebBrowser 实例。</param>
        ///<返回>
        ///<see cref="Task{NavigationEntry}"/> 执行时返回当前的 <see cref="NavigationEntry"/> 或 null
        /// </returns>
        public static Task<NavigationEntry> GetVisibleNavigationEntryAsync(this IChromiumWebBrowserBase browser)
        {
            var host = browser.GetBrowserHost();

            if (host == null)
            {
                return Task.FromResult<NavigationEntry>(null);
            }

            if (Cef.CurrentlyOnThread(CefThreadIds.TID_UI))
            {
                var entry = host.GetVisibleNavigationEntry();

                return Task.FromResult<NavigationEntry>(entry);
            }

            var tcs = new TaskCompletionSource<NavigationEntry>(TaskCreationOptions.RunContinuationsAsynchronously);

            Cef.UIThreadTaskFactory.StartNew(delegate
            {
                var entry = host.GetVisibleNavigationEntry();

                tcs.TrySetResult(entry);
            });

            return tcs.Task;
        }

        /// <summary>
        ///下载指定的 <paramref name="url"/> 并调用 <paramref name="completeHandler"/>
        ///当下载完成时。发出 GET 请求。
        ///</摘要>
        ///<param name="frame">有效框架</param>
        ///<param name="url">下载地址</param>
        ///<param name="completeHandler">下载完成时执行的操作。</param>
        public static void DownloadUrl(this IFrame frame, string url, Action<IUrlRequest, Stream> completeHandler)
        {
            if (!frame.IsValid)
            {
                throw new Exception("框架无效，无法继续.");
            }

            //可以在任何有效的 CEF 线程上创建，这里我们将使用 CEF UI 线程
            Cef.UIThreadTaskFactory.StartNew(delegate
            {
                var request = frame.CreateRequest(false);

                request.Method = "GET";
                request.Url = url;

                var memoryStream = new MemoryStream();

                var urlRequestClient = Fluent.UrlRequestClient
                    .Create()
                    .OnDownloadData((req, stream) =>
                    {
                        stream.CopyTo(memoryStream);
                    })
                    .OnRequestComplete((req) =>
                    {
                        memoryStream.Position = 0;

                        completeHandler?.Invoke(req, memoryStream);
                    })
                    .Build();

                var urlRequest = frame.CreateUrlRequest(request, urlRequestClient);
            });
        }

        /// <summary>
        /// 将指定的 <paramref name="url"/> 下载为 <see cref="T:byte[]"/>。
        ///发出 GET 请求。
        ///</摘要>
        ///<param name="frame">有效框架</param>
        ///<param name="url">下载地址</param>
        ///<param name="urlRequestFlags">控制缓存策略</param>
        ///<returns>可以等待获取表示 Url 的 <see cref="T:byte[]"/> 的任务</returns>
        public static Task<byte[]> DownloadUrlAsync(this IFrame frame, string url, UrlRequestFlags urlRequestFlags = UrlRequestFlags.None)
        {
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame));
            }

            if (!frame.IsValid)
            {
                throw new Exception("Frame is invalid, unable to continue.");
            }

            var taskCompletionSource = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);

            //可以在任何有效的 CEF 线程上创建，这里我们将使用 CEF UI 线程
            Cef.UIThreadTaskFactory.StartNew(delegate
            {
                var request = frame.CreateRequest(false);

                request.Method = "GET";
                request.Url = url;
                request.Flags = urlRequestFlags;

                var memoryStream = new MemoryStream();

                var urlRequestClient = Fluent.UrlRequestClient
                    .Create()
                    .OnDownloadData((req, stream) =>
                    {
                        stream.CopyTo(memoryStream);
                    })
                    .OnRequestComplete((req) =>
                    {
                        if (req.RequestStatus == UrlRequestStatus.Success)
                        {
                            taskCompletionSource.TrySetResult(memoryStream.ToArray());
                        }
                        else
                        {
                            taskCompletionSource.TrySetException(new Exception("RequestStatus:" + req.RequestStatus + ";StatusCode:" + req.Response.StatusCode));
                        }
                    })
                    .Build();

                var urlRequest = frame.CreateUrlRequest(request, urlRequestClient);
            });

            return taskCompletionSource.Task;
        }

        /// <summary>
        ///切换当前浏览器的音频静音。
        ///如果 <paramref name="browser"/> 为 null 或已被释放
        ///那么这个命令将是一个空操作。
        ///</摘要>
        ///<param name="browser">此方法扩展的 ChromiumWebBrowser 实例。</param>
        public static void ToggleAudioMute(this IChromiumWebBrowserBase browser)
        {
            if (browser.IsDisposed || Cef.IsShutdown)
            {
                return;
            }

            _ = Cef.UIThreadTaskFactory.StartNew(delegate
            {
                var cefBrowser = browser.BrowserCore;

                if (cefBrowser == null || cefBrowser.IsDisposed)
                {
                    return;
                }

                var host = cefBrowser.GetHost();

                var isAudioMuted = host.IsAudioMuted;

                host.SetAudioMuted(!isAudioMuted);
            });
        }

        /// <summary>
        ///在 <paramref name="frame"/> 的上下文中评估 javascript 代码。脚本将被执行
        ///异步，该方法返回一个可以等待获取结果的任务。
        ///</摘要>
        ///<typeparam name="T">类型</typeparam>
        ///<exception cref="ArgumentOutOfRangeException">当一个或多个参数超出所需范围时抛出。</exception>
        ///<exception cref="Exception">如果发生 Javascript 错误则抛出。</exception>
        ///<param name="frame">此方法扩展的 IFrame 实例。</param>
        ///<param name="script">应该执行的Javascript代码。</param>
        ///<param name="timeout">（可选）超时后应中止 Javascript 代码执行。</param>
        ///<返回>
        ///<see cref="Task{T}"/> 可以等待获取脚本执行的结果。 <参见 cref="ModelBinding.DefaultBinder"/>
        ///用于将结果转换为所需的类型。属性名称是从驼峰命名法转换而来的。
        ///如果脚本执行返回错误，则抛出异常。
        /// </returns>
        public static async Task<T> EvaluateScriptAsync<T>(this IFrame frame, string script, TimeSpan? timeout = null)
        {
            WebBrowserExtensions.ThrowExceptionIfFrameNull(frame);

            if (timeout.HasValue && timeout.Value.TotalMilliseconds > uint.MaxValue)
            {
                throw new ArgumentOutOfRangeException("timeout", "Timeout greater than Maximum allowable value of " + UInt32.MaxValue);
            }

            var response = await frame.EvaluateScriptAsync(script, timeout: timeout, useImmediatelyInvokedFuncExpression: false).ConfigureAwait(false);

            if (response.Success)
            {
                var binder = DefaultBinder.Instance;

                return (T)binder.Bind(response.Result, typeof(T));
            }

            throw new Exception(response.Message);
        }

        /// <summary>
        ///在 ChromiumWebBrowser 的 MainFrame 上下文中评估一些 Javascript 代码。脚本将被执行
        ///异步，该方法返回一个封装来自 Javascript 响应的任务
        ///</摘要>
        ///<typeparam name="T">类型</typeparam>
        ///<exception cref="ArgumentOutOfRangeException">当一个或多个参数超出所需范围时抛出。</exception>
        ///<param name="browser">此方法扩展的 IBrowser 实例。</param>
        ///<param name="script">应该执行的 JavaScript 代码。</param>
        ///<param name="timeout">（可选）超时后应中止 JavaScript 代码执行。</param>
        ///<返回>
        ///<see cref="Task{T}"/> 可以等待获取 JavaScript 执行的结果。
        /// </returns>
        public static Task<T> EvaluateScriptAsync<T>(this IBrowser browser, string script, TimeSpan? timeout = null)
        {
            WebBrowserExtensions.ThrowExceptionIfBrowserNull(browser);

            using (var frame = browser.MainFrame)
            {
                return frame.EvaluateScriptAsync<T>(script, timeout: timeout);
            }
        }

        /// <summary>
        ///在此浏览器主框架的上下文中评估 Javascript。脚本将被执行
        ///异步，该方法返回一个封装来自 Javascript 响应的任务
        ///</摘要>
        ///<exception cref="ArgumentOutOfRangeException">当一个或多个参数超出所需范围时抛出。</exception>
        ///<typeparam name="T">类型</typeparam>
        ///<param name="chromiumWebBrowser">此方法扩展的 ChromiumWebBrowser 实例。</param>
        ///<param name="script">应该执行的Javascript代码。</param>
        ///<param name="timeout">（可选）超时后应中止 Javascript 代码执行。</param>
        ///<返回>
        ///<see cref="Task{T}"/> 可以等待获取脚本执行的结果。
        ///</返回>
        public static Task<T> EvaluateScriptAsync<T>(this IChromiumWebBrowserBase chromiumWebBrowser, string script, TimeSpan? timeout = null)
        {
            WebBrowserExtensions.ThrowExceptionIfChromiumWebBrowserDisposed(chromiumWebBrowser);

            if (chromiumWebBrowser is IWebBrowser b)
            {
                if (b.CanExecuteJavascriptInMainFrame == false)
                {
                    WebBrowserExtensions.ThrowExceptionIfCanExecuteJavascriptInMainFrameFalse();
                }
            }

            return chromiumWebBrowser.BrowserCore.EvaluateScriptAsync<T>(script, timeout);
        }
    }
}
