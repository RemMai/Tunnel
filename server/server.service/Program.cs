using Common.Libs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Linq;
using System.Threading;
using Common.Server;
using System.IO;
using Common.Extensions;
using Common.Extensions.AutoInject.Extensions;
using Common.Proxy;
using Common.ForWard.Enums;
using Common.Proxy.Enums;
using Common.Proxy.Implementations;
using Common.Server.Interfaces;
using Common.Server.Models;
using Common.Vea.Enums;
using Common.Vea.Implementations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Server.Service.ForWard.Implementations;
using Server.Service.Users.Implementations;
using Server.Service.Vea.Implementations;


ThreadPool.SetMinThreads(150, 150);
Assembly[] assemblies = new[]
{
    typeof(ForwardMessenger).Assembly,
    typeof(UsersMessenger).Assembly,
    typeof(ProxyMessenger).Assembly,
    typeof(VeaSocks5ProxyPlugin).Assembly,
    typeof(VeaMessenger).Assembly,
    typeof(VeaSocks5MessengerIds).Assembly,
    //以下是为了获取信息
    typeof(SignInMessengerIds).Assembly,
    typeof(ProxyMessengerIds).Assembly,
    typeof(ForwardMessengerIds).Assembly,
    typeof(SignInAccessValidator).Assembly,
}.Concat(AppDomain.CurrentDomain.GetAssemblies()).Distinct().ToArray();


var host = Host.CreateApplicationBuilder();
host.Services.AddAutoInject(assemblies);
host.Services.AddDefaultSerilog();
var app = host.Build();
AppDomain.CurrentDomain.UnhandledException += (a, b) => { Log.Error(b.ExceptionObject + ""); };

Log.Information("正在启动...");
PluginLoader.Init(app.Services, assemblies);
PrintProxyPlugin(app.Services);
var config = app.Services.GetService<Server.Config>();
Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
Log.Information("没什么报红的，就说明运行成功了");
Log.Information($"UDP端口:{config.Udp}");
Log.Information($"TCP端口:{config.Tcp}");
Log.Warning($"当前版本：{Helper.Version}，如果客户端版本与此不一致，则可能无法连接");
Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));

app.Run();

void PrintProxyPlugin(IServiceProvider services)
{
    var iAccesses = services.GetServices<IAccess>().Concat(ProxyPluginLoader.plugins.Values)
        .OrderBy(c => c.Access)
        .Distinct()
        .ToList();
    Log.Information(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
    Log.Debug("权限值,uint 每个权限占一位，最多32个权限");
   
    foreach (var item in iAccesses)
    {
        Log.Information(
            $"{Convert.ToString(item.Access, 2).PadLeft(Logger.Instance.PaddingWidth, '0')}  {item.Name}");
    }

    Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
}