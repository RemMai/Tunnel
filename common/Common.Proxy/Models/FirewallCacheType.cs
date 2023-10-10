using System;
using Common.Proxy.Enums;

namespace Common.Proxy.Models
{
    public sealed class FirewallCacheType
    {
        public FirewallProtocolType Protocols { get; set; }
        public byte PluginIds { get; set; }
        public ulong[] Ips { get; set; } = Array.Empty<ulong>();
    }
}