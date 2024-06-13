//版权所有 © 2015 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace CefSharp.Wpf.Internals
{
    /// <summary>
    ///内部 WpfExtension 方法 -您不太可能需要使用这些方法，
    ///如果您这样做的话，它们将被公开。
    /// </summary>
    public static class WpfExtensions
    {
        /// <summary>
        /// 获取修饰符。
        ///</摘要>
        ///<param name="e">包含事件数据的 <see cref="MouseEventArgs"/> 实例.</param>
        /// <returns>Cef事件标志.</returns>
        public static CefEventFlags GetModifiers(this MouseEventArgs e)
        {
            CefEventFlags modifiers = 0;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                modifiers |= CefEventFlags.LeftMouseButton;
            }
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                modifiers |= CefEventFlags.MiddleMouseButton;
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                modifiers |= CefEventFlags.RightMouseButton;
            }

            modifiers |= GetModifierKeys(modifiers);

            return modifiers;
        }

        /// <summary>
        /// 获取修饰符。
        ///</摘要>
        ///<param name="e">包含事件数据的 <see cref="DragEventArgs"/> 实例。</param>
        ///<返回>CefEventFlags.</returns>
        public static CefEventFlags GetModifiers(this DragEventArgs e)
        {
            return GetModifierKeys();
        }


        /// <summary>
        ///获取键盘修饰符。
        ///</摘要>
        ///<返回>CefEventFlags.</returns>
        public static CefEventFlags GetModifierKeys(CefEventFlags modifiers = 0)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                modifiers |= CefEventFlags.ControlDown | CefEventFlags.IsLeft;
            }

            if (Keyboard.IsKeyDown(Key.RightCtrl))
            {
                modifiers |= CefEventFlags.ControlDown | CefEventFlags.IsRight;
            }

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                modifiers |= CefEventFlags.ShiftDown | CefEventFlags.IsLeft;
            }

            if (Keyboard.IsKeyDown(Key.RightShift))
            {
                modifiers |= CefEventFlags.ShiftDown | CefEventFlags.IsRight;
            }

            if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                modifiers |= CefEventFlags.AltDown | CefEventFlags.IsLeft;
            }

            if (Keyboard.IsKeyDown(Key.RightAlt))
            {
                modifiers |= CefEventFlags.AltDown | CefEventFlags.IsRight;
            }

            return modifiers;
        }

        /// <summary>
        /// 获取修饰符。
        ///</摘要>
        ///<param name="e">包含事件数据的 <see cref="KeyEventArgs"/> 实例。</param>
        ///<返回>CefEventFlags.</returns>
        public static CefEventFlags GetModifiers(this KeyEventArgs e)
        {
            CefEventFlags modifiers = 0;

            //出于性能原因，仅读取修饰符一次
            //https://referencesource.microsoft.com/#PresentationCore/Core/CSharp/System/Windows/Input/KeyboardDevice.cs,227
            var keyboardDeviceModifiers = e.KeyboardDevice.Modifiers;

            if (keyboardDeviceModifiers.HasFlag(ModifierKeys.Shift))
            {
                modifiers |= CefEventFlags.ShiftDown;
            }

            if (keyboardDeviceModifiers.HasFlag(ModifierKeys.Alt))
            {
                modifiers |= CefEventFlags.AltDown;
            }

            if (keyboardDeviceModifiers.HasFlag(ModifierKeys.Control))
            {
                modifiers |= CefEventFlags.ControlDown;
            }

            return modifiers;
        }

        /// <summary>
        ///获取拖动数据包装器。
        ///</摘要>
        ///<param name="e">包含事件数据的 <see cref="DragEventArgs"/> 实例。</param>
        ///<returns>CefDragDataWrapper.</returns>
        public static IDragData GetDragData(this DragEventArgs e)
        {
            // 转换拖动数据
            var dragData = DragData.Create();

            // 文件
            dragData.IsFile = e.Data.GetDataPresent(DataFormats.FileDrop);
            if (dragData.IsFile)
            {
                // 根据文档，当拖动到浏览器中时，我们只需要指定 FileNames，而不是 FileName (http://magpcss.org/ceforum/apidocs3/projects/(default)/CefDragData.html)
                foreach (var filePath in (string[])e.Data.GetData(DataFormats.FileDrop))
                {
                    var displayName = Path.GetFileName(filePath);

                    dragData.AddFile(filePath.Replace("\\", "/"), displayName);
                }
            }

            // Link/Url
            var link = GetLink(e.Data);
            dragData.IsLink = !string.IsNullOrEmpty(link);
            if (dragData.IsLink)
            {
                dragData.LinkUrl = link;
            }

            // Text/HTML
            dragData.IsFragment = e.Data.GetDataPresent(DataFormats.Text);
            if (dragData.IsFragment)
            {
                dragData.FragmentText = (string)e.Data.GetData(DataFormats.Text);

                var htmlData = (string)e.Data.GetData(DataFormats.Html);
                if (htmlData != String.Empty)
                {
                    dragData.FragmentHtml = htmlData;
                }
            }

            return dragData;
        }

        /// <summary>
        /// 获取链接。
        ///</摘要>
        ///<param name="data">数据。</param>
        ///<returns>System.String.</returns>
        private static string GetLink(IDataObject data)
        {
            const string asciiUrlDataFormatName = "UniformResourceLocator";
            const string unicodeUrlDataFormatName = "UniformResourceLocatorW";

            // 尝试 Unicode
            if (data.GetDataPresent(unicodeUrlDataFormatName))
            {
                // 尝试从数据中读取 Unicode URL
                var unicodeUrl = ReadUrlFromDragDropData(data, unicodeUrlDataFormatName, Encoding.Unicode);
                if (unicodeUrl != null)
                {
                    return unicodeUrl;
                }
            }

            // 尝试 ASCII
            if (data.GetDataPresent(asciiUrlDataFormatName))
            {
                // 尝试从数据中读取 ASCII URL
                return ReadUrlFromDragDropData(data, asciiUrlDataFormatName, Encoding.ASCII);
            }

            // 无效链接
            return null;
        }

        /// <summary>
        ///使用特定文本编码从拖放数据中读取 URL。
        ///</摘要>
        ///<param name="data">拖放数据。</param>
        ///<param name="urlDataFormatName">URL类型的数据格式名称</param>
        ///<param name="urlEncoding">URL类型的文本编码</param>
        ///<returns>一个 URL，或者 <see langword="null" /> 如果 <paramref name="data" /> 不包含 URL
        ///正确的类型.</returns>
        private static string ReadUrlFromDragDropData(IDataObject data, string urlDataFormatName, Encoding urlEncoding)
        {
            // 从数据中读取URL
            string url;
            using (var urlStream = (Stream)data.GetData(urlDataFormatName))
            {
                using (TextReader reader = new StreamReader(urlStream, urlEncoding))
                {
                    url = reader.ReadToEnd();
                }
            }

            // 拖放数据中的 URL 通常会用空字符填充，因此请删除这些字符
            return url.TrimEnd('\0');
        }
    }
}
