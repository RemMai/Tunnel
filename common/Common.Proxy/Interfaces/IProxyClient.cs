using System.Threading.Tasks;
using Common.Proxy.Models;

namespace Common.Proxy.Interfaces
{
    public interface IProxyClient
    {
        Task InputData(ProxyInfo data);
    }
}
