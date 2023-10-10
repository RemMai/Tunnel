using System.Net;
using Common.Server;
using Common.Server.Interfaces;

namespace Common.Proxy
{
    public sealed class ProxyInfo : ProxyBaseInfo
    {
        public ushort ListenPort { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public IConnection Connection { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public IProxyPlugin ProxyPlugin { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public IPEndPoint ClientEP { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public HttpHeaderCacheInfo HeadersCache { get; set; }

        public bool IsMagicData { get; set; }
    }
}
