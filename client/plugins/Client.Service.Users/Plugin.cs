using System;
using Microsoft.Extensions.DependencyInjection;
using Common.Server;
using System.Reflection;
using Common.User;
using Common.Libs;

namespace Client.Service.Users
{
    public sealed class Plugin : IPlugin
    {
        public void Init(IServiceProvider services, Assembly[] assemblies)
        {
            var config = services.GetService<Common.User.Config>();
            Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
            Logger.Instance.Info("账号权限模块已加载");
            Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
        }

        public void LoadBefore(IServiceCollection services, Assembly[] assemblies)
        {
            // services.AddSingleton<Common.User.Config>();
            // services.AddSingleton<IUserStore, UserStore>();
            // services.AddSingleton<IUserMapInfoCaching, UserMapInfoCaching>();
        }
    }
}
