using System;
using Common.Libs;
using Common.Server;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Client.Service.forward.Implementations;
using Common.ForWard.Interfaces;
using Common.Proxy.Implementations;
using Serilog;

namespace Client.Service.ForWard
{
    public sealed class Plugin : IPlugin
    {
        public void Init(IServiceProvider services, Assembly[] assemblies)
        {
            ProxyPluginLoader.LoadPlugin(services.GetService<IForwardProxyPlugin>());
            ForwardTransfer forwardTransfer = services.GetService<ForwardTransfer>();

            Common.ForWard.Config config = services.GetService<Common.ForWard.Config>();

            Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
            Log.Debug($"端口转发已加载，插件id:{config.Plugin}");
            if (config.ConnectEnable)
            {
                Log.Debug($"端口转发已允许连接");
            }
            else
            {
                Log.Information($"端口转发未允许连接");
            }
            forwardTransfer.Start();
            Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
        }
    }
}
