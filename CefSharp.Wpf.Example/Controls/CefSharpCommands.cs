//版权所有 © 2015 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System.Windows.Input;

namespace CefSharp.Wpf.Example.Controls
{
    public static class CefSharpCommands
    {
        public static RoutedUICommand Exit = new RoutedUICommand("Exit", "Exit", typeof(CefSharpCommands));
        public static RoutedUICommand OpenTabCommand = new RoutedUICommand("OpenTabCommand", "OpenTabCommand", typeof(CefSharpCommands));
        public static RoutedUICommand PrintTabToPdfCommand = new RoutedUICommand("PrintTabToPdfCommand", "PrintTabToPdfCommand", typeof(CefSharpCommands));
        public static RoutedUICommand CustomCommand = new RoutedUICommand("CustomCommand", "CustomCommand", typeof(CefSharpCommands));
    }
}
