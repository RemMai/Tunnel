using System;
using System.Linq;
using System.Net;
using Common.Extensions.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Realize
{
    [AutoInject(ServiceLifetime.Singleton, typeof(IIPv6AddressRequest))]
    internal sealed class IPv6AddressRequest : IIPv6AddressRequest
    {
        private readonly byte[] ipv6LocalBytes = new byte[] { 254, 128, 0, 0, 0, 0, 0, 0 };
        public IPAddress[] GetIPV6() 
        {
            return Dns.GetHostAddresses(Dns.GetHostName())
                 .Where(c => c.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                 .Where(c => c.GetAddressBytes().AsSpan(0, 8).SequenceEqual(ipv6LocalBytes) == false).ToArray();
        }
    }
}
