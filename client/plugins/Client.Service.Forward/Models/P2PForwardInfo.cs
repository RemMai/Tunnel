using System.Net;

namespace client.service.forward.Models
{
    public sealed class P2PForwardInfo
    {
        public uint ID { get; set; }
        public ulong ConnectionId { get; set; }
        public string SourceIp { get; set; } = string.Empty;
        public IPAddress TargetIp { get; set; } = IPAddress.Any;
        public ushort TargetPort { get; set; }
        public string Desc { get; set; } = string.Empty;
    }
}