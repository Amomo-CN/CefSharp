//版权所有 © 2015 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Collections.Generic;

namespace CefSharp.Wpf.Experimental
{
    /// <summary>
    /// 在创建弹出窗口之前<b></b>调用，可用于在需要时取消弹出窗口创建
    ///或修改<see cref="IBrowserSettings"/>。
    ///需要注意的是，该接口的方法是在 CEF UI 线程上调用的，
    ///默认情况下与您的应用程序 UI 线程不同。
    ///</摘要>
    ///<param name="chromiumWebBrowser">ChromiumWebBrowser 控件</param>
    ///<param name="browser">启动此弹出窗口的浏览器实例。</param>
    ///<param name="frame">启动此弹出窗口的 HTML 框架。</param>
    ///<param name="targetUrl">弹出内容的 URL。 （这可能为空/null）</param>
    ///<param name="targetFrameName">弹出窗口的名称。 （这可能为空/null）</param>
    ///<param name="targetDisposition">该值指示用户打算前往的位置
    ///打开弹出窗口（例如当前选项卡、新选项卡等）</param>
    ///<param name="userGesture">如果通过显式用户手势打开弹出窗口，则该值将为 true
    ///（例如单击链接）或 false 如果弹出窗口自动打开（例如通过 DomContentLoaded 事件）。</param>
    ///<param name="browserSettings">浏览器设置，默认为源浏览器</param>
    ///<returns>取消创建弹出窗口返回 true，否则返回 false。</returns>
    public delegate PopupCreation LifeSpanHandlerOnBeforePopupCreatedDelegate(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IBrowserSettings browserSettings);

    /// <summary>
    ///创建 <see cref="ChromiumWebBrowser"/> 时调用。
    ///调用时，您必须将控件添加到其预期的父级。
    ///</摘要>
    ///<param name="control">弹出主机控件</param>
    ///<param name="url">url</param>
    ///<param name="targetFrameName">目标框架名称</param>
    ///<param name="windowInfo">WindowInfo</param>
    public delegate void LifeSpanHandlerOnPopupCreatedDelegate(ChromiumWebBrowser control, string url, string targetFrameName, IWindowInfo windowInfo);

    /// <summary>
    /// 在创建 <see cref="IBrowser"/> 实例时调用。
    ///<see cref="IBrowser"/> 引用在调用 <see cref="LifeSpanHandlerOnPopupDestroyedDelegate"/> 之前一直有效
    ///</摘要>
    ///<param name="control">弹出 ChromiumWebBrowser 控件，如果浏览器托管在本机弹出窗口中，则可能为 null。
    ///DevTools 默认情况下将托管在本机弹出窗口中。</param>
    ///<param name="browser">浏览器</param>
    public delegate void LifeSpanHandlerOnPopupBrowserCreatedDelegate(ChromiumWebBrowser control, IBrowser browser);

    /// <summary>
    /// 当要从其父级中删除 <see cref="ChromiumWebBrowser"/> 时调用。
    ///调用时，您必须删除/处置 <see cref="ChromiumWebBrowser"/>。
    ///</摘要>
    ///<param name="control">弹出 ChromiumWebBrowser 控件</param>
    ///<param name="browser">浏览器</param>
    public delegate void LifeSpanHandlerOnPopupDestroyedDelegate(ChromiumWebBrowser control, IBrowser browser);

    /// <summary>
    ///调用以创建 <see cref="ChromiumWebBrowser"/> 的新实例。允许创建派生/自定义
    ///<see cref="ChromiumWebBrowser"/> 的实现。
    ///</摘要>
    ///<returns><see cref="ChromiumWebBrowser"/> 的自定义实例。</returns>
    public delegate ChromiumWebBrowser LifeSpanHandlerCreatePopupChromiumWebBrowser();

    /// <summary>
    /// WPF -实验性 LifeSpanHandler 实现，可用于使用新的 <see cref="ChromiumWebBrowser"/> 实例托管弹出窗口。
    /// </summary>
    public class LifeSpanHandler : CefSharp.Handler.LifeSpanHandler
    {
        private LifeSpanHandlerOnBeforePopupCreatedDelegate onBeforePopupCreated;
        private LifeSpanHandlerOnPopupDestroyedDelegate onPopupDestroyed;
        private LifeSpanHandlerOnPopupBrowserCreatedDelegate onPopupBrowserCreated;
        private LifeSpanHandlerOnPopupCreatedDelegate onPopupCreated;
        private LifeSpanHandlerCreatePopupChromiumWebBrowser chromiumWebBrowserCreatedDelegate;
        private Dictionary<IntPtr, ChromiumWebBrowser> chromiumWebBrowserMap = new Dictionary<IntPtr, ChromiumWebBrowser>();
        private ChromiumWebBrowser pendingChromiumWebBrowser;

