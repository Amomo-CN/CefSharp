using CefSharp.Structs;

namespace CefSharp.Wpf.Internals
{
    /// <summary>
    /// 实现该接口来控制鼠标位置的变换
    /// </summary>
    public interface IMousePositionTransform
    {
        System.Windows.Point UpdatePopupSizeAndPosition(Rect originalRect, Rect viewRect);
        void OnPopupShow(bool isOpen);
        void TransformMousePoint(ref System.Windows.Point point);
    }
}
