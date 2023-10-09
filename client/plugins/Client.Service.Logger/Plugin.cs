using System;
using Common.Libs;
using Common.Server;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Client.Service.Logger
{
    public sealed class Plugin : IPlugin
    {
        public void Init(IServiceProvider services, Assembly[] assemblies)
        {
            LoggerClientService plugin = services.GetService<LoggerClientService>();
            Config config = services.GetService<Config>();
            Common.Libs.Logger.Instance.OnLogger += (model) =>
            {
                if (config.Enable)
                {
                    plugin.Data.Add(model);
                    if (plugin.Data.Count > config.MaxLength)
                    {
                        plugin.Data.RemoveAt(0);
                    }
                }
            };

            Common.Libs.Logger.Instance.Warning(string.Empty.PadRight(Common.Libs.Logger.Instance.PaddingWidth, '='));
            Common.Libs.Logger.Instance.Debug($"日志收集已加载");
            if (config.Enable)
            {
                Common.Libs.Logger.Instance.Debug($"日志收集已启用：最长条数:{config.MaxLength}");
            }
            else
            {
                Common.Libs.Logger.Instance.Info($"日志收集未启用");
            }

            Common.Libs.Logger.Instance.Warning(string.Empty.PadRight(Common.Libs.Logger.Instance.PaddingWidth, '='));
        }

        public void LoadBefore(IServiceCollection services, Assembly[] assemblies)
        {
            // services.AddSingleton<Config>();
        }
    }
}