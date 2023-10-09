using Common.Libs;
using Common.Server;
using Common.Server.Model;
using System.Threading.Tasks;
using Client.Messengers.PunchHole;
using Client.Messengers.PunchHole.udp;
using Common.Libs.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Realize.Messengers.PunchHole.udp
{
    /// <summary>
    /// udp打洞消息
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class PunchHoleUdp : IPunchHole
    {
        private readonly IPunchHoleUdp punchHoleUdp;
        public PunchHoleUdp(IPunchHoleUdp punchHoleUdp)
        {
            this.punchHoleUdp = punchHoleUdp;
        }

        public PunchHoleTypes Type => PunchHoleTypes.UDP;

        public async Task Execute(IConnection connection, PunchHoleRequestInfo info)
        {
            await punchHoleUdp.InputData(new PunchHoleStepModel { Connection = connection, RawData = info });
        }
    }

}
