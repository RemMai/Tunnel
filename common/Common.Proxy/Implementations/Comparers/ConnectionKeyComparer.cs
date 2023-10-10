using System.Collections.Generic;
using Common.Proxy.Models;

namespace Common.Proxy.Implementations.Comparers
{
    public sealed class ConnectionKeyComparer : IEqualityComparer<ConnectionKey>
    {
        public bool Equals(ConnectionKey x, ConnectionKey y)
        {
            return x.RequestId == y.RequestId && x.ConnectId == y.ConnectId;
        }

        public int GetHashCode(ConnectionKey obj)
        {
            return obj.RequestId.GetHashCode() ^ obj.ConnectId.GetHashCode();
        }
    }
}