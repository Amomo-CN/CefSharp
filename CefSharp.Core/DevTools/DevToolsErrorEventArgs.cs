//版权所有 © 2021 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;

namespace CefSharp.DevTools
{
    /// <summary>
    ///DevToolsErrorEventArgs -当发生异常时引发
    ///尝试引发 <see cref="IDevToolsClient.DevToolsEvent"/>
    /// </summary>
    public class DevToolsErrorEventArgs : EventArgs
    {
        /// <summary>
        /// 活动名称
        /// </summary>
        public string EventName { get; private set; }

        /// <summary>
        /// Json
        /// </summary>
        public string Json { get; private set; }

        /// <summary>
        /// 例外
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// 开发工具错误事件参数
        ///</摘要>
        ///<param name="eventName">事件名称</param>
        ///<param name="json">json</param>
        ///<param name="ex">异常</param>
        public DevToolsErrorEventArgs(string eventName, string json, Exception ex)
        {
            EventName = eventName;
            Json = json;
            Exception = ex;
        }
    }
}
