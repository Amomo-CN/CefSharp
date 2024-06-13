//版权所有 © 2015 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CefSharp.Example.JavascriptBinding
{
    public class ExceptionTestBoundObject
    {
        [DebuggerStepThrough]
        private double DivisionByZero(int zero)
        {
            return 10 / zero;
        }

        [DebuggerStepThrough]
        public double TriggerNestedExceptions()
        {
            try
            {
                try
                {
                    return DivisionByZero(0);
                }
                catch (Exception innerException)
                {
                    throw new InvalidOperationException("嵌套异常无效", innerException);
                }
            }
            catch (Exception e)
            {
                throw new OperationCanceledException("嵌套异常已取消", e);
            }
        }

        [DebuggerStepThrough]
        public int TriggerParameterException(int parameter)
        {
            return parameter;
        }

        public void TestCallbackException(IJavascriptCallback errorCallback, IJavascriptCallback errorCallbackResult)
        {
            const int taskDelay = 500;

            Task.Run(async () =>
            {
                await Task.Delay(taskDelay);

                using (errorCallback)
                {
                    JavascriptResponse result = await errorCallback.ExecuteAsync("来自 C# 的回调被延迟 " + taskDelay + "ms");
                    string resultMessage;
                    if (result.Success)
                    {
                        resultMessage = "致命：错误回调中没有抛出异常";
                    }
                    else
                    {
                        resultMessage = "抛出异常: " + result.Message;
                    }
                    await errorCallbackResult.ExecuteAsync(resultMessage);
                }
            });
        }
    }
}
