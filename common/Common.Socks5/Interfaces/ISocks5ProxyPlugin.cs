using System.Net;
using Common.Proxy;

namespace Common.Socks5.Interfaces
{
    public interface ISocks5ProxyPlugin : IProxyPlugin
    {
        public IPAddress ProxyIp => IPAddress.Any;
    }
}