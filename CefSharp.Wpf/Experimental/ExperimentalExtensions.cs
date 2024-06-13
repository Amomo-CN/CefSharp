using CefSharp.Wpf.Internals;

namespace CefSharp.Wpf.Experimental
{
    /// <summary>
    /// 实验性扩展
    /// </summary>
    public static class ExperimentalExtensions
    {
        /// <summary>
        ///默认情况下，当靠近页面底部时，Html 下拉菜单会离开屏幕
        ///调用此方法以使用 <see cref="MousePositionTransform"/> 实现
        ///重新定位弹出窗口和鼠标。
        ///
        /// 问题 https://github.com/cefsharp/CefSharp/issues/2820
        ///</摘要>
        ///<param name="chromiumWebBrowser">浏览器</param>
        public static void UsePopupMouseTransform(this ChromiumWebBrowser chromiumWebBrowser)
        {
            chromiumWebBrowser.MousePositionTransform = new MousePositionTransform();
        }

        /// <summary>
        /// 使用自定义 <see cref="IMousePositionTransform"/> 实现
        ///</摘要>
        ///<param name="chromiumWebBrowser">浏览器</param>
        ///<param name="mousePositionTransform"><see cref="IMousePositionTransform"/> 的自定义实现
        ///或者如果为 null，则默认为 <see cref="NoOpMousePositionTransform"/>。
        /// </param>
        public static void UsePopupMouseTransform(this ChromiumWebBrowser chromiumWebBrowser, IMousePositionTransform mousePositionTransform)
        {
            chromiumWebBrowser.MousePositionTransform = mousePositionTransform ?? new NoOpMousePositionTransform();
        }
    }
}
