using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using CefSharp.Wpf;
using CefSharp.Wpf.Example;

namespace Amomo
{


public static class 高精度计时器
{
    private static Stopwatch 计时器 = new Stopwatch();

    public static string 获取并重置()
    {
        if (!计时器.IsRunning)
        {
            计时器.Start();
            return "0.000秒";
        }
        var 上次运行时间 = 计时器.Elapsed;
        计时器.Restart();
        return $"{上次运行时间.TotalMilliseconds:F3}毫秒";
    }
}
}
