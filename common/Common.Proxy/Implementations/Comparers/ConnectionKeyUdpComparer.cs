using System.Collections.Generic;
using Common.Proxy.Models;

namespace Common.Proxy.Implementations.Comparers
{
    public sealed class ConnectionKeyUdpComparer : IEqualityComparer<ConnectionKeyUdp>
    {
        public bool Equals(ConnectionKeyUdp x, ConnectionKeyUdp y)
        {
            return x.Source != null && x.Source.Equals(y.Source) && x.ConnectId == y.ConnectId;
        }
        public int GetHashCode(ConnectionKeyUdp obj)
        {
            if (obj.Source == null) return 0;
            return obj.Source.GetHashCode() ^ obj.ConnectId.GetHashCode();
        }
    }
}