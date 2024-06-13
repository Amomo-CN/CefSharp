//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

//注意：CefSharp.Core 命名空间中的类已对 intellisnse 隐藏，因此用户不会直接使用它们

using System.Collections.Specialized;

namespace CefSharp
{
    /// <inheritdoc/>
    public class Request : IRequest
    {
        internal Core.Request request = new Core.Request();

        /// <inheritdoc/>
        public UrlRequestFlags Flags
        {
            get { return request.Flags; }
            set { request.Flags = value; }
        }

        /// <inheritdoc/>
        public string Url
        {
            get { return request.Url; }
            set { request.Url = value; }
        }

        /// <inheritdoc/>
        public ulong Identifier
        {
            get { return request.Identifier; }
        }

        /// <inheritdoc/>
        public string Method
        {
            get { return request.Method; }
            set { request.Method = value; }
        }

        /// <inheritdoc/>
        public string ReferrerUrl
        {
            get { return request.ReferrerUrl; }
        }

        /// <inheritdoc/>
        public ResourceType ResourceType
        {
            get { return request.ResourceType; }
        }

        /// <inheritdoc/>
        public ReferrerPolicy ReferrerPolicy
        {
            get { return request.ReferrerPolicy; }
        }

        /// <inheritdoc/>
        public NameValueCollection Headers
        {
            get { return request.Headers; }
            set { request.Headers = value; }
        }

        /// <inheritdoc/>
        public IPostData PostData
        {
            get { return request.PostData; }
            set { request.PostData = value; }
        }

        /// <inheritdoc/>
        public TransitionType TransitionType
        {
            get { return request.TransitionType; }
        }

        /// <inheritdoc/>
        public bool IsDisposed
        {
            get { return request.IsDisposed; }
        }

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get { return request.IsReadOnly; }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            request.Dispose();
        }

        /// <inheritdoc/>
        public string GetHeaderByName(string name)
        {
            return request.GetHeaderByName(name);
        }

        /// <inheritdoc/>
        public void InitializePostData()
        {
            request.InitializePostData();
        }

        /// <inheritdoc/>
        public void SetHeaderByName(string name, string value, bool overwrite)
        {
            request.SetHeaderByName(name, value, overwrite);
        }

        /// <inheritdoc/>
        public void SetReferrer(string referrerUrl, ReferrerPolicy policy)
        {
            request.SetReferrer(referrerUrl, policy);
        }

        ///<摘要>
        ///在内部用于获取底层 <see cref="IRequest"/> 实例。
        ///您不太可能自己使用它。
        ///</摘要>
        ///<returns>最里面的实例</returns>
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public IRequest UnWrap()
        {
            return request;
        }

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
