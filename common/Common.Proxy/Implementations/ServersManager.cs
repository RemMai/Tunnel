using System;
using System.Collections.Concurrent;
using Common.Libs.Extends;
using Common.Proxy.Models;
using Serilog;

namespace Common.Proxy.Implementations
{
    public sealed class ServersManager
    {
        public ConcurrentDictionary<ushort, ServerInfo> services = new();

        public bool TryAdd(ServerInfo model)
        {
            return services.TryAdd(model.Port, model);
        }

        public bool Contains(ushort port)
        {
            return services.ContainsKey(port);
        }

        public bool TryGetValue(ushort port, out ServerInfo c)
        {
            return services.TryGetValue(port, out c);
        }

        public bool TryRemove(ushort port, out ServerInfo c)
        {
            bool res = services.TryRemove(port, out c);
            if (res)
            {
                try
                {
                    Log.Warning($"{c.ProxyPlugin.Name}->port:{port}  stoped");
                    c.ProxyPlugin.Stoped(port);
                    c.Socket.SafeClose();
                    c.UdpClient.Dispose();
                    GC.Collect();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message + "\r\n" + ex.StackTrace);
                }
            }

            return res;
        }

        public void Clear()
        {
            foreach (var item in services.Values)
            {
                try
                {
                    Log.Warning($"{item.ProxyPlugin.Name}->port:{item.Port}  stoped");
                    item.ProxyPlugin.Stoped(item.Port);
                    item.Socket.SafeClose();
                    GC.Collect();
                    // GC.SuppressFinalize(item);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message + "\r\n" + ex.StackTrace);
                }
            }

            services.Clear();
        }
    }
}