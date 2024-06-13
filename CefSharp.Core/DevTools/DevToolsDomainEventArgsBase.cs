//版权所有 © 2021 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Runtime.Serialization;

namespace CefSharp.DevTools
{
    /// <summary>
    /// DevTools 域事件参数基础
    /// </summary>
    [DataContract]
    public abstract class DevToolsDomainEventArgsBase : EventArgs
    {
#if !NETCOREAPP
        public static object StringToEnum(Type enumType, string input)
        {
            return DevToolsDomainEntityBase.StringToEnum(enumType, input);
        }

        public static string EnumToString(Enum e)
        {
            return DevToolsDomainEntityBase.EnumToString(e);
        }
#endif
    }
}
