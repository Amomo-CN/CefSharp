//版权所有 © 2018 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Rect = CefSharp.Structs.Rect;

namespace CefSharp.Wpf.Rendering.Experimental
{
    /// <summary>
    /// ByteArrayWritableBitmapRenderHandler -创建/更新 WritableBitmap
    ///对于每个 OnPaint 调用，都会创建一个新的 byte[]，然后进行更新。没有锁定是
    ///执行并为每个 OnPaint 调用分配内存，因此内存将非常昂贵
    ///明智的。
    /// </summary>
    /// <seealso cref="CefSharp.Wpf.IRenderHandler" />
    public class ByteArrayWritableBitmapRenderHandler : IRenderHandler
    {
        private readonly double dpiX;
        private readonly double dpiY;
        private readonly bool invalidateDirtyRect;
        private readonly DispatcherPriority dispatcherPriority;

        /// <summary>
        /// 初始化 <see cref="WritableBitmapRenderHandler"/> 类的新实例。
        ///</摘要>
        ///<param name="dpiX">dpi x.</param>
        ///<param name="dpiY">dpi y。</param>
        ///<param name="invalidateDirtyRect">如果为 true 则仅更新直接矩形，否则将重绘整个位图</param>
        ///<param name="dispatcherPriority">UI 线程上更新位图的优先级</param>
        public ByteArrayWritableBitmapRenderHandler(double dpiX, double dpiY, bool invalidateDirtyRect = true, DispatcherPriority dispatcherPriority = DispatcherPriority.Render)
        {
            this.dpiX = dpiX;
            this.dpiY = dpiY;
            this.invalidateDirtyRect = invalidateDirtyRect;
            this.dispatcherPriority = dispatcherPriority;
        }

        /// </<inheritdoc/>
        void IRenderHandler.OnAcceleratedPaint(bool isPopup, Rect dirtyRect, AcceleratedPaintInfo acceleratedPaintInfo)
        {
            //不曾用过
        }

        void IRenderHandler.OnPaint(bool isPopup, Rect dirtyRect, IntPtr buffer, int width, int height, Image image)
        {
            if (image.Dispatcher.HasShutdownStarted)
            {
                return;
            }

            int pixels = width * height;
            int numberOfBytes = pixels * AbstractRenderHandler.BytesPerPixel;
            var stride = width * AbstractRenderHandler.BytesPerPixel;
            var tempBuffer = new byte[numberOfBytes];

            //将非托管内存复制到我们的缓冲区
            Marshal.Copy(buffer, tempBuffer, 0, numberOfBytes);

            image.Dispatcher.BeginInvoke((Action)(() =>
            {
                var bitmap = image.Source as WriteableBitmap;

                if (bitmap == null || bitmap.PixelHeight != height || bitmap.PixelWidth != width)
                {
                    if (image.Source != null)
                    {
                        image.Source = null;
                        GC.Collect(1);
                    }

                    image.Source = bitmap = new WriteableBitmap(width, height, dpiX, dpiY, AbstractRenderHandler.PixelFormat, null);
                }

                //获取临时缓冲区的指针
                var tempBufferPtr = Marshal.UnsafeAddrOfPinnedArrayElement(tempBuffer, 0);

                //默认情况下，我们只会更新那些遇到 MILERR_WIN32ERROR 异常的脏矩形 (#2035)
                //最好升级到更新的.Net版本（只需要安装客户端运行时，不需要编译
                //针对较新的版本。或者使整个位图无效
                if (invalidateDirtyRect)
                {
                    // 更新脏区
                    var sourceRect = new Int32Rect(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height);

                    bitmap.Lock();
                    bitmap.WritePixels(sourceRect, tempBufferPtr, numberOfBytes, stride, dirtyRect.X, dirtyRect.Y);
                    bitmap.Unlock();
                }
                else
                {
                    // 更新整个位图
                    var sourceRect = new Int32Rect(0, 0, width, height);

                    bitmap.Lock();
                    bitmap.WritePixels(sourceRect, tempBufferPtr, numberOfBytes, stride);
                    bitmap.Unlock();
                }
            }), dispatcherPriority);
        }

        void IDisposable.Dispose()
        {

        }
    }
}
