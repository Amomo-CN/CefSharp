//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

//注意：CefSharp.Core 命名空间中的类已对 intellisnse 隐藏，因此用户不会直接使用它们

namespace CefSharp
{
    /// <inheritdoc/>
    public class UrlRequest : CefSharp.Core.UrlRequest
    {
        /// <inheritdoc/>
        public UrlRequest(IRequest request, IUrlRequestClient urlRequestClient) : base(request, urlRequestClient)
        {
        }

        /// <inheritdoc/>
        public UrlRequest(IRequest request, IUrlRequestClient urlRequestClient, IRequestContext requestContext) : base(request, urlRequestClient, requestContext)
        {
        }

        /// <summary>
        ///创建一个不与特定浏览器或框架关联的新 URL 请求。
        ///如果您想要，请使用 <see cref="IFrame.CreateUrlRequest(IRequest, IUrlRequestClient)"/>
        ///请求具有此关联，在这种情况下可能会有不同的处理方式。
        ///对于来自浏览器进程的请求：可能会被客户端通过 <see cref="IResourceRequestHandler"/> 或 <see cref="ISchemeHandlerFactory"/> 拦截。
        ///POST 数据只能包含 PDE_TYPE_FILE 或 PDE_TYPE_BYTES 类型的单个元素。
        ///使用全局RequestContext
        ///</摘要>
        ///<param name="request">请求</param>
        ///<param name="urlRequestClient">url请求客户端</param>
        public IUrlRequest Create(IRequest request, IUrlRequestClient urlRequestClient)
        {
            return new CefSharp.Core.UrlRequest(request, urlRequestClient);
        }

        /// <summary>
        ///创建一个不与特定浏览器或框架关联的新 URL 请求。
        ///如果您想要，请使用 <see cref="IFrame.CreateUrlRequest(IRequest, IUrlRequestClient)"/>
        ///请求具有此关联，在这种情况下可能会有不同的处理方式。
        ///对于来自浏览器进程的请求：可能会被客户端通过 <see cref="IResourceRequestHandler"/> 或 <see cref="ISchemeHandlerFactory"/> 拦截。
        ///POST 数据只能包含 PDE_TYPE_FILE 或 PDE_TYPE_BYTES 类型的单个元素。
        ///</摘要>
        ///<param name="request">请求</param>
        ///<param name="urlRequestClient">url请求客户端</param>
        ///<param name="requestContext">与此请求关联的请求上下文。</param>
        public IUrlRequest Create(IRequest request, IUrlRequestClient urlRequestClient, IRequestContext requestContext)
        {
            return new CefSharp.Core.UrlRequest(request, urlRequestClient, requestContext);
        }
    }
}
