using Common.Libs.Extends;
using System.Threading.Tasks;
using Client.Service.Ui.Api.ClientServer;
using Common.Libs.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Service.Proxy
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
            return await config.AddFirewall(arg.Content.DeJson<Common.Proxy.FirewallItem>());
        }

        public async Task<bool> Remove(ClientServiceParamsInfo arg)
        {
            return await config.RemoveFirewall(uint.Parse(arg.Content));
        }
    }
}
