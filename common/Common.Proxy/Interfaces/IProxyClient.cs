using System.Threading.Tasks;


namespace Common.Proxy
{
    public interface IProxyClient
    {
        Task InputData(ProxyInfo data);
    }
}
