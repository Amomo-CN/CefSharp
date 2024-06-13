//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CefSharp
{
    ///<摘要>
    ///CLR 模块初始化器
    ///如果需要的话用于加载libcef.dll
    /// </summary>
    public static class Initializer
    {
        //TODO: 目前仅进行内部调试，如果用户想要的话，需要改进
        //从中获取有意义的数据。
        internal static IntPtr? LibCefHandle { get; private set; }
        internal static bool LibCefLoaded { get; private set; }
        internal static string LibCefPath { get; private set; }
        internal static string BrowserSubProcessPath { get; private set; }
        internal static string BrowserSubProcessCorePath { get; private set; }

        [ModuleInitializer]
        internal static void ModuleInitializer()
        {
            string currentFolder;

            var executingAssembly = Assembly.GetEntryAssembly();
            //当从非托管应用程序加载托管程序集时，GetEntryAssembly 方法可以返回 null。
            //https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assembly.getentryassembly?view=net-5.0
            if (executingAssembly == null)
            {
                currentFolder = GetCefSharpCoreAssemblyLocation();
            }
            else
            {
                currentFolder = Path.GetDirectoryName(executingAssembly.Location);
            }

            //在 .NET 5.0 及更高版本中，对于捆绑程序集，Assembly.Location 是空字符串。
            //https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assembly.location?view=net-5.0
            //这会导致 Path.GetDirectoryName 返回 null，在这种情况下请使用 AppContext.BaseDirectory
            //否则Path.Combine将抛出异常。
            //在.NET 5.0及更高版本中，对于捆绑程序集，AppContext.BaseDirectory返回主机可执行文件的包含目录。
            //https://docs.microsoft.com/en-us/dotnet/api/system.appcontext.basedirectory?view=net-5.0
            if (string.IsNullOrEmpty(currentFolder))
            {
                currentFolder = AppContext.BaseDirectory;
            }

            var libCefPath = Path.Combine(currentFolder, "libcef.dll");

            if (File.Exists(libCefPath))
            {
                //我们没有加载CEF，它已经在我们的调用程序集旁边，
                //框架应该自己正确加载它
                LibCefPath = libCefPath;
                LibCefLoaded = false;
            }
            else
            {
                var arch = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
                var archFolder = $"runtimes\\win-{arch}\\native";
                libCefPath = Path.Combine(currentFolder, archFolder, "libcef.dll");

                if (!File.Exists(libCefPath))
                {
                    //对于动态加载库且没有 RuntimeIdentifier 的情况
                    //指定，尝试将 libcef.dll 定位到 CefSharp.Core.dll 旁边
                    currentFolder = GetCefSharpCoreAssemblyLocation();

                    libCefPath = Path.Combine(currentFolder, archFolder, "libcef.dll");
                }

                if (File.Exists(libCefPath))
                {
                    LibCefLoaded = NativeLibrary.TryLoad(libCefPath, out IntPtr handle);

                    if (LibCefLoaded)
                    {
                        BrowserSubProcessPath = Path.Combine(currentFolder, archFolder, "CefSharp.BrowserSubprocess.exe");
                        BrowserSubProcessCorePath = Path.Combine(currentFolder, archFolder, "CefSharp.BrowserSubprocess.Core.dll");

                        LibCefPath = libCefPath;
                        LibCefHandle = handle;
                    }
                }
                else
                {
                    LibCefPath = libCefPath;
                    LibCefLoaded = false;
                }
            }
        }

        private static string GetCefSharpCoreAssemblyLocation()
        {
            return Path.GetDirectoryName(typeof(Initializer).Assembly.Location);
        }
    }
}
