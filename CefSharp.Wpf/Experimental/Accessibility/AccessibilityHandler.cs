//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Windows;

namespace CefSharp.Wpf.Experimental.Accessibility
{
    /// <summary>
    ///提供有限的只读辅助功能处理程序实现。
    ///要启用辅助功能支持，请使用 --force-renderer-accessibility 来启用
    ///对于所有浏览器或调用 <see cref="IBrowserHost.SetAccessibilityState(CefState)"/>
    ///在每个浏览器的基础上启用。默认情况下，辅助功能是禁用的。
    ///启用可访问性会影响性能，直到禁用可访问性。
    /// </summary>
    public class AccessibilityHandler : IAccessibilityHandler
    {
        public BrowserAutomationPeer AutomationPeer { get; protected set; }

        protected readonly FrameworkElement owner;

        public AccessibilityHandler(FrameworkElement owner)
        {
            this.AutomationPeer = new BrowserAutomationPeer(owner);

            this.owner = owner;
        }

        void IAccessibilityHandler.OnAccessibilityLocationChange(IValue value)
        {
            OnAccessibilityLocationChange(value);
        }

        /// <summary>
        ///在渲染器进程将可访问性位置更改发送到浏览器进程后调用。
        ///</摘要>
        ///<param name="value">更新的位置信息。</param>
        protected virtual void OnAccessibilityLocationChange(IValue value)
        {

        }

        void IAccessibilityHandler.OnAccessibilityTreeChange(IValue value)
        {
            OnAccessibilityTreeChange(value);
        }

        /// <summary>
        ///在渲染器进程将可访问性树更改发送到浏览器进程后调用。
        ///</摘要>
        ///<param name="value">更新树信息。</param>
        protected virtual void OnAccessibilityTreeChange(IValue value)
        {
            if (value.Type != Enums.ValueType.Dictionary)
            {
                return;
            }

            var accessibilityUpdateDictionary = value.GetDictionary();
            if (accessibilityUpdateDictionary == null || !accessibilityUpdateDictionary.ContainsKey("ax_tree_id"))
            {
                return;
            }

            string treeId = accessibilityUpdateDictionary["ax_tree_id"].GetString();

            owner.Dispatcher.Invoke(new Action(() =>
                AutomationPeer.OnAccessibilityTreeChange(treeId, accessibilityUpdateDictionary)
            ));
        }
    }
}
