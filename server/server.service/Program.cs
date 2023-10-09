using Common.Libs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Linq;
using System.Threading;
using Common.Server;
using System.IO;
using Server.Service.Users;
using Common.Proxy;
using Server.Service.ForWard;
using Common.ForWard;
using Common.Libs.AutoInject.Extensions;
using Common.Server.Model;
using Server.Service.Vea;
using Common.Vea;
using Microsoft.Extensions.Hosting;


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

var app = host.Build();

AppDomain.CurrentDomain.UnhandledException += (a, b) => { Logger.Instance.Error(b.ExceptionObject + ""); };
LoggerConsole();
Logger.Instance.Info("正在启动...");
PluginLoader.Init(app.Services, assemblies);
PrintProxyPlugin(app.Services);
var config = app.Services.GetService<Server.Config>();
Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
Logger.Instance.Info("没什么报红的，就说明运行成功了");
Logger.Instance.Info($"UDP端口:{config.Udp}");
Logger.Instance.Info($"TCP端口:{config.Tcp}");
Logger.Instance.Warning($"当前版本：{Helper.Version}，如果客户端版本与此不一致，则可能无法连接");
Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));

app.Run();


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

void PrintProxyPlugin(IServiceProvider services)
{
    var iAccesses = services.GetServices<IAccess>().Concat(ProxyPluginLoader.plugins.Values).OrderBy(c => c.Access);
    Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
    Logger.Instance.Debug("权限值,uint 每个权限占一位，最多32个权限");
    foreach (var item in iAccesses)
    {
        Logger.Instance.Info(
            $"{Convert.ToString(item.Access, 2).PadLeft(Logger.Instance.PaddingWidth, '0')}  {item.Name}");
    }

    Logger.Instance.Warning(string.Empty.PadRight(Logger.Instance.PaddingWidth, '='));
}