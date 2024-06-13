//版权所有 © 2022 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

namespace CefSharp.DevTools.Target
{
    ///<摘要>
    ///TargetFilter 中的条目按顺序与目标和匹配的第一个条目进行匹配
    ///根据条目中排除字段的值确定是否包含目标。
    ///如果未指定过滤器，则假定为 [{type: "browser", except: true}, {type: "tab", except: true}, {}] （即包括除浏览器和选项卡之外的所有内容） 。
    ///</摘要>
    public class TargetFilter : DevToolsDomainEntityBase
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 排除
        /// </summary>
        public bool Exclude { get; set; }
    }
}
