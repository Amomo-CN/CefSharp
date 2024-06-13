//版权所有 © 2014 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;

using CefSharp.Wpf.Handler;

namespace CefSharp.Wpf.Example.Handlers
{
    public class MenuHandler : CefSharp.Wpf.Handler.ContextMenuHandler
    {
        public MenuHandler(bool addDevtoolsMenuItems = false) : base(addDevtoolsMenuItems)
        {
        }

        protected override void OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            base.OnBeforeContextMenu(chromiumWebBrowser, browser, frame, parameters, model);

            Console.WriteLine("上下文菜单已打开");
            Console.WriteLine(parameters.MisspelledWord);

            if (model.Count > 0)
            {
                model.AddSeparator();
            }

            //对于此菜单处理程序 28440 和 28441 由 Show/Close DevTools 命令使用
            model.AddItem((CefMenuCommand)26501, "做点什么");

            //To disable context menu then clear
            // model.Clear();
        }

        protected override void ExecuteCommand(IBrowser browser, ContextMenuExecuteModel model)
        {
            //Custom item
            if (model.MenuCommand == (CefMenuCommand)26501)
            {
                Console.WriteLine("使用自定义菜单");
            }
            else
            {
                base.ExecuteCommand(browser, model);
            }
        }
    }
}
