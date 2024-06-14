//版权所有 © 2015 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;

namespace CefSharp.Example
{
    /// <summary>
    /// 用于演示如何将异步 JavaScript 事件返回到 .Net 运行时环境的类。
    /// </summary>
    /// <seealso cref="ScriptedMethods"/>
    /// <seealso cref="resources/ScriptedMethodsTest.html"/>
    public class ScriptedMethodsBoundObject
    {
        /// <summary>
        /// 当 Javascript 事件到达时引发。
        /// </summary>
        public event Action<string, object> EventArrived;

        /// <summary>
        /// 该方法将暴露在Javascript环境中。这是
        ///当某些感兴趣的事件发生时在 Javascript 环境中调用
        ///发生。
        /// </summary>
        /// <param name="eventName">事件的名称。</param>
        /// <param name="eventData">调用者提供的与事件相关的数据。</param>
        /// <备注>
        /// 默认情况下 RaiseEvent 将被转换为 raiseEvent 作为 JavaScript 函数。
        /// 这是通过设置camelCaseJavascriptNames调用RegisterJsObject时可配置的；
        /// </remarks>
        public void RaiseEvent(string eventName, object eventData = null)
        {
            if (EventArrived != null)
            {
                EventArrived(eventName, eventData);
            }
        }
    }
}
