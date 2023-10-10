using Common.Proxy.Models;

namespace Common.Proxy.Interfaces
{
    public interface IProxyPluginValidator
    {
        bool Validate(ProxyInfo info);
    }
}