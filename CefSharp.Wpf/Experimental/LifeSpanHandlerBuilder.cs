//版权所有 © 2022 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

namespace CefSharp.Wpf.Experimental
{
    /// <summary>
    ///流畅的 <see cref="LifeSpanHandler"/> 构建器
    /// </summary>
    public class LifeSpanHandlerBuilder
    {
        private readonly LifeSpanHandler handler;

        /// <summary>
        /// LifeSpanHandlerBuilder
        /// </summary>
        /// <param name="chromiumWebBrowserCreatedDelegate">
        ///指定后，委托将用于创建 <see cref="ChromiumWebBrowser"/>
        ///实例。允许用户创建自己的自定义实例来扩展 <see cref="ChromiumWebBrowser"/>
        /// </param>
        public LifeSpanHandlerBuilder(LifeSpanHandlerCreatePopupChromiumWebBrowser chromiumWebBrowserCreatedDelegate)
        {
            handler = new LifeSpanHandler(chromiumWebBrowserCreatedDelegate);
        }

        /// <summary>
        /// <see cref="LifeSpanHandlerOnBeforePopupCreatedDelegate"/> 将在创建弹出窗口<b>之前</b>被调用，并且
        ///如果需要，可用于取消弹出窗口创建，修改 <see cref="IBrowserSettings"/> 并禁用 javascript。
        ///</摘要>
        ///<param name="onBeforePopupCreated">创建弹出窗口之前调用的操作。</param>
        ///<returns><see cref="LifeSpanHandlerBuilder"/> 实例允许您将方法调用链接在一起</returns>
        public LifeSpanHandlerBuilder OnBeforePopupCreated(LifeSpanHandlerOnBeforePopupCreatedDelegate onBeforePopupCreated)
        {
            handler.OnBeforePopupCreated(onBeforePopupCreated);

            return this;
        }

        /// <summary>
        /// <see cref="LifeSpanHandlerOnPopupCreatedDelegate"/> 将在 <see cref="ChromiumWebBrowser"/> 被调用时被调用
        ///创建。当调用 <see cref="LifeSpanHandlerOnPopupCreatedDelegate"/> 时，您必须将控件添加到其预期的父级。
        ///</摘要>
        ///<param name="onPopupCreated">销毁 Popup 时调用的操作。</param>
        ///<returns><see cref="LifeSpanHandlerBuilder"/> 实例允许您将方法调用链接在一起</returns>
        public LifeSpanHandlerBuilder OnPopupCreated(LifeSpanHandlerOnPopupCreatedDelegate onPopupCreated)
        {
            handler.OnPopupCreated(onPopupCreated);

            return this;
        }

        /// <summary>
        /// <see cref="LifeSpanHandlerOnPopupBrowserCreatedDelegate"/> 将在 <see cref="IBrowser"/> 被调用时被调用
        ///创建。 <see cref="IBrowser"/> 实例在 <see cref="OnPopupDestroyed(LifeSpanHandlerOnPopupDestroyedDelegate)"/> 之前有效
        ///叫做。 <see cref="IBrowser"/> 提供对 CEF 浏览器的低级访问，您可以访问框架、查看源代码、
        ///执行导航（通过框架）等。
        ///</摘要>
        ///<param name="onPopupBrowserCreated">创建 <see cref="IBrowser"/> 时调用的操作。</param>
        ///<returns><see cref="LifeSpanHandlerBuilder"/> 实例允许您将方法调用链接在一起</returns>
        public LifeSpanHandlerBuilder OnPopupBrowserCreated(LifeSpanHandlerOnPopupBrowserCreatedDelegate onPopupBrowserCreated)
        {
            handler.OnPopupBrowserCreated(onPopupBrowserCreated);

            return this;
        }

        /// <summary>
        /// 当要调用 <see cref="ChromiumWebBrowser"/> 时，将调用 <see cref="LifeSpanHandlerOnPopupDestroyedDelegate"/>
        ///从它的父级中删除。
        ///当调用 <see cref="LifeSpanHandlerOnPopupDestroyedDelegate"/> 时，您必须删除/处置 <see cref="ChromiumWebBrowser"/>。
        ///</摘要>
        ///<param name="onPopupDestroyed">要销毁 Popup 时调用的操作。</param>
        ///<returns><see cref="LifeSpanHandlerBuilder"/> 实例允许您将方法调用链接在一起</returns>
        public LifeSpanHandlerBuilder OnPopupDestroyed(LifeSpanHandlerOnPopupDestroyedDelegate onPopupDestroyed)
        {
            handler.OnPopupDestroyed(onPopupDestroyed);

            return this;
        }

        /// <summary>
        /// 创建一个 <see cref="ILifeSpanHandler"/> 实现
        ///可用于将弹出窗口托管为选项卡/控件。 
        ///</摘要>
        ///<returns>一个 <see cref="ILifeSpanHandler"/> 实例</returns>
        public ILifeSpanHandler Build()
        {
            return handler;
        }
    }
}
