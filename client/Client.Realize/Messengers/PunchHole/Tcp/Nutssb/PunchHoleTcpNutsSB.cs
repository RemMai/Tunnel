using Common.Server;
using Common.Server.Model;
using System.Threading.Tasks;
using Client.Messengers.PunchHole;
using Client.Messengers.PunchHole.Tcp;
using Common.Libs.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Realize.Messengers.PunchHole.Tcp.NutsSB
{
    /// <summary>
    /// tcp打洞消息
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class PunchHoleTcpNutsSB : IPunchHole
    {
        private readonly IPunchHoleTcp punchHoleTcp;
        public PunchHoleTcpNutsSB(IPunchHoleTcp punchHoleTcp)
        {
            this.punchHoleTcp = punchHoleTcp;
        }

        public PunchHoleTypes Type => PunchHoleTypes.TCP_NUTSSB;

        public async Task Execute(IConnection connection, PunchHoleRequestInfo info)
        {
            await punchHoleTcp.InputData(new PunchHoleStepModel
            {
                Connection = connection,
                RawData = info
            });
        }
    }
}
