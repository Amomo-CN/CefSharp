//版权所有 © 2019 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using CefSharp.Enums;
using CefSharp.Structs;
using System;
using System.IO;

namespace CefSharp.Example.Handlers
{
    public class AudioHandler : Handler.AudioHandler
    {
        private ChannelLayout channelLayout;
        private int channelCount;
        private int sampleRate;
        private readonly FileStream rawAudioFile;

        public AudioHandler(string path) : base()
        {
            // 包含原始音频数据（PCM、32 位、浮点）的输出文件将保存在此路径中
            this.rawAudioFile = new FileStream(path, FileMode.Create, FileAccess.Write);
        }

        protected override bool GetAudioParameters(IWebBrowser chromiumWebBrowser, IBrowser browser, ref AudioParameters parameters)
        {
            // 返回 true 以激活音频流捕获
            return true;
        }

        protected override void OnAudioStreamStarted(IWebBrowser chromiumWebBrowser, IBrowser browser, AudioParameters parameters, int channels)
        {
            this.channelLayout = parameters.ChannelLayout;
            this.sampleRate = parameters.SampleRate;
            this.channelCount = channels;
        }

        protected override void OnAudioStreamPacket(IWebBrowser chromiumWebBrowser, IBrowser browser, IntPtr data, int noOfFrames, long pts)
        {
            /*
             * 注意：data 是一个数组，表示原始 PCM 数据为浮点类型，即 4 字节值
             *基于 noOfFrames 和传递给 IAudioHandler.OnAudioStreamStarted 的通道值
             *您可以计算数据数组的大小（以字节为单位）。
             *
             *音频数据（PCM、32 位、浮点）将保存到 rawAudioFile 流中。
             */

            unsafe
            {
                float** channelData = (float**)data.ToPointer();
                int size = channelCount * noOfFrames * 4;
                byte[] samples = new byte[size];
                fixed (byte* pDestByte = samples)
                {
                    float* pDest = (float*)pDestByte;

                    for (int i = 0; i < noOfFrames; i++)
                    {
                        for (int c = 0; c < channelCount; c++)
                        {
                            *pDest++ = channelData[c][i];
                        }
                    }
                }
                rawAudioFile.Write(samples, 0, size);
            }
        }

        protected override void OnAudioStreamStopped(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            base.OnAudioStreamStopped(chromiumWebBrowser, browser);
        }

        protected override void OnAudioStreamError(IWebBrowser chromiumWebBrowser, IBrowser browser, string errorMessage)
        {
            base.OnAudioStreamError(chromiumWebBrowser, browser, errorMessage);
        }

        protected override void Dispose(bool disposing)
        {
            rawAudioFile.Close();
            base.Dispose(disposing);
        }
    }
}
