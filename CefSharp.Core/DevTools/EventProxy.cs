//版权所有 © 2021 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

//最初基于 https://github.com/CefNet/CefNet.DevTools.Protocol/blob/0a124720474a469b5cef03839418f5e1debaf2f0/CefNet.DevTools.Protocol/Internal/EventProxy.T.cs

using System;
using System.IO;
using System.Threading;

namespace CefSharp.DevTools
{
    /// <summary>
    /// 通用类型事件代理
    ///</摘要>
    ///<typeparam name="T">事件参数类型</typeparam>
    internal class EventProxy<T> : IEventProxy
    {
        private event EventHandler<T> handlers;
        private Func<string, Stream, T> convert;

        /// <summary>
        ///构造函数
        ///</摘要>
        ///<param name="convert">用于从流转换为事件参数的委托</param>
        public EventProxy(Func<string, Stream, T> convert)
        {
            this.convert = convert;
        }

        /// <summary>
        /// 添加事件处理程序
        ///</摘要>
        ///<param name="handler">要添加的事件处理程序</param>
        public void AddHandler(EventHandler<T> handler)
        {
            handlers += handler;
        }

        /// <summary>
        /// 删除事件处理程序
        ///</摘要>
        ///<param name="handler">要删除的事件处理程序</param>
        ///<returns>如果此代理的最后一个事件处理程序已被删除，则返回 true.</returns>
        public bool RemoveHandler(EventHandler<T> handler)
        {
            handlers -= handler;

            return handlers == null;
        }

        /// <inheritdoc/>
        public void Raise(object sender, string eventName, Stream stream, SynchronizationContext syncContext)
        {
            stream.Position = 0;

            var args = convert(eventName, stream);

            if (syncContext == null)
            {
                handlers?.Invoke(sender, args);
            }
            else
            {
                syncContext.Post(new SendOrPostCallback(state =>
                {
                    handlers?.Invoke(sender, args);

                }), null);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            handlers = null;
        }
    }
}
