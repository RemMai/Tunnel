using System;
using System.Net;
using Common.Libs.Extends;

namespace Common.Proxy
{
    public sealed class HttpHeaderCacheInfo
    {
        public IPAddress Addr { get; set; }
        public string Name { get; set; }
        public string Proxy { get; set; }

        public byte[] Build()
        {
            return $"Snltty-Addr: {Addr}\r\nSnltty-Node: {Uri.UnescapeDataString(Name)}\r\nSnltty-Proxy: {Proxy}\r\n".ToBytes();
        }
    }
}