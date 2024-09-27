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
                throw new Exception("A call to WithCachePath has already been made, it's not possible to share settings with another RequestContext.");
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
        /// 设置 RequestContext 时与首选项名称关联的值
        ///已初始化。如果值为空，则首选项将恢复为原来的状态
        ///默认值。如果设置首选项失败，则不会抛出任何错误，您
        ///必须检查 CEF 日志文件。
        ///通过命令行设置的首选项通常无法修改。
        ///</摘要>
        ///<param name="name">偏好键</param>
        ///<param name="value">偏好值</param>
        ///<returns>返回RequestContextBuilder实例</returns>
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
        ///当RequestContext初始化时设置代理服务器。
        ///如果值为 null，则首选项将恢复到原来的状态
        ///默认值。如果设置首选项失败，则不会抛出任何错误，您
        ///必须检查 CEF 日志文件。
        ///通过命令行设置的代理无法修改。
        ///</摘要>
        ///<param name="host">代理主机</param>
        ///<returns>返回RequestContextBuilder实例</returns>
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
        /// 当RequestContext初始化时设置代理服务器。
        ///如果值为 null，则首选项将恢复到原来的状态
        ///默认值。如果设置首选项失败，则不会抛出任何错误，您
        ///必须检查 CEF 日志文件。
        ///通过命令行设置的代理无法修改。
        ///</摘要>
        ///<param name="host">代理主机</param>
        ///<param name="port">代理端口（可选）</param>
        ///<returns>返回RequestContextBuilder实例</returns>
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
        ///当RequestContext初始化时设置代理服务器。
        ///如果值为 null，则首选项将恢复到原来的状态
        ///默认值。如果设置首选项失败，则不会抛出任何错误，您
        ///必须检查 CEF 日志文件。
        ///通过命令行设置的代理无法修改。
        ///</摘要>
        ///<param name="scheme">代理方案</param>
        ///<param name="host">代理主机</param>
        ///<param name="port">代理端口（可选）</param>
        ///<returns>返回RequestContextBuilder实例</returns>
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
        /// 与其他RequestContext共享存储
        ///</摘要>
        ///<param name="other">与此RequestContext共享存储</param>
        /// <returns>返回 RequestContextBuilder 实例</returns>
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
