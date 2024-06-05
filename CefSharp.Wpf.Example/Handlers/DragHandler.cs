// 版权声明，指出CefSharp项目的作者及源代码使用的BSD风格许可协议
// 使用者需遵守LICENSE文件中的条款

using System; // 引入基础类库，提供基本的类型和命名空间
using System.Collections.Generic; // 引入集合类库，用于处理列表和集合
using System.Drawing; // 引入Windows Forms中的绘图类库，用于处理图形区域
using CefSharp.Enums; // 引入CefSharp枚举类型，用于处理拖放操作

// 命名空间定义，组织相关的类和接口
namespace CefSharp.Wpf.Example.Handlers
{
    // 自定义DragHandler类，实现IDragHandler接口，处理拖放操作
    public class DragHandler : IDragHandler, IDisposable
    {
        // 定义一个事件，当可拖动区域发生变化时触发
        public event Action<Region> RegionsChanged;

        // 重写OnDragEnter方法，处理拖放进入事件
        // 返回值：如果处理了事件，返回true；否则返回false，让浏览器进行默认处理
        bool IDragHandler.OnDragEnter(IWebBrowser chromiumWebBrowser, IBrowser browser, IDragData dragData, DragOperationsMask mask)
        {
            // 返回false表示不处理此事件，让浏览器使用其默认行为处理拖放进入
            return false;
        }

        // 重写OnDraggableRegionsChanged方法，处理可拖动区域变更事件
        // 参数：
        // - chromiumWebBrowser：CefSharp的Web浏览器实例
        // - browser：代表浏览器的实例
        // - frame：当前操作的浏览器框架
        // - regions：包含可拖动区域信息的列表
        void IDragHandler.OnDraggableRegionsChanged(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IList<DraggableRegion> regions)
        {
            // 弹出式浏览器窗口在WPF中通常是原生窗口，我们无法直接通过此方法处理它们的拖拽
            //By default popup browers are native windows in WPF so we cannot handle their drag using this method
            if (browser.IsPopup == false)
            {  //NOTE: I haven't tested with dynamically adding removing regions so this may need some tweaking
                // 初始化一个复合区域，用于存储所有可拖动区域
                Region draggableRegion = null;
                //获取单个区域并构造一个代表所有区域的复杂区域。
                // 遍历提供的可拖动区域列表
                foreach (var region in regions)
                {
                    // 根据DrageableRegion的坐标和尺寸创建一个矩形
                    var rect = new Rectangle(region.X, region.Y, region.Width, region.Height);

                    // 如果这是首次处理区域，则创建一个新的Region
                    if (draggableRegion == null)
                    {
                        draggableRegion = new Region(rect);
                    }
                    else
                    {
                        // 如果区域标记为可拖动，则将其合并到复合区域中
                        if (region.Draggable)
                        {
                            draggableRegion.Union(rect);
                        }
                        // 如果区域不可拖动，则从复合区域中排除它
                        else
                        {
                        //在这个场景中，我们有一个外部区域，它是可拖动的
                        // 不是的内部区域，我们必须排除不可拖动的。
                        // 本例中并未涵盖所有场景。
                        // 注意：这里仅处理了简单情况，复杂的嵌套不可拖动区域可能需要更精细的逻辑
                            draggableRegion.Exclude(rect);
                        }
                    }
                }

                // 如果有订阅者注册了RegionsChanged事件，则触发该事件，并传入最终的可拖动区域
                var handler = RegionsChanged;
                if (handler != null)
                {
                    handler(draggableRegion);
                }
            }
        }

        // 实现IDisposable接口，确保资源正确释放
        // 当DragHandler不再使用时，应调用Dispose以清理资源，避免内存泄漏
        public void Dispose()
        {
            // 清空事件订阅者列表，防止内存泄漏
            RegionsChanged = null;
        }
    }
}