using System; // 引入基础类库，提供基本的类型和命名空间支持
using System.Collections.Specialized; // 引入特殊化的集合类，用于处理项更改通知
using System.Windows; // 引入WPF核心类库，提供依赖属性、事件和UI元素
using System.Windows.Automation.Peers; // 引入UI自动化支持，用于辅助技术
using System.Windows.Controls; // 引入WPF控件类库，提供基本的UI控件
using System.Windows.Controls.Primitives; // 引入WPF控件基础类库，提供控件的基本行为和模板支持
using TabControlAutomationPeer = CefSharp.Wpf.Experimental.Accessibility.TabControlAutomationPeer; // 重命名Tab控件的自动化同行，以便在命名空间中避免冲突

// 命名空间定义，组织相关控件和逻辑
namespace CefSharp.Wpf.Example.Controls
{
    /// <summary>
    /// 扩展的TabControl，旨在保存显示的项，从而避免在切换标签页时因卸载和重新加载视觉树而带来的性能损失。
    /// </summary>
    /// <remarks>
    /// 基于http://stackoverflow.com/a/9802346上的示例，该示例又基于
    /// http://www.pluralsight-training.net/community/blogs/eburke/archive/2009/04/30/keeping-the-wpf-tab-control-from-destroying-its-children.aspx，
    /// 并做了一些修改以便在执行拖放操作时重用TabItem的ContentPresenter。
    /// </remarks>
    [TemplatePart(Name = "PART_ItemsHolder", Type = typeof(Panel))] // 指定控件模板中的必需部件
    public class NonReloadingTabControl : TabControl
    {
        private Panel itemsHolderPanel; // 用于存储Tab项内容的面板

        // 构造函数，初始化控件并监听项容器生成器的状态变化以获取初始数据绑定的选定项
        public NonReloadingTabControl()
        {
            ItemContainerGenerator.StatusChanged += ItemContainerGeneratorStatusChanged;
        }

        // 监听项容器生成器状态变化的回调，当容器生成完毕时，生成选定的项
        private void ItemContainerGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                ItemContainerGenerator.StatusChanged -= ItemContainerGeneratorStatusChanged;
                UpdateSelectedItem();
            }
        }

        // 应用模板后获取ItemsHolder并生成任何子项
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            itemsHolderPanel = GetTemplateChild("PART_ItemsHolder") as Panel;
            UpdateSelectedItem();
        }

        // 处理项目变更，根据变更类型移除或添加相应的子项
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (itemsHolderPanel == null) return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    itemsHolderPanel.Children.Clear();
                    break;
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    RemoveOldItems(e);
                    UpdateSelectedItem();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException("Replace 动作尚未实现");
            }
        }

        // 选择改变时更新显示的项
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            UpdateSelectedItem();
        }

        // 更新当前选中的项的显示
        private void UpdateSelectedItem()
        {
            if (itemsHolderPanel == null) return;

            var item = GetSelectedTabItem();
            if (item != null) CreateChildContentPresenter(item);

            foreach (ContentPresenter child in itemsHolderPanel.Children)
                child.Visibility = ((child.Tag as TabItem)?.IsSelected ?? false) ? Visibility.Visible : Visibility.Collapsed;
        }

        // 为指定项创建ContentPresenter，如果已存在则直接返回
        private ContentPresenter CreateChildContentPresenter(object item)
        {
            if (item == null) return null;

            var cp = FindChildContentPresenter(item);
            if (cp != null) return cp;

            var tabItem = item as TabItem;
            cp = new ContentPresenter
            {
                Content = tabItem?.Content ?? item,
                ContentTemplate = this.SelectedContentTemplate,
                ContentTemplateSelector = this.SelectedContentTemplateSelector,
                ContentStringFormat = this.SelectedContentStringFormat,
                Visibility = Visibility.Collapsed,
                Tag = tabItem ?? (this.ItemContainerGenerator.ContainerFromItem(item))
            };
            itemsHolderPanel.Children.Add(cp);
            return cp;
        }

        // 查找与特定数据关联的ContentPresenter
        private ContentPresenter FindChildContentPresenter(object data)
        {
            if (data is TabItem) data = (data as TabItem).Content;
            if (data == null || itemsHolderPanel == null) return null;

            foreach (ContentPresenter cp in itemsHolderPanel.Children)
                if (cp.Content == data) return cp;

            return null;
        }

        // 获取当前选中的TabItem
        protected TabItem GetSelectedTabItem()
        {
            var selectedItem = SelectedItem;
            return selectedItem as TabItem ?? ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as TabItem;
        }

        // 创建自定义的UI自动化同行，以支持辅助技术
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new TabControlAutomationPeer(this);
        }

        // 移除旧的项
        private void RemoveOldItems(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (var item in e.OldItems)
                    RemoveChildContentPresenter(item);
        }

        // 移除与特定项关联的ContentPresenter
        private void RemoveChildContentPresenter(object item)
        {
            var cp = FindChildContentPresenter(item);
            if (cp != null)
                itemsHolderPanel.Children.Remove(cp);
        }
    }
}