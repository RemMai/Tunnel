using System;
using System.Collections.Concurrent;
using Common.Proxy;
using Common.Proxy.Interfaces;
using Common.Proxy.Models;
using Serilog;

namespace Common.Proxy.Implementations
{
    public static class ProxyPluginLoader
    {
        delegate void DelegateValidateData(Memory<byte> data);

        delegate void DelegateValidateAccess(ProxyInfo info);

        delegate void DelegateCommandAnswer(ProxyInfo info);

        public static ConcurrentDictionary<byte, IProxyPlugin> plugins = new ConcurrentDictionary<byte, IProxyPlugin>();

        public static void LoadPlugin(IProxyPlugin plugin)
        {
            if (plugins.ContainsKey(plugin.Id))
            {
                Log.Error($"plugin {plugin.Id} : {plugin.GetType().Name} already exists");
            }
            else
            {
                plugins.TryAdd(plugin.Id, plugin);
            }
        }

        public static bool GetPlugin(byte id, out IProxyPlugin plugin)
        {
            return plugins.TryGetValue(id, out plugin);
        }
    }
}