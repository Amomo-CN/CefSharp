using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Media;

namespace CefSharp.Wpf.Experimental.Accessibility
{
    /// <summary>
    /// 默认 TabControl 的 AutomationPeer 对其中的控件一无所知，因为它们是动态加载的。
    ///此类的目的是修复此行为。
    ///</摘要>
    ///<备注>
    ///取自 https://www.colinsalmcorner.com/post/genericautomationpeer--helping-the-coded-ui-framework-find-your-custom-controls
    /// </remarks>
    public class TabControlAutomationPeer : UIElementAutomationPeer
    {
        public TabControlAutomationPeer(UIElement owner) : base(owner)
        {
        }

        protected override List<AutomationPeer> GetChildrenCore()
        {
            var list = base.GetChildrenCore();
            list.AddRange(GetChildPeers(Owner));
            return list;
        }

        private List<AutomationPeer> GetChildPeers(UIElement element)
        {
            var list = new List<AutomationPeer>();

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as UIElement;
                if (child != null)
                {
                    var childPeer = CreatePeerForElement(child);
                    if (childPeer != null)
                    {
                        list.Add(childPeer);
                    }
                    else
                    {
                        list.AddRange(GetChildPeers(child));
                    }
                }
            }

            return list;
        }
    }
}
