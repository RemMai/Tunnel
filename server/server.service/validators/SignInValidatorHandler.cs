using Server.Messengers.SignIn;
using Microsoft.Extensions.DependencyInjection;
using Common.Libs;
using System.Reflection;
using System.Linq;
using System;
using Common.Extensions.AutoInject.Attributes;
using Common.Server.Enums;
using Common.Server.Models;

namespace Server.Service.Validators
{
    [AutoInject(ServiceLifetime.Singleton, typeof(ISignInValidatorHandler))]
    public sealed class SignInValidatorHandler : ISignInValidatorHandler
    {
        Wrap<ISignInValidator> first;
        Wrap<ISignInValidator> last;

        private readonly Config config;
        private readonly IClientSignInCaching clientSignInCache;
        private readonly IServiceProvider serviceProvider;

        public SignInValidatorHandler(Config config, IClientSignInCaching clientSignInCache,
            IServiceScopeFactory serviceScopeFactory)
        {
            this.config = config;
            this.clientSignInCache = clientSignInCache;
            this.serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;
        }

        public void LoadValidator()
        {
            foreach (ISignInValidator validator in
                     serviceProvider.GetServices<ISignInValidator>().OrderBy(c => c.Order))
            {
                if (first == null)
                {
                    first = new Wrap<ISignInValidator> { Value = validator };
                    last = first;
                }
                else
                {
                    last.Next = new Wrap<ISignInValidator> { Value = validator };
                    last = last.Next;
                }
            }
        }

        public SignInResultInfo.SignInResultInfoCodes Validate(SignInParamsInfo model, ref uint access)
        {
            //未开启登入，且不是管理员分组
            if (config.RegisterEnable == false &&
                (string.IsNullOrWhiteSpace(config.AdminGroup) || model.GroupId != config.AdminGroup))
            {
                return SignInResultInfo.SignInResultInfoCodes.ENABLE;
            }

            //重名
            if (clientSignInCache.Get(model.ConnectionId, out SignInCacheInfo client))
            {
                try
                {
                    //同个设备
                    if (model.Connection.Address.Address.Equals(client.Connection.Address.Address) &&
                        model.LocalIps[1].Equals(client.LocalIps[1]))
                    {
                        clientSignInCache.Remove(client.ConnectionId);
                    }
                    else
                    {
                        model.ConnectionId = 0;
                    }
                }
                catch (Exception)
                {
                    clientSignInCache.Remove(client.ConnectionId);
                }
            }

            //是管理员分组的
            if (string.IsNullOrWhiteSpace(config.AdminGroup) == false && model.GroupId == config.AdminGroup)
            {
                access |= (uint)EnumServiceAccess.All;
            }
            else
            {
                //验证账号
                //其它自定义验证
                Wrap<ISignInValidator> current = first;
                while (current != null)
                {
                    SignInResultInfo.SignInResultInfoCodes code = current.Value.Validate(model.Args, ref access);
                    if (code != SignInResultInfo.SignInResultInfoCodes.OK)
                    {
                        return code;
                    }

                    current = current.Next;
                }
            }

            return SignInResultInfo.SignInResultInfoCodes.OK;
        }

        public void Validated(SignInCacheInfo cache)
        {
            Wrap<ISignInValidator> current = first;
            while (current != null)
            {
                current.Value.Validated(cache);
                current = current.Next;
            }
        }

        sealed class Wrap<T>
        {
            public T Value { get; set; }
            public Wrap<T> Next { get; set; }
        }
    }
}