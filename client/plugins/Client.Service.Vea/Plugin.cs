using System;
using Common.Libs;
using Common.Server;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Client.Service.Vea.Implementations;
using Client.Service.Vea.Interfaces;
using Common.Proxy.Implementations;
using Serilog;

namespace Client.Service.Vea
{
    public sealed class Plugin : IPlugin
    {
        public void Init(IServiceProvider services, Assembly[] assemblies)
        {
            ProxyPluginLoader.LoadPlugin(services.GetService<IVeaSocks5ProxyPlugin>());
            var transfer = services.GetService<VeaTransfer>();

            Models.Config config = services.GetService<Models.Config>();

            Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
            Log.Debug($"虚拟网卡插件已加载，插件id:{config.Plugin}");
            if (config.ListenEnable)
            {
                Log.Debug($"虚拟网卡插件已开启");
            }
            else
            {
                Log.Information($"虚拟网卡插件未开启");
            }

            if (config.ConnectEnable)
            {
                Log.Debug($"虚拟网卡插件已允许连接");
            }
            else
            {
                Log.Information($"虚拟网卡插件未允许连接");
            }

            Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
        }
    }
}