        /// <summary>
        ///默认构造函数
        /// </summary>
        /// <param name="chromiumWebBrowserCreatedDelegate">用于创建自定义 <see cref="ChromiumWebBrowser" /> 实例的可选委托。</param>
        public LifeSpanHandler(LifeSpanHandlerCreatePopupChromiumWebBrowser chromiumWebBrowserCreatedDelegate = null)
        {
            this.chromiumWebBrowserCreatedDelegate = chromiumWebBrowserCreatedDelegate;
        }

        /// <summary>
        /// <see cref="LifeSpanHandlerOnBeforePopupCreatedDelegate"/> 将在创建弹出窗口<b>之前</b>被调用，并且
        ///可用于取消弹出窗口创建（如果需要）或修改 <see cref="IBrowserSettings"/>。
        ///</摘要>
        ///<param name="onBeforePopupCreated">创建弹出窗口之前调用的操作。</param>
        ///<returns><see cref="LifeSpanHandler"/> 实例允许您将方法调用链接在一起</returns>
        public LifeSpanHandler OnBeforePopupCreated(LifeSpanHandlerOnBeforePopupCreatedDelegate onBeforePopupCreated)
        {
            this.onBeforePopupCreated = onBeforePopupCreated;

            return this;
        }

        /// <summary>
        /// <see cref="LifeSpanHandlerOnPopupCreatedDelegate"/> 将在 <see cref="ChromiumWebBrowser"/> 被调用时被调用
        ///创建。当调用 <see cref="LifeSpanHandlerOnPopupCreatedDelegate"/> 时，您必须将控件添加到其预期的父级。
        ///</摘要>
        ///<param name="onPopupCreated">当 Popup 主机已创建并准备好附加到其父级时调用的操作。</param>
        ///<returns><see cref="LifeSpanHandler"/> 实例允许您将方法调用链接在一起</returns>
        public LifeSpanHandler OnPopupCreated(LifeSpanHandlerOnPopupCreatedDelegate onPopupCreated)
        {
            this.onPopupCreated = onPopupCreated;

            return this;
        }

        /// <summary>
        /// <see cref="LifeSpanHandlerOnPopupBrowserCreatedDelegate"/> 将在 <see cref="IBrowser"/> 被调用时被调用
        ///创建。 <see cref="IBrowser"/> 实例在 <see cref="OnPopupDestroyed(LifeSpanHandlerOnPopupDestroyedDelegate)"/> 之前有效
        ///叫做。 <see cref="IBrowser"/> 提供对 CEF 浏览器的低级访问，您可以访问框架、查看源代码、
        ///执行导航（通过框架）等。
        ///</摘要>
        ///<param name="onPopupBrowserCreated">创建 <see cref="IBrowser"/> 时调用的操作。</param>
        ///<returns><see cref="LifeSpanHandler"/> 实例允许您将方法调用链接在一起</returns>
        public LifeSpanHandler OnPopupBrowserCreated(LifeSpanHandlerOnPopupBrowserCreatedDelegate onPopupBrowserCreated)
        {
            this.onPopupBrowserCreated = onPopupBrowserCreated;

            return this;
        }

        /// <summary>
        /// 当要调用 <see cref="ChromiumWebBrowser"/> 时，将调用 <see cref="LifeSpanHandlerOnPopupDestroyedDelegate"/>
        ///从它的父级中删除。
        ///当调用 <see cref="LifeSpanHandlerOnPopupDestroyedDelegate"/> 时，您必须删除/处置 <see cref="ChromiumWebBrowser"/>。
        ///</摘要>
        ///<param name="onPopupDestroyed">要销毁 Popup 时调用的操作。</param>
        ///<returns><see cref="LifeSpanHandler"/> 实例允许您将方法调用链接在一起</returns>
        public LifeSpanHandler OnPopupDestroyed(LifeSpanHandlerOnPopupDestroyedDelegate onPopupDestroyed)
        {
            this.onPopupDestroyed = onPopupDestroyed;

            return this;
        }

