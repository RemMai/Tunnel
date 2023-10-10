using Common.Libs.AutoInject.Attributes;
using Common.Server;
using Common.Server.Enums;
using Common.Server.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Realize.Messengers.Relay
{
    [AutoInject(ServiceLifetime.Singleton, typeof(IRelayValidator), typeof(IAccess))]
    public sealed class RelayValidator : IRelayValidator, IAccess
    {
        public uint Access => (uint)EnumServiceAccess.Relay;
        public string Name => "Relay";

        private readonly Config config;
        private readonly IServiceAccessValidator serviceAccessValidator;

        public RelayValidator(Config config, IServiceAccessValidator serviceAccessValidator)
        {
            this.config = config;
            this.serviceAccessValidator = serviceAccessValidator;
        }

        public bool Validate(IConnection connection)
        {
            return config.Client.UseRelay ||
                   serviceAccessValidator.Validate(connection.ConnectId, (uint)EnumServiceAccess.Relay);
        }
    }
}