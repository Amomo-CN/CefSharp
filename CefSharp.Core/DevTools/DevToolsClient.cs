//版权所有 © 2020 CefSharp 作者。版权所有。
//
//此源代码的使用受 BSD 风格许可证的约束，该许可证可在 LICENSE 文件中找到。

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CefSharp.Callback;
using CefSharp.Internals;

namespace CefSharp.DevTools
{
    /// <summary>
    /// 开发工具客户端
    /// </summary>
    public partial class DevToolsClient : IDevToolsMessageObserver, IDevToolsClient
    {
        private readonly ConcurrentDictionary<int, DevToolsMethodResponseContext> queuedCommandResults = new ConcurrentDictionary<int, DevToolsMethodResponseContext>();
        private readonly ConcurrentDictionary<string, IEventProxy> eventHandlers = new ConcurrentDictionary<string, IEventProxy>();
        private IBrowser browser;
        private IRegistration devToolsRegistration;
        private bool devToolsAttached;
        private SynchronizationContext syncContext;
        private int disposeCount;

        /// <inheritdoc/>
        public event EventHandler<DevToolsEventArgs> DevToolsEvent;

        /// <inheritdoc/>
        public event EventHandler<DevToolsErrorEventArgs> DevToolsEventError;

        /// <summary>
        ///捕获当前的 <see cref="SynchronizationContext"/> 所以
        ///继续在原始调用线程上执行。如果
        ///<see cref="SynchronizationContext.Current"/> 为 null
        ///<see cref="ExecuteDevToolsMethodAsync(string, IDictionary{string, object})"/>
        ///然后延续将在 CEF UI 线程上运行（默认情况下
        ///这与 WPF/WinForms UI 线程不同）。
        ///</摘要>
        public bool CaptureSyncContext { get; set; }

        /// <summary>
        ///当提供不为 null 时 <see cref="SynchronizationContext"/>
        ///将用于运行连续。默认为空
        ///设置此属性将会改变 <see cref="CaptureSyncContext"/>
        ///为假。
        ///</摘要>
        public SynchronizationContext SyncContext
        {
            get { return syncContext; }
            set
            {
                CaptureSyncContext = false;
                syncContext = value;
            }
        }

        /// <summary>
        ///开发工具客户端
        ///</摘要>
        ///<param name="browser">与此 DevTools 客户端关联的浏览器</param>
        public DevToolsClient(IBrowser browser)
        {
            this.browser = browser;

            CaptureSyncContext = true;
        }

        /// <summary>
        ///存储对 IRegistration 的引用，该引用在以下情况下返回
        ///你注册了一个观察者。
        ///</摘要>
        ///<param name="devToolsRegistration">注册</param>
        public void SetDevToolsObserverRegistration(IRegistration devToolsRegistration)
        {
            this.devToolsRegistration = devToolsRegistration;
        }

        /// <inheritdoc/>
        public void AddEventHandler<T>(string eventName, EventHandler<T> eventHandler) where T : EventArgs
        {
            var eventProxy = eventHandlers.GetOrAdd(eventName, _ => new EventProxy<T>(DeserializeJsonEvent<T>));

            var p = (EventProxy<T>)eventProxy;

            p.AddHandler(eventHandler);
        }

        /// <inheritdoc/>
        public bool RemoveEventHandler<T>(string eventName, EventHandler<T> eventHandler) where T : EventArgs
        {
            if (eventHandlers.TryGetValue(eventName, out IEventProxy eventProxy))
            {
                var p = ((EventProxy<T>)eventProxy);

                if (p.RemoveHandler(eventHandler))
                {
                    return !eventHandlers.TryRemove(eventName, out _);
                }
            }

            return true;
        }

        /// <summary>
        ///通过 DevTools 协议执行方法调用。该方法可以在任何线程上调用。
        ///有关详细信息，请参阅 https://chromedevtools.github.io/devtools-protocol/上的 DevTools 协议文档
        ///支持的方法和预期的 <paramref name="parameters"/> 字典内容。
        ///</摘要>
        ///<param name="method">是方法名称</param>
        ///<param name="parameters">是以字典形式表示的方法参数，
        ///可能为空。</param>
        ///<returns>返回一个可以等待获取方法结果的Task</returns>
        public Task<DevToolsMethodResponse> ExecuteDevToolsMethodAsync(string method, IDictionary<string, object> parameters = null)
        {
            return ExecuteDevToolsMethodAsync<DevToolsMethodResponse>(method, parameters);
        }

