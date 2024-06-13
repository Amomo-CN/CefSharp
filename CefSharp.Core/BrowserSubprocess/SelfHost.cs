//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

//注意：CefSharp.Core 命名空间中的类已对 intellisnse 隐藏，因此用户不会直接使用它们

using CefSharp.Internals;
using System;
using System.IO;

namespace CefSharp.BrowserSubprocess
{
    /// <summary>
    /// SelfHost 允许您的应用程序可执行文件用作 BrowserSubProcess
    ///以最小的努力。
    /// https://github.com/cefsharp/CefSharp/wiki/SelfHost-BrowserSubProcess
    /// </summary>
    /// <example>
    /// //WinForms 示例
    /// public class Program
    /// {
    ///	  [STAThread]
    ///   public static int Main(string[] args)
    ///   {
    ///     var exitCode = CefSharp.BrowserSubprocess.SelfHost.Main(args);
    ///
    ///     if (exitCode >= 0)
    ///     {
    ///       return exitCode;
    ///     }
    ///
    ///     var settings = new CefSettings();
    ///     //应用程序可执行文件的绝对路径
    ///     settings.BrowserSubprocessPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
    ///
    ///     Cef.Initialize(settings);
    ///
    ///     var browser = new BrowserForm(true);
    ///     Application.Run(browser);
    ///
    ///     return 0;
    ///   }
    /// }
    /// </example>
    public class SelfHost
    {
        /// <summary>
        ///应从应用程序入口点函数（通常为 Program.Main）调用此函数
        ///执行辅助进程，例如GPU、渲染器、实用程序
        ///此重载专门用于.Net Core。用于托管您自己的 BrowserSubProcess
        ///最好使用此类提供的 Main 方法。
        ///-传入命令行参数
        ///</摘要>
        ///<param name="args">命令行参数</param>
        ///<返回>
        ///如果为浏览器进程调用（由无“type”命令行值标识），它将立即返回
        ///值为-1。如果调用一个可识别的辅助进程，它将阻塞，直到该进程退出
        ///然后返回进程退出代码。
        /// </returns>
        public static int Main(string[] args)
        {
            var type = CommandLineArgsParser.GetArgumentValue(args, CefSharpArguments.SubProcessTypeArgument);

            if (string.IsNullOrEmpty(type))
            {
                //如果命令行 CEF/Chromium 假定缺少 --type 参数
                //这是主进程（因为所有子进程都必须有一个类型参数）。
                //返回-1来指示此行为。
                return -1;
            }


#if NETCOREAPP
            var browserSubprocessDllPath = Initializer.BrowserSubProcessCorePath;
            if (!File.Exists(browserSubprocessDllPath))
            {
                browserSubprocessDllPath = Path.Combine(Path.GetDirectoryName(typeof(CefSharp.Core.BrowserSettings).Assembly.Location), "CefSharp.BrowserSubprocess.Core.dll");
            }
            var browserSubprocessDll = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(browserSubprocessDllPath);
            var browserSubprocessExecutableType = browserSubprocessDll.GetType("CefSharp.BrowserSubprocess.BrowserSubprocessExecutable");
#else
            var browserSubprocessDllPath = Path.Combine(Path.GetDirectoryName(typeof(CefSharp.Core.BrowserSettings).Assembly.Location), "CefSharp.BrowserSubprocess.Core.dll");
            var browserSubprocessDll = System.Reflection.Assembly.LoadFrom(browserSubprocessDllPath);
            var browserSubprocessExecutableType = browserSubprocessDll.GetType("CefSharp.BrowserSubprocess.WcfBrowserSubprocessExecutable");
#endif

            var browserSubprocessExecutable = Activator.CreateInstance(browserSubprocessExecutableType);

            var mainMethod = browserSubprocessExecutableType.GetMethod("MainSelfHost", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            var argCount = mainMethod.GetParameters();

            var methodArgs = new object[] { args };

            var exitCode = mainMethod.Invoke(null, methodArgs);

            return (int)exitCode;
        }
    }
}
