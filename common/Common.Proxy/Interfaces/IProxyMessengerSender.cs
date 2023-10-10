using System.Threading.Tasks;
using Common.Proxy.Models;

namespace Common.Proxy.Interfaces
{
    public interface IProxyMessengerSender
    {
        public Task<bool> Request(ProxyInfo data, bool unconnectedMessage = false);
        public Task<bool> Response(ProxyInfo data, bool unconnectedMessage = false);

        public Task<bool> ResponseClose(ProxyInfo data);
        public Task<bool> RequestClose(ProxyInfo data);
    }
}