using client.service.ui.api.Interfaces;
using Common.Extensions.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace client.service.ui.api.service.Implementations
{
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class ServerClientService : IClientService
    {
        public void Default()
        {

        }
    }
}