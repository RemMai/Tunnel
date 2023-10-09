using Common.Libs;
using Common.Server;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using Client.Service.Ui.Api.ClientServer;
using Client.Service.Ui.Api.Service.WebServer;

namespace Client.Service.Ui.Api.Service
{
    public sealed class Plugin : IPlugin
    {
        public void Init(IServiceProvider services, Assembly[] assemblies)
        {
            LoadWebAfter(services, assemblies);
            LoadApiAfter(services, assemblies);
        }

        private void LoadWebAfter(IServiceProvider services, Assembly[] assemblies)
        {
            var config = services.GetService<Config>();

            if (config.EnableWeb)
            {
                services.GetService<IWebServer>().Start();
                Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
                Logger.Instance.Debug("管理UI，web已启用");
                Logger.Instance.Info($"管理UI web1 :http://{config.Web.BindIp}:{config.Web.Port}");
                Logger.Instance.Info($"管理UI web2 :https://snltty.gitee.io/p2p-tunnel");
                Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
            }
            else
            {
                Logger.Instance.Debug("管理UI，web未启用");
            }
        }

        private void LoadApiAfter(IServiceProvider services, Assembly[] assemblies)
        {
            IClientServer clientServer = services.GetService<IClientServer>();

            var config = services.GetService<Config>();

            Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
            if (config.EnableWeb)
            {
                clientServer.Websocket();
                Logger.Instance.Debug($"管理UI，websocket已启用:{config.Websocket.BindIp}:{config.Websocket.Port}");
            }
            else
            {
                Logger.Instance.Info($"管理UI，websocket未启用");
            }

            if (config.EnableCommand)
            {
                clientServer.NamedPipe();
                Logger.Instance.Debug($"管理UI，命令行已启用");
            }
            else
            {
                Logger.Instance.Info($"管理UI，命令行未启用");
            }

            if (config.EnableApi)
            {
                clientServer.LoadPlugins(assemblies);
                Logger.Instance.Debug($"管理UI，api已启用");
            }
            else
            {
                Logger.Instance.Info($"管理UI，api未启用");
            }

            Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
        }
    }
}