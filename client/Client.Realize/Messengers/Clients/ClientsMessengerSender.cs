using Common.Libs.Extends;
using Common.Server;
using System;
using System.Threading.Tasks;
using Common.Extensions.AutoInject.Attributes;
using Common.Server.Implementations;
using Common.Server.Interfaces;
using Common.Server.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Realize.Messengers.Clients
{
    /// <summary>
    /// 客户端消息发送
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class ClientsMessengerSender
    {
        private readonly MessengerSender messengerSender;
        public ClientsMessengerSender(MessengerSender messengerSender)
        {
            this.messengerSender = messengerSender;
        }

        /// <summary>
        /// 收到服务器的客户端列表信息
        /// </summary>
        public Action<ClientsInfo> OnServerClientsData { get; set; } = (param) => { };

        /// <summary>
        /// 添加通道
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="targetId"></param>
        /// <param name="port"></param>
        /// <param name="localPort"></param>
        /// <returns></returns>
        public async Task<ushort> AddTunnel(IConnection connection,ulong selfId, ulong targetId, ushort localPort)
        {
            var resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = connection,
                Payload = new TunnelRegisterInfo { LocalPort = localPort, SelfId = selfId, TargetId = targetId }.ToBytes(),
                MessengerId = (ushort)ClientsMessengerIds.AddTunnel,
                Timeout = 2000
            }).ConfigureAwait(false);

            if (resp.Code == MessageResponeCodes.OK)
            {
                return resp.Data.Span.ToUInt16();
            }
            return 0;
        }
        /// <summary>
        /// 删除通道
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tunnelName"></param>
        /// <returns></returns>
        public async Task RemoveTunnel(IConnection connection, ulong tunnelName)
        {
            await messengerSender.SendOnly(new MessageRequestWrap
            {
                Connection = connection,
                Payload = tunnelName.ToBytes(),
                MessengerId = (ushort)ClientsMessengerIds.RemoveTunnel,
                Timeout = 2000
            }).ConfigureAwait(false);
        }
    }
}