        /// <summary>
        ///通过 DevTools 协议执行方法调用。该方法可以在任何线程上调用。
        ///有关详细信息，请参阅 https://chromedevtools.github.io/devtools-protocol/上的 DevTools 协议文档
        ///支持的方法和预期的 <paramref name="parameters"/> 字典内容。
        ///</摘要>
        ///<typeparam name="T">结果将被反序列化成的类型。</typeparam>
        ///<param name="method">是方法名称</param>
        ///<param name="parameters">是以字典形式表示的方法参数，
        ///可能为空。</param>
        ///<returns>返回一个可以等待获取方法结果的Task</returns>
        public Task<T> ExecuteDevToolsMethodAsync<T>(string method, IDictionary<string, object> parameters = null) where T : DevToolsDomainResponseBase
        {
            if (browser == null || browser.IsDisposed)
            {
                //TODO：尽可能将命令排队
                throw new ObjectDisposedException(nameof(IBrowser));
            }

            var taskCompletionSource = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

            var methodResultContext = new DevToolsMethodResponseContext(
                type: typeof(T),
                setResult: o => taskCompletionSource.TrySetResult((T)o),
                setException: taskCompletionSource.TrySetException,
                syncContext: CaptureSyncContext ? SynchronizationContext.Current : SyncContext
            );

            var browserHost = browser.GetHost();

            var messageId = browserHost.GetNextDevToolsMessageId();

            if (!queuedCommandResults.TryAdd(messageId, methodResultContext))
            {
                throw new DevToolsClientException(string.Format("无法将 MessageId {0} 添加到queuedCommandResults ConcurrentDictionary。", messageId));
            }

            //目前在CEF UI Thread上我们可以直接执行
            if (CefThread.CurrentlyOnUiThread)
            {
                ExecuteDevToolsMethod(browserHost, messageId, method, parameters, methodResultContext);
            }
            //ExecuteDevToolsMethod 只能在 CEF UI 线程上调用
            else if (CefThread.CanExecuteOnUiThread)
            {
                CefThread.ExecuteOnUiThread(() =>
                {
                    ExecuteDevToolsMethod(browserHost, messageId, method, parameters, methodResultContext);
                });
            }
            else
            {
                queuedCommandResults.TryRemove(messageId, out methodResultContext);
                throw new DevToolsClientException("无法在 CEF UI 线程上调用 ExecuteDevToolsMethod.");
            }

            return taskCompletionSource.Task;
        }

