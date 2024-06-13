//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

//注意：CefSharp.Core 命名空间中的类已对 intellisnse 隐藏，因此用户不会直接使用它们

namespace CefSharp
{
    /// <inheritdoc/>
    public class Request : CefSharp.Core.Request
    {
        ///<摘要>
        ///创建一个新的 <see cref="IRequest"/> 实例
        ///</摘要>
        ///<returns>请求</returns>
        public static IRequest Create()
        {
            return new CefSharp.Core.Request();
        }
    }
}
