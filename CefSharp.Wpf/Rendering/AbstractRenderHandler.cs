//版权所有 © 2019 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Rect = CefSharp.Structs.Rect;

namespace CefSharp.Wpf.Rendering
{
    /// <summary>
    /// 实现 <see cref="IRenderHandler"/> 的基础知识
    ///</摘要>
    ///<另见 cref="CefSharp.Wpf.IRenderHandler" />
    public abstract class AbstractRenderHandler : IDisposable, IRenderHandler
    {
        internal static readonly PixelFormat PixelFormat = PixelFormats.Pbgra32;
        internal static int BytesPerPixel = PixelFormat.BitsPerPixel / 8;

        protected object lockObject = new object();

        protected Size viewSize;
        protected Size popupSize;
        protected DispatcherPriority dispatcherPriority;

        protected MemoryMappedFile viewMemoryMappedFile;
        protected MemoryMappedFile popupMemoryMappedFile;
        protected MemoryMappedViewAccessor viewMemoryMappedViewAccessor;
        protected MemoryMappedViewAccessor popupMemoryMappedViewAccessor;

        /// <summary>
        /// 处置的值，如果它是 1（一），则该实例要么被处置
        ///或正在处理中
        /// </summary>
        private int disposeSignaled;

        /// <summary>
        ///获取一个值，该值指示此实例是否已释放。
        ///</摘要>
        ///<value><see langword="true"/> 如果此实例已释放；否则，<参见 langword="true"/>.</value>
        public bool IsDisposed
        {
            get
            {
                return Interlocked.CompareExchange(ref disposeSignaled, 1, 1) == 1;
            }
        }

        /// <summary>
        /// 释放 <see cref="AbstractRenderHandler"/> 使用的所有资源 object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放 <see cref="AbstractRenderHandler"/> 的非托管资源和（可选）托管资源
        ///</摘要>
        ///<param name="dispose"><see langword="true" /> 释放托管和非托管资源； <see langword="false" /> 仅释放非托管资源.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.CompareExchange(ref disposeSignaled, 1, 0) != 0)
            {
                return;
            }

            if (!disposing)
            {
                return;
            }

            lock (lockObject)
            {
                ReleaseMemoryMappedView(ref popupMemoryMappedFile, ref popupMemoryMappedViewAccessor);
                ReleaseMemoryMappedView(ref viewMemoryMappedFile, ref viewMemoryMappedViewAccessor);
            }
        }

        protected void ReleaseMemoryMappedView(ref MemoryMappedFile mappedFile, ref MemoryMappedViewAccessor stream)
        {
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }

            if (mappedFile != null)
            {
                mappedFile.Dispose();
                mappedFile = null;
            }
        }

        /// </<inheritdoc/>
        public virtual void OnAcceleratedPaint(bool isPopup, Rect dirtyRect, AcceleratedPaintInfo acceleratedPaintInfo)
        {
            // 不曾用过
        }

        /// <summary>
        /// 当应该绘制元素时调用。 （从 CefRenderHandler.OnPaint 调用）
        ///仅当 <see cref="IWindowInfo.SharedTextureEnabled"/> 设置为 false 时才会调用此方法。
        ///</摘要>
        ///<param name="isPopup">指示该元素是视图还是弹出窗口小部件。</param>
        ///<param name="dirtyRect">包含需要重新绘制的像素坐标中的矩形集合</param>
        ///<param name="buffer">位图大小为宽度 *高度 *4 字节，表示具有左上角原点的 BGRA 图像</param>
        ///<param name="width">宽度</param>
        ///<param name="height">高度</param>
        ///<param name="image">用作渲染位图父级的图像</param>
        public virtual void OnPaint(bool isPopup, Rect dirtyRect, IntPtr buffer, int width, int height, Image image)
        {
            if (IsDisposed || image.Dispatcher.HasShutdownStarted)
            {
                return;
            }

            if (isPopup)
            {
                CreateOrUpdateBitmap(isPopup, dirtyRect, buffer, width, height, image, ref popupSize, ref popupMemoryMappedFile, ref popupMemoryMappedViewAccessor);
            }
            else
            {
                CreateOrUpdateBitmap(isPopup, dirtyRect, buffer, width, height, image, ref viewSize, ref viewMemoryMappedFile, ref viewMemoryMappedViewAccessor);
            }
        }

        protected abstract void CreateOrUpdateBitmap(bool isPopup, Rect dirtyRect, IntPtr buffer, int width, int height, Image image, ref Size currentSize, ref MemoryMappedFile mappedFile, ref MemoryMappedViewAccessor viewAccessor);
    }
}
