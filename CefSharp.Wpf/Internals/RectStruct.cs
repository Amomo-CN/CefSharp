//版权所有 © 2018 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System.Runtime.InteropServices;

using CefSharp.Structs;

namespace CefSharp.Wpf.Internals
{
    /// <summary>
    /// RECT结构定义了矩形的左上角和右下角的坐标。
    ///</摘要>
    ///<参见 cref="https://docs.microsoft.com/en-us/previous-versions/dd162897(v=vs.85)"/>
    ///<备注>
    ///按照惯例，矩形的右边缘和下边缘通常被认为是排他的。 
    ///换句话说，坐标为（right,bottom）的像素紧邻矩形的外部。
    ///例如，当 RECT 传递给 FillRect 函数时，矩形将被填充到（但不包括） 
    ///右列和底行像素。该结构与 RECTL 结构相同。
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct RectStruct
    {
        /// <summary>
        /// 矩形左上角的 x 坐标。
        /// </summary>
        public int Left;

        /// <summary>
        /// 矩形左上角的 y 坐标。
        /// </summary>
        public int Top;

        /// <summary>
        /// 矩形右下角的 x 坐标。
        /// </summary>
        public int Right;

        /// <summary>
        /// 矩形右下角的 y 坐标。
        /// </summary>
        public int Bottom;

        public static implicit operator Rect(RectStruct rect)
        {
            return new Rect(0, 0, rect.Right - rect.Left, rect.Bottom - rect.Top);
        }
    }
}
