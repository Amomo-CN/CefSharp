//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

//注意：CefSharp.Core 命名空间中的类已对 intellisnse 隐藏，因此用户不会直接使用它们
namespace CefSharp
{
    ///<摘要>
    ///用于表示请求发布数据中的单个元素的类。
    ///此类的方法可以在任何线程上调用。
    /// </summary>
    public class PostDataElement : IPostDataElement
    {
        internal CefSharp.Core.PostDataElement postDataElement = new CefSharp.Core.PostDataElement();

        /// <inheritdoc/>
        public string File
        {
            get { return postDataElement.File; }
            set { postDataElement.File = value; }
        }

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get { return postDataElement.IsReadOnly; }
        }

        /// <inheritdoc/>
        public PostDataElementType Type
        {
            get { return postDataElement.Type; }
        }

        /// <inheritdoc/>
        public byte[] Bytes
        {
            get { return postDataElement.Bytes; }
            set { postDataElement.Bytes = value; }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            postDataElement.Dispose();
        }

        /// <inheritdoc/>
        public void SetToEmpty()
        {
            postDataElement.SetToEmpty();
        }

        ///<摘要>
        ///在内部用于获取底层 <see cref="IPostDataElement"/> 实例。
        ///您不太可能自己使用它。
        ///</摘要>
        ///<returns>最里面的实例</returns>
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public IPostDataElement UnWrap()
        {
            return postDataElement;
        }

        ///<摘要>
        ///创建 <see cref="IPostDataElement"/> 的新实例
        ///</摘要>
        ///<returns>PostDataElement</returns>
        public static IPostDataElement Create()
        {
            return new CefSharp.Core.PostDataElement();
        }
    }
}
