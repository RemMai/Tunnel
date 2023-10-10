using System;

namespace client.service.forward.server.Models
{
    public sealed class ServerForwardWrapInfo
    {
        public ServerForwardItemInfo[] Webs { get; set; } = Array.Empty<ServerForwardItemInfo>();
        public ServerForwardItemInfo[] Tunnels { get; set; } = Array.Empty<ServerForwardItemInfo>();
    }
}