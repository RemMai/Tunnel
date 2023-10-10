using System;
using System.Buffers.Binary;
using System.Linq;
using System.Threading.Tasks;
using Client.Messengers.Clients;
using client.service.ui.api.Enums;
using client.service.ui.api.Interfaces;
using client.service.ui.api.Models;
using Client.Service.Vea.Models;
using Common.Libs;
using Common.Libs.AutoInject.Attributes;
using Common.Libs.Extends;
using Common.Proxy;
using Common.proxy.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Service.Vea.Implementations
{
    /// <summary>
    /// 组网前端接口
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class VeaClientService : IClientService
    {
        private readonly Models.Config config;
        private readonly VeaTransfer veaTransfer;
        private readonly VeaMessengerSender veaMessengerSender;
        private readonly IClientInfoCaching clientInfoCaching;
        private readonly IProxyServer proxyServer;

        public VeaClientService(Models.Config config, VeaTransfer VeaTransfer, VeaMessengerSender veaMessengerSender, IClientInfoCaching clientInfoCaching, IProxyServer proxyServer)
        {
            this.config = config;
            this.veaTransfer = VeaTransfer;
            this.veaMessengerSender = veaMessengerSender;
            this.clientInfoCaching = clientInfoCaching;
            this.proxyServer = proxyServer;
        }

        public async Task<bool> Run(ClientServiceParamsInfo arg)
        {
            try
            {
                return await veaTransfer.Run();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
                arg.SetCode(ClientServiceResponseCodes.Error, ex.Message);
            }
            return false;
        }

        public async Task<EnumProxyCommandStatusMsg> Test(ClientServiceParamsInfo arg)
        {
            TestTargetInfo fmodel = arg.Content.DeJson<TestTargetInfo>();
            return await veaTransfer.Test(fmodel.Host, fmodel.Port);
        }

        /// <summary>
        /// 去获取在线设备
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task<bool> Online(ClientServiceParamsInfo arg)
        {
            ulong id = ulong.Parse(arg.Content);
            if (clientInfoCaching.Get(id, out ClientInfo client))
            {
                return await veaMessengerSender.GetOnLine(client.Connection);
            }
            return false;
        }
        /// <summary>
        /// 在线设备
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public VeaLanIPAddressOnLine Onlines(ClientServiceParamsInfo arg)
        {
            ulong id = ulong.Parse(arg.Content);
            if (veaTransfer.Onlines.TryGetValue(id, out VeaLanIPAddressOnLine online))
            {
                return online;
            }
            return new VeaLanIPAddressOnLine();
        }

        /// <summary>
        /// 各个客户端
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public object List(ClientServiceParamsInfo arg)
        {
            EnumProxyCommandStatusMsg lastError = EnumProxyCommandStatusMsg.Success;
            proxyServer.LastError((ushort)config.ListenPort, out lastError);

            return veaTransfer.IPList.ToDictionary(c => c.Value.Client.ConnectionId, d => new
            {
                IP = string.Join(".", BinaryPrimitives.ReverseEndianness(d.Value.IP).ToBytes()),
                LanIPs = d.Value.LanIPs.Select(c => new { IPAddress = string.Join(".", BinaryPrimitives.ReverseEndianness(c.IPAddress).ToBytes()), Mask = c.MaskLength }),
                d.Value.NetWork,
                Mask = d.Value.MaskLength,
                LastError = lastError
            });
        }

        /// <summary>
        /// 重装其网卡
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task<bool> Reset(ClientServiceParamsInfo arg)
        {
            ulong id = ulong.Parse(arg.Content);
            if (clientInfoCaching.Get(id, out ClientInfo client))
            {
                await veaMessengerSender.Reset(client.Connection, id);
            }
            return true;
        }

        /// <summary>
        /// 刷新ip列表
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public bool Update(ClientServiceParamsInfo arg)
        {
            veaTransfer.UpdateIp();
            return true;
        }
    }
}
