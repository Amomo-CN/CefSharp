using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using TabControlAutomationPeer = CefSharp.Wpf.Experimental.Accessibility.TabControlAutomationPeer;

namespace CefSharp.Wpf.Example.Controls
{
    /// <summary>
    ///扩展 TabControl，它保存显示的项目，这样您就不会受到性能影响
    ///切换选项卡时卸载并重新加载 VisualTree
    ///</摘要>
    ///<备注>
    ///基于 http://stackoverflow.com/a/9802346 的示例，该示例又基于
    ///http://www.pluralsight-training.net/community/blogs/eburke/archive/2009/04/30/keeping-the-wpf-tab-control-from-destroying-its-children.aspx
    ///进行一些修改，以便在执行拖放操作时重用 TabItem 的 ContentPresenter
    /// </remarks>
    [TemplatePart(Name = "PART_ItemsHolder", Type = typeof(Panel))]
    public class NonReloadingTabControl : TabControl
    {
        private Panel itemsHolderPanel;

        public NonReloadingTabControl()
        {
            // 这是必要的，以便我们获得初始数据绑定所选项目
            ItemContainerGenerator.StatusChanged += ItemContainerGeneratorStatusChanged;
        }

        /// <summary>
        /// 如果容器完成，则生成所选项目
        ///</摘要>
        ///<param name="sender">发件人。</param>
        ///<param name="e">包含事件数据的 <see cref="EventArgs"/> 实例.</param>
        private void ItemContainerGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                ItemContainerGenerator.StatusChanged -= ItemContainerGeneratorStatusChanged;
                UpdateSelectedItem();
            }
        }

        /// <summary>
        /// 获取 ItemsHolder 并生成任何子项
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            itemsHolderPanel = GetTemplateChild("PART_ItemsHolder") as Panel;
            UpdateSelectedItem();
        }

        /// <summary>
        /// 当项目发生变化时，我们会删除所有生成的面板子项，并根据需要添加任何新的子项
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="NotifyCollectionChangedEventArgs"/> 实例.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (itemsHolderPanel == null)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    itemsHolderPanel.Children.Clear();
                    break;

                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            var cp = FindChildContentPresenter(item);
                            if (cp != null)
                            {
                                itemsHolderPanel.Children.Remove(cp);
                            }
                        }
                    }

                    // 不要对新项目做任何事情，因为我们不想这样做
                    //创建未显示的视觉效果
                    UpdateSelectedItem();
                    break;

                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException("替换尚未实施");
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            UpdateSelectedItem();
        }

        private void UpdateSelectedItem()
        {
            if (itemsHolderPanel == null)
            {
                return;
            }

            // 如有必要，生成 ContentPresenter
            var item = GetSelectedTabItem();
            if (item != null)
            {
                CreateChildContentPresenter(item);
            }

            // 展示正确的孩子
            foreach (ContentPresenter child in itemsHolderPanel.Children)
            {
                child.Visibility = ((child.Tag as TabItem).IsSelected) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private ContentPresenter CreateChildContentPresenter(object item)
        {
            if (item == null)
            {
                return null;
            }

            var cp = FindChildContentPresenter(item);

            if (cp != null)
            {
                return cp;
            }

            var tabItem = item as TabItem;
            cp = new ContentPresenter
            {
                Content = (tabItem != null) ? tabItem.Content : item,
                ContentTemplate = this.SelectedContentTemplate,
                ContentTemplateSelector = this.SelectedContentTemplateSelector,
                ContentStringFormat = this.SelectedContentStringFormat,
                Visibility = Visibility.Collapsed,
                Tag = tabItem ?? (this.ItemContainerGenerator.ContainerFromItem(item))
            };
            itemsHolderPanel.Children.Add(cp);
            return cp;
        }

        private ContentPresenter FindChildContentPresenter(object data)
        {
            if (data is TabItem)
            {
                data = (data as TabItem).Content;
            }

            if (data == null)
            {
                return null;
            }

            if (itemsHolderPanel == null)
            {
                return null;
            }

            foreach (ContentPresenter cp in itemsHolderPanel.Children)
            {
                if (cp.Content == data)
                {
                    return cp;
                }
            }

            return null;
        }

        protected TabItem GetSelectedTabItem()
        {
            var selectedItem = SelectedItem;
            if (selectedItem == null)
            {
                return null;
            }

            var item = selectedItem as TabItem ?? ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as TabItem;

            return item;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new TabControlAutomationPeer(this);
        }
    }
}
