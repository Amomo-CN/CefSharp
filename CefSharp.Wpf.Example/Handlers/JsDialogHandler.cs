// 版权声明，指出CefSharp项目的作者及源代码使用的BSD风格许可协议
// 使用者需遵守LICENSE文件中的条款

using System.Windows; // 引入WPF相关类，如MessageBox

// 命名空间定义，组织相关的类和接口
namespace CefSharp.Wpf.Example.Handlers
{
    // 自定义JsDialogHandler类，继承自CefSharp.Handler.JsDialogHandler，处理JavaScript对话框
    public class JsDialogHandler : CefSharp.Handler.JsDialogHandler
    {
        // 重写OnJSDialog方法，处理JavaScript对话框请求
        protected override bool OnJSDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, CefJsDialogType dialogType, string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage)
        {
            // 类型转换，将IWebBrowser接口转换为ChromiumWebBrowser对象
            var b = (ChromiumWebBrowser)chromiumWebBrowser;

            // 使用WPF的Dispatcher确保在主线程上执行对话框操作
            b.Dispatcher.InvokeAsync(() =>
            {
                // 根据对话框类型处理不同的对话框
                switch (dialogType)
                {
                    // 处理确认对话框
                    case CefJsDialogType.Confirm:
                        // 显示一个确认对话框，标题为"A page at [URL] says:"，内容为messageText
                        // 用户可以选择"Yes"或"No"，返回值为用户的选择结果
                        var messageBoxResult = MessageBox.Show(messageText, $"A page at {originUrl} says:", MessageBoxButton.YesNo);

                        // 如果用户点击"Yes"，则继续执行JavaScript对话框；否则取消
                        callback.Continue(messageBoxResult == MessageBoxResult.Yes);
                        break;

                    // 处理警告对话框
                    case CefJsDialogType.Alert:
                        // 显示一个警告对话框，标题为"A page at [URL] says:"，内容为messageText
                        // 用户只能点击"OK"，返回值为用户点击了"OK"
                        var messageBoxResult2 = MessageBox.Show(messageText, $"A page at {originUrl} says:", MessageBoxButton.OK);

                        // 总是继续执行JavaScript对话框，因为警告对话框没有取消选项
                        callback.Continue(messageBoxResult2 == MessageBoxResult.OK);
                        break;

                    // 处理提示对话框
                    case CefJsDialogType.Prompt:
                        // 使用PromptDialog显示自定义提示对话框，获取用户输入
                        var result = PromptDialog.Prompt(messageText, $"A page at {originUrl} says:", defaultPromptText);

                        // 继续执行JavaScript对话框，同时传递用户输入的文本
                        callback.Continue(result.Item1, userInput: result.Item2);
                        break;
                }
            });

            // 返回true表示已处理对话框，阻止CefSharp的默认处理
            return true;
        }
    }
}