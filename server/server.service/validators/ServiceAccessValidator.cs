using Common.Libs.AutoInject.Attributes;
using Common.Server;
using Microsoft.Extensions.DependencyInjection;
using Server.Messengers.SignIn;

namespace Server.Service.Validators
{
    [AutoInject(ServiceLifetime.Singleton, typeof(IServiceAccessValidator))]
    public sealed class ServiceAccessValidator : Common.Server.ServiceAccessValidator
    {
        private readonly IClientSignInCaching clientSignInCaching;
        public ServiceAccessValidator(IClientSignInCaching clientSignInCaching)
        {
            this.clientSignInCaching = clientSignInCaching;
        }

        public override bool Validate(ulong connectionid, uint service)
        {
            if (clientSignInCaching.Get(connectionid, out SignInCacheInfo user))
            {
                return Validate(user.UserAccess, service);
            }
            return false;
        }
    }
}
