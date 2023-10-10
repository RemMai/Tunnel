using Client.Messengers.Clients;
using Client.Messengers.Relay;
using Common.Extensions.AutoInject.Attributes;
using Common.Server;
using Common.Server.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Realize.Messengers.Relay
{
    [AutoInject(ServiceLifetime.Singleton,typeof(IRelaySourceConnectionSelector))]
    public sealed class RelaySourceConnectionSelector : IRelaySourceConnectionSelector
    {
        private readonly IClientInfoCaching clientInfoCaching;
        public RelaySourceConnectionSelector(IClientInfoCaching clientInfoCaching, IClientConnectsCaching connecRouteCaching)
        {
            this.clientInfoCaching = clientInfoCaching;
            clientInfoCaching.OnOffline += (client) =>
            {
                connecRouteCaching.Remove(client.ConnectionId);
            };
        }
        public IConnection Select(IConnection connection, ulong relayid)
        {
            if (relayid > 0)
            {
                if (clientInfoCaching.Get(relayid, out ClientInfo client))
                {
                    return client.Connection;
                }
            }
            return connection;
        }
    }


}
