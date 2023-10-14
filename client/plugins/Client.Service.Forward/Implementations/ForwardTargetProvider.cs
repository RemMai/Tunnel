using Client.Messengers.Clients;
using Client.Messengers.Signin;
using Common.Extensions.AutoInject.Attributes;
using Common.ForWard.Implementations;
using Common.ForWard.Interfaces;
using Common.Libs;
using Common.Proxy.Models;
using Common.Server.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Client.Service.forward.Implementations
{
    /// <summary>
    /// 转发目标提供
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton, typeof(IForwardTargetProvider))]
    internal class ForwardTargetProvider : IForwardTargetProvider
    {
        private readonly IClientInfoCaching clientInfoCaching;
        private readonly IForwardTargetCaching<ForwardTargetCacheInfo> forwardTargetCaching;
        private readonly SignInStateInfo signInStateInfo;

        public ForwardTargetProvider(IClientInfoCaching clientInfoCaching,
            IForwardTargetCaching<ForwardTargetCacheInfo> forwardTargetCaching, SignInStateInfo signInStateInfo)
        {
            this.clientInfoCaching = clientInfoCaching;
            this.forwardTargetCaching = forwardTargetCaching;
            this.signInStateInfo = signInStateInfo;
            signInStateInfo.OnChange += (state) => { forwardTargetCaching.ClearConnection(); };
            clientInfoCaching.OnOffline += (client) => { forwardTargetCaching.ClearConnection(client.ConnectionId); };
        }

        public bool Contains(ushort port)
        {
            return forwardTargetCaching.Contains(port);
        }

        /// <summary>
        /// 根据host获取目标连接
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="info"></param>
        public void Get(string domain, ProxyInfo info)
        {
            GetTarget(forwardTargetCaching.Get(domain), info);
            if (Logger.Instance.LoggerLevel <= LoggerTypes.WARNING)
            {
                if (info.Connection == null || info.Connection.Connected == false)
                {
                    Log.Warning(
                        $"{info.ProxyPlugin.Name}->domain:{domain}->target not exists or not connect");
                }
            }
        }

        /// <summary>
        /// 根据端口获取目标连接
        /// </summary>
        /// <param name="port"></param>
        /// <param name="info"></param>
        public void Get(ushort port, ProxyInfo info)
        {
            GetTarget(forwardTargetCaching.Get(port), info);
            if (Logger.Instance.LoggerLevel <= LoggerTypes.WARNING)
            {
                if (info.Connection == null || info.Connection.Connected == false)
                {
                    Log.Warning($"{info.ProxyPlugin.Name}->port:{port}->target not exists or not connect");
                }
            }
        }

        private void GetTarget(ForwardTargetCacheInfo cacheInfo, ProxyInfo info)
        {
            if (cacheInfo != null)
            {
                if (cacheInfo.Connection == null || cacheInfo.Connection.Connected == false)
                {
                    cacheInfo.Connection = SelectConnection(cacheInfo);
                }

                info.Connection = cacheInfo.Connection;
                info.TargetAddress = cacheInfo.IPAddress;
                info.TargetPort = cacheInfo.Port;
            }
        }

        private IConnection SelectConnection(ForwardTargetCacheInfo cacheInfo)
        {
            if (cacheInfo.ConnectionId == 0)
            {
                return signInStateInfo.Connection;
            }

            if (clientInfoCaching.Get(cacheInfo.ConnectionId, out ClientInfo client))
            {
                return client.Connection;
            }

            return null;
        }
    }
}