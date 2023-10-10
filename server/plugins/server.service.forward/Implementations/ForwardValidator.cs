using System.Collections.Generic;
using Common.ForWard.Interfaces;
using Common.Libs.AutoInject.Attributes;
using Common.Server.Enums;
using Common.Server.Models;
using Microsoft.Extensions.DependencyInjection;
using Server.Messengers.SignIn;

namespace Server.Service.ForWard.Implementations
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
            access |= (config.ConnectEnable ? forwardProxyPlugin.Access : (uint)EnumServiceAccess.None);
            return SignInResultInfo.SignInResultInfoCodes.OK;
        }

        public void Validated(SignInCacheInfo cache)
        {
        }
    }
}