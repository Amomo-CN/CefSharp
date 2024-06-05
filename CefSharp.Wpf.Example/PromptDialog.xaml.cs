// 版权声明，指出CefSharp的源代码使用BSD风格的许可协议，具体条款可在LICENSE文件中找到

// 引入System命名空间，包含基础类型和类
using System;
// 引入System.Windows命名空间，包含WPF框架的基础类型和特性
using System.Windows;

// 命名空间声明，此代码属于CefSharp.Wpf.Example项目
namespace CefSharp.Wpf.Example
{
    // 定义PromptDialog类，这是一个简单的提示对话框
    public partial class PromptDialog : Window
    {
        // 构造函数，初始化控件
        public PromptDialog()
        {
            InitializeComponent();
        }

        // 静态方法，用于显示提示对话框并获取用户输入
        public static Tuple<bool, string> Prompt(string messageText, string title, string defaultPromptText = "")
        {
            // 创建PromptDialog实例并设置标题
            var window = new PromptDialog
            {
                Title = title
            };

            // 设置对话框的消息文本和默认用户输入文本
            window.messageText.Text = messageText;
            window.userPrompt.Text = defaultPromptText;

            // 显示对话框并获取结果
            var result = window.ShowDialog();

            // 如果用户点击确定，返回一个包含结果和用户输入的元组
            if (result == true)
            {
                return Tuple.Create(true, window.userPrompt.Text);
            }
            // 如果用户取消，返回一个包含否定结果和空字符串的元组
            return Tuple.Create(false, string.Empty);
        }

        // 确定按钮点击事件处理程序，设置对话框结果为真并关闭
        private void OkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            Close();
        }

        // 取消按钮点击事件处理程序，关闭对话框
        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
/*
这段代码定义了一个名为PromptDialog的WPF窗口类，用于显示一个简单的提示对话框
用户可以输入文本并选择确认或取消。Prompt是一个静态方法，用于创建并显示对话框
根据用户的选择返回一个包含布尔值（是否确认）和用户输入字符串的元组
OkClick和CancelClick分别是确定和取消按钮的事件处理程序，用于关闭对话框并设置对话框结果。
*/