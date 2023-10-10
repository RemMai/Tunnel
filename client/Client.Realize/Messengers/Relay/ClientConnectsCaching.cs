using System.Collections.Concurrent;
using Client.Messengers.Relay;
using Common.Libs.AutoInject.Attributes;
using Common.Server.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Realize.Messengers.Relay
{
    [AutoInject(ServiceLifetime.Singleton,typeof(IClientConnectsCaching))]
    public sealed class ClientConnectsCaching : IClientConnectsCaching
    {
        ConcurrentDictionary<ulong, ulong[]> connectsDic = new ConcurrentDictionary<ulong, ulong[]>();

        public ConcurrentDictionary<ulong, ulong[]> Connects => connectsDic;
        public void AddConnects(ConnectsInfo connects)
        {
            connectsDic.AddOrUpdate(connects.Id, connects.Connects, (a, b) => connects.Connects);
        }
        public void Remove(ulong id)
        {
            connectsDic.TryRemove(id, out _);
        }
        public void Clear()
        {
            connectsDic.Clear();
        }

    }
}
