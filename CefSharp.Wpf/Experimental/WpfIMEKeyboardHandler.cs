//版权所有 © 2018 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using CefSharp.Internals;
using CefSharp.Structs;
using CefSharp.Wpf.Internals;

using Point = System.Windows.Point;
using Range = CefSharp.Structs.Range;
using Rect = CefSharp.Structs.Rect;

namespace CefSharp.Wpf.Experimental
{
    /// <summary>
    /// 支持 IME 的 WPF 键盘处理程序实现
    ///</摘要>
    ///<seealso cref="T:CefSharp.Wpf.Internals.WpfKeyboardHandler"/>
    public class WpfImeKeyboardHandler : WpfKeyboardHandler
    {
        private int languageCodeId;
        private bool systemCaret;
        private bool isSetup;
        private List<Rect> compositionBounds = new List<Rect>();
        private HwndSource source;
        private HwndSourceHook sourceHook;
        private bool hasImeComposition;
        private MouseButtonEventHandler mouseDownEventHandler;
        private bool isActive;

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="owner">所有者。</param>
        public WpfImeKeyboardHandler(ChromiumWebBrowser owner) : base(owner)
        {
        }

        /// <summary>
        /// 改变成分范围。
        ///</摘要>
        ///<param name="selectionRange">选择范围。</param>
        ///<param name="characterBounds">字符边界。</param>
        public void ChangeCompositionRange(Range selectionRange, Rect[] characterBounds)
        {
            if (!isActive)
            {
                return;
            }

            var screenInfo = ((IRenderWebBrowser)owner).GetScreenInfo();
            var scaleFactor = screenInfo.HasValue ? screenInfo.Value.DeviceScaleFactor : 1.0f;

            //这是在 CEF UI 线程上调用的，我们需要调用回主 UI 线程来
            //访问UI控件
            owner.UiThreadRunAsync(() =>
            {
                //TODO：为每个构图范围更改获取根窗口似乎很昂贵，
                //我们应该缓存位置并在窗口移动时更新它。
                var parentWindow = (FrameworkElement)Window.GetWindow(owner);

                //在Winform嵌入的wpf borwser模式下，Window.GetWindow(owner)为null，因此使用自定义函数来获取最外层的可视元素。
                if (parentWindow == null)
                {
                    parentWindow = GetOutermostElement(owner);
                }

                if (parentWindow != null)
                {
                    //TODO: 我们到底在计算什么？？？
                    var point = owner.TransformToAncestor(parentWindow).Transform(new Point(0, 0));

                    var rects = new List<Rect>();

                    foreach (var item in characterBounds)
                    {
                        rects.Add(new Rect(
                            (int)((point.X + item.X) * scaleFactor),
                            (int)((point.Y + item.Y) * scaleFactor),
                            (int)(item.Width * scaleFactor),
                            (int)(item.Height * scaleFactor)));
                    }

                    compositionBounds = rects;
                    MoveImeWindow(source.Handle);
                }
            });
        }

        /// <summary>
        /// 设置 Ime 键盘处理程序特定的挂钩和事件
        ///</摘要>
        ///<param name="source">HwndSource.</param>
        public override void Setup(HwndSource source)
        {
            if (isSetup)
            {
                return;
            }

            isSetup = true;

            this.source = source;
            sourceHook = SourceHook;
            source.AddHook(SourceHook);

            owner.GotFocus += OwnerGotFocus;
            owner.LostFocus += OwnerLostFocus;

            mouseDownEventHandler = new MouseButtonEventHandler(OwnerMouseDown);

            owner.AddHandler(UIElement.MouseDownEvent, mouseDownEventHandler, true);

            // 如果所有者在添加处理程序之前获得焦点，那么我们必须在此处运行“获得焦点”代码
            //否则在所有情况下都无法正确设置 IME
            if (owner.IsFocused)
            {
                SetActive();
            }
        }

        /// <summary>
        /// 执行与释放、释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public override void Dispose()
        {
            // 注意 可以在处置后运行安装程序，以“重置”此实例
            //由于 ChromiumWebBrowser.PresentationSourceChangedHandler 中的代码
            if (!isSetup)
            {
                return;
            }

            isSetup = false;

            owner.GotFocus -= OwnerGotFocus;
            owner.LostFocus -= OwnerLostFocus;

            owner.RemoveHandler(UIElement.MouseDownEvent, mouseDownEventHandler);

            if (source != null && sourceHook != null)
            {
                source.RemoveHook(sourceHook);
                source = null;
            }
        }

        private void OwnerMouseDown(object sender, MouseButtonEventArgs e)
        {
            CloseImeComposition();
        }

        private void OwnerGotFocus(object sender, RoutedEventArgs e)
        {
            SetActive();
        }

        private void OwnerLostFocus(object sender, RoutedEventArgs e)
        {
            SetInactive();
        }

