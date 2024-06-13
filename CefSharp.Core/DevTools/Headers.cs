//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Collections.Generic;
using System.Linq;

namespace CefSharp.DevTools.Network
{
    /// <summary>
    /// 请求/响应标头作为 JSON 对象的键/值。
    /// </摘要>
    /// <备注>
    /// CDP 使用逗号分隔值来存储多个标头值。
    /// 使用 <see cref="TryGetValues(string, out string[])"/> 或 <see cref="GetCommaSeparatedValues(string)"/> 获取 string[]
    /// 对于具有多个值的标头。
    /// </备注>
    /// 用于处理基于逗号分隔的标头值的辅助方法 https://github.com/dotnet/aspnetcore/blob/52eff90fbcfca39b7eb58baad597df6a99a542b0/src/Http/Http.Abstractions/src/Extensions/HeaderDictionaryExtensions.cs
    public class Headers : Dictionary<string, string>
    {
        /// <summary>
        /// 初始化 Headers 类的新实例。
        /// </summary>
        public Headers() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        ///返回自身
        /// </summary>
        /// <returns>Dictionary of headers</returns>
        public Dictionary<string, string> ToDictionary()
        {
            return this;
        }

        /// <summary>
        /// 获取指定键的值数组。值以逗号分隔，并将被拆分为字符串[]。
        ///引用的值不会被分割，并且引号将被删除。
        ///</摘要>
        ///<param name="key">标头名称。</param>
        ///<param name="values">将字典中的关联值分成单独的值，如果键不存在则返回 null。</param>
        ///<returns>如果字典包含具有指定键的元素则为 true；否则为假.</returns>
        public bool TryGetValues(string key, out string[] values)
        {
            values = null;
            string value;

            if (TryGetValue(key, out value))
            {
                var list = new List<string>();

                var valueStartIndex = -1;
                var valueEndIndex = -1;
                var inQuote = false;
                for (var i = 0; i < value.Length; i++)
                {
                    var c = value[i];

                    if (c == '\"')
                    {
                        inQuote = !inQuote;
                        continue;
                    }

                    if (!inQuote && char.IsWhiteSpace(c))
                    {
                        continue;
                    }

                    if (valueStartIndex == -1)
                    {
                        valueStartIndex = i;
                    }

                    if (!inQuote && c == ',')
                    {
                        if (valueEndIndex == -1)
                        {
                            list.Add(string.Empty);
                        }
                        else
                        {
                            list.Add(value.Substring(valueStartIndex, valueEndIndex + 1 - valueStartIndex));
                        }
                        valueStartIndex = -1;
                        valueEndIndex = -1;
                        continue;
                    }

                    valueEndIndex = i;
                }
                if (valueEndIndex == -1)
                {
                    list.Add(string.Empty);
                }
                else
                {
                    list.Add(value.Substring(valueStartIndex, valueEndIndex + 1 - valueStartIndex));
                }

                values = list.ToArray();

                return true;
            }

            return false;
        }

        /// <summary>
        /// 从字典中获取关联值，并将其分成单独的值。
        ///引用的值不会被分割，并且引号将被删除。
        ///</摘要>
        ///<param name="key">标头名称。</param>
        ///<returns>将字典中的关联值分成单独的值，如果键不存在则返回 null.</returns>
        public string[] GetCommaSeparatedValues(string key)
        {
            string[] values;

            if (TryGetValues(key, out values))
            {
                return values;
            }

            return null;
        }

        /// <summary>
        /// 引用包含逗号的任何值，然后用逗号将所有值与任何现有值连接起来。
        ///</摘要>
        ///<param name="key">标头名称。</param>
        ///<param name="values">标头值.</param>
        public void AppendCommaSeparatedValues(string key, params string[] values)
        {
            if (TryGetValue(key, out var existingValue))
            {
                this[key] = existingValue + "," + string.Join(",", values.Select(QuoteIfNeeded));
            }
            else
            {
                SetCommaSeparatedValues(key, values);
            }
        }

        /// <summary>
        /// 引用包含逗号的任何值，然后用逗号连接所有值。
        ///</摘要>
        ///<param name="key">标头名称。</param>
        ///<param name="values">标头值.</param>
        public void SetCommaSeparatedValues(string key, params string[] values)
        {
            this[key] = string.Join(",", values.Select(QuoteIfNeeded));
        }

        private static string QuoteIfNeeded(string value)
        {
            if (!string.IsNullOrEmpty(value) &&
                value.Contains(',') &&
                (value[0] != '"' || value[value.Length - 1] != '"'))
            {
                return "\"" + value + "\"";
            }

            return value;
        }
    }
}
