﻿using System.Threading.Tasks;
using Client.Messengers.Clients;
using Client.Messengers.PunchHole;
using Common.Extensions.AutoInject.Attributes;
using Common.Server.Interfaces;
using Common.Server.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Realize.Messengers.PunchHole
{
    /// <summary>
    /// 掉线
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton,typeof(IPunchHole))]
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
