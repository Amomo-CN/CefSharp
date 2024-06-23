//版权所有 © 2019 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System.Windows.Input;

using CefSharp.Enums;
using CefSharp.Structs;
using CefSharp.Wpf.Internals;

namespace CefSharp.Wpf.Experimental
{
    /// <summary>
    ///实验性 ChromiumWebBrowser 实现，包括对 Stylus 的支持
    ///使用默认的 WPF 触摸实现。存在已知的性能问题
    ///此默认实现，解决方法例如 https://github.com/jaytwo/WmTouchDevice
    ///可能需要考虑。 .Net 4.7 支持较新的 WM_Pointer 实现
    ///应该解决问题，请参阅https://github.com/dotnet/docs/blob/master/docs/framework/migration-guide/mitigation-pointer-based-touch-and-stylus-support.md
    ///原始PR https://github.com/cefsharp/CefSharp/pull/2745
    ///原作者 https://github.com/GSonofNun
    ///触摸支持已合并到 ChromiumWebBrowser 中，此类中仅存在样式支持
    ///</摘要>
    public class ChromiumWebBrowserWithTouchSupport : ChromiumWebBrowser
    {
        /// <summary>
        /// 当未处理的 <see cref="E:System.Windows.Input.StylusDown" /> 附加事件到达其路由中派生自此类的元素时调用。实现此方法以添加对此事件的类处理。
        ///</摘要>
        ///<param name="e">包含事件数据的 <see cref="T:System.Windows.Input.StylusDownEventArgs" />。</param>
        protected override void OnStylusDown(StylusDownEventArgs e)
        {
            //触摸和手写笔都会引发手写笔事件。
            //我们使用包含更多中间点的OnTouchXXX方法来处理用户的触摸，以便我们可以更快地跟踪用户的手指。
            //使用e.StylusDevice.TabletDevice.Type == TabletDeviceType.Stylus确保只有手写笔。
            if (e.StylusDevice.TabletDevice.Type == TabletDeviceType.Stylus)
            {
                Focus();
                // 捕获触控笔，因此即使触控笔在 StylusUp 之前离开控件，触控笔事件仍会推送到 CEF。
                //此行为类似于其他浏览器处理手写笔输入的方式。
                CaptureStylus();
                OnStylus(e, TouchEventType.Pressed);
            }
            base.OnStylusDown(e);
        }

        /// <summary>
        /// 当未处理的 <see cref="E:System.Windows.Input.StylusMove" /> 附加事件到达其路由中派生自此类的元素时调用。实现此方法以添加对此事件的类处理。
        ///</摘要>
        ///<param name="e">包含事件数据的 <see cref="T:System.Windows.Input.StylusDownEventArgs" />。</param>
        protected override void OnStylusMove(StylusEventArgs e)
        {
            //触摸和手写笔都会引发手写笔事件。
            //我们使用包含更多中间点的OnTouchXXX方法来处理用户的触摸，以便我们可以更快地跟踪用户的手指。
            //使用e.StylusDevice.TabletDevice.Type == TabletDeviceType.Stylus确保只有手写笔。
            if (e.StylusDevice.TabletDevice.Type == TabletDeviceType.Stylus)
            {
                OnStylus(e, TouchEventType.Moved);
            }
            base.OnStylusMove(e);
        }

        /// <summary>
        ///当未处理的 <see cref="E:System.Windows.Input.StylusUp" /> 附加事件到达其路由中派生自此类的元素时调用。实现此方法以添加对此事件的类处理。
        ///</摘要>
        ///<param name="e">包含事件数据的 <see cref="T:System.Windows.Input.StylusDownEventArgs" />。</param>
        protected override void OnStylusUp(StylusEventArgs e)
        {
            //触摸和手写笔都会引发手写笔事件。
            //我们使用包含更多中间点的OnTouchXXX方法来处理用户的触摸，以便我们可以更快地跟踪用户的手指。
            //使用e.StylusDevice.TabletDevice.Type == TabletDeviceType.Stylus确保只有手写笔。
            if (e.StylusDevice.TabletDevice.Type == TabletDeviceType.Stylus)
            {
                ReleaseStylusCapture();
                OnStylus(e, TouchEventType.Released);
            }
            base.OnStylusUp(e);
        }

        /// <summary>
        /// 处理 <see cref="E:Stylus" /> 事件。
        ///</摘要>
        ///<param name="e">包含事件数据的 <see cref="StylusEventArgs"/> 实例。</param>
        ///<param name="touchEventType"><see cref="TouchEventType"/> 事件类型</param>
        private void OnStylus(StylusEventArgs e, TouchEventType touchEventType)
        {
            var browser = GetBrowser();

            if (!e.Handled && browser != null)
            {
                var modifiers = WpfExtensions.GetModifierKeys();
                var pointerType = e.StylusDevice.Inverted ? PointerType.Eraser : PointerType.Pen;
                //将所有点发送给主机
                foreach (var stylusPoint in e.GetStylusPoints(this))
                {
                    var touchEvent = new TouchEvent()
                    {
                        Id = e.StylusDevice.Id,
                        X = (float)stylusPoint.X,
                        Y = (float)stylusPoint.Y,
                        PointerType = pointerType,
                        Pressure = stylusPoint.PressureFactor,
                        Type = touchEventType,
                        Modifiers = modifiers,
                    };

                    browser.GetHost().SendTouchEvent(touchEvent);
                }
                e.Handled = true;
            }
        }
    }
}
