//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

//注意：CefSharp.Core 命名空间中的类已对 intellisnse 隐藏，因此用户不会直接使用它们

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CefSharp
{
    /// <inheritdoc/>
    public class RequestContext : IRequestContext
    {
        private CefSharp.Core.RequestContext requestContext;

        /// <inheritdoc/>
        public RequestContext()
        {
            requestContext = new CefSharp.Core.RequestContext();
        }

        /// <inheritdoc/>
        public RequestContext(IRequestContext otherRequestContext)
        {
            if (otherRequestContext == null)
            {
                throw new ArgumentNullException(nameof(otherRequestContext));
            }
            requestContext = new CefSharp.Core.RequestContext(otherRequestContext);
        }

        /// <inheritdoc/>
        public RequestContext(IRequestContext otherRequestContext, IRequestContextHandler requestContextHandler)
        {
            if (otherRequestContext == null)
            {
                throw new ArgumentNullException(nameof(otherRequestContext));
            }

            if (requestContextHandler == null)
            {
                throw new ArgumentNullException(nameof(requestContextHandler));
            }

            requestContext = new CefSharp.Core.RequestContext(otherRequestContext, requestContextHandler);
        }

        /// <inheritdoc/>
        public RequestContext(IRequestContextHandler requestContextHandler)
        {
            if (requestContextHandler == null)
            {
                throw new ArgumentNullException(nameof(requestContextHandler));
            }
            requestContext = new CefSharp.Core.RequestContext(requestContextHandler);
        }

        /// <inheritdoc/>
        public RequestContext(RequestContextSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            requestContext = new CefSharp.Core.RequestContext(settings.settings);
        }

        /// <inheritdoc/>
        public RequestContext(RequestContextSettings settings, IRequestContextHandler requestContextHandler)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (requestContextHandler == null)
            {
                throw new ArgumentNullException(nameof(requestContextHandler));
            }

            requestContext = new CefSharp.Core.RequestContext(settings.settings, requestContextHandler);
        }

        ///<摘要>
        ///创建一个新的RequestContextBuilder，可用于流畅地设置
        ///优先
        ///</摘要>
        ///<returns>返回一个新的RequestContextBuilder</returns>
        public static RequestContextBuilder Configure()
        {
            var builder = new RequestContextBuilder();

            return builder;
        }

        /// <inheritdoc/>
        public bool IsGlobal
        {
            get { return requestContext.IsGlobal; }
        }

        /// <inheritdoc/>
        public string CachePath
        {
            get { return requestContext.CachePath; }
        }

        /// <inheritdoc/>
        public bool IsDisposed
        {
            get { return requestContext.IsDisposed; }
        }

        /// <inheritdoc/>
        public bool IsSame(IRequestContext context)
        {
            return requestContext.IsSame(context);
        }

        /// <inheritdoc/>
        public bool IsSharingWith(IRequestContext context)
        {
            return requestContext.IsSharingWith(context);
        }

        /// <inheritdoc/>
        public ICookieManager GetCookieManager(ICompletionCallback callback)
        {
            return requestContext.GetCookieManager(callback);
        }

        /// <inheritdoc/>
        public bool RegisterSchemeHandlerFactory(string schemeName, string domainName, ISchemeHandlerFactory factory)
        {
            return requestContext.RegisterSchemeHandlerFactory(schemeName, domainName, factory);
        }

        /// <inheritdoc/>
        public bool ClearSchemeHandlerFactories()
        {
            return requestContext.ClearSchemeHandlerFactories();
        }

        /// <inheritdoc/>
        public bool HasPreference(string name)
        {
            return requestContext.HasPreference(name);
        }

        /// <inheritdoc/>
        public object GetPreference(string name)
        {
            return requestContext.GetPreference(name);
        }

        /// <inheritdoc/>
        public IDictionary<string, object> GetAllPreferences(bool includeDefaults)
        {
            return requestContext.GetAllPreferences(includeDefaults);
        }

        /// <inheritdoc/>
        public bool CanSetPreference(string name)
        {
            return requestContext.CanSetPreference(name);
        }

        /// <inheritdoc/>
        public bool SetPreference(string name, object value, out string error)
        {
            return requestContext.SetPreference(name, value, out error);
        }

        /// <inheritdoc/>
        public void ClearCertificateExceptions(ICompletionCallback callback)
        {
            requestContext.ClearCertificateExceptions(callback);
        }

        /// <inheritdoc/>
        public void ClearHttpAuthCredentials(ICompletionCallback callback = null)
        {
            requestContext.ClearHttpAuthCredentials(callback);
        }

        /// <inheritdoc/>
        public void CloseAllConnections(ICompletionCallback callback)
        {
            requestContext.CloseAllConnections(callback);
        }

        /// <inheritdoc/>
        public Task<ResolveCallbackResult> ResolveHostAsync(Uri origin)
        {
            return requestContext.ResolveHostAsync(origin);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            requestContext.Dispose();
        }

        ///<摘要>
        ///在内部用于获取底层 <see cref="IRequestContext"/> 实例。
        ///您不太可能自己使用它。
        ///</摘要>
        ///<returns>最里面的实例</returns>
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public IRequestContext UnWrap()
        {
            return requestContext;
        }
    }
}
