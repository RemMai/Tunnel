using System;
using System.Collections.Concurrent;
using Common.Libs;
using Common.Libs.Extends;

namespace Common.Proxy
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
                    Logger.Instance.Warning($"{c.ProxyPlugin.Name}->port:{port}  stoped");
                    c.ProxyPlugin.Stoped(port);
                    c.Socket.SafeClose();
                    c.UdpClient.Dispose();
                    GC.Collect();
                }
                catch (Exception ex)
                {
                    if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                        Logger.Instance.Error(ex);
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
                    Logger.Instance.Warning($"{item.ProxyPlugin.Name}->port:{item.Port}  stoped");
                    item.ProxyPlugin.Stoped(item.Port);
                    item.Socket.SafeClose();
                    GC.Collect();
                    // GC.SuppressFinalize(item);
                }
                catch (Exception ex)
                {
                    if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                        Logger.Instance.Error(ex);
                }
            }

            services.Clear();
        }
    }
}