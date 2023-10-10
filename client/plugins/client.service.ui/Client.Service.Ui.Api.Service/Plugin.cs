using Common.Libs;
using Common.Server;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using client.service.ui.api.Interfaces;
using Client.Service.Ui.Api.Service.WebServer;
using Serilog;

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
                Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
                Log.Debug("管理UI，web已启用");
                Log.Information($"管理UI web1 :http://{config.Web.BindIp}:{config.Web.Port}");
                Log.Information($"管理UI web2 :https://snltty.gitee.io/p2p-tunnel");
                Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
            }
            else
            {
                Log.Debug("管理UI，web未启用");
            }
        }

        private void LoadApiAfter(IServiceProvider services, Assembly[] assemblies)
        {
            IClientServer clientServer = services.GetService<IClientServer>();

            var config = services.GetService<Config>();

            Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
            if (config.EnableWeb)
            {
                clientServer.Websocket();
                Log.Debug($"管理UI，websocket已启用:{config.Websocket.BindIp}:{config.Websocket.Port}");
            }
            else
            {
                Log.Information($"管理UI，websocket未启用");
            }

            if (config.EnableCommand)
            {
                clientServer.NamedPipe();
                Log.Debug($"管理UI，命令行已启用");
            }
            else
            {
                Log.Information($"管理UI，命令行未启用");
            }

            if (config.EnableApi)
            {
                clientServer.LoadPlugins(assemblies);
                Log.Debug($"管理UI，api已启用");
            }
            else
            {
                Log.Information($"管理UI，api未启用");
            }

            Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
        }
    }
}