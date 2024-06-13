//本代码和信息按“原样”提供，不提供任何保证
//任何类型，无论是明示的还是暗示的，包括但不限于
//对适销性和/或适用性的默示保证
//特殊用途。
//
//版权所有 (c) Microsoft Corporation。版权所有

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;

using static Microsoft.Windows.Input.TouchKeyboard.Rcw.InputPaneRcw;

namespace Microsoft.Windows.Input.TouchKeyboard
{
    /// <summary>
    /// 提供Win10触摸键盘-显示/隐藏
    ///注意：文档表明需要 Win10 SDK 来编译它。
    ///https://github.com/Microsoft/WPF-Samples/blob/master/Input%20and%20Commands/TouchKeyboard/TouchKeyboardNotifier/Readme.md
    ///</摘要>
    ///<备注>
    ///改编自https://github.com/Microsoft/WPF-Samples/blob/master/Input%20and%20Commands/TouchKeyboard/TouchKeyboardNotifier/TouchKeyboardEventManager.cs
    ///根据 MIT 许可证获得许可，请参阅 https://github.com/Microsoft/WPF-Samples/blob/master/LICENSE
    /// </remarks>
    [CLSCompliant(true)]
    internal class TouchKeyboardEventManager : IDisposable
    {
        private const string InputPaneTypeName = "Windows.UI.ViewManagement.InputPane, Windows, ContentType=WindowsRuntime";
        /// <summary>
        /// WinRT 输入窗格类型。
        /// </summary>
        private readonly Type inputPaneType = null;

        private IInputPaneInterop inputPaneInterop = null;

        private IInputPane2 inputPanel = null;

        private bool disposed = false;

        /// <summary>
        /// 是否支持调用触摸键盘
        /// </summary>
        private readonly bool touchKeyboardSupported = false;

        /// <summary>
        /// 触摸键盘事件管理器
        ///</摘要>
        /// <paramname="handle">需要 HWND 才能对 IInputPaneInterop 进行本机互操作调用</param>
        internal TouchKeyboardEventManager(IntPtr handle)
        {
            inputPaneType = Type.GetType(InputPaneTypeName);

            // 获取并转换 InputPane COM 实例
            inputPaneInterop = WindowsRuntimeMarshal.GetActivationFactory(inputPaneType) as IInputPaneInterop;

            touchKeyboardSupported = inputPaneInterop != null;

            if (touchKeyboardSupported)
            {
                //获取此 HWND 的实际输入窗格
                inputPanel = inputPaneInterop.GetForWindow(handle, typeof(IInputPane2).GUID);
            }
        }

        /// <summary>
        /// 返回 InputPane 的实例
        /// </summary>
        /// <returns>输入面板</returns>
        internal IInputPane2 GetInputPane()
        {
            if (!touchKeyboardSupported)
            {
                throw new PlatformNotSupportedException("此操作系统不支持对触摸键盘 API 的本机访问!");
            }

            return inputPanel;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (inputPanel != null)
                    {
                        Marshal.FinalReleaseComObject(inputPanel);

                        inputPanel = null;
                    }

                    if (inputPaneInterop != null)
                    {
                        Marshal.FinalReleaseComObject(inputPaneInterop);

                        inputPaneInterop = null;
                    }
                }
            }

            disposed = true;
        }

        // 添加此代码是为了正确实现一次性模式。
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
