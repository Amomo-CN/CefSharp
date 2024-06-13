//版权所有 © 2016 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CefSharp.SchemeHandler;

namespace CefSharp.Example.Handlers
{
    public class BrowserProcessHandler : CefSharp.Handler.BrowserProcessHandler
    {
        /// <summary>
        /// 调用 Cef.DoMessageLoopWork 之间的时间间隔
        /// </summary>
        protected const int SixtyTimesPerSecond = 1000 / 60;  // 60fps
        /// <summary>
        /// 我们愿意在调用 OnScheduleMessagePumpWork() 之间等待的最大毫秒数。
        /// </summary>
        protected const int ThirtyTimesPerSecond = 1000 / 30;  //30fps

        protected override void OnContextInitialized()
        {
            //Global CookieManager 已初始化，您现在可以设置 cookie
            var cookieManager = Cef.GetGlobalCookieManager();

            if (cookieManager.SetCookie("custom://cefsharp/home.html", new Cookie
            {
                Name = "CefSharpTestCookie",
                Value = "ILikeCookies",
                Expires = DateTime.Now.AddDays(1)
            }))
            {
                cookieManager.VisitUrlCookiesAsync("custom://cefsharp/home.html", false).ContinueWith(previous =>
                {
                    if (previous.Status == TaskStatus.RanToCompletion)
                    {
                        var cookies = previous.Result;

                        foreach (var cookie in cookies)
                        {
                            Debug.WriteLine("CookieName: " + cookie.Name);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("未找到 Cookie");
                    }
                });

                cookieManager.VisitAllCookiesAsync().ContinueWith(t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        var cookies = t.Result;

                        foreach (var cookie in cookies)
                        {
                            Debug.WriteLine("CookieName: " + cookie.Name);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("未找到 Cookie");
                    }
                });
            }

            //请求上下文已初始化，您现在可以设置首选项，例如代理服务器设置
            //完成后处理上下文 -如果可能的话最好不要保留引用。
            using (var context = Cef.GetGlobalRequestContext())
            {
                string errorMessage;

                //您可以使用“.”符号设置大多数首选项，而不必创建一组复杂的字典。
                //默认为true，可以更改为false以禁用文本区域大小调整
                //这只是一个示例，可以通过首选项使用许多配置选项
                var success = context.SetPreference("webkit.webprefs.text_areas_are_resizable", true, out errorMessage);

                if (!success)
                {
                    //检查错误消息以获取更多详细信息
                }

                //string error;
                //var dicts = new List<string> { "en-GB", "en-US" };
                //var success = context.SetPreference("spellcheck.dictionaries", dicts, out error);

                //no-proxy-server 标志在 CefExample.cs 类中设置，您必须在测试之前将其删除
                //这段代码输出
                //var v = new Dictionary<string, string>
                //{
                //    ["mode"] = "fixed_servers",
                //    ["server"] = "scheme://host:port"
                //};
                //success = context.SetPreference("proxy", v, out errorMessage);

                //可以为默认的 http 和 https 方案注册一个方案处理程序
                //在此示例中，我们为 https://cefsharp.example 注册FolderSchemeHandlerFactory
                //最好包含域名，这样只有对该域的请求才会转发到您的方案处理程序
                //可以拦截某个方案的所有请求，包括内置的 http/https 请求，请务必小心！
                const string cefSharpExampleResourcesFolder =
#if !NETCOREAPP
                    @"..\..\..\..\CefSharp.Example\Resources";
#else
                    @"..\..\..\..\..\..\CefSharp.Example\Resources";
#endif
                var folderSchemeHandlerExample = new FolderSchemeHandlerFactory(rootFolder: cefSharpExampleResourcesFolder,
                                                                        hostName: "cefsharp.example", //可选参数无主机名检查是否为空
                                                                        defaultPage: "home.html"); //可选参数默认为index.html

                context.RegisterSchemeHandlerFactory("https", "cefsharp.example", folderSchemeHandlerExample);
            }
        }
    }
}
