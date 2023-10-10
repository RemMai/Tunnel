using System.Threading.Tasks;
using Common.Proxy.Enums;
using Common.Proxy.Models;

namespace Common.Proxy.Interfaces
{
    public interface IProxyServer
    {
        public bool Start(ushort port, byte plugin, byte rsv = 0);
        public void Stop(byte plugin);
        public void Stop(ushort port);
        public void Stop();
        public void LastError(ushort port, out EnumProxyCommandStatusMsg commandStatusMsg);
        public Task InputData(ProxyInfo info);
    }


   

}