﻿using System;
using Common.Server;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Client.Service.Logger.Implementations;
using Serilog;

namespace Client.Service.Logger
{
    public sealed class Plugin : IPlugin
    {
        public void Init(IServiceProvider services, Assembly[] assemblies)
        {
            LoggerClientService plugin = services.GetService<LoggerClientService>();
            Client.Service.logger.Models.Config config = services.GetService<Client.Service.logger.Models.Config>();
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

            Log.Warning(string.Empty.PadRight(Common.Libs.Logger.Instance.PaddingWidth, '='));
            Log.Debug($"日志收集已加载");
            if (config.Enable)
            {
                Log.Debug($"日志收集已启用：最长条数:{config.MaxLength}");
            }
            else
            {
                Log.Information($"日志收集未启用");
            }

            Log.Warning(string.Empty.PadRight(Common.Libs.Logger.Instance.PaddingWidth, '='));
        }
    }
}