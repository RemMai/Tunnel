using Common.ForWard;
using Common.Server;
using Common.Server.Model;
using Server.Service.ForWard.model;
using System.Threading.Tasks;
using Common.Libs.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Service.ForWard.Server
{
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class ServerForwardMessengerSender
    {
        private readonly MessengerSender messengerSender;

        public ServerForwardMessengerSender(MessengerSender messengerSender)
        {
            this.messengerSender = messengerSender;
        }

        public async Task<MessageResponeInfo> GetDomains(IConnection Connection)
        {
            return await messengerSender.SendReply(new MessageRequestWrap
            {
                MessengerId = (ushort)ForwardMessengerIds.Domains,
                Connection = Connection
            }).ConfigureAwait(false);
        }

        public async Task<MessageResponeInfo> GetPorts(IConnection Connection)
        {
            return await messengerSender.SendReply(new MessageRequestWrap
            {
                MessengerId = (ushort)ForwardMessengerIds.Ports,
                Connection = Connection
            }).ConfigureAwait(false);
        }

        public async Task<MessageResponeInfo> SignOut(IConnection Connection, ForwardSignOutInfo data)
        {
            return await messengerSender.SendReply(new MessageRequestWrap
            {
                MessengerId = (ushort)ForwardMessengerIds.SignOut,
                Connection = Connection,
                Payload = data.ToBytes()
            }).ConfigureAwait(false);
        }

        public async Task<MessageResponeInfo> SignIn(IConnection Connection, ForwardSignInInfo data)
        {
            return await messengerSender.SendReply(new MessageRequestWrap
            {
                MessengerId = (ushort)ForwardMessengerIds.SignIn,
                Connection = Connection,
                Payload = data.ToBytes(),
            }).ConfigureAwait(false);
        }
    }
}