using System;
using System.Linq;
using System.Reflection;
using Common.Extensions.AutoInject.Attributes;
using Common.Libs;
using Common.Proxy.Interfaces;
using Common.Proxy.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Proxy.Implementations
{
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class ProxyPluginValidatorHandler
    {
        Wrap<IProxyPluginValidator> first;
        Wrap<IProxyPluginValidator> last;

        private readonly IServiceProvider serviceProvider;

        public ProxyPluginValidatorHandler(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;
        }

        public void LoadValidator(Assembly[] assemblys)
        {
            foreach (IProxyPluginValidator validator in ReflectionHelper
                         .GetInterfaceSchieves(assemblys, typeof(IProxyPluginValidator)).Distinct()
                         .Select(c => (IProxyPluginValidator)serviceProvider.GetService(c)))
            {
                if (first == null)
                {
                    first = new Wrap<IProxyPluginValidator> { Value = validator };
                    last = first;
                }
                else
                {
                    last.Next = new Wrap<IProxyPluginValidator> { Value = validator };
                    last = last.Next;
                }
            }
        }

        public bool Validate(ProxyInfo info)
        {
            Wrap<IProxyPluginValidator> current = first;
            while (current != null)
            {
                if (current.Value.Validate(info) == false)
                {
                    return false;
                }

                current = current.Next;
            }

            return true;
        }
    }
}