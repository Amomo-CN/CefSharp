//版权所有 © 2017 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Size = System.Windows.Size;

namespace CefSharp.Wpf.Example.Controls
{
    /// <summary>
    /// 支持屏幕截图的示例 -改编自 https://github.com/cefsharp/CefSharp/pull/462/
    /// </summary>
    public class ChromiumWebBrowserWithScreenshotSupport : ChromiumWebBrowser
    {
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        private static readonly PixelFormat PixelFormat = PixelFormats.Pbgra32;
        private static readonly int BytesPerPixel = PixelFormat.BitsPerPixel / 8;

        private volatile bool isTakingScreenshot = false;
        private Size? screenshotSize;
        private int oldFrameRate;
        private int ignoreFrames = 0;
        private TaskCompletionSource<InteropBitmap> screenshotTaskCompletionSource;
        private CancellationTokenRegistration? cancellationTokenRegistration;

        public ICommand ScreenshotCommand { get; set; }

        public ChromiumWebBrowserWithScreenshotSupport() : base()
        {
            ScreenshotCommand = new RelayCommand(TakeScreenshot);
        }

        public Task<InteropBitmap> TakeScreenshot(Size screenshotSize, int? frameRate = 1, int? ignoreFrames = 0, CancellationToken? cancellationToken = null)
        {
            if (screenshotTaskCompletionSource != null && screenshotTaskCompletionSource.Task.Status == TaskStatus.Running)
            {
                throw new Exception("屏幕截图已在进行中，您必须等待上一个屏幕截图完成");
            }

            if (IsBrowserInitialized == false)
            {
                throw new Exception("浏览器尚未完成初始化或正在处置");
            }

            if (IsLoading)
            {
                throw new Exception("浏览器加载时无法截图");
            }

            var browserHost = this.GetBrowserHost();

            if (browserHost == null)
            {
                throw new Exception("浏览器主机为空");
            }

            screenshotTaskCompletionSource = new TaskCompletionSource<InteropBitmap>(TaskCreationOptions.RunContinuationsAsynchronously);

            if (cancellationToken.HasValue)
            {
                var token = cancellationToken.Value;
                cancellationTokenRegistration = token.Register(() =>
                {
                    screenshotTaskCompletionSource.TrySetCanceled();

                    cancellationTokenRegistration?.Dispose();

                }, useSynchronizationContext: false);
            }

            if (frameRate.HasValue)
            {
                oldFrameRate = browserHost.WindowlessFrameRate;
                browserHost.WindowlessFrameRate = frameRate.Value;
            }

            this.screenshotSize = screenshotSize;
            this.isTakingScreenshot = true;
            this.ignoreFrames = ignoreFrames.GetValueOrDefault() < 0 ? 0 : ignoreFrames.GetValueOrDefault();
            //使用所需的屏幕截图尺寸调整浏览器大小
            //生成的位图永远不会渲染到屏幕上
            browserHost.WasResized();

            return screenshotTaskCompletionSource.Task;
        }

        protected override CefSharp.Structs.Rect GetViewRect()
        {
            if (isTakingScreenshot)
            {
                return new CefSharp.Structs.Rect(0, 0, (int)Math.Ceiling(screenshotSize.Value.Width), (int)Math.Ceiling(screenshotSize.Value.Height));
            }

            return base.GetViewRect();
        }

        protected override void OnPaint(bool isPopup, Structs.Rect dirtyRect, IntPtr buffer, int width, int height)
        {
            if (isTakingScreenshot)
            {
                //我们忽略前n帧
                if (ignoreFrames > 0)
                {
                    ignoreFrames--;
                    return;
                }

                //等到我们有一个与我们请求的更新尺寸相匹配的框架
                if (screenshotSize.HasValue && screenshotSize.Value.Width == width && screenshotSize.Value.Height == height)
                {
                    var stride = width * BytesPerPixel;
                    var numberOfBytes = stride * height;

                    //为屏幕截图创建自己的内存映射视图并将缓冲区复制到其中。
                    //如果我们要创建很多屏幕截图，那么最好分配一个大的缓冲区
                    //并重用它。
                    var mappedFile = MemoryMappedFile.CreateNew(null, numberOfBytes, MemoryMappedFileAccess.ReadWrite);
                    var viewAccessor = mappedFile.CreateViewAccessor();

                    CopyMemory(viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle(), buffer, (uint)numberOfBytes);

                    //位图需要在 UI 线程上创建
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        var backBuffer = mappedFile.SafeMemoryMappedFileHandle.DangerousGetHandle();
                        //NOTE:Interopbitmap 不支持 DPI 缩放
                        var bitmap = (InteropBitmap)Imaging.CreateBitmapSourceFromMemorySection(backBuffer,
                            width, height, PixelFormat, stride, 0);
                        screenshotTaskCompletionSource.TrySetResult(bitmap);

                        isTakingScreenshot = false;
                        var browserHost = GetBrowser().GetHost();
                        //将帧速率返回到之前的值
                        browserHost.WindowlessFrameRate = oldFrameRate;
                        //让浏览器知道尺寸变化，以便正常渲染可以继续
                        browserHost.WasResized();

                        viewAccessor?.Dispose();
                        mappedFile?.Dispose();

                        cancellationTokenRegistration?.Dispose();
                    }));
                }
            }
            else
            {
                base.OnPaint(isPopup, dirtyRect, buffer, width, height);
            }
        }

        private void TakeScreenshot()
        {
            var uiThreadTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            const string script = "[document.body.scrollWidth, document.body.scrollHeight]";
            this.EvaluateScriptAsync(script).ContinueWith((scriptTask) =>
            {
                var javascriptResponse = scriptTask.Result;

                if (javascriptResponse.Success)
                {
                    var widthAndHeight = (List<object>)javascriptResponse.Result;

                    var screenshotSize = new Size((int)widthAndHeight[0], (int)widthAndHeight[1]);

                    TakeScreenshot(screenshotSize, ignoreFrames: 0).ContinueWith((screenshotTask) =>
                     {
                         if (screenshotTask.Status == TaskStatus.RanToCompletion)
                         {
                             try
                             {
                                 var bitmap = screenshotTask.Result;
                                 var tempFile = Path.GetTempFileName().Replace(".tmp", ".png");
                                 using (var stream = new FileStream(tempFile, FileMode.Create))
                                 {
                                     var encoder = new PngBitmapEncoder();
                                     encoder.Frames.Add(BitmapFrame.Create(bitmap));
                                     encoder.Save(stream);
                                 }

                                 Process.Start(new ProcessStartInfo
                                 {
                                     UseShellExecute = true,
                                     FileName = tempFile
                                 });
                             }
                             catch (Exception ex)
                             {
                                 var msg = ex.ToString();
                             }
                         }
                         else
                         {
                             MessageBox.Show("无法捕获屏幕截图");
                         }
                     }, uiThreadTaskScheduler); //确保 Continuation 在 UI 线程上运行

                }
                else
                {
                    MessageBox.Show("无法获取截图大小");
                }
            }, uiThreadTaskScheduler);
        }
    }
}
