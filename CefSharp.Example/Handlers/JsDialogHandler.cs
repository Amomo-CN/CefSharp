//版权所有 © 2015 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。
namespace CefSharp.Example.Handlers
{
    public class JsDialogHandler : IJsDialogHandler
    {
        bool IJsDialogHandler.OnJSDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, CefJsDialogType dialogType, string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage)
        {
            return false;
        }

        bool IJsDialogHandler.OnBeforeUnloadDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, string message, bool isReload, IJsDialogCallback callback)
        {
            //自定义实现看起来像
            //-在 UI 线程上创建/显示对话框
            //-一旦用户响应就执行回调
            //-回调.Continue(true);
            //-返回 true

            //注意：返回 false 会触发默认行为，如果返回 false 则无需执行回调。
            return false;
        }

        void IJsDialogHandler.OnResetDialogState(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {

        }

        void IJsDialogHandler.OnDialogClosed(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {

        }
    }
}
