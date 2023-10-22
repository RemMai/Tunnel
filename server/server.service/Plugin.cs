using Common.Libs;
using Microsoft.Extensions.DependencyInjection;
using System;
using Common.Server;
using Common.Server.Servers.Tcp;
using Common.Server.Servers.Udp;
using System.Reflection;
using Common.Libs.DataBase;
using Server.Service.Validators;
using Server.Messengers.SignIn;
using Server.Service.Messengers.SignIn;
using Common.Proxy.Implementations;
using Common.Proxy.Implementations.MessengerSenders;
using Common.Proxy.Interfaces;
using Common.Server.Implementations;
using Common.Server.Interfaces;
using Serilog;

namespace Server.Service
{
    public sealed class Plugin : IPlugin
    {

        public void Init(IServiceProvider services, Assembly[] assemblies)
        {
            var config = services.GetService<Config>();

            var server = services.GetService<ITcpServer>();
            server.SetBufferSize((1 << (byte)config.TcpBufferSize) * 1024);
            try
            {
                server.Start(config.Tcp);
                Log.Information("TCP服务已开启");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }

            var udpServer = services.GetService<IUdpServer>();
            try
            {
                udpServer.Start(config.Udp, timeout: config.TimeoutDelay);
                Log.Information("UDP服务已开启");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }

            MessengerResolver messengerResolver = services.GetService<MessengerResolver>();
            messengerResolver.LoadMessenger(assemblies);

            ISignInValidatorHandler signInMiddlewareHandler = services.GetService<ISignInValidatorHandler>();
            
            signInMiddlewareHandler.LoadValidator();

            ProxyPluginValidatorHandler proxyPluginValidatorHandler =
                services.GetService<ProxyPluginValidatorHandler>();
            proxyPluginValidatorHandler.LoadValidator(assemblies);
        }
    }
}