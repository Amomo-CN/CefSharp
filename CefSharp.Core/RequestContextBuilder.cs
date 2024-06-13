//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

//注意：CefSharp.Core 命名空间中的类已对 intellisnse 隐藏，因此用户不会直接使用它们

using CefSharp.Handler;
using System;

namespace CefSharp
{
    /// <summary>
    ///用于创建 IRequestContext 实例的流畅样式构建器。
    /// </summary>
    public class RequestContextBuilder
    {
        private RequestContextSettings settings;
        private IRequestContext otherContext;
        private RequestContextHandler handler;

        void ThrowExceptionIfContextAlreadySet()
        {
            if (otherContext != null)
            {
                throw new Exception("A call to WithSharedSettings has already been made, it is no possible to provide custom settings.");
            }
        }

        void ThrowExceptionIfCustomSettingSpecified()
        {
            if (settings != null)
            {
                throw new Exception("A call to WithCachePath/PersistUserPreferences has already been made, it's not possible to share settings with another RequestContext.");
            }
        }
        ///<摘要>
        ///创建实际的RequestContext实例
        ///</摘要>
        ///<returns>返回一个新的RequestContext实例。</returns>
        public IRequestContext Create()
        {
            if (otherContext != null)
            {
                return new CefSharp.Core.RequestContext(otherContext, handler);
            }

            if (settings != null)
            {
                return new CefSharp.Core.RequestContext(settings.settings, handler);
            }

            if (handler != null)
            {
                return new CefSharp.Core.RequestContext(handler);
            }

            return new CefSharp.Core.RequestContext();
        }

        /// <summary>
        ///Action在IRequestContextHandler.OnRequestContextInitialized中被调用
        ///</摘要>
        ///<param name="action">当上下文初始化时调用。</param>
        ///<returns>返回RequestContextBuilder实例</returns>      
        public RequestContextBuilder OnInitialize(Action<IRequestContext> action)
        {
            if (handler == null)
            {
                handler = new RequestContextHandler();
            }

            handler.OnInitialize(action);

            return this;
        }

        ///<摘要>
        ///设置缓存路径
        ///</摘要>
        ///<参数名称=“缓存路径”>
        ///此请求上下文的缓存数据将存储的位置
        ///磁盘。如果该值非空，则它必须是绝对路径
        ///等于 CefSettings.RootCachePath 或其子目录。
        ///如果该值为空，则浏览器将以“隐身模式”创建
        ///其中内存缓存用于存储，并且没有数据持久保存到磁盘。
        ///HTML5 数据库（例如 localStorage）仅在以下情况下才会跨会话持久存在：
        ///指定缓存路径。共享全局浏览器缓存及相关
        ///配置设置此值以匹配 CefSettings.CachePath 值。
        ///</参数>
        ///<returns>返回RequestContextBuilder实例</returns>
        public RequestContextBuilder WithCachePath(string cachePath)
        {
            ThrowExceptionIfContextAlreadySet();

            if (settings == null)
            {
                settings = new RequestContextSettings();
            }

            settings.CachePath = cachePath;

            return this;
        }

        /// <summary>
        /// Invoke this method tp persist user preferences as a JSON file in the cache path directory.
        /// Can be set globally using the CefSettings.PersistUserPreferences value.
        /// This value will be ignored if CachePath is empty or if it matches the CefSettings.CachePath value.
        /// </summary>
        /// <returns>Returns RequestContextBuilder instance</returns>
        public RequestContextBuilder PersistUserPreferences()
        {
            ThrowExceptionIfContextAlreadySet();

            if (settings == null)
            {
                settings = new RequestContextSettings();
            }

            settings.PersistUserPreferences = true;

            return this;
        }

        /// <summary>
        /// Set the value associated with preference name when the RequestContext
        /// is initialzied. If value is null the preference will be restored to its
        /// default value. If setting the preference fails no error is throw, you
        /// must check the CEF Log file.
        /// Preferences set via the command-line usually cannot be modified.
        /// </summary>
        /// <param name="name">preference key</param>
        /// <param name="value">preference value</param>
        /// <returns>Returns RequestContextBuilder instance</returns>
        public RequestContextBuilder WithPreference(string name, object value)
        {
            if (handler == null)
            {
                handler = new RequestContextHandler();
            }

            handler.SetPreferenceOnContextInitialized(name, value);

            return this;
        }

        /// <summary>
        /// Set the Proxy server when the RequestContext is initialzied.
        /// If value is null the preference will be restored to its
        /// default value. If setting the preference fails no error is throw, you
        /// must check the CEF Log file.
        /// Proxy set via the command-line cannot be modified.
        /// </summary>
        /// <param name="host">proxy host</param>
        /// <returns>Returns RequestContextBuilder instance</returns>
        public RequestContextBuilder WithProxyServer(string host)
        {
            if (handler == null)
            {
                handler = new RequestContextHandler();
            }

            handler.SetProxyOnContextInitialized(host, null);

            return this;
        }

        /// <summary>
        /// Set the Proxy server when the RequestContext is initialzied.
        /// If value is null the preference will be restored to its
        /// default value. If setting the preference fails no error is throw, you
        /// must check the CEF Log file.
        /// Proxy set via the command-line cannot be modified.
        /// </summary>
        /// <param name="host">proxy host</param>
        /// <param name="port">proxy port (optional)</param>
        /// <returns>Returns RequestContextBuilder instance</returns>
        public RequestContextBuilder WithProxyServer(string host, int? port)
        {
            if (handler == null)
            {
                handler = new RequestContextHandler();
            }

            handler.SetProxyOnContextInitialized(host, port);

            return this;
        }

        /// <summary>
        /// Set the Proxy server when the RequestContext is initialzied.
        /// If value is null the preference will be restored to its
        /// default value. If setting the preference fails no error is throw, you
        /// must check the CEF Log file.
        /// Proxy set via the command-line cannot be modified.
        /// </summary>
        /// <param name="scheme">proxy scheme</param>
        /// <param name="host">proxy host</param>
        /// <param name="port">proxy port (optional)</param>
        /// <returns>Returns RequestContextBuilder instance</returns>
        public RequestContextBuilder WithProxyServer(string scheme, string host, int? port)
        {
            if (handler == null)
            {
                handler = new RequestContextHandler();
            }

            handler.SetProxyOnContextInitialized(scheme, host, port);

            return this;
        }

        /// <summary>
        /// Shares storage with other RequestContext
        /// </summary>
        /// <param name="other">shares storage with this RequestContext</param>
        /// <returns>Returns RequestContextBuilder instance</returns>
        public RequestContextBuilder WithSharedSettings(IRequestContext other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            ThrowExceptionIfCustomSettingSpecified();

            otherContext = other;

            return this;
        }
    }
}
