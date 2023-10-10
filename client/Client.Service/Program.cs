using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using Client;
using Client.Messengers.Signin;
using Client.Realize.Messengers.PunchHole;
using Client.Service.ForWard;
using Client.Service.ForWard.Server;
using client.service.forward.server.Implementations;
using Client.Service.Logger;
using client.service.logger.Implementations;
using Client.Service.Proxy;
using Client.Service.Proxy.Implementations;
using client.service.ui.api.service.Implementations;
using Client.Service.Users.Implementations;
using Client.Service.Users.Server.Implementations;
using Client.Service.Vea;
using Client.Service.Vea.Implementations;
using Client.Service.Vea.Server;
using Client.Service.Vea.Server.Implementations;
using Common.Extensions;
using Common.Extensions.AutoInject.Extensions;
using Common.ForWard.Enums;
using Common.Libs;
using Common.Proxy;
using Common.Proxy.Enums;
using Common.Proxy.Implementations;
using Common.Server;
using Common.Server.Enums;
using Common.Server.Interfaces;
using Common.Server.Models;
using Common.User.Enums;
using Common.Vea.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

ThreadPool.SetMinThreads(1024, 1024);
ThreadPool.SetMaxThreads(65535, 65535);

Assembly[] assemblies = new[]
{
    typeof(ClientServer).Assembly,
    typeof(LoggerClientService).Assembly,
    typeof(PunchHoleMessenger).Assembly,
    typeof(SignInMessengerIds).Assembly,
    typeof(ProxyMessenger).Assembly,
    typeof(ProxyClientService).Assembly,
    typeof(ProxyMessengerIds).Assembly,
    typeof(IIPv6AddressRequest).Assembly,
    typeof(ForwardClientService).Assembly,
    typeof(ServerForwardClientService).Assembly,
    typeof(ForwardMessengerIds).Assembly,
    typeof(VeaClientService).Assembly,
    typeof(ServerVeaClientService).Assembly,
    typeof(VeaSocks5MessengerIds).Assembly,
    typeof(UsersClientService).Assembly,
    typeof(ServerUsersClientService).Assembly,
    typeof(UsersMessengerIds).Assembly,
}.Concat(AppDomain.CurrentDomain.GetAssemblies()).Distinct().ToArray();

AppDomain.CurrentDomain.UnhandledException += (a, b) => { Log.Error(b.ExceptionObject + ""); };
var host = Host.CreateApplicationBuilder();
host.Services.AddAutoInject(assemblies);
host.Services.AddDefaultSerilog();
var app = host.Build();
PluginLoader.Init(app.Services, assemblies);
PrintProxyPlugin(app.Services, assemblies);
SignInStateInfo signInStateInfo = app.Services.GetService<SignInStateInfo>();
Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
Log.Information("获取外网距离ing...");
signInStateInfo.LocalInfo.RouteLevel = NetworkHelper.GetRouteLevel(out List<IPAddress> ips);
signInStateInfo.LocalInfo.RouteIps = ips.ToArray();
Log.Information($"外网距离:{signInStateInfo.LocalInfo.RouteLevel}");
Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
Log.Warning("没什么报红的，就说明运行成功了");
Log.Warning($"当前版本：{Helper.Version}，如果与服务器版本不一致，则可能无法连接");
Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));

var config = app.Services.GetService<Client.Config>();
app.Services.GetService<ISignInTransfer>().SignIn(config.Client.AutoReg);

app.Run();

static void PrintProxyPlugin(IServiceProvider services, Assembly[] assemblies)
{
    var iAccesses = ReflectionHelper.GetInterfaceSchieves(assemblies, typeof(IAccess))
        .Distinct().Select(services.GetService).Concat(ProxyPluginLoader.plugins.Values)
        .OfType<IAccess>();

    Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
    Log.Debug("权限值,uint 每个权限占一位，最多32个权限");
    Log.Information(
        $"{Convert.ToString((uint)EnumServiceAccess.Relay, 2).PadLeft(Logger.Instance.PaddingWidth, '0')}  Relay");
    foreach (var item in iAccesses.OrderBy(c => c.Access))
    {
        Log.Information(
            $"{Convert.ToString(item.Access, 2).PadLeft(Logger.Instance.PaddingWidth, '0')}  {item.Name}");
    }

    Log.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
}