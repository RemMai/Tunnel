using Common.ForWard;
using Common.Server.Model;
using Server.Messengers.SignIn;
using System.Collections.Generic;
using Common.Libs.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Server.Service.ForWard
{
    [AutoInject(ServiceLifetime.Singleton, typeof(ISignInValidator))]
    public sealed class ForwardValidator : ISignInValidator
    {
        private readonly Common.ForWard.Config config;
        private readonly IForwardProxyPlugin forwardProxyPlugin;

        public ForwardValidator(Common.ForWard.Config config, IForwardProxyPlugin forwardProxyPlugin)
        {
            this.config = config;
            this.forwardProxyPlugin = forwardProxyPlugin;
        }

        public EnumSignInValidatorOrder Order => EnumSignInValidatorOrder.Level9;


        public SignInResultInfo.SignInResultInfoCodes Validate(Dictionary<string, string> args, ref uint access)
        {
            access |= (config.ConnectEnable ? forwardProxyPlugin.Access : (uint)Common.Server.EnumServiceAccess.None);
            return SignInResultInfo.SignInResultInfoCodes.OK;
        }

        public void Validated(SignInCacheInfo cache)
        {
        }
    }
}