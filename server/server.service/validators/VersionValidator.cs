using Server.Messengers.SignIn;
using System.Collections.Generic;
using Common.Libs;
using Common.Libs.AutoInject.Attributes;
using Common.Server;
using Common.Server.Interfaces;
using Common.Server.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Server.Service.Validators
{
    [AutoInject(ServiceLifetime.Singleton, typeof(ISignInValidator), typeof(IAccess))]
    public sealed class VersionValidator : ISignInValidator, IAccess
    {
        public EnumSignInValidatorOrder Order => EnumSignInValidatorOrder.None;

        public uint Access => 0;
        public string Name => "version";

        public SignInResultInfo.SignInResultInfoCodes Validate(Dictionary<string, string> args, ref uint access)
        {
            if (args.TryGetValue("version", out string version) && version == Helper.Version)
            {
                return SignInResultInfo.SignInResultInfoCodes.OK;
            }

            return SignInResultInfo.SignInResultInfoCodes.UNKNOW;
        }

        public void Validated(SignInCacheInfo cache)
        {
        }
    }
}