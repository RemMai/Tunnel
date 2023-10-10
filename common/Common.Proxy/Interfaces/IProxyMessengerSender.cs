using System;
using System.Threading.Tasks;
using System.Text;
using Common.Libs;
using Common.Libs.AutoInject.Attributes;
using Common.Server;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Proxy
{
    public interface IProxyMessengerSender
    {
        public Task<bool> Request(ProxyInfo data, bool unconnectedMessage = false);
        public Task<bool> Response(ProxyInfo data, bool unconnectedMessage = false);

        public Task<bool> ResponseClose(ProxyInfo data);
        public Task<bool> RequestClose(ProxyInfo data);
    }
}