        private void SetActive()
        {
            // 如果尚未设置为 false，则首先设置为 false，因为值会发生变化（以及引发变化）
            //介于 false 和 true 之间对于 IME 在所有情况下都能正常工作是必要的
            if (InputMethod.GetIsInputMethodEnabled(owner))
            {
                InputMethod.SetIsInputMethodEnabled(owner, false);
            }
            if (InputMethod.GetIsInputMethodSuspended(owner))
            {
                InputMethod.SetIsInputMethodSuspended(owner, false);
            }

            // 为了使 IME 正常运行，需要这些调用。
            InputMethod.SetIsInputMethodEnabled(owner, true);
            InputMethod.SetIsInputMethodSuspended(owner, true);

            isActive = true;
        }

        private void SetInactive()
        {
            isActive = false;

            // 为了使 IME 正常运行，需要这些调用。
            InputMethod.SetIsInputMethodEnabled(owner, false);
            InputMethod.SetIsInputMethodSuspended(owner, false);
        }

        private IntPtr SourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            handled = false;

            if (!isActive || !isSetup || owner == null || owner.IsDisposed || !owner.IsBrowserInitialized)
            {
                return IntPtr.Zero;
            }

            var browserHost = owner.GetBrowserHost();

            if (browserHost == null)
            {
                return IntPtr.Zero;
            }

            switch ((WM)msg)
            {
                case WM.IME_SETCONTEXT:
                {
                    OnImeSetContext(hwnd, (uint)msg, wParam, lParam);
                    handled = true;
                    break;
                }
                case WM.IME_STARTCOMPOSITION:
                {
                    OnIMEStartComposition(hwnd);
                    hasImeComposition = true;
                    handled = true;
                    break;
                }
                case WM.IME_COMPOSITION:
                {
                    OnImeComposition(browserHost, hwnd, lParam.ToInt32());
                    handled = true;
                    break;
                }
                case WM.IME_ENDCOMPOSITION:
                {
                    OnImeEndComposition(browserHost, hwnd);
                    hasImeComposition = false;
                    handled = true;
                    break;
                }
            }

            return handled ? IntPtr.Zero : new IntPtr(1);
        }

        private void CloseImeComposition()
        {
            if (hasImeComposition)
            {
                // 将焦点设置为 0，这会破坏 IME 建议窗口。
                ImeNative.SetFocus(IntPtr.Zero);
                // 恢复焦点。
                ImeNative.SetFocus(source.Handle);
            }
        }

        private void OnImeComposition(IBrowserHost browserHost, IntPtr hwnd, int lParam)
        {
            string text = string.Empty;

            if (ImeHandler.GetResult(hwnd, (uint)lParam, out text))
            {
                browserHost.ImeCommitText(text, new Range(int.MaxValue, int.MaxValue), 0);
                browserHost.ImeSetComposition(text, new CompositionUnderline[0], new Range(int.MaxValue, int.MaxValue), new Range(0, 0));
                browserHost.ImeFinishComposingText(false);
            }
            else
            {
                var underlines = new List<CompositionUnderline>();
                int compositionStart = 0;

                if (ImeHandler.GetComposition(hwnd, (uint)lParam, underlines, ref compositionStart, out text))
                {
                    if (languageCodeId == ImeNative.LANG_KOREAN)
                    {
                        browserHost.ImeSetComposition(text, underlines.ToArray(),
                        new Range(int.MaxValue, int.MaxValue), new Range(compositionStart + underlines.Count, compositionStart + underlines.Count));
                    }
                    else
                    {
                        browserHost.ImeSetComposition(text, underlines.ToArray(),
                        new Range(int.MaxValue, int.MaxValue), new Range(compositionStart, compositionStart));
                    }

                    UpdateCaretPosition(compositionStart - 1);
                }
                else
                {
                    CancelComposition(browserHost, hwnd);
                }
            }
        }

        /// <summary>
        ///取消合成。
        ///</摘要>
        ///<param name="browserHost">浏览器主机</param>
        ///<param name="hwnd">hwnd。</param>
        private void CancelComposition(IBrowserHost browserHost, IntPtr hwnd)
        {
            browserHost.ImeCancelComposition();
            DestroyImeWindow(hwnd);
        }

        private void OnImeEndComposition(IBrowserHost browserHost, IntPtr hwnd)
        {
            // 韩语 IME 以某种方式忽略对 ::ImeFinishCompositingText() 的函数调用
            //在 ::OnImeComposition() 中提交相同的字母
            if (languageCodeId != ImeNative.LANG_KOREAN)
            {
                browserHost.ImeFinishComposingText(false);
            }
            DestroyImeWindow(hwnd);
        }

        private void OnImeSetContext(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            // 我们自己处理 IME 组合窗口（但让 IME 候选者
            //窗口由 IME 通过 DefWindowProc()) 处理，因此清除
            //ISC_SHOWUICOMPOSITIONWINDOW 标志：
            ImeNative.DefWindowProc(hwnd, msg, wParam, (IntPtr)(lParam.ToInt64() & ~ImeNative.ISC_SHOWUICOMPOSITIONWINDOW));
            //TODO：我们应该调用 ImmNotifyIME 吗？

            CreateImeWindow(hwnd);
            MoveImeWindow(hwnd);
        }

        private void OnIMEStartComposition(IntPtr hwnd)
        {
            CreateImeWindow(hwnd);
            MoveImeWindow(hwnd);
        }

