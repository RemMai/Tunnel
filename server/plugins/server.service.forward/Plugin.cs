using Microsoft.Extensions.DependencyInjection;
using Common.Libs;
using Common.Server;
using System.Reflection;
using System;
using Common.ForWard.Enums;
using Common.ForWard.Interfaces;
using Common.Proxy.Implementations;
using Common.Proxy.Interfaces;
using Serilog;

namespace Server.Service.ForWard
{
    public sealed class Plugin : IPlugin
    {
        public void Init(IServiceProvider services, Assembly[] assemblies)
        {
            ProxyPluginLoader.LoadPlugin(services.GetService<IForwardProxyPlugin>());
            
            IProxyServer proxyServer = services.GetService<IProxyServer>();

            Common.ForWard.Config config = services.GetService<Common.ForWard.Config>();
            Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
            Log.Information($"端口转发穿透已加载，插件id:{config.Plugin}");
            if (config.ConnectEnable)
            {
                Log.Debug($"端口转发穿透已允许注册");
            }
            else
            {
                Log.Information($"端口转发穿透未允许注册");
            }

            Log.Information("端口转发穿透服务已启动...");
            foreach (ushort port in config.WebListens)
            {
                proxyServer.Start(port, config.Plugin, (byte)ForwardAliveTypes.Web);
                Log.Warning($"端口转发穿透监听:{port}");
            }

            Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
        }
    }
}
