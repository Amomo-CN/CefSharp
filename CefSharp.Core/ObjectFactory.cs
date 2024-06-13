//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

//注意：CefSharp.Core 命名空间中的类已对 intellisnse 隐藏，因此用户不会直接使用它们

using System;

namespace CefSharp.Core
{
    /// <summary>
    ///创建公共 Api 类的实例，<see cref="IBrowserSettings"/>,
    ///<参见 cref="IWindowInfo"/> 等
    /// </summary>
    public static class ObjectFactory
    {
        public static readonly Type BrowserSetingsType = typeof(CefSharp.Core.BrowserSettings);
        public static readonly Type RequestContextType = typeof(CefSharp.Core.RequestContext);

        /// <summary>
        ///创建 <see cref="IBrowserSettings"/> 的新实例
        ///</摘要>
        ///<param name="autoDispose">在创建浏览器后释放浏览器设置</param>
        ///<returns>返回<see cref="IBrowserSettings"/>的新实例</returns>
        public static IBrowserSettings CreateBrowserSettings(bool autoDispose)
        {
            return new CefSharp.Core.BrowserSettings(autoDispose);
        }

        /// <summary>
        ///创建 <see cref="IWindowInfo"/> 的新实例
        ///</摘要>
        ///<returns>返回 <see cref="IWindowInfo"/> 的新实例</returns>
        public static IWindowInfo CreateWindowInfo()
        {
            return new CefSharp.Core.WindowInfo();
        }

        /// <summary>
        ///创建 <see cref="IPostData"/> 的新实例
        ///</摘要>
        ///<returns>返回<see cref="IPostData"/>的新实例</returns>
        public static IPostData CreatePostData()
        {
            return new CefSharp.Core.PostData();
        }

        /// <summary>
        ///创建 <see cref="IPostDataElement"/> 的新实例
        ///</摘要>
        ///<returns>返回<see cref="IPostDataElement"/>的新实例</returns>
        public static IPostDataElement CreatePostDataElement()
        {
            return new CefSharp.Core.PostDataElement();
        }

        /// <summary>
        ///创建 <see cref="IRequest"/> 的新实例
        ///</摘要>
        ///<returns>返回<see cref="IRequest"/>的新实例</returns>
        public static IRequest CreateRequest()
        {
            return new CefSharp.Core.Request();
        }

        /// <summary>
        ///创建 <see cref="IUrlRequest"/> 的新实例
        ///</摘要>
        ///<param name="request">请求</param>
        ///<param name="urlRequestClient">url请求客户端</param>
        ///<returns>返回<see cref="IUrlRequest"/>的新实例</returns>
        public static IUrlRequest CreateUrlRequest(IRequest request, IUrlRequestClient urlRequestClient)
        {
            return new CefSharp.Core.UrlRequest(request, urlRequestClient);
        }

        /// <summary>
        ///创建 <see cref="IUrlRequest"/> 的新实例
        ///</摘要>
        ///<param name="request">请求</param>
        ///<param name="urlRequestClient">url请求客户端</param>
        ///<param name="requestContext">请求上下文</param>
        ///<returns>返回<see cref="IUrlRequest"/>的新实例</returns>
        public static IUrlRequest CreateUrlRequest(IRequest request, IUrlRequestClient urlRequestClient, IRequestContext requestContext)
        {
            return new CefSharp.Core.UrlRequest(request, urlRequestClient, requestContext);
        }

        /// <summary>
        ///创建 <see cref="IDragData"/> 的新实例
        ///</摘要>
        ///<returns>返回<see cref="IDragData"/>的新实例</returns>
        public static IDragData CreateDragData()
        {
            return Core.DragData.Create();
        }

        /// <summary>
        ///创建一个新的 <see cref="RequestContextBuilder"/> 可用于
        ///以流畅的闪烁创建一个新的 <see cref="IRequestContext"/>。
        ///调用 <see cref="RequestContextBuilder.Create"/> 创建实际的
        ///<see cref="IRequestContext"/> 实例
        ///</摘要>
        ///<returns>RequestContextBuilder</returns>
        public static RequestContextBuilder ConfigureRequestContext()
        {
            return new RequestContextBuilder();
        }
    }
}
