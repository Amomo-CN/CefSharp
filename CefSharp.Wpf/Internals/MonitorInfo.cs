//版权所有 © 2018 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Runtime.InteropServices;

namespace CefSharp.Wpf.Internals
{
    /// <summary>
    /// MonitorInfo 是 MonitorFromWindow 和 GetMonitorInfo 的包装类
    /// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-monitorfromwindow
    /// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getmonitorinfoa
    /// </summary>
    internal static class MonitorInfo
    {
        private const int MONITOR_DEFAULTTONULL = 0;
        private const int MONITOR_DEFAULTTOPRIMARY = 1;
        private const int MONITOR_DEFAULTTONEAREST = 2;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx lpmi);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        /// <summary>
        /// 获取所提供的窗口句柄的监视器信息
        ///</摘要>
        ///<param name="windowHandle">窗口句柄</param>
        ///<param name="monitorInfo">监控信息</param>
        internal static void GetMonitorInfoForWindowHandle(IntPtr windowHandle, ref MonitorInfoEx monitorInfo)
        {
            var hMonitor = MonitorFromWindow(windowHandle, MONITOR_DEFAULTTONEAREST);
            GetMonitorInfo(hMonitor, ref monitorInfo);
        }
    }
}
