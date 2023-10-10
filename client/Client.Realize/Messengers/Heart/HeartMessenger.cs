using Common.Libs;
using Common.Libs.AutoInject.Attributes;
using Common.Server;
using Common.Server.Attributes;
using Common.Server.Interfaces;
using Common.Server.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Realize.Messengers.Heart
{
    /// <summary>
    /// 心跳包
    /// </summary>
    [MessengerIdRange((ushort)HeartMessengerIds.Min, (ushort)HeartMessengerIds.Max)]
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class HeartMessenger : IMessenger
    {
        public HeartMessenger()
        {
        }

        /// <summary>
        /// 活着
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        [MessengerId((ushort)HeartMessengerIds.Alive)]
        public void Alive(IConnection connection)
        {
            connection.Write(Helper.TrueArray);
        }

        [MessengerId((ushort)HeartMessengerIds.Test)]
        public void Test(IConnection connection)
        {
        }
    }
}
