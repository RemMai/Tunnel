using Common.Libs;
using Microsoft.Extensions.DependencyInjection;
using System;
using Common.Server;
using Common.Server.Servers.Iocp;
using Common.Server.Servers.RUdp;
using System.Reflection;
using Common.Libs.DataBase;
using Server.Service.Validators;
using Server.Messengers.SignIn;
using Server.Service.Messengers.SignIn;
using Common.Proxy;

namespace Server.Service
{
    public sealed class Plugin : IPlugin
    {
        public void LoadBefore(ServiceCollection services, Assembly[] assemblies)
        {
            // TODO

            services.AddTransient(typeof(IConfigDataProvider<>), typeof(ConfigDataFileProvider<>));
            services.AddSingleton<Config>();
            services.AddSingleton<ITcpServer, TcpServer>();
            services.AddSingleton<IUdpServer, UdpServer>();

            services.AddSingleton<IClientSignInCaching, ClientSignInCaching>();
            services.AddSingleton<IRelaySourceConnectionSelector, RelaySourceConnectionSelector>();


            services.AddSingleton<ISignInValidatorHandler, SignInValidatorHandler>();
            services.AddSingleton<IRelayValidator, RelayValidator>();
            services.AddSingleton<IServiceAccessValidator, Validators.ServiceAccessValidator>();


            services.AddSingleton<MessengerResolver>();
            services.AddSingleton<MessengerSender>();
            services.AddSingleton<ICryptoFactory, CryptoFactory>();
            services.AddSingleton<IAsymmetricCrypto, RsaCrypto>();
            services.AddSingleton<WheelTimer<object>>();

            services.AddSingleton<Common.Proxy.Config>();
            services.AddSingleton<IProxyMessengerSender, ProxyMessengerSender>();
            services.AddSingleton<IProxyClient, ProxyClient>();
            services.AddSingleton<IProxyServer, ProxyServer>();
            services.AddSingleton<ProxyPluginValidatorHandler>();
            foreach (Type item in ReflectionHelper.GetInterfaceSchieves(assemblies, typeof(IProxyPluginValidator)))
            {
                services.AddSingleton(item);
            }


            foreach (Type item in ReflectionHelper.GetInterfaceSchieves(assemblies, typeof(IMessenger)))
            {
                services.AddSingleton(item);
            }

            foreach (Type item in ReflectionHelper.GetInterfaceSchieves(assemblies, typeof(ISignInValidator)))
            {
                services.AddSingleton(item);
            }
        }

        public void Init(IServiceProvider services, Assembly[] assemblies)
        {
            var config = services.GetService<Config>();

            var server = services.GetService<ITcpServer>();
            server.SetBufferSize((1 << (byte)config.TcpBufferSize) * 1024);
            try
            {
                server.Start(config.Tcp);
                Logger.Instance.Info("TCP服务已开启");
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
            }

            var udpServer = services.GetService<IUdpServer>();
            try
            {
                udpServer.Start(config.Udp, timeout: config.TimeoutDelay);
                Logger.Instance.Info("UDP服务已开启");
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
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