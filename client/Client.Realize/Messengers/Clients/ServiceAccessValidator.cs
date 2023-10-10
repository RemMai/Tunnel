using Client.Messengers.Clients;
using Client.Messengers.Signin;
using Common.Extensions.AutoInject.Attributes;
using Common.Server;
using Common.Server.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Realize.Messengers.Clients
{
    [AutoInject(ServiceLifetime.Singleton, typeof(IServiceAccessValidator))]
    public sealed class ServiceAccessValidator : Common.Server.Implementations.ServiceAccessValidator
    {
        private readonly IClientInfoCaching clientInfoCaching;
        private readonly SignInStateInfo signInStateInfo;
        public ServiceAccessValidator(IClientInfoCaching clientInfoCaching, SignInStateInfo signInStateInfo)
        {
            this.clientInfoCaching = clientInfoCaching;
            this.signInStateInfo = signInStateInfo;
        }

        public override bool Validate(ulong connectionid, uint service)
        {
            if (signInStateInfo.ConnectId == connectionid) return true;

            if (clientInfoCaching.Get(connectionid, out ClientInfo user))
            {
                return Validate(user.UserAccess, service);
            }
            return false;
        }
    }
}
