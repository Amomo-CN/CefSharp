//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

namespace CefSharp.DevTools
{
    /// <summary>
    /// DevTools 方法响应
    /// </summary>
    public class DevToolsMethodResponse : DevToolsDomainResponseBase
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// 成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 方法响应为 Json 字符串
        /// </summary>
        public string ResponseAsJsonString { get; set; }
    }
}
