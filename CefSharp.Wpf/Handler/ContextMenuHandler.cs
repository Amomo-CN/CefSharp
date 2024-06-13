//版权所有 © 2021 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;

namespace CefSharp.Wpf.Handler
{
    /// <summary>
    ///使用 <see cref="ContextMenu"/> 的 <see cref="IContextMenuHandler"/> 实现
    ///显示上下文菜单。
    /// </summary>
    public class ContextMenuHandler : CefSharp.Handler.ContextMenuHandler
    {
        /// <summary>
        ///打开 DevTools <see cref="CefMenuCommand"/> Id
        /// </summary>
        public const int CefMenuCommandShowDevToolsId = 28440;
        /// <summary>
        /// 关闭 DevTools <see cref="CefMenuCommand"/> Id
        /// </summary>
        public const int CefMenuCommandCloseDevToolsId = 28441;

        private readonly bool addDevtoolsMenuItems;

        public ContextMenuHandler(bool addDevtoolsMenuItems = false)
        {
            this.addDevtoolsMenuItems = addDevtoolsMenuItems;
        }

        /// <inheritdoc/>
        protected override void OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            if (addDevtoolsMenuItems)
            {
                if (model.Count > 0)
                {
                    model.AddSeparator();
                }

                model.AddItem((CefMenuCommand)CefMenuCommandShowDevToolsId, "显示开发工具（检查）");
                model.AddItem((CefMenuCommand)CefMenuCommandCloseDevToolsId, "关闭开发工具");
            }
        }

        /// <inheritdoc/>
        protected override void OnContextMenuDismissed(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
        {
            var webBrowser = (ChromiumWebBrowser)chromiumWebBrowser;

            webBrowser.UiThreadRunAsync(() =>
            {
                webBrowser.ContextMenu = null;
            });
        }


        /// <inheritdoc/>
        protected override bool RunContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            var webBrowser = (ChromiumWebBrowser)chromiumWebBrowser;

            //IMenuModel 仅在此方法的上下文中有效，因此需要在 UI 线程上调用之前读取值
            var menuItems = GetMenuItems(model);
            var dictionarySuggestions = parameters.DictionarySuggestions;
            var xCoord = parameters.XCoord;
            var yCoord = parameters.YCoord;
            var misspelledWord = parameters.MisspelledWord;
            var selectionText = parameters.SelectionText;

            webBrowser.UiThreadRunAsync(() =>
            {
                var menu = new ContextMenu
                {
                    IsOpen = true,
                    Placement = PlacementMode.Mouse
                };

                RoutedEventHandler handler = null;

                handler = (s, e) =>
                {
                    menu.Closed -= handler;

                    //如果回调已被处理，那么它已经被执行
                    //所以不要调用Cancel
                    if (!callback.IsDisposed)
                    {
                        callback.Cancel();
                    }
                };

                menu.Closed += handler;

                foreach (var item in menuItems)
                {
                    if (item.IsSeperator)
                    {
                        menu.Items.Add(new Separator());

                        continue;
                    }

                    if (item.CommandId == CefMenuCommand.NotFound)
                    {
                        continue;
                    }

                    var menuItem = new MenuItem
                    {
                        Header = item.Label.Replace("&", "_"),
                        IsEnabled = item.IsEnabled,
                        IsChecked = item.IsChecked.GetValueOrDefault(),
                        IsCheckable = item.IsChecked.HasValue,
                        Command = new DelegateCommand(() =>
                        {
                            //BUG：CEF 当前未正确执行回调，因此我们手动映射以下命令
                            //看 https://github.com/cefsharp/CefSharp/issues/1767
                            //以下行在以前的版本中有效，但现在不行，因此下面的自定义处理
                            //callback.Continue(item.Item2, CefEventFlags.None);
                            ExecuteCommand(browser, new ContextMenuExecuteModel(item.CommandId, dictionarySuggestions, xCoord, yCoord, selectionText, misspelledWord));
                        }),
                    };

                    //TODO: Make this recursive and remove duplicate code
                    if (item.SubMenus != null && item.SubMenus.Count > 0)
                    {
                        foreach (var subItem in item.SubMenus)
                        {
                            if (subItem.CommandId == CefMenuCommand.NotFound)
                            {
                                continue;
                            }

                            if (subItem.IsSeperator)
                            {
                                menu.Items.Add(new Separator());

                                continue;
                            }

                            var subMenuItem = new MenuItem
                            {
                                Header = subItem.Label.Replace("&", "_"),
                                IsEnabled = subItem.IsEnabled,
                                IsChecked = subItem.IsChecked.GetValueOrDefault(),
                                IsCheckable = subItem.IsChecked.HasValue,
                                Command = new DelegateCommand(() =>
                                {
                                    //BUG：CEF 当前未正确执行回调，因此我们手动映射以下命令
                                    //看 https://github.com/cefsharp/CefSharp/issues/1767
                                    //以下行在以前的版本中有效，但现在不行，因此下面的自定义处理
                                    //callback.Continue(item.Item2, CefEventFlags.None);
                                    ExecuteCommand(browser, new ContextMenuExecuteModel(subItem.CommandId, dictionarySuggestions, xCoord, yCoord, selectionText, misspelledWord));
                                }),
                            };

                            menuItem.Items.Add(subMenuItem);
                        }
                    }

                    menu.Items.Add(menuItem);
                }
                webBrowser.ContextMenu = menu;
            });

            return true;
        }

