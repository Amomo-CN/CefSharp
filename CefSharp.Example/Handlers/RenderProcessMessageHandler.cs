//版权所有 © 2015 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;

namespace CefSharp.Example.Handlers
{
    public class RenderProcessMessageHandler : IRenderProcessMessageHandler
    {
        void IRenderProcessMessageHandler.OnFocusedNodeChanged(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IDomNode node)
        {
            var message = node == null ? "lost focus" : node.ToString();

            Console.WriteLine("OnFocusedNodeChanged() - " + message);
        }

        void IRenderProcessMessageHandler.OnContextCreated(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
        {
            // 为每个创建的 V8Context 调用，检查 IFrame.IsMain 以确定 V8Context 来自主框架
            if (frame.IsMain)
            {
                //const string script = "document.addEventListener('DOMContentLoaded', function(){ alert('DomLoaded'); });";

                //frame.ExecuteJavaScriptAsync(script);
            }
        }

        void IRenderProcessMessageHandler.OnContextReleased(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
        {
            //V8Context 即将发布，使用此通知取消您可能拥有的任何长时间运行的任务
        }

        void IRenderProcessMessageHandler.OnUncaughtException(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, JavascriptException exception)
        {
            Console.WriteLine("OnUncaughtException() - " + exception.Message);
        }
    }
}
