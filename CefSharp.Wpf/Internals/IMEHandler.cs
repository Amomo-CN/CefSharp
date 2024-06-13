//版权所有 © 2018 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Collections.Generic;
using System.Text;
using CefSharp.Structs;
using Range = CefSharp.Structs.Range;

namespace CefSharp.Wpf.Internals
{
    /// <summary>
    /// ImeHandler 在收到消息 WM_IME_COMPOSITION 时提供实现。
    /// </summary>
    public static class ImeHandler
    {
        //下划线的黑色 SkColor 值。
        public static uint ColorUNDERLINE = 0xFF000000;
        //背景的透明 SkColor 值。
        public static uint ColorBKCOLOR = 0x00000000;

        public static bool GetResult(IntPtr hwnd, uint lParam, out string text)
        {
            var hIMC = ImeNative.ImmGetContext(hwnd);

            var ret = GetString(hIMC, lParam, ImeNative.GCS_RESULTSTR, out text);

            ImeNative.ImmReleaseContext(hwnd, hIMC);

            return ret;
        }

        public static bool GetComposition(IntPtr hwnd, uint lParam, List<CompositionUnderline> underlines, ref int compositionStart, out string text)
        {
            var hIMC = ImeNative.ImmGetContext(hwnd);

            bool ret = GetString(hIMC, lParam, ImeNative.GCS_COMPSTR, out text);
            if (ret)
            {
                GetCompositionInfo(hwnd, lParam, text, underlines, ref compositionStart);
            }

            ImeNative.ImmReleaseContext(hwnd, hIMC);

            return ret;
        }

        private static bool GetString(IntPtr hIMC, uint lParam, uint type, out string text)
        {
            text = string.Empty;

            if (!IsParam(lParam, type))
            {
                return false;
            }

            var strLen = ImeNative.ImmGetCompositionString(hIMC, type, null, 0);
            if (strLen <= 0)
            {
                return false;
            }

            // 缓冲区包含字符（2 个字节）
            byte[] buffer = new byte[strLen];
            ImeNative.ImmGetCompositionString(hIMC, type, buffer, strLen);
            text = Encoding.Unicode.GetString(buffer);

            return true;
        }

        private static void GetCompositionInfo(IntPtr hwnd, uint lParam, string text, List<CompositionUnderline> underlines, ref int compositionStart)
        {
            var hIMC = ImeNative.ImmGetContext(hwnd);

            underlines.Clear();

            byte[] attributes = null;
            int targetStart = text.Length;
            int targetEnd = text.Length;
            if (IsParam(lParam, ImeNative.GCS_COMPATTR))
            {
                attributes = GetCompositionSelectionRange(hIMC, ref targetStart, ref targetEnd);
            }

            //检索选择范围信息。如果指定了 CS_NOMOVECARET
            //这意味着不应移动光标，因此我们将插入符号放在
            //组合字符串的开头。否则我们应该尊重
            //GCS_CURSORPOS 值（如果可用）。
            if (!IsParam(lParam, ImeNative.CS_NOMOVECARET) && IsParam(lParam, ImeNative.GCS_CURSORPOS))
            {
                // IMM32 不支持组合中的非零宽度选择。所以
                //始终使用插入符号位置作为选择范围。
                int cursor = (int)ImeNative.ImmGetCompositionString(hIMC, ImeNative.GCS_CURSORPOS, null, 0);
                compositionStart = cursor;
            }
            else
            {
                compositionStart = 0;
            }

            if (attributes != null &&
                // 之前的字符
                ((compositionStart > 0 && (compositionStart - 1) < attributes.Length && attributes[compositionStart - 1] == ImeNative.ATTR_INPUT)
                ||
                // 之后的字符
                (compositionStart >= 0 && compositionStart < attributes.Length && attributes[compositionStart] == ImeNative.ATTR_INPUT)))
            {
                // 正如 MS 对其 ime 实现所做的那样，我们应该只使用 GCS_CURSORPOS 如果字符
                //在新输入之前或之后。
                // https://referencesource.microsoft.com/#PresentationFramework/src/Framework/System/windows/Documents/ImmComposition.cs,1079
            }
            else
            {
                compositionStart = text.Length;
            }

            if (IsParam(lParam, ImeNative.GCS_COMPCLAUSE))
            {
                GetCompositionUnderlines(hIMC, targetStart, targetEnd, underlines);
            }

            if (underlines.Count == 0)
            {
                var range = new Range();

                bool thick = false;

                if (targetStart > 0)
                {
                    range = new Range(0, targetStart);
                }

                if (targetEnd > targetStart)
                {
                    range = new Range(targetStart, targetEnd);
                    thick = true;
                }

                if (targetEnd < text.Length)
                {
                    range = new Range(targetEnd, text.Length);
                }

                underlines.Add(new CompositionUnderline(range, ColorUNDERLINE, ColorBKCOLOR, thick));
            }

            ImeNative.ImmReleaseContext(hwnd, hIMC);
        }

        private static void GetCompositionUnderlines(IntPtr hIMC, int targetStart, int targetEnd, List<CompositionUnderline> underlines)
        {
            var clauseSize = ImeNative.ImmGetCompositionString(hIMC, ImeNative.GCS_COMPCLAUSE, null, 0);
            if (clauseSize <= 0)
            {
                return;
            }

            int clauseLength = (int)clauseSize / sizeof(Int32);

            // buffer包含32字节（4字节）数组
            var clauseData = new byte[(int)clauseSize];
            ImeNative.ImmGetCompositionString(hIMC, ImeNative.GCS_COMPCLAUSE, clauseData, clauseSize);

            var clauseLength_1 = clauseLength - 1;
            for (int i = 0; i < clauseLength_1; i++)
            {
                int from = BitConverter.ToInt32(clauseData, i * sizeof(Int32));
                int to = BitConverter.ToInt32(clauseData, (i + 1) * sizeof(Int32));

                var range = new Range(from, to);
                bool thick = (range.From >= targetStart && range.To <= targetEnd);

                underlines.Add(new CompositionUnderline(range, ColorUNDERLINE, ColorBKCOLOR, thick));
            }
        }

        private static byte[] GetCompositionSelectionRange(IntPtr hIMC, ref int targetStart, ref int targetEnd)
        {
            var attributeSize = ImeNative.ImmGetCompositionString(hIMC, ImeNative.GCS_COMPATTR, null, 0);
            if (attributeSize <= 0)
            {
                return null;
            }

            int start = 0;
            int end = 0;

            // 缓冲区包含8位数组
            var attributeData = new byte[attributeSize];
            ImeNative.ImmGetCompositionString(hIMC, ImeNative.GCS_COMPATTR, attributeData, attributeSize);

            for (start = 0; start < attributeSize; ++start)
            {
                if (IsSelectionAttribute(attributeData[start]))
                {
                    break;
                }
            }

            for (end = start; end < attributeSize; ++end)
            {
                if (!IsSelectionAttribute(attributeData[end]))
                {
                    break;
                }
            }

            targetStart = start;
            targetEnd = end;
            return attributeData;
        }

        private static bool IsSelectionAttribute(byte attribute)
        {
            return (attribute == ImeNative.ATTR_TARGET_CONVERTED || attribute == ImeNative.ATTR_TARGET_NOTCONVERTED);
        }

        private static bool IsParam(uint lParam, uint type)
        {
            return (lParam & type) == type;
        }
    }
}
