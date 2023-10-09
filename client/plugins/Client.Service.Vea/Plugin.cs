using System;
using Common.Libs;
using Common.Proxy;
using Common.Server;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Client.Service.Vea
{
    public sealed class Plugin : IPlugin
    {
        public void Init(IServiceProvider services, Assembly[] assemblies)
        {
            ProxyPluginLoader.LoadPlugin(services.GetService<IVeaSocks5ProxyPlugin>());
            var transfer = services.GetService<VeaTransfer>();

            Config config = services.GetService<Config>();

            Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
            Logger.Instance.Debug($"虚拟网卡插件已加载，插件id:{config.Plugin}");
            if (config.ListenEnable)
            {
                Logger.Instance.Debug($"虚拟网卡插件已开启");
            }
            else
            {
                Logger.Instance.Info($"虚拟网卡插件未开启");
            }

            if (config.ConnectEnable)
            {
                Logger.Instance.Debug($"虚拟网卡插件已允许连接");
            }
            else
            {
                Logger.Instance.Info($"虚拟网卡插件未允许连接");
            }

            Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
        }

        public void LoadBefore(IServiceCollection services, Assembly[] assemblies)
        {
  
        }
    }
}