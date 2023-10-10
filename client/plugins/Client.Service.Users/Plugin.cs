using System;
using Microsoft.Extensions.DependencyInjection;
using Common.Server;
using System.Reflection;
using Common.Libs;
using Serilog;

namespace Client.Service.Users
{
    public sealed class Plugin : IPlugin
    {
        public void Init(IServiceProvider services, Assembly[] assemblies)
        {
            var config = services.GetService<Common.User.Config>();
            Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
            Log.Information("账号权限模块已加载");
            Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
        }
    }
}
