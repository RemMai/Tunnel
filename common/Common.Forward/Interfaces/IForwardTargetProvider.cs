using Common.Proxy;
using Common.Proxy.Models;

namespace Common.ForWard.Interfaces
{
    public interface IForwardTargetProvider
    {
        bool Contains(ushort port);
        void Get(string domain, ProxyInfo info);
        void Get(ushort port, ProxyInfo info);
    }
}