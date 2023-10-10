using Microsoft.Extensions.DependencyInjection;
using Common.Server;
using System;
using System.Reflection;
using Client.Messengers.Clients;
using Client.Realize.Messengers.PunchHole;
using Common.Proxy;
using Common.Server.Implementations;

namespace Client.Realize.Messengers
{
    public sealed class Plugin : IPlugin
    {
        public void Init(IServiceProvider services, Assembly[] assemblies)
        {
            services.GetService<IClientsTransfer>();
            services.GetService<MessengerSender>();
            MessengerResolver messengerResolver = services.GetService<MessengerResolver>();

            //加载所有的消息处理器
            messengerResolver.LoadMessenger(assemblies);
            //加载所有的打洞消息处理器
            services.GetService<PunchHoleMessengerSender>().LoadPlugins(assemblies);

            ProxyPluginValidatorHandler proxyPluginValidatorHandler =
                services.GetService<ProxyPluginValidatorHandler>();
            proxyPluginValidatorHandler.LoadValidator(assemblies);
        }
    }
}