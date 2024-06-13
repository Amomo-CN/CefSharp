//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

//注意：CefSharp.Core 命名空间中的类已对 intellisnse 隐藏，因此用户不会直接使用它们

using System;
using CefSharp.Enums;
using CefSharp.Structs;

namespace CefSharp
{
    /// <inheritdoc/>
    public class WindowInfo : IWindowInfo
    {
        private CefSharp.Core.WindowInfo windowInfo = new CefSharp.Core.WindowInfo();

        /// <inheritdoc/>
        public int X
        {
            get { return windowInfo.X; }
            set { windowInfo.X = value; }
        }

        /// <inheritdoc/>
        public int Y
        {
            get { return windowInfo.Y; }
            set { windowInfo.Y = value; }
        }

        /// <inheritdoc/>
        public int Width
        {
            get { return windowInfo.Width; }
            set { windowInfo.Width = value; }
        }

        /// <inheritdoc/>
        public int Height
        {
            get { return windowInfo.Height; }
            set { windowInfo.Height = value; }
        }

        /// <inheritdoc/>
        public uint Style
        {
            get { return windowInfo.Style; }
            set { windowInfo.Style = value; }
        }

        /// <inheritdoc/>
        public uint ExStyle
        {
            get { return windowInfo.ExStyle; }
            set { windowInfo.ExStyle = value; }
        }

        /// <inheritdoc/>
        public IntPtr ParentWindowHandle
        {
            get { return windowInfo.ParentWindowHandle; }
            set { windowInfo.ParentWindowHandle = value; }
        }

        /// <inheritdoc/>
        public bool WindowlessRenderingEnabled
        {
            get { return windowInfo.WindowlessRenderingEnabled; }
            set { windowInfo.WindowlessRenderingEnabled = value; }
        }

        /// <inheritdoc/>
        public bool SharedTextureEnabled
        {
            get { return windowInfo.SharedTextureEnabled; }
            set { windowInfo.SharedTextureEnabled = value; }
        }

        /// <inheritdoc/>
        public bool ExternalBeginFrameEnabled
        {
            get { return windowInfo.ExternalBeginFrameEnabled; }
            set { windowInfo.ExternalBeginFrameEnabled = value; }
        }

        /// <inheritdoc/>
        public IntPtr WindowHandle
        {
            get { return windowInfo.WindowHandle; }
            set { windowInfo.WindowHandle = value; }
        }

        /// <inheritdoc/>
        public string WindowName
        {
            get { return windowInfo.WindowName; }
            set { windowInfo.WindowName = value; }
        }

        /// <inheritdoc/>
        public CefRuntimeStyle RuntimeStyle
        {
            get { return windowInfo.RuntimeStyle; }
            set { windowInfo.RuntimeStyle = value; }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            windowInfo.Dispose();
        }

        /// <inheritdoc/>
        public void SetAsChild(IntPtr parentHandle)
        {
            windowInfo.SetAsChild(parentHandle);
        }

        /// <inheritdoc/>
        public void SetAsChild(IntPtr parentHandle, Rect windowBounds)
        {
            windowInfo.SetAsChild(parentHandle, windowBounds);
        }

        /// <inheritdoc/>
        public void SetAsChild(IntPtr parentHandle, int left, int top, int right, int bottom)
        {
            windowInfo.SetAsChild(parentHandle, left, top, right, bottom);
        }

        /// <inheritdoc/>
        public void SetAsPopup(IntPtr parentHandle, string windowName)
        {
            windowInfo.SetAsPopup(parentHandle, windowName);
        }

        /// <inheritdoc/>
        public void SetAsWindowless(IntPtr parentHandle)
        {
            windowInfo.SetAsWindowless(parentHandle);
        }

        ///<摘要>
        ///创建一个新的 <see cref="IWindowInfo"/> 实例
        ///</摘要>
        ///<returns>WindowInfo</returns>
        public static IWindowInfo Create()
        {
            return new CefSharp.Core.WindowInfo();
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public IWindowInfo UnWrap()
        {
            return windowInfo;
        }
    }
}
