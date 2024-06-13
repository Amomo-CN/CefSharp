//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

//注意：CefSharp.Core 命名空间中的类已对 intellisnse 隐藏，因此用户不会直接使用它们

namespace CefSharp
{
    /// <inheritdoc/>
    public class RequestContext : CefSharp.Core.RequestContext
    {
        /// <inheritdoc/>
        public RequestContext() : base()
        {
        }

        /// <inheritdoc/>
        public RequestContext(CefSharp.IRequestContext otherRequestContext) : base(otherRequestContext)
        {

        }

        /// <inheritdoc/>
        public RequestContext(CefSharp.IRequestContext otherRequestContext, CefSharp.IRequestContextHandler requestContextHandler) : base(otherRequestContext, requestContextHandler)
        {
        }

        /// <inheritdoc/>
        public RequestContext(CefSharp.IRequestContextHandler requestContextHandler) : base(requestContextHandler)
        {
        }

        /// <inheritdoc/>
        public RequestContext(CefSharp.RequestContextSettings settings) : base(settings.settings)
        {

        }

        /// <inheritdoc/>
        public RequestContext(CefSharp.RequestContextSettings settings, CefSharp.IRequestContextHandler requestContextHandler) : base(settings.settings, requestContextHandler)
        {
        }

        public override void LoadExtension(string rootDirectory, string manifestJson, IExtensionHandler handler)
        {
            if (Cef.CurrentlyOnThread(CefThreadIds.TID_UI))
            {
                base.LoadExtension(rootDirectory, manifestJson, handler);
            }
            else
            {
                Cef.UIThreadTaskFactory.StartNew(() =>
                {
                    base.LoadExtension(rootDirectory, manifestJson, handler);
                });
            }
        }

        /// <summary>
        ///创建一个新的RequestContextBuilder，可用于流畅地设置
        ///优先
        ///</摘要>
        ///<returns>返回一个新的RequestContextBuilder</returns>
        public static RequestContextBuilder Configure()
        {
            var builder = new RequestContextBuilder();

            return builder;
        }
    }
}
