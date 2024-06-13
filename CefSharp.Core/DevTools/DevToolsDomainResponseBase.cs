//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

namespace CefSharp.DevTools
{
    /// <summary>
    /// 开发工具域响应库
    /// </summary>
    [System.Runtime.Serialization.DataContractAttribute]
    public abstract class DevToolsDomainResponseBase
    {
        /// <summary>
        ///从字符串转换为base64字节数组
        ///</摘要>
        ///<param name="data">字符串数据</param>
        ///<returns>字节数组</returns>
        public byte[] Convert(string data)
        {
            return System.Convert.FromBase64String(data);
        }
    }
}
