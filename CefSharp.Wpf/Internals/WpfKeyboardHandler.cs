//版权所有 © 2018 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System.Windows.Input;
using System.Windows.Interop;

namespace CefSharp.Wpf.Internals
{
    public class WpfKeyboardHandler : IWpfKeyboardHandler
    {
        /// <summary>
        /// 所有者浏览器实例
        /// </summary>
        protected readonly ChromiumWebBrowser owner;

        public WpfKeyboardHandler(ChromiumWebBrowser owner)
        {
            this.owner = owner;
        }

        public virtual void Setup(HwndSource source)
        {
            // 在这儿无事可做
        }

        public virtual void Dispose()
        {
            // 在这儿无事可做
        }

        public virtual void HandleKeyPress(KeyEventArgs e)
        {
            var browser = owner.BrowserCore;
            var key = e.SystemKey == Key.None ? e.Key : e.SystemKey;
            if (browser != null)
            {
                int message;
                int virtualKey = 0;

                switch (key)
                {
                    case Key.LeftAlt:
                    case Key.RightAlt:
                        {
                            virtualKey = (int)VirtualKeys.Menu;
                            break;
                        }

                    case Key.LeftCtrl:
                    case Key.RightCtrl:
                        {
                            virtualKey = (int)VirtualKeys.Control;
                            break;
                        }

                    case Key.LeftShift:
                    case Key.RightShift:
                        {
                            virtualKey = (int)VirtualKeys.Shift;
                            break;
                        }

                    default:
                        virtualKey = KeyInterop.VirtualKeyFromKey(key);
                        break;
                }

                if (e.IsDown)
                {
                    message = (int)(e.SystemKey != Key.None ? WM.SYSKEYDOWN : WM.KEYDOWN);
                }
                else
                {
                    message = (int)(e.SystemKey != Key.None ? WM.SYSKEYUP : WM.KEYUP);
                }

                browser.GetHost().SendKeyEvent(message, virtualKey, 0);
            }

            // 像这样挂钩 Tab 键使得 Tab 键的聚焦本质上就像
            //KeyboardNavigation.TabNavigation="循环";您将永远无法通过 Tab 键脱离网络浏览器的控制。
            //我们还添加了条件，以便当 Web 浏览器控件放入列表框时允许 ctrl+a 起作用。
            //防止使用箭头以及 home 和 end 键进行键盘导航
            if (key == Key.Tab || key == Key.Home || key == Key.End || key == Key.Up
                               || key == Key.Down || key == Key.Left || key == Key.Right
                               || (key == Key.A && Keyboard.Modifiers == ModifierKeys.Control))
            {
                e.Handled = true;
            }
        }

        public virtual void HandleTextInput(TextCompositionEventArgs e)
        {
            var browser = owner.BrowserCore;
            if (browser != null)
            {
                var browserHost = browser.GetHost();
                for (int i = 0; i < e.Text.Length; i++)
                {
                    browserHost.SendKeyEvent((int)WM.CHAR, e.Text[i], 0);
                }
                e.Handled = true;
            }
        }
    }
}
