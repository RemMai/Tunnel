﻿using Common.Extensions.AutoInject.Attributes;
using Common.ForWard.Implementations;
using Common.ForWard.Interfaces;
using Common.Proxy.Models;
using Microsoft.Extensions.DependencyInjection;
using Server.Messengers.SignIn;

namespace Server.Service.ForWard.Implementations
{
    [AutoInject(ServiceLifetime.Singleton, typeof(IForwardTargetProvider))]
    internal class ForwardTargetProvider : IForwardTargetProvider
    {
        private readonly IClientSignInCaching clientSignInCaching;
        private readonly IForwardTargetCaching<ForwardTargetCacheInfo> forwardTargetCaching;

        public ForwardTargetProvider(IClientSignInCaching clientSignInCaching, IForwardTargetCaching<ForwardTargetCacheInfo> forwardTargetCaching)
        {
            this.clientSignInCaching = clientSignInCaching;
            this.forwardTargetCaching = forwardTargetCaching;

            clientSignInCaching.OnOffline += (client) =>
            {
                forwardTargetCaching.ClearConnection(client.ConnectionId);
            };
        }

        public bool Contains(ushort port)
        {
            return forwardTargetCaching.Contains(port);
        }

        public void Get(string host, ProxyInfo info)
        {
            GetTarget(forwardTargetCaching.Get(host), info);
        }

        public void Get(ushort port, ProxyInfo info)
        {
            GetTarget(forwardTargetCaching.Get(port), info);
        }

        private void GetTarget(ForwardTargetCacheInfo cacheInfo, ProxyInfo info)
        {
            if (cacheInfo != null)
            {
                if (cacheInfo.Connection == null || cacheInfo.Connection.Connected == false)
                {
                    if (clientSignInCaching.Get(cacheInfo.ConnectionId, out SignInCacheInfo client))
                    {
                        cacheInfo.Connection = client.Connection;
                    }
                }
                info.Connection = cacheInfo.Connection;
                info.TargetAddress = cacheInfo.IPAddress;
                info.TargetPort = cacheInfo.Port;
            }
        }
    }

}