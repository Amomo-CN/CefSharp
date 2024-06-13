//版权所有 © 2018 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.IO.MemoryMappedFiles;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Rect = CefSharp.Structs.Rect;

namespace CefSharp.Wpf.Rendering
{
    /// <summary>
    ///WritableBitmapRenderHandler -创建/更新 WritableBitmap
    ///当大小匹配时使用 MemoryMappedFile 进行双缓冲
    ///或在需要时创建一个新的 WritableBitmap
    ///</摘要>
    /// <seealso cref="CefSharp.Wpf.IRenderHandler" />
    public class WritableBitmapRenderHandler : AbstractRenderHandler
    {
        private readonly double dpiX;
        private readonly double dpiY;
        private readonly bool invalidateDirtyRect;

        /// <summary>
        /// 初始化 <see cref="WritableBitmapRenderHandler"/> 类的新实例。
        ///</摘要>
        ///<param name="dpiX">dpi x.</param>
        ///<param name="dpiY">dpi y。</param>
        ///<param name="invalidateDirtyRect">如果为 true 则仅更新直接矩形，否则将重绘整个位图</param>
        ///<param name="dispatcherPriority">UI 线程上更新位图的优先级</param>
        public WritableBitmapRenderHandler(double dpiX, double dpiY, bool invalidateDirtyRect = true, DispatcherPriority dispatcherPriority = DispatcherPriority.Render)
        {
            this.dpiX = dpiX;
            this.dpiY = dpiY;
            this.invalidateDirtyRect = invalidateDirtyRect;
            this.dispatcherPriority = dispatcherPriority;
        }

        /// <summary>
        ///当为 true 时，如果脏矩形（要更新的矩形）
        ///小于全宽/高则仅复制脏矩形
        ///从 CEF 本机缓冲区到我们自己的托管缓冲区。
        ///设置为 true 以在仅更新屏幕的一小部分时提高性能。
        ///当前默认为 false。
        /// </summary>
        public bool CopyOnlyDirtyRect { get; set; }

