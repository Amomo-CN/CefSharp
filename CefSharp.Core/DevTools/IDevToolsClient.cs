//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CefSharp.DevTools
{
    /// <summary>
    /// 开发工具客户端
    /// </summary>
    public interface IDevToolsClient : IDisposable
    {
        /// <summary>
        /// 将在收到 DevTools 协议事件时调用。事件默认是禁用的，需要
        ///在每个域的基础上启用，例如发送 Network.enable （或调用 <see cref="Network.NetworkClient.EnableAsync(int?, int?, int?)"/>）
        ///启用网络相关事件。
        /// </summary>
        event EventHandler<DevToolsEventArgs> DevToolsEvent;

        /// <summary>
        /// 当尝试引发错误时将被调用 <see cref="DevToolsEvent"/>
        /// </summary>
        event EventHandler<DevToolsErrorEventArgs> DevToolsEventError;

        /// <summary>
        ///为 DevTools 协议事件添加事件处理程序。事件默认是禁用的，需要
        ///在每个域的基础上启用，例如发送 Network.enable （或调用 <see cref="Network.NetworkClient.EnableAsync(int?, int?, int?)"/>）
        ///启用网络相关事件。
        ///</摘要>
        ///<typeparam name="T">事件将被反序列化到的事件参数类型。</typeparam>
        ///<param name="eventName">是要监听的事件名称</param>
        ///<param name="eventHandler">事件发生时调用的事件处理程序</param>
        void AddEventHandler<T>(string eventName, EventHandler<T> eventHandler) where T : EventArgs;

        /// <summary>
        /// 删除 DevTools 协议事件的事件处理程序。
        ///</摘要>
        ///<typeparam name="T">事件将被反序列化到的事件参数类型。</typeparam>
        ///<param name="eventName">是要监听的事件名称</param>
        ///<param name="eventHandler">事件发生时调用的事件处理程序</param>
        ///<返回>
        ///如果 <paramref name="eventName"/> 的所有处理程序都已被删除，则返回 false，
        ///否则，如果仍有已注册的处理程序，则返回 true。
        /// </returns>
        bool RemoveEventHandler<T>(string eventName, EventHandler<T> eventHandler) where T : EventArgs;

        /// <summary>
        /// 通过 DevTools 协议执行方法调用。该方法可以在任何线程上调用。
        ///有关详细信息，请参阅 https://chromedevtools.github.io/devtools-protocol/上的 DevTools 协议文档
        ///支持的方法和预期的 <paramref name="parameters"/> 字典内容。
        ///</摘要>
        ///<typeparam name="T">方法结果将被反序列化到的类型。</typeparam>
        ///<param name="method">是方法名称</param>
        ///<param name="parameters">是以字典形式表示的方法参数，
        ///可能为空。</param>
        ///<returns>返回一个可以等待获取方法结果的Task</returns>
        Task<T> ExecuteDevToolsMethodAsync<T>(string method, IDictionary<string, object> parameters = null) where T : DevToolsDomainResponseBase;
    }
}
