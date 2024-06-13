// Copyright © 2016 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

namespace CefSharp.Example.Handlers
{
    public class RequestContextHandler : CefSharp.Handler.RequestContextHandler
    {
        protected override IResourceRequestHandler GetResourceRequestHandler(IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            // 对于默认行为返回 null
            //return null;

            //在 RequestContext 级别处理资源请求
            //实现CefSharp.IResourceRequestHandler或者继承CefSharp.Handler.ResourceRequestHandler
            return new ExampleResourceRequestHandler();
        }

        protected override void OnRequestContextInitialized(IRequestContext requestContext)
        {
            // 您可以在此处为新初始化的请求上下文设置首选项。
            //注意，这里是在CEF UI Thread上调用的，所以可以直接调用SetPreference

            //使用它来检查设置首选项是否在您的代码中起作用
            //您应该看到最小字体大小现在为 24pt
            //string errorMessage;
            //var success = requestContext.SetPreference("webkit.webprefs.minimum_font_size", 24, out errorMessage);

            // 这是设置代理的首选位置，因为它在发出第一个请求之前被调用，
            //确保所有请求都通过指定的代理
            //string errorMessage;
            //bool success = requestContext.SetProxy("http://localhost:8080", out errorMessage); 
        }
    }
}
