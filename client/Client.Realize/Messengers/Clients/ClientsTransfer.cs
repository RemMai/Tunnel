﻿using Common.Libs;
using Common.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client.Messengers.Clients;
using Client.Messengers.PunchHole;
using Client.Messengers.PunchHole.Tcp;
using Client.Messengers.PunchHole.udp;
using Client.Messengers.Relay;
using Client.Messengers.Signin;
using Client.Realize.Messengers.Crypto;
using Client.Realize.Messengers.Heart;
using Client.Realize.Messengers.PunchHole;
using Client.Realize.Messengers.Relay;
using Common.Extensions.AutoInject.Attributes;
using Common.Libs.Extends;
using Common.Server.Interfaces;
using Common.Server.Models;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Client.Realize.Messengers.Clients
{
    /// <summary>
    /// 客户端操作类
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton, typeof(IClientsTransfer))]
    public sealed class ClientsTransfer : IClientsTransfer
    {
        private BoolSpace firstClients = new BoolSpace(true);

        private readonly ClientsMessengerSender clientsMessengerSender;
        private readonly IPunchHoleUdp punchHoleUdp;
        private readonly IPunchHoleTcp punchHoleTcp;
        private readonly SignInStateInfo signInState;
        private readonly IClientInfoCaching clientInfoCaching;
        private readonly PunchHoleMessengerSender punchHoleMessengerSender;
        private readonly Config config;
        private readonly HeartMessengerSender heartMessengerSender;
        private readonly RelayMessengerSender relayMessengerSender;
        private readonly IClientConnectsCaching connecRouteCaching;
        private readonly PunchHoleDirectionConfig punchHoleDirectionConfig;
        private readonly CryptoSwap cryptoSwap;

        private object lockObject = new();


        public ClientsTransfer(ClientsMessengerSender clientsMessengerSender,
            IPunchHoleUdp punchHoleUdp, IPunchHoleTcp punchHoleTcp, IClientInfoCaching clientInfoCaching,
            SignInStateInfo signInState, PunchHoleMessengerSender punchHoleMessengerSender, Config config,
            ITcpServer tcpServer, HeartMessengerSender heartMessengerSender,
            RelayMessengerSender relayMessengerSender, IClientsTunnel clientsTunnel,
            IClientConnectsCaching connecRouteCaching,
            PunchHoleDirectionConfig punchHoleDirectionConfig, CryptoSwap cryptoSwap
        )
        {
            this.clientsMessengerSender = clientsMessengerSender;
            this.punchHoleUdp = punchHoleUdp;
            this.punchHoleTcp = punchHoleTcp;
            this.signInState = signInState;
            this.clientInfoCaching = clientInfoCaching;
            this.config = config;
            this.heartMessengerSender = heartMessengerSender;
            this.relayMessengerSender = relayMessengerSender;
            this.connecRouteCaching = connecRouteCaching;
            this.punchHoleMessengerSender = punchHoleMessengerSender;
            this.punchHoleDirectionConfig = punchHoleDirectionConfig;
            this.cryptoSwap = cryptoSwap;

            punchHoleUdp.OnStepHandler += OnPunchHoleStep;
            punchHoleTcp.OnStepHandler += OnPunchHoleStep;

            //掉线的
            tcpServer.OnDisconnect += (connection) => OnDisconnect(connection, signInState.Connection);
            clientsTunnel.OnDisConnect = OnDisconnect;
            clientInfoCaching.OnOffline += OnOffline;
            clientInfoCaching.OnOfflineAfter += OnOfflineAfter;

            //中继连线
            relayMessengerSender.OnRelay += (param) => { _ = Relay(param.Connection, param.RelayIds, false); };

            //有人要求反向链接
            punchHoleMessengerSender.OnReverse += OnReverse;

            signInState.OnBind += OnBind;

            //收到来自服务器的 在线客户端 数据
            clientsMessengerSender.OnServerClientsData += OnServerSendClients;
        }

        private void OnPunchHoleStep(object sender, PunchHoleStepModel arg)
        {
            byte step1 = 0, step2fail = 0, step3 = 0, step4 = 0;
            if (arg.Connection.ServerType == ServerType.TCP)
            {
                if (config.Client.UseTcp == false) return;
                step1 = (byte)PunchHoleTcpNutssBSteps.STEP_1;
                step2fail = (byte)PunchHoleTcpNutssBSteps.STEP_2_FAIL;
                step3 = (byte)PunchHoleTcpNutssBSteps.STEP_3;
                step4 = (byte)PunchHoleTcpNutssBSteps.STEP_4;
            }
            else if (arg.Connection.ServerType == ServerType.UDP)
            {
                if (config.Client.UseUdp == false) return;
                step1 = (byte)PunchHoleUdpSteps.STEP_1;
                step2fail = (byte)PunchHoleUdpSteps.STEP_2_Fail;
                step3 = (byte)PunchHoleUdpSteps.STEP_3;
                step4 = (byte)PunchHoleUdpSteps.STEP_4;
            }

            //3 4步骤是已成功连接，设置上线状态
            if (arg.RawData.PunchStep == step3 || arg.RawData.PunchStep == step4)
            {
                if (clientInfoCaching.Get(arg.RawData.FromId, out ClientInfo client))
                {
                    //3是被动方 4是主动方
                    ClientOnlineTypes onlineType = arg.RawData.PunchStep == step3
                        ? ClientOnlineTypes.Passive
                        : ClientOnlineTypes.Active;
                    clientInfoCaching.Online(arg.RawData.FromId, arg.Connection, ClientConnectTypes.P2P, onlineType);
                    _ = clientsMessengerSender.RemoveTunnel(signInState.Connection, arg.RawData.FromId);
                    //主动方 记录一下，是我这边主动打洞成功的，下次由我这边开始尝试
                    if (arg.RawData.PunchStep == step4)
                    {
                        punchHoleDirectionConfig.Add(client.Name);
                    }
                }
            }
            //被动方收到打洞消息，设置状态位连接中
            else if (arg.RawData.PunchStep == step1)
            {
                clientInfoCaching.SetConnecting(arg.RawData.FromId, true);
            }
            //打洞失败
            else if (arg.RawData.PunchStep == step2fail)
            {
                clientInfoCaching.Offline(arg.RawData.FromId, ClientOfflineTypes.Manual);
            }
        }

        private void OnOffline(ClientInfo client)
        {
            if (clientInfoCaching.Get(client.ConnectionId, out _))
            {
                signInState.LocalInfo.IsConnecting = true;
                client.SetConnecting(true);
                punchHoleMessengerSender.SendOffline(client.ConnectionId).Wait();
                client.SetConnecting(false);
                signInState.LocalInfo.IsConnecting = false;
            }
        }

        private void OnOfflineAfter(ClientInfo client)
        {
            if (signInState.Connected == false)
            {
                clientInfoCaching.Remove(client.ConnectionId);
                return;
            }

            if (clientInfoCaching.Get(client.ConnectionId, out _))
            {
                //主动连接的，未知掉线信息的，去尝试重连一下
                if (config.Client.UseReConnect && client.OnlineType == ClientOnlineTypes.Active &&
                    client.OfflineType == ClientOfflineTypes.Disconnect)
                {
                    Log.Warning($"尝试对【{client.Name}】重新打洞");
                    ConnectClient(client);
                }
            }
        }

        private void OnDisconnect(IConnection connection, IConnection regConnection)
        {
            if (IConnection.Equals2(connection, regConnection) || connection?.Address.Port == config.Server.TcpPort)
            {
                return;
            }


            Log.Warning($"{connection.ServerType} Client 断开~~~~${connection.Address}");
            if (clientInfoCaching.Get(connection.ConnectId, out ClientInfo client))
            {
                if (ReferenceEquals(connection, client.Connection))
                {
                    clientInfoCaching.Offline(connection.ConnectId, ClientOfflineTypes.Disconnect);
                }
            }
        }


        public async Task SendOffline(ulong toid)
        {
            await punchHoleMessengerSender.SendOffline(toid);
        }

        /// <summary>
        /// 连它
        /// </summary>
        /// <param name="id"></param>
        public void ConnectClient(ulong id)
        {
            if (clientInfoCaching.Get(id, out ClientInfo client))
            {
                ConnectClient(client);
            }
        }

        /// <summary>
        /// 连它
        /// </summary>
        /// <param name="client"></param>
        public void ConnectClient(ClientInfo client)
        {
            if (client.ConnectionId == signInState.ConnectId)
            {
                Log.Error($"canot connect you self");
                return;
            }

            if (signInState.LocalInfo.IsConnecting)
            {
                return;
            }

            Task.Run(async () =>
            {
                /* 两边先试TCP，没成功，再两边都试试UDP
                 *
                 * TryReverseTcpBit     0b00000010
                 * TryReverseUdpBit     0b00000001
                 * TryReverseTcpUdpBit  0b00000011
                 * TryReverseDefault    0b00000000
                 * 1、A 00 00 -> 00 10 -> B
                 *      A 试了tcp没成功，让B试试
                 * 2、B 00 10 -> 10 00 -> 10 10 -> A
                 *      B 收到反连接请求，先交换保存状态，试试TCP，没成功，继续给A试试，
                 * 3、A 10 10 -> 10 10 -> 10 11 -> B
                 *      A 收到反连接请求，先交换保存状态，前面试过TCP了，试试UDP，没成功，继续让B试试
                 * 4、B 10 11 -> 11 10 -> 11 11
                 *      B 收到反链接请求，先交换保存状态，前面试过TCP了，试试UDP，成就成，没成就全部结束
                 */
                EnumConnectResult result = EnumConnectResult.Fail;
                //tcp没试过，先试试tcp
                if ((client.TryReverseValue & ClientInfo.TryReverseTcpBit) == ClientInfo.TryReverseDefault)
                {
                    client.TryReverseValue |= ClientInfo.TryReverseTcpBit;
                    result = await ConnectTcp(client).ConfigureAwait(false);
                }
                //udp没试过，试试udp
                else if ((client.TryReverseValue & ClientInfo.TryReverseUdpBit) == ClientInfo.TryReverseDefault)
                {
                    client.TryReverseValue |= ClientInfo.TryReverseUdpBit;
                    result = await ConnectUdp(client).ConfigureAwait(false);
                }

                //没成功
                if (result == EnumConnectResult.Fail)
                {
                    //对面有没试过的，让对面试试
                    if ((client.TryReverseValue >> 2 & ClientInfo.TryReverseTcpUdpBit) !=
                        ClientInfo.TryReverseTcpUdpBit)
                    {
                        ConnectReverse(client);
                    }
                    //都试过了， 都不行，中继 
                    else if (config.Client.AutoRelay &&
                             ((EnumClientAccess.UseAutoRelay & (EnumClientAccess)client.ClientAccess) ==
                              EnumClientAccess.UseAutoRelay))
                    {
                        _ = Relay(client, true);
                    }
                }
                else if (result == EnumConnectResult.BreakOff)
                {
                    //Log.Error($"打洞被跳过，最大的可能是，【{Client.Name}】的打洞失败消息比本消息“反向连接”来的晚，可以重新手动尝试");
                }

                client.TryReverseValue = ClientInfo.TryReverseDefault;
            });
        }

        //收到反连接请求
        private void OnReverse(PunchHoleRequestInfo info)
        {
            if (clientInfoCaching.Get(info.FromId, out ClientInfo client))
            {
                PunchHoleReverseInfo model = new PunchHoleReverseInfo();
                model.DeBytes(info.Data);
                //交换状态 , 11 01 -> 01 11
                client.TryReverseValue =
                    (byte)(((model.Value & ClientInfo.TryReverseTcpUdpBit) << 2) | (model.Value >> 2));
                ConnectClient(client);
            }
            else
            {
                Log.Error($"收到反向连接，但是这个客户端不存在，可能是因为对面比此客户端更早收到客户端列表数据");
            }
        }

        /// <summary>
        /// 连我
        /// </summary>
        /// <param name="id"></param>
        public void ConnectReverse(ulong id)
        {
            if (clientInfoCaching.Get(id, out ClientInfo client))
            {
                ConnectReverse(client);
            }
        }

        /// <summary>
        /// 连我
        /// </summary>
        /// <param name="client"></param>
        public void ConnectReverse(ClientInfo client)
        {
            punchHoleMessengerSender.SendReverse(client).ConfigureAwait(false);
        }

        /// <summary>
        /// 重启
        /// </summary>
        /// <param name="id"></param>
        public void Reset(ulong id)
        {
            punchHoleMessengerSender.SendReset(id).ConfigureAwait(false);
        }

        /// <summary>
        /// 停止连接
        /// </summary>
        /// <param name="id"></param>
        public void ConnectStop(ulong id)
        {
        }

        private async Task<EnumConnectResult> ConnectUdp(ClientInfo client)
        {
            if (client.Connecting)
            {
                return EnumConnectResult.BreakOff;
            }

            if ((config.Client.UseUdp & ((EnumClientAccess.UseUdp & (EnumClientAccess)client.ClientAccess) ==
                                         EnumClientAccess.UseUdp)) == false)
            {
                return EnumConnectResult.Fail;
            }

            client.SetConnecting(true);

            for (int i = 0; i < 2; i++)
            {
                ConnectResultModel result = await punchHoleUdp.Send(new ConnectParams
                {
                    Id = client.ConnectionId,
                    NewTunnel = 1,
                    LocalPort = 0
                }).ConfigureAwait(false);
                if (result.State)
                {
                    return EnumConnectResult.Success;
                }

                var messageTemplate = (result.Result as ConnectFailModel)?.Msg;
                if (messageTemplate != null)
                {
                    Log.Error(messageTemplate);
                }
            }

            client.SetConnecting(false);
            return EnumConnectResult.Fail;
        }

        private async Task<EnumConnectResult> ConnectTcp(ClientInfo client)
        {
            if (client.Connecting)
            {
                return EnumConnectResult.BreakOff;
            }

            if ((config.Client.UseTcp & ((EnumClientAccess.UseTcp & (EnumClientAccess)client.ClientAccess) ==
                                         EnumClientAccess.UseTcp)) == false)
            {
                return EnumConnectResult.Fail;
            }

            client.SetConnecting(true);

            byte[] tunnelNames = new byte[] { 1, 1 };
            for (int i = 0; i < tunnelNames.Length; i++)
            {
                clientInfoCaching.AddTunnelPort(client.ConnectionId, signInState.LocalInfo.Port);
                ConnectResultModel result = await punchHoleTcp.Send(new ConnectParams
                {
                    Id = client.ConnectionId,
                    NewTunnel = tunnelNames[i],
                    LocalPort = signInState.LocalInfo.Port
                }).ConfigureAwait(false);
                if (result.State)
                {
                    return EnumConnectResult.Success;
                }

                string messageTemplate = (result.Result as ConnectFailModel)?.Msg;
                if (messageTemplate != null)
                {
                    Log.Error(messageTemplate);
                }
            }

            client.SetConnecting(false);
            return EnumConnectResult.Fail;
        }

        /// <summary>
        /// ping
        /// </summary>
        /// <returns></returns>
        public async Task Ping()
        {
            foreach (var item in clientInfoCaching.All())
            {
                try
                {
                    var start = DateTime.Now;
                    var res = await heartMessengerSender.Alive(item.Connection).ConfigureAwait(false);
                    if (res)
                    {
                        item.Connection.RoundTripTime = (ushort)(DateTime.Now - start).TotalMilliseconds;
                    }
                    else
                    {
                        item.Connection.RoundTripTime = -1;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public async Task<bool> Test(ulong id, Memory<byte> data)
        {
            if (clientInfoCaching.Get(id, out ClientInfo client))
            {
                return await heartMessengerSender.Test(client.Connection, data);
            }

            return false;
        }

        private void OnBind(bool state)
        {
            if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
            {
                Log.Debug($"Clients 登出清理");
            }

            firstClients.Reset();
            clientInfoCaching.Clear();
            if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
            {
                Log.Debug($"Clients 登出清理结束");
            }
        }

        private void OnServerSendClients(ClientsInfo clients)
        {
            try
            {
                if (signInState.Connection == null || clients.Clients == null)
                {
                    return;
                }

                lock (lockObject)
                {
                    IEnumerable<ulong> remoteIds = clients.Clients.Select(c => c.ConnectionId);
                    //下线了的
                    IEnumerable<ulong> offlines = clientInfoCaching.All().Where(c => c.Connected == false)
                        .Select(c => c.ConnectionId).Except(remoteIds).Where(c => c != signInState.ConnectId);
                    foreach (ulong offid in offlines)
                    {
                        clientInfoCaching.Remove(offid);
                    }

                    //新上线的或者更新的
                    foreach (ClientsClientInfo item in clients.Clients.Where(c =>
                                 c.ConnectionId != signInState.ConnectId))
                    {
                        EnumClientAccess enumClientAccess = (EnumClientAccess)item.ClientAccess;
                        bool has = clientInfoCaching.Get(item.ConnectionId, out ClientInfo client);
                        if (has == false)
                        {
                            client = new ClientInfo();
                            client.ConnectionId = item.ConnectionId;
                        }

                        client.ClientAccess = item.ClientAccess;
                        client.Name = item.Name;
                        clientInfoCaching.Add(client);

                        if (has == false && firstClients.IsDefault && client.Connected == false)
                        {
                            if (config.Client.UsePunchHole &&
                                ((EnumClientAccess.UsePunchHole & (EnumClientAccess)client.ClientAccess) ==
                                 EnumClientAccess.UsePunchHole))
                            {
                                //主动打洞成功过
                                if (punchHoleDirectionConfig.Contains(client.Name))
                                {
                                    ConnectClient(client);
                                }
                                //否则让对方主动
                                else
                                {
                                    ConnectReverse(client);
                                }
                            }
                            else if (config.Client.AutoRelay &&
                                     ((EnumClientAccess.UseAutoRelay & (EnumClientAccess)client.ClientAccess) ==
                                      EnumClientAccess.UseAutoRelay))
                            {
                                _ = Relay(client, true);
                            }
                        }
                    }

                    firstClients.Reverse();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 获取各个客户端的连接状态
        /// </summary>
        /// <returns></returns>
        public async Task<ConcurrentDictionary<ulong, ulong[]>> Connects()
        {
            await relayMessengerSender.AskConnects();
            return connecRouteCaching.Connects;
        }

        /// <summary>
        /// 各个线路的延迟
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public async Task<int[]> Delay(ulong[][] paths)
        {
            int[] data = new int[paths.Length];
            DateTime current;
            for (int i = 0; i < paths.Length; i++)
            {
                ulong[] item = paths[i];

                data[i] = -1;

                IConnection connection = null;
                if (item[1] == 0)
                {
                    connection = signInState.Connection;
                }
                else
                {
                    if (clientInfoCaching.Get(item[1], out ClientInfo client))
                    {
                        connection = client.Connection;
                    }
                }

                if (connection == null || connection.Connected == false)
                {
                    continue;
                }

                current = DateTime.Now;
                bool res = await relayMessengerSender.Delay(item, connection);
                if (res)
                {
                    data[i] = (int)(DateTime.Now - current).TotalMilliseconds;
                }
            }

            return data;
        }

        private async Task Relay(ClientInfo client, bool notify = false)
        {
            if ((signInState.RemoteInfo.Access & 1) != 1)
            {
                Log.Warning($"server Relay not available");
                return;
            }

            IConnection connection = signInState.Connection;
            if (client.Connected == true)
            {
                return;
            }

            await Relay(connection, new ulong[] { signInState.ConnectId, 0, client.ConnectionId }, notify);
        }

        /// <summary>
        /// 中继
        /// </summary>
        /// <param name="sourceConnection"></param>
        /// <param name="relayids"></param>
        /// <param name="notify"></param>
        /// <returns></returns>
        public async Task Relay(IConnection sourceConnection, Memory<ulong> relayids, bool notify = false)
        {
            if (relayids.Length < 3)
            {
                Log.Error($"relayids length least 3");
                return;
            }

            ;
            if (sourceConnection == null || sourceConnection.Connected == false)
            {
                Log.Error($"sourceConnection is null");
                return;
            }

            IConnection connection = sourceConnection.Clone();
            connection.Relay = true;
            connection.RelayId = relayids;

            if (notify)
            {
                bool relayResult = await relayMessengerSender.Relay(relayids, connection);
                if (relayResult == false)
                {
                    Log.Error($"Relay fail");
                    return;
                }

                if (config.Client.Encode)
                {
                    ICrypto crypto = await cryptoSwap.Swap(connection, config.Client.EncodePassword);
                    if (crypto == null)
                    {
                        Log.Error("交换密钥失败，如果客户端设置了密钥，则目标端必须设置相同的密钥，如果目标端未设置密钥，则客户端必须留空");
                        return;
                    }

                    connection.EncodeEnable(crypto);
                }
            }

            ClientConnectTypes relayType =
                relayids.Span[1] == 0 ? ClientConnectTypes.RelayServer : ClientConnectTypes.RelayNode;
            ClientOnlineTypes onlineType = notify == false ? ClientOnlineTypes.Passive : ClientOnlineTypes.Active;
            clientInfoCaching.Online(relayids.Span[^1], connection, relayType, onlineType);
        }

        /// <summary>
        /// 打洞结果
        /// </summary>
        [Flags]
        enum EnumConnectResult : byte
        {
            /// <summary>
            /// 失败
            /// </summary>
            Fail = 1,

            /// <summary>
            /// 成功
            /// </summary>
            Success = 2,

            /// <summary>
            /// 跳过
            /// </summary>
            BreakOff = 4
        }
    }
}