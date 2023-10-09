using Common.Libs;
using Common.Server;
using Common.Server.Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Libs.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Realize.Messengers.Heart
{
    /// <summary>
    /// 心跳消息发送
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class HeartMessengerSender
    {
        private readonly MessengerSender messengerSender;
        public HeartMessengerSender(MessengerSender messengerSender)
        {
            this.messengerSender = messengerSender;
        }
        /// <summary>
        /// 发送心跳消息
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task<bool> Alive(IConnection connection)
        {
            var resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = connection,
                MessengerId = (ushort)HeartMessengerIds.Alive,
                Timeout = 2000
            }, unconnectedMessage: true).ConfigureAwait(false);
            return resp.Code == MessageResponeCodes.OK && Helper.TrueArray.AsSpan().SequenceEqual(resp.Data.Span);
        }

        public async Task<bool> Test(IConnection connection, Memory<byte> data)
        {
            return await messengerSender.SendOnly(new MessageRequestWrap
            {
                Connection = connection,
                MessengerId = (ushort)HeartMessengerIds.Alive,
                Timeout = 2000,
                Payload = data
            }).ConfigureAwait(false);
        }
    }
}
