using System.Collections.Generic;
using System.Reflection;
using Client.Service.Ui.api.Models;

namespace Client.Service.Ui.api.Interfaces
{
    /// <summary>
    /// 前端接口服务
    /// </summary>
    public interface IClientServer
    {
        /// <summary>
        /// websocket
        /// </summary>
        public void WebSocket();
        /// <summary>
        /// 具名插槽
        /// </summary>
        public void NamedPipe();
        /// <summary>
        /// 加载插件
        /// </summary>
        /// <param name="assemblies"></param>
        public void LoadPlugins(IEnumerable<Assembly> assemblies);
        /// <summary>
        /// 获取配置插件列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ClientServiceConfigureInfo> GetConfigures();
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public IClientConfigure GetConfigure(string className);
        /// <summary>
        /// 获取配置名称列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetServices();

        /// <summary>
        /// 主动通知
        /// </summary>
        /// <param name="resp"></param>
        public void Notify(ClientServiceResponseInfo resp);
    }
}
