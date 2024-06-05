// 引入System.Collections.Generic命名空间，包含泛型集合类
using System.Collections.Generic;
// 引入System.Linq命名空间，提供查询数据的LINQ扩展方法
using System.Linq;
// 引入System.Windows命名空间，包含WPF框架的基础类型和特性
using System.Windows;

// 命名空间声明，此代码属于CefSharp.Wpf.Example项目
namespace CefSharp.Wpf.Example
{
    // 交互逻辑类，与TestWindow.xaml窗口关联
    ///
    /// <summary>
    /// 提供TestWindow.xaml窗口的用户界面交互逻辑
    ///</summary>
    ///
    public partial class StandardTabControlWindow : Window
    {
        // 定义一个列表，存储ChromiumWebBrowser实例
        public List<ChromiumWebBrowser> Tabs { get; }

        // 构造函数，初始化窗体和ChromiumWebBrowser实例
        public StandardTabControlWindow()
        {
            // 初始化组件，加载XAML定义的UI元素
            InitializeComponent();

            // 使用Enumerable.Range生成1到10的数字序列
            // 对每个数字，创建一个新的ChromiumWebBrowser实例并设置其地址为"google.com"
            // 将所有浏览器实例转换为List并赋值给Tabs属性
            Tabs = Enumerable.Range(1, 10).Select(x => new ChromiumWebBrowser
            {
                Address = "google.com"
            }).ToList();

            // 设置DataContext为当前对象，使XAML可以绑定到这个对象的属性
            DataContext = this;
        }
    }
}

/*
这段代码定义了一个名为StandardTabControlWindow的WPF窗口类
它包含了10个ChromiumWebBrowser实例，每个实例的地址都设置为"google.com"
Tabs属性是一个List<ChromiumWebBrowser>，用于存储这些浏览器实例
在构造函数中，使用Enumerable.Range生成一个从1到10的数字序列
然后使用Select方法对每个数字创建一个ChromiumWebBrowser实例
并将它们添加到Tabs列表中。最后，将DataContext设置为this
以便在XAML中可以绑定到StandardTabControlWindow的属性。
*/