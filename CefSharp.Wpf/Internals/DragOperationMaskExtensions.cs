//版权所有 © 2019 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System.Windows;

using CefSharp.Enums;

namespace CefSharp.Wpf.Internals
{
    internal static class DragOperationMaskExtensions
    {
        /// <summary>
        ///将 .NET 拖放效果转换为 CEF 拖动操作
        ///</摘要>
        ///<param name="dragDropEffects">拖放效果。</param>
        ///<returns>DragOperationsMask.</returns>
        public static DragOperationsMask GetDragOperationsMask(this DragDropEffects dragDropEffects)
        {
            var operations = DragOperationsMask.None;

            if (dragDropEffects.HasFlag(DragDropEffects.All))
            {
                operations |= DragOperationsMask.Every;
            }
            if (dragDropEffects.HasFlag(DragDropEffects.Copy))
            {
                operations |= DragOperationsMask.Copy;
            }
            if (dragDropEffects.HasFlag(DragDropEffects.Move))
            {
                operations |= DragOperationsMask.Move;
            }
            if (dragDropEffects.HasFlag(DragDropEffects.Link))
            {
                operations |= DragOperationsMask.Link;
            }

            return operations;
        }

        /// <summary>
        /// 获取拖动效果。
        ///</摘要>
        ///<param name="mask">掩码。</param>
        ///<returns>DragDropEffects.</returns>
        public static DragDropEffects GetDragEffects(this DragOperationsMask mask)
        {
            if ((mask & DragOperationsMask.Every) == DragOperationsMask.Every)
            {
                // return all effects (!= DragDropEffects.All, which doesn't include Link)
                return DragDropEffects.Scroll | DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
            }
            if ((mask & DragOperationsMask.Copy) == DragOperationsMask.Copy)
            {
                return DragDropEffects.Copy;
            }
            if ((mask & DragOperationsMask.Move) == DragOperationsMask.Move)
            {
                return DragDropEffects.Move;
            }
            if ((mask & DragOperationsMask.Link) == DragOperationsMask.Link)
            {
                return DragDropEffects.Link;
            }
            return DragDropEffects.None;
        }
    }
}
