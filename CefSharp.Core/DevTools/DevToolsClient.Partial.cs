using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefSharp.DevTools.Page
{
    public partial class Viewport
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Viewport()
        {
            Scale = 1.0;
        }
    }
}

namespace CefSharp.DevTools.Network
{
    public partial class NetworkClient
    {
        /// <summary>
        /// 获取资源并返回内容。
        ///</摘要>
        ///<param name = "frameId">要获取资源的框架 ID。对于框架目标是必需的，对于工作目标应该省略。</param>
        ///<param name = "url">要获取内容的资源的 URL。</param>
        ///<param name = "options">请求的选项。</param>
        ///<returns>返回 System.Threading.Tasks.Task<LoadNetworkResourceResponse></returns>
        ///<备注>
        ///LoadNetworkResourceAsync 的这种重载是为了避免重大更改，因为可选参数现在始终位于末尾
        ///之前它们在开始时没有被标记为可选。
        /// </remarks>
        public System.Threading.Tasks.Task<LoadNetworkResourceResponse> LoadNetworkResourceAsync(string frameId, string url, CefSharp.DevTools.Network.LoadNetworkResourceOptions options)
        {
            var dict = new System.Collections.Generic.Dictionary<string, object>();
            if (!(string.IsNullOrEmpty(frameId)))
            {
                dict.Add("frameId", frameId);
            }

            dict.Add("url", url);
            dict.Add("options", options.ToDictionary());
            return _client.ExecuteDevToolsMethodAsync<LoadNetworkResourceResponse>("Network.loadNetworkResource", dict);
        }
    }
}
