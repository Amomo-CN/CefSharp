//版权所有 © 2021 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

//最初基于https://github.com/CefNet/CefNet.DevTools.Protocol/blob/0a124720474a469b5cef03839418f5e1debaf2f0/CefNet.DevTools.Protocol/IEventProxy.cs

using System;
using System.IO;
using System.Threading;

namespace CefSharp.DevTools
{
    /// <summary>
    /// 事件代理
    /// </summary>
    internal interface IEventProxy : IDisposable
    {
        ///<摘要>
        ///引发事件
        ///</摘要>
        ///<param name="sender">发件人</param>
        ///<param name="eventName">事件名称</param>
        ///<param name="stream">包含 JSON 的流</param>
        ///<param name="syncContext">SynchronizationContext</param>
        void Raise(object sender, string eventName, Stream stream, SynchronizationContext syncContext);
    }
}
