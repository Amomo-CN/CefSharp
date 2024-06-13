//版权所有 © 2022 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

namespace CefSharp.Wpf.Experimental
{
    /// <summary>
    /// 弹出窗口创建选项
    /// </summary>
    public enum PopupCreation
    {
        /// <summary>
        /// 弹出窗口创建被取消，不会发生进一步的操作
        /// </summary>
        Cancel = 0,
        /// <summary>
        /// 弹出窗口创建将照常继续。
        /// </summary>
        Continue,
        /// <summary>
        /// 弹出窗口创建将在禁用 JavaScript 的情况下继续进行。
        /// </summary>
        ContinueWithJavascriptDisabled
    }
}
