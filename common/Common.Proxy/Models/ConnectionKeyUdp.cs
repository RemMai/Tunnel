using System.Net;

namespace Common.Proxy
{
    public readonly struct ConnectionKeyUdp
    {
        public readonly IPEndPoint Source { get; }
        public readonly ulong ConnectId { get; }

        public ConnectionKeyUdp(ulong connectId, IPEndPoint source)
        {
            ConnectId = connectId;
            Source = source;
        }
    }
}