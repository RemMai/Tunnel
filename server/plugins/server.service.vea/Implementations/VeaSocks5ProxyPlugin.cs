using System;
using System.Net;
using Common.Libs.AutoInject.Attributes;
using Common.Proxy;
using Common.proxy.Enums;
using Common.Server.Interfaces;
using Common.Server.Models;
using Common.Vea.Implementations;
using Common.Vea.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Server.Messengers.SignIn;

namespace Server.Service.Vea.Implementations
{
    public interface IVeaSocks5ProxyPlugin : IProxyPlugin
    {
    }


    [AutoInject(ServiceLifetime.Singleton, typeof(IVeaAccessValidator), typeof(IVeaSocks5ProxyPlugin), typeof(IAccess))]
    public class VeaSocks5ProxyPlugin : IVeaSocks5ProxyPlugin, IVeaAccessValidator
    {
        public byte Id => config.Plugin;
        public bool ConnectEnable => config.Enable;
        public EnumBufferSize BufferSize => EnumBufferSize.KB_8;
        public IPAddress BroadcastBind => IPAddress.Any;
        public HttpHeaderCacheInfo Headers { get; set; }
        public Memory<byte> HeadersBytes { get; set; }
        public uint Access => Common.Vea.Config.access;
        public string Name => "vea";
        public ushort Port => 0;

        private readonly Common.Vea.Config config;
        private readonly IServiceAccessValidator serviceAccessValidator;
        private readonly IClientSignInCaching clientSignInCaching;

        public VeaSocks5ProxyPlugin(Common.Vea.Config config, IServiceAccessValidator serviceAccessValidator,
            IClientSignInCaching clientSignInCaching)
        {
            this.config = config;
            this.serviceAccessValidator = serviceAccessValidator;
            this.clientSignInCaching = clientSignInCaching;
        }

        public bool HandleRequestData(ProxyInfo info)
        {
            return true;
        }

        public EnumProxyValidateDataResult ValidateData(ProxyInfo info)
        {
            return EnumProxyValidateDataResult.Equal;
        }

        public bool HandleAnswerData(ProxyInfo info)
        {
            return true;
        }

        public bool Validate(ulong connectionId, out VeaAccessValidateResult result)
        {
            result = null;
            if (clientSignInCaching.Get(connectionId, out SignInCacheInfo sign) == false)
            {
                return false;
            }

            result = new VeaAccessValidateResult
            {
                Connection = sign.Connection,
                Key = sign.GroupId,
                Name = sign.Name
            };

            return config.Enable || serviceAccessValidator.Validate(connectionId, config.Access);
        }
    }
}