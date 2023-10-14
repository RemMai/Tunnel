using Client.Service.Ui.api.Interfaces;
using Common.Extensions.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Service.Ui.Api.Service.Implementations
{
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class ServerClientService : IClientService
    {
        public void Default()
        {

        }
    }
}