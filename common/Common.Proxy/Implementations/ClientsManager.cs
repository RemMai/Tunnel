using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Common.Libs;
using Common.Libs.Extends;
using Common.Proxy;
using Common.Proxy.Models;
using Serilog;

namespace Common.Proxy.Implementations
{
    public sealed class ClientsManager
    {
        private ConcurrentDictionary<ulong, ProxyUserToken> clients = new();

        public bool TryAdd(ProxyUserToken model)
        {
            return clients.TryAdd(model.Request.RequestId, model);
        }

        public bool TryGetValue(ulong id, out ProxyUserToken c)
        {
            return clients.TryGetValue(id, out c);
        }

        public bool TryRemove(ulong id, out ProxyUserToken c)
        {
            bool res = clients.TryRemove(id, out c);
            if (res)
            {
                try
                {
                    c?.Socket.SafeClose();
                    c.PoolBuffer = Helper.EmptyArray;
                    c?.Saea.Dispose();
                    GC.Collect();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message + "\r\n" + ex.StackTrace);
                }
            }

            return res;
        }

        public void Clear(int sourcePort)
        {
            IEnumerable<ulong> requestIds = clients.Where(c => c.Value.Server.Port == sourcePort).Select(c => c.Key);
            foreach (var requestId in requestIds)
            {
                TryRemove(requestId, out _);
            }
        }

        public void Clear()
        {
            IEnumerable<ulong> requestIds = clients.Select(c => c.Key);
            foreach (var requestId in requestIds)
            {
                TryRemove(requestId, out _);
            }
        }
    }
}