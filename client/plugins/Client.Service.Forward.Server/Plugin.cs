using System;
using Common.Server;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Client.Service.ForWard.Server
{
    public sealed class Plugin : IPlugin
    {
        public void Init(IServiceProvider services, Assembly[] assemblies)
        {
            services.GetService<ServerForwardTransfer>();
        }

        public void LoadBefore(IServiceCollection services, Assembly[] assemblies)
        {
            // services.AddSingleton<ServerForwardTransfer>();
            // services.AddSingleton<ServerForwardMessengerSender>();
        }
    }
}
