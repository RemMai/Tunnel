﻿using Common.Server;
using System.Threading.Tasks;
using Client.Messengers.PunchHole;
using Common.Extensions.AutoInject.Attributes;
using Common.Server.Interfaces;
using Common.Server.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Realize.Messengers.PunchHole
{
    /// <summary>
    /// 反向连接
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton, typeof(IPunchHole))]
    public sealed class PunchHoleReverse : IPunchHole
    {
        private readonly PunchHoleMessengerSender punchHoleMessengerSender;

        public PunchHoleReverse(PunchHoleMessengerSender punchHoleMessengerSender)
        {
            this.punchHoleMessengerSender = punchHoleMessengerSender;
        }

        public PunchHoleTypes Type => PunchHoleTypes.REVERSE;

        public async Task Execute(IConnection connection, PunchHoleRequestInfo info)
        {
            punchHoleMessengerSender.OnReverse?.Invoke(info);
            await Task.CompletedTask;
        }
    }
}