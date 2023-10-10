using System.Collections.Generic;
using Common.ForWard;
using Common.ForWard.Enums;
using Common.Proxy;
using Common.Proxy.Enums;

namespace client.service.forward.Models
{
    public sealed class P2PListenInfo
    {
        public uint ID { get; set; }
        public ushort Port { get; set; }
        public ForwardAliveTypes AliveType { get; set; } = ForwardAliveTypes.Web;
        public List<P2PForwardInfo> Forwards { get; set; } = new List<P2PForwardInfo>();
        public bool Listening { get; set; } = false;
        public string Desc { get; set; } = string.Empty;
        public EnumProxyCommandStatusMsg LastError { get; set; }
    }
}