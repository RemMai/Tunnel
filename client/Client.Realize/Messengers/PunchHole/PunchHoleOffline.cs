using Common.Server;
using Common.Server.Model;
using System.Threading.Tasks;
using Client.Messengers.Clients;
using Client.Messengers.PunchHole;
using Common.Libs.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Realize.Messengers.PunchHole
{
    /// <summary>
    /// 掉线
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class PunchHoleOffline : IPunchHole
    {
        private readonly IClientInfoCaching clientInfoCaching;
        public PunchHoleOffline(IClientInfoCaching clientInfoCaching)
        {

            this.clientInfoCaching = clientInfoCaching;
        }

        public PunchHoleTypes Type => PunchHoleTypes.OFFLINE;

        public async Task Execute(IConnection connection, PunchHoleRequestInfo info)
        {
            clientInfoCaching.Offline(info.FromId);
            await Task.CompletedTask;
        }
    }
}
