using System.Net.Sockets;
using Common.proxy.Enums;

namespace Common.Proxy
{
    public sealed class ServerInfo
    {
        public IProxyPlugin ProxyPlugin { get; set; }
        public ushort Port { get; set; }
        public Socket Socket { get; set; }
        public UdpClient UdpClient { get; set; }
        public EnumProxyCommandStatusMsg LastError { get; set; }
    }
}