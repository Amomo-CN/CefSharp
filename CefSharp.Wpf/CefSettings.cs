//版权所有 © 2018 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

namespace CefSharp.Wpf
{
    /// <summary>
    /// 初始化设置。其中许多设置和其他设置也可以配置
    ///使用命令行开关。
    /// </summary>
    public class CefSettings : CefSettingsBase
    {
        /// <summary>
        /// 使用默认值初始化
        /// </summary>
        public CefSettings() : base()
        {
            WindowlessRenderingEnabled = true;

            //禁用 Web 内容的多线程、合成器滚动
            //通过 OSR 渲染，提高滚动性能是相当常见的
            //https://peter.sh/experiments/chromium-command-line-switches/#disable-threaded-scrolling
            //CefCommandLineArgs.Add("禁用线程滚动");
        }
    }
}
