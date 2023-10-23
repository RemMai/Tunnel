using Common.Server;
using System.Threading.Tasks;
using Client.Messengers.PunchHole;
using Client.Messengers.Signin;
using Common.Extensions.AutoInject.Attributes;
using Common.Server.Interfaces;
using Common.Server.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Realize.Messengers.PunchHole
{
    /// <summary>
    /// 重启
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton, typeof(IPunchHole))]
    public sealed class PunchHoleReset : IPunchHole
    {
        private readonly ISignInTransfer signinTransfer;

        public PunchHoleReset(ISignInTransfer signinTransfer)
        {
            this.signinTransfer = signinTransfer;
        }

        public PunchHoleTypes Type => PunchHoleTypes.RESET;

        public async Task Execute(IConnection connection, PunchHoleRequestInfo info)
        {
            _ = signinTransfer.SignIn(true);
            await Task.CompletedTask;
        }
    }
}