using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using client.service.ui.api.Enums;
using client.service.ui.api.Interfaces;
using client.service.ui.api.Models;
using client.service.ui.api.service.Models;
using Common.Extensions.AutoInject.Attributes;
using Common.Libs.Extends;
using Microsoft.Extensions.DependencyInjection;

namespace client.service.ui.api.service.Implementations
{
    /// <summary>
    /// 前端配置文件修改接口
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class ConfigureClientService : IClientService
    {
        private readonly IClientServer clientServer;
        private readonly Client.Config clientConfig;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientServer"></param>
        public ConfigureClientService(IClientServer clientServer, Client.Config clientConfig)
        {
            this.clientServer = clientServer;
            this.clientConfig = clientConfig;
        }

        /// <summary>
        /// 获取配置列表
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public IEnumerable<ClientServiceConfigureInfo> Configures(ClientServiceParamsInfo arg)
        {
            return clientServer.GetConfigures();
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task<string> Configure(ClientServiceParamsInfo arg)
        {
            var plugin = clientServer.GetConfigure(arg.Content);
            if (plugin != null)
            {
                return await plugin.Load().ConfigureAwait(false);
            }

            return string.Empty;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task<bool> Save(ClientServiceParamsInfo arg)
        {
            SaveParamsInfo model = arg.Content.DeJson<SaveParamsInfo>();
            var plugin = clientServer.GetConfigure(model.ClassName);
            if (plugin != null)
            {
                if (await plugin.Save(model.Content).ConfigureAwait(false) == false)
                {
                    arg.SetCode(ClientServiceResponseCodes.Error, "configure fail");
                }
            }

            return true;
        }

        /// <summary>
        /// 获取服务列表
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public IEnumerable<string> Services(ClientServiceParamsInfo arg)
        {
            if (clientConfig.Client.Services.Length > 0)
            {
                return clientServer.GetServices().Intersect(clientConfig.Client.Services)
                    .Append(clientConfig.Client.Services[0]).Reverse();
            }

            return clientServer.GetServices();
        }
    }
}