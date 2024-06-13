//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

//注意：CefSharp.Core 命名空间中的类已对 intellisnse 隐藏，因此用户不会直接使用它们

namespace CefSharp
{
    /// <inheritdoc/>
    public class BrowserSettings : IBrowserSettings
    {
        private CefSharp.Core.BrowserSettings settings;

        /// <inheritdoc/>
        public BrowserSettings(bool autoDispose = false)
        {
            settings = new CefSharp.Core.BrowserSettings(autoDispose);
        }

        /// <inheritdoc/>
        public string StandardFontFamily
        {
            get { return settings.StandardFontFamily; }
            set { settings.StandardFontFamily = value; }
        }

        /// <inheritdoc/>
        public string FixedFontFamily
        {
            get { return settings.FixedFontFamily; }
            set { settings.FixedFontFamily = value; }
        }

        /// <inheritdoc/>
        public string SerifFontFamily
        {
            get { return settings.SerifFontFamily; }
            set { settings.SerifFontFamily = value; }
        }

        /// <inheritdoc/>
        public string SansSerifFontFamily
        {
            get { return settings.SansSerifFontFamily; }
            set { settings.SansSerifFontFamily = value; }
        }

        /// <inheritdoc/>
        public string CursiveFontFamily
        {
            get { return settings.CursiveFontFamily; }
            set { settings.CursiveFontFamily = value; }
        }

        /// <inheritdoc/>
        public string FantasyFontFamily
        {
            get { return settings.FantasyFontFamily; }
            set { settings.FantasyFontFamily = value; }
        }

        /// <inheritdoc/>
        public int DefaultFontSize
        {
            get { return settings.DefaultFontSize; }
            set { settings.DefaultFontSize = value; }
        }

        /// <inheritdoc/>
        public int DefaultFixedFontSize
        {
            get { return settings.DefaultFixedFontSize; }
            set { settings.DefaultFixedFontSize = value; }
        }

        /// <inheritdoc/>
        public int MinimumFontSize
        {
            get { return settings.MinimumFontSize; }
            set { settings.MinimumFontSize = value; }
        }

        /// <inheritdoc/>
        public int MinimumLogicalFontSize
        {
            get { return settings.MinimumLogicalFontSize; }
            set { settings.MinimumLogicalFontSize = value; }
        }

        /// <inheritdoc/>
        public string DefaultEncoding
        {
            get { return settings.DefaultEncoding; }
            set { settings.DefaultEncoding = value; }
        }

        /// <inheritdoc/>
        public CefState RemoteFonts
        {
            get { return settings.RemoteFonts; }
            set { settings.RemoteFonts = value; }
        }

        /// <inheritdoc/>
        public CefState Javascript
        {
            get { return settings.Javascript; }
            set { settings.Javascript = value; }
        }

        /// <inheritdoc/>
        public CefState JavascriptCloseWindows
        {
            get { return settings.JavascriptCloseWindows; }
            set { settings.JavascriptCloseWindows = value; }
        }

        /// <inheritdoc/>
        public CefState JavascriptAccessClipboard
        {
            get { return settings.JavascriptAccessClipboard; }
            set { settings.JavascriptAccessClipboard = value; }
        }

        /// <inheritdoc/>
        public CefState JavascriptDomPaste
        {
            get { return settings.JavascriptDomPaste; }
            set { settings.JavascriptDomPaste = value; }
        }

        /// <inheritdoc/>
        public CefState ImageLoading
        {
            get { return settings.ImageLoading; }
            set { settings.ImageLoading = value; }
        }

        /// <inheritdoc/>
        public CefState ImageShrinkStandaloneToFit
        {
            get { return settings.ImageShrinkStandaloneToFit; }
            set { settings.ImageShrinkStandaloneToFit = value; }
        }

        /// <inheritdoc/>
        public CefState TextAreaResize
        {
            get { return settings.TextAreaResize; }
            set { settings.TextAreaResize = value; }
        }

        /// <inheritdoc/>
        public CefState TabToLinks
        {
            get { return settings.TabToLinks; }
            set { settings.TabToLinks = value; }
        }

        /// <inheritdoc/>
        public CefState LocalStorage
        {
            get { return settings.LocalStorage; }
            set { settings.LocalStorage = value; }
        }

        /// <inheritdoc/>
        public CefState Databases
        {
            get { return settings.Databases; }
            set { settings.Databases = value; }
        }

        /// <inheritdoc/>
        public CefState WebGl
        {
            get { return settings.WebGl; }
            set { settings.WebGl = value; }
        }

        /// <inheritdoc/>
        public uint BackgroundColor
        {
            get { return settings.BackgroundColor; }
            set { settings.BackgroundColor = value; }
        }

        /// <inheritdoc/>
        public int WindowlessFrameRate
        {
            get { return settings.WindowlessFrameRate; }
            set { settings.WindowlessFrameRate = value; }
        }

        /// <inheritdoc/>
        public bool IsDisposed
        {
            get { return settings.IsDisposed; }
        }

        /// <inheritdoc/>
        public bool AutoDispose
        {
            get { return settings.AutoDispose; }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            settings.Dispose();
        }

        /// <summary>
       ///在内部用于获取底层 <see cref="IBrowserSettings"/> 实例。
        ///您不太可能自己使用它。
        ///</摘要>
        ///<returns>最里面的实例</returns>
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public IBrowserSettings UnWrap()
        {
            return settings;
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
