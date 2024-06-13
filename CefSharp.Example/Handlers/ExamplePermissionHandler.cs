//版权所有 © 2022 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using CefSharp.Handler;

namespace CefSharp.Example.Handlers
{
    /// <summary>
    /// 一个简单的 <see cref="PermissionHandler"/> 实现，以编程方式允许
    ///无需用户交互的所有请求。
    /// </summary>
    public class ExamplePermissionHandler : PermissionHandler
    {
        protected override bool OnShowPermissionPrompt(IWebBrowser chromiumWebBrowser, IBrowser browser, ulong promptId, string requestingOrigin, PermissionRequestType requestedPermissions, IPermissionPromptCallback callback)
        {
            using (callback)
            {
                System.Diagnostics.Debug.WriteLine($"{promptId}|{requestedPermissions} {requestingOrigin}");
                callback.Continue(PermissionRequestResult.Accept);
                return true;
            }
        }

        protected override void OnDismissPermissionPrompt(IWebBrowser chromiumWebBrowser, IBrowser browser, ulong promptId, PermissionRequestResult result)
        {
            base.OnDismissPermissionPrompt(chromiumWebBrowser, browser, promptId, result);
        }

        protected override bool OnRequestMediaAccessPermission(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string requestingOrigin, MediaAccessPermissionType requestedPermissions, IMediaAccessCallback callback)
        {
            using (callback)
            {
                //允许请求的权限
                callback.Continue(requestedPermissions);
                return true;
            }
        }
    }
}
