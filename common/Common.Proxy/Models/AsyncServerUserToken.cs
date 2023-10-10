using System;
using System.Net.Sockets;
using Common.Libs;
using Common.Libs.Extends;

namespace Common.Proxy
{
    public sealed class AsyncServerUserToken
    {
        public ConnectionKey Key { get; set; }
        public Socket TargetSocket { get; set; }
        public ProxyInfo Data { get; set; }
        public bool IsClosed { get; set; } = false;
        public byte[] PoolBuffer { get; set; }

        public void Clear()
        {
            TargetSocket?.SafeClose();

            PoolBuffer = Helper.EmptyArray;
            GC.Collect();
            GC.SuppressFinalize(this);
        }
    }
}