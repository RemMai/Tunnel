using Common.ForWard;
using Common.ForWard.Enums;

namespace Client.Service.forward.Models
{
    public sealed class P2PListenAddParams
    {
        public uint ID { get; set; }
        public ushort Port { get; set; }
        public bool Listening { get; set; }
        public ForwardAliveTypes AliveType { get; set; } = ForwardAliveTypes.Web;
        public string Desc { get; set; } = string.Empty;
    }
}