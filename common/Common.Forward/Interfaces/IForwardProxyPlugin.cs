using System;
using Common.Proxy.Interfaces;

namespace Common.ForWard.Interfaces
{
    public interface IForwardProxyPlugin : IProxyPlugin
    {
        public Action<ushort> AfterStart { get; set; }
        public Action<ushort> AfterStop { get; set; }
    }
}