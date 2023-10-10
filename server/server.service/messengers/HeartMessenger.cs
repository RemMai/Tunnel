using Common.Libs;
using Common.Libs.AutoInject.Attributes;
using Common.Server;
using Common.Server.Attributes;
using Common.Server.Interfaces;
using Common.Server.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Server.Service.Messengers
{
    [MessengerIdRange((ushort)HeartMessengerIds.Min, (ushort)HeartMessengerIds.Max)]
    [AutoInject(ServiceLifetime.Singleton, typeof(IMessenger))]
    public sealed class HeartMessenger : IMessenger
    {
        public HeartMessenger()
        {
        }

        [MessengerId((ushort)HeartMessengerIds.Alive)]
        public void Alive(IConnection connection)
        {
            connection.Write(Helper.TrueArray);
        }
    }
}