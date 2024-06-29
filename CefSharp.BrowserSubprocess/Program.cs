// Copyright © 2013 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System.Diagnostics;
using CefSharp.RenderProcess;

namespace CefSharp.BrowserSubprocess
{
    /// <summary>
    ///实现自己的BrowserSubprocess时
    ///-对于完整的 .Net 使用 <see cref="WcfBrowserSubprocessExecutable"/>
    ///-对于.Net Core 使用 <see cref="BrowserSubprocessExecutable"/> （无 WCF 支持）
    ///-包含带有 dpi/compatibility 部分的 app.manifest，这是必需的（该项目包含相关内容）。
    ///-如果您的目标是 x86/Win32 那么您应该设置 /LargeAddressAware (https://docs.microsoft.com/en-us/cpp/build/reference/largeaddressaware?view=vs-2017)
    /// </summary>
    public class Program
    {
        public static int Main(string[] args)
        {
            Debug.WriteLine("BrowserSubprocess starting up with command line: " + string.Join("\n", args));

            //Add your own custom implementation of IRenderProcessHandler here
            IRenderProcessHandler handler = null;

            //WcfBrowserSubprocessExecutable提供BrowserSubProcess功能
            //特定于CefSharp，WCF支持（Sync JSB所需）将可选
            //如果存在 CefSharpArguments.WcfEnabledArgument 命令行参数则启用
            //对于.Net Core，使用BrowserSubprocessExecutable，因为没有WCF支持
            var browserProcessExe = new WcfBrowserSubprocessExecutable();
            var result = browserProcessExe.Main(args, handler);

            Debug.WriteLine("BrowserSubprocess shutting down.");

            return result;
        }
    }
}
