//版权所有 © 2014 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System.Collections.Generic;
using System.IO;
using CefSharp.Handler;

namespace CefSharp.Example.Handlers
{
    public class TempFileDialogHandler : DialogHandler
    {
        protected override bool OnFileDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, IReadOnlyCollection<string> acceptFilters, IReadOnlyCollection<string> acceptExtensions, IReadOnlyCollection<string> acceptDescriptions, IFileDialogCallback callback)
        {
            callback.Continue(new List<string> { Path.GetRandomFileName() });

            return true;
        }
    }
}
