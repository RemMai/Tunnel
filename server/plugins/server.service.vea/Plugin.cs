using Microsoft.Extensions.DependencyInjection;
using Common.Server;
using System.Reflection;
using Common.Libs;
using System;
using Common.Proxy.Implementations;
using Serilog;
using Server.Service.Vea.Implementations;

namespace Server.Service.Vea
{
    public sealed class Plugin : IPlugin
    {
        public void Init(IServiceProvider services, Assembly[] assemblies)
        {
            ProxyPluginLoader.LoadPlugin(services.GetService<IVeaSocks5ProxyPlugin>());
            var config = services.GetService<Common.Vea.Config>();
            Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
            Log.Information("组网自动分配IP模块已加载");
            Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
        }
    }
}
