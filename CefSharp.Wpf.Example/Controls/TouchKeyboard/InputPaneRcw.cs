//本代码和信息按“原样”提供，不提供任何保证
//任何类型，无论是明示的还是暗示的，包括但不限于
//对适销性和/或适用性的默示保证
//特殊用途。
//
//版权所有 (c) Microsoft Corporation。版权所有

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Windows.Input.TouchKeyboard.Rcw
{
    /// <summary>
    /// 包含用于调用 InputPane（tiptsf 触摸键盘）的内部 RCW
    /// </summary>
    /// <remarks>
    /// 改编自https://github.com/Microsoft/WPF-Samples/blob/master/Input%20and%20Commands/TouchKeyboard/TouchKeyboardNotifier/InputPaneRcw.cs
    ///根据 MIT 许可证获得许可，请参阅 https://github.com/Microsoft/WPF-Samples/blob/master/LICENSE
    /// </remarks>
    internal static class InputPaneRcw
    {
        internal enum TrustLevel
        {
            BaseTrust,
            PartialTrust,
            FullTrust
        }

        [Guid("75CF2C57-9195-4931-8332-F0B409E916AF"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport]
        internal interface IInputPaneInterop
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            void GetIids(out uint iidCount, [MarshalAs(UnmanagedType.LPStruct)] out Guid iids);

            [MethodImpl(MethodImplOptions.InternalCall)]
            void GetRuntimeClassName([MarshalAs(UnmanagedType.BStr)] out string className);

            [MethodImpl(MethodImplOptions.InternalCall)]
            void GetTrustLevel(out TrustLevel TrustLevel);

            [MethodImpl(MethodImplOptions.InternalCall)]
            IInputPane2 GetForWindow([In] IntPtr appWindow, [In] ref Guid riid);
        }

        [Guid("8A6B3F26-7090-4793-944C-C3F2CDE26276"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport]
        internal interface IInputPane2
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            void GetIids(out uint iidCount, [MarshalAs(UnmanagedType.LPStruct)] out Guid iids);

            [MethodImpl(MethodImplOptions.InternalCall)]
            void GetRuntimeClassName([MarshalAs(UnmanagedType.BStr)] out string className);

            [MethodImpl(MethodImplOptions.InternalCall)]
            void GetTrustLevel(out TrustLevel TrustLevel);

            [MethodImpl(MethodImplOptions.InternalCall)]
            bool TryShow();

            [MethodImpl(MethodImplOptions.InternalCall)]
            bool TryHide();
        }
    }
}
