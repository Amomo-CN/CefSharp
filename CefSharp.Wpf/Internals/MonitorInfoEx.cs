//版权所有 © 2018 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System.Runtime.InteropServices;

namespace CefSharp.Wpf.Internals
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct MonitorInfoEx
    {
        private const int CCHDEVICENAME = 32;
        /// <summary>
        /// 结构的大小（以字节为单位）。在调用 GetMonitorInfo 函数之前，将此成员设置为 sizeof(MONITORINFOEX) (72)。 
        ///这样做可以让函数确定您传递给它的结构类型。
        /// </summary>
        public int Size;

        /// <summary>
        ///一个 RECT 结构，指定显示监视器矩形，以虚拟屏幕坐标表示。 
        ///请注意，如果显示器不是主显示显示器，则某些矩形的坐标可能为负值。
        /// </summary>
        public RectStruct Monitor;

        /// <summary>
        /// 一个RECT结构，指定应用程序可以使用的显示监视器的工作区域矩形， 
        ///以虚拟屏幕坐标表示。 Windows 使用此矩形来最大化显示器上的应用程序。 
        ///rcMonitor 中的其余区域包含系统窗口，例如任务栏和侧边栏。 
        ///请注意，如果显示器不是主显示显示器，则某些矩形的坐标可能为负值。
        ///</摘要>
        public RectStruct WorkArea;

        /// <summary>
        ///显示监视器的属性。
        ///
        ///该成员可以是以下值：
        ///1 : MONITORINFOF_PRIMARY
        /// </summary>
        public uint Flags;

        /// <summary>
        /// 指定正在使用的监视器的设备名称的字符串。大多数应用程序没有使用显示监视器名称， 
        ///因此可以通过使用 MONITORINFO 结构来节省一些字节。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        public string DeviceName;

        public void Init()
        {
            Size = 40 + 2 * CCHDEVICENAME;
            DeviceName = string.Empty;
        }
    }
}
