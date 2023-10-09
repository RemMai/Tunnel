using Microsoft.Extensions.DependencyInjection;
using Common.Libs;
using Common.Server;
using System.Reflection;
using Common.ForWard;
using Common.Proxy;
using System;

namespace Server.Service.ForWard
{
    public sealed class Plugin : IPlugin
    {
        public void Init(IServiceProvider services, Assembly[] assemblies)
        {
            ProxyPluginLoader.LoadPlugin(services.GetService<IForwardProxyPlugin>());
            
            IProxyServer proxyServer = services.GetService<IProxyServer>();

            Common.ForWard.Config config = services.GetService<Common.ForWard.Config>();
            Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
            Logger.Instance.Info($"端口转发穿透已加载，插件id:{config.Plugin}");
            if (config.ConnectEnable)
            {
                Logger.Instance.Debug($"端口转发穿透已允许注册");
            }
            else
            {
                Logger.Instance.Info($"端口转发穿透未允许注册");
            }

            Logger.Instance.Info("端口转发穿透服务已启动...");
            foreach (ushort port in config.WebListens)
            {
                proxyServer.Start(port, config.Plugin, (byte)ForwardAliveTypes.Web);
                Logger.Instance.Warning($"端口转发穿透监听:{port}");
            }

            Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
        }
    }
}
