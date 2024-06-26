//版权所有 © 2013 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using CefSharp.Example.Properties;

namespace CefSharp.Example
{
    public class CefSharpSchemeHandlerFactory : ISchemeHandlerFactory
    {
        public const string SchemeName = "custom";
        public const string SchemeNameTest = "test";

        private static readonly IDictionary<string, object> ResourceDictionary;

        static CefSharpSchemeHandlerFactory()
        {
            ResourceDictionary = new Dictionary<string, object>
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
                { "/BindingTestNetCore.html", Resources.BindingTestNetCore },
                { "/BindingTestAsync.js", Resources.BindingTestAsync },
                { "/BindingTestSync.js", Resources.BindingTestSync },
                { "/BindingTestSingle.html", Resources.BindingTestSingle },
                { "/LegacyBindingTest.html", Resources.LegacyBindingTest },
                { "/PostMessageTest.html", Resources.PostMessageTest },
                { "/ExceptionTest.html", Resources.ExceptionTest },
                { "/PopupTest.html", Resources.PopupTest },
                { "/SchemeTest.html", Resources.SchemeTest },
                { "/TooltipTest.html", Resources.TooltipTest },
                { "/FramedWebGLTest.html", Resources.FramedWebGLTest },
                { "/MultiBindingTest.html", Resources.MultiBindingTest },
                { "/ScriptedMethodsTest.html", Resources.ScriptedMethodsTest },
                { "/ResponseFilterTest.html", Resources.ResponseFilterTest },
                { "/DraggableRegionTest.html", Resources.DraggableRegionTest },
                { "/DragDropCursorsTest.html", Resources.DragDropCursorsTest },
                { "/CssAnimationTest.html", Resources.CssAnimation },
                { "/CdmSupportTest.html", Resources.CdmSupportTest },
                { "/Recaptcha.html", Resources.Recaptcha },
                { "/UnicodeExampleGreaterThan32kb.html", Resources.UnicodeExampleGreaterThan32kb },
                { "/UnocodeExampleEqualTo32kb.html", Resources.UnocodeExampleEqualTo32kb },
                { "/JavascriptCallbackTest.html", Resources.JavascriptCallbackTest },
                { "/BindingTestsAsyncTask.html", Resources.BindingTestsAsyncTask },
                { "/BindingApiCustomObjectNameTest.html", Resources.BindingApiCustomObjectNameTest },
                { "/HelloWorld.html", Resources.HelloWorld },
                { "/ImageTest.html", Resources.ImageTest }
            };

            ResourceDictionary.Add("/assets/images/beach-2089936_1920.jpg", (byte[])TypeDescriptor.GetConverter(Resources.beach.GetType()).ConvertTo(Resources.beach, typeof(byte[])));
        }

        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            //笔记：
            //-该方案处理程序完全忽略“主机”部分。
            //-如果您为 http/https 方案注册 ISchemeHandlerFactory，您还应该指定一个域名
            //-避免在此方法中进行大量处理，因为这会影响性能。
            //-使用默认 ResourceHandler 实现
            var uri = new Uri(request.Url);
            var fileName = uri.AbsolutePath;

            //直接从磁盘加载文件
            if (fileName.EndsWith("CefSharp.Core.xml", StringComparison.OrdinalIgnoreCase))
            {
                //查找 mimeType 的便捷帮助方法
                var mimeType = Cef.GetMimeType("xml");
                //加载 CefSharp.Core.xml 的资源处理程序
                return ResourceHandler.FromFilePath("CefSharp.Core.xml", mimeType, autoDisposeStream: true);
            }

            if (fileName.EndsWith("Logo.png", StringComparison.OrdinalIgnoreCase))
            {
                //Convenient helper method to lookup the mimeType
                var mimeType = Cef.GetMimeType("png");
                //Load a resource handler for Logo.png
                return ResourceHandler.FromFilePath("..\\..\\..\\..\\CefSharp.WinForms.Example\\Resources\\chromium-256.png", mimeType, autoDisposeStream: true);
            }

            if (uri.Host == "cefsharp.com" && schemeName == "https" && (string.Equals(fileName, "/PostDataTest.html", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(fileName, "/PostDataAjaxTest.html", StringComparison.OrdinalIgnoreCase)))
            {
                return new CefSharpSchemeHandler();
            }

            if (string.Equals(fileName, "/EmptyResponseFilterTest.html", StringComparison.OrdinalIgnoreCase))
            {
                return ResourceHandler.FromString("", mimeType: ResourceHandler.DefaultMimeType);
            }

            object resource;
            if (ResourceDictionary.TryGetValue(fileName, out resource))
            {
                var fileExtension = Path.GetExtension(fileName);
                var mimeType = Cef.GetMimeType(fileExtension);

                if (resource is string resourceString)
                {
                    return ResourceHandler.FromString(resourceString, includePreamble: true, mimeType: mimeType);
                }

                if (resource is byte[] resourceByteArray)
                {
                    return ResourceHandler.FromByteArray(resourceByteArray, mimeType: mimeType);
                }
            }

            return null;
        }
    }
}
