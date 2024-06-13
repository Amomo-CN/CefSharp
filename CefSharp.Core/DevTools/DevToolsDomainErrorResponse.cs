//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System.Runtime.Serialization;

namespace CefSharp.DevTools
{
    /// <summary>
    /// Error Message parsed from JSON
    /// e.g. {"code":-32601,"message":"'Browser.getWindowForTarget' wasn't found"}
    /// </summary>
    [DataContract]
    public class DevToolsDomainErrorResponse
    {
        /// <summary>
        /// 消息编号
        /// </summary>
        [IgnoreDataMember]
        public int MessageId { get; set; }

        /// <summary>
        /// 错误代码
        /// </summary>
        [DataMember(Name = "code", IsRequired = true)]
        public int Code
        {
            get;
            set;
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        [DataMember(Name = "message", IsRequired = true)]
        public string Message
        {
            get;
            set;
        }
    }
}