        protected virtual void ExecuteCommand(IBrowser browser, ContextMenuExecuteModel model)
        {
            // 如果用户为拼写错误选择了替换词，请在此处替换。
            if (model.MenuCommand >= CefMenuCommand.SpellCheckSuggestion0 &&
                model.MenuCommand <= CefMenuCommand.SpellCheckSuggestion4)
            {
                int sugestionIndex = ((int)model.MenuCommand) - (int)CefMenuCommand.SpellCheckSuggestion0;
                if (sugestionIndex < model.DictionarySuggestions.Count)
                {
                    var suggestion = model.DictionarySuggestions[sugestionIndex];
                    browser.ReplaceMisspelling(suggestion);
                }

                return;
            }

            switch (model.MenuCommand)
            {
                // 导航。
                case CefMenuCommand.Back:
                    {
                        browser.GoBack();
                        break;
                    }
                case CefMenuCommand.Forward:
                    {
                        browser.GoForward();
                        break;
                    }
                case CefMenuCommand.Reload:
                    {
                        browser.Reload();
                        break;
                    }
                case CefMenuCommand.ReloadNoCache:
                    {
                        browser.Reload(ignoreCache: true);
                        break;
                    }
                case CefMenuCommand.StopLoad:
                    {
                        browser.StopLoad();
                        break;
                    }

                //编辑
                case CefMenuCommand.Undo:
                    {
                        browser.FocusedFrame.Undo();
                        break;
                    }
                case CefMenuCommand.Redo:
                    {
                        browser.FocusedFrame.Redo();
                        break;
                    }
                case CefMenuCommand.Cut:
                    {
                        browser.FocusedFrame.Cut();
                        break;
                    }
                case CefMenuCommand.Copy:
                    {
                        browser.FocusedFrame.Copy();
                        break;
                    }
                case CefMenuCommand.Paste:
                    {
                        browser.FocusedFrame.Paste();
                        break;
                    }
                case CefMenuCommand.Delete:
                    {
                        browser.FocusedFrame.Delete();
                        break;
                    }
                case CefMenuCommand.SelectAll:
                    {
                        browser.FocusedFrame.SelectAll();
                        break;
                    }

                // 各种各样的。
                case CefMenuCommand.Print:
                    {
                        browser.GetHost().Print();
                        break;
                    }
                case CefMenuCommand.ViewSource:
                    {
                        browser.FocusedFrame.ViewSource();
                        break;
                    }
                case CefMenuCommand.Find:
                    {
                        browser.GetHost().Find(model.SelectionText, true, false, false);
                        break;
                    }

                // 拼写检查。
                case CefMenuCommand.AddToDictionary:
                    {
                        browser.GetHost().AddWordToDictionary(model.MisspelledWord);
                        break;
                    }

                case (CefMenuCommand)CefMenuCommandShowDevToolsId:
                    {
                        browser.GetHost().ShowDevTools(inspectElementAtX: model.XCoord, inspectElementAtY: model.YCoord);
                        break;
                    }
                case (CefMenuCommand)CefMenuCommandCloseDevToolsId:
                    {
                        browser.GetHost().CloseDevTools();
                        break;
                    }
            }
        }

        private static IList<MenuModel> GetMenuItems(IMenuModel model)
        {
            var menuItems = new List<MenuModel>();

            for (var i = 0; i < model.Count; i++)
            {
                var type = model.GetTypeAt(i);
                bool? isChecked = null;

                if (type == MenuItemType.Check)
                {
                    isChecked = model.IsCheckedAt(i);
                }

                var subItems = model.GetSubMenuAt(i);

                var subMenus = subItems == null ? null : GetMenuItems(subItems);

                var menuItem = new MenuModel
                {
                    Label = model.GetLabelAt(i),
                    CommandId = model.GetCommandIdAt(i),
                    IsEnabled = model.IsEnabledAt(i),
                    Type = type,
                    IsSeperator = type == MenuItemType.Separator,
                    IsChecked = isChecked,
                    SubMenus = subMenus
                };

                menuItems.Add(menuItem);
            }

            return menuItems;
        }

        //TODO:每个文件一个类
        internal class MenuModel
        {
            internal string Label { get; set; }
            internal CefMenuCommand CommandId { get; set; }
            internal bool IsEnabled { get; set; }
            internal bool IsSeperator { get; set; }
            internal bool? IsChecked { get; set; }
            internal MenuItemType Type { get; set; }

            internal IList<MenuModel> SubMenus { get; set; }
        }
    }
}
