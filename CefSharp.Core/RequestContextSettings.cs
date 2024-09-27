//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

//注意：CefSharp.Core 命名空间中的类已对 intellisnse 隐藏，因此用户不会直接使用它们

using System;

namespace CefSharp
{
    /// <summary>
    /// RequestContext Settings
    /// </summary>
    public class RequestContextSettings
    {
        internal Core.RequestContextSettings settings = new Core.RequestContextSettings();

        /// <summary>
        ///保存会话cookie（没有过期日期或有效性的cookie
        ///间隔）默认情况下，当使用全局cookie管理器时将此值设置为
        ///真的。会话 cookie 通常是暂时的，并且大多数
        ///Web 浏览器不会保留它们。可以使用全局设置
        ///CefSettings.PersistSessionCookies 值。如果出现以下情况，该值将被忽略
        ///CachePath 为空或者是否与 CefSettings.CachePath 值匹配。
        /// </summary>
        public bool PersistSessionCookies
        {
            get { return settings.PersistSessionCookies; }
            set { settings.PersistSessionCookies = value; }
        }

        /// <summary>
        ///此请求上下文的缓存数据将存储的位置
        ///磁盘。如果该值非空，则它必须是绝对路径
        ///等于 CefSettings.RootCachePath 或其子目录。
        ///如果该值为空，则浏览器将以“隐身模式”创建
        ///其中内存缓存用于存储，并且没有数据持久保存到磁盘。
        ///HTML5 数据库（例如 localStorage）仅在以下情况下才会跨会话持久存在：
        ///指定缓存路径。共享全局浏览器缓存及相关
        ///配置设置此值以匹配 CefSettings.CachePath 值。
        /// </summary>
        public String CachePath
        {
            get { return settings.CachePath; }
            set { settings.CachePath = value; }
        }

        /// <summary>
        ///逗号分隔的语言代码有序列表，没有任何空格
        ///将在“Accept-Language”HTTP 标头中使用。可以全局设置
        ///使用 CefSettings.accept_language_list 值或重写
        ///浏览器基础使用BrowserSettings.AcceptLanguageList 值。如果
        ///所有值均为空，则将使用“en-US,en”。该值将是
        ///如果 CachePath 与 CefSettings.CachePath 值匹配，则忽略。
        /// </summary>
        public String AcceptLanguageList
        {
            get { return settings.AcceptLanguageList; }
            set { settings.AcceptLanguageList = value; }
        }

        /// <summary>
        ///关联支持的以逗号分隔的方案列表
        ///ICookieManager。如果 CookieableSchemesExcludeDefaults 为 false
        ///默认方案（“http”、“https”、“ws”和“wss”）也将受到支持。
        ///指定 CookieableSchemesList 值和设置
        ///CookieableSchemesExcludeDefaults 为 true 将禁用所有加载
        ///并为此管理器保存 cookie。如果出现以下情况，该值将被忽略
        ///<see cref="CachePath"/> 与 <see cref="CefSettingsBase.CachePath"/> 值匹配。
        /// </summary>
        public string CookieableSchemesList
        {
            get { return settings.CookieableSchemesList; }
            set { settings.CookieableSchemesList = value; }
        }

        /// <summary>
        ///如果 CookieableSchemesExcludeDefaults 为 false
        ///默认方案（“http”、“https”、“ws”和“wss”）也将受到支持。
        ///指定 CookieableSchemesList 值和设置
        ///CookieableSchemesExcludeDefaults 为 true 将禁用所有加载
        ///并为此管理器保存 cookie。如果出现以下情况，该值将被忽略
        ///<see cref="CachePath"/> 与 <see cref="CefSettingsBase.CachePath"/> 值匹配。
        /// </summary>
        public bool CookieableSchemesExcludeDefaults
        {
            get { return settings.CookieableSchemesExcludeDefaults; }
            set { settings.CookieableSchemesExcludeDefaults = value; }
        }
    }
}
