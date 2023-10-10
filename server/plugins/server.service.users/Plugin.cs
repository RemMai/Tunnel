using Microsoft.Extensions.DependencyInjection;
using Common.Server;
using System.Reflection;
using Common.Libs;
using System;
using Serilog;

namespace Server.Service.Users
{
    public sealed class Plugin : IPlugin
    {
        public void Init(IServiceProvider services, Assembly[] assemblies)
        {
            var config = services.GetService<Common.User.Config>();
            Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
            Log.Information("账号模块已加载");
            if (config.Enable)
            {
                Log.Debug($"已启用账号验证");
            }
            else
            {
                Log.Information($"未启用账号验证");
            }
            Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
        }
    }
}
