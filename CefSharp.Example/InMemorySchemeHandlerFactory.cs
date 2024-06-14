//版权所有 © 2016 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Collections.Generic;
using System.IO;
using CefSharp.Example.Properties;

namespace CefSharp.Example
{
    /// <summary>
    ///演示 ISchemeHandlerFactory 从内存中的资源中读取该内容，可以
    ///很容易从磁盘上的文件中获取 -仅当您已经预加载数据时才使用此方法，请勿执行
    ///此处的任何 Web 请求或数据库查找，仅适用于静态数据。
    /// </summary>
    public class InMemorySchemeHandlerFactory : ISchemeHandlerFactory
    {
        /// <summary>
        /// 只是一个简单的资源查找字典
        /// </summary>
        private static readonly IDictionary<string, string> ResourceDictionary;

        static InMemorySchemeHandlerFactory()
        {
            ResourceDictionary = new Dictionary<string, string>
            {
                { "/home.html", Resources.home_html },

                { "/assets/css/shCore.css", Resources.assets_css_shCore_css },
                { "/assets/css/shCoreDefault.css", Resources.assets_css_shCoreDefault_css },
                { "/assets/css/docs.css", Resources.assets_css_docs_css },
                { "/assets/js/application.js", Resources.assets_js_application_js },
                { "/assets/js/jquery.js", Resources.assets_js_jquery_js },
                { "/assets/js/shBrushCSharp.js", Resources.assets_js_shBrushCSharp_js },
                { "/assets/js/shBrushJScript.js", Resources.assets_js_shBrushJScript_js },
                { "/assets/js/shCore.js", Resources.assets_js_shCore_js },

                { "/bootstrap/bootstrap-theme.min.css", Resources.bootstrap_theme_min_css },
                { "/bootstrap/bootstrap.min.css", Resources.bootstrap_min_css },
                { "/bootstrap/bootstrap.min.js", Resources.bootstrap_min_js },

                { "/BindingTest.html", Resources.BindingTest },
                { "/ExceptionTest.html", Resources.ExceptionTest },
                { "/PopupTest.html", Resources.PopupTest },
                { "/SchemeTest.html", Resources.SchemeTest },
                { "/TooltipTest.html", Resources.TooltipTest },
                { "/DragDropCursorsTest.html", Resources.DragDropCursorsTest },
                { "/FramedWebGLTest.html", Resources.FramedWebGLTest },
                { "/MultiBindingTest.html", Resources.MultiBindingTest },
                { "/ScriptedMethodsTest.html", Resources.ScriptedMethodsTest },
                { "/ResponseFilterTest.html", Resources.ResponseFilterTest },
            };
        }

        /// <summary>
        /// 实现 ISchemeHandlerFactory 所需的唯一方法
        /// </summary>
        /// <参数名称=“浏览器”>浏览器</参数>
        ///<param name="frame">框架</param>
        ///<param name="schemeName">此处理程序注册的方案名称</param>
        ///<param name="request">请求，我们将使用它来检查 Url 并加载适当的资源</param>
        /// <returns>
        /// 返回 null 来调用默认行为，这在您在 http/https 方案上注册处理程序时非常有用
        ///如果我们在查找字典中有一个表示资源的字符串，则将其作为 IResourceHandler 返回
        /// </returns>
        IResourceHandler ISchemeHandlerFactory.Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            //如果需要，您可以匹配方案/主机，在本例中我们不需要它，所以保持简单。
            var uri = new Uri(request.Url);
            var fileName = uri.AbsolutePath;
            var extension = Path.GetExtension(fileName);

            string resource;
            if (ResourceDictionary.TryGetValue(fileName, out resource) && !string.IsNullOrEmpty(resource))
            {
                //对于 css/js/etc 来说，指定 mime/type 很重要，这里我们使用文件扩展名来执行查找
                //有重载，您可以指定更多选项，包括 Encoding、mimeType
                return ResourceHandler.FromString(resource, mimeType: Cef.GetMimeType(extension));
            }

            return null;
        }
    }
}
