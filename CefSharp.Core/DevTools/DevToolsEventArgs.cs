//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

namespace CefSharp.DevTools
{
    /// <summary>
    /// 开发工具事件 EventAargs
    /// </summary>
    public class DevToolsEventArgs : DevToolsDomainEventArgsBase
    {
        /// <summary>
        /// 活动名称
        /// </summary>
        public string EventName { get; private set; }

        /// <summary>
        ///事件参数为 Json 字符串
        /// </summary>
        public string ParametersAsJsonString { get; private set; }

        public DevToolsEventArgs(string eventName, string paramsAsJsonString)
        {
            EventName = eventName;
            ParametersAsJsonString = paramsAsJsonString;
        }
    }
}