        /// <inheritdoc/>
        protected override void CreateOrUpdateBitmap(bool isPopup, Rect dirtyRect, IntPtr buffer, int width, int height, Image image, ref Size currentSize, ref MemoryMappedFile mappedFile, ref MemoryMappedViewAccessor viewAccessor)
        {
            bool createNewBitmap = false;

            lock (lockObject)
            {
                int pixels = width * height;
                int numberOfBytes = pixels * BytesPerPixel;

                createNewBitmap = mappedFile == null || currentSize.Height != height || currentSize.Width != width;

                if (createNewBitmap)
                {
                    //如果 MemoryMappedFile 小于我们需要的大小，则创建一个更大的文件
                    //如果它更大，那么我们需要然后，而不是经历昂贵的费用
                    //分配一个新的，我们将只使用旧的，并且只访问我们需要的字节数。
                    if (viewAccessor == null || viewAccessor.Capacity < numberOfBytes)
                    {
                        ReleaseMemoryMappedView(ref mappedFile, ref viewAccessor);

                        mappedFile = MemoryMappedFile.CreateNew(null, numberOfBytes, MemoryMappedFileAccess.ReadWrite);

                        viewAccessor = mappedFile.CreateViewAccessor();
                    }

                    currentSize.Height = height;
                    currentSize.Width = width;
                }

                if (CopyOnlyDirtyRect)
                {
                    // 对于完整的缓冲区更新，我们只需执行简单的复制
                    //否则只会更新一部分。
                    if (width == dirtyRect.Width && height == dirtyRect.Height)
                    {
                        NativeMethodWrapper.MemoryCopy(viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle(), buffer, numberOfBytes);
                    }
                    else
                    {
                        //TODO: 我们或许可以在这里进行一些小的优化。
                        //var numberOfBytesToCopy = dirtyRect.Width * BytesPerPixel;
                        //var safeMemoryMappedViewHandle = viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle();

                        //for (int offset = width * dirtyRect.Y + dirtyRect.X; offset < (dirtyRect.Y + dirtyRect.Height) * width; offset += width)
                        //{
                        //    var b = offset * BytesPerPixel;
                        //    NativeMethodWrapper.MemoryCopy(safeMemoryMappedViewHandle + b, buffer + b, numberOfBytesToCopy);
                        //}

                        for (int offset = width * dirtyRect.Y + dirtyRect.X; offset < (dirtyRect.Y + dirtyRect.Height) * width; offset += width)
                        {
                            NativeMethodWrapper.MemoryCopy(viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle() + offset * BytesPerPixel, buffer + offset * BytesPerPixel, dirtyRect.Width * BytesPerPixel);
                        }
                    }
                }
                else
                {
                    NativeMethodWrapper.MemoryCopy(viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle(), buffer, numberOfBytes);
                }

                //引用用于更新 WriteableBitmap 的 sourceBuffer，
                //一旦我们进入 UI 线程，我们需要检查它是否仍然有效
                var sourceBuffer = viewAccessor.SafeMemoryMappedViewHandle;

                image.Dispatcher.BeginInvoke((Action)(() =>
                {
                    lock (lockObject)
                    {
                        if (sourceBuffer.IsClosed || sourceBuffer.IsInvalid)
                        {
                            return;
                        }

                        var size = isPopup ? popupSize : viewSize;

                        //如果之前多次调用OnPaint
                        //我们的 BeginInvoke 调用，我们检查大小是否与最近的大小相匹配
                        //更新，缓冲区已经被覆盖（帧被有效丢弃）
                        //所以我们忽略这个调用
                        //https://github.com/cefsharp/CefSharp/issues/3114
                        if (size.Width != width || size.Height != height)
                        {
                            return;
                        }

                        if (createNewBitmap || image.Source is null)
                        {
                            if (image.Source != null)
                            {
                                image.Source = null;
                                GC.Collect(1);
                            }

                            image.Source = new WriteableBitmap(width, height, dpiX, dpiY, PixelFormat, null);
                        }

                        var stride = width * BytesPerPixel;
                        var noOfBytes = stride * height;

                        var bitmap = (WriteableBitmap)image.Source;

                        //有时大幅调整 ChromiumWebBrowser 的大小时
                        //我们最终可能会得到缓冲区大小与位图大小不匹配的结果
                        //忽略这些帧，因为渲染最终应该会赶上
                        //（CEF可以在WPF执行渲染周期之前生成多个帧）
                        //https://github.com/cefsharp/CefSharp/issues/3474
                        if (width > bitmap.PixelWidth || height > bitmap.PixelHeight)
                        {
                            return;
                        }

                        var sourceBufferPtr = sourceBuffer.DangerousGetHandle();

                        // 问题 https://github.com/cefsharp/CefSharp/issues/4426
                        if (sourceBufferPtr == IntPtr.Zero)
                        {
                            return;
                        }

                        //默认情况下，我们只会更新那些遇到 MILERR_WIN32ERROR 异常的脏矩形 (#2035)
                        //最好升级到更新的.Net版本（只需要安装客户端运行时，不需要编译
                        //针对较新的版本。或者使整个位图无效
                        if (invalidateDirtyRect)
                        {
                            // 更新脏区
                            var sourceRect = new Int32Rect(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height);

                            bitmap.Lock();
                            bitmap.WritePixels(sourceRect, sourceBufferPtr, noOfBytes, stride, dirtyRect.X, dirtyRect.Y);
                            bitmap.Unlock();
                        }
                        else
                        {
                            // 更新整个位图
                            var sourceRect = new Int32Rect(0, 0, width, height);

                            bitmap.Lock();
                            bitmap.WritePixels(sourceRect, sourceBufferPtr, noOfBytes, stride);
                            bitmap.Unlock();
                        }
                    }
                }), dispatcherPriority);
            }
        }
    }
}
