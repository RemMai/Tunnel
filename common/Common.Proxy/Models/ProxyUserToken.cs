using System.Net.Sockets;

namespace Common.Proxy
{
    public sealed class ProxyUserToken
    {
        public byte Rsv { get; set; }
        public Socket Socket { get; set; }
        public ServerInfo Server { get; set; }
        public SocketAsyncEventArgs Saea { get; set; }
        public byte[] PoolBuffer { get; set; }
        public bool Receive { get; set; }

        public ProxyInfo Request { get; set; } = new ProxyInfo { };
    }
}