        private void ExecuteDevToolsMethod(IBrowserHost browserHost, int messageId, string method, IDictionary<string, object> parameters, DevToolsMethodResponseContext methodResultContext)
        {
            try
            {
                var returnedMessageId = browserHost.ExecuteDevToolsMethod(messageId, method, parameters);
                if (returnedMessageId == 0)
                {
                    throw new DevToolsClientException(string.Format("无法执行开发工具方法 {0}.", method));
                }
                else if (returnedMessageId != messageId)
                {
                    //由于某种原因，我们的消息 ID 不匹配
                    throw new DevToolsClientException(string.Format("生成的消息 ID {0} 与返回的消息 ID 不匹配 {1}", returnedMessageId, messageId));
                }
            }
            catch (Exception ex)
            {
                queuedCommandResults.TryRemove(messageId, out _);
                methodResultContext.SetException(ex);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            //Dispose 可以从不同的线程调用
            //CEF维护一个引用和用户
            //维护一个参考，我们在极少数情况下
            //我们最终从不同的地方处理了两次#3725
            //线程。这将确保我们的处理只运行一次。
            if (Interlocked.Increment(ref disposeCount) == 1)
            {
                DevToolsEvent = null;
                devToolsRegistration?.Dispose();
                devToolsRegistration = null;
                browser = null;

                var events = eventHandlers.Values;
                eventHandlers.Clear();

                foreach (var evt in events)
                {
                    evt.Dispose();
                }
            }
        }

        /// <inheritdoc/>
        void IDevToolsMessageObserver.OnDevToolsAgentAttached(IBrowser browser)
        {
            devToolsAttached = true;
        }

        /// <inheritdoc/>
        void IDevToolsMessageObserver.OnDevToolsAgentDetached(IBrowser browser)
        {
            devToolsAttached = false;
        }

        /// <inheritdoc/>
        void IDevToolsMessageObserver.OnDevToolsEvent(IBrowser browser, string method, Stream parameters)
        {
            try
            {
                var evt = DevToolsEvent;

                //仅当我们有事件处理程序时才解析数据
                if (evt != null)
                {
                    var paramsAsJsonString = StreamToString(parameters, leaveOpen: true);

                    evt(this, new DevToolsEventArgs(method, paramsAsJsonString));
                }

                if (eventHandlers.TryGetValue(method, out IEventProxy eventProxy))
                {
                    eventProxy.Raise(this, method, parameters, SyncContext);
                }
            }
            catch (Exception ex)
            {
                var errorEvent = DevToolsEventError;

                var json = "";

                if (parameters.Length > 0)
                {
                    parameters.Position = 0;

                    try
                    {
                        json = StreamToString(parameters, leaveOpen: false);
                    }
                    catch (Exception)
                    {
                        //TODO：我们是否以某种方式将此异常传递给用户？
                    }
                }

                var args = new DevToolsErrorEventArgs(method, json, ex);

                errorEvent?.Invoke(this, args);
            }
        }

        /// <inheritdoc/>
        bool IDevToolsMessageObserver.OnDevToolsMessage(IBrowser browser, Stream message)
        {
            return false;
        }

        /// <inheritdoc/>
        void IDevToolsMessageObserver.OnDevToolsMethodResult(IBrowser browser, int messageId, bool success, Stream result)
        {
            DevToolsMethodResponseContext context;
            if (queuedCommandResults.TryRemove(messageId, out context))
            {
                if (success)
                {
                    if (context.Type == typeof(DevToolsMethodResponse) || context.Type == typeof(DevToolsDomainResponseBase))
                    {
                        context.SetResult(new DevToolsMethodResponse
                        {
                            Success = success,
                            MessageId = messageId,
                            ResponseAsJsonString = StreamToString(result),
                        });
                    }
                    else
                    {
                        try
                        {
                            context.SetResult(DeserializeJson(context.Type, result));
                        }
                        catch (Exception ex)
                        {
                            context.SetException(ex);
                        }
                    }
                }
                else
                {
                    var errorObj = DeserializeJson<DevToolsDomainErrorResponse>(result);
                    errorObj.MessageId = messageId;

                    context.SetException(new DevToolsClientException("DevTools Client Error :" + errorObj.Message, errorObj));
                }
            }
        }


        /// <summary>
        /// 将 JSON 流反序列化为 .Net 对象。
        ///对于.Net Core/.Net 5.0 使用System.Text.Json
        ///.Net 4.6.2 使用 System.Runtime.Serialization.Json
        ///</摘要>
        ///<typeparam name="T">对象类型</typeparam>
        ///<param name="eventName">事件名称</param>
        ///<param name="stream">JSON 流</param>
        ///<returns>类型为<typeparamref name="T"/>的对象</returns>

        private static T DeserializeJsonEvent<T>(string eventName, Stream stream) where T : EventArgs
        {
            if (typeof(T) == typeof(EventArgs))
            {
                return (T)EventArgs.Empty;
            }

            if (typeof(T) == typeof(DevToolsEventArgs))
            {
                var paramsAsJsonString = StreamToString(stream, leaveOpen: true);
                var args = new DevToolsEventArgs(eventName, paramsAsJsonString);

                return (T)(object)args;
            }

            return (T)DeserializeJson(typeof(T), stream);
        }

        /// <summary>
        /// 将 JSON 流反序列化为 .Net 对象。
        ///对于.Net Core/.Net 5.0 使用System.Text.Json
        ///.Net 4.6.2 使用 System.Runtime.Serialization.Json
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        ///<param name="stream">JSON 流</param>
        ///<returns>类型为<typeparamref name="T"/>的对象</returns>
        private static T DeserializeJson<T>(Stream stream)
        {
            return (T)DeserializeJson(typeof(T), stream);
        }

#if NETCOREAPP
        private static readonly System.Text.Json.JsonSerializerOptions DefaultJsonSerializerOptions = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            IgnoreNullValues = true,
            Converters = { new CefSharp.Internals.Json.JsonEnumConverterFactory() },
        };
#else
        private static readonly System.Runtime.Serialization.Json.DataContractJsonSerializerSettings DefaultJsonSerializerSettings = new System.Runtime.Serialization.Json.DataContractJsonSerializerSettings
        {
            UseSimpleDictionaryFormat = true,
        };
#endif

        /// <summary>
        ///将 JSON 流反序列化为 .Net 对象。
        ///对于.Net Core/.Net 5.0 使用System.Text.Json
        ///.Net 4.6.2 使用 System.Runtime.Serialization.Json
        ///</摘要>
        ///<param name="type">对象类型</param>
        ///<param name="stream">JSON 流</param>
        ///<returns>类型为<paramref name="type"/>的对象</returns>
        private static object DeserializeJson(Type type, Stream stream)
        {
#if NETCOREAPP
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IgnoreNullValues = true,
            };

            options.Converters.Add(new CefSharp.Internals.Json.JsonEnumConverterFactory());

            // TODO: 当 System.Text.Json 更新时使用 synchronus Deserialize<T>(Stream)

            var memoryStream = new MemoryStream((int)stream.Length);
            stream.CopyTo(memoryStream);

            return System.Text.Json.JsonSerializer.Deserialize(memoryStream.ToArray(), type, DefaultJsonSerializerOptions);
#else
            var dcs = new System.Runtime.Serialization.Json.DataContractJsonSerializer(type, DefaultJsonSerializerSettings);
            return dcs.ReadObject(stream);
#endif
        }

        private static string StreamToString(Stream stream, bool leaveOpen = false)
        {
            using (var streamReader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: leaveOpen))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
