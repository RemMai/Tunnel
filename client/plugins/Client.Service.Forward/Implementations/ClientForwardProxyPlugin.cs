using System;
using Client.Messengers.Signin;
using Common.Extensions.AutoInject.Attributes;
using Common.ForWard;
using Common.ForWard.Implementations;
using Common.ForWard.Interfaces;
using Common.Proxy;
using Common.Proxy.Interfaces;
using Common.Proxy.Models;
using Common.Server;
using Common.Server.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Config = Client.Config;

namespace client.service.forward.Implementations
{
    [AutoInject(ServiceLifetime.Singleton, typeof(IForwardProxyPlugin), typeof(IAccess))]
    public class ClientForwardProxyPlugin : ForwardProxyPlugin
    {
        public override HttpHeaderCacheInfo Headers { get; set; }
        public override Memory<byte> HeadersBytes { get; set; }

        public ClientForwardProxyPlugin(
            Common.ForWard.Config config,
            Config config1,
            IProxyServer proxyServer,
            IForwardTargetProvider forwardTargetProvider,
            SignInStateInfo signInStateInfo)
            : base(config, proxyServer, forwardTargetProvider)
        {
            signInStateInfo.OnChange += (bool state) =>
            {
                Headers = new HttpHeaderCacheInfo
                {
                    Addr = signInStateInfo.RemoteInfo.Ip,
                    Name = config1.Client.Name,
                    Proxy = Name
                };
            };
        }
    }
}