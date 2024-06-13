using CefSharp.Structs;

namespace CefSharp.Wpf.Internals
{
    public sealed class MousePositionTransform : IMousePositionTransform
    {
        /// <summary>
        /// x 偏移。
        /// </summary>
        private int xOffset;

        /// <summary>
        /// Y 轴偏移。
        /// </summary>
        private int yOffset;

        /// <summary>
        /// 原来的矩形。
        /// </summary>
        private Rect originalRect;

        /// <summary>
        /// 调整后的矩形
        /// </summary>
        private Rect adjustedRect;

        /// <summary>
        /// 弹出窗口是否打开。
        /// </summary>
        private bool isOpen;

        /// <summary>
        ///更新弹出窗口的大小和位置。
        ///</摘要>
        ///<param name="originalRect"></param>
        ///<参数名称=“viewRect”></参数>
        ///<returns>调整后的点。</returns>
        System.Windows.Point IMousePositionTransform.UpdatePopupSizeAndPosition(Rect originalRect, Rect viewRect)
        {
            int x = originalRect.X,
                prevX = originalRect.X,
                y = originalRect.Y,
                prevY = originalRect.Y,
                xOffset = 0,
                yOffset = 0;

            //如果弹出窗口超出视图，请尝试重新定位原点
            if (originalRect.X + originalRect.Width > viewRect.Width)
            {
                x = viewRect.Width - originalRect.Width;
                xOffset = prevX - x;
            }

            if (originalRect.Y + originalRect.Height > viewRect.Height)
            {
                y = y - originalRect.Height - 20;
                yOffset = prevY - y;
            }

            // 如果 x 或 y 变为负数，则将它们再次移至 0
            if (x < 0)
            {
                x = 0;
                xOffset = prevX;
            }

            if (y < 0)
            {
                y = 0;
                yOffset = prevY;
            }

            if (x != prevX || y != prevY)
            {
                this.isOpen = true;

                this.xOffset = xOffset;
                this.yOffset = yOffset;

                Rect adjustedRect = new Rect(x, y, x + originalRect.Width, y + originalRect.Height);

                this.originalRect = originalRect;
                this.adjustedRect = adjustedRect;

                if (this.originalRect.Y < this.adjustedRect.Y + this.adjustedRect.Height)
                {
                    var newY = this.adjustedRect.Y + this.adjustedRect.Height;
                    this.originalRect = new Rect(originalRect.X, newY, originalRect.Width, originalRect.Y + originalRect.Height - newY);
                }
            }

            return new System.Windows.Point(x, y);
        }

        /// <summary>
        /// 重置偏移量和原始矩形。
        ///<param name="isOpen">弹出窗口是否打开。</param>
        /// </summary>
        void IMousePositionTransform.OnPopupShow(bool isOpen)
        {
            if (!isOpen)
            {
                this.isOpen = false;

                this.xOffset = 0;
                this.yOffset = 0;

                this.originalRect = new Rect();
                this.originalRect = new Rect();
            }
        }

        /// <summary>
        ///当弹出窗口可见时调整鼠标坐标。
        ///</摘要>
        ///<param name="point">原点</param>
        ///<returns>如果需要则变换后的点，否则返回原始点.</returns>
        void IMousePositionTransform.TransformMousePoint(ref System.Windows.Point point)
        {
            if (!isOpen)
                return;

            if (!IsInsideOriginalRect(point) && IsInsideAdjustedRect(point))
                point = new System.Windows.Point((int)point.X + this.xOffset, (int)point.Y + this.yOffset);
        }

        /// <summary>
        /// 检查给定点是否在原始矩形内。
        ///</摘要>
        ///<param name="point">点。</param>
        ///<returns>如果该点位于原始矩形内，则返回 true，否则返回 false.</returns>
        private bool IsInsideOriginalRect(System.Windows.Point point)
        {
            return point.X >= this.originalRect.X &&
                   point.X < this.originalRect.X + this.originalRect.Width &&
                   point.Y >= this.originalRect.Y &&
                   point.Y < this.originalRect.Y + this.originalRect.Height;
        }

        /// <summary>
        /// 检查给定点是否位于调整后的矩形内。
        ///</摘要>
        ///<param name="point">点。</param>
        ///<returns>如果该点位于调整后的矩形内，则返回 true，否则返回 false.</returns>
        private bool IsInsideAdjustedRect(System.Windows.Point point)
        {
            return point.X >= this.adjustedRect.X &&
                   point.X < this.adjustedRect.X + this.adjustedRect.Width &&
                   point.Y >= this.adjustedRect.Y &&
                   point.Y < this.adjustedRect.Y + this.adjustedRect.Height;
        }
    }
}