        /// <summary>
        /// 创建 <see cref="LifeSpanHandlerBuilder"/> 的新实例
        ///可用于创建特定于 WinForms 的 <see cref="ILifeSpanHandler"/>
        ///简化了将弹出窗口作为控件/选项卡托管的过程的实现。
        ///</摘要>
        ///<返回>
        ///一个 <see cref="LifeSpanHandlerBuilder"/> 可用于流畅地创建 <see cref="ILifeSpanHandler"/>。
        ///调用后调用 <see cref="LifeSpanHandlerBuilder.Build"/> 创建实际实例
        ///<参见 cref="LifeSpanHandlerBuilder.OnPopupCreated(LifeSpanHandlerOnPopupCreatedDelegate)"/> etc.
        /// </returns>
        public static LifeSpanHandlerBuilder Create(LifeSpanHandlerCreatePopupChromiumWebBrowser chromiumWebBrowserCreatedDelegate = null)
        {
            return new LifeSpanHandlerBuilder(chromiumWebBrowserCreatedDelegate);
        }

        /// <inheritdoc/>
        protected override bool OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            newBrowser = null;

            var userAction = onBeforePopupCreated?.Invoke(browserControl, browser, frame, targetUrl, targetFrameName, targetDisposition, userGesture, browserSettings) ?? PopupCreation.Continue;

            // 取消弹出窗口创建
            if (userAction == PopupCreation.Cancel)
            {
                return true;
            }

            if (userAction == PopupCreation.ContinueWithJavascriptDisabled)
            {
                noJavascriptAccess = true;
            }

            //没有任何操作，因此我们将采用默认行为。
            if (onPopupCreated == null)
            {
                return false;
            }

            var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;

            ChromiumWebBrowser control = null;

            // 调用 WPF UI 线程以创建新的 ChromiumWebBrowser
            chromiumWebBrowser.Dispatcher.Invoke(() =>
            {
                control = chromiumWebBrowserCreatedDelegate?.Invoke();

                if (control == null)
                {
                    control = new ChromiumWebBrowser();
                }

                control.SetAsPopup();
                control.LifeSpanHandler = this;

                // 当前的假设是弹出窗口是按顺序创建的
                pendingChromiumWebBrowser = control;

                onPopupCreated?.Invoke(control, targetUrl, targetFrameName, windowInfo);

                var owner = System.Windows.Window.GetWindow(control);

                if (owner == null)
                {
                    windowInfo.SetAsWindowless(IntPtr.Zero);
                }
                else
                {
                    var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(owner);
                    //确保已创建窗口句柄（在 WPF 中，每个窗口只有一个句柄，而不是每个控件）
                    var handle = windowInteropHelper.EnsureHandle();

                    //ParentHandle 值将用于标识监视器信息并充当对话框的父窗口，
                    //上下文菜单等。如果未提供parentHandle，则将使用主屏幕监视器和一些
                    //需要父窗口的功能可能无法正常运行。
                    windowInfo.SetAsWindowless(parentHandle: handle);
                }
            });

            newBrowser = control;

            return false;
        }

        /// <inheritdoc/>
        protected override void OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
        {
            // 对于 DevTools/Native 弹出窗口，挂起的 ChromiumWebBrowser 应该为 null
            if (browser.IsPopup && pendingChromiumWebBrowser != null)
            {
                chromiumWebBrowserMap.Add(browser.GetHost().GetWindowHandle(), pendingChromiumWebBrowser);

                onPopupBrowserCreated?.Invoke(pendingChromiumWebBrowser, browser);

                pendingChromiumWebBrowser = null;
            }
        }

        /// <inheritdoc/>
        protected override bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            if (browser.IsPopup && !browser.IsDisposed)
            {
                var handle = browser.GetHost().GetWindowHandle();

                if (chromiumWebBrowserMap.TryGetValue(handle, out ChromiumWebBrowser control))
                {
                    // 控件为空，使用默认行为
                    if (control == null)
                    {
                        return false;
                    }

                    if (!control.Dispatcher.HasShutdownStarted)
                    {
                        //我们需要以同步方式调用，以便我们的 IBrowser 对象仍在范围内
                        //以异步方式调用会导致 IBrowser 在我们之前被释放
                        //可以访问它。
                        control.Dispatcher.Invoke(new Action(() =>
                        {
                            onPopupDestroyed?.Invoke(control, browser);

                            if (!control.IsDisposed)
                            {
                                control.Dispose();
                            }
                        }));
                    }

                    chromiumWebBrowserMap.Remove(handle);

                    return true;
                }
            }

            //我们没有找到该控件，因此我们使用默认行为
            //默认弹出窗口例如DevTools 需要返回 false
            //因此发送 WM_CLOSE 来关闭窗口。
            return false;
        }
    }
}
