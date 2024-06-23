//版权所有 © 2018 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Windows.Input;
using System.Windows.Interop;

using CefSharp.Internals;

namespace CefSharp.Wpf.Internals
{
    public class WpfLegacyKeyboardHandler : IWpfKeyboardHandler
    {
        /// <summary>
        /// 源钩子
        /// </summary>		
        private HwndSourceHook sourceHook;

        /// <summary>
        ///来源
        /// </summary>		
        private HwndSource source;

        /// <summary>
        /// 所有者浏览器实例
        /// </summary>
        private readonly ChromiumWebBrowser owner;

        public WpfLegacyKeyboardHandler(ChromiumWebBrowser owner)
        {
            this.owner = owner;
        }

        public virtual void Setup(HwndSource source)
        {
            this.source = source;
            if (source != null)
            {
                sourceHook = SourceHook;
                source.AddHook(SourceHook);
            }
        }

        public virtual void Dispose()
        {
            if (source != null && sourceHook != null)
            {
                source.RemoveHook(sourceHook);
            }
            source = null;
        }

        /// <summary>		
        /// WindowProc 回调拦截器。处理发送给源 hWnd 的 Windows 消息，并将它们传递给		
        ///根据需要包含浏览器。		
        ///</摘要>		
        ///<param name="hWnd">源句柄。</param>		
        ///<param name="message">消息。</param>		
        ///<param name="wParam">附加消息信息。</param>		
        ///<param name="lParam">更多消息信息。</param>
        ///<param name="handled">如果设置为<c>true</c>，则该事件已被其他人处理。</param>		
        ///<返回>IntPtr.</returns>		
        protected virtual IntPtr SourceHook(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (handled)
            {
                return IntPtr.Zero;
            }

            switch ((WM)message)
            {
                case WM.SYSCHAR:
                case WM.SYSKEYDOWN:
                case WM.SYSKEYUP:
                case WM.KEYDOWN:
                case WM.KEYUP:
                case WM.CHAR:
                case WM.IME_CHAR:
                {
                    if (!owner.IsKeyboardFocused)
                    {
                        break;
                    }

                    if (message == (int)WM.SYSKEYDOWN &&
                        wParam.ToInt32() == KeyInterop.VirtualKeyFromKey(Key.F4))
                    {
                        // 我们不希望 CEF 接收此事件（并将其标记为已处理），因为这使得不可能		
                        //按 Alt-F4 关闭基于 CefSharp 的应用程序，这有点不好。
                        return IntPtr.Zero;
                    }

                    var browser = owner.BrowserCore;
                    if (browser != null)
                    {
                        browser.GetHost().SendKeyEvent(message, wParam.CastToInt32(), lParam.CastToInt32());
                        handled = true;
                    }

                    break;
                }
            }

            return IntPtr.Zero;
        }

        public virtual void HandleKeyPress(KeyEventArgs e)
        {
            // 当 KeyDown 和 KeyUp 冒泡时，它们似乎在有机会之前就被处理了
            //触发由我们的 SourceHook 处理的适当的 WM_ 消息，因此我们必须在这里处理这些额外的键。
            //像这样挂钩 Tab 键使得 Tab 键聚焦本质上就像
            //KeyboardNavigation.TabNavigation="循环";您将永远无法通过 Tab 键脱离网络浏览器的控制。
            //我们还添加了条件，以便当 Web 浏览器控件放入列表框时允许 ctrl+a 起作用。
            if (e.Key == Key.Tab || e.Key == Key.Home || e.Key == Key.End || e.Key == Key.Up
                                 || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right
                                 || (e.Key == Key.A && Keyboard.Modifiers == ModifierKeys.Control))
            {
                var modifiers = e.GetModifiers();
                var message = (int)(e.IsDown ? WM.KEYDOWN : WM.KEYUP);
                var virtualKey = KeyInterop.VirtualKeyFromKey(e.Key);
                var browser = owner.BrowserCore;

                if (browser != null)
                {
                    browser.GetHost().SendKeyEvent(message, virtualKey, (int)modifiers);
                    e.Handled = true;
                }
            }
        }

        public virtual void HandleTextInput(TextCompositionEventArgs e)
        {
            // 在这儿无事可做
        }
    }
}
