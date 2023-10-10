using System;
using Common.proxy.Enums;

namespace Common.Proxy
{
    public sealed class FirewallCacheType
    {
        public FirewallProtocolType Protocols { get; set; }
        public byte PluginIds { get; set; }
        public ulong[] Ips { get; set; } = Array.Empty<ulong>();
    }
}