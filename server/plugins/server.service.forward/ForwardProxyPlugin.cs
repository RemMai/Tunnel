using Common.ForWard;
using Common.Proxy;
using Server.Messengers.SignIn;
using System;
using System.Collections.Generic;
using System.Linq;
using Common.Libs.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Server.Service.ForWard
{
    [AutoInject(ServiceLifetime.Singleton, typeof(IForwardProxyPlugin))]
    public sealed class ForwardProxyPlugin : Common.ForWard.ForwardProxyPlugin
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