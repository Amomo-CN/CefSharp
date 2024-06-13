//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Runtime.Serialization;

namespace CefSharp.DevTools
{
    /// <summary>
    /// 执行 DevTools 协议方法出现问题时引发的异常。
    /// </summary>
    [Serializable]
    public class DevToolsClientException : Exception
    {
        /// <summary>
        /// Get the Error Response
        /// </summary>
        public DevToolsDomainErrorResponse Response
        {
            get; private set;
        }

        /// <summary>
        /// 使用其消息初始化 <see cref="DevToolsClientException"/> 类的新实例
        ///字符串设置为默认消息。
        /// </summary>
        public DevToolsClientException() : base("Error occurred whilst executing DevTools protocol method")
        {
        }

        /// <summary>
        ///使用指定的错误消息初始化 <see cref="DevToolsClientException"/> 类的新实例。
        /// </summary>
        /// <param name="message">信息</param>
        public DevToolsClientException(string message) : base(message)
        {
        }

        /// <summary>
        /// 使用指定的错误消息初始化 <see cref="DevToolsClientException"/> 类的新实例。
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="errorResponse">错误响应</param>
        public DevToolsClientException(string message, DevToolsDomainErrorResponse errorResponse) : base(message)
        {
            Response = errorResponse;
        }

        /// <summary>
        /// 使用指定的错误消息初始化 <see cref="DevToolsClientException"/> 类的新实例
        ///和一个内部异常。
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="inner">内部异常</param>
        public DevToolsClientException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <inheritdoc/>
        protected DevToolsClientException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {

        }
    }
}