        private void CreateImeWindow(IntPtr hwnd)
        {
            // 中文/日文输入法以某种方式忽略函数调用
            //::ImmSetCandidateWindow()，并使用当前系统的位置
            //插入符改为 -::GetCaretPos()。
            //因此，我们为中文输入法创建一个临时系统插入符号并使用
            //在此输入上下文期间。
            //由于有些第三方日文输入法也使用::GetCaretPos()来判断
            //他们的窗口位置，我们还为日语 IME 创建插入符号。
            languageCodeId = PrimaryLangId(InputLanguageManager.Current.CurrentInputLanguage.KeyboardLayoutId);

            if (languageCodeId == ImeNative.LANG_JAPANESE || languageCodeId == ImeNative.LANG_CHINESE)
            {
                if (!systemCaret)
                {
                    if (ImeNative.CreateCaret(hwnd, IntPtr.Zero, 1, 1))
                    {
                        systemCaret = true;
                    }
                }
            }
        }

        private int PrimaryLangId(int lgid)
        {
            return lgid & 0x3ff;
        }

        private void MoveImeWindow(IntPtr hwnd)
        {
            if (compositionBounds.Count == 0)
            {
                return;
            }

            var hIMC = ImeNative.ImmGetContext(hwnd);

            var rc = compositionBounds[0];

            var x = rc.X + rc.Width;
            var y = rc.Y + rc.Height;

            const int kCaretMargin = 1;
            // 正如 ImeInput::CreateImeWindow() 中的注释中所写，
            //中文输入法忽略对 ::ImmSetCandidateWindow() 的函数调用
            //当用户禁用 TSF（文本服务框架）和 CUAS（Cicero
            //不知道应用程序支持）。
            //另一方面，当用户启用 TSF 和 CUAS 时，中文输入法
            //忽略当前系统插入符的位置并使用
            //赋予 ::ImmSetCandidateWindow() 的参数及其“dwStyle”
            //参数 CFS_CANDIDATEPOS。
            //因此，我们不仅调用::ImmSetCandidateWindow()，而且还调用
            //设置临时系统插入符的位置（如果存在）。
            var candidatePosition = new ImeNative.CANDIDATEFORM
            {
                dwIndex = 0,
                dwStyle = (int)ImeNative.CFS_CANDIDATEPOS,
                ptCurrentPos = new ImeNative.POINT(x, y),
                rcArea = new ImeNative.RECT(0, 0, 0, 0)
            };
            ImeNative.ImmSetCandidateWindow(hIMC, ref candidatePosition);

            if (systemCaret)
            {
                ImeNative.SetCaretPos(x, y);
            }

            if (languageCodeId == ImeNative.LANG_CHINESE)
            {
                // 中文输入法需要设置组合窗口
                var compositionPotision = new ImeNative.COMPOSITIONFORM
                {
                    dwStyle = (int)ImeNative.CFS_POINT,
                    ptCurrentPos = new ImeNative.POINT(x, y),
                    rcArea = new ImeNative.RECT(0, 0, 0, 0)
                };
                ImeNative.ImmSetCompositionWindow(hIMC, ref compositionPotision);
            }

            if (languageCodeId == ImeNative.LANG_KOREAN)
            {
                // 中文输入法和日文输入法需要左上角
                //插入符号移动候选窗口的位置。
                //另一方面，韩语输入法需要左下角
                //插入符移动候选窗口。
                y += kCaretMargin;
            }
            // 日语 IME 和韩语 IME 也使用给定的矩形
            //::ImmSetCandidateWindow() 及其 'dwStyle' 参数 CFS_EXCLUDE
            //当用户禁用 TSF 和 CUAS 时移动候选窗口。
            //因此，我们在这里也设置这个参数。
            var excludeRectangle = new ImeNative.CANDIDATEFORM
            {
                dwIndex = 0,
                dwStyle = (int)ImeNative.CFS_EXCLUDE,
                ptCurrentPos = new ImeNative.POINT(x, y),
                rcArea = new ImeNative.RECT(rc.X, rc.Y, x, y + kCaretMargin)
            };
            ImeNative.ImmSetCandidateWindow(hIMC, ref excludeRectangle);

            ImeNative.ImmReleaseContext(hwnd, hIMC);
        }

        private void DestroyImeWindow(IntPtr hwnd)
        {
            if (systemCaret)
            {
                ImeNative.DestroyCaret();
                systemCaret = false;
            }
        }

        //TODO: 我们是否应该删除它，它只是一个方法
        private void UpdateCaretPosition(int index)
        {
            MoveImeWindow(source.Handle);
        }

        /// <summary>
        /// 获取浏览器最外层元素 
        ///</摘要>
        ///<param name="control">浏览器</param>
        ///<returns>最外层元素</returns>
        private FrameworkElement GetOutermostElement(FrameworkElement control)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(control);
            DependencyObject current = control;

            while (parent != null)
            {
                current = parent;
                parent = VisualTreeHelper.GetParent(current);
            }

            return current as FrameworkElement;
        }
    }
}
