//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

//注意：CefSharp.Core 命名空间中的类已对 intellisnse 隐藏，因此用户不会直接使用它们

using CefSharp.Internals;

namespace CefSharp
{
    ///<摘要>
    ///通过 <see cref="Create(IWebBrowserInternal, bool)"/> 创建 <see cref="IBrowserAdapter"/> 实例
    ///这是桥接 ChromiumWebBrowser 实现和 VC++ 的主要对象
    ///</摘要>
    public static class ManagedCefBrowserAdapter
    {
        /// <summary>
        ///创建一个新的<see cref="IBrowserAdapter"/>实例，它是unmanged之间交互的主要方法
        ///CEF 实现和我们的 ChromiumWebBrowser 实例。
        ///</摘要>
        ///<param name="webBrowserInternal">对 ChromiumWebBrowser 实例的引用</param>
        ///<param name="offScreenRendering">对于 WPF/OffScreen 为 true，对于 WinForms 和其他基于 Hwnd 的实现为 false</param>
        ///<returns><see cref="IBrowserAdapter"/> 的实例</returns>
        public static IBrowserAdapter Create(IWebBrowserInternal webBrowserInternal, bool offScreenRendering)
        {
            return new CefSharp.Core.ManagedCefBrowserAdapter(webBrowserInternal, offScreenRendering);
        }
    }
}
