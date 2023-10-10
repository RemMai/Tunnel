using Client.Messengers.Signin;
using Common.Extensions.AutoInject.Attributes;
using Common.Server;
using Common.Server.Attributes;
using Common.Server.Interfaces;
using Common.Server.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Realize.Messengers.Clients
{
    /// <summary>
    /// 服务器发来的客户端列表
    /// </summary>
    [MessengerIdRange((ushort)ClientsMessengerIds.Min, (ushort)ClientsMessengerIds.Max)]
    [AutoInject(ServiceLifetime.Singleton, typeof(IMessenger))]
    public sealed class ClientsMessenger : IMessenger
    {
        private readonly ClientsMessengerSender clientsMessengerSender;
        private readonly ISignInTransfer signInTransfer;

        public ClientsMessenger(ClientsMessengerSender clientsMessengerSender, ISignInTransfer signInTransfer)
        {
            this.clientsMessengerSender = clientsMessengerSender;
            this.signInTransfer = signInTransfer;
        }

        /// <summary>
        /// 通知信息
        /// </summary>
        /// <param name="connection"></param>
        [MessengerId((ushort)ClientsMessengerIds.Notify)]
        public void Notify(IConnection connection)
        {
            ClientsInfo res = new ClientsInfo();
            res.DeBytes(connection.ReceiveRequestWrap.Payload);
            clientsMessengerSender.OnServerClientsData?.Invoke(res);
        }

        /// <summary>
        /// 退出信息
        /// </summary>
        /// <param name="connection"></param>
        [MessengerId((ushort)ClientsMessengerIds.Exit)]
        public void Exit(IConnection connection)
        {
            signInTransfer.Exit();
        }
    }
}