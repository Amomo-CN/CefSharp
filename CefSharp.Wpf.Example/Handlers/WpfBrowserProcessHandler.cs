// 版权声明，指出CefSharp项目的作者及源代码使用的BSD风格许可协议
// 使用者需遵守LICENSE文件中的条款

using System; // 引入基础类库，提供基本的类型和命名空间
using System.Timers; // 引入定时器类，用于执行周期性任务
using System.Windows.Threading; // 引入WPF调度器，用于UI线程上的异步调用

using CefSharp.Example.Handlers; // 引入示例处理器命名空间，包含自定义处理器类

// 命名空间定义，组织相关的类和接口
namespace CefSharp.Wpf.Example.Handlers
{
    /// <summary>
    /// 实验性质的实现 - 此类实现较为简单，不适合生产环境使用。
    /// 参考CEF的官方实现，请查看以下链接:
    /// https://bitbucket.org/chromiumembedded/cef/commits/1ff26aa02a656b3bc9f0712591c92849c5909e04?at=2785
    ///</summary>
    public class WpfBrowserProcessHandler : BrowserProcessHandler // 继承自BrowserProcessHandler，自定义浏览器进程处理逻辑
    {
        private Timer timer; // 定时器，用于控制消息循环工作的频率
        private Dispatcher dispatcher; // WPF调度器，用于在UI线程上安排工作

        // 构造函数，接收UI线程的调度器作为参数
        public WpfBrowserProcessHandler(Dispatcher dispatcher)
        {
            // 初始化定时器，设置间隔为每秒30次，并自动重置
            timer = new Timer { Interval = ThirtyTimesPerSecond, AutoReset = true };
            timer.Start(); // 启动定时器
            timer.Elapsed += TimerTick; // 订阅Elapsed事件，处理消息循环工作

            this.dispatcher = dispatcher; // 保存调度器实例
            this.dispatcher.ShutdownStarted += DispatcherShutdownStarted; // 注册调度器关闭开始的事件处理
        }

        // 调度器关闭开始时的处理方法，停止定时器
        private void DispatcherShutdownStarted(object sender, EventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
            }
        }

        // 定时器触发时执行的方法，安排在UI线程上执行Cef.DoMessageLoopWork
        private void TimerTick(object sender, EventArgs e)
        {
            dispatcher.BeginInvoke((Action)(() => Cef.DoMessageLoopWork()), DispatcherPriority.Render);
        }

        // 重写基类方法，根据延迟时间安排消息泵工作
        protected override void OnScheduleMessagePumpWork(long delay)
        {
            // 若延迟大于设定的最大值，则使用每秒30次的频率
            if (delay > ThirtyTimesPerSecond)
            {
                delay = ThirtyTimesPerSecond;
            }

            // 延迟小于等于0时立即执行消息循环工作，否则依赖于定时器
            if (delay <= 0)
            {
                dispatcher.BeginInvoke((Action)(() => Cef.DoMessageLoopWork()), DispatcherPriority.Normal);
            }
        }

        // 重写释放资源方法，清理定时器和调度器的引用
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dispatcher != null)
                {
                    dispatcher.ShutdownStarted -= DispatcherShutdownStarted;
                    dispatcher = null;
                }

                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                    timer = null;
                }
            }

            base.Dispose(disposing); // 调用基类的Dispose方法
        }
    }
}