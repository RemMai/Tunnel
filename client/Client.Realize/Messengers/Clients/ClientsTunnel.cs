using Common.Libs;
using Common.Server.Servers.Tcp;
using Common.Server.Servers.Udp;
using System;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using Client.Messengers.Clients;
using Client.Messengers.Signin;
using Common.Extensions.AutoInject.Attributes;
using Common.Libs.Extends;
using Common.Server.Interfaces;
using Common.Server.Models;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Client.Realize.Messengers.Clients
{
    /// <summary>
    /// 客户端打洞通道
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton,typeof(IClientsTunnel))]
    public sealed class ClientsTunnel : IClientsTunnel
    {
        private readonly SignInStateInfo signInState;
        private readonly IClientInfoCaching clientInfoCaching;
        private readonly Config config;
        private readonly IUdpServer udpServer;
        private readonly ITcpServer tcpServer;
        private readonly ClientsMessengerSender clientsMessengerSender;

        /// <summary>
        /// 断开
        /// </summary>
        public Action<IConnection, IConnection> OnDisConnect { get; set; } = (IConnection connection, IConnection connection1) => { };

        public ClientsTunnel(ClientsMessengerSender clientsMessengerSender, IClientInfoCaching clientInfoCaching, SignInStateInfo signInState, Config config, IUdpServer udpServer, ITcpServer tcpServer
        )
        {
            this.clientsMessengerSender = clientsMessengerSender;
            this.signInState = signInState;
            this.clientInfoCaching = clientInfoCaching;
            this.config = config;
            this.udpServer = udpServer;
            this.tcpServer = tcpServer;
        }

        /// <summary>
        /// 新绑定
        /// </summary>
        /// <param name="serverType"></param>
        /// <param name="selfId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public async Task<ushort> NewBind(ServerType serverType, ulong selfId, ulong targetId)
        {
            IPAddress serverAddress = NetworkHelper.GetDomainIp(config.Server.Ip);
            while (true)
            {
                try
                {
                    ushort localPort = NetworkHelper.GetRandomPort(new System.Collections.Generic.List<ushort> { signInState.LocalInfo.Port });
                    _ = serverType switch
                    {
                        ServerType.TCP => await NewBindTcp(localPort, serverAddress, selfId, targetId),
                        ServerType.UDP => await NewBindUdp(localPort, serverAddress, selfId, targetId),
                        _ => 0,
                    };
                    return localPort;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message + "\r\n" + ex.StackTrace);
                }
            }
        }
        /// <summary>
        /// 新绑定一个udp
        /// </summary>
        /// <param name="localport"></param>
        /// <param name="serverAddress"></param>
        /// <param name="selfId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        private async Task<ushort> NewBindUdp(ushort localport, IPAddress serverAddress, ulong selfId, ulong targetId)
        {
            IConnection connection = null;
            UdpServer tempUdpServer = new UdpServer
            {
                OnPacket = udpServer.InputData
            };

            tempUdpServer.OnDisconnect += (IConnection _connection) =>
            {
                Task.Run(() =>
                {
                    //不是跟服务器的连接对象，那就是打洞的
                    if (ReferenceEquals(connection, _connection) == false)
                    {
                        OnDisConnect(_connection, signInState.Connection);
                        clientInfoCaching.RemoveUdpserver(targetId);
                        tempUdpServer.Disponse();
                    }
                });
            };
            tempUdpServer.Start(localport, config.Client.TimeoutDelay);
            tempUdpServer.SetSpeedLimit(config.Client.UdpUploadSpeedLimit);

            var tcs = new TaskCompletionSource<ushort>();
            tempUdpServer.OnMessage += (remoteEndpoint, data) =>
            {
                tcs.SetResult(data.ToUInt16());
                tempUdpServer.OnMessage = null;
            };
            tempUdpServer.SendUnconnectedMessage(new TunnelRegisterInfo
            {
                LocalPort = localport,
                SelfId = selfId,
                TargetId = targetId
            }.ToBytes(), new IPEndPoint(serverAddress, config.Server.UdpPort));

            ushort port = await tcs.Task.ConfigureAwait(false);
            if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
            {
                Log.Debug($"Clients new udp port {port}");
            }

            clientInfoCaching.AddTunnelPort(targetId, localport);
            clientInfoCaching.AddUdpserver(targetId, tempUdpServer);

            return port;
        }

        /// <summary>
        /// 新绑定一个tcp
        /// </summary>
        /// <param name="localport"></param>
        /// <param name="serverAddress"></param>
        /// <param name="selfId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        private async Task<ushort> NewBindTcp(ushort localport, IPAddress serverAddress, ulong selfId, ulong targetId)
        {
            TcpServer tempTcpServer = new TcpServer();
            tempTcpServer.SetBufferSize((1 << (byte)config.Client.TcpBufferSize) * 1024);
            tempTcpServer.OnConnected1 = (socket) =>
            {
                tcpServer.BindReceive(socket, (1 << (byte)config.Client.TcpBufferSize) * 1024);
                tempTcpServer.Disponse();
            };
            tempTcpServer.Start1(localport);

            IPEndPoint bindEndpoint = new IPEndPoint(config.Client.BindIp, localport);
            Socket tcpSocket = new(bindEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            tcpSocket.IPv6Only(bindEndpoint.AddressFamily, false);

            tcpSocket.KeepAlive(time: config.Client.TimeoutDelay / 1000 / 5);
            tcpSocket.ReuseBind(bindEndpoint);
            tcpSocket.Connect(new IPEndPoint(serverAddress, config.Server.TcpPort));

            IPAddress localAddress = (tcpSocket.LocalEndPoint as IPEndPoint).Address;

            IConnection connection = tcpServer.BindReceive(tcpSocket, (byte)config.Client.TcpBufferSize * 1024);

            ushort port = await clientsMessengerSender.AddTunnel(connection, selfId, targetId, localport);
            if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
            {
                Log.Debug($"Clients new tcp port {port}");
            }
            clientInfoCaching.AddTunnelPort(targetId, localport);
            //clientInfoCaching.AddUdpserver(targetId, tempTcpServer);

            return port;
        }
    }
}
