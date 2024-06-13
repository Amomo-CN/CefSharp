//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

//注意：CefSharp.Core 命名空间中的类已对 intellisnse 隐藏，因此用户不会直接使用它们

using System.Collections.Generic;

namespace CefSharp
{
    /// <inheritdoc/>
    public class PostData : IPostData
    {
        private CefSharp.Core.PostData postData = new CefSharp.Core.PostData();

        /// <inheritdoc/>
        public IList<IPostDataElement> Elements
        {
            get { return postData.Elements; }
        }

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get { return postData.IsReadOnly; }
        }

        /// <inheritdoc/>
        public bool IsDisposed
        {
            get { return postData.IsDisposed; }
        }

        /// <inheritdoc/>
        public bool HasExcludedElements
        {
            get { return postData.HasExcludedElements; }
        }

        /// <inheritdoc/>
        public bool AddElement(IPostDataElement element)
        {
            return postData.AddElement(element);
        }

        /// <inheritdoc/>
        public IPostDataElement CreatePostDataElement()
        {
            return new CefSharp.Core.PostDataElement();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            postData.Dispose();
        }

        /// <inheritdoc/>
        public bool RemoveElement(IPostDataElement element)
        {
            return postData.RemoveElement(element);
        }

        /// <inheritdoc/>
        public void RemoveElements()
        {
            postData.RemoveElements();
        }

        ///<摘要>
        ///在内部用于获取底层 <see cref="IPostData"/> 实例。
        ///您不太可能自己使用它。
        ///</摘要>
        ///<returns>最里面的实例</returns>
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public IPostData UnWrap()
        {
            return postData;
        }

        ///<摘要>
        ///创建 <see cref="IPostData"/> 的新实例
        ///</摘要>
        ///<returns>PostData</returns>
        public static IPostData Create()
        {
            return new CefSharp.Core.PostData();
        }
    }
}
