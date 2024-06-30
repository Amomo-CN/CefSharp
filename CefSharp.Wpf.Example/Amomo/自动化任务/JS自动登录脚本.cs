using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

using System.Threading.Tasks;
using System.Windows.Controls;

using CefSharp;
using CefSharp.Wpf.Example;
using CefSharp.Wpf.Example.ViewModels;

using static CefSharp.Wpf.Example.MainWindow;

namespace Amomo
{
     class JS自动登录
    {
        #region 注入JS自动化脚本
        // 注入JS自动化脚本

        private ObservableCollection<BrowserTabViewModel> 浏览器标签集合;
        private TabControl 标签控制;

        // 构造函数，接收BrowserTabs集合和TabControl
        public JS自动登录(ObservableCollection<BrowserTabViewModel> 浏览器标签集合传入, TabControl 标签控制传入)
        {
            this.浏览器标签集合 = 浏览器标签集合传入;
            this.标签控制 = 标签控制传入;
        }

        public async Task JS登录脚本开始(string 传入URL)
        {
            //  await Task.Delay(300);
            System.Diagnostics.Debug.WriteLine("开始注入脚本...");

            // 初始化重试参数
            int maxAttempts = 20;
            int retryDelayMs = 50;

            // 封装需要重试的操作
            async Task<(bool Success, string ErrorMessage)> AttemptInjection()
            {
                // 获取当前选中的标签页
                var currentTab = 浏览器标签集合[标签控制.SelectedIndex];

                // 检查CefWebBrowser是否有效
                if (currentTab == null || currentTab.WebBrowser == null)
                {
                    return (false, "CefWebBrowser 为空，无法注入脚本");
                }
                if (!currentTab.WebBrowser.CanExecuteJavascriptInMainFrame)
                {
                    return (false, "当前不能在主框架中执行JavaScript");
                }

                // 初始化脚本内容和路径
                string scriptContent = string.Empty;
                string scriptPath = string.Empty;

                // 根据不同的URL加载不同的JavaScript脚本文件
                if (传入URL.StartsWith("http://192.168.10.173:8088"))
                {
                    scriptPath = "资源/JS脚本文件/APS自动登录.js";
                }
                else if (传入URL.StartsWith("http://192.168.100.216:8081"))
                {
                    scriptPath = "资源/JS脚本文件/U9自动登录.js";
                }
                else if (传入URL.StartsWith("http://192.168.10.209:8080"))
                {
                    scriptPath = "资源/JS脚本文件/报表自动登录.js";
                }
                else
                {
                    return (false, $"没有匹配到合适的脚本路径 (URL: {传入URL})");
                }

                scriptContent = await ReadScriptFileAsync(scriptPath);

                // 执行JavaScript脚本
                if (!string.IsNullOrEmpty(scriptContent))
                {
                    System.Diagnostics.Debug.WriteLine($"脚本内容已加载，准备执行... (URL: {传入URL}, 脚本路径: {scriptPath})");
                    await currentTab.WebBrowser.EvaluateScriptAsync(scriptContent);
                    return (true, null);
                }
                else
                {
                    return (false, $"脚本内容为空，未执行任何操作 (URL: {传入URL}, 脚本路径: {scriptPath})");
                }
            }

            // 重试执行脚本注入操作
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                var (success, errorMessage) = await AttemptInjection();
                if (success)
                {
                    return;
                }

                Debug.WriteLine($"脚本注入尝试失败，第 {attempt + 1} 次重试，原因: {errorMessage}");
                await Task.Delay(retryDelayMs);
            }
            Debug.WriteLine($"脚本注入操作在重试 {maxAttempts} 次后仍然失败。");
            //throw new InvalidOperationException($"脚本注入操作在重试 {maxAttempts} 次后仍然失败。");
        }

        // 读取脚本文件并返回文件内容
        private async Task<string> ReadScriptFileAsync(string filePath)
        {
            try
            {
                using (var streamReader = new StreamReader(filePath))
                {
                    return await streamReader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"读取脚本文件时发生异常: {ex} (脚本路径: {filePath})");
                return string.Empty;
            }
        }

        #endregion
    }
}
