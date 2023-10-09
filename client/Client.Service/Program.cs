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
using Client.Service.Logger;
using Client.Service.Proxy;
using Client.Service.Ui.Api.Service.ClientServer;
using Client.Service.Users;
using Client.Service.Users.Server;
using Client.Service.Vea;
using Client.Service.Vea.Server;
using Common.Libs;
using Common.Libs.AutoInject.Extensions;
using Common.Proxy;
using Common.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

ThreadPool.SetMinThreads(1024, 1024);
ThreadPool.SetMaxThreads(65535, 65535);

Assembly[] assemblies = new[]
{
    typeof(ClientServer).Assembly,
    typeof(LoggerClientService).Assembly,
    typeof(PunchHoleMessenger).Assembly,
    typeof(Common.Server.Model.SignInMessengerIds).Assembly,
    typeof(ProxyMessenger).Assembly,
    typeof(ProxyClientService).Assembly,
    typeof(ProxyMessengerIds).Assembly,
    typeof(IIPv6AddressRequest).Assembly,
    typeof(ForwardClientService).Assembly,
    typeof(ServerForwardClientService).Assembly,
    typeof(Common.ForWard.ForwardMessengerIds).Assembly,
    typeof(VeaClientService).Assembly,
    typeof(ServerVeaClientService).Assembly,
    typeof(Common.Vea.VeaSocks5MessengerIds).Assembly,
    typeof(UsersClientService).Assembly,
    typeof(ServerUsersClientService).Assembly,
    typeof(Common.User.UsersMessengerIds).Assembly,
}.Concat(AppDomain.CurrentDomain.GetAssemblies()).Distinct().ToArray();
AppDomain.CurrentDomain.UnhandledException += (a, b) => { Logger.Instance.Error(b.ExceptionObject + ""); };
LoggerConsole();
var host = Host.CreateApplicationBuilder();
host.Services.AddAutoInject(assemblies);
var app = host.Build();
PluginLoader.Init(app.Services, assemblies);
PrintProxyPlugin(app.Services, assemblies);

SignInStateInfo signInStateInfo = app.Services.GetService<SignInStateInfo>();
Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
Logger.Instance.Info("获取外网距离ing...");
signInStateInfo.LocalInfo.RouteLevel = NetworkHelper.GetRouteLevel(out List<IPAddress> ips);
signInStateInfo.LocalInfo.RouteIps = ips.ToArray();
Logger.Instance.Info($"外网距离:{signInStateInfo.LocalInfo.RouteLevel}");
Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));

Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
Logger.Instance.Warning("没什么报红的，就说明运行成功了");
Logger.Instance.Warning($"当前版本：{Helper.Version}，如果与服务器版本不一致，则可能无法连接");
Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));

var config = app.Services.GetService<Client.Config>();
app.Services.GetService<ISignInTransfer>().SignIn(config.Client.AutoReg);

app.Run();
return;



void LoggerConsole()
{
    if (Directory.Exists("log") == false)
    {
        Directory.CreateDirectory("log");
    }

    Logger.Instance.OnLogger += (model) =>
    {
        ConsoleColor currentForeColor = Console.ForegroundColor;
        switch (model.Type)
        {
            case LoggerTypes.DEBUG:
                Console.ForegroundColor = ConsoleColor.Blue;
                break;
            case LoggerTypes.INFO:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LoggerTypes.WARNING:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LoggerTypes.ERROR:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            default:
                break;
        }

        string line = $"[{model.Type,-7}][{model.Time:yyyy-MM-dd HH:mm:ss}]:{model.Content}";
        Console.WriteLine(line);
        Console.ForegroundColor = currentForeColor;

        using StreamWriter sw = File.AppendText(Path.Combine("log", $"{DateTime.Now:yyyy-MM-dd}.log"));
        sw.WriteLine(line);
        sw.Flush();
        sw.Close();
        sw.Dispose();
    };
}

static void PrintProxyPlugin(IServiceProvider services, Assembly[] assemblies)
{
    var iAccesses = ReflectionHelper.GetInterfaceSchieves(assemblies, typeof(IAccess))
        .Distinct().Select(services.GetService).Concat(ProxyPluginLoader.plugins.Values)
        .OfType<IAccess>();

    Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
    Logger.Instance.Debug("权限值,uint 每个权限占一位，最多32个权限");
    Logger.Instance.Info(
        $"{Convert.ToString((uint)EnumServiceAccess.Relay, 2).PadLeft(Logger.Instance.PaddingWidth, '0')}  Relay");
    foreach (var item in iAccesses.OrderBy(c => c.Access))
    {
        Logger.Instance.Info(
            $"{Convert.ToString(item.Access, 2).PadLeft(Logger.Instance.PaddingWidth, '0')}  {item.Name}");
    }

    Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
}