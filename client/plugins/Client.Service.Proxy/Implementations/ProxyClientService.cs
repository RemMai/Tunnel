using System.Threading.Tasks;
using client.service.ui.api.Interfaces;
using client.service.ui.api.Models;
using Common.Extensions.AutoInject.Attributes;
using Common.Libs.Extends;
using Common.Proxy.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Service.Proxy.Implementations
{
    /// <summary>
    /// proxy
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class ProxyClientService : IClientService
    {
        private readonly Common.Proxy.Config config;

        public ProxyClientService(Common.Proxy.Config config)
        {
            this.config = config;
        }

        public Common.Proxy.Config Get(ClientServiceParamsInfo arg)
        {
            return config;
        }

        public async Task<bool> Add(ClientServiceParamsInfo arg)
        {
            return await config.AddFirewall(arg.Content.DeJson<FirewallItem>());
        }

        public async Task<bool> Remove(ClientServiceParamsInfo arg)
        {
            return await config.RemoveFirewall(uint.Parse(arg.Content));
        }
    }
}
