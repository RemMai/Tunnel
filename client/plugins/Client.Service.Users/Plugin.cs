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
            services.GetService<Common.User.Config>();
            Log.Information("账号权限模块已加载...");
        }
    }
}