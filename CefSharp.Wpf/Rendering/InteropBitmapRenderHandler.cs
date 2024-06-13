//版权所有 © 2018 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.IO.MemoryMappedFiles;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;
using Rect = CefSharp.Structs.Rect;

namespace CefSharp.Wpf.Rendering
{
    /// <summary>
    /// InteropBitmapRenderHandler -创建/更新 InteropBitmap
    ///当大小匹配时使用 MemoryMappedFile 进行双缓冲
    ///或在需要时创建一个新的 InteropBitmap
    /// </summary>
    /// <seealso cref="CefSharp.Wpf.IRenderHandler" />
    public class InteropBitmapRenderHandler : AbstractRenderHandler
    {
        /// <summary>
        ///初始化 <see cref="InteropBitmapRenderHandler"/> 类的新实例。
        ///</摘要>
        ///<param name="dispatcherPriority">UI 线程上更新位图的优先级</param>
        public InteropBitmapRenderHandler(DispatcherPriority dispatcherPriority = DispatcherPriority.Render)
        {
            this.dispatcherPriority = dispatcherPriority;
        }

        protected override void CreateOrUpdateBitmap(bool isPopup, Rect dirtyRect, IntPtr buffer, int width, int height, Image image, ref Size currentSize, ref MemoryMappedFile mappedFile, ref MemoryMappedViewAccessor viewAccessor)
        {
            var createNewBitmap = false;

            lock (lockObject)
            {
                int pixels = width * height;
                int numberOfBytes = pixels * BytesPerPixel;

                createNewBitmap = mappedFile == null || currentSize.Height != height || currentSize.Width != width;

                if (createNewBitmap)
                {
                    ReleaseMemoryMappedView(ref mappedFile, ref viewAccessor);

                    mappedFile = MemoryMappedFile.CreateNew(null, numberOfBytes, MemoryMappedFileAccess.ReadWrite);

                    viewAccessor = mappedFile.CreateViewAccessor();

                    currentSize.Height = height;
                    currentSize.Width = width;
                }

                NativeMethodWrapper.MemoryCopy(viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle(), buffer, numberOfBytes);

                //引用 backBufferHandle，一旦我们进入 UI 线程，我们需要检查它是否仍然有效
                var backBufferHandle = mappedFile.SafeMemoryMappedFileHandle;

                //在 WPF UI 线程上调用
                image.Dispatcher.BeginInvoke((Action)(() =>
                {
                    lock (lockObject)
                    {
                        if (backBufferHandle.IsClosed || backBufferHandle.IsInvalid)
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

                        if (createNewBitmap)
                        {
                            if (image.Source != null)
                            {
                                image.Source = null;
                                //TODO: 在较新版本的 .Net 中还需要这样做吗？
                                GC.Collect(1);
                            }

                            var stride = width * BytesPerPixel;
                            var bitmap = (InteropBitmap)Imaging.CreateBitmapSourceFromMemorySection(backBufferHandle.DangerousGetHandle(), width, height, PixelFormat, stride, 0);
                            image.Source = bitmap;
                        }
                        else if (image.Source != null)
                        {
                            var sourceRect = new Int32Rect(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height);
                            var bitmap = (InteropBitmap)image.Source;
                            bitmap.Invalidate(sourceRect);
                        }
                    }
                }), dispatcherPriority);
            }
        }
    }
}
