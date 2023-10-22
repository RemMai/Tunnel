using System;
using System.Collections.Generic;
using System.Linq;
using Common.Extensions.AutoInject.Attributes;
using Common.ForWard.Implementations;
using Common.ForWard.Interfaces;
using Common.Proxy.Interfaces;
using Common.Proxy.Models;
using Microsoft.Extensions.DependencyInjection;
using Server.Messengers.SignIn;

namespace Server.Service.ForWard.Implementations
{
    [AutoInject(ServiceLifetime.Singleton, typeof(IForwardProxyPlugin))]
    public sealed class ForwardProxyPlugin : Common.ForWard.Implementations.ForwardProxyPlugin
    {
        public override HttpHeaderCacheInfo Headers { get; set; }
        public override Memory<byte> HeadersBytes { get; set; }

        public ForwardProxyPlugin(
            Common.ForWard.Config config,
            IProxyServer proxyServer,
            IForwardTargetProvider forwardTargetProvider,
            IClientSignInCaching clientSignInCaching,
            IForwardTargetCaching<ForwardTargetCacheInfo> forwardTargetCaching)
            : base(config, proxyServer, forwardTargetProvider)
        {
            clientSignInCaching.OnOffline += (client) =>
            {
                List<ushort> keys = forwardTargetCaching.Remove(client.ConnectionId).ToList();
                if (keys.Any())
                {
                    foreach (ushort item in keys)
                    {
                        proxyServer.Stop(item);
                    }
                }
            };
        }

        public override bool HandleRequestData(ProxyInfo info)
        {
            info.ProxyPlugin.Headers = new HttpHeaderCacheInfo
                { Addr = info.ClientEP.Address, Name = "/", Proxy = Name };
            return base.HandleRequestData(info);
        }
    }
}