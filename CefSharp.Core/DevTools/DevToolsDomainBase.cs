//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。


using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using CefSharp.DevTools.Browser;
using CefSharp.DevTools.Network;

namespace CefSharp.DevTools
{
    /// <summary>
    ///DevTools 域基类
    ///提供一些基本的辅助方法
    /// </summary>
    public abstract class DevToolsDomainBase
    {
#if NETCOREAPP
        /// <summary>
        /// 将枚举转换为字符串
        /// </summary>
        /// <param name="val">enum</param>
        /// <returns>string</returns>
        protected string EnumToString(Enum val)
        {
            return Internals.Json.JsonEnumConverterFactory.ConvertEnumToString(val);
        }

        /// <summary>
        /// 枚举到字符串
        /// </summary>
        /// <param name="values">类型数组 <see cref="CefSharp.DevTools.Network.ContentEncoding"/></param>
        /// <returns>可枚举字符串</returns>
        protected IEnumerable<string> EnumToString(CefSharp.DevTools.Network.ContentEncoding[] values)
        {
            foreach (var val in values)
            {
                yield return Internals.Json.JsonEnumConverterFactory.ConvertEnumToString(val);
            }
        }

        /// <summary>
        /// 枚举到字符串
        /// </summary>
        /// <param name="values">类型数组<see cref="CefSharp.DevTools.Emulation.DisabledImageType"/></param>
        /// <returns>可枚举字符串</returns>
        protected IEnumerable<string> EnumToString(CefSharp.DevTools.Emulation.DisabledImageType[] values)
        {
            foreach (var val in values)
            {
                yield return Internals.Json.JsonEnumConverterFactory.ConvertEnumToString(val);
            }
        }

        /// <summary>
        /// 枚举到字符串
        /// </summary>
        /// <param name="values">类型数组 <see cref="PermissionType"/></param>
        /// <returns>可枚举字符串</returns>
        protected IEnumerable<string> EnumToString(PermissionType[] values)
        {
            foreach (var val in values)
            {
                yield return Internals.Json.JsonEnumConverterFactory.ConvertEnumToString(val);
            }
        }

        /// <summary>
        /// Enum to string
        /// </summary>
        /// <param name="values">类型数组 <see cref="CefSharp.DevTools.DOMDebugger.CSPViolationType"/></param>
        /// <returns>可枚举字符串</returns>
        protected IEnumerable<string> EnumToString(CefSharp.DevTools.DOMDebugger.CSPViolationType[] values)
        {
            foreach (var val in values)
            {
                yield return Internals.Json.JsonEnumConverterFactory.ConvertEnumToString(val);
            }
        }
#else
        protected string EnumToString(Enum val)
        {
            var memInfo = val.GetType().GetMember(val.ToString());
            var dataMemberAttribute = (EnumMemberAttribute)Attribute.GetCustomAttribute(memInfo[0], typeof(EnumMemberAttribute), false);

            return dataMemberAttribute.Value;
        }

        //TODO: 创建一个将枚举数组转换为字符串的通用函数
        protected IEnumerable<string> EnumToString(ContentEncoding[] values)
        {
            foreach (var val in values)
            {
                var memInfo = val.GetType().GetMember(val.ToString());
                var dataMemberAttribute = (EnumMemberAttribute)Attribute.GetCustomAttribute(memInfo[0], typeof(EnumMemberAttribute), false);

                yield return dataMemberAttribute.Value;
            }
        }

        protected IEnumerable<string> EnumToString(Browser.PermissionType[] values)
        {
            foreach (var val in values)
            {
                var memInfo = val.GetType().GetMember(val.ToString());
                var dataMemberAttribute = (EnumMemberAttribute)Attribute.GetCustomAttribute(memInfo[0], typeof(EnumMemberAttribute), false);

                yield return dataMemberAttribute.Value;
            }
        }

        protected IEnumerable<string> EnumToString(CefSharp.DevTools.Emulation.DisabledImageType[] values)
        {
            foreach (var val in values)
            {
                var memInfo = val.GetType().GetMember(val.ToString());
                var dataMemberAttribute = (EnumMemberAttribute)Attribute.GetCustomAttribute(memInfo[0], typeof(EnumMemberAttribute), false);

                yield return dataMemberAttribute.Value;
            }
        }

        protected IEnumerable<string> EnumToString(CefSharp.DevTools.DOMDebugger.CSPViolationType[] values)
        {
            foreach (var val in values)
            {
                var memInfo = val.GetType().GetMember(val.ToString());
                var dataMemberAttribute = (EnumMemberAttribute)Attribute.GetCustomAttribute(memInfo[0], typeof(EnumMemberAttribute), false);

                yield return dataMemberAttribute.Value;
            }
        }
#endif

        protected string ToBase64String(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
    }
}
