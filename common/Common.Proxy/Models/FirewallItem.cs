using System;
using Common.Proxy.Enums;

namespace Common.Proxy.Models
{
    public sealed class FirewallItem
    {
        public uint ID { get; set; }
        public byte PluginId { get; set; }
        public FirewallProtocolType Protocol { get; set; }
        public FirewallType Type { get; set; }
        public string Port { get; set; } = string.Empty;
        public string[] IP { get; set; } = Array.Empty<string>();
        public string Remark { get; set; } = string.Empty;
    }
}