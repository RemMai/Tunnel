using System;
using Common.ForWard;
using Common.Libs;
using Common.Proxy;
using Common.Server;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using client.service.forward.Implementations;
using Common.ForWard.Interfaces;

namespace Client.Service.ForWard
{
    public sealed class Plugin : IPlugin
    {
        public void Init(IServiceProvider services, Assembly[] assemblies)
        {
            ProxyPluginLoader.LoadPlugin(services.GetService<IForwardProxyPlugin>());
            ForwardTransfer forwardTransfer = services.GetService<ForwardTransfer>();

            Common.ForWard.Config config = services.GetService<Common.ForWard.Config>();

            Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
            Logger.Instance.Debug($"端口转发已加载，插件id:{config.Plugin}");
            if (config.ConnectEnable)
            {
                Logger.Instance.Debug($"端口转发已允许连接");
            }
            else
            {
                Logger.Instance.Info($"端口转发未允许连接");
            }
            forwardTransfer.Start();
            Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
        }
    }
}
