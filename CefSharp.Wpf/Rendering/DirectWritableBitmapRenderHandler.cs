//版权所有 © 2019 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Rect = CefSharp.Structs.Rect;

namespace CefSharp.Wpf.Rendering
{
    /// <summary>
    /// DirectWritableBitmapRenderHandler -直接复制缓冲区
    ///进入 writeableBitmap.BackBuffer。不使用额外的副本或锁定。
    ///仅当 CEF UI 线程和 WPF UI 线程相同时才能使用 (MultiThreadedMessageLoop = false)
    ///</摘要>
    ///<seealso cref="CefSharp.Wpf.IRenderHandler" />
    public class DirectWritableBitmapRenderHandler : IRenderHandler
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
        public DirectWritableBitmapRenderHandler(double dpiX, double dpiY, bool invalidateDirtyRect = true, DispatcherPriority dispatcherPriority = DispatcherPriority.Render)
        {
            if (!Cef.CurrentlyOnThread(CefThreadIds.TID_UI))
            {
                throw new NotSupportedException("仅当 CEF 集成到 WPF 消息循环中时才能使用 (MultiThreadedMessageLoop = false)。");
            }

            this.dpiX = dpiX;
            this.dpiY = dpiY;
            this.invalidateDirtyRect = invalidateDirtyRect;
        }

        /// </<inheritdoc/>
        void IDisposable.Dispose()
        {

        }

        /// </<inheritdoc/>
        void IRenderHandler.OnAcceleratedPaint(bool isPopup, Rect dirtyRect, AcceleratedPaintInfo acceleratedPaintInfo)
        {
            throw new NotImplementedException();
        }

        /// </<inheritdoc/>
        void IRenderHandler.OnPaint(bool isPopup, Rect dirtyRect, IntPtr buffer, int width, int height, Image image)
        {
            var writeableBitmap = image.Source as WriteableBitmap;
            if (writeableBitmap == null || writeableBitmap.PixelWidth != width || writeableBitmap.PixelHeight != height)
            {
                image.Source = writeableBitmap = new WriteableBitmap(width, height, dpiX, dpiY, AbstractRenderHandler.PixelFormat, null);
            }

            if (writeableBitmap != null)
            {
                writeableBitmap.Lock();

                NativeMethodWrapper.MemoryCopy(writeableBitmap.BackBuffer, buffer, writeableBitmap.BackBufferStride * writeableBitmap.PixelHeight);

                if (invalidateDirtyRect)
                {
                    writeableBitmap.AddDirtyRect(new Int32Rect(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height));
                }
                else
                {
                    writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight));
                }
                writeableBitmap.Unlock();
            }
        }
    }
}

