using System.Net.Sockets;
using Common.Proxy.Enums;
using Common.Proxy.Interfaces;

namespace Common.Proxy.Models
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