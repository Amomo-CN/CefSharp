//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Collections.Generic;
using System.Threading;

namespace CefSharp.DevTools
{
    // 用于在正确的同步上下文中设置方法响应的帮助程序类
    internal struct DevToolsMethodResponseContext
    {
        public readonly Type Type;
        private readonly Func<object, bool> setResult;
        private readonly Func<Exception, bool> setException;
        private readonly SynchronizationContext syncContext;

        public DevToolsMethodResponseContext(Type type, Func<object, bool> setResult, Func<Exception, bool> setException, SynchronizationContext syncContext)
        {
            Type = type;
            this.setResult = setResult;
            this.setException = setException;
            this.syncContext = syncContext;
        }

        public void SetResult(object result)
        {
            InvokeOnSyncContext(setResult, result);
        }

        public void SetException(Exception ex)
        {
            InvokeOnSyncContext(setException, ex);
        }

        private void InvokeOnSyncContext<T>(Func<T, bool> fn, T value)
        {
            if (syncContext == null || syncContext == SynchronizationContext.Current)
            {
                fn(value);
            }
            else
            {
                // 使用 KeyValuePair 将方法和值传递到回调中以避免捕获委托中的局部变量。
                syncContext.Post(new SendOrPostCallback(state =>
                {
                    var kv = (KeyValuePair<Func<T, bool>, T>)state;
                    kv.Key(kv.Value);
                }), new KeyValuePair<Func<T, bool>, T>(fn, value));
            }
        }
    }
}
