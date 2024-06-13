//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

//注意：CefSharp.Core 命名空间中的类已对 intellisnse 隐藏，因此用户不会直接使用它们

using System;

namespace CefSharp
{
    /// <summary>
    ///用于低级操作、内存复制的本机静态方法
    ///避免 P/Invoke，因为我们可以直接调用 C++ API。
    /// </summary>
    public static class NativeMethodWrapper
    {
        public static void MemoryCopy(IntPtr dest, IntPtr src, int numberOfBytes)
        {
            CefSharp.Core.NativeMethodWrapper.MemoryCopy(dest, src, numberOfBytes);
        }

        public static bool IsFocused(IntPtr handle)
        {
            return CefSharp.Core.NativeMethodWrapper.IsFocused(handle);
        }

        public static void SetWindowPosition(IntPtr handle, int x, int y, int width, int height)
        {
            CefSharp.Core.NativeMethodWrapper.SetWindowPosition(handle, x, y, width, height);
        }

        public static void SetWindowParent(IntPtr child, IntPtr newParent)
        {
            CefSharp.Core.NativeMethodWrapper.SetWindowParent(child, newParent);
        }

        public static void RemoveExNoActivateStyle(IntPtr browserHwnd)
        {
            CefSharp.Core.NativeMethodWrapper.RemoveExNoActivateStyle(browserHwnd);
        }

        public static IntPtr LoadCursorFromLibCef(int resourceIdentifier)
        {
            return CefSharp.Core.NativeMethodWrapper.LoadCursorFromLibCef(resourceIdentifier);
        }
    }
}
