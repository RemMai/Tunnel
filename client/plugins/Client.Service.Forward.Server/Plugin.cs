using System;
using Common.Server;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Client.Service.ForWard.Server.Implementations;

namespace Client.Service.ForWard.Server
{
    public sealed class Plugin : IPlugin
    {
        public void Init(IServiceProvider services, Assembly[] assemblies)
        {
            services.GetService<ServerForwardTransfer>();
        }
    }
}
