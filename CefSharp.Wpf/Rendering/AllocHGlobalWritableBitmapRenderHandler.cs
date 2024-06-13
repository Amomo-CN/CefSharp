//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Rect = CefSharp.Structs.Rect;

namespace CefSharp.Wpf.Rendering
{
    /// <summary>
    ///AllocHGlobalWritableBitmapRenderHandler -创建/更新 WritableBitmap
    ///使用 <see cref="Marshal.AllocHGlobal(int)"/> 分配内存
    ///当大小匹配或创建新的 WritableBitmap 时进行双缓冲
    ///在需要的时候。
    ///</摘要>
    ///<另见 cref="IRenderHandler" />
    public class AllocHGlobalWritableBitmapRenderHandler : IRenderHandler
    {
        private readonly double dpiX;
        private readonly double dpiY;
        private readonly PaintElement view;
        private readonly PaintElement popup;
        private readonly DispatcherPriority dispatcherPriority;
        private readonly object lockObject = new object();

        /// <summary>
        /// 处置的值，如果它是 1（一），则该实例要么被处置
        ///或正在处理中
        /// </summary>
        private int disposeSignaled;

        /// <summary>
        /// 初始化 <see cref="AllocHGlobalWritableBitmapRenderHandler"/> 类的新实例。
        ///</摘要>
        ///<param name="dpiX">dpi x.</param>
        ///<param name="dpiY">dpi y。</param>
        ///<param name="invalidateDirtyRect">如果为 true 则仅更新直接矩形，否则将重绘整个位图</param>
        ///<param name="dispatcherPriority">UI 线程上更新位图的优先级</param>
        public AllocHGlobalWritableBitmapRenderHandler(double dpiX, double dpiY, bool invalidateDirtyRect = true, DispatcherPriority dispatcherPriority = DispatcherPriority.Render)
        {
            this.dpiX = dpiX;
            this.dpiY = dpiY;
            this.dispatcherPriority = dispatcherPriority;

            view = new PaintElement(dpiX, dpiY, invalidateDirtyRect);
            popup = new PaintElement(dpiX, dpiY, invalidateDirtyRect);
        }

        /// <summary>
        /// 获取一个值，该值指示此实例是否已释放。
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
        /// </summary>
        /// <param name="dispose"><see langword="true" /> 释放托管和非托管资源； <see langword="false" /> 仅释放非托管资源。</param>
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
                view?.Dispose();
                popup?.Dispose();
            }
        }

        /// </<inheritdoc/>
        void IRenderHandler.OnAcceleratedPaint(bool isPopup, Rect dirtyRect, AcceleratedPaintInfo acceleratedPaintInfo)
        {
            //不曾用过
        }

        void IRenderHandler.OnPaint(bool isPopup, Rect dirtyRect, IntPtr buffer, int width, int height, Image image)
        {
            lock (lockObject)
            {
                if (IsDisposed || image.Dispatcher.HasShutdownStarted)
                {
                    return;
                }

                var paintElement = isPopup ? popup : view;
                paintElement?.UpdateBuffer(dirtyRect, buffer, width, height, image);
                paintElement?.UpdateImage(lockObject);
            }
        }

        //TODO：没有嵌套类，并且该类有更好的名称
        ///<摘要>
        ///要渲染的位图的详细信息
        /// </summary>
        private class PaintElement
        {
            private readonly double dpiX;
            private readonly double dpiY;
            private Image image;
            private int width;
            private int height;
            private Rect dirtyRect;
            private IntPtr buffer;
            private int bufferSize;
            private int imageSize;
            private readonly bool invalidateDirtyRect;
            internal bool IsDirty { get; set; }

            internal PaintElement(double dpiX, double dpiY, bool invalidateDirtyRect)
            {
                this.dpiX = dpiX;
                this.dpiY = dpiY;
                this.invalidateDirtyRect = invalidateDirtyRect;
            }

            internal void Dispose()
            {
                image = null;

                if (buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buffer);
                    buffer = IntPtr.Zero;
                }
            }

            internal void UpdateBuffer(Rect dirtyRect, IntPtr sourceBuffer, int width, int height, Image image)
            {
                imageSize = (width * height) * AbstractRenderHandler.BytesPerPixel;

                if (bufferSize < imageSize)
                {
                    Marshal.FreeHGlobal(buffer);
                    buffer = Marshal.AllocHGlobal(imageSize);
                    bufferSize = imageSize;
                }

                this.width = width;
                this.height = height;
                this.dirtyRect = dirtyRect;

                NativeMethodWrapper.MemoryCopy(buffer, sourceBuffer, imageSize);

                this.image = image;
                IsDirty = true;
            }

            internal void UpdateImage(object lockObject)
            {
                image.Dispatcher.BeginInvoke((Action)(() =>
                {
                    lock (lockObject)
                    {
                        //如果 OnPaint 在 BeginInvoke 调用之前被调用了几次
                        //我们可以无事可做地结束在这里。
                        if (IsDirty && image != null)
                        {
                            var bitmap = image.Source as WriteableBitmap;
                            var createNewBitmap = bitmap == null || bitmap.PixelWidth != width || bitmap.PixelHeight != height;
                            if (createNewBitmap)
                            {
                                if (image.Source != null)
                                {
                                    image.Source = null;
                                    GC.Collect(1);
                                }

                                image.Source = bitmap = new WriteableBitmap(width, height, dpiX, dpiY, AbstractRenderHandler.PixelFormat, null);
                            }

                            if (bitmap != null)
                            {
                                //默认情况下，我们只会更新那些遇到 MILERR_WIN32ERROR 异常的脏矩形 (#2035)
                                //最好升级到更新的.Net版本（只需要安装客户端运行时，不需要编译
                                //针对较新的版本。或者使整个位图无效
                                if (invalidateDirtyRect)
                                {
                                    // 更新脏区
                                    var sourceRect = new Int32Rect(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height);

                                    bitmap.Lock();
                                    bitmap.WritePixels(sourceRect, buffer, imageSize, width * AbstractRenderHandler.BytesPerPixel, sourceRect.X, sourceRect.Y);
                                    bitmap.Unlock();
                                }
                                else
                                {
                                    // 更新整个位图
                                    var sourceRect = new Int32Rect(0, 0, width, height);

                                    bitmap.Lock();
                                    bitmap.WritePixels(sourceRect, buffer, imageSize, width * AbstractRenderHandler.BytesPerPixel, sourceRect.X, sourceRect.Y);
                                    bitmap.Unlock();
                                }
                            }

                            IsDirty = false;
                        }
                    }
                }));
            }
        };
    }
}
