//版权所有 © 2015 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CefSharp.Web;

namespace CefSharp.Example.JavascriptBinding
{
    public class AsyncBoundObject
    {
        //我们预计这里会出现异常，因此告诉 VS 忽略
        [DebuggerHidden]
        public void Error()
        {
            throw new Exception("这是来自 C 的异常#");
        }

        //我们预计这里会出现异常，因此告诉 VS 忽略
        [DebuggerHidden]
        public int Div(int divident, int divisor)
        {
            return divident / divisor;
        }

        public string DivWithBlockingTaskCall(int dividend, int divisor)
        {
            var taskToWaitOn = ExecuteTaskBeforeDivision(dividend, divisor);
            taskToWaitOn.Wait();
            return taskToWaitOn.Result;
        }

        private async Task<string> ExecuteTaskBeforeDivision(int dividend, int divisor)
        {
            await RunAsync();
            return (dividend / divisor).ToString();
        }

        private Task RunAsync()
        {
            return Task.Run(() => Run());
        }

        private void Run()
        {
            Debug.WriteLine("AsyncBoundObject Run 执行.");
        }

        public string Hello(string name)
        {
            return "Hello " + name;
        }

        public string DoSomething()
        {
            Thread.Sleep(1000);

            return "等待1000ms才返回";
        }

        public JsSerializableStruct ReturnObject(string name)
        {
            return new JsSerializableStruct
            {
                Value = name
            };
        }

        public JsSerializableClass ReturnClass(string name)
        {
            return new JsSerializableClass
            {
                Value = name
            };
        }

        public JsonString ReturnClassAsJsonString(string name)
        {
            return JsonString.FromObject(new JsSerializableClass { Value = name });
        }

        public JsSerializableStruct[] ReturnStructArray(string name)
        {
            return new[]
            {
                new JsSerializableStruct { Value = name + "Item1" },
                new JsSerializableStruct { Value = name + "Item2" }
            };
        }

        public JsSerializableClass[] ReturnClassesArray(string name)
        {
            return new[]
            {
                new JsSerializableClass { Value = name + "Item1" },
                new JsSerializableClass { Value = name + "Item2" }
            };
        }

        public DateTime EchoDateTime(DateTime arg0)
        {
            return arg0;
        }

        public DateTime? EchoNullableDateTime(DateTime? arg0)
        {
            return arg0;
        }

        public string[] EchoArray(string[] arg)
        {
            return arg;
        }

        public int[] EchoValueTypeArray(int[] arg)
        {
            return arg;
        }

        public int[][] EchoMultidimensionalArray(int[][] arg)
        {
            return arg;
        }

        public string DynamiObjectList(IList<dynamic> objects)
        {
            var builder = new StringBuilder();

            foreach (var browser in objects)
            {
                builder.Append("Browser(Name:" + browser.Name + ";Engine:" + browser.Engine.Name + ");");
            }

            return builder.ToString();
        }

        public List<string> MethodReturnsList()
        {
            return new List<string>()
            {
                "Element 0 - First",
                "Element 1",
                "Element 2 - Last",
            };
        }

        public List<List<string>> MethodReturnsListOfLists()
        {
            return new List<List<string>>()
            {
                new List<string>() {"Element 0, 0", "Element 0, 1" },
                new List<string>() {"Element 1, 0", "Element 1, 1" },
                new List<string>() {"Element 2, 0", "Element 2, 1" },
            };
        }

        public Dictionary<string, int> MethodReturnsDictionary1()
        {
            return new Dictionary<string, int>()
            {
                {"five", 5},
                {"ten", 10}
            };
        }

        public Dictionary<string, object> MethodReturnsDictionary2()
        {
            return new Dictionary<string, object>()
            {
                {"onepointfive", 1.5},
                {"five", 5},
                {"ten", "ten"},
                {"twotwo", new Int32[]{2, 2} }
            };
        }

        public Dictionary<string, IDictionary> MethodReturnsDictionary3()
        {
            return new Dictionary<string, IDictionary>()
            {
                {"data", MethodReturnsDictionary2()}
            };
        }

        public Tuple<bool, string> PassSimpleClassAsArgument(SimpleClass simpleClass)
        {
            if (simpleClass == null)
            {
                return Tuple.Create(false, "PassSimpleClassAsArgument dictionary param is null");
            }

            return Tuple.Create(true, "TestString:" + simpleClass.TestString + ";SubClasses[0].PropertyOne:" + simpleClass.SubClasses[0].PropertyOne);
        }

        //以下测试方法只能在以下情况下使用：
        //CefSharpSettings.ConcurrentTaskExecution = true;
        //对于这些有一组单独的单元测试

        public Task<string> ReturnTaskStringAsync()
        {
            return Task.FromResult(nameof(ReturnTaskStringAsync));
        }

        public async void VoidReturnAsync()
        {
            await Task.Delay(1000);

            Debug.WriteLine("延迟1秒.");
        }

        public async Task<string> JavascriptCallbackEvalPromise(string msg, IJavascriptCallback callback)
        {
            var response = await callback.ExecuteAsync(msg);

            //Echo the response
            return (string)response.Result;
        }

        public async Task WaitBeforeReturnAsync(int milliseconds)
        {
            await Task.Delay(milliseconds);

            Debug.WriteLine("延迟毫秒:" + milliseconds);
        }

        public async Task<string> AsyncWaitTwoSeconds(string str)
        {
            await Task.Delay(2000);

            return str;
        }

        public async Task<string[]> AsyncDownloadFileAndSplitOnNewLines(string url)
        {
            var webClient = new WebClient();
            var download = await webClient.DownloadStringTaskAsync(new Uri(url));

            var lines = download.Split('\n').Where(x => !string.IsNullOrEmpty(x.Trim())).ToArray();

            return lines;
        }

        //我们预计这里会出现异常，因此告诉 VS 忽略
        [DebuggerHidden]
        public async Task<string> AsyncThrowException()
        {
            await Task.Delay(2000);

            throw new Exception("预期异常");
        }

        public uint UIntAddModel(UIntAddModel model)
        {
            return model.ParamA + model.ParamB;
        }

        public uint UIntAdd(uint paramA, uint paramB)
        {
            return paramA + paramB;
        }

        public async Task<string> JavascriptOptionalCallbackEvalPromise(bool invokeCallback, string msg, IJavascriptCallback callback)
        {
            using (callback)
            {
                if (invokeCallback)
                {
                    var response = await callback.ExecuteAsync(msg).ConfigureAwait(false);
                    //回显响应
                    return (string)response.Result;
                }

                return msg;
            }
        }
    }
}
