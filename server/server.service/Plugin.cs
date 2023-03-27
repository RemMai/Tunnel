﻿using common.libs;
using Microsoft.Extensions.DependencyInjection;
using common.server.model;
using System;
using System.Collections.Generic;
using System.Linq;
using common.server;
using common.server.servers.iocp;
using common.server.servers.rudp;
using System.Reflection;
using common.libs.database;
using server.messengers;
using server.service.validators;
using System.Net;
using server.service.messengers;
using common.libs.extends;
using server.messengers.singnin;
using server.service.messengers.singnin;

namespace server.service
{
    public sealed class Plugin : IPlugin
    {
        public void LoadBefore(ServiceCollection services, Assembly[] assemblys)
        {
            services.AddTransient(typeof(IConfigDataProvider<>), typeof(ConfigDataFileProvider<>));
            services.AddSingleton<Config>();
            services.AddSingleton<ITcpServer, TcpServer>();
            services.AddSingleton<IUdpServer, UdpServer>();

            services.AddSingleton<IClientSignInCaching, ClientSignInCaching>();
            services.AddSingleton<IRelaySourceConnectionSelector, messengers.RelaySourceConnectionSelector>();


            services.AddSingleton<ISignInValidatorHandler, SignInValidatorHandler>();
            services.AddSingleton<IServiceAccessValidator, ServiceAccessValidator>();
            services.AddSingleton<IRelayValidator, RelayValidator>();
            services.AddSingleton<IUserStore, UserStore>();


            services.AddSingleton<MessengerResolver>();
            services.AddSingleton<MessengerSender>();
            services.AddSingleton<ICryptoFactory, CryptoFactory>();
            services.AddSingleton<IAsymmetricCrypto, RsaCrypto>();
            services.AddSingleton<WheelTimer<object>>();

            foreach (Type item in ReflectionHelper.GetInterfaceSchieves(assemblys, typeof(IMessenger)))
            {
                services.AddSingleton(item);
            }
            foreach (Type item in ReflectionHelper.GetInterfaceSchieves(assemblys, typeof(ISignInValidator)))
            {
                services.AddSingleton(item);
            }
            foreach (Type item in ReflectionHelper.GetInterfaceSchieves(assemblys, typeof(ISignInAccess)))
            {
                services.AddSingleton(item);
            }
        }
        public void LoadAfter(ServiceProvider services, Assembly[] assemblys)
        {
            var config = services.GetService<Config>();

            var server = services.GetService<ITcpServer>();
            server.SetBufferSize(config.TcpBufferSize);
            server.Start(config.Tcp);
            Logger.Instance.Info("TCP服务已开启");

            var udpServer = services.GetService<IUdpServer>();
            udpServer.Start(config.Udp, timeout: config.TimeoutDelay);
            Logger.Instance.Info("UDP服务已开启");

            MessengerResolver messenger = services.GetService<MessengerResolver>();
            foreach (Type item in ReflectionHelper.GetInterfaceSchieves(assemblys, typeof(IMessenger)).Distinct())
            {
                messenger.LoadMessenger(item, services.GetService(item));
            }


            ISignInValidatorHandler signInMiddlewareHandler = services.GetService<ISignInValidatorHandler>();
            foreach (Type item in ReflectionHelper.GetInterfaceSchieves(assemblys, typeof(ISignInValidator)).Distinct())
            {
                signInMiddlewareHandler.LoadValidator((ISignInValidator)services.GetService(item));
            }
            foreach (Type item in ReflectionHelper.GetInterfaceSchieves(assemblys, typeof(ISignInAccess)).Distinct())
            {
                signInMiddlewareHandler.LoadAccess((ISignInAccess)services.GetService(item));
            }

            Loop(services);
            Udp((UdpServer)udpServer, messenger);
        }

        private void Loop(ServiceProvider services)
        {
            IClientSignInCaching clientRegisterCache = services.GetService<IClientSignInCaching>();
            MessengerResolver messengerResolver = services.GetService<MessengerResolver>();
            MessengerSender messengerSender = services.GetService<MessengerSender>();

            clientRegisterCache.OnChanged.Sub((changeClient) =>
            {
                List<ClientsClientInfo> clients = clientRegisterCache.Get(changeClient.GroupId).Where(c => c.Connection != null && c.Connection.Connected).OrderBy(c => c.ConnectionId).Select(c => new ClientsClientInfo
                {
                    Connection = c.Connection,
                    Id = c.ConnectionId,
                    Name = c.Name,
                    Access = c.ClientAccess,
                }).ToList();

                if (clients.Any())
                {
                    byte[] bytes = new ClientsInfo
                    {
                        Clients = clients.ToArray()
                    }.ToBytes();
                    foreach (ClientsClientInfo client in clients)
                    {
                        messengerSender.SendOnly(new MessageRequestWrap
                        {
                            Connection = client.Connection,
                            Payload = bytes,
                            MessengerId = (ushort)ClientsMessengerIds.Notify
                        }).Wait();
                    }
                }
            });
        }

        ClientsMessenger clientsMessenger;
        private void Udp(UdpServer udpServer, MessengerResolver messenger)
        {
            if (messenger.GetMessenger((ushort)ClientsMessengerIds.AddTunnel, out object obj))
            {
                clientsMessenger = obj as ClientsMessenger;
            }
            udpServer.OnMessage += (IPEndPoint remoteEndpoint, Memory<byte> data) =>
            {
                try
                {
                    TunnelRegisterInfo model = new TunnelRegisterInfo();
                    model.DeBytes(data);
                    if (clientsMessenger != null)
                    {
                        clientsMessenger.AddTunnel(model, remoteEndpoint.Port);
                        udpServer.SendUnconnectedMessage(((ushort)remoteEndpoint.Port).ToBytes(), remoteEndpoint);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.DebugError(ex);
                }
            };
        }
    }
}
