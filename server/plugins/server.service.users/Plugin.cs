using Microsoft.Extensions.DependencyInjection;
using Common.Server;
using System.Reflection;
using Common.Libs;
using System;

namespace Server.Service.Users
{
    public sealed class Plugin : IPlugin
    {
        public void Init(IServiceProvider services, Assembly[] assemblies)
        {
            var config = services.GetService<Common.User.Config>();
            Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
            Logger.Instance.Info("账号模块已加载");
            if (config.Enable)
            {
                Logger.Instance.Debug($"已启用账号验证");
            }
            else
            {
                Logger.Instance.Info($"未启用账号验证");
            }
            Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
        }
    }
}
