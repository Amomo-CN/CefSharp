//版权所有 © 2015 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CefSharp.Example
{
    /// <summary>
    /// 其功能主要通过评估或实现的方法
    ///在浏览器中执行脚本。
    /// </summary>
    public static class ScriptedMethods
    {
        /// <summary>
        ///确定框架中的活动元素是否接受文本输入。
        /// </summary>
        /// <param name="frame">测试该框架中的活动元素。</param>
        /// <returns>如果活动元素接受文本输入，则为 True.</returns>
        public static async Task<bool> ActiveElementAcceptsTextInput(this IFrame frame)
        {
            if (frame == null)
            {
                throw new ArgumentException("需要 IFrame 实例.", "frame");
            }

            // 应缩小生产版本的脚本。剧本
            //也可以从文件中读取...
            const string script =
                  @"(function ()
                    {
                        var isText = false;
                        var activeElement = document.activeElement;
                        if (activeElement) {
                            if (activeElement.tagName.toLowerCase() === 'textarea') {
                                isText = true;
                            } else {
                                if (activeElement.tagName.toLowerCase() === 'input') {
                                    if (activeElement.hasAttribute('type')) {
                                        var inputType = activeElement.getAttribute('type').toLowerCase();
                                        if (inputType === 'text' || inputType === 'email' || inputType === 'password' || inputType === 'tel' || inputType === 'number' || inputType === 'range' || inputType === 'search' || inputType === 'url') {
                                            isText = true;
                                        }
                                    }
                                }
                            }
                        }
                        return isText;
                    })();";

            var response = await frame.EvaluateScriptAsync(script);
            if (!response.Success)
            {
                throw new Exception(response.Message);
            }

            return (bool)response.Result;
        }

        /// <summary>
        ///确定框架是否包含具有指定 id 的元素。
        /// </summary>
        /// <param name="frame">测试此框架中的元素.</param>
        /// <param name="id">要查找的id.</param>
        /// <returns>如果框架中存在具有指定 id 的元素，则为 True.</returns>
        public static async Task<bool> ElementWithIdExists(this IFrame frame, string id)
        {
            if (frame == null)
            {
                throw new ArgumentException("需要 IFrame 实例.", "frame");
            }

            var script =
                @"(function () {
                    var n = document.getElementById('##ID##');
                    return n !== null && typeof n !== 'undefined';
                })();";

            // 对于简单的内联脚本，您可以使用 String.Format() 但
            //注意 javascript 代码中的大括号。如果从文件中读取
            //包含可以通过以下方式替换的令牌可能更安全
            //正则表达式。
            script = Regex.Replace(script, "##ID##", id);

            var response = await frame.EvaluateScriptAsync(script);
            if (!response.Success)
            {
                throw new Exception(response.Message);
            }

            return (bool)response.Result;
        }

        /// <summary>
        /// 使用提供的 id 在元素上设置事件侦听器。当。。。的时候
        ///事件侦听器回调被调用时将尝试传递
        ///绑定到浏览器的.Net 类的事件信息。看
        /// ScriptedMethodsBoundObject.
        /// </summary>
        /// <param name="frame">元素在此框架中。</param>
        ///<param name="id">框架中存在的元素的 id。</param>
        ///<param name="eventName">订阅此事件。例如“点击'.</param>
        public static void ListenForEvent(this IFrame frame, string id, string eventName)
        {
            if (frame == null)
            {
                throw new ArgumentException("需要 IFrame 实例.", "frame");
            }

            // 使用提供的 DOM 元素添加点击事件侦听器
            //ID。单击该元素时，ScriptedMethodsBoundObject 的
            //RaiseEvent 函数被调用。这是获得的一种方法
            //来自网页的异步事件。通常通过网络
            //页面会意识到 window.boundEvent.RaiseEvent 并且会
            //只需根据需要提高它即可。
            //
            //应缩小生产版本的脚本。剧本
            //也可以从文件中读取...
            var script =
                @"(async function ()
                {
                    await CefSharp.BindObjectAsync('boundEvent');
                    var counter = 0;
                    var elem = document.getElementById('##ID##');
                    elem.removeAttribute('disabled');
                    elem.addEventListener('##EVENT##', function(e){
                        if (!window.boundEvent){
                            console.log('window.boundEvent does not exist.');
                            return;
                        }
                        counter++;
                        //注意RaiseEvent在JS中被转换为raiseEvent（这是在注册对象时配置的）
                        window.boundEvent.raiseEvent('##EVENT##', {count: counter, id: e.target.id, tagName: e.target.tagName});
                    });
                    console.log(`Added ##EVENT## listener to ${elem.id}.`);
                })();";

            // 对于简单的内联脚本，您可以使用 String.Format() 但
            //注意 javascript 代码中的大括号。如果从文件中读取
            //包含可以通过以下方式替换的令牌可能更安全
            //正则表达式。
            script = Regex.Replace(script, "##ID##", id);
            script = Regex.Replace(script, "##EVENT##", eventName);

            frame.ExecuteJavaScriptAsync(script);
        }
    }
}
