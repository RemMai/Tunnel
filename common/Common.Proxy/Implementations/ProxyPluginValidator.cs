using Common.Extensions.AutoInject.Attributes;
using Common.Proxy.Enums;
using Common.Proxy.Interfaces;
using Common.Proxy.Models;
using Common.Server.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Proxy.Implementations
{
    [AutoInject(ServiceLifetime.Singleton)]
    public class ProxyPluginValidator : IProxyPluginValidator
    {
        private readonly IServiceAccessValidator serviceAccessValidator;
        private readonly Config config;

        public ProxyPluginValidator(IServiceAccessValidator serviceAccessValidator, Config config)
        {
            this.serviceAccessValidator = serviceAccessValidator;
            this.config = config;
        }

        public bool Validate(ProxyInfo info)
        {
            bool res = info.ProxyPlugin.ConnectEnable ||
                       serviceAccessValidator.Validate(info.Connection.ConnectId, info.ProxyPlugin.Access);
            if (res == false)
            {
                info.CommandStatusMsg = EnumProxyCommandStatusMsg.EnableOrAccess;
                return false;
            }

            if (config.FirewallDenied(info))
            {
                info.CommandStatusMsg = EnumProxyCommandStatusMsg.Firewail;
                return false;
            }

            return true;
        }
    }
}