// 版权所有 © 2020 CefSharp 作者。版权所有。
//
// 此源代码的使用受 BSD 样式许可证的约束，该许可证可在 LICENSE 文件中找到。

//注意：CefSharp.Core 命名空间中的类已对 intellisnse 隐藏，因此用户不会直接使用它们

namespace CefSharp
{
    /// <inheritdoc/>
    public class BrowserSettings : CefSharp.Core.BrowserSettings
    {
        /// <inheritdoc/>
        public BrowserSettings(bool autoDispose = false) : base(autoDispose)
        {

        }

        /// <summary>
        ///创建 <see cref="IBrowserSettings"/> 的新实例
        ///</摘要>
        ///<param name="autoDispose">如果您打算重用实例，则设置为 false，否则设置为 true</param>
        ///<returns>浏览器设置</returns>
        public static IBrowserSettings Create(bool autoDispose = false)
        {
            return new CefSharp.Core.BrowserSettings(autoDispose);
        }
    }
}
