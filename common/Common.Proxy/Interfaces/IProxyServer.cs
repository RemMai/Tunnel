using Common.Libs.Extends;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Libs;
using Common.Libs.AutoInject.Attributes;
using Common.proxy.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Proxy
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