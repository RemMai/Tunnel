using System.Net;
using Common.ForWard.Enums;

namespace client.service.forward.server.Models
{
    public sealed class ServerForwardItemInfo
    {
        public ForwardAliveTypes AliveType { get; set; } = ForwardAliveTypes.Web;
        public string Domain { get; set; }
        public ushort ServerPort { get; set; }
        public IPAddress LocalIp { get; set; }
        public ushort LocalPort { get; set; }
        public string Desc { get; set; } = string.Empty;
        public bool Listening { get; set; } = false;
    